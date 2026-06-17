namespace TheOtherRoles.Roles.Modifier;

public static class Shifter
{
    public static PlayerControl shifter;

    public static PlayerControl futureShift;
    public static PlayerControl currentTarget;

    public static bool shiftNeutral;
    public static bool shiftALLNeutra;
    public static bool resetRole;

    public static Sprite buttonSprite = new ResourceSprite("ShiftButton.png");

    public static bool NotShift(PlayerControl player)
    {
        if (player == null) return false;
        if (player.IsImpostor()) return true;
        if (shiftNeutral)
        {
            if (shiftALLNeutra)
            {
                return player == Jackal.Sidekick ||
                       player == Pavlovsdogs.pavlovsowner ||
                       player == Avenger.Player ||
                       Jackal.jackal.Any(x => x == player) ||
                       Infected.Player.Any(x => x == player) ||
                       Pavlovsdogs.pavlovsdogs.Any(x => x == player) ||
                       player == Akujo.akujo ||
                       player == Lawyer.lawyer;
            }
            else
            {
                return player == Jackal.Sidekick ||
                       player == Werewolf.werewolf ||
                       player == Lawyer.lawyer ||
                       player == Avenger.Player ||
                       player == Juggernaut.juggernaut ||
                       player == Akujo.akujo ||
                       player == Pelican.Player ||
                       player == Swooper.swooper ||
                       player == SchrodingersCat.Player ||
                       player == Pavlovsdogs.pavlovsowner ||
                       Infected.Player.Any(x => x == player) ||
                       Jackal.jackal.Any(x => x == player) ||
                       Infected.Player.Any(x => x == player) ||
                       Pavlovsdogs.pavlovsdogs.Any(x => x == player);
            }
        }
        return player.IsNeutral();
    }

    public static void shiftRole(PlayerControl player1, PlayerControl player2, bool repeat = true)
    {
        if (player1 == null || player2 == null) return;
        var role = RoleInfo.getRoleInfoForPlayer(player2, false, false).FirstOrDefault();
        if (role == null) return;

        RPCProcedure.ResetRole(role.roleId, player2, resetRole);
        if (role.roleId == RoleId.Avenger) role = RoleInfo.jester;
        RPCProcedure.setRole(player1.PlayerId, (byte)role.roleId);
        if (repeat) shiftRole(player2, player1, false);
    }

    public static void clearAndReload()
    {
        shifter = null;
        futureShift = null;
        currentTarget = null;
        shiftNeutral = CustomOptionHolder.modifierShiftNeutral.GetBool();
        shiftALLNeutra = CustomOptionHolder.modifierShiftALLNeutral.GetBool();
        resetRole = CustomOptionHolder.modifierShiftReload.GetBool();
    }
}