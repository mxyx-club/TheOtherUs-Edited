using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class Blind
{
    public static PlayerControl blind;
    public static Color color = new Color32(48, 21, 89, byte.MaxValue);


    public static void clearAndReload()
    {
        blind = null;
    }
}
