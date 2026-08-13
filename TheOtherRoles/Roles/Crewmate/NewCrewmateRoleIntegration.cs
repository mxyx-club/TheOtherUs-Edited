using TheOtherRoles.Attributes;
using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

/// <summary>
/// Network and lifecycle glue shared by TrapperPlus and Aurial. Keeping the
/// host-confirmed paths here avoids mixing them with the legacy Trapper RPCs.
/// </summary>
public static class NewCrewmateRoleIntegration
{
    private static readonly Dictionary<byte, float> lastRadiationAt = new();
    private static readonly Dictionary<byte, float> lastTrapPlacementAt = new();
    private static float radiationAvailableAt;
    private static float trapAvailableAt;
    private static int actionRoundCooldownGeneration;

    [PluginModuleInitializer]
    public static void Initialize()
    {
        TrapperPlus.HostTriggerConfirmed -= BroadcastTrapTrigger;
        TrapperPlus.HostTriggerConfirmed += BroadcastTrapTrigger;
    }

    [OnMeetingStart]
    public static void OnMeetingStart()
    {
        TrapperPlus.OnMeetingStart();
        lastRadiationAt.Clear();
        lastTrapPlacementAt.Clear();
        ScheduleActionRoundCooldowns();
    }

    private static void BroadcastTrapTrigger(byte ownerId, int trapId, byte targetId, RoleId roleSnapshot)
    {
        if (!AmongUsClient.Instance.AmHost) return;

        var writer = StartRPC(CustomRPC.TriggerInfoTrap);
        writer.Write(ownerId);
        writer.Write(trapId);
        writer.Write(targetId);
        writer.Write((byte)roleSnapshot);
        writer.EndRPC();
        // The host has already applied this result before raising the event.
    }

    public static void PlaceLocalInfoTrap()
    {
        var owner = PlayerControl.LocalPlayer;
        if (owner == null || owner != TrapperPlus.Player || !owner.CanMove || !InGame || InMeeting ||
            ExileController.Instance || !TrapperPlus.CanPlaceTrap(owner))
            return;

        var position = owner.transform.position;
        if (AmongUsClient.Instance.AmHost)
        {
            BroadcastInfoTrapPlacement(owner, position);
            return;
        }

        var writer = StartRPC(CustomRPC.PlaceInfoTrap, HostPlayer);
        writer.Write((byte)0); // request
        writer.Write(owner.PlayerId);
        writer.Write(position);
        writer.EndRPC();
    }

    public static void HandleInfoTrapPlacementRpc(PlayerControl sender, MessageReader reader)
    {
        var phase = reader.ReadByte();
        var ownerId = reader.ReadByte();

        if (phase == 0)
        {
            var requestedPosition = reader.ReadVector3();
            if (!AmongUsClient.Instance.AmHost || sender?.PlayerId != ownerId)
                return;

            var owner = PlayerById(ownerId);
            if (owner == null || owner != TrapperPlus.Player || !owner.IsAlive() || !owner.CanMove ||
                !InGame || InMeeting || ExileController.Instance || !TrapperPlus.CanPlaceTrap(owner))
                return;

            var now = Time.realtimeSinceStartup;
            if (now < trapAvailableAt ||
                (lastTrapPlacementAt.TryGetValue(ownerId, out var lastPlacement) && now - lastPlacement < TrapperPlus.cooldown))
                return;

            if (Vector2.Distance(owner.GetTruePosition(), requestedPosition) > 0.75f)
                return;

            BroadcastInfoTrapPlacement(owner, owner.transform.position, now);
            return;
        }

        if (phase != 1 || sender != HostPlayer)
            return;

        var trapId = reader.ReadInt32();
        var position = reader.ReadVector3();
        var canonicalOwner = PlayerById(ownerId);
        if (canonicalOwner != null)
        {
            var placed = TrapperPlus.PlaceTrap(canonicalOwner, position, trapId);
            if (placed != null)
            {
                lastTrapPlacementAt[ownerId] = Time.realtimeSinceStartup;
                ConfirmLocalTrapPlacement(ownerId);
            }
        }
    }

    public static void HandleInfoTrapTriggerRpc(PlayerControl sender, MessageReader reader)
    {
        var ownerId = reader.ReadByte();
        var trapId = reader.ReadInt32();
        var targetId = reader.ReadByte();
        var roleSnapshot = (RoleId)reader.ReadByte();
        if (sender == HostPlayer)
            TrapperPlus.RegisterTrigger(ownerId, trapId, targetId, roleSnapshot);
    }

