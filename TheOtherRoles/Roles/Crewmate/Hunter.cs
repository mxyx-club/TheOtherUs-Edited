namespace TheOtherRoles.Roles.Crewmate;

public class Hunter
{
    public static PlayerControl Player;
    public static PlayerControl currentTarget;
    public static Color color = new Color32(179, 179, 230, byte.MaxValue);

    public static bool InfectedDeathFlag;

    public static float cooldown = 30f;
    public static int MaxCount = 5;
    public static int UsedCount;
    public static bool CanStakeRoundOne;
    public static int GuessCount;
    public static int RemainingCount => MaxCount - UsedCount;


    public static void CheckTarget(byte targetId)
    {
        var target = playerById(targetId);

        if (Infected.Player.Any(x => x.PlayerId == targetId))
        {
            var writer = StartRPC(CustomRPC.UncheckedMurderPlayer);
            writer.Write(Player.PlayerId);
            writer.Write(target.PlayerId);
            writer.Write(true);
            writer.EndRPC();
            RPCProcedure.uncheckedMurderPlayer(Player.PlayerId, target.PlayerId, true);

        }
        else
        {
            if (RemainingCount <= 0) RpcMurderPlayer(Player, Player);
        }
        UsedCount++;
    }


    public static void ClearAndReload()
    {
        Player = null;
        UsedCount = 0;
        InfectedDeathFlag = false;
        cooldown = CustomOptionHolder.hunterCooldown.GetFloat();
        MaxCount = CustomOptionHolder.hunterMaxCount.GetInt();
        CanStakeRoundOne = CustomOptionHolder.hunterCanStakeRoundOne.GetBool();
        GuessCount = CustomOptionHolder.hunterCanShootNum.GetInt();
    }
}
