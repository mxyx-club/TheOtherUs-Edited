using TheOtherRoles.Modules;
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

    public static ResourceSprite escapeMarkButtonSprite = new("Mark.png");
    public static ResourceSprite escapeButtonSprite = new("Recall.png");
    public static bool usedPlace;

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
