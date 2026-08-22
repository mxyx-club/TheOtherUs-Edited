namespace TheOtherRoles.Roles.Impostor;

public static class Vampire
{
    public static PlayerControl vampire;
    public static Color color = Palette.ImpostorRed;
    public static PlayerControl currentThrall;

    public static float delay = 10f;
    public static float cooldown = 30f;
    public static bool canKillNearGarlics = true;
    public static bool localPlacedGarlic;
    public static bool garlicsActive = true;
    public static bool garlicButton;

    public static bool hasRecruited;          // 是否已经招募过
    public static bool thrallNotified;        // 眷属是否已得知转变
    public static bool message;
    public static float recruitCooldown = 25f;
    public static bool canRecruit = true;

    public static PlayerControl currentTarget;
    public static PlayerControl bitten;
    public static bool targetNearGarlic;

    public static Sprite buttonSprite = new ResourceSprite("VampireButton.png");
    public static Sprite recruitButtonSprite = new ResourceSprite("VampireRecruit.png");
    public static Sprite garlicButtonSprite = new ResourceSprite("GarlicButton.png");

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
        canRecruit = CustomOptionHolder.vampireCanRecruit.GetBool();
        recruitCooldown = CustomOptionHolder.vampireRecruitCooldown.GetFloat();

        currentThrall = null;
        hasRecruited = false;
        thrallNotified = false;
        message = false;
    }
}
