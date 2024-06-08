using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Magician
{
    public static PlayerControl magician;
    public static Color color = new Color32(82, 108, 173, byte.MaxValue);

    public static float placeHatCooldown = 10f;
    public static float TeleportTime = 10f;
    public static float probabilityBlueCards;
    public static float probabilityRedCards;
    public static float probabilityPurpleCards;
    public static bool resetPlaceAfterMeeting;

    public static ResourceSprite Cards = new("Cards.png");
    public static ResourceSprite BlueCard = new("BlueCard.png");
    public static ResourceSprite PurpleCard = new("PurpleCard.png");
    public static ResourceSprite RedCard = new("RedCard.png");

    public static void clearAndReload()
    {
        magician = null;
        /*
        TeleportTime = CustomOptionHolder.jumperJumpTime.getFloat();
        probabilityBlueCards = CustomOptionHolder.magicianProbabilityBlueCards.getFloat();
        probabilityRedCards = CustomOptionHolder.magicianProbabilityRedCards.getFloat();
        probabilityPurpleCards = CustomOptionHolder.magicianProbabilityPurpleCards.getFloat();
        */
    }
}
