namespace TheOtherRoles.Roles.Modifier;

public static class Giant
{
    public static PlayerControl giant;
    public static float speed = 0.75f;
    public static float size = 1.12f;

    public static void clearAndReload()
    {
        giant = null;
        speed = CustomOptionHolder.modifierGiantSpped.getFloat();
    }
}
