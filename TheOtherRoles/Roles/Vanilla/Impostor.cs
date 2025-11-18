namespace TheOtherRoles.Roles.Vanilla;
public class Impostor : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleInfo = new(
        typeof(Impostor),
        (p) => new Impostor(p),
        RoleId.Impostor,
        RoleType.Impostor,
        "Impostor",
        color,
        100000,
        null,
        false
    );

    public Impostor(PlayerControl p) : base(p, roleInfo) { }

    public override void Initialize() { }
}
