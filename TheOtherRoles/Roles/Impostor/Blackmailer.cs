namespace TheOtherRoles.Roles.Impostor;

public static class Blackmailer
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;
    public static Color blackmailedColor = Palette.White;

    public static bool alreadyShook;
    public static PlayerControl blackmailed;
    public static PlayerControl currentTarget;
    public static float cooldown = 30f;

    public static Sprite LetterSprite = new ResourceSprite("BlackmailerLetter.png", 125);
    public static Sprite blackmailButtonSprite = new ResourceSprite("BlackmailerBlackmailButton.png");
    public static Sprite overlaySprite = new ResourceSprite("BlackmailerOverlay.png", 100);

    public static IEnumerator BlackmailShhh()
    {
        //Helpers.showFlash(new Color32(49, 28, 69, byte.MinValue), 3f, "Blackmail", false, 0.75f);
        yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0f, 0f, 0f, 0.98f));
        var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
        var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
        HudManager.Instance.shhhEmblem.transform.localPosition = new Vector3(
            HudManager.Instance.shhhEmblem.transform.localPosition.x,
            HudManager.Instance.shhhEmblem.transform.localPosition.y,
            HudManager.Instance.FullScreen.transform.position.z + 1f);
        HudManager.Instance.shhhEmblem.TextImage.text = GetString("BlackmailShhhText");
        HudManager.Instance.shhhEmblem.HoldDuration = 3f;
        yield return HudManager.Instance.ShowEmblem(true);
        HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
        HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
        yield return HudManager.Instance.CoFadeFullScreen(new Color(0f, 0f, 0f, 0.98f), Color.clear);
        yield return null;
    }

    public static void clearAndReload()
    {
        Player = null;
        currentTarget = null;
        blackmailed = null;
        alreadyShook = false;
        cooldown = CustomOptionHolder.blackmailerCooldown.GetFloat();
    }

    [HarmonyPatch]
    public static class Blackmailer_Patch
    {
        public static Sprite PrevXMark;
        public static Sprite PrevOverlay;

        public const float LetterXOffset = 0.22f;
        public const float LetterYOffset = -0.18f;


        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        private static void MeetingStartPostfix(MeetingHud __instance)
        {
            //Nothing here for now. What to do when local player who is blackmailed starts meeting
            // Blackmail target
            if (Player.IsAlive() && blackmailed.IsAlive() && blackmailed == PlayerControl.LocalPlayer)
            {
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, GetString("Blackmailer.BlackmailChat"));
                SoundEffectsManager.play("witchSpell");
                Coroutines.Start(BlackmailShhh());
            }
            else if (Player.IsDead())
            {
                blackmailed = null;
            }

            if (blackmailed != null && Player.IsAlive())
            {
                var pva = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == blackmailed.PlayerId);
                pva.XMark.gameObject.SetActive(true);
                if (PrevXMark == null) PrevXMark = pva.XMark.sprite;
                pva.XMark.sprite = LetterSprite;
                pva.XMark.transform.localScale = pva.XMark.transform.localScale * 0.75f;
                pva.XMark.transform.localPosition = new Vector3(
                    pva.XMark.transform.localPosition.x + LetterXOffset,
                    pva.XMark.transform.localPosition.y + LetterYOffset,
                    pva.XMark.transform.localPosition.z);
            }
        }


        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update)), HarmonyPostfix]
        private static void MeetingUpdatePostfix(MeetingHud __instance)
        {
            if (Player.IsAlive() && blackmailed.IsAlive())
            {
                // Blackmailer show overlay
                var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == blackmailed.PlayerId);
                playerState.Overlay.gameObject.SetActive(true);
                if (PrevOverlay == null) PrevOverlay = playerState.Overlay.sprite;
                playerState.Overlay.sprite = overlaySprite;
                if (__instance.state != MeetingHud.VoteStates.Animating && !alreadyShook)
                {
                    alreadyShook = true;
                    __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die)), HarmonyPostfix]
        private static void PlayerControlDiePostfix(PlayerControl __instance)
        {
            if (InMeeting && (__instance == Player || __instance == blackmailed) && blackmailed != null)
            {
                var pva = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == blackmailed.PlayerId);
                if (pva != null)
                {
                    if (PrevOverlay != null)
                    {
                        pva.Overlay.sprite = PrevOverlay;
                        pva.Overlay.gameObject.SetActive(blackmailed.IsDead());
                        pva.AmDead = blackmailed.IsDead();
                    }

                    if (PrevXMark != null/* && PrevOverlay != null*/)
                    {
                        pva.XMark.sprite = PrevXMark;
                        //pva.Overlay.sprite = PrevOverlay;
                        pva.XMark.transform.localScale = pva.XMark.transform.localScale * 1.334f;
                        pva.XMark.transform.localPosition = new Vector3(
                            pva.XMark.transform.localPosition.x - LetterXOffset,
                            pva.XMark.transform.localPosition.y - LetterYOffset,
                            pva.XMark.transform.localPosition.z);
                        pva.XMark.gameObject.SetActive(blackmailed.IsDead());
                    }

                    blackmailed = null;
                }
            }
        }
    }
}
