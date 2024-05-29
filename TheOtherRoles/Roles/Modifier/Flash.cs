using System.Collections.Generic;

namespace TheOtherRoles.Roles.Modifier;

public static class Flash
{
    public static List<PlayerControl> flash = new();
    public static float speed = 1.5f;

    public static void clearAndReload()
    {
        flash = new List<PlayerControl>();
        speed = CustomOptionHolder.modifierFlashSpeed.getFloat();
    }
}
