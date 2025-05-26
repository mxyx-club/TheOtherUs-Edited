using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

public static class Tracker
{
    public static PlayerControl tracker;
    public static Color color = new Color32(100, 58, 220, byte.MaxValue);
    public static List<Arrow> localArrows = new();

    public static float updateIntervall = 5f;
    public static bool resetTargetAfterMeeting;
    public static bool canTrackCorpses;
    public static float corpsesTrackingCooldown = 30f;
    public static float corpsesTrackingDuration = 5f;
    public static float corpsesTrackingTimer;
    public static int trackingMode;
    public static List<Vector3> deadBodyPositions = new();

    public static PlayerControl currentTarget;
    public static PlayerControl tracked;
    public static bool usedTracker;
    public static float timeUntilUpdate;
    public static Arrow arrow = new(Color.blue);

    public static GameObject DangerMeterParent;
    public static DangerMeter Meter;

    public static ResourceSprite trackCorpsesButtonSprite = new("PathfindButton.png");

    public static ResourceSprite buttonSprite = new("TrackerButton.png");

    public static void resetTracked()
    {
        currentTarget = tracked = null;
        usedTracker = false;
        if (arrow?.arrow != null) UObject.Destroy(arrow.arrow);
        arrow = new Arrow(Color.blue);
        arrow.arrow?.SetActive(false);
    }

    public static void clearAndReload()
    {
        tracker = null;
        resetTracked();
        timeUntilUpdate = 0f;
        updateIntervall = CustomOptionHolder.trackerUpdateIntervall.GetFloat();
        resetTargetAfterMeeting = CustomOptionHolder.trackerResetTargetAfterMeeting.GetBool();
        if (localArrows != null)
        {
            foreach (Arrow arrow in localArrows)
                if (arrow?.arrow != null)
                    UObject.Destroy(arrow.arrow);
        }
        deadBodyPositions.Clear();
        corpsesTrackingTimer = 0f;
        corpsesTrackingCooldown = CustomOptionHolder.trackerCorpsesTrackingCooldown.GetFloat();
        corpsesTrackingDuration = CustomOptionHolder.trackerCorpsesTrackingDuration.GetFloat();
        canTrackCorpses = CustomOptionHolder.trackerCanTrackCorpses.GetBool();
        trackingMode = CustomOptionHolder.trackerTrackingMethod.GetSelection();
        if (DangerMeterParent)
        {
            Meter.gameObject.Destroy();
            DangerMeterParent.Destroy();
        }
    }
}
