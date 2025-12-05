namespace TheOtherRoles.Roles.Impostor;

public class Butcher
{
    public static PlayerControl butcher;
    public static byte dissectedId;
    public static Color color = Palette.ImpostorRed;

    public static float dissectionCooldown = 30f;
    public static float dissectionDuration = 10f;
    public static int dissectedBodyCount = 5;

    public static bool canDissection;

    public static Sprite ButtonSprite = new ResourceSprite("DissectedButton.png");

    public static void clearAndReload()
    {
        butcher = null;
        dissectedId = byte.MaxValue;
        canDissection = true;
        dissectionCooldown = CustomOptionHolder.butcherDissectionCooldown.GetFloat();
        dissectionDuration = CustomOptionHolder.butcherDissectionDuration.GetFloat();
        dissectedBodyCount = CustomOptionHolder.butcherDissectedBodyCount.GetInt();
    }
}