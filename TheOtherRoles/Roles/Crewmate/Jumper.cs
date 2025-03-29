using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Jumper
{
    public static PlayerControl jumper;
    public static Color color = new Color32(204, 155, 20, byte.MaxValue); // mint

    public static float JumpTime = 30f;

    public static bool resetPlaceAfterMeeting;

    public static int ChargesGainOnMeeting = 2;
    public static float MaxCharges = 3f;
    public static int Charges = 1;

    public static Vector3 jumpLocation;

    public static ResourceSprite jumpMarkButtonSprite = new("JumperMarkButton.png");
    public static ResourceSprite jumpJumpButtonSprite = new("JumperJumpButton.png");
    public static bool usedPlace;

    public static void resetPlaces()
    {
        Charges = 1;
        jumpLocation = Vector3.zero;
        usedPlace = false;
    }

    public static void clearAndReload()
    {
        resetPlaces();
        jumpLocation = Vector3.zero;
        jumper = null;
        resetPlaceAfterMeeting = CustomOptionHolder.jumperResetPlaceAfterMeeting.GetBool();
        Charges = CustomOptionHolder.jumperMaxCharges.GetInt();
        JumpTime = CustomOptionHolder.jumperJumpTime.GetFloat();
        ChargesGainOnMeeting = CustomOptionHolder.jumperChargesGainOnMeeting.GetInt();
        MaxCharges = CustomOptionHolder.jumperMaxCharges.GetFloat();
        usedPlace = false;
    }
}
