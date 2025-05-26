namespace TheOtherRoles.Roles.Modifier;

public static class Flash
{
    public static List<PlayerControl> flash = new();
    public static float speed = 1.5f;

    public static void clearAndReload()
    {
        flash.Clear();
        speed = CustomOptionHolder.modifierFlashSpeed.GetFloat();
    }
}
