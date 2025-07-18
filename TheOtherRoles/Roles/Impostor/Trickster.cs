using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

public static class Trickster
{
    public static PlayerControl trickster;
    public static Color color = Palette.ImpostorRed;
    public static float placeBoxCooldown = 30f;
    public static float lightsOutCooldown = 30f;
    public static float lightsOutDuration = 10f;
    public static float lightsOutTimer;

    public static Sprite placeBoxButtonSprite = new ResourceSprite("PlaceJackInTheBoxButton.png");
    public static Sprite lightOutButtonSprite = new ResourceSprite("LightsOutButton.png");
    public static Sprite tricksterVentButtonSprite = new ResourceSprite("TricksterVentButton.png");

    public static void clearAndReload()
    {
        trickster = null;
        lightsOutTimer = 0f;
        placeBoxCooldown = CustomOptionHolder.tricksterPlaceBoxCooldown.GetFloat();
        lightsOutCooldown = CustomOptionHolder.tricksterLightsOutCooldown.GetFloat();
        lightsOutDuration = CustomOptionHolder.tricksterLightsOutDuration.GetFloat();
        JackInTheBox.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
    }
}
