using System.Collections.Generic;
using AmongUs.GameOptions;
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Options;

internal class ModOption
{
    public static float ButtonCooldown => CustomOptionHolder.resteButtonCooldown.GetFloat();
    public static bool PreventTaskEnd => CustomOptionHolder.disableTaskGameEnd.GetBool();
    public static float KillCooddown => GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
    public static int NumImpostors => GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors;
    public static bool DebugMode => CustomOptionHolder.debugMode.GetBool();
    public static bool DisableGameEnd => DebugMode && CustomOptionHolder.disableGameEnd.GetBool();
    public static NormalGameOptionsV08 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;

    // Set values
    public static int maxNumberOfMeetings = 10;
    public static bool blockSkippingInEmergencyMeetings;
    public static bool noVoteIsSelfVote;
    public static bool hidePlayerNames;
    public static bool allowParallelMedBayScans;
    public static bool showLighterDarker = true;
    public static bool showFPS = true;
    public static bool localHats;
    public static bool toggleCursor = true;
    public static bool enableSoundEffects = true;
    public static bool showKeyReminder;
    public static bool shieldFirstKill;
    public static bool hideVentAnim;
    public static bool impostorSeeRoles;
    public static bool transparentTasks;
    public static bool hideOutOfSightNametags;
    public static bool randomLigherPlayer;
    public static bool disableMedscanWalking;
    public static bool isCanceled;
    public static bool ShowChatNotifications = true;
    public static bool MuteLobbyBGM;

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
    public static bool fungleDisableCamoComms;
    public static bool randomGameStartPosition;
    public static bool allowModGuess;
    public static CustomGamemodes gameMode = CustomGamemodes.Classic;

    // Updating values
    public static int meetingsCount;
    public static List<SurvCamera> camerasToAdd = new();
    public static List<Vent> ventsToSeal = new();
    public static Dictionary<byte, PoolablePlayer> playerIcons = new();
    public static string firstKillName;
    public static PlayerControl firstKillPlayer;

    // public static bool canUseAdmin  { get { return restrictDevices == 0 || restrictAdminTime > 0f || CachedPlayer.LocalPlayer.PlayerControl == Hacker.hacker || CachedPlayer.LocalPlayer.Data.IsDead; }}

    //public static bool couldUseAdmin { get { return restrictDevices == 0 || restrictAdminTimeMax > 0f  || CachedPlayer.LocalPlayer.PlayerControl == Hacker.hacker || CachedPlayer.LocalPlayer.Data.IsDead; }}

    public static bool canUseCameras => restrictDevices == 0 || restrictCamerasTime > 0f ||
                                        CachedPlayer.LocalPlayer.PlayerControl == Hacker.hacker ||
                                        CachedPlayer.LocalPlayer.Data.IsDead;

    public static bool couldUseCameras => restrictDevices == 0 || restrictCamerasTimeMax > 0f ||
                                          CachedPlayer.LocalPlayer.PlayerControl == Hacker.hacker ||
                                          CachedPlayer.LocalPlayer.Data.IsDead;

    public static bool canUseVitals => restrictDevices == 0 || restrictVitalsTime > 0f ||
                                       CachedPlayer.LocalPlayer.PlayerControl == Hacker.hacker ||
                                       CachedPlayer.LocalPlayer.Data.IsDead;

    public static bool couldUseVitals => restrictDevices == 0 || restrictVitalsTimeMax > 0f ||
                                         CachedPlayer.LocalPlayer.PlayerControl == Hacker.hacker ||
                                         CachedPlayer.LocalPlayer.Data.IsDead;

    public static void clearAndReloadMapOptions()
    {
        meetingsCount = 0;
        camerasToAdd = new();
        ventsToSeal = new();
        playerIcons = new Dictionary<byte, PoolablePlayer>();

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
        //restrictAdminTime = restrictAdminTimeMax = CustomOptionHolder.restrictAdmin.getFloat();
        restrictCamerasTime = restrictCamerasTimeMax = CustomOptionHolder.restrictCameras.GetFloat();
        restrictVitalsTime = restrictVitalsTimeMax = CustomOptionHolder.restrictVents.GetFloat();
        disableCamsRoundOne = CustomOptionHolder.disableCamsRound1.GetBool();
        randomGameStartPosition = CustomOptionHolder.randomGameStartPosition.GetBool();
        randomLigherPlayer = CustomOptionHolder.randomLigherPlayer.GetBool();
        allowModGuess = CustomOptionHolder.allowModGuess.GetBool();
        disableSabotage = CustomOptionHolder.disableSabotage.GetBool();
        firstKillPlayer = null;
        isRoundOne = true;
        isCanceled = false;
    }

    public static void reloadPluginOptions()
    {
        showFPS = Main.ShowFPS.Value;
        toggleCursor = Main.ToggleCursor.Value;
        enableSoundEffects = Main.EnableSoundEffects.Value;
        showKeyReminder = Main.ShowKeyReminder.Value;
        localHats = Main.LocalHats.Value;
        ShowChatNotifications = Main.ShowChatNotifications.Value;
        MuteLobbyBGM = Main.MuteLobbyBGM.Value;
    }

    public static void resetDeviceTimes()
    {
        //restrictAdminTime = restrictAdminTimeMax;
        restrictCamerasTime = restrictCamerasTimeMax;
        restrictVitalsTime = restrictVitalsTimeMax;
    }
}