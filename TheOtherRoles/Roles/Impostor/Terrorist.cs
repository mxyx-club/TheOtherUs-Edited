namespace TheOtherRoles.Roles.Impostor;

public static class Terrorist
{
    public static PlayerControl terrorist;
    public static Color color = Palette.ImpostorRed;

    public static float destructionTime = 20f;
    public static float destructionRange = 2f;
    public static float hearRange = 30f;
    public static float defuseDuration = 3f;
    public static float bombCooldown = 15f;
    public static float bombActiveAfter = 3f;
    public static bool selfExplosion => destructionTime + bombActiveAfter <= 0.5f;

    public static Sprite buttonSprite = new ResourceSprite("Bomb_Button_Plant.png");

    public static void clearAndReload()
    {
        terrorist = null;
        destructionTime = CustomOptionHolder.terroristBombDestructionTime.GetFloat();
        destructionRange = CustomOptionHolder.terroristBombDestructionRange.GetFloat() / 10;
        hearRange = CustomOptionHolder.terroristBombHearRange.GetFloat() / 10;
        defuseDuration = CustomOptionHolder.terroristDefuseDuration.GetFloat();
        bombCooldown = CustomOptionHolder.terroristBombCooldown.GetFloat();
        bombActiveAfter = CustomOptionHolder.terroristBombActiveAfter.GetFloat();
    }
}
