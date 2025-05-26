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

    public static void PromotesRole()
    {
        var player = executioner;
        var target = Executioner.target;
        if (player.IsAlive() && target.IsDead())
        {
            Pursuer.Player.Add(player);
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
    }
}
