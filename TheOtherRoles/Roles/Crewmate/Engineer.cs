namespace TheOtherRoles.Roles.Crewmate;

public class Engineer
{
    public static PlayerControl engineer;
    public static Color color = new Color32(0, 40, 245, byte.MaxValue);
    public static ResourceSprite buttonSprite = new("RepairButton.png");

    public static bool resetFixAfterMeeting;

    //public static bool expertRepairs = false;
    public static bool remoteFix = true;
    public static int remainingFixes = 1;
    public static bool highlightForImpostors = true;
    public static bool highlightForTeamJackal = true;

    public static void resetFixes()
    {
        remainingFixes = CustomOptionHolder.engineerNumberOfFixes.GetInt();
    }

    public static void clearAndReload()
    {
        engineer = null;
        remoteFix = CustomOptionHolder.engineerRemoteFix.GetBool();
        //expertRepairs = CustomOptionHolder.engineerExpertRepairs.getBool();
        resetFixAfterMeeting = CustomOptionHolder.engineerResetFixAfterMeeting.GetBool();
        remainingFixes = CustomOptionHolder.engineerNumberOfFixes.GetInt();
        highlightForImpostors = CustomOptionHolder.engineerHighlightForImpostors.GetBool();
        highlightForTeamJackal = CustomOptionHolder.engineerHighlightForTeamJackal.GetBool();
    }
}

