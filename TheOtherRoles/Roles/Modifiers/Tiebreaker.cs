namespace TheOtherRoles.Roles.Modifier;

public class Tiebreaker : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Tiebreaker),
        (p) => new Tiebreaker(p),
        RoleId.Tiebreaker,
        "TieBreaker",
        color,
        401400
    );

    public Tiebreaker(PlayerControl p) : base(p, roleinfo) { }

    public override void Initialize() { }
}
