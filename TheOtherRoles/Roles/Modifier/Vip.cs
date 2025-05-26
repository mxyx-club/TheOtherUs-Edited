namespace TheOtherRoles.Roles.Modifier;

public static class Vip
{
    public static List<PlayerControl> vip = new();
    public static bool showColor = true;

    public static void clearAndReload()
    {
        vip.Clear();
        showColor = CustomOptionHolder.modifierVipShowColor.GetBool();
    }
}
