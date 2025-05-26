namespace TheOtherRoles.Roles.Modifier;

// Modifier
public static class Bait
{
    public static List<PlayerControl> bait = new();
    public static Color color = new Color32(0, 247, 255, byte.MaxValue);

    public static float reportDelayMin;
    public static bool SwapCrewmate;
    //public static bool SwapImpostor;
    //public static bool SwapNeutral;
    public static float reportDelayMax;
    public static bool showKillFlash = true;

    public static void clearAndReload()
    {
        bait.Clear();
        reportDelayMin = CustomOptionHolder.modifierBaitReportDelayMin.GetFloat();
        reportDelayMax = CustomOptionHolder.modifierBaitReportDelayMax.GetFloat();
        if (reportDelayMin > reportDelayMax) reportDelayMin = reportDelayMax;
        showKillFlash = CustomOptionHolder.modifierBaitShowKillFlash.GetBool();
        SwapCrewmate = CustomOptionHolder.modifierBaitSwapCrewmate.GetBool();
        //SwapNeutral = CustomOptionHolder.modifierBaitSwapNeutral.getBool();
        //SwapImpostor = CustomOptionHolder.modifierBaitSwapImpostor.getBool();
    }
}
