using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class Specoality
{
    public static PlayerControl specoality;
    public static PlayerControl canNoGuess;
    public static Color color = Palette.ImpostorRed;
    public static int linearfunction = 1;

    public static void clearAndReload()
    {
        specoality = null;
        canNoGuess = null;
        linearfunction = 1;
    }
}
