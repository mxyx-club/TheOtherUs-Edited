namespace TheOtherRoles.Roles.Crewmate;

public class Vigilante
{
    public static PlayerControl vigilante;
    public static Color color = new Color32(255, 255, 0, byte.MaxValue);

    public static int remainingShotsNiceGuesser = 2;
    public static bool hasMultipleShotsPerMeeting;

    public static void clearAndReload()
    {
        vigilante = null;

        remainingShotsNiceGuesser = CustomOptionHolder.guesserNumberOfShots.GetInt();
        hasMultipleShotsPerMeeting = CustomOptionHolder.guesserHasMultipleShotsPerMeeting.GetBool();
    }
}