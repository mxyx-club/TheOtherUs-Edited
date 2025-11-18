namespace TheOtherRoles.Roles.Modifier;

public class Indomitable : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Indomitable),
        (p) => new Indomitable(p),
        RoleId.Indomitable,
        "Indomitable",
        color,
        402300
    );

    public Indomitable(PlayerControl p) : base(p, roleinfo) { }

    public override void Initialize() { }
}
