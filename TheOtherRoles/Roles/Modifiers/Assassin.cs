namespace TheOtherRoles.Roles.Modifier;

public class Assassin : ModifierBase, IGuesser
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleinfo = new(
        typeof(Assassin),
        (p) => new Assassin(p),
        RoleId.Assassin,
        "Assassin",
        color,
        404000,
        AddOptions
    );

    public Assassin(PlayerControl p) : base(p, roleinfo) { }

    public int Charges { get; set; }
    public static bool assassinMultipleShotsPerMeeting;
    public static bool assassinKillsThroughShield = true;
    public static bool evilGuesserCanGuessCrewmate = true;
    public static bool evilGuesserCanGuessSpy = true;
    public static bool guesserCantGuessSnitch;

    public static CustomOption modifierAssassinNumberOfShots;
    public static CustomOption modifierAssassinMultipleShotsPerMeeting;
    public static CustomOption modifierAssassinKillsThroughShield;
    public static CustomOption guesserEvilCanKillSpy;
    public static CustomOption guesserEvilCanKillCrewmate;
    public static CustomOption guesserCantGuessSnitchIfTaksDone;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierAssassinNumberOfShots = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierAssassinNumberOfShots", 3f, 1f, 15f, 1f, roleinfo.RoleOption);
        modifierAssassinMultipleShotsPerMeeting = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierAssassinMultipleShotsPerMeeting", true, roleinfo.RoleOption);
        guesserEvilCanKillSpy = CustomOption.Create(configId++, CustomOptionType.Modifiers, "guesserEvilCanKillSpy", true, roleinfo.RoleOption);
        guesserEvilCanKillCrewmate = CustomOption.Create(configId++, CustomOptionType.Modifiers, "guesserEvilCanKillCrewmate", true, roleinfo.RoleOption);
        guesserCantGuessSnitchIfTaksDone = CustomOption.Create(configId++, CustomOptionType.Modifiers, "guesserCantGuessSnitchIfTaksDone", true, roleinfo.RoleOption);
        modifierAssassinKillsThroughShield = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierAssassinKillsThroughShield", false, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        Charges = modifierAssassinNumberOfShots.GetInt();
        assassinMultipleShotsPerMeeting = modifierAssassinMultipleShotsPerMeeting.GetBool();
        assassinKillsThroughShield = modifierAssassinKillsThroughShield.GetBool();
        evilGuesserCanGuessCrewmate = guesserEvilCanKillCrewmate.GetBool();
        evilGuesserCanGuessSpy = guesserEvilCanKillSpy.GetBool();
        guesserCantGuessSnitch = guesserCantGuessSnitchIfTaksDone.GetBool();
    }

    public override string UpdateMeetingVoteText(MeetingHud __instance)
    {
        var meetingInfoText = string.Format(GetString("guesserGuessesLeft"), Charges);
        return meetingInfoText;
    }
}