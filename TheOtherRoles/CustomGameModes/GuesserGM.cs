namespace TheOtherRoles.CustomGameModes;

internal class GuesserGM
{
    // Guesser Gamemode
    public static List<GuesserGM> guessers = new();
    public static Color color = new Color32(255, 255, 0, byte.MaxValue);

    public PlayerControl guesser;
    public int shots = CustomOptionHolder.guesserGamemodeNumberOfShots.GetInt();

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
            g.shots = CustomOptionHolder.guesserGamemodeNumberOfShots.GetInt();
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
}