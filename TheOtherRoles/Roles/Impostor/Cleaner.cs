namespace TheOtherRoles.Roles.Impostor;

public static class Cleaner
{
    public static PlayerControl cleaner;
    public static Color color = Palette.ImpostorRed;

    public static float cooldown = 30f;

    public static ResourceSprite buttonSprite = new("CleanButton.png");

    public static void clearAndReload()
    {
        cleaner = null;
        cooldown = CustomOptionHolder.cleanerCooldown.GetFloat();
    }
}
