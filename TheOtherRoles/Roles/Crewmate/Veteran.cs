using Rewired;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Veteran : RoleBase, IPowerCrew
{
    public static Color color = new Color32(255, 77, 0, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Veteran),
        (p) => new Veteran(p),
        RoleId.Veteran,
        RoleType.Crewmate,
        "Veteran",
        color,
        302200,
        AddOptions
    );

    public Veteran(PlayerControl p) : base(p, roleinfo) { }

    public static float alertDuration = 3f;
    public static float cooldown = 30f;

    public CustomButton veteranAlertButton;
    public bool alertActive;

    public static Sprite buttonSprite = new ResourceSprite("Alert.png");
    public static CustomOption veteranCooldown;
    public static CustomOption veteranAlertDuration;
    public static RemoteProcess<PlayerControl> VeteranAlert = new("VeteranAlert", (player, _) =>
    {
        if (player.TryGetRole<Veteran>(out var veteran))
        {
            veteran.alertActive = true;
            var end = new LateTask(() => { veteran.alertActive = false; }, Veteran.alertDuration, "Veteran Alert End");
        }
    });

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        veteranCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "veteranCooldown", 25f, 5f, 60f, 2.5f, roleinfo.RoleOption);
        veteranAlertDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "veteranAlertDuration", 12.5f, 2.5f, 20f, 0.5f, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        alertActive = false;
        alertDuration = veteranAlertDuration.GetFloat();
        cooldown = veteranCooldown.GetFloat();
    }

    public override void CleanUp(HudManager __instance)
    {
        veteranAlertButton.Destroy();
        veteranAlertButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Veteran Alert
        veteranAlertButton?.Destroy();
        veteranAlertButton = new CustomButton(
            () =>
            {
                VeteranAlert.Invoke(PlayerControl.LocalPlayer);
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                veteranAlertButton.Timer = veteranAlertButton.MaxTimer;
                veteranAlertButton.isEffectActive = false;
                veteranAlertButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            alertDuration,
            () => { veteranAlertButton.Timer = veteranAlertButton.MaxTimer; },
            buttonText: GetString("AlertText")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = alertDuration,
        };
    }
}
