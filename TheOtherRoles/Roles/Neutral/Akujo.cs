using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Akujo
{
    public static Color color = new Color32(142, 69, 147, byte.MaxValue);
    public static PlayerControl akujo;
    public static PlayerControl honmei;
    public static List<PlayerControl> keeps;
    public static PlayerControl currentTarget;
    public static DateTime startTime;

    public static float timeLimit = 1300f;
    public static bool knowsRoles = true;
    public static bool honmeiCannotFollowWin;
    public static bool honmeiOptimizeWin;
    public static int timeLeft;
    public static bool forceKeeps;
    public static int keepsLeft;
    public static int numKeeps;

    private static Sprite honmeiSprite;
    public static Sprite getHonmeiSprite()
    {
        if (honmeiSprite) return honmeiSprite;
        honmeiSprite = loadSpriteFromResources("TheOtherRoles.Resources.AkujoHonmeiButton.png", 115f);
        return honmeiSprite;
    }

    private static Sprite keepSprite;
    public static Sprite getKeepSprite()
    {
        if (keepSprite) return keepSprite;
        keepSprite = loadSpriteFromResources("TheOtherRoles.Resources.AkujoKeepButton.png", 115f);
        return keepSprite;
    }
    public static bool existing()
    {
        return honmei != null && !honmei.Data.Disconnected;
    }

    public static bool existingWithKiller()
    {
        return existing() && (honmei == Jackal.jackal
                           || honmei == Sidekick.sidekick
                           || honmei == Werewolf.werewolf
                           || honmei == Juggernaut.juggernaut
                           || honmei == Arsonist.arsonist
                           || honmei == Vulture.vulture
                           || honmei == Lawyer.lawyer
                           || honmei == Jester.jester
                           || honmei == Thief.thief
                           || honmei == Doomsayer.doomsayer
                           || honmei.Data.Role.IsImpostor);
    }
    public static void breakLovers(PlayerControl lover)
    {
        if ((Lovers.lover1 != null && lover == Lovers.lover1) || (Lovers.lover2 != null && lover == Lovers.lover2))
        {
            PlayerControl otherLover = lover.getPartner();
            if (otherLover != null)
            {
                Lovers.clearAndReload();
                otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoveStolen);
            }
        }
    }

    public static void clearAndReload()
    {
        akujo = null;
        honmei = null;
        keeps = new List<PlayerControl>();
        currentTarget = null;
        startTime = DateTime.UtcNow;
        timeLimit = CustomOptionHolder.akujoTimeLimit.getFloat();
        forceKeeps = CustomOptionHolder.akujoForceKeeps.getBool();
        knowsRoles = CustomOptionHolder.akujoKnowsRoles.getBool();
        honmeiCannotFollowWin = CustomOptionHolder.akujoHonmeiCannotFollowWin.getBool();
        honmeiOptimizeWin = CustomOptionHolder.akujoHonmeiOptimizeWin.getBool();
        timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
        numKeeps = Math.Min((int)CustomOptionHolder.akujoNumKeeps.getFloat(), PlayerControl.AllPlayerControls.Count - 2);
        keepsLeft = numKeeps;
    }
}
