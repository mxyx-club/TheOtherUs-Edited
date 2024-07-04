using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Veteran
{
    public static PlayerControl veteran;
    public static Color color = new Color32(255, 77, 0, byte.MaxValue);

    public static float alertDuration = 3f;
    public static float cooldown = 30f;

    public static bool alertActive;

    public static ResourceSprite buttonSprite = new("Alert.png");

    public static void clearAndReload()
    {
        veteran = null;
        alertActive = false;
        alertDuration = CustomOptionHolder.veteranAlertDuration.getFloat();
        cooldown = CustomOptionHolder.veteranCooldown.getFloat();
    }
}
