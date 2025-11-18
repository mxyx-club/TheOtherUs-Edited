using AmongUs.QuickChat;
using TheOtherRoles.Objects;
using UnityEngine.UI;
using static MeetingHud;
using static TheOtherRoles.Options.ModOption;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
internal class MeetingHudPatch
{
    public static int MeetingCount;
    private static GameData.PlayerInfo StartMeetingPlayer;
    public static bool shookAlready;


    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHudStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            Message("Start", "Meeting");
            shookAlready = false;
            MeetingCount++;

            if (PlayerControl.LocalPlayer.IsDead()) CanSeeGhostInfo = true;

            CustomRoleManager.AllActiveRoles.Values.Do(x => x.OnMeetingStart(__instance));
            CustomRoleManager.AllActiveModifier.Values.SelectMany(x => x).Do(x => x.OnMeetingStart(__instance));

            // Remove first kill shield
            if (!PlayerControl.AllPlayerControls.ToList().All(x => x.IsAlive())) firstKillPlayer = null;

            var LightColorSprite = new ResourceSprite("ColorLight.png", 75);
            var DarkColorSprite = new ResourceSprite("ColorDark.png", 75);

            foreach (var pva in __instance.playerStates)
            {
                var player = PlayerById(pva.TargetPlayerId);
                if (player == null || player.Data == null) continue;
                var isLightColor = IsLightColor(player);

                var colorButton = pva.Buttons.transform.GetChild(0).gameObject;
                var newButton = UObject.Instantiate(colorButton, pva.transform);
                var renderer = newButton.GetComponent<SpriteRenderer>();

                renderer.sprite = isLightColor ? LightColorSprite : DarkColorSprite;

                newButton.transform.position = colorButton.transform.position - new Vector3(-0.85f, 0.16f, -2f);
                newButton.layer = 5;
                newButton.name = "ColorIcon";
                newButton.transform.parent = colorButton.transform.parent.parent;
                newButton.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.OnDestroy))]
    public class MeetingHudEnd
    {
        private static void Postfix(MeetingHud __instance)
        {
            Message("Destroy", "Meeting");
            CustomObject.EndMeeting();
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
    private class MeetingCalculateVotesPatch
    {
        private static PlayerVoteArea swapped1;
        private static PlayerVoteArea swapped2;

        public static bool CheckVoted(PlayerVoteArea playerVoteArea)
        {
            if (playerVoteArea.AmDead || playerVoteArea.DidVote)
                return false;

            var playerInfo = GameData.Instance.GetPlayerById(playerVoteArea.TargetPlayerId);
            if (playerInfo == null)
                return false;

            return true;
        }

        private static Dictionary<byte, float> CalculateVotes(MeetingHud __instance)
        {
            var dictionary = new Dictionary<byte, float>();

            foreach (var pva in __instance.playerStates)
            {
                if (pva.VotedFor is 252 or 255 or 254) continue;
                var player = PlayerById(pva.TargetPlayerId);
                if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected) continue;

                if (PlayerById(pva.TargetPlayerId).Is(RoleId.InfoSleuth))
                {
                    var writer = StartRPC(CustomRPC.InfoSleuthSetTarget);
                    writer.Write(pva.TargetPlayerId);
                    writer.Write(pva.VotedFor);
                    writer.EndRPC();
                    RPCProcedure.infoSleuthSetTarget(pva.TargetPlayerId, pva.VotedFor);
                }

                float additionalVotes = 1;

                foreach (var mayor in GetRoles<Mayor>().Where(x => x.Player.IsAlive()))
                {
                    if (pva.TargetPlayerId == mayor.PlayerId)
                        additionalVotes = mayor.Revealed ? Mayor.Vote : 1;
                }

                if (PlayerById(pva.TargetPlayerId).Is(RoleId.Tiebreaker))
                    additionalVotes += 0.5f;

                foreach (var pros in GetRoles(RoleId.Prosecutor).Cast<Prosecutor>().Where(x => x.Player.IsAlive() && x.ProsecuteThisMeeting))
                {
                    if (pva.TargetPlayerId == pros.PlayerId)
                        additionalVotes = 15;
                    else
                        additionalVotes = 0;
                }

                if (dictionary.TryGetValue(pva.VotedFor, out var currentVotes))
                    dictionary[pva.VotedFor] = currentVotes + additionalVotes;

                else
                    dictionary[pva.VotedFor] = additionalVotes;
            }

            // Swapper swap votes
            if (GetRoles(RoleId.Swapper).Any())
            {
                swapped1 = null;
                swapped2 = null;
                foreach (var playerVoteArea in __instance.playerStates)
                {
                    if (playerVoteArea.TargetPlayerId == Swapper.TargetId1) swapped1 = playerVoteArea;
                    if (playerVoteArea.TargetPlayerId == Swapper.TargetId2) swapped2 = playerVoteArea;
                }

                if (swapped1 == null || swapped2 == null) return dictionary;

                dictionary.TryAdd(swapped1.TargetPlayerId, 0);
                dictionary.TryAdd(swapped2.TargetPlayerId, 0);

                (dictionary[swapped1.TargetPlayerId], dictionary[swapped2.TargetPlayerId]) = (
                    dictionary[swapped2.TargetPlayerId], dictionary[swapped1.TargetPlayerId]);
            }
            return dictionary;
        }


        private static bool Prefix(MeetingHud __instance)
        {
            if (!__instance.playerStates.All(ps => ps.AmDead || ps.DidVote)) return false;
            // If skipping is disabled, replace skipps/no-votes with self vote
            if (StartMeetingPlayer == null && blockSkippingInEmergencyMeetings && noVoteIsSelfVote)
                foreach (var playerVoteArea in __instance.playerStates)
                    if (playerVoteArea.VotedFor == 254)
                        playerVoteArea.VotedFor = playerVoteArea.TargetPlayerId; // TargetPlayerId

            var self = CalculateVotes(__instance);
            //var max = self.MaxPair(out var tie);
            var exiled = PlayerControl.LocalPlayer.Data;
            bool tie = false;

            VoterState[] states;
            List<VoterState> statesList = new();

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];

                //バランサー処理
                if (Balancer.currentAbilityUser != null)
                {
                    if (PlayerById(playerVoteArea.TargetPlayerId) != null &&
                        playerVoteArea.VotedFor != Balancer.targetplayerright.PlayerId &&
                        playerVoteArea.VotedFor != Balancer.targetplayerleft.PlayerId)
                    {
                        playerVoteArea.VotedFor = Helpers.GetRandom((byte[])([Balancer.targetplayerright.PlayerId, Balancer.targetplayerleft.PlayerId]));
                    }
                }

                statesList.Add(new VoterState()
                {
                    VoterId = playerVoteArea.TargetPlayerId,
                    VotedForId = playerVoteArea.VotedFor
                });
            }

            states = statesList.ToArray();
            var VotingData = CalculateVotes(__instance);
            byte exileId = byte.MaxValue;
            float max1 = 0;
            foreach (var data in VotingData)
            {
                if (data.Value > max1)
                {
                    exileId = data.Key;
                    max1 = data.Value;
                    tie = false;
                }
                else if (data.Value == max1)
                {
                    exileId = byte.MaxValue;
                    tie = true;
                }

                exiled = GameData.Instance.AllPlayers.FirstOrDefault(info => !tie && info.PlayerId == exileId);

                if (tie && Balancer.currentAbilityUser != null)
                {
                    exiled = Balancer.targetplayerleft.Data;
                }
            }
            // RPCVotingComplete
            __instance.RpcVotingComplete(states, exiled, tie);
            return false;
        }
    }


    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.RpcVotingComplete))]
    public static class MeetingHudRpcVotingCompletePatch
    {
        public static void Prefix(MeetingHud __instance, Il2CppStructArray<VoterState> states, GameData.PlayerInfo exiled, bool tie)
        {
            Info($"exiled: {exiled?.PlayerName}, states: {states?.Count(x => x.VotedForId == exiled?.PlayerId)}, tie: {tie}", "RpcVotingComplete");
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    private class MeetingHudBloopAVoteIconPatch
    {
        public static bool Prefix(MeetingHud __instance, GameData.PlayerInfo voterPlayer, int index, Transform parent)
        {
            var spriteRenderer = UObject.Instantiate(__instance.PlayerVotePrefab);
            var showVoteColors = !GameManager.Instance.LogicOptions.GetAnonymousVotes() || CanSeeGhostInfo ||
                                 (PlayerControl.LocalPlayer.Is(RoleId.Prosecutor) && Prosecutor.canSeeVoteColors &&
                                  TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data).Item1 >=
                                  Prosecutor.tasksNeededToSeeVoteColors) ||
                                 PlayerControl.LocalPlayer.Is(RoleId.Watcher);
            if (showVoteColors && !Prosecutor.IsProsecuteMeeting)
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
            else
                PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);

            var transform = spriteRenderer.transform;
            transform.SetParent(parent);
            transform.localScale = Vector3.zero;
            var component = parent.GetComponent<PlayerVoteArea>();
            if (component != null) spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);

            __instance.StartCoroutine(Effects.Bloop(index * 0.3f, transform));
            parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);

            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
    private class MeetingHudPopulateVotesPatch
    {
        private static bool Prefix(MeetingHud __instance, Il2CppStructArray<VoterState> states)
        {
            // Swapper swap
            PlayerVoteArea swapped1 = null;
            PlayerVoteArea swapped2 = null;


            foreach (var playerVoteArea in __instance.playerStates)
            {
                if (playerVoteArea.TargetPlayerId == Swapper.TargetId1) swapped1 = playerVoteArea;
                if (playerVoteArea.TargetPlayerId == Swapper.TargetId2) swapped2 = playerVoteArea;
            }

            var doSwap = swapped1 != null && swapped2 != null && Swapper.currentAbilityUser.IsAlive();
            if (doSwap)
            {
                var localPosition = swapped1.transform.localPosition;
                __instance.StartCoroutine(Effects.Slide3D(swapped1.transform, localPosition, swapped2.transform.localPosition, 1.5f));
                __instance.StartCoroutine(Effects.Slide3D(swapped2.transform, swapped2.transform.localPosition, localPosition, 1.5f));
                Swapper.currentAbilityUser.GetRole<Swapper>().charges--;
            }

            __instance.TitleText.text = FastDestroyableSingleton<TranslationController>.Instance
                .GetString(StringNames.MeetingVotingResults, new Il2CppReferenceArray<Il2CppSystem.Object>(0));


            var allNums = new Dictionary<int, int>();

            var skipvote = 0;

            var isProsecuting = false;
            foreach (var pros in GetRoles<Prosecutor>())
            {
                if (pros.Player.Data.IsDead || pros.Player.Data.Disconnected) continue;
                if (pros.ProsecuteThisMeeting)
                {
                    isProsecuting = true;
                }
            }

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var pva = __instance.playerStates[i];
                var targetPlayerId = pva.TargetPlayerId;
                allNums.Add(i, 0);

                pva.ClearForResults();

                pva = doSwap switch
                {
                    // Swapper change playerVoteArea that gets the votes
                    true when pva.TargetPlayerId == swapped1.TargetPlayerId => swapped2,
                    true when pva.TargetPlayerId == swapped2.TargetPlayerId => swapped1,
                    _ => pva
                };

                foreach (var voterState in states)
                {
                    var playerById = GameData.Instance.GetPlayerById(voterState.VoterId);
                    var pros = playerById.Object.GetRole<Prosecutor>();
                    if (Prosecutor.IsProsecuteMeeting)
                    {
                        byte targetId = pva.TargetPlayerId;
                        if (doSwap)
                        {
                            if (pva.TargetPlayerId == swapped2.TargetPlayerId) targetId = swapped1.TargetPlayerId;
                            if (pva.TargetPlayerId == swapped1.TargetPlayerId) targetId = swapped2.TargetPlayerId;
                        }

                        if (voterState.VoterId == pros.Player.PlayerId && pros.ProsecuteThisMeeting)
                        {
                            if (playerById == null)
                            {
                                Error($"找不到投票者的玩家信息: {voterState.VoterId}");
                                pros.IsAbilityUsed = true;
                            }
                            else if (i == 0 && voterState.SkippedVote) // 显示检察官的跳过票数
                            {
                                for (var repeat = 0; repeat < 6; repeat++)
                                    __instance.BloopAVoteIcon(playerById, allNums[i], pva.transform);
                                pros.IsAbilityUsed = true;
                            }
                            else if (voterState.VotedForId == targetId) // 显示检察官的投票动画
                            {
                                for (var repeat = 0; repeat < 6; repeat++)
                                    __instance.BloopAVoteIcon(playerById, allNums[i], pva.transform);

                                allNums[i] += 6;
                                pros.IsAbilityUsed = true;
                            }
                        }
                    }
                }

                var playervote = 0;
                foreach (var voterState in states)
                {
                    if (isProsecuting) break;

                    var DataById = GameData.Instance.GetPlayerById(voterState.VoterId);
                    if (DataById == null)
                    {
                        Warn($"找不到投票者的玩家信息: {voterState.VoterId}");
                    }
                    else if (i == 0 && voterState.SkippedVote && !DataById.IsDead) // 显示跳过票数
                    {
                        __instance.BloopAVoteIcon(DataById, skipvote, __instance.SkippedVoting.transform);

                        if (Mayor.GetRevealed(PlayerById(voterState.VoterId)))
                            for (var repeat = 1; repeat < Mayor.Vote; repeat++)
                                __instance.BloopAVoteIcon(DataById, skipvote, __instance.SkippedVoting.transform);
                        skipvote++;
                    }
                    else if (voterState.VotedForId == targetPlayerId && !DataById.IsDead) // 显示玩家票数
                    {
                        __instance.BloopAVoteIcon(DataById, playervote, pva.transform);

                        if (Mayor.GetRevealed(PlayerById(voterState.VoterId)))
                            for (var repeat = 1; repeat < Mayor.Vote; repeat++)
                                __instance.BloopAVoteIcon(DataById, playervote, pva.transform);
                        playervote++;
                    }
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    private class MeetingHudVotingCompletedPatch
    {
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] byte[] states,
            [HarmonyArgument(1)] GameData.PlayerInfo exiled, [HarmonyArgument(2)] bool tie)
        {
            // Reset swapper values
            Camouflager.CamoTimer = 0f;

            __instance.exiledPlayer = __instance.wasTie ? null : __instance.exiledPlayer;
            var exiledString = exiled == null ? "null" : exiled.PlayerName;
            Message($"被驱逐玩家: {exiledString}");
            Message($"是否平票: {tie}");
        }

        private static void Prefix(MeetingHud __instance,
            [HarmonyArgument(0)] Il2CppStructArray<VoterState> states,
            [HarmonyArgument(1)] GameData.PlayerInfo exiled,
            [HarmonyArgument(2)] bool tie)
        {
            if (tie && Balancer.currentAbilityUser != null)
            {
                Balancer.IsDoubleExile = true;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
    private class PlayerVoteAreaSelectPatch
    {
        private static bool Prefix(MeetingHud __instance)
        {
            return !(PlayerControl.LocalPlayer != null && HandleGuesser.isGuesser(PlayerControl.LocalPlayer.PlayerId) &&
                     Guesser.guesserUI != null);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    private class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            // Count meetings
            if (meetingTarget == null) meetingsCount++;
            // Count meetings
            if (meetingTarget == null) meetingsCount++;
            // Save the meeting target
            StartMeetingPlayer = meetingTarget;
            isRoundOne = false;

            // Remove revealed traps
            Trap.clearRevealedTraps();

            // Reset zoomed out ghosts
            toggleZoom(true);

            // Stop all playing sounds
            SoundEffectsManager.stopAll();

            // Close In-Game Settings Display if open
            HudManagerUpdate.CloseSettings();
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    private class MeetingHudUpdatePatch
    {
        private static VoteStates lastState;

        private static void Prefix(MeetingHud __instance)
        {
            lastState = __instance.state;
        }

        private static void Postfix(MeetingHud __instance)
        {
            if (__instance.state == VoteStates.NotVoted && __instance.state != lastState && lastState == VoteStates.Discussion)
            {
                __instance.discussionTimer += GetPenaltyVotingTime();
            }

            CustomRoleManager.AllActiveRoles.Values.Do(x => x.OnMeetingUpdate(__instance));
            CustomRoleManager.AllActiveModifier.Values.SelectMany(x => x).Do(x => x.OnMeetingUpdate(__instance));

            // Deactivate skip Button if skipping on emergency meetings is disabled
            if (StartMeetingPlayer == null && blockSkippingInEmergencyMeetings)
                __instance.SkipVoteButton.gameObject.SetActive(false);

            updateMeetingText(__instance);
        }
    }

    //private static float _lastUpdateTime;
    public static void updateMeetingText(MeetingHud __instance)
    {
        if (PlayerControl.LocalPlayer.IsDead()) return;

        if (Instance.state is not VoteStates.Voted and not VoteStates.NotVoted and not VoteStates.Discussion) return;

        //if (Time.time - _lastUpdateTime < 1f) return;
        //_lastUpdateTime = Time.time;

        List<string> meetingInfoText = new();
        bool flag = false;
        var text = PlayerControl.LocalPlayer.GetRoleBase()?.UpdateMeetingVoteText(__instance) ?? "";
        if (!text.IsNullOrWhiteSpace())
        {
            flag = true;
            meetingInfoText.Add(PlayerControl.LocalPlayer.GetRoleBase()?.UpdateMeetingVoteText(__instance) ?? "");
        }

        PlayerControl.LocalPlayer.GetModifierBase()?.Do((x) =>
        {
            flag = true;
            var text = x.UpdateMeetingVoteText(__instance);
            if (!text.IsNullOrWhiteSpace()) meetingInfoText.Add(text);
        });

        if (flag) __instance.TimerText.text = $"{string.Join(" | ", meetingInfoText)}\n{__instance.TimerText.text}";
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
    private class MeetingHudClearVotePatch
    {
        private static void Postfix(MeetingHud __instance)
        {
            CustomRoleManager.AllActiveRoles.Values.Do(x => x.ClearVotes(__instance));
        }
    }


    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    private class MeetingChatNotification
    {
        private static void Postfix(MeetingHud __instance)
        {
            /*var chat = FastDestroyableSingleton<HudManager>.Instance.Chat;
            var local = PlayerControl.LocalPlayer;
            var num = (int)chat.timeSinceLastMessage;
            foreach (var p in PlayerControl.AllPlayerControls)
            {
                var player = p;
                if (player != local || player.Data.IsDead || num != 0) continue;
                var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.SetMeetingChatOverlay);
                writer.Write(player.PlayerId);
                writer.Write(local.PlayerId);
                writer.EndRPC();
                RPCProcedure.setChatNotificationOverlay(local.PlayerId, player.PlayerId);
                break;
            }*/
        }
    }

    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
    public class BlockChatBlackmailed
    {
        public static bool Prefix(QuickChatMenu __instance)
        {
            var blackmailed = CustomRoleManager.AllActiveRoles.Values.Where(x => x.RoleId == RoleId.Blackmailer).Cast<Blackmailer>();
            foreach (var role in blackmailed)
            {
                if (role.Player != null && role.blackmailed != null && role.blackmailed == PlayerControl.LocalPlayer)
                {
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch]
    public class ShowHost
    {
        public static TextMeshPro Text;
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        public static void Setup(MeetingHud __instance)
        {
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame || Balancer.currentAbilityUser != null) return;
            __instance.ProceedButton.gameObject.transform.localPosition = new(-2.5f, 2.2f, 0);
            __instance.ProceedButton.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            __instance.ProceedButton.GetComponent<PassiveButton>().enabled = false;
            __instance.HostIcon.gameObject.SetActive(true);
            __instance.ProceedButton.gameObject.SetActive(true);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update)), HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (Balancer.currentAbilityUser != null) return;
            var host = GameData.Instance.GetHost();

            if (host != null)
            {
                PlayerMaterial.SetColors(host.DefaultOutfit.ColorId, __instance.HostIcon);
                if (Text == null) Text = __instance.ProceedButton.gameObject.GetComponentInChildren<TextMeshPro>();
                Text.text = $"{"Host".Translate()}: {host.PlayerName}";
            }
        }
    }
}