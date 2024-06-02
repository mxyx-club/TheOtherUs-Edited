using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Arsonist
{
    public static PlayerControl arsonist;
    public static Color color = new Color32(238, 112, 46, byte.MaxValue);

    public static float cooldown = 30f;
    public static float duration = 3f;
    public static bool triggerArsonistWin;

    public static PlayerControl currentTarget;
    public static PlayerControl douseTarget;
    public static List<PlayerControl> dousedPlayers = new();

    private static Sprite douseSprite;

    private static Sprite igniteSprite;

    public static Sprite getDouseSprite()
    {
        if (douseSprite) return douseSprite;
        douseSprite = loadSpriteFromResources("TheOtherRoles.Resources.DouseButton.png", 115f);
        return douseSprite;
    }

    public static Sprite getIgniteSprite()
    {
        if (igniteSprite) return igniteSprite;
        igniteSprite = loadSpriteFromResources("TheOtherRoles.Resources.IgniteButton.png", 115f);
        return igniteSprite;
    }

    public static bool dousedEveryoneAlive()
    {
        return CachedPlayer.AllPlayers.All(x =>
        {
            return x.PlayerControl == arsonist || x.Data.IsDead || x.Data.Disconnected ||
                   dousedPlayers.Any(y => y.PlayerId == x.PlayerId);
        });
    }

    public static void clearAndReload()
    {
        arsonist = null;
        currentTarget = null;
        douseTarget = null;
        triggerArsonistWin = false;
        dousedPlayers = new List<PlayerControl>();
        foreach (var p in MapOption.playerIcons.Values)
            if (p != null && p.gameObject != null)
                p.gameObject.SetActive(false);
        cooldown = CustomOptionHolder.arsonistCooldown.getFloat();
        duration = CustomOptionHolder.arsonistDuration.getFloat();
    }
}
