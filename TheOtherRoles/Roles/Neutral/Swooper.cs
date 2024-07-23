using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Swooper
{
    public static PlayerControl swooper;
    public static PlayerControl currentTarget;
    public static float cooldown = 30f;
    public static bool isInvisable;
    public static Color color = new Color32(224, 197, 219, byte.MaxValue);
    public static float duration = 5f;
    public static float swoopCooldown = 30f;
    public static float swoopTimer;
    public static float swoopSpeed;
    public static bool hasImpVision;
    public static bool canUseVents;

    public static ResourceSprite SwoopButton = new("Swoop.png");

    public static void clearAndReload()
    {
        swooper = null;
        isInvisable = false;
        cooldown = CustomOptionHolder.swooperKillCooldown.getFloat();
        swoopCooldown = CustomOptionHolder.swooperCooldown.getFloat();
        duration = CustomOptionHolder.swooperDuration.getFloat();
        hasImpVision = CustomOptionHolder.swooperHasImpVision.getBool();
        swoopSpeed = CustomOptionHolder.swooperSpeed.getFloat();
        canUseVents = CustomOptionHolder.swooperCanUseVents.getBool();
    }
}
