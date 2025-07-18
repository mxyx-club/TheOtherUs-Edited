namespace TheOtherRoles.Roles.Crewmate;

public static class Trapper
{
    public static PlayerControl trapper;
    public static Color color = new Color32(110, 57, 105, byte.MaxValue);

    public static float cooldown = 30f;
    public static int maxCharges = 5;
    public static int rechargeTasksNumber = 3;
    public static int rechargedTasks = 3;
    public static int charges = 1;
    public static int trapCountToReveal = 2;
    public static List<PlayerControl> playersOnMap = new();
    public static int infoType; // 0 = Role, 1 = Good/Evil, 2 = Name
    public static float trapDuration = 5f;

    public static Sprite trapButtonSprite = new ResourceSprite("Trapper_Place_Button.png");

    public static void clearAndReload()
    {
        trapper = null;
        cooldown = CustomOptionHolder.trapperCooldown.GetFloat();
        maxCharges = CustomOptionHolder.trapperMaxCharges.GetInt();
        rechargeTasksNumber = CustomOptionHolder.trapperRechargeTasksNumber.GetInt();
        rechargedTasks = CustomOptionHolder.trapperRechargeTasksNumber.GetInt();
        charges = CustomOptionHolder.trapperMaxCharges.GetInt() / 2;
        trapCountToReveal = CustomOptionHolder.trapperTrapNeededTriggerToReveal.GetInt();
        playersOnMap = new List<PlayerControl>();
        infoType = CustomOptionHolder.trapperInfoType.GetSelection();
        trapDuration = CustomOptionHolder.trapperTrapDuration.GetFloat();
    }
}
