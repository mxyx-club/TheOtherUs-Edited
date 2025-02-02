using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Arsonist
{
    public static PlayerControl arsonist;
    public static Color color = new Color32(238, 112, 46, byte.MaxValue);

    public static float cooldown = 30f;
    public static float duration = 3f;
    public static bool igniteCooldownRemoved;

    public static PlayerControl currentTarget;
    public static PlayerControl currentTarget2;
    public static PlayerControl douseTarget;
    public static List<PlayerControl> dousedPlayers = new();

    public static ResourceSprite douseSprite = new("DouseButton.png");

    public static ResourceSprite igniteSprite = new("IgniteButton.png");

    public static void clearAndReload()
    {
        arsonist = null;
        currentTarget = null;
        currentTarget2 = null;
        douseTarget = null;
        dousedPlayers = new List<PlayerControl>();
        foreach (var p in ModOption.playerIcons.Values)
            if (p != null && p.gameObject != null)
                p.gameObject.SetActive(false);

        cooldown = CustomOptionHolder.arsonistCooldown.GetFloat();
        duration = CustomOptionHolder.arsonistDuration.GetFloat();
        igniteCooldownRemoved = CustomOptionHolder.arsonistIgniteCdRemoved.GetBool();
    }
}
