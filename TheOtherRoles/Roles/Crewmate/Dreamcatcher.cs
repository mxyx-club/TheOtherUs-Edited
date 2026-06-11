namespace TheOtherRoles.Roles.Crewmate;

public class Dreamcatcher
{
    public static PlayerControl Player;
    public static PlayerControl CurrenTarget;
    public static PlayerControl Dreamed;
    public static PlayerControl LastDreamed;










    public static void ClearAndReload()
    {
        Player = null;
        CurrenTarget = null;
        Dreamed = null;
        LastDreamed = null;
    }
}
