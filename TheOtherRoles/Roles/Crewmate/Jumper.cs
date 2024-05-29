using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Jumper
{
    public static PlayerControl jumper;
    public static Color color = new Color32(204, 155, 20, byte.MaxValue); // mint

    public static float JumpTime = 30f;
    public static float ChargesOnPlace = 1f;

    public static bool resetPlaceAfterMeeting;

    public static float ChargesGainOnMeeting = 2f;
    public static float MaxCharges = 3f;
    public static float Charges = 1f;

    public static Vector3 jumpLocation;

    private static Sprite jumpMarkButtonSprite;
    private static Sprite jumpButtonSprite;
    public static bool usedPlace;

    public static Sprite getJumpMarkButtonSprite()
    {
        if (jumpMarkButtonSprite) return jumpMarkButtonSprite;
        jumpMarkButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.JumperButton.png", 115f);
        return jumpMarkButtonSprite;
    }

    public static Sprite getJumpButtonSprite()
    {
        if (jumpButtonSprite) return jumpButtonSprite;
        jumpButtonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.JumperJumpButton.png", 115f);
        return jumpButtonSprite;
    }

    public static void resetPlaces()
    {
        Charges = Mathf.RoundToInt(ChargesOnPlace);
        jumpLocation = Vector3.zero;
        usedPlace = false;
    }

    public static void clearAndReload()
    {
        resetPlaces();
        jumpLocation = Vector3.zero;
        jumper = null;
        resetPlaceAfterMeeting = CustomOptionHolder.jumperResetPlaceAfterMeeting.getBool();
        Charges = CustomOptionHolder.jumperMaxCharges.getFloat();
        JumpTime = CustomOptionHolder.jumperJumpTime.getFloat();
        ChargesOnPlace = 1f;
        ChargesGainOnMeeting = CustomOptionHolder.jumperChargesGainOnMeeting.getFloat();
        MaxCharges = CustomOptionHolder.jumperMaxCharges.getFloat();
        usedPlace = false;
    }
}
