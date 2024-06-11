using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Thief
{
    public static PlayerControl thief;
    public static Color color = new Color32(71, 99, 45, byte.MaxValue);
    public static PlayerControl currentTarget;
    public static PlayerControl formerThief;

    public static float cooldown = 30f;

    public static bool suicideFlag; // Used as a flag for suicide

    public static bool hasImpostorVision;
    public static bool canUseVents;
    public static bool canKillSheriff;
    public static bool canKillDeputy;
    public static bool canKillVeteren;
    public static bool canStealWithGuess;

    public static void clearAndReload()
    {
        thief = null;
        suicideFlag = false;
        currentTarget = null;
        formerThief = null;
        hasImpostorVision = CustomOptionHolder.thiefHasImpVision.getBool();
        cooldown = CustomOptionHolder.thiefCooldown.getFloat();
        canUseVents = CustomOptionHolder.thiefCanUseVents.getBool();
        canKillSheriff = CustomOptionHolder.thiefCanKillSheriff.getBool();
        canKillDeputy = CustomOptionHolder.thiefCanKillDeputy.getBool();
        canKillVeteren = CustomOptionHolder.thiefCanKillVeteren.getBool();
        canStealWithGuess = CustomOptionHolder.thiefCanStealWithGuess.getBool();
    }

    public static bool isFailedThiefKill(PlayerControl target, PlayerControl killer, RoleInfo targetRole)
    {
        return killer == thief && !target.Data.Role.IsImpostor && !new List<RoleInfo>
        {   RoleInfo.jackal,
            RoleInfo.sidekick,
            RoleInfo.werewolf,
            RoleInfo.juggernaut,
            RoleInfo.swooper,
            canKillSheriff ? RoleInfo.sheriff : null,
            canKillDeputy ? RoleInfo.deputy : null,
            canKillVeteren ? RoleInfo.veteren : null
        }.Contains(targetRole);
    }
}
