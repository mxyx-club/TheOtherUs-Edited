namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Blackmailer : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Blackmailer),
        (p) => new Blackmailer(p),
        RoleId.Blackmailer,
        RoleType.Impostor,
        "Blackmailer",
        color,
        102600,
        AddOptions
    );

    public Blackmailer(PlayerControl p) : base(p, roleInfo) { }

    public static Dictionary<PlayerControl, byte> blackmailedBy = new();
    public PlayerControl currentTarget;
    public PlayerControl blackmailed;

    public static Color blackmailedColor = Palette.White;
    public static bool alreadyShook;
    public static float cooldown = 30f;
    public static CustomOption blackmailerCooldown;

    public CustomButton blackmailerButton;
    public static Sprite blackmailButtonSprite = new ResourceSprite("BlackmailerBlackmailButton.png");
    public static Sprite overlaySprite = new ResourceSprite("BlackmailerOverlay.png", 100);
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> BlackmailPlayer = new("BlackmailPlayer", (data, _) =>
    {
        if (data.player == null || data.target == null) return;
        if (data.player.TryGetRole<Blackmailer>(out var blackmailer))
        {
            blackmailer.blackmailed = data.target;
            Blackmailer.blackmailedBy[data.player] = data.target.PlayerId;
        }
    });

    public static bool IsBlackmailed(PlayerControl player)
    {
        if (player == null || blackmailedBy.Count == 0)
            return false;
        foreach (var pair in blackmailedBy)
        {
            if (pair.Key.IsAlive() && pair.Value == player.PlayerId)
                return true;
        }
        return false;
    }


    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        blackmailerCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "blackmailerCooldown", 15f, 5f, 120f, 2.5f, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        currentTarget = null;
        blackmailed = null;
        alreadyShook = false;
        blackmailedBy.Remove(Player);
        cooldown = blackmailerCooldown.GetFloat();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        //Nothing here for now. What to do when local player who is blackmailed starts meeting
        if (PlayerControl.LocalPlayer.IsAlive() && IsBlackmailed(PlayerControl.LocalPlayer))
            Coroutines.Start(BlackmailShhh());
    }

    public override void OnMeetingUpdate(MeetingHud __instance)
    {
        if (Player != null && blackmailed != null)
        {
            // Blackmailer show overlay
            var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == blackmailed.PlayerId);
            playerState.Overlay.gameObject.SetActive(true);
            playerState.Overlay.sprite = overlaySprite;
            if (__instance.state != MeetingHud.VoteStates.Animating && !alreadyShook)
            {
                alreadyShook = true;
                __instance.StartCoroutine(Effects.SwayX(playerState.transform));
            }
        }
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        blackmailedBy.Clear();
        alreadyShook = false;
    }

    public override void CleanUp(HudManager __instance)
    {
        blackmailerButton?.Destroy();
        blackmailerButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        blackmailerButton?.Destroy();
        blackmailerButton = new CustomButton(
            () =>
            {
                // Action when Pressed
                var target = currentTarget;
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;
                BlackmailPlayer.Invoke((PlayerControl.LocalPlayer, target));
                currentTarget = null;
                blackmailerButton.Timer = 1f;
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                // Could Use
                currentTarget = SetTarget();
                SetPlayerOutline(currentTarget, blackmailedColor);

                if (blackmailed == null)
                {
                    blackmailerButton.showTargetNameOnButton(currentTarget, GetString("BlackmailerText"));
                }
                return currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { blackmailerButton.Timer = blackmailerButton.MaxTimer; },
            blackmailButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("BlackmailerText")
        )
        {
            MaxTimer = cooldown
        };
    }
}
