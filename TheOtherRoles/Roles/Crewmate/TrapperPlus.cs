using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

/// <summary>
/// The result of the information collected by a TrapperPlus during one action round.
/// Player identities are deliberately not retained in the public report.
/// </summary>
public sealed class TrapperPlusReport
{
    public enum ReportState
    {
        NoPlayers,
        NotEnoughPlayers,
        Ready,
    }

    public ReportState State { get; }
    public IReadOnlyList<RoleId> Roles { get; }

    internal TrapperPlusReport(ReportState state, IEnumerable<RoleId> roles)
    {
        State = state;
        Roles = roles.ToArray();
    }

    public string ToLocalizedText()
    {
        if (State == ReportState.NoPlayers)
            return GetString("TrapperPlus.Report.NoPlayers");

        if (State == ReportState.NotEnoughPlayers)
            return GetString("TrapperPlus.Report.NotEnoughPlayers");

        var roleNames = Roles.Select(roleId =>
        {
            return RoleInfo.RoleInfoById.TryGetValue(roleId, out var roleInfo)
                ? roleInfo.Name
                : roleId.ToString();
        });

        return string.Format(GetString("TrapperPlus.Report.Roles"), string.Join(", ", roleNames));
    }
}

/// <summary>
/// State and host-side trigger handling for the Town Of Us R style Trapper.
/// The existing local legacy Trapper remains completely separate.
/// </summary>
public static class TrapperPlus
{
    private sealed class OwnerState
    {
        public int RemainingTraps;
        public readonly Dictionary<byte, RoleId> TriggeredPlayers = new();
        public TrapperPlusReport LastReport = EmptyReport();

        public OwnerState(int uses)
        {
            RemainingTraps = uses;
        }
    }

    public static PlayerControl Player;
    public static PlayerControl trapper
    {
        get => Player;
        set => Player = value;
    }

    public static readonly Color color = new Color32(167, 209, 179, byte.MaxValue);

    // These defaults are also useful before the host option holder has been loaded.
    // CustomOptionHolder should call Configure at the beginning of each game.
    public static float cooldown = 10f;
    public static float minDwellTime = 1f;
    public static bool removeTrapsEachRound = true;
    public static int maxTraps = 5;
    public static float trapSize = 0.25f;
    public static int minPlayersToReport = 3;

    public static Sprite trapButtonSprite = new ResourceSprite("TrapperPlus_Place_Button.png");

    private static readonly Dictionary<byte, OwnerState> ownerStates = new();
    private static int nextTrapId;

    /// <summary>
    /// Called only by the host after a continuous dwell has completed. The shared RPC
    /// integration should subscribe once and broadcast owner, trap, target and role.
    /// </summary>
    public static event Action<byte, int, byte, RoleId> HostTriggerConfirmed;

    public static int charges
    {
        get => GetRemainingTraps(Player);
        set
        {
            if (Player != null)
                GetOrCreateState(Player.PlayerId).RemainingTraps = Math.Max(0, value);
        }
    }

    public static void Configure(
        float trapCooldown,
        float minimumDwellTime,
        bool removeEachRound,
        int maximumTraps,
        float radiusMultiplier,
        int minimumReportPlayers)
    {
        cooldown = Math.Max(0f, trapCooldown);
        minDwellTime = Math.Max(0f, minimumDwellTime);
        removeTrapsEachRound = removeEachRound;
        maxTraps = Math.Max(1, maximumTraps);
        trapSize = Math.Max(0.01f, radiusMultiplier);
        minPlayersToReport = Math.Max(1, minimumReportPlayers);
    }

    public static void clearAndReload()
    {
        foreach (var trap in InfoTrap.AllObjects.ToArray())
            trap?.Destroy();

        Player = null;
        ownerStates.Clear();
        nextTrapId = 0;
    }

    public static void ClearAndReload() => clearAndReload();

