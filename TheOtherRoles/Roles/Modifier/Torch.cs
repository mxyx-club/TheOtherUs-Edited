using System.Collections.Generic;

namespace TheOtherRoles.Roles.Modifier;

public static class Torch
{
    public static List<PlayerControl> torch = new();
    public static float vision = 1;

    public static void clearAndReload()
    {
        torch = new List<PlayerControl>();
        vision = CustomOptionHolder.modifierTorchVision.getFloat();
    }
}
