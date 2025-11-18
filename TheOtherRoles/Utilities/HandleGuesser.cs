using TheOtherRoles.Mode;

namespace TheOtherRoles.Utilities;

public static class HandleGuesser
{
    public static bool isGuesserGm;
    public static bool hasMultipleShotsPerMeeting;
    public static bool killsThroughShield = true;
    public static bool evilGuesserCanGuessSpy = true;
    public static bool guesserCantGuessSnitch;

    public static ResourceSprite targetSprite = new("TargetIcon.png", 150f);

    public static bool isGuesser(byte playerId)
    {
        if (PlayerById(playerId)?.Is(RoleId.Doomsayer) == true) return true;

        return isGuesserGm ? GuesserGM.isGuesser(playerId) : Guesser.isGuesser(playerId);
    }

    public static void clear(byte playerId)
    {
        if (isGuesserGm) GuesserGM.clear(playerId);
        else Guesser.clear(playerId);
    }

    public static int remainingShots(byte playerId, bool shoot = false)
    {
        if (PlayerById(playerId)?.Is(RoleId.Doomsayer) == true) return 15;

        return isGuesserGm ? GuesserGM.remainingShots(playerId, shoot) : Guesser.remainingShots(playerId, shoot);
    }

    public static void clearAndReload()
    {
        GuesserGM.clearAndReload();
        isGuesserGm = ModOption.GameMode == CustomGameModes.Guesser;
        if (isGuesserGm)
        {
            guesserCantGuessSnitch = GuesserGM.guesserGamemodeCantGuessSnitchIfTaksDone.GetBool();
            hasMultipleShotsPerMeeting = GuesserGM.guesserGamemodeHasMultipleShotsPerMeeting.GetBool();
            killsThroughShield = GuesserGM.guesserGamemodeKillsThroughShield.GetBool();
            evilGuesserCanGuessSpy = GuesserGM.guesserGamemodeEvilCanKillSpy.GetBool();
        }
        else
        {
            guesserCantGuessSnitch = Assassin.guesserCantGuessSnitchIfTaksDone.GetBool();
            hasMultipleShotsPerMeeting = Vigilante.guesserHasMultipleShotsPerMeeting.GetBool();
            killsThroughShield = Vigilante.guesserKillsThroughShield.GetBool();
            evilGuesserCanGuessSpy = Assassin.guesserEvilCanKillSpy.GetBool();
        }
    }
}