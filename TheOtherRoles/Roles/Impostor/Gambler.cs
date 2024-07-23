using UnityEngine;

namespace TheOtherRoles.Roles.Impostor;
public class Gambler
{
    public static PlayerControl gambler;
    public static Color color = Palette.ImpostorRed;

    public static float minCooldown;
    public static float maxCooldown;
    public static int successRate;

    public static bool GetSuc()
    {
        var rate = rnd.Next(0, 100);
        Message($"Gambler rate {rate} : {successRate}");
        return rate > successRate;
    }

    public static void clearAndReload()
    {
        gambler = null;
        minCooldown = CustomOptionHolder.gamblerMinCooldown.getFloat();
        maxCooldown = CustomOptionHolder.gamblerMaxCooldown.getFloat();
        successRate = CustomOptionHolder.gamblerSuccessRate.getSelection() * 10;
    }

}