    public static void RequestRadiation()
    {
        var caster = PlayerControl.LocalPlayer;
        if (caster == null || caster != Aurial.aurial || !caster.IsAlive()) return;

        if (AmongUsClient.Instance.AmHost)
        {
            BroadcastRadiation(caster.PlayerId);
            return;
        }

        var writer = StartRPC(CustomRPC.AurialRadiate, HostPlayer);
        writer.Write((byte)0); // request
        writer.Write(caster.PlayerId);
        writer.EndRPC();
    }

    public static void HandleRadiationRpc(PlayerControl sender, MessageReader reader)
    {
        var phase = reader.ReadByte();
        var casterId = reader.ReadByte();

        if (phase == 0)
        {
            if (AmongUsClient.Instance.AmHost && sender?.PlayerId == casterId)
                BroadcastRadiation(casterId);
            return;
        }

        if (phase != 1 || sender != HostPlayer)
            return;

        var count = reader.ReadByte();
        var targets = new List<byte>(count);
        for (var i = 0; i < count; i++) targets.Add(reader.ReadByte());

        var newlyRevealed = Aurial.ReceiveRadiation(casterId, targets);
        if (Aurial.aurial?.AmOwner == true)
        {
            Aurial.ShowLocalPulse();
            Aurial.ShowLocalRadiationResult(targets, newlyRevealed);
        }

        ConfirmLocalRadiation(casterId);

        // Every client observes accepted host results. Keeping this timestamp on
        // non-host clients preserves the remaining cooldown if host migration
        // occurs during the action round.
        lastRadiationAt[casterId] = Time.realtimeSinceStartup;
    }

    private static void BroadcastRadiation(byte casterId)
    {
        var caster = PlayerById(casterId);
        if (!AmongUsClient.Instance.AmHost || caster == null || Aurial.aurial != caster || !caster.IsAlive() ||
            !caster.CanMove || !InGame || InMeeting || ExileController.Instance)
            return;

        var now = Time.realtimeSinceStartup;
        if (now < radiationAvailableAt ||
            (lastRadiationAt.TryGetValue(casterId, out var lastCast) && now - lastCast < Aurial.cooldown))
            return;
        lastRadiationAt[casterId] = now;

        var targets = Aurial.GetRadiationTargets(casterId);
        var writer = StartRPC(CustomRPC.AurialRadiate);
        writer.Write((byte)1); // canonical result
        writer.Write(casterId);
        writer.Write((byte)targets.Count);
        foreach (var targetId in targets) writer.Write(targetId);
        writer.EndRPC();

        var newlyRevealed = Aurial.ReceiveRadiation(casterId, targets);
        if (Aurial.aurial.AmOwner)
        {
            Aurial.ShowLocalPulse();
            Aurial.ShowLocalRadiationResult(targets, newlyRevealed);
        }
        ConfirmLocalRadiation(casterId);
    }

    private static void BroadcastInfoTrapPlacement(PlayerControl owner, Vector3 position, float acceptedAt = -1f)
    {
        if (!AmongUsClient.Instance.AmHost || owner == null || owner != TrapperPlus.Player ||
            !owner.IsAlive() || !owner.CanMove || !InGame || InMeeting || ExileController.Instance ||
            !TrapperPlus.CanPlaceTrap(owner))
            return;

        var now = acceptedAt >= 0f ? acceptedAt : Time.realtimeSinceStartup;
        if (now < trapAvailableAt ||
            (lastTrapPlacementAt.TryGetValue(owner.PlayerId, out var lastPlacement) && now - lastPlacement < TrapperPlus.cooldown))
            return;
        lastTrapPlacementAt[owner.PlayerId] = now;

        var trapId = TrapperPlus.AllocateTrapId();
        var writer = StartRPC(CustomRPC.PlaceInfoTrap);
        writer.Write((byte)1); // canonical placement
        writer.Write(owner.PlayerId);
        writer.Write(trapId);
        writer.Write(position);
        writer.EndRPC();
        if (TrapperPlus.PlaceTrap(owner, position, trapId) != null)
            ConfirmLocalTrapPlacement(owner.PlayerId);
    }

