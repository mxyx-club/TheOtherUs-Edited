namespace TheOtherRoles.Roles.Impostor;

public static class Blackmailer
{
    public static PlayerControl blackmailer;
    public static Color color = Palette.ImpostorRed;
    public static Color blackmailedColor = Palette.White;

    public static bool alreadyShook;
    public static PlayerControl blackmailed;
    public static PlayerControl currentTarget;
    public static float cooldown = 30f;
    public static ResourceSprite blackmailButtonSprite = new("BlackmailerBlackmailButton.png");
    public static ResourceSprite overlaySprite = new("BlackmailerOverlay.png", 100);

    public static void clearAndReload()
    {
        blackmailer = null;
        currentTarget = null;
        blackmailed = null;
        alreadyShook = false;
        cooldown = CustomOptionHolder.blackmailerCooldown.GetFloat();
    }
}
