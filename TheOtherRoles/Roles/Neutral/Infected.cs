namespace TheOtherRoles.Roles.Neutral;

public class Infected
{
    public static List<PlayerControl> Player = new();
    public static PlayerControl currentTarget;
    public static Color color = new Color32(179, 217, 77, byte.MaxValue);

    public static int MaxPlayer = 3;
    public static int ActiveLimit = 2;
    public static float cooldown = 30f;
    public static bool canUseVents = true;
    public static bool hasImpostorVision;
    public static int CreatedCount;
    public static bool IsGuesser;
    public static int GuessCount;

    public static void KillPlayer(PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null) return;

        if (Hunter.Player != null && target == Hunter.Player)
        {
            RpcMurderPlayer(target, player);
            return;
        }

        if (SchrodingersCat.Player != null && target == SchrodingersCat.Player)
        {
            RpcMurderPlayer(player, target);
            return;
        }

        if (CreatedCount >= MaxPlayer || Player.Count(x => x.IsAlive()) >= ActiveLimit || target.IsKiller())
        {
            RpcMurderPlayer(player, target);
        }
        else
        {
            var writer = StartRPC(CustomRPC.InfectedTarget);
            writer.Write(player.PlayerId);
            writer.Write(target.PlayerId);
            writer.EndRPC();
            InfectedTarget(player.PlayerId, target.PlayerId);
        }
    }

    public static void InfectedTarget(byte playerId, byte targetId)
    {
        var player = PlayerById(playerId);
        var target = PlayerById(targetId);

        if (Executioner.target == target && Executioner.executioner != null && !Executioner.executioner.Data.IsDead)
        {
            if (Lawyer.lawyer == null && Executioner.promotesToLawyer)
            {
                Lawyer.lawyer = Executioner.executioner;
                Lawyer.target = Executioner.target;
                Executioner.clearAndReload();
            }
            else if (!Executioner.promotesToLawyer)
            {
                Pursuer.Player.Add(Executioner.executioner);
                Executioner.clearAndReload();
            }
        }
        RPCProcedure.erasePlayerRoles(targetId);
        Player.Add(target);
        CreatedCount++;
    }


    public static void clearAndReload()
    {
        Player = new();
        currentTarget = null;
        CreatedCount = 0;
        cooldown = CustomOptionHolder.infectedKillCooldown.GetFloat();
        MaxPlayer = CustomOptionHolder.infectedMaxPlayer.GetInt();
        ActiveLimit = CustomOptionHolder.infectedActiveLimit.GetInt();
        ActiveLimit = Mathf.Min(ActiveLimit, MaxPlayer);
        canUseVents = CustomOptionHolder.infectedCanUseVents.GetBool();
        IsGuesser = CustomOptionHolder.infectedCanUseGuess.GetBool();
        hasImpostorVision = CustomOptionHolder.infectedHasImpostorVision.GetBool();
        GuessCount = CustomOptionHolder.infectedGuessCount.GetInt();
    }
}
