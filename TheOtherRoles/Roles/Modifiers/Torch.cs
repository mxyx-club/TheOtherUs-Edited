namespace TheOtherRoles.Roles.Modifier;

public class Torch : ModifierBase
{
    public static Color color = Color.yellow;
    public static float vision = 1;
    public static CustomOption modifierTorchVision;
    public static readonly RoleInfo roleinfo = new(
        typeof(Torch),
        (p) => new Torch(p),
        RoleId.Torch,
        "Torch",
        color,
        401800,
        AddOptions
    );
    public Torch(PlayerControl p) : base(p, roleinfo) { }
    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierTorchVision = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierTorchVision", 1.5f, 1f, 3f, 0.125f, roleinfo.RoleOption);
    }
    public override void Initialize()
    {
        vision = modifierTorchVision.GetFloat();
    }
}
