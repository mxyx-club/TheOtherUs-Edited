namespace TheOtherRoles.Roles.Modifier;

public static class Cursed
{
    public static PlayerControl cursed;
    public static Color color = new Color32(0, 247, 255, byte.MaxValue);
    public static bool hideModifier;

    public static void clearAndReload()
    {
        cursed = null;
        hideModifier = CustomOptionHolder.modifierHideCursed.GetBool();
    }

    public static void TurnToImpostor(byte playerId)
    {
        var player = PlayerById(playerId);
        RPCProcedure.erasePlayerRoles(playerId);
        if (player == cursed) clearAndReload();
        RPCProcedure.setRole(playerId, (byte)RoleId.Impostor);

        if (Executioner.executioner.IsAlive() && Executioner.target == player) Executioner.PromotesRole();
    }
}
