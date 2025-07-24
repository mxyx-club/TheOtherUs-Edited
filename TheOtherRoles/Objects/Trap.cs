using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;

public class Trap
{
    public static List<Trap> traps = new();
    public static Dictionary<byte, Trap> trapPlayerIdMap = new();

    private static int instanceCounter;

    private static Sprite trapSprite = new ResourceSprite("Trapper_Trap_Ingame.png", 300);
    private Arrow arrow = new(Color.blue);
    private int neededCount = Trapper.trapCountToReveal;
    public int instanceId;
    public bool revealed;
    public GameObject trap;
    public List<PlayerControl> trappedPlayer = new();
    public bool triggerable;
    private int usedCount;

    public Trap(Vector2 p)
    {
        trap = new GameObject("Trap") { layer = 11 };
        trap.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        var position = new Vector3(p.x, p.y, (p.y / 1000) + 0.001f); // just behind player
        trap.transform.position = position;
        neededCount = Trapper.trapCountToReveal;

        var trapRenderer = trap.AddComponent<SpriteRenderer>();
        trapRenderer.sprite = trapSprite;
        trap.SetActive(false);
        if (PlayerControl.LocalPlayer.PlayerId == Trapper.trapper.PlayerId) trap.SetActive(true);
        trapRenderer.color = Color.white * new Vector4(1, 1, 1, 0.5f);
        instanceId = ++instanceCounter;
        traps.Add(this);
        arrow.Update(position);
        arrow.arrow.SetActive(false);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5, new Action<float>(x =>
        {
            if (x == 1f)
            {
                triggerable = true;
                trapRenderer.color = Color.white;
            }
        })));
    }

    [OnGameStart, OnGameEnd]
    public static void clearTraps()
    {
        foreach (var t in traps)
        {
            UObject.Destroy(t.arrow.arrow);
            UObject.Destroy(t.trap);
        }

        traps = new();
        trapPlayerIdMap = new Dictionary<byte, Trap>();
        instanceCounter = 0;
    }

    public static void clearRevealedTraps()
    {
        var trapsToClear = traps.FindAll(x => x.revealed);

        foreach (var t in trapsToClear)
        {
            traps.Remove(t);
            UObject.Destroy(t.trap);
        }
    }

    public static void triggerTrap(byte targetId, byte trapId)
    {
        var t = traps.FirstOrDefault(x => x.instanceId == trapId);
        var target = PlayerById(targetId);
        if (Trapper.trapper == null || t == null || t.trappedPlayer.Contains(target) || target == null) return;
        var localIsTrapper = PlayerControl.LocalPlayer.PlayerId == Trapper.trapper.PlayerId;
        trapPlayerIdMap.TryAdd(targetId, t);
        t.usedCount++;
        t.triggerable = false;
        if (targetId == PlayerControl.LocalPlayer.PlayerId || targetId == Trapper.trapper.PlayerId)
        {
            t.trap.SetActive(true);
            SoundEffectsManager.play("trapperTrap");
        }

        target.moveable = false;
        target.NetTransform.Halt();
        Trapper.playersOnMap.Add(target);
        if (localIsTrapper) t.arrow.arrow.SetActive(true);

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Trapper.trapDuration, new Action<float>(p =>
        {
            if (p == 1f)
            {
                target.moveable = true;
                Trapper.playersOnMap.RemoveAll(x => x == target);
                if (trapPlayerIdMap.ContainsKey(targetId)) trapPlayerIdMap.Remove(targetId);
                t.arrow.arrow.SetActive(false);
            }
        })));

        if (t.usedCount == t.neededCount) t.revealed = true;

        t.trappedPlayer.Add(target);
        t.triggerable = true;

        // Add trapped Info into Trapper chat
        if (Trapper.trapper.IsAlive() && (PlayerControl.LocalPlayer == Trapper.trapper || CanSeeGhostInfo))
        {
            foreach (var trap in traps)
            {
                if (!trap.revealed) continue;
                var message = $"陷阱 {trap.instanceId}日志: \n";
                trap.trappedPlayer = trap.trappedPlayer.OrderBy(x => rnd.Next()).ToList();
                message = trap.trappedPlayer.Aggregate(message, (current, p) => current + Trapper.infoType switch
                {
                    0 => RoleInfo.GetRolesString(p, false, false, false).FirstOrDefault() + "\n",
                    1 when (isEvilNeutral(p) || isKillerNeutral(p) || p.IsImpostor()) ^ Vortox.Reversal => "邪恶职业 \n",
                    1 => "善良职业 \n",
                    _ => p.Data.PlayerName + "\n"
                });

                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Trapper.trapper, $"{message}");
            }
        }
        Trapper.playersOnMap = new List<PlayerControl>();
    }

    public static void Update()
    {
        if (Trapper.trapper == null) return;
        var player = PlayerControl.LocalPlayer;
        var vent = MapUtilities.CachedShipStatus.AllVents[0];
        var closestDistance = float.MaxValue;

        if (vent == null || player == null) return;
        var ud = vent.UsableDistance / 2;
        Trap target = null;
        foreach (var trap in traps)
        {
            if (trap.arrow.arrow.active) trap.arrow.Update();
            if (trap.revealed || !trap.triggerable || trap.trappedPlayer.Contains(player)) continue;
            if (player.inVent || !player.CanMove) continue;
            var distance = Vector2.Distance(trap.trap.transform.position, player.GetTruePosition());
            if (distance <= ud && distance < closestDistance)
            {
                closestDistance = distance;
                target = trap;
            }
        }
        if (target?.revealed == false && player.PlayerId != Trapper.trapper.PlayerId && player.IsAlive())
        {
            var writer = StartRPC(CustomRPC.TriggerTrap);
            writer.Write(player.PlayerId);
            writer.Write(target.instanceId);
            writer.EndRPC();
            RPCProcedure.triggerTrap(player.PlayerId, (byte)target.instanceId);
        }

        if (!CanSeeGhostInfo || player.PlayerId == Trapper.trapper.PlayerId) return;
        foreach (var trap in traps.Where(trap => !trap.trap.active))
            trap.trap.SetActive(true);
    }
}