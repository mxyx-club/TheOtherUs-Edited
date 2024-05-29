using System.Collections.Generic;
using TheOtherRoles.CustomGameModes;
using UnityEngine;
using static TheOtherRoles.CustomOption;
using static TheOtherRoles.TheOtherRoles;
using Types = TheOtherRoles.CustomOption.CustomOptionType;

namespace TheOtherRoles;

public class CustomOptionHolder
{
    public static string[] rates = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];

    public static string[] ratesModifier =
        ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15"];

    public static string[] presets =
        ["预设 1", "预设 2", "预设 3", "Skeld预设", "Mira预设", "Polus预设", "Airship预设", "Fungle预设", "Submerged预设"];

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
    public static CustomOption deadImpsBlockSabotage;

    public static CustomOption cultistSpawnRate;

    public static CustomOption minerSpawnRate;
    public static CustomOption minerCooldown;
    public static CustomOption mafiaSpawnRate;
    public static CustomOption janitorCooldown;

    public static CustomOption yoyoSpawnRate;
    public static CustomOption yoyoBlinkDuration;
    public static CustomOption yoyoMarkCooldown;
    public static CustomOption yoyoMarkStaysOverMeeting;
    public static CustomOption yoyoHasAdminTable;
    public static CustomOption yoyoAdminTableCooldown;
    public static CustomOption yoyoSilhouetteVisibility;

    public static CustomOption morphlingSpawnRate;
    public static CustomOption morphlingCooldown;
    public static CustomOption morphlingDuration;

    public static CustomOption bomberSpawnRate;
    public static CustomOption bomberBombCooldown;
    public static CustomOption bomberDelay;
    public static CustomOption bomberTimer;
    //public static CustomOption bomberHotPotatoMode;

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
    public static CustomOption poucherSpawnModifier;

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

    public static CustomOption evilTrapperSpawnRate;
    public static CustomOption evilTrapperNumTrap;
    public static CustomOption evilTrapperKillTimer;
    public static CustomOption evilTrapperCooldown;
    public static CustomOption evilTrapperMaxDistance;
    public static CustomOption evilTrapperTrapRange;
    public static CustomOption evilTrapperExtensionTime;
    public static CustomOption evilTrapperPenaltyTime;
    public static CustomOption evilTrapperBonusTime;

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
    public static CustomOption mayorSabotageRemoteMeetings;
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
    public static CustomOption trackerTrackingMethod;

    public static CustomOption snitchSpawnRate;
    public static CustomOption snitchLeftTasksForReveal;
    public static CustomOption snitchSeeMeeting;
    public static CustomOption snitchCanSeeRoles;
    public static CustomOption snitchIncludeNeutralTeam;
    public static CustomOption snitchTeamNeutraUseDifferentArrowColor;
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
    public static CustomOption bodyGuardShowShielded;

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
    public static CustomOption jumperResetPlaceAfterMeeting;
    public static CustomOption jumperChargesGainOnMeeting;
    public static CustomOption jumperMaxCharges;

    public static CustomOption escapistSpawnRate;
    public static CustomOption escapistEscapeTime;
    public static CustomOption escapistResetPlaceAfterMeeting;
    public static CustomOption escapistChargesGainOnMeeting;
    public static CustomOption escapistMaxCharges;

    public static CustomOption MagicianSpawnRate;
    public static CustomOption MagicianCooldown;
    public static CustomOption MagicianTeleportTime;
    public static CustomOption MagicianProbabilityBlueCards;
    public static CustomOption MagicianProbabilityRedCards;
    public static CustomOption MagicianProbabilityPurpleCards;

    public static CustomOption werewolfSpawnRate;
    public static CustomOption werewolfRampageCooldown;
    public static CustomOption werewolfRampageDuration;
    public static CustomOption werewolfKillCooldown;

    public static CustomOption thiefSpawnRate;
    public static CustomOption thiefCooldown;
    public static CustomOption thiefHasImpVision;
    public static CustomOption thiefCanUseVents;
    public static CustomOption thiefCanKillSheriff;
    public static CustomOption thiefCanKillDeputy;
    public static CustomOption thiefCanKillVeteren;
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
    public static CustomOption akujoForceKeeps;
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
    public static CustomOption prophetCanCallEmergency;
    public static CustomOption prophetIsRevealed;
    public static CustomOption prophetExaminesToBeRevealed;

    public static CustomOption terroristSpawnRate;
    public static CustomOption terroristBombDestructionTime;
    public static CustomOption terroristBombDestructionRange;
    public static CustomOption terroristBombHearRange;
    public static CustomOption terroristDefuseDuration;
    public static CustomOption terroristBombCooldown;
    public static CustomOption terroristBombActiveAfter;

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

    public static CustomOption modifierGiant;
    public static CustomOption modifierGiantSpped;

    public static CustomOption modifierIndomitable;

    public static CustomOption modifierBlind;

    public static CustomOption modifierTunneler;

    public static CustomOption modifierButtonBarry;
    public static CustomOption modifierButtonSabotageRemoteMeetings;

    public static CustomOption modifierWatcher;

    public static CustomOption modifierRadar;

    public static CustomOption modifierSlueth;

    public static CustomOption modifierCursed;
    public static CustomOption modifierShowCursed;

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

    public static CustomOption ShowVentsOnMap;
    public static CustomOption ShowVentsOnMeetingMap;
    public static CustomOption enableMapOptions;
    public static CustomOption randomGameStartPosition;
    public static CustomOption randomGameStartToVents;
    public static CustomOption allowModGuess;
    public static CustomOption ghostSpeed;
    public static CustomOption finishTasksBeforeHauntingOrZoomingOut;
    public static CustomOption camsNightVision;
    public static CustomOption camsNoNightVisionIfImpVision;

    public static CustomOption preventTaskEnd;
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

    public static CustomOption IsReactorDurationSetting;
    public static CustomOption SkeldLifeSuppTimeLimit;
    public static CustomOption SkeldReactorTimeLimit;
    public static CustomOption MiraLifeSuppTimeLimit;
    public static CustomOption MiraReactorTimeLimit;
    public static CustomOption PolusReactorTimeLimit;
    public static CustomOption AirshipReactorTimeLimit;
    public static CustomOption FungleReactorTimeLimit;
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

    public static CustomOption WireTaskIsRandomOption;
    public static CustomOption WireTaskNumOption;


    public static CustomOption disableMedbayWalk;

    public static CustomOption enableCamoComms;
    public static CustomOption fungleDisableCamoComms;

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
    public static CustomOption hideHuntSpeed;
    public static CustomOption hideCrewSpeed;

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
    public static CustomOption propHuntEnableUnstuck;
    public static CustomOption propHuntUnstuckCooldown;
    public static CustomOption propHuntUnstuckDuration;
    public static CustomOption propHuntNumberOfHunters;
    public static CustomOption hunterInitialBlackoutTime;
    public static CustomOption hunterMissCooldown;
    public static CustomOption hunterHitCooldown;
    public static CustomOption hunterMaxMissesBeforeDeath;
    public static CustomOption propBecomesHunterWhenFound;
    public static CustomOption propHunterVision;
    public static CustomOption propVision;
    public static CustomOption propHuntSpeed;
    public static CustomOption propPropSpeed;
    public static CustomOption propHuntRevealCooldown;
    public static CustomOption propHuntRevealDuration;
    public static CustomOption propHuntRevealPunish;
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

    internal static readonly Dictionary<byte, byte[]> blockedRolePairings = [];

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
        vanillaSettings = TheOtherRolesPlugin.Instance.Config.Bind("Preset0", "VanillaOptions", "");

        // Role Options
        presetSelection = Create(0, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "presetSelection"), presets, null, true);
        activateRoles = Create(1, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "activateRoles"), true, null, true);

        anyPlayerCanStopStart = Create(3, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "anyPlayerCanStopStart"), false, null, false);

        // Using new id's for the options to not break compatibilty with older versions
        crewmateRolesCountMin = Create(5, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMin"), 15f, 0f, 30f, 1f, null, true);
        crewmateRolesCountMax = Create(6, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesCountMax"), 15f, 0f, 30f, 1f);
        crewmateRolesFill = Create(7, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "crewmateRolesFill"), false);
        neutralRolesCountMin = Create(8, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMin"), 2f, 0f, 15f, 1f);
        neutralRolesCountMax = Create(9, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "neutralRolesCountMax"), 2f, 0f, 15f, 1f);
        impostorRolesCountMin = Create(10, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMin"), 15f, 0f, 15f, 1f);
        impostorRolesCountMax = Create(11, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "impostorRolesCountMax"), 15f, 0f, 15f, 1f);
        modifiersCountMin = Create(12, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "modifiersCountMin"), 15f, 0f, 30f, 1f);
        modifiersCountMax = Create(13, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "modifiersCountMax"), 15f, 0f, 30f, 1f);

        //-------------------------- Other options 1 - 599 -------------------------- //

        resteButtonCooldown = Create(20, Types.General, "resteButtonCooldown", 10f, 2.5f, 30f, 2.5f, null, true);
        maxNumberOfMeetings = Create(21, Types.General, "maxNumberOfMeetings", 10, 0, 15, 1, null, true);
        blockSkippingInEmergencyMeetings = Create(22, Types.General, "blockSkippingInEmergencyMeetings", false);
        noVoteIsSelfVote = Create(23, Types.General, "noVoteIsSelfVote", false, blockSkippingInEmergencyMeetings);
        shieldFirstKill = Create(24, Types.General, "shieldFirstKill", false);
        hidePlayerNames = Create(25, Types.General, "hidePlayerNames", false);
        hideOutOfSightNametags = Create(26, Types.General, "hideOutOfSightNametags", true);
        hideVentAnimOnShadows = Create(27, Types.General, "hideVentAnimOnShadows", false);
        showButtonTarget = Create(28, Types.General, "showButtonTarget", true);
        impostorSeeRoles = Create(30, Types.General, "impostorSeeRoles", false);
        blockGameEnd = Create(29, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "blockGameEnd"), true);
        deadImpsBlockSabotage = Create(32, Types.General, cs(Palette.ImpostorRed, "deadImpsBlockSabotage"), false);
        allowModGuess = Create(31, Types.General, "allowModGuess", false);
        //ghostSpeed = Create(33, Types.General, "ghostSpeed", 1f, 0.75f, 5f, 0.125f);

        transparentTasks = Create(40, Types.General, "transparentTasks", false, null, true);
        disableMedbayWalk = Create(41, Types.General, "disableMedbayWalk", false);
        allowParallelMedBayScans = Create(44, Types.General, "allowParallelMedBayScans", false);
        finishTasksBeforeHauntingOrZoomingOut = Create(42, Types.General, "finishTasksBeforeHauntingOrZoomingOut", false);
        //preventTaskEnd = Create(43, Types.General, "preventTaskEnd", false);

        //Map options
        enableMapOptions = Create(200, Types.General, "enableMapOptions", false, null, true);

        IsReactorDurationSetting = Create(201, Types.General, "IsReactorDurationSetting", false, enableMapOptions, true);
        SkeldReactorTimeLimit = Create(202, Types.General, "SkeldReactorTimeLimit", 30f, 15f, 60f, 2.5f, IsReactorDurationSetting);
        SkeldLifeSuppTimeLimit = Create(203, Types.General, "SkeldLifeSuppTimeLimit", 30f, 15f, 60f, 2.5f, IsReactorDurationSetting);
        MiraLifeSuppTimeLimit = Create(204, Types.General, "MiraLifeSuppTimeLimit", 30f, 15f, 60f, 2.5f, IsReactorDurationSetting);
        MiraReactorTimeLimit = Create(205, Types.General, "MiraReactorTimeLimit", 30f, 15f, 60f, 2.5f, IsReactorDurationSetting);
        PolusReactorTimeLimit = Create(206, Types.General, "PolusReactorTimeLimit", 60f, 15f, 90f, 2.5f, IsReactorDurationSetting);
        AirshipReactorTimeLimit = Create(207, Types.General, "AirshipReactorTimeLimit", 75f, 15f, 120f, 2.5f, IsReactorDurationSetting);
        FungleReactorTimeLimit = Create(208, Types.General, "FungleReactorTimeLimit", 45f, 15f, 60f, 2.5f, IsReactorDurationSetting);

        randomGameStartPosition = Create(50, Types.General, "randomGameStartPosition", false, enableMapOptions, true);
        randomGameStartToVents = Create(51, Types.General, "randomGameStartToVents", true, randomGameStartPosition);

        ShowVentsOnMap = Create(211, Types.General, getString("ShowVentsOnMapText"), false, enableMapOptions, true);
        ShowVentsOnMeetingMap = Create(212, Types.General, getString("ShowVentsOnMeetingMap"), true, ShowVentsOnMap);

        enableMiraModify = Create(70, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "Mira"), false, enableMapOptions, true);
        miraVitals = Create(71, Types.General, "miraVitals", false, enableMiraModify);

        enableBetterPolus = Create(80, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "Polus"), false, enableMapOptions);
        movePolusVents = Create(81, Types.General, "movePolusVents", false, enableBetterPolus);
        addPolusVents = Create(82, Types.General, "addPolusVents", false, enableBetterPolus);
        movePolusVitals = Create(83, Types.General, "movePolusVitals", false, enableBetterPolus);
        swapNavWifi = Create(84, Types.General, "swapNavWifi", false, enableBetterPolus);
        moveColdTemp = Create(85, Types.General, "moveColdTemp", false, enableBetterPolus);

        enableAirShipModify = Create(90, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "AirShip"), false, enableMapOptions);
        airshipOptimize = Create(91, Types.General, "airshipOptimize", false, enableAirShipModify);
        addAirShipVents = Create(92, Types.General, "addAirShipVents", false, enableAirShipModify);
        airshipLadder = Create(93, Types.General, "airshipLadder", false, enableAirShipModify);

        enableFungleModify = Create(100, Types.General, cs(new Color(200f / 200f, 200f / 200f, 0, 1f), "Fungle"), false, enableMapOptions);
        fungleElectrical = Create(101, Types.General, "fungleElectrical", false, enableFungleModify);

        WireTaskIsRandomOption = Create(221, Types.General, "WireTaskIsRandomOption", false, enableMapOptions, true);
        WireTaskNumOption = Create(222, Types.General, "WireTaskNumOption", 3f, 1f, 8f, 1f, WireTaskIsRandomOption);

        enableCamoComms = Create(120, Types.General, cs(Palette.ImpostorRed, "enableCamoComms"), false, enableMapOptions, true);
        fungleDisableCamoComms = Create(211, Types.General, "fungleDisableCamoComms", true, enableCamoComms);
        restrictDevices = Create(121, Types.General, "restrictDevices", ["optionOff".Translate(), "restrictDevices2".Translate(), "restrictDevices3".Translate()], enableMapOptions);
        //restrictAdmin = Create(122, Types.General, "restrictAdmin", 30f, 0f, 600f, 5f, restrictDevices);
        restrictCameras = Create(123, Types.General, "restrictCameras", 30f, 0f, 600f, 5f, restrictDevices);
        restrictVents = Create(124, Types.General, "restrictVents", 30f, 0f, 600f, 5f, restrictDevices);
        disableCamsRound1 = Create(125, Types.General, "disableCamsRound1", false, enableMapOptions);
        camsNightVision = Create(126, Types.General, "camsNightVision", false, enableMapOptions);
        camsNoNightVisionIfImpVision = Create(127, Types.General, "camsNoNightVisionIfImpVision", false, camsNightVision);

        dynamicMap = Create(130, Types.General, "dynamicMap", false, enableMapOptions, true);
        dynamicMapEnableSkeld = Create(131, Types.General, "Skeld", rates, dynamicMap);
        dynamicMapEnableMira = Create(132, Types.General, "Mira", rates, dynamicMap);
        dynamicMapEnablePolus = Create(133, Types.General, "Polus", rates, dynamicMap);
        dynamicMapEnableAirShip = Create(134, Types.General, "Airship", rates, dynamicMap);
        dynamicMapEnableFungle = Create(135, Types.General, "Fungle", rates, dynamicMap);
        dynamicMapEnableSubmerged = Create(136, Types.General, "Submerged", rates, dynamicMap);
        dynamicMapSeparateSettings = Create(137, Types.General, "dynamicMapSeparateSettings", false, dynamicMap);

        //-------------------------- Impostor Options 10000-19999 -------------------------- //

        modifierAssassin = Create(10000, Types.Impostor, cs(Palette.ImpostorRed, "modifierAssassin"), rates, null, true);
        modifierAssassinQuantity = Create(10001, Types.Impostor, "modifierAssassinQuantity", ratesModifier, modifierAssassin);
        modifierAssassinNumberOfShots = Create(10002, Types.Impostor, "modifierAssassinNumberOfShots", 3f, 1f, 15f, 1f, modifierAssassin);
        modifierAssassinMultipleShotsPerMeeting = Create(10003, Types.Impostor, "modifierAssassinMultipleShotsPerMeeting", true, modifierAssassin);
        guesserEvilCanKillSpy = Create(10004, Types.Impostor, "guesserEvilCanKillSpy", true, modifierAssassin);
        guesserEvilCanKillCrewmate = Create(10005, Types.Impostor, "guesserEvilCanKillCrewmate", true, modifierAssassin);
        guesserCantGuessSnitchIfTaksDone = Create(10006, Types.Impostor, "guesserCantGuessSnitchIfTaksDone", true, modifierAssassin);
        modifierAssassinKillsThroughShield = Create(10007, Types.Impostor, "modifierAssassinKillsThroughShield", false, modifierAssassin);
        modifierAssassinCultist = Create(10008, Types.Impostor, "modifierAssassinCultist", false, modifierAssassin);

        mafiaSpawnRate = Create(10100, Types.Impostor, cs(Janitor.color, "Mafia"), rates, null, true);
        janitorCooldown = Create(10101, Types.Impostor, "清洁工清理冷却", 25f, 10f, 60f, 2.5f, mafiaSpawnRate);

        morphlingSpawnRate = Create(10110, Types.Impostor, cs(Morphling.color, "Morphling"), rates, null, true);
        morphlingCooldown = Create(10111, Types.Impostor, "化形冷却", 15f, 10f, 60f, 2.5f, morphlingSpawnRate);
        morphlingDuration = Create(10112, Types.Impostor, "化形持续时间", 15f, 1f, 20f, 0.5f, morphlingSpawnRate);

        bomberSpawnRate = Create(10120, Types.Impostor, cs(Bomber.color, "Bomber"), rates, null, true);
        bomberBombCooldown = Create(10121, Types.Impostor, "炸弹冷却", 25f, 10f, 60f, 2.5f, bomberSpawnRate);
        bomberDelay = Create(10122, Types.Impostor, "炸弹激活时间", 5f, 0f, 20f, 0.5f, bomberSpawnRate);
        bomberTimer = Create(10123, Types.Impostor, "炸弹爆炸时间", 10f, 5f, 30f, 0.5f, bomberSpawnRate);
        //bomberHotPotatoMode = Create(10124, Types.Impostor, "烫手山芋模式", true, bomberSpawnRate);

        undertakerSpawnRate = Create(10130, Types.Impostor, cs(Undertaker.color, "Undertaker"), rates, null, true);
        undertakerDragingDelaiAfterKill = Create(10131, Types.Impostor, "从击杀到恢复拖曳能力所需时间", 0f, 0f, 15, 0.5f, undertakerSpawnRate);
        undertakerDragingAfterVelocity = Create(10132, Types.Impostor, "拖拽过程的行动速度", 0.75f, 0.5f, 1.5f, 0.125f, undertakerSpawnRate);
        undertakerCanDragAndVent = Create(10133, Types.Impostor, "拖曳过程中可使用管道", true, undertakerSpawnRate);

        camouflagerSpawnRate = Create(10140, Types.Impostor, cs(Camouflager.color, "Camouflager"), rates, null, true);
        camouflagerCooldown = Create(10141, Types.Impostor, "隐蔽状态冷却", 25f, 10f, 60f, 2.5f, camouflagerSpawnRate);
        camouflagerDuration = Create(10142, Types.Impostor, "隐蔽状态持续时间", 12.5f, 1f, 20f, 0.5f, camouflagerSpawnRate);

        vampireSpawnRate = Create(10150, Types.Impostor, cs(Vampire.color, "Vampire"), rates, null, true);
        vampireKillDelay = Create(10151, Types.Impostor, "从吸血到击杀所需时间", 5f, 1f, 20f, 0.5f, vampireSpawnRate);
        vampireCooldown = Create(10152, Types.Impostor, "吸血冷却", 25f, 10f, 60f, 2.5f, vampireSpawnRate);
        vampireGarlicButton = Create(10153, Types.Impostor, "发放大蒜", true, vampireSpawnRate);
        vampireCanKillNearGarlics = Create(10154, Types.Impostor, "可在大蒜附近击杀", true, vampireGarlicButton);

        eraserSpawnRate = Create(10160, Types.Impostor, cs(Eraser.color, "Eraser"), rates, null, true);
        eraserCooldown = Create(10161, Types.Impostor, "抹除冷却", 25f, 10f, 120f, 2.5f, eraserSpawnRate);
        eraserCanEraseAnyone = Create(10162, Types.Impostor, "可抹除任何人", false, eraserSpawnRate);

        poucherSpawnRate = Create(10320, Types.Impostor, cs(Palette.ImpostorRed, "Poucher"), rates, null, true);
        poucherSpawnModifier = Create(10321, Types.Impostor, "生成为附加职业", false, poucherSpawnRate);

        mimicSpawnRate = Create(10170, Types.Impostor, cs(Mimic.color, "Mimic"), rates, null, true);

        escapistSpawnRate = Create(10180, Types.Impostor, cs(Escapist.color, "Escapist"), rates, null, true);
        escapistEscapeTime = Create(10181, Types.Impostor, "标记/逃逸冷却", 15f, 0f, 60f, 2.5f, escapistSpawnRate);
        escapistMaxCharges = Create(10185, Types.Impostor, "可传送次数", 3, 1, 10, 1, escapistSpawnRate);
        escapistResetPlaceAfterMeeting = Create(10183, Types.Impostor, "会议后重置目标地点", false, escapistSpawnRate);
        escapistChargesGainOnMeeting = Create(10184, Types.Impostor, "会议后增加次数", 2, 0, 10, 1, escapistSpawnRate);

        cultistSpawnRate = Create(10190, Types.Impostor, cs(Cultist.color, "Cultist"), rates, null, true);

        tricksterSpawnRate = Create(10200, Types.Impostor, cs(Trickster.color, "Trickster"), rates, null, true);
        tricksterPlaceBoxCooldown = Create(10201, Types.Impostor, "放置惊吓盒冷却", 20f, 2.5f, 30f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutCooldown = Create(10202, Types.Impostor, "熄灯冷却", 25f, 10f, 60f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutDuration = Create(10203, Types.Impostor, "熄灯持续时间", 12.5f, 5f, 60f, 0.5f, tricksterSpawnRate);

        cleanerSpawnRate = Create(10210, Types.Impostor, cs(Cleaner.color, "Cleaner"), rates, null, true);
        cleanerCooldown = Create(10211, Types.Impostor, "清理冷却", 25f, 10f, 60f, 2.5f, cleanerSpawnRate);

        warlockSpawnRate = Create(10220, Types.Impostor, cs(Cleaner.color, "Cleaner"), rates, null, true);
        warlockCooldown = Create(10221, Types.Impostor, "术法冷却", 20f, 10f, 60f, 2.5f, warlockSpawnRate);
        warlockRootTime = Create(10222, Types.Impostor, "使用术法击杀后定身持续时间", 3f, 0f, 15f, 0.25f, warlockSpawnRate);

        bountyHunterSpawnRate = Create(10230, Types.Impostor, cs(BountyHunter.color, "BountyHunter"), rates, null, true);
        bountyHunterBountyDuration = Create(10231, Types.Impostor, "赏金目标更换间隔", 60f, 10f, 180f, 5f, bountyHunterSpawnRate);
        bountyHunterReducedCooldown = Create(10232, Types.Impostor, "击杀目标后的奖励冷却", 2.5f, 0f, 30f, 2.5f, bountyHunterSpawnRate);
        bountyHunterPunishmentTime = Create(10233, Types.Impostor, "击杀非目标后的惩罚冷却", 10f, 0f, 60f, 2.5f, bountyHunterSpawnRate);
        bountyHunterShowArrow = Create(10234, Types.Impostor, "显示指向悬赏目标的箭头", true, bountyHunterSpawnRate);
        bountyHunterArrowUpdateIntervall = Create(10235, Types.Impostor, "箭头更新间隔", 0.5f, 0f, 15f, 0.5f, bountyHunterShowArrow);

        witchSpawnRate = Create(10240, Types.Impostor, cs(Witch.color, "Witch"), rates, null, true);
        witchCooldown = Create(10241, Types.Impostor, "诅咒冷却", 20f, 10f, 60, 2.5f, witchSpawnRate);
        witchAdditionalCooldown = Create(10242, Types.Impostor, "诅咒冷却递增", 5f, 0f, 60f, 2.5f, witchSpawnRate);
        witchCanSpellAnyone = Create(10243, Types.Impostor, "可诅咒任何人", false, witchSpawnRate);
        witchSpellCastingDuration = Create(10244, Types.Impostor, "贴身诅咒所需时间", 0.5f, 0f, 10f, 0.25f, witchSpawnRate);
        witchTriggerBothCooldowns = Create(10245, Types.Impostor, "诅咒与击杀冷却共用", false, witchSpawnRate);
        witchVoteSavesTargets = Create(10246, Types.Impostor, "驱逐女巫可拯救被诅咒者", true, witchSpawnRate);

        ninjaSpawnRate = Create(10250, Types.Impostor, cs(Ninja.color, "Ninja"), rates, null, true);
        ninjaCooldown = Create(10251, Types.Impostor, "标记冷却", 20f, 10f, 60f, 2.5f, ninjaSpawnRate);
        ninjaKnowsTargetLocation = Create(10252, Types.Impostor, "显示指向忍杀对象的箭头", true, ninjaSpawnRate);
        ninjaTraceTime = Create(10253, Types.Impostor, "忍杀后树叶痕迹持续时间", 6f, 1f, 20f, 0.5f, ninjaSpawnRate);
        ninjaTraceColorTime = Create(10254, Types.Impostor, "忍杀后痕迹褪色所需时间", 3f, 0f, 20f, 0.5f, ninjaSpawnRate);
        ninjaInvisibleDuration = Create(10255, Types.Impostor, "忍杀后隐身持续时间", 10f, 0f, 20f, 0.5f, ninjaSpawnRate);

        blackmailerSpawnRate = Create(10260, Types.Impostor, cs(Blackmailer.color, "Blackmailer"), rates, null, true);
        blackmailerCooldown = Create(10261, Types.Impostor, "勒索冷却", 15f, 5f, 120f, 2.5f, blackmailerSpawnRate);

        terroristSpawnRate = Create(10270, Types.Impostor, cs(Terrorist.color, "Terrorist"), rates, null, true);
        terroristBombDestructionTime = Create(10271, Types.Impostor, "炸弹引爆时间", 0f, 0f, 120f, 0.5f, terroristSpawnRate);
        terroristBombDestructionRange = Create(10272, Types.Impostor, "炸弹爆炸范围", 30f, 5f, 250f, 5f, terroristSpawnRate);
        terroristBombHearRange = Create(10273, Types.Impostor, "爆炸前预警范围\n声音传播距离", 60f, 5f, 250f, 5f, terroristSpawnRate);
        terroristDefuseDuration = Create(10274, Types.Impostor, "拆除炸弹所需时间", 2f, 0f, 30f, 0.5f, terroristSpawnRate);
        terroristBombCooldown = Create(10275, Types.Impostor, "炸弹放置冷却", 0f, 5f, 60f, 2.5f, terroristSpawnRate);
        terroristBombActiveAfter = Create(10276, Types.Impostor, "炸弹激活时间", 0f, 0f, 15f, 0.5f, terroristSpawnRate);

        minerSpawnRate = Create(10280, Types.Impostor, cs(Miner.color, "Miner"), rates, null, true);
        minerCooldown = Create(10281, Types.Impostor, "制造管道冷却", 20f, 10f, 60f, 2.5f, minerSpawnRate);

        yoyoSpawnRate = Create(10290, Types.Impostor, cs(Yoyo.color, "Yoyo"), rates, null, true);
        yoyoMarkCooldown = Create(10292, Types.Impostor, "标记冷却", 15f, 2.5f, 120f, 2.5f, yoyoSpawnRate);
        yoyoBlinkDuration = Create(10291, Types.Impostor, "两段瞬移间隔时间", 15f, 2.5f, 120f, 2.5f, yoyoSpawnRate);
        yoyoMarkStaysOverMeeting = Create(10293, Types.Impostor, "会议后不重置标记地点", false, yoyoSpawnRate);
        yoyoHasAdminTable = Create(10294, Types.Impostor, "随身管理室地图", true, yoyoSpawnRate);
        yoyoAdminTableCooldown = Create(10295, Types.Impostor, "查看地图冷却", 15f, 2.5f, 120f, 2.5f, yoyoHasAdminTable);
        yoyoSilhouetteVisibility = Create(10296, Types.Impostor, "第一段残影透明度", ["0%", "10%", "20%", "30%", "40%", "50%"], yoyoSpawnRate);

        evilTrapperSpawnRate = Create(10300, Types.Impostor, cs(EvilTrapper.color, "EvilTrapper"), rates, null, true);
        evilTrapperNumTrap = Create(10301, Types.Impostor, "最大可同时放下的陷阱数", 2f, 1f, 10f, 1f, evilTrapperSpawnRate);
        evilTrapperExtensionTime = Create(10302, Types.Impostor, "陷阱激活所需时间", 5f, 2f, 10f, 0.5f, evilTrapperSpawnRate);
        evilTrapperCooldown = Create(10303, Types.Impostor, "放置陷阱冷却", 15f, 10f, 60f, 2.5f, evilTrapperSpawnRate);
        evilTrapperKillTimer = Create(10304, Types.Impostor, "掉进陷阱后被陷阱击杀所需时间", 5f, 1f, 30f, 1f, evilTrapperSpawnRate);
        evilTrapperTrapRange = Create(10305, Types.Impostor, "陷阱有效范围", 1f, 0.5f, 5f, 0.125f, evilTrapperSpawnRate);
        evilTrapperMaxDistance = Create(10306, Types.Impostor, "陷阱击杀声音播放有效范围", 10f, 1f, 60f, 2.5f, evilTrapperSpawnRate);
        evilTrapperPenaltyTime = Create(10307, Types.Impostor, "设陷者进行通常击杀的追加冷却", 10f, 0f, 30f, 2.5f, evilTrapperSpawnRate);
        evilTrapperBonusTime = Create(10308, Types.Impostor, "设陷者通常击杀落入陷阱的玩家的缩减冷却", 8f, 0f, 15f, 0.5f, evilTrapperSpawnRate);

        //-------------------------- Neutral Options 20000-29999 -------------------------- //

        amnisiacSpawnRate = Create(20110, Types.Neutral, cs(Amnisiac.color, "Amnisiac"), rates, null, true);
        amnisiacShowArrows = Create(20111, Types.Neutral, "显示指向尸体的箭头", true, amnisiacSpawnRate);
        amnisiacResetRole = Create(20112, Types.Neutral, "回忆后重置该职业技能使用次数", true, amnisiacSpawnRate);

        jesterSpawnRate = Create(20100, Types.Neutral, cs(Jester.color, "Jester"), rates, null, true);
        jesterCanCallEmergency = Create(20101, Types.Neutral, "小丑可召开会议", true, jesterSpawnRate);
        jesterCanVent = Create(20102, Types.Neutral, "小丑可使用管道", true, jesterSpawnRate);
        jesterHasImpostorVision = Create(20103, Types.Neutral, "拥有内鬼视野", true, jesterSpawnRate);

        arsonistSpawnRate = Create(20120, Types.Neutral, cs(Arsonist.color, "Arsonist"), rates, null, true);
        arsonistCooldown = Create(20121, Types.Neutral, "涂油冷却", 12.5f, 5f, 60f, 2.5f, arsonistSpawnRate);
        arsonistDuration = Create(20122, Types.Neutral, "涂油所需时间", 0.5f, 0f, 10f, 0.25f, arsonistSpawnRate);

        jackalSpawnRate = Create(20130, Types.Neutral, cs(Jackal.color, "Jackal"), rates, null, true);
        jackalKillCooldown = Create(20131, Types.Neutral, "豺狼/跟班击杀冷却", 25f, 10f, 60f, 2.5f, jackalSpawnRate);
        jackalCanUseVents = Create(20132, Types.Neutral, "豺狼可使用管道", true, jackalSpawnRate);
        jackalCanUseSabo = Create(20133, Types.Neutral, "豺狼/跟班可进行破坏", false, jackalSpawnRate);
        jackalAndSidekickHaveImpostorVision = Create(20134, Types.Neutral, "豺狼/跟班拥有内鬼视野", false, jackalSpawnRate);
        jackalCanCreateSidekick = Create(20135, Types.Neutral, cs(Jackal.color, "豺狼可以招募跟班"), false, jackalSpawnRate);
        jackalCreateSidekickCooldown = Create(20136, Types.Neutral, "豺狼招募冷却", 25f, 10f, 60f, 2.5f, jackalCanCreateSidekick);
        jackalCanImpostorFindSidekick = Create(20137, Types.Neutral, cs(Palette.ImpostorRed, "伪装者可以发现队友变为跟班"), true, jackalCanCreateSidekick);
        sidekickCanKill = Create(20138, Types.Neutral, "跟班可进行击杀", true, jackalCanCreateSidekick);
        sidekickCanUseVents = Create(20139, Types.Neutral, "跟班可使用管道", true, jackalCanCreateSidekick);
        sidekickPromotesToJackal = Create(20140, Types.Neutral, "豺狼死后跟班可晋升", false, jackalCanCreateSidekick);
        jackalPromotedFromSidekickCanCreateSidekick = Create(20141, Types.Neutral, "晋升后的豺狼可以招募跟班", true, sidekickPromotesToJackal);
        jackalCanCreateSidekickFromImpostor = Create(20142, Types.Neutral, "豺狼可以招募伪装者为跟班", true, jackalCanCreateSidekick);
        jackalKillFakeImpostor = Create(20143, Types.Neutral, "豺狼不可击杀被招募失败的伪装者", true, jackalCanCreateSidekick);

        vultureSpawnRate = Create(20170, Types.Neutral, cs(Vulture.color, "Vulture"), rates, null, true);
        vultureCooldown = Create(20171, Types.Neutral, "吞噬冷却", 15f, 10f, 60f, 2.5f, vultureSpawnRate);
        vultureNumberToWin = Create(20172, Types.Neutral, "获胜所需吞噬次数", 3f, 1f, 10f, 1f, vultureSpawnRate);
        vultureCanUseVents = Create(20173, Types.Neutral, "可使用管道", true, vultureSpawnRate);
        vultureShowArrows = Create(20174, Types.Neutral, "显示指向尸体的箭头", true, vultureSpawnRate);

        lawyerSpawnRate = Create(20180, Types.Neutral, cs(Lawyer.color, "Lawyer"), rates, null, true);
        lawyerIsProsecutorChance = Create(20181, Types.Neutral, "律师为处刑者的概率", rates, lawyerSpawnRate);
        lawyerTargetKnows = Create(20182, Types.Neutral, "客户知道律师存在", true, lawyerSpawnRate);
        lawyerVision = Create(20183, Types.Neutral, "视野倍率", 1.5f, 0.25f, 3f, 0.25f, lawyerSpawnRate);
        lawyerKnowsRole = Create(20184, Types.Neutral, "律师可得知目标职业", true, lawyerSpawnRate);
        lawyerCanCallEmergency = Create(20185, Types.Neutral, "律师/处刑者可召开会议", true, lawyerSpawnRate);
        lawyerTargetCanBeJester = Create(20186, Types.Neutral, "小丑可以成为律师的客户", false, lawyerSpawnRate);
        pursuerCooldown = Create(20187, Types.Neutral, "起诉人空包弹冷却", 20f, 5f, 60f, 2.5f, lawyerSpawnRate);
        pursuerBlanksNumber = Create(20188, Types.Neutral, "起诉人空包弹可用次数", 6f, 1f, 20f, 1f, lawyerSpawnRate);

        swooperSpawnRate = Create(20150, Types.Neutral, cs(Swooper.color, "Swooper"), rates, null, true);
        swooperKillCooldown = Create(20151, Types.Neutral, "击杀冷却", 25f, 10f, 60f, 2.5f, swooperSpawnRate);
        swooperCooldown = Create(20152, Types.Neutral, "隐身冷却", 20f, 10f, 60f, 2.5f, swooperSpawnRate);
        swooperDuration = Create(20153, Types.Neutral, "隐身持续时间", 15f, 1f, 20f, 0.5f, swooperSpawnRate);
        swooperHasImpVision = Create(20154, Types.Neutral, "拥有内鬼视野", true, swooperSpawnRate);

        werewolfSpawnRate = Create(20200, Types.Neutral, cs(Werewolf.color, "Werewolf"), rates, null, true);
        werewolfRampageCooldown = Create(20201, Types.Neutral, "狂暴冷却", 25f, 10f, 60f, 2.5f, werewolfSpawnRate);
        werewolfRampageDuration = Create(20202, Types.Neutral, "狂暴持续时间", 15f, 1f, 20f, 0.5f, werewolfSpawnRate);
        werewolfKillCooldown = Create(20203, Types.Neutral, "击杀冷却", 3f, 1f, 60f, 0.5f, werewolfSpawnRate);

        juggernautSpawnRate = Create(20210, Types.Neutral, cs(Juggernaut.color, "Juggernaut"), rates, null, true);
        juggernautCooldown = Create(20211, Types.Neutral, "击杀冷却", 25f, 2.5f, 60f, 2.5f, juggernautSpawnRate);
        juggernautHasImpVision = Create(20212, Types.Neutral, "天启拥有伪装者视野", true, juggernautSpawnRate);
        juggernautReducedkillEach = Create(20213, Types.Neutral, "每次击杀后减少的cd", 5f, 1f, 15f, 0.5f, juggernautSpawnRate);

        doomsayerSpawnRate = Create(20221, Types.Neutral, cs(Doomsayer.color, "Doomsayer"), rates, null, true);
        doomsayerCooldown = Create(20222, Types.Neutral, "技能冷却", 20f, 2.5f, 60f, 2.5f, doomsayerSpawnRate);
        doomsayerHasMultipleShotsPerMeeting = Create(20223, Types.Neutral, "猜测成功后可继续猜测", true, doomsayerSpawnRate);
        doomsayerShowInfoInGhostChat = Create(20224, Types.Neutral, "灵魂可见猜测结果", true, doomsayerSpawnRate);
        doomsayerCanGuessImpostor = Create(20226, Types.Neutral, "可以猜测" + cs(Palette.ImpostorRed, "伪装者"), true, doomsayerSpawnRate);
        doomsayerCanGuessNeutral = Create(20225, Types.Neutral, "可以猜测" + cs(Color.gray, "中立"), true, doomsayerSpawnRate);
        doomsayerOnlineTarger = Create(20227, Types.Neutral, "是否获取已有职业", false, doomsayerSpawnRate);
        doomsayerKillToWin = Create(20228, Types.Neutral, "需要成功猜测几次获胜", 3f, 1f, 10f, 1f, doomsayerSpawnRate);
        doomsayerDormationNum = Create(20229, Types.Neutral, "预言的职业数量", 5f, 1f, 10f, 1f, doomsayerSpawnRate);

        akujoSpawnRate = Create(20231, Types.Neutral, cs(Akujo.color, "Akujo"), rates, null, true);
        akujoTimeLimit = Create(20232, Types.Neutral, "魅魔招募真爱的时间", 600f, 90f, 1200f, 15f, akujoSpawnRate);
        akujoForceKeeps = Create(20236, Types.Neutral, "魅魔不招募备胎会自杀", false, akujoSpawnRate);
        akujoNumKeeps = Create(20233, Types.Neutral, "可招募备胎的数量", 1f, 0f, 10f, 1f, akujoSpawnRate);
        akujoKnowsRoles = Create(20234, Types.Neutral, "魅魔是否知道目标职业", true, akujoSpawnRate);
        akujoHonmeiCannotFollowWin = Create(20235, Types.Neutral, "真爱无法跟随阵营获胜", true, akujoSpawnRate);

        thiefSpawnRate = Create(20240, Types.Neutral, cs(Thief.color, "Thief"), rates, null, true);
        thiefCooldown = Create(20241, Types.Neutral, "窃取冷却", 25f, 5f, 120f, 2.5f, thiefSpawnRate);
        thiefCanKillSheriff = Create(20242, Types.Neutral, "身份窃贼可以击杀" + cs(Sheriff.color, "警长"), true, thiefSpawnRate);
        thiefCanKillDeputy = Create(20246, Types.Neutral, "身份窃贼可以击杀" + cs(Deputy.color, "捕快"), true, thiefSpawnRate);
        thiefCanKillVeteren = Create(20247, Types.Neutral, "身份窃贼可以击杀" + cs(Veteren.color, "老兵"), true, thiefSpawnRate);
        thiefHasImpVision = Create(20243, Types.Neutral, "身份窃贼拥有伪装者视野", true, thiefSpawnRate);
        thiefCanUseVents = Create(20244, Types.Neutral, "身份窃贼可以使用管道", true, thiefSpawnRate);
        thiefCanStealWithGuess = Create(20245, Types.Neutral, "身份窃贼可通过猜测窃取身份\n(赌怪模式)", true, thiefSpawnRate);

        //-------------------------- Crewmate Options 30000-39999 -------------------------- //

        guesserSpawnRate = Create(30100, Types.Crewmate, cs(Guesser.color, "Vigilante"), rates, null, true);
        guesserNumberOfShots = Create(30101, Types.Crewmate, "可猜测次数", 3f, 1f, 15f, 1f, guesserSpawnRate);
        guesserHasMultipleShotsPerMeeting = Create(30102, Types.Crewmate, "同一轮会议可多次猜测", true, guesserSpawnRate);
        guesserShowInfoInGhostChat = Create(30103, Types.Crewmate, "灵魂可见猜测结果", true, guesserSpawnRate);
        guesserKillsThroughShield = Create(30104, Types.Crewmate, "猜测无视法医护盾保护", false, guesserSpawnRate);

        mayorSpawnRate = Create(30110, Types.Crewmate, cs(Mayor.color, "Mayor"), rates, null, true);
        mayorCanSeeVoteColors = Create(30111, Types.Crewmate, "拥有窥视能力", true, mayorSpawnRate);
        mayorTasksNeededToSeeVoteColors = Create(30112, Types.Crewmate, "获得窥视能力所需完成的任务数", 5f, 0f, 20f, 1f, mayorCanSeeVoteColors);
        mayorMeetingButton = Create(30113, Types.Crewmate, "可远程召开会议", true, mayorSpawnRate);
        mayorMaxRemoteMeetings = Create(30114, Types.Crewmate, "远程召开会议可用次数", 1f, 1f, 5f, 1f, mayorMeetingButton);
        mayorSabotageRemoteMeetings = Create(30115, Types.Crewmate, "可在破坏时使用\n无效设置", false, mayorMeetingButton);
        mayorChooseSingleVote = Create(30116, Types.Crewmate, "市长可选择投单票", ["关闭", "投票前选择", "会议结束前选择"], mayorSpawnRate);

        engineerSpawnRate = Create(30120, Types.Crewmate, cs(Engineer.color, "Engineer"), rates, null, true);
        engineerRemoteFix = Create(30121, Types.Crewmate, "可远程修理破坏", true, engineerSpawnRate);
        engineerResetFixAfterMeeting = Create(30122, Types.Crewmate, "会议后重置修理次数", true, engineerRemoteFix);
        engineerNumberOfFixes = Create(30123, Types.Crewmate, "远程修理可用次数", 1f, 1f, 3f, 1f, engineerRemoteFix);
        //engineerExpertRepairs = Create(30124, Types.Crewmate, "高级修复模式", false, engineerSpawnRate);
        engineerHighlightForImpostors = Create(30125, Types.Crewmate, "内鬼可见工程师管道高光", true, engineerSpawnRate);
        engineerHighlightForTeamJackal = Create(30126, Types.Crewmate, "豺狼/跟班可见工程师管道高光 ", true, engineerSpawnRate);

        privateInvestigatorSpawnRate = Create(30130, Types.Crewmate, cs(PrivateInvestigator.color, "PrivateInvestigator"), rates, null, true);
        privateInvestigatorSeeColor = Create(30131, Types.Crewmate, "可见技能触发时对方具体颜色", true, privateInvestigatorSpawnRate);

        sheriffSpawnRate = Create(30141, Types.Crewmate, cs(Sheriff.color, "Sheriff"), rates, null, true);
        sheriffCooldown = Create(30142, Types.Crewmate, "执法冷却", 30f, 10f, 60f, 2.5f, sheriffSpawnRate);
        sheriffMisfireKills = Create(30143, Types.Crewmate, "走火时死亡对象", ["警长", "对方", "双方"], sheriffSpawnRate);
        sheriffCanKillNeutrals = Create(30150, Types.Crewmate, "可执法独立阵营", false, sheriffSpawnRate);
        sheriffCanKillAmnesiac = Create(30153, Types.Crewmate, "可执法 " + cs(Amnisiac.color, "失忆者"), false, sheriffCanKillNeutrals);
        sheriffCanKillJester = Create(30151, Types.Crewmate, "可执法 " + cs(Jester.color, "小丑"), true, sheriffCanKillNeutrals);
        sheriffCanKillLawyer = Create(30156, Types.Crewmate, "可执法 " + cs(Lawyer.color, "律师"), true, sheriffCanKillNeutrals);
        sheriffCanKillProsecutor = Create(30152, Types.Crewmate, "可执法 " + cs(Lawyer.color, "处刑者"), true, sheriffCanKillNeutrals);
        sheriffCanKillPursuer = Create(30158, Types.Crewmate, "可执法 " + cs(Pursuer.color, "起诉人"), true, sheriffCanKillNeutrals);
        sheriffCanKillVulture = Create(30155, Types.Crewmate, "可执法 " + cs(Vulture.color, "秃鹫"), true, sheriffCanKillNeutrals);
        sheriffCanKillThief = Create(30157, Types.Crewmate, "可执法 " + cs(Thief.color, "身份窃贼"), true, sheriffCanKillNeutrals);
        sheriffCanKillDoomsayer = Create(30159, Types.Crewmate, "可执法 " + cs(Doomsayer.color, "末日预言家"), true, sheriffCanKillNeutrals);
        sheriffCanKillArsonist = Create(30154, Types.Crewmate, "可执法 " + cs(Arsonist.color, "纵火犯"), true, sheriffCanKillNeutrals);
        deputySpawnRate = Create(30170, Types.Crewmate, cs(Deputy.color, "可拥有一名捕快"), rates, sheriffSpawnRate);
        deputyNumberOfHandcuffs = Create(30171, Types.Crewmate, "手铐可用次数", 5f, 1f, 10f, 1f, deputySpawnRate);
        deputyHandcuffCooldown = Create(30172, Types.Crewmate, "手铐冷却", 25f, 10f, 60f, 2.5f, deputySpawnRate);
        deputyHandcuffDuration = Create(30173, Types.Crewmate, "手铐持续时间", 12.5f, 5f, 60f, 2.5f, deputySpawnRate);
        deputyKnowsSheriff = Create(30174, Types.Crewmate, "警长/捕快可以互相确认 ", true, deputySpawnRate);
        deputyGetsPromoted = Create(30175, Types.Crewmate, "警长死后捕快可晋升", ["否", "立即晋升", "会议后晋升"], deputySpawnRate);
        deputyKeepsHandcuffs = Create(30176, Types.Crewmate, "晋升后保留手铐技能", true, deputyGetsPromoted);

        lighterSpawnRate = Create(30180, Types.Crewmate, cs(Lighter.color, "Lighter"), rates, null, true);
        lighterModeLightsOnVision = Create(30181, Types.Crewmate, "灯光正常时的视野倍率", 1.5f, 0.25f, 5f, 0.25f, lighterSpawnRate);
        lighterModeLightsOffVision = Create(30182, Types.Crewmate, "熄灯时的视野倍率", 0.5f, 0.25f, 5f, 0.25f, lighterSpawnRate);
        lighterFlashlightWidth = Create(30183, Types.Crewmate, "手电筒范围", 0.3f, 0.1f, 1f, 0.1f, lighterSpawnRate);

        detectiveSpawnRate = Create(30190, Types.Crewmate, cs(Detective.color, "Detective"), rates, null, true);
        detectiveAnonymousFootprints = Create(30191, Types.Crewmate, "匿名脚印", false, detectiveSpawnRate);
        detectiveFootprintIntervall = Create(30192, Types.Crewmate, "脚印更新间隔", 0.25f, 0.25f, 10f, 0.25f, detectiveSpawnRate);
        detectiveFootprintDuration = Create(30193, Types.Crewmate, "脚印持续时间", 12.5f, 0.5f, 30f, 0.5f, detectiveSpawnRate);
        detectiveReportNameDuration = Create(30194, Types.Crewmate, "以下时间内报告可得知凶手职业", 15f, 0f, 60f, 2.5f, detectiveSpawnRate);
        detectiveReportColorDuration = Create(30195, Types.Crewmate, "以下时间内报告可得知凶手颜色类型", 60f, 0f, 120f, 2.5f, detectiveSpawnRate);

        medicSpawnRate = Create(30200, Types.Crewmate, cs(Medic.color, "Medic"), rates, null, true);
        medicShowShielded = Create(30201, Types.Crewmate, "可见医生护盾的玩家", ["所有人", "被保护者+法医", "法医"], medicSpawnRate);
        medicBreakShield = Create(30202, Types.Crewmate, "护盾持续生效", true, medicSpawnRate);
        medicShowAttemptToMedic = Create(30203, Types.Crewmate, "法医可见击杀尝试", true, medicBreakShield);
        medicShowAttemptToShielded = Create(30204, Types.Crewmate, "被保护者可见击杀尝试", false, medicBreakShield);
        medicResetTargetAfterMeeting = Create(30205, Types.Crewmate, "会议后重置保护目标", false, medicSpawnRate);
        medicSetOrShowShieldAfterMeeting = Create(30206, Types.Crewmate, "护盾生效与可见时机", ["立即生效且可见", "立即生效且会议后可见", "会议后生效且可见"], medicSpawnRate);
        medicReportNameDuration = Create(30207, Types.Crewmate, "以下时间内报告可得知凶手名字", 5f, 0f, 60f, 2.5f, medicBreakShield);
        medicReportColorDuration = Create(30208, Types.Crewmate, "以下时间内报告可得知凶手颜色类型", 30f, 0f, 120f, 2.5f, medicBreakShield);

        bodyGuardSpawnRate = Create(30340, Types.Crewmate, cs(BodyGuard.color, "BodyGuard"), rates, null, true);
        bodyGuardResetTargetAfterMeeting = Create(30341, Types.Crewmate, "会议后重置保护目标", true, bodyGuardSpawnRate);
        bodyGuardShowShielded = Create(30343, Types.Crewmate, "保护目标可见护盾的存在", true, medicSpawnRate);
        bodyGuardFlash = Create(30342, Types.Crewmate, "死亡闪光", true, bodyGuardSpawnRate);

        timeMasterSpawnRate = Create(30210, Types.Crewmate, cs(TimeMaster.color, "TimeMaster"), rates, null, true);
        timeMasterCooldown = Create(30211, Types.Crewmate, "时光之盾冷却", 20f, 10f, 60f, 2.5f, timeMasterSpawnRate);
        timeMasterRewindTime = Create(30212, Types.Crewmate, "回溯时间", 6f, 1f, 10f, 1f, timeMasterSpawnRate);
        timeMasterShieldDuration = Create(30213, Types.Crewmate, "时光之盾持续时间", 12.5f, 1f, 20f, 1f, timeMasterSpawnRate);

        veterenSpawnRate = Create(30220, Types.Crewmate, cs(Veteren.color, "Veteren"), rates, null, true);
        veterenCooldown = Create(30221, Types.Crewmate, "警戒冷却", 25f, 10f, 120f, 2.5f, veterenSpawnRate);
        veterenAlertDuration = Create(30222, Types.Crewmate, "警戒持续时间", 15f, 1f, 20f, 1f, veterenSpawnRate);

        swapperSpawnRate = Create(30230, Types.Crewmate, cs(Swapper.color, "Swapper"), rates, null, true);
        swapperCanCallEmergency = Create(30231, Types.Crewmate, "可召开会议", false, swapperSpawnRate);
        swapperCanFixSabotages = Create(30232, Types.Crewmate, "可修理紧急破坏", false, swapperSpawnRate);
        swapperCanOnlySwapOthers = Create(30233, Types.Crewmate, "只可交换他人", false, swapperSpawnRate);
        swapperSwapsNumber = Create(30234, Types.Crewmate, "初始可换票次数", 1f, 0f, 5f, 1f, swapperSpawnRate);
        swapperRechargeTasksNumber = Create(30235, Types.Crewmate, "充能所需任务数", 2f, 1f, 10f, 1f, swapperSpawnRate);

        seerSpawnRate = Create(30240, Types.Crewmate, cs(Seer.color, "Seer"), rates, null, true);
        seerMode = Create(30241, Types.Crewmate, "感知模式", ["死亡闪光+可见灵魂", "死亡闪光", "可见灵魂"], seerSpawnRate);
        seerLimitSoulDuration = Create(30242, Types.Crewmate, "限制灵魂可见时间", false, seerSpawnRate);
        seerSoulDuration = Create(30243, Types.Crewmate, "灵魂可见时间", 30f, 0f, 120f, 2.5f, seerLimitSoulDuration);

        hackerSpawnRate = Create(30250, Types.Crewmate, cs(Hacker.color, "Hacker"), rates, null, true);
        hackerCooldown = Create(30251, Types.Crewmate, "黑入冷却", 20f, 5f, 60f, 2.5f, hackerSpawnRate);
        hackerHackeringDuration = Create(30252, Types.Crewmate, "黑入持续时间", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate);
        hackerOnlyColorType = Create(30253, Types.Crewmate, "黑入后只可见颜色类型", false, hackerSpawnRate);
        hackerToolsNumber = Create(30254, Types.Crewmate, "移动设备最多使用次数", 5f, 1f, 30f, 1f, hackerSpawnRate);
        hackerRechargeTasksNumber = Create(30255, Types.Crewmate, "充能所需任务数", 2f, 1f, 5f, 1f, hackerSpawnRate);
        hackerNoMove = Create(30256, Types.Crewmate, "使用移动设备时不可移动", true, hackerSpawnRate);

        trackerSpawnRate = Create(30260, Types.Crewmate, cs(Tracker.color, "Tracker"), rates, null, true);
        trackerUpdateIntervall = Create(30261, Types.Crewmate, "箭头更新间隔", 0.5f, 0f, 30f, 0.5f, trackerSpawnRate);
        trackerResetTargetAfterMeeting = Create(30262, Types.Crewmate, "会议后重置跟踪目标 ", false, trackerSpawnRate);
        trackerCanTrackCorpses = Create(30263, Types.Crewmate, "可寻找尸体", true, trackerSpawnRate);
        trackerCorpsesTrackingCooldown = Create(30264, Types.Crewmate, "寻找尸体冷却", 17.5f, 5f, 60f, 2.5f, trackerCanTrackCorpses);
        trackerCorpsesTrackingDuration = Create(30265, Types.Crewmate, "寻找持续时间", 7.5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses);
        trackerTrackingMethod = Create(30266, Types.Crewmate, "追踪模式", ["仅箭头", "仅检测是否接近", "两者皆有"], trackerSpawnRate);

        prophetSpawnRate = Create(30360, Types.Crewmate, cs(Prophet.color, "Prophet"), rates, null, true);
        prophetCooldown = Create(30361, Types.Crewmate, "冷却时间", 20f, 5f, 60f, 2.5f, prophetSpawnRate);
        prophetNumExamines = Create(30362, Types.Crewmate, "预言总次数", 4f, 1f, 10f, 1f, prophetSpawnRate);
        prophetCanCallEmergency = Create(30363, Types.Crewmate, "可以召开紧急会议", true, prophetSpawnRate);
        prophetIsRevealed = Create(30364, Types.Crewmate, "可以被杀手发现", false, prophetSpawnRate);
        prophetExaminesToBeRevealed = Create(30365, Types.Crewmate, "被发现所需揭示次数", 3f, 1f, 10f, 1f, prophetIsRevealed);
        prophetKillCrewAsRed = Create(30366, Types.Crewmate, "击杀型船员显示为红名", false, prophetSpawnRate);
        prophetBenignNeutralAsRed = Create(30367, Types.Crewmate, "善良型中立显示为红名", false, prophetSpawnRate);
        prophetEvilNeutralAsRed = Create(30368, Types.Crewmate, "邪恶型中立显示为红名", true, prophetSpawnRate);

        snitchSpawnRate = Create(30270, Types.Crewmate, cs(Snitch.color, "Snitch"), rates, null, true);
        snitchLeftTasksForReveal = Create(30271, Types.Crewmate, "剩余多少任务时可被发现", 1f, 0f, 10f, 1f, snitchSpawnRate);
        snitchSeeMeeting = Create(30272, Types.Crewmate, "可在会议中查看信息", true, snitchSpawnRate);
        snitchCanSeeRoles = Create(30273, Types.Crewmate, "可以在会议中查看具体职业", false, snitchSpawnRate);
        snitchIncludeNeutralTeam = Create(30274, Types.Crewmate, "可揭示中立阵营", ["关闭", "仅杀手中立", "所有邪恶中立", "全部中立阵营"], snitchSpawnRate);
        snitchTeamNeutraUseDifferentArrowColor = Create(30275, Types.Crewmate, "为中立阵营使用不同颜色的提醒\n(关闭则看谁都是红色)", true, snitchIncludeNeutralTeam);

        spySpawnRate = Create(30280, Types.Crewmate, cs(Spy.color, "Spy"), rates, null, true);
        spyCanDieToSheriff = Create(30281, Types.Crewmate, "可被警长执法", false, spySpawnRate);
        spyImpostorsCanKillAnyone = Create(30282, Types.Crewmate, "卧底在场时伪装者可击杀队友", true, spySpawnRate);
        spyCanEnterVents = Create(30283, Types.Crewmate, "可使用管道", true, spySpawnRate);
        spyHasImpostorVision = Create(30284, Types.Crewmate, "拥有内鬼视野", true, spySpawnRate);

        portalmakerSpawnRate = Create(30290, Types.Crewmate, cs(Portalmaker.color, "Portalmaker"), rates, null, true);
        portalmakerCooldown = Create(30291, Types.Crewmate, "构建星门冷却", 15f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerUsePortalCooldown = Create(30292, Types.Crewmate, "使用星门冷却", 15f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerLogOnlyColorType = Create(30293, Types.Crewmate, "星门日志只显示颜色类型", true, portalmakerSpawnRate);
        portalmakerLogHasTime = Create(30294, Types.Crewmate, "星门日志记录使用时间", true, portalmakerSpawnRate);
        portalmakerCanPortalFromAnywhere = Create(30295, Types.Crewmate, "可从任何地方传送至自己放置的传送门", true, portalmakerSpawnRate);

        securityGuardSpawnRate = Create(30300, Types.Crewmate, cs(SecurityGuard.color, "SecurityGuard"), rates, null, true);
        securityGuardCooldown = Create(30301, Types.Crewmate, "保安冷却", 15f, 10f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardTotalScrews = Create(30302, Types.Crewmate, "保安螺丝数", 10f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamPrice = Create(30303, Types.Crewmate, "监控所需螺丝数", 3f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardVentPrice = Create(30304, Types.Crewmate, "封锁所需螺丝数", 2f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamDuration = Create(30305, Types.Crewmate, "保安技能持续时间", 10f, 2.5f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardCamMaxCharges = Create(30306, Types.Crewmate, "最大充能数", 5f, 1f, 30f, 1f, securityGuardSpawnRate);
        securityGuardCamRechargeTasksNumber = Create(30307, Types.Crewmate, "充能所需任务数", 3f, 1f, 10f, 1f, securityGuardSpawnRate);
        securityGuardNoMove = Create(30308, Types.Crewmate, "看监控时无法移动", true, securityGuardSpawnRate);

        mediumSpawnRate = Create(30310, Types.Crewmate, cs(Medium.color, "Medium"), rates, null, true);
        mediumCooldown = Create(30311, Types.Crewmate, "通灵冷却", 7.5f, 2.5f, 120f, 2.5f, mediumSpawnRate);
        mediumDuration = Create(30312, Types.Crewmate, "通灵所需时间", 0.5f, 0f, 15f, 0.5f, mediumSpawnRate);
        mediumOneTimeUse = Create(30313, Types.Crewmate, "每个灵魂只能被通灵一次", false, mediumSpawnRate);
        mediumChanceAdditionalInfo = Create(30314, Types.Crewmate, "回答包含其他信息的可能性", rates, mediumSpawnRate);

        jumperSpawnRate = Create(30320, Types.Crewmate, cs(Jumper.color, "Jumper"), rates, null, true);
        jumperJumpTime = Create(30321, Types.Crewmate, "标记/传送冷却", 10f, 0f, 60f, 2.5f, jumperSpawnRate);
        jumperMaxCharges = Create(30325, Types.Crewmate, "可传送次数", 3, 0, 10, 1, jumperSpawnRate);
        jumperResetPlaceAfterMeeting = Create(30323, Types.Crewmate, "会议后重置标记位置", false, jumperSpawnRate);
        jumperChargesGainOnMeeting = Create(30324, Types.Crewmate, "会议后增加传送次数", 2, 0, 10, 1, jumperSpawnRate);
        /*
        magicianSpawnRate = CustomOption.Create(30330, Types.Crewmate, cs(Magician.color, "Magician"), rates, null, true);
        magicianCooldown = CustomOption.Create(30331, Types.Crewmate, "放置冷却", 15f, 0f, 60f, 2.5f, magicianSpawnRate);
        magicianTeleportTime = CustomOption.Create(30332, Types.Crewmate, "传送冷却", 15f, 0f, 60f, 2.5f, magicianSpawnRate);
        magicianProbabilityBlueCards = CustomOption.Create(30333, Types.Crewmate, "抽到蓝牌的概率", rates, magicianSpawnRate);
        magicianProbabilityRedCards = CustomOption.Create(30334, Types.Crewmate, "抽到红牌的概率", rates, magicianSpawnRate);
        magicianProbabilityPurpleCards = CustomOption.Create(30335, Types.Crewmate, "抽到紫牌的概率", rates, magicianSpawnRate);
        */
        trapperSpawnRate = Create(30350, Types.Crewmate, cs(Trapper.color, "Trapper"), rates, null, true);
        trapperCooldown = Create(30351, Types.Crewmate, "放置冷却", 20f, 5f, 120f, 2.5f, trapperSpawnRate);
        trapperMaxCharges = Create(30352, Types.Crewmate, "最大陷阱数", 4f, 1f, 15f, 1f, trapperSpawnRate);
        trapperRechargeTasksNumber = Create(30353, Types.Crewmate, "充能所需任务数", 2f, 1f, 15f, 1f, trapperSpawnRate);
        trapperTrapNeededTriggerToReveal = Create(30354, Types.Crewmate, "陷阱触发提示所需人数", 2f, 1f, 10f, 1f, trapperSpawnRate);
        trapperAnonymousMap = Create(30355, Types.Crewmate, "显示匿名地图", false, trapperSpawnRate);
        trapperInfoType = Create(30356, Types.Crewmate, "陷阱信息类型", ["职业", "善良/邪恶", "名字"], trapperSpawnRate);
        trapperTrapDuration = Create(30357, Types.Crewmate, "陷阱定身时间", 5f, 1f, 15f, 0.5f, trapperSpawnRate);

        //-------------------------- Modifier (1000 - 1999) -------------------------- //

        modifiersAreHidden = Create(40000, Types.Modifier, cs(Color.yellow, "隐藏死亡触发的附加职业"), true, null, true);

        modifierDisperser = Create(40100, Types.Modifier, cs(Palette.ImpostorRed, "Disperser"), rates, null, true);
        //modifierDisperserRemainingDisperses = CustomOption.Create(40102, Types.Modifier, "分散次数", 1f,1f,5f,1f, modifierDisperser);
        modifierDisperserDispersesToVent = Create(40101, Types.Modifier, "分散至管道位置", false, modifierDisperser);

        modifierLastImpostor = Create(40110, Types.Modifier, cs(Palette.ImpostorRed, "LastImpostor"), false, null, true);
        modifierLastImpostorDeduce = Create(40111, Types.Modifier, "绝境者击杀冷却减少", 5f, 2.5f, 15f, 2.5f, modifierLastImpostor);

        modifierBloody = Create(40120, Types.Modifier, cs(Color.yellow, "Bloody"), rates, null, true);
        modifierBloodyQuantity = Create(40121, Types.Modifier, cs(Color.yellow, "溅血数量"), ratesModifier, modifierBloody);
        modifierBloodyDuration = Create(40122, Types.Modifier, "痕迹持续时间", 10f, 3f, 60f, 0.5f, modifierBloody);

        modifierAntiTeleport = Create(40130, Types.Modifier, cs(Color.yellow, "AntiTeleport"), rates, null, true);
        modifierAntiTeleportQuantity = Create(40131, Types.Modifier, cs(Color.yellow, "通讯兵数量"), ratesModifier, modifierAntiTeleport);

        modifierTieBreaker = Create(40140, Types.Modifier, cs(Color.yellow, "TieBreaker"), rates, null, true);

        modifierBait = Create(40150, Types.Modifier, cs(Color.yellow, "Bait"), rates, null, true);
        modifierBaitSwapCrewmate = Create(40151, Types.Modifier, "只分配给船员阵营", false, modifierBait);
        modifierBaitReportDelayMin = Create(40152, Types.Modifier, "诱饵报告延迟时间(最小)", 0f, 0f, 10f, 0.125f, modifierBait);
        modifierBaitReportDelayMax = Create(40153, Types.Modifier, "诱饵报告延迟时间(最大)", 0f, 0f, 10f, 0.5f, modifierBait);
        modifierBaitShowKillFlash = Create(40154, Types.Modifier, "用闪光灯警告杀手", true, modifierBait);

        modifierLover = Create(40160, Types.Modifier, cs(Color.yellow, "Lover"), rates, null, true);
        modifierLoverImpLoverRate = Create(40161, Types.Modifier, "恋人中有内鬼的概率", rates, modifierLover);
        modifierLoverBothDie = Create(40162, Types.Modifier, "恋人共死", true, modifierLover);
        modifierLoverEnableChat = Create(40163, Types.Modifier, "启用私密聊天文字频道", true, modifierLover);

        modifierSunglasses = Create(40170, Types.Modifier, cs(Color.yellow, "Sunglasses"), rates, null, true);
        modifierSunglassesQuantity = Create(40171, Types.Modifier, cs(Color.yellow, "太阳镜数量"), ratesModifier, modifierSunglasses);
        modifierSunglassesVision = Create(40172, Types.Modifier, "太阳镜的视野倍率", ["-10%", "-20%", "-30%", "-40%", "-50%"], modifierSunglasses);

        modifierTorch = Create(40180, Types.Modifier, cs(Color.yellow, "Torch"), rates, null, true);
        modifierTorchQuantity = Create(40181, Types.Modifier, cs(Color.yellow, "火炬人数"), ratesModifier, modifierTorch);
        modifierTorchVision = Create(40182, Types.Modifier, "火炬的视野倍率", 1.5f, 1f, 3f, 0.125f, modifierTorch);

        modifierFlash = Create(40190, Types.Modifier, cs(Color.yellow, "Flash"), rates, null, true);
        modifierFlashQuantity = Create(40191, Types.Modifier, cs(Color.yellow, "闪电侠人数"), ratesModifier, modifierFlash);
        modifierFlashSpeed = Create(40192, Types.Modifier, "闪电侠的移速倍率", 1.25f, 1f, 3f, 0.125f, modifierFlash);

        modifierMultitasker = Create(40200, Types.Modifier, cs(Color.yellow, "Multitasker"), rates, null, true);
        modifierMultitaskerQuantity = Create(40201, Types.Modifier, cs(Color.yellow, "多线程人数"), ratesModifier, modifierMultitasker);

        modifierMini = Create(40210, Types.Modifier, cs(Color.yellow, "Mini"), rates, null, true);
        modifierMiniGrowingUpDuration = Create(40211, Types.Modifier, "小孩长大所需时间", 400f, 100f, 1500f, 25f, modifierMini);
        modifierMiniGrowingUpInMeeting = Create(40212, Types.Modifier, "小孩会议期间可成长", true, modifierMini);

        modifierGiant = Create(40220, Types.Modifier, cs(Color.yellow, "Giant"), rates, null, true);
        modifierGiantSpped = Create(40221, Types.Modifier, "巨人的移速倍率", 0.75f, 0.5f, 1.5f, 0.05f, modifierGiant);

        modifierIndomitable = Create(40230, Types.Modifier, cs(Color.yellow, "Indomitable"), rates, null, true);

        modifierBlind = Create(40240, Types.Modifier, cs(Color.yellow, "Blind"), rates, null, true);

        modifierWatcher = Create(40250, Types.Modifier, cs(Color.yellow, "Watcher"), rates, null, true);

        modifierRadar = Create(40260, Types.Modifier, cs(Color.yellow, "Radar"), rates, null, true);

        modifierTunneler = Create(40270, Types.Modifier, cs(Color.yellow, "Tunneler"), rates, null, true);

        modifierButtonBarry = Create(40280, Types.Modifier, cs(Color.yellow, "ButtonBarry"), rates, null, true);
        modifierButtonSabotageRemoteMeetings = Create(40281, Types.Modifier, "可在破坏时使用", false, modifierButtonBarry);

        modifierSlueth = Create(40290, Types.Modifier, cs(Color.yellow, "Slueth"), rates, null, true);

        modifierCursed = Create(40300, Types.Modifier, cs(Color.yellow, "Cursed"), rates, null, true);
        modifierShowCursed = Create(40301, Types.Modifier, "隐藏职业", false, modifierCursed);

        modifierVip = Create(40310, Types.Modifier, cs(Color.yellow, "Vip"), rates, null, true);
        modifierVipQuantity = Create(40311, Types.Modifier, cs(Color.yellow, "VIP人数"), ratesModifier, modifierVip);
        modifierVipShowColor = Create(40312, Types.Modifier, "死亡时全场提示阵营颜色", true, modifierVip);

        modifierInvert = Create(40320, Types.Modifier, cs(Color.yellow, "Invert"), rates, null, true);
        modifierInvertQuantity = Create(40321, Types.Modifier, cs(Color.yellow, "酒鬼人数"), ratesModifier, modifierInvert);
        modifierInvertDuration = Create(40322, Types.Modifier, "醉酒状态持续几轮会议", 2f, 1f, 15f, 1f, modifierInvert);

        modifierChameleon = Create(40330, Types.Modifier, cs(Color.yellow, "Chameleon"), rates, null, true);
        modifierChameleonQuantity = Create(40331, Types.Modifier, cs(Color.yellow, "变色龙数量"), ratesModifier, modifierChameleon);
        modifierChameleonHoldDuration = Create(40332, Types.Modifier, "从不动到褪色开始的间隔时间", 3f, 1f, 10f, 0.5f, modifierChameleon);
        modifierChameleonFadeDuration = Create(40333, Types.Modifier, "褪色过程持续时间", 1f, 0.25f, 10f, 0.25f, modifierChameleon);
        modifierChameleonMinVisibility = Create(40334, Types.Modifier, "最低透明度", ["0%", "10%", "20%", "30%", "40%", "50%"], modifierChameleon);

        modifierShifter = Create(40340, Types.Modifier, cs(Color.yellow, "Shifter"), rates, null, true);
        modifierShiftNeutral = Create(40341, Types.Modifier, "可交换部分中立不带刀职业", false, modifierShifter);


        //-------------------------- Guesser Gamemode 2000 - 2999 -------------------------- //

        guesserGamemodeCrewNumber = Create(2001, Types.Guesser, cs(Guesser.color, "船员阵营赌怪数"), 3f, 0f, 15f, 1f, null, true);
        guesserGamemodeNeutralNumber = Create(2002, Types.Guesser, cs(Guesser.color, "中立阵营赌怪数"), 3f, 0f, 15f, 1f);
        guesserGamemodeImpNumber = Create(2003, Types.Guesser, cs(Guesser.color, "伪装者阵营赌怪数"), 3f, 0f, 15f, 1f);
        guesserForceJackalGuesser = Create(2007, Types.Guesser, "强制豺狼成为赌怪", false, null, true);
        guesserGamemodeSidekickIsAlwaysGuesser = Create(2012, Types.Guesser, "强制跟班成为赌怪", false);
        guesserForceThiefGuesser = Create(2011, Types.Guesser, "强制身份窃贼为赌怪", false);
        guesserGamemodeHaveModifier = Create(2004, Types.Guesser, "赌怪可以拥有附加职业", true, null, true);
        guesserGamemodeNumberOfShots = Create(2005, Types.Guesser, "赌怪猜测最大次数", 2f, 1f, 15f, 1f);
        guesserGamemodeHasMultipleShotsPerMeeting = Create(2006, Types.Guesser, "一轮会议可多次猜测", false);
        guesserGamemodeKillsThroughShield = Create(2008, Types.Guesser, "赌怪猜测无视护盾", true);
        guesserGamemodeEvilCanKillSpy = Create(2009, Types.Guesser, "邪恶的赌怪可猜测卧底", true);
        guesserGamemodeCantGuessSnitchIfTaksDone = Create(2010, Types.Guesser, "赌怪不可猜测已完成任务的告密者", true);

        //-------------------------- Hide N Seek 3000 - 3999 -------------------------- //

        hideNSeekMap = Create(3020, Types.HideNSeekMain, cs(Color.yellow, "CustomMap"),
            ["Skeld", "Mira", "Polus", "Airship", "Fungle", "Submerged", "LevelImpostor"], null, true, () =>
            {
                var map = hideNSeekMap.selection;
                if (map >= 3) map++;
                GameOptionsManager.Instance.currentNormalGameOptions.MapId = (byte)map;
            });
        hideNSeekHunterCount = Create(3000, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekHunterCount"), 1f, 1f, 3f, 1f);
        hideNSeekKillCooldown = Create(3021, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekKillCooldown"), 10f, 2.5f, 60f, 2.5f);
        hideNSeekHunterVision = Create(3001, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekHunterVision"), 0.5f, 0.25f, 2f, 0.25f);
        hideNSeekHuntedVision = Create(3002, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekHuntedVision"), 2f, 0.25f, 5f, 0.25f);
        hideNSeekCommonTasks = Create(3023, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekCommonTasks"), 1f, 0f, 4f, 1f);
        hideNSeekShortTasks = Create(3024, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekShortTasks"), 3f, 1f, 23f, 1f);
        hideNSeekLongTasks = Create(3025, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekLongTasks"), 3f, 0f, 15f, 1f);
        hideNSeekTimer = Create(3003, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekTimer"), 5f, 1f, 30f, 0.5f);
        hideNSeekTaskWin = Create(3004, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekTaskWin"), false);
        hideNSeekTaskPunish = Create(3017, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekTaskPunish"), 10f, 0f, 30f, 1f);
        hideNSeekCanSabotage = Create(3019, Types.HideNSeekMain, cs(Color.yellow, "hideNSeekCanSabotage"), false);
        hideHuntSpeed = Create(3027, Types.HideNSeekMain, cs(Color.yellow, "hideHuntSpeed"), 1f, 0.5f, 2f, 0.125f);
        hideCrewSpeed = Create(3028, Types.HideNSeekMain, cs(Color.yellow, "hideCrewSpeed"), 1f, 0.5f, 2f, 0.125f);

        hideNSeekHunterWaiting = Create(3022, Types.HideNSeekMain, cs(Color.red, "hideNSeekHunterWaiting"), 15f, 2.5f, 60f, 2.5f, null, true);
        hunterLightCooldown = Create(3005, Types.HideNSeekRoles, cs(Color.red, "hunterLightCooldown"), 30f, 5f, 60f, 1f);
        hunterLightDuration = Create(3006, Types.HideNSeekRoles, cs(Color.red, "hunterLightDuration"), 10f, 1f, 60f, 1f);
        hunterLightVision = Create(3007, Types.HideNSeekRoles, cs(Color.red, "hunterLightVision"), 2f, 1f, 5f, 0.25f);
        hunterLightPunish = Create(3008, Types.HideNSeekRoles, cs(Color.red, "hunterLightPunish"), 5f, 0f, 30f, 1f);
        hunterAdminCooldown = Create(3009, Types.HideNSeekRoles, cs(Color.red, "hunterAdminCooldown"), 30f, 5f, 60f, 1f);
        hunterAdminDuration = Create(3010, Types.HideNSeekRoles, cs(Color.red, "hunterAdminDuration"), 5f, 1f, 60f, 1f);
        hunterAdminPunish = Create(3011, Types.HideNSeekRoles, cs(Color.red, "hunterAdminPunish"), 5f, 0f, 30f, 1f);
        hunterArrowCooldown = Create(3012, Types.HideNSeekRoles, cs(Color.red, "hunterArrowCooldown"), 30f, 5f, 60f, 1f);
        hunterArrowDuration = Create(3013, Types.HideNSeekRoles, cs(Color.red, "hunterArrowDuration"), 5f, 0f, 60f, 1f);
        hunterArrowPunish = Create(3014, Types.HideNSeekRoles, cs(Color.red, "hunterArrowPunish"), 5f, 0f, 30f, 1f);

        huntedShieldCooldown = Create(3015, Types.HideNSeekRoles, cs(Color.gray, "huntedShieldCooldown"), 30f, 5f, 60f, 1f, null, true);
        huntedShieldDuration = Create(3016, Types.HideNSeekRoles, cs(Color.gray, "huntedShieldDuration"), 5f, 1f, 60f, 1f);
        huntedShieldRewindTime = Create(3018, Types.HideNSeekRoles, cs(Color.gray, "huntedShieldRewindTime"), 3f, 1f, 10f, 1f);
        huntedShieldNumber = Create(3026, Types.HideNSeekRoles, cs(Color.grey, "huntedShieldNumber"), 3f, 1f, 15f, 1f);

        //-------------------------- Prop Hunt General Options 4000 - 4999 -------------------------- //

        propHuntMap = Create(4020, Types.PropHunt, cs(Color.yellow, "CustomMap"),
            ["Skeld", "Mira", "Polus", "Airship", "Fungle", "Submerged", "LevelImpostor"], null, true, () =>
            {
                var map = propHuntMap.selection;
                if (map >= 3) map++;
                GameOptionsManager.Instance.currentNormalGameOptions.MapId = (byte)map;
            });
        propHuntTimer = Create(4021, Types.PropHunt, cs(Color.yellow, "propHuntTimer"), 5f, 1f, 30f, 0.5f);
        propHunterVision = Create(4006, Types.PropHunt, cs(Color.yellow, "propHunterVision"), 0.5f, 0.25f, 2f, 0.25f);
        propVision = Create(4007, Types.PropHunt, cs(Color.yellow, "propVision"), 2f, 0.25f, 5f, 0.25f);
        propHuntSpeed = Create(4025, Types.PropHunt, cs(Color.yellow, "propHuntSpeed"), 1f, 0.5f, 2f, 0.125f);
        propPropSpeed = Create(4026, Types.PropHunt, cs(Color.yellow, "propPropSpeed"), 1f, 0.5f, 2f, 0.125f);

        propHuntEnableUnstuck = Create(4027, Types.PropHunt, cs(Color.yellow, "启用穿墙技能"), ["关闭", "猎人", "躲藏者", "开启"], null, true);
        propHuntUnstuckCooldown = Create(4011, Types.PropHunt, cs(Color.yellow, "穿墙冷却时间"), 30f, 2.5f, 60f, 2.5f, propHuntEnableUnstuck);
        propHuntUnstuckDuration = Create(4012, Types.PropHunt, cs(Color.yellow, "穿墙持续时间"), 2f, 1f, 60f, 1f, propHuntEnableUnstuck);
        // Hunter Options
        propHuntNumberOfHunters = Create(4000, Types.PropHunt, cs(Color.red, "propHuntNumberOfHunters"), 1f, 1f, 5f, 1f, null, true);
        hunterInitialBlackoutTime = Create(4001, Types.PropHunt, cs(Color.red, "hunterInitialBlackoutTime"), 10f, 5f, 20f, 1f);
        hunterMissCooldown = Create(4004, Types.PropHunt, cs(Color.red, "hunterMissCooldown"), 10f, 2.5f, 60f, 2.5f);
        hunterHitCooldown = Create(4005, Types.PropHunt, cs(Color.red, "hunterHitCooldown"), 10f, 2.5f, 60f, 2.5f);
        propHuntRevealCooldown = Create(4008, Types.PropHunt, cs(Color.red, "propHuntRevealCooldown"), 30f, 10f, 90f, 2.5f);
        propHuntRevealDuration = Create(4009, Types.PropHunt, cs(Color.red, "propHuntRevealDuration"), 5f, 1f, 60f, 1f);
        propHuntRevealPunish = Create(4010, Types.PropHunt, cs(Color.red, "propHuntRevealPunish"), 10f, 0f, 1800f, 5f);
        propHuntAdminCooldown = Create(4022, Types.PropHunt, cs(Color.red, "propHuntAdminCooldown"), 30f, 2.5f, 1800f, 2.5f);
        propHuntFindCooldown = Create(4023, Types.PropHunt, cs(Color.red, "propHuntFindCooldown"), 60f, 2.5f, 1800f, 2.5f);
        propHuntFindDuration = Create(4024, Types.PropHunt, cs(Color.red, "propHuntFindDuration"), 5f, 1f, 15f, 1f);
        // Prop Options
        propBecomesHunterWhenFound = Create(4003, Types.PropHunt, cs(Palette.CrewmateBlue, "propBecomesHunterWhenFound"), false, null, true);
        propHuntInvisEnabled = Create(4013, Types.PropHunt, cs(Palette.CrewmateBlue, "propHuntInvisEnabled"), true, null, true);
        propHuntInvisCooldown = Create(4014, Types.PropHunt, cs(Palette.CrewmateBlue, "propHuntInvisCooldown"), 40f, 10f, 120f, 2.5f, propHuntInvisEnabled);
        propHuntInvisDuration = Create(4015, Types.PropHunt, cs(Palette.CrewmateBlue, "propHuntInvisDuration"), 5f, 2.5f, 30f, 2.5f, propHuntInvisEnabled);
        propHuntSpeedboostEnabled = Create(4016, Types.PropHunt, cs(Palette.CrewmateBlue, "propHuntSpeedboostEnabled"), true, null, true);
        propHuntSpeedboostCooldown = Create(4017, Types.PropHunt, cs(Palette.CrewmateBlue, "propHuntSpeedboostCooldown"), 45f, 2.5f, 120f, 2.5f, propHuntSpeedboostEnabled);
        propHuntSpeedboostDuration = Create(4018, Types.PropHunt, cs(Palette.CrewmateBlue, "propHuntSpeedboostDuration"), 10f, 2.5f, 30f, 2.5f, propHuntSpeedboostEnabled);
        propHuntSpeedboostSpeed = Create(4019, Types.PropHunt, cs(Palette.CrewmateBlue, "propHuntSpeedboostSpeed"), 2f, 1.25f, 5f, 0.25f, propHuntSpeedboostEnabled);

        blockedRolePairings.Add((byte)RoleId.Vampire, [(byte)RoleId.Warlock]);
        blockedRolePairings.Add((byte)RoleId.Witch, [(byte)RoleId.Warlock]);
        blockedRolePairings.Add((byte)RoleId.Warlock, [(byte)RoleId.Vampire]);

        blockedRolePairings.Add((byte)RoleId.Vulture, [(byte)RoleId.Cleaner]);
        blockedRolePairings.Add((byte)RoleId.Cleaner, [(byte)RoleId.Vulture]);

        blockedRolePairings.Add((byte)RoleId.Ninja, [(byte)RoleId.Swooper]);
        blockedRolePairings.Add((byte)RoleId.Swooper, [(byte)RoleId.Ninja]);
    }
}
