namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Camouflager : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Camouflager),
        (p) => new Camouflager(p),
        RoleId.Camouflager,
        RoleType.Impostor,
        "Camouflager",
        color,
        101400,
        AddOptions
    );

    public Camouflager(PlayerControl p) : base(p, roleInfo) { }

    public static float cooldown = 30f;
    public static float duration = 10f;

    public static float CamoTimer;

    public static CustomOption camouflagerCooldown;
    public static CustomOption camouflagerDuration;

    public CustomButton camouflagerButton;
    public static Sprite buttonSprite = new ResourceSprite("CamoButton.png");
    public static RemoteProcess<float> CamouflagerCamouflage = new("CamouflagerCamouflage", (time, _) =>
    {
        Camouflager.CamoTimer = time;
    });

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        camouflagerCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "camouflagerCooldown", 25f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        camouflagerDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "camouflagerDuration", 12.5f, 1f, 20f, 0.5f, roleInfo.RoleOption);
    }

    public static void resetCamouflage()
    {
        if (isCamoComms) return;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
        {
            if ((p.TryGetRole<Ninja>(out var ninja) && ninja.isInvisable)
                 || (p.TryGetRole<Swooper>(out var swooper) && swooper.IsInvisable)
                 || (p.TryGetRole<Jackal>(out var jackal) && jackal.IsInvisable))
            {
                continue;
            }

            p.setDefaultLook();
            CamoTimer = 0f;
        }
    }

    public override void Initialize()
    {
        resetCamouflage();
        CamoTimer = 0f;
        cooldown = camouflagerCooldown.GetFloat();
        duration = camouflagerDuration.GetFloat();
    }


    public override void OnMeetingStart(MeetingHud __instance)
    {
        CamoTimer = 0f;
    }

    public override void CleanUp(HudManager __instance)
    {
        camouflagerButton?.Destroy();
        camouflagerButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Camouflager camouflage
        camouflagerButton?.Destroy();
        camouflagerButton = new CustomButton(
            () =>
            {
                CamouflagerCamouflage.Invoke(1);
                SoundEffectsManager.play("morphlingMorph");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () => { return !isActiveCamoComms && PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                camouflagerButton.Timer = camouflagerButton.MaxTimer;
                camouflagerButton.isEffectActive = false;
                camouflagerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            duration,
            () =>
            {
                camouflagerButton.Timer = camouflagerButton.MaxTimer;
                SoundEffectsManager.play("morphlingMorph");
            },
            buttonText: GetString("CamouflageText")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = duration,
        };
    }
}