namespace TheOtherRoles.Roles.Impostor;

public static class Bomber
{
    public static PlayerControl bomber;
    public static Color color = Palette.ImpostorRed;

    public static float cooldown = 30f;
    public static float bombDelay = 10f;
    public static float bombTimer = 10f;
    public static bool triggerBothCooldowns;
    public static bool canGiveToBomber;
    public static bool hotPotatoMode;

    public static bool bombActive;

    public static bool hasAlerted;
    public static float timeLeft;
    public static PlayerControl currentTarget;
    public static PlayerControl currentBombTarget;
    public static PlayerControl hasBombPlayer;


    public static Sprite buttonSprite = new ResourceSprite("Bomber2.png");

    public static void clearAndReload()
    {
        bomber = null;
        currentTarget = null;
        currentBombTarget = null;
        hasBombPlayer = null;
        bombActive = false;
        cooldown = CustomOptionHolder.bomberBombCooldown.GetFloat();
        bombDelay = CustomOptionHolder.bomberDelay.GetFloat();
        bombTimer = CustomOptionHolder.bomberTimer.GetFloat();
        triggerBothCooldowns = CustomOptionHolder.bomberTriggerBothCooldowns.GetBool();
        canGiveToBomber = CustomOptionHolder.bomberCanGiveToBomber.GetBool();
        hotPotatoMode = CustomOptionHolder.bomberHotPotatoMode.GetBool();
    }
}
