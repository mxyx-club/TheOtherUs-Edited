using System.Collections.Generic;
using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;
public class Survivor
{
    public static List<PlayerControl> survivor = [];
    public static PlayerControl target;
    public static List<PlayerControl> blankedList = [];
    public static Color color = new Color32(255, 230, 77, byte.MaxValue);
    public static ResourceSprite VestButtonSprite = new("TheOtherRoles.Resources.Vest.png");

    public static bool vestEnable;
    public static int vestNumber;
    public static float vestCooldown;
    public static float vestDuration;
    public static float vestResetCooldown;
    public static bool blanksEnable;
    public static int blanksNumber;
    public static float blanksCooldown;

    public static int blanksUsed;
    public static int vestUsed;
    public static bool vestActive;
    public static int remainingVests => vestNumber - vestUsed;
    public static int remainingBlanks => blanksNumber - blanksUsed;

    public static void clearAndReload()
    {
        survivor = [];
        target = null;
        blankedList = [];

        vestActive = false;
        blanksUsed = 0;
        vestUsed = 0;
        vestEnable = CustomOptionHolder.survivorVestEnable.getBool();
        vestNumber = CustomOptionHolder.survivorVestNumber.GetInt();
        vestCooldown = CustomOptionHolder.survivorVestCooldown.getFloat();
        vestDuration = CustomOptionHolder.survivorVestDuration.getFloat();
        vestResetCooldown = CustomOptionHolder.survivorVestResetCooldown.getFloat();
        blanksEnable = CustomOptionHolder.survivorBlanksEnable.getBool();
        blanksNumber = CustomOptionHolder.survivorBlanksNumber.GetInt();
        blanksCooldown = CustomOptionHolder.survivorBlanksCooldown.getFloat();
    }
}
