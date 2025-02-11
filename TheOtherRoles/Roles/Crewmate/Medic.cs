using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Medic
{
    public static PlayerControl medic;
    public static PlayerControl shielded;
    public static PlayerControl futureShielded;

    public static Color color = new Color32(126, 251, 194, byte.MaxValue);
    public static bool usedShield;

    public static int showShielded;
    public static bool showAttemptToShielded;
    public static bool showAttemptToMedic;
    public static bool unbreakableShield = true;
    public static bool setShieldAfterMeeting;
    public static bool showShieldAfterMeeting;
    public static bool meetingAfterShielding;
    public static bool reset;
    public static float ReportNameDuration;
    public static float ReportColorDuration;

    public static Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);
    public static PlayerControl currentTarget;

    public static ResourceSprite buttonSprite = new("ShieldButton.png");

    public static void resetShielded()
    {
        currentTarget = shielded = null;
        usedShield = false;
    }

    public static bool shieldVisible(PlayerControl target)
    {
        bool hasVisibleShield = false;

        bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
        if (shielded != null && ((target == shielded && !isMorphedMorphling) || (isMorphedMorphling && Morphling.morphTarget == shielded)))
        {
            // Everyone or Ghost info
            hasVisibleShield = showShielded == 0 || ShowGhostInfo
                || (showShielded == 1 && (PlayerControl.LocalPlayer == shielded
                || PlayerControl.LocalPlayer == medic)) // Shielded + Medic
                || (showShielded == 2 && PlayerControl.LocalPlayer == medic);
            // Medic only                                                                  
            // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
            hasVisibleShield = hasVisibleShield && (meetingAfterShielding
                || !showShieldAfterMeeting
                || PlayerControl.LocalPlayer == medic
                || ShowGhostInfo);
        }
        return hasVisibleShield;
    }
    public static void clearAndReload()
    {
        medic = null;
        shielded = null;
        futureShielded = null;
        currentTarget = null;
        usedShield = false;
        reset = CustomOptionHolder.medicResetTargetAfterMeeting.GetBool();
        showShielded = CustomOptionHolder.medicShowShielded.GetSelection();
        showAttemptToShielded = CustomOptionHolder.medicShowAttemptToShielded.GetBool();
        unbreakableShield = CustomOptionHolder.medicBreakShield.GetBool();
        showAttemptToMedic = CustomOptionHolder.medicShowAttemptToMedic.GetBool();
        setShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.GetSelection() == 2;
        showShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.GetSelection() == 1;
        ReportNameDuration = CustomOptionHolder.medicReportNameDuration.GetFloat();
        ReportColorDuration = CustomOptionHolder.medicReportColorDuration.GetFloat();
        meetingAfterShielding = false;
    }
}
