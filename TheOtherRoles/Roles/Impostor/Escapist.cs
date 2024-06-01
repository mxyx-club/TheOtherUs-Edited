using UnityEngine;

namespace TheOtherRoles.Roles.Impostor;

public static class Escapist
{
    public static PlayerControl escapist;
    public static Color color = Palette.ImpostorRed;

    public static float EscapeTime = 30f;
    public static float ChargesOnPlace = 1f;

    public static bool resetPlaceAfterMeeting;

    public static float ChargesGainOnMeeting = 2f;
    public static float MaxCharges = 3f;
    public static float Charges = 1f;

    public static Vector3 escapeLocation;

    private static Sprite escapeMarkButtonSprite;
    private static Sprite escapeButtonSprite;
    public static bool usedPlace;

    public static Sprite getEscapeMarkButtonSprite()
    {
        if (escapeMarkButtonSprite) return escapeMarkButtonSprite;
        escapeMarkButtonSprite = loadSpriteFromResources("TheOtherRoles.Resources.Mark.png", 115f);
        return escapeMarkButtonSprite;
    }

    public static Sprite getEscapeButtonSprite()
    {
        if (escapeButtonSprite) return escapeButtonSprite;
        escapeButtonSprite = loadSpriteFromResources("TheOtherRoles.Resources.Recall.png", 115f);
        return escapeButtonSprite;
    }

    public static void resetPlaces()
    {
        Charges = Mathf.RoundToInt(ChargesOnPlace);
        escapeLocation = Vector3.zero;
        usedPlace = false;
    }

    public static void clearAndReload()
    {
        resetPlaces();
        escapeLocation = Vector3.zero;
        escapist = null;
        resetPlaceAfterMeeting = CustomOptionHolder.escapistResetPlaceAfterMeeting.getBool();
        Charges = 1f;
        EscapeTime = CustomOptionHolder.escapistEscapeTime.getFloat();
        ChargesGainOnMeeting = CustomOptionHolder.escapistChargesGainOnMeeting.getFloat();
        MaxCharges = CustomOptionHolder.escapistMaxCharges.getFloat();
        usedPlace = false;
    }
}
