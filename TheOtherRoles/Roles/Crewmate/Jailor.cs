using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Crewmate;

public class Jailor
{
    public static PlayerControl Player;
    public static Color color = new Color32(166, 166, 166, byte.MaxValue);

    public static PlayerControl currentTarget;
    public static PlayerControl Jailed;

    public static int usesCount;
    public static float cooldown = 10f;

    public static Sprite buttonSprite = new ResourceSprite("Jail.png");
    public static Sprite jailedSprite = new ResourceSprite("InJail.png", 95);
    public static Sprite Jail = new ResourceSprite("JailCell.png", 105);
    public static Sprite TargetSprite = new ResourceSprite("TargetIcon.png", 150);

    public static void MeetingStart(MeetingHud __instance)
    {
        if (Jailed.IsDead() || Player == Blackmailer.blackmailed || Player.IsDead())
        {
            Jailed = null;
            return;
        }

        if (PlayerControl.LocalPlayer == Jailed)
        {
            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, GetString("Jailor.JailedChat"));
        }
        else if (PlayerControl.LocalPlayer == Player)
        {
            usesCount--;
            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, GetString("Jailor.JailorChat"));
        }

        if (PlayerControl.LocalPlayer == Jailed && Jailed != Blackmailer.blackmailed)
        {
            Coroutines.Start(ShowJailed());
            SoundManager.Instance.PlaySound(UnityHelper.loadAudioClipFromResources("TheOtherRoles.Resources.Balancer.chain.raw"), false, 0.6f);
        }

        var pva = __instance.playerStates?.FirstOrDefault(x => x.TargetPlayerId == Jailed?.PlayerId);
        if (pva == null) return;
        GameObject template = pva.Buttons.transform.Find("CancelButton").gameObject;
        GameObject jailCell = UObject.Instantiate(template, pva.transform);
        jailCell.name = "JailCell";
        var parent = template.transform.parent.parent;
        var cellRenderer = jailCell.GetComponent<SpriteRenderer>();
        var passive = jailCell.GetComponent<PassiveButton>();
        cellRenderer.sprite = jailedSprite;
        jailCell.transform.localPosition = new Vector3(-0.95f, 0.01f, -2f);
        jailCell.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        jailCell.layer = 5;
        jailCell.transform.parent = parent;
        jailCell.transform.GetChild(0).gameObject.Destroy();
        passive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();

        if (PlayerControl.LocalPlayer != Player) return;
        GameObject targetBox = UObject.Instantiate(template, pva.transform);
        targetBox.name = "JailTargetIcon";
        targetBox.transform.localPosition = new Vector3(1f, 0.03f, -1f);
        SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
        renderer.sprite = TargetSprite;
        renderer.color = Color.red;
        PassiveButton button = targetBox.GetComponent<PassiveButton>();
        button.OnClick.RemoveAllListeners();
        button.OnClick.AddListener(() =>
        {
            if (__instance.state is MeetingHud.VoteStates.Results or MeetingHud.VoteStates.Discussion) return;
            var writer = StartRPC(CustomRPC.ExiledJailed);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(Jailed.PlayerId);
            writer.EndRPC();
            ExiledJailed(PlayerControl.LocalPlayer, Jailed);
            targetBox?.Destroy();
        });
    }

    public static void ExiledJailed(PlayerControl player, PlayerControl target)
    {
        if (player.IsDead()) return;

        if (Guesser.guesserUI != null) Guesser.guesserUIExitButton.OnClick.Invoke();

        target.SetDie();
        Avenger.OnPlayerDeath(player, target);

        PlayerData.SetDeathReason(target, CustomDeathReason.Jailed, player);

        foreach (var playerState in MeetingHud.Instance.playerStates)
        {
            if (playerState.TargetPlayerId != target.PlayerId) continue;
            playerState.transform.FindChild("JailCell")?.gameObject?.Destroy();
            playerState.transform.FindChild("JailTargetIcon")?.gameObject?.Destroy();
        }
        Jailed = null;

        HudManager.Instance.KillOverlay.ShowKillAnimation(target.Data, target.Data);

        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(target.KillSfx, false, 0.8f);

        if (MeetingHud.Instance)
        {
            ExtendMeetingTime(CustomOptionHolder.guessExtendmeetingTime.GetFloat());
            MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, target.PlayerId);

            var partner = target.GetPartner();
            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                bool shouldClearVote = CustomOptionHolder.guessReVote.GetBool()
                    || (target != null && pva.VotedFor == target.PlayerId)
                    || (partner != null && pva.VotedFor == partner.PlayerId);

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

        if (target.IsCrew())
        {
            usesCount = 0;
        }
    }

    public static IEnumerator ShowJailed()
    {
        yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0f, 0f, 0f, 0.8f));
        var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
        var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
        HudManager.Instance.shhhEmblem.transform.localPosition = new Vector3(
            TempPosition.x, TempPosition.y,
            HudManager.Instance.FullScreen.transform.position.z + 1f);
        HudManager.Instance.shhhEmblem.TextImage.text = "Jailor.YouAreJailed".Translate();
        HudManager.Instance.shhhEmblem.Body.sprite = Jail;
        HudManager.Instance.shhhEmblem.Hand.sprite = null;
        HudManager.Instance.shhhEmblem.Background.sprite = null;
        HudManager.Instance.shhhEmblem.HoldDuration = 2.5f;
        yield return HudManager.Instance.ShowEmblem(true);
        HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
        HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
        yield return HudManager.Instance.CoFadeFullScreen(new Color(0f, 0f, 0f, 0.8f), Color.clear);
        yield return null;
    }

    public static void JailPlayer(PlayerControl player, PlayerControl target)
    {
        Jailed = target;
    }

    public static void JailorSendMessage(PlayerControl player, string message)
    {
        ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.JailorChat;
        message = Cs(Color.red, message);
        if (PlayerControl.LocalPlayer == Jailed || CanSeeGhostInfo)
        {
            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Jailed, message);
            SoundManager.Instance.PlaySound(HudManager.Instance?.Chat?.messageSound, false, 1f, null);
        }
        else if (PlayerControl.LocalPlayer == Player)
        {
            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Player, message);
        }
    }

    public static void ClearAndReload()
    {
        Player = null;
        currentTarget = null;
        Jailed = null;
        usesCount = CustomOptionHolder.jailorUseCount.GetInt();
        cooldown = CustomOptionHolder.jailorCooldown.GetFloat();
    }
}
