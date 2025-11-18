namespace TheOtherRoles.Roles.Modifier;

public class Tunneler : ModifierBase
{
    public static Color color = Color.yellow;
    public static bool NoTask;
    public static CustomOption modifierTunnelerNoTask;
    public static readonly RoleInfo roleinfo = new(
        typeof(Tunneler),
        (p) => new Tunneler(p),
        RoleId.Tunneler,
        "Tunneler",
        color,
        402700,
        AddOptions
    );
    public Tunneler(PlayerControl p) : base(p, roleinfo) { }
    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierTunnelerNoTask = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierTunnelerNoTask", false, roleinfo.RoleOption);
    }
    public override void Initialize()
    {
        NoTask = modifierTunnelerNoTask.GetBool();
    }
}
