namespace TheOtherRoles.Roles.Impostor;

public class Butcher
{
    public static PlayerControl butcher;
    public static PlayerControl dissected;
    public static Color color = Palette.ImpostorRed;

    public static float dissectionCooldown = 30f;
    public static float dissectionDuration = 10f;
    public static int dissectedBodyCount = 5;

    public static bool canDissection;

    public static ResourceSprite ButtonSprite = new("DissectedButton.png");

    public static void clearAndReload()
    {
        butcher = null;
        dissected = null;
        canDissection = true;
        dissectionCooldown = CustomOptionHolder.butcherDissectionCooldown.GetFloat();
        dissectionDuration = CustomOptionHolder.butcherDissectionDuration.GetFloat();
        dissectedBodyCount = CustomOptionHolder.butcherDissectedBodyCount.GetInt();
    }
}