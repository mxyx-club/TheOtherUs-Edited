using TheOtherRoles.Objects;

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

    public static Sprite buttonSprite = new ResourceSprite("Bomb_Button_Plant.png");

    public static void clearBomb(bool flag = true)
    {
        if (bomb != null)
        {
            UObject.Destroy(bomb.bomb);
            UObject.Destroy(bomb.background);
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
        destructionTime = CustomOptionHolder.terroristBombDestructionTime.GetFloat();
        destructionRange = CustomOptionHolder.terroristBombDestructionRange.GetFloat() / 10;
        hearRange = CustomOptionHolder.terroristBombHearRange.GetFloat() / 10;
        defuseDuration = CustomOptionHolder.terroristDefuseDuration.GetFloat();
        bombCooldown = CustomOptionHolder.terroristBombCooldown.GetFloat();
        bombActiveAfter = CustomOptionHolder.terroristBombActiveAfter.GetFloat();
        Bomb.clearBackgroundSprite();
    }
}
