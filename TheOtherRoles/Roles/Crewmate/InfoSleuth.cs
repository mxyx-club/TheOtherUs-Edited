using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public class InfoSleuth
{
    public static PlayerControl infoSleuth;
    public static PlayerControl target;
    public static Color color = new Color32(200, 105, 228, byte.MaxValue);

    public static int infoType = 1;

    public static void clearAndReload()
    {
        infoSleuth = null;
        target = null;
        infoType = CustomOptionHolder.infoSleuthInfoType.getSelection();
    }
}