namespace TheOtherRoles.Mode;

internal class GuesserGM
{
    // Guesser Gamemode
    public static bool Enabled => guesserEnabled.GetBool();
    public static List<GuesserGM> guessers = new();
    public static Color color = new Color32(255, 255, 0, byte.MaxValue);

    public PlayerControl guesser;
    public int shots = guesserGamemodeNumberOfShots.GetInt();

    public static CustomOption guesserEnabled;
    public static CustomOption guesserGamemodeCrewNumber;
    public static CustomOption guesserGamemodeNeutralNumber;
    public static CustomOption guesserGamemodeImpNumber;
    public static CustomOption guesserForceJackalGuesser;
    public static CustomOption guesserGamemodeSidekickIsAlwaysGuesser;
    public static CustomOption guesserGamemodePavlovsdogIsAlwaysGuesser;
    public static CustomOption guesserForcePavlovsGuesser;
    public static CustomOption guesserForceThiefGuesser;
    public static CustomOption guesserGamemodeHaveModifier;
    public static CustomOption guesserGamemodeNumberOfShots;
    public static CustomOption guesserGamemodeHasMultipleShotsPerMeeting;
    public static CustomOption guesserGamemodeKillsThroughShield;

    public GuesserGM(PlayerControl player)
    {
        guesser = player;
        guessers.Add(this);
    }

    public static int remainingShots(byte playerId, bool shoot = false)
    {
        var g = guessers.FirstOrDefault(x => x.guesser.PlayerId == playerId);
        if (g == null) return 0;
        if (shoot) g.shots--;
        return g.shots;
    }

    public static void clear(byte playerId)
    {
        var g = guessers.FirstOrDefault(x => x.guesser.PlayerId == playerId);
        if (g != null)
        {
            g.shots = guesserGamemodeNumberOfShots.GetInt();
            guessers.Remove(g);
        }
    }

    public static void clearAndReload()
    {
        guessers.Clear();
    }

    public static bool isGuesser(byte playerId)
    {
        return guessers.Any(x => x.guesser.PlayerId == playerId);
    }

    public static void AddOptions()
    {
        //-------------------------- Guesser Gamemode 2000 - 2999 -------------------------- //
        guesserEnabled = CustomOption.Create(2000, CustomOptionType.Guesser, "guesserEnabled", false, null, true);
        guesserGamemodeCrewNumber = CustomOption.Create(2001, CustomOptionType.Guesser, Cs(Color.yellow, "guesserGamemodeCrewNumber"), 2f, 0f, 15f, 1f, guesserEnabled, true);
        guesserGamemodeNeutralNumber = CustomOption.Create(2002, CustomOptionType.Guesser, Cs(Color.yellow, "guesserGamemodeNeutralNumber"), 2f, 0f, 15f, 1f, guesserEnabled);
        guesserGamemodeImpNumber = CustomOption.Create(2003, CustomOptionType.Guesser, Cs(Color.yellow, "guesserGamemodeImpNumber"), 2f, 0f, 15f, 1f, guesserEnabled);
        guesserForceJackalGuesser = CustomOption.Create(2007, CustomOptionType.Guesser, "guesserForceJackalGuesser", false, guesserEnabled, true);
        guesserGamemodeSidekickIsAlwaysGuesser = CustomOption.Create(2012, CustomOptionType.Guesser, "guesserGamemodeSidekickIsAlwaysGuesser", false, guesserEnabled);
        guesserForcePavlovsGuesser = CustomOption.Create(2013, CustomOptionType.Guesser, "guesserForcePavlovsGuesser", false, guesserEnabled);
        guesserGamemodePavlovsdogIsAlwaysGuesser = CustomOption.Create(2015, CustomOptionType.Guesser, "guesserGamemodePavlovsdogIsAlwaysGuesser", false, guesserEnabled);
        guesserForceThiefGuesser = CustomOption.Create(2011, CustomOptionType.Guesser, "guesserForceThiefGuesser", false, guesserEnabled);
        guesserGamemodeHaveModifier = CustomOption.Create(2004, CustomOptionType.Guesser, "guesserGamemodeHaveModifier", true, guesserEnabled, true);
        guesserGamemodeNumberOfShots = CustomOption.Create(2005, CustomOptionType.Guesser, "guesserGamemodeNumberOfShots", 3f, 1f, 15f, 1f, guesserEnabled);
        guesserGamemodeHasMultipleShotsPerMeeting = CustomOption.Create(2006, CustomOptionType.Guesser, "guesserGamemodeHasMultipleShotsPerMeeting", true, guesserEnabled);
        guesserGamemodeKillsThroughShield = CustomOption.Create(2008, CustomOptionType.Guesser, "guesserGamemodeKillsThroughShield", true, guesserEnabled);
    }
}