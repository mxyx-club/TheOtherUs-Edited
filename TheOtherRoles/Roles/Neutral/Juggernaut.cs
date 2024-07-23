using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Juggernaut
{
    public static PlayerControl juggernaut;
    public static Color color = new Color32(140, 0, 77, byte.MaxValue);
    public static PlayerControl currentTarget;

    public static float cooldown = 30f;
    public static float reducedkill = 5f;
    public static bool hasImpostorVision;
    public static bool canUseVents;

    public static void setkill()
    {
        cooldown -= reducedkill;
        if (cooldown <= 0f) cooldown = 0f;
    }

    public static void clearAndReload()
    {
        juggernaut = null;
        currentTarget = null;
        hasImpostorVision = CustomOptionHolder.juggernautHasImpVision.getBool();
        canUseVents = CustomOptionHolder.juggernautCanUseVents.getBool();
        cooldown = CustomOptionHolder.juggernautCooldown.getFloat();
        reducedkill = CustomOptionHolder.juggernautReducedkillEach.getFloat();
    }
}
