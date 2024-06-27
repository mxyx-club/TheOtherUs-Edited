using TheOtherRoles.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Roles.Impostor;

public static class Terrorist
{
    public static PlayerControl terrorist;
    public static Color color = Palette.ImpostorRed;

    public static Bomb bomb;
    public static bool isPlanted;
    public static bool isActive;
    public static float destructionTime = 20f;
    public static float destructionRange = 2f;
    public static float hearRange = 30f;
    public static float defuseDuration = 3f;
    public static float bombCooldown = 15f;
    public static float bombActiveAfter = 3f;
    public static bool selfExplosion => destructionTime + bombActiveAfter <= 1;

    private static Sprite buttonSprite;

    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = loadSpriteFromResources("TheOtherRoles.Resources.Bomb_Button_Plant.png", 115f);
        return buttonSprite;
    }

    public static void clearBomb(bool flag = true)
    {
        if (bomb != null)
        {
            Object.Destroy(bomb.bomb);
            Object.Destroy(bomb.background);
            bomb = null;
        }

        isPlanted = false;
        isActive = false;
        if (flag) SoundEffectsManager.stop("bombFuseBurning");
    }

    public static void clearAndReload()
    {
        clearBomb(false);
        terrorist = null;
        bomb = null;
        isPlanted = false;
        isActive = false;
        destructionTime = CustomOptionHolder.terroristBombDestructionTime.getFloat();
        destructionRange = CustomOptionHolder.terroristBombDestructionRange.getFloat() / 10;
        hearRange = CustomOptionHolder.terroristBombHearRange.getFloat() / 10;
        defuseDuration = CustomOptionHolder.terroristDefuseDuration.getFloat();
        bombCooldown = CustomOptionHolder.terroristBombCooldown.getFloat();
        bombActiveAfter = CustomOptionHolder.terroristBombActiveAfter.getFloat();
        Bomb.clearBackgroundSprite();
    }
}
