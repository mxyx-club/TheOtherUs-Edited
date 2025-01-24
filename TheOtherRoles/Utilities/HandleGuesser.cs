using TheOtherRoles.CustomGameModes;

namespace TheOtherRoles.Utilities;

public static class HandleGuesser
{
    public static bool isGuesserGm;
    public static bool hasMultipleShotsPerMeeting;
    public static bool killsThroughShield = true;
    public static bool evilGuesserCanGuessSpy = true;
    public static bool guesserCantGuessSnitch;
    public static int tasksToUnlock;

    public static ResourceSprite targetSprite = new("TargetIcon.png", 150f);

    public static bool isGuesser(byte playerId)
    {
        if (Doomsayer.doomsayer != null && Doomsayer.doomsayer.PlayerId == playerId) return true;

        return isGuesserGm ? GuesserGM.isGuesser(playerId) : Guesser.isGuesser(playerId);
    }

    public static void clear(byte playerId)
    {
        if (isGuesserGm) GuesserGM.clear(playerId);
        else Guesser.clear(playerId);
    }

    public static int remainingShots(byte playerId, bool shoot = false)
    {
        if (Doomsayer.doomsayer != null && Doomsayer.doomsayer.PlayerId == playerId) return 15;

        return isGuesserGm ? GuesserGM.remainingShots(playerId, shoot) : Guesser.remainingShots(playerId, shoot);
    }

    public static void clearAndReload()
    {
        GuesserGM.clearAndReload();
        isGuesserGm = ModOption.gameMode == CustomGamemodes.Guesser;
        if (isGuesserGm)
        {
            guesserCantGuessSnitch = CustomOptionHolder.guesserGamemodeCantGuessSnitchIfTaksDone.GetBool();
            hasMultipleShotsPerMeeting = CustomOptionHolder.guesserGamemodeHasMultipleShotsPerMeeting.GetBool();
            killsThroughShield = CustomOptionHolder.guesserGamemodeKillsThroughShield.GetBool();
            evilGuesserCanGuessSpy = CustomOptionHolder.guesserGamemodeEvilCanKillSpy.GetBool();
            tasksToUnlock = CustomOptionHolder.guesserGamemodeCrewGuesserNumberOfTasks.GetInt();
        }
        else
        {
            guesserCantGuessSnitch = CustomOptionHolder.guesserCantGuessSnitchIfTaksDone.GetBool();
            hasMultipleShotsPerMeeting = CustomOptionHolder.guesserHasMultipleShotsPerMeeting.GetBool();
            killsThroughShield = CustomOptionHolder.guesserKillsThroughShield.GetBool();
            evilGuesserCanGuessSpy = CustomOptionHolder.guesserEvilCanKillSpy.GetBool();
        }
    }
}