using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

public class Snitch : RoleBase
{
    public static Color color = new Color32(184, 251, 79, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Snitch),
        (p) => new Snitch(p),
        RoleId.Snitch,
        RoleType.Crewmate,
        "Snitch",
        color,
        302700,
        AddOptions
    );

    public Snitch(PlayerControl p) : base(p, roleinfo) { }

    public List<Arrow> localArrows = new();
    public static int taskCountForReveal = 1;
    public static bool seeInMeeting;
    public static bool canSeeRoles;
    public static bool teamNeutraUseDifferentArrowColor = true;

    public enum includeNeutralTeam
    {
        NoIncNeutral = 0,
        KillNeutral = 1,
        EvilNeutral = 2,
        AllNeutral = 3
    }

    public static includeNeutralTeam Team = includeNeutralTeam.KillNeutral;
    public static TextMeshPro text;

    public static CustomOption snitchLeftTasksForReveal;
    public static CustomOption snitchSeeMeeting;
    public static CustomOption snitchCanSeeRoles;
    public static CustomOption snitchIncludeNeutralTeam;
    public static CustomOption snitchTeamNeutraUseDifferentArrowColor;

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        snitchLeftTasksForReveal = CustomOption.Create(configId++, CustomOptionType.Crewmate, "snitchLeftTasksForReveal", 1, 0, 10, 1, roleinfo.RoleOption);
        snitchSeeMeeting = CustomOption.Create(configId++, CustomOptionType.Crewmate, "snitchSeeMeeting", true, roleinfo.RoleOption);
        snitchCanSeeRoles = CustomOption.Create(configId++, CustomOptionType.Crewmate, "snitchCanSeeRoles", false, roleinfo.RoleOption);
        snitchIncludeNeutralTeam = CustomOption.Create(configId++, CustomOptionType.Crewmate, "snitchIncludeNeutralTeam",
            ["optionOff", "snitchIncludeNeutralTeam2", "snitchIncludeNeutralTeam3", "snitchIncludeNeutralTeam4"], roleinfo.RoleOption);
        snitchTeamNeutraUseDifferentArrowColor = CustomOption.Create(configId++, CustomOptionType.Crewmate, "snitchTeamNeutraUseDifferentArrowColor", true, snitchIncludeNeutralTeam);
    }

    public override void Initialize()
    {
        if (localArrows != null)
        {
            foreach (Arrow arrow in localArrows)
                if (arrow?.arrow != null)
                    UObject.Destroy(arrow.arrow);
        }
        localArrows.Clear();
        taskCountForReveal = snitchLeftTasksForReveal.GetInt();
        seeInMeeting = snitchSeeMeeting.GetBool();
        if (text != null) UObject.Destroy(text);
        text = null;

        canSeeRoles = snitchCanSeeRoles.GetBool();
        Team = (includeNeutralTeam)snitchIncludeNeutralTeam.GetSelection();
        teamNeutraUseDifferentArrowColor = snitchTeamNeutraUseDifferentArrowColor.GetBool();
    }

    public void ArrowUpdate()
    {
        if (localArrows == null) return;

        foreach (var arrow in localArrows) arrow.arrow.SetActive(false);

        if (Player.IsDead()) return;

        var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Player.Data);
        var numberOfTasks = playerTotal - playerCompleted;

        var snitchIsDead = Player.Data.IsDead;
        var local = PlayerControl.LocalPlayer;

        var forImpTeam = local.Data.Role.IsImpostor;
        var forKillerTeam = Team == includeNeutralTeam.KillNeutral && local.IsKillerNeutral();
        var forEvilTeam = Team == includeNeutralTeam.EvilNeutral && local.IsEvilNeutral();
        var forNeutraTeam = Team == includeNeutralTeam.AllNeutral && local.IsNeutral();

        if (numberOfTasks <= taskCountForReveal && (forImpTeam || forKillerTeam || forEvilTeam || forNeutraTeam))
        {
            if (localArrows.Count == 0) localArrows.Add(new Arrow(color));
            if (localArrows.Count != 0 && localArrows[0] != null)
            {
                localArrows[0].arrow.SetActive(true);
                localArrows[0].Update(Player.transform.position);
            }
        }
        else if (local == Player && numberOfTasks == 0 && !snitchIsDead)
        {
            var arrowIndex = 0;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                var arrowForImp = p.IsImpostor();
                //if (Mimic.mimic == p) arrowForImp = true;
                var arrowForKillerTeam = Team == includeNeutralTeam.KillNeutral && p.IsKillerNeutral();
                var arrowForEvilTeam = Team == includeNeutralTeam.EvilNeutral && p.IsEvilNeutral();
                var arrowForNeutraTeam = Team == includeNeutralTeam.AllNeutral && p.IsNeutral();
                var targetsRole = p.GetRoleInfo();

                if (!p.Data.IsDead && (arrowForImp || arrowForKillerTeam || arrowForEvilTeam || arrowForNeutraTeam))
                {
                    if (arrowIndex >= localArrows.Count)
                    {
                        localArrows.Add(new Arrow(Palette.ImpostorRed));
                    }
                    if (arrowIndex < localArrows.Count && localArrows[arrowIndex] != null)
                    {
                        localArrows[arrowIndex].arrow.SetActive(true);
                        if (arrowForImp)
                        {
                            localArrows[arrowIndex].Update(p.transform.position, Palette.ImpostorRed);
                        }
                        else if (arrowForKillerTeam || arrowForEvilTeam || arrowForNeutraTeam)
                        {
                            localArrows[arrowIndex].Update(p.transform.position, teamNeutraUseDifferentArrowColor ? targetsRole.Color : Palette.ImpostorRed);
                        }
                    }
                    arrowIndex++;
                }
            }
        }
        // else
    }

    public void TextUpdate()
    {
        var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Player.Data);
        var numberOfTasks = playerTotal - playerCompleted;

        var local = PlayerControl.LocalPlayer;

        var isDead = local == Player || local.Data.IsDead;
        var forImpTeam = local.IsImpostor();
        var forKillerTeam = Team == includeNeutralTeam.KillNeutral && local.IsKillerNeutral();
        var forEvilTeam = Team == includeNeutralTeam.EvilNeutral && local.IsEvilNeutral();
        var forNeutraTeam = Team == includeNeutralTeam.AllNeutral && local.IsNeutral();

        if (numberOfTasks <= taskCountForReveal && (forImpTeam || forKillerTeam || forEvilTeam || forNeutraTeam || isDead))
        {
            if (text == null && !Player.IsDead())
            {
                text = UObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                text.enableWordWrapping = false;
                text.transform.localScale = Vector3.one * 0.75f;
                text.transform.localPosition += new Vector3(0f, 1.8f, -69f);
                text.gameObject.SetActive(true);
            }
            else if (!Player.IsDead())
            {
                text.text = $"告密者还活着: {playerCompleted} / {playerTotal}";
            }
            else
            {
                text?.Destroy();
                text = null;
            }
        }
        else if (text != null)
        {
            text.Destroy();
            text = null;
        }
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        if (Player != null && Player.IsDead() && text != null) UObject.Destroy(text);
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        ArrowUpdate();
        TextUpdate();
        var local = PlayerControl.LocalPlayer;
        var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Player.Data);
        int numberOfTasks = playerTotal - playerCompleted;

        bool forImp = local.IsImpostor();
        bool forKillerTeam = Team == includeNeutralTeam.KillNeutral && local.IsKillerNeutral();
        bool forEvilTeam = Team == includeNeutralTeam.EvilNeutral && local.IsEvilNeutral();
        bool forNeutraTeam = Team == includeNeutralTeam.AllNeutral && local.IsNeutral();

        if (numberOfTasks <= taskCountForReveal && Player.IsAlive())
        {
            if (forImp || forKillerTeam || forEvilTeam || forNeutraTeam)
            {
                setPlayerNameColor(Player, color);
            }
        }

        if (numberOfTasks == 0 && seeInMeeting && Player.IsAlive())
        {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                bool TargetsImp = p.Data.Role.IsImpostor;
                bool TargetsKillerTeam = Team == includeNeutralTeam.KillNeutral && p.IsKillerNeutral();
                bool TargetsEvilTeam = Team == includeNeutralTeam.EvilNeutral && p.IsEvilNeutral();
                bool TargetsNeutraTeam = Team == includeNeutralTeam.AllNeutral && p.IsNeutral();
                var targetsRole = p.GetRoleInfo();
                if (local == Player && (TargetsImp || TargetsKillerTeam || TargetsEvilTeam || TargetsNeutraTeam))
                {
                    if (teamNeutraUseDifferentArrowColor)
                    {
                        setPlayerNameColor(p, targetsRole.Color);
                    }
                    else
                    {
                        setPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
            }
        }

    }
}
