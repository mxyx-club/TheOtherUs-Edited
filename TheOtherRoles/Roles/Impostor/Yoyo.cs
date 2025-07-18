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

    public static Sprite markButtonSprite = new ResourceSprite("YoyoMarkButtonSprite.png");
    public static Sprite blinkButtonSprite = new ResourceSprite("YoyoBlinkButtonSprite.png");

    public static void markLocation(Vector3 position)
    {
        markedLocation = position;
    }

    public static void clearAndReload()
    {
        yoyo = null;
        blinkDuration = CustomOptionHolder.yoyoBlinkDuration.GetFloat();
        markCooldown = CustomOptionHolder.yoyoMarkCooldown.GetFloat();
        markStaysOverMeeting = CustomOptionHolder.yoyoMarkStaysOverMeeting.GetBool();
        hasAdminTable = CustomOptionHolder.yoyoHasAdminTable.GetBool();
        adminCooldown = CustomOptionHolder.yoyoAdminTableCooldown.GetFloat();
        silhouetteVisibility = CustomOptionHolder.yoyoSilhouetteVisibility.GetSelection() / 10f;

        markedLocation = null;

    }
}
