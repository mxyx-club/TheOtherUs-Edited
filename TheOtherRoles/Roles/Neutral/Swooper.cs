namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Swooper : RoleBase, INeutral
{
    public static Color color = new Color32(224, 197, 219, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Swooper),
        (p) => new Swooper(p),
        RoleId.Swooper,
        RoleType.Neutral,
        "Swooper",
        color,
        206600,
        AddOptions
    );

    public Swooper(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Kill;
    public override bool? CanUseVent => swooperCanUseVents.GetBool();
    public override bool? HasImpVision => swooperHasImpVision.GetBool();
    public override bool? IsKiller => true;

    public PlayerControl currentTarget;
    public float swoopTimer;
    public bool IsInvisable;

    public static float cooldown = 30f;
    public static float duration = 5f;
    public static float swoopCooldown = 30f;
    public static float swoopSpeed;

    public static CustomOption swooperKillCooldown;
    public static CustomOption swooperCooldown;
    public static CustomOption swooperDuration;
    public static CustomOption swooperSpeed;
    public static CustomOption swooperCanUseVents;
    public static CustomOption swooperHasImpVision;

    public CustomButton swooperSwoopButton;
    public CustomButton swooperKillButton;
    public static Sprite SwoopButtonSprite = new ResourceSprite("Swoop.png");
    public static RemoteProcess<(PlayerControl player, bool flag)> SetSwoop = new("SetSwoop", (data, _) =>
    {
        var target = data.player;

        if (data.player.TryGetRole<Swooper>(out var swooper))
        {
            if (target == null) return;
            if (!data.flag)
            {
                target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
                target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
                target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(1f);
                if (!isActiveCamoComms && !MushroomSabotageActive)
                    target.setDefaultLook();
                swooper.IsInvisable = false;
                return;
            }

            target.setLook("", 6, "", "", "", "");
            var color = Color.clear;
            var canSee = swooper.Player == PlayerControl.LocalPlayer || CanSeeGhostInfo;
            if (canSee) color.a = 0.1f;
            target.cosmetics.currentBodySprite.BodySprite.color = color;
            target.cosmetics.colorBlindText.gameObject.SetActive(false);
            target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
            swooper.swoopTimer = Swooper.duration;
            swooper.IsInvisable = true;
        }
    });

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        swooperKillCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "killCooldown", 25f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        swooperCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "swooperCooldown", 20f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        swooperDuration = CustomOption.Create(configId++, CustomOptionType.Neutral, "swooperDuration", 15f, 1f, 20f, 0.5f, roleinfo.RoleOption);
        swooperSpeed = CustomOption.Create(configId++, CustomOptionType.Neutral, "swooperSpeed", 1.5f, 1f, 3f, 0.125f, roleinfo.RoleOption);
        swooperCanUseVents = CustomOption.Create(configId++, CustomOptionType.Neutral, "canUseVents", true, roleinfo.RoleOption);
        swooperHasImpVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "hasImpVision", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        Player = null;
        IsInvisable = false;
        cooldown = swooperKillCooldown.GetFloat();
        swoopCooldown = swooperCooldown.GetFloat();
        duration = swooperDuration.GetFloat();
        swoopSpeed = swooperSpeed.GetFloat();
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        swoopTimer -= Time.deltaTime;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (IsInvisable && swoopTimer <= 0 && Player == player)
        {
            SetSwoop.Invoke((Player, false));
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        swooperKillButton?.Destroy();
        swooperSwoopButton?.Destroy();
        swooperKillButton = null;
        swooperSwoopButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Swooper Kill
        swooperKillButton?.Destroy();
        swooperKillButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, currentTarget);

                swooperKillButton.Timer = swooperKillButton.MaxTimer;
                currentTarget = null;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                var untargetablePlayers = new List<PlayerControl>();
                untargetablePlayers.AddRange(Mini.UngrownMinis);
                currentTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(currentTarget, Palette.ImpostorRed);

                swooperKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));
                return currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { swooperKillButton.Timer = swooperKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode
        )
        {
            MaxTimer = cooldown,
        };

        swooperSwoopButton?.Destroy();
        swooperSwoopButton = new CustomButton(
            () =>
            {
                SetSwoop.Invoke((Player, true));
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                // Exclude Jackal from targeting the Mini unless it has grown up
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                swooperSwoopButton.Timer = swooperSwoopButton.MaxTimer;
                swooperSwoopButton.isEffectActive = false;
                swooperSwoopButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                IsInvisable = false;
            },
            SwoopButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            duration,
            () => { swooperSwoopButton.Timer = swooperSwoopButton.MaxTimer; },
            buttonText: GetString("SwoopText")
        )
        {
            MaxTimer = swoopCooldown,
            EffectDuration = duration,
        };
    }
}
