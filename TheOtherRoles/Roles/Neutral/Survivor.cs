using System.Collections.Generic;
using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;
public class Survivor
{
    public static List<PlayerControl> survivor = new();
    public static Color color = new Color32(255, 230, 77, byte.MaxValue);
    public static ResourceSprite VestButtonSprite = new("TheOtherRoles.Resources.Vest.png");

    public static bool vestEnable;
    public static float vestNumber;
    public static float vestCooldown;
    public static float vestDuration;
    public static float vestResetCooldown;
    public static bool blanksEnable;
    public static float blanksNumber;
    public static float blanksCooldown;

    public static bool vestActive;
    public static void clearAndReload()
    {
        survivor = [];
        vestActive = false;
        vestEnable = CustomOptionHolder.survivorVestEnable.getBool();
        vestNumber = CustomOptionHolder.survivorVestNumber.getFloat();
        vestCooldown = CustomOptionHolder.survivorVestCooldown.getFloat();
        vestDuration = CustomOptionHolder.survivorVestDuration.getFloat();
        vestResetCooldown = CustomOptionHolder.survivorVestResetCooldown.getFloat();
        blanksEnable = CustomOptionHolder.survivorBlanksEnable.getBool();
        blanksNumber = CustomOptionHolder.survivorBlanksNumber.getFloat();
        blanksCooldown = CustomOptionHolder.survivorBlanksCooldown.getFloat();
    }
}
