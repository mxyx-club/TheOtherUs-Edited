namespace TheOtherRoles.Roles.Crewmate;

public class Hacker
{
    public static PlayerControl hacker;
    public static Minigame vitals;
    public static Minigame doorLog;
    public static Color color = new Color32(117, 250, 76, byte.MaxValue);

    public static float cooldown = 30f;
    public static float duration = 10f;
    public static float toolsNumber = 5f;
    public static bool onlyColorType;
    public static float hackerTimer;
    public static int rechargeTasksNumber = 2;
    public static int rechargedTasks = 2;
    public static int chargesVitals = 1;
    public static int chargesAdminTable = 1;
    public static bool cantMove = true;

    public static Sprite buttonSprite = new ResourceSprite("HackerButton.png");
    private static Sprite vitalsSprite;
    private static Sprite logSprite;
    private static Sprite adminSprite;

    public static Sprite getVitalsSprite()
    {
        if (vitalsSprite) return vitalsSprite;
        vitalsSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
        return vitalsSprite;
    }

    public static Sprite getLogSprite()
    {
        if (logSprite) return logSprite;
        logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
        return logSprite;
    }

    public static Sprite getAdminSprite()
    {
        var mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        // Polus
        var button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton];
        if (isSkeld || mapId == 3)
            // Skeld || Dleks
            button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
        else if (isMira)
            // Mira HQ
            button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton];
        else if (isAirship)
            // Airship
            button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton];
        else if (isFungle)
            // Hacker can Access the Admin panel on Fungle
            button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
        adminSprite = button.Image;
        return adminSprite;
    }

    public static void clearAndReload()
    {
        hacker = null;
        vitals = null;
        doorLog = null;
        hackerTimer = 0f;
        adminSprite = null;
        cooldown = CustomOptionHolder.hackerCooldown.GetFloat();
        duration = CustomOptionHolder.hackerHackeringDuration.GetFloat();
        onlyColorType = CustomOptionHolder.hackerOnlyColorType.GetBool();
        toolsNumber = CustomOptionHolder.hackerToolsNumber.GetFloat();
        rechargeTasksNumber = CustomOptionHolder.hackerRechargeTasksNumber.GetInt();
        rechargedTasks = CustomOptionHolder.hackerRechargeTasksNumber.GetInt();
        chargesVitals = CustomOptionHolder.hackerToolsNumber.GetInt() / 2;
        chargesAdminTable = CustomOptionHolder.hackerToolsNumber.GetInt() / 2;
        cantMove = CustomOptionHolder.hackerNoMove.GetBool();
    }
}
