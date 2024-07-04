using System.Collections.Generic;
using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Pursuer
{
    public static List<PlayerControl> pursuer = new();
    public static PlayerControl target;
    public static Color color = new Color32(145, 164, 30, byte.MaxValue);
    public static List<PlayerControl> blankedList = [];
    public static int blanks;

    public static float cooldown = 30f;
    public static int blanksNumber = 5;

    public static ResourceSprite buttonSprite = new("PursuerButton.png");


    public static void clearAndReload()
    {
        pursuer = [];
        target = null;
        blankedList = [];
        blanks = 0;

        cooldown = CustomOptionHolder.pursuerBlanksCooldown.getFloat();
        blanksNumber = Mathf.RoundToInt(CustomOptionHolder.pursuerBlanksNumber.getFloat());
    }
}
