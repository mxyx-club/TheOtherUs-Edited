using TheOtherRoles.CustomGameModes;

namespace TheOtherRoles.Utilities;

public static class HandleGuesser
{
    public static bool isGuesserGm => GuesserGM.Enabled;
    public static bool hasMultipleShotsPerMeeting;
    public static bool killsThroughShield = true;
    public static bool evilGuesserCanGuessSpy = true;
    public static bool guesserCantGuessSnitch;

    public static ResourceSprite targetSprite = new("TargetIcon.png", 150f);

    public static bool isGuesser(byte playerId)
    {
        if (Doomsayer.doomsayer != null && Doomsayer.doomsayer.PlayerId == playerId) return true;

        if (Infected.Player.Any(x => x.PlayerId == playerId) && Infected.IsGuesser) return true;

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

        if (Infected.Player.Any(x => x.PlayerId == playerId) && Infected.IsGuesser)
        {
            if (shoot) Infected.GuessCount--;
            return Infected.GuessCount;
        }

        return isGuesserGm ? GuesserGM.remainingShots(playerId, shoot) : Guesser.remainingShots(playerId, shoot);
    }

    public static void clearAndReload()
    {
        GuesserGM.clearAndReload();
        if (isGuesserGm)
        {
            guesserCantGuessSnitch = GuesserGM.guesserGamemodeCantGuessSnitchIfTaksDone.GetBool();
            hasMultipleShotsPerMeeting = GuesserGM.guesserGamemodeHasMultipleShotsPerMeeting.GetBool();
            killsThroughShield = GuesserGM.guesserGamemodeKillsThroughShield.GetBool();
            evilGuesserCanGuessSpy = GuesserGM.guesserGamemodeEvilCanKillSpy.GetBool();
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