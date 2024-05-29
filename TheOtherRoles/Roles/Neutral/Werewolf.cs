using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Werewolf
{
    public static PlayerControl werewolf;
    public static PlayerControl currentTarget;
    public static Color color = new Color32(79, 56, 21, byte.MaxValue);

    // Kill Button 
    public static float killCooldown = 3f;

    // Rampage Button
    public static float rampageCooldown = 30f;
    public static float rampageDuration = 5f;
    public static bool canUseVents;
    public static bool canKill;
    public static bool hasImpostorVision;

    public static Sprite buttonSprite;

    public static Sprite getRampageButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Rampage.png", 115f);
        return buttonSprite;
    }

    public static Vector3 getRampageVector()
    {
        return new Vector3(-2.7f, -0.06f, 0);
    }

    public static void clearAndReload()
    {
        werewolf = null;
        currentTarget = null;
        canUseVents = false;
        canKill = false;
        hasImpostorVision = false;
        rampageCooldown = CustomOptionHolder.werewolfRampageCooldown.getFloat();
        rampageDuration = CustomOptionHolder.werewolfRampageDuration.getFloat();
        killCooldown = CustomOptionHolder.werewolfKillCooldown.getFloat();
    }
}
