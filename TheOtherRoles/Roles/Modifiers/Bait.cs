namespace TheOtherRoles.Roles.Modifier;

// Modifier
public class Bait : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Bait),
        (p) => new Bait(p),
        RoleId.Bait,
        "Bait",
        color,
        401500,
        AddOptions
    );

    public Bait(PlayerControl p) : base(p, roleinfo) { }

    public override RoleType[] RemoveTeam { get; set; }
    public override RoleId[] RemoveRole { get; set; } = [RoleId.SchrodingersCat];

    public static int reportDelayMin;
    public static int reportDelayMax;
    public static bool SwapCrewmate;
    public static bool showKillFlash = true;

    public static CustomOption modifierBaitReportDelayMin;
    public static CustomOption modifierBaitReportDelayMax;
    public static CustomOption modifierBaitShowKillFlash;
    public static CustomOption modifierBaitSwapCrewmate;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierBaitSwapCrewmate = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierBaitSwapCrewmate", false, roleinfo.RoleOption);
        modifierBaitReportDelayMin = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierBaitReportDelayMin", 0, 0, 10, 1, roleinfo.RoleOption);
        modifierBaitReportDelayMax = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierBaitReportDelayMax", 1, 0, 10, 1, roleinfo.RoleOption);
        modifierBaitShowKillFlash = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierBaitShowKillFlash", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        reportDelayMin = modifierBaitReportDelayMin.GetInt();
        reportDelayMax = modifierBaitReportDelayMax.GetInt();
        if (reportDelayMin > reportDelayMax) reportDelayMin = reportDelayMax;
        showKillFlash = modifierBaitShowKillFlash.GetBool();
        SwapCrewmate = modifierBaitSwapCrewmate.GetBool();
        RemoveTeam = SwapCrewmate ? [RoleType.Neutral, RoleType.Impostor] : [];
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Info.Target?.PlayerId == Player.PlayerId)
        {
            float reportDelay = rnd.Next(reportDelayMin, reportDelayMax + 1);

            if (showKillFlash && Info.Killer == PlayerControl.LocalPlayer)
                showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
            if (Info.Killer.AmOwner)
            {
                _ = new LateTask(() =>
                {
                    Info.Killer.CmdReportDeadBody(Player?.Data ?? null);
                }, reportDelay, "Murder Bait");
            }
        }

    }
}
