namespace TheOtherRoles.Roles.Modifier;

[CustomRpcHolder]
public class LastImpostor : ModifierBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleinfo = new(
        typeof(LastImpostor),
        (p) => new LastImpostor(p),
        RoleId.LastImpostor,
        "LastImpostor",
        color,
        401100,
        AddOptions,
        false
    );

    public LastImpostor(PlayerControl p) : base(p, roleinfo) { }

    public static float deduce = 2.5f;
    public static bool isEnable;

    public static CustomOption modifierLastImpostorDeduce;
    public static CustomOption modifierLastImpostor;
    public static RemoteProcess<PlayerControl> ImpostorPromotesToLastImpostor = new("ImpostorPromotesToLastImpostor", (target, _) =>
    {
        GetModifier(RoleId.LastImpostor).Do(x => x.Destroy());
        CustomRoleManager.CreateRole(target, RoleId.LastImpostor);
    });


    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierLastImpostor = CustomOption.Create(configId++, CustomOptionType.Modifiers, Cs(color, "LastImpostor"), false, null, true);
        modifierLastImpostorDeduce = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierLastImpostorDeduce", 5f, 2.5f, 15f, 2.5f, modifierLastImpostor);
    }

    public override void Initialize()
    {
        deduce = modifierLastImpostorDeduce.GetFloat();
        isEnable = modifierLastImpostor.GetBool();
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (!isEnable || !InGame || (ModOption.NumImpostors == 1 && !ModOption.DebugMode)) return;

        if (PlayerControl.LocalPlayer.IsImpostor() && PlayerControl.LocalPlayer.IsAlive() && Player != PlayerControl.LocalPlayer
            && PlayerControl.AllPlayerControls.Count(x => x.IsImpostor() && x.IsAlive()) == 1)
        {
            ImpostorPromotesToLastImpostor.Invoke(PlayerControl.LocalPlayer);
        }
    }
}
