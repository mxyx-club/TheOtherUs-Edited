using TheOtherRoles.Mode;
using static TheOtherRoles.Options.CustomOption;
using Types = TheOtherRoles.Options.CustomOptionType;

namespace TheOtherRoles.Options;

public class CustomOptionHolder
{
    public static string[] rates = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];

    public static string[] ratesCount =
        ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15"];
    public static string[] ratesRandom =
        ["Random", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15"];

    public static string[] presets =
        ["预设 1", "预设 2", "预设 3", "Skeld预设", "Mira预设", "Polus预设", "Airship预设", "Fungle预设", "Submerged预设"];

    public static CustomOption presetSelection;
    public static CustomOption anyPlayerCanStopStart;
    public static CustomOption isDraftMode;
    public static CustomOption draftModeAmountOfChoices;
    public static CustomOption draftModeTimeToChoose;
    public static CustomOption draftModeShowRoles;
    public static CustomOption draftModeHideImpRoles;
    public static CustomOption draftModeHideNeutralRoles;
    public static CustomOption draftModeHideCrewmateRoles;
    public static CustomOption neutralRolesCountMin;
    public static CustomOption neutralRolesCountMax;
    public static CustomOption killerNeutralRolesCountMin;
    public static CustomOption killerNeutralRolesCountMax;
    public static CustomOption modifiersCountMin;
    public static CustomOption modifiersCountMax;

    public static CustomOption NetworkTransformLowLevel;
    public static CustomOption NetworkTransformType;

    public static CustomOption resteButtonCooldown;
    public static CustomOption shieldFirstKill;
    public static CustomOption hidePlayerNames;
    public static CustomOption hideOutOfSightNametags;
    public static CustomOption hideVentAnimOnShadows;
    public static CustomOption showButtonTarget;
    public static CustomOption impostorSeeRoles;
    public static CustomOption blockGameEnd;
    public static CustomOption randomLigherPlayer;
    public static CustomOption randomGameStartPosition;
    public static CustomOption randomGameStartToVents;
    public static CustomOption ghostSpeed;

    public static CustomOption gmEnabled;
    public static CustomOption gmIsHost;
    public static CustomOption gmDiesAtStart;

    public static CustomOption MeetingOptions;
    public static CustomOption disableMeeting;
    public static CustomOption maxNumberOfMeetings;
    public static CustomOption blockSkippingInEmergencyMeetings;
    public static CustomOption noVoteIsSelfVote;
    public static CustomOption guessReVote;
    public static CustomOption guessExtendmeetingTime;
    public static CustomOption exiledController;
    public static CustomOption exiledReviveRole;
    public static CustomOption exiledShowTeamNum;
    public static CustomOption playerDieReducedTime;
    public static CustomOption minMeetingTime;

    public static CustomOption TaskOptions;
    public static CustomOption WireTaskIsRandomOption;
    public static CustomOption WireTaskNumOption;
    public static CustomOption transparentTasks;
    public static CustomOption disableMedbayWalk;
    public static CustomOption allowParallelMedBayScans;
    public static CustomOption finishTasksBeforeHauntingOrZoomingOut;
    public static CustomOption disableTaskGameEnd;

    public static CustomOption SaboOptions;
    public static CustomOption disableSabotage;
    public static CustomOption deadImpsBlockSabotage;
    public static CustomOption enableCamoComms;
    public static CustomOption IsReactorDurationSetting;
    public static CustomOption SkeldReactorTimeLimit;
    public static CustomOption SkeldLifeSuppTimeLimit;
    public static CustomOption MiraLifeSuppTimeLimit;
    public static CustomOption MiraReactorTimeLimit;
    public static CustomOption PolusReactorTimeLimit;
    public static CustomOption AirshipReactorTimeLimit;
    public static CustomOption FungleReactorTimeLimit;

    public static CustomOption MapOptions;
    public static CustomOption enableSkeldModify;
    public static CustomOption skeldVitals;
    public static CustomOption enableMiraModify;
    public static CustomOption miraVitals;
    public static CustomOption enableBetterPolus;
    public static CustomOption movePolusVents;
    public static CustomOption addPolusVents;
    public static CustomOption movePolusVitals;
    public static CustomOption swapNavWifi;
    public static CustomOption moveColdTemp;
    public static CustomOption enableAirShipModify;
    public static CustomOption airshipOptimize;
    public static CustomOption addAirShipVents;
    public static CustomOption airshipLadder;
    public static CustomOption enableFungleModify;
    public static CustomOption fungleElectrical;
    public static CustomOption dynamicMap;
    public static CustomOption dynamicMapEnableSkeld;
    public static CustomOption dynamicMapEnableMira;
    public static CustomOption dynamicMapEnablePolus;
    public static CustomOption dynamicMapEnableAirShip;
    public static CustomOption dynamicMapEnableFungle;
    public static CustomOption dynamicMapEnableSubmerged;
    public static CustomOption dynamicMapSeparateSettings;

    public static CustomOption DevicesOption;
    public static CustomOption restrictDevices;
    public static CustomOption restrictCameras;
    public static CustomOption restrictVents;
    public static CustomOption disableCamsRound1;
    public static CustomOption camsNightVision;
    public static CustomOption camsNoNightVisionIfImpVision;

    public static CustomOption debugMode;
    public static CustomOption logRpcSend;
    public static CustomOption disableGameEnd;

    // Neutral
    public static CustomOption specterSpawnRate;
    public static CustomOption specterResetRole;
    public static CustomOption specterDuration;
    public static CustomOption specterAfterMeetingTakeRole;
    public static CustomOption specterAfterMeetingRevived;

    // Crewmate
    public static CustomOption ghostEngineerSpawnRate;

    public static CustomOption poltergeistSpawnRate;
    public static CustomOption poltergeistCooldown;
    public static CustomOption poltergeistRadius;

    // Ability
    public static CustomOption modifiersAreHidden;

    public static void Load()
    {
        vanillaSettings = Main.Instance.Config.Bind("Preset0", "VanillaOptions", "");

        //-------------------------- Role options 0 - 99 -------------------------- //
        presetSelection = Create(0, Types.General, Cs(new Color32(204, 204, 0, 255), "presetSelection"), presets, null, true);
        anyPlayerCanStopStart = Create(3, Types.General, Cs(new Color32(204, 204, 0, 255), "anyPlayerCanStopStart"), false);

        isDraftMode = Create(30, Types.General, Cs(Color.yellow, "isDraftMode"), false, null, true, onChange: () =>
        {
            neutralRolesCountMax.isHeader = isDraftMode.GetBool();
        });
        draftModeAmountOfChoices = Create(31, Types.General, Cs(Color.yellow, "draftModeAmountOfChoices"), 3f, 2f, 6f, 1f, isDraftMode, false);
        draftModeTimeToChoose = Create(32, Types.General, Cs(Color.yellow, "draftModeTimeToChoose"), 5f, 3f, 20f, 1f, isDraftMode, false);
        draftModeShowRoles = Create(33, Types.General, Cs(Color.yellow, "draftModeShowRoles"), false, isDraftMode, false);
        draftModeHideImpRoles = Create(34, Types.General, Cs(Color.yellow, "draftModeHideImpRoles"), false, draftModeShowRoles, false);
        draftModeHideNeutralRoles = Create(35, Types.General, Cs(Color.yellow, "draftModeHideNeutralRoles"), false, draftModeShowRoles, false);
        draftModeHideCrewmateRoles = Create(36, Types.General, Cs(Color.yellow, "draftModeHideCrewmateRoles"), false, draftModeShowRoles, false);

        neutralRolesCountMin = Create(8, Types.General, Cs(new Color32(204, 204, 0, 255), "neutralRolesCountMin"), 2f, 0f, 15f, 1f, null, true, isHidden: () => isDraftMode.GetBool());
        neutralRolesCountMax = Create(9, Types.General, Cs(new Color32(204, 204, 0, 255), "neutralRolesCountMax"), 2f, 0f, 15f, 1f, isHeader: isDraftMode.GetBool());
        killerNeutralRolesCountMin = Create(10, Types.General, Cs(new Color32(204, 204, 0, 255), "killerNeutralRolesCountMin"), ratesRandom, isHidden: () => isDraftMode.GetBool());
        killerNeutralRolesCountMax = Create(11, Types.General, Cs(new Color32(204, 204, 0, 255), "killerNeutralRolesCountMax"), ratesRandom);
        modifiersCountMin = Create(12, Types.General, Cs(new Color32(204, 204, 0, 255), "modifiersCountMin"), 15f, 0f, 30f, 1f);
        modifiersCountMax = Create(13, Types.General, Cs(new Color32(204, 204, 0, 255), "modifiersCountMax"), 15f, 0f, 30f, 1f);

        NetworkTransformLowLevel = Create(15, Types.General, "NetworkTransformLowLevel", ["optionOff", "NetworkTransformLowLevel.On"], null, true);
        NetworkTransformType = Create(16, Types.General, "NetworkTransformType", [
            "NetworkTransformType.Bad",
            "NetworkTransformType.SuperLow",
            "NetworkTransformType.Low",
            "NetworkTransformType.Medium",
            "NetworkTransformType.High",
            "NetworkTransformType.Max"], NetworkTransformLowLevel);


        //-------------------------- Other options 100 - 999 -------------------------- //

        //Global options
        resteButtonCooldown = Create(100, Types.General, "resteButtonCooldown", 20f, 2.5f, 30f, 2.5f, null, true);
        shieldFirstKill = Create(101, Types.General, "shieldFirstKill", false);
        hidePlayerNames = Create(102, Types.General, "hidePlayerNames", false);
        hideOutOfSightNametags = Create(103, Types.General, "hideOutOfSightNametags", true);
        hideVentAnimOnShadows = Create(104, Types.General, "hideVentAnimOnShadows", false);
        showButtonTarget = Create(105, Types.General, "showButtonTarget", true);
        impostorSeeRoles = Create(106, Types.General, Cs(Palette.ImpostorRed, "impostorSeeRoles"), false);
        blockGameEnd = Create(107, Types.General, Cs(Color.yellow, "blockGameEnd"), true);
        randomLigherPlayer = Create(108, Types.General, "randomLigherPlayer", true);
        randomGameStartPosition = Create(110, Types.General, "randomGameStartPosition", false);
        randomGameStartToVents = Create(111, Types.General, "randomGameStartToVents", true, randomGameStartPosition);
        ghostSpeed = Create(112, Types.General, "ghostSpeed", 1f, 0.75f, 5f, 0.125f);

        gmEnabled = Create(150, Types.General, Cs(GM.color, "gmEnabled"), false, null, true);
        gmIsHost = Create(151, Types.General, "gmIsHost", true, gmEnabled);
        gmDiesAtStart = Create(152, Types.General, "gmDiesAtStart", true, gmEnabled);

        //Meeting options
        MeetingOptions = Create(200, Types.General, Cs(new Color32(255, 85, 234, byte.MaxValue), "MeetingOptions"), false, null, true);
        disableMeeting = Create(201, Types.General, "disableMeeting", false, MeetingOptions);
        maxNumberOfMeetings = Create(202, Types.General, "maxNumberOfMeetings", 10, 0, 15, 1, MeetingOptions);
        blockSkippingInEmergencyMeetings = Create(203, Types.General, "blockSkippingInEmergencyMeetings", false, MeetingOptions);
        noVoteIsSelfVote = Create(204, Types.General, "noVoteIsSelfVote", false, blockSkippingInEmergencyMeetings);
        guessReVote = Create(205, Types.General, "guessReVote", false, MeetingOptions);
        guessExtendmeetingTime = Create(206, Types.General, "guessExtendmeetingTime", 15f, 0f, 60f, 5f, guessReVote);
        playerDieReducedTime = Create(210, Types.General, "playerDieReducedTime", 10f, 5f, 30f, 2.5f, MeetingOptions);
        minMeetingTime = Create(211, Types.General, "minMeetingTime", 60f, 30f, 150f, 5f, playerDieReducedTime);
        exiledController = Create(207, Types.General, "exileController", false, MeetingOptions);
        exiledReviveRole = Create(208, Types.General, "exiledReviveRole", ["optionOff", "Role", "Team"], exiledController);
        exiledShowTeamNum = Create(209, Types.General, "exiledShowTeamNum", false, exiledController);

        //Task options
        TaskOptions = Create(300, Types.General, Cs(Palette.CrewmateBlue, "TaskOptions"), false, null, true);
        WireTaskIsRandomOption = Create(301, Types.General, "WireTaskIsRandomOption", false, TaskOptions);
        WireTaskNumOption = Create(302, Types.General, "WireTaskNumOption", 3f, 1f, 8f, 1f, WireTaskIsRandomOption);
        transparentTasks = Create(303, Types.General, "transparentTasks", false, TaskOptions);
        disableMedbayWalk = Create(304, Types.General, "disableMedbayWalk", false, TaskOptions);
        allowParallelMedBayScans = Create(305, Types.General, "allowParallelMedBayScans", false, TaskOptions);
        finishTasksBeforeHauntingOrZoomingOut = Create(306, Types.General, "finishTasksBeforeHauntingOrZoomingOut", false, TaskOptions);
        disableTaskGameEnd = Create(307, Types.General, "disableTaskGameEnd", false, TaskOptions);

        //Sabotage options
        SaboOptions = Create(400, Types.General, Cs(Palette.ImpostorRed, "SaboOptions"), false, null, true);
        disableSabotage = Create(401, Types.General, Cs(Palette.ImpostorRed, "disableSabotage"), false, SaboOptions);
        deadImpsBlockSabotage = Create(402, Types.General, Cs(Palette.ImpostorRed, "deadImpsBlockSabotage"), false, SaboOptions);
        enableCamoComms = Create(403, Types.General, Cs(Palette.ImpostorRed, "enableCamoComms"), false, SaboOptions);
        IsReactorDurationSetting = Create(410, Types.General, "IsReactorDurationSetting", false, SaboOptions);
        SkeldReactorTimeLimit = Create(411, Types.General, "SkeldReactorTimeLimit", 30f, 0f, 30f, 2.5f, IsReactorDurationSetting);
        SkeldLifeSuppTimeLimit = Create(412, Types.General, "SkeldLifeSuppTimeLimit", 30f, 0f, 30f, 2.5f, IsReactorDurationSetting);
        MiraLifeSuppTimeLimit = Create(413, Types.General, "MiraLifeSuppTimeLimit", 30f, 0f, 45f, 2.5f, IsReactorDurationSetting);
        MiraReactorTimeLimit = Create(414, Types.General, "MiraReactorTimeLimit", 30f, 0f, 45f, 2.5f, IsReactorDurationSetting);
        PolusReactorTimeLimit = Create(415, Types.General, "PolusReactorTimeLimit", 60f, 0f, 60f, 2.5f, IsReactorDurationSetting);
        AirshipReactorTimeLimit = Create(416, Types.General, "AirshipReactorTimeLimit", 75f, 0f, 90f, 2.5f, IsReactorDurationSetting);
        FungleReactorTimeLimit = Create(417, Types.General, "FungleReactorTimeLimit", 45f, 0f, 60f, 2.5f, IsReactorDurationSetting);

        //Map options
        MapOptions = Create(500, Types.General, Cs(new Color32(223, 157, 192, byte.MaxValue), "MapOptions"), false, null, true);
        // The Skeld
        enableSkeldModify = Create(510, Types.General, Cs(Color.yellow, "The Skeld"), false, MapOptions);
        skeldVitals = Create(511, Types.General, "miraVitals", false, enableSkeldModify);
        //Mira
        enableMiraModify = Create(520, Types.General, Cs(Color.yellow, "Mira"), false, MapOptions);
        miraVitals = Create(521, Types.General, "miraVitals", false, enableMiraModify);
        //Polus
        enableBetterPolus = Create(530, Types.General, Cs(Color.yellow, "Polus"), false, MapOptions);
        movePolusVents = Create(531, Types.General, "movePolusVents", false, enableBetterPolus);
        addPolusVents = Create(532, Types.General, "addPolusVents", false, enableBetterPolus);
        movePolusVitals = Create(533, Types.General, "movePolusVitals", false, enableBetterPolus);
        swapNavWifi = Create(534, Types.General, "swapNavWifi", false, enableBetterPolus);
        moveColdTemp = Create(535, Types.General, "moveColdTemp", false, enableBetterPolus);
        //AirShip
        enableAirShipModify = Create(540, Types.General, Cs(Color.yellow, "AirShip"), false, MapOptions);
        airshipOptimize = Create(541, Types.General, "airshipOptimize", false, enableAirShipModify);
        addAirShipVents = Create(542, Types.General, "addAirShipVents", false, enableAirShipModify);
        airshipLadder = Create(543, Types.General, "airshipLadder", false, enableAirShipModify);
        //Fungle
        enableFungleModify = Create(550, Types.General, Cs(Color.yellow, "Fungle"), false, MapOptions);
        fungleElectrical = Create(551, Types.General, "fungleElectrical", false, enableFungleModify);
        //dynamicMap options
        dynamicMap = Create(580, Types.General, "dynamicMap", false, MapOptions, true);
        dynamicMapEnableSkeld = Create(581, Types.General, "Skeld", rates, dynamicMap);
        dynamicMapEnableMira = Create(582, Types.General, "Mira", rates, dynamicMap);
        dynamicMapEnablePolus = Create(583, Types.General, "Polus", rates, dynamicMap);
        dynamicMapEnableAirShip = Create(584, Types.General, "Airship", rates, dynamicMap);
        dynamicMapEnableFungle = Create(585, Types.General, "Fungle", rates, dynamicMap);
        dynamicMapEnableSubmerged = Create(586, Types.General, "Submerged", rates, dynamicMap);
        dynamicMapSeparateSettings = Create(587, Types.General, "dynamicMapSeparateSettings", false, dynamicMap);

        //Devices Option
        DevicesOption = Create(600, Types.General, Cs(new Color32(255, 50, 0, byte.MaxValue), "DevicesOption"), false, null, true);
        restrictDevices = Create(601, Types.General, "restrictDevices", ["optionOff", "restrictDevices2", "restrictDevices3"], DevicesOption);
        //restrictAdmin = Create(602, Types.General, "restrictAdmin", 30f, 0f, 600f, 5f, restrictDevices);
        restrictCameras = Create(603, Types.General, "restrictCameras", 30f, 0f, 600f, 5f, restrictDevices);
        restrictVents = Create(604, Types.General, "restrictVents", 30f, 0f, 600f, 5f, restrictDevices);
        disableCamsRound1 = Create(605, Types.General, "disableCamsRound1", false, DevicesOption);
        camsNightVision = Create(606, Types.General, "camsNightVision", false, DevicesOption);
        camsNoNightVisionIfImpVision = Create(607, Types.General, "camsNoNightVisionIfImpVision", false, camsNightVision);

        debugMode = Create(950, Types.General, "debugMode", false, null, true);
        logRpcSend = Create(951, Types.General, "logRpcSend", false, debugMode);
        disableGameEnd = Create(952, Types.General, "DisableGameEnd", false, debugMode);

        //-------------------------- Impostor Options 10000-19999 -------------------------- //

        var sortedRoleInfo = RoleInfo.AllRoleInfo.Where(role => !role.IsHidden).OrderBy(role => role.ConfigId);

        // Impostor
        sortedRoleInfo.Where(role => role.RoleType == RoleType.Impostor).Do(info =>
        {
            info.CreateOption();
        });

        //-------------------------- Neutral Options 20000-29999 -------------------------- //

        // Neutral
        specterSpawnRate = Create(50020, Types.Neutral, Cs(Specter.color, "SpecterOption"), rates, null, true);
        specterResetRole = Create(50021, Types.Neutral, "amnisiacResetRole", true, specterSpawnRate);
        specterDuration = Create(50022, Types.Neutral, "specterDuration", 1.5f, 0.25f, 5f, 0.25f, specterSpawnRate);
        specterAfterMeetingTakeRole = Create(50023, Types.Neutral, "specterAfterMeetingTakeRole", false, specterSpawnRate);
        specterAfterMeetingRevived = Create(50024, Types.Neutral, "specterAfterMeetingRevived", false, specterSpawnRate);

        sortedRoleInfo.Where(role => role.RoleType == RoleType.Neutral).Do(info =>
        {
            info.CreateOption();
        });


        //-------------------------- Crewmate Options 30000-39999 -------------------------- //

        ghostEngineerSpawnRate = Create(50010, Types.Crewmate, Cs(GhostEngineer.color, "GhostEngineerOptions"), rates, null, true);

        poltergeistSpawnRate = Create(50030, Types.Crewmate, Cs(Poltergeist.color, "PoltergeistOptions"), rates, null, true);
        poltergeistCooldown = Create(50031, Types.Crewmate, "poltergeistCooldown", 5f, 2.5f, 60f, 2.5f, poltergeistSpawnRate);
        poltergeistRadius = Create(50032, Types.Crewmate, "poltergeistRadius", 0.75f, 0.5f, 2f, 0.125f, poltergeistSpawnRate);

        sortedRoleInfo.Where(role => role.RoleType == RoleType.Crewmate).Do(info =>
        {
            info.CreateOption();
        });

        //-------------------------- Modifier (40000 - 49999) -------------------------- //

        modifiersAreHidden = Create(40000, Types.Modifiers, Cs(Color.yellow, "modifiersAreHidden"), false, null, true);

        sortedRoleInfo.Where(role => role.RoleType == RoleType.Modifier).Do(info =>
        {
            info.CreateOption();
        });

        //-------------------------- Guesser Gamemode 2000 - 2999 -------------------------- //
        GuesserGM.AddOptions();
    }
}
