namespace TheOtherRoles.Roles.Modifier;

public class Multitasker : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Multitasker),
        (p) => new Multitasker(p),
        RoleId.Multitasker,
        "Multitasker",
        color,
        402000
    );

    public Multitasker(PlayerControl p) : base(p, roleinfo) { }

    public override void Initialize() { }
}
