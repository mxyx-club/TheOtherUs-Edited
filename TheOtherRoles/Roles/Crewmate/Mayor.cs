using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Mayor
{
    public static PlayerControl mayor;
    public static Color color = new Color32(32, 77, 66, byte.MaxValue);
    public static ResourceSprite emergencySprite = new("EmergencyButton.png", 550f);
    public static PlayerVoteArea Reveal;
    public static bool StartReveal;
    public static bool Revealed;

    public static bool meetingButton = true;
    public static int remoteMeetingsLeft = 1;
    public static bool SabotageRemoteMeetings = true;
    public static int vision = 5;

    public static void clearAndReload()
    {
        mayor = null;
        StartReveal = false;
        Revealed = false;
        meetingButton = CustomOptionHolder.mayorMeetingButton.getBool();
        remoteMeetingsLeft = Mathf.RoundToInt(CustomOptionHolder.mayorMaxRemoteMeetings.getFloat());
        SabotageRemoteMeetings = CustomOptionHolder.mayorSabotageRemoteMeetings.getBool();
        vision = CustomOptionHolder.mayorRevealVision.getSelection() + 2;
    }
}