using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class Slueth
{
    public static PlayerControl slueth;
    public static Color color = new Color32(48, 21, 89, byte.MaxValue);
    public static List<PlayerControl> reported = new();

    public static void clearAndReload()
    {
        slueth = null;
        reported = new List<PlayerControl>();
    }
}
