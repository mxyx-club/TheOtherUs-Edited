using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

public static class Snitch
{
    public static PlayerControl snitch;
    public static Color color = new Color32(184, 251, 79, byte.MaxValue);

    public static List<Arrow> localArrows = new();
    public static int taskCountForReveal = 1;
    public static bool seeInMeeting;
    //public static bool canSeeRoles;
    public static bool teamNeutraUseDifferentArrowColor = true;
    public static bool needsUpdate = true;
    public static bool CanGuessIfTaksDone;

    public enum includeNeutralTeam
    {
        NoIncNeutral = 0,
        KillNeutral = 1,
        EvilNeutral = 2,
        AllNeutral = 3
    }

    public static includeNeutralTeam Team = includeNeutralTeam.KillNeutral;
    public static TextMeshPro text;

    public static void clearAndReload()
    {
        if (localArrows != null)
        {
            foreach (Arrow arrow in localArrows)
                if (arrow?.arrow != null)
                    UObject.Destroy(arrow.arrow);
        }
        localArrows.Clear();
        taskCountForReveal = CustomOptionHolder.snitchLeftTasksForReveal.GetInt();
        seeInMeeting = CustomOptionHolder.snitchSeeMeeting.GetBool();
        if (text != null) UObject.Destroy(text);
        text = null;
        needsUpdate = true;

        //canSeeRoles = CustomOptionHolder.snitchCanSeeRoles.GetBool();
        Team = CustomOptionHolder.snitchIncludeNeutralTeam.GetSelection<includeNeutralTeam>();
        teamNeutraUseDifferentArrowColor = CustomOptionHolder.snitchTeamNeutraUseDifferentArrowColor.GetBool();
        CanGuessIfTaksDone = CustomOptionHolder.snitchCanGuessIfTaksDone.GetBool();
        snitch = null;
    }
}
