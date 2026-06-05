using AmongUs.QuickChat;
using MoreLinq;
using System.Text;
using TheOtherRoles.Mode;
using TheOtherRoles.Objects;
using UnityEngine.UI;
using static MeetingHud;
using static TheOtherRoles.Options.ModOption;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
internal class MeetingHudPatch
{
    public static bool IsEmergencyMeetings;
    private static GameData.PlayerInfo ReportTarget;

    public static int MeetingCount;
    private static bool[] selections;
    private static SpriteRenderer[] renderers;
    private static PassiveButton[] swapperButtonList;
    private static TextMeshPro meetingExtraButtonLabel;
    public static GameObject MeetingExtraButton;
    public static bool shookAlready;
    private static PlayerVoteArea swapped1;
    private static PlayerVoteArea swapped2;

    private static Sprite LightColorSprite = new ResourceSprite("ColorLight.png", 75);
    private static Sprite DarkColorSprite = new ResourceSprite("ColorDark.png", 75);

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

        var writer = StartRPC(CustomRPC.SwapperSwap);
        writer.Write(firstPlayer);
        writer.Write(secondPlayer);
        writer.EndRPC();
        RPCProcedure.swapperSwap(firstPlayer, secondPlayer);
    }

    public static void swapperCheckAndReturnSwap(MeetingHud __instance, byte dyingPlayerId)
    {
        // someone was guessed or dced in the meeting, check if this affects the swapper.
        if (Swapper.swapper.IsDead() || __instance.state == VoteStates.Results) return;

        // reset swap.
        var reset = false;
        if (dyingPlayerId == Swapper.playerId1 || dyingPlayerId == Swapper.playerId2 || dyingPlayerId == byte.MaxValue - 1)
        {
            reset = true;
            Swapper.playerId1 = Swapper.playerId2 = byte.MaxValue;
        }

        // Only for the swapper: Reset all the buttons and charges value to their original state.
        if (PlayerControl.LocalPlayer != Swapper.swapper) return;

        // check if dying player was a selected player (but not confirmed yet)
        for (var i = 0; i < __instance.playerStates.Count; i++)
        {
            reset = reset || (selections[i] && __instance?.playerStates[i]?.TargetPlayerId == dyingPlayerId);
            if (reset) break;
        }

        if (!reset) return;


        for (var i = 0; i < selections.Length; i++)
        {
            selections[i] = false;
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || (playerVoteArea.TargetPlayerId == Swapper.swapper.PlayerId && Swapper.canOnlySwapOthers)) continue;
            renderers[i].color = Color.red;
            var copyI = i;
            swapperButtonList[i].OnClick.RemoveAllListeners();
            swapperButtonList[i].OnClick.AddListener((Action)(() => swapperOnClick(copyI, __instance)));
        }
    }

    private static void mayorToggleVoteTwice(MeetingHud __instance)
    {
        __instance.playerStates[0].Cancel(); // This will stop the underlying buttons of the template from showing up
        if (__instance.state is VoteStates.Results or VoteStates.Discussion || Mayor.mayor.IsDead())
            return;

        if (Mayor.Mode == Mayor.MayorMode.Revealed)
        {
            if (Gaoler.IsPrisoner(Mayor.mayor))
            {
                UObject.Destroy(MeetingExtraButton);
                return;
            }

            Mayor.Revealed = true;
            var writer = StartRPC(CustomRPC.MayorRevealed);
            writer.EndRPC();
            UObject.Destroy(MeetingExtraButton);
        }


        if (Mayor.Mode == Mayor.MayorMode.Regular)
        {
            Mayor.MultiVote = !Mayor.MultiVote;

            MessageWriter writer = StartRPC(CustomRPC.MayorMultiVote);
            writer.Write(Mayor.MultiVote);
            writer.EndRPC();

            var text = Mayor.MultiVote ? string.Format(GetString("Mayor.MultiVote.On"), Mayor.Votes) : GetString("Mayor.MultiVote.Off");

            meetingExtraButtonLabel.text = Cs(Mayor.MultiVote ? Color.green : Color.red, text);
        }
    }

    private static void populateButtonsPostfix(MeetingHud __instance)
    {
        // Add Swapper Buttons
        var addSwapperButtons = Swapper.swapper.IsAlive() && PlayerControl.LocalPlayer == Swapper.swapper
            && PlayerControl.LocalPlayer.CanUseMeetingAbility();

        var addMayorButton = Mayor.mayor.IsAlive() && PlayerControl.LocalPlayer == Mayor.mayor
            && ((Mayor.Mode == Mayor.MayorMode.Revealed && !Mayor.Revealed) || (Mayor.Mode == Mayor.MayorMode.Regular && Mayor.VoteCountToggle))
            && PlayerControl.LocalPlayer.CanUseMeetingAbility();

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
                var checkbox = UObject.Instantiate(template, playerVoteArea.transform, true);
                checkbox.transform.position = template.transform.position;
                checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                if ((HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(PlayerControl.LocalPlayer.PlayerId)) || (Mimic.mimic?.PlayerId == PlayerControl.LocalPlayer.PlayerId))
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
            var meetingUI = UObject.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var textTemplate = __instance.playerStates[0].NameText;
            var meetingExtraButtonParent = new GameObject().transform;
            meetingExtraButtonParent.SetParent(meetingUI);
            var meetingExtraButton = UObject.Instantiate(buttonTemplate, meetingExtraButtonParent);
            MeetingExtraButton = meetingExtraButton.gameObject;

            var meetingExtraButtonMask = UObject.Instantiate(maskTemplate, meetingExtraButtonParent);
            meetingExtraButtonLabel = UObject.Instantiate(textTemplate, meetingExtraButton);
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
            if (Mayor.Mode == Mayor.MayorMode.Revealed)
                meetingExtraButtonLabel.text = Cs(Mayor.color, GetString("Mayor.Revealed"));
            else
                meetingExtraButtonLabel.text = Cs(Color.red, GetString("Mayor.MultiVote.Off"));

            var passiveButton = meetingExtraButton.GetComponent<PassiveButton>();
            passiveButton.OnClick.RemoveAllListeners();
            if (!PlayerControl.LocalPlayer.Data.IsDead && addMayorButton)
                passiveButton.OnClick.AddListener((Action)(() => mayorToggleVoteTwice(__instance)));

            meetingExtraButton.parent.gameObject.SetActive(false);
            __instance.StartCoroutine(Effects.Lerp(7.27f, new Action<float>(p =>
            {
                // Button appears delayed, so that its visible in the voting screen only!
                if ((int)p == 1) meetingExtraButton.parent.gameObject.SetActive(true);
            })));
        }

        var isGuesser = HandleGuesser.isGuesser(PlayerControl.LocalPlayer.PlayerId);

        // Add overlay for spelled players
        if (Witch.witch != null && Witch.futureSpelled != null)
        {
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (Witch.futureSpelled.Any(x => x.PlayerId == pva.TargetPlayerId))
                {
                    var local = PlayerControl.LocalPlayer;
                    var rend = new GameObject().AddComponent<SpriteRenderer>();
                    rend.transform.SetParent(pva.transform);
                    rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                    rend.transform.localPosition = new Vector3(-0.5f, -0.03f, -1f);
                    if (local == Swapper.swapper && (isGuesser || local.PlayerId == Mimic.mimic?.PlayerId))
                        rend.transform.localPosition = new Vector3(-0.725f, -0.15f, -1f);
                    rend.sprite = Witch.spelledOverlaySprite;
                }
            }
        }

        // Add Guesser Buttons
        var GuesserRemainingShots = HandleGuesser.remainingShots(PlayerControl.LocalPlayer.PlayerId);

        if (isGuesser
            && PlayerControl.LocalPlayer.IsAlive()
            && GuesserRemainingShots > 0
            && PlayerControl.LocalPlayer.CanUseMeetingAbility()
            && (PlayerControl.LocalPlayer != WolfLord.Player || !WolfLord.Revealed || WolfLord.Killed))
        {
            Doomsayer.CanShoot = true;
            //int i = 0;
            foreach (var (pvae, i) in __instance.playerStates.Select((pvae, i) => (pvae, i)))
            {
                if (pvae.AmDead || pvae.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

                if (Eraser.eraser.IsAlive() && PlayerControl.LocalPlayer == Eraser.eraser)
                {
                    if (!Eraser.canEraseGuess && Eraser.alreadyErased.Any(x => x == pvae.TargetPlayerId))
                    {
                        continue;
                    }
                }

                var template = pvae.Buttons.transform.Find("CancelButton").gameObject;
                var targetBox = UObject.Instantiate(template, pvae.transform);
                targetBox.name = "ShootButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                var renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = HandleGuesser.targetSprite;
                var button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                var copiedIndex = i;
                button.OnClick.AddListener((Action)(() => Guesser.guesserOnClick(copiedIndex, __instance)));
                //i++;
            }
        }
    }

    public static void updateMeetingText(MeetingHud __instance)
    {
        if (PlayerControl.LocalPlayer.IsDead()) return;

        if (__instance.state is not VoteStates.Voted and not VoteStates.NotVoted and not VoteStates.Discussion) return;

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
            meetingInfoText = string.Format(GetString("PartTimerMeetingInfo"), PartTimer.deathTurn);
        }
        else if (PlayerControl.LocalPlayer == SoulSight.Player)
        {
            meetingInfoText = SoulSight.Score >= SoulSight.ScoreToWin
                ? GetString("SoulSightMeetingInfo2")
                : string.Format(GetString("SoulSightMeetingInfo1"), SoulSight.ScoreToWin - SoulSight.Score);
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
        else if (PlayerControl.LocalPlayer == BandLeader.Player)
        {
            if (BandLeader.Formed) meetingInfoText = string.Format(GetString("BandLeaderFormed"), $"{$"{BandLeader.WinCondition}Team".Translate()}");
            else meetingInfoText = GetString("BandLeaderBad");
        }
        else if (Infected.Player.Any(x => x == PlayerControl.LocalPlayer) && Infected.IsGuesser)
        {
            meetingInfoText = string.Format(GetString("InfectedGuesserCount"), Infected.GuessCount);
        }
        else if (PlayerControl.LocalPlayer == Gaoler.Player && Gaoler.Player.IsAlive() && !Gaoler.hasSelectedThisMeeting && Gaoler.remainingUses > 0 && !Gaoler.endMeetingSelection)
        {
            int timeLeft = (int)(Gaoler.selectionWindow - (DateTime.UtcNow - Gaoler.meetingStartTime).TotalSeconds);
            meetingInfoText = timeLeft > 0 ? string.Format(GetString("GaolerSelectTimeLeft"), timeLeft) : "";
        }

        __instance.TimerText.gameObject.SetActive(true);

        if (NormalOptions.VotingTime != 0)
        {
            __instance.TimerText.text = $"{meetingInfoText}\n{__instance.TimerText.text}";
        }
        else
        {
            if (meetingInfoText.IsNullOrWhiteSpace()) __instance.TimerText.gameObject.SetActive(false);
            __instance.TimerText.text = $"{meetingInfoText}";
        }

    }

    [HarmonyPatch]
    public class ShowHost
    {
        public static TextMeshPro Text;
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        public static void Setup(MeetingHud __instance)
        {
            if (Anonymous.IsEnabled) return;
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
            if (Anonymous.IsEnabled) return;
            if (Balancer.currentAbilityUser != null) return;
            var host = GameData.Instance.GetHost();

            if (host != null)
            {
                PlayerMaterial.SetColors(host.DefaultOutfit.ColorId, __instance.HostIcon);
                if (Text == null)
                {
                    Text = __instance.ProceedButton.gameObject.GetComponentInChildren<TextMeshPro>();
                }
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
                var player = PlayerById(playerVoteArea.TargetPlayerId);
                if (player.IsDead()) continue;

                if (InfoSleuth.infoSleuth != null && playerVoteArea.TargetPlayerId == InfoSleuth.infoSleuth.PlayerId)
                {
                    var writer = StartRPC(CustomRPC.InfoSleuthSetTarget);
                    writer.Write(playerVoteArea.VotedFor);
                    writer.EndRPC();
                    RPCProcedure.infoSleuthSetTarget(playerVoteArea.VotedFor);
                }

                float additionalVotes = 1;
                if (Prosecutor.prosecutor.IsAlive() && Prosecutor.prosecutor.PlayerId == playerVoteArea.TargetPlayerId)
                    additionalVotes = Prosecutor.ProsecuteThisMeeting ? 15 : 1;

                if (Mayor.mayor.IsAlive() && Mayor.mayor.PlayerId == playerVoteArea.TargetPlayerId)
                    additionalVotes = Mayor.GetVotes();

                if (Tiebreaker.tiebreaker.IsAlive() && Tiebreaker.tiebreaker.PlayerId == playerVoteArea.TargetPlayerId)
                    additionalVotes = 1.5f;

                if (Prosecutor.prosecutor.IsAlive() && Prosecutor.ProsecuteThisMeeting && Prosecutor.prosecutor.PlayerId != playerVoteArea.TargetPlayerId)
                    additionalVotes = 0;

                if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var currentVotes))
                    dictionary[playerVoteArea.VotedFor] = currentVotes + additionalVotes;

                else
                    dictionary[playerVoteArea.VotedFor] = additionalVotes;
            }

            // Swapper swap votes
            if (Swapper.swapper.IsDead() || Gaoler.IsPrisoner(Swapper.swapper)) return dictionary;

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

            (dictionary[swapped1.TargetPlayerId], dictionary[swapped2.TargetPlayerId]) = (dictionary[swapped2.TargetPlayerId], dictionary[swapped1.TargetPlayerId]);

            return dictionary;
        }

        private static bool Prefix(MeetingHud __instance)
        {
            if (!__instance.playerStates.Where(x => PlayerById(x.TargetPlayerId).CanUseMeetingAbility()).All(ps => ps.AmDead || ps.DidVote))
                return false;

            var behavior = IsEmergencyMeetings ? blockSkippingInEmergencyMeetings : blockSkippingInGeneralMeetings;

            foreach (var pva in __instance.playerStates)
            {
                switch (behavior)
                {
                    case NoVoteBehavior.SkipAsSelfVote:
                        if (pva.VotedFor == 253) pva.VotedFor = pva.TargetPlayerId;
                        break;
                    case NoVoteBehavior.SkipAsAbstain:
                        if (pva.VotedFor == 253) pva.VotedFor = 254;
                        break;
                    case NoVoteBehavior.skipOrAbstainAsSelfVote:
                        if (pva.VotedFor is 253 or 254) pva.VotedFor = pva.TargetPlayerId;
                        break;
                }
            }

            var self = CalculateVotes(__instance);
            //var max = self.MaxPair(out var tie);
            GameData.PlayerInfo exiled = null;
            bool tie = false;

            VoterState[] states;
            List<VoterState> statesList = new();

            foreach (var pva in __instance.playerStates)
            {
                var player = PlayerById(pva.TargetPlayerId);
                //バランサー処理
                if (Balancer.currentAbilityUser != null)
                {
                    if (player != null && pva.VotedFor != Balancer.targetplayerright.PlayerId && pva.VotedFor != Balancer.targetplayerleft.PlayerId)
                    {
                        if (pva.TargetPlayerId == Balancer.currentAbilityUser.PlayerId)
                        {
                            pva.VotedFor = 254;
                        }
                        else
                        {
                            bool chooseRight = rnd.Next(0, 2) == 0;
                            pva.VotedFor = chooseRight
                                ? Balancer.targetplayerright.PlayerId
                                : Balancer.targetplayerleft.PlayerId;
                        }
                    }
                }

                bool isPrisoner = Gaoler.IsPrisoner(player);
                if (Mayor.mayor != null && Mayor.mayor.PlayerId == pva.TargetPlayerId && Mayor.Mode == Mayor.MayorMode.StoreVotes && isPrisoner)
                {
                    pva.VotedFor = 254;
                    Mayor.CurrentVote = 0;
                    Mayor.MyVotes -= Mayor.AddVotes;
                    var writer = StartRPC(CustomRPC.MayorSetVoteCount);
                    writer.Write(0);
                    writer.Write(Mayor.MyVotes);
                    writer.EndRPC();
                }
                else if (isPrisoner)
                {
                    pva.VotedFor = 254;
                }

                if (Mayor.mayor != null && Mayor.mayor.PlayerId == pva.TargetPlayerId && Mayor.Mode == Mayor.MayorMode.StoreVotes && Mayor.CurrentVote == 0)
                {
                    pva.VotedFor = 254;
                }

                if (Prosecutor.prosecutor?.PlayerId == pva.TargetPlayerId && Prosecutor.ProsecuteThisMeeting && pva.VotedFor > 250)
                {
                    Prosecutor.Prosecuted = false;
                    Prosecutor.ProsecuteThisMeeting = false;
                    Prosecutor.StartProsecute = false;
                    var writer = StartRPC(CustomRPC.Prosecute);
                    writer.Write(false);
                    writer.EndRPC();
                }

                statesList.Add(new VoterState()
                {
                    VoterId = pva.TargetPlayerId,
                    VotedForId = pva.VotedFor
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

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
    public static class MeetingHudClearVotePatch
    {
        public static void Prefix(MeetingHud __instance)
        {
            Info("ClearVote");
            swapperCheckAndReturnSwap(__instance, byte.MaxValue - 1);

            Prosecutor.ProsecuteThisMeeting = false;
            Prosecutor.StartProsecute = false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.RpcVotingComplete))]
    public static class MeetingHudRpcVotingCompletePatch
    {
        public static void Prefix(MeetingHud __instance, Il2CppStructArray<VoterState> states, GameData.PlayerInfo exiled, bool tie)
        {
            Info($"exiled: {exiled?.PlayerName}, states: {states?.Count(x => x.VotedForId is < 250 or 253)}, stateFor: {states?.Count(x => x.VotedForId == exiled?.PlayerId)}, tie: {tie}", "RpcVotingComplete");
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    private class MeetingHudBloopAVoteIconPatch
    {
        public static bool Prefix(MeetingHud __instance, GameData.PlayerInfo voterPlayer, int index, Transform parent)
        {
            var spriteRenderer = UObject.Instantiate(__instance.PlayerVotePrefab);
            var showVoteColors = !NormalOptions.AnonymousVotes || CanSeeGhostInfo ||
                                 (Prosecutor.prosecutor != null && Prosecutor.prosecutor == PlayerControl.LocalPlayer && Prosecutor.CanSeeVoteColors) ||
                                 (Watcher.watcher != null && PlayerControl.LocalPlayer == Watcher.watcher);
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

            var doSwap = swapped1 != null && swapped2 != null && Swapper.swapper.IsAlive() && !Gaoler.IsPrisoner(Swapper.swapper);
            if (doSwap)
            {
                var localPosition = swapped1.transform.localPosition;
                __instance.StartCoroutine(Effects.Slide3D(swapped1.transform, localPosition, swapped2.transform.localPosition, 1.5f));
                __instance.StartCoroutine(Effects.Slide3D(swapped2.transform, swapped2.transform.localPosition, localPosition, 1.5f));
                Swapper.charges--;
            }

            __instance.TitleText.text = UObject.FindObjectOfType<TranslationController>().GetString(StringNames.MeetingVotingResults, []);
            var allNums = new Dictionary<int, int>();

            var num = 0;
            var mayorVotesDisplayed = 0;

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var pva = __instance.playerStates[i];
                var targetId = pva.TargetPlayerId;
                allNums.Add(i, 0);

                pva.ClearForResults();
                var num2 = 0;

                // 处理全部投票
                for (var j = 0; j < states.Length; j++)
                {
                    var voterState = states[j];
                    var playerById = GameData.Instance.GetPlayerById(voterState.VoterId);

                    if (playerById == null || playerById.IsDead) continue;

                    if (doSwap)
                    {
                        if (pva.TargetPlayerId == swapped2.TargetPlayerId) targetId = swapped1.TargetPlayerId;
                        if (pva.TargetPlayerId == swapped1.TargetPlayerId) targetId = swapped2.TargetPlayerId;
                    }

                    if (i == 0 && voterState.SkippedVote) // 显示跳过
                    {
                        if (Prosecutor.ProsecuteThisMeeting && Prosecutor.prosecutor.IsAlive())
                        {
                            if (voterState.VoterId == Prosecutor.prosecutor.PlayerId)
                            {
                                for (var repeat = 0; repeat < 6; repeat++)
                                    __instance.BloopAVoteIcon(playerById, allNums[i], __instance.SkippedVoting.transform);
                                Prosecutor.Prosecuted = true;
                            }
                            continue;
                        }

                        __instance.BloopAVoteIcon(playerById, num, __instance.SkippedVoting.transform);

                        if (Mayor.mayor.IsAlive() && voterState.VoterId == Mayor.mayor.PlayerId)
                        {
                            if (mayorVotesDisplayed >= Mayor.GetVotes() - 1)
                            {
                                mayorVotesDisplayed = 0;
                                continue;
                            }
                            mayorVotesDisplayed++;
                            j--;
                        }
                        num++;
                    }
                    else if (voterState.VotedForId == targetId) // 显示投票
                    {
                        if (Prosecutor.ProsecuteThisMeeting && Prosecutor.prosecutor.IsAlive())
                        {
                            if (voterState.VoterId == Prosecutor.prosecutor.PlayerId)
                            {
                                for (var repeat = 0; repeat < 6; repeat++)
                                    __instance.BloopAVoteIcon(playerById, allNums[i], pva.transform);
                                Prosecutor.Prosecuted = true;
                            }
                            continue;
                        }

                        if (Mayor.mayor.IsAlive() && voterState.VoterId == Mayor.mayor.PlayerId)
                        {
                            if (mayorVotesDisplayed >= Mayor.GetVotes())
                            {
                                mayorVotesDisplayed = 0;
                                continue;
                            }
                            mayorVotesDisplayed++;
                            j--;
                            __instance.BloopAVoteIcon(playerById, num2, pva.transform);
                        }
                        else
                        {
                            __instance.BloopAVoteIcon(playerById, num2, pva.transform);
                        }

                        num2++;
                    }
                    continue;
                }
            }
            return false;
        }
    }

    [HarmonyPatch]
    private class MeetingHudCastVotePatch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm)), HarmonyPrefix]
        private static bool ConfirmFix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.CanUseMeetingAbility()) return false;
            return true;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CmdCastVote)), HarmonyPrefix]
        private static bool CastVoteFix(MeetingHud __instance, [HarmonyArgument(0)] byte srcPlayerId)
        {
            var voter = PlayerById(srcPlayerId);
            if (!voter.CanUseMeetingAbility()) return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    private class MeetingHudVotingCompletedPatch
    {
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] byte[] states,
            [HarmonyArgument(1)] GameData.PlayerInfo exiled, [HarmonyArgument(2)] bool tie)
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
                                          Lawyer.target.PlayerId == exiled.PlayerId && Jester.Player.Any(x => x.PlayerId != Lawyer.target?.PlayerId));
            }

            Camouflager.camoComms = false;

            // Snitch
            if (Snitch.snitch != null && !Snitch.needsUpdate && Snitch.snitch.Data.IsDead && Snitch.text != null) UObject.Destroy(Snitch.text);

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
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            // Save AntiTeleport position, if the player is able to move (i.e. not on a ladder or a gap thingy)
            if (PlayerControl.LocalPlayer.MyPhysics.enabled && !PlayerControl.LocalPlayer.inMovingPlat && (PlayerControl.LocalPlayer.moveable
                || PlayerControl.LocalPlayer.inVent
                || HudManagerStartPatch.hackerVitalsButton.IsEffectActive
                || HudManagerStartPatch.hackerAdminTableButton.IsEffectActive
                || HudManagerStartPatch.securityGuardCamButton.IsEffectActive
                || (Portal.isTeleporting && Portal.teleportedPlayers.Last().playerId == PlayerControl.LocalPlayer.PlayerId)))
            {
                AntiTeleport.position = PlayerControl.LocalPlayer.transform.position;
            }

            // Medium meeting start time
            Alchemyst.meetingStartTime = DateTime.UtcNow;
            // Count meetings
            if (meetingTarget == null) meetingsCount++;
            // Count meetings
            if (meetingTarget == null) meetingsCount++;
            // Save the meeting target
            ReportTarget = meetingTarget;
            IsEmergencyMeetings = meetingTarget == null;
            isRoundOne = false;

            // Add Portal info into Portalmaker Chat:
            if (Portalmaker.portalmaker != null &&
                (PlayerControl.LocalPlayer == Portalmaker.portalmaker || CanSeeGhostInfo) &&
                !Portalmaker.portalmaker.Data.IsDead && Portal.teleportedPlayers.Count > 0)
            {
                var msg = new StringBuilder(GetString("Portalmaker.LogHeader"));

                foreach (var entry in Portal.teleportedPlayers)
                {
                    var timeBeforeMeeting = (int)(DateTime.UtcNow - entry.time).TotalSeconds;

                    if (Portalmaker.logShowsTime)
                    {
                        msg.AppendFormat(GetString("Portalmaker.LogEntry"), timeBeforeMeeting, entry.name);
                    }
                    else
                    {
                        msg.AppendFormat(GetString("Portalmaker.LogEntryNoTime"), entry.name);
                    }
                }

                HudManager.Instance.Chat.AddChat(Portalmaker.portalmaker, msg.ToString());
            }

            // Remove revealed traps
            Trap.clearRevealedTraps();

            CustomObject.StartMeeting();

            // Reset zoomed out ghosts
            toggleZoom(true);

            // Stop all playing sounds
            SoundEffectsManager.stopAll();

            // Close In-Game Settings Display if open
            HudManagerUpdate.CloseSettings();
        }
    }

    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
    public class BlockChatBlackmailed
    {
        public static bool Prefix(QuickChatMenu __instance)
        {
            if (Blackmailer.Player != null && Blackmailer.blackmailed != null && Blackmailer.blackmailed == PlayerControl.LocalPlayer)
            {
                return false;
            }
            return true;
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

            // Deactivate skip Button if skipping on emergency meetings is disabled
            if (ReportTarget == null && blockSkippingInEmergencyMeetings == NoVoteBehavior.Enable)
            {
                __instance.SkipVoteButton.gameObject.SetActive(false);
            }

            if (ReportTarget != null && blockSkippingInGeneralMeetings == NoVoteBehavior.Enable)
            {
                __instance.SkipVoteButton.gameObject.SetActive(false);
            }

            updateMeetingText(__instance);
            Balancer.UpdateButton(__instance);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHudStart
    {
        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        public static void MeetingStartPatch(MeetingHud __instance)
        {
            Message("Start", "Meeting");
            shookAlready = false;
            MeetingCount++;

            Attributes.OnMeetingStartAttribute.Invoke();

            Oracle.SendOracleReport();

            if (PlayerControl.LocalPlayer.IsDead()) CanSeeGhostInfo = true;

            // Remove first kill shield
            if (!PlayerControl.AllPlayerControls.ToList().All(x => x.IsAlive())) firstKillPlayer = null;

            if (Witch.witch.IsDead()) Witch.futureSpelled.Clear();



            if (PartTimer.partTimer.IsAlive() && PartTimer.target == null) PartTimer.deathTurn--;

            if (Balancer.balancer.IsAlive() && PlayerControl.LocalPlayer == Balancer.balancer)
            {
                if (PlayerControl.LocalPlayer.CanUseMeetingAbility()) Balancer.MeetingStart(__instance);
            }

            Redemptor.RevivedPlayer = null;
            Undertaker.dragedBody = null;
            Jailor.MeetingStart(__instance);

            foreach (var playerState in Instance?.playerStates ?? Enumerable.Empty<PlayerVoteArea>())
            {
                var meetingInfoTransform = playerState.NameText.transform.parent.Find("WitnessInfo");
                if (meetingInfoTransform != null)
                {
                    UObject.Destroy(meetingInfoTransform.gameObject);
                }
            }

            if (BandLeader.Player.IsAlive() && !BandLeader.Formed && PlayerControl.LocalPlayer == BandLeader.Player)
            {
                var (allNeutral, allCrew, allImpostor) = (
                    BandLeader.Members.All(x => x.IsNeutral()),
                    BandLeader.Members.All(x => x.IsCrew(AndCat: true)),
                    BandLeader.Members.All(x => x.IsImpostor(AndCat: true)));

                if (BandLeader.Members.Length == 3 && (allNeutral || allCrew || allImpostor))
                {
                    BandLeader.Formed = true;
                    if (allCrew) BandLeader.WinCondition = BandLeader.WinnerFlags.Crewmate;
                    else if (allImpostor) BandLeader.WinCondition = BandLeader.WinnerFlags.Impostor;
                    else if (allNeutral) BandLeader.WinCondition = BandLeader.WinnerFlags.Neutral;
                    var writer = StartRPC(CustomRPC.BandLeaderFormed);
                    writer.Write((byte)BandLeader.WinCondition);
                    writer.Write(true);
                    writer.EndRPC();
                    BandLeader.BandLeaderFormed((byte)BandLeader.WinCondition, true);
                }
                else if (BandLeader.Members.Length == 3)
                {
                    var writer = StartRPC(CustomRPC.BandLeaderFormed);
                    writer.Write((byte)0);
                    writer.Write(false);
                    writer.EndRPC();
                    BandLeader.BandLeaderFormed(0, false);
                }
            }

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
            Gaoler.hasSelectedThisMeeting = false;
            Gaoler.currentPrisoner = null;
            Gaoler.endMeetingSelection = false;
            Message("Destroy", "Meeting");
            CustomObject.EndMeeting(__instance);
        }
    }
}