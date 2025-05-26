namespace TheOtherRoles.Roles.Crewmate;

public static class Mayor
{
    public static PlayerControl mayor;
    public static Color color = new Color32(32, 77, 66, byte.MaxValue);
    public static ResourceSprite emergencySprite = new("EmergencyButton.png", 550f);
    public static bool Revealed;
    public static int Vote;

    public static bool meetingButton = true;
    public static int remoteMeetingsLeft = 1;
    public static bool SabotageRemoteMeetings = true;
    public static int vision = 5;

    public static void clearAndReload()
    {
        mayor = null;
        Revealed = false;
        Vote = CustomOptionHolder.mayorVote.GetInt();
        meetingButton = CustomOptionHolder.mayorMeetingButton.GetBool();
        remoteMeetingsLeft = CustomOptionHolder.mayorMaxRemoteMeetings.GetInt();
        SabotageRemoteMeetings = CustomOptionHolder.mayorSabotageRemoteMeetings.GetBool();
        vision = CustomOptionHolder.mayorRevealVision.GetSelection() + 2;
    }
}