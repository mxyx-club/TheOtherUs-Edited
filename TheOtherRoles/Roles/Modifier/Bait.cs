using System.Collections.Generic;
using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

// Modifier
public static class Bait
{
    public static List<PlayerControl> bait = new();
    public static Dictionary<DeadPlayer, float> active = new();
    public static Color color = new Color32(0, 247, 255, byte.MaxValue);

    public static float reportDelayMin;
    public static bool SwapCrewmate;
    //public static bool SwapImpostor;
    //public static bool SwapNeutral;
    public static float reportDelayMax;
    public static bool showKillFlash = true;

    public static void clearAndReload()
    {
        bait = new List<PlayerControl>();
        active = new Dictionary<DeadPlayer, float>();
        reportDelayMin = CustomOptionHolder.modifierBaitReportDelayMin.getFloat();
        reportDelayMax = CustomOptionHolder.modifierBaitReportDelayMax.getFloat();
        if (reportDelayMin > reportDelayMax) reportDelayMin = reportDelayMax;
        showKillFlash = CustomOptionHolder.modifierBaitShowKillFlash.getBool();
        SwapCrewmate = CustomOptionHolder.modifierBaitSwapCrewmate.getBool();
        //SwapNeutral = CustomOptionHolder.modifierBaitSwapNeutral.getBool();
        //SwapImpostor = CustomOptionHolder.modifierBaitSwapImpostor.getBool();
    }
}
