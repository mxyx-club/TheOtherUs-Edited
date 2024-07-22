using System;
using TheOtherRoles.Utilities;
using Random = System.Random;

namespace TheOtherRoles.Roles;

public static class RoleHelpers
{
    public static bool CanMultipleShots(PlayerControl dyingTarget)
    {
        if (dyingTarget == CachedPlayer.LocalPlayer.PlayerControl)
            return false;

        if (HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId)
            && HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId) > 1
            && HandleGuesser.hasMultipleShotsPerMeeting)
            return true;

        return CachedPlayer.LocalPlayer.PlayerControl == Doomsayer.doomsayer && Doomsayer.hasMultipleShotsPerMeeting &&
               Doomsayer.CanShoot;
    }
    public static readonly Random rnd = new((int)DateTime.Now.Ticks);
}