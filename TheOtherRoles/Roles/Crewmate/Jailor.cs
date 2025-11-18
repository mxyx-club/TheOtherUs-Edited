using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Jailor : RoleBase
{
    public static Color color = new Color32(166, 166, 166, byte.MaxValue);
    public static Dictionary<byte, byte> JailorList = new();

    public static readonly RoleInfo roleinfo = new(
        typeof(Jailor),
        (p) => new Jailor(p),
        RoleId.Jailor,
        RoleType.Crewmate,
        "Jailor",
        color,
        304100,
        AddOptions
    );

    public PlayerControl Jailed;
    public PlayerControl currentTarget;
    public int usesCount;
    public float cooldown = 10f;

    public static Sprite buttonSprite = new ResourceSprite("Jail.png");
    public static Sprite jailedSprite = new ResourceSprite("InJail.png", 95);
    public static Sprite Jail = new ResourceSprite("JailCell.png", 105);
    public static Sprite TargetSprite = new ResourceSprite("TargetIcon.png", 150);

    public static CustomOption jailorCooldown;
    public static CustomOption jailorUseCount;

    public CustomButton jailorButton;
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> JailPlayer = new("JailPlayer", (data, _) =>
    // public static void JailPlayer(PlayerControl player, PlayerControl target)
    {
        var role = data.player.GetRole<Jailor>();
        role?.Jailed = data.target;
        JailorList[data.player.PlayerId] = data.target.PlayerId;
    });

    public static bool IsJailed(PlayerControl jailed, out PlayerControl jailor)
    {
        jailor = null;
        if (jailed == null) return false;
        foreach (var pair in JailorList)
        {
            if (pair.Value == jailed.PlayerId)
            {
                jailor = PlayerById(pair.Key);
                if (jailor.IsDead()) return false; ;
                return true;
            }
        }
        return false;
    }

    public Jailor(PlayerControl p) : base(p, roleinfo) { }

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        jailorCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "jailorCooldown", 20f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        jailorUseCount = CustomOption.Create(configId++, CustomOptionType.Crewmate, "jailorUseCount", 3, 1, 15, 1, roleinfo.RoleOption);
    }

    public static void ExiledJailed(PlayerControl player, PlayerControl target)
    {
        if (player.IsDead()) return;
        var role = player.GetRole<Jailor>();
        if (Guesser.guesserUI != null) Guesser.guesserUIExitButton.OnClick.Invoke();

        target.Exiled();
        PlayerData.SetDeathReason(target, CustomDeathReason.Jailed, player);

        HudManager.Instance.KillOverlay.ShowKillAnimation(target.Data, target.Data);

        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(target.KillSfx, false, 0.8f);

        if (MeetingHud.Instance)
        {
            ExtendMeetingTime(CustomOptionHolder.guessExtendmeetingTime.GetFloat());

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
            role.usesCount = 0;
        }

        JailorList.Remove(player.PlayerId);
    }

    public static void JailorSendMessage(PlayerControl player, string message)
    {
        ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.JailorChat;
        var role = player.GetRole<Jailor>();
        if (role == null) return;
        message = Cs(Color.red, message);
        if (PlayerControl.LocalPlayer == role.Jailed || CanSeeGhostInfo)
        {
            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(role.Jailed, message);
            Message($"SendMessage: {message}");
        }
        else if (PlayerControl.LocalPlayer == role.Player)
        {
            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(role.Player, message);
            Message($"SendMessage: {message}");
        }
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (JailorList.TryGetValue(PlayerId, out _))
        {
            JailorList.Remove(PlayerId);
        }
    }

    public override void Initialize()
    {
        Jailed = null;
        currentTarget = null;
        cooldown = jailorCooldown.GetFloat();
        usesCount = jailorUseCount.GetInt();
    }

    public override void CleanUp(HudManager __instance)
    {
        jailorButton?.Destroy();
        jailorButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        jailorButton?.Destroy();
        jailorButton = new CustomButton(
            () =>
            {
                var target = currentTarget;
                if (target == null) return;
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;

                // var writer = StartRPC(CustomRPC.JailorJail);
                // writer.Write(PlayerControl.LocalPlayer.PlayerId);
                // writer.Write(target.PlayerId);
                // writer.EndRPC();
                JailPlayer.Invoke((PlayerControl.LocalPlayer, target));

                SoundEffectsManager.play("deputyHandcuff");

                jailorButton.Timer = jailorButton.MaxTimer;
                currentTarget = null;
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer && usesCount > 0;
            },
            () =>
            {
                jailorButton.UsesCount = usesCount;
                currentTarget = SetTarget();
                SetPlayerOutline(currentTarget, color);
                jailorButton.showTargetNameOnButton(currentTarget);

                return PlayerControl.LocalPlayer.CanMove && currentTarget != null;
            },
            () => { jailorButton.Timer = jailorButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("jailButtonText")
        );
    }
}