    public static int AllocateTrapId() => nextTrapId++;

    public static bool CanPlaceTrap(PlayerControl owner = null)
    {
        owner ??= Player;
        return owner != null && owner.IsAlive() && GetRemainingTraps(owner) > 0;
    }

    /// <summary>
    /// Creates the synchronized trap instance and consumes one use. The sender should
    /// include a value from AllocateTrapId in the placement RPC so every client uses
    /// the same InfoTrap.NetworkId.
    /// </summary>
    public static InfoTrap PlaceTrap(PlayerControl owner, Vector3 position, int trapId = -1)
    {
        if (owner == null || Player == null || owner.PlayerId != Player.PlayerId)
            return null;

        if (trapId < 0)
            trapId = AllocateTrapId();

        var existing = InfoTrap.Find(trapId);
        if (existing != null)
            return existing.OwnerId == owner.PlayerId ? existing : null;

        nextTrapId = Math.Max(nextTrapId, trapId + 1);

        var state = GetOrCreateState(owner.PlayerId);
        if (state.RemainingTraps <= 0)
            return null;

        state.RemainingTraps--;
        return new InfoTrap(owner, position, trapId);
    }

    /// <summary>
    /// Applies a confirmed trigger on all clients. Returns false for duplicate players
    /// in the same action round, including duplicates produced by different traps.
    /// </summary>
    public static bool RegisterTrigger(byte ownerId, int trapId, byte targetId, RoleId roleSnapshot)
    {
        var trap = InfoTrap.Find(trapId);
        if (trap == null || trap.OwnerId != ownerId)
            return false;

        trap.MarkTargetCompleted(targetId);

        var state = GetOrCreateState(ownerId);
        if (state.TriggeredPlayers.ContainsKey(targetId))
            return false;

        state.TriggeredPlayers.Add(targetId, roleSnapshot);
        return true;
    }

    internal static void ConfirmHostDwell(InfoTrap trap, PlayerControl target)
    {
        if (trap == null || target == null || !AmongUsClient.Instance.AmHost)
            return;

        var roleSnapshot = GetPrimaryRole(target);
        if (!RegisterTrigger(trap.OwnerId, trap.NetworkId, target.PlayerId, roleSnapshot))
            return;

        HostTriggerConfirmed?.Invoke(trap.OwnerId, trap.NetworkId, target.PlayerId, roleSnapshot);
    }

    public static RoleId GetPrimaryRole(PlayerControl target)
    {
        if (target == null)
            return RoleId.DefaultRole;

        var mainRole = PlayerData.GetPlayerData(target)?.MainRole ?? RoleId.DefaultRole;
        if (mainRole != RoleId.DefaultRole)
            return mainRole;

        return target.Data?.Role?.IsImpostor == true ? RoleId.Impostor : RoleId.Crewmate;
    }

    /// <summary>
    /// Freezes and clears the evidence for every owner, then applies the configured
    /// per-round trap/uses reset. Call once from MeetingHud.Start.
    /// </summary>
    public static TrapperPlusReport OnMeetingStart(bool showLocalReport = true)
    {
        TrapperPlusReport localReport = EmptyReport();
        var localId = PlayerControl.LocalPlayer?.PlayerId;

        // A holder can be erased or disconnected while another player is
        // borrowing this static role. Those invalidated snapshots deliberately
        // restore no holder; discard their private state and objects instead of
        // leaking an old report into a later assignment of TrapperPlus.
        var activeOwnerId = Player?.Data?.Disconnected == false ? Player.PlayerId : (byte?)null;
        foreach (var staleOwnerId in ownerStates.Keys.Where(id => id != activeOwnerId).ToArray())
        {
            foreach (var staleTrap in InfoTrap.AllObjects.Where(trap => trap.OwnerId == staleOwnerId).ToArray())
                staleTrap?.Destroy();
            ownerStates.Remove(staleOwnerId);
        }

        // Ensure an assigned TrapperPlus still receives the "no players" report even
        // when they never opened the button HUD or placed a trap this round.
        if (Player != null)
            _ = GetOrCreateState(Player.PlayerId);

        foreach (var (ownerId, state) in ownerStates.ToArray())
        {
            var report = BuildReport(state);
            state.LastReport = report;
            state.TriggeredPlayers.Clear();

            if (localId == ownerId)
                localReport = report;

            if (showLocalReport && (localId == ownerId || CanSeeGhostInfo))
            {
                var owner = PlayerById(ownerId);
                if (owner != null && HudManager.Instance?.Chat != null)
                    HudManager.Instance.Chat.AddChat(owner, report.ToLocalizedText());
            }

            if (removeTrapsEachRound)
                state.RemainingTraps = maxTraps;
        }

        foreach (var trap in InfoTrap.AllObjects.ToArray())
        {
            if (removeTrapsEachRound)
                trap?.Destroy();
            else
                trap?.ResetForNextRound();
        }

        return localReport;
    }

