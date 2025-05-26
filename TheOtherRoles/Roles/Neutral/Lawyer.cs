namespace TheOtherRoles.Roles.Neutral;

public static class Lawyer
{
    public static PlayerControl lawyer;
    public static PlayerControl target;
    public static Color color = new Color32(134, 153, 25, byte.MaxValue);
    public static bool canCallEmergency = true;
    public static bool targetKnows;
    public static bool stolenWin;
    public static bool notAckedExiled;

    public static float vision = 1f;
    public static bool lawyerKnowsRole;
    public static bool targetCanBeJester;
    public static bool targetWasGuessed;

    public static void PromotesToPursuer(bool suicide = false)
    {
        var player = lawyer;

        if (suicide)
        {
            player?.Exiled();
            GameHistory.RpcOverrideDeathReasonAndKiller(player, CustomDeathReason.LawyerSuicide, player);
            return;
        }

        if (player.IsAlive() && target.IsDead())
        {
            Pursuer.Player.Add(player);
            clearAndReload();
        }
    }

    public static void clearAndReload(bool clearTarget = true)
    {
        lawyer = null;
        if (clearTarget)
        {
            target = null;
            targetWasGuessed = false;
        }

        vision = CustomOptionHolder.lawyerVision.GetFloat();
        targetKnows = CustomOptionHolder.lawyerTargetKnows.GetBool();
        lawyerKnowsRole = CustomOptionHolder.lawyerKnowsRole.GetBool();
        targetCanBeJester = CustomOptionHolder.lawyerTargetCanBeJester.GetBool();
        stolenWin = CustomOptionHolder.lawyerStolenWin.GetBool();
        canCallEmergency = CustomOptionHolder.lawyerCanCallEmergency.GetBool();
    }
}
