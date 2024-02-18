using System.Collections.Generic;
using TheOtherRoles.Modules;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using Types = TheOtherRoles.CustomOption.CustomOptionType;

namespace TheOtherRoles;

public class CustomOptionHolder
{
    public static string[] rates = { "0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };

    public static string[] ratesModifier =
        { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };

    public static string[] presets =
        { "预设 1", "预设 2", "随机地图预设Skeld", "随机地图预设Mira", "随机地图预设Polus", "随机地图预设Airship", "随机地图预设Submerged" };

    public static CustomOption presetSelection;
    public static CustomOption activateRoles;
    public static CustomOption enableCrowdedPlayer;
    public static CustomOption MaxPlayer;
    public static CustomOption crewmateRolesCountMin;
    public static CustomOption crewmateRolesCountMax;
    public static CustomOption crewmateRolesFill;
    public static CustomOption neutralRolesCountMin;
    public static CustomOption neutralRolesCountMax;
    public static CustomOption impostorRolesCountMin;
    public static CustomOption impostorRolesCountMax;
    public static CustomOption modifiersCountMin;
    public static CustomOption modifiersCountMax;

    public static CustomOption enableCodenameHorsemode;
    public static CustomOption enableCodenameDisableHorses;

    public static CustomOption cultistSpawnRate;

    public static CustomOption minerSpawnRate;
    public static CustomOption minerCooldown;
    public static CustomOption mafiaSpawnRate;
    public static CustomOption janitorCooldown;

    public static CustomOption morphlingSpawnRate;
    public static CustomOption morphlingCooldown;
    public static CustomOption morphlingDuration;

    public static CustomOption bomber2SpawnRate;
    public static CustomOption bomber2BombCooldown;
    public static CustomOption bomber2Delay;

    public static CustomOption bomber2Timer;
    //public static CustomOption bomber2HotPotatoMode;

    public static CustomOption undertakerSpawnRate;
    public static CustomOption undertakerDragingDelaiAfterKill;
    public static CustomOption undertakerCanDragAndVent;

    public static CustomOption camouflagerSpawnRate;
    public static CustomOption camouflagerCooldown;
    public static CustomOption camouflagerDuration;

    public static CustomOption vampireSpawnRate;
    public static CustomOption vampireKillDelay;
    public static CustomOption vampireCooldown;
    public static CustomOption vampireGarlicButton;
    public static CustomOption vampireCanKillNearGarlics;

    public static CustomOption poucherSpawnRate;
    public static CustomOption mimicSpawnRate;

    public static CustomOption eraserSpawnRate;
    public static CustomOption eraserCooldown;
    public static CustomOption eraserCanEraseAnyone;

    public static CustomOption guesserSpawnRate;
    public static CustomOption guesserIsImpGuesserRate;
    public static CustomOption guesserNumberOfShots;
    public static CustomOption guesserHasMultipleShotsPerMeeting;
    public static CustomOption guesserShowInfoInGhostChat;
    public static CustomOption guesserKillsThroughShield;
    public static CustomOption guesserEvilCanKillSpy;
    public static CustomOption guesserEvilCanKillCrewmate;
    public static CustomOption guesserSpawnBothRate;
    public static CustomOption guesserCantGuessSnitchIfTaksDone;

    public static CustomOption jesterSpawnRate;
    public static CustomOption jesterCanCallEmergency;
    public static CustomOption jesterCanVent;
    public static CustomOption jesterHasImpostorVision;

    public static CustomOption amnisiacSpawnRate;
    public static CustomOption amnisiacShowArrows;
    public static CustomOption amnisiacResetRole;

    public static CustomOption arsonistSpawnRate;
    public static CustomOption arsonistCooldown;
    public static CustomOption arsonistDuration;

    public static CustomOption jackalSpawnRate;
    public static CustomOption jackalKillCooldown;
    public static CustomOption jackalChanceSwoop;
    public static CustomOption swooperCooldown;
    public static CustomOption swooperDuration;
    public static CustomOption jackalCreateSidekickCooldown;
    public static CustomOption jackalKillFakeImpostor;
    public static CustomOption jackalCanUseVents;
    public static CustomOption jackalCanUseSabo;
    public static CustomOption jackalCanCreateSidekick;
    public static CustomOption sidekickPromotesToJackal;
    public static CustomOption sidekickCanKill;
    public static CustomOption sidekickCanUseVents;
    public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;
    public static CustomOption jackalCanCreateSidekickFromImpostor;
    public static CustomOption jackalAndSidekickHaveImpostorVision;

    public static CustomOption bountyHunterSpawnRate;
    public static CustomOption bountyHunterBountyDuration;
    public static CustomOption bountyHunterReducedCooldown;
    public static CustomOption bountyHunterPunishmentTime;
    public static CustomOption bountyHunterShowArrow;
    public static CustomOption bountyHunterArrowUpdateIntervall;

    public static CustomOption witchSpawnRate;
    public static CustomOption witchCooldown;
    public static CustomOption witchAdditionalCooldown;
    public static CustomOption witchCanSpellAnyone;
    public static CustomOption witchSpellCastingDuration;
    public static CustomOption witchTriggerBothCooldowns;
    public static CustomOption witchVoteSavesTargets;

    public static CustomOption ninjaSpawnRate;
    public static CustomOption ninjaCooldown;
    public static CustomOption ninjaKnowsTargetLocation;
    public static CustomOption ninjaTraceTime;
    public static CustomOption ninjaTraceColorTime;
    public static CustomOption ninjaInvisibleDuration;

    public static CustomOption blackmailerSpawnRate;
    public static CustomOption blackmailerCooldown;

    public static CustomOption mayorSpawnRate;
    public static CustomOption mayorCanSeeVoteColors;
    public static CustomOption mayorTasksNeededToSeeVoteColors;
    public static CustomOption mayorMeetingButton;
    public static CustomOption mayorMaxRemoteMeetings;
    public static CustomOption mayorTaskRemoteMeetings;
    public static CustomOption mayorChooseSingleVote;

    public static CustomOption portalmakerSpawnRate;
    public static CustomOption portalmakerCooldown;
    public static CustomOption portalmakerUsePortalCooldown;
    public static CustomOption portalmakerLogOnlyColorType;
    public static CustomOption portalmakerLogHasTime;
    public static CustomOption portalmakerCanPortalFromAnywhere;

    public static CustomOption engineerSpawnRate;

    public static CustomOption engineerRemoteFix;

    //public static CustomOption engineerExpertRepairs;
    public static CustomOption engineerResetFixAfterMeeting;
    public static CustomOption engineerNumberOfFixes;
    public static CustomOption engineerHighlightForImpostors;
    public static CustomOption engineerHighlightForTeamJackal;

    public static CustomOption privateInvestigatorSpawnRate;
    public static CustomOption privateInvestigatorSeeColor;

    public static CustomOption sheriffSpawnRate;
    public static CustomOption sheriffMisfireKills;
    public static CustomOption sheriffCooldown;
    public static CustomOption sheriffCanKillNeutrals;
    public static CustomOption sheriffCanKillArsonist;
    public static CustomOption sheriffCanKillLawyer;
    public static CustomOption sheriffCanKillProsecutor;
    public static CustomOption sheriffCanKillJester;
    public static CustomOption sheriffCanKillVulture;
    public static CustomOption sheriffCanKillThief;
    public static CustomOption sheriffCanKillAmnesiac;
    public static CustomOption sheriffCanKillPursuer;
    public static CustomOption sheriffCanKillDoomsayer;
    public static CustomOption deputySpawnRate;

    public static CustomOption deputyNumberOfHandcuffs;
    public static CustomOption deputyHandcuffCooldown;
    public static CustomOption deputyGetsPromoted;
    public static CustomOption deputyKeepsHandcuffs;
    public static CustomOption deputyHandcuffDuration;
    public static CustomOption deputyKnowsSheriff;

    public static CustomOption lighterSpawnRate;
    public static CustomOption lighterModeLightsOnVision;
    public static CustomOption lighterModeLightsOffVision;
    public static CustomOption lighterFlashlightWidth;

    public static CustomOption detectiveSpawnRate;
    public static CustomOption detectiveAnonymousFootprints;
    public static CustomOption detectiveFootprintIntervall;
    public static CustomOption detectiveFootprintDuration;
    public static CustomOption detectiveReportNameDuration;
    public static CustomOption detectiveReportColorDuration;

    public static CustomOption timeMasterSpawnRate;
    public static CustomOption timeMasterCooldown;
    public static CustomOption timeMasterRewindTime;
    public static CustomOption timeMasterShieldDuration;

    public static CustomOption veterenSpawnRate;
    public static CustomOption veterenCooldown;
    public static CustomOption veterenAlertDuration;

    public static CustomOption medicSpawnRate;
    public static CustomOption medicShowShielded;
    public static CustomOption medicShowAttemptToShielded;
    public static CustomOption medicSetOrShowShieldAfterMeeting;
    public static CustomOption medicShowAttemptToMedic;
    public static CustomOption medicSetShieldAfterMeeting;
    public static CustomOption medicBreakShield;
    public static CustomOption medicResetTargetAfterMeeting;

    public static CustomOption swapperSpawnRate;
    public static CustomOption swapperCanCallEmergency;
    public static CustomOption swapperCanFixSabotages;
    public static CustomOption swapperCanOnlySwapOthers;
    public static CustomOption swapperSwapsNumber;
    public static CustomOption swapperRechargeTasksNumber;

    public static CustomOption seerSpawnRate;
    public static CustomOption seerMode;
    public static CustomOption seerSoulDuration;
    public static CustomOption seerLimitSoulDuration;

    public static CustomOption hackerSpawnRate;
    public static CustomOption hackerCooldown;
    public static CustomOption hackerHackeringDuration;
    public static CustomOption hackerOnlyColorType;
    public static CustomOption hackerToolsNumber;
    public static CustomOption hackerRechargeTasksNumber;
    public static CustomOption hackerNoMove;

    public static CustomOption trackerSpawnRate;
    public static CustomOption trackerUpdateIntervall;
    public static CustomOption trackerResetTargetAfterMeeting;
    public static CustomOption trackerCanTrackCorpses;
    public static CustomOption trackerCorpsesTrackingCooldown;
    public static CustomOption trackerCorpsesTrackingDuration;

    public static CustomOption snitchSpawnRate;
    public static CustomOption snitchLeftTasksForReveal;
    public static CustomOption snitchMode;
    public static CustomOption snitchTargets;

    public static CustomOption spySpawnRate;
    public static CustomOption spyCanDieToSheriff;
    public static CustomOption spyImpostorsCanKillAnyone;
    public static CustomOption spyCanEnterVents;
    public static CustomOption spyHasImpostorVision;

    public static CustomOption tricksterSpawnRate;
    public static CustomOption tricksterPlaceBoxCooldown;
    public static CustomOption tricksterLightsOutCooldown;
    public static CustomOption tricksterLightsOutDuration;

    public static CustomOption cleanerSpawnRate;
    public static CustomOption cleanerCooldown;

    public static CustomOption warlockSpawnRate;
    public static CustomOption warlockCooldown;
    public static CustomOption warlockRootTime;

    public static CustomOption securityGuardSpawnRate;
    public static CustomOption securityGuardCooldown;
    public static CustomOption securityGuardTotalScrews;
    public static CustomOption securityGuardCamPrice;
    public static CustomOption securityGuardVentPrice;
    public static CustomOption securityGuardCamDuration;
    public static CustomOption securityGuardCamMaxCharges;
    public static CustomOption securityGuardCamRechargeTasksNumber;
    public static CustomOption securityGuardNoMove;

    public static CustomOption bodyGuardSpawnRate;
    public static CustomOption bodyGuardFlash;
    public static CustomOption bodyGuardResetTargetAfterMeeting;

    public static CustomOption vultureSpawnRate;
    public static CustomOption vultureCooldown;
    public static CustomOption vultureNumberToWin;
    public static CustomOption vultureCanUseVents;
    public static CustomOption vultureShowArrows;

    public static CustomOption mediumSpawnRate;
    public static CustomOption mediumCooldown;
    public static CustomOption mediumDuration;
    public static CustomOption mediumOneTimeUse;
    public static CustomOption mediumChanceAdditionalInfo;

    public static CustomOption lawyerSpawnRate;
    public static CustomOption lawyerTargetKnows;
    public static CustomOption lawyerIsProsecutorChance;
    public static CustomOption lawyerTargetCanBeJester;
    public static CustomOption lawyerVision;
    public static CustomOption lawyerKnowsRole;
    public static CustomOption lawyerCanCallEmergency;
    public static CustomOption pursuerCooldown;
    public static CustomOption pursuerBlanksNumber;

    public static CustomOption jumperSpawnRate;
    public static CustomOption jumperJumpTime;
    public static CustomOption jumperChargesOnPlace;

    public static CustomOption jumperResetPlaceAfterMeeting;
    //   public static CustomOption jumperChargesGainOnMeeting;
    //public static CustomOption jumperMaxCharges;

    public static CustomOption escapistSpawnRate;
    public static CustomOption escapistEscapeTime;
    public static CustomOption escapistChargesOnPlace;

    public static CustomOption escapistResetPlaceAfterMeeting;
    //   public static CustomOption jumperChargesGainOnMeeting;
    //public static CustomOption escapistMaxCharges;

    public static CustomOption werewolfSpawnRate;
    public static CustomOption werewolfRampageCooldown;
    public static CustomOption werewolfRampageDuration;
    public static CustomOption werewolfKillCooldown;

    public static CustomOption thiefSpawnRate;
    public static CustomOption thiefCooldown;
    public static CustomOption thiefHasImpVision;
    public static CustomOption thiefCanUseVents;
    public static CustomOption thiefCanKillSheriff;
    public static CustomOption thiefCanStealWithGuess;

    //天启添加
    public static CustomOption juggernautSpawnRate;
    public static CustomOption juggernautCooldown;
    public static CustomOption juggernautHasImpVision;

    public static CustomOption juggernautReducedkillEach;

    //末日预言家
    public static CustomOption doomsayerSpawnRate;
    public static CustomOption doomsayerCooldown;
    public static CustomOption doomsayerHasMultipleShotsPerMeeting;
    public static CustomOption doomsayerShowInfoInGhostChat;
    public static CustomOption doomsayerCanGuessNeutral;
    public static CustomOption doomsayerCanGuessImpostor;
    public static CustomOption doomsayerOnlineTarger;
    public static CustomOption doomsayerGuesserCantGuessSnitch;
    public static CustomOption doomsayerKillToWin;
    public static CustomOption doomsayerDormationNum;

    public static CustomOption trapperSpawnRate;
    public static CustomOption trapperCooldown;
    public static CustomOption trapperMaxCharges;
    public static CustomOption trapperRechargeTasksNumber;
    public static CustomOption trapperTrapNeededTriggerToReveal;
    public static CustomOption trapperAnonymousMap;
    public static CustomOption trapperInfoType;
    public static CustomOption trapperTrapDuration;

    public static CustomOption bomberSpawnRate;
    public static CustomOption bomberBombDestructionTime;
    public static CustomOption bomberBombDestructionRange;
    public static CustomOption bomberBombHearRange;
    public static CustomOption bomberDefuseDuration;
    public static CustomOption bomberBombCooldown;
    public static CustomOption bomberBombActiveAfter;

    public static CustomOption modifiersAreHidden;

    public static CustomOption modifierAssassin;
    public static CustomOption modifierAssassinQuantity;
    public static CustomOption modifierAssassinNumberOfShots;
    public static CustomOption modifierAssassinMultipleShotsPerMeeting;
    public static CustomOption modifierAssassinKillsThroughShield;
    public static CustomOption modifierAssassinCultist;

    public static CustomOption modifierBait;
    public static CustomOption modifierBaitQuantity;
    public static CustomOption modifierBaitReportDelayMin;
    public static CustomOption modifierBaitReportDelayMax;
    public static CustomOption modifierBaitShowKillFlash;

    public static CustomOption modifierLover;
    public static CustomOption modifierLoverImpLoverRate;
    public static CustomOption modifierLoverBothDie;
    public static CustomOption modifierLoverEnableChat;

    public static CustomOption modifierBloody;
    public static CustomOption modifierBloodyQuantity;
    public static CustomOption modifierBloodyDuration;

    public static CustomOption modifierAntiTeleport;
    public static CustomOption modifierAntiTeleportQuantity;

    public static CustomOption modifierTieBreaker;

    public static CustomOption modifierSunglasses;
    public static CustomOption modifierSunglassesQuantity;
    public static CustomOption modifierSunglassesVision;

    public static CustomOption modifierTorch;
    public static CustomOption modifierTorchVision;
    public static CustomOption modifierTorchQuantity;

    public static CustomOption modifierMultitasker;
    public static CustomOption modifierMultitaskerQuantity;

    public static CustomOption modifierDisperser;

    public static CustomOption modifierMini;
    public static CustomOption modifierMiniGrowingUpDuration;
    public static CustomOption modifierMiniGrowingUpInMeeting;

    public static CustomOption modifierIndomitable;

    public static CustomOption modifierBlind;

    public static CustomOption modifierTunneler;

    public static CustomOption modifierWatcher;

    public static CustomOption modifierRadar;

    public static CustomOption modifierSlueth;
    //public static CustomOption modifierSwooper;

    public static CustomOption modifierCursed;

    public static CustomOption modifierVip;
    public static CustomOption modifierVipQuantity;
    public static CustomOption modifierVipShowColor;

    public static CustomOption modifierInvert;
    public static CustomOption modifierInvertQuantity;
    public static CustomOption modifierInvertDuration;

    public static CustomOption modifierChameleon;
    public static CustomOption modifierChameleonQuantity;
    public static CustomOption modifierChameleonHoldDuration;
    public static CustomOption modifierChameleonFadeDuration;
    public static CustomOption modifierChameleonMinVisibility;

    public static CustomOption modifierShifter;

    public static CustomOption maxNumberOfMeetings;
    public static CustomOption blockSkippingInEmergencyMeetings;
    public static CustomOption noVoteIsSelfVote;
    public static CustomOption hidePlayerNames;
    public static CustomOption showButtonTarget;
    public static CustomOption blockGameEnd;
    public static CustomOption allowParallelMedBayScans;
    public static CustomOption shieldFirstKill;
    public static CustomOption hideVentAnimOnShadows;
    public static CustomOption disableCamsRound1;
    public static CustomOption hideOutOfSightNametags;
    public static CustomOption impostorSeeRoles;
    public static CustomOption transparentTasks;
    public static CustomOption randomGameStartPosition;
    public static CustomOption allowModGuess;
    public static CustomOption finishTasksBeforeHauntingOrZoomingOut;
    public static CustomOption camsNightVision;
    public static CustomOption camsNoNightVisionIfImpVision;

    public static CustomOption dynamicMap;
    public static CustomOption dynamicMapEnableSkeld;
    public static CustomOption dynamicMapEnableMira;
    public static CustomOption dynamicMapEnablePolus;
    public static CustomOption dynamicMapEnableAirShip;
    public static CustomOption dynamicMapEnableFungle;
    public static CustomOption dynamicMapEnableSubmerged;
    public static CustomOption dynamicMapSeparateSettings;


    public static CustomOption enableBetterPolus;

    public static CustomOption movePolusVents;

    //添加新管道
    public static CustomOption addPolusVents;
    public static CustomOption swapNavWifi;
    public static CustomOption movePolusVitals;
    public static CustomOption moveColdTemp;

    public static CustomOption enableAirShipModify;
    public static CustomOption addAirShipVents;

    public static CustomOption disableMedbayWalk;

    public static CustomOption enableCamoComms;

    public static CustomOption restrictDevices;

    //public static CustomOption restrictAdmin;
    public static CustomOption restrictCameras;
    public static CustomOption restrictVents;

    //Guesser Gamemode
    public static CustomOption guesserGamemodeCrewNumber;
    public static CustomOption guesserGamemodeNeutralNumber;
    public static CustomOption guesserGamemodeImpNumber;
    public static CustomOption guesserForceJackalGuesser;
    public static CustomOption guesserForceThiefGuesser;
    public static CustomOption guesserGamemodeHaveModifier;
    public static CustomOption guesserGamemodeNumberOfShots;
    public static CustomOption guesserGamemodeHasMultipleShotsPerMeeting;
    public static CustomOption guesserGamemodeKillsThroughShield;
    public static CustomOption guesserGamemodeEvilCanKillSpy;
    public static CustomOption guesserGamemodeCantGuessSnitchIfTaksDone;

    // Hide N Seek Gamemode
    public static CustomOption hideNSeekHunterCount;
    public static CustomOption hideNSeekKillCooldown;
    public static CustomOption hideNSeekHunterVision;
    public static CustomOption hideNSeekHuntedVision;
    public static CustomOption hideNSeekTimer;
    public static CustomOption hideNSeekCommonTasks;
    public static CustomOption hideNSeekShortTasks;
    public static CustomOption hideNSeekLongTasks;
    public static CustomOption hideNSeekTaskWin;
    public static CustomOption hideNSeekTaskPunish;
    public static CustomOption hideNSeekCanSabotage;
    public static CustomOption hideNSeekMap;
    public static CustomOption hideNSeekHunterWaiting;

    public static CustomOption hunterLightCooldown;
    public static CustomOption hunterLightDuration;
    public static CustomOption hunterLightVision;
    public static CustomOption hunterLightPunish;
    public static CustomOption hunterAdminCooldown;
    public static CustomOption hunterAdminDuration;
    public static CustomOption hunterAdminPunish;
    public static CustomOption hunterArrowCooldown;
    public static CustomOption hunterArrowDuration;
    public static CustomOption hunterArrowPunish;

    public static CustomOption huntedShieldCooldown;
    public static CustomOption huntedShieldDuration;
    public static CustomOption huntedShieldRewindTime;
    public static CustomOption huntedShieldNumber;

    // Prop Hunt Settings
    public static CustomOption propHuntMap;
    public static CustomOption propHuntTimer;
    public static CustomOption propHuntNumberOfHunters;
    public static CustomOption hunterInitialBlackoutTime;
    public static CustomOption hunterMissCooldown;
    public static CustomOption hunterHitCooldown;
    public static CustomOption hunterMaxMissesBeforeDeath;
    public static CustomOption propBecomesHunterWhenFound;
    public static CustomOption propHunterVision;
    public static CustomOption propVision;
    public static CustomOption propHuntRevealCooldown;
    public static CustomOption propHuntRevealDuration;
    public static CustomOption propHuntRevealPunish;
    public static CustomOption propHuntUnstuckCooldown;
    public static CustomOption propHuntUnstuckDuration;
    public static CustomOption propHuntInvisCooldown;
    public static CustomOption propHuntInvisDuration;
    public static CustomOption propHuntSpeedboostCooldown;
    public static CustomOption propHuntSpeedboostDuration;
    public static CustomOption propHuntSpeedboostSpeed;
    public static CustomOption propHuntSpeedboostEnabled;
    public static CustomOption propHuntInvisEnabled;
    public static CustomOption propHuntAdminCooldown;
    public static CustomOption propHuntFindCooldown;
    public static CustomOption propHuntFindDuration;

    internal static readonly Dictionary<byte, byte[]> blockedRolePairings = new();

    public static string cs(Color c, string s)
    {
        return $"<color=#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}{ToByte(c.a):X2}>{s}</color>";
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static bool isMapSelectionOption(CustomOption option)
    {
        return option == propHuntMap && option == hideNSeekMap;
    }

    public static void Load()
    {
        CustomOption.vanillaSettings = TheOtherRolesPlugin.Instance.Config.Bind("预设0", "原版设置", "");

        // Role Options
        presetSelection = CustomOption.Create(0, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "预设"),
            presets, null, true);
        activateRoles = CustomOption.Create(1, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "启用模组职业并禁用原版职业"), true, null, true);

        if (EventUtility.canBeEnabled)
            enableCodenameHorsemode =
                CustomOption.Create(10423, Types.General, cs(Color.green, "启用愚人节马模式"), true, null, true);

        if (EventUtility.canBeEnabled)
            enableCodenameDisableHorses = CustomOption.Create(10424, Types.General, cs(Color.green, "禁用马模式"), false,
                enableCodenameHorsemode);

        enableCrowdedPlayer = CustomOption.Create(15000, Types.General,
            cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "启用百人"), false, null, true);
        CrowdedPlayer.Enable = enableCrowdedPlayer.getBool();

