namespace TheOtherRoles.Roles.Crewmate;
public class Vigilante : GuesserBase, IPowerCrew, IGuesser
{
    public static Color color = new Color32(255, 255, 0, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Vigilante),
        (p) => new Vigilante(p),
        RoleId.Vigilante,
        RoleType.Crewmate,
        "Vigilante",
        color,
        301000,
        AddOptions
    );

    public Vigilante(PlayerControl p) : base(p, roleinfo) { }

    public override int Charges { get; set; }
    public static bool hasMultipleShotsPerMeeting;
    public static bool killsThroughShield = true;

    public static CustomOption guesserNumberOfShots;
    public static CustomOption guesserHasMultipleShotsPerMeeting;
    public static CustomOption guesserShowInfoInGhostChat;
    public static CustomOption guesserKillsThroughShield;

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        guesserNumberOfShots = CustomOption.Create(configId++, CustomOptionType.Crewmate, "guesserNumberOfShots", 3f, 1f, 15f, 1f, roleinfo.RoleOption);
        guesserHasMultipleShotsPerMeeting = CustomOption.Create(configId++, CustomOptionType.Crewmate, "guesserHasMultipleShotsPerMeeting", true, roleinfo.RoleOption);
        guesserShowInfoInGhostChat = CustomOption.Create(configId++, CustomOptionType.Crewmate, "guesserShowInfoInGhostChat", true, roleinfo.RoleOption);
        guesserKillsThroughShield = CustomOption.Create(configId++, CustomOptionType.Crewmate, "guesserKillsThroughShield", false, roleinfo.RoleOption);
    }
    public override void Initialize()
    {
        Charges = guesserNumberOfShots.GetInt();
        hasMultipleShotsPerMeeting = guesserHasMultipleShotsPerMeeting.GetBool();
        killsThroughShield = guesserKillsThroughShield.GetBool();
    }

    public override string UpdateMeetingVoteText(MeetingHud __instance)
    {
        var meetingInfoText = string.Format(GetString("guesserGuessesLeft"), Charges);
        return meetingInfoText;
    }
}