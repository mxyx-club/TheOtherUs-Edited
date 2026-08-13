namespace TheOtherRoles.Objects;

public class Trap : CustomObjectBase<Trap>
{
    private static readonly Dictionary<byte, int> activeFreezeCounts = new();
    private static readonly Dictionary<byte, bool> moveableBeforeTrapFreeze = new();

    public PlayerControl trapper;
    public List<PlayerControl> trapped = new();
    private readonly HashSet<byte> activeFrozenTargets = new();

    private static Sprite trapSprite = new ResourceSprite("Trapper_Trap_Ingame.png", 300);
    private Arrow arrow = new(Color.blue);
    private int neededCount = Trapper.trapCountToReveal;
    public bool revealed;
    public bool triggerable;
    private int usedCount;
    private string room;

    public GameObject ArrowObject => arrow?.arrow;

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
            if (GameObject == null || Renderer == null) return;
            triggerable = true;
            Renderer.color = Color.white;
        }, 5f);
    }

    public override void OnDestroy()
    {
        try
        {
            foreach (var playerId in activeFrozenTargets.ToArray())
                ReleaseTarget(playerId);
            trapped.Clear();
            arrow?.arrow?.Destroy();
        }
        catch { }
        base.OnDestroy();
    }

    public static void clearRevealedTraps()
    {
        var trapsToClear = AllObjects.FindAll(x => x.revealed);
        foreach (var t in trapsToClear)
        {
            t.Destroy();
        }
    }

    public static void ClearAllTraps(PlayerControl trapper, bool active)
    {
        var traps = AllObjects.Where(x => x.trapper == trapper && (active || !x.revealed)).ToArray();
        foreach (var t in traps)
        {
            t?.Destroy();
        }
    }

    public static void triggerTrap(byte targetId, int trapId)
    {
        var t = AllObjects.FirstOrDefault(x => x.Id == trapId);
        var target = PlayerById(targetId);
        if (Trapper.trapper == null || t == null || target == null || t.trapped.Contains(target)) return;

        t.usedCount++;
        t.triggerable = false;
        if (targetId == PlayerControl.LocalPlayer.PlayerId || targetId == Trapper.trapper.PlayerId)
        {
            SoundEffectsManager.play("trapperTrap");
        }

        t.FreezeTarget(target);
        target.NetTransform.Halt();
        if (PlayerControl.LocalPlayer.PlayerId == t.trapper.PlayerId) t.arrow.arrow.SetActive(true);

        _ = new LateTask(() =>
        {
            // The trap may have been destroyed by a meeting/role restore. Its
            // OnDestroy owns the release in that case; this stale callback must
            // never unlock the player (or another trap's active freeze).
            if (t.GameObject == null || !t.activeFrozenTargets.Contains(targetId)) return;
            t.ReleaseTarget(targetId);
            if (t.arrow?.arrow == null) return;
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

    public static void UpdateTrap()
    {
        foreach (var trap in AllObjects.ToArray())
        {
            trap.Update(PlayerControl.LocalPlayer);
        }
    }

    public void Update(PlayerControl player)
    {
        if (Imitator.IsImitating(Trapper.trapper, RoleId.Trapper) && trapper != Trapper.trapper)
        {
            GameObject.SetActive(false);
            return;
        }

        if (arrow.arrow.active) arrow.Update();

        var canSee = CanSeeGhostInfo || PlayerControl.LocalPlayer == trapper || trapped.Any(x => x.PlayerId == player.PlayerId);
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
                writer.Write(Id);
                writer.EndRPC();
                triggerTrap(player.PlayerId, Id);
            }
        }
    }

    private void FreezeTarget(PlayerControl target)
    {
        if (target == null || !activeFrozenTargets.Add(target.PlayerId))
            return;

        if (activeFreezeCounts.TryGetValue(target.PlayerId, out var count))
        {
            activeFreezeCounts[target.PlayerId] = count + 1;
        }
        else
        {
            activeFreezeCounts[target.PlayerId] = 1;
            moveableBeforeTrapFreeze[target.PlayerId] = target.moveable;
        }

        target.moveable = false;
    }

    private void ReleaseTarget(byte playerId)
    {
        if (!activeFrozenTargets.Remove(playerId) || !activeFreezeCounts.TryGetValue(playerId, out var count))
            return;

        if (count > 1)
        {
            activeFreezeCounts[playerId] = count - 1;
            return;
        }

        activeFreezeCounts.Remove(playerId);
        var restoreMoveable = moveableBeforeTrapFreeze.TryGetValue(playerId, out var wasMoveable) && wasMoveable;
        moveableBeforeTrapFreeze.Remove(playerId);
        var player = PlayerById(playerId);
        if (player != null)
            player.moveable = restoreMoveable;
    }
}
