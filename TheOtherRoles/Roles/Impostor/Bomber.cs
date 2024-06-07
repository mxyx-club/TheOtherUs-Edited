using UnityEngine;

namespace TheOtherRoles.Roles.Impostor;

public static class Bomber
{
    public static PlayerControl bomber;
    public static Color color = Palette.ImpostorRed;
    public static Color alertColor = Palette.ImpostorRed;

    public static float cooldown = 30f;
    public static float bombDelay = 10f;
    public static float bombTimer = 10f;

    public static bool bombActive;
    //public static bool hotPotatoMode = false;

    public static PlayerControl currentBombTarget;
    public static bool hasAlerted;
    public static int timeLeft;
    public static PlayerControl currentTarget;
    public static PlayerControl hasBomb;


    private static Sprite buttonSprite;

    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = loadSpriteFromResources("TheOtherRoles.Resources.Bomber2.png", 115f);
        return buttonSprite;
    }

    public static void clearAndReload()
    {
        bomber = null;
        bombActive = false;
        cooldown = CustomOptionHolder.bomberBombCooldown.getFloat();
        bombDelay = CustomOptionHolder.bomberDelay.getFloat();
        bombTimer = CustomOptionHolder.bomberTimer.getFloat();
        //hotPotatoMode = CustomOptionHolder.bomberHotPotatoMode.getBool();
    }
}
