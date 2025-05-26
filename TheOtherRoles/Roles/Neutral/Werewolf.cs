namespace TheOtherRoles.Roles.Neutral;

public static class Werewolf
{
    public static PlayerControl werewolf;
    public static PlayerControl currentTarget;
    public static Color color = new Color32(79, 56, 21, byte.MaxValue);

    public static float killCooldown = 3f;
    public static float rampageCooldown = 30f;
    public static float rampageDuration = 5f;
    public static bool canUseVents;
    public static bool canKill;
    public static bool hasImpostorVision;

    public static ResourceSprite buttonSprite = new("Rampage.png");

    public static void clearAndReload()
    {
        werewolf = null;
        currentTarget = null;
        canUseVents = false;
        canKill = false;
        hasImpostorVision = false;
        rampageCooldown = CustomOptionHolder.werewolfRampageCooldown.GetFloat();
        rampageDuration = CustomOptionHolder.werewolfRampageDuration.GetFloat();
        killCooldown = CustomOptionHolder.werewolfKillCooldown.GetFloat();
    }
}
