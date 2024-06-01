using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Mayor
{
    public static PlayerControl mayor;
    public static Color color = new Color32(32, 77, 66, byte.MaxValue);
    public static Minigame emergency;
    public static Sprite emergencySprite;
    public static int remoteMeetingsLeft = 1;

    public static bool canSeeVoteColors;
    public static int tasksNeededToSeeVoteColors;
    public static bool meetingButton = true;
    public static int mayorChooseSingleVote;

    public static bool voteTwice = true;
    public static bool SabotageRemoteMeetings = true;

    public static Sprite getMeetingSprite()
    {
        if (emergencySprite) return emergencySprite;
        emergencySprite = loadSpriteFromResources("TheOtherRoles.Resources.EmergencyButton.png", 550f);
        return emergencySprite;
    }

    public static void clearAndReload()
    {
        mayor = null;
        emergency = null;
        emergencySprite = null;
        remoteMeetingsLeft = Mathf.RoundToInt(CustomOptionHolder.mayorMaxRemoteMeetings.getFloat());
        canSeeVoteColors = CustomOptionHolder.mayorCanSeeVoteColors.getBool();
        tasksNeededToSeeVoteColors = (int)CustomOptionHolder.mayorTasksNeededToSeeVoteColors.getFloat();
        meetingButton = CustomOptionHolder.mayorMeetingButton.getBool();
        mayorChooseSingleVote = CustomOptionHolder.mayorChooseSingleVote.getSelection();
        voteTwice = true;

        SabotageRemoteMeetings = false;
        //SabotageRemoteMeetings = CustomOptionHolder.mayorSabotageRemoteMeetings.getBool();
    }
}