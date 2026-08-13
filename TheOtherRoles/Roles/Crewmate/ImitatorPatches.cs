namespace TheOtherRoles.Roles.Crewmate;

[HarmonyPatch]
public static class ImitatorPatches
{
    private static readonly Dictionary<byte, GameObject> selectionButtons = new();
    private static byte pendingTargetId = byte.MaxValue;
    private static RoleId pendingRole = RoleId.DefaultRole;
    private static bool selectionSentThisMeeting;

    public static void NotifyTargetInvalidated(byte targetId)
    {
        if (pendingTargetId != targetId)
            return;
        pendingTargetId = byte.MaxValue;
        pendingRole = RoleId.DefaultRole;
        if (selectionButtons.TryGetValue(targetId, out var button))
            button?.SetActive(false);
    }

    [HarmonyPatch(typeof(RoleHelpers), nameof(RoleHelpers.ResetRoleSelection))]
    private static class GlobalRoleResetPatch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.First)]
        private static void Prefix()
        {
            if (Imitator.Player != null && Imitator.IsActive)
                Imitator.Abort(Imitator.Player.PlayerId);
        }
    }

    [HarmonyPatch(typeof(RPCProcedure), nameof(RPCProcedure.erasePlayerRoles))]
    private static class EraseRolePatch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.First)]
        private static void Prefix(byte playerId)
        {
            Imitator.NotifyPlayerStateChanged(playerId);
            if (Imitator.Player?.PlayerId == playerId && Imitator.IsActive)
                Imitator.Abort(playerId);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    private static class MeetingStartPatch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.First)]
        private static void Prefix()
        {
            Imitator.BeginMeeting();
            pendingTargetId = byte.MaxValue;
            pendingRole = RoleId.DefaultRole;
            selectionSentThisMeeting = false;
            selectionButtons.Clear();

            if (!Imitator.HasAnyActiveSession)
                return;

            // Restore deterministically on every client. The owner also sends the
            // explicit RPC so a client entering the meeting a frame later converges.
            foreach (var holder in PlayerControl.AllPlayerControls.ToArray().Where(player => player != null && Imitator.IsActiveFor(player.PlayerId)))
            {
                if (holder.AmOwner)
                {
                    var writer = StartRPC(CustomRPC.EndImitation);
                    writer.Write(holder.PlayerId);
                    Imitator.TryGetSessionGeneration(holder, Imitator.GetEffectiveRole(holder), out var generation);
                    writer.Write((int)generation);
                    writer.EndRPC();
                }
                Imitator.End(holder.PlayerId);
            }
        }

        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        private static void Postfix(MeetingHud __instance)
        {
            Imitator.ShowMeetingCarry(__instance);
            AddSelectionButtons(__instance);
        }
    }

    // Some game versions finish building playerStates in ServerStart rather than
    // MeetingHud.Start. The dictionary guard prevents duplicate buttons.
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ServerStart))]
    private static class MeetingServerStartPatch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        private static void Postfix(MeetingHud __instance) => AddSelectionButtons(__instance);
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Deserialize))]
    private static class MeetingDeserializePatch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(1)] bool initialState)
        {
            if (initialState)
                AddSelectionButtons(__instance);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    private static class VotingCompletePatch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        private static void Postfix()
        {
            if (selectionSentThisMeeting)
                return;

            var local = PlayerControl.LocalPlayer;
            if (local == null || Imitator.Player != local || local.IsDead())
                return;

            selectionSentThisMeeting = true;

            var targetId = pendingTargetId;
            var role = pendingRole;
            var target = PlayerById(targetId);
            if (targetId == byte.MaxValue || target == null || target.Data == null ||
                !target.Data.IsDead || target.Data.Disconnected || Imitator.GetEligibleRole(target) != role)
            {
                targetId = byte.MaxValue;
                role = RoleId.DefaultRole;
            }

            var writer = StartRPC(CustomRPC.SetImitatorTarget, HostPlayer);
            writer.Write((byte)0); // owner request; only the host may canonicalize it
            writer.Write(local.PlayerId);
            writer.Write(targetId);
            writer.Write((byte)role);
            writer.Write((int)Imitator.CurrentMeetingGeneration);
            writer.EndRPC();

            if (AmongUsClient.Instance.AmHost &&
                Imitator.TryHostAuthorizeSelection(local, local.PlayerId, targetId, role,
                    Imitator.CurrentMeetingGeneration, out var canonicalTarget, out var canonicalRole))
                BroadcastCanonicalSelection(local.PlayerId, canonicalTarget, canonicalRole,
                    Imitator.CurrentMeetingGeneration);

            foreach (var button in selectionButtons.Values)
                button?.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.OnDestroy))]
    private static class MeetingDestroyPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            selectionButtons.Clear();
            Imitator.ClearMeetingCarries();
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    private static class ExileWrapUpPatch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        private static void Postfix() => TryHostStartImitations();
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    private static class AirshipWrapUpPatch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        private static void Postfix() => TryHostStartImitations();
    }

    [HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(GameObject))]
    private static class SubmergedWrapUpPatch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.Last)]
        private static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.IsSubmerged || obj?.name?.Contains("ExileCutscene") != true)
                return;
            TryHostStartImitations();
        }
    }

    private static void AddSelectionButtons(MeetingHud meeting)
    {
        var local = PlayerControl.LocalPlayer;
        if (meeting == null || local == null || Imitator.Player != local || local.IsDead() ||
            PlayerData.GetPlayerData(local)?.MainRole != RoleId.Imitator)
            return;

        foreach (var voteArea in meeting.playerStates ?? Enumerable.Empty<PlayerVoteArea>())
        {
            var target = PlayerById(voteArea.TargetPlayerId);
            var role = Imitator.GetEligibleRole(target);
            if (!Imitator.IsCompatible(role) || selectionButtons.ContainsKey(voteArea.TargetPlayerId))
                continue;

            var template = voteArea.Buttons?.transform?.Find("CancelButton")?.gameObject;
            template ??= voteArea.Buttons?.transform?.childCount > 0
                ? voteArea.Buttons.transform.GetChild(0).gameObject
                : null;
            if (template == null)
                continue;

            var targetId = target.PlayerId;
            var buttonObject = UObject.Instantiate(template, voteArea.transform);
            buttonObject.name = "ImitatorSelectButton";
            buttonObject.layer = 5;
            buttonObject.transform.localScale = template.transform.localScale * 0.8f;
            buttonObject.transform.localPosition = template.transform.localPosition + new Vector3(-0.75f, 0f, -1f);
            buttonObject.transform.parent = template.transform.parent.parent;

            var renderer = buttonObject.GetComponent<SpriteRenderer>();
            renderer.sprite = Imitator.selectSprite;

            var passive = buttonObject.GetComponent<PassiveButton>();
            passive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            passive.OnClick.AddListener((Action)(() => ToggleSelection(meeting, targetId, role)));

            selectionButtons[targetId] = buttonObject;
        }
    }

    private static void ToggleSelection(MeetingHud meeting, byte targetId, RoleId role)
    {
        if (meeting == null || meeting.state == MeetingHud.VoteStates.Results)
            return;

        if (pendingTargetId == targetId)
        {
            pendingTargetId = byte.MaxValue;
            pendingRole = RoleId.DefaultRole;
        }
        else
        {
            var target = PlayerById(targetId);
            if (Imitator.GetEligibleRole(target) != role)
                return;
            pendingTargetId = targetId;
            pendingRole = role;
        }

        foreach (var (playerId, button) in selectionButtons)
        {
            var renderer = button?.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.sprite = playerId == pendingTargetId ? Imitator.deselectSprite : Imitator.selectSprite;
        }
    }

    public static void HandleSelectionRpc(PlayerControl sender, MessageReader reader)
    {
        var phase = reader.ReadByte();
        var imitatorId = reader.ReadByte();
        var targetId = reader.ReadByte();
        var role = (RoleId)reader.ReadByte();
        var generation = (uint)reader.ReadInt32();

        if (phase == 0)
        {
            if (!AmongUsClient.Instance.AmHost ||
                !Imitator.TryHostAuthorizeSelection(sender, imitatorId, targetId, role, generation,
                    out var canonicalTarget, out var canonicalRole))
                return;
            BroadcastCanonicalSelection(imitatorId, canonicalTarget, canonicalRole, generation);
            return;
        }

        if (phase == 1 && sender == HostPlayer)
            Imitator.ApplyHostSelection(imitatorId, targetId, role, generation);
    }

    private static void BroadcastCanonicalSelection(byte imitatorId, byte targetId, RoleId role, uint generation)
    {
        var writer = StartRPC(HostPlayer, CustomRPC.SetImitatorTarget);
        writer.Write((byte)1);
        writer.Write(imitatorId);
        writer.Write(targetId);
        writer.Write((byte)role);
        writer.Write((int)generation);
        writer.EndRPC();
        Imitator.ApplyHostSelection(imitatorId, targetId, role, generation);
    }

    private static void TryHostStartImitations()
    {
        if (AmongUsClient.Instance?.AmHost != true)
            return;

        foreach (var start in Imitator.GetAuthorizedStartsForHost())
        {
            var writer = StartRPC(HostPlayer, CustomRPC.StartImitation);
            writer.Write(start.ImitatorId);
            writer.Write((byte)start.Role);
            writer.Write((int)start.MeetingGeneration);
            writer.EndRPC();
            Imitator.StartAuthorized(start.ImitatorId, start.Role, start.MeetingGeneration);
        }
    }
}
