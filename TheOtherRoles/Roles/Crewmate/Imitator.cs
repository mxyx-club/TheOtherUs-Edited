using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

/// <summary>
/// The Imitator keeps its canonical role and faction, but borrows the static holder
/// and a fresh set of runtime resources from one compatible dead crewmate role for
/// a single action round.  Every borrowed static state is restored byte-for-byte at
/// the next meeting; irreversible world actions (kills, revives and repairs) remain.
/// </summary>
public static class Imitator
{
    public sealed class Selection
    {
        public byte TargetId { get; internal set; } = byte.MaxValue;
        public RoleId Role { get; internal set; } = RoleId.DefaultRole;
    }

    public readonly record struct AuthorizedStart(byte ImitatorId, RoleId Role, uint MeetingGeneration);

    private sealed class MeetingAuthorization
    {
        public byte TargetId;
        public RoleId Role;
        public uint MeetingGeneration;
    }

    private sealed class PendingRedemption
    {
        public byte TargetId;
        public uint SessionGeneration;
        public bool SelfSacrificeConfirmed;
        public float ExpiresAt;
    }

    private sealed class MeetingCarry
    {
        public byte ImitatorId;
        public RoleId CopiedRole;
        public string OracleReport;
        public byte JailedId = byte.MaxValue;
        public int JailUses;
        public TrapperPlusReport TrapperReport;
    }

    private sealed class StaticSnapshot
    {
        private readonly List<(FieldInfo Field, object Value)> values = new();

        public StaticSnapshot(params Type[] types)
        {
            foreach (var type in types.Where(type => type != null).Distinct())
            {
                foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (field.IsLiteral || field.IsInitOnly)
                        continue;

                    try
                    {
                        values.Add((field, field.GetValue(null)));
                    }
                    catch (Exception ex)
                    {
                        Warn($"Could not snapshot {type.Name}.{field.Name}: {ex.Message}", "Imitator");
                    }
                }
            }
        }

