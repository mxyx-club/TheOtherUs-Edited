namespace TheOtherRoles.Roles.Modifier;

public class Slueth : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Slueth),
        (p) => new Slueth(p),
        RoleId.Slueth,
        "Slueth",
        color,
        402900
    );

    public Slueth(PlayerControl p) : base(p, roleinfo) { }

    public HashSet<PlayerControl> reported = new();

    public override void OnReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target)
    {
        if (reporter == Player && target != null)
        {
            var report = PlayerById(target.PlayerId);
            reported.Add(report);
        }

    }

    public override void Initialize()
    {
        reported.Clear();
    }
}
