using Sentry.Unity.NativeUtils;
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
        { "预设 1", "预设 2", "Skeld预设", "Mira预设", "Polus预设", "Airship预设", "Fungle预设", "Submerged预设" };

    public static CustomOption presetSelection;
    public static CustomOption activateRoles;
    public static CustomOption crewmateRolesCountMin;
    public static CustomOption crewmateRolesCountMax;
    public static CustomOption crewmateRolesFill;
    public static CustomOption neutralRolesCountMin;
    public static CustomOption neutralRolesCountMax;
    public static CustomOption impostorRolesCountMin;
    public static CustomOption impostorRolesCountMax;
    public static CustomOption modifiersCountMin;
    public static CustomOption modifiersCountMax;

    public static CustomOption anyPlayerCanStopStart;
    public static CustomOption enableEventMode;

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
    public static CustomOption undertakerDragingAfterVelocity;
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
    public static CustomOption guesserNumberOfShots;
    public static CustomOption guesserHasMultipleShotsPerMeeting;
    public static CustomOption guesserShowInfoInGhostChat;
    public static CustomOption guesserKillsThroughShield;
    public static CustomOption guesserEvilCanKillSpy;
    public static CustomOption guesserEvilCanKillCrewmate;
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
    public static CustomOption jackalCreateSidekickCooldown;
    public static CustomOption jackalKillFakeImpostor;
    public static CustomOption jackalCanUseVents;
    public static CustomOption jackalCanUseSabo;
    public static CustomOption jackalCanCreateSidekick;
    public static CustomOption jackalCanImpostorFindSidekick;
    public static CustomOption sidekickPromotesToJackal;
    public static CustomOption sidekickCanKill;
    public static CustomOption sidekickCanUseVents;
    public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;
    public static CustomOption jackalCanCreateSidekickFromImpostor;
    public static CustomOption jackalAndSidekickHaveImpostorVision;


    public static CustomOption swooperSpawnRate;
    public static CustomOption swooperKillCooldown;
    public static CustomOption swooperCooldown;
    public static CustomOption swooperDuration;
    public static CustomOption swooperHasImpVision;

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
    //public static CustomOption detectiveReportRoleDuration;
    //public static CustomOption detectiveReportInfoDuration;

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
    public static CustomOption medicReportNameDuration;
    public static CustomOption medicReportColorDuration;

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
    public static CustomOption snitchSeeMeeting;
    //public static CustomOption snitchCanSeeRoles;
    public static CustomOption snitchIncludeTeamJackal;
    public static CustomOption snitchTeamJackalUseDifferentArrowColor;
    //public static CustomOption snitchMode;
    //public static CustomOption snitchTargets;

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
    //public static CustomOption jumperChargesGainOnMeeting;
    public static CustomOption jumperMaxCharges;
    /*
    public static CustomOption ArcanistSpawnRate;
    public static CustomOption ArcanistCooldown;
    public static CustomOption ArcanistTeleportTime;
    public static CustomOption ArcanistProbabilityBlueCards;
    public static CustomOption ArcanistProbabilityRedCards;
    public static CustomOption ArcanistProbabilityPurpleCards;
    */
    public static CustomOption escapistSpawnRate;
    public static CustomOption escapistEscapeTime;
    public static CustomOption escapistChargesOnPlace;
    public static CustomOption escapistResetPlaceAfterMeeting;
    //public static CustomOption escapistChargesGainOnMeeting;
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

    public static CustomOption juggernautSpawnRate;
    public static CustomOption juggernautCooldown;
    public static CustomOption juggernautHasImpVision;
    public static CustomOption juggernautReducedkillEach;

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

    public static CustomOption akujoSpawnRate;
    public static CustomOption akujoTimeLimit;
    public static CustomOption akujoKnowsRoles;
    public static CustomOption akujoNumKeeps;
    public static CustomOption akujoHonmeiCannotFollowWin;

    public static CustomOption trapperSpawnRate;
    public static CustomOption trapperCooldown;
    public static CustomOption trapperMaxCharges;
    public static CustomOption trapperRechargeTasksNumber;
    public static CustomOption trapperTrapNeededTriggerToReveal;
    public static CustomOption trapperAnonymousMap;
    public static CustomOption trapperInfoType;
    public static CustomOption trapperTrapDuration;

    public static CustomOption prophetSpawnRate;
    public static CustomOption prophetCooldown;
    public static CustomOption prophetNumExamines;
    public static CustomOption prophetKillCrewAsRed;
    public static CustomOption prophetBenignNeutralAsRed;
    public static CustomOption prophetEvilNeutralAsRed;
    public static CustomOption prophetKillNeutralAsRed;
    public static CustomOption prophetCanCallEmergency;
    public static CustomOption prophetIsRevealed;
    public static CustomOption prophetExaminesToBeRevealed;

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
    public static CustomOption modifierBaitReportDelayMin;
    public static CustomOption modifierBaitReportDelayMax;
    public static CustomOption modifierBaitShowKillFlash;
    public static CustomOption modifierBaitSwapCrewmate;
    //public static CustomOption modifierBaitSwapNeutral;
    //public static CustomOption modifierBaitSwapImpostor;

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

    public static CustomOption modifierFlash;
    public static CustomOption modifierFlashQuantity;
    public static CustomOption modifierFlashSpeed;

    public static CustomOption modifierMultitasker;
    public static CustomOption modifierMultitaskerQuantity;

    public static CustomOption modifierDisperser;
    //public static CustomOption modifierDisperserRemainingDisperses;
    public static CustomOption modifierDisperserDispersesToVent;

    public static CustomOption modifierMini;
    public static CustomOption modifierMiniGrowingUpDuration;
    public static CustomOption modifierMiniGrowingUpInMeeting;

    public static CustomOption modifierIndomitable;

    public static CustomOption modifierBlind;

    public static CustomOption modifierTunneler;

    public static CustomOption modifierButtonBarry;
    public static CustomOption modifierButtonTaskRemoteMeetings;

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
    public static CustomOption modifierShiftNeutral;

    public static CustomOption modifierLastImpostor;
    public static CustomOption modifierLastImpostorDeduce;

    public static CustomOption resteButtonCooldown;
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

    public static CustomOption enableMiraModify;
    public static CustomOption miraVitals;

    public static CustomOption enableAirShipModify;
    public static CustomOption airshipOptimize;
    public static CustomOption airshipLadder;
    public static CustomOption addAirShipVents;

    public static CustomOption enableFungleModify;
    public static CustomOption fungleElectrical;

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
    public static CustomOption guesserGamemodeSidekickIsAlwaysGuesser;
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
        presetSelection = CustomOption.Create(0, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "预设"), presets, null, true);
        activateRoles = CustomOption.Create(1, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "启用模组职业并禁用原版职业"), true, null, true);

        anyPlayerCanStopStart = CustomOption.Create(3, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "任何玩家都可以阻止游戏开始"), false, null, false);

        if (Utilities.EventUtility.canBeEnabled) enableEventMode = CustomOption.Create(4, Types.General, cs(Color.green, "启用特殊模式"), true, null, true);

        // Using new id's for the options to not break compatibilty with older versions
        crewmateRolesCountMin = CustomOption.Create(5, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最小船员阵营职业数"), 15f, 0f, 30f, 1f, null, true);
        crewmateRolesCountMax = CustomOption.Create(6, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最大船员阵营职业数"), 15f, 0f, 30f, 1f);
        crewmateRolesFill = CustomOption.Create(7, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "所有船员必定拥有职业\n(无视最小/最大数量)"), false);
        neutralRolesCountMin = CustomOption.Create(8, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最小独立阵营职业数"), 2f, 0f, 15f, 1f);
        neutralRolesCountMax = CustomOption.Create(9, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最大独立阵营职业数"), 2f, 0f, 15f, 1f);
        impostorRolesCountMin = CustomOption.Create(10, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最小内鬼阵营职业数"), 15f, 0f, 15f, 1f);
        impostorRolesCountMax = CustomOption.Create(11, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最大内鬼阵营职业数"), 15f, 0f, 15f, 1f);
        modifiersCountMin = CustomOption.Create(12, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最小附加职业数"), 15f, 0f, 30f, 1f);
        modifiersCountMax = CustomOption.Create(13, Types.General,
            cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "最大附加职业数"), 15f, 0f, 30f, 1f);

        //-------------------------- Impostor Options 10000-19999 -------------------------- //

        modifierAssassin = CustomOption.Create(10000, Types.Impostor, cs(Color.red, "刺客"), rates, null, true);
        modifierAssassinQuantity = CustomOption.Create(10001, Types.Impostor, cs(Color.red, "刺客数量"), ratesModifier, modifierAssassin);
        modifierAssassinNumberOfShots = CustomOption.Create(10002, Types.Impostor, "猜测次数（刺客共享）", 3f, 1f, 15f, 1f, modifierAssassin);
        modifierAssassinMultipleShotsPerMeeting = CustomOption.Create(10003, Types.Impostor, "同一轮会议可多次猜测", true, modifierAssassin);
        guesserEvilCanKillSpy = CustomOption.Create(10004, Types.Impostor, "可以猜测职业“卧底”", true, modifierAssassin);
        guesserEvilCanKillCrewmate = CustomOption.Create(10005, Types.Impostor, "可以猜测职业“船员”", true, modifierAssassin);
        guesserCantGuessSnitchIfTaksDone = CustomOption.Create(10006, Types.Impostor, "不可猜测完成任务的告密者", true, modifierAssassin);
        modifierAssassinKillsThroughShield = CustomOption.Create(10007, Types.Impostor, "猜测无视法医护盾保护", false, modifierAssassin);
        modifierAssassinCultist = CustomOption.Create(10008, Types.Impostor, "新信徒可成为刺客", false, modifierAssassin);

        mafiaSpawnRate = CustomOption.Create(10100, Types.Impostor, cs(Janitor.color, "黑手党"), rates, null, true);
        janitorCooldown = CustomOption.Create(10101, Types.Impostor, "清洁工清理冷却", 25f, 10f, 60f, 2.5f, mafiaSpawnRate);

        morphlingSpawnRate = CustomOption.Create(10110, Types.Impostor, cs(Morphling.color, "化形者"), rates, null, true);
        morphlingCooldown = CustomOption.Create(10111, Types.Impostor, "化形冷却", 15f, 10f, 60f, 2.5f, morphlingSpawnRate);
        morphlingDuration = CustomOption.Create(10112, Types.Impostor, "化形持续时间", 15f, 1f, 20f, 0.5f, morphlingSpawnRate);

        bomber2SpawnRate = CustomOption.Create(10120, Types.Impostor, cs(Bomber2.color, "炸弹狂"), rates, null, true);
        bomber2BombCooldown = CustomOption.Create(10121, Types.Impostor, "炸弹冷却", 25f, 10f, 60f, 2.5f, bomber2SpawnRate);
        bomber2Delay = CustomOption.Create(10122, Types.Impostor, "炸弹激活时间", 5f, 0f, 20f, 0.5f, bomber2SpawnRate);
        bomber2Timer = CustomOption.Create(10123, Types.Impostor, "炸弹爆炸时间", 10f, 5f, 30f, 0.5f, bomber2SpawnRate);
        //bomber2HotPotatoMode = CustomOption.Create(10124, Types.Impostor, "烫手山芋模式", true, bomber2SpawnRate);

        undertakerSpawnRate = CustomOption.Create(10130, Types.Impostor, cs(Undertaker.color, "送葬者"), rates, null, true);
        undertakerDragingDelaiAfterKill = CustomOption.Create(10131, Types.Impostor, "从击杀到恢复拖曳能力所需时间", 0f, 0f, 15, 0.5f, undertakerSpawnRate);
        undertakerDragingAfterVelocity = CustomOption.Create(10132, Types.Impostor, "拖拽过程的行动速度", 0.75f, 0.5f, 1.5f, 0.125f, undertakerSpawnRate);
        undertakerCanDragAndVent = CustomOption.Create(10133, Types.Impostor, "拖曳过程中可使用管道", true, undertakerSpawnRate);

        camouflagerSpawnRate = CustomOption.Create(10140, Types.Impostor, cs(Camouflager.color, "隐蔽者"), rates, null, true);
        camouflagerCooldown = CustomOption.Create(10141, Types.Impostor, "隐蔽状态冷却", 25f, 10f, 60f, 2.5f, camouflagerSpawnRate);
        camouflagerDuration = CustomOption.Create(10142, Types.Impostor, "隐蔽状态持续时间", 12.5f, 1f, 20f, 0.5f, camouflagerSpawnRate);

        vampireSpawnRate = CustomOption.Create(10150, Types.Impostor, cs(Vampire.color, "吸血鬼"), rates, null, true);
        vampireKillDelay = CustomOption.Create(10151, Types.Impostor, "从吸血到击杀所需时间", 5f, 1f, 20f, 0.5f, vampireSpawnRate);
        vampireCooldown = CustomOption.Create(10152, Types.Impostor, "吸血冷却", 25f, 10f, 60f, 2.5f, vampireSpawnRate);
        vampireGarlicButton = CustomOption.Create(10153, Types.Impostor, "发放大蒜", true, vampireSpawnRate);
        vampireCanKillNearGarlics = CustomOption.Create(10154, Types.Impostor, "可在大蒜附近击杀", true, vampireGarlicButton);

        eraserSpawnRate = CustomOption.Create(10160, Types.Impostor, cs(Eraser.color, "抹除者"), rates, null, true);
        eraserCooldown = CustomOption.Create(10161, Types.Impostor, "抹除冷却", 25f, 10f, 120f, 2.5f, eraserSpawnRate);
        eraserCanEraseAnyone = CustomOption.Create(10162, Types.Impostor, "可抹除任何人", false, eraserSpawnRate);

        mimicSpawnRate = CustomOption.Create(10170, Types.Impostor, cs(Mimic.color, "模仿者"), rates, null, true);

        escapistSpawnRate = CustomOption.Create(10180, Types.Impostor, cs(Escapist.color, "逃逸者"), rates, null, true);
        escapistEscapeTime = CustomOption.Create(10181, Types.Impostor, "标记/逃逸冷却", 15f, 0f, 60f, 2.5f, escapistSpawnRate);
        escapistChargesOnPlace = CustomOption.Create(10182, Types.Impostor, "每次逃逸/传送消耗点数", 1, 1, 10, 1, escapistSpawnRate);
        escapistResetPlaceAfterMeeting = CustomOption.Create(10183, Types.Impostor, "会议后重置目标地点", false, escapistSpawnRate);
        //jumperChargesGainOnMeeting = CustomOption.Create(10184, Types.Crewmate, "会议后增加点数", 2, 0, 10, 1, escapistSpawnRate);
        //escapistMaxCharges = CustomOption.Create(10185, Types.Impostor, "技能点数上限", 3, 0, 10, 1, escapistSpawnRate);

        cultistSpawnRate = CustomOption.Create(10190, Types.Impostor, cs(Cultist.color, "传教士"), rates, null, true);

        tricksterSpawnRate = CustomOption.Create(10200, Types.Impostor, cs(Trickster.color, "骗术师"), rates, null, true);
        tricksterPlaceBoxCooldown = CustomOption.Create(10201, Types.Impostor, "放置惊吓盒冷却", 20f, 2.5f, 30f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutCooldown = CustomOption.Create(10202, Types.Impostor, "熄灯冷却", 25f, 10f, 60f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutDuration = CustomOption.Create(10203, Types.Impostor, "熄灯持续时间", 12.5f, 5f, 60f, 0.5f, tricksterSpawnRate);

        cleanerSpawnRate = CustomOption.Create(10210, Types.Impostor, cs(Cleaner.color, "清理者"), rates, null, true);
        cleanerCooldown = CustomOption.Create(10211, Types.Impostor, "清理冷却", 25f, 10f, 60f, 2.5f, cleanerSpawnRate);

        warlockSpawnRate = CustomOption.Create(10220, Types.Impostor, cs(Cleaner.color, "术士"), rates, null, true);
        warlockCooldown = CustomOption.Create(10221, Types.Impostor, "术法冷却", 25f, 10f, 60f, 2.5f, warlockSpawnRate);
        warlockRootTime = CustomOption.Create(10222, Types.Impostor, "使用术法击杀后定身持续时间", 0.5f, 0f, 15f, 0.25f, warlockSpawnRate);

        bountyHunterSpawnRate = CustomOption.Create(10230, Types.Impostor, cs(BountyHunter.color, "赏金猎人"), rates, null, true);
        bountyHunterBountyDuration = CustomOption.Create(10231, Types.Impostor, "赏金目标更换间隔", 60f, 10f, 180f, 5f, bountyHunterSpawnRate);
        bountyHunterReducedCooldown = CustomOption.Create(10232, Types.Impostor, "击杀目标后的奖励冷却", 2.5f, 0f, 30f, 2.5f, bountyHunterSpawnRate);
        bountyHunterPunishmentTime = CustomOption.Create(10233, Types.Impostor, "击杀非目标后的惩罚冷却", 10f, 0f, 60f, 2.5f, bountyHunterSpawnRate);
        bountyHunterShowArrow = CustomOption.Create(10234, Types.Impostor, "显示指向悬赏目标的箭头", true, bountyHunterSpawnRate);
        bountyHunterArrowUpdateIntervall = CustomOption.Create(10235, Types.Impostor, "箭头更新间隔", 0.5f, 0f, 15f, 0.5f, bountyHunterShowArrow);

        witchSpawnRate = CustomOption.Create(10240, Types.Impostor, cs(Witch.color, "女巫"), rates, null, true);
        witchCooldown = CustomOption.Create(10241, Types.Impostor, "诅咒冷却", 25f, 10f, 60, 2.5f, witchSpawnRate);
        witchAdditionalCooldown = CustomOption.Create(10242, Types.Impostor, "诅咒冷却递增", 5f, 0f, 60f, 2.5f, witchSpawnRate);
        witchCanSpellAnyone = CustomOption.Create(10243, Types.Impostor, "可诅咒任何人", false, witchSpawnRate);
        witchSpellCastingDuration = CustomOption.Create(10244, Types.Impostor, "贴身诅咒所需时间", 0.25f, 0f, 10f, 0.25f, witchSpawnRate);
        witchTriggerBothCooldowns = CustomOption.Create(10245, Types.Impostor, "诅咒与击杀冷却共用", false, witchSpawnRate);
        witchVoteSavesTargets = CustomOption.Create(10246, Types.Impostor, "驱逐女巫可拯救被诅咒者", true, witchSpawnRate);

        ninjaSpawnRate = CustomOption.Create(10250, Types.Impostor, cs(Ninja.color, "忍者"), rates, null, true);
        ninjaCooldown = CustomOption.Create(10251, Types.Impostor, "标记冷却", 25f, 10f, 60f, 2.5f, ninjaSpawnRate);
        ninjaKnowsTargetLocation = CustomOption.Create(10252, Types.Impostor, "显示指向忍杀对象的箭头", true, ninjaSpawnRate);
        ninjaTraceTime = CustomOption.Create(10253, Types.Impostor, "忍杀后树叶痕迹持续时间", 6f, 1f, 20f, 0.5f, ninjaSpawnRate);
        ninjaTraceColorTime = CustomOption.Create(10254, Types.Impostor, "忍杀后痕迹褪色所需时间", 3f, 0f, 20f, 0.5f, ninjaSpawnRate);
        ninjaInvisibleDuration = CustomOption.Create(10255, Types.Impostor, "忍杀后隐身持续时间", 10f, 0f, 20f, 0.5f, ninjaSpawnRate);

        blackmailerSpawnRate = CustomOption.Create(10260, Types.Impostor, cs(Blackmailer.color, "勒索者"), rates, null, true);
        blackmailerCooldown = CustomOption.Create(10261, Types.Impostor, "勒索冷却", 15f, 5f, 120f, 2.5f, blackmailerSpawnRate);

        minerSpawnRate = CustomOption.Create(10280, Types.Impostor, cs(Miner.color, "管道工"), rates, null, true);
        minerCooldown = CustomOption.Create(10281, Types.Impostor, "制造管道冷却", 20f, 10f, 60f, 2.5f, minerSpawnRate);

        bomberSpawnRate = CustomOption.Create(10270, Types.Impostor, cs(Bomber.color, "恐怖分子"), rates, null, true);
        bomberBombDestructionTime = CustomOption.Create(10271, Types.Impostor, "炸弹引爆时间", 0f, 0f, 120f, 2.5f, bomberSpawnRate);
        bomberBombDestructionRange = CustomOption.Create(10272, Types.Impostor, "炸弹爆炸范围", 40f, 5f, 250f, 5f, bomberSpawnRate);
        bomberBombHearRange = CustomOption.Create(10273, Types.Impostor, "爆炸前预警范围", 45f, 5f, 250f, 5f, bomberSpawnRate);
        bomberDefuseDuration = CustomOption.Create(10274, Types.Impostor, "拆除炸弹所需时间", 0f, 0f, 30f, 0.5f, bomberSpawnRate);
        bomberBombCooldown = CustomOption.Create(10275, Types.Impostor, "炸弹放置冷却", 0f, 5f, 60f, 2.5f, bomberSpawnRate);
        bomberBombActiveAfter = CustomOption.Create(10276, Types.Impostor, "炸弹激活时间", 0f, 0f, 15f, 0.5f, bomberSpawnRate);

        //-------------------------- Neutral Options 20000-29999 -------------------------- //

        amnisiacSpawnRate = CustomOption.Create(20110, Types.Neutral, cs(Amnisiac.color, "失忆者"), rates, null, true);
        amnisiacShowArrows = CustomOption.Create(20111, Types.Neutral, "显示指向尸体的箭头", true, amnisiacSpawnRate);
        amnisiacResetRole = CustomOption.Create(20112, Types.Neutral, "回忆后重置该职业技能使用次数", true, amnisiacSpawnRate);

        jesterSpawnRate = CustomOption.Create(20100, Types.Neutral, cs(Jester.color, "小丑"), rates, null, true);
        jesterCanCallEmergency = CustomOption.Create(20101, Types.Neutral, "小丑可召开会议", true, jesterSpawnRate);
        jesterCanVent = CustomOption.Create(20102, Types.Neutral, "小丑可使用管道", true, jesterSpawnRate);
        jesterHasImpostorVision = CustomOption.Create(20103, Types.Neutral, "拥有内鬼视野", false, jesterSpawnRate);

        arsonistSpawnRate = CustomOption.Create(20120, Types.Neutral, cs(Arsonist.color, "纵火犯"), rates, null, true);
        arsonistCooldown = CustomOption.Create(20121, Types.Neutral, "涂油冷却", 12.5f, 5f, 60f, 2.5f, arsonistSpawnRate);
        arsonistDuration = CustomOption.Create(20122, Types.Neutral, "涂油所需时间", 0.25f, 0f, 10f, 0.25f, arsonistSpawnRate);

        jackalSpawnRate = CustomOption.Create(20130, Types.Neutral, cs(Jackal.color, "豺狼"), rates, null, true);
        jackalKillCooldown = CustomOption.Create(20131, Types.Neutral, "豺狼/跟班击杀冷却", 30f, 10f, 60f, 2.5f, jackalSpawnRate);
        jackalCanUseVents = CustomOption.Create(20132, Types.Neutral, "豺狼可使用管道", true, jackalSpawnRate);
        jackalCanUseSabo = CustomOption.Create(20133, Types.Neutral, "豺狼/跟班可进行破坏", false, jackalSpawnRate);
        jackalAndSidekickHaveImpostorVision = CustomOption.Create(20134, Types.Neutral, "豺狼/跟班拥有内鬼视野", false, jackalSpawnRate);
        jackalCanCreateSidekick = CustomOption.Create(20135, Types.Neutral, cs(Jackal.color, "豺狼可以招募跟班"), false, jackalSpawnRate);
        jackalCreateSidekickCooldown = CustomOption.Create(20136, Types.Neutral, "豺狼招募冷却", 30f, 10f, 60f, 2.5f, jackalCanCreateSidekick);
        jackalCanImpostorFindSidekick = CustomOption.Create(20137, Types.Neutral, cs(Color.red, "伪装者可以发现队友变为跟班"), true, jackalCanCreateSidekick);
        sidekickCanKill = CustomOption.Create(20138, Types.Neutral, "跟班可进行击杀", false, jackalCanCreateSidekick);
        sidekickCanUseVents = CustomOption.Create(20139, Types.Neutral, "跟班可使用管道", true, jackalCanCreateSidekick);
        sidekickPromotesToJackal = CustomOption.Create(20140, Types.Neutral, "豺狼死后跟班可晋升", false, jackalCanCreateSidekick);
        jackalPromotedFromSidekickCanCreateSidekick = CustomOption.Create(20141, Types.Neutral, "晋升后的豺狼可以招募跟班", true, sidekickPromotesToJackal);
        jackalCanCreateSidekickFromImpostor = CustomOption.Create(20142, Types.Neutral, "豺狼可以招募伪装者为跟班", true, jackalCanCreateSidekick);
        jackalKillFakeImpostor = CustomOption.Create(20143, Types.Neutral, "豺狼不可击杀被招募失败的伪装者", true, jackalCanCreateSidekick);

        vultureSpawnRate = CustomOption.Create(20170, Types.Neutral, cs(Vulture.color, "秃鹫"), rates, null, true);
        vultureCooldown = CustomOption.Create(20171, Types.Neutral, "吞噬冷却", 15f, 10f, 60f, 2.5f, vultureSpawnRate);
        vultureNumberToWin = CustomOption.Create(20172, Types.Neutral, "获胜所需吞噬次数", 3f, 1f, 10f, 1f, vultureSpawnRate);
        vultureCanUseVents = CustomOption.Create(20173, Types.Neutral, "可使用管道", true, vultureSpawnRate);
        vultureShowArrows = CustomOption.Create(20174, Types.Neutral, "显示指向尸体的箭头", true, vultureSpawnRate);

        lawyerSpawnRate = CustomOption.Create(20180, Types.Neutral, cs(Lawyer.color, "律师"), rates, null, true);
        lawyerIsProsecutorChance = CustomOption.Create(20181, Types.Neutral, "律师为处刑者的概率", rates, lawyerSpawnRate);
        lawyerTargetKnows = CustomOption.Create(20182, Types.Neutral, "客户知道律师存在", true, lawyerSpawnRate);
        lawyerVision = CustomOption.Create(20183, Types.Neutral, "视野倍率", 1.5f, 0.25f, 3f, 0.25f, lawyerSpawnRate);
        lawyerKnowsRole = CustomOption.Create(20184, Types.Neutral, "律师/处刑者可得知目标职业", false, lawyerSpawnRate);
        lawyerCanCallEmergency = CustomOption.Create(20185, Types.Neutral, "律师/处刑者可召开会议", true, lawyerSpawnRate);
        lawyerTargetCanBeJester = CustomOption.Create(20186, Types.Neutral, "小丑可以成为律师的客户", false, lawyerSpawnRate);
        pursuerCooldown = CustomOption.Create(20187, Types.Neutral, "起诉人空包弹冷却", 20f, 5f, 60f, 2.5f, lawyerSpawnRate);
        pursuerBlanksNumber = CustomOption.Create(20188, Types.Neutral, "起诉人空包弹可用次数", 6f, 1f, 20f, 1f, lawyerSpawnRate);

        swooperSpawnRate = CustomOption.Create(20150, Types.Neutral, cs(Swooper.color, "隐身人"), rates, null, true);
        swooperKillCooldown = CustomOption.Create(20151, Types.Neutral, "击杀冷却", 30f, 10f, 60f, 2.5f, swooperSpawnRate);
        swooperCooldown = CustomOption.Create(20152, Types.Neutral, "隐身冷却", 25f, 10f, 60f, 2.5f, swooperSpawnRate);
        swooperDuration = CustomOption.Create(20153, Types.Neutral, "隐身持续时间", 15f, 1f, 20f, 0.5f, swooperSpawnRate);
        swooperHasImpVision = CustomOption.Create(20154, Types.Neutral, "拥有内鬼视野", true, swooperSpawnRate);

        werewolfSpawnRate = CustomOption.Create(20200, Types.Neutral, cs(Werewolf.color, "月下狼人"), rates, null, true);
        werewolfRampageCooldown = CustomOption.Create(20201, Types.Neutral, "狂暴冷却", 30f, 10f, 60f, 2.5f, werewolfSpawnRate);
        werewolfRampageDuration = CustomOption.Create(20202, Types.Neutral, "狂暴持续时间", 15f, 1f, 20f, 0.5f, werewolfSpawnRate);
        werewolfKillCooldown = CustomOption.Create(20203, Types.Neutral, "击杀冷却", 3f, 1f, 60f, 0.5f, werewolfSpawnRate);

        juggernautSpawnRate = CustomOption.Create(20210, Types.Neutral, cs(Juggernaut.color, "天启"), rates, null, true);
        juggernautCooldown = CustomOption.Create(20211, Types.Neutral, "击杀冷却", 30f, 2.5f, 60f, 2.5f, juggernautSpawnRate);
        juggernautHasImpVision = CustomOption.Create(20212, Types.Neutral, "天启拥有伪装者视野", true, juggernautSpawnRate);
        juggernautReducedkillEach = CustomOption.Create(20213, Types.Neutral, "每次击杀后减少的cd", 5f, 1f, 15f, 0.5f, juggernautSpawnRate);

        doomsayerSpawnRate = CustomOption.Create(20221, Types.Neutral, cs(Doomsayer.color, "末日预言家"), rates, null, true);
        doomsayerCooldown = CustomOption.Create(20222, Types.Neutral, "技能冷却", 25f, 2.5f, 60f, 2.5f, doomsayerSpawnRate);
        doomsayerHasMultipleShotsPerMeeting = CustomOption.Create(20223, Types.Neutral, "猜测成功后可继续猜测", true, doomsayerSpawnRate);
        doomsayerShowInfoInGhostChat = CustomOption.Create(20224, Types.Neutral, "灵魂可见猜测结果", true, doomsayerSpawnRate);
        doomsayerCanGuessNeutral = CustomOption.Create(20225, Types.Neutral, "可以猜测中立", true, doomsayerSpawnRate);
        doomsayerCanGuessImpostor = CustomOption.Create(20226, Types.Neutral, "可以猜测伪装者", true, doomsayerSpawnRate);
        doomsayerOnlineTarger = CustomOption.Create(20227, Types.Neutral, "是否获取已有职业", false, doomsayerSpawnRate);
        doomsayerKillToWin = CustomOption.Create(20228, Types.Neutral, "需要成功猜测几次获胜", 3f, 1f, 10f, 1f, doomsayerSpawnRate);
        doomsayerDormationNum = CustomOption.Create(20229, Types.Neutral, "预言的职业数量", 5f, 1f, 10f, 1f, doomsayerSpawnRate);

        akujoSpawnRate = CustomOption.Create(20231, Types.Neutral, cs(Akujo.color, "魅魔"), rates, null, true);
        akujoTimeLimit = CustomOption.Create(20232, Types.Neutral, "魅魔招募真爱的时间", 300f, 60f, 1200f, 15f, akujoSpawnRate);
        akujoNumKeeps = CustomOption.Create(20233, Types.Neutral, "可招募备胎的数量", 1f, 0f, 10f, 1f, akujoSpawnRate);
        akujoKnowsRoles = CustomOption.Create(20234, Types.Neutral, "魅魔是否知道目标职业", true, akujoSpawnRate);
        akujoHonmeiCannotFollowWin = CustomOption.Create(20235, Types.Neutral, "真爱无法跟随阵营获胜", true, akujoSpawnRate);

        thiefSpawnRate = CustomOption.Create(20240, Types.Neutral, cs(Thief.color, "身份窃贼"), rates, null, true);
        thiefCooldown = CustomOption.Create(20241, Types.Neutral, "窃取冷却", 30f, 5f, 120f, 2.5f, thiefSpawnRate);
        thiefCanKillSheriff = CustomOption.Create(20242, Types.Neutral, "身份窃贼可以击杀警长", true, thiefSpawnRate);
        thiefHasImpVision = CustomOption.Create(20243, Types.Neutral, "身份窃贼拥有伪装者视野", true, thiefSpawnRate);
        thiefCanUseVents = CustomOption.Create(20244, Types.Neutral, "身份窃贼可以使用管道", true, thiefSpawnRate);
        thiefCanStealWithGuess = CustomOption.Create(20245, Types.Neutral, "身份窃贼可通过猜测窃取身份\n(赌怪模式)", false, thiefSpawnRate);

        //-------------------------- Crewmate Options 30000-39999 -------------------------- //

        guesserSpawnRate = CustomOption.Create(30100, Types.Crewmate, cs(Guesser.color, "侠客"), rates, null, true);
        guesserNumberOfShots = CustomOption.Create(30101, Types.Crewmate, "可猜测次数", 3f, 1f, 15f, 1f, guesserSpawnRate);
        guesserHasMultipleShotsPerMeeting = CustomOption.Create(30102, Types.Crewmate, "同一轮会议可多次猜测", true, guesserSpawnRate);
        guesserShowInfoInGhostChat = CustomOption.Create(30103, Types.Crewmate, "灵魂可见猜测结果", true, guesserSpawnRate);
        guesserKillsThroughShield = CustomOption.Create(30104, Types.Crewmate, "猜测无视法医护盾保护", false, guesserSpawnRate);

        mayorSpawnRate = CustomOption.Create(30110, Types.Crewmate, cs(Mayor.color, "市长"), rates, null, true);
        mayorCanSeeVoteColors = CustomOption.Create(30111, Types.Crewmate, "拥有窥视能力", true, mayorSpawnRate);
        mayorTasksNeededToSeeVoteColors = CustomOption.Create(30112, Types.Crewmate, "获得窥视能力所需完成的任务数", 5f, 0f, 20f, 1f, mayorCanSeeVoteColors);
        mayorMeetingButton = CustomOption.Create(30113, Types.Crewmate, "可远程召开会议", true, mayorSpawnRate);
        mayorMaxRemoteMeetings = CustomOption.Create(30114, Types.Crewmate, "远程召开会议可用次数", 1f, 1f, 5f, 1f, mayorMeetingButton);
        mayorTaskRemoteMeetings = CustomOption.Create(30115, Types.Crewmate, "可在破坏时使用（无效设置）", false, mayorMeetingButton);
        mayorChooseSingleVote = CustomOption.Create(30116, Types.Crewmate, "市长可选择投单票", ["关闭", "投票前选择", "会议结束前选择"], mayorSpawnRate);

        engineerSpawnRate = CustomOption.Create(30120, Types.Crewmate, cs(Engineer.color, "工程师"), rates, null, true);
        engineerRemoteFix = CustomOption.Create(30121, Types.Crewmate, "可远程修理破坏", true, engineerSpawnRate);
        engineerResetFixAfterMeeting = CustomOption.Create(30122, Types.Crewmate, "会议后重置修理次数", true, engineerRemoteFix);
        engineerNumberOfFixes = CustomOption.Create(30123, Types.Crewmate, "远程修理可用次数", 1f, 1f, 3f, 1f, engineerRemoteFix);
        //engineerExpertRepairs = CustomOption.Create(30124, Types.Crewmate, "高级修复模式", false, engineerSpawnRate);
        engineerHighlightForImpostors = CustomOption.Create(30125, Types.Crewmate, "内鬼可见工程师管道高光", true, engineerSpawnRate);
        engineerHighlightForTeamJackal = CustomOption.Create(30126, Types.Crewmate, "豺狼/跟班可见工程师管道高光 ", true, engineerSpawnRate);

        privateInvestigatorSpawnRate = CustomOption.Create(30130, Types.Crewmate, cs(PrivateInvestigator.color, "观察者"), rates, null, true);
        privateInvestigatorSeeColor = CustomOption.Create(30131, Types.Crewmate, "可见技能触发时对方具体颜色", true, privateInvestigatorSpawnRate);

        sheriffSpawnRate = CustomOption.Create(30141, Types.Crewmate, cs(Sheriff.color, "警长"), rates, null, true);
        sheriffCooldown = CustomOption.Create(30142, Types.Crewmate, "执法冷却", 30f, 10f, 60f, 2.5f, sheriffSpawnRate);
        sheriffMisfireKills = CustomOption.Create(30143, Types.Crewmate, "走火时死亡对象", ["警长", "对方", "双方"], sheriffSpawnRate);
        sheriffCanKillNeutrals = CustomOption.Create(30150, Types.Crewmate, "可执法独立阵营", false, sheriffSpawnRate);
        sheriffCanKillAmnesiac = CustomOption.Create(30153, Types.Crewmate, "可执法 " + cs(Amnisiac.color, "失忆者"), false, sheriffCanKillNeutrals);
        sheriffCanKillJester = CustomOption.Create(30151, Types.Crewmate, "可执法 " + cs(Jester.color, "小丑"), true, sheriffCanKillNeutrals);
        sheriffCanKillLawyer = CustomOption.Create(30156, Types.Crewmate, "可执法 " + cs(Lawyer.color, "律师"), true, sheriffCanKillNeutrals);
        sheriffCanKillProsecutor = CustomOption.Create(30152, Types.Crewmate, "可执法 " + cs(Lawyer.color, "处刑者"), true, sheriffCanKillNeutrals);
        sheriffCanKillPursuer = CustomOption.Create(30158, Types.Crewmate, "可执法 " + cs(Pursuer.color, "起诉人"), false, sheriffCanKillNeutrals);
        sheriffCanKillVulture = CustomOption.Create(30155, Types.Crewmate, "可执法 " + cs(Vulture.color, "秃鹫"), true, sheriffCanKillNeutrals);
        sheriffCanKillThief = CustomOption.Create(30157, Types.Crewmate, "可执法 " + cs(Thief.color, "身份窃贼"), true, sheriffCanKillNeutrals);
        sheriffCanKillDoomsayer = CustomOption.Create(30159, Types.Crewmate, "可执法 " + cs(Doomsayer.color, "末日预言家"), true, sheriffCanKillNeutrals);
        sheriffCanKillArsonist = CustomOption.Create(30154, Types.Crewmate, "可执法 " + cs(Arsonist.color, "纵火犯"), true, sheriffCanKillNeutrals);
        deputySpawnRate = CustomOption.Create(30170, Types.Crewmate, "可拥有一名捕快", rates, sheriffSpawnRate);
        deputyNumberOfHandcuffs = CustomOption.Create(30171, Types.Crewmate, "手铐可用次数", 5f, 1f, 10f, 1f, deputySpawnRate);
        deputyHandcuffCooldown = CustomOption.Create(30172, Types.Crewmate, "手铐冷却", 25f, 10f, 60f, 2.5f, deputySpawnRate);
        deputyHandcuffDuration = CustomOption.Create(30173, Types.Crewmate, "手铐持续时间", 12.5f, 5f, 60f, 2.5f, deputySpawnRate);
        deputyKnowsSheriff = CustomOption.Create(30174, Types.Crewmate, "警长/捕快可以互相确认 ", true, deputySpawnRate);
        deputyGetsPromoted = CustomOption.Create(30175, Types.Crewmate, "警长死后捕快可晋升", ["否", "立即晋升", "会议后晋升"], deputySpawnRate);
        deputyKeepsHandcuffs = CustomOption.Create(30176, Types.Crewmate, "晋升后保留手铐技能", true, deputyGetsPromoted);

        lighterSpawnRate = CustomOption.Create(30180, Types.Crewmate, cs(Lighter.color, "执灯人"), rates, null, true);
        lighterModeLightsOnVision = CustomOption.Create(30181, Types.Crewmate, "灯光正常时的视野倍率", 1.5f, 0.25f, 5f, 0.25f, lighterSpawnRate);
        lighterModeLightsOffVision = CustomOption.Create(30182, Types.Crewmate, "熄灯时的视野倍率", 0.5f, 0.25f, 5f, 0.25f, lighterSpawnRate);
        lighterFlashlightWidth = CustomOption.Create(30183, Types.Crewmate, "手电筒范围", 0.3f, 0.1f, 1f, 0.1f, lighterSpawnRate);

        detectiveSpawnRate = CustomOption.Create(30190, Types.Crewmate, cs(Detective.color, "侦探"), rates, null, true);
        detectiveAnonymousFootprints = CustomOption.Create(30191, Types.Crewmate, "匿名脚印", false, detectiveSpawnRate);
        detectiveFootprintIntervall = CustomOption.Create(30192, Types.Crewmate, "脚印更新间隔", 0.25f, 0.25f, 10f, 0.25f, detectiveSpawnRate);
        detectiveFootprintDuration = CustomOption.Create(30193, Types.Crewmate, "脚印持续时间", 12.5f, 0.5f, 30f, 0.5f, detectiveSpawnRate);
        detectiveReportNameDuration = CustomOption.Create(30194, Types.Crewmate, "以下时间内报告可得知凶手职业", 15f, 0f, 60f, 2.5f, detectiveSpawnRate);
        detectiveReportColorDuration = CustomOption.Create(30195, Types.Crewmate, "以下时间内报告可得知凶手颜色类型", 60f, 0f, 120f, 2.5f, detectiveSpawnRate);

        medicSpawnRate = CustomOption.Create(30200, Types.Crewmate, cs(Medic.color, "医生"), rates, null, true);
        medicShowShielded = CustomOption.Create(30201, Types.Crewmate, "可见医生护盾的玩家", ["所有人", "被保护者+法医", "法医"], medicSpawnRate);
        medicBreakShield = CustomOption.Create(30202, Types.Crewmate, "护盾持续生效", true, medicSpawnRate);
        medicShowAttemptToMedic = CustomOption.Create(30203, Types.Crewmate, "法医可见击杀尝试", true, medicBreakShield);
        medicShowAttemptToShielded = CustomOption.Create(30204, Types.Crewmate, "被保护者可见击杀尝试", false, medicBreakShield);
        medicResetTargetAfterMeeting = CustomOption.Create(30205, Types.Crewmate, "会议后重置保护目标", false, medicSpawnRate);
        medicSetOrShowShieldAfterMeeting = CustomOption.Create(30206, Types.Crewmate, "护盾生效与可见时机", ["立即生效且可见", "立即生效且会议后可见", "会议后生效且可见"], medicSpawnRate);
        medicReportNameDuration = CustomOption.Create(30207, Types.Crewmate, "以下时间内报告可得知凶手名字", 5f, 0f, 60f, 2.5f, medicBreakShield);
        medicReportColorDuration = CustomOption.Create(30208, Types.Crewmate, "以下时间内报告可得知凶手颜色类型", 30f, 0f, 120f, 2.5f, medicBreakShield);

        timeMasterSpawnRate = CustomOption.Create(30210, Types.Crewmate, cs(TimeMaster.color, "时间之主"), rates, null, true);
        timeMasterCooldown = CustomOption.Create(30211, Types.Crewmate, "时光之盾冷却", 25f, 10f, 60f, 2.5f, timeMasterSpawnRate);
        timeMasterRewindTime = CustomOption.Create(30212, Types.Crewmate, "回溯时间", 6f, 1f, 10f, 1f, timeMasterSpawnRate);
        timeMasterShieldDuration = CustomOption.Create(30213, Types.Crewmate, "时光之盾持续时间", 12.5f, 1f, 20f, 1f, timeMasterSpawnRate);

        veterenSpawnRate = CustomOption.Create(30220, Types.Crewmate, cs(Veteren.color, "老兵"), rates, null, true);
        veterenCooldown = CustomOption.Create(30221, Types.Crewmate, "警戒冷却", 25f, 10f, 120f, 2.5f, veterenSpawnRate);
        veterenAlertDuration = CustomOption.Create(30222, Types.Crewmate, "警戒持续时间", 15f, 1f, 20f, 1f, veterenSpawnRate);

        swapperSpawnRate = CustomOption.Create(30230, Types.Crewmate, cs(Swapper.color, "换票师"), rates, null, true);
        swapperCanCallEmergency = CustomOption.Create(30231, Types.Crewmate, "可召开会议", false, swapperSpawnRate);
        swapperCanFixSabotages = CustomOption.Create(30232, Types.Crewmate, "可修理紧急破坏", false, swapperSpawnRate);
        swapperCanOnlySwapOthers = CustomOption.Create(30233, Types.Crewmate, "只可交换他人", false, swapperSpawnRate);
        swapperSwapsNumber = CustomOption.Create(30234, Types.Crewmate, "初始可换票次数", 1f, 0f, 5f, 1f, swapperSpawnRate);
        swapperRechargeTasksNumber = CustomOption.Create(30235, Types.Crewmate, "充能所需任务数", 2f, 1f, 10f, 1f, swapperSpawnRate);

        seerSpawnRate = CustomOption.Create(30240, Types.Crewmate, cs(Seer.color, "灵媒"), rates, null, true);
        seerMode = CustomOption.Create(30241, Types.Crewmate, "感知模式", ["死亡闪光+可见灵魂", "死亡闪光", "可见灵魂"], seerSpawnRate);
        seerLimitSoulDuration = CustomOption.Create(30242, Types.Crewmate, "限制灵魂可见时间", false, seerSpawnRate);
        seerSoulDuration = CustomOption.Create(30243, Types.Crewmate, "灵魂可见时间", 30f, 0f, 120f, 2.5f, seerLimitSoulDuration);

        hackerSpawnRate = CustomOption.Create(30250, Types.Crewmate, cs(Hacker.color, "黑客"), rates, null, true);
        hackerCooldown = CustomOption.Create(30251, Types.Crewmate, "黑入冷却", 20f, 5f, 60f, 2.5f, hackerSpawnRate);
        hackerHackeringDuration = CustomOption.Create(30252, Types.Crewmate, "黑入持续时间", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate);
        hackerOnlyColorType = CustomOption.Create(30253, Types.Crewmate, "黑入后只可见颜色类型", false, hackerSpawnRate);
        hackerToolsNumber = CustomOption.Create(30254, Types.Crewmate, "移动设备最多使用次数", 5f, 1f, 30f, 1f, hackerSpawnRate);
        hackerRechargeTasksNumber = CustomOption.Create(30255, Types.Crewmate, "充能所需任务数", 2f, 1f, 5f, 1f, hackerSpawnRate);
        hackerNoMove = CustomOption.Create(30256, Types.Crewmate, "使用移动设备时不可移动", true, hackerSpawnRate);

        trackerSpawnRate = CustomOption.Create(30260, Types.Crewmate, cs(Tracker.color, "追踪者"), rates, null, true);
        trackerUpdateIntervall = CustomOption.Create(30261, Types.Crewmate, "箭头更新间隔", 0.5f, 0f, 30f, 0.5f, trackerSpawnRate);
        trackerResetTargetAfterMeeting = CustomOption.Create(30262, Types.Crewmate, "会议后重置跟踪目标 ", false, trackerSpawnRate);
        trackerCanTrackCorpses = CustomOption.Create(30263, Types.Crewmate, "可寻找尸体", true, trackerSpawnRate);
        trackerCorpsesTrackingCooldown = CustomOption.Create(30264, Types.Crewmate, "寻找尸体冷却", 20f, 5f, 120f, 2.5f, trackerCanTrackCorpses);
        trackerCorpsesTrackingDuration = CustomOption.Create(30265, Types.Crewmate, "寻找持续时间", 5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses);

        prophetSpawnRate = CustomOption.Create(30360, Types.Crewmate, cs(Prophet.color, "预言家"), rates, null, true);
        prophetCooldown = CustomOption.Create(30361, Types.Crewmate, "冷却时间", 25f, 5f, 60f, 2.5f, prophetSpawnRate);
        prophetNumExamines = CustomOption.Create(30362, Types.Crewmate, "预言总次数", 4f, 1f, 10f, 1f, prophetSpawnRate);
        prophetCanCallEmergency = CustomOption.Create(30363, Types.Crewmate, "可以召开紧急会议", true, prophetSpawnRate);
        prophetIsRevealed = CustomOption.Create(30364, Types.Crewmate, "可以被执刃者发现", false, prophetSpawnRate);
        prophetExaminesToBeRevealed = CustomOption.Create(30365, Types.Crewmate, "被发现所需揭示次数", 3f, 1f, 10f, 1f, prophetIsRevealed);
        prophetKillCrewAsRed = CustomOption.Create(30366, Types.Crewmate, "击杀型船员显示为红名", false, prophetSpawnRate);
        prophetBenignNeutralAsRed = CustomOption.Create(30367, Types.Crewmate, "善良型中立显示为红名", false, prophetSpawnRate);
        prophetEvilNeutralAsRed = CustomOption.Create(30368, Types.Crewmate, "邪恶型中立显示为红名", true, prophetSpawnRate);
        prophetKillNeutralAsRed = CustomOption.Create(30369, Types.Crewmate, "击杀型中立显示为红名", true, prophetSpawnRate);
        /*
        snitchSpawnRate = CustomOption.Create(30270, Types.Crewmate, cs(Snitch.color, "告密者"), rates, null, true);
        snitchLeftTasksForReveal = CustomOption.Create(30271, Types.Crewmate, "剩余多少任务时可被发现", 1f, 0f, 10f, 1f, snitchSpawnRate);
        snitchMode = CustomOption.Create(30272, Types.Crewmate, "信息显示", ["聊天框", "地图", "聊天框+地图"], snitchSpawnRate);
        snitchTargets = CustomOption.Create(30273, Types.Crewmate, "显示目标", ["所有邪恶职业", "杀手职业"], snitchSpawnRate);
        */
        snitchSpawnRate = CustomOption.Create(30270, Types.Crewmate, cs(Snitch.color, "告密者"), rates, null, true);
        snitchLeftTasksForReveal = CustomOption.Create(30271, Types.Crewmate, "剩余多少任务揭示告密者的位置", 1f, 0f, 5f, 1f, snitchSpawnRate);
        snitchSeeMeeting = CustomOption.Create(30272, Types.Crewmate, "可在会议中查看信息", false, snitchSpawnRate);
        //snitchCanSeeRoles = CustomOption.Create(30273, Types.Crewmate, "Can See Roles", false, snitchSpawnRate);
        snitchIncludeTeamJackal = CustomOption.Create(30274, Types.Crewmate, "可揭示豺狼阵营", false, snitchSpawnRate);
        snitchTeamJackalUseDifferentArrowColor = CustomOption.Create(30275, Types.Crewmate, "为豺狼阵营使用不同颜色的箭头", true, snitchIncludeTeamJackal);

        spySpawnRate = CustomOption.Create(30280, Types.Crewmate, cs(Spy.color, "卧底"), rates, null, true);
        spyCanDieToSheriff = CustomOption.Create(30281, Types.Crewmate, "可被警长执法", false, spySpawnRate);
        spyImpostorsCanKillAnyone = CustomOption.Create(30282, Types.Crewmate, "卧底在场时伪装者可击杀队友", true, spySpawnRate);
        spyCanEnterVents = CustomOption.Create(30283, Types.Crewmate, "可使用管道", true, spySpawnRate);
        spyHasImpostorVision = CustomOption.Create(30284, Types.Crewmate, "拥有内鬼视野", true, spySpawnRate);

        portalmakerSpawnRate = CustomOption.Create(30290, Types.Crewmate, cs(Portalmaker.color, "星门缔造者"), rates, null, true);
        portalmakerCooldown = CustomOption.Create(30291, Types.Crewmate, "构建星门冷却", 20f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerUsePortalCooldown = CustomOption.Create(30292, Types.Crewmate, "使用星门冷却", 15f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerLogOnlyColorType = CustomOption.Create(30293, Types.Crewmate, "星门日志只显示颜色类型", true, portalmakerSpawnRate);
        portalmakerLogHasTime = CustomOption.Create(30294, Types.Crewmate, "星门日志记录使用时间", true, portalmakerSpawnRate);
        portalmakerCanPortalFromAnywhere = CustomOption.Create(30295, Types.Crewmate, "可从任何地方传送至自己放置的传送门", true, portalmakerSpawnRate);

        securityGuardSpawnRate = CustomOption.Create(30300, Types.Crewmate, cs(SecurityGuard.color, "保安"), rates, null, true);
        securityGuardCooldown = CustomOption.Create(30301, Types.Crewmate, "保安冷却", 20f, 10f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardTotalScrews = CustomOption.Create(30302, Types.Crewmate, "保安螺丝数", 10f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamPrice = CustomOption.Create(30303, Types.Crewmate, "监控所需螺丝数", 3f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardVentPrice = CustomOption.Create(30304, Types.Crewmate, "封锁所需螺丝数", 2f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamDuration = CustomOption.Create(30305, Types.Crewmate, "保安技能持续时间", 10f, 2.5f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardCamMaxCharges = CustomOption.Create(30306, Types.Crewmate, "最大充能数", 5f, 1f, 30f, 1f, securityGuardSpawnRate);
        securityGuardCamRechargeTasksNumber = CustomOption.Create(30307, Types.Crewmate, "充能所需任务数", 3f, 1f, 10f, 1f, securityGuardSpawnRate);
        securityGuardNoMove = CustomOption.Create(30308, Types.Crewmate, "看监控时无法移动", true, securityGuardSpawnRate);

        mediumSpawnRate = CustomOption.Create(30310, Types.Crewmate, cs(Medium.color, "通灵师"), rates, null, true);
        mediumCooldown = CustomOption.Create(30311, Types.Crewmate, "通灵冷却", 10f, 2.5f, 120f, 2.5f, mediumSpawnRate);
        mediumDuration = CustomOption.Create(30312, Types.Crewmate, "通灵所需时间", 0.5f, 0f, 15f, 0.5f, mediumSpawnRate);
        mediumOneTimeUse = CustomOption.Create(30313, Types.Crewmate, "每个灵魂只能被通灵一次", false, mediumSpawnRate);
        mediumChanceAdditionalInfo = CustomOption.Create(30314, Types.Crewmate, "回答包含其他信息的可能性", rates, mediumSpawnRate);

        jumperSpawnRate = CustomOption.Create(30320, Types.Crewmate, cs(Jumper.color, "传送师"), rates, null, true);
        jumperJumpTime = CustomOption.Create(30321, Types.Crewmate, "标记/传送冷却", 10f, 0f, 60f, 2.5f, jumperSpawnRate);
        //jumperChargesOnPlace = CustomOption.Create(30322, Types.Crewmate, "每次传送所消耗点数", 1, 1, 10, 1, jumperSpawnRate);
        jumperMaxCharges = CustomOption.Create(30325, Types.Crewmate, "可传送次数", 3, 0, 10, 1, jumperSpawnRate);
        jumperResetPlaceAfterMeeting = CustomOption.Create(30323, Types.Crewmate, "会议后重置标记位置", false, jumperSpawnRate);
        //jumperChargesGainOnMeeting = CustomOption.Create(30324, Types.Crewmate, "会议后增加传送点数", 2, 0, 10, 1, jumperSpawnRate);
        /*
        magicianSpawnRate = CustomOption.Create(30330, Types.Crewmate, cs(Magician.color, "魔术师"), rates, null, true);
        magicianCooldown = CustomOption.Create(30331, Types.Crewmate, "放置冷却", 15f, 0f, 60f, 2.5f, magicianSpawnRate);
        magicianTeleportTime = CustomOption.Create(30332, Types.Crewmate, "传送冷却", 15f, 0f, 60f, 2.5f, magicianSpawnRate);
        magicianProbabilityBlueCards = CustomOption.Create(30333, Types.Crewmate, "抽到蓝牌的概率", rates, magicianSpawnRate);
        magicianProbabilityRedCards = CustomOption.Create(30334, Types.Crewmate, "抽到红牌的概率", rates, magicianSpawnRate);
        magicianProbabilityPurpleCards = CustomOption.Create(30335, Types.Crewmate, "抽到紫牌的概率", rates, magicianSpawnRate);
        */
        bodyGuardSpawnRate = CustomOption.Create(30340, Types.Crewmate, cs(BodyGuard.color, "保镖"), rates, null, true);
        bodyGuardResetTargetAfterMeeting = CustomOption.Create(30341, Types.Crewmate, "会议后重置保护目标", true, bodyGuardSpawnRate);
        bodyGuardFlash = CustomOption.Create(30342, Types.Crewmate, "死亡闪光", true, bodyGuardSpawnRate);

        trapperSpawnRate = CustomOption.Create(30350, Types.Crewmate, cs(Trapper.color, "设陷师"), rates, null, true);
        trapperCooldown = CustomOption.Create(30351, Types.Crewmate, "放置冷却", 20f, 5f, 120f, 2.5f, trapperSpawnRate);
        trapperMaxCharges = CustomOption.Create(30352, Types.Crewmate, "最大陷阱数", 4f, 1f, 15f, 1f, trapperSpawnRate);
        trapperRechargeTasksNumber = CustomOption.Create(30353, Types.Crewmate, "充能所需任务数", 2f, 1f, 15f, 1f, trapperSpawnRate);
        trapperTrapNeededTriggerToReveal = CustomOption.Create(30354, Types.Crewmate, "陷阱触发提示所需人数", 2f, 1f, 10f, 1f, trapperSpawnRate);
        trapperAnonymousMap = CustomOption.Create(30355, Types.Crewmate, "显示匿名地图", false, trapperSpawnRate);
        trapperInfoType = CustomOption.Create(30356, Types.Crewmate, "陷阱信息类型", ["职业", "善良/邪恶", "名字"], trapperSpawnRate);
        trapperTrapDuration = CustomOption.Create(30357, Types.Crewmate, "陷阱定身时间", 5f, 1f, 15f, 0.5f, trapperSpawnRate);

        //-------------------------- Modifier (1000 - 1999) -------------------------- //

        modifiersAreHidden = CustomOption.Create(1000, Types.Modifier, cs(Color.yellow, "隐藏死亡触发的附加职业"), true, null, true);

        modifierDisperser = CustomOption.Create(1001, Types.Modifier, cs(Color.red, "分散者"), rates, null, true);
        //modifierDisperserRemainingDisperses = CustomOption.Create(1002, Types.Modifier, "分散次数", 1f,1f,5f,1f, modifierDisperser);
        modifierDisperserDispersesToVent = CustomOption.Create(1003, Types.Modifier, "分散至管道位置", false, modifierDisperser);

        poucherSpawnRate = CustomOption.Create(1230, Types.Modifier, cs(Color.red, "入殓师"), rates, null, true);

        modifierLastImpostor = CustomOption.Create(1240, Types.Modifier, cs(Color.red, "绝境者"), false, null, true);
        modifierLastImpostorDeduce = CustomOption.Create(1241, Types.Modifier, "绝境者击杀冷却减少", 5f, 2.5f, 15f, 2.5f, modifierLastImpostor);

        modifierBloody = CustomOption.Create(1010, Types.Modifier, cs(Color.yellow, "溅血者"), rates, null, true);
        modifierBloodyQuantity = CustomOption.Create(1011, Types.Modifier, cs(Color.yellow, "溅血数量"), ratesModifier, modifierBloody);
        modifierBloodyDuration = CustomOption.Create(1012, Types.Modifier, "痕迹持续时间", 10f, 3f, 60f, 0.5f, modifierBloody);

        modifierAntiTeleport = CustomOption.Create(1020, Types.Modifier, cs(Color.yellow, "通讯兵"), rates, null, true);
        modifierAntiTeleportQuantity = CustomOption.Create(1021, Types.Modifier, cs(Color.yellow, "通讯兵数量"), ratesModifier, modifierAntiTeleport);

        modifierTieBreaker = CustomOption.Create(1030, Types.Modifier, cs(Color.yellow, "破平者"), rates, null, true);

        modifierBait = CustomOption.Create(1040, Types.Modifier, cs(Color.yellow, "诱饵"), rates, null, true);
        modifierBaitSwapCrewmate = CustomOption.Create(1041, Types.Modifier, "只分配给船员阵营", false, modifierBait);
        modifierBaitReportDelayMin = CustomOption.Create(1044, Types.Modifier, "诱饵报告延迟时间(最小)", 0f, 0f, 10f, 0.125f, modifierBait);
        modifierBaitReportDelayMax = CustomOption.Create(1045, Types.Modifier, "诱饵报告延迟时间(最大)", 0f, 0f, 10f, 0.5f, modifierBait);
        modifierBaitShowKillFlash = CustomOption.Create(1046, Types.Modifier, "用闪光灯警告杀手", true, modifierBait);

        modifierLover = CustomOption.Create(1050, Types.Modifier, cs(Color.yellow, "恋人"), rates, null, true);
        modifierLoverImpLoverRate = CustomOption.Create(1051, Types.Modifier, "恋人中有内鬼的概率", rates, modifierLover);
        modifierLoverBothDie = CustomOption.Create(1052, Types.Modifier, "恋人共死", true, modifierLover);
        modifierLoverEnableChat = CustomOption.Create(1053, Types.Modifier, "启用私密聊天文字频道", true, modifierLover);

        modifierSunglasses = CustomOption.Create(1060, Types.Modifier, cs(Color.yellow, "太阳镜"), rates, null, true);
        modifierSunglassesQuantity = CustomOption.Create(1061, Types.Modifier, cs(Color.yellow, "太阳镜数量"), ratesModifier, modifierSunglasses);
        modifierSunglassesVision = CustomOption.Create(1062, Types.Modifier, "太阳镜的视野倍率", ["-10%", "-20%", "-30%", "-40%", "-50%"], modifierSunglasses);

        modifierTorch = CustomOption.Create(1070, Types.Modifier, cs(Color.yellow, "火炬"), rates, null, true);
        modifierTorchQuantity = CustomOption.Create(1071, Types.Modifier, cs(Color.yellow, "火炬人数"), ratesModifier, modifierTorch);
        modifierTorchVision = CustomOption.Create(1072, Types.Modifier, "火炬的视野倍率", 1.5f, 1f, 3f, 0.125f, modifierTorch);

        modifierFlash = CustomOption.Create(1210, Types.Modifier, cs(Color.yellow, "闪电侠"), rates, null, true);
        modifierFlashQuantity = CustomOption.Create(1211, Types.Modifier, cs(Color.yellow, "闪电侠人数"), ratesModifier, modifierFlash);
        modifierFlashSpeed = CustomOption.Create(1212, Types.Modifier, "闪电侠的移速倍率", 1.25f, 1f, 3f, 0.125f, modifierFlash);

        modifierMultitasker = CustomOption.Create(1080, Types.Modifier, cs(Color.yellow, "多线程"), rates, null, true);
        modifierMultitaskerQuantity = CustomOption.Create(1081, Types.Modifier, cs(Color.yellow, "多线程人数"), ratesModifier, modifierMultitasker);

        modifierMini = CustomOption.Create(1090, Types.Modifier, cs(Color.yellow, "小孩"), rates, null, true);
        modifierMiniGrowingUpDuration = CustomOption.Create(1091, Types.Modifier, "小孩长大所需时间", 400f, 100f, 1500f, 25f, modifierMini);
        modifierMiniGrowingUpInMeeting = CustomOption.Create(1092, Types.Modifier, "小孩会议期间可成长", true, modifierMini);

        modifierIndomitable = CustomOption.Create(1100, Types.Modifier, cs(Color.yellow, "不屈者"), rates, null, true);

        modifierBlind = CustomOption.Create(1110, Types.Modifier, cs(Color.yellow, "胆小鬼"), rates, null, true);

        modifierWatcher = CustomOption.Create(1120, Types.Modifier, cs(Color.yellow, "窥视者"), rates, null, true);

        modifierRadar = CustomOption.Create(1130, Types.Modifier, cs(Color.yellow, "雷达"), rates, null, true);

        modifierTunneler = CustomOption.Create(1140, Types.Modifier, cs(Color.yellow, "管道工程师"), rates, null, true);

        modifierButtonBarry = CustomOption.Create(1220, Types.Modifier, cs(Color.yellow, "执钮人"), rates, null, true);
        modifierButtonTaskRemoteMeetings = CustomOption.Create(1221, Types.Modifier, "可在破坏时使用（无效设置）", false, modifierButtonBarry);

        modifierSlueth = CustomOption.Create(1150, Types.Modifier, cs(Color.yellow, "掘墓人"), rates, null, true);

        modifierCursed = CustomOption.Create(1160, Types.Modifier, cs(Color.yellow, "反骨"), rates, null, true);

        modifierVip = CustomOption.Create(1170, Types.Modifier, cs(Color.yellow, "VIP"), rates, null, true);
        modifierVipQuantity = CustomOption.Create(1171, Types.Modifier, cs(Color.yellow, "VIP人数"), ratesModifier, modifierVip);
        modifierVipShowColor = CustomOption.Create(1172, Types.Modifier, "死亡时全场提示阵营颜色", true, modifierVip);

        modifierInvert = CustomOption.Create(1180, Types.Modifier, cs(Color.yellow, "酒鬼"), rates, null, true);
        modifierInvertQuantity = CustomOption.Create(1181, Types.Modifier, cs(Color.yellow, "酒鬼人数"), ratesModifier, modifierInvert);
        modifierInvertDuration = CustomOption.Create(1182, Types.Modifier, "醉酒状态持续几轮会议", 2f, 1f, 15f, 1f, modifierInvert);

        modifierChameleon = CustomOption.Create(1190, Types.Modifier, cs(Color.yellow, "变色龙"), rates, null, true);
        modifierChameleonQuantity = CustomOption.Create(1191, Types.Modifier, cs(Color.yellow, "变色龙数量"), ratesModifier, modifierChameleon);
        modifierChameleonHoldDuration = CustomOption.Create(1192, Types.Modifier, "从不动到褪色开始的间隔时间", 3f, 1f, 10f, 0.5f, modifierChameleon);
        modifierChameleonFadeDuration = CustomOption.Create(1193, Types.Modifier, "褪色过程持续时间", 1f, 0.25f, 10f, 0.25f, modifierChameleon);
        modifierChameleonMinVisibility = CustomOption.Create(1194, Types.Modifier, "最低透明度", ["0%", "10%", "20%", "30%", "40%", "50%"], modifierChameleon);

        modifierShifter = CustomOption.Create(1200, Types.Modifier, cs(Color.yellow, "交换师"), rates, null, true);
        modifierShiftNeutral = CustomOption.Create(1201, Types.Modifier, "可交换部分中立不带刀职业", false, modifierShifter);


        //-------------------------- Guesser Gamemode 2000 - 2999 -------------------------- //

        guesserGamemodeCrewNumber = CustomOption.Create(2001, Types.Guesser, cs(Guesser.color, "船员阵营赌怪数"), 3f, 0f, 15f, 1f, null, true);
        guesserGamemodeNeutralNumber = CustomOption.Create(2002, Types.Guesser, cs(Guesser.color, "中立阵营赌怪数"), 3f, 0f, 15f, 1f, null, true);
        guesserGamemodeImpNumber = CustomOption.Create(2003, Types.Guesser, cs(Guesser.color, "伪装者阵营赌怪数"), 3f, 0f, 15f, 1f, null, true);
        guesserForceJackalGuesser = CustomOption.Create(2007, Types.Guesser, "强制豺狼成为赌怪", false, null, true);
        //guesserGamemodeSidekickIsAlwaysGuesser = CustomOption.Create(2012, Types.Guesser, "跟班继承赌怪", false, null, true);
        guesserForceThiefGuesser = CustomOption.Create(2011, Types.Guesser, "强制身份窃贼为赌怪", false, null, true);
        guesserGamemodeHaveModifier = CustomOption.Create(2004, Types.Guesser, "赌怪可以拥有附加职业", true);
        guesserGamemodeNumberOfShots = CustomOption.Create(2005, Types.Guesser, "赌怪猜测最大次数", 2f, 1f, 15f, 1f);
        guesserGamemodeHasMultipleShotsPerMeeting = CustomOption.Create(2006, Types.Guesser, "一轮会议可多次猜测", false);
        guesserGamemodeKillsThroughShield = CustomOption.Create(2008, Types.Guesser, "赌怪猜测无视护盾", true);
        guesserGamemodeEvilCanKillSpy = CustomOption.Create(2009, Types.Guesser, "邪恶的赌怪可猜测卧底", true);
        guesserGamemodeCantGuessSnitchIfTaksDone = CustomOption.Create(2010, Types.Guesser, "赌怪不可猜测已完成任务的告密者", true);

        //-------------------------- Hide N Seek 3000 - 3999 -------------------------- //

        hideNSeekMap = CustomOption.Create(3020, Types.HideNSeekMain, cs(Color.yellow, "地图"),
            ["骷髅舰", "米拉总部", "波鲁斯", "飞艇", "真菌丛林", "潜艇", "自定义地图"], null, true, () =>
            {
                var map = hideNSeekMap.selection;
                if (map >= 3) map++;
                GameOptionsManager.Instance.currentNormalGameOptions.MapId = (byte)map;
            });
        hideNSeekHunterCount = CustomOption.Create(3000, Types.HideNSeekMain, cs(Color.yellow, "猎人数量"), 1f, 1f, 3f, 1f);
        hideNSeekKillCooldown = CustomOption.Create(3021, Types.HideNSeekMain, cs(Color.yellow, "击杀冷却"), 10f, 2.5f, 60f, 2.5f);
        hideNSeekHunterVision = CustomOption.Create(3001, Types.HideNSeekMain, cs(Color.yellow, "猎人视野"), 0.5f, 0.25f, 2f, 0.25f);
        hideNSeekHuntedVision = CustomOption.Create(3002, Types.HideNSeekMain, cs(Color.yellow, "猎物视野"), 2f, 0.25f, 5f, 0.25f);
        hideNSeekCommonTasks = CustomOption.Create(3023, Types.HideNSeekMain, cs(Color.yellow, "普通任务"), 1f, 0f, 4f, 1f);
        hideNSeekShortTasks = CustomOption.Create(3024, Types.HideNSeekMain, cs(Color.yellow, "短任务"), 3f, 1f, 23f, 1f);
        hideNSeekLongTasks = CustomOption.Create(3025, Types.HideNSeekMain, cs(Color.yellow, "长任务"), 3f, 0f, 15f, 1f);
        hideNSeekTimer = CustomOption.Create(3003, Types.HideNSeekMain, cs(Color.yellow, "最少躲藏时间"), 5f, 1f, 30f, 0.5f);
        hideNSeekTaskWin = CustomOption.Create(3004, Types.HideNSeekMain, cs(Color.yellow, "可以任务获胜"), false);
        hideNSeekTaskPunish = CustomOption.Create(3017, Types.HideNSeekMain, cs(Color.yellow, "完成任务减少躲藏时间"), 10f, 0f, 30f, 1f);
        hideNSeekCanSabotage = CustomOption.Create(3019, Types.HideNSeekMain, cs(Color.yellow, "启用破坏"), false);
        hideNSeekHunterWaiting = CustomOption.Create(3022, Types.HideNSeekMain, cs(Color.yellow, "猎人等待入场时间"), 15f, 2.5f, 60f, 2.5f);

        hunterLightCooldown = CustomOption.Create(3005, Types.HideNSeekRoles, cs(Color.red, "猎人电灯冷却"), 30f, 5f, 60f, 1f, null, true);
        hunterLightDuration = CustomOption.Create(3006, Types.HideNSeekRoles, cs(Color.red, "猎人电灯持续时间"), 10f, 1f, 60f, 1f);
        hunterLightVision = CustomOption.Create(3007, Types.HideNSeekRoles, cs(Color.red, "猎人电灯视野"), 2f, 1f, 5f, 0.25f);
        hunterLightPunish = CustomOption.Create(3008, Types.HideNSeekRoles, cs(Color.red, "猎人电灯惩罚躲藏时间"), 5f, 0f, 30f, 1f);
        hunterAdminCooldown = CustomOption.Create(3009, Types.HideNSeekRoles, cs(Color.red, "猎人管理地图冷却"), 30f, 5f, 60f, 1f);
        hunterAdminDuration = CustomOption.Create(3010, Types.HideNSeekRoles, cs(Color.red, "猎人管理地图持续时间"), 5f, 1f, 60f, 1f);
        hunterAdminPunish = CustomOption.Create(3011, Types.HideNSeekRoles, cs(Color.red, "猎人管理地图惩罚躲藏时间"), 5f, 0f, 30f, 1f);
        hunterArrowCooldown = CustomOption.Create(3012, Types.HideNSeekRoles, cs(Color.red, "猎人追踪冷却时间"), 30f, 5f, 60f, 1f);
        hunterArrowDuration = CustomOption.Create(3013, Types.HideNSeekRoles, cs(Color.red, "猎人追踪持续时间"), 5f, 0f, 60f, 1f);
        hunterArrowPunish = CustomOption.Create(3014, Types.HideNSeekRoles, cs(Color.red, "猎人追踪惩罚躲藏时间"), 5f, 0f, 30f, 1f);

        huntedShieldCooldown = CustomOption.Create(3015, Types.HideNSeekRoles, cs(Color.gray, "躲藏者护盾冷却时间"), 30f, 5f, 60f, 1f, null, true);
        huntedShieldDuration = CustomOption.Create(3016, Types.HideNSeekRoles, cs(Color.gray, "躲藏者护盾持续时间"), 5f, 1f, 60f, 1f);
        huntedShieldRewindTime = CustomOption.Create(3018, Types.HideNSeekRoles, cs(Color.gray, "躲藏者回溯时间"), 3f, 1f, 10f, 1f);
        huntedShieldNumber = CustomOption.Create(3026, Types.HideNSeekRoles, cs(Color.grey, "躲藏者护盾数量"), 3f, 1f, 15f, 1f);

        //-------------------------- Prop Hunt General Options 4000 - 4999 -------------------------- //

        propHuntMap = CustomOption.Create(4020, Types.PropHunt, cs(Color.yellow, "地图"),
            new[] { "骷髅舰", "米拉总部", "波鲁斯", "飞艇", "蘑菇岛", "潜艇", "自定义地图" }, null, true, () =>
            {
                var map = propHuntMap.selection;
                if (map >= 3) map++;
                GameOptionsManager.Instance.currentNormalGameOptions.MapId = (byte)map;
            });
        propHuntTimer = CustomOption.Create(4021, Types.PropHunt, cs(Color.yellow, "最少躲藏时间"), 5f, 1f, 30f, 0.5f);
        propHuntUnstuckCooldown = CustomOption.Create(4011, Types.PropHunt, cs(Color.yellow, "穿墙冷却时间"), 30f, 2.5f, 60f, 2.5f);
        propHuntUnstuckDuration = CustomOption.Create(4012, Types.PropHunt, cs(Color.yellow, "穿墙持续时间"), 2f, 1f, 60f, 1f);
        propHunterVision = CustomOption.Create(4006, Types.PropHunt, cs(Color.yellow, "猎人视野"), 0.5f, 0.25f, 2f, 0.25f);
        propVision = CustomOption.Create(4007, Types.PropHunt, cs(Color.yellow, "躲藏者视野"), 2f, 0.25f, 5f, 0.25f);
        // Hunter Options
        propHuntNumberOfHunters = CustomOption.Create(4000, Types.PropHunt, cs(Color.red, "猎人数量"), 1f, 1f, 5f, 1f, null, true);
        hunterInitialBlackoutTime = CustomOption.Create(4001, Types.PropHunt, cs(Color.red, "猎人等待入场时间"), 10f, 5f, 20f, 1f);
        hunterMissCooldown = CustomOption.Create(4004, Types.PropHunt, cs(Color.red, "错误击杀后的冷却"), 10f, 2.5f, 60f, 2.5f);
        hunterHitCooldown = CustomOption.Create(4005, Types.PropHunt, cs(Color.red, "击杀后的冷却"), 10f, 2.5f, 60f, 2.5f);
        propHuntRevealCooldown = CustomOption.Create(4008, Types.PropHunt, cs(Color.red, "变形冷却时间"), 30f, 10f, 90f, 2.5f);
        propHuntRevealDuration = CustomOption.Create(4009, Types.PropHunt, cs(Color.red, "变形持续时间"), 5f, 1f, 60f, 1f);
        propHuntRevealPunish = CustomOption.Create(4010, Types.PropHunt, cs(Color.red, "揭示惩罚时间"), 10f, 0f, 1800f, 5f);
        propHuntAdminCooldown = CustomOption.Create(4022, Types.PropHunt, cs(Color.red, "猎人查看管理地图冷却时间"), 30f, 2.5f, 1800f, 2.5f);
        propHuntFindCooldown = CustomOption.Create(4023, Types.PropHunt, cs(Color.red, "寻找冷却时间"), 60f, 2.5f, 1800f, 2.5f);
        propHuntFindDuration = CustomOption.Create(4024, Types.PropHunt, cs(Color.red, "寻找持续时间"), 5f, 1f, 15f, 1f);
        // Prop Options
        propBecomesHunterWhenFound = CustomOption.Create(4003, Types.PropHunt, cs(Palette.CrewmateBlue, "猎物被发现后转化为猎人"), false, null, true);
        propHuntInvisEnabled = CustomOption.Create(4013, Types.PropHunt, cs(Palette.CrewmateBlue, "启用隐形"), true, null, true);
        propHuntInvisCooldown = CustomOption.Create(4014, Types.PropHunt, cs(Palette.CrewmateBlue, "隐形冷却时间"), 40f, 10f, 120f, 2.5f, propHuntInvisEnabled);
        propHuntInvisDuration = CustomOption.Create(4015, Types.PropHunt, cs(Palette.CrewmateBlue, "隐形持续时间"), 5f, 2.5f, 30f, 2.5f, propHuntInvisEnabled);
        propHuntSpeedboostEnabled = CustomOption.Create(4016, Types.PropHunt, cs(Palette.CrewmateBlue, "启用疾跑"), true, null, true);
        propHuntSpeedboostCooldown = CustomOption.Create(4017, Types.PropHunt, cs(Palette.CrewmateBlue, "疾跑冷却时间"), 45f, 2.5f, 120f, 2.5f, propHuntSpeedboostEnabled);
        propHuntSpeedboostDuration = CustomOption.Create(4018, Types.PropHunt, cs(Palette.CrewmateBlue, "疾跑持续时间"), 10f, 2.5f, 30f, 2.5f, propHuntSpeedboostEnabled);
        propHuntSpeedboostSpeed = CustomOption.Create(4019, Types.PropHunt, cs(Palette.CrewmateBlue, "疾跑提升速度"), 2f, 1.25f, 5f, 0.25f, propHuntSpeedboostEnabled);

        //-------------------------- Other options 1 - 599 -------------------------- //

        resteButtonCooldown = CustomOption.Create(14, Types.General, "游戏开局时重置CD", 10f, 2.5f, 30f, 2.5f);
        maxNumberOfMeetings = CustomOption.Create(15, Types.General, "会议总次数(不计入市长会议次数)", 10, 0, 15, 1, null, true);
        blockSkippingInEmergencyMeetings = CustomOption.Create(16, Types.General, "紧急会议禁止跳过", false);
        noVoteIsSelfVote = CustomOption.Create(17, Types.General, "不投票默认投自己", false, blockSkippingInEmergencyMeetings);
        blockGameEnd = CustomOption.Create(29, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "强力职业在场不结束游戏"), true);
        hidePlayerNames = CustomOption.Create(18, Types.General, "隐藏玩家名字", false);
        allowParallelMedBayScans = CustomOption.Create(19, Types.General, "允许同时进行扫描任务", false);
        allowModGuess = CustomOption.Create(20, Types.General, "允许猜测部分附加职业", false);
        shieldFirstKill = CustomOption.Create(21, Types.General, "首刀保护", false);
        hideOutOfSightNametags = CustomOption.Create(22, Types.General, "隐藏受阻碍的玩家名称", true);
        hideVentAnimOnShadows = CustomOption.Create(23, Types.General, "隐藏视野外管道动画", false);
        finishTasksBeforeHauntingOrZoomingOut = CustomOption.Create(24, Types.General, "未完成所有任务前不能使用跟随及千里眼", true);
        camsNightVision = CustomOption.Create(25, Types.General, "熄灯时监控开启夜视模式", false, null, true);
        camsNoNightVisionIfImpVision = CustomOption.Create(26, Types.General, "内鬼无视监控的夜视模式", false, camsNightVision);
        impostorSeeRoles = CustomOption.Create(27, Types.General, "内鬼可见队友职业", false);
        transparentTasks = CustomOption.Create(28, Types.General, "任务界面透明", false);
        dynamicMap = CustomOption.Create(51, Types.General, "随机地图", false, null, true);
        dynamicMapEnableSkeld = CustomOption.Create(52, Types.General, "Skeld", rates, dynamicMap);
        dynamicMapEnableMira = CustomOption.Create(53, Types.General, "Mira", rates, dynamicMap);
        dynamicMapEnablePolus = CustomOption.Create(54, Types.General, "Polus", rates, dynamicMap);
        dynamicMapEnableAirShip = CustomOption.Create(55, Types.General, "Airship", rates, dynamicMap);
        dynamicMapEnableFungle = CustomOption.Create(56, Types.General, "Fungle", rates, dynamicMap);
        dynamicMapEnableSubmerged = CustomOption.Create(57, Types.General, "Submerged", rates, dynamicMap);
        dynamicMapSeparateSettings = CustomOption.Create(58, Types.General, "使用随机地图设置预设", false, dynamicMap);

        //Map options
        randomGameStartPosition = CustomOption.Create(7910, Types.General, "随机出生点", false);
        enableBetterPolus = CustomOption.Create(7801, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "Polus"), false);
        movePolusVents = CustomOption.Create(7802, Types.General, "改变管道布局", false, enableBetterPolus);
        addPolusVents = CustomOption.Create(7803, Types.General, "添加新管道\n (样本室-办公室-运输船)", false, enableBetterPolus);
        movePolusVitals = CustomOption.Create(7804, Types.General, "将生命检测仪移动到实验室", false, enableBetterPolus);
        swapNavWifi = CustomOption.Create(7805, Types.General, "重启WIFI与导航任务位置交换", false, enableBetterPolus);
        moveColdTemp = CustomOption.Create(7806, Types.General, "温度调节任务移动至配电室下方", false, enableBetterPolus);

        enableMiraModify = CustomOption.Create(7811, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "Mira"), false);
        miraVitals = CustomOption.Create(7812, Types.General, "添加生命检测装置", false, enableMiraModify);

        enableAirShipModify = CustomOption.Create(7821, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "AirShip"), false);
        airshipOptimize = CustomOption.Create(7822, Types.General, "优化地图", false, enableAirShipModify);
        addAirShipVents = CustomOption.Create(7823, Types.General, "添加新管道\n (会议室-配电室)", false, enableAirShipModify);
        airshipLadder = CustomOption.Create(7824, Types.General, "增加额外梯子\n (会议室-间隙室)", false, enableAirShipModify);

        enableFungleModify = CustomOption.Create(7831, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "Fungle"), false);
        fungleElectrical = CustomOption.Create(7832, Types.General, "添加电力系统\n (食堂-实验室-上引擎)", false, enableFungleModify);

        enableCamoComms = CustomOption.Create(7901, Types.General, cs(Color.red, "通信破坏开启小黑人"), false);
        disableMedbayWalk = CustomOption.Create(7902, Types.General, "任务动画不可见", false);
        restrictDevices = CustomOption.Create(7903, Types.General, "限制信息设备使用", new[] { "否", "每一回合", "每局游戏" });
        //restrictAdmin = CustomOption.Create(7904, Types.General, "Restrict Admin Table", 30f, 0f, 600f, 5f, restrictDevices);
        restrictCameras = CustomOption.Create(7905, Types.General, "限制监控观看", 30f, 0f, 600f, 5f, restrictDevices);
        restrictVents = CustomOption.Create(7906, Types.General, "限制心电图观看", 30f, 0f, 600f, 5f, restrictDevices);
        disableCamsRound1 = CustomOption.Create(7907, Types.General, "第一回合无法看监控", false);
        showButtonTarget = CustomOption.Create(7908, Types.General, "技能按钮显示目标", true);


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
