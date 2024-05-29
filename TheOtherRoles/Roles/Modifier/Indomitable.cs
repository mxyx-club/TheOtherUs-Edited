using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class Indomitable
{
    public static PlayerControl indomitable;
    public static Color color = new Color32(0, 247, 255, byte.MaxValue);


    public static void clearAndReload()
    {
        indomitable = null;
    }
}
