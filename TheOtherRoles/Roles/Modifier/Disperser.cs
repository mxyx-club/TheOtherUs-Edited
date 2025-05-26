namespace TheOtherRoles.Roles.Modifier;

public static class Disperser
{
    public static PlayerControl disperser;
    public static Color color = Palette.ImpostorRed;
    public static int remainingDisperses = 1;
    public static bool DispersesToVent;
    public static ResourceSprite buttonSprite = new("Disperse.png");

    public static void clearAndReload()
    {
        disperser = null;
        remainingDisperses = 1;
        //remainingDisperses = CustomOptionHolder.modifierDisperserRemainingDisperses.GetInt();
        DispersesToVent = CustomOptionHolder.modifierDisperserDispersesToVent.GetBool();
    }
}
