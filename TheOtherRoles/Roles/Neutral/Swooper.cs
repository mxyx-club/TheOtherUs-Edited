namespace TheOtherRoles.Roles.Neutral;

public static class Swooper
{
    public static PlayerControl swooper;
    public static PlayerControl currentTarget;
    public static float cooldown = 30f;
    public static bool isInvisable;
    public static Color color = new Color32(224, 197, 219, byte.MaxValue);
    public static float duration = 5f;
    public static float swoopCooldown = 30f;
    public static float swoopTimer;
    public static float swoopSpeed;
    public static bool hasImpVision;
    public static bool canUseVents;

    public static ResourceSprite SwoopButtonSprite = new("Swoop.png");

    public static void clearAndReload()
    {
        swooper = null;
        isInvisable = false;
        cooldown = CustomOptionHolder.swooperKillCooldown.GetFloat();
        swoopCooldown = CustomOptionHolder.swooperCooldown.GetFloat();
        duration = CustomOptionHolder.swooperDuration.GetFloat();
        hasImpVision = CustomOptionHolder.swooperHasImpVision.GetBool();
        swoopSpeed = CustomOptionHolder.swooperSpeed.GetFloat();
        canUseVents = CustomOptionHolder.swooperCanUseVents.GetBool();
    }
}
