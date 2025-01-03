using UnityEngine;
using static TheOtherRoles.Options.CustomOption;
using Types = TheOtherRoles.Options.CustomOption.CustomOptionType;

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
    public static CustomOption neutralRolesCountMin;
    public static CustomOption neutralRolesCountMax;
    public static CustomOption killerNeutralRolesCountMin;
    public static CustomOption killerNeutralRolesCountMax;
    public static CustomOption modifiersCountMin;
    public static CustomOption modifiersCountMax;

    public static CustomOption anyPlayerCanStopStart;
    public static CustomOption enableEventMode;
    public static CustomOption deadImpsBlockSabotage;
    public static CustomOption randomLigherPlayer;

    public static CustomOption minerSpawnRate;
    public static CustomOption minerCooldown;

    public static CustomOption yoyoSpawnRate;
    public static CustomOption yoyoBlinkDuration;
    public static CustomOption yoyoMarkCooldown;
    public static CustomOption yoyoMarkStaysOverMeeting;
    public static CustomOption yoyoHasAdminTable;
    public static CustomOption yoyoAdminTableCooldown;
    public static CustomOption yoyoSilhouetteVisibility;

    public static CustomOption wolfLordSpawnRate;

    public static CustomOption morphlingSpawnRate;
    public static CustomOption morphlingCooldown;
    public static CustomOption morphlingDuration;

    public static CustomOption bomberSpawnRate;
    public static CustomOption bomberBombCooldown;
    public static CustomOption bomberDelay;
    public static CustomOption bomberTimer;
    public static CustomOption bomberTriggerBothCooldowns;
    public static CustomOption bomberCanGiveToBomber;
    public static CustomOption bomberHotPotatoMode;

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
    public static CustomOption modifierPoucher;

    public static CustomOption modifierVortox;
    public static CustomOption modifierVortoxSkipMeeting;
    public static CustomOption modifierVortoxSkipNum;

    public static CustomOption butcherSpawnRate;
    public static CustomOption butcherDissectionCooldown;
    public static CustomOption butcherDissectionDuration;
    public static CustomOption butcherDissectedBodyCount;

    public static CustomOption mimicSpawnRate;

    public static CustomOption eraserSpawnRate;
    public static CustomOption eraserCooldown;
    public static CustomOption eraserCanEraseAnyone;
    public static CustomOption erasercanEraseGuess;

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

    public static CustomOption survivorSpawnRate;
    public static CustomOption survivorVestEnable;
    public static CustomOption survivorVestNumber;
    public static CustomOption survivorVestCooldown;
    public static CustomOption survivorVestDuration;
    public static CustomOption survivorVestResetCooldown;
    public static CustomOption survivorBlanksEnable;
    public static CustomOption survivorBlanksCooldown;
    public static CustomOption survivorBlanksNumber;

    public static CustomOption amnisiacSpawnRate;
    public static CustomOption amnisiacShowArrows;
    public static CustomOption amnisiacResetRole;

    public static CustomOption arsonistSpawnRate;
    public static CustomOption arsonistCooldown;
    public static CustomOption arsonistDuration;
    public static CustomOption arsonistIgniteCdRemoved;

    public static CustomOption jackalSpawnRate;
    public static CustomOption jackalKillCooldown;
    public static CustomOption jackalChanceSwoop;
    public static CustomOption jackalSwooperCooldown;
    public static CustomOption jackalSwooperDuration;
    public static CustomOption jackalCreateSidekickCooldown;
    public static CustomOption jackalCanUseVents;
    public static CustomOption jackalCanUseSabo;
    public static CustomOption jackalCanCreateSidekick;
    public static CustomOption sidekickPromotesToJackal;
    public static CustomOption sidekickCanKill;
    public static CustomOption sidekickCanUseVents;
    public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;
    public static CustomOption jackalAndSidekickHaveImpostorVision;
    public static CustomOption jackalkillFakeImpostor;

    public static CustomOption pavlovsownerSpawnRate;
    public static CustomOption pavlovsownerAndJackalAsWell;
    public static CustomOption pavlovsownerCreateDogCooldown;
    public static CustomOption pavlovsownerCreateDogNum;
    public static CustomOption pavlovsownerKillCooldown;
    public static CustomOption pavlovsownerCanUseSabo;
    public static CustomOption pavlovsownerHasImpostorVision;
    public static CustomOption pavlovsownerCanUseVents;
    public static CustomOption pavlovsownerRampage;
    public static CustomOption pavlovsownerRampageKillCooldown;
    public static CustomOption pavlovsownerRampageDeathTime;

    public static CustomOption evilTrapperSpawnRate;
    public static CustomOption evilTrapperNumTrap;
    public static CustomOption evilTrapperKillTimer;
    public static CustomOption evilTrapperCooldown;
    public static CustomOption evilTrapperMaxDistance;
    public static CustomOption evilTrapperTrapRange;
    public static CustomOption evilTrapperExtensionTime;
    public static CustomOption evilTrapperPenaltyTime;
    public static CustomOption evilTrapperBonusTime;

    public static CustomOption gamblerSpawnRate;
    public static CustomOption gamblerMinCooldown;
    public static CustomOption gamblerMaxCooldown;
    public static CustomOption gamblerSuccessRate;

    public static CustomOption grenadierSpawnRate;
    public static CustomOption grenadierCooldown;
    public static CustomOption grenadierDuration;
    public static CustomOption grenadierFlashRadius;
    public static CustomOption grenadierTeamIndicators;

    public static CustomOption swooperSpawnRate;
    public static CustomOption swooperKillCooldown;
    public static CustomOption swooperCooldown;
    public static CustomOption swooperDuration;
    public static CustomOption swooperSpeed;
    public static CustomOption swooperCanUseVents;
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
    public static CustomOption mayorMeetingButton;
    public static CustomOption mayorMaxRemoteMeetings;
    public static CustomOption mayorSabotageRemoteMeetings;
    public static CustomOption mayorVote;
    public static CustomOption mayorRevealVision;

    public static CustomOption prosecutorSpawnRate;
    public static CustomOption prosecutorCanSeeVoteColors;
    public static CustomOption prosecutorTasksNeededToSeeVoteColors;
    public static CustomOption prosecutorDiesOnIncorrectPros;
    public static CustomOption prosecutorCanCallEmergency;

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

    public static CustomOption ghostEngineerSpawnRate;

    public static CustomOption specterSpawnRate;
    public static CustomOption specterResetRole;
    public static CustomOption specterAfterMeeting;

    public static CustomOption sheriffSpawnRate;
    public static CustomOption sheriffMisfireKills;
    public static CustomOption sheriffCooldown;
    public static CustomOption sheriffCanKillNeutrals;
    public static CustomOption sheriffCanKillLawyer;
    public static CustomOption sheriffCanKillSurvivor;
    public static CustomOption sheriffCanKillExecutioner;
    public static CustomOption sheriffCanKillJester;
    public static CustomOption sheriffCanKillVulture;
    public static CustomOption sheriffCanKillThief;
    public static CustomOption sheriffCanKillAmnesiac;
    public static CustomOption sheriffCanKillPursuer;
    public static CustomOption sheriffCanKillPartTimer;
    public static CustomOption sheriffCanKillDoomsayer;
    public static CustomOption deputySpawnRate;

    public static CustomOption deputyNumberOfHandcuffs;
    public static CustomOption deputyHandcuffCooldown;
    public static CustomOption deputyGetsPromoted;
    public static CustomOption deputyKeepsHandcuffs;
    public static CustomOption deputyHandcuffDuration;
    public static CustomOption deputyKnowsSheriff;

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

    public static CustomOption veteranSpawnRate;
    public static CustomOption veteranCooldown;
    public static CustomOption veteranAlertDuration;

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
    //public static CustomOption lawyerIsProsecutorChance;
    public static CustomOption lawyerTargetCanBeJester;
    public static CustomOption lawyerVision;
    public static CustomOption lawyerKnowsRole;
    public static CustomOption lawyerStolenWin;
    public static CustomOption lawyerCanCallEmergency;

    //public static CustomOption pursuerSpawnRate;
    public static CustomOption pursuerBlanksCooldown;
    public static CustomOption pursuerBlanksNumber;

    public static CustomOption partTimerSpawnRate;
    public static CustomOption partTimerCooldown;
    public static CustomOption partTimerDeathTurn;
    public static CustomOption partTimerKnowsRole;

    public static CustomOption executionerSpawnRate;
    public static CustomOption executionerCanCallEmergency;
    public static CustomOption executionerPromotesToLawyer;
    public static CustomOption executionerOnTargetDead;

    public static CustomOption witnessSpawnRate;
    public static CustomOption witnessMarkTimer;
    public static CustomOption witnessWinCount;
    public static CustomOption witnessMeetingDie;
    public static CustomOption witnessSkipMeeting;

    public static CustomOption balancerSpawnRate;
    public static CustomOption balancerCount;
    public static CustomOption balancerVoteTime;

    public static CustomOption jumperSpawnRate;
    public static CustomOption jumperJumpTime;
    public static CustomOption jumperResetPlaceAfterMeeting;
    public static CustomOption jumperChargesGainOnMeeting;
    public static CustomOption jumperMaxCharges;

    public static CustomOption escapistSpawnRate;
    public static CustomOption escapistEscapeTime;
    public static CustomOption escapistResetPlaceAfterMeeting;

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
    public static CustomOption thiefCanKillVeteran;
    public static CustomOption thiefCanStealWithGuess;

    public static CustomOption juggernautSpawnRate;
    public static CustomOption juggernautCooldown;
    public static CustomOption juggernautHasImpVision;
    public static CustomOption juggernautCanUseVents;
    public static CustomOption juggernautReducedkillEach;

    public static CustomOption doomsayerSpawnRate;
    public static CustomOption doomsayerCooldown;
    public static CustomOption doomsayerHasMultipleShotsPerMeeting;
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
    public static CustomOption akujoHonmeiOptimizeWin;

    public static CustomOption trapperSpawnRate;
    public static CustomOption trapperCooldown;
    public static CustomOption trapperMaxCharges;
    public static CustomOption trapperRechargeTasksNumber;
    public static CustomOption trapperTrapNeededTriggerToReveal;
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

    public static CustomOption infoSleuthSpawnRate;
    public static CustomOption infoSleuthInfoType;

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

    public static CustomOption modifierBait;
    public static CustomOption modifierBaitReportDelayMin;
    public static CustomOption modifierBaitReportDelayMax;
    public static CustomOption modifierBaitShowKillFlash;
    public static CustomOption modifierBaitSwapCrewmate;
    //public static CustomOption modifierBaitSwapNeutral;
    //public static CustomOption modifierBaitSwapImpostor;

    public static CustomOption modifierAftermath;

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
    public static CustomOption modifierHideCursed;

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
    public static CustomOption modifierShiftALLNeutral;

    public static CustomOption modifierSpecoality;
    public static CustomOption modifierSpecoalityIsGlobal;

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

    //public static CustomOption enableMapOptions;
    public static CustomOption randomGameStartPosition;
    public static CustomOption randomGameStartToVents;
    public static CustomOption allowModGuess;
    public static CustomOption ghostSpeed;
    public static CustomOption finishTasksBeforeHauntingOrZoomingOut;
    public static CustomOption disableSabotage;
    public static CustomOption camsNightVision;
    public static CustomOption camsNoNightVisionIfImpVision;

    public static CustomOption disableTaskGameEnd;
    public static CustomOption dynamicMap;
    public static CustomOption dynamicMapEnableSkeld;
    public static CustomOption dynamicMapEnableMira;
    public static CustomOption dynamicMapEnablePolus;
    public static CustomOption dynamicMapEnableAirShip;
    public static CustomOption dynamicMapEnableFungle;
    public static CustomOption dynamicMapEnableSubmerged;
    public static CustomOption dynamicMapSeparateSettings;

    public static CustomOption debugMode;
    public static CustomOption disableGameEnd;

    public static CustomOption enableBetterPolus;

    public static CustomOption movePolusVents;

    public static CustomOption guessReVote;
    public static CustomOption guessExtendmeetingTime;

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
    public static CustomOption guesserGamemodePavlovsdogIsAlwaysGuesser;
    public static CustomOption guesserForcePavlovsGuesser;
    public static CustomOption guesserForceThiefGuesser;
    public static CustomOption guesserGamemodeHaveModifier;
    public static CustomOption guesserGamemodeNumberOfShots;
    public static CustomOption guesserGamemodeHasMultipleShotsPerMeeting;
    public static CustomOption guesserGamemodeKillsThroughShield;
    public static CustomOption guesserGamemodeEvilCanKillSpy;
    public static CustomOption guesserGamemodeCantGuessSnitchIfTaksDone;

    public static string cs(Color c, string s)
    {
        return $"<color=#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}{ToByte(c.a):X2}>{s}</color>";
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static void Load()
    {
        vanillaSettings = TheOtherRolesPlugin.Instance.Config.Bind("Preset0", "VanillaOptions", "");

        // Role Options
        presetSelection = Create(0, Types.General, cs(new Color32(204, 204, 0, 255), "presetSelection"), presets, null, true);

        anyPlayerCanStopStart = Create(3, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0), "anyPlayerCanStopStart"), false, null, false);

        neutralRolesCountMin = Create(8, Types.General, cs(new Color32(204, 204, 0, 255), "neutralRolesCountMin"), 2f, 0f, 15f, 1f, null, true);
        neutralRolesCountMax = Create(9, Types.General, cs(new Color32(204, 204, 0, 255), "neutralRolesCountMax"), 2f, 0f, 15f, 1f);
        killerNeutralRolesCountMin = Create(10, Types.General, cs(new Color32(204, 204, 0, 255), "killerNeutralRolesCountMin"), ratesRandom);
        killerNeutralRolesCountMax = Create(11, Types.General, cs(new Color32(204, 204, 0, 255), "killerNeutralRolesCountMax"), ratesRandom);
        modifiersCountMin = Create(12, Types.General, cs(new Color32(204, 204, 0, 255), "modifiersCountMin"), 15f, 0f, 30f, 1f);
        modifiersCountMax = Create(13, Types.General, cs(new Color32(204, 204, 0, 255), "modifiersCountMax"), 15f, 0f, 30f, 1f);

        //-------------------------- Other options 1 - 599 -------------------------- //

        resteButtonCooldown = Create(20, Types.General, "resteButtonCooldown", 20f, 2.5f, 30f, 2.5f, null, true);
        maxNumberOfMeetings = Create(21, Types.General, "maxNumberOfMeetings", 10, 0, 15, 1, null, true);
        blockSkippingInEmergencyMeetings = Create(22, Types.General, "blockSkippingInEmergencyMeetings", false);
        noVoteIsSelfVote = Create(23, Types.General, "noVoteIsSelfVote", false, blockSkippingInEmergencyMeetings);
        shieldFirstKill = Create(24, Types.General, "shieldFirstKill", false);
        hidePlayerNames = Create(25, Types.General, "hidePlayerNames", false);
        hideOutOfSightNametags = Create(26, Types.General, "hideOutOfSightNametags", true);
        hideVentAnimOnShadows = Create(27, Types.General, "hideVentAnimOnShadows", false);
        showButtonTarget = Create(28, Types.General, "showButtonTarget", true);
        impostorSeeRoles = Create(30, Types.General, cs(Palette.ImpostorRed, "impostorSeeRoles"), false);
        blockGameEnd = Create(29, Types.General, cs(Color.yellow, "blockGameEnd"), true);
        deadImpsBlockSabotage = Create(32, Types.General, cs(Palette.ImpostorRed, "deadImpsBlockSabotage"), false);
        randomLigherPlayer = Create(34, Types.General, "randomLigherPlayer", true);
        disableSabotage = Create(35, Types.General, cs(Palette.ImpostorRed, "disableSabotage"), false, null);
        allowModGuess = Create(31, Types.General, "allowModGuess", false);
        //ghostSpeed = Create(33, Types.General, "ghostSpeed", 1f, 0.75f, 5f, 0.125f);

        guessReVote = Create(221, Types.General, "guessReVote", false, null, true);
        guessExtendmeetingTime = Create(222, Types.General, "guessExtendmeetingTime", 15f, 0f, 60f, 5f);

        WireTaskIsRandomOption = Create(44, Types.General, "WireTaskIsRandomOption", false, null, true);
        WireTaskNumOption = Create(45, Types.General, "WireTaskNumOption", 3f, 1f, 8f, 1f, WireTaskIsRandomOption);
        transparentTasks = Create(40, Types.General, "transparentTasks", false);
        disableMedbayWalk = Create(41, Types.General, "disableMedbayWalk", false);
        allowParallelMedBayScans = Create(44, Types.General, "allowParallelMedBayScans", false);
        finishTasksBeforeHauntingOrZoomingOut = Create(42, Types.General, "finishTasksBeforeHauntingOrZoomingOut", false);
        disableTaskGameEnd = Create(43, Types.General, "disableTaskGameEnd", false);

        //Map options
        //enableMapOptions = Create(200, Types.General, "enableMapOptions", false, null, true);

        IsReactorDurationSetting = Create(201, Types.General, "IsReactorDurationSetting", false, null, true);
        SkeldReactorTimeLimit = Create(202, Types.General, "SkeldReactorTimeLimit", 30f, 0f, 30f, 2.5f, IsReactorDurationSetting);
        SkeldLifeSuppTimeLimit = Create(203, Types.General, "SkeldLifeSuppTimeLimit", 30f, 0f, 30f, 2.5f, IsReactorDurationSetting);
        MiraLifeSuppTimeLimit = Create(204, Types.General, "MiraLifeSuppTimeLimit", 30f, 0f, 45f, 2.5f, IsReactorDurationSetting);
        MiraReactorTimeLimit = Create(205, Types.General, "MiraReactorTimeLimit", 30f, 0f, 45f, 2.5f, IsReactorDurationSetting);
        PolusReactorTimeLimit = Create(206, Types.General, "PolusReactorTimeLimit", 60f, 0f, 60f, 2.5f, IsReactorDurationSetting);
        AirshipReactorTimeLimit = Create(207, Types.General, "AirshipReactorTimeLimit", 75f, 0f, 90f, 2.5f, IsReactorDurationSetting);
        FungleReactorTimeLimit = Create(208, Types.General, "FungleReactorTimeLimit", 45f, 0f, 60f, 2.5f, IsReactorDurationSetting);

        randomGameStartPosition = Create(50, Types.General, "randomGameStartPosition", false, null, true);
        randomGameStartToVents = Create(51, Types.General, "randomGameStartToVents", true, randomGameStartPosition);

        enableMiraModify = Create(70, Types.General, cs(Color.yellow, "Mira"), false, null, true);
        miraVitals = Create(71, Types.General, "miraVitals", false, enableMiraModify);

        enableBetterPolus = Create(80, Types.General, cs(Color.yellow, "Polus"), false, null);
        movePolusVents = Create(81, Types.General, "movePolusVents", false, enableBetterPolus);
        addPolusVents = Create(82, Types.General, "addPolusVents", false, enableBetterPolus);
        movePolusVitals = Create(83, Types.General, "movePolusVitals", false, enableBetterPolus);
        swapNavWifi = Create(84, Types.General, "swapNavWifi", false, enableBetterPolus);
        moveColdTemp = Create(85, Types.General, "moveColdTemp", false, enableBetterPolus);

        enableAirShipModify = Create(90, Types.General, cs(Color.yellow, "AirShip"), false, null);
        airshipOptimize = Create(91, Types.General, "airshipOptimize", false, enableAirShipModify);
        addAirShipVents = Create(92, Types.General, "addAirShipVents", false, enableAirShipModify);
        airshipLadder = Create(93, Types.General, "airshipLadder", false, enableAirShipModify);

        enableFungleModify = Create(100, Types.General, cs(Color.yellow, "Fungle"), false, null);
        fungleElectrical = Create(101, Types.General, "fungleElectrical", false, enableFungleModify);

        enableCamoComms = Create(120, Types.General, cs(Palette.ImpostorRed, "enableCamoComms"), false, null, true);
        //fungleDisableCamoComms = Create(211, Types.General, "fungleDisableCamoComms", true, enableCamoComms);
        restrictDevices = Create(121, Types.General, "restrictDevices", ["optionOff", "restrictDevices2", "restrictDevices3"], null);
        //restrictAdmin = Create(122, Types.General, "restrictAdmin", 30f, 0f, 600f, 5f, restrictDevices);
        restrictCameras = Create(123, Types.General, "restrictCameras", 30f, 0f, 600f, 5f, restrictDevices);
        restrictVents = Create(124, Types.General, "restrictVents", 30f, 0f, 600f, 5f, restrictDevices);
        disableCamsRound1 = Create(125, Types.General, "disableCamsRound1", false, null);
        camsNightVision = Create(126, Types.General, "camsNightVision", false, null);
        camsNoNightVisionIfImpVision = Create(127, Types.General, "camsNoNightVisionIfImpVision", false, camsNightVision);

        dynamicMap = Create(130, Types.General, "dynamicMap", false, null, true);
        dynamicMapEnableSkeld = Create(131, Types.General, "Skeld", rates, dynamicMap);
        dynamicMapEnableMira = Create(132, Types.General, "Mira", rates, dynamicMap);
        dynamicMapEnablePolus = Create(133, Types.General, "Polus", rates, dynamicMap);
        dynamicMapEnableAirShip = Create(134, Types.General, "Airship", rates, dynamicMap);
        dynamicMapEnableFungle = Create(135, Types.General, "Fungle", rates, dynamicMap);
        dynamicMapEnableSubmerged = Create(136, Types.General, "Submerged", rates, dynamicMap);
        dynamicMapSeparateSettings = Create(137, Types.General, "dynamicMapSeparateSettings", false, dynamicMap);

        debugMode = Create(900, Types.General, "debugMode", false, null, true);
        disableGameEnd = Create(901, Types.General, "DisableGameEnd", false, debugMode);

        //-------------------------- Impostor Options 10000-19999 -------------------------- //

        wolfLordSpawnRate = Create(10100, Types.Impostor, cs(WolfLord.color, "WolfLord"), rates, null, true);

        morphlingSpawnRate = Create(10110, Types.Impostor, cs(Morphling.color, "Morphling"), rates, null, true);
        morphlingCooldown = Create(10111, Types.Impostor, "morphlingCooldown", 15f, 10f, 60f, 2.5f, morphlingSpawnRate);
        morphlingDuration = Create(10112, Types.Impostor, "morphlingDuration", 15f, 1f, 20f, 0.5f, morphlingSpawnRate);

        bomberSpawnRate = Create(10120, Types.Impostor, cs(Bomber.color, "Bomber"), rates, null, true);
        bomberBombCooldown = Create(10121, Types.Impostor, "bomberBombCooldown", 25f, 10f, 60f, 2.5f, bomberSpawnRate);
        bomberDelay = Create(10122, Types.Impostor, "bomberDelay", 5f, 0f, 20f, 0.5f, bomberSpawnRate);
        bomberTimer = Create(10123, Types.Impostor, "bomberTimer", 10f, 5f, 30f, 0.5f, bomberSpawnRate);
        bomberTriggerBothCooldowns = Create(10125, Types.Impostor, "bomberTriggerBothCooldowns", false, bomberSpawnRate);
        bomberCanGiveToBomber = Create(10126, Types.Impostor, "bomberCanGiveToBomber", false, bomberSpawnRate);
        bomberHotPotatoMode = Create(10124, Types.Impostor, "bomberHotPotatoMode", true, bomberSpawnRate);

        undertakerSpawnRate = Create(10130, Types.Impostor, cs(Undertaker.color, "Undertaker"), rates, null, true);
        undertakerDragingDelaiAfterKill = Create(10131, Types.Impostor, "undertakerDragingDelaiAfterKill", 0f, 0f, 15, 0.5f, undertakerSpawnRate);
        undertakerDragingAfterVelocity = Create(10132, Types.Impostor, "undertakerDragingAfterVelocity", 0.75f, 0.5f, 1.5f, 0.125f, undertakerSpawnRate);
        undertakerCanDragAndVent = Create(10133, Types.Impostor, "undertakerCanDragAndVent", true, undertakerSpawnRate);

        camouflagerSpawnRate = Create(10140, Types.Impostor, cs(Camouflager.color, "Camouflager"), rates, null, true);
        camouflagerCooldown = Create(10141, Types.Impostor, "camouflagerCooldown", 25f, 10f, 60f, 2.5f, camouflagerSpawnRate);
        camouflagerDuration = Create(10142, Types.Impostor, "camouflagerDuration", 12.5f, 1f, 20f, 0.5f, camouflagerSpawnRate);

        vampireSpawnRate = Create(10150, Types.Impostor, cs(Vampire.color, "Vampire"), rates, null, true);
        vampireKillDelay = Create(10151, Types.Impostor, "vampireKillDelay", 5f, 1f, 10f, 0.5f, vampireSpawnRate);
        vampireCooldown = Create(10152, Types.Impostor, "vampireCooldown", 25f, 10f, 60f, 2.5f, vampireSpawnRate);
        vampireGarlicButton = Create(10153, Types.Impostor, "vampireGarlicButton", true, vampireSpawnRate);
        vampireCanKillNearGarlics = Create(10154, Types.Impostor, "vampireCanKillNearGarlics", true, vampireGarlicButton);

        eraserSpawnRate = Create(10160, Types.Impostor, cs(Eraser.color, "Eraser"), rates, null, true);
        eraserCooldown = Create(10161, Types.Impostor, "eraserCooldown", 25f, 10f, 120f, 2.5f, eraserSpawnRate);
        eraserCanEraseAnyone = Create(10162, Types.Impostor, "eraserCanEraseAnyone", false, eraserSpawnRate);
        erasercanEraseGuess = Create(10163, Types.Impostor, "erasercanEraseGuess", false, eraserSpawnRate);

        poucherSpawnRate = Create(10320, Types.Impostor, cs(Palette.ImpostorRed, "Poucher"), rates, null, true, false, () =>
        {
            if (modifierPoucher.selection > 0) poucherSpawnRate.selection = 0;
        });

        butcherSpawnRate = Create(10310, Types.Impostor, cs(Palette.ImpostorRed, "Butcher"), rates, null, true);
        butcherDissectionCooldown = Create(10312, Types.Impostor, "butcherDissectionCooldown", 25f, 10f, 60f, 2.5f, butcherSpawnRate);
        butcherDissectionDuration = Create(10313, Types.Impostor, "butcherDissectionDuration", 1f, 0f, 10f, 0.25f, butcherSpawnRate);
        butcherDissectedBodyCount = Create(10314, Types.Impostor, "butcherDissectedBodyCount", 5, 3, 15, 1, butcherSpawnRate);

        mimicSpawnRate = Create(10170, Types.Impostor, cs(Mimic.color, "Mimic"), rates, null, true);

        escapistSpawnRate = Create(10180, Types.Impostor, cs(Escapist.color, "Escapist"), rates, null, true);
        escapistEscapeTime = Create(10181, Types.Impostor, "escapistEscapeTime", 15f, 0f, 60f, 2.5f, escapistSpawnRate);
        escapistResetPlaceAfterMeeting = Create(10183, Types.Impostor, "escapistResetPlaceAfterMeeting", false, escapistSpawnRate);

        tricksterSpawnRate = Create(10200, Types.Impostor, cs(Trickster.color, "Trickster"), rates, null, true);
        tricksterPlaceBoxCooldown = Create(10201, Types.Impostor, "tricksterPlaceBoxCooldown", 20f, 2.5f, 30f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutCooldown = Create(10202, Types.Impostor, "tricksterLightsOutCooldown", 25f, 10f, 60f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutDuration = Create(10203, Types.Impostor, "tricksterLightsOutDuration", 12.5f, 5f, 60f, 0.5f, tricksterSpawnRate);

        cleanerSpawnRate = Create(10210, Types.Impostor, cs(Cleaner.color, "Cleaner"), rates, null, true);
        cleanerCooldown = Create(10211, Types.Impostor, "cleanerCooldown", 25f, 10f, 60f, 2.5f, cleanerSpawnRate);

        warlockSpawnRate = Create(10220, Types.Impostor, cs(Warlock.color, "Warlock"), rates, null, true);
        warlockCooldown = Create(10221, Types.Impostor, "warlockCooldown", 20f, 10f, 60f, 2.5f, warlockSpawnRate);
        warlockRootTime = Create(10222, Types.Impostor, "warlockRootTime", 3f, 0f, 15f, 0.25f, warlockSpawnRate);

        bountyHunterSpawnRate = Create(10230, Types.Impostor, cs(BountyHunter.color, "BountyHunter"), rates, null, true);
        bountyHunterBountyDuration = Create(10231, Types.Impostor, "bountyHunterBountyDuration", 60f, 10f, 180f, 5f, bountyHunterSpawnRate);
        bountyHunterReducedCooldown = Create(10232, Types.Impostor, "bountyHunterReducedCooldown", 2.5f, 0f, 30f, 0.5f, bountyHunterSpawnRate);
        bountyHunterPunishmentTime = Create(10233, Types.Impostor, "bountyHunterPunishmentTime", 10f, 0f, 60f, 2.5f, bountyHunterSpawnRate);
        bountyHunterShowArrow = Create(10234, Types.Impostor, "bountyHunterShowArrow", true, bountyHunterSpawnRate);
        bountyHunterArrowUpdateIntervall = Create(10235, Types.Impostor, "bountyHunterArrowUpdateIntervall", 0.5f, 0f, 15f, 0.5f, bountyHunterShowArrow);

        witchSpawnRate = Create(10240, Types.Impostor, cs(Witch.color, "Witch"), rates, null, true);
        witchCooldown = Create(10241, Types.Impostor, "witchCooldown", 20f, 10f, 60, 2.5f, witchSpawnRate);
        witchAdditionalCooldown = Create(10242, Types.Impostor, "witchAdditionalCooldown", 5f, 0f, 60f, 2.5f, witchSpawnRate);
        witchCanSpellAnyone = Create(10243, Types.Impostor, "witchCanSpellAnyone", false, witchSpawnRate);
        witchSpellCastingDuration = Create(10244, Types.Impostor, "witchSpellCastingDuration", 0.5f, 0f, 10f, 0.25f, witchSpawnRate);
        witchTriggerBothCooldowns = Create(10245, Types.Impostor, "witchTriggerBothCooldowns", false, witchSpawnRate);
        witchVoteSavesTargets = Create(10246, Types.Impostor, "witchVoteSavesTargets", true, witchSpawnRate);

        ninjaSpawnRate = Create(10250, Types.Impostor, cs(Ninja.color, "Ninja"), rates, null, true);
        ninjaCooldown = Create(10251, Types.Impostor, "ninjaCooldown", 20f, 10f, 60f, 2.5f, ninjaSpawnRate);
        ninjaKnowsTargetLocation = Create(10252, Types.Impostor, "ninjaKnowsTargetLocation", true, ninjaSpawnRate);
        ninjaTraceTime = Create(10253, Types.Impostor, "ninjaTraceTime", 6f, 1f, 20f, 0.5f, ninjaSpawnRate);
        ninjaTraceColorTime = Create(10254, Types.Impostor, "ninjaTraceColorTime", 3f, 0f, 20f, 0.5f, ninjaSpawnRate);
        ninjaInvisibleDuration = Create(10255, Types.Impostor, "ninjaInvisibleDuration", 10f, 0f, 20f, 0.5f, ninjaSpawnRate);

        blackmailerSpawnRate = Create(10260, Types.Impostor, cs(Blackmailer.color, "Blackmailer"), rates, null, true);
        blackmailerCooldown = Create(10261, Types.Impostor, "blackmailerCooldown", 15f, 5f, 120f, 2.5f, blackmailerSpawnRate);

        terroristSpawnRate = Create(10270, Types.Impostor, cs(Terrorist.color, "Terrorist"), rates, null, true);
        terroristBombDestructionTime = Create(10271, Types.Impostor, "terroristBombDestructionTime", 0f, 0f, 120f, 0.5f, terroristSpawnRate);
        terroristBombDestructionRange = Create(10272, Types.Impostor, "terroristBombDestructionRange", 30f, 5f, 250f, 5f, terroristSpawnRate);
        terroristBombHearRange = Create(10273, Types.Impostor, "terroristBombHearRange", 60f, 5f, 250f, 5f, terroristSpawnRate);
        terroristDefuseDuration = Create(10274, Types.Impostor, "terroristDefuseDuration", 2f, 0f, 30f, 0.5f, terroristSpawnRate);
        terroristBombCooldown = Create(10275, Types.Impostor, "terroristBombCooldown", 0f, 5f, 60f, 2.5f, terroristSpawnRate);
        terroristBombActiveAfter = Create(10276, Types.Impostor, "terroristBombActiveAfter", 0f, 0f, 15f, 0.5f, terroristSpawnRate);

        minerSpawnRate = Create(10280, Types.Impostor, cs(Miner.color, "Miner"), rates, null, true);
        minerCooldown = Create(10281, Types.Impostor, "minerCooldown", 20f, 10f, 60f, 2.5f, minerSpawnRate);

        yoyoSpawnRate = Create(10290, Types.Impostor, cs(Yoyo.color, "Yoyo"), rates, null, true);
        yoyoMarkCooldown = Create(10292, Types.Impostor, "yoyoMarkCooldown", 15f, 2.5f, 120f, 2.5f, yoyoSpawnRate);
        yoyoBlinkDuration = Create(10291, Types.Impostor, "yoyoBlinkDuration", 15f, 2.5f, 120f, 2.5f, yoyoSpawnRate);
        yoyoMarkStaysOverMeeting = Create(10293, Types.Impostor, "yoyoMarkStaysOverMeeting", false, yoyoSpawnRate);
        yoyoHasAdminTable = Create(10294, Types.Impostor, "yoyoHasAdminTable", true, yoyoSpawnRate);
        yoyoAdminTableCooldown = Create(10295, Types.Impostor, "yoyoAdminTableCooldown", 15f, 2.5f, 120f, 2.5f, yoyoHasAdminTable);
        yoyoSilhouetteVisibility = Create(10296, Types.Impostor, "yoyoSilhouetteVisibility", ["0%", "10%", "20%", "30%", "40%", "50%"], yoyoSpawnRate);

        evilTrapperSpawnRate = Create(10300, Types.Impostor, cs(EvilTrapper.color, "EvilTrapper"), rates, null, true);
        evilTrapperNumTrap = Create(10301, Types.Impostor, "evilTrapperNumTrap", 2f, 1f, 10f, 1f, evilTrapperSpawnRate);
        evilTrapperExtensionTime = Create(10302, Types.Impostor, "evilTrapperExtensionTime", 5f, 2f, 10f, 0.5f, evilTrapperSpawnRate);
        evilTrapperCooldown = Create(10303, Types.Impostor, "evilTrapperCooldown", 15f, 10f, 60f, 2.5f, evilTrapperSpawnRate);
        evilTrapperKillTimer = Create(10304, Types.Impostor, "evilTrapperKillTimer", 5f, 1f, 30f, 1f, evilTrapperSpawnRate);
        evilTrapperTrapRange = Create(10305, Types.Impostor, "evilTrapperTrapRange", 1f, 0.25f, 2f, 0.125f, evilTrapperSpawnRate);
        evilTrapperMaxDistance = Create(10306, Types.Impostor, "evilTrapperMaxDistance", 10f, 0f, 20f, 0.25f, evilTrapperSpawnRate);
        evilTrapperPenaltyTime = Create(10307, Types.Impostor, "evilTrapperPenaltyTime", 0f, 0f, 30f, 0.5f, evilTrapperSpawnRate);
        evilTrapperBonusTime = Create(10308, Types.Impostor, "evilTrapperBonusTime", 10f, 0f, 15f, 0.5f, evilTrapperSpawnRate);

        gamblerSpawnRate = Create(10330, Types.Impostor, cs(Gambler.color, "Gambler"), rates, null, true);
        gamblerMinCooldown = Create(10331, Types.Impostor, "gamblerMinCooldown", 2.5f, 0f, 45f, 0.5f, gamblerSpawnRate);
        gamblerMaxCooldown = Create(10332, Types.Impostor, "gamblerMaxCooldown", 40f, 10f, 90f, 2.5f, gamblerSpawnRate);
        gamblerSuccessRate = Create(10333, Types.Impostor, "gamblerSuccessRate", rates, gamblerSpawnRate);

        grenadierSpawnRate = Create(10340, Types.Impostor, cs(Grenadier.color, "Grenadier"), rates, null, true);
        grenadierCooldown = Create(10341, Types.Impostor, "grenadierCooldown", 20f, 0f, 45f, 2.5f, grenadierSpawnRate);
        grenadierDuration = Create(10342, Types.Impostor, "grenadierDuration", 8f, 4f, 10f, 0.5f, grenadierSpawnRate);
        grenadierFlashRadius = Create(10343, Types.Impostor, "grenadierFlashRadius", 1f, 0.25f, 5f, 0.125f, grenadierSpawnRate);
        grenadierTeamIndicators = Create(10344, Types.Impostor, "grenadierTeamIndicators",
            ["optionOff", "grenadierIndicators2", "grenadierIndicators3"], grenadierSpawnRate);

        //-------------------------- Neutral Options 20000-29999 -------------------------- //

        specterSpawnRate = Create(50020, Types.Neutral, cs(Specter.color, "SpecterOptions"), rates, null, true);
        specterResetRole = Create(50021, Types.Neutral, "amnisiacResetRole", true, specterSpawnRate);
        specterAfterMeeting = Create(50022, Types.Neutral, "specterAfterMeeting", false, specterSpawnRate);

        survivorSpawnRate = Create(20280, Types.Neutral, cs(Survivor.color, "Survivor"), rates, null, true);
        survivorVestEnable = Create(20281, Types.Neutral, "survivorVestEnable", true, survivorSpawnRate);
        survivorVestNumber = Create(20282, Types.Neutral, "survivorVestNumber", 5f, 1f, 20f, 1f, survivorVestEnable);
        survivorVestCooldown = Create(20283, Types.Neutral, "survivorVestCooldown", 20f, 2.5f, 60f, 2.5f, survivorVestEnable);
        survivorVestDuration = Create(20284, Types.Neutral, "survivorVestDuration", 10f, 2.5f, 60f, 0.5f, survivorVestEnable);
        survivorVestResetCooldown = Create(20285, Types.Neutral, "survivorVestResetCooldown", 5f, 2.5f, 60f, 2.5f, survivorVestEnable);
        survivorBlanksEnable = Create(20286, Types.Neutral, "survivorBlanksEnable", false, survivorSpawnRate);
        survivorBlanksCooldown = Create(20287, Types.Neutral, "survivorBlanksCooldown", 20f, 5f, 60f, 2.5f, survivorBlanksEnable);
        survivorBlanksNumber = Create(20288, Types.Neutral, "survivorBlanksNumber", 6f, 1f, 20f, 1f, survivorBlanksEnable);

        amnisiacSpawnRate = Create(20110, Types.Neutral, cs(Amnisiac.color, "Amnisiac"), rates, null, true);
        amnisiacShowArrows = Create(20111, Types.Neutral, "amnisiacShowArrows", true, amnisiacSpawnRate);
        amnisiacResetRole = Create(20112, Types.Neutral, "amnisiacResetRole", true, amnisiacSpawnRate);

        jesterSpawnRate = Create(20100, Types.Neutral, cs(Jester.color, "Jester"), rates, null, true);
        jesterCanCallEmergency = Create(20101, Types.Neutral, "jesterCanCallEmergency", true, jesterSpawnRate);
        jesterCanVent = Create(20102, Types.Neutral, "jesterCanVent", true, jesterSpawnRate);
        jesterHasImpostorVision = Create(20103, Types.Neutral, "jesterHasImpostorVision", true, jesterSpawnRate);

        partTimerSpawnRate = Create(20290, Types.Neutral, cs(PartTimer.color, "PartTimer"), rates, null, true);
        partTimerCooldown = Create(20291, Types.Neutral, "partTimerCooldown", 20f, 2.5f, 60f, 2.5f, partTimerSpawnRate);
        partTimerDeathTurn = Create(20292, Types.Neutral, "partTimerDeathTurn", 2, 1, 6, 1, partTimerSpawnRate);
        partTimerKnowsRole = Create(20293, Types.Neutral, "partTimerIsCheckTargetRole", true, partTimerSpawnRate);

        witnessSpawnRate = Create(20301, Types.Neutral, cs(Witness.color, "Witness"), rates, null, true);
        witnessMarkTimer = Create(20302, Types.Neutral, "witnessMarkTimer", 30, 20, 90, 5, witnessSpawnRate);
        witnessWinCount = Create(20303, Types.Neutral, "witnessWinCount", 2, 1, 6, 1, witnessSpawnRate);
        witnessMeetingDie = Create(20304, Types.Neutral, "witnessMeetingDie", true, witnessSpawnRate);
        witnessSkipMeeting = Create(20305, Types.Neutral, "witnessSkipMeeting", true, witnessSpawnRate);

        jackalSpawnRate = Create(20130, Types.Neutral, cs(Jackal.color, "Jackal"), rates, null, true);
        jackalChanceSwoop = Create(20142, Types.Neutral, cs(Swooper.color, "jackalChanceSwoop"), rates, jackalSpawnRate);
        jackalKillCooldown = Create(20131, Types.Neutral, "jackalKillCooldown", 25f, 10f, 60f, 2.5f, jackalSpawnRate);
        jackalSwooperCooldown = Create(20143, Types.Neutral, "jackalSwooperCooldown", 25f, 10f, 60f, 2.5f, jackalChanceSwoop);
        jackalSwooperDuration = Create(20144, Types.Neutral, "jackalSwooperDuration", 12.5f, 1f, 20f, 0.5f, jackalChanceSwoop);
        jackalCanUseVents = Create(20132, Types.Neutral, "jackalCanUseVents", true, jackalSpawnRate);
        jackalCanUseSabo = Create(20133, Types.Neutral, "jackalCanUseSabo", false, jackalSpawnRate);
        jackalAndSidekickHaveImpostorVision = Create(20134, Types.Neutral, "jackalAndSidekickHaveImpostorVision", true, jackalSpawnRate);
        jackalCanCreateSidekick = Create(20135, Types.Neutral, cs(Jackal.color, "jackalCanCreateSidekick"), false, jackalSpawnRate);
        jackalCreateSidekickCooldown = Create(20136, Types.Neutral, "jackalCreateSidekickCooldown", 25f, 10f, 60f, 2.5f, jackalCanCreateSidekick);
        jackalkillFakeImpostor = Create(20145, Types.Neutral, cs(Palette.ImpostorRed, "jackalkillFakeImpostor"), false, jackalCanCreateSidekick);
        sidekickCanKill = Create(20138, Types.Neutral, "sidekickCanKill", true, jackalCanCreateSidekick);
        sidekickCanUseVents = Create(20139, Types.Neutral, "sidekickCanUseVents", true, jackalCanCreateSidekick);
        sidekickPromotesToJackal = Create(20140, Types.Neutral, "sidekickPromotesToJackal", false, jackalCanCreateSidekick);
        jackalPromotedFromSidekickCanCreateSidekick = Create(20141, Types.Neutral, "jackalPromotedFromSidekickCanCreateSidekick", true, sidekickPromotesToJackal);

        pavlovsownerSpawnRate = Create(20250, Types.Neutral, cs(Pavlovsdogs.color, "Pavlovsowner"), rates, null, true);
        pavlovsownerAndJackalAsWell = Create(20251, Types.Neutral, "pavlovsownerAndJackalAsWell", true, pavlovsownerSpawnRate);
        pavlovsownerKillCooldown = Create(20252, Types.Neutral, "pavlovsownerKillCooldown", 25f, 10f, 60f, 2.5f, pavlovsownerSpawnRate);
        pavlovsownerCreateDogCooldown = Create(20253, Types.Neutral, "pavlovsownerCreateDogCooldown", 25f, 10f, 60f, 2.5f, pavlovsownerSpawnRate);
        pavlovsownerCreateDogNum = Create(20254, Types.Neutral, "pavlovsownerCreateDogNum", 3f, 1f, 15f, 1f, pavlovsownerSpawnRate);
        pavlovsownerCanUseSabo = Create(20255, Types.Neutral, "pavlovsownerCanUseSabo", true, pavlovsownerSpawnRate);
        pavlovsownerHasImpostorVision = Create(20256, Types.Neutral, "pavlovsownerHasImpostorVision", true, pavlovsownerSpawnRate);
        pavlovsownerCanUseVents = Create(20257, Types.Neutral, "pavlovsownerCanUseVents",
            ["Pavlovsdogs", "Pavlovsowner", "pavlovsownerCanUseVents3"], pavlovsownerSpawnRate);
        pavlovsownerRampage = Create(20260, Types.Neutral, "pavlovsownerRampage", true, pavlovsownerSpawnRate);
        pavlovsownerRampageKillCooldown = Create(20261, Types.Neutral, "pavlovsownerRampageKillCooldown", 15f, 5f, 60f, 2.5f, pavlovsownerRampage);
        pavlovsownerRampageDeathTime = Create(20262, Types.Neutral, "pavlovsownerRampageDeathTime", 60f, 30f, 180f, 2.5f, pavlovsownerRampageKillCooldown);

        arsonistSpawnRate = Create(20120, Types.Neutral, cs(Arsonist.color, "Arsonist"), rates, null, true);
        arsonistCooldown = Create(20121, Types.Neutral, "arsonistCooldown", 12.5f, 5f, 60f, 2.5f, arsonistSpawnRate);
        arsonistDuration = Create(20122, Types.Neutral, "arsonistDuration", 0.25f, 0f, 10f, 0.125f, arsonistSpawnRate);
        arsonistIgniteCdRemoved = Create(20123, Types.Neutral, "arsonistIgniteCdRemoved", false, arsonistSpawnRate);

        swooperSpawnRate = Create(20150, Types.Neutral, cs(Swooper.color, "Swooper"), rates, null, true);
        swooperKillCooldown = Create(20151, Types.Neutral, "swooperKillCooldown", 25f, 10f, 60f, 2.5f, swooperSpawnRate);
        swooperCooldown = Create(20152, Types.Neutral, "swooperCooldown", 20f, 10f, 60f, 2.5f, swooperSpawnRate);
        swooperDuration = Create(20153, Types.Neutral, "swooperDuration", 15f, 1f, 20f, 0.5f, swooperSpawnRate);
        swooperSpeed = Create(20154, Types.Neutral, "swooperSpeed", 1.5f, 1f, 3f, 0.125f, swooperSpawnRate);
        swooperCanUseVents = Create(20155, Types.Neutral, "canUseVents", true, swooperSpawnRate);
        swooperHasImpVision = Create(20156, Types.Neutral, "swooperHasImpVision", true, swooperSpawnRate);

        werewolfSpawnRate = Create(20200, Types.Neutral, cs(Werewolf.color, "Werewolf"), rates, null, true);
        werewolfRampageCooldown = Create(20201, Types.Neutral, "werewolfRampageCooldown", 25f, 10f, 60f, 2.5f, werewolfSpawnRate);
        werewolfRampageDuration = Create(20202, Types.Neutral, "werewolfRampageDuration", 15f, 0.5f, 20f, 0.5f, werewolfSpawnRate);
        werewolfKillCooldown = Create(20203, Types.Neutral, "werewolfKillCooldown", 3f, 1f, 60f, 0.5f, werewolfSpawnRate);

        juggernautSpawnRate = Create(20210, Types.Neutral, cs(Juggernaut.color, "Juggernaut"), rates, null, true);
        juggernautCooldown = Create(20211, Types.Neutral, "juggernautCooldown", 25f, 2.5f, 60f, 2.5f, juggernautSpawnRate);
        juggernautHasImpVision = Create(20212, Types.Neutral, "juggernautHasImpVision", true, juggernautSpawnRate);
        juggernautCanUseVents = Create(20113, Types.Neutral, "canUseVents", true, juggernautSpawnRate);
        juggernautReducedkillEach = Create(20114, Types.Neutral, "juggernautReducedkillEach", 5f, 1f, 15f, 0.5f, juggernautSpawnRate);

        vultureSpawnRate = Create(20170, Types.Neutral, cs(Vulture.color, "Vulture"), rates, null, true);
        vultureCooldown = Create(20171, Types.Neutral, "vultureCooldown", 12.5f, 10f, 60f, 2.5f, vultureSpawnRate);
        vultureNumberToWin = Create(20172, Types.Neutral, "vultureNumberToWin", 3f, 1f, 15f, 1f, vultureSpawnRate);
        vultureCanUseVents = Create(20173, Types.Neutral, "canUseVents", true, vultureSpawnRate);
        vultureShowArrows = Create(20174, Types.Neutral, "vultureShowArrows", true, vultureSpawnRate);

        lawyerSpawnRate = Create(20180, Types.Neutral, cs(Lawyer.color, "Lawyer"), rates, null, true);
        lawyerTargetKnows = Create(20182, Types.Neutral, "lawyerTargetKnows", true, lawyerSpawnRate);
        lawyerVision = Create(20183, Types.Neutral, "lawyerVision", 1.5f, 0.25f, 3f, 0.25f, lawyerSpawnRate);
        lawyerKnowsRole = Create(20184, Types.Neutral, "lawyerKnowsRole", true, lawyerSpawnRate);
        lawyerCanCallEmergency = Create(20185, Types.Neutral, "lawyerCanCallEmergency", true, lawyerSpawnRate);
        lawyerStolenWin = Create(20189, Types.Neutral, "lawyerStolenWin", false, lawyerSpawnRate);
        lawyerTargetCanBeJester = Create(20186, Types.Neutral, "lawyerTargetCanBeJester", false, lawyerSpawnRate);

        //pursuerSpawnRate = Create(20270, Types.Neutral, cs(Pursuer.color, "Pursuer"), rates, null, true);
        pursuerBlanksCooldown = Create(20272, Types.Neutral, "pursuerBlanksCooldown", 20f, 5f, 60f, 2.5f, lawyerSpawnRate);
        pursuerBlanksNumber = Create(20273, Types.Neutral, "pursuerBlanksNumber", 6f, 1f, 20f, 1f, lawyerSpawnRate);

        executionerSpawnRate = Create(20190, Types.Neutral, cs(Executioner.color, "Executioner"), rates, null, true);
        executionerCanCallEmergency = Create(20191, Types.Neutral, "executionerCanCallEmergency", true, executionerSpawnRate);
        executionerPromotesToLawyer = Create(20192, Types.Neutral, "executionerPromotesToLawyer", true, executionerSpawnRate);
        //executionerOnTargetDead = Create(20193, Types.Neutral, "目标死亡后变为", [cs(Pursuer.color, "Pursuer"), cs(Jester.color, "Jester"), cs(Amnisiac.color, "Amnisiac"), "Crewmate"], executionerSpawnRate);

        doomsayerSpawnRate = Create(20221, Types.Neutral, cs(Doomsayer.color, "Doomsayer"), rates, null, true);
        doomsayerCooldown = Create(20222, Types.Neutral, "doomsayerCooldown", 20f, 2.5f, 60f, 2.5f, doomsayerSpawnRate);
        doomsayerHasMultipleShotsPerMeeting = Create(20223, Types.Neutral, "doomsayerHasMultipleShotsPerMeeting", true, doomsayerSpawnRate);
        doomsayerOnlineTarger = Create(20227, Types.Neutral, "doomsayerOnlineTarger", false, doomsayerSpawnRate);
        doomsayerDormationNum = Create(20229, Types.Neutral, "doomsayerDormationNum", 5f, 2f, 10f, 1f, doomsayerSpawnRate);
        doomsayerCanGuessImpostor = Create(20226, Types.Neutral, $"{"doomsayerCanGuess".Translate()} {cs(Palette.ImpostorRed, "ImpostorRolesText".Translate())}", true, doomsayerSpawnRate);
        doomsayerCanGuessNeutral = Create(20225, Types.Neutral, $"{"doomsayerCanGuess".Translate()} {cs(Color.gray, "NeutralRolesText".Translate())}", true, doomsayerSpawnRate);
        doomsayerKillToWin = Create(20228, Types.Neutral, "doomsayerKillToWin", 3f, 1f, 10f, 1f, doomsayerSpawnRate);

        akujoSpawnRate = Create(20231, Types.Neutral, cs(Akujo.color, "Akujo"), rates, null, true);
        akujoTimeLimit = Create(20232, Types.Neutral, "akujoTimeLimit", 450f, 120f, 1200f, 30f, akujoSpawnRate);
        akujoForceKeeps = Create(20236, Types.Neutral, "akujoForceKeeps", false, akujoSpawnRate);
        akujoNumKeeps = Create(20233, Types.Neutral, "akujoNumKeeps", 1f, 0f, 5f, 1f, akujoSpawnRate);
        akujoKnowsRoles = Create(20234, Types.Neutral, "akujoKnowsRoles", true, akujoSpawnRate);
        akujoHonmeiCannotFollowWin = Create(20235, Types.Neutral, "akujoHonmeiCannotFollowWin", true, akujoSpawnRate);
        akujoHonmeiOptimizeWin = Create(20237, Types.Neutral, "akujoHonmeiOptimizeWin", true, akujoSpawnRate);

        thiefSpawnRate = Create(20240, Types.Neutral, cs(Thief.color, "Thief"), rates, null, true);
        thiefCooldown = Create(20241, Types.Neutral, "thiefCooldown", 25f, 5f, 120f, 2.5f, thiefSpawnRate);
        thiefCanKillSheriff = Create(20242, Types.Neutral, $"{"thiefCanKill".Translate()}{cs(Sheriff.color, "Sheriff".Translate())}", true, thiefSpawnRate);
        thiefCanKillDeputy = Create(20246, Types.Neutral, $"{"thiefCanKill".Translate()}{cs(Deputy.color, "Deputy".Translate())}", true, thiefSpawnRate);
        thiefCanKillVeteran = Create(20247, Types.Neutral, $"{"thiefCanKill".Translate()}{cs(Veteran.color, "Veteran".Translate())}", true, thiefSpawnRate);
        thiefHasImpVision = Create(20243, Types.Neutral, "thiefHasImpVision", true, thiefSpawnRate);
        thiefCanUseVents = Create(20244, Types.Neutral, "thiefCanUseVents", true, thiefSpawnRate);
        thiefCanStealWithGuess = Create(20245, Types.Neutral, "thiefCanStealWithGuess", true, thiefSpawnRate);

        //-------------------------- Crewmate Options 30000-39999 -------------------------- //

        ghostEngineerSpawnRate = Create(50010, Types.Crewmate, cs(GhostEngineer.color, "GhostEngineerOptions"), rates, null, true);

        guesserSpawnRate = Create(30100, Types.Crewmate, cs(Vigilante.color, "Vigilante"), rates, null, true);
        guesserNumberOfShots = Create(30101, Types.Crewmate, "guesserNumberOfShots", 3f, 1f, 15f, 1f, guesserSpawnRate);
        guesserHasMultipleShotsPerMeeting = Create(30102, Types.Crewmate, "guesserHasMultipleShotsPerMeeting", true, guesserSpawnRate);
        guesserShowInfoInGhostChat = Create(30103, Types.Crewmate, "guesserShowInfoInGhostChat", true, guesserSpawnRate);
        guesserKillsThroughShield = Create(30104, Types.Crewmate, "guesserKillsThroughShield", false, guesserSpawnRate);

        sheriffSpawnRate = Create(30141, Types.Crewmate, cs(Sheriff.color, "Sheriff"), rates, null, true);
        sheriffCooldown = Create(30142, Types.Crewmate, "sheriffCooldown", 25f, 10f, 60f, 2.5f, sheriffSpawnRate);
        sheriffMisfireKills = Create(30143, Types.Crewmate, "sheriffMisfireKills",
            ["sheriffMisfireKills1", "sheriffMisfireKills2", "sheriffMisfireKills3"], sheriffSpawnRate);
        sheriffCanKillNeutrals = Create(30150, Types.Crewmate, "sheriffCanKillNeutrals", false, sheriffSpawnRate);
        sheriffCanKillSurvivor = Create(30160, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(Survivor.color, "Survivor".Translate())}", false, sheriffCanKillNeutrals);
        sheriffCanKillAmnesiac = Create(30153, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(Amnisiac.color, "Amnisiac".Translate())}", false, sheriffCanKillNeutrals);
        sheriffCanKillPursuer = Create(30158, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(Pursuer.color, "Pursuer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillPartTimer = Create(30161, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(PartTimer.color, "PartTimer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillJester = Create(30151, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(Jester.color, "Jester".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillLawyer = Create(30156, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(Lawyer.color, "Lawyer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillExecutioner = Create(30152, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(Executioner.color, "Executioner".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillVulture = Create(30155, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(Vulture.color, "Vulture".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillDoomsayer = Create(30159, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(Doomsayer.color, "Doomsayer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillThief = Create(30157, Types.Crewmate, $"{"sheriffCanKill".Translate()}{cs(Thief.color, "Thief".Translate())}", true, sheriffCanKillNeutrals);

        deputySpawnRate = Create(30170, Types.Crewmate, cs(Deputy.color, "deputySpawnRate"), rates, sheriffSpawnRate);
        deputyNumberOfHandcuffs = Create(30171, Types.Crewmate, "deputyNumberOfHandcuffs", 5f, 1f, 15f, 1f, deputySpawnRate);
        deputyHandcuffCooldown = Create(30172, Types.Crewmate, "deputyHandcuffCooldown", 20f, 10f, 60f, 2.5f, deputySpawnRate);
        deputyHandcuffDuration = Create(30173, Types.Crewmate, "deputyHandcuffDuration", 10f, 5f, 60f, 2.5f, deputySpawnRate);
        deputyGetsPromoted = Create(30175, Types.Crewmate, "deputyGetsPromoted",
            ["optionOff", "deputyGetsPromoted2", "deputyGetsPromoted3"], deputySpawnRate);
        deputyKnowsSheriff = Create(30174, Types.Crewmate, "deputyKnowsSheriff", true, deputySpawnRate);
        deputyKeepsHandcuffs = Create(30176, Types.Crewmate, "deputyKeepsHandcuffs", true, deputyGetsPromoted);

        mayorSpawnRate = Create(30110, Types.Crewmate, cs(Mayor.color, "Mayor"), rates, null, true);
        mayorMeetingButton = Create(30113, Types.Crewmate, "mayorMeetingButton", false, mayorSpawnRate);
        mayorMaxRemoteMeetings = Create(30114, Types.Crewmate, "mayorMaxRemoteMeetings", 1f, 1f, 5f, 1f, mayorMeetingButton);
        mayorSabotageRemoteMeetings = Create(30115, Types.Crewmate, "mayorSabotageRemoteMeetings", false, mayorMeetingButton);
        mayorVote = Create(30117, Types.Crewmate, "mayorVote", 2, 1, 4, 1, mayorSpawnRate);
        mayorRevealVision = Create(30116, Types.Crewmate, "mayorRevealVision", ["-20%", "-30%", "-40%", "-50%"], mayorSpawnRate);

        prosecutorSpawnRate = Create(30370, Types.Crewmate, cs(Prosecutor.color, "Prosecutor"), rates, null, true);
        prosecutorCanSeeVoteColors = Create(30111, Types.Crewmate, "mayorCanSeeVoteColors", true, prosecutorSpawnRate);
        prosecutorTasksNeededToSeeVoteColors = Create(30112, Types.Crewmate, "mayorTasksNeededToSeeVoteColors", 5f, 0f, 20f, 1f, prosecutorCanSeeVoteColors);
        prosecutorDiesOnIncorrectPros = Create(30371, Types.Crewmate, "prosecutorDiesOnIncorrectPros", true, prosecutorSpawnRate);
        prosecutorCanCallEmergency = Create(30371, Types.Crewmate, "prosecutorCanCallEmergency", true, prosecutorSpawnRate);

        engineerSpawnRate = Create(30120, Types.Crewmate, cs(Engineer.color, "Engineer"), rates, null, true);
        engineerRemoteFix = Create(30121, Types.Crewmate, "engineerRemoteFix", true, engineerSpawnRate);
        engineerResetFixAfterMeeting = Create(30122, Types.Crewmate, "engineerResetFixAfterMeeting", true, engineerRemoteFix);
        engineerNumberOfFixes = Create(30123, Types.Crewmate, "engineerNumberOfFixes", 1f, 1f, 3f, 1f, engineerRemoteFix);
        //engineerExpertRepairs = Create(30124, Types.Crewmate, "engineerExpertRepairs", false, engineerSpawnRate);
        engineerHighlightForImpostors = Create(30125, Types.Crewmate, "engineerHighlightForImpostors", true, engineerSpawnRate);
        engineerHighlightForTeamJackal = Create(30126, Types.Crewmate, "engineerHighlightForTeamJackal", true, engineerSpawnRate);

        detectiveSpawnRate = Create(30190, Types.Crewmate, cs(Detective.color, "Detective"), rates, null, true);
        detectiveAnonymousFootprints = Create(30191, Types.Crewmate, "detectiveAnonymousFootprints",
            ["optionOff", "detectiveAnonymousFootprints1", "optionOn"], detectiveSpawnRate);
        detectiveFootprintIntervall = Create(30192, Types.Crewmate, "detectiveFootprintIntervall", 0.25f, 0.25f, 10f, 0.25f, detectiveSpawnRate);
        detectiveFootprintDuration = Create(30193, Types.Crewmate, "detectiveFootprintDuration", 12.5f, 0.5f, 30f, 0.5f, detectiveSpawnRate);
        detectiveReportNameDuration = Create(30194, Types.Crewmate, "detectiveReportNameDuration", 10f, 0f, 60f, 2.5f, detectiveSpawnRate);
        detectiveReportColorDuration = Create(30195, Types.Crewmate, "detectiveReportColorDuration", 30f, 0f, 120f, 2.5f, detectiveSpawnRate);

        medicSpawnRate = Create(30200, Types.Crewmate, cs(Medic.color, "Medic"), rates, null, true);
        medicShowShielded = Create(30201, Types.Crewmate, "medicShowShielded",
            ["medicShowShielded1", "medicShowShielded2", "medicShowShielded3"], medicSpawnRate);
        medicBreakShield = Create(30202, Types.Crewmate, "medicBreakShield", true, medicSpawnRate);
        medicShowAttemptToMedic = Create(30203, Types.Crewmate, "medicShowAttemptToMedic", true, medicBreakShield);
        medicShowAttemptToShielded = Create(30204, Types.Crewmate, "medicShowAttemptToShielded", false, medicBreakShield);
        medicResetTargetAfterMeeting = Create(30205, Types.Crewmate, "medicResetTargetAfterMeeting", false, medicSpawnRate);
        medicSetOrShowShieldAfterMeeting = Create(30206, Types.Crewmate, "medicSetOrShowShieldAfterMeeting",
            ["medicSetOrShowShieldAfterMeeting1", "medicSetOrShowShieldAfterMeeting2", "medicSetOrShowShieldAfterMeeting3"], medicSpawnRate);
        medicReportNameDuration = Create(30207, Types.Crewmate, "medicReportNameDuration", 5f, 0f, 60f, 2.5f, medicBreakShield);
        medicReportColorDuration = Create(30208, Types.Crewmate, "medicReportColorDuration", 30f, 0f, 120f, 2.5f, medicBreakShield);

        bodyGuardSpawnRate = Create(30340, Types.Crewmate, cs(BodyGuard.color, "BodyGuard"), rates, null, true);
        bodyGuardResetTargetAfterMeeting = Create(30341, Types.Crewmate, "bodyGuardResetTargetAfterMeeting", true, bodyGuardSpawnRate);
        bodyGuardShowShielded = Create(30343, Types.Crewmate, "bodyGuardShowShielded", true, bodyGuardSpawnRate);
        bodyGuardFlash = Create(30342, Types.Crewmate, "bodyGuardFlash", true, bodyGuardSpawnRate);

        timeMasterSpawnRate = Create(30210, Types.Crewmate, cs(TimeMaster.color, "TimeMaster"), rates, null, true);
        timeMasterCooldown = Create(30211, Types.Crewmate, "timeMasterCooldown", 20f, 10f, 60f, 2.5f, timeMasterSpawnRate);
        timeMasterShieldDuration = Create(30213, Types.Crewmate, "timeMasterShieldDuration", 15f, 2.5f, 20f, 0.5f, timeMasterSpawnRate);
        timeMasterRewindTime = Create(30212, Types.Crewmate, "timeMasterRewindTime", 9f, 1f, 10f, 1f, timeMasterSpawnRate);

        veteranSpawnRate = Create(30220, Types.Crewmate, cs(Veteran.color, "Veteran"), rates, null, true);
        veteranCooldown = Create(30221, Types.Crewmate, "veteranCooldown", 25f, 10f, 60f, 2.5f, veteranSpawnRate);
        veteranAlertDuration = Create(30222, Types.Crewmate, "veteranAlertDuration", 12.5f, 2.5f, 20f, 0.5f, veteranSpawnRate);

        swapperSpawnRate = Create(30230, Types.Crewmate, cs(Swapper.color, "Swapper"), rates, null, true);
        swapperCanCallEmergency = Create(30231, Types.Crewmate, "swapperCanCallEmergency", true, swapperSpawnRate);
        swapperCanFixSabotages = Create(30232, Types.Crewmate, "swapperCanFixSabotages", true, swapperSpawnRate);
        swapperCanOnlySwapOthers = Create(30233, Types.Crewmate, "swapperCanOnlySwapOthers", false, swapperSpawnRate);
        swapperSwapsNumber = Create(30234, Types.Crewmate, "swapperSwapsNumber", 1f, 0f, 5f, 1f, swapperSpawnRate);
        swapperRechargeTasksNumber = Create(30235, Types.Crewmate, "swapperRechargeTasksNumber", 2f, 1f, 10f, 1f, swapperSpawnRate);

        seerSpawnRate = Create(30240, Types.Crewmate, cs(Seer.color, "Seer"), rates, null, true);
        seerMode = Create(30241, Types.Crewmate, "seerMode", ["seerMode1", "seerMode2", "seerMode3"], seerSpawnRate);
        seerLimitSoulDuration = Create(30242, Types.Crewmate, "seerLimitSoulDuration", false, seerSpawnRate);
        seerSoulDuration = Create(30243, Types.Crewmate, "seerSoulDuration", 30f, 0f, 120f, 2.5f, seerLimitSoulDuration);

        hackerSpawnRate = Create(30250, Types.Crewmate, cs(Hacker.color, "Hacker"), rates, null, true);
        hackerCooldown = Create(30251, Types.Crewmate, "hackerCooldown", 15f, 5f, 60f, 2.5f, hackerSpawnRate);
        hackerHackeringDuration = Create(30252, Types.Crewmate, "hackerHackeringDuration", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate);
        hackerOnlyColorType = Create(30253, Types.Crewmate, "hackerOnlyColorType", false, hackerSpawnRate);
        hackerToolsNumber = Create(30254, Types.Crewmate, "hackerToolsNumber", 5f, 1f, 30f, 1f, hackerSpawnRate);
        hackerRechargeTasksNumber = Create(30255, Types.Crewmate, "hackerRechargeTasksNumber", 2f, 1f, 5f, 1f, hackerSpawnRate);
        hackerNoMove = Create(30256, Types.Crewmate, "hackerNoMove", true, hackerSpawnRate);

        trackerSpawnRate = Create(30260, Types.Crewmate, cs(Tracker.color, "Tracker"), rates, null, true);
        trackerUpdateIntervall = Create(30261, Types.Crewmate, "trackerUpdateIntervall", 0.5f, 0f, 30f, 0.5f, trackerSpawnRate);
        trackerResetTargetAfterMeeting = Create(30262, Types.Crewmate, "trackerResetTargetAfterMeeting ", false, trackerSpawnRate);
        trackerCanTrackCorpses = Create(30263, Types.Crewmate, "trackerCanTrackCorpses", true, trackerSpawnRate);
        trackerCorpsesTrackingCooldown = Create(30264, Types.Crewmate, "trackerCorpsesTrackingCooldown", 17.5f, 5f, 60f, 2.5f, trackerCanTrackCorpses);
        trackerCorpsesTrackingDuration = Create(30265, Types.Crewmate, "trackerCorpsesTrackingDuration", 7.5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses);
        trackerTrackingMethod = Create(30266, Types.Crewmate, "trackerTrackingMethod",
            ["trackerTrackingMethod1", "trackerTrackingMethod2", "trackerTrackingMethod3"], trackerSpawnRate);

        snitchSpawnRate = Create(30270, Types.Crewmate, cs(Snitch.color, "Snitch"), rates, null, true);
        snitchLeftTasksForReveal = Create(30271, Types.Crewmate, "snitchLeftTasksForReveal", 1, 0, 10, 1, snitchSpawnRate);
        snitchSeeMeeting = Create(30272, Types.Crewmate, "snitchSeeMeeting", true, snitchSpawnRate);
        snitchIncludeNeutralTeam = Create(30274, Types.Crewmate, "snitchIncludeNeutralTeam",
            ["optionOff", "snitchIncludeNeutralTeam2", "snitchIncludeNeutralTeam3", "snitchIncludeNeutralTeam4"], snitchSpawnRate);
        snitchTeamNeutraUseDifferentArrowColor = Create(30275, Types.Crewmate, "snitchTeamNeutraUseDifferentArrowColor", true, snitchIncludeNeutralTeam);

        prophetSpawnRate = Create(30360, Types.Crewmate, cs(Prophet.color, "Prophet"), rates, null, true);
        prophetCooldown = Create(30361, Types.Crewmate, "prophetCooldown", 20f, 5f, 60f, 2.5f, prophetSpawnRate);
        prophetNumExamines = Create(30362, Types.Crewmate, "prophetNumExamines", 4, 1, 10, 1, prophetSpawnRate);
        prophetCanCallEmergency = Create(30363, Types.Crewmate, "prophetCanCallEmergency", true, prophetSpawnRate);
        prophetIsRevealed = Create(30364, Types.Crewmate, "prophetIsRevealed", false, prophetSpawnRate);
        prophetExaminesToBeRevealed = Create(30365, Types.Crewmate, "prophetExaminesToBeRevealed", 3, 1, 10, 1, prophetIsRevealed);
        prophetKillCrewAsRed = Create(30366, Types.Crewmate, "prophetKillCrewAsRed", false, prophetSpawnRate);
        prophetBenignNeutralAsRed = Create(30367, Types.Crewmate, "prophetBenignNeutralAsRed", false, prophetSpawnRate);
        prophetEvilNeutralAsRed = Create(30368, Types.Crewmate, "prophetEvilNeutralAsRed", true, prophetSpawnRate);
        prophetKillNeutralAsRed = Create(30369, Types.Crewmate, "prophetKillNeutralAsRed", true, prophetSpawnRate);

        infoSleuthSpawnRate = Create(30380, Types.Crewmate, cs(InfoSleuth.color, "InfoSleuth"), rates, null, true);
        infoSleuthInfoType = Create(30381, Types.Crewmate, "infoSleuthInfoType",
            ["infoSleuthInfoType1", "infoSleuthInfoType2", "infoSleuthInfoType3"], infoSleuthSpawnRate);

        spySpawnRate = Create(30280, Types.Crewmate, cs(Spy.color, "Spy"), rates, null, true);
        spyCanDieToSheriff = Create(30281, Types.Crewmate, "spyCanDieToSheriff", false, spySpawnRate);
        spyImpostorsCanKillAnyone = Create(30282, Types.Crewmate, "spyImpostorsCanKillAnyone", true, spySpawnRate);
        spyCanEnterVents = Create(30283, Types.Crewmate, "spyCanEnterVents", true, spySpawnRate);
        spyHasImpostorVision = Create(30284, Types.Crewmate, "spyHasImpostorVision", true, spySpawnRate);

        portalmakerSpawnRate = Create(30290, Types.Crewmate, cs(Portalmaker.color, "Portalmaker"), rates, null, true);
        portalmakerCooldown = Create(30291, Types.Crewmate, "portalmakerCooldown", 15f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerUsePortalCooldown = Create(30292, Types.Crewmate, "portalmakerUsePortalCooldown", 15f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerLogOnlyColorType = Create(30293, Types.Crewmate, "portalmakerLogOnlyColorType", true, portalmakerSpawnRate);
        portalmakerLogHasTime = Create(30294, Types.Crewmate, "portalmakerLogHasTime", true, portalmakerSpawnRate);
        portalmakerCanPortalFromAnywhere = Create(30295, Types.Crewmate, "portalmakerCanPortalFromAnywhere", true, portalmakerSpawnRate);

        securityGuardSpawnRate = Create(30300, Types.Crewmate, cs(SecurityGuard.color, "SecurityGuard"), rates, null, true);
        securityGuardCooldown = Create(30301, Types.Crewmate, "securityGuardCooldown", 15f, 10f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardTotalScrews = Create(30302, Types.Crewmate, "securityGuardTotalScrews", 6f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamPrice = Create(30303, Types.Crewmate, "securityGuardCamPrice", 2f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardVentPrice = Create(30304, Types.Crewmate, "securityGuardVentPrice", 1f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamDuration = Create(30305, Types.Crewmate, "securityGuardCamDuration", 10f, 2.5f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardCamMaxCharges = Create(30306, Types.Crewmate, "securityGuardCamMaxCharges", 5f, 1f, 30f, 1f, securityGuardSpawnRate);
        securityGuardCamRechargeTasksNumber = Create(30307, Types.Crewmate, "securityGuardCamRechargeTasksNumber", 3f, 1f, 10f, 1f, securityGuardSpawnRate);
        securityGuardNoMove = Create(30308, Types.Crewmate, "securityGuardNoMove", true, securityGuardSpawnRate);

        mediumSpawnRate = Create(30310, Types.Crewmate, cs(Medium.color, "Medium"), rates, null, true);
        mediumCooldown = Create(30311, Types.Crewmate, "mediumCooldown", 7.5f, 2.5f, 120f, 2.5f, mediumSpawnRate);
        mediumDuration = Create(30312, Types.Crewmate, "mediumDuration", 0.5f, 0f, 15f, 0.5f, mediumSpawnRate);
        mediumOneTimeUse = Create(30313, Types.Crewmate, "mediumOneTimeUse", false, mediumSpawnRate);
        mediumChanceAdditionalInfo = Create(30314, Types.Crewmate, "mediumChanceAdditionalInfo", rates, mediumSpawnRate);

        jumperSpawnRate = Create(30320, Types.Crewmate, cs(Jumper.color, "Jumper"), rates, null, true);
        jumperJumpTime = Create(30321, Types.Crewmate, "jumperJumpTime", 10f, 0f, 60f, 2.5f, jumperSpawnRate);
        jumperMaxCharges = Create(30325, Types.Crewmate, "jumperMaxCharges", 3, 0, 10, 1, jumperSpawnRate);
        jumperResetPlaceAfterMeeting = Create(30323, Types.Crewmate, "jumperResetPlaceAfterMeeting", false, jumperSpawnRate);
        jumperChargesGainOnMeeting = Create(30324, Types.Crewmate, "jumperChargesGainOnMeeting", 2, 0, 10, 1, jumperSpawnRate);

        balancerSpawnRate = Create(30330, Types.Crewmate, cs(Balancer.color, "Balancer"), rates, null, true);
        balancerCount = Create(30331, Types.Crewmate, "balancerCount", 1, 1, 3, 1, balancerSpawnRate);
        balancerVoteTime = Create(30332, Types.Crewmate, "balancerVoteTime", 60, 15, 150, 5, balancerSpawnRate);

        trapperSpawnRate = Create(30350, Types.Crewmate, cs(Trapper.color, "Trapper"), rates, null, true);
        trapperCooldown = Create(30351, Types.Crewmate, "trapperCooldown", 20f, 5f, 120f, 2.5f, trapperSpawnRate);
        trapperMaxCharges = Create(30352, Types.Crewmate, "trapperMaxCharges", 5f, 1f, 15f, 1f, trapperSpawnRate);
        trapperRechargeTasksNumber = Create(30353, Types.Crewmate, "trapperRechargeTasksNumber", 2f, 1f, 15f, 1f, trapperSpawnRate);
        trapperTrapNeededTriggerToReveal = Create(30354, Types.Crewmate, "trapperTrapNeededTriggerToReveal", 2f, 1f, 10f, 1f, trapperSpawnRate);
        trapperInfoType = Create(30356, Types.Crewmate, "trapperInfoType", ["trapperInfoType1", "trapperInfoType2", "trapperInfoType3"], trapperSpawnRate);
        trapperTrapDuration = Create(30357, Types.Crewmate, "trapperTrapDuration", 5f, 1f, 15f, 0.5f, trapperSpawnRate);

        //-------------------------- Modifier (40000 - 49999) -------------------------- //

        modifiersAreHidden = Create(40000, Types.Modifier, cs(Color.yellow, "modifiersAreHidden"), false, null, true);

        modifierLover = Create(40160, Types.Modifier, cs(Lovers.color, "Lover"), rates, null, true);
        modifierLoverImpLoverRate = Create(40161, Types.Modifier, "modifierLoverImpLoverRate", rates, modifierLover);
        modifierLoverBothDie = Create(40162, Types.Modifier, "modifierLoverBothDie", true, modifierLover);
        modifierLoverEnableChat = Create(40163, Types.Modifier, "modifierLoverEnableChat", true, modifierLover);

        modifierAssassin = Create(10000, Types.Modifier, cs(Assassin.color, "modifierAssassin"), rates, null, true);
        modifierAssassinQuantity = Create(10001, Types.Modifier, "modifierAssassinQuantity", ratesCount, modifierAssassin);
        modifierAssassinNumberOfShots = Create(10002, Types.Modifier, "modifierAssassinNumberOfShots", 3f, 1f, 15f, 1f, modifierAssassin);
        modifierAssassinMultipleShotsPerMeeting = Create(10003, Types.Modifier, "modifierAssassinMultipleShotsPerMeeting", true, modifierAssassin);
        guesserEvilCanKillSpy = Create(10004, Types.Modifier, "guesserEvilCanKillSpy", true, modifierAssassin);
        guesserEvilCanKillCrewmate = Create(10005, Types.Modifier, "guesserEvilCanKillCrewmate", true, modifierAssassin);
        guesserCantGuessSnitchIfTaksDone = Create(10006, Types.Modifier, "guesserCantGuessSnitchIfTaksDone", true, modifierAssassin);
        modifierAssassinKillsThroughShield = Create(10007, Types.Modifier, "modifierAssassinKillsThroughShield", false, modifierAssassin);

        modifierSpecoality = Create(40350, Types.Modifier, cs(Palette.ImpostorRed, "Specoality"), rates, null, true);
        modifierSpecoalityIsGlobal = Create(40351, Types.Modifier, "modifierSpecoalityIsGlobal", false, modifierSpecoality);

        modifierDisperser = Create(40100, Types.Modifier, cs(Palette.ImpostorRed, "Disperser"), rates, null, true);
        modifierDisperserDispersesToVent = Create(40101, Types.Modifier, "modifierDisperserDispersesToVent", true, modifierDisperser);

        modifierPoucher = Create(40370, Types.Modifier, cs(Palette.ImpostorRed, "Poucher"), rates, null, true, false, () =>
        {
            poucherSpawnRate.selection = 0;
        });

        modifierVortox = Create(40380, Types.Modifier, cs(Vortox.color, "Vortox"), rates, null, true);
        modifierVortoxSkipMeeting = Create(40381, Types.Modifier, "modifierVortoxSkipMeeting", true, modifierVortox);
        modifierVortoxSkipNum = Create(40382, Types.Modifier, "modifierVortoxSkipNum", 4, 1, 10, 1, modifierVortoxSkipMeeting);

        modifierLastImpostor = Create(40110, Types.Modifier, cs(Palette.ImpostorRed, "LastImpostor"), false, null, true);
        modifierLastImpostorDeduce = Create(40111, Types.Modifier, "modifierLastImpostorDeduce", 5f, 2.5f, 15f, 2.5f, modifierLastImpostor);

        modifierBloody = Create(40120, Types.Modifier, cs(Color.yellow, "Bloody"), rates, null, true);
        modifierBloodyQuantity = Create(40121, Types.Modifier, cs(Color.yellow, "modifierBloodyQuantity"), ratesCount, modifierBloody);
        modifierBloodyDuration = Create(40122, Types.Modifier, "modifierBloodyDuration", 10f, 3f, 60f, 0.5f, modifierBloody);

        modifierAntiTeleport = Create(40130, Types.Modifier, cs(Color.yellow, "AntiTeleport"), rates, null, true);
        modifierAntiTeleportQuantity = Create(40131, Types.Modifier, cs(Color.yellow, "modifierAntiTeleportQuantity"), ratesCount, modifierAntiTeleport);

        modifierTieBreaker = Create(40140, Types.Modifier, cs(Color.yellow, "TieBreaker"), rates, null, true);

        modifierBait = Create(40150, Types.Modifier, cs(Color.yellow, "Bait"), rates, null, true);
        modifierBaitSwapCrewmate = Create(40151, Types.Modifier, "modifierBaitSwapCrewmate", false, modifierBait);
        modifierBaitReportDelayMin = Create(40152, Types.Modifier, "modifierBaitReportDelayMin", 0f, 0f, 10f, 0.125f, modifierBait);
        modifierBaitReportDelayMax = Create(40153, Types.Modifier, "modifierBaitReportDelayMax", 0.5f, 0f, 10f, 0.5f, modifierBait);
        modifierBaitShowKillFlash = Create(40154, Types.Modifier, "modifierBaitShowKillFlash", true, modifierBait);

        modifierAftermath = Create(40360, Types.Modifier, cs(Color.yellow, "Aftermath"), rates, null, true);

        modifierSunglasses = Create(40170, Types.Modifier, cs(Color.yellow, "Sunglasses"), rates, null, true);
        modifierSunglassesQuantity = Create(40171, Types.Modifier, cs(Color.yellow, "modifierSunglassesQuantity"), ratesCount, modifierSunglasses);
        modifierSunglassesVision = Create(40172, Types.Modifier, "modifierSunglassesVision", ["-10%", "-20%", "-30%", "-40%", "-50%"], modifierSunglasses);

        modifierTorch = Create(40180, Types.Modifier, cs(Color.yellow, "Torch"), rates, null, true);
        modifierTorchQuantity = Create(40181, Types.Modifier, cs(Color.yellow, "modifierTorchQuantity"), ratesCount, modifierTorch);
        modifierTorchVision = Create(40182, Types.Modifier, "modifierTorchVision", 1.5f, 1f, 3f, 0.125f, modifierTorch);

        modifierFlash = Create(40190, Types.Modifier, cs(Color.yellow, "Flash"), rates, null, true);
        modifierFlashQuantity = Create(40191, Types.Modifier, cs(Color.yellow, "modifierFlashQuantity"), ratesCount, modifierFlash);
        modifierFlashSpeed = Create(40192, Types.Modifier, "modifierFlashSpeed", 1.25f, 1f, 3f, 0.125f, modifierFlash);

        modifierMultitasker = Create(40200, Types.Modifier, cs(Color.yellow, "Multitasker"), rates, null, true);
        modifierMultitaskerQuantity = Create(40201, Types.Modifier, cs(Color.yellow, "modifierMultitaskerQuantity"), ratesCount, modifierMultitasker);

        modifierMini = Create(40210, Types.Modifier, cs(Color.yellow, "Mini"), rates, null, true);
        modifierMiniGrowingUpDuration = Create(40211, Types.Modifier, "modifierMiniGrowingUpDuration", 400f, 100f, 1500f, 25f, modifierMini);
        modifierMiniGrowingUpInMeeting = Create(40212, Types.Modifier, "modifierMiniGrowingUpInMeeting", true, modifierMini);

        modifierGiant = Create(40220, Types.Modifier, cs(Color.yellow, "Giant"), rates, null, true);
        modifierGiantSpped = Create(40221, Types.Modifier, "modifierGiantSpped", 0.75f, 0.5f, 1.5f, 0.05f, modifierGiant);

        modifierIndomitable = Create(40230, Types.Modifier, cs(Color.yellow, "Indomitable"), rates, null, true);

        modifierBlind = Create(40240, Types.Modifier, cs(Color.yellow, "Blind"), rates, null, true);

        modifierWatcher = Create(40250, Types.Modifier, cs(Color.yellow, "Watcher"), rates, null, true);

        modifierRadar = Create(40260, Types.Modifier, cs(Color.yellow, "Radar"), rates, null, true);

        modifierTunneler = Create(40270, Types.Modifier, cs(Color.yellow, "Tunneler"), rates, null, true);

        modifierButtonBarry = Create(40280, Types.Modifier, cs(Color.yellow, "ButtonBarry"), rates, null, true);
        modifierButtonSabotageRemoteMeetings = Create(40281, Types.Modifier, "modifierButtonSabotageRemoteMeetings", true, modifierButtonBarry);

        modifierSlueth = Create(40290, Types.Modifier, cs(Color.yellow, "Slueth"), rates, null, true);

        modifierCursed = Create(40300, Types.Modifier, cs(Color.yellow, "Cursed"), rates, null, true);
        modifierHideCursed = Create(40301, Types.Modifier, "modifierShowCursed", false, modifierCursed);

        modifierVip = Create(40310, Types.Modifier, cs(Color.yellow, "Vip"), rates, null, true);
        modifierVipQuantity = Create(40311, Types.Modifier, cs(Color.yellow, "modifierVipQuantity"), ratesCount, modifierVip);
        modifierVipShowColor = Create(40312, Types.Modifier, "modifierVipShowColor", true, modifierVip);

        modifierInvert = Create(40320, Types.Modifier, cs(Color.yellow, "Invert"), rates, null, true);
        modifierInvertQuantity = Create(40321, Types.Modifier, cs(Color.yellow, "modifierInvertQuantity"), ratesCount, modifierInvert);
        modifierInvertDuration = Create(40322, Types.Modifier, "modifierInvertDuration", 2f, 1f, 15f, 1f, modifierInvert);

        modifierChameleon = Create(40330, Types.Modifier, cs(Color.yellow, "Chameleon"), rates, null, true);
        modifierChameleonQuantity = Create(40331, Types.Modifier, cs(Color.yellow, "modifierChameleonQuantity"), ratesCount, modifierChameleon);
        modifierChameleonHoldDuration = Create(40332, Types.Modifier, "modifierChameleonHoldDuration", 3f, 1f, 10f, 0.5f, modifierChameleon);
        modifierChameleonFadeDuration = Create(40333, Types.Modifier, "modifierChameleonFadeDuration", 1f, 0.25f, 10f, 0.25f, modifierChameleon);
        modifierChameleonMinVisibility = Create(40334, Types.Modifier, "modifierChameleonMinVisibility", ["0%", "10%", "20%", "30%", "40%", "50%"], modifierChameleon);

        modifierShifter = Create(40340, Types.Modifier, cs(Color.yellow, "Shifter"), rates, null, true);
        modifierShiftNeutral = Create(40341, Types.Modifier, "modifierShiftNeutral", false, modifierShifter);
        modifierShiftALLNeutral = Create(40342, Types.Modifier, "modifierShiftALLNeutral", false, modifierShiftNeutral);

        //-------------------------- Guesser Gamemode 2000 - 2999 -------------------------- //

        guesserGamemodeCrewNumber = Create(2001, Types.Guesser, cs(Color.yellow, "guesserGamemodeCrewNumber"), 2f, 0f, 15f, 1f, null, true);
        guesserGamemodeNeutralNumber = Create(2002, Types.Guesser, cs(Color.yellow, "guesserGamemodeNeutralNumber"), 2f, 0f, 15f, 1f);
        guesserGamemodeImpNumber = Create(2003, Types.Guesser, cs(Color.yellow, "guesserGamemodeImpNumber"), 2f, 0f, 15f, 1f);
        guesserForceJackalGuesser = Create(2007, Types.Guesser, "guesserForceJackalGuesser", false, null, true);
        guesserGamemodeSidekickIsAlwaysGuesser = Create(2012, Types.Guesser, "guesserGamemodeSidekickIsAlwaysGuesser", false);
        guesserForcePavlovsGuesser = Create(2013, Types.Guesser, "guesserForcePavlovsGuesser", false);
        guesserGamemodePavlovsdogIsAlwaysGuesser = Create(2015, Types.Guesser, "guesserGamemodePavlovsdogIsAlwaysGuesser", false);
        guesserForceThiefGuesser = Create(2011, Types.Guesser, "guesserForceThiefGuesser", false);
        guesserGamemodeHaveModifier = Create(2004, Types.Guesser, "guesserGamemodeHaveModifier", true, null, true);
        guesserGamemodeNumberOfShots = Create(2005, Types.Guesser, "guesserGamemodeNumberOfShots", 3f, 1f, 15f, 1f);
        guesserGamemodeHasMultipleShotsPerMeeting = Create(2006, Types.Guesser, "guesserGamemodeHasMultipleShotsPerMeeting", true);
        guesserGamemodeKillsThroughShield = Create(2008, Types.Guesser, "guesserGamemodeKillsThroughShield", true);
        guesserGamemodeEvilCanKillSpy = Create(2009, Types.Guesser, "guesserGamemodeEvilCanKillSpy", true);
        guesserGamemodeCantGuessSnitchIfTaksDone = Create(2010, Types.Guesser, "guesserGamemodeCantGuessSnitchIfTaksDone", true);
    }
}
