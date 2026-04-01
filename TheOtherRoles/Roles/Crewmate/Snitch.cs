using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

public static class Snitch
{
    public static PlayerControl snitch;
    public static Color color = new Color32(184, 251, 79, byte.MaxValue);

    public static List<Arrow> localArrows = new();
    public static int revealTaskReduction = 1;
    public static int taskCountForReveal = 1;
    public static bool seeInMeeting;
    public static bool canSeeRoles;
    public static bool teamNeutralUseDifferentArrowColor = true;
    public static bool needsUpdate = true;
    public static bool CanGuessIfTasksDone;

    public static includeNeutralTeam Team = includeNeutralTeam.KillNeutral;
    public static TextMeshPro text;

    public enum includeNeutralTeam
    {
        NoIncNeutral = 0,
        KillNeutral = 1,
        EvilNeutral = 2,
        AllNeutral = 3
    }

    public static Tuple<int, int> GetRevealTask()
    {
        if (snitch?.Data == null) return Tuple.Create(0, 0);

        var (completed, total) = TasksHandler.taskInfo(snitch.Data);
        if (total <= 0) return Tuple.Create(0, 0);

        int need = total - revealTaskReduction;
        int minNeed = taskCountForReveal + 1;
        need = Math.Clamp(Math.Max(need, minNeed), 1, total);
        int remain = Math.Max(0, need - completed);

        return Tuple.Create(remain, need);
    }

    public static bool IsEnemy(PlayerControl player)
    {
        if (player == null) return false;

        return player.IsImpostor() ||
               (Team == includeNeutralTeam.AllNeutral && player.IsNeutral()) ||
               (Team == includeNeutralTeam.KillNeutral && player.IsKillerNeutral()) ||
               (Team == includeNeutralTeam.EvilNeutral && (player.IsEvilNeutral() || player.IsKillerNeutral()))
               ;
    }

    public static bool CanRevealRole(PlayerControl local, PlayerControl target)
    {
        if (!canSeeRoles || snitch == null || !snitch.IsAlive()) return false;

        var (remainTasks, _) = GetRevealTask();
        bool canReveal = remainTasks == 0;

        return canReveal && local == snitch && IsEnemy(target);
    }

    public static void clearAndReload()
    {
        if (localArrows != null)
        {
            foreach (Arrow arrow in localArrows)
                if (arrow?.arrow != null)
                    UObject.Destroy(arrow.arrow);
        }
        localArrows.Clear();

        if (text != null) UObject.Destroy(text);
        text = null;
        needsUpdate = true;

        snitch = null;

        revealTaskReduction = CustomOptionHolder.snitchRevealTaskReduction.GetInt();
        taskCountForReveal = CustomOptionHolder.snitchLeftTasksForReveal.GetInt();
        seeInMeeting = CustomOptionHolder.snitchSeeMeeting.GetBool();
        canSeeRoles = CustomOptionHolder.snitchCanSeeRoles.GetBool();
        Team = CustomOptionHolder.snitchIncludeNeutralTeam.GetSelection<includeNeutralTeam>();
        teamNeutralUseDifferentArrowColor = CustomOptionHolder.snitchTeamNeutralUseDifferentArrowColor.GetBool();
        CanGuessIfTasksDone = CustomOptionHolder.snitchCanGuessIfTasksDone.GetBool();
    }
}