    public static TrapperPlusReport GetLastReport(PlayerControl owner = null)
    {
        owner ??= Player;
        return owner != null && ownerStates.TryGetValue(owner.PlayerId, out var state)
            ? state.LastReport
            : EmptyReport();
    }

    public static IReadOnlyDictionary<byte, RoleId> GetRoundEvidence(PlayerControl owner = null)
    {
        owner ??= Player;
        return owner != null
            ? new Dictionary<byte, RoleId>(GetOrCreateState(owner.PlayerId).TriggeredPlayers)
            : new Dictionary<byte, RoleId>();
    }

    public static int GetRemainingTraps(PlayerControl owner = null)
    {
        owner ??= Player;
        return owner == null ? 0 : GetOrCreateState(owner.PlayerId).RemainingTraps;
    }

    /// <summary>
    /// Starts an owner with fresh resources, primarily for an Imitator session.
    /// </summary>
    public static void ResetOwnerState(PlayerControl owner, bool destroyOwnedTraps = true)
    {
        if (owner == null)
            return;

        if (destroyOwnedTraps)
            InfoTrap.ClearAll(owner);

        ownerStates[owner.PlayerId] = new OwnerState(maxTraps);
    }

    /// <summary>
    /// Removes only one owner's reversible TrapperPlus session state.
    /// </summary>
    public static void RemoveOwnerState(PlayerControl owner, bool destroyOwnedTraps = true)
    {
        if (owner == null)
            return;

        if (destroyOwnedTraps)
            InfoTrap.ClearAll(owner);

        ownerStates.Remove(owner.PlayerId);
    }

    internal static bool IsActiveOwner(byte ownerId)
    {
        return Player != null && Player.PlayerId == ownerId && Player.Data?.Disconnected != true;
    }

    private static OwnerState GetOrCreateState(byte ownerId)
    {
        if (!ownerStates.TryGetValue(ownerId, out var state))
        {
            state = new OwnerState(maxTraps);
            ownerStates.Add(ownerId, state);
        }

        return state;
    }

    private static TrapperPlusReport BuildReport(OwnerState state)
    {
        if (state.TriggeredPlayers.Count < minPlayersToReport)
            return new TrapperPlusReport(TrapperPlusReport.ReportState.NotEnoughPlayers, Array.Empty<RoleId>());

        var roles = state.TriggeredPlayers.Values.ToList();
        for (var i = roles.Count - 1; i > 0; i--)
        {
            var j = rnd.Next(i + 1);
            (roles[i], roles[j]) = (roles[j], roles[i]);
        }

        return new TrapperPlusReport(TrapperPlusReport.ReportState.Ready, roles);
    }

    private static TrapperPlusReport EmptyReport()
    {
        return new TrapperPlusReport(TrapperPlusReport.ReportState.NoPlayers, Array.Empty<RoleId>());
    }
}
