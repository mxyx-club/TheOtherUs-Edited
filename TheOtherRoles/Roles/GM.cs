namespace TheOtherRoles.Roles;

public class GM : RoleBase
{
    public static Color color = new Color32(255, 91, 112, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(GM),
        (p) => new GM(p),
        RoleId.GM,
        RoleType.Special,
        "GM",
        color,
        480000,
        null,
        false,
        true);

    public GM(PlayerControl p) : base(p, roleinfo, false) { }

    public static bool IsEnabled;
    public static bool IsHost;
    public static bool GameStartIsDie;

    public override bool? HasImpVision { get; set; } = true;

    public override void Initialize()
    {
        IsEnabled = CustomOptionHolder.gmEnabled.GetBool();
        IsHost = CustomOptionHolder.gmIsHost.GetBool();
        GameStartIsDie = CustomOptionHolder.gmDiesAtStart.GetBool();
    }

    public override void OnGameStart()
    {
        Player.Die(DeathReason.Exile, false);
    }
}
