using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public class Aftermath
{
    public static PlayerControl aftermath;
    public static Color color = new Color32(236, 98, 165, byte.MaxValue);

    public static void clearAndReload()
    {
        aftermath = null;
    }
}
