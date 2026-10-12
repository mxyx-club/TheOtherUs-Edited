namespace TheOtherRoles.Objects;

public class Trap : NetworkCustomObject
{
    static Trap()
    {
        Register<Trap>((pos, payload) => new Trap(PlayerById(payload[0]), pos));
    }

    public PlayerControl trapper;
    public List<PlayerControl> trapped = new();

    private static Sprite trapSprite = new ResourceSprite("Trapper_Trap_Ingame.png", 300);
    private Arrow arrow = new(Color.blue);
    private int neededCount = Trapper.trapCountToReveal;
    public bool revealed;
    public bool triggerable;
    private int usedCount;
    private string room;

    public Trap(PlayerControl player, Vector3 pos)
    {
        GameObject.layer = 11;
        GameObject.name = "Trap " + Id;
        var position = new Vector3(pos.x, pos.y, pos.y + 0.001f);
        GameObject.transform.position = position;
        neededCount = Trapper.trapCountToReveal;

        Renderer.sprite = trapSprite;
        Renderer.color = Color.white * new Vector4(1, 1, 1, 0.5f);
        trapper = player;
        arrow.Update(position);
        arrow.arrow.SetActive(false);
        GameObject.SetActive(false);
        if (PlayerControl.LocalPlayer == player) GameObject.SetActive(true);

        var lastRoom = FastDestroyableSingleton<HudManager>.Instance?.roomTracker?.LastRoom?.RoomId ?? null;
        room = lastRoom != null ? FastDestroyableSingleton<TranslationController>.Instance.GetString((SystemTypes)lastRoom) : "室外";

        _ = new LateTask(() =>
        {
            triggerable = true;
            Renderer.color = Color.white;
        }, 5f);
    }

    public override void OnDestroy()
    {
        try
        {
            arrow?.arrow?.Destroy();
        }
        catch { }
        base.OnDestroy();
    }

    public static void clearRevealedTraps()
    {
        var trapsToClear = AllObjects.OfType<Trap>().Where(x => x.revealed).ToArray();
        foreach (var t in trapsToClear)
        {
            t.Destroy();
        }
    }

    public static void ClearAllTraps(PlayerControl trapper, bool active)
    {
        var traps = AllObjects.OfType<Trap>().Where(x => x.trapper == trapper && (active || !x.revealed));
        foreach (var t in traps)
        {
            t?.Destroy();
        }
    }

    public static void triggerTrap(byte targetId, ushort trapId)
    {
        var t = Find<Trap>(trapId);
        var target = PlayerById(targetId);
        if (Trapper.trapper == null || t == null || t.trapped.Contains(target) || target == null) return;

        t.usedCount++;
        t.triggerable = false;
        if (targetId == PlayerControl.LocalPlayer.PlayerId || targetId == Trapper.trapper?.PlayerId)
        {
            SoundEffectsManager.play("trapperTrap");
        }

        target.moveable = false;
        target.NetTransform.Halt();
        if (PlayerControl.LocalPlayer.PlayerId == t.trapper?.PlayerId) t.arrow.arrow.SetActive(true);

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
            var message = $"陷阱 {t.room} 日志: \n";
            t.trapped = t.trapped.OrderBy(x => rnd.Next()).ToList();
            message = t.trapped.Aggregate(message, (current, p) => current + Trapper.infoType switch
            {
                0 => RoleInfo.GetRolesString(p, false, false, false) + "\n",
                1 when (p.IsEvilNeutral() || p.IsKillerNeutral() || p.IsImpostor()) ^ Vortox.Reversal => "邪恶职业 \n",
                1 => "善良职业 \n",
                _ => p.Data.PlayerName + "\n"
            });

            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Trapper.trapper, $"{message}");
        }
    }

    public override void Update()
    {
        var player = PlayerControl.LocalPlayer;
        if (arrow.arrow.active) arrow.Update();

        var canSee = CanSeeGhostInfo || player == trapper || trapped.Any(x => x.PlayerId == player.PlayerId);
        GameObject.SetActive(canSee);

        if (revealed || !triggerable || trapped.Any(x => x.PlayerId == player.PlayerId)) return;

        if (player.inVent || !player.CanMove) return;
        var distance = Vector2.Distance(GameObject.transform.position, player.GetTruePosition());
        if (distance < 0.6f)
        {
            if (!revealed && player.PlayerId != trapper.PlayerId && player.IsAlive())
            {
                var writer = StartRPC(CustomRPC.TriggerTrap);
                writer.Write(player.PlayerId);
                writer.Write(NetworkId);
                writer.EndRPC();
                triggerTrap(player.PlayerId, NetworkId);
            }
        }
    }
}