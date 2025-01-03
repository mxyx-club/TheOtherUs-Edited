using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.QuickChat;
using Hazel;
using Reactor.Utilities;
using TheOtherRoles.Buttons;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static MeetingHud;
using static TheOtherRoles.Options.ModOption;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
internal class MeetingHudPatch
{
    private static bool[] selections;
    private static SpriteRenderer[] renderers;
    private static NetworkedPlayerInfo target;
    private static PassiveButton[] swapperButtonList;
    private static TextMeshPro meetingExtraButtonLabel;
    public static GameObject MeetingExtraButton;
    public static bool shookAlready;
    private static PlayerVoteArea swapped1;
    private static PlayerVoteArea swapped2;

    private static void swapperOnClick(int i, MeetingHud __instance)
    {
        if (Swapper.charges <= 0 || __instance.state == VoteStates.Results || __instance.playerStates[i].AmDead) return;

        var selectedCount = selections.Count(b => b);
        var renderer = renderers[i];

        byte firstPlayer = byte.MaxValue;
        byte secondPlayer = byte.MaxValue;

        switch (selectedCount)
        {
            case 0:
                renderer.color = Color.green;
                selections[i] = true;
                firstPlayer = __instance.playerStates[i].TargetPlayerId;
                break;
            case 1:
                if (selections[i])
                {
                    renderer.color = Color.red;
                    selections[i] = false;
                    firstPlayer = byte.MaxValue;
                }
                else
                {
                    selections[i] = true;
                    renderer.color = Color.green;

                    for (int A = 0; A < selections.Length; A++)
                    {
                        if (selections[A])
                        {
                            if (firstPlayer != byte.MaxValue)
                            {
                                secondPlayer = __instance.playerStates[A].TargetPlayerId;
                                break;
                            }
                            else
                            {
                                firstPlayer = __instance.playerStates[A].TargetPlayerId;
                            }
                        }
                    }
                }
                break;
            case 2 when !selections[i]:
                //firstPlayer = byte.MaxValue;
                return;
            case 2:
                renderer.color = Color.red;
                selections[i] = false;
                if (__instance.playerStates[i].TargetPlayerId == firstPlayer) firstPlayer = byte.MaxValue;
                else if (__instance.playerStates[i].TargetPlayerId == secondPlayer) secondPlayer = byte.MaxValue;
                break;
        }

        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
            (byte)CustomRPC.SwapperSwap, SendOption.Reliable, -1);
        writer.Write(firstPlayer);
        writer.Write(secondPlayer);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.swapperSwap(firstPlayer, secondPlayer);
    }
    public static void swapperCheckAndReturnSwap(MeetingHud __instance, byte dyingPlayerId)
    {
        // someone was guessed or dced in the meeting, check if this affects the swapper.
        if (Swapper.swapper == null || __instance.state == VoteStates.Results) return;

        // reset swap.
        var reset = false;
        if (dyingPlayerId == Swapper.playerId1 || dyingPlayerId == Swapper.playerId2 || dyingPlayerId == byte.MaxValue - 1)
        {
            reset = true;
            Swapper.playerId1 = Swapper.playerId2 = byte.MaxValue;
        }

        // Only for the swapper: Reset all the buttons and charges value to their original state.
        if (CachedPlayer.LocalPlayer.PlayerControl != Swapper.swapper) return;

        // check if dying player was a selected player (but not confirmed yet)
        for (var i = 0; i < __instance.playerStates.Count; i++)
        {
            reset = reset || (selections[i] && __instance.playerStates[i].TargetPlayerId == dyingPlayerId);
            if (reset) break;
        }

        if (!reset) return;


        for (var i = 0; i < selections.Length; i++)
        {
            selections[i] = false;
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead ||
                (playerVoteArea.TargetPlayerId == Swapper.swapper.PlayerId && Swapper.canOnlySwapOthers)) continue;
            renderers[i].color = Color.red;
            var copyI = i;
            swapperButtonList[i].OnClick.RemoveAllListeners();
            swapperButtonList[i].OnClick.AddListener((Action)(() => swapperOnClick(copyI, __instance)));
        }
    }

    private static void mayorToggleVoteTwice(MeetingHud __instance)
    {
        __instance.playerStates[0].Cancel(); // This will stop the underlying buttons of the template from showing up
        if (__instance.state == VoteStates.Results || Mayor.mayor.Data.IsDead) return;

        Mayor.Revealed = true;

        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
            (byte)CustomRPC.MayorRevealed, SendOption.Reliable);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        Object.Destroy(MeetingExtraButton);
    }

    private static void populateButtonsPostfix(MeetingHud __instance)
    {
        // Add Swapper Buttons
        var addSwapperButtons = Swapper.swapper != null && CachedPlayer.LocalPlayer.PlayerControl == Swapper.swapper &&
                                !Swapper.swapper.Data.IsDead;
        var addMayorButton = Mayor.mayor != null && CachedPlayer.LocalPlayer.PlayerControl == Mayor.mayor &&
                             !Mayor.mayor.Data.IsDead && !Mayor.Revealed;
        if (addSwapperButtons)
        {
            selections = new bool[__instance.playerStates.Length];
            renderers = new SpriteRenderer[__instance.playerStates.Length];
            swapperButtonList = new PassiveButton[__instance.playerStates.Length];

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                if (playerVoteArea.AmDead || (playerVoteArea.TargetPlayerId == Swapper.swapper.PlayerId && Swapper.canOnlySwapOthers))
                    continue;

                var template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                var checkbox = Object.Instantiate(template, playerVoteArea.transform, true);
                checkbox.transform.position = template.transform.position;
                checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                if ((HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId))
                    || (Mimic.mimic?.PlayerId == CachedPlayer.LocalPlayer.PlayerId))
                    checkbox.transform.localPosition = new Vector3(-0.5f, 0.03f, -1.3f);
                var renderer = checkbox.GetComponent<SpriteRenderer>();
                renderer.sprite = Swapper.spriteCheck;
                renderer.color = Color.red;

                if (Swapper.charges <= 0) renderer.color = Color.gray;

                var button = checkbox.GetComponent<PassiveButton>();
                swapperButtonList[i] = button;
                button.OnClick.RemoveAllListeners();
                var copiedIndex = i;
                button.OnClick.AddListener((Action)(() => swapperOnClick(copiedIndex, __instance)));

                selections[i] = false;
                renderers[i] = renderer;
            }
        }

        // Add meeting extra button, i.e. Swapper Confirm Button or Mayor Toggle Double Vote Button. Swapper Button uses ExtraButtonText on the Left of the Button. (Future meeting buttons can easily be added here)
        if (addMayorButton)
        {
            var meetingUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var textTemplate = __instance.playerStates[0].NameText;
            var meetingExtraButtonParent = new GameObject().transform;
            meetingExtraButtonParent.SetParent(meetingUI);
            var meetingExtraButton = Object.Instantiate(buttonTemplate, meetingExtraButtonParent);
            MeetingExtraButton = meetingExtraButton.gameObject;

            var meetingExtraButtonMask = Object.Instantiate(maskTemplate, meetingExtraButtonParent);
            meetingExtraButtonLabel = Object.Instantiate(textTemplate, meetingExtraButton);
            meetingExtraButton.GetComponent<SpriteRenderer>().sprite =
                ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

            meetingExtraButtonParent.localPosition = new Vector3(0, -2.225f, -5);
            meetingExtraButtonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
            meetingExtraButtonLabel.alignment = TextAlignmentOptions.Center;
            meetingExtraButtonLabel.transform.localPosition =
                new Vector3(0, 0, meetingExtraButtonLabel.transform.localPosition.z);

            var localScale = meetingExtraButtonLabel.transform.localScale;
            localScale = new Vector3(
                localScale.x * 1.5f,
                localScale.x * 1.7f,
                localScale.x * 1.7f);
            meetingExtraButtonLabel.transform.localScale = localScale;
            meetingExtraButtonLabel.text = cs(Mayor.color, "揭示身份");

            var passiveButton = meetingExtraButton.GetComponent<PassiveButton>();
            passiveButton.OnClick.RemoveAllListeners();
            if (!CachedPlayer.LocalPlayer.Data.IsDead && addMayorButton)
                passiveButton.OnClick.AddListener((Action)(() => mayorToggleVoteTwice(__instance)));

            meetingExtraButton.parent.gameObject.SetActive(false);
            __instance.StartCoroutine(Effects.Lerp(7.27f, new Action<float>(p =>
            {
                // Button appears delayed, so that its visible in the voting screen only!
                if ((int)p == 1) meetingExtraButton.parent.gameObject.SetActive(true);
            })));
        }

        var isGuesser = HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId);

        // Add overlay for spelled players
        if (Witch.witch != null && Witch.futureSpelled != null)
        {
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (Witch.futureSpelled.Any(x => x.PlayerId == pva.TargetPlayerId))
                {
                    var local = CachedPlayer.LocalPlayer.PlayerControl;
                    var rend = new GameObject().AddComponent<SpriteRenderer>();
                    rend.transform.SetParent(pva.transform);
                    rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                    rend.transform.localPosition = new Vector3(-0.5f, -0.03f, -1f);
                    if (local == Swapper.swapper && (isGuesser || Swapper.swapper.PlayerId == Mimic.mimic.PlayerId)) rend.transform.localPosition = new Vector3(-0.725f, -0.15f, -1f);
                    rend.sprite = Witch.spelledOverlaySprite;
                }
            }
        }

        // Add Guesser Buttons
        var GuesserRemainingShots = HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId);
        if (!isGuesser || CachedPlayer.LocalPlayer.IsDead || GuesserRemainingShots <= 0 ||
            (PlayerControl.LocalPlayer == WolfLord.Player && WolfLord.Revealed)) return;
        {
            Doomsayer.CanShoot = true;
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];

                if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == CachedPlayer.LocalPlayer.PlayerId) continue;

                if (!Eraser.canEraseGuess && CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.PlayerControl == Eraser.eraser
                    && Eraser.alreadyErased.Contains(playerVoteArea.TargetPlayerId)) continue;

                var template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                var targetBox = Object.Instantiate(template, playerVoteArea.transform);
                targetBox.name = "ShootButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                var renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = HandleGuesser.targetSprite;
                var button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                var copiedIndex = i;
                button.OnClick.AddListener((Action)(() => Guesser.guesserOnClick(copiedIndex, __instance)));
            }
        }
    }


    public static void updateMeetingText(MeetingHud __instance)
    {
        if (PlayerControl.LocalPlayer.Data.IsDead) return;

        if (Instance.state is not VoteStates.Voted and not VoteStates.NotVoted and not VoteStates.Discussion)
            return;

        var meetingInfoText = "";
        int numGuesses = HandleGuesser.isGuesser(PlayerControl.LocalPlayer.PlayerId)
            ? HandleGuesser.remainingShots(PlayerControl.LocalPlayer.PlayerId) : 0;
        if (numGuesses > 0)
        {
            meetingInfoText = string.Format(GetString("guesserGuessesLeft"), numGuesses);
        }

        if (PlayerControl.LocalPlayer == Akujo.akujo && (Akujo.honmei == null || Akujo.keeps.Count < 1) && Akujo.timeLeft > 0)
        {
            meetingInfoText = string.Format(GetString("akujoTimeRemaining"), $"{TimeSpan.FromSeconds(Akujo.timeLeft):mm\\:ss}");
        }
        else if (PlayerControl.LocalPlayer == Doomsayer.doomsayer)
        {
            meetingInfoText = string.Format(GetString("DoomsayerKilledToWin"), Doomsayer.killToWin - Doomsayer.killedToWin);
        }
        else if (PlayerControl.LocalPlayer == Swapper.swapper)
        {
            meetingInfoText = string.Format(GetString("SwapperCharges"), Swapper.charges);
        }
        else if (PlayerControl.LocalPlayer == PartTimer.partTimer && PartTimer.target == null)
        {
            meetingInfoText = string.Format(GetString("PartTimerMeetingInfo"), Swapper.charges);
        }
        else if (PlayerControl.LocalPlayer == Witness.Player)
        {
            if (Witness.timeLeft > 0 && Witness.killerTarget == null)
                meetingInfoText = string.Format(GetString("WitnessTimerLeft2"), $"{TimeSpan.FromSeconds(Witness.timeLeft):mm\\:ss}");
            else if (Witness.timeLeft > 0 && Witness.target == null)
                meetingInfoText = string.Format(GetString("WitnessTimerLeft"), $"{TimeSpan.FromSeconds(Witness.timeLeft):mm\\:ss}");
            else
                meetingInfoText = string.Format(GetString("WitnessWinLeft"), $"{Witness.exileToWin - Witness.exiledCount}");
        }

        if (meetingInfoText == "") return;
        __instance.TimerText.text = $"{meetingInfoText}\n" + __instance.TimerText.text;
    }

    [HarmonyPatch]
    public class ShowHost
    {
        public static TextMeshPro Text;
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        [HarmonyPostfix]
        public static void Setup(MeetingHud __instance)
        {
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame || Balancer.currentAbilityUser != null) return;
            __instance.ProceedButton.gameObject.transform.localPosition = new(-2.5f, 2.2f, 0);
            __instance.ProceedButton.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            __instance.ProceedButton.GetComponent<PassiveButton>().enabled = false;
            __instance.HostIcon.gameObject.SetActive(true);
            __instance.ProceedButton.gameObject.SetActive(true);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]
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

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
    private class MeetingCalculateVotesPatch
    {
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

            foreach (var playerVoteArea in __instance.playerStates)
            {
                if (playerVoteArea.VotedFor is 252 or 255 or 254) continue;
                var player = playerById(playerVoteArea.TargetPlayerId);
                if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected) continue;

                if (InfoSleuth.infoSleuth != null && playerVoteArea.TargetPlayerId == InfoSleuth.infoSleuth.PlayerId)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.InfoSleuthTarget, SendOption.Reliable);
                    writer.Write(playerVoteArea.VotedFor);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.infoSleuthTarget(playerVoteArea.VotedFor);
                }

                float additionalVotes = 1;
                if (Prosecutor.prosecutor != null && Prosecutor.prosecutor.PlayerId == playerVoteArea.TargetPlayerId)
                    additionalVotes = Prosecutor.ProsecuteThisMeeting ? 15 : 1;

                if (Mayor.mayor != null && Mayor.mayor.PlayerId == playerVoteArea.TargetPlayerId)
                    additionalVotes = Mayor.Revealed ? Mayor.Vote : 1;

                if (Tiebreaker.tiebreaker != null && Tiebreaker.tiebreaker.PlayerId == playerVoteArea.TargetPlayerId)
                    additionalVotes = 1.5f;

                if (Prosecutor.prosecutor != null && Prosecutor.ProsecuteThisMeeting && Prosecutor.prosecutor.PlayerId != playerVoteArea.TargetPlayerId)
                    additionalVotes = 0;

                if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var currentVotes))
                    dictionary[playerVoteArea.VotedFor] = currentVotes + additionalVotes;

                else
                    dictionary[playerVoteArea.VotedFor] = additionalVotes;
            }

            // Swapper swap votes
            if (Swapper.swapper == null || Swapper.swapper.Data.IsDead) return dictionary;
            {
                swapped1 = null;
                swapped2 = null;
                foreach (var playerVoteArea in __instance.playerStates)
                {
                    if (playerVoteArea.TargetPlayerId == Swapper.playerId1) swapped1 = playerVoteArea;
                    if (playerVoteArea.TargetPlayerId == Swapper.playerId2) swapped2 = playerVoteArea;
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
            if (target == null && blockSkippingInEmergencyMeetings && noVoteIsSelfVote)
                foreach (var playerVoteArea in __instance.playerStates)
                    if (playerVoteArea.VotedFor == 254)
                        playerVoteArea.VotedFor = playerVoteArea.TargetPlayerId; // TargetPlayerId

            var self = CalculateVotes(__instance);
            //var max = self.MaxPair(out var tie);
            var exiled = CachedPlayer.LocalPlayer.Data;
            bool tie = false;

            VoterState[] states;
            List<VoterState> statesList = new();

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];

                //バランサー処理
                if (Balancer.currentAbilityUser != null)
                {
                    if (playerVoteArea.VotedFor != Balancer.targetplayerright.PlayerId && playerVoteArea.VotedFor != Balancer.targetplayerleft.PlayerId)
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

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    private class MeetingHudBloopAVoteIconPatch
    {
        public static bool Prefix(MeetingHud __instance, NetworkedPlayerInfo voterPlayer, int index, Transform parent)
        {
            var spriteRenderer = Object.Instantiate(__instance.PlayerVotePrefab);
            var showVoteColors = !GameManager.Instance.LogicOptions.GetAnonymousVotes() || shouldShowGhostInfo() ||
                                 (Prosecutor.prosecutor != null && Prosecutor.prosecutor == CachedPlayer.LocalPlayer.PlayerControl &&
                                  Prosecutor.canSeeVoteColors && TasksHandler.taskInfo(CachedPlayer.LocalPlayer.Data).Item1 >=
                                  Prosecutor.tasksNeededToSeeVoteColors) ||
                                 (Watcher.watcher != null && CachedPlayer.LocalPlayer.PlayerControl == Watcher.watcher);
            if (showVoteColors && !Prosecutor.ProsecuteThisMeeting)
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
                if (playerVoteArea.TargetPlayerId == Swapper.playerId1) swapped1 = playerVoteArea;
                if (playerVoteArea.TargetPlayerId == Swapper.playerId2) swapped2 = playerVoteArea;
            }

            var doSwap = swapped1 != null && swapped2 != null && Swapper.swapper != null && Swapper.swapper.IsAlive();
            if (doSwap)
            {
                var localPosition = swapped1.transform.localPosition;
                __instance.StartCoroutine(Effects.Slide3D(swapped1.transform, localPosition, swapped2.transform.localPosition, 1.5f));
                __instance.StartCoroutine(Effects.Slide3D(swapped2.transform, swapped2.transform.localPosition, localPosition, 1.5f));
                Swapper.charges--;
            }

            __instance.TitleText.text = FastDestroyableSingleton<TranslationController>.Instance
                .GetString(StringNames.MeetingVotingResults, new Il2CppReferenceArray<Il2CppSystem.Object>(0));

            var allNums = new Dictionary<int, int>();
            __instance.TitleText.text = Object.FindObjectOfType<TranslationController>().GetString(StringNames.MeetingVotingResults, []);

            var amountOfSkippedVoters = 0;
            var num = 0;
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                var targetPlayerId = playerVoteArea.TargetPlayerId;
                allNums.Add(i, 0);

                playerVoteArea = doSwap switch
                {
                    // Swapper change playerVoteArea that gets the votes
                    true when playerVoteArea.TargetPlayerId == swapped1.TargetPlayerId => swapped2,
                    true when playerVoteArea.TargetPlayerId == swapped2.TargetPlayerId => swapped1,
                    _ => playerVoteArea
                };

                playerVoteArea.ClearForResults();
                var num2 = 0;
                for (var j = 0; j < states.Length; j++)
                {
                    if (Prosecutor.ProsecuteThisMeeting && Prosecutor.prosecutor.IsAlive()) break;
                    var voterState = states[j];
                    var playerById = GameData.Instance.GetPlayerById(voterState.VoterId);
                    if (playerById == null)
                    {
                        Warn($"找不到投票者的玩家信息: {voterState.VoterId}");
                    }
                    else if (i == 0 && voterState.SkippedVote && !playerById.IsDead)
                    {
                        __instance.BloopAVoteIcon(playerById, num, __instance.SkippedVoting.transform);

                        if (Mayor.mayor != null && voterState.VoterId == Mayor.mayor.PlayerId && Mayor.Revealed)
                            for (var repeat = 1; repeat < Mayor.Vote; repeat++)
                                __instance.BloopAVoteIcon(playerById, num, __instance.SkippedVoting.transform);

                        num++;
                    }
                    else if (voterState.VotedForId == targetPlayerId && !playerById.IsDead)
                    {
                        __instance.BloopAVoteIcon(playerById, num2, playerVoteArea.transform);

                        if (Mayor.mayor != null && voterState.VoterId == Mayor.mayor.PlayerId && Mayor.Revealed)
                            for (var repeat = 1; repeat < Mayor.Vote; repeat++)
                                __instance.BloopAVoteIcon(playerById, num2, playerVoteArea.transform);

                        num2++;
                    }
                }

                for (var j = 0; j < states.Length; j++)
                {
                    var voterState = states[j];
                    var playerById = GameData.Instance.GetPlayerById(voterState.VoterId);

                    if (Prosecutor.prosecutor == null) continue;
                    if (Prosecutor.prosecutor.IsDead()) continue;
                    if (Prosecutor.ProsecuteThisMeeting)
                    {
                        byte targetId = playerVoteArea.TargetPlayerId;
                        if (doSwap)
                        {
                            if (playerVoteArea.TargetPlayerId == swapped2.TargetPlayerId) targetId = swapped1.TargetPlayerId;
                            if (playerVoteArea.TargetPlayerId == swapped1.TargetPlayerId) targetId = swapped2.TargetPlayerId;
                        }

                        if (voterState.VoterId == Prosecutor.prosecutor.PlayerId)
                        {
                            if (playerById == null)
                            {
                                Error($"找不到投票者的玩家信息: {voterState.VoterId}");
                                Prosecutor.Prosecuted = true;
                            }
                            else if (i == 0 && voterState.SkippedVote)
                            {
                                for (var repeat = 0; repeat < 6; repeat++)
                                    __instance.BloopAVoteIcon(playerById, allNums[i], playerVoteArea.transform);

                                amountOfSkippedVoters += 6;
                                Prosecutor.Prosecuted = true;
                            }
                            else if (voterState.VotedForId == targetId)
                            {
                                for (var repeat = 0; repeat < 6; repeat++)
                                    __instance.BloopAVoteIcon(playerById, allNums[i], playerVoteArea.transform);

                                allNums[i] += 6;
                                Prosecutor.Prosecuted = true;
                            }
                        }
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
            [HarmonyArgument(1)] NetworkedPlayerInfo exiled, [HarmonyArgument(2)] bool tie)
        {
            // Reset swapper values
            Swapper.playerId1 = byte.MaxValue;
            Swapper.playerId2 = byte.MaxValue;

            // Lovers, Lawyer & Pursuer save next to be exiled, because RPC of ending game comes before RPC of exiled
            Lovers.notAckedExiledIsLover = false;
            Lawyer.notAckedExiled = false;
            if (exiled != null)
            {
                Lovers.notAckedExiledIsLover = (Lovers.lover1 != null && Lovers.lover1.PlayerId == exiled.PlayerId) ||
                                               (Lovers.lover2 != null && Lovers.lover2.PlayerId == exiled.PlayerId);
                Lawyer.notAckedExiled = (Pursuer.Player != null && Pursuer.Player.Any(id => id.PlayerId == exiled.PlayerId)) ||
                                         (Lawyer.lawyer != null && Lawyer.target != null &&
                                          Lawyer.target.PlayerId == exiled.PlayerId && Lawyer.target != Jester.jester);
            }

            Camouflager.camoComms = false;

            // Mini
            if (!Mini.isGrowingUpInMeeting)
                Mini.timeOfGrowthStart = Mini.timeOfGrowthStart.Add(DateTime.UtcNow.Subtract(Mini.timeOfMeetingStart)).AddSeconds(10);

            // Snitch
            if (Snitch.snitch != null && !Snitch.needsUpdate && Snitch.snitch.Data.IsDead && Snitch.text != null) Object.Destroy(Snitch.text);

            __instance.exiledPlayer = __instance.wasTie ? null : __instance.exiledPlayer;
            var exiledString = exiled == null ? "null" : exiled.PlayerName;
            Message($"被驱逐玩家: {exiledString}");
            Message($"是否平票: {tie}");
        }

        private static void Prefix(MeetingHud __instance,
            [HarmonyArgument(0)] Il2CppStructArray<VoterState> states,
            [HarmonyArgument(1)] NetworkedPlayerInfo exiled,
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
            return !(CachedPlayer.LocalPlayer != null && HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId) &&
                     Guesser.guesserUI != null);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ServerStart))]
    private class MeetingServerStartPatch
    {
        private static void Postfix(MeetingHud __instance)
        {
            populateButtonsPostfix(__instance);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Deserialize))]
    private class MeetingDeserializePatch
    {
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] MessageReader reader, [HarmonyArgument(1)] bool initialState)
        {
            // Add swapper buttons
            if (initialState) populateButtonsPostfix(__instance);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    private class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
        {
            var roomTracker = FastDestroyableSingleton<HudManager>.Instance.roomTracker;
            var roomId = byte.MinValue;
            if (roomTracker != null && roomTracker.LastRoom != null) roomId = (byte)roomTracker.LastRoom.RoomId;

            // Resett Bait list
            Bait.active = new Dictionary<DeadPlayer, float>();
            // Save AntiTeleport position, if the player is able to move (i.e. not on a ladder or a gap thingy)
            if (CachedPlayer.LocalPlayer.PlayerPhysics.enabled && (CachedPlayer.LocalPlayer.PlayerControl.moveable
                                                                   || CachedPlayer.LocalPlayer.PlayerControl.inVent
                                                                   || HudManagerStartPatch.hackerVitalsButton.isEffectActive
                                                                   || HudManagerStartPatch.hackerAdminTableButton.isEffectActive
                                                                   || HudManagerStartPatch.securityGuardCamButton.isEffectActive
                                                                   || (Portal.isTeleporting &&
                                                                       Portal.teleportedPlayers.Last().playerId ==
                                                                       CachedPlayer.LocalPlayer.PlayerId)))
                if (!CachedPlayer.LocalPlayer.PlayerControl.inMovingPlat)
                    AntiTeleport.position = CachedPlayer.LocalPlayer.transform.position;

            // Medium meeting start time
            Medium.meetingStartTime = DateTime.UtcNow;
            // Mini
            Mini.timeOfMeetingStart = DateTime.UtcNow;
            Mini.ageOnMeetingStart = Mathf.FloorToInt(Mini.growingProgress() * 18);
            // Count meetings
            if (meetingTarget == null) meetingsCount++;
            // Reset vampire bitten
            Vampire.bitten = null;
            // Count meetings
            if (meetingTarget == null) meetingsCount++;
            // Save the meeting target
            target = meetingTarget;
            isRoundOne = false;

            // Blackmail target
            if (Blackmailer.blackmailed != null && Blackmailer.blackmailed == CachedPlayer.LocalPlayer.PlayerControl)
            {
                Coroutines.Start(BlackmailShhh());
            }

            // Add Portal info into Portalmaker Chat:
            if (Portalmaker.portalmaker != null &&
                (CachedPlayer.LocalPlayer.PlayerControl == Portalmaker.portalmaker || shouldShowGhostInfo()) &&
                !Portalmaker.portalmaker.Data.IsDead)
                if (Portal.teleportedPlayers.Count > 0)
                {
                    var msg = "星门使用日志:\n";
                    foreach (var entry in Portal.teleportedPlayers)
                    {
                        var timeBeforeMeeting = (float)(DateTime.UtcNow - entry.time).TotalMilliseconds / 1000;
                        msg += Portalmaker.logShowsTime ? $"{(int)timeBeforeMeeting} 秒前: " : "";
                        msg += $"{entry.name} 使用了星门\n";
                    }

                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Portalmaker.portalmaker, $"{msg}");
                }

            // Remove revealed traps
            Trap.clearRevealedTraps();

            Terrorist.clearBomb();

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
        public static Sprite Overlay => Blackmailer.overlaySprite;

        private static void Postfix(MeetingHud __instance)
        {
            // Deactivate skip Button if skipping on emergency meetings is disabled
            if (target == null && blockSkippingInEmergencyMeetings)
                __instance.SkipVoteButton.gameObject.SetActive(false);

            updateMeetingText(__instance);

            if (Blackmailer.blackmailer != null && Blackmailer.blackmailed != null)
            {
                // Blackmailer show overlay
                var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == Blackmailer.blackmailed.PlayerId);
                playerState.Overlay.gameObject.SetActive(true);
                playerState.Overlay.sprite = Overlay;
                if (__instance.state != VoteStates.Animating && !Blackmailer.alreadyShook)
                {
                    Blackmailer.alreadyShook = true;
                    __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    private class MeetingChatNotification
    {
        private static void Postfix(MeetingHud __instance)
        {
            var chat = FastDestroyableSingleton<HudManager>.Instance.Chat;
            var playerControl = CachedPlayer.LocalPlayer.PlayerControl;
            var num = (int)chat.timeSinceLastMessage;
            foreach (var allPlayer in CachedPlayer.AllPlayers)
            {
                PlayerControl playerControl2 = allPlayer;
                if (playerControl2 != playerControl || playerControl2.Data.IsDead || num != 0) continue;
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetMeetingChatOverlay,
                    SendOption.Reliable);
                writer.Write(playerControl2.PlayerId);
                writer.Write(playerControl.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setChatNotificationOverlay(playerControl.PlayerId, playerControl2.PlayerId);
                break;
            }
        }
    }

    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
    public class BlockChatBlackmailed
    {
        public static bool Prefix(QuickChatMenu __instance)
        {
            if (Blackmailer.blackmailer != null && Blackmailer.blackmailed != null && Blackmailer.blackmailed == CachedPlayer.LocalPlayer.PlayerControl)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHudStart
    {
        public static Sprite Letter => Blackmailer.overlaySprite;

        public static void Postfix(MeetingHud __instance)
        {
            Message("会议开始");
            shookAlready = false;
            if (CachedPlayer.LocalPlayer.IsDead) CanSeeRoleInfo = true;
            // Remove first kill shield
            firstKillPlayer = null;

            //Nothing here for now. What to do when local player who is blackmailed starts meeting
            if (Blackmailer.blackmailed != null
                && Blackmailer.blackmailed.Data.PlayerId == CachedPlayer.LocalPlayer.PlayerId
                && Blackmailer.blackmailed.IsAlive())
                Coroutines.Start(BlackmailShhh());

            if (PartTimer.partTimer.IsAlive() && PartTimer.target == null) PartTimer.deathTurn--;

            if (Balancer.balancer.IsAlive() && PlayerControl.LocalPlayer == Balancer.balancer)
            {
                Balancer.Balancer_Patch.MeetingHudStartPostfix(__instance);
            }

            foreach (var playerState in Instance?.playerStates ?? Enumerable.Empty<PlayerVoteArea>())
            {
                var meetingInfoTransform = playerState.NameText.transform.parent.Find("WitnessInfo");
                if (meetingInfoTransform != null)
                {
                    Object.Destroy(meetingInfoTransform.gameObject);
                }
            }
        }
    }
}