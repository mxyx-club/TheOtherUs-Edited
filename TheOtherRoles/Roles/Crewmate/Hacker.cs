namespace TheOtherRoles.Roles.Crewmate;

public class Hacker : RoleBase
{
    public static Color color = new Color32(117, 250, 76, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Hacker),
        (p) => new Hacker(p),
        RoleId.Hacker,
        RoleType.Crewmate,
        "Hacker",
        color,
        302500,
        AddOptions
    );

    public Hacker(PlayerControl p) : base(p, roleinfo) { }

    public static Minigame vitals;
    public static Minigame doorLog;

    public static float cooldown = 30f;
    public static float duration = 10f;
    public static float toolsNumber = 5f;
    public static bool onlyColorType;
    public float hackerTimer;
    public static int rechargeTasksNumber = 2;
    public static int rechargedTasks = 2;
    public static int chargesVitals = 1;
    public static int chargesAdminTable = 1;
    public static bool cantMove = true;

    private static Sprite buttonSprite = new ResourceSprite("HackerButton.png");
    private static Sprite vitalsSprite;
    private static Sprite logSprite;
    private static Sprite adminSprite;
    public CustomButton hackerButton;
    public CustomButton hackerVitalsButton;
    public CustomButton hackerAdminTableButton;


    public static CustomOption hackerCooldown;
    public static CustomOption hackerHackeringDuration;
    public static CustomOption hackerOnlyColorType;
    public static CustomOption hackerToolsNumber;
    public static CustomOption hackerRechargeTasksNumber;
    public static CustomOption hackerNoMove;

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        // hackerSpawnRate = CustomOption.Create(configId++, Types.Crewmate, cs(Hacker.color, "Hacker"), rates, null, true);
        hackerCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "hackerCooldown", 15f, 5f, 60f, 2.5f, roleinfo.RoleOption);
        hackerHackeringDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "hackerHackeringDuration", 10f, 2.5f, 60f, 2.5f, roleinfo.RoleOption);
        hackerOnlyColorType = CustomOption.Create(configId++, CustomOptionType.Crewmate, "hackerOnlyColorType", false, roleinfo.RoleOption);
        hackerToolsNumber = CustomOption.Create(configId++, CustomOptionType.Crewmate, "hackerToolsNumber", 5f, 1f, 30f, 1f, roleinfo.RoleOption);
        hackerRechargeTasksNumber = CustomOption.Create(configId++, CustomOptionType.Crewmate, "hackerRechargeTasksNumber", 2f, 1f, 5f, 1f, roleinfo.RoleOption);
        hackerNoMove = CustomOption.Create(configId++, CustomOptionType.Crewmate, "hackerNoMove", true, roleinfo.RoleOption);
    }

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

    public override void Initialize()
    {
        vitals = null;
        doorLog = null;
        hackerTimer = 0f;
        adminSprite = null;
        cooldown = hackerCooldown.GetFloat();
        duration = hackerHackeringDuration.GetFloat();
        onlyColorType = hackerOnlyColorType.GetBool();
        toolsNumber = hackerToolsNumber.GetFloat();
        rechargeTasksNumber = hackerRechargeTasksNumber.GetInt();
        rechargedTasks = hackerRechargeTasksNumber.GetInt();
        chargesVitals = hackerToolsNumber.GetInt() / 2;
        chargesAdminTable = hackerToolsNumber.GetInt() / 2;
        cantMove = hackerNoMove.GetBool();
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        hackerTimer -= Time.deltaTime;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (Player.IsDead()) return;
        var (playerCompleted, _) = TasksHandler.taskInfo(Player.Data);
        if (playerCompleted == rechargedTasks)
        {
            rechargedTasks += rechargeTasksNumber;
            if (toolsNumber > chargesVitals) chargesVitals++;
            if (toolsNumber > chargesAdminTable) chargesAdminTable++;
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        hackerButton?.Destroy();
        hackerVitalsButton?.Destroy();
        hackerAdminTableButton?.Destroy();
        hackerButton = null;
        hackerVitalsButton = null;
        hackerAdminTableButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Hacker button
        hackerButton?.Destroy();
        hackerButton = new CustomButton(
            () =>
            {
                hackerTimer = duration;
                SoundEffectsManager.play("hackerHack");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () => { return true; },
            () =>
            {
                hackerButton.Timer = hackerButton.MaxTimer;
                hackerButton.isEffectActive = false;
                hackerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.modKillInput.keyCode,
            true,
            0f,
            () => { hackerButton.Timer = hackerButton.MaxTimer; },
            buttonText: GetString("hackerButtonText")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = duration
        };

        hackerAdminTableButton?.Destroy();
        hackerAdminTableButton = new CustomButton(
            () =>
            {
                if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                {
                    var __instance = FastDestroyableSingleton<HudManager>.Instance;
                    __instance.InitMap();
                    MapBehaviour.Instance.ShowCountOverlay(true, true, true);
                }

                if (cantMove) PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                chargesAdminTable--;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                if (hackerAdminTableButton.ButtonTitle != null)
                    hackerAdminTableButton.ButtonTitle.text = $"{chargesAdminTable} / {toolsNumber}";
                return chargesAdminTable > 0;
            },
            () =>
            {
                hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
                hackerAdminTableButton.isEffectActive = false;
                hackerAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            getAdminSprite(),
            __instance,
            __instance.AbilityButton,
            ModInputManager.secondaryAbilityInput.keyCode,
            true,
            0f,
            () => true,
            () =>
            {
                if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                {
                    var __instance = FastDestroyableSingleton<HudManager>.Instance;
                    __instance.InitMap();
                    MapBehaviour.Instance.ShowCountOverlay(true, true, true);
                }
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
            },
            () =>
            {
                hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
                if (!hackerVitalsButton.isEffectActive) PlayerControl.LocalPlayer.moveable = true;
                if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
            },
            GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3,
            GetString("AdminMapText")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = duration
        };

        hackerVitalsButton?.Destroy();
        hackerVitalsButton = new CustomButton(
            () =>
            {
                if (GameOptionsManager.Instance.currentNormalGameOptions.MapId != 1)
                {
                    if (vitals == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("panel_vitals") || x.gameObject.name.Contains("Vitals"));
                        if (e == null || Camera.main == null) return;
                        vitals = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    vitals.transform.SetParent(Camera.main.transform, false);
                    vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    vitals.Begin(null);
                }
                else
                {
                    if (doorLog == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>()
                            .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        doorLog = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    doorLog.transform.SetParent(Camera.main.transform, false);
                    doorLog.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    doorLog.Begin(null);
                }

                if (cantMove) PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 

                chargesVitals--;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() &&
                       GameOptionsManager.Instance.currentGameOptions.MapId != 0 &&
                       GameOptionsManager.Instance.currentNormalGameOptions.MapId != 3;
            },
            () =>
            {
                if (hackerVitalsButton.ButtonTitle != null)
                    hackerVitalsButton.ButtonTitle.text = $"{chargesVitals} / {toolsNumber}";
                hackerVitalsButton.actionButton.graphic.sprite =
                    isMira ? getLogSprite() : getVitalsSprite();
                hackerVitalsButton.actionButton.OverrideText(isMira ? GetString("hackerDoorLogText") : GetString("hackerVitalText"));
                return chargesVitals > 0;
            },
            () =>
            {
                hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                hackerVitalsButton.isEffectActive = false;
                hackerVitalsButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            getVitalsSprite(),
            __instance,
            __instance.AbilityButton,
           ModInputManager.abilityInput.keyCode,
            true,
            0f,
            () => true,
            () =>
            {
                if (GameOptionsManager.Instance.currentNormalGameOptions.MapId != 1)
                {
                    if (vitals == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("panel_vitals") || x.gameObject.name.Contains("Vitals"));
                        if (e == null || Camera.main == null) return;
                        vitals = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    vitals.transform.SetParent(Camera.main.transform, false);
                    vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    vitals.Begin(null);
                }
                else
                {
                    if (doorLog == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>()
                            .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        doorLog = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    doorLog.transform.SetParent(Camera.main.transform, false);
                    doorLog.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    doorLog.Begin(null);
                }

            },
            () =>
            {
                hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                if (!hackerAdminTableButton.isEffectActive) PlayerControl.LocalPlayer.moveable = true;
                if (Minigame.Instance)
                {
                    if (isMira) doorLog.ForceClose();
                    else vitals.ForceClose();
                }
            },
            false,
            isMira ? GetString("hackerDoorLogText") : GetString("hackerVitalText")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = duration
        };

        // hackerVitalsChargesText -> hackerVitalsButton
        // hackerAdminTableChargesText -> hackerAdminTableButton
    }
}
