using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.Roles.RoleInfo;
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