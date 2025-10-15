using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Neutral;

public class Avenger
{
    public static PlayerControl Player;
    public static PlayerControl Lover;
    public static Color color = new Color32(141, 111, 131, byte.MaxValue);

    public static float killCooldown = 30f;
    public static bool IsGuessable;
    public static bool CanFreeKill;
    public static bool canUseVents;
    public static bool hasImpostorVision;
    public static bool ShowArrows;
    public static float UpdateIntervall;

    public static WinnerFlags WinCondition;
    public static Arrow Arrow;
    public static float AvengerRate;

    public static PlayerControl Target;
    public static PlayerControl currentTarget;

    public static void ClearAndReload()
    {
        Player = Lover = null;
        Target = currentTarget = null;
        IsGuessable = CustomOptionHolder.avengerIsGuessable.GetBool();
        CanFreeKill = CustomOptionHolder.avengerCanFreeKill.GetBool();
        AvengerRate = CustomOptionHolder.modifierLoverAvengerChance.GetSelection() * 10;
        canUseVents = CustomOptionHolder.avengerCanUseVents.GetBool();
        hasImpostorVision = CustomOptionHolder.avengerHasImpVision.GetBool();
        killCooldown = CustomOptionHolder.avengerKillCooldown.GetFloat();
        ShowArrows = CustomOptionHolder.avengerShowArrows.GetBool();
        UpdateIntervall = CustomOptionHolder.avengerUpdateIntervall.GetFloat();
        WinCondition = (WinnerFlags)CustomOptionHolder.avengerWinCondition.GetSelection();
        Arrow?.arrow?.Destroy();
        Arrow = null;
    }


    public enum WinnerFlags
    {
        FollowWin,
        StealWin,
        RevengeWin
    }

}
