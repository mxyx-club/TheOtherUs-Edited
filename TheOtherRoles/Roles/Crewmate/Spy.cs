namespace TheOtherRoles.Roles.Crewmate;

public class Spy : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleinfo = new(
        typeof(Spy),
        (p) => new Spy(p),
        RoleId.Spy,
        RoleType.Crewmate,
        "Spy",
        color,
        302800,
        AddOptions
    );

    public static readonly List<Spy> AllSpy = new();

    public Spy(PlayerControl p) : base(p, roleinfo) { AllSpy.Add(this); }

    public static bool impostorsCanKillAnyone = true;
    public static bool canEnterVents;
    public static bool hasImpostorVision;

    public static CustomOption spyCanDieToSheriff;
    public static CustomOption spyImpostorsCanKillAnyone;
    public static CustomOption spyCanEnterVents;
    public static CustomOption spyHasImpostorVision;

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        spyCanDieToSheriff = CustomOption.Create(configId++, CustomOptionType.Crewmate, "spyCanDieToSheriff", false, roleinfo.RoleOption);
        spyImpostorsCanKillAnyone = CustomOption.Create(configId++, CustomOptionType.Crewmate, "spyImpostorsCanKillAnyone", true, roleinfo.RoleOption);
        spyCanEnterVents = CustomOption.Create(configId++, CustomOptionType.Crewmate, "canUseVents", true, roleinfo.RoleOption);
        spyHasImpostorVision = CustomOption.Create(configId++, CustomOptionType.Crewmate, "hasImpVision", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        impostorsCanKillAnyone = spyImpostorsCanKillAnyone.GetBool();
        canEnterVents = spyCanEnterVents.GetBool();
        hasImpostorVision = spyHasImpostorVision.GetBool();
    }
}
