namespace TheOtherRoles.Roles.Neutral;

public class PartTimer
{
    public static PlayerControl partTimer;
    public static Color color = new Color32(0, 255, 0, byte.MaxValue);
    public static PlayerControl currentTarget;
    public static PlayerControl target;

    public static float cooldown;
    public static int DeathDefaultTurn;
    public static int deathTurn;
    public static bool knowsRole;

    public static Sprite buttonSprite = new ResourceSprite("PartTimerButton.png");

    public static void clearAndReload()
    {
        partTimer = null;
        currentTarget = null;
        target = null;
        cooldown = CustomOptionHolder.partTimerCooldown.GetFloat();
        deathTurn = DeathDefaultTurn = CustomOptionHolder.partTimerDeathTurn.GetInt();
        knowsRole = CustomOptionHolder.partTimerKnowsRole.GetBool();
    }
}
