using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Impostor;

public static class Yoyo
{
    public static PlayerControl yoyo;
    public static Color color = Palette.ImpostorRed;

    public static float blinkDuration;
    public static float markCooldown;
    public static bool markStaysOverMeeting;
    public static bool hasAdminTable;
    public static float adminCooldown;
    public static float SilhouetteVisibility => silhouetteVisibility == 0 && (PlayerControl.LocalPlayer == yoyo || PlayerControl.LocalPlayer.Data.IsDead) ? 0.1f : silhouetteVisibility;
    public static float silhouetteVisibility;

    public static Vector3? markedLocation;

    public static ResourceSprite markButtonSprite = new("YoyoMarkButtonSprite.png");
    public static ResourceSprite blinkButtonSprite = new("YoyoBlinkButtonSprite.png");

    public static void markLocation(Vector3 position)
    {
        markedLocation = position;
    }

    public static void clearAndReload()
    {
        blinkDuration = CustomOptionHolder.yoyoBlinkDuration.getFloat();
        markCooldown = CustomOptionHolder.yoyoMarkCooldown.getFloat();
        markStaysOverMeeting = CustomOptionHolder.yoyoMarkStaysOverMeeting.getBool();
        hasAdminTable = CustomOptionHolder.yoyoHasAdminTable.getBool();
        adminCooldown = CustomOptionHolder.yoyoAdminTableCooldown.getFloat();
        silhouetteVisibility = CustomOptionHolder.yoyoSilhouetteVisibility.getSelection() / 10f;

        markedLocation = null;

    }
}
