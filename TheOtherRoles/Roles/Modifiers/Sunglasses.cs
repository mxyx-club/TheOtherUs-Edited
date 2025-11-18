namespace TheOtherRoles.Roles.Modifier;

public class Sunglasses : ModifierBase
{
    public static Color color = Color.yellow;
    public static int vision = 1;
    public static CustomOption modifierSunglassesVision;
    public static readonly RoleInfo roleinfo = new(
        typeof(Sunglasses),
        (p) => new Sunglasses(p),
        RoleId.Sunglasses,
        "Sunglasses",
        color,
        401700,
        AddOptions
    );
    public Sunglasses(PlayerControl p) : base(p, roleinfo) { }
    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierSunglassesVision = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierSunglassesVision", ["-10%", "-20%", "-30%", "-40%", "-50%"], roleinfo.RoleOption);
    }
    public override void Initialize()
    {
        vision = modifierSunglassesVision.GetSelection() + 1;
    }
}
