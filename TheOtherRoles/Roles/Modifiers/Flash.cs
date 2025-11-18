namespace TheOtherRoles.Roles.Modifier;

public class Flash : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Flash),
        (p) => new Flash(p),
        RoleId.Flash,
        "Flash",
        color,
        401900,
        AddOptions
    );

    public Flash(PlayerControl p) : base(p, roleinfo) { }

    public static float speed = 1.5f;
    public static CustomOption modifierFlashSpeed;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierFlashSpeed = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierFlashSpeed", 1.25f, 1f, 3f, 0.125f, roleinfo.RoleOption);
    }
    public override void Initialize()
    {
        speed = modifierFlashSpeed.GetFloat();
    }
}
