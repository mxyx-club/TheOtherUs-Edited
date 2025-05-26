namespace TheOtherRoles.Roles.Crewmate;

public static class Portalmaker
{
    public static PlayerControl portalmaker;
    public static Color color = new Color32(69, 69, 169, byte.MaxValue);

    public static float cooldown;
    public static float usePortalCooldown;
    public static bool logOnlyHasColors;
    public static bool logShowsTime;
    public static bool canPortalFromAnywhere;

    public static ResourceSprite placePortalButtonSprite = new("PlacePortalButton.png");
    public static ResourceSprite usePortalButtonSprite = new("UsePortalButton.png");

    public static void clearAndReload()
    {
        portalmaker = null;
        cooldown = CustomOptionHolder.portalmakerCooldown.GetFloat();
        usePortalCooldown = CustomOptionHolder.portalmakerUsePortalCooldown.GetFloat();
        logOnlyHasColors = CustomOptionHolder.portalmakerLogOnlyColorType.GetBool();
        logShowsTime = CustomOptionHolder.portalmakerLogHasTime.GetBool();
        canPortalFromAnywhere = CustomOptionHolder.portalmakerCanPortalFromAnywhere.GetBool();
    }
}