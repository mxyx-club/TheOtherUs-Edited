namespace TheOtherRoles.Roles.Modifier;

public static class Specoality
{
    public static PlayerControl specoality;
    public static Color color = Palette.ImpostorRed;
    public static int linearfunction = 1;
    public static bool IsGlobalModifier;

    public static void clearAndReload()
    {
        specoality = null;
        linearfunction = 1;
        IsGlobalModifier = CustomOptionHolder.modifierSpecoalityIsGlobal.GetBool();
    }
}
