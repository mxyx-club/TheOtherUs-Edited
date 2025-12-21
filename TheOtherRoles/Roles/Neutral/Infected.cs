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

    public static bool KillPlayer(PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null) return false;
        if (!CheckMurderPlayer(player, target)) return false;

        if (SchrodingersCat.Player != null && target == SchrodingersCat.Player && SchrodingersCat.remainingChange > 0)
        {
            return RpcCustomMurderPlayer(player, target); ;
        }

        if (CreatedCount >= MaxPlayer || Player.Count(x => x.IsAlive()) >= ActiveLimit || target.IsKiller())
        {
            return RpcCustomMurderPlayer(player, target, true);
        }
        else
        {
            var writer = StartRPC(CustomRPC.InfectedTarget);
            writer.Write(player.PlayerId);
            writer.Write(target.PlayerId);
            writer.Write(CreatedCount + 1);
            writer.EndRPC();
            InfectedTarget(player.PlayerId, target.PlayerId, CreatedCount + 1);
            return true;
        }
    }

    public static void InfectedTarget(byte playerId, byte targetId, int count)
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
        RPCProcedure.setRole(targetId, (byte)RoleId.Infected);
        CreatedCount = count;
    }


    public static void clearAndReload()
    {
        Player = new();
        currentTarget = null;
        CreatedCount = 1;
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
