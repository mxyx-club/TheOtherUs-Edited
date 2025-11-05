namespace TheOtherRoles.Roles.Ghost;

public class Clog
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;

    public static float GhostCooldown;
    public static float GhostDuration;
    public static float GhostRange;
    public static int CanUseNum;
    public static bool OnlyUsedOnce;

    public static bool IsUsed;
    public static int UsedNum;

    public static Sprite ButtonSprite = new ResourceSprite("GhostButton.png");

    public static void ClearAndReload()
    {
        Player = null;
        IsUsed = false;
        UsedNum = 0;
        GhostCooldown = CustomOptionHolder.clogGhostCooldown.GetFloat();
        GhostDuration = CustomOptionHolder.clogGhostDuration.GetFloat();
        GhostRange = CustomOptionHolder.clogGhostRange.GetFloat();
        OnlyUsedOnce = CustomOptionHolder.clogOnlyUsedOnce.GetBool();
        CanUseNum = CustomOptionHolder.clogUseNum.GetInt();
    }
}
