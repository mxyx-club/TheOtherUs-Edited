using AmongUs.GameOptions;

namespace TheOtherRoles.Options;

internal class ModOption
{
    public static float ButtonCooldown => CustomOptionHolder.resteButtonCooldown.GetFloat();
    public static bool PreventTaskEnd => CustomOptionHolder.disableTaskGameEnd.GetBool();
    public static float KillCooldown => GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
    public static int NumImpostors => GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors;
    public static bool DebugMode => CustomOptionHolder.debugMode.GetBool();
    public static bool DisableGameEnd => DebugMode && CustomOptionHolder.disableGameEnd.GetBool();
    public static NormalGameOptionsV07 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;

    public static bool showFPS => Main.ShowFPS.Value;
    public static bool toggleCursor => Main.ToggleCursor.Value;
    public static bool enableSoundEffects => Main.EnableSoundEffects.Value;
    public static bool showKeyReminder => Main.ShowKeyReminder.Value;

    // Set values
    public static int maxNumberOfMeetings = 10;
    public static bool blockSkippingInEmergencyMeetings;
    public static bool noVoteIsSelfVote;
    public static bool hidePlayerNames;
    public static bool allowParallelMedBayScans;
    public static bool showLighterDarker = true;
    public static bool shieldFirstKill;
    public static bool hideVentAnim;
    public static bool impostorSeeRoles;
    public static bool transparentTasks;
    public static bool hideOutOfSightNametags;
    public static bool randomLigherPlayer;
    public static bool disableMedscanWalking;
    public static bool DisableMeeting;
    public static bool isCanceled;

    public static int restrictDevices;

    // public static float restrictAdminTime = 600f;
    //public static float restrictAdminTimeMax = 600f;
    public static float restrictCamerasTime = 600f;
    public static float restrictCamerasTimeMax = 600f;
    public static float restrictVitalsTime = 600f;
    public static float restrictVitalsTimeMax = 600f;
    public static bool disableCamsRoundOne;
    public static bool isRoundOne = true;
    public static bool camoComms;
    public static bool disableSabotage;
    //public static bool ShowVentsOnMap;
    //public static bool ShowVentsOnMeetingMap;
    public static bool fungleDisableCamoComms;
    public static bool randomGameStartPosition;
    public static CustomGameModes GameMode = CustomGameModes.Classic;
    public static int ImpostorChatChannel;
    public static bool EnableOtherLog;

    // Updating values
    public static int meetingsCount;
    public static List<SurvCamera> camerasToAdd = new();
    public static List<Vent> ventsToSeal = new();
    public static Dictionary<byte, PoolablePlayer> playerIcons = new();
    public static string firstKillName;
    public static PlayerControl firstKillPlayer;

    /*public static bool canUseAdmin => restrictDevices == 0 || restrictAdminTime > 0f ||
                                      PlayerControl.LocalPlayer == Hacker.hacker ||
                                      PlayerControl.LocalPlayer.Data.IsDead;

    public static bool couldUseAdmin => restrictDevices == 0 || restrictAdminTimeMax > 0f ||
                                        PlayerControl.LocalPlayer == Hacker.hacker ||
                                        PlayerControl.LocalPlayer.Data.IsDead;*/

    public static bool canUseCameras => restrictDevices == 0 || restrictCamerasTime > 0f ||
                                        PlayerControl.LocalPlayer == Hacker.hacker ||
                                        PlayerControl.LocalPlayer.Data.IsDead;

    public static bool couldUseCameras => restrictDevices == 0 || restrictCamerasTimeMax > 0f ||
                                          PlayerControl.LocalPlayer == Hacker.hacker ||
                                          PlayerControl.LocalPlayer.Data.IsDead;

    public static bool canUseVitals => restrictDevices == 0 || restrictVitalsTime > 0f ||
                                       PlayerControl.LocalPlayer == Hacker.hacker ||
                                       PlayerControl.LocalPlayer.Data.IsDead;

    public static bool couldUseVitals => restrictDevices == 0 || restrictVitalsTimeMax > 0f ||
                                         PlayerControl.LocalPlayer == Hacker.hacker ||
                                         PlayerControl.LocalPlayer.Data.IsDead;

    public static void clearAndReloadModOptions()
    {
        meetingsCount = 0;
        camerasToAdd = new();
        ventsToSeal = new();
        playerIcons = new Dictionary<byte, PoolablePlayer>();

        NormalOptions.ConfirmImpostor = CustomOptionHolder.exiledController.GetBool() && CustomOptionHolder.exiledShowTeamNum.GetBool();
        maxNumberOfMeetings = CustomOptionHolder.maxNumberOfMeetings.GetInt();
        blockSkippingInEmergencyMeetings = CustomOptionHolder.blockSkippingInEmergencyMeetings.GetBool();
        blockSkippingInEmergencyMeetings = CustomOptionHolder.blockSkippingInEmergencyMeetings.GetBool();
        noVoteIsSelfVote = CustomOptionHolder.noVoteIsSelfVote.GetBool();
        hidePlayerNames = CustomOptionHolder.hidePlayerNames.GetBool();
        hideOutOfSightNametags = CustomOptionHolder.hideOutOfSightNametags.GetBool();
        hideVentAnim = CustomOptionHolder.hideVentAnimOnShadows.GetBool();
        allowParallelMedBayScans = CustomOptionHolder.allowParallelMedBayScans.GetBool();
        disableMedscanWalking = CustomOptionHolder.disableMedbayWalk.GetBool();
        camoComms = CustomOptionHolder.enableCamoComms.GetBool();
        fungleDisableCamoComms = false;
        shieldFirstKill = CustomOptionHolder.shieldFirstKill.GetBool();
        impostorSeeRoles = CustomOptionHolder.impostorSeeRoles.GetBool();
        transparentTasks = CustomOptionHolder.transparentTasks.GetBool();
        restrictDevices = CustomOptionHolder.restrictDevices.GetSelection();
        DisableMeeting = CustomOptionHolder.disableMeeting.GetBool();
        //restrictAdminTime = restrictAdminTimeMax = CustomOptionHolder.restrictAdmin.getFloat();
        restrictCamerasTime = restrictCamerasTimeMax = CustomOptionHolder.restrictCameras.GetFloat();
        restrictVitalsTime = restrictVitalsTimeMax = CustomOptionHolder.restrictVents.GetFloat();
        disableCamsRoundOne = CustomOptionHolder.disableCamsRound1.GetBool();
        randomGameStartPosition = CustomOptionHolder.randomGameStartPosition.GetBool();
        randomLigherPlayer = CustomOptionHolder.randomLigherPlayer.GetBool();
        disableSabotage = CustomOptionHolder.disableSabotage.GetBool();
        EnableOtherLog = CustomOptionHolder.enableOtherLog.GetBool();
        ImpostorChatChannel = CustomOptionHolder.impostorChatChannel.GetSelection();
        //ShowVentsOnMap = CustomOptionHolder.showVentsOnMap.GetBool();
        //ShowVentsOnMeetingMap = CustomOptionHolder.showVentsOnMap.GetQuantity() == 1;
        firstKillPlayer = null;
        isRoundOne = true;
        isCanceled = false;
    }

    public static void resetDeviceTimes()
    {
        //restrictAdminTime = restrictAdminTimeMax;
        restrictCamerasTime = restrictCamerasTimeMax;
        restrictVitalsTime = restrictVitalsTimeMax;
    }
}