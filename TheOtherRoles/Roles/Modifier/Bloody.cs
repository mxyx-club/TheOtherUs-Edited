namespace TheOtherRoles.Roles.Modifier;

public static class Bloody
{
    public static List<PlayerControl> bloody = new();
    public static Dictionary<byte, float> active = new();
    public static Dictionary<byte, byte> bloodyKillerMap = new();

    public static float duration = 5f;

    public static void clearAndReload()
    {
        bloody.Clear();
        active = new();
        bloodyKillerMap = new();
        duration = CustomOptionHolder.modifierBloodyDuration.GetFloat();
    }
}
