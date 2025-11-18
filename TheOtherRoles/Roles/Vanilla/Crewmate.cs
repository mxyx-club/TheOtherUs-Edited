namespace TheOtherRoles.Roles.Vanilla;

public class Crewmate : RoleBase
{
    public static Color color = Palette.CrewmateBlue;

    public static readonly RoleInfo roleInfo = new(
        typeof(Crewmate),
        (p) => new Crewmate(p),
        RoleId.Crewmate,
        RoleType.Crewmate,
        "Crewmate",
        color,
        300000,
        null,
        false
    );

    public Crewmate(PlayerControl p) : base(p, roleInfo) { }

    public override void Initialize() { }
}
