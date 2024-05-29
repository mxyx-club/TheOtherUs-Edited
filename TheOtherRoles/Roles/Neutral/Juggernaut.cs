using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Juggernaut
{
    public static PlayerControl juggernaut;
    public static Color color = new(0.55f, 0f, 0.3f, byte.MaxValue);
    public static PlayerControl currentTarget;

    public static float cooldown = 30f;
    public static float reducedkill = 5f;
    public static bool hasImpostorVision;

    public static void setkill()
    {
        cooldown = cooldown - reducedkill;
        if (cooldown <= 0f) cooldown = 0f;
    }

    public static void clearAndReload()
    {
        juggernaut = null;
        currentTarget = null;
        hasImpostorVision = CustomOptionHolder.juggernautHasImpVision.getBool();
        cooldown = CustomOptionHolder.juggernautCooldown.getFloat();
        reducedkill = CustomOptionHolder.juggernautReducedkillEach.getFloat();
    }
}
