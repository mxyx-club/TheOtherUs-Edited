using System.Collections.Generic;

namespace TheOtherRoles.Roles.Modifier;

public static class Multitasker
{
    public static List<PlayerControl> multitasker = new();

    public static void clearAndReload()
    {
        multitasker = new List<PlayerControl>();
    }
}
