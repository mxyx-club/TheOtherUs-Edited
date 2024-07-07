using System.Collections.Generic;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Roles.Neutral;

public class Amnisiac
{
    public static PlayerControl amnisiac;
    public static List<Arrow> localArrows = new();
    public static Color color = new(0.5f, 0.7f, 1f, 1f);
    public static List<PoolablePlayer> poolIcons = new();

    public static bool showArrows = true;
    public static bool resetRole;

    public static ResourceSprite buttonSprite = new("Remember.png");

    public static void clearAndReload()
    {
        amnisiac = null;
        showArrows = CustomOptionHolder.amnisiacShowArrows.getBool();
        resetRole = CustomOptionHolder.amnisiacResetRole.getBool();
        if (localArrows != null)
            foreach (var arrow in localArrows)
                if (arrow?.arrow != null)
                    Object.Destroy(arrow.arrow);
        localArrows = new List<Arrow>();
    }
}
