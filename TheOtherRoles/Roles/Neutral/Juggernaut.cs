namespace TheOtherRoles.Roles.Neutral;

public class Juggernaut : RoleBase, INeutral
{
    public static Color color = new Color32(140, 0, 77, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Juggernaut),
        (p) => new Juggernaut(p),
        RoleId.Juggernaut,
        RoleType.Neutral,
        "Juggernaut",
        color,
        206800,
        AddOptions,
        IsKiller: true
    );

    public Juggernaut(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Kill;
    public override bool? CanUseVent => juggernautCanUseVents.GetBool();
    public override bool? HasImpVision => juggernautHasImpVision.GetBool();

    public PlayerControl currentTarget;

    public static float cooldown = 30f;
    public static float reducedkill = 5f;

    public static CustomOption juggernautCooldown;
    public static CustomOption juggernautHasImpVision;
    public static CustomOption juggernautCanUseVents;
    public static CustomOption juggernautReducedkillEach;

    public CustomButton juggernautKillButton;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        juggernautCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "killCooldown", 25f, 2.5f, 60f, 2.5f, roleinfo.RoleOption);
        juggernautHasImpVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "hasImpVision", true, roleinfo.RoleOption);
        juggernautCanUseVents = CustomOption.Create(configId++, CustomOptionType.Neutral, "canUseVents", true, roleinfo.RoleOption);
        juggernautReducedkillEach = CustomOption.Create(configId++, CustomOptionType.Neutral, "juggernautReducedkillEach", 5f, 1f, 15f, 0.5f, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        Player = null;
        currentTarget = null;
        cooldown = juggernautCooldown.GetFloat();
        reducedkill = juggernautReducedkillEach.GetFloat();
    }

    public override void CleanUp(HudManager __instance)
    {
        juggernautKillButton?.Destroy();
        juggernautKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // 天启击杀 Kill
        juggernautKillButton?.Destroy();
        juggernautKillButton = new CustomButton(
            () =>
            {
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, currentTarget)) return;

                cooldown = Mathf.Max(0, cooldown - reducedkill);
                juggernautKillButton.MaxTimer = cooldown;

                juggernautKillButton.Timer = juggernautKillButton.MaxTimer;
                currentTarget = null;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                currentTarget = SetTarget();
                juggernautKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));
                return currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { juggernautKillButton.Timer = juggernautKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
