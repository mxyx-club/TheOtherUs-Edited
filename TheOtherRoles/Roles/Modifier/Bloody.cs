namespace TheOtherRoles.Roles.Modifier;

public static class Bloody
{
    public static List<PlayerControl> bloody = new();

    public static float duration = 5f;

    public static void clearAndReload()
    {
        bloody.Clear();
        duration = CustomOptionHolder.modifierBloodyDuration.GetFloat();
    }
}
