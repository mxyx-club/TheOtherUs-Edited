using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;

public class Trap
{
    public static List<Trap> AllTraps = new();

    public GameObject trap;

    public int trapId;
    private static int maxId;

    public PlayerControl trapper;
    public List<PlayerControl> trapped = new();

    private static Sprite trapSprite = new ResourceSprite("Trapper_Trap_Ingame.png", 300);
    private Arrow arrow = new(Color.blue);
    private int neededCount = Trapper.trapCountToReveal;
    public bool revealed;
    public bool triggerable;
    private int usedCount;
    private string room;

    public Trap(PlayerControl player, Vector2 p)
    {
        trap = new GameObject("Trap") { layer = 11 };
        trap.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        var position = new Vector3(p.x, p.y, (p.y / 1000) + 0.001f); // just behind player
        trap.transform.position = position;
        neededCount = Trapper.trapCountToReveal;

        var trapRenderer = trap.AddComponent<SpriteRenderer>();
        trapRenderer.sprite = trapSprite;
        trapRenderer.color = Color.white * new Vector4(1, 1, 1, 0.5f);
        trapId = ++maxId;
        trapper = player;
        arrow.Update(position);
        arrow.arrow.SetActive(false);
        trap.SetActive(false);
        if (PlayerControl.LocalPlayer == player) trap.SetActive(true);

        var lastRoom = FastDestroyableSingleton<HudManager>.Instance?.roomTracker?.LastRoom?.RoomId ?? null;
        room = lastRoom != null ? FastDestroyableSingleton<TranslationController>.Instance.GetString((SystemTypes)lastRoom) : "室外";

        _ = new LateTask(() =>
        {
            triggerable = true;
            trapRenderer.color = Color.white;
        }, 5f);

        AllTraps.Add(this);
    }

    public void Destroy()
    {
        try
        {

            if (arrow != null) UObject.Destroy(arrow.arrow);
            if (trap != null) UObject.Destroy(trap);
        }
        catch { }
        AllTraps.Remove(this);
    }

    [OnGameStart, OnGameEnd]
    public static void ClearAndReload()
    {
        foreach (var t in AllTraps.ToArray())
        {
            t.Destroy();
        }

        AllTraps = new();
        maxId = 0;
    }

    public static void clearRevealedTraps()
    {
        var trapsToClear = AllTraps.FindAll(x => x.revealed);
        foreach (var t in trapsToClear)
        {
            t.Destroy();
        }
    }

    public static void ClearAllTraps(PlayerControl trapper, bool active)
    {
        var traps = AllTraps.Where(x => x.trapper == trapper && (active || !x.revealed));
        foreach (var t in traps)
        {
            t?.Destroy();
        }
    }

    public static void triggerTrap(byte targetId, int trapId)
    {
        var t = AllTraps.FirstOrDefault(x => x.trapId == trapId);
        var target = PlayerById(targetId);
        if (Trapper.trapper == null || t == null || t.trapped.Contains(target) || target == null) return;

        t.usedCount++;
        t.triggerable = false;
        if (targetId == PlayerControl.LocalPlayer.PlayerId || targetId == Trapper.trapper.PlayerId)
        {
            SoundEffectsManager.play("trapperTrap");
        }

        target.moveable = false;
        target.NetTransform.Halt();
        if (PlayerControl.LocalPlayer.PlayerId == t.trapper.PlayerId) t.arrow.arrow.SetActive(true);

        _ = new LateTask(() =>
        {
            target.moveable = true;
            t.arrow.arrow.SetActive(false);
            t.triggerable = true;
        }, Trapper.trapDuration);

        t.trapped.Add(target);

        if (t.usedCount == t.neededCount) t.revealed = true;

        if (t.revealed && (PlayerControl.LocalPlayer == t.trapper || CanSeeGhostInfo))
        {
            var message = $"陷阱 {t.room} {t.trapId} 日志: \n";
            t.trapped = t.trapped.OrderBy(x => rnd.Next()).ToList();
            message = t.trapped.Aggregate(message, (current, p) => current + Trapper.infoType switch
            {
                0 => RoleInfo.GetRolesString(p, false, false, false) + "\n",
                1 when (isEvilNeutral(p) || isKillerNeutral(p) || p.IsImpostor()) ^ Vortox.Reversal => "邪恶职业 \n",
                1 => "善良职业 \n",
                _ => p.Data.PlayerName + "\n"
            });

            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Trapper.trapper, $"{message}");
        }
    }

    public static void UpdateTrap()
    {
        foreach (var trap in AllTraps)
        {
            trap.Update(PlayerControl.LocalPlayer);
        }
    }

    public void Update(PlayerControl player)
    {
        if (arrow.arrow.active) arrow.Update();

        var canSee = CanSeeGhostInfo || PlayerControl.LocalPlayer == trapper || trapped.Any(x => x.PlayerId == player.PlayerId);
        trap.SetActive(canSee);

        if (revealed || !triggerable || trapped.Any(x => x.PlayerId == player.PlayerId)) return;

        if (player.inVent || !player.CanMove) return;
        var distance = Vector2.Distance(trap.transform.position, player.GetTruePosition());
        if (distance < 0.8f)
        {
            if (!revealed && player.PlayerId != trapper.PlayerId && player.IsAlive())
            {
                var writer = StartRPC(CustomRPC.TriggerTrap);
                writer.Write(player.PlayerId);
                writer.Write(trapId);
                writer.EndRPC();
                triggerTrap(player.PlayerId, trapId);
            }
        }
    }
}