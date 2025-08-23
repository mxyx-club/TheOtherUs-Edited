using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Modifier;

public static class Radar
{
    public static PlayerControl radar;
    public static Arrow localArrow;
    public static Color color = new Color32(255, 0, 128, byte.MaxValue);

    public static void clearAndReload()
    {
        radar = null;
        if (localArrow?.arrow != null)
            UObject.Destroy(localArrow.arrow);
    }
}
