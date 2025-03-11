namespace TheOtherRoles.Helper;

public static class PlayerHelpers
{
    public static void RpcMurderyPlayer(this PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null) return;
        player.MurderPlayer(target, MurderResultFlags.Succeeded);
        GameHistory.OverrideDeathReasonAndKiller(target, CustomDeathReason.Kill, player);
    }
}
