using TheOtherRoles.Utilities;
using UnityEngine;

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

    private static Sprite placePortalButtonSprite;
    private static Sprite usePortalButtonSprite;
    private static Sprite usePortalSpecialButtonSprite1;
    private static Sprite usePortalSpecialButtonSprite2;
    private static Sprite logSprite;

    public static Sprite getPlacePortalButtonSprite()
    {
        if (placePortalButtonSprite) return placePortalButtonSprite;
        placePortalButtonSprite =
            Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PlacePortalButton.png", 115f);
        return placePortalButtonSprite;
    }

    public static Sprite getUsePortalButtonSprite()
    {
        if (usePortalButtonSprite) return usePortalButtonSprite;
        usePortalButtonSprite =
            Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalButton.png", 115f);
        return usePortalButtonSprite;
    }

    public static Sprite getUsePortalSpecialButtonSprite(bool first)
    {
        if (first)
        {
            if (usePortalSpecialButtonSprite1) return usePortalSpecialButtonSprite1;
            usePortalSpecialButtonSprite1 =
                Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalSpecialButton1.png", 115f);
            return usePortalSpecialButtonSprite1;
        }

        if (usePortalSpecialButtonSprite2) return usePortalSpecialButtonSprite2;
        usePortalSpecialButtonSprite2 =
            Helpers.loadSpriteFromResources("TheOtherRoles.Resources.UsePortalSpecialButton2.png", 115f);
        return usePortalSpecialButtonSprite2;
    }

    public static Sprite getLogSprite()
    {
        if (logSprite) return logSprite;
        logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton
            .fastUseSettings[ImageNames.DoorLogsButton].Image;
        return logSprite;
    }

    public static void clearAndReload()
    {
        portalmaker = null;
        cooldown = CustomOptionHolder.portalmakerCooldown.getFloat();
        usePortalCooldown = CustomOptionHolder.portalmakerUsePortalCooldown.getFloat();
        logOnlyHasColors = CustomOptionHolder.portalmakerLogOnlyColorType.getBool();
        logShowsTime = CustomOptionHolder.portalmakerLogHasTime.getBool();
        canPortalFromAnywhere = CustomOptionHolder.portalmakerCanPortalFromAnywhere.getBool();
    }
}