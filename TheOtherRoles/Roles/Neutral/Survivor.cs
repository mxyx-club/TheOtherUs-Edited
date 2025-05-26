namespace TheOtherRoles.Roles.Neutral;
public class Survivor
{
    public static List<PlayerControl> Player = new();
    public static PlayerControl target;
    public static List<PlayerControl> blankedList = new();
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
        Player.Clear();
        target = null;
        blankedList.Clear();

        vestActive = false;
        blanksUsed = 0;
        vestUsed = 0;
        vestEnable = CustomOptionHolder.survivorVestEnable.GetBool();
        vestNumber = CustomOptionHolder.survivorVestNumber.GetInt();
        vestCooldown = CustomOptionHolder.survivorVestCooldown.GetFloat();
        vestDuration = CustomOptionHolder.survivorVestDuration.GetFloat();
        vestResetCooldown = CustomOptionHolder.survivorVestResetCooldown.GetFloat();
        blanksEnable = CustomOptionHolder.survivorBlanksEnable.GetBool();
        blanksNumber = CustomOptionHolder.survivorBlanksNumber.GetInt();
        blanksCooldown = CustomOptionHolder.survivorBlanksCooldown.GetFloat();
    }
}
