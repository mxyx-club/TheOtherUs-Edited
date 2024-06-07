using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Roles.Impostor;

public static class Mafia
{
    public static PlayerControl godfather;
    public static PlayerControl mafioso;
    public static PlayerControl janitor;
    public static Color color = Palette.ImpostorRed;

    public static float cooldown = 30f;

    public static ResourceSprite buttonSprite = new("CleanButton.png");

    public static void clearAndReload()
    {
        godfather = null;
        mafioso = null;
        janitor = null;
        cooldown = CustomOptionHolder.janitorCooldown.getFloat();
    }
}
