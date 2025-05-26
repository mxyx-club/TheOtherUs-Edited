namespace TheOtherRoles.Roles.Impostor;

public static class Poucher
{
    public static PlayerControl poucher;
    public static bool spawnModifier;
    public static Color color = Palette.ImpostorRed;
    public static List<PlayerControl> killed = new();


    public static void clearAndReload()
    {
        poucher = null;
        killed.Clear();
        spawnModifier = CustomOptionHolder.modifierPoucher.GetBool();
    }
}
