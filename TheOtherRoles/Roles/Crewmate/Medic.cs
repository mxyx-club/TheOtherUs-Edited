using TheOtherRoles.Utilities;
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

    private static Sprite buttonSprite;

    public static void resetShielded()
    {
        currentTarget = shielded = null;
        usedShield = false;
    }

    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = loadSpriteFromResources("TheOtherRoles.Resources.ShieldButton.png", 115f);
        return buttonSprite;
    }
    public static bool shieldVisible(PlayerControl target)
    {
        bool hasVisibleShield = false;

        bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
        if (shielded != null && ((target == shielded && !isMorphedMorphling) || (isMorphedMorphling && Morphling.morphTarget == shielded)))
        {   // Everyone or Ghost info
            hasVisibleShield = showShielded == 0 || shouldShowGhostInfo()
                || (showShielded == 1 && (CachedPlayer.LocalPlayer.PlayerControl == shielded
                || CachedPlayer.LocalPlayer.PlayerControl == medic)) // Shielded + Medic
                || (showShielded == 2 && CachedPlayer.LocalPlayer.PlayerControl == medic); // Medic only
                                                                                           // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
            hasVisibleShield = hasVisibleShield && (meetingAfterShielding
                || !showShieldAfterMeeting
                || CachedPlayer.LocalPlayer.PlayerControl == medic
                || shouldShowGhostInfo());
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
        reset = CustomOptionHolder.medicResetTargetAfterMeeting.getBool();
        showShielded = CustomOptionHolder.medicShowShielded.getSelection();
        showAttemptToShielded = CustomOptionHolder.medicShowAttemptToShielded.getBool();
        unbreakableShield = CustomOptionHolder.medicBreakShield.getBool();
        showAttemptToMedic = CustomOptionHolder.medicShowAttemptToMedic.getBool();
        setShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 2;
        showShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.getSelection() == 1;
        ReportNameDuration = CustomOptionHolder.medicReportNameDuration.getFloat();
        ReportColorDuration = CustomOptionHolder.medicReportColorDuration.getFloat();
        meetingAfterShielding = false;
    }
}
