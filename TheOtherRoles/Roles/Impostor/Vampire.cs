namespace TheOtherRoles.Roles.Impostor;

public static class Vampire
{
    public static PlayerControl vampire;
    public static Color color = Palette.ImpostorRed;

    public static float delay = 10f;
    public static float cooldown = 30f;
    public static bool canKillNearGarlics = true;
    public static bool localPlacedGarlic;
    public static bool garlicsActive = true;
    public static bool garlicButton;

    public static PlayerControl currentTarget;
    public static PlayerControl bitten;
    public static bool targetNearGarlic;

    public static ResourceSprite buttonSprite = new("VampireButton.png");

    public static ResourceSprite garlicButtonSprite = new("GarlicButton.png");

    public static void clearAndReload()
    {
        vampire = null;
        bitten = null;
        targetNearGarlic = false;
        localPlacedGarlic = false;
        currentTarget = null;
        garlicsActive = CustomOptionHolder.vampireSpawnRate.GetSelection() > 0;
        delay = CustomOptionHolder.vampireKillDelay.GetFloat();
        cooldown = CustomOptionHolder.vampireCooldown.GetFloat();
        canKillNearGarlics = CustomOptionHolder.vampireCanKillNearGarlics.GetBool();
        garlicButton = CustomOptionHolder.vampireGarlicButton.GetBool();
    }
}
