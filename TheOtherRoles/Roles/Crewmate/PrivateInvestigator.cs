using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class PrivateInvestigator
{
    public static PlayerControl privateInvestigator;
    public static Color color = new Color32(77, 77, 255, byte.MaxValue);
    private static Sprite buttonSprite;
    public static PlayerControl watching;
    public static PlayerControl currentTarget;


    public static bool seeFlashColor;

    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Watch.png", 115f);
        return buttonSprite;
    }


    public static void clearAndReload(bool clearList = true)
    {
        privateInvestigator = null;
        watching = null;
        currentTarget = null;
        seeFlashColor = CustomOptionHolder.privateInvestigatorSeeColor.getBool();
    }
}