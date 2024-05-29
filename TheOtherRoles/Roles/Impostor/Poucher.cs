using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles.Roles.Impostor;

public static class Poucher
{
    public static PlayerControl poucher;
    public static bool spawnModifier;
    public static Color color = Palette.ImpostorRed;
    public static List<PlayerControl> killed = new();


    public static void clearAndReload(bool clearList = true)
    {
        poucher = null;
        spawnModifier = CustomOptionHolder.poucherSpawnModifier.getBool();
        if (clearList) killed = new List<PlayerControl>();
    }
}
