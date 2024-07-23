using System.Collections.Generic;
using TheOtherRoles.Objects;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Roles.Crewmate;

public static class Snitch
{
    public static PlayerControl snitch;
    public static Color color = new Color32(184, 251, 79, byte.MaxValue);

    public static List<Arrow> localArrows = new List<Arrow>();
    public static int taskCountForReveal = 1;
    public static bool seeInMeeting;
    //public static bool canSeeRoles;
    public static bool teamNeutraUseDifferentArrowColor = true;
    public static bool needsUpdate = true;

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
                    Object.Destroy(arrow.arrow);
        }
        localArrows = new List<Arrow>();
        taskCountForReveal = Mathf.RoundToInt(CustomOptionHolder.snitchLeftTasksForReveal.getFloat());
        seeInMeeting = CustomOptionHolder.snitchSeeMeeting.getBool();
        if (text != null) Object.Destroy(text);
        text = null;
        needsUpdate = true;

        //canSeeRoles = CustomOptionHolder.snitchCanSeeRoles.getBool();
        Team = (includeNeutralTeam)CustomOptionHolder.snitchIncludeNeutralTeam.getSelection();
        teamNeutraUseDifferentArrowColor = CustomOptionHolder.snitchTeamNeutraUseDifferentArrowColor.getBool();
        snitch = null;
    }
}
