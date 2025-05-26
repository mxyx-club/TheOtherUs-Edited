namespace TheOtherRoles.Roles.Modifier;

public static class Sunglasses
{
    public static List<PlayerControl> sunglasses = new();
    public static int vision = 1;

    public static void clearAndReload()
    {
        sunglasses.Clear();
        vision = CustomOptionHolder.modifierSunglassesVision.GetSelection() + 1;
    }
}
