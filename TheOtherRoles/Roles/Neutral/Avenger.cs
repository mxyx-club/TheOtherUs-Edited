namespace TheOtherRoles.Roles.Neutral;

public class Avenger
{
    public static PlayerControl Player;
    public static PlayerControl Lover;
    public static Color color = new Color32(141, 111, 131, byte.MaxValue);

    public static float killCooldown = 30f;
    public static bool canUseVents;
    public static bool hasImpostorVision;

    public static PlayerControl Target;
    public static PlayerControl currentTarget;



    public static void clearAndReload()
    {
        Player = null;
        currentTarget = null;
        //canUseVents = false;
        //hasImpostorVision = false;
    }




}