    /// <summary>
    /// Applies the configured first-round cooldown after CustomButton.Initialize
    /// has reset every visible button to the global lobby cooldown.
    /// </summary>
    public static void ResetLocalInitialCooldowns()
    {
        var local = PlayerControl.LocalPlayer;
        if (local == null)
            return;

        if (local == TrapperPlus.Player && HudManagerStartPatch.trapperPlusButton != null)
        {
            HudManagerStartPatch.trapperPlusButton.MaxTimer = TrapperPlus.cooldown;
            HudManagerStartPatch.trapperPlusButton.Timer = TrapperPlus.cooldown;
            HudManagerStartPatch.trapperPlusButton.EffectTimer = 0f;
            HudManagerStartPatch.trapperPlusButton.IsEffectActive = false;
        }

        if (local == Aurial.aurial && HudManagerStartPatch.aurialRadiateButton != null)
        {
            HudManagerStartPatch.aurialRadiateButton.MaxTimer = Aurial.cooldown;
            HudManagerStartPatch.aurialRadiateButton.Timer = Aurial.cooldown;
            HudManagerStartPatch.aurialRadiateButton.EffectTimer = 0f;
            HudManagerStartPatch.aurialRadiateButton.IsEffectActive = false;
        }
    }

    private static void ConfirmLocalTrapPlacement(byte ownerId)
    {
        if (PlayerControl.LocalPlayer?.PlayerId != ownerId)
            return;

        var button = HudManagerStartPatch.trapperPlusButton;
        if (button != null)
        {
            button.MaxTimer = TrapperPlus.cooldown;
            button.Timer = button.MaxTimer;
        }
        SoundEffectsManager.play("trapperTrap");
    }

    private static void ConfirmLocalRadiation(byte casterId)
    {
        if (PlayerControl.LocalPlayer?.PlayerId != casterId)
            return;

        var button = HudManagerStartPatch.aurialRadiateButton;
        if (button != null)
        {
            button.MaxTimer = Aurial.cooldown;
            button.Timer = button.MaxTimer;
        }
    }

    // RoleHelpers reloads the configured cooldowns from resetVariables' normal
    // OnGameStart initializer. Run afterwards so the host gate never retains a
    // previous lobby's values when the host changes these options.
    [OnGameStart(50)]
    public static void ResetHostRateLimits()
    {
        lastRadiationAt.Clear();
        lastTrapPlacementAt.Clear();
        var now = Time.realtimeSinceStartup;
        radiationAvailableAt = now + Aurial.cooldown;
        trapAvailableAt = now + TrapperPlus.cooldown;
        actionRoundCooldownGeneration++;
    }

    public static void BeginActionRoundCooldowns()
    {
        // Cancel any MeetingHud-based fallback waiter. The normal path invokes
        // this method from the actual exile wrap-up, when movement resumes.
        actionRoundCooldownGeneration++;
        if (!InGame)
            return;

        lastRadiationAt.Clear();
        lastTrapPlacementAt.Clear();
        var now = Time.realtimeSinceStartup;
        radiationAvailableAt = now + Aurial.cooldown;
        trapAvailableAt = now + TrapperPlus.cooldown;
    }

    private static void ScheduleActionRoundCooldowns()
    {
        var generation = ++actionRoundCooldownGeneration;
        Coroutines.Start(WaitForActionRound(generation));
    }

    private static IEnumerator WaitForActionRound(int generation)
    {
        while (InMeeting || ExileController.Instance)
            yield return null;

        if (generation == actionRoundCooldownGeneration && InGame)
            BeginActionRoundCooldowns();
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    private static class BaseExileCooldownPatch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        private static void Postfix() => BeginActionRoundCooldowns();
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    private static class AirshipExileCooldownPatch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        private static void Postfix() => BeginActionRoundCooldowns();
    }

    // Submerged owns its exile cutscene lifecycle outside the vanilla
    // ExileController wrap-up methods. Match the compatibility signal already
    // used by Imitator so both roles begin their round at the same point.
    [HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(GameObject))]
    private static class SubmergedExileCooldownPatch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.Last)]
        private static void Prefix(GameObject obj)
        {
            if (SubmergedCompatibility.IsSubmerged && obj?.name?.Contains("ExileCutscene") == true)
                BeginActionRoundCooldowns();
        }
    }
}