        public void Restore()
        {
            foreach (var (field, value) in values)
            {
                try
                {
                    field.SetValue(null, value);
                }
                catch (Exception ex)
                {
                    Warn($"Could not restore {field.DeclaringType?.Name}.{field.Name}: {ex.Message}", "Imitator");
                }
            }
        }
    }

    private sealed class VentSnapshot
    {
        private readonly Vent vent;
        private readonly string name;
        private readonly Sprite sprite;
        private readonly Color color;
        private readonly Sprite fungleSprite;
        private readonly Color fungleColor;
        private readonly Dictionary<FieldInfo, object> animationFields = new();

        public VentSnapshot(Vent vent)
        {
            this.vent = vent;
            if (vent == null)
                return;

            name = vent.name;
            sprite = vent.myRend?.sprite;
            color = vent.myRend != null ? vent.myRend.color : Color.white;

            if (vent.transform.childCount > 3)
            {
                var renderer = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    fungleSprite = renderer.sprite;
                    fungleColor = renderer.color;
                }
            }

            foreach (var fieldName in new[] { "EnterVentAnim", "ExitVentAnim" })
            {
                var field = typeof(Vent).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                    animationFields[field] = field.GetValue(vent);
            }
        }

        public void Restore()
        {
            if (vent == null)
                return;

            vent.name = name;
            if (vent.myRend != null)
            {
                vent.myRend.sprite = sprite;
                vent.myRend.color = color;
            }

            if (vent.transform.childCount > 3)
            {
                var renderer = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = fungleSprite;
                    renderer.color = fungleColor;
                }
            }

            foreach (var (field, value) in animationFields)
                field.SetValue(vent, value);
        }
    }

    private sealed class ImitationSession
    {
        private readonly PlayerControl imitator;
        private readonly byte sourceTargetId;
        private readonly PlayerControl originalHolder;
        private readonly StaticSnapshot roleSnapshot;
        private readonly StaticSnapshot portalSnapshot;
        private readonly Aurial.SessionSnapshot aurialSnapshot;
        private readonly List<SurvCamera> originalPendingCameras;
        private readonly List<Vent> originalPendingVents;
        private readonly List<VentSnapshot> ventSnapshots;
        private readonly bool originalFirstPortalActive;
        private readonly bool originalSecondPortalActive;
        private readonly List<(Trap Trap, bool GameObjectWasActive, bool ArrowWasActive)> suspendedLegacyTraps;
        private readonly bool localPlayerWasHandcuffed;
        private readonly List<GameObject> localSeerSouls = new();
        private readonly List<SpriteRenderer> localAlchemystSouls = new();
        private readonly List<(GameObject Object, bool WasActive)> hiddenSourceObjects = new();
        internal bool dreamKillConsumed;
        private bool originalHolderInvalidated;

        public RoleId Role { get; }
        public uint Generation { get; }

        public ImitationSession(PlayerControl imitator, byte sourceTargetId, RoleId role)
        {
            this.imitator = imitator;
            this.sourceTargetId = sourceTargetId;
            Role = role;
            Generation = NextSessionGeneration();
            originalHolder = role == RoleId.Aurial ? Aurial.aurial : GetOriginalHolder(role, sourceTargetId);
            if (role is RoleId.Sheriff or RoleId.Deputy && PlayerControl.LocalPlayer != null)
            {
                var localId = PlayerControl.LocalPlayer.PlayerId;
                localPlayerWasHandcuffed = Sheriff.handcuffedPlayers?.Contains(localId) == true ||
                                          Sheriff.handcuffedKnows?.ContainsKey(localId) == true;
            }

            var roleType = GetStateType(role);
            if (role != RoleId.Aurial)
                roleSnapshot = new StaticSnapshot(roleType);

            if (role == RoleId.Portalmaker)
            {
                portalSnapshot = new StaticSnapshot(typeof(Portal));
                originalFirstPortalActive = Portal.firstPortal?.portalGameObject?.activeSelf ?? false;
                originalSecondPortalActive = Portal.secondPortal?.portalGameObject?.activeSelf ?? false;
                Portal.firstPortal?.portalGameObject?.SetActive(false);
                Portal.secondPortal?.portalGameObject?.SetActive(false);
            }

            if (role == RoleId.SecurityGuard)
            {
                originalPendingCameras = ModOption.camerasToAdd;
                originalPendingVents = ModOption.ventsToSeal;
                ventSnapshots = MapUtilities.CachedShipStatus?.AllVents?
                    .Where(vent => vent != null)
                    .Select(vent => new VentSnapshot(vent))
                    .ToList() ?? new List<VentSnapshot>();
                ModOption.camerasToAdd = new List<SurvCamera>();
                ModOption.ventsToSeal = new List<Vent>();
            }

            if (role == RoleId.Trapper)
            {
                suspendedLegacyTraps = Trap.AllObjects
                    .Where(trap => trap != null && trap.trapper != imitator)
                    .Select(trap => (Trap: trap,
                        GameObjectWasActive: trap.GameObject?.activeSelf ?? false,
                        ArrowWasActive: trap.ArrowObject?.activeSelf ?? false))
                    .ToList();
                foreach (var (trap, _, _) in suspendedLegacyTraps)
                {
                    trap?.GameObject?.SetActive(false);
                    if (trap?.ArrowObject != null)
                        trap.ArrowObject.SetActive(false);
                }
            }

            CaptureAndHideSourceObjects(role);

            try
            {
                if (role == RoleId.Aurial)
                    aurialSnapshot = Aurial.BeginImitation(imitator);
                else
                    InitializeFreshRole(imitator, role);
            }
            catch
            {
                RestoreOriginalState();
                throw;
            }
        }

        public MeetingCarry Finish(bool preserveMeetingCarry)
        {
            MeetingCarry carry = null;

            try
            {
                try
                {
                    if (preserveMeetingCarry)
                        carry = CaptureMeetingCarry();
                }
                catch (Exception ex)
                {
                    Warn($"Could not capture borrowed {Role} meeting data: {ex}", "Imitator");
                }

                try
                {
                    CleanupBorrowedState();
                }
                catch (Exception ex)
                {
                    Warn($"Borrowed {Role} cleanup failed: {ex}", "Imitator");
                }
            }
            finally
            {
                RestoreOriginalState();
            }

            return carry;
        }

        public void InvalidateOriginalHolder(byte playerId)
        {
            if (originalHolder?.PlayerId == playerId)
                originalHolderInvalidated = true;
        }

        public bool SourceIs(byte playerId) => sourceTargetId == playerId;

        public bool OriginalHolderIs(byte playerId) => originalHolder?.PlayerId == playerId;

        public bool LocalPlayerWasHandcuffed => localPlayerWasHandcuffed;

        public void RecordLocalDeath(PlayerControl deadPlayer, PlayerControl killer, CustomDeathReason deathReason)
        {
            if (deadPlayer == null || deadPlayer == imitator || imitator?.AmOwner != true || !imitator.IsAlive())
                return;

            var position = deadPlayer.transform.position;
            switch (Role)
            {
                case RoleId.Seer when Seer.mode is 0 or 2:
                {
                    var renderer = CreateLocalSoul("Imitator Seer Soul", position, Seer.soulSprite);
                    if (renderer != null)
                    {
                        localSeerSouls.Add(renderer.gameObject);
                        if (Seer.limitSoulDuration)
                        {
                            var soul = renderer.gameObject;
                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Seer.soulDuration,
                                new Action<float>(p =>
                                {
                                    if (renderer != null)
                                    {
                                        var color = renderer.color;
                                        color.a = Mathf.Clamp01(1f - p);
                                        renderer.color = color;
                                    }

                                    if (p != 1f)
                                        return;
                                    soul?.Destroy();
                                    localSeerSouls.Remove(soul);
                                })));
                        }
                    }
                    break;
                }

                case RoleId.Alchemyst:
                {
                    var dead = new Alchemyst.DeadPlayer(deadPlayer, DateTime.UtcNow, deathReason,
                        killer ?? deadPlayer, position);
                    Alchemyst.deadBodies ??= new List<Tuple<Alchemyst.DeadPlayer, Vector3>>();
                    Alchemyst.deadBodies.Add(Tuple.Create(dead, position));

                    var renderer = CreateLocalSoul("Imitator Alchemyst Soul", position, Alchemyst.soulSprite);
                    if (renderer != null)
                    {
                        Alchemyst.souls ??= new List<SpriteRenderer>();
                        Alchemyst.souls.Add(renderer);
                        localAlchemystSouls.Add(renderer);
                    }
                    break;
                }
            }
        }

        private void RestoreOriginalState()
        {
            // A copied Sheriff does not own Deputy state. Keep legitimate cuff
            // updates made by a still-living real Deputy while this session was
            // active, while excluding the borrowed Sheriff from the holder list.
            List<PlayerControl> currentSheriffs = null;
            List<byte> currentHandcuffedPlayers = null;
            Dictionary<byte, float> currentHandcuffedKnows = null;
            PlayerControl currentDeputy = null;
            PlayerControl currentFormerDeputy = null;
            var currentRemainingHandcuffs = 0f;
            if (Role == RoleId.Sheriff)
            {
                currentSheriffs = Sheriff.Player?
                    .Where(player => player != null && player.PlayerId != imitator.PlayerId)
                    .ToList() ?? new List<PlayerControl>();
                currentHandcuffedPlayers = new List<byte>(Sheriff.handcuffedPlayers ?? new List<byte>());
                currentHandcuffedKnows = new Dictionary<byte, float>(Sheriff.handcuffedKnows ?? new Dictionary<byte, float>());
                currentDeputy = Sheriff.Deputy;
                currentFormerDeputy = Sheriff.formerDeputy;
                currentRemainingHandcuffs = Sheriff.remainingHandcuffs;
            }

            if (Role == RoleId.Aurial)
            {
                Aurial.EndImitation(aurialSnapshot, !originalHolderInvalidated && IsStillMainRole(aurialSnapshot?.Holder, Role));
            }
            else
            {
                roleSnapshot?.Restore();
                if (originalHolderInvalidated)
                {
                    ClearRestoredHolder(Role, originalHolder?.PlayerId ?? byte.MaxValue);
                    if (Role == RoleId.TrapperPlus)
                        TrapperPlus.RemoveOwnerState(originalHolder, true);
                }
            }

            if (Role == RoleId.Sheriff)
            {
                if (originalHolderInvalidated && originalHolder != null)
                    currentSheriffs.RemoveAll(player => player?.PlayerId == originalHolder.PlayerId);
                Sheriff.Player = currentSheriffs;
                Sheriff.handcuffedPlayers = currentHandcuffedPlayers;
                Sheriff.handcuffedKnows = currentHandcuffedKnows;
                Sheriff.Deputy = currentDeputy;
                Sheriff.formerDeputy = currentFormerDeputy;
                Sheriff.remainingHandcuffs = currentRemainingHandcuffs;
            }

            if (Role == RoleId.Portalmaker)
            {
                portalSnapshot?.Restore();
                Portal.firstPortal?.portalGameObject?.SetActive(originalFirstPortalActive);
                Portal.secondPortal?.portalGameObject?.SetActive(originalSecondPortalActive);
            }

            if (Role == RoleId.SecurityGuard)
            {
                ModOption.camerasToAdd = originalPendingCameras ?? new List<SurvCamera>();
                ModOption.ventsToSeal = originalPendingVents ?? new List<Vent>();
            }


            if (Role == RoleId.Trapper)
            {
                foreach (var (trap, gameObjectWasActive, arrowWasActive) in suspendedLegacyTraps ??
                             new List<(Trap Trap, bool GameObjectWasActive, bool ArrowWasActive)>())
                {
                    if (originalHolderInvalidated && trap?.trapper == originalHolder)
                    {
                        trap.Destroy();
                        continue;
                    }
                    if (trap?.GameObject != null)
                        trap.GameObject.SetActive(gameObjectWasActive);
                    if (trap?.ArrowObject != null)
                        trap.ArrowObject.SetActive(arrowWasActive);
                }
            }


            foreach (var (obj, wasActive) in hiddenSourceObjects)
                if (obj != null)
                    obj.SetActive(wasActive);
        }

        private void CaptureAndHideSourceObjects(RoleId role)
        {
            void Hide(GameObject obj)
            {
                if (obj == null || hiddenSourceObjects.Any(entry => entry.Object == obj))
                    return;
                hiddenSourceObjects.Add((obj, obj.activeSelf));
                obj.SetActive(false);
            }

            void HideArrows(IEnumerable<Arrow> arrows)
            {
                foreach (var arrow in arrows?.ToArray() ?? Array.Empty<Arrow>())
                    Hide(arrow?.arrow);
            }

            switch (role)
            {
                case RoleId.Tracker:
                    HideArrows(Tracker.localArrows);
                    Hide(Tracker.arrow?.arrow);
                    Hide(Tracker.DangerMeterParent);
                    break;
                case RoleId.Snitch:
                    HideArrows(Snitch.localArrows);
                    Hide(Snitch.text?.gameObject);
                    break;
                case RoleId.Prophet:
                    HideArrows(Prophet.arrows);
                    break;
            }
        }

        private MeetingCarry CaptureMeetingCarry()
        {
            var carry = new MeetingCarry
            {
                ImitatorId = imitator.PlayerId,
                CopiedRole = Role,
            };

            if (Role == RoleId.TrapperPlus)
            {
                var roles = TrapperPlus.GetRoundEvidence(imitator).Values.ToList();
                TrapperPlusReport.ReportState state;
                if (roles.Count < TrapperPlus.minPlayersToReport)
                    state = TrapperPlusReport.ReportState.NotEnoughPlayers;
                else
                {
                    state = TrapperPlusReport.ReportState.Ready;
                    for (var i = roles.Count - 1; i > 0; i--)
                    {
                        var j = rnd.Next(i + 1);
                        (roles[i], roles[j]) = (roles[j], roles[i]);
                    }
                }
                carry.TrapperReport = new TrapperPlusReport(state, roles);
            }
            else if (Role == RoleId.Oracle && imitator.AmOwner && imitator.IsAlive() && Oracle.Confesser != null)
            {
                carry.OracleReport = Oracle.BuildReport();
            }
            else if (Role == RoleId.Jailor && Jailor.Jailed != null)
            {
                carry.JailedId = Jailor.Jailed.PlayerId;
                carry.JailUses = Jailor.usesCount;
            }

            return carry;
        }

        private void CleanupBorrowedState()
        {
            switch (Role)
            {
                case RoleId.Portalmaker:
                    Portal.firstPortal?.portalGameObject?.Destroy();
                    if (Portal.secondPortal != Portal.firstPortal)
                        Portal.secondPortal?.portalGameObject?.Destroy();
                    Portal.firstPortal = null;
                    Portal.secondPortal = null;
                    Portal.bothPlacedAndEnabled = false;
                    Portal.isTeleporting = false;
                    Portal.teleportedPlayers?.Clear();
                    break;

                case RoleId.Tracker:
                    DestroyArrows(Tracker.localArrows);
                    Tracker.arrow?.arrow?.Destroy();
                    Tracker.DangerMeterParent?.Destroy();
                    break;

                case RoleId.Detective:
                    FootprintHolder.Instance?.ClearSessionFootprints(imitator.PlayerId, Generation);
                    break;

                case RoleId.Seer:
                    foreach (var soul in localSeerSouls.ToArray())
                        soul?.Destroy();
                    localSeerSouls.Clear();
                    break;

                case RoleId.Snitch:
                    DestroyArrows(Snitch.localArrows);
                    Snitch.text?.Destroy();
                    break;

                case RoleId.Prophet:
                    DestroyArrows(Prophet.arrows);
                    break;

                case RoleId.SecurityGuard:
                    if (imitator?.AmOwner == true)
                        CloseBorrowedDevices(Role);
                    foreach (var camera in ModOption.camerasToAdd?.ToArray() ?? Array.Empty<SurvCamera>())
                        camera?.gameObject?.Destroy();
                    foreach (var snapshot in ventSnapshots ?? new List<VentSnapshot>())
                        snapshot.Restore();
                    ModOption.camerasToAdd = new List<SurvCamera>();
                    ModOption.ventsToSeal = new List<Vent>();
                    break;

                case RoleId.Alchemyst:
                    foreach (var soul in localAlchemystSouls.ToArray())
                        soul?.gameObject?.Destroy();
                    localAlchemystSouls.Clear();
                    break;

                case RoleId.Hacker:
                    if (imitator?.AmOwner == true)
                        CloseBorrowedDevices(Role);
                    break;

                case RoleId.Veteran:
                    Veteran.alertActive = false;
                    break;

                case RoleId.Trapper:
                    Trap.ClearAllTraps(imitator, true);
                    break;

                case RoleId.Redemptor:
                    Redemptor.arrow?.arrow?.Destroy();
                    Redemptor.text?.Destroy();
                    Redemptor.Prayering = false;
                    Redemptor.Revelating = false;
                    break;

                case RoleId.TrapperPlus:
                    TrapperPlus.RemoveOwnerState(imitator, true);
                    break;
            }

            if (Role is RoleId.Engineer or RoleId.Spy)
                ExitBorrowedVent();

            if (imitator?.AmOwner == true)
                ResetBorrowedButtons(Role);
        }

        private void ExitBorrowedVent()
        {
            if (imitator?.AmOwner != true || !imitator.inVent || imitator.MyPhysics == null)
                return;

            if (Vent.currentVent != null)
                imitator.MyPhysics.RpcExitVent(Vent.currentVent.Id);
            imitator.MyPhysics.ExitAllVents();
        }
    }

    public static PlayerControl Player;
    public static readonly Color color = new Color32(179, 217, 77, byte.MaxValue);
    public static Sprite selectSprite = new ResourceSprite("ImitateSelect.png");
    public static Sprite deselectSprite = new ResourceSprite("ImitateDeselect.png");

    private static readonly Dictionary<byte, Selection> selections = new();
    private static readonly Dictionary<byte, ImitationSession> sessions = new();
    private static readonly Dictionary<byte, MeetingCarry> meetingCarries = new();
    private static readonly HashSet<byte> consumedSourceIds = new();
    private static readonly Dictionary<byte, MeetingAuthorization> meetingAuthorizations = new();
    private static readonly HashSet<byte> meetingSelectionRequests = new();
    private static readonly Dictionary<byte, PendingRedemption> pendingRedemptions = new();
    private static uint nextSessionGeneration;
    private static uint meetingGeneration;
    private static uint hostStartedMeetingGeneration;
    private static bool meetingSelectionOpen;

    private static uint NextSessionGeneration()
    {
        unchecked
        {
            nextSessionGeneration++;
            if (nextSessionGeneration == 0)
                nextSessionGeneration++;
            return nextSessionGeneration;
        }
    }

    public static readonly HashSet<RoleId> CompatibleRoles = new()
    {
        RoleId.Portalmaker,
        RoleId.Engineer,
        RoleId.Sheriff,
        RoleId.Deputy,
        RoleId.BodyGuard,
        RoleId.Jumper,
        RoleId.Detective,
        RoleId.Veteran,
        RoleId.Medic,
        RoleId.Seer,
        RoleId.Hacker,
        RoleId.Tracker,
        RoleId.Snitch,
        RoleId.Prophet,
        RoleId.Spy,
        RoleId.SecurityGuard,
        RoleId.Alchemyst,
        RoleId.Trapper,
        RoleId.Redemptor,
        RoleId.Jailor,
        RoleId.Dreamcatcher,
        RoleId.Oracle,
        RoleId.TrapperPlus,
        RoleId.Aurial,
    };

    public static byte SelectedTargetId => Player != null && selections.TryGetValue(Player.PlayerId, out var selection)
        ? selection.TargetId
        : byte.MaxValue;

    public static RoleId SelectedRole => Player != null && selections.TryGetValue(Player.PlayerId, out var selection)
        ? selection.Role
        : RoleId.DefaultRole;

    public static RoleId ActiveCopiedRole => Player != null && sessions.TryGetValue(Player.PlayerId, out var session)
        ? session.Role
        : RoleId.DefaultRole;

    public static bool IsActive => Player != null && sessions.ContainsKey(Player.PlayerId);
    public static bool HasAnyActiveSession => sessions.Count > 0;

    public static bool IsActiveFor(byte playerId) => sessions.ContainsKey(playerId);

    public static uint CurrentMeetingGeneration => meetingGeneration;

    public static void BeginMeeting()
    {
        pendingRedemptions.Clear();
        unchecked
        {
            meetingGeneration++;
            if (meetingGeneration == 0)
                meetingGeneration++;
        }

        meetingSelectionOpen = true;
        meetingAuthorizations.Clear();
        meetingSelectionRequests.Clear();
        foreach (var imitatorId in selections.Keys.ToArray())
            ClearSelection(imitatorId, false);
    }

    public static Selection GetSelection(byte imitatorId)
    {
        if (!selections.TryGetValue(imitatorId, out var selection))
        {
            selection = new Selection();
            selections[imitatorId] = selection;
        }
        return selection;
    }

    /// <summary>RPC 231 receiver.</summary>
    public static void SetSelection(byte imitatorId, byte targetId, RoleId roleSnapshot)
    {
        var selection = GetSelection(imitatorId);
        if (targetId == byte.MaxValue || !IsCompatible(roleSnapshot))
        {
            selection.TargetId = byte.MaxValue;
            selection.Role = RoleId.DefaultRole;
            return;
        }

        var target = PlayerById(targetId);
        if (target == null || target.Data == null || !target.Data.IsDead || target.Data.Disconnected ||
            GetEligibleRole(target) != roleSnapshot)
        {
            selection.TargetId = byte.MaxValue;
            selection.Role = RoleId.DefaultRole;
            return;
        }

        selection.TargetId = targetId;
        selection.Role = roleSnapshot;
    }

    public static bool TryHostAuthorizeSelection(PlayerControl sender, byte imitatorId, byte targetId,
        RoleId roleSnapshot, uint generation, out byte canonicalTargetId, out RoleId canonicalRole)
    {
        canonicalTargetId = byte.MaxValue;
        canonicalRole = RoleId.DefaultRole;
        if (AmongUsClient.Instance?.AmHost != true || !meetingSelectionOpen || MeetingHud.Instance == null ||
            generation == 0 || generation != meetingGeneration || sender == null || sender.PlayerId != imitatorId ||
            meetingSelectionRequests.Contains(imitatorId))
            return false;

        var imitator = PlayerById(imitatorId);
        if (imitator == null || Imitator.Player != imitator || !imitator.IsAlive() ||
            PlayerData.GetPlayerData(imitator)?.MainRole != RoleId.Imitator)
            return false;

        // The normal UI submits once VotingComplete has locked the meeting.
        // Requests sent during the action phase or while votes are still editable
        // never mint a start token.
        if (MeetingHud.Instance.state != MeetingHud.VoteStates.Results)
            return false;

        meetingSelectionRequests.Add(imitatorId);

        var target = PlayerById(targetId);
        if (targetId != byte.MaxValue && target != null && GetEligibleRole(target) == roleSnapshot)
        {
            canonicalTargetId = targetId;
            canonicalRole = roleSnapshot;
            SetSelection(imitatorId, targetId, roleSnapshot);
            meetingAuthorizations[imitatorId] = new MeetingAuthorization
            {
                TargetId = targetId,
                Role = roleSnapshot,
                MeetingGeneration = generation,
            };
        }
        else
        {
            ClearSelection(imitatorId, false);
        }

        return true;
    }

    public static bool ApplyHostSelection(byte imitatorId, byte targetId, RoleId roleSnapshot, uint generation)
    {
        if (generation == 0 || generation != meetingGeneration)
            return false;

        if (targetId == byte.MaxValue || roleSnapshot == RoleId.DefaultRole)
        {
            ClearSelection(imitatorId, false);
            meetingAuthorizations.Remove(imitatorId);
            return true;
        }

        SetSelection(imitatorId, targetId, roleSnapshot);
        var selection = GetSelection(imitatorId);
        if (selection.TargetId != targetId || selection.Role != roleSnapshot)
            return false;

        meetingAuthorizations[imitatorId] = new MeetingAuthorization
        {
            TargetId = targetId,
            Role = roleSnapshot,
            MeetingGeneration = generation,
        };
        return true;
    }

    public static IReadOnlyList<AuthorizedStart> GetAuthorizedStartsForHost()
    {
        if (AmongUsClient.Instance?.AmHost != true || meetingGeneration == 0 ||
            hostStartedMeetingGeneration == meetingGeneration)
            return Array.Empty<AuthorizedStart>();

        hostStartedMeetingGeneration = meetingGeneration;
        meetingSelectionOpen = false;
        return meetingAuthorizations
            .Where(entry => entry.Value.MeetingGeneration == meetingGeneration)
            .Select(entry => new AuthorizedStart(entry.Key, entry.Value.Role, entry.Value.MeetingGeneration))
            .ToArray();
    }

    public static bool StartAuthorized(byte imitatorId, RoleId copiedRole, uint generation)
    {
        if (generation == 0 || generation != meetingGeneration ||
            !meetingAuthorizations.TryGetValue(imitatorId, out var authorization) ||
            authorization.MeetingGeneration != generation || authorization.Role != copiedRole)
            return false;

        meetingAuthorizations.Remove(imitatorId);
        meetingSelectionOpen = false;
        return Start(imitatorId, copiedRole);
    }

    /// <summary>RPC 232 receiver.</summary>
    public static bool Start(byte imitatorId, RoleId copiedRole)
    {
        var imitator = PlayerById(imitatorId);
        if (imitator == null || Player == null || Player.PlayerId != imitatorId ||
            imitator.Data == null || imitator.Data.IsDead || imitator.Data.Disconnected ||
            PlayerData.GetPlayerData(imitator)?.MainRole != RoleId.Imitator || !IsCompatible(copiedRole))
        {
            ClearSelection(imitatorId);
            return false;
        }

        if (sessions.TryGetValue(imitatorId, out var existing))
        {
            if (existing.Role == copiedRole)
                return true;
            EndInternal(imitatorId, false);
        }

        try
        {
            var selection = GetSelection(imitatorId);
            var sourceTarget = PlayerById(selection.TargetId);
            if (sourceTarget == null || sourceTarget.Data == null || !sourceTarget.Data.IsDead ||
                sourceTarget.Data.Disconnected || selection.Role != copiedRole || GetEligibleRole(sourceTarget) != copiedRole)
            {
                ClearSelection(imitatorId);
                return false;
            }

            sessions[imitatorId] = new ImitationSession(imitator, sourceTarget.PlayerId, copiedRole);
            if (imitator.AmOwner)
                ResetBorrowedButtons(copiedRole);
            ClearSelection(imitatorId);
            return true;
        }
        catch (Exception ex)
        {
            Error($"Failed to start {copiedRole} imitation: {ex}", "Imitator");
            EndInternal(imitatorId, false);
            ClearSelection(imitatorId);
            return false;
        }
    }

    /// <summary>RPC 233 receiver.</summary>
    public static void End(byte imitatorId) => EndInternal(imitatorId, true);

    public static void EndAuthorized(byte imitatorId, uint generation)
    {
        if (generation != 0 && sessions.TryGetValue(imitatorId, out var session) && session.Generation == generation)
            EndInternal(imitatorId, true);
    }

    public static void EndAll(bool preserveMeetingCarry)
    {
        foreach (var imitatorId in sessions.Keys.ToArray())
            EndInternal(imitatorId, preserveMeetingCarry);
    }

    /// <summary>
    /// Restores the borrowed static holder without producing meeting evidence.
    /// Role erasure and the global game reset call this before they begin clearing
    /// role singletons, otherwise a late snapshot restore could resurrect a holder
    /// which the reset has already removed.
    /// </summary>
    public static void Abort(byte imitatorId) => EndInternal(imitatorId, false);

    public static void NotifyPlayerRoleInvalidated(byte playerId)
    {
        foreach (var session in sessions.Values)
            session.InvalidateOriginalHolder(playerId);
    }

    public static void NotifyPlayerStateChanged(byte playerId)
    {
        pendingRedemptions.Remove(playerId);
        foreach (var (imitatorId, session) in sessions.ToArray())
        {
            if (!session.OriginalHolderIs(playerId))
                continue;
            session.InvalidateOriginalHolder(playerId);
            EndInternal(imitatorId, false);
        }
        ClearSelectionsForTarget(playerId);
        CancelForPlayer(playerId);
    }

    public static void NotifyPlayerRevived(byte playerId)
    {
        pendingRedemptions.Remove(playerId);
        consumedSourceIds.Remove(playerId);
        ClearSelectionsForTarget(playerId);
        CancelForPlayer(playerId);
    }

    public static void NotifyRoleConsumed(RoleId role, byte sourcePlayerId)
    {
        consumedSourceIds.Add(sourcePlayerId);
        ClearSelectionsForTarget(sourcePlayerId);
        foreach (var (imitatorId, session) in sessions.ToArray())
        {
            if (session.Role == role || session.SourceIs(sourcePlayerId))
                EndInternal(imitatorId, false);
        }
    }

    public static void NotifyRoleAssigned(RoleId role, byte newHolderId)
    {
        if (!IsCompatible(role))
            return;

        foreach (var (imitatorId, session) in sessions.ToArray())
            if (session.Role == role && imitatorId != newHolderId)
                EndInternal(imitatorId, false);
    }

    public static bool TryGetBorrowedSourceRole(PlayerControl player, out RoleId role)
    {
        role = RoleId.DefaultRole;
        if (player == null)
            return false;

        foreach (var session in sessions.Values)
        {
            if (!session.SourceIs(player.PlayerId))
                continue;
            role = session.Role;
            return true;
        }
        return false;
    }

    private static void CancelForPlayer(byte playerId)
    {
        foreach (var (imitatorId, session) in sessions.ToArray())
        {
            if (imitatorId == playerId || session.SourceIs(playerId))
                EndInternal(imitatorId, false);
        }
    }

    private static void ClearSelectionsForTarget(byte targetId)
    {
        foreach (var (imitatorId, selection) in selections.ToArray())
        {
            if (selection.TargetId != targetId)
                continue;
            ClearSelection(imitatorId, false);
            meetingAuthorizations.Remove(imitatorId);
        }
        ImitatorPatches.NotifyTargetInvalidated(targetId);
    }

    public static bool IsImitating(PlayerControl player, RoleId role = RoleId.DefaultRole)
    {
        if (player == null || !sessions.TryGetValue(player.PlayerId, out var session))
            return false;
        return role == RoleId.DefaultRole || session.Role == role;
    }

    public static bool HasActiveRole(RoleId role) => sessions.Values.Any(session => session.Role == role);

    public static bool TryConsumeDreamKill(PlayerControl owner)
    {
        if (owner == null || !sessions.TryGetValue(owner.PlayerId, out var session) ||
            session.Role != RoleId.Dreamcatcher || session.dreamKillConsumed)
            return false;
        session.dreamKillConsumed = true;
        return true;
    }

    public static bool IsDreamKillAvailable(PlayerControl owner)
    {
        return owner != null && sessions.TryGetValue(owner.PlayerId, out var session) &&
               session.Role == RoleId.Dreamcatcher && !session.dreamKillConsumed;
    }

    public static bool IsSessionCurrent(PlayerControl owner, RoleId role)
        => IsImitating(owner, role);

    public static bool TryGetSessionGeneration(PlayerControl owner, RoleId role, out uint generation)
    {
        generation = 0;
        if (owner == null || !sessions.TryGetValue(owner.PlayerId, out var session) || session.Role != role)
            return false;
        generation = session.Generation;
        return true;
    }

    public static bool IsSessionCurrent(PlayerControl owner, RoleId role, uint generation)
    {
        return generation != 0 && owner != null && sessions.TryGetValue(owner.PlayerId, out var session) &&
               session.Role == role && session.Generation == generation;
    }

    public static void EnableBorrowedPortals(PlayerControl owner)
    {
        if (IsImitating(owner, RoleId.Portalmaker))
            Portal.EnablePlacedPortals();
    }

    public static bool PrepareBorrowedRedemption(PlayerControl caster, byte targetId, uint generation)
    {
        var target = PlayerById(targetId);
        if (!IsSessionCurrent(caster, RoleId.Redemptor, generation) || target == null ||
            target == caster || target.Data == null || !target.Data.IsDead || target.Data.Disconnected)
            return false;

        pendingRedemptions[caster.PlayerId] = new PendingRedemption
        {
            TargetId = targetId,
            SessionGeneration = generation,
            SelfSacrificeConfirmed = false,
            ExpiresAt = Time.time + Mathf.Max(10f, Redemptor.reviveDuration + 10f),
        };
        return true;
    }

    public static void AuthorizeBorrowedRedemptionDeath(PlayerControl caster)
    {
        if (caster == null || !pendingRedemptions.TryGetValue(caster.PlayerId, out var pending) ||
            !IsSessionCurrent(caster, RoleId.Redemptor, pending.SessionGeneration))
            return;

        var target = PlayerById(pending.TargetId);
        if (target == null || target.Data == null || !target.Data.IsDead || target.Data.Disconnected)
        {
            pendingRedemptions.Remove(caster.PlayerId);
            return;
        }

        pending.SelfSacrificeConfirmed = true;
        pending.ExpiresAt = Time.time + Mathf.Max(5f, Redemptor.reviveDuration + 5f);
    }

    public static bool TryCompleteBorrowedRedemption(PlayerControl caster, byte targetId, uint generation)
    {
        if (caster == null || generation == 0 ||
            !pendingRedemptions.TryGetValue(caster.PlayerId, out var pending) ||
            pending.TargetId != targetId || pending.SessionGeneration != generation ||
            !pending.SelfSacrificeConfirmed || Time.time > pending.ExpiresAt || caster.Data?.IsDead != true ||
            PlayerData.GetPlayerData(caster)?.MainRole != RoleId.Imitator)
            return false;

        pendingRedemptions.Remove(caster.PlayerId);
        var target = PlayerById(targetId);
        if (target == null || target == caster || target.Data == null || !target.Data.IsDead || target.Data.Disconnected)
            return false;

        Redemptor.RevivePlayerForCaster(caster, targetId);
        return true;
    }

    public static bool TryCompleteBorrowedPrayer(PlayerControl caster, byte targetId, uint generation)
    {
        var target = PlayerById(targetId);
        if (!Redemptor.prayer || !IsSessionCurrent(caster, RoleId.Redemptor, generation) || target == null ||
            target == caster || target.Data == null || !target.Data.IsDead || target.Data.Disconnected)
            return false;

        Redemptor.RevivePlayerForCaster(caster, targetId);
        Redemptor.RevivedPlayer = target;
        Redemptor.target = null;
        return true;
    }

    public static void RecordLocalDeath(PlayerControl deadPlayer, PlayerControl killer, CustomDeathReason deathReason)
    {
        if (deadPlayer != null)
            consumedSourceIds.Remove(deadPlayer.PlayerId);
        foreach (var session in sessions.Values.ToArray())
            session.RecordLocalDeath(deadPlayer, killer, deathReason);
    }

    public static RoleId GetEffectiveRole(PlayerControl player)
    {
        return player != null && sessions.TryGetValue(player.PlayerId, out var session)
            ? session.Role
            : PlayerData.GetPlayerData(player)?.MainRole ?? RoleId.DefaultRole;
    }

    public static RoleId GetEligibleRole(PlayerControl target)
    {
        if (target == null || target.Data == null || !target.Data.IsDead || target.Data.Disconnected ||
            consumedSourceIds.Contains(target.PlayerId))
            return RoleId.DefaultRole;

        var role = PlayerData.GetPlayerData(target)?.MainRole ?? RoleId.DefaultRole;
        return IsCompatible(role) ? role : RoleId.DefaultRole;
    }

    public static bool IsCompatible(RoleId role) => CompatibleRoles.Contains(role);

    public static void ShowMeetingCarry(MeetingHud meeting)
    {
        if (meeting == null)
            return;

        foreach (var carry in meetingCarries.Values.ToArray())
        {
            var owner = PlayerById(carry.ImitatorId);
            if (owner == null)
                continue;

            if (carry.TrapperReport != null && (owner.AmOwner || CanSeeGhostInfo))
                HudManager.Instance?.Chat?.AddChat(owner, carry.TrapperReport.ToLocalizedText());

            if (!string.IsNullOrWhiteSpace(carry.OracleReport) && owner.AmOwner)
                HudManager.Instance?.Chat?.AddChat(owner, carry.OracleReport);

            if (carry.JailedId != byte.MaxValue)
                ShowCarriedJail(meeting, owner, PlayerById(carry.JailedId), carry.JailUses);
        }
    }

    public static bool TryGetCarriedJail(PlayerControl participant, out PlayerControl owner, out PlayerControl jailed)
    {
        owner = null;
        jailed = null;
        if (participant == null || MeetingHud.Instance == null)
            return false;

        foreach (var carry in meetingCarries.Values)
        {
            if (carry.CopiedRole != RoleId.Jailor || carry.JailedId == byte.MaxValue)
                continue;

            var candidateOwner = PlayerById(carry.ImitatorId);
            var candidateJailed = PlayerById(carry.JailedId);
            if (participant.PlayerId != carry.ImitatorId && participant.PlayerId != carry.JailedId)
                continue;
            if (!CanUseCarriedJail(candidateOwner, candidateJailed))
                continue;

            owner = candidateOwner;
            jailed = candidateJailed;
            return true;
        }

        return false;
    }

    public static bool CanExecuteCarriedJail(PlayerControl owner, PlayerControl jailed)
    {
        if (!CanUseCarriedJail(owner, jailed))
            return false;

        return meetingCarries.TryGetValue(owner.PlayerId, out var carry) &&
               carry.CopiedRole == RoleId.Jailor &&
               carry.JailedId == jailed.PlayerId &&
               carry.JailUses > 0;
    }

    public static void ClearCarriedJail(byte ownerId, byte jailedId)
    {
        if (!meetingCarries.TryGetValue(ownerId, out var carry) ||
            carry.CopiedRole != RoleId.Jailor || carry.JailedId != jailedId)
            return;

        meetingCarries.Remove(ownerId);
        foreach (var playerState in MeetingHud.Instance?.playerStates ?? Enumerable.Empty<PlayerVoteArea>())
        {
            playerState.transform.FindChild("ImitatorJailCell")?.gameObject?.Destroy();
            playerState.transform.FindChild("ImitatorJailTargetIcon")?.gameObject?.Destroy();
        }
    }

    public static void ClearMeetingCarries() => meetingCarries.Clear();

    public static void clearAndReload()
    {
        foreach (var imitatorId in sessions.Keys.ToArray())
            EndInternal(imitatorId, false);

        selections.Clear();
        sessions.Clear();
        meetingCarries.Clear();
        consumedSourceIds.Clear();
        meetingAuthorizations.Clear();
        meetingSelectionRequests.Clear();
        pendingRedemptions.Clear();
        meetingSelectionOpen = false;
        meetingGeneration = 0;
        hostStartedMeetingGeneration = 0;
        nextSessionGeneration = 0;
        Player = null;
    }

    public static void ClearAndReload() => clearAndReload();

    private static void EndInternal(byte imitatorId, bool preserveMeetingCarry)
    {
        if (!sessions.Remove(imitatorId, out var session))
        {
            ClearSelection(imitatorId);
            return;
        }

        try
        {
            var carry = session.Finish(preserveMeetingCarry);
            if (carry != null)
                meetingCarries[imitatorId] = carry;
        }
        catch (Exception ex)
        {
            Error($"Failed to finish {session.Role} imitation: {ex}", "Imitator");
        }
        finally
        {
            if (session.Role == RoleId.Deputy && !session.LocalPlayerWasHandcuffed)
                CustomButton.setAllButtonsHandcuffedStatus(false);
            ClearSelection(imitatorId);
        }
    }

    private static void ClearSelection(byte imitatorId, bool clearAuthorization = true)
    {
        var selection = GetSelection(imitatorId);
        selection.TargetId = byte.MaxValue;
        selection.Role = RoleId.DefaultRole;
        if (clearAuthorization)
            meetingAuthorizations.Remove(imitatorId);
    }

    private static void CloseBorrowedDevices(RoleId role)
    {
        try
        {
            if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled)
                MapBehaviour.Instance.Close();
            if (Minigame.Instance)
                Minigame.Instance.ForceClose();

            if (role == RoleId.Hacker)
            {
                var vitals = Hacker.vitals;
                var doorLog = Hacker.doorLog;
                if (vitals)
                    vitals.ForceClose();
                if (doorLog && doorLog != vitals)
                    doorLog.ForceClose();
                Hacker.vitals = null;
                Hacker.doorLog = null;
            }
            else if (role == RoleId.SecurityGuard)
            {
                var minigame = SecurityGuard.minigame;
                if (minigame)
                    minigame.ForceClose();
                SecurityGuard.minigame = null;
            }
        }
        catch (Exception ex)
        {
            Warn($"Could not close borrowed device UI: {ex.Message}", "Imitator");
        }
        if (PlayerControl.LocalPlayer != null)
            PlayerControl.LocalPlayer.moveable = true;

        var buttons = role == RoleId.Hacker
            ? new[]
            {
                HudManagerStartPatch.hackerButton,
                HudManagerStartPatch.hackerVitalsButton,
                HudManagerStartPatch.hackerAdminTableButton,
            }
            : new[] { HudManagerStartPatch.securityGuardButton, HudManagerStartPatch.securityGuardCamButton };
        foreach (var button in buttons.Where(button => button != null))
        {
            button.IsEffectActive = false;
            button.EffectTimer = 0f;
            button.Timer = button.MaxTimer;
            if (button.actionButton?.cooldownTimerText != null)
                button.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        }
    }

    private static void ResetBorrowedButtons(RoleId role)
    {
        IEnumerable<CustomButton> buttons = role switch
        {
            RoleId.Portalmaker => new[]
            {
                HudManagerStartPatch.portalmakerPlacePortalButton,
                HudManagerStartPatch.usePortalButton,
                HudManagerStartPatch.portalmakerMoveToPortalButton,
            },
            RoleId.Engineer => new[] { HudManagerStartPatch.engineerRepairButton },
            RoleId.Sheriff => new[] { HudManagerStartPatch.sheriffKillButton },
            RoleId.Deputy => new[] { HudManagerStartPatch.deputyHandcuffButton },
            RoleId.BodyGuard => new[] { HudManagerStartPatch.bodyGuardGuardButton },
            RoleId.Jumper => new[] { HudManagerStartPatch.jumperMarkButton, HudManagerStartPatch.jumperJumpButton },
            RoleId.Veteran => new[] { HudManagerStartPatch.veteranAlertButton },
            RoleId.Medic => new[] { HudManagerStartPatch.medicShieldButton },
            RoleId.Hacker => new[]
            {
                HudManagerStartPatch.hackerButton,
                HudManagerStartPatch.hackerVitalsButton,
                HudManagerStartPatch.hackerAdminTableButton,
            },
            RoleId.Tracker => new[]
            {
                HudManagerStartPatch.trackerTrackPlayerButton,
                HudManagerStartPatch.trackerTrackCorpsesButton,
            },
            RoleId.Prophet => new[] { HudManagerStartPatch.prophetButton },
            RoleId.SecurityGuard => new[]
            {
                HudManagerStartPatch.securityGuardButton,
                HudManagerStartPatch.securityGuardCamButton,
            },
            RoleId.Alchemyst => new[] { HudManagerStartPatch.alchemystButton, HudManagerStartPatch.alchemystKillButton },
            RoleId.Trapper => new[] { HudManagerStartPatch.trapperButton },
            RoleId.Redemptor => new[]
            {
                HudManagerStartPatch.redemptorRevelationButton,
                HudManagerStartPatch.redemptorPrayerButton,
                HudManagerStartPatch.redemptorReviveButton,
            },
            RoleId.Jailor => new[] { HudManagerStartPatch.jailorButton },
            RoleId.Dreamcatcher => new[] { HudManagerStartPatch.dreamcatcherButton },
            RoleId.Oracle => new[] { HudManagerStartPatch.oracleButton },
            RoleId.TrapperPlus => new[] { HudManagerStartPatch.trapperPlusButton },
            RoleId.Aurial => new[] { HudManagerStartPatch.aurialRadiateButton },
            _ => Array.Empty<CustomButton>(),
        };

        foreach (var button in buttons.Where(button => button != null))
        {
            button.Timer = button.MaxTimer < 1f ? 0f : button.MaxTimer;
            button.EffectTimer = 0f;
            button.IsEffectActive = false;
            button.Multiplier = 1f;
            if (button.actionButton?.cooldownTimerText != null)
                button.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        }
    }

    private static bool IsStillMainRole(PlayerControl player, RoleId role)
    {
        return player != null && player.Data?.Disconnected != true &&
               PlayerData.GetPlayerData(player)?.MainRole == role;
    }

    private static PlayerControl GetOriginalHolder(RoleId role, byte preferredPlayerId = byte.MaxValue)
    {
        return role switch
        {
            RoleId.Portalmaker => Portalmaker.portalmaker,
            RoleId.Engineer => Engineer.engineer,
            RoleId.Sheriff => Sheriff.Player?.FirstOrDefault(player => player?.PlayerId == preferredPlayerId) ?? Sheriff.Player?.FirstOrDefault(),
            RoleId.Deputy => Sheriff.Deputy,
            RoleId.BodyGuard => BodyGuard.bodyguard,
            RoleId.Jumper => Jumper.jumper,
            RoleId.Detective => Detective.detective,
            RoleId.Veteran => Veteran.veteran,
            RoleId.Medic => Medic.medic,
            RoleId.Seer => Seer.seer,
            RoleId.Hacker => Hacker.hacker,
            RoleId.Tracker => Tracker.tracker,
            RoleId.Snitch => Snitch.snitch,
            RoleId.Prophet => Prophet.prophet,
            RoleId.Spy => Spy.spy,
            RoleId.SecurityGuard => SecurityGuard.securityGuard,
            RoleId.Alchemyst => Alchemyst.Player,
            RoleId.Trapper => Trapper.trapper,
            RoleId.Redemptor => Redemptor.Player,
            RoleId.Jailor => Jailor.Player,
            RoleId.Dreamcatcher => Dreamcatcher.Player,
            RoleId.Oracle => Oracle.Player,
            RoleId.TrapperPlus => TrapperPlus.Player,
            _ => null,
        };
    }

    private static void ClearRestoredHolder(RoleId role, byte invalidPlayerId)
    {
        switch (role)
        {
            case RoleId.Portalmaker: Portalmaker.portalmaker = null; break;
            case RoleId.Engineer: Engineer.engineer = null; break;
            case RoleId.Sheriff: Sheriff.Player?.RemoveAll(player => player?.PlayerId == invalidPlayerId); break;
            case RoleId.Deputy: Sheriff.Deputy = null; break;
            case RoleId.BodyGuard: BodyGuard.bodyguard = null; break;
            case RoleId.Jumper: Jumper.jumper = null; break;
            case RoleId.Detective: Detective.detective = null; break;
            case RoleId.Veteran: Veteran.veteran = null; break;
            case RoleId.Medic: Medic.medic = null; break;
            case RoleId.Seer: Seer.seer = null; break;
            case RoleId.Hacker: Hacker.hacker = null; break;
            case RoleId.Tracker: Tracker.tracker = null; break;
            case RoleId.Snitch: Snitch.snitch = null; break;
            case RoleId.Prophet: Prophet.prophet = null; break;
            case RoleId.Spy: Spy.spy = null; break;
            case RoleId.SecurityGuard: SecurityGuard.securityGuard = null; break;
            case RoleId.Alchemyst: Alchemyst.Player = null; break;
            case RoleId.Trapper: Trapper.trapper = null; break;
            case RoleId.Redemptor: Redemptor.Player = null; break;
            case RoleId.Jailor: Jailor.Player = null; break;
            case RoleId.Dreamcatcher: Dreamcatcher.Player = null; break;
            case RoleId.Oracle: Oracle.Player = null; break;
            case RoleId.TrapperPlus: TrapperPlus.Player = null; break;
        }
    }

    private static Type GetStateType(RoleId role)
    {
        return role switch
        {
            RoleId.Portalmaker => typeof(Portalmaker),
            RoleId.Engineer => typeof(Engineer),
            RoleId.Sheriff or RoleId.Deputy => typeof(Sheriff),
            RoleId.BodyGuard => typeof(BodyGuard),
            RoleId.Jumper => typeof(Jumper),
            RoleId.Detective => typeof(Detective),
            RoleId.Veteran => typeof(Veteran),
            RoleId.Medic => typeof(Medic),
            RoleId.Seer => typeof(Seer),
            RoleId.Hacker => typeof(Hacker),
            RoleId.Tracker => typeof(Tracker),
            RoleId.Snitch => typeof(Snitch),
            RoleId.Prophet => typeof(Prophet),
            RoleId.Spy => typeof(Spy),
            RoleId.SecurityGuard => typeof(SecurityGuard),
            RoleId.Alchemyst => typeof(Alchemyst),
            RoleId.Trapper => typeof(Trapper),
            RoleId.Redemptor => typeof(Redemptor),
            RoleId.Jailor => typeof(Jailor),
            RoleId.Dreamcatcher => typeof(Dreamcatcher),
            RoleId.Oracle => typeof(Oracle),
            RoleId.TrapperPlus => typeof(TrapperPlus),
            _ => null,
        };
    }

    private static void InitializeFreshRole(PlayerControl player, RoleId role)
    {
        switch (role)
        {
            case RoleId.Portalmaker:
                Portalmaker.portalmaker = player;
                Portal.firstPortal = null;
                Portal.secondPortal = null;
                Portal.bothPlacedAndEnabled = false;
                Portal.isTeleporting = false;
                Portal.teleportedPlayers = new List<Portal.tpLogEntry>();
                break;

            case RoleId.Engineer:
                Engineer.engineer = player;
                Engineer.UsedFix = false;
                Engineer.remainingFixes = CustomOptionHolder.engineerNumberOfFixes.GetInt();
                break;

            case RoleId.Sheriff:
                InitializeSheriff(player, false);
                break;

            case RoleId.Deputy:
                InitializeSheriff(player, true);
                break;

            case RoleId.BodyGuard:
                BodyGuard.bodyguard = player;
                BodyGuard.guarded = null;
                BodyGuard.currentTarget = null;
                BodyGuard.usedGuard = false;
                break;

            case RoleId.Jumper:
                Jumper.jumper = player;
                Jumper.jumpLocation = Vector3.zero;
                Jumper.usedPlace = false;
                Jumper.Charges = Math.Max(0, Mathf.FloorToInt(Jumper.MaxCharges));
                break;

            case RoleId.Detective:
                Detective.detective = player;
                Detective.timer = 0f;
                break;

            case RoleId.Veteran:
                Veteran.veteran = player;
                Veteran.alertActive = false;
                break;

            case RoleId.Medic:
                Medic.medic = player;
                Medic.shielded = null;
                Medic.futureShielded = null;
                Medic.currentTarget = null;
                Medic.usedShield = false;
                Medic.meetingAfterShielding = false;
                break;

            case RoleId.Seer:
                Seer.seer = player;
                Seer.deadBodyPositions = new List<Vector3>();
                break;

            case RoleId.Hacker:
                Hacker.hacker = player;
                Hacker.vitals = null;
                Hacker.doorLog = null;
                Hacker.hackerTimer = 0f;
                Hacker.chargesVitals = Math.Max(0, Mathf.FloorToInt(Hacker.toolsNumber));
                Hacker.chargesAdminTable = Math.Max(0, Mathf.FloorToInt(Hacker.toolsNumber));
                Hacker.rechargedTasks = Hacker.rechargeTasksNumber;
                break;

            case RoleId.Tracker:
                Tracker.tracker = player;
                Tracker.localArrows = new List<Arrow>();
                Tracker.deadBodyPositions = new List<Vector3>();
                Tracker.currentTarget = null;
                Tracker.tracked = null;
                Tracker.usedTracker = false;
                Tracker.timeUntilUpdate = 0f;
                Tracker.corpsesTrackingTimer = 0f;
                Tracker.arrow = new Arrow(Color.blue);
                Tracker.arrow.arrow?.SetActive(false);
                Tracker.DangerMeterParent = null;
                Tracker.Meter = null;
                break;

            case RoleId.Snitch:
                Snitch.snitch = player;
                Snitch.localArrows = new List<Arrow>();
                Snitch.needsUpdate = true;
                Snitch.text = null;
                break;

            case RoleId.Prophet:
                Prophet.prophet = player;
                Prophet.currentTarget = null;
                Prophet.examined = new Dictionary<PlayerControl, bool>();
                Prophet.examinesLeft = Prophet.examineNum;
                Prophet.isRevealed = false;
                Prophet.arrows = new List<Arrow>();
                break;

            case RoleId.Spy:
                Spy.spy = player;
                break;

            case RoleId.SecurityGuard:
                SecurityGuard.securityGuard = player;
                SecurityGuard.remainingScrews = SecurityGuard.totalScrews;
                SecurityGuard.placedCameras = 0;
                SecurityGuard.charges = SecurityGuard.maxCharges;
                SecurityGuard.rechargedTasks = SecurityGuard.rechargeTasksNumber;
                SecurityGuard.ventTarget = null;
                SecurityGuard.minigame = null;
                break;

            case RoleId.Alchemyst:
                Alchemyst.Player = player;
                Alchemyst.target = null;
                Alchemyst.soulTarget = null;
                Alchemyst.deadBodies = new List<Tuple<Alchemyst.DeadPlayer, Vector3>>();
                Alchemyst.futureDeadBodies = new List<Tuple<Alchemyst.DeadPlayer, Vector3>>();
                Alchemyst.souls = new List<SpriteRenderer>();
                Alchemyst.meetingStartTime = DateTime.UtcNow;
                Alchemyst.skillUseCount = 0;
                Alchemyst.canUseKill = false;
                Alchemyst.CurrentTarget = null;
                break;

            case RoleId.Trapper:
                Trapper.trapper = player;
                Trapper.charges = Trapper.maxCharges;
                Trapper.rechargedTasks = Trapper.rechargeTasksNumber;
                Trapper.playersOnMap = new List<PlayerControl>();
                break;

            case RoleId.Redemptor:
                Redemptor.Player = player;
                Redemptor.target = null;
                Redemptor.arrow = null;
                Redemptor.Revelating = false;
                Redemptor.Prayering = false;
                Redemptor.RevivedPlayer = null;
                Redemptor.text = null;
                break;

            case RoleId.Jailor:
                Jailor.Player = player;
                Jailor.currentTarget = null;
                Jailor.Jailed = null;
                Jailor.usesCount = CustomOptionHolder.jailorUseCount.GetInt();
                break;

            case RoleId.Dreamcatcher:
                Dreamcatcher.Player = player;
                Dreamcatcher.CurrentTarget = null;
                Dreamcatcher.Dreamed = null;
                Dreamcatcher.LastDreamed = null;
                Dreamcatcher.ShieldUsed = false;
                break;

            case RoleId.Oracle:
                Oracle.Player = player;
                Oracle.CurrentTarget = null;
                Oracle.Confesser = null;
                Oracle.ConfesserType = Oracle.CRoleType.None;
                break;

            case RoleId.TrapperPlus:
                TrapperPlus.Player = player;
                TrapperPlus.ResetOwnerState(player, true);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(role), role, "Unsupported Imitator role");
        }
    }

    private static void InitializeSheriff(PlayerControl player, bool deputy)
    {
        // StaticSnapshot is intentionally shallow, so every mutable Sheriff
        // collection must be replaced before the borrowed role can mutate it.
        // This keeps the original holder's lists byte-for-byte restorable.
        Sheriff.Player = new List<PlayerControl>(Sheriff.Player ?? new List<PlayerControl>());
        Sheriff.handcuffedPlayers = deputy
            ? new List<byte>()
            : new List<byte>(Sheriff.handcuffedPlayers ?? new List<byte>());
        Sheriff.handcuffedKnows = deputy
            ? new Dictionary<byte, float>()
            : new Dictionary<byte, float>(Sheriff.handcuffedKnows ?? new Dictionary<byte, float>());

        // Both variants are authorized directly by Imitator.IsImitating in
        // Buttons.cs. Never add the borrower to Sheriff.Player or Deputy: doing
        // so would grant both skills and would make deputyCheckPromotion treat an
        // imitation as a canonical, living Sheriff/Deputy.
        Sheriff.Player.RemoveAll(existing => existing?.PlayerId == player.PlayerId);

        Sheriff.currentTarget = null;
        if (deputy)
            Sheriff.remainingHandcuffs = CustomOptionHolder.deputyNumberOfHandcuffs.GetFloat();
    }

    private static SpriteRenderer CreateLocalSoul(string name, Vector3 position, Sprite sprite)
    {
        if (sprite == null)
            return null;

        var soul = new GameObject(name) { layer = 5 };
        soul.transform.position = new Vector3(position.x, position.y, (position.y / 1000f) - 1f);
        soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        var renderer = soul.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        return renderer;
    }

    private static void DestroyArrows(IEnumerable<Arrow> arrows)
    {
        if (arrows == null)
            return;
        foreach (var arrow in arrows.ToArray())
            arrow?.arrow?.Destroy();
    }

    private static void ShowCarriedJail(MeetingHud meeting, PlayerControl owner, PlayerControl jailed, int uses)
    {
        if (!CanUseCarriedJail(owner, jailed))
            return;

        if (PlayerControl.LocalPlayer == jailed)
        {
            HudManager.Instance?.Chat?.AddChat(jailed, GetString("Jailor.JailedChat"));
            Coroutines.Start(Jailor.ShowJailed());
        }
        else if (PlayerControl.LocalPlayer == owner)
        {
            HudManager.Instance?.Chat?.AddChat(owner, GetString("Jailor.JailorChat"));
        }

        var voteArea = meeting.playerStates?.FirstOrDefault(area => area.TargetPlayerId == jailed.PlayerId);
        var template = voteArea?.Buttons?.transform?.Find("CancelButton")?.gameObject;
        if (voteArea == null || template == null)
            return;

        var cell = UObject.Instantiate(template, voteArea.transform);
        cell.name = "ImitatorJailCell";
        cell.transform.localPosition = new Vector3(-0.95f, 0.01f, -2f);
        cell.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        cell.transform.parent = template.transform.parent.parent;
        cell.GetComponent<SpriteRenderer>().sprite = Jailor.jailedSprite;
        cell.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();

        if (!owner.AmOwner || uses <= 0 || !CanExecuteCarriedJail(owner, jailed))
            return;

        var targetButton = UObject.Instantiate(template, voteArea.transform);
        targetButton.name = "ImitatorJailTargetIcon";
        targetButton.transform.localPosition = new Vector3(1f, 0.03f, -1f);
        targetButton.transform.parent = template.transform.parent.parent;
        var renderer = targetButton.GetComponent<SpriteRenderer>();
        renderer.sprite = Jailor.TargetSprite;
        renderer.color = Color.red;
        var passive = targetButton.GetComponent<PassiveButton>();
        passive.OnClick.RemoveAllListeners();
        passive.OnClick.AddListener((Action)(() =>
        {
            if (meeting.state is MeetingHud.VoteStates.Results or MeetingHud.VoteStates.Discussion ||
                !CanExecuteCarriedJail(owner, jailed))
                return;

            var writer = StartRPC(CustomRPC.ExiledJailed);
            writer.Write(owner.PlayerId);
            writer.Write(jailed.PlayerId);
            writer.EndRPC();
            Jailor.ExiledJailed(owner, jailed);
            targetButton?.Destroy();
            cell?.Destroy();
        }));
    }

    private static bool CanUseCarriedJail(PlayerControl owner, PlayerControl jailed)
    {
        return owner.IsAlive() && jailed.IsAlive() && owner.CanUseMeetingAbility() &&
               Blackmailer.blackmailed != owner && !Gaoler.IsPrisoner(owner);
    }
}
