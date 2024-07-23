using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Jester
{
    public static PlayerControl jester;
    public static Color color = new Color32(236, 98, 165, byte.MaxValue);

    public static bool triggerJesterWin;
    public static bool canCallEmergency = true;
    public static bool canUseVents;
    public static bool hasImpostorVision;

    public static void clearAndReload()
    {
        jester = null;
        triggerJesterWin = false;
        canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.getBool();
        canUseVents = CustomOptionHolder.jesterCanVent.getBool();
        hasImpostorVision = CustomOptionHolder.jesterHasImpostorVision.getBool();
    }
}
