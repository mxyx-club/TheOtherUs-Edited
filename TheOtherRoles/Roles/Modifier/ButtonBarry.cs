using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class ButtonBarry
{
    public static PlayerControl buttonBarry;
    public static int remoteMeetingsLeft = 1;
    public static bool SabotageRemoteMeetings;

    private static Sprite buttonSprite;

    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.EmergencyButton.png", 550f);
        return buttonSprite;
    }
    public static void clearAndReload()
    {
        buttonBarry = null;
        remoteMeetingsLeft = 1;

        SabotageRemoteMeetings = false;
        //SabotageRemoteMeetings = CustomOptionHolder.modifierButtonSabotageRemoteMeetings.getBool();
    }
}
