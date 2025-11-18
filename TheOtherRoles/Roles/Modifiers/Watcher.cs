namespace TheOtherRoles.Roles.Modifier;

public class Watcher : ModifierBase
{
    public static Color color = Color.yellow;
    public static readonly RoleInfo roleinfo = new(
        typeof(Watcher),
        (p) => new Watcher(p),
        RoleId.Watcher,
        "Watcher",
        color,
        402500
    );
    public Watcher(PlayerControl p) : base(p, roleinfo) { }
    public override void Initialize() { }
}
