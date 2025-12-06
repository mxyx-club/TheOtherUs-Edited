using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Impostor;

public class WolfLord
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;

    public static bool Revealed;
    public static bool Killed;

    public static Sprite TargetSprite = new ResourceSprite("TargetIcon.png", 150);

    public static void ClearAndReload()
    {
        Player = null;
        Revealed = false;
        Killed = false;
    }

    public static void WolfLordkilled(byte targetId)
    {
        var target = PlayerById(targetId);
        Revealed = true;
        if (target == null) return;

        Killed = true;

        target.CustomExiled(true);
        Avenger.OnPlayerDeath(Player, target);
        Akujo.otherLover(target)?.CustomExiled(true);

        PlayerData.SetDeathReason(target, CustomDeathReason.Kill, Player);
        if (target == Balancer.currentTarget) Balancer.currentTarget = null;
        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(target.KillSfx, false, 0.8f);

        if (PlayerControl.LocalPlayer == target)
            FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(Player.Data, target.Data);

        if (MeetingHud.Instance)
        {
            ExtendMeetingTime(CustomOptionHolder.guessExtendmeetingTime.GetFloat());
            MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, targetId);

            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                var dyingPartner = target.GetPartner();
                byte partnerId = dyingPartner != null ? dyingPartner.PlayerId : targetId;
                bool shouldClearVote = CustomOptionHolder.guessReVote.GetBool() || pva.VotedFor == targetId || pva.VotedFor == partnerId;

                if (shouldClearVote)
                {
                    pva.UnsetVote();
                    var voteAreaPlayer = PlayerById(pva.TargetPlayerId);
                    if (voteAreaPlayer?.AmOwner == false) continue;
                    MeetingHud.Instance.ClearVote();
                }
            }
            if (AmongUsClient.Instance.AmHost) MeetingHud.Instance.CheckForEndVoting();
        }
    }

    [HarmonyPatch]
    public static class WolfLord_Patch
    {
        private static TextMeshPro meetingExtraButtonLabel;
        private static GameObject MeetingExtraButton;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        private static void MeetingStartPostfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.CanUseMeetingAbility()) return;
            if (__instance && !Killed && Revealed)
            {
                ButtonToggle(__instance);
                return;
            }
            if (Player.IsAlive() && PlayerControl.LocalPlayer == Player && !Revealed)
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
                meetingExtraButton.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

                meetingExtraButtonParent.localPosition = new Vector3(0, -2.225f, -5);
                meetingExtraButtonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
                meetingExtraButtonLabel.alignment = TextAlignmentOptions.Center;
                meetingExtraButtonLabel.transform.localPosition = new Vector3(0, 0, meetingExtraButtonLabel.transform.localPosition.z);

                var localScale = meetingExtraButtonLabel.transform.localScale;
                localScale = new Vector3(
                    localScale.x * 1.5f,
                    localScale.x * 1.7f,
                    localScale.x * 1.7f);
                meetingExtraButtonLabel.transform.localScale = localScale;
                meetingExtraButtonLabel.text = Cs(color, "WolfLord.Button".Translate());

                var passiveButton = meetingExtraButton.GetComponent<PassiveButton>();
                passiveButton.OnClick.RemoveAllListeners();
                if (Player.IsAlive()) passiveButton.OnClick.AddListener((Action)(() => ButtonToggle(__instance)));

                meetingExtraButton.parent.gameObject.SetActive(false);
                __instance.StartCoroutine(Effects.Lerp(7.27f, new Action<float>(p =>
                {
                    if ((int)p == 1) meetingExtraButton.parent.gameObject.SetActive(true);
                })));
            }
        }

        public static void ClearButton()
        {
            if (MeetingExtraButton != null) UObject.Destroy(MeetingExtraButton);
        }

        private static void ButtonToggle(MeetingHud __instance)
        {
            __instance.playerStates[0].Cancel(); // This will stop the underlying buttons of the template from showing up
            if (__instance.state is MeetingHud.VoteStates.Discussion or MeetingHud.VoteStates.Results || Player.IsDead()) return;

            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.WolfLordkilled);
            writer.Write(byte.MaxValue);
            writer.EndRPC();
            WolfLordkilled(byte.MaxValue);

            UObject.Destroy(MeetingExtraButton);

            foreach (var playerState in __instance.playerStates)
            {
                var guesser = playerState.transform.FindChild("ShootButton");
                if (guesser != null) UObject.Destroy(guesser.gameObject);
            }

            if (Guesser.guesserUI != null && Guesser.guesserUIExitButton != null)
                Guesser.guesserUIExitButton.OnClick.Invoke();

            if (PlayerControl.LocalPlayer == Player && PlayerControl.LocalPlayer.IsAlive())
            {
                foreach (var pva in __instance.playerStates)
                {
                    var player = PlayerById(pva.TargetPlayerId);
                    if (player.IsAlive() && player != Player && !player.IsImpostor())
                    {
                        GameObject template = pva.Buttons.transform.Find("CancelButton").gameObject;
                        GameObject targetBox = UObject.Instantiate(template, pva.transform);
                        targetBox.name = "WolfLordIcon";
                        targetBox.transform.localPosition = new Vector3(1f, 0.03f, -1f);
                        SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                        renderer.sprite = TargetSprite;
                        renderer.color = Color.red;
                        PassiveButton button = targetBox.GetComponent<PassiveButton>();
                        button.OnClick.RemoveAllListeners();
                        button.OnClick.AddListener(() => WolfLordOnClick(pva, __instance));
                    }
                }
            }
        }

        private static void WolfLordOnClick(PlayerVoteArea pva, MeetingHud __instance)
        {
            var target = PlayerById(pva.TargetPlayerId);
            if (Player == null || !Revealed || Killed || target == null) return;
            if (__instance.state is MeetingHud.VoteStates.Discussion or MeetingHud.VoteStates.Results) return;

            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.WolfLordkilled);
            writer.Write(target.PlayerId);
            writer.EndRPC();
            WolfLordkilled(target.PlayerId);

            foreach (var playerState in __instance.playerStates)
            {
                var icon = playerState.transform.FindChild("WolfLordIcon");
                if (icon != null) UObject.Destroy(icon.gameObject);

                var guesser = playerState.transform.FindChild("ShootButton");
                if (guesser != null) UObject.Destroy(guesser.gameObject);
            }

            if (Guesser.guesserUI != null && Guesser.guesserUIExitButton != null)
                Guesser.guesserUIExitButton.OnClick.Invoke();
        }
    }
}
