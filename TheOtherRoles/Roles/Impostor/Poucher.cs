namespace TheOtherRoles.Roles.Impostor;

public class Poucher : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Poucher),
        (p) => new Poucher(p),
        RoleId.Poucher,
        RoleType.Impostor,
        "Poucher",
        color,
        103200,
        null
    );

    public Poucher(PlayerControl p) : base(p, roleInfo) { }

    public List<PlayerControl> killed = new();

    public override void Initialize()
    {
        killed.Clear();
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Info.Killer == Player && Info.Target != null)
        {
            killed.Add(Info.Target);
        }
    }
}
