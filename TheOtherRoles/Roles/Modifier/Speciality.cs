using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

// Modifier
public static class Specoality
{
    public static PlayerControl specoality;
    public static Color color = new Color32(48, 21, 89, byte.MaxValue);
    public static int linearfunction = 1;

    public static void clearAndReload()
    {
        linearfunction = 1;
        //SwapNeutral = CustomOptionHolder.modifierBaitSwapNeutral.getBool();
        //SwapImpostor = CustomOptionHolder.modifierBaitSwapImpostor.getBool();
    }
}
