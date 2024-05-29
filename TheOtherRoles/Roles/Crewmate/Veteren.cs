using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Veteren
{
    public static PlayerControl veteren;
    public static Color color = new Color32(255, 77, 0, byte.MaxValue);

    public static float alertDuration = 3f;
    public static float cooldown = 30f;

    public static bool alertActive;

    private static Sprite buttonSprite;

    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Alert.png", 115f);
        return buttonSprite;
    }

    public static void clearAndReload()
    {
        veteren = null;
        alertActive = false;
        alertDuration = CustomOptionHolder.veterenAlertDuration.getFloat();
        cooldown = CustomOptionHolder.veterenCooldown.getFloat();
    }
}