        MaxPlayer = CustomOption.Create(15001, Types.General,
            cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "最大玩家数"), 20f, 20f, 120f, 1f, enableCrowdedPlayer);

        CrowdedPlayer.MaxPlayer = MaxPlayer.GetInt();
        // Using new id's for the options to not break compatibilty with older versions
        crewmateRolesCountMin = CustomOption.Create(300, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最小船员阵营职业数"), 15f, 0f, 15f, 1f, null, true);
        crewmateRolesCountMax = CustomOption.Create(301, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最大船员阵营职业数"), 15f, 0f, 15f, 1f);
        crewmateRolesFill = CustomOption.Create(308, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "所有船员必定拥有职业\n(无视最小/最大数量)"), false);
        neutralRolesCountMin = CustomOption.Create(302, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最小独立阵营职业数"), 15f, 0f, 15f, 1f);
        neutralRolesCountMax = CustomOption.Create(303, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最大独立阵营职业数"), 15f, 0f, 15f, 1f);
        impostorRolesCountMin = CustomOption.Create(304, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最小内鬼阵营职业数"), 15f, 0f, 15f, 1f);
        impostorRolesCountMax = CustomOption.Create(305, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最大内鬼阵营职业数"), 15f, 0f, 15f, 1f);
        modifiersCountMin = CustomOption.Create(306, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最小附加职业数"), 15f, 0f, 15f, 1f);
        modifiersCountMax = CustomOption.Create(307, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最大附加职业数"), 15f, 0f, 15f, 1f);

        modifierAssassin = CustomOption.Create(2000, Types.Impostor, cs(Color.red, "刺客"), rates, null, true);
        modifierAssassinQuantity =
            CustomOption.Create(2001, Types.Impostor, cs(Color.red, "刺客数量"), ratesModifier, modifierAssassin);
        modifierAssassinNumberOfShots =
            CustomOption.Create(2002, Types.Impostor, "可猜测次数", 5f, 1f, 15f, 1f, modifierAssassin);
        modifierAssassinMultipleShotsPerMeeting =
            CustomOption.Create(2003, Types.Impostor, "同一轮会议可多次猜测", true, modifierAssassin);
        guesserEvilCanKillSpy = CustomOption.Create(2004, Types.Impostor, "可以猜测职业“卧底”", true, modifierAssassin);
        guesserEvilCanKillCrewmate = CustomOption.Create(20045, Types.Impostor, "可以猜测职业“船员”", true, modifierAssassin);
        guesserCantGuessSnitchIfTaksDone =
            CustomOption.Create(2005, Types.Impostor, "不可猜测完成任务的告密者", true, modifierAssassin);
        modifierAssassinKillsThroughShield =
            CustomOption.Create(2006, Types.Impostor, "猜测无视法医护盾保护", false, modifierAssassin);
        modifierAssassinCultist = CustomOption.Create(2004446, Types.Impostor, "新信徒可成为刺客", false, modifierAssassin);

        mafiaSpawnRate = CustomOption.Create(18, Types.Impostor, cs(Janitor.color, "黑手党"), rates, null, true);
        janitorCooldown = CustomOption.Create(19, Types.Impostor, "清洁工清理冷却", 30f, 10f, 60f, 2.5f, mafiaSpawnRate);

        morphlingSpawnRate = CustomOption.Create(20, Types.Impostor, cs(Morphling.color, "化形者"), rates, null, true);
        morphlingCooldown = CustomOption.Create(21, Types.Impostor, "化形冷却", 30f, 10f, 60f, 2.5f, morphlingSpawnRate);
        morphlingDuration = CustomOption.Create(22, Types.Impostor, "化形持续时间", 10f, 1f, 20f, 0.5f, morphlingSpawnRate);

        bomber2SpawnRate = CustomOption.Create(8840, Types.Impostor, cs(Bomber2.color, "炸弹狂"), rates, null, true);
        bomber2BombCooldown = CustomOption.Create(8841, Types.Impostor, "炸弹冷却", 30f, 10f, 60f, 2.5f, bomber2SpawnRate);
        bomber2Delay = CustomOption.Create(8842, Types.Impostor, "炸弹激活时间", 5f, 0f, 20f, 0.5f, bomber2SpawnRate);
        bomber2Timer = CustomOption.Create(8843, Types.Impostor, "炸弹爆炸时间", 15f, 5f, 30f, 0.5f, bomber2SpawnRate);
        //bomber2HotPotatoMode = CustomOption.Create(2526236, Types.Impostor, "Hot Potato Mode", false, bomber2SpawnRate);

        undertakerSpawnRate = CustomOption.Create(1201, Types.Impostor, cs(Undertaker.color, "送葬者"), rates, null, true);
        undertakerDragingDelaiAfterKill = CustomOption.Create(1202, Types.Impostor, "从击杀到恢复拖曳能力所需时间", 0f, 0f, 15, 0.5f,
            undertakerSpawnRate);
        undertakerCanDragAndVent = CustomOption.Create(1203, Types.Impostor, "拖曳过程中可使用管道", true, undertakerSpawnRate);

        camouflagerSpawnRate = CustomOption.Create(30, Types.Impostor, cs(Camouflager.color, "隐蔽者"), rates, null, true);
        camouflagerCooldown =
            CustomOption.Create(31, Types.Impostor, "隐蔽状态冷却", 30f, 10f, 60f, 2.5f, camouflagerSpawnRate);
        camouflagerDuration =
            CustomOption.Create(32, Types.Impostor, "隐蔽状态持续时间", 10f, 1f, 20f, 0.5f, camouflagerSpawnRate);

        vampireSpawnRate = CustomOption.Create(40, Types.Impostor, cs(Vampire.color, "吸血鬼"), rates, null, true);
        vampireKillDelay = CustomOption.Create(41, Types.Impostor, "从吸血到击杀所需时间", 5f, 1f, 20f, 0.5f, vampireSpawnRate);
        vampireCooldown = CustomOption.Create(42, Types.Impostor, "吸血冷却", 30f, 10f, 60f, 2.5f, vampireSpawnRate);
        vampireGarlicButton = CustomOption.Create(43277854, Types.Impostor, "发放大蒜", true, vampireSpawnRate);
        vampireCanKillNearGarlics = CustomOption.Create(43, Types.Impostor, "可在大蒜附近击杀", true, vampireGarlicButton);

        eraserSpawnRate = CustomOption.Create(230, Types.Impostor, cs(Eraser.color, "抹除者"), rates, null, true);
        eraserCooldown = CustomOption.Create(231, Types.Impostor, "抹除冷却", 30f, 10f, 120f, 2.5f, eraserSpawnRate);
        eraserCanEraseAnyone = CustomOption.Create(232, Types.Impostor, "可抹除任何人", false, eraserSpawnRate);

        poucherSpawnRate = CustomOption.Create(8833, Types.Impostor, cs(Poucher.color, "入殓师"), rates, null, true);
        mimicSpawnRate = CustomOption.Create(8835, Types.Impostor, cs(Mimic.color, "模仿者"), rates, null, true);

        escapistSpawnRate = CustomOption.Create(905000, Types.Impostor, cs(Escapist.color, "逃逸者"), rates, null, true);
        escapistEscapeTime =
            CustomOption.Create(905100, Types.Impostor, "标记/逃逸冷却", 15f, 0f, 60f, 2.5f, escapistSpawnRate);
        escapistChargesOnPlace =
            CustomOption.Create(905200, Types.Impostor, "每次逃逸/传送消耗点数", 1, 1, 10, 1, escapistSpawnRate);
        //jumperResetPlaceAfterMeeting = CustomOption.Create(9052, Types.Crewmate, "Reset Places After Meeting", true, jumperSpawnRate);
        //jumperChargesGainOnMeeting = CustomOption.Create(9053, Types.Crewmate, "Charges Gained After Meeting", 2, 0, 10, 1, jumperSpawnRate);
        //escapistMaxCharges = CustomOption.Create(905400, Types.Impostor, "Maximum Charges", 3, 0, 10, 1, escapistSpawnRate);

        cultistSpawnRate = CustomOption.Create(3801, Types.Impostor, cs(Cultist.color, "传教士"), rates, null, true);

        tricksterSpawnRate = CustomOption.Create(250, Types.Impostor, cs(Trickster.color, "骗术师"), rates, null, true);
        tricksterPlaceBoxCooldown =
            CustomOption.Create(251, Types.Impostor, "放置惊吓盒冷却", 10f, 2.5f, 30f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutCooldown =
            CustomOption.Create(252, Types.Impostor, "熄灯冷却", 30f, 10f, 60f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutDuration =
            CustomOption.Create(253, Types.Impostor, "熄灯持续时间", 10f, 5f, 60f, 0.5f, tricksterSpawnRate);

        cleanerSpawnRate = CustomOption.Create(260, Types.Impostor, cs(Cleaner.color, "清理者"), rates, null, true);
        cleanerCooldown = CustomOption.Create(261, Types.Impostor, "清理冷却", 30f, 10f, 60f, 2.5f, cleanerSpawnRate);

        warlockSpawnRate = CustomOption.Create(270, Types.Impostor, cs(Cleaner.color, "术士"), rates, null, true);
        warlockCooldown = CustomOption.Create(271, Types.Impostor, "术法冷却", 30f, 10f, 60f, 2.5f, warlockSpawnRate);
        warlockRootTime =
            CustomOption.Create(272, Types.Impostor, "使用术法击杀后定身持续时间", 0f, 0f, 15f, 0.5f, warlockSpawnRate);

        bountyHunterSpawnRate =
            CustomOption.Create(320, Types.Impostor, cs(BountyHunter.color, "赏金猎人"), rates, null, true);
        bountyHunterBountyDuration =
            CustomOption.Create(321, Types.Impostor, "赏金目标更换间隔", 60f, 10f, 180f, 5f, bountyHunterSpawnRate);
        bountyHunterReducedCooldown = CustomOption.Create(322, Types.Impostor, "击杀目标后的奖励冷却", 2.5f, 0f, 30f, 2.5f,
            bountyHunterSpawnRate);
        bountyHunterPunishmentTime =
            CustomOption.Create(323, Types.Impostor, "击杀非目标后的惩罚冷却", 20f, 0f, 60f, 2.5f, bountyHunterSpawnRate);
        bountyHunterShowArrow = CustomOption.Create(324, Types.Impostor, "显示指向悬赏目标的箭头", true, bountyHunterSpawnRate);
        bountyHunterArrowUpdateIntervall =
            CustomOption.Create(325, Types.Impostor, "箭头更新间隔", 2.5f, 0f, 15f, 0.5f, bountyHunterShowArrow);

        witchSpawnRate = CustomOption.Create(370, Types.Impostor, cs(Witch.color, "女巫"), rates, null, true);
        witchCooldown = CustomOption.Create(371, Types.Impostor, "诅咒冷却", 30f, 10f, 60, 2.5f, witchSpawnRate);
        witchAdditionalCooldown =
            CustomOption.Create(372, Types.Impostor, "诅咒冷却递增", 10f, 0f, 60f, 2.5f, witchSpawnRate);
        witchCanSpellAnyone = CustomOption.Create(373, Types.Impostor, "可诅咒任何人", false, witchSpawnRate);
        witchSpellCastingDuration =
            CustomOption.Create(374, Types.Impostor, "贴身诅咒所需时间", 1f, 0f, 10f, 0.5f, witchSpawnRate);
        witchTriggerBothCooldowns = CustomOption.Create(375, Types.Impostor, "诅咒与击杀冷却共用", true, witchSpawnRate);
        witchVoteSavesTargets = CustomOption.Create(376, Types.Impostor, "驱逐女巫可拯救被诅咒者", true, witchSpawnRate);

        ninjaSpawnRate = CustomOption.Create(380, Types.Impostor, cs(Ninja.color, "忍者"), rates, null, true);
        ninjaCooldown = CustomOption.Create(381, Types.Impostor, "标记冷却", 30f, 10f, 60f, 2.5f, ninjaSpawnRate);
        ninjaKnowsTargetLocation = CustomOption.Create(382, Types.Impostor, "显示指向忍杀对象的箭头", true, ninjaSpawnRate);
        ninjaTraceTime = CustomOption.Create(383, Types.Impostor, "忍杀后树叶痕迹持续时间", 5f, 1f, 20f, 0.5f, ninjaSpawnRate);
        ninjaTraceColorTime =
            CustomOption.Create(384, Types.Impostor, "忍杀后痕迹褪色所需时间", 2f, 0f, 20f, 0.5f, ninjaSpawnRate);
        ninjaInvisibleDuration =
            CustomOption.Create(385, Types.Impostor, "忍杀后隐身持续时间", 10f, 0f, 20f, 0.5f, ninjaSpawnRate);

        blackmailerSpawnRate =
            CustomOption.Create(710, Types.Impostor, cs(Blackmailer.color, "勒索者"), rates, null, true);
        blackmailerCooldown =
            CustomOption.Create(711, Types.Impostor, "勒索冷却", 15f, 5f, 120f, 2.5f, blackmailerSpawnRate);

        bomberSpawnRate = CustomOption.Create(460, Types.Impostor, cs(Bomber.color, "恐怖分子"), rates, null, true);
        bomberBombDestructionTime =
            CustomOption.Create(461, Types.Impostor, "炸弹引爆时间", 20f, 2.5f, 120f, 2.5f, bomberSpawnRate);
        bomberBombDestructionRange =
            CustomOption.Create(462, Types.Impostor, "炸弹爆炸范围", 60f, 5f, 250f, 5f, bomberSpawnRate);
        bomberBombHearRange = CustomOption.Create(463, Types.Impostor, "炸弹可见范围", 50f, 5f, 250f, 5f, bomberSpawnRate);
        bomberDefuseDuration =
            CustomOption.Create(464, Types.Impostor, "拆除炸弹所需时间", 3f, 0.5f, 30f, 0.5f, bomberSpawnRate);
        bomberBombCooldown = CustomOption.Create(465, Types.Impostor, "炸弹放置冷却", 15f, 2.5f, 30f, 2.5f, bomberSpawnRate);
        bomberBombActiveAfter =
            CustomOption.Create(466, Types.Impostor, "炸弹激活时间", 3f, 0.5f, 15f, 0.5f, bomberSpawnRate);
        /*
                    guesserSpawnRate = CustomOption.Create(310, Types.Neutral, cs(Guesser.color, "Guesser"), rates, null, true);
                    guesserIsImpGuesserRate = CustomOption.Create(311, Types.Neutral, "Chance That The Guesser Is An Impostor", rates, guesserSpawnRate);
                    guesserNumberOfShots = CustomOption.Create(312, Types.Neutral, "Guesser Number Of Shots", 2f, 1f, 15f, 1f, guesserSpawnRate);
                    guesserHasMultipleShotsPerMeeting = CustomOption.Create(313, Types.Neutral, "Guesser Can Shoot Multiple Times Per Meeting", false, guesserSpawnRate);
                    guesserKillsThroughShield  = CustomOption.Create(315, Types.Neutral, "Guesses Ignore The Medic Shield", true, guesserSpawnRate);
                    guesserEvilCanKillSpy  = CustomOption.Create(316, Types.Neutral, "Evil Guesser Can Guess The Spy", true, guesserSpawnRate);
                    guesserSpawnBothRate = CustomOption.Create(317, Types.Neutral, "Both Guesser Spawn Rate", rates, guesserSpawnRate);
                    guesserCantGuessSnitchIfTaksDone = CustomOption.Create(318, Types.Neutral, "Guesser Can't Guess Snitch When Tasks Completed", true, guesserSpawnRate);
                    */

        jesterSpawnRate = CustomOption.Create(60, Types.Neutral, cs(Jester.color, "小丑"), rates, null, true);
        jesterCanCallEmergency = CustomOption.Create(61, Types.Neutral, "小丑可召开会议", true, jesterSpawnRate);
        jesterCanVent = CustomOption.Create(1901, Types.Neutral, "小丑可使用管道", true, jesterSpawnRate);
        jesterHasImpostorVision = CustomOption.Create(62, Types.Neutral, "拥有内鬼视野", false, jesterSpawnRate);

        amnisiacSpawnRate = CustomOption.Create(616, Types.Neutral, cs(Amnisiac.color, "失忆者"), rates, null, true);
        amnisiacShowArrows = CustomOption.Create(617, Types.Neutral, "显示指向尸体的箭头", true, amnisiacSpawnRate);
        amnisiacResetRole = CustomOption.Create(618, Types.Neutral, "回忆后重置该职业技能使用次数", true, amnisiacSpawnRate);

        arsonistSpawnRate = CustomOption.Create(290, Types.Neutral, cs(Arsonist.color, "纵火犯"), rates, null, true);
        arsonistCooldown = CustomOption.Create(291, Types.Neutral, "涂油冷却", 12.5f, 2.5f, 60f, 2.5f, arsonistSpawnRate);
        arsonistDuration = CustomOption.Create(292, Types.Neutral, "涂油所需时间", 0.5f, 0f, 10f, 0.5f, arsonistSpawnRate);

        jackalSpawnRate = CustomOption.Create(220, Types.Neutral, cs(Jackal.color, "豺狼"), rates, null, true);
        jackalKillCooldown = CustomOption.Create(221, Types.Neutral, "豺狼/跟班击杀冷却", 30f, 10f, 60f, 2.5f, jackalSpawnRate);
        jackalChanceSwoop = CustomOption.Create(3642134, Types.Neutral, "豺狼获得隐身能力的概率", rates, jackalSpawnRate);
        swooperCooldown = CustomOption.Create(1111, Types.Neutral, "隐身冷却", 30f, 10f, 60f, 2.5f, jackalChanceSwoop);
        swooperDuration = CustomOption.Create(1112, Types.Neutral, "隐身持续时间", 10f, 1f, 20f, 0.5f, jackalChanceSwoop);
        jackalCreateSidekickCooldown =
            CustomOption.Create(222, Types.Neutral, "豺狼招募冷却", 30f, 10f, 60f, 2.5f, jackalSpawnRate);
        jackalCanUseVents = CustomOption.Create(223, Types.Neutral, "豺狼可使用管道", true, jackalSpawnRate);
        jackalCanUseSabo = CustomOption.Create(8876, Types.Neutral, "豺狼/跟班可进行破坏", false, jackalSpawnRate);
        jackalCanCreateSidekick = CustomOption.Create(224, Types.Neutral, "豺狼可以招募跟班", false, jackalSpawnRate);
        sidekickPromotesToJackal = CustomOption.Create(225, Types.Neutral, "豺狼死后跟班可晋升", false, jackalCanCreateSidekick);
        sidekickCanKill = CustomOption.Create(226, Types.Neutral, "跟班可进行击杀", false, jackalCanCreateSidekick);
        sidekickCanUseVents = CustomOption.Create(227, Types.Neutral, "跟班可使用管道", true, jackalCanCreateSidekick);
        jackalPromotedFromSidekickCanCreateSidekick =
            CustomOption.Create(228, Types.Neutral, "晋升后的豺狼可以招募跟班", true, sidekickPromotesToJackal);
        jackalCanCreateSidekickFromImpostor =
            CustomOption.Create(229, Types.Neutral, "豺狼可以招募伪装者为跟班", true, jackalCanCreateSidekick);
        jackalKillFakeImpostor =
            CustomOption.Create(7885, Types.Neutral, "豺狼不可击杀被招募失败的伪装者", true, jackalCanCreateSidekick);
        jackalAndSidekickHaveImpostorVision =
            CustomOption.Create(430, Types.Neutral, "豺狼/跟班拥有内鬼视野", false, jackalSpawnRate);

        minerSpawnRate = CustomOption.Create(1120, Types.Impostor, cs(Miner.color, "管道工"), rates, null, true);
        minerCooldown = CustomOption.Create(1121, Types.Impostor, "制造管道冷却", 25f, 10f, 60f, 2.5f, minerSpawnRate);

        vultureSpawnRate = CustomOption.Create(340, Types.Neutral, cs(Vulture.color, "秃鹫"), rates, null, true);
        vultureCooldown = CustomOption.Create(341, Types.Neutral, "吞噬冷却", 15f, 10f, 60f, 2.5f, vultureSpawnRate);
        vultureNumberToWin = CustomOption.Create(342, Types.Neutral, "获胜所需吞噬次数", 3f, 1f, 10f, 1f, vultureSpawnRate);
        vultureCanUseVents = CustomOption.Create(343, Types.Neutral, "可使用管道", true, vultureSpawnRate);
        vultureShowArrows = CustomOption.Create(344, Types.Neutral, "显示指向尸体的箭头", true, vultureSpawnRate);

        lawyerSpawnRate = CustomOption.Create(350, Types.Neutral, cs(Lawyer.color, "律师"), rates, null, true);
        lawyerIsProsecutorChance = CustomOption.Create(358, Types.Neutral, "律师为处刑者的概率", rates, lawyerSpawnRate);
        lawyerTargetKnows = CustomOption.Create(3511, Types.Neutral, "客户知道律师存在", true, lawyerSpawnRate);
        lawyerVision = CustomOption.Create(354, Types.Neutral, "视野倍率", 1f, 0.25f, 3f, 0.25f, lawyerSpawnRate);
        lawyerKnowsRole = CustomOption.Create(355, Types.Neutral, "律师/处刑者可得知目标职业", false, lawyerSpawnRate);
        lawyerCanCallEmergency = CustomOption.Create(352, Types.Neutral, "律师/处刑者可召开会议", true, lawyerSpawnRate);
        lawyerTargetCanBeJester = CustomOption.Create(351, Types.Neutral, "小丑可以成为律师的客户", false, lawyerSpawnRate);
        pursuerCooldown = CustomOption.Create(356, Types.Neutral, "起诉人空包弹冷却", 30f, 5f, 60f, 2.5f, lawyerSpawnRate);
        pursuerBlanksNumber = CustomOption.Create(357, Types.Neutral, "起诉人空包弹可用次数", 5f, 1f, 20f, 1f, lawyerSpawnRate);

        werewolfSpawnRate = CustomOption.Create(1501, Types.Neutral, cs(Werewolf.color, "月下狼人"), rates, null, true);
        werewolfRampageCooldown =
            CustomOption.Create(1502, Types.Neutral, "狂暴冷却", 30f, 10f, 60f, 2.5f, werewolfSpawnRate);
        werewolfRampageDuration =
            CustomOption.Create(1503, Types.Neutral, "狂暴持续时间", 15f, 1f, 20f, 0.5f, werewolfSpawnRate);
        werewolfKillCooldown = CustomOption.Create(1504, Types.Neutral, "击杀冷却", 3f, 1f, 60f, 0.5f, werewolfSpawnRate);

        juggernautSpawnRate = CustomOption.Create(10101, Types.Neutral, cs(Juggernaut.color, "天启"), rates, null, true);
        juggernautCooldown =
            CustomOption.Create(10102, Types.Neutral, "击杀冷却", 30f, 2.5f, 60f, 2.5f, juggernautSpawnRate);
        juggernautHasImpVision = CustomOption.Create(10103, Types.Neutral, "天启拥有伪装者视野", true, juggernautSpawnRate);
        juggernautReducedkillEach =
            CustomOption.Create(10104, Types.Neutral, "每次击杀后减少的cd", 5f, 1f, 15f, 0.5f, juggernautSpawnRate);

        doomsayerSpawnRate = CustomOption.Create(10111, Types.Neutral, cs(Doomsayer.color, "末日预言家"), rates, null, true);
        doomsayerCooldown = CustomOption.Create(10112, Types.Neutral, "技能冷却", 30f, 2.5f, 60f, 2.5f, doomsayerSpawnRate);
        doomsayerHasMultipleShotsPerMeeting =
            CustomOption.Create(10113, Types.Neutral, "猜测成功后可继续猜测", true, doomsayerSpawnRate);
        doomsayerShowInfoInGhostChat = CustomOption.Create(10114, Types.Neutral, "灵魂可见猜测结果", true, doomsayerSpawnRate);
        doomsayerCanGuessNeutral = CustomOption.Create(10115, Types.Neutral, "可以猜测中立", true, doomsayerSpawnRate);
        doomsayerCanGuessImpostor = CustomOption.Create(10116, Types.Neutral, "可以猜测伪装者", true, doomsayerSpawnRate);
        doomsayerOnlineTarger = CustomOption.Create(10117, Types.Neutral, "是否获取已有职业", false, doomsayerSpawnRate);
        doomsayerKillToWin =
            CustomOption.Create(10118, Types.Neutral, "需要成功猜测几次获胜", 3f, 1f, 10f, 1f, doomsayerSpawnRate);
        doomsayerDormationNum =
            CustomOption.Create(10119, Types.Neutral, "预言的职业数量", 3f, 1f, 10f, 1f, doomsayerSpawnRate);

        guesserSpawnRate = CustomOption.Create(310, Types.Crewmate, cs(Guesser.color, "侠客"), rates, null, true);
        guesserNumberOfShots = CustomOption.Create(311, Types.Crewmate, "可猜测次数", 3f, 1f, 15f, 1f, guesserSpawnRate);
        guesserHasMultipleShotsPerMeeting =
            CustomOption.Create(312, Types.Crewmate, "同一轮会议可多次猜测", true, guesserSpawnRate);
        guesserShowInfoInGhostChat = CustomOption.Create(313, Types.Crewmate, "灵魂可见猜测结果", true, guesserSpawnRate);
        guesserKillsThroughShield = CustomOption.Create(314, Types.Crewmate, "猜测无视法医护盾保护", false, guesserSpawnRate);

        mayorSpawnRate = CustomOption.Create(80, Types.Crewmate, cs(Mayor.color, "市长"), rates, null, true);
        mayorCanSeeVoteColors = CustomOption.Create(81, Types.Crewmate, "拥有窥视能力", false, mayorSpawnRate);
        mayorTasksNeededToSeeVoteColors = CustomOption.Create(82, Types.Crewmate, "获得窥视能力所需完成的任务数", 5f, 0f, 20f, 1f,
            mayorCanSeeVoteColors);
        mayorMeetingButton = CustomOption.Create(83, Types.Crewmate, "可远程召开会议", true, mayorSpawnRate);
        mayorMaxRemoteMeetings =
            CustomOption.Create(84, Types.Crewmate, "远程召开会议可用次数", 1f, 1f, 5f, 1f, mayorMeetingButton);
        mayorTaskRemoteMeetings =
            CustomOption.Create(85, Types.Crewmate, "可在破坏时使用", false, mayorMeetingButton);
        mayorChooseSingleVote = CustomOption.Create(85, Types.Crewmate, "市长可选择投单票", new[] { "关闭", "投票前选择", "会议结束前选择" },
            mayorSpawnRate);

        engineerSpawnRate = CustomOption.Create(90, Types.Crewmate, cs(Engineer.color, "工程师"), rates, null, true);
        engineerRemoteFix = CustomOption.Create(911221, Types.Crewmate, "可远程修理破坏", true, engineerSpawnRate);
        engineerResetFixAfterMeeting = CustomOption.Create(9111, Types.Crewmate, "会议后重置修理次数", true, engineerRemoteFix);
        engineerNumberOfFixes = CustomOption.Create(91, Types.Crewmate, "远程修理可用次数", 1f, 1f, 3f, 1f, engineerRemoteFix);
        //engineerExpertRepairs = CustomOption.Create(91121, Types.Crewmate, "Advanced Sabotage Repair", false, engineerSpawnRate);
        engineerHighlightForImpostors = CustomOption.Create(92, Types.Crewmate, "内鬼可见工程师管道高光", true, engineerSpawnRate);
        engineerHighlightForTeamJackal =
            CustomOption.Create(93, Types.Crewmate, "豺狼/跟班可见工程师管道高光 ", true, engineerSpawnRate);

        privateInvestigatorSpawnRate = CustomOption.Create(8839, Types.Crewmate, cs(PrivateInvestigator.color, "观察者"),
            rates, null, true);
        privateInvestigatorSeeColor =
            CustomOption.Create(8844, Types.Crewmate, "可见技能触发时对方具体颜色", true, privateInvestigatorSpawnRate);

        sheriffSpawnRate = CustomOption.Create(100, Types.Crewmate, cs(Sheriff.color, "警长"), rates, null, true);
        sheriffCooldown = CustomOption.Create(101, Types.Crewmate, "执法冷却", 30f, 10f, 60f, 2.5f, sheriffSpawnRate);
        sheriffMisfireKills =
            CustomOption.Create(2101, Types.Crewmate, "走火时死亡对象", new[] { "警长", "对方", "双方" }, sheriffSpawnRate);
        sheriffCanKillNeutrals = CustomOption.Create(102, Types.Crewmate, "可执法独立阵营", false, sheriffSpawnRate);
        sheriffCanKillJester = CustomOption.Create(2104, Types.Crewmate, "可执法 " + cs(Jester.color, "小丑"), false,
            sheriffCanKillNeutrals);
        sheriffCanKillProsecutor = CustomOption.Create(2105, Types.Crewmate, "可执法 " + cs(Lawyer.color, "处刑者"), false,
            sheriffCanKillNeutrals);
        sheriffCanKillAmnesiac = CustomOption.Create(210278, Types.Crewmate, "可执法 " + cs(Amnisiac.color, "失忆者"), false,
            sheriffCanKillNeutrals);
        sheriffCanKillArsonist = CustomOption.Create(2102, Types.Crewmate, "可执法 " + cs(Arsonist.color, "纵火犯"), false,
            sheriffCanKillNeutrals);
        sheriffCanKillVulture = CustomOption.Create(2107, Types.Crewmate, "可执法 " + cs(Vulture.color, "秃鹫"), false,
            sheriffCanKillNeutrals);
        sheriffCanKillLawyer = CustomOption.Create(2103, Types.Crewmate, " 可执法 " + cs(Lawyer.color, "律师"), false,
            sheriffCanKillNeutrals);
        sheriffCanKillThief = CustomOption.Create(210277, Types.Crewmate, "可执法 " + cs(Thief.color, "身份窃贼"), false,
            sheriffCanKillNeutrals);
        sheriffCanKillPursuer = CustomOption.Create(2106, Types.Crewmate, "可执法 " + cs(Pursuer.color, "起诉人"), false,
            sheriffCanKillNeutrals);
        sheriffCanKillDoomsayer = CustomOption.Create(2108, Types.Crewmate, "可执法 " + cs(Doomsayer.color, "末日预言家"),
            false, sheriffCanKillNeutrals);

        deputySpawnRate = CustomOption.Create(103, Types.Crewmate, "可拥有一名捕快", rates, sheriffSpawnRate);
        deputyNumberOfHandcuffs = CustomOption.Create(104, Types.Crewmate, "手铐可用次数", 3f, 1f, 10f, 1f, deputySpawnRate);
        deputyHandcuffCooldown = CustomOption.Create(105, Types.Crewmate, "手铐冷却", 30f, 10f, 60f, 2.5f, deputySpawnRate);
        deputyHandcuffDuration =
            CustomOption.Create(106, Types.Crewmate, "手铐持续时间", 15f, 5f, 60f, 2.5f, deputySpawnRate);
        deputyKnowsSheriff = CustomOption.Create(107, Types.Crewmate, "警长/捕快可以互相确认 ", true, deputySpawnRate);
        deputyGetsPromoted = CustomOption.Create(108, Types.Crewmate, "警长死后捕快可晋升", new[] { "否", "立即晋升", "会议后晋升" },
            deputySpawnRate);
        deputyKeepsHandcuffs = CustomOption.Create(109, Types.Crewmate, "晋升后保留手铐技能", true, deputyGetsPromoted);

        lighterSpawnRate = CustomOption.Create(110, Types.Crewmate, cs(Lighter.color, "执灯人"), rates, null, true);
        lighterModeLightsOnVision = CustomOption.Create(111, Types.Crewmate, "灯光正常时点灯状态下的视野倍率", 1.5f, 0.25f, 5f, 0.25f,
            lighterSpawnRate);
        lighterModeLightsOffVision = CustomOption.Create(112, Types.Crewmate, "熄灯时点灯状态下的视野倍率", 0.5f, 0.25f, 5f, 0.25f,
            lighterSpawnRate);
        lighterFlashlightWidth =
            CustomOption.Create(113, Types.Crewmate, "手电筒范围", 0.3f, 0.1f, 1f, 0.1f, lighterSpawnRate);

        detectiveSpawnRate = CustomOption.Create(120, Types.Crewmate, cs(Detective.color, "侦探"), rates, null, true);
        detectiveAnonymousFootprints = CustomOption.Create(121, Types.Crewmate, "匿名脚印", false, detectiveSpawnRate);
        detectiveFootprintIntervall =
            CustomOption.Create(122, Types.Crewmate, "脚印更新间隔", 0.5f, 0.25f, 10f, 0.25f, detectiveSpawnRate);
        detectiveFootprintDuration =
            CustomOption.Create(123, Types.Crewmate, "脚印持续时间", 10f, 0.5f, 30f, 0.5f, detectiveSpawnRate);
        detectiveReportNameDuration =
            CustomOption.Create(124, Types.Crewmate, "以下时间内报告可得知凶手姓名", 5, 0, 60, 2.5f, detectiveSpawnRate);
        detectiveReportColorDuration = CustomOption.Create(125, Types.Crewmate, "以下时间内报告可得知凶手颜色类型", 30, 0, 120, 2.5f,
            detectiveSpawnRate);

        timeMasterSpawnRate = CustomOption.Create(130, Types.Crewmate, cs(TimeMaster.color, "时间之主"), rates, null, true);
        timeMasterCooldown =
            CustomOption.Create(131, Types.Crewmate, "时光之盾冷却", 30f, 10f, 60f, 2.5f, timeMasterSpawnRate);
        timeMasterRewindTime = CustomOption.Create(132, Types.Crewmate, "回溯时间", 3f, 1f, 10f, 1f, timeMasterSpawnRate);
        timeMasterShieldDuration =
            CustomOption.Create(133, Types.Crewmate, "时光之盾持续时间", 10f, 1f, 20f, 1f, timeMasterSpawnRate);

        veterenSpawnRate = CustomOption.Create(4450, Types.Crewmate, cs(Veteren.color, "老兵"), rates, null, true);
        veterenCooldown = CustomOption.Create(4451, Types.Crewmate, "警戒冷却", 30f, 10f, 120f, 2.5f, veterenSpawnRate);
        veterenAlertDuration = CustomOption.Create(4452, Types.Crewmate, "警戒持续时间", 3f, 1f, 20f, 1f, veterenSpawnRate);

        medicSpawnRate = CustomOption.Create(140, Types.Crewmate, cs(Medic.color, "医生"), rates, null, true);
        medicShowShielded = CustomOption.Create(143, Types.Crewmate, "可见医生护盾的玩家", new[] { "所有人", "被保护者+法医", "法医" },
            medicSpawnRate);
        medicBreakShield = CustomOption.Create(1146, Types.Crewmate, "护盾持续生效", true, medicSpawnRate);
        medicShowAttemptToShielded = CustomOption.Create(144, Types.Crewmate, "被保护者可见击杀尝试", false, medicBreakShield);
        medicResetTargetAfterMeeting = CustomOption.Create(1423234, Types.Crewmate, "会议后重置保护目标", false, medicSpawnRate);
        medicSetOrShowShieldAfterMeeting = CustomOption.Create(145, Types.Crewmate, "护盾生效与可见时机",
            new[] { "立即生效且可见", "立即生效且会议后可见", "会议后生效且可见" }, medicSpawnRate);
        medicShowAttemptToMedic = CustomOption.Create(146, Types.Crewmate, "法医可见对被保护者的击杀尝试", false, medicBreakShield);

        swapperSpawnRate = CustomOption.Create(150, Types.Crewmate, cs(Swapper.color, "换票师"), rates, null, true);
        swapperCanCallEmergency = CustomOption.Create(151, Types.Crewmate, "可召开会议", false, swapperSpawnRate);
        swapperCanFixSabotages = CustomOption.Create(1512, Types.Crewmate, "可修理紧急破坏", false, swapperSpawnRate);
        swapperCanOnlySwapOthers = CustomOption.Create(152, Types.Crewmate, "只可交换他人", false, swapperSpawnRate);

        swapperSwapsNumber = CustomOption.Create(153, Types.Crewmate, "初始可换票次数", 1f, 0f, 5f, 1f, swapperSpawnRate);
        swapperRechargeTasksNumber =
            CustomOption.Create(154, Types.Crewmate, "充能所需任务数", 2f, 1f, 10f, 1f, swapperSpawnRate);


        seerSpawnRate = CustomOption.Create(160, Types.Crewmate, cs(Seer.color, "灵媒"), rates, null, true);
        seerMode = CustomOption.Create(161, Types.Crewmate, "感知模式", new[] { "死亡闪光+可见灵魂", "死亡闪光", "可见灵魂" },
            seerSpawnRate);
        seerLimitSoulDuration = CustomOption.Create(163, Types.Crewmate, "限制灵魂可见时间", false, seerSpawnRate);
        seerSoulDuration =
            CustomOption.Create(162, Types.Crewmate, "灵魂可见时间", 15f, 0f, 120f, 2.5f, seerLimitSoulDuration);

        hackerSpawnRate = CustomOption.Create(170, Types.Crewmate, cs(Hacker.color, "黑客"), rates, null, true);
        hackerCooldown = CustomOption.Create(171, Types.Crewmate, "黑入冷却", 30f, 5f, 60f, 2.5f, hackerSpawnRate);
        hackerHackeringDuration =
            CustomOption.Create(172, Types.Crewmate, "黑入持续时间", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate);
        hackerOnlyColorType = CustomOption.Create(173, Types.Crewmate, "黑入后只可见颜色类型", false, hackerSpawnRate);
        hackerToolsNumber = CustomOption.Create(174, Types.Crewmate, "移动设备最大充能次数", 5f, 1f, 30f, 1f, hackerSpawnRate);
        hackerRechargeTasksNumber =
            CustomOption.Create(175, Types.Crewmate, "充能所需任务数", 2f, 1f, 5f, 1f, hackerSpawnRate);
        hackerNoMove = CustomOption.Create(176, Types.Crewmate, "使用移动设备时不可移动", true, hackerSpawnRate);

        trackerSpawnRate = CustomOption.Create(200, Types.Crewmate, cs(Tracker.color, "追踪者"), rates, null, true);
        trackerUpdateIntervall =
            CustomOption.Create(201, Types.Crewmate, "箭头更新间隔", 0.5f, 0f, 30f, 0.5f, trackerSpawnRate);
        trackerResetTargetAfterMeeting =
            CustomOption.Create(202, Types.Crewmate, "会议后重置跟踪目标 ", false, trackerSpawnRate);
        trackerCanTrackCorpses = CustomOption.Create(203, Types.Crewmate, "可寻找尸体", true, trackerSpawnRate);
        trackerCorpsesTrackingCooldown =
            CustomOption.Create(204, Types.Crewmate, "寻找尸体冷却", 20f, 5f, 120f, 2.5f, trackerCanTrackCorpses);
        trackerCorpsesTrackingDuration =
            CustomOption.Create(205, Types.Crewmate, "寻找持续时间", 5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses);

        snitchSpawnRate = CustomOption.Create(210, Types.Crewmate, cs(Snitch.color, "告密者"), rates, null, true);
        snitchLeftTasksForReveal =
            CustomOption.Create(219, Types.Crewmate, "剩余多少任务时可被发现", 1f, 0f, 10f, 1f, snitchSpawnRate);
        snitchMode = CustomOption.Create(211, Types.Crewmate, "信息显示", new[] { "聊天框", "地图", "聊天框+地图" }, snitchSpawnRate);
        snitchTargets = CustomOption.Create(212, Types.Crewmate, "显示目标", new[] { "所有邪恶职业", "杀手职业" }, snitchSpawnRate);

        spySpawnRate = CustomOption.Create(240, Types.Crewmate, cs(Spy.color, "卧底"), rates, null, true);
        spyCanDieToSheriff = CustomOption.Create(241, Types.Crewmate, "可被警长执法", false, spySpawnRate);
        spyImpostorsCanKillAnyone = CustomOption.Create(242, Types.Crewmate, "卧底在场时伪装者可击杀队友", true, spySpawnRate);
        spyCanEnterVents = CustomOption.Create(243, Types.Crewmate, "可使用管道", true, spySpawnRate);
        spyHasImpostorVision = CustomOption.Create(244, Types.Crewmate, "拥有内鬼视野", true, spySpawnRate);

        portalmakerSpawnRate =
            CustomOption.Create(390, Types.Crewmate, cs(Portalmaker.color, "星门缔造者"), rates, null, true);
        portalmakerCooldown =
            CustomOption.Create(391, Types.Crewmate, "构建星门冷却", 20f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerUsePortalCooldown =
            CustomOption.Create(392, Types.Crewmate, "使用星门冷却", 20f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerLogOnlyColorType =
            CustomOption.Create(393, Types.Crewmate, "星门日志只显示颜色类型", true, portalmakerSpawnRate);
        portalmakerLogHasTime = CustomOption.Create(394, Types.Crewmate, "星门日志记录使用时间", true, portalmakerSpawnRate);
        portalmakerCanPortalFromAnywhere =
            CustomOption.Create(395, Types.Crewmate, "可从任何地方传送至自己放置的传送门", true, portalmakerSpawnRate);

        securityGuardSpawnRate =
            CustomOption.Create(280, Types.Crewmate, cs(SecurityGuard.color, "保安"), rates, null, true);
        securityGuardCooldown =
            CustomOption.Create(281, Types.Crewmate, "保安冷却", 20f, 10f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardTotalScrews =
            CustomOption.Create(282, Types.Crewmate, "保安螺丝数", 10f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamPrice =
            CustomOption.Create(283, Types.Crewmate, "监控所需螺丝数", 3f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardVentPrice =
            CustomOption.Create(284, Types.Crewmate, "封锁所需螺丝数", 2f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamDuration =
            CustomOption.Create(285, Types.Crewmate, "保安技能持续时间", 10f, 2.5f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardCamMaxCharges =
            CustomOption.Create(286, Types.Crewmate, "最大充能数", 5f, 1f, 30f, 1f, securityGuardSpawnRate);
        securityGuardCamRechargeTasksNumber =
            CustomOption.Create(287, Types.Crewmate, "充能所需任务数", 3f, 1f, 10f, 1f, securityGuardSpawnRate);
        securityGuardNoMove = CustomOption.Create(288, Types.Crewmate, "看监控时无法移动", true, securityGuardSpawnRate);

        mediumSpawnRate = CustomOption.Create(360, Types.Crewmate, cs(Medium.color, "通灵师"), rates, null, true);
        mediumCooldown = CustomOption.Create(361, Types.Crewmate, "通灵冷却", 15f, 2.5f, 120f, 2.5f, mediumSpawnRate);
        mediumDuration = CustomOption.Create(362, Types.Crewmate, "通灵所需时间", 3f, 0f, 15f, 0.5f, mediumSpawnRate);
        mediumOneTimeUse = CustomOption.Create(363, Types.Crewmate, "每个灵魂只能被通灵一次", false, mediumSpawnRate);
        mediumChanceAdditionalInfo = CustomOption.Create(364, Types.Crewmate, "回答包含其他信息的可能性", rates, mediumSpawnRate);

        jumperSpawnRate = CustomOption.Create(9050, Types.Crewmate, cs(Jumper.color, "传送师"), rates, null, true);
        jumperJumpTime = CustomOption.Create(9051, Types.Crewmate, "传送冷却", 15f, 0f, 60f, 2.5f, jumperSpawnRate);
        jumperChargesOnPlace = CustomOption.Create(9052, Types.Crewmate, "每次传送所消耗点数", 1, 1, 10, 1, jumperSpawnRate);
        //jumperResetPlaceAfterMeeting = CustomOption.Create(9052, Types.Crewmate, "Reset Places After Meeting", true, jumperSpawnRate);
        //jumperChargesGainOnMeeting = CustomOption.Create(9053, Types.Crewmate, "Charges Gained After Meeting", 2, 0, 10, 1, jumperSpawnRate);
        //jumperMaxCharges = CustomOption.Create(9054, Types.Crewmate, "Maximum Charges", 3, 0, 10, 1, jumperSpawnRate);

        bodyGuardSpawnRate = CustomOption.Create(8820, Types.Crewmate, cs(BodyGuard.color, "保镖"), rates, null, true);
        bodyGuardResetTargetAfterMeeting =
            CustomOption.Create(8821, Types.Crewmate, "会议后重置保护目标", true, bodyGuardSpawnRate);
        bodyGuardFlash = CustomOption.Create(8822, Types.Crewmate, "死亡闪光", true, bodyGuardSpawnRate);

        thiefSpawnRate = CustomOption.Create(400, Types.Neutral, cs(Thief.color, "身份窃贼"), rates, null, true);
        thiefCooldown = CustomOption.Create(401, Types.Neutral, "窃取冷却", 30f, 5f, 120f, 2.5f, thiefSpawnRate);
        thiefCanKillSheriff = CustomOption.Create(402, Types.Neutral, "身份窃贼可以击杀警长", true, thiefSpawnRate);
        thiefHasImpVision = CustomOption.Create(403, Types.Neutral, "身份窃贼拥有伪装者视野", true, thiefSpawnRate);
        thiefCanUseVents = CustomOption.Create(404, Types.Neutral, "身份窃贼可以使用管道", true, thiefSpawnRate);
        thiefCanStealWithGuess =
            CustomOption.Create(405, Types.Neutral, "身份窃贼可通过猜测窃取身份\n(赌怪模式)", false, thiefSpawnRate);

        /*
        public static CustomOption doomsayerSpawnRate;
        public static CustomOption doomsayerCooldown;
        public static CustomOption doomsayerHasMultipleShotsPerMeeting;
        public static CustomOption doomsayerShowInfoInGhostChat;
        public static CustomOption doomsayerCanGuessNeutral;
        public static CustomOption doomsayerCanGuessImpostor;
        public static CustomOption doomsayerGuesserCantGuessSnitch;
        public static CustomOption doomsayerkillToWin;

        guesserSpawnRate = CustomOption.Create(310, Types.Crewmate, cs(Guesser.color, "赌怪"), rates, null, true);
        guesserNumberOfShots = CustomOption.Create(311, Types.Crewmate, "可猜测次数", 5f, 1f, 15f, 1f, guesserSpawnRate);
        guesserHasMultipleShotsPerMeeting = CustomOption.Create(312, Types.Crewmate, "同一轮会议可多次猜测", true, guesserSpawnRate);
        guesserShowInfoInGhostChat = CustomOption.Create(313, Types.Crewmate, "灵魂可见猜测结果", true, guesserSpawnRate);
        guesserKillsThroughShield = CustomOption.Create(314, Types.Crewmate, "猜测无视法医护盾保护", false, guesserSpawnRate);
        */
        trapperSpawnRate = CustomOption.Create(410, Types.Crewmate, cs(Trapper.color, "设陷师"), rates, null, true);
        trapperCooldown = CustomOption.Create(420, Types.Crewmate, "放置冷却", 20f, 5f, 120f, 2.5f, trapperSpawnRate);
        trapperMaxCharges = CustomOption.Create(440, Types.Crewmate, "最大陷阱数", 3f, 1f, 15f, 1f, trapperSpawnRate);
        trapperRechargeTasksNumber =
            CustomOption.Create(450, Types.Crewmate, "充能所需任务数", 2f, 1f, 15f, 1f, trapperSpawnRate);
        trapperTrapNeededTriggerToReveal =
            CustomOption.Create(451, Types.Crewmate, "陷阱触发提示所需人数", 2f, 1f, 10f, 1f, trapperSpawnRate);
        trapperAnonymousMap = CustomOption.Create(452, Types.Crewmate, "显示匿名地图", false, trapperSpawnRate);
        trapperInfoType =
            CustomOption.Create(453, Types.Crewmate, "陷阱信息类型", new[] { "职业", "善良/邪恶", "名字" }, trapperSpawnRate);
        trapperTrapDuration = CustomOption.Create(454, Types.Crewmate, "陷阱定身时间", 5f, 1f, 15f, 0.5f, trapperSpawnRate);

        // Modifier (1000 - 1999)
        modifiersAreHidden =
            CustomOption.Create(1009, Types.Modifier, cs(Color.yellow, "隐藏死亡触发的附加职业"), true, null, true);

        modifierDisperser = CustomOption.Create(200220, Types.Modifier, cs(Color.red, "分散者"), rates, null, true);

        modifierBloody = CustomOption.Create(1000, Types.Modifier, cs(Color.yellow, "溅血者"), rates, null, true);
        modifierBloodyQuantity =
            CustomOption.Create(1001, Types.Modifier, cs(Color.yellow, "溅血数量"), ratesModifier, modifierBloody);
        modifierBloodyDuration = CustomOption.Create(1002, Types.Modifier, "痕迹持续时间", 10f, 3f, 60f, 1f, modifierBloody);

        modifierAntiTeleport = CustomOption.Create(1010, Types.Modifier, cs(Color.yellow, "通讯兵"), rates, null, true);
        modifierAntiTeleportQuantity = CustomOption.Create(1011, Types.Modifier, cs(Color.yellow, "通讯兵数量"),
            ratesModifier, modifierAntiTeleport);

        modifierTieBreaker = CustomOption.Create(1020, Types.Modifier, cs(Color.yellow, "破平者"), rates, null, true);

        modifierBait = CustomOption.Create(1030, Types.Modifier, cs(Color.yellow, "诱饵"), rates, null, true);
        modifierBaitQuantity =
            CustomOption.Create(1031, Types.Modifier, cs(Color.yellow, "诱饵数量"), ratesModifier, modifierBait);
        modifierBaitReportDelayMin =
            CustomOption.Create(1032, Types.Modifier, "诱饵报告延迟时间(最小)", 0f, 0f, 10f, 1f, modifierBait);
        modifierBaitReportDelayMax =
            CustomOption.Create(1033, Types.Modifier, "诱饵报告延迟时间(最大)", 0f, 0f, 10f, 1f, modifierBait);
        modifierBaitShowKillFlash = CustomOption.Create(1034, Types.Modifier, "用闪光灯警告杀手", true, modifierBait);

        modifierLover = CustomOption.Create(1040, Types.Modifier, cs(Color.yellow, "恋人"), rates, null, true);
        modifierLoverImpLoverRate = CustomOption.Create(1041, Types.Modifier, "恋人中有内鬼的概率", rates, modifierLover);
        modifierLoverBothDie = CustomOption.Create(1042, Types.Modifier, "恋人共死", true, modifierLover);
        modifierLoverEnableChat = CustomOption.Create(1043, Types.Modifier, "启用私密聊天文字频道", true, modifierLover);

        modifierSunglasses = CustomOption.Create(1050, Types.Modifier, cs(Color.yellow, "太阳镜"), rates, null, true);
        modifierSunglassesQuantity = CustomOption.Create(1051, Types.Modifier, cs(Color.yellow, "太阳镜数量"), ratesModifier,
            modifierSunglasses);
        modifierSunglassesVision = CustomOption.Create(1052, Types.Modifier, "太阳镜的视野倍率",
            new[] { "-10%", "-20%", "-30%", "-40%", "-50%" }, modifierSunglasses);

        modifierTorch = CustomOption.Create(1053, Types.Modifier, cs(Color.yellow, "火炬"), rates, null, true);
        modifierTorchQuantity =
            CustomOption.Create(1054, Types.Modifier, cs(Color.yellow, "火炬人数"), ratesModifier, modifierTorch);
        modifierTorchVision = CustomOption.Create(1056, Types.Modifier, "火炬的视野倍率",
            new[] { "+10%", "+20%", "+30%", "+40%", "+50%" }, modifierTorch);

        modifierMultitasker = CustomOption.Create(10523233, Types.Modifier, cs(Color.yellow, "多线程"), rates, null, true);
        modifierMultitaskerQuantity = CustomOption.Create(10232354, Types.Modifier, cs(Color.yellow, "多线程人数"),
            ratesModifier, modifierMultitasker);

        modifierMini = CustomOption.Create(1061, Types.Modifier, cs(Color.yellow, "小孩"), rates, null, true);
        modifierMiniGrowingUpDuration =
            CustomOption.Create(1062, Types.Modifier, "小孩长大所需时间", 400f, 100f, 1500f, 25f, modifierMini);
        modifierMiniGrowingUpInMeeting = CustomOption.Create(1063, Types.Modifier, "小孩会议期间可成长", true, modifierMini);

        modifierIndomitable = CustomOption.Create(1276, Types.Modifier, cs(Color.yellow, "不屈者"), rates, null, true);

        modifierBlind = CustomOption.Create(8810, Types.Modifier, cs(Color.yellow, "胆小鬼"), rates, null, true);

        modifierWatcher = CustomOption.Create(10401, Types.Modifier, cs(Color.yellow, "窥视者"), rates, null, true);

        modifierRadar = CustomOption.Create(1040122, Types.Modifier, cs(Color.yellow, "雷达"), rates, null, true);

        modifierTunneler = CustomOption.Create(8819, Types.Modifier, cs(Color.yellow, "管道工程师"), rates, null, true);

        modifierSlueth = CustomOption.Create(8830, Types.Modifier, cs(Color.yellow, "掘墓人"), rates, null, true);

        modifierCursed = CustomOption.Create(1277, Types.Modifier, cs(Color.yellow, "反骨"), rates, null, true);

        modifierVip = CustomOption.Create(1070, Types.Modifier, cs(Color.yellow, "VIP"), rates, null, true);
        modifierVipQuantity =
            CustomOption.Create(1071, Types.Modifier, cs(Color.yellow, "VIP人数"), ratesModifier, modifierVip);
        modifierVipShowColor = CustomOption.Create(1072, Types.Modifier, "死亡时全场提示阵营颜色", true, modifierVip);

        modifierInvert = CustomOption.Create(1080, Types.Modifier, cs(Color.yellow, "酒鬼"), rates, null, true);
        modifierInvertQuantity =
            CustomOption.Create(1081, Types.Modifier, cs(Color.yellow, "酒鬼人数"), ratesModifier, modifierInvert);
        modifierInvertDuration =
            CustomOption.Create(1082, Types.Modifier, "醉酒状态持续几轮会议", 3f, 1f, 15f, 1f, modifierInvert);

        modifierChameleon = CustomOption.Create(1090, Types.Modifier, cs(Color.yellow, "变色龙"), rates, null, true);
        modifierChameleonQuantity = CustomOption.Create(1091, Types.Modifier, cs(Color.yellow, "变色龙数量"), ratesModifier,
            modifierChameleon);
        modifierChameleonHoldDuration =
            CustomOption.Create(1092, Types.Modifier, "从不动到褪色开始的间隔时间", 3f, 1f, 10f, 0.5f, modifierChameleon);
        modifierChameleonFadeDuration =
            CustomOption.Create(1093, Types.Modifier, "褪色过程持续时间", 1f, 0.25f, 10f, 0.25f, modifierChameleon);
        modifierChameleonMinVisibility = CustomOption.Create(1094, Types.Modifier, "最低透明度",
            new[] { "0%", "10%", "20%", "30%", "40%", "50%" }, modifierChameleon);

        modifierShifter = CustomOption.Create(1100, Types.Modifier, cs(Color.yellow, "交换师"), rates, null, true);

        // Guesser Gamemode (2000 - 2999)
        guesserGamemodeCrewNumber = CustomOption.Create(2001, Types.Guesser, cs(Guesser.color, "船员阵营赌怪数"), 15f, 1f, 15f,
            1f, null, true);
        guesserGamemodeNeutralNumber = CustomOption.Create(2002, Types.Guesser, cs(Guesser.color, "中立阵营赌怪数"), 15f, 1f,
            15f, 1f, null, true);
        guesserGamemodeImpNumber = CustomOption.Create(2003, Types.Guesser, cs(Guesser.color, "伪装者阵营赌怪数"), 15f, 1f, 15f,
            1f, null, true);
        guesserForceJackalGuesser = CustomOption.Create(2007, Types.Guesser, "强制豺狼成为赌怪", false, null, true);
        guesserForceThiefGuesser = CustomOption.Create(2011, Types.Guesser, "强制身份窃贼为赌怪", false, null, true);
        guesserGamemodeHaveModifier = CustomOption.Create(2004, Types.Guesser, "赌怪可以拥有附加职业", true);
        guesserGamemodeNumberOfShots = CustomOption.Create(2005, Types.Guesser, "赌怪猜测最大次数", 3f, 1f, 15f, 1f);
        guesserGamemodeHasMultipleShotsPerMeeting = CustomOption.Create(2006, Types.Guesser, "一轮会议可多次猜测", false);
        guesserGamemodeKillsThroughShield = CustomOption.Create(2008, Types.Guesser, "赌怪猜测无视护盾", true);
        guesserGamemodeEvilCanKillSpy = CustomOption.Create(2009, Types.Guesser, "邪恶的赌怪可猜测卧底", true);
        guesserGamemodeCantGuessSnitchIfTaksDone = CustomOption.Create(2010, Types.Guesser, "赌怪不可猜测已完成任务的告密者", true);

        // Hide N Seek Gamemode (3000 - 3999)
        hideNSeekMap = CustomOption.Create(3020, Types.HideNSeekMain, cs(Color.yellow, "地图"),
            new[] { "骷髅舰", "米拉总部", "波鲁斯", "飞艇", "真菌丛林", "潜艇", "自定义地图" }, null, true, () =>
            {
                var map = hideNSeekMap.selection;
                if (map >= 3) map++;
                GameOptionsManager.Instance.currentNormalGameOptions.MapId = (byte)map;
            });
        hideNSeekHunterCount = CustomOption.Create(3000, Types.HideNSeekMain, cs(Color.yellow, "猎人数量"), 1f, 1f, 3f, 1f);
        hideNSeekKillCooldown =
            CustomOption.Create(3021, Types.HideNSeekMain, cs(Color.yellow, "击杀冷却"), 10f, 2.5f, 60f, 2.5f);
        hideNSeekHunterVision =
            CustomOption.Create(3001, Types.HideNSeekMain, cs(Color.yellow, "猎人视野"), 0.5f, 0.25f, 2f, 0.25f);
        hideNSeekHuntedVision =
            CustomOption.Create(3002, Types.HideNSeekMain, cs(Color.yellow, "猎物视野"), 2f, 0.25f, 5f, 0.25f);
        hideNSeekCommonTasks = CustomOption.Create(3023, Types.HideNSeekMain, cs(Color.yellow, "普通任务"), 1f, 0f, 4f, 1f);
        hideNSeekShortTasks = CustomOption.Create(3024, Types.HideNSeekMain, cs(Color.yellow, "短任务"), 3f, 1f, 23f, 1f);
        hideNSeekLongTasks = CustomOption.Create(3025, Types.HideNSeekMain, cs(Color.yellow, "长任务"), 3f, 0f, 15f, 1f);
        hideNSeekTimer = CustomOption.Create(3003, Types.HideNSeekMain, cs(Color.yellow, "最少躲藏时间"), 5f, 1f, 30f, 0.5f);
        hideNSeekTaskWin = CustomOption.Create(3004, Types.HideNSeekMain, cs(Color.yellow, "可以任务获胜"), false);
        hideNSeekTaskPunish =
            CustomOption.Create(3017, Types.HideNSeekMain, cs(Color.yellow, "完成任务减少躲藏时间"), 10f, 0f, 30f, 1f);
        hideNSeekCanSabotage = CustomOption.Create(3019, Types.HideNSeekMain, cs(Color.yellow, "启用破坏"), false);
        hideNSeekHunterWaiting =
            CustomOption.Create(3022, Types.HideNSeekMain, cs(Color.yellow, "猎人等待入场时间"), 15f, 2.5f, 60f, 2.5f);

        hunterLightCooldown = CustomOption.Create(3005, Types.HideNSeekRoles, cs(Color.red, "猎人电灯冷却"), 30f, 5f, 60f, 1f,
            null, true);
        hunterLightDuration =
            CustomOption.Create(3006, Types.HideNSeekRoles, cs(Color.red, "猎人电灯持续时间"), 10f, 1f, 60f, 1f);
        hunterLightVision = CustomOption.Create(3007, Types.HideNSeekRoles, cs(Color.red, "猎人电灯视野"), 2f, 1f, 5f, 0.25f);
        hunterLightPunish =
            CustomOption.Create(3008, Types.HideNSeekRoles, cs(Color.red, "猎人电灯惩罚躲藏时间"), 5f, 0f, 30f, 1f);
        hunterAdminCooldown =
            CustomOption.Create(3009, Types.HideNSeekRoles, cs(Color.red, "猎人管理地图冷却"), 30f, 5f, 60f, 1f);
        hunterAdminDuration =
            CustomOption.Create(3010, Types.HideNSeekRoles, cs(Color.red, "猎人管理地图持续时间"), 5f, 1f, 60f, 1f);
        hunterAdminPunish =
            CustomOption.Create(3011, Types.HideNSeekRoles, cs(Color.red, "猎人管理地图惩罚躲藏时间"), 5f, 0f, 30f, 1f);
        hunterArrowCooldown =
            CustomOption.Create(3012, Types.HideNSeekRoles, cs(Color.red, "猎人追踪冷却时间"), 30f, 5f, 60f, 1f);
        hunterArrowDuration =
            CustomOption.Create(3013, Types.HideNSeekRoles, cs(Color.red, "猎人追踪持续时间"), 5f, 0f, 60f, 1f);
        hunterArrowPunish =
            CustomOption.Create(3014, Types.HideNSeekRoles, cs(Color.red, "猎人追踪惩罚躲藏时间"), 5f, 0f, 30f, 1f);

        huntedShieldCooldown = CustomOption.Create(3015, Types.HideNSeekRoles, cs(Color.gray, "躲藏者护盾冷却时间"), 30f, 5f,
            60f, 1f, null, true);
        huntedShieldDuration =
            CustomOption.Create(3016, Types.HideNSeekRoles, cs(Color.gray, "躲藏者护盾持续时间"), 5f, 1f, 60f, 1f);
        huntedShieldRewindTime =
            CustomOption.Create(3018, Types.HideNSeekRoles, cs(Color.gray, "躲藏者回溯时间"), 3f, 1f, 10f, 1f);
        huntedShieldNumber =
            CustomOption.Create(3026, Types.HideNSeekRoles, cs(Color.gray, "躲藏者护盾数量"), 3f, 1f, 15f, 1f);

        // Prop Hunt General Options
        propHuntMap = CustomOption.Create(4020, Types.PropHunt, cs(Color.yellow, "地图"),
            new[] { "骷髅舰", "米拉总部", "波鲁斯", "飞艇", "蘑菇岛", "潜艇", "自定义地图" }, null, true, () =>
            {
                var map = propHuntMap.selection;
                if (map >= 3) map++;
                GameOptionsManager.Instance.currentNormalGameOptions.MapId = (byte)map;
            });
        propHuntTimer = CustomOption.Create(4021, Types.PropHunt, cs(Color.yellow, "最少躲藏时间"), 5f, 1f, 30f, 0.5f);
        propHuntUnstuckCooldown =
            CustomOption.Create(4011, Types.PropHunt, cs(Color.yellow, "穿墙冷却时间"), 30f, 2.5f, 60f, 2.5f);
        propHuntUnstuckDuration =
            CustomOption.Create(4012, Types.PropHunt, cs(Color.yellow, "穿墙持续时间"), 2f, 1f, 60f, 1f);
        propHunterVision = CustomOption.Create(4006, Types.PropHunt, cs(Color.yellow, "猎人视野"), 0.5f, 0.25f, 2f, 0.25f);
        propVision = CustomOption.Create(4007, Types.PropHunt, cs(Color.yellow, "躲藏者视野"), 2f, 0.25f, 5f, 0.25f);
        // Hunter Options
        propHuntNumberOfHunters =
            CustomOption.Create(4000, Types.PropHunt, cs(Color.red, "猎人数量"), 1f, 1f, 5f, 1f, null, true);
        hunterInitialBlackoutTime =
            CustomOption.Create(4001, Types.PropHunt, cs(Color.red, "猎人等待入场时间"), 10f, 5f, 20f, 1f);
        hunterMissCooldown = CustomOption.Create(4004, Types.PropHunt, cs(Color.red, "错误击杀后的冷却"), 10f, 2.5f, 60f, 2.5f);
        hunterHitCooldown = CustomOption.Create(4005, Types.PropHunt, cs(Color.red, "击杀后的冷却"), 10f, 2.5f, 60f, 2.5f);
        propHuntRevealCooldown =
            CustomOption.Create(4008, Types.PropHunt, cs(Color.red, "变形冷却时间"), 30f, 10f, 90f, 2.5f);
        propHuntRevealDuration = CustomOption.Create(4009, Types.PropHunt, cs(Color.red, "变形持续时间"), 5f, 1f, 60f, 1f);
        propHuntRevealPunish = CustomOption.Create(4010, Types.PropHunt, cs(Color.red, "揭示惩罚时间"), 10f, 0f, 1800f, 5f);
        propHuntAdminCooldown =
            CustomOption.Create(4022, Types.PropHunt, cs(Color.red, "猎人查看管理地图冷却时间"), 30f, 2.5f, 1800f, 2.5f);
        propHuntFindCooldown =
            CustomOption.Create(4023, Types.PropHunt, cs(Color.red, "寻找冷却时间"), 60f, 2.5f, 1800f, 2.5f);
        propHuntFindDuration = CustomOption.Create(4024, Types.PropHunt, cs(Color.red, "寻找持续时间"), 5f, 1f, 15f, 1f);
        // Prop Options
        propBecomesHunterWhenFound = CustomOption.Create(4003, Types.PropHunt, cs(Palette.CrewmateBlue, "猎物被发现后转化为猎人"),
            false, null, true);
        propHuntInvisEnabled =
            CustomOption.Create(4013, Types.PropHunt, cs(Palette.CrewmateBlue, "启用隐形"), true, null, true);
        propHuntInvisCooldown = CustomOption.Create(4014, Types.PropHunt, cs(Palette.CrewmateBlue, "隐形冷却时间"), 40f, 10f,
            120f, 2.5f, propHuntInvisEnabled);
        propHuntInvisDuration = CustomOption.Create(4015, Types.PropHunt, cs(Palette.CrewmateBlue, "隐形持续时间"), 5f, 2.5f,
            30f, 2.5f, propHuntInvisEnabled);
        propHuntSpeedboostEnabled =
            CustomOption.Create(4016, Types.PropHunt, cs(Palette.CrewmateBlue, "启用疾跑"), true, null, true);
        propHuntSpeedboostCooldown = CustomOption.Create(4017, Types.PropHunt, cs(Palette.CrewmateBlue, "疾跑冷却时间"), 45f,
            2.5f, 120f, 2.5f, propHuntSpeedboostEnabled);
        propHuntSpeedboostDuration = CustomOption.Create(4018, Types.PropHunt, cs(Palette.CrewmateBlue, "疾跑持续时间"), 10f,
            2.5f, 30f, 2.5f, propHuntSpeedboostEnabled);
        propHuntSpeedboostSpeed = CustomOption.Create(4019, Types.PropHunt, cs(Palette.CrewmateBlue, "疾跑提升速度"), 2f,
            1.25f, 5f, 0.25f, propHuntSpeedboostEnabled);


        // Other options
        maxNumberOfMeetings = CustomOption.Create(3, Types.General, "会议总次数(不计入市长会议次数)", 10, 0, 15, 1, null, true);
        blockSkippingInEmergencyMeetings = CustomOption.Create(4, Types.General, "会议禁止跳过", false);
        noVoteIsSelfVote = CustomOption.Create(5, Types.General, "不投票默认投自己", false, blockSkippingInEmergencyMeetings);
        hidePlayerNames = CustomOption.Create(6, Types.General, "隐藏玩家名字", false);
        allowParallelMedBayScans = CustomOption.Create(7, Types.General, "允许同时进行扫描任务", false);
        shieldFirstKill = CustomOption.Create(8, Types.General, "首刀保护", false);
        hideOutOfSightNametags = CustomOption.Create(6006, Types.General, "隐藏受阻碍的玩家名称", false);
        hideVentAnimOnShadows = CustomOption.Create(822445, Types.General, "隐藏视野外管道动画", false);
        finishTasksBeforeHauntingOrZoomingOut = CustomOption.Create(9, Types.General, "未完成所有任务前不能使用跟随及千里眼", true);
        camsNightVision = CustomOption.Create(11, Types.General, "熄灯时监控开启夜视模式", false, null, true);
        camsNoNightVisionIfImpVision = CustomOption.Create(12, Types.General, "内鬼无视监控的夜视模式", false, camsNightVision);
        impostorSeeRoles = CustomOption.Create(9, Types.General, "内鬼可见队友职业", false);
        transparentTasks = CustomOption.Create(814142, Types.General, "任务界面透明", false);
        dynamicMap = CustomOption.Create(500, Types.General, "随机地图", false, null, true);
        dynamicMapEnableSkeld = CustomOption.Create(501, Types.General, "Skeld", rates, dynamicMap);
        dynamicMapEnableMira = CustomOption.Create(502, Types.General, "Mira", rates, dynamicMap);
        dynamicMapEnablePolus = CustomOption.Create(503, Types.General, "Polus", rates, dynamicMap);
        dynamicMapEnableAirShip = CustomOption.Create(504, Types.General, "Airship", rates, dynamicMap);
        dynamicMapEnableFungle = CustomOption.Create(506, Types.General, "Fungle", rates, dynamicMap);
        dynamicMapEnableSubmerged = CustomOption.Create(505, Types.General, "Submerged", rates, dynamicMap);
        dynamicMapSeparateSettings = CustomOption.Create(509, Types.General, "使用随机地图设置预设", false, dynamicMap);

        enableBetterPolus =
            CustomOption.Create(7878, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "更好的Polus地图"), false);
        movePolusVents = CustomOption.Create(7879, Types.General, "改变管道布局", false, enableBetterPolus);
        addPolusVents = CustomOption.Create(7883, Types.General, "添加新管道\n 样本室-办公室-武器室", false, enableBetterPolus);
        movePolusVitals = CustomOption.Create(7880, Types.General, "将生命检测仪移动到实验室", false, enableBetterPolus);
        swapNavWifi = CustomOption.Create(7881, Types.General, "重启WIFI与导航任务位置交换", false, enableBetterPolus);
        moveColdTemp = CustomOption.Create(7882, Types.General, "温度调节任务移动至配电室下方", false, enableBetterPolus);

        enableAirShipModify =
            CustomOption.Create(7895, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "更好的AirShip地图"), false);
        addAirShipVents = CustomOption.Create(7896, Types.General, "添加新管道\n 会议室-配电室", false, enableAirShipModify);

        enableCamoComms = CustomOption.Create(1105, Types.General, cs(Color.red, "通信破坏开启小黑人"), false);
        disableMedbayWalk = CustomOption.Create(8847, Types.General, "任务动画不可见", false);
        restrictDevices = CustomOption.Create(1101, Types.General, "限制信息设备使用", new[] { "否", "每一回合", "每局游戏" });
        //restrictAdmin = CustomOption.Create(1102, Types.General, "Restrict Admin Table", 30f, 0f, 600f, 5f, restrictDevices);
        restrictCameras = CustomOption.Create(1103, Types.General, "限制监控观看", 30f, 0f, 600f, 5f, restrictDevices);
        restrictVents = CustomOption.Create(1104, Types.General, "限制心电图观看", 30f, 0f, 600f, 5f, restrictDevices);
        disableCamsRound1 = CustomOption.Create(8834, Types.General, "第一回合无法看监控", false);
        showButtonTarget = CustomOption.Create(9994, Types.General, "技能按钮显示目标", true);
        blockGameEnd = CustomOption.Create(9995, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "强力职业在场不结束游戏"), false);
        randomGameStartPosition = CustomOption.Create(9041, Types.General, "随机出生点", false);
        allowModGuess = CustomOption.Create(9043, Types.General, "允许猜测部分附加职业", false);


        blockedRolePairings.Add((byte)RoleId.Vampire, new[] { (byte)RoleId.Warlock });
        blockedRolePairings.Add((byte)RoleId.Warlock, new[] { (byte)RoleId.Vampire });
        blockedRolePairings.Add((byte)RoleId.Spy, new[] { (byte)RoleId.Mini });
        blockedRolePairings.Add((byte)RoleId.Mini, new[] { (byte)RoleId.Spy });
        blockedRolePairings.Add((byte)RoleId.Vulture, new[] { (byte)RoleId.Cleaner });
        blockedRolePairings.Add((byte)RoleId.Cleaner, new[] { (byte)RoleId.Vulture });

        blockedRolePairings.Add((byte)RoleId.Mayor, new[] { (byte)RoleId.Watcher });
        blockedRolePairings.Add((byte)RoleId.Watcher, new[] { (byte)RoleId.Mayor });
        blockedRolePairings.Add((byte)RoleId.Engineer, new[] { (byte)RoleId.Tunneler });
        blockedRolePairings.Add((byte)RoleId.Tunneler, new[] { (byte)RoleId.Engineer });
        blockedRolePairings.Add((byte)RoleId.Bomber2, new[] { (byte)RoleId.Bait });
        blockedRolePairings.Add((byte)RoleId.Bait, new[] { (byte)RoleId.Bomber2 });
    }
}