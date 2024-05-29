using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class Watcher
{
    public static PlayerControl watcher;
    public static Color color = new Color32(48, 21, 89, byte.MaxValue);


    public static void clearAndReload()
    {
        watcher = null;
    }
}
