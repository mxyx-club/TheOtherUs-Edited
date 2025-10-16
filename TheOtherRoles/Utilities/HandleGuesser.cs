using TheOtherRoles.Mode;

namespace TheOtherRoles.Utilities;

public static class HandleGuesser
{
    public static bool isGuesserGm => GuesserGM.Enabled;
    public static bool hasMultipleShotsPerMeeting;

    public static Sprite targetSprite = new ResourceSprite("TargetIcon.png", 150f);

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

    public static bool CanMultipleShots(PlayerControl dyingTarget)
    {
        if (dyingTarget == PlayerControl.LocalPlayer) return false;

        if (PlayerControl.LocalPlayer == Doomsayer.doomsayer)
        {
            if (Doomsayer.hasMultipleShotsPerMeeting && Doomsayer.CanShoot)
            {
                return true;
            }
            return false;
        }
        else
        {
            if (remainingShots(PlayerControl.LocalPlayer.PlayerId) <= 0) return false;

            if (GuesserGM.Enabled)
            {
                if (isGuesser(PlayerControl.LocalPlayer.PlayerId) && hasMultipleShotsPerMeeting) return true;
            }
            else
            {
                if (PlayerControl.LocalPlayer == Vigilante.vigilante && Vigilante.hasMultipleShotsPerMeeting)
                    return true;
                else if (Assassin.assassin.Any(x => x == PlayerControl.LocalPlayer) && Assassin.assassinMultipleShotsPerMeeting)
                    return true;
            }
        }
        return false;
    }

    public static void clearAndReload()
    {
        GuesserGM.clearAndReload();
        if (isGuesserGm)
        {
            hasMultipleShotsPerMeeting = GuesserGM.guesserGamemodeHasMultipleShotsPerMeeting.GetBool();
        }
        else
        {
            hasMultipleShotsPerMeeting = CustomOptionHolder.guesserHasMultipleShotsPerMeeting.GetBool();
        }
    }
}