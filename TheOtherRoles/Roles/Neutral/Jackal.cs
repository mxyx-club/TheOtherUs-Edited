using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Jackal
{
    public static PlayerControl jackal;

    public static Color color = new Color32(0, 180, 235, byte.MaxValue);
    public static PlayerControl currentTarget;
    public static List<PlayerControl> formerJackals = new();

    public static float cooldown = 30f;
    public static float createSidekickCooldown = 30f;
    public static bool canUseVents = true;
    public static bool canCreateSidekick = true;
    public static bool jackalPromotedFromSidekickCanCreateSidekick = true;
    public static bool hasImpostorVision;
    public static bool CanImpostorFindSidekick;
    public static bool wasTeamRed;
    public static bool canSabotage;
    public static bool wasImpostor;
    public static bool wasSpy;

    public static float chanceSwoop;
    public static bool canSwoop;
    public static float swoopTimer;
    public static float swoopCooldown = 30f;
    public static float duration = 30f;
    public static bool isInvisable;

    public static ResourceSprite SidekickButton = new("SidekickButton.png");

    public static void removeCurrentJackal()
    {
        if (!formerJackals.Any(x => x.PlayerId == jackal.PlayerId)) formerJackals.Add(jackal);
        jackal = null;
        currentTarget = null;
        cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
        createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
    }

    public static void clearAndReload()
    {
        jackal = null;
        currentTarget = null;
        isInvisable = false;
        cooldown = CustomOptionHolder.jackalKillCooldown.getFloat();
        swoopCooldown = CustomOptionHolder.jackalSwooperCooldown.getFloat();
        duration = CustomOptionHolder.jackalSwooperDuration.getFloat();
        createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.getFloat();
        canUseVents = CustomOptionHolder.jackalCanUseVents.getBool();
        canSabotage = CustomOptionHolder.jackalCanUseSabo.getBool();
        CanImpostorFindSidekick = CustomOptionHolder.jackalCanImpostorFindSidekick.getBool();
        canCreateSidekick = CustomOptionHolder.jackalCanCreateSidekick.getBool();
        jackalPromotedFromSidekickCanCreateSidekick = CustomOptionHolder.jackalPromotedFromSidekickCanCreateSidekick.getBool();
        formerJackals.Clear();
        hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.getBool();
        wasTeamRed = wasImpostor = wasSpy = false;
        chanceSwoop = CustomOptionHolder.jackalChanceSwoop.getSelection() / 10f;
        canSwoop = rnd.NextDouble() < chanceSwoop;
    }
}
