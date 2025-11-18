using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

public class Detective : RoleBase
{
    public static Color color = new Color32(8, 180, 180, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Detective),
        (p) => new Detective(p),
        RoleId.Detective,
        RoleType.Crewmate,
        "Detective",
        color,
        301900,
        AddOptions
    );
    public Detective(PlayerControl p) : base(p, roleinfo) { }

    public static float footprintIntervall = 1f;
    public static float footprintDuration = 1f;
    public static int anonymousFootprints;
    public static float reportNameDuration;
    public static float reportColorDuration = 20f;
    public static float timer = 6.2f;

    public static CustomOption detectiveAnonymousFootprints;
    public static CustomOption detectiveFootprintIntervall;
    public static CustomOption detectiveFootprintDuration;
    public static CustomOption detectiveReportNameDuration;
    public static CustomOption detectiveReportColorDuration;
    //public static float reportRoleDuration;
    //public static float reportInfoDuration = 20f;

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        detectiveAnonymousFootprints = CustomOption.Create(configId++, CustomOptionType.Crewmate, "detectiveAnonymousFootprints",
            ["optionOff", "detectiveAnonymousFootprints1", "optionOn"], roleinfo.RoleOption);
        detectiveFootprintIntervall = CustomOption.Create(configId++, CustomOptionType.Crewmate, "detectiveFootprintIntervall", 0.25f, 0.25f, 10f, 0.25f, roleinfo.RoleOption);
        detectiveFootprintDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "detectiveFootprintDuration", 12.5f, 0.5f, 30f, 0.5f, roleinfo.RoleOption);
        detectiveReportNameDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "detectiveReportNameDuration", 10f, 0f, 60f, 2.5f, roleinfo.RoleOption);
        detectiveReportColorDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "detectiveReportColorDuration", 30f, 0f, 120f, 2.5f, roleinfo.RoleOption);

    }

    public override void Initialize()
    {
        anonymousFootprints = detectiveAnonymousFootprints.GetSelection();
        footprintIntervall = detectiveFootprintIntervall.GetFloat();
        footprintDuration = detectiveFootprintDuration.GetFloat();
        reportNameDuration = detectiveReportNameDuration.GetFloat();
        reportColorDuration = detectiveReportColorDuration.GetFloat();
        timer = 6.2f;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (InMeeting || Player.IsDead()) return;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            timer = footprintIntervall;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                if (p.IsAlive() && p != PlayerControl.LocalPlayer && !p.inVent)
                    FootprintHolder.Instance.MakeFootprint(p);
        }
    }

    public override void OnReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target)
    {
        if (reporter == Player)
        {
            var deadPlayer = PlayerData.AllPlayerData.Values.Where(x => x.Player?.PlayerId == target?.PlayerId && x.IsDead)?.FirstOrDefault();
            if (deadPlayer != null && deadPlayer.KilledBy != null)
            {
                var timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.DeathTimer).TotalMilliseconds;
                var msg = "";
                var killer = deadPlayer.KilledBy;
                float timer = (float)Math.Round(timeSinceDeath / 1000);
                if (Vortox.Reversal)
                {
                    timer += rnd.Next(-2, 3);
                    if (timer < 0) timer = 1;
                }
                if (timer <= reportNameDuration)
                {
                    msg = $"尸检报告: 凶手的职业似乎是 {target?.Object?.GetRoleBase()?.RoleName ?? "(NULL)"} !\n尸体在 {timer} 秒前死亡";
                }
                else if (timer <= reportColorDuration)
                {
                    msg = $"尸检报告: 凶手的阵营似乎是 {teamString(killer)} !\n尸体在{timer}秒前死亡";
                }
                else
                {
                    msg = $"尸检报告: 死亡时间太久，无法获取信息\n尸体在 {timer} 秒前死亡";
                }
            }
        }
    }
}
