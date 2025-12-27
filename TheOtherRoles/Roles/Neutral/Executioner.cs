namespace TheOtherRoles.Roles.Neutral;

public static class Executioner
{
    public static PlayerControl executioner;
    public static PlayerControl target;
    public static Color color = new Color32(140, 64, 5, byte.MaxValue);
    public static bool canCallEmergency;
    public static bool triggerExecutionerWin;
    public static bool promotesToLawyer;
    public static bool targetWasGuessed;
    public static OnTargetDead onTargetDead;

    public enum OnTargetDead
    {
        Pursuer,
        Jester,
        Amnisiac,
        Crewmate
    }

    public static void PromotesRole()
    {
        var player = executioner;
        var target = Executioner.target;
        if (player.IsAlive() && target.IsDead())
        {
            switch (onTargetDead)
            {
                case OnTargetDead.Pursuer:
                    RPCProcedure.setRole(player.PlayerId, (byte)RoleId.Pursuer);
                    break;
                case OnTargetDead.Jester:
                    RPCProcedure.setRole(player.PlayerId, (byte)RoleId.Jester);
                    break;
                case OnTargetDead.Amnisiac:
                    RPCProcedure.setRole(player.PlayerId, (byte)RoleId.Amnisiac);
                    break;
                case OnTargetDead.Crewmate:
                    RPCProcedure.setRole(player.PlayerId, (byte)RoleId.Crewmate);
                    break;
            }
            clearAndReload();
        }
    }

    public static void clearAndReload(bool clearTarget = true)
    {
        if (clearTarget)
        {
            target = null;
            targetWasGuessed = false;
        }
        executioner = null;
        triggerExecutionerWin = false;
        promotesToLawyer = CustomOptionHolder.executionerPromotesToLawyer.GetBool();
        canCallEmergency = CustomOptionHolder.executionerCanCallEmergency.GetBool();
        onTargetDead = CustomOptionHolder.executionerOnTargetDead.GetSelection<OnTargetDead>();

    }
}
