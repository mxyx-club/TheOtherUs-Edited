using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Neutral;

public static class Vulture
{
    public static PlayerControl vulture;
    public static Color color = new Color32(139, 69, 19, byte.MaxValue);
    public static List<Arrow> localArrows = new();
    public static float cooldown = 30f;
    public static int vultureNumberToWin = 4;
    public static int eatenBodies;
    public static bool triggerVultureWin;
    public static bool canUseVents = true;
    public static bool showArrows = true;
    public static Sprite buttonSprite = new ResourceSprite("VultureButton.png");

    public static void clearAndReload()
    {
        vulture = null;
        vultureNumberToWin = CustomOptionHolder.vultureNumberToWin.GetInt();
        eatenBodies = 0;
        cooldown = CustomOptionHolder.vultureCooldown.GetFloat();
        triggerVultureWin = false;
        canUseVents = CustomOptionHolder.vultureCanUseVents.GetBool();
        showArrows = CustomOptionHolder.vultureShowArrows.GetBool();
        foreach (var arrow in localArrows)
            if (arrow?.arrow != null)
                UObject.Destroy(arrow.arrow);
        localArrows.Clear();
    }
}
