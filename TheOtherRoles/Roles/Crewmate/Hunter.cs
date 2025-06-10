namespace TheOtherRoles.Roles.Crewmate;

public class Hunter
{
    public static PlayerControl Player;
    public static PlayerControl currentTarget;
    public static Color color = new Color32(179, 179, 230, byte.MaxValue);


    public static void ClearAndReload()
    {
        Player = null;
    }
}
