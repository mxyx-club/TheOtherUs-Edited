namespace TheOtherRoles.Roles.Impostor;

public static class Escapist
{
    public static PlayerControl escapist;
    public static Color color = Palette.ImpostorRed;

    public static float EscapeTime = 30f;
    public static float ChargesOnPlace = 1f;

    public static bool resetPlaceAfterMeeting;

    public static Vector3 escapeLocation = Vector3.zero;

    public static Sprite escapeEscapeButtonSprite = new ResourceSprite("Mark.png");
    public static Sprite escapeButtonSprite = new ResourceSprite("Recall.png");

    public static void resetPlaces()
    {
        escapeLocation = Vector3.zero;
    }

    public static void clearAndReload()
    {
        resetPlaces();
        escapeLocation = Vector3.zero;
        escapist = null;
        resetPlaceAfterMeeting = CustomOptionHolder.escapistResetPlaceAfterMeeting.GetBool();
        EscapeTime = CustomOptionHolder.escapistEscapeTime.GetFloat();
    }
}
