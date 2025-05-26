using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

public static class Ninja
{
    public static PlayerControl ninja;
    public static Color color = Palette.ImpostorRed;

    public static PlayerControl ninjaMarked;
    public static PlayerControl currentTarget;
    public static float cooldown = 30f;
    public static float traceTime = 1f;
    public static bool knowsTargetLocation;
    public static float invisibleDuration = 5f;

    public static float invisibleTimer;
    public static bool isInvisable;
    public static ResourceSprite markButtonSprite = new("NinjaMarkButton.png");
    public static ResourceSprite killButtonSprite = new("NinjaAssassinateButton.png");
    public static Arrow arrow = new(Color.black);

    public static void clearAndReload()
    {
        ninja = null;
        currentTarget = ninjaMarked = null;
        cooldown = CustomOptionHolder.ninjaCooldown.GetFloat();
        knowsTargetLocation = CustomOptionHolder.ninjaKnowsTargetLocation.GetBool();
        traceTime = CustomOptionHolder.ninjaTraceTime.GetFloat();
        invisibleDuration = CustomOptionHolder.ninjaInvisibleDuration.GetFloat();
        invisibleTimer = 0f;
        isInvisable = false;
        if (arrow?.arrow != null) UObject.Destroy(arrow.arrow);
        arrow = new Arrow(Color.black);
        arrow.arrow?.SetActive(false);
    }
}
