namespace TheOtherRoles.Roles.Neutral;

public class Werewolf : RoleBase, INeutral
{
    public static Color color = new Color32(79, 56, 21, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Werewolf),
        (p) => new Werewolf(p),
        RoleId.Werewolf,
        RoleType.Neutral,
        "Werewolf",
        color,
        206700,
        AddOptions
    );

    public Werewolf(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Kill;
    public override bool? CanUseVent => Rampage || Player.inVent;
    public override bool? HasImpVision => Rampage;

    public PlayerControl currentTarget;
    public bool Rampage;

    public static float killCooldown = 3f;
    public static float rampageCooldown = 30f;
    public static float rampageDuration = 5f;

    public static CustomOption werewolfRampageCooldown;
    public static CustomOption werewolfRampageDuration;
    public static CustomOption werewolfKillCooldown;

    public CustomButton werewolfRampageButton;
    public CustomButton werewolfKillButton;
    public static ResourceSprite buttonSprite = new("Rampage.png");

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        werewolfRampageCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "werewolfRampageCooldown", 25f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        werewolfRampageDuration = CustomOption.Create(configId++, CustomOptionType.Neutral, "werewolfRampageDuration", 15f, 0.5f, 20f, 0.5f, roleinfo.RoleOption);
        werewolfKillCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "werewolfKillCooldown", 3f, 1f, 60f, 0.5f, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        currentTarget = null;
        Rampage = false;
        rampageCooldown = werewolfRampageCooldown.GetFloat();
        rampageDuration = werewolfRampageDuration.GetFloat();
        killCooldown = werewolfKillCooldown.GetFloat();
    }

    public override void CleanUp(HudManager __instance)
    {
        werewolfKillButton?.Destroy();
        werewolfRampageButton?.Destroy();
        werewolfKillButton = null;
        werewolfRampageButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Werewolf Kill
        werewolfKillButton?.Destroy();
        werewolfKillButton = new CustomButton(
            () =>
            {
                if (currentTarget == null) return;
                RpcCustomMurderPlayer(currentTarget, PlayerControl.LocalPlayer);
                werewolfKillButton.Timer = werewolfKillButton.MaxTimer;
                currentTarget = null;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && Rampage;
            },
            () =>
            {
                currentTarget = SetTarget();
                werewolfKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));
                return currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { werewolfKillButton.Timer = werewolfKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        )
        {
            MaxTimer = killCooldown,
        };

        werewolfRampageButton = new CustomButton(
            () =>
            {
                Rampage = true;
                werewolfKillButton.Timer = 0f;
                werewolfRampageButton.PositionOffset = new Vector3(0, 1.92f, 0);
            },
            () =>
            {
                // Can See
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                // On Click
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                // On Meeting End
                werewolfRampageButton.Timer = werewolfRampageButton.MaxTimer;
                werewolfRampageButton.isEffectActive = false;
                werewolfRampageButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Rampage = false;
                //werewolfRampageButton.PositionOffset = CustomButton.ButtonPositions.upperRowRight;
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.modKillInput.keyCode,
            true,
            rampageDuration,
            () =>
            {
                //werewolfRampageButton.PositionOffset = CustomButton.ButtonPositions.upperRowRight;
                werewolfRampageButton.Timer = werewolfRampageButton.MaxTimer;
                Rampage = false;
            },
            buttonText: "WerewolfRampage".Translate()
        )
        {
            MaxTimer = rampageCooldown,
            EffectDuration = rampageDuration,
        };
    }
}
