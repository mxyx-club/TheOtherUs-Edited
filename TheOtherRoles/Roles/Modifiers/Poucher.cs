namespace TheOtherRoles.Roles.Modifier;

public class PoucherA : ModifierBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleinfo = new(
        typeof(PoucherA),
        (p) => new PoucherA(p),
        RoleId.PoucherA,
        "Poucher",
        color,
        403600
    );

    public PoucherA(PlayerControl p) : base(p, roleinfo) { }

    public static bool spawnModifier;
    public HashSet<PlayerControl> killed = new();

    public override RoleType[] RemoveTeam { get; set; } = [RoleType.Neutral, RoleType.Crewmate];

    public override void Initialize()
    {
        killed.Clear();
        spawnModifier = roleinfo.RoleOption.GetBool();
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Info.Killer == Player)
        {
            killed.Add(Info.Target);
        }
    }
}
