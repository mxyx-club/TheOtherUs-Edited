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
        ["preset1", "preset2", "preset3", "PresetSkeld", "PresetMira", "PresetPolus", "PresetAirship", "PresetFungle", "PresetCustomMap"];

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

    public static CustomOption resteButtonCooldown;
    public static CustomOption shieldFirstKill;
    public static CustomOption hidePlayerNames;
    public static CustomOption hideOutOfSightNametags;
    public static CustomOption hideVentAnimOnShadows;
    public static CustomOption showButtonTarget;
    public static CustomOption impostorSeeRoles;
    public static CustomOption blockGameEnd;
    public static CustomOption randomLigherPlayer;
    //public static CustomOption showVentsOnMap;
    public static CustomOption randomGameStartPosition;
    public static CustomOption randomGameStartToVents;
    public static CustomOption ghostSpeed;

    public static CustomOption impostorChatChannel;

    public static CustomOption MeetingOptions;
    public static CustomOption disableMeeting;
    public static CustomOption maxNumberOfMeetings;
    public static CustomOption blockSkippingInEmergencyMeetings;
    public static CustomOption noVoteIsSelfVote;
    public static CustomOption guessReVote;
    public static CustomOption guessExtendmeetingTime;
    public static CustomOption exiledController;
    public static CustomOption exiledRevealRole;
    public static CustomOption exiledShowTeamNum;
    public static CustomOption exiledShowTeamSelect;
    public static CustomOption playerDieReducedTime;
    public static CustomOption minMeetingTime;

    public static CustomOption NetworkTransformLowLevel;
    public static CustomOption NetworkTransformType;

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
    public static CustomOption funglSpawnType;
    public static CustomOption fungleElectrical;
    public static CustomOption TheFungleMushroomMixupOption;
    public static CustomOption TheFungleMushroomMixupCantOpenMeeting;
    public static CustomOption TheFungleMushroomMixupTime;
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
    public static CustomOption disableGameEnd;
    public static CustomOption logRpcSend;
    public static CustomOption enableOtherLog;

    public static CustomOption wolfLordSpawnRate;

    public static CustomOption poucherSpawnRate;

    public static CustomOption professionalSpawnRate;
    public static CustomOption professionalWhoCanSeeBodies;

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

    public static CustomOption eraserSpawnRate;
    public static CustomOption eraserCooldown;
    public static CustomOption eraserCanEraseAnyone;
    public static CustomOption erasercanEraseGuess;

    public static CustomOption butcherSpawnRate;
    public static CustomOption butcherDissectionCooldown;
    public static CustomOption butcherDissectionDuration;
    public static CustomOption butcherDissectedBodyCount;

    public static CustomOption mimicSpawnRate;

    public static CustomOption marionetteSpawnRate;
    public static CustomOption marionettePlaceCooldown;
    public static CustomOption marionetteDecoyDelayedDisplay;
    public static CustomOption marionetteDecoyPermanent;
    public static CustomOption marionetteResetPlaceAfterMeeting;
    public static CustomOption marionetteDecoyDuration;
    public static CustomOption marionetteSwapCooldown;
    public static CustomOption marionetteShowDecoy;
    public static CustomOption marionetteMonitoringCanMove;

    public static CustomOption tricksterSpawnRate;
    public static CustomOption tricksterPlaceBoxCooldown;
    public static CustomOption tricksterLightsOutCooldown;
    public static CustomOption tricksterLightsOutDuration;

    public static CustomOption cleanerSpawnRate;
    public static CustomOption cleanerCooldown;

    public static CustomOption warlockSpawnRate;
    public static CustomOption warlockCooldown;
    public static CustomOption warlockRootTime;

    public static CustomOption bountyHunterSpawnRate;
    public static CustomOption bountyHunterBountyDuration;
    public static CustomOption bountyHunterReducedCooldown;
    public static CustomOption bountyHunterPunishmentTime;
    public static CustomOption bountyHunterShowArrow;
    public static CustomOption bountyHunterArrowUpdateIntervall;
    public static CustomOption bountyHunterChangeTargetCooldown;

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

    public static CustomOption terroristSpawnRate;
    public static CustomOption terroristMode;
    public static CustomOption terroristBombCooldown;
    public static CustomOption terroristBombDestructionTime;
    public static CustomOption terroristBombDestructionRange;
    public static CustomOption terroristBombHearRange;
    public static CustomOption terroristDefuseDuration;
    public static CustomOption terroristBombCanDefuse;
    public static CustomOption terroristBombActiveAfter;

    public static CustomOption minerSpawnRate;
    public static CustomOption minerCooldown;

    public static CustomOption yoyoSpawnRate;
    public static CustomOption yoyoMarkCooldown;
    public static CustomOption yoyoBlinkDuration;
    public static CustomOption yoyoMarkStaysOverMeeting;
    public static CustomOption yoyoHasAdminTable;
    public static CustomOption yoyoAdminTableCooldown;
    public static CustomOption yoyoSilhouetteVisibility;

    public static CustomOption evilTrapperSpawnRate;
    public static CustomOption evilTrapperNumTrap;
    public static CustomOption evilTrapperExtensionTime;
    public static CustomOption evilTrapperCooldown;
    public static CustomOption evilTrapperKillTimer;
    public static CustomOption evilTrapperTrapRange;
    public static CustomOption evilTrapperMaxDistance;
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

    public static CustomOption gunsmithSpawnRate;
    public static CustomOption gunsmithKillCooldown;
    public static CustomOption gunsmithSetKillCooldown;
    public static CustomOption gunsmithMaxChangeCount;

    public static CustomOption berserkerSpawnRate;
    public static CustomOption berserkerKillCooldown;
    public static CustomOption berserkerRampageCooldown;
    public static CustomOption berserkerRampageDuration;

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

    public static CustomOption jesterSpawnRate;
    public static CustomOption jesterCanCallEmergency;
    public static CustomOption jesterCanVent;
    public static CustomOption jesterHasImpostorVision;
    public static CustomOption jesterCanDragDeadBody;
    public static CustomOption jesterDragingVelocity;

    public static CustomOption partTimerSpawnRate;
    public static CustomOption partTimerCooldown;
    public static CustomOption partTimerDeathTurn;
    public static CustomOption partTimerKnowsRole;

    public static CustomOption witnessSpawnRate;
    public static CustomOption witnessMarkTimer;
    public static CustomOption witnessWinCount;
    public static CustomOption witnessMeetingDie;
    public static CustomOption witnessSkipMeeting;

    public static CustomOption bandLeaderSpawnRate;
    public static CustomOption bandLeaderKillCooldown;
    public static CustomOption bandLeaderCreateCooldown;

    public static CustomOption jackalSpawnRate;
    public static CustomOption jackalChanceSwoop;
    public static CustomOption jackalKillCooldown;
    public static CustomOption jackalSwooperCooldown;
    public static CustomOption jackalSwooperDuration;
    public static CustomOption jackalCanUseVents;
    public static CustomOption jackalCanUseSabo;
    public static CustomOption jackalAndSidekickHaveImpostorVision;
    public static CustomOption jackalCanCreateSidekick;
    public static CustomOption jackalCreateSidekickCooldown;
    public static CustomOption jackalkillFakeImpostor;
    public static CustomOption sidekickCanKill;
    public static CustomOption sidekickCanUseVents;
    public static CustomOption sidekickPromotesToJackal;
    public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;

    public static CustomOption pavlovsownerSpawnRate;
    public static CustomOption onlyOneNeutralTeam;
    public static CustomOption pavlovsownerKillCooldown;
    public static CustomOption pavlovsownerCreateDogCooldown;
    public static CustomOption pavlovsownerCreateDogNum;
    public static CustomOption pavlovsownerCanUseSabo;
    public static CustomOption pavlovsownerHasImpostorVision;
    public static CustomOption pavlovsownerCanUseVents;
    public static CustomOption pavlovsownerRampage;
    public static CustomOption pavlovsownerRampageKillCooldown;
    public static CustomOption pavlovsownerRampageDeathTime;

    public static CustomOption infectedSpawnRate;
    public static CustomOption infectedKillCooldown;
    public static CustomOption infectedMaxPlayer;
    public static CustomOption infectedActiveLimit;
    public static CustomOption infectedCanUseVents;
    public static CustomOption infectedHasImpostorVision;
    public static CustomOption infectedCanUseGuess;
    public static CustomOption infectedGuessCount;

    public static CustomOption arsonistSpawnRate;
    public static CustomOption arsonistCooldown;
    public static CustomOption arsonistDuration;
    public static CustomOption arsonistIgniteCdRemoved;

    public static CustomOption pelicanSpawnRate;
    public static CustomOption pelicanCooldown;
    public static CustomOption pelicanReduceCooldown;
    public static CustomOption pelicanHasImpVision;
    public static CustomOption pelicanCanUseVents;

    public static CustomOption swooperSpawnRate;
    public static CustomOption swooperKillCooldown;
    public static CustomOption swooperCooldown;
    public static CustomOption swooperDuration;
    public static CustomOption swooperSpeed;
    public static CustomOption swooperCanUseVents;
    public static CustomOption swooperHasImpVision;

    public static CustomOption werewolfSpawnRate;
    public static CustomOption werewolfRampageCooldown;
    public static CustomOption werewolfRampageDuration;
    public static CustomOption werewolfKillCooldown;
    public static CustomOption werewolfCanUseVents;

    public static CustomOption juggernautSpawnRate;
    public static CustomOption juggernautCooldown;
    public static CustomOption juggernautHasImpVision;
    public static CustomOption juggernautCanUseVents;
    public static CustomOption juggernautReducedkillEach;

    public static CustomOption vultureSpawnRate;
    public static CustomOption vultureCooldown;
    public static CustomOption vultureNumberToWin;
    public static CustomOption vultureCanUseVents;
    public static CustomOption vultureShowArrows;

    public static CustomOption lawyerSpawnRate;
    public static CustomOption lawyerTargetKnows;
    public static CustomOption lawyerVision;
    public static CustomOption lawyerKnowsRole;
    public static CustomOption lawyerCanCallEmergency;
    public static CustomOption lawyerStolenWin;
    public static CustomOption lawyerTargetCanBeJester;

    //public static CustomOption pursuerSpawnRate;
    public static CustomOption pursuerBlanksCooldown;
    public static CustomOption pursuerBlanksNumber;

    public static CustomOption executionerSpawnRate;
    public static CustomOption executionerCanCallEmergency;
    public static CustomOption executionerPromotesToLawyer;
    //public static CustomOption executionerOnTargetDead;

    public static CustomOption doomsayerSpawnRate;
    public static CustomOption doomsayerCooldown;
    public static CustomOption doomsayerHasMultipleShotsPerMeeting;
    public static CustomOption doomsayerOnlineTarger;
    public static CustomOption doomsayerDormationNum;
    public static CustomOption doomsayerCanGuessImpostor;
    public static CustomOption doomsayerCanGuessNeutral;
    public static CustomOption doomsayerKillToWin;

    public static CustomOption schrodingersCatSpawnRate;
    public static CustomOption schrodingersCatCanKill;
    public static CustomOption schrodingersCatCooldown;
    public static CustomOption schrodingersCatHasImpVision;
    public static CustomOption schrodingersCatTeamChanges;
    public static CustomOption schrodingersCatMaxChangeCount;
    public static CustomOption schrodingersCatIsGuessable;

    public static CustomOption akujoSpawnRate;
    public static CustomOption akujoTimeLimit;
    public static CustomOption akujoForceKeeps;
    public static CustomOption akujoNumKeeps;
    public static CustomOption akujoKnowsRoles;
    public static CustomOption akujoHonmeiCannotFollowWin;
    public static CustomOption akujoHonmeiOptimizeWin;

    public static CustomOption thiefSpawnRate;
    public static CustomOption thiefCooldown;
    public static CustomOption thiefCanKillSheriff;
    public static CustomOption thiefCanKillDeputy;
    public static CustomOption thiefCanKillVeteran;
    public static CustomOption thiefHasImpVision;
    public static CustomOption thiefCanUseVents;
    public static CustomOption thiefCanStealWithGuess;

    public static CustomOption guesserSpawnRate;
    public static CustomOption guesserNumberOfShots;
    public static CustomOption guesserHasMultipleShotsPerMeeting;
    public static CustomOption guesserShowInfoInGhostChat;

    public static CustomOption sheriffSpawnRate;
    public static CustomOption sheriffCooldown;
    public static CustomOption sheriffMisfireKills;
    public static CustomOption sheriffCanKillNeutrals;
    public static CustomOption sheriffCanKillSurvivor;
    public static CustomOption sheriffCanKillAmnesiac;
    public static CustomOption sheriffCanKillPursuer;
    public static CustomOption sheriffCanKillPartTimer;
    public static CustomOption sheriffCanKillJester;
    public static CustomOption sheriffCanKillLawyer;
    public static CustomOption sheriffCanKillExecutioner;
    public static CustomOption sheriffCanKillVulture;
    public static CustomOption sheriffCanKillDoomsayer;
    public static CustomOption sheriffCanKillThief;
    public static CustomOption sheriffCanKillWitness;
    public static CustomOption sheriffCanKillBandLeader;

    public static CustomOption deputySpawnRate;
    public static CustomOption deputyNumberOfHandcuffs;
    public static CustomOption deputyHandcuffCooldown;
    public static CustomOption deputyHandcuffDuration;
    public static CustomOption deputyGetsPromoted;
    public static CustomOption deputyKnowsSheriff;
    public static CustomOption deputyKeepsHandcuffs;

    public static CustomOption mayorSpawnRate;
    public static CustomOption mayorMode;
    public static CustomOption mayorVote;
    public static CustomOption mayorAnonymousVote;
    public static CustomOption mayorMultiVoting;
    public static CustomOption mayorRevealVision;
    public static CustomOption mayorInitialVotes;
    public static CustomOption mayorAddVotes;
    public static CustomOption mayorMaxVotes;
    public static CustomOption mayorVoteLimit;
    public static CustomOption mayorSaveVoteAnime;
    public static CustomOption mayorMeetingButton;

    public static CustomOption prosecutorSpawnRate;
    public static CustomOption prosecutorCanSeeVoteColors;
    public static CustomOption prosecutorTasksNeededToSeeVoteColors;
    public static CustomOption prosecutorDiesOnIncorrectPros;
    public static CustomOption prosecutorCanCallEmergency;

    public static CustomOption veteranSpawnRate;
    public static CustomOption veteranCooldown;
    public static CustomOption veteranAlertDuration;

    public static CustomOption jailorSpawnRate;
    public static CustomOption jailorCooldown;
    public static CustomOption jailorUseCount;

    public static CustomOption engineerSpawnRate;
    public static CustomOption engineerRemoteFix;
    public static CustomOption engineerResetFixAfterMeeting;
    public static CustomOption engineerNumberOfFixes;
    public static CustomOption engineerHighlightForImpostors;
    public static CustomOption engineerHighlightForTeamJackal;

    public static CustomOption swapperSpawnRate;
    public static CustomOption swapperCanCallEmergency;
    public static CustomOption swapperCanFixSabotages;
    public static CustomOption swapperCanOnlySwapOthers;
    public static CustomOption swapperSwapsNumber;
    public static CustomOption swapperRechargeTasksNumber;

    public static CustomOption balancerSpawnRate;
    public static CustomOption balancerCount;
    public static CustomOption balancerVoteTime;

    public static CustomOption medicSpawnRate;
    public static CustomOption medicShowShielded;
    public static CustomOption medicBreakShield;
    public static CustomOption medicGuessShield;
    public static CustomOption medicShowAttemptToMedic;
    public static CustomOption medicShowAttemptToShielded;
    public static CustomOption medicResetTargetAfterMeeting;
    public static CustomOption medicSetOrShowShieldAfterMeeting;
    public static CustomOption medicReportNameDuration;
    public static CustomOption medicReportColorDuration;

    public static CustomOption detectiveSpawnRate;
    public static CustomOption detectiveAnonymousFootprints;
    public static CustomOption detectiveFootprintIntervall;
    public static CustomOption detectiveFootprintDuration;
    public static CustomOption detectiveReportNameDuration;
    public static CustomOption detectiveReportColorDuration;

    public static CustomOption redemptorSpawnRate;
    public static CustomOption redemptorRevelation;
    public static CustomOption redemptorRevelationCooldown;
    public static CustomOption redemptorRevelationDuration;
    public static CustomOption redemptorPrayer;
    public static CustomOption redemptorPrayerCooldown;
    public static CustomOption redemptorPrayerDuration;
    public static CustomOption redemptorReviveDuration;

    public static CustomOption bodyGuardSpawnRate;
    public static CustomOption bodyGuardResetTargetAfterMeeting;
    public static CustomOption bodyGuardShowShielded;
    public static CustomOption bodyGuardFlash;

    public static CustomOption seerSpawnRate;
    public static CustomOption seerMode;
    public static CustomOption seerLimitSoulDuration;
    public static CustomOption seerSoulDuration;

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
    public static CustomOption snitchCanSeeRoles;
    public static CustomOption snitchIncludeNeutralTeam;
    public static CustomOption snitchTeamNeutraUseDifferentArrowColor;
    public static CustomOption snitchCanGuessIfTaksDone;

    public static CustomOption prophetSpawnRate;
    public static CustomOption prophetCooldown;
    public static CustomOption prophetNumExamines;
    public static CustomOption prophetCanCallEmergency;
    public static CustomOption prophetIsRevealed;
    public static CustomOption prophetExaminesToBeRevealed;
    public static CustomOption prophetKillCrewAsRed;
    public static CustomOption prophetBenignNeutralAsRed;
    public static CustomOption prophetEvilNeutralAsRed;
    public static CustomOption prophetKillNeutralAsRed;

    public static CustomOption infoSleuthSpawnRate;
    public static CustomOption infoSleuthInfoType;

    public static CustomOption mediumSpawnRate;
    public static CustomOption mediumCooldown;
    public static CustomOption mediumDuration;
    public static CustomOption mediumOneTimeUse;
    public static CustomOption mediumChanceAdditionalInfo;

    public static CustomOption guesserEvilCanKillCrewmate;

    public static CustomOption portalmakerSpawnRate;
    public static CustomOption portalmakerCooldown;
    public static CustomOption portalmakerUsePortalCooldown;
    public static CustomOption portalmakerLogOnlyColorType;
    public static CustomOption portalmakerLogHasTime;
    public static CustomOption portalmakerCanPortalFromAnywhere;

    public static CustomOption spySpawnRate;
    public static CustomOption spyEvilCanKillSpy;
    public static CustomOption spyCanDieToSheriff;
    public static CustomOption spyImpostorsCanKillAnyone;
    public static CustomOption spyCanEnterVents;
    public static CustomOption spyHasImpostorVision;

    public static CustomOption securityGuardSpawnRate;
    public static CustomOption securityGuardCooldown;
    public static CustomOption securityGuardTotalScrews;
    public static CustomOption securityGuardCamPrice;
    public static CustomOption securityGuardVentPrice;
    public static CustomOption securityGuardCamDuration;
    public static CustomOption securityGuardCamMaxCharges;
    public static CustomOption securityGuardCamRechargeTasksNumber;
    public static CustomOption securityGuardNoMove;

    public static CustomOption jumperSpawnRate;
    public static CustomOption jumperJumpTime;
    public static CustomOption jumperResetPlaceAfterMeeting;
    public static CustomOption jumperChargesGainOnMeeting;
    public static CustomOption jumperMaxCharges;

    public static CustomOption trapperSpawnRate;
    public static CustomOption trapperCooldown;
    public static CustomOption trapperMaxCharges;
    public static CustomOption trapperRechargeTasksNumber;
    public static CustomOption trapperTrapNeededTriggerToReveal;
    public static CustomOption trapperInfoType;
    public static CustomOption trapperTrapDuration;

    public static CustomOption modifiersAreHidden;

    public static CustomOption modifierAssassin;
    public static CustomOption modifierAssassinQuantity;
    public static CustomOption modifierAssassinNumberOfShots;
    public static CustomOption modifierAssassinMultipleShotsPerMeeting;
    public static CustomOption modifierAssassinKillsThroughShield;

    public static CustomOption modifierVortox;
    public static CustomOption modifierVortoxReversal;
    public static CustomOption modifierVortoxSkipMeeting;
    public static CustomOption modifierVortoxSkipNum;

    public static CustomOption modifierPoucher;

    public static CustomOption modifierProfessional;
    public static CustomOption modifierProfessionalWhoCanSeeBodies;

    public static CustomOption modifierSpecoality;
    public static CustomOption modifierSpecoalityIsGlobal;

    public static CustomOption modifierLastImpostor;
    public static CustomOption modifierLastImpostorDeduce;

    public static CustomOption modifierBait;
    public static CustomOption modifierBaitReportDelayMin;
    public static CustomOption modifierBaitReportDelayMax;
    public static CustomOption modifierBaitShowKillFlash;
    public static CustomOption modifierBaitSwapCrewmate;

    public static CustomOption modifierAftermath;

    public static CustomOption modifierLover;
    public static CustomOption modifierLoverImpLoverRate;
    public static CustomOption modifierLoverNeutraValid;
    public static CustomOption modifierLoverCanGetModifiers;
    public static CustomOption modifierLoverAvengerChance;
    public static CustomOption modifierLoverEnableChat;

    public static CustomOption avengerIsGuessable;
    public static CustomOption avengerKillCooldown;
    public static CustomOption avengerCanFreeKill;
    public static CustomOption avengerKnowTarget;
    public static CustomOption avengerTargetKnowPlayer;
    public static CustomOption avengerShowArrows;
    public static CustomOption avengerUpdateIntervall;
    public static CustomOption avengerHasImpVision;
    public static CustomOption avengerCanUseVents;
    public static CustomOption avengerOnlyAliveWin;
    public static CustomOption avengerWinCondition;
    public static CustomOption avengerTargetWasKilledByOther;
    public static CustomOption avengerTargetWasExiled;

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
    public static CustomOption modifierTunnelerNoTask;

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

    public static CustomOption modifierChameleon;
    public static CustomOption modifierChameleonQuantity;
    public static CustomOption modifierChameleonHoldDuration;
    public static CustomOption modifierChameleonFadeDuration;
    public static CustomOption modifierChameleonMinVisibility;

    public static CustomOption modifierShifter;
    public static CustomOption modifierShiftNeutral;
    public static CustomOption modifierShiftALLNeutral;
    public static CustomOption modifierShiftReload;


    public static CustomOption clogSpawnRate;
    public static CustomOption clogGhostRange;
    public static CustomOption clogUseNum;
    public static CustomOption clogOnlyUsedOnce;
    public static CustomOption clogGhostCooldown;
    public static CustomOption clogGhostDuration;


    public static CustomOption specterSpawnRate;
    public static CustomOption specterResetRole;
    public static CustomOption specterDuration;
    public static CustomOption specterAfterMeetingTakeRole;
    public static CustomOption specterAfterMeetingRevived;


    public static CustomOption ghostEngineerSpawnRate;

    public static CustomOption poltergeistSpawnRate;
    public static CustomOption poltergeistCooldown;
    public static CustomOption poltergeistRadius;


    public static void Load()
    {
        VanillaSettings = Main.Instance.Config.Bind("Preset0", "VanillaOptions", "");

        //-------------------------- Role options 0 - 99 -------------------------- //
        presetSelection = Create(0, Types.General, Cs(new Color32(204, 204, 0, 255), "presetSelection"), presets, null, true);
        anyPlayerCanStopStart = Create(3, Types.General, Cs(new Color32(204, 204, 0, 255), "anyPlayerCanStopStart"), false);

        isDraftMode = Create(900, Types.General, Cs(Color.yellow, "isDraftMode"), false, null, true,
            onChange: (x) => neutralRolesCountMax.IsHeader = isDraftMode.GetBool());
        draftModeAmountOfChoices = Create(901, Types.General, Cs(Color.yellow, "draftModeAmountOfChoices"), 3, 2, 6, 1, isDraftMode, false);
        draftModeTimeToChoose = Create(902, Types.General, Cs(Color.yellow, "draftModeTimeToChoose"), 5, 3, 20, 1, isDraftMode);
        draftModeShowRoles = Create(903, Types.General, Cs(Color.yellow, "draftModeShowRoles"), false, isDraftMode, false);
        draftModeHideImpRoles = Create(904, Types.General, Cs(Color.yellow, "draftModeHideImpRoles"), false, draftModeShowRoles, false);
        draftModeHideNeutralRoles = Create(905, Types.General, Cs(Color.yellow, "draftModeHideNeutralRoles"), false, draftModeShowRoles, false);
        draftModeHideCrewmateRoles = Create(906, Types.General, Cs(Color.yellow, "draftModeHideCrewmateRoles"), false, draftModeShowRoles, false);

        neutralRolesCountMin = Create(8, Types.General, Cs(new Color32(204, 204, 0, 255), "neutralRolesCountMin"), 2, 0, 15, 1, null, true,
            isHidden: () => isDraftMode.GetBool(),
            onChange: (x) => x.SyncMinMax(false, neutralRolesCountMax));
        neutralRolesCountMax = Create(9, Types.General, Cs(new Color32(204, 204, 0, 255), "neutralRolesCountMax"), 2, 0, 15, 1,
            isHeader: isDraftMode.GetBool(),
            onChange: (x) => x.SyncMinMax(true, neutralRolesCountMin, killerNeutralRolesCountMax));
        killerNeutralRolesCountMin = Create(10, Types.General, Cs(new Color32(204, 204, 0, 255), "killerNeutralRolesCountMin"), ratesRandom,
            isHidden: () => isDraftMode.GetBool(),
            onChange: (x) => x.SyncMinMax(false, killerNeutralRolesCountMax));
        killerNeutralRolesCountMax = Create(11, Types.General, Cs(new Color32(204, 204, 0, 255), "killerNeutralRolesCountMax"), ratesRandom,
            onChange: (x) => { x.SyncMinMax(true, killerNeutralRolesCountMin); x.SyncMinMax(false, neutralRolesCountMax); });
        modifiersCountMin = Create(12, Types.General, Cs(new Color32(204, 204, 0, 255), "modifiersCountMin"), 15, 0, 30, 1,
            onChange: (x) => x.SyncMinMax(false, modifiersCountMax));
        modifiersCountMax = Create(13, Types.General, Cs(new Color32(204, 204, 0, 255), "modifiersCountMax"), 15, 0, 30, 1,
            onChange: (x) => x.SyncMinMax(true, modifiersCountMin));

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
        //showVentsOnMap = Create(115, Types.General, "ShowVentsOnMap", ["optionOff", "ShowVentsOnMeetingMap", "optionOn"]);
        randomGameStartPosition = Create(110, Types.General, "randomGameStartPosition", false);
        randomGameStartToVents = Create(111, Types.General, "randomGameStartToVents", true, randomGameStartPosition);
        ghostSpeed = Create(112, Types.General, "ghostSpeed", 1f, 0.75f, 5f, 0.125f);

        impostorChatChannel = Create(701, Types.General, Cs(Palette.ImpostorRed, "ImpostorChatChannel"),
            ["optionOff", "ImpostorChatChannel.1", "ImpostorChatChannel.2", "optionOn"], null, true);

        //Meeting options
        MeetingOptions = Create(200, Types.General, Cs(new Color32(255, 85, 234, byte.MaxValue), "MeetingOptions"), false, null, true);
        disableMeeting = Create(201, Types.General, "disableMeeting", false, MeetingOptions);
        maxNumberOfMeetings = Create(202, Types.General, "maxNumberOfMeetings", 10, 0, 15, 1, MeetingOptions);
        blockSkippingInEmergencyMeetings = Create(203, Types.General, "blockSkippingInEmergencyMeetings", false, MeetingOptions);
        noVoteIsSelfVote = Create(204, Types.General, "noVoteIsSelfVote", false, blockSkippingInEmergencyMeetings);
        guessReVote = Create(205, Types.General, "guessReVote", false, MeetingOptions);
        guessExtendmeetingTime = Create(206, Types.General, "guessExtendmeetingTime", 15f, 0f, 60f, 5f, guessReVote);
        playerDieReducedTime = Create(210, Types.General, "playerDieReducedTime", 0f, 0f, 30f, 2.5f, MeetingOptions);
        minMeetingTime = Create(211, Types.General, "minMeetingTime", 60f, 30f, 180f, 5f, playerDieReducedTime);
        exiledController = Create(207, Types.General, "exileController", false, MeetingOptions);
        exiledRevealRole = Create(208, Types.General, "exiledRevealRole", ["optionOff", "Role", "Team"], exiledController);
        exiledShowTeamNum = Create(209, Types.General, "exiledShowTeamNum", false, exiledController);
        exiledShowTeamSelect = Create(212, Types.General, "exiledShowTeamSelect", ["ShowTeamSelect0", "ShowTeamSelect1", "ShowTeamSelect2"], exiledShowTeamNum);

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
        SkeldReactorTimeLimit = Create(411, Types.General, "SkeldReactorTimeLimit", 30f, 0f, 120f, 2.5f, IsReactorDurationSetting);
        SkeldLifeSuppTimeLimit = Create(412, Types.General, "SkeldLifeSuppTimeLimit", 30f, 0f, 120f, 2.5f, IsReactorDurationSetting);
        MiraLifeSuppTimeLimit = Create(413, Types.General, "MiraLifeSuppTimeLimit", 30f, 0f, 120f, 2.5f, IsReactorDurationSetting);
        MiraReactorTimeLimit = Create(414, Types.General, "MiraReactorTimeLimit", 30f, 0f, 120f, 2.5f, IsReactorDurationSetting);
        PolusReactorTimeLimit = Create(415, Types.General, "PolusReactorTimeLimit", 60f, 0f, 120f, 2.5f, IsReactorDurationSetting);
        AirshipReactorTimeLimit = Create(416, Types.General, "AirshipReactorTimeLimit", 75f, 0f, 120f, 2.5f, IsReactorDurationSetting);
        FungleReactorTimeLimit = Create(417, Types.General, "FungleReactorTimeLimit", 45f, 0f, 120f, 2.5f, IsReactorDurationSetting);

        //Map options
        MapOptions = Create(500, Types.General, Cs(new Color32(223, 157, 192, byte.MaxValue), "MapOptions"), false, null, true);
        //The Skeld
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
        funglSpawnType = Create(555, Types.General, "funglSpawnType", ["Random", "Select"], enableFungleModify);
        fungleElectrical = Create(551, Types.General, "fungleElectrical", false, enableFungleModify);
        TheFungleMushroomMixupOption = Create(552, Types.General, "TheFungleMushroomMixupOption", false, enableFungleModify);
        TheFungleMushroomMixupTime = Create(553, Types.General, "TheFungleMushroomMixupTime", 10f, 1f, 30f, 0.5f, TheFungleMushroomMixupOption);
        TheFungleMushroomMixupCantOpenMeeting = Create(554, Types.General, "TheFungleMushroomMixupCantOpenMeeting", false, TheFungleMushroomMixupOption);
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
        disableGameEnd = Create(951, Types.General, "DisableGameEnd", false, debugMode);
        logRpcSend = Create(952, Types.General, "logRpcSend", false);
        enableOtherLog = Create(953, Types.General, "enableOtherLog", false);

        //-------------------------- Impostor Options 100000 -------------------------- //

        wolfLordSpawnRate = Create(101000, Types.Impostor, Cs(WolfLord.color, "WolfLord"), rates, null, true);

        poucherSpawnRate = Create(103200, Types.Impostor, Cs(Palette.ImpostorRed, "Poucher"), rates, null, true,
            onChange: (x) => { if (modifierPoucher.Selection > 0 && x.Selection > 0) x.updateSelection(0); });

        professionalSpawnRate = Create(103700, Types.Impostor, Cs(Palette.ImpostorRed, "Professional"), rates, null, true,
            onChange: (x) => { if (modifierProfessional.Selection > 0 && x.Selection > 0) x.updateSelection(0); });
        professionalWhoCanSeeBodies = Create(103701, Types.Impostor, "professionalWhoCanSeeBodies",
            ["professionalWhoCanSeeBodies.1", "professionalWhoCanSeeBodies.2", "professionalWhoCanSeeBodies.3"], professionalSpawnRate,
            onChange: (x) => { if (modifierProfessionalWhoCanSeeBodies.Selection != x.Selection) modifierProfessionalWhoCanSeeBodies.updateSelection(x.Selection); });

        morphlingSpawnRate = Create(101100, Types.Impostor, Cs(Morphling.color, "Morphling"), rates, null, true);
        morphlingCooldown = Create(101101, Types.Impostor, "morphlingCooldown", 15f, 10f, 60f, 2.5f, morphlingSpawnRate);
        morphlingDuration = Create(101102, Types.Impostor, "morphlingDuration", 15f, 1f, 20f, 0.5f, morphlingSpawnRate);

        bomberSpawnRate = Create(101200, Types.Impostor, Cs(Bomber.color, "Bomber"), rates, null, true);
        bomberBombCooldown = Create(101201, Types.Impostor, "bomberBombCooldown", 25f, 10f, 60f, 2.5f, bomberSpawnRate);
        bomberDelay = Create(101202, Types.Impostor, "bomberDelay", 5f, 0f, 20f, 0.5f, bomberSpawnRate);
        bomberTimer = Create(101203, Types.Impostor, "bomberTimer", 10f, 5f, 30f, 0.5f, bomberSpawnRate);
        bomberTriggerBothCooldowns = Create(101204, Types.Impostor, "bomberTriggerBothCooldowns", false, bomberSpawnRate);
        bomberCanGiveToBomber = Create(101205, Types.Impostor, "bomberCanGiveToBomber", false, bomberSpawnRate);
        bomberHotPotatoMode = Create(101206, Types.Impostor, "bomberHotPotatoMode", true, bomberSpawnRate);

        undertakerSpawnRate = Create(101300, Types.Impostor, Cs(Undertaker.color, "Undertaker"), rates, null, true);
        undertakerDragingDelaiAfterKill = Create(101301, Types.Impostor, "undertakerDragingDelaiAfterKill", 0f, 0f, 15, 0.5f, undertakerSpawnRate);
        undertakerDragingAfterVelocity = Create(101302, Types.Impostor, "undertakerDragingAfterVelocity", 0.75f, 0.5f, 1.5f, 0.125f, undertakerSpawnRate);
        undertakerCanDragAndVent = Create(101303, Types.Impostor, "undertakerCanDragAndVent", true, undertakerSpawnRate);

        camouflagerSpawnRate = Create(101400, Types.Impostor, Cs(Camouflager.color, "Camouflager"), rates, null, true);
        camouflagerCooldown = Create(101401, Types.Impostor, "camouflagerCooldown", 25f, 10f, 60f, 2.5f, camouflagerSpawnRate);
        camouflagerDuration = Create(101402, Types.Impostor, "camouflagerDuration", 12.5f, 1f, 20f, 0.5f, camouflagerSpawnRate);

        vampireSpawnRate = Create(101500, Types.Impostor, Cs(Vampire.color, "Vampire"), rates, null, true);
        vampireKillDelay = Create(101501, Types.Impostor, "vampireKillDelay", 5f, 1f, 10f, 0.5f, vampireSpawnRate);
        vampireCooldown = Create(101502, Types.Impostor, "vampireCooldown", 25f, 10f, 60f, 2.5f, vampireSpawnRate);
        vampireGarlicButton = Create(101503, Types.Impostor, "vampireGarlicButton", true, vampireSpawnRate);
        vampireCanKillNearGarlics = Create(101504, Types.Impostor, "vampireCanKillNearGarlics", true, vampireGarlicButton);

        eraserSpawnRate = Create(101600, Types.Impostor, Cs(Eraser.color, "Eraser"), rates, null, true);
        eraserCooldown = Create(101601, Types.Impostor, "eraserCooldown", 25f, 10f, 120f, 2.5f, eraserSpawnRate);
        eraserCanEraseAnyone = Create(101602, Types.Impostor, "eraserCanEraseAnyone", false, eraserSpawnRate);
        erasercanEraseGuess = Create(101603, Types.Impostor, "erasercanEraseGuess", false, eraserSpawnRate);

        butcherSpawnRate = Create(103100, Types.Impostor, Cs(Palette.ImpostorRed, "Butcher"), rates, null, true);
        butcherDissectionCooldown = Create(103101, Types.Impostor, "butcherDissectionCooldown", 25f, 10f, 60f, 2.5f, butcherSpawnRate);
        butcherDissectionDuration = Create(103102, Types.Impostor, "butcherDissectionDuration", 1f, 0f, 10f, 0.25f, butcherSpawnRate);
        butcherDissectedBodyCount = Create(103103, Types.Impostor, "butcherDissectedBodyCount", 5, 3, 15, 1, butcherSpawnRate);

        mimicSpawnRate = Create(101700, Types.Impostor, Cs(Mimic.color, "Mimic"), rates, null, true);

        marionetteSpawnRate = Create(101800, Types.Impostor, Cs(Marionette.color, "Marionette"), rates, null, true);
        marionettePlaceCooldown = Create(101801, Types.Impostor, "marionettePlaceCooldown", 10f, 5f, 30f, 2.5f, marionetteSpawnRate);
        marionetteDecoyDelayedDisplay = Create(101802, Types.Impostor, "marionetteDecoyDelayedDisplay", 10f, 5f, 30f, 2.5f, marionetteSpawnRate);
        marionetteDecoyPermanent = Create(101803, Types.Impostor, "marionetteDecoyPermanent", true, marionetteSpawnRate);
        marionetteResetPlaceAfterMeeting = Create(101804, Types.Impostor, "marionetteResetPlaceAfterMeeting", false, marionetteDecoyPermanent);
        marionetteDecoyDuration = Create(101805, Types.Impostor, "marionetteDecoyDuration", 60f, 25f, 120f, 5f, marionetteSpawnRate,
            isHidden: () => marionetteDecoyPermanent.GetBool());
        marionetteSwapCooldown = Create(101806, Types.Impostor, "marionetteSwapCooldown", 10f, 5f, 45f, 2.5f, marionetteSpawnRate);
        marionetteShowDecoy = Create(101807, Types.Impostor, "marionetteShowDecoy", ["marionetteShowDecoy1", "marionetteShowDecoy2", "marionetteShowDecoy3"], marionetteSpawnRate);
        marionetteMonitoringCanMove = Create(101808, Types.Impostor, "marionetteMonitoringCanMove", true, marionetteSpawnRate);

        tricksterSpawnRate = Create(102000, Types.Impostor, Cs(Trickster.color, "Trickster"), rates, null, true);
        tricksterPlaceBoxCooldown = Create(102001, Types.Impostor, "tricksterPlaceBoxCooldown", 20f, 2.5f, 30f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutCooldown = Create(102002, Types.Impostor, "tricksterLightsOutCooldown", 25f, 10f, 60f, 2.5f, tricksterSpawnRate);
        tricksterLightsOutDuration = Create(102003, Types.Impostor, "tricksterLightsOutDuration", 12.5f, 5f, 60f, 0.5f, tricksterSpawnRate);

        cleanerSpawnRate = Create(102100, Types.Impostor, Cs(Cleaner.color, "Cleaner"), rates, null, true);
        cleanerCooldown = Create(102101, Types.Impostor, "cleanerCooldown", 25f, 10f, 60f, 2.5f, cleanerSpawnRate);

        warlockSpawnRate = Create(102200, Types.Impostor, Cs(Warlock.color, "Warlock"), rates, null, true);
        warlockCooldown = Create(102201, Types.Impostor, "warlockCooldown", 20f, 10f, 60f, 2.5f, warlockSpawnRate);
        warlockRootTime = Create(102202, Types.Impostor, "warlockRootTime", 3f, 0f, 15f, 0.25f, warlockSpawnRate);

        bountyHunterSpawnRate = Create(102300, Types.Impostor, Cs(BountyHunter.color, "BountyHunter"), rates, null, true);
        bountyHunterBountyDuration = Create(102301, Types.Impostor, "bountyHunterBountyDuration", 60f, 10f, 180f, 5f, bountyHunterSpawnRate);
        bountyHunterReducedCooldown = Create(102302, Types.Impostor, "bountyHunterReducedCooldown", 2.5f, 0f, 30f, 0.5f, bountyHunterSpawnRate);
        bountyHunterPunishmentTime = Create(102303, Types.Impostor, "bountyHunterPunishmentTime", 10f, 0f, 60f, 2.5f, bountyHunterSpawnRate);
        bountyHunterShowArrow = Create(102304, Types.Impostor, "bountyHunterShowArrow", true, bountyHunterSpawnRate);
        bountyHunterArrowUpdateIntervall = Create(102305, Types.Impostor, "bountyHunterArrowUpdateIntervall", 0.5f, 0f, 15f, 0.5f, bountyHunterShowArrow);
        bountyHunterChangeTargetCooldown = Create(102306, Types.Impostor, "bountyHunterChangeTargetCooldown", 30f, 15f, 90f, 2.5f, bountyHunterSpawnRate);

        witchSpawnRate = Create(102400, Types.Impostor, Cs(Witch.color, "Witch"), rates, null, true);
        witchCooldown = Create(102401, Types.Impostor, "witchCooldown", 20f, 10f, 60, 2.5f, witchSpawnRate);
        witchAdditionalCooldown = Create(102402, Types.Impostor, "witchAdditionalCooldown", 5f, 0f, 60f, 2.5f, witchSpawnRate);
        witchCanSpellAnyone = Create(102403, Types.Impostor, "witchCanSpellAnyone", false, witchSpawnRate);
        witchSpellCastingDuration = Create(102404, Types.Impostor, "witchSpellCastingDuration", 0.5f, 0f, 10f, 0.25f, witchSpawnRate);
        witchTriggerBothCooldowns = Create(102405, Types.Impostor, "witchTriggerBothCooldowns", false, witchSpawnRate);
        witchVoteSavesTargets = Create(102406, Types.Impostor, "witchVoteSavesTargets", true, witchSpawnRate);

        ninjaSpawnRate = Create(102500, Types.Impostor, Cs(Ninja.color, "Ninja"), rates, null, true);
        ninjaCooldown = Create(102501, Types.Impostor, "ninjaCooldown", 20f, 10f, 60f, 2.5f, ninjaSpawnRate);
        ninjaKnowsTargetLocation = Create(102502, Types.Impostor, "ninjaKnowsTargetLocation", true, ninjaSpawnRate);
        ninjaTraceTime = Create(102503, Types.Impostor, "ninjaTraceTime", 6f, 1f, 20f, 0.5f, ninjaSpawnRate);
        ninjaTraceColorTime = Create(102504, Types.Impostor, "ninjaTraceColorTime", 3f, 0f, 20f, 0.5f, ninjaSpawnRate);
        ninjaInvisibleDuration = Create(102505, Types.Impostor, "ninjaInvisibleDuration", 10f, 0f, 20f, 0.5f, ninjaSpawnRate);

        blackmailerSpawnRate = Create(102600, Types.Impostor, Cs(Blackmailer.color, "Blackmailer"), rates, null, true);
        blackmailerCooldown = Create(102601, Types.Impostor, "blackmailerCooldown", 15f, 5f, 120f, 2.5f, blackmailerSpawnRate);

        terroristSpawnRate = Create(102700, Types.Impostor, Cs(Terrorist.color, "Terrorist"), rates, null, true);
        terroristMode = Create(102701, Types.Impostor, "terroristMode", ["terroristMode1", "terroristMode2"], terroristSpawnRate);
        terroristBombCooldown = Create(102702, Types.Impostor, "terroristBombCooldown", 15f, 5f, 60f, 2.5f, terroristSpawnRate);
        terroristBombActiveAfter = Create(102703, Types.Impostor, "terroristBombActiveAfter", 15f, 1f, 15f, 0.5f, terroristSpawnRate,
            isHidden: () => { return terroristMode.Selection == 0; });
        terroristBombDestructionTime = Create(102704, Types.Impostor, "terroristBombDestructionTime", 0f, 0f, 120f, 0.5f, terroristSpawnRate,
            isHidden: () => { return terroristMode.Selection == 0; });
        terroristBombDestructionRange = Create(102705, Types.Impostor, "terroristBombDestructionRange", 35f, 5f, 250f, 5f, terroristSpawnRate);
        terroristBombHearRange = Create(102706, Types.Impostor, "terroristBombHearRange", 60f, 5f, 250f, 5f, terroristSpawnRate);
        terroristBombCanDefuse = Create(102707, Types.Impostor, "terroristBombCanDefuse", true, terroristSpawnRate,
            isHidden: () => { return terroristMode.Selection == 0; });
        terroristDefuseDuration = Create(102708, Types.Impostor, "terroristDefuseDuration", 2f, 0f, 30f, 0.5f, terroristBombCanDefuse);

        minerSpawnRate = Create(102800, Types.Impostor, Cs(Miner.color, "Miner"), rates, null, true);
        minerCooldown = Create(102801, Types.Impostor, "minerCooldown", 20f, 10f, 60f, 2.5f, minerSpawnRate);

        yoyoSpawnRate = Create(102900, Types.Impostor, Cs(Yoyo.color, "Yoyo"), rates, null, true);
        yoyoMarkCooldown = Create(102901, Types.Impostor, "yoyoMarkCooldown", 15f, 2.5f, 120f, 2.5f, yoyoSpawnRate);
        yoyoBlinkDuration = Create(102902, Types.Impostor, "yoyoBlinkDuration", 15f, 2.5f, 120f, 2.5f, yoyoSpawnRate);
        yoyoMarkStaysOverMeeting = Create(102903, Types.Impostor, "yoyoMarkStaysOverMeeting", false, yoyoSpawnRate);
        yoyoHasAdminTable = Create(102904, Types.Impostor, "yoyoHasAdminTable", true, yoyoSpawnRate);
        yoyoAdminTableCooldown = Create(102905, Types.Impostor, "yoyoAdminTableCooldown", 15f, 2.5f, 120f, 2.5f, yoyoHasAdminTable);
        yoyoSilhouetteVisibility = Create(102906, Types.Impostor, "yoyoSilhouetteVisibility", ["0%", "10%", "20%", "30%", "40%", "50%"], yoyoSpawnRate);

        evilTrapperSpawnRate = Create(103000, Types.Impostor, Cs(EvilTrapper.color, "EvilTrapper"), rates, null, true);
        evilTrapperNumTrap = Create(103001, Types.Impostor, "evilTrapperNumTrap", 2, 1, 10, 1, evilTrapperSpawnRate);
        evilTrapperExtensionTime = Create(103002, Types.Impostor, "evilTrapperExtensionTime", 5f, 2f, 10f, 0.5f, evilTrapperSpawnRate);
        evilTrapperCooldown = Create(103003, Types.Impostor, "evilTrapperCooldown", 15f, 10f, 60f, 2.5f, evilTrapperSpawnRate);
        evilTrapperKillTimer = Create(103004, Types.Impostor, "evilTrapperKillTimer", 5f, 1f, 30f, 1f, evilTrapperSpawnRate);
        evilTrapperTrapRange = Create(103005, Types.Impostor, "evilTrapperTrapRange", 0.5f, 0.2f, 1.5f, 0.1f, evilTrapperSpawnRate);
        evilTrapperMaxDistance = Create(103006, Types.Impostor, "evilTrapperMaxDistance", 10f, 0f, 20f, 0.25f, evilTrapperSpawnRate);
        evilTrapperPenaltyTime = Create(103007, Types.Impostor, "evilTrapperPenaltyTime", 0f, 0f, 30f, 0.5f, evilTrapperSpawnRate);
        evilTrapperBonusTime = Create(103008, Types.Impostor, "evilTrapperBonusTime", 10f, 0f, 15f, 0.5f, evilTrapperSpawnRate);

        gamblerSpawnRate = Create(103300, Types.Impostor, Cs(Gambler.color, "Gambler"), rates, null, true);
        gamblerMinCooldown = Create(103301, Types.Impostor, "gamblerMinCooldown", 2.5f, 0f, 45f, 0.5f, gamblerSpawnRate);
        gamblerMaxCooldown = Create(103302, Types.Impostor, "gamblerMaxCooldown", 40f, 10f, 90f, 2.5f, gamblerSpawnRate);
        gamblerSuccessRate = Create(103303, Types.Impostor, "gamblerSuccessRate", rates, gamblerSpawnRate);

        grenadierSpawnRate = Create(103400, Types.Impostor, Cs(Grenadier.color, "Grenadier"), rates, null, true);
        grenadierCooldown = Create(103401, Types.Impostor, "grenadierCooldown", 20f, 0f, 45f, 2.5f, grenadierSpawnRate);
        grenadierDuration = Create(103402, Types.Impostor, "grenadierDuration", 8f, 4f, 10f, 0.5f, grenadierSpawnRate);
        grenadierFlashRadius = Create(103403, Types.Impostor, "grenadierFlashRadius", 1f, 0.25f, 5f, 0.125f, grenadierSpawnRate);
        grenadierTeamIndicators = Create(103404, Types.Impostor, "grenadierTeamIndicators", true, grenadierSpawnRate);

        gunsmithSpawnRate = Create(103500, Types.Impostor, Cs(Gunsmith.color, "Gunsmith"), rates, null, true);
        gunsmithKillCooldown = Create(103501, Types.Impostor, "killCooldown", 25f, 10f, 60f, 2.5f, gunsmithSpawnRate);
        gunsmithSetKillCooldown = Create(103502, Types.Impostor, "gunsmithSetKillCooldown", 0f, 0f, 7.5f, 0.5f, gunsmithSpawnRate);
        gunsmithMaxChangeCount = Create(103503, Types.Impostor, "gunsmithMaxChangeCount", 5, 1, 15, 1, gunsmithSpawnRate);

        berserkerSpawnRate = Create(103600, Types.Impostor, Cs(Berserker.color, "Berserker"), rates, null, true);
        berserkerKillCooldown = Create(103601, Types.Impostor, "killCooldown", 25f, 10f, 60f, 2.5f, berserkerSpawnRate);
        berserkerRampageCooldown = Create(103602, Types.Impostor, "berserkerRampageCooldown", 10f, 5f, 60f, 0.5f, berserkerSpawnRate);
        berserkerRampageDuration = Create(103603, Types.Impostor, "berserkerKillDuration", 3f, 0.5f, 10f, 0.25f, berserkerSpawnRate);

        //-------------------------- Neutral Options 200000 -------------------------- //

        survivorSpawnRate = Create(202800, Types.Neutral, Cs(Survivor.color, "Survivor"), rates, null, true);
        survivorVestEnable = Create(202801, Types.Neutral, "survivorVestEnable", true, survivorSpawnRate);
        survivorVestNumber = Create(202802, Types.Neutral, "survivorVestNumber", 5f, 1f, 20f, 1f, survivorVestEnable);
        survivorVestCooldown = Create(202803, Types.Neutral, "survivorVestCooldown", 20f, 2.5f, 60f, 2.5f, survivorVestEnable);
        survivorVestDuration = Create(202804, Types.Neutral, "survivorVestDuration", 10f, 2.5f, 60f, 0.5f, survivorVestEnable);
        survivorVestResetCooldown = Create(202805, Types.Neutral, "survivorVestResetCooldown", 5f, 2.5f, 60f, 2.5f, survivorVestEnable);
        survivorBlanksEnable = Create(202806, Types.Neutral, "survivorBlanksEnable", false, survivorSpawnRate);
        survivorBlanksCooldown = Create(202807, Types.Neutral, "survivorBlanksCooldown", 20f, 5f, 60f, 2.5f, survivorBlanksEnable);
        survivorBlanksNumber = Create(202808, Types.Neutral, "survivorBlanksNumber", 6f, 1f, 20f, 1f, survivorBlanksEnable);

        amnisiacSpawnRate = Create(201100, Types.Neutral, Cs(Amnisiac.color, "Amnisiac"), rates, null, true);
        amnisiacShowArrows = Create(201101, Types.Neutral, "amnisiacShowArrows", true, amnisiacSpawnRate);
        amnisiacResetRole = Create(201102, Types.Neutral, "amnisiacResetRole", true, amnisiacSpawnRate);

        jesterSpawnRate = Create(201000, Types.Neutral, Cs(Jester.color, "Jester"), rates, null, true);
        jesterCanCallEmergency = Create(201001, Types.Neutral, "canCallEmergency", true, jesterSpawnRate);
        jesterCanVent = Create(201002, Types.Neutral, "jesterCanVent", true, jesterSpawnRate);
        jesterHasImpostorVision = Create(201003, Types.Neutral, "hasImpVision", true, jesterSpawnRate);
        jesterCanDragDeadBody = Create(201004, Types.Neutral, "jesterCanDragDeadBody", true, jesterSpawnRate);
        jesterDragingVelocity = Create(201005, Types.Neutral, "undertakerDragingAfterVelocity", 0.75f, 0.5f, 1.5f, 0.125f, jesterCanDragDeadBody);

        partTimerSpawnRate = Create(202900, Types.Neutral, Cs(PartTimer.color, "PartTimer"), rates, null, true);
        partTimerCooldown = Create(202901, Types.Neutral, "partTimerCooldown", 20f, 2.5f, 60f, 2.5f, partTimerSpawnRate);
        partTimerDeathTurn = Create(202902, Types.Neutral, "partTimerDeathTurn", 2, 1, 6, 1, partTimerSpawnRate);
        partTimerKnowsRole = Create(202903, Types.Neutral, "partTimerIsCheckTargetRole", true, partTimerSpawnRate);

        witnessSpawnRate = Create(203000, Types.Neutral, Cs(Witness.color, "Witness"), rates, null, true);
        witnessMarkTimer = Create(203001, Types.Neutral, "witnessMarkTimer", 30, 20, 90, 5, witnessSpawnRate);
        witnessWinCount = Create(203002, Types.Neutral, "witnessWinCount", 2, 1, 6, 1, witnessSpawnRate);
        witnessMeetingDie = Create(203003, Types.Neutral, "witnessMeetingDie", true, witnessSpawnRate);
        witnessSkipMeeting = Create(203004, Types.Neutral, "witnessSkipMeeting", true, witnessSpawnRate);

        bandLeaderSpawnRate = Create(203200, Types.Neutral, Cs(BandLeader.color, "BandLeader"), rates, null, true);
        bandLeaderCreateCooldown = Create(203201, Types.Neutral, "bandLeaderCreateDistance", 5f, 2.5f, 30f, 0.5f, bandLeaderSpawnRate);
        bandLeaderKillCooldown = Create(203202, Types.Neutral, "bandLeaderKillCooldown", 20f, 2.5f, 60f, 2.5f, bandLeaderSpawnRate);

        onlyOneNeutralTeam = Create(200000, Types.Neutral, "onlyOneNeutralTeam", true, null, true);

        jackalSpawnRate = Create(201300, Types.Neutral, Cs(Jackal.color, "Jackal"), rates, null, true);
        jackalChanceSwoop = Create(201301, Types.Neutral, Cs(Swooper.color, "jackalChanceSwoop"), rates, jackalSpawnRate);
        jackalSwooperCooldown = Create(201303, Types.Neutral, "jackalSwooperCooldown", 25f, 10f, 60f, 2.5f, jackalChanceSwoop);
        jackalSwooperDuration = Create(201304, Types.Neutral, "jackalSwooperDuration", 12.5f, 1f, 20f, 0.5f, jackalChanceSwoop);
        jackalKillCooldown = Create(201302, Types.Neutral, "killCooldown", 25f, 10f, 60f, 2.5f, jackalSpawnRate);
        jackalCanUseVents = Create(201305, Types.Neutral, "jackalCanUseVents", true, jackalSpawnRate);
        jackalCanUseSabo = Create(201306, Types.Neutral, "jackalCanUseSabo", false, jackalSpawnRate);
        jackalAndSidekickHaveImpostorVision = Create(201307, Types.Neutral, "jackalAndSidekickHaveImpostorVision", true, jackalSpawnRate);
        jackalCanCreateSidekick = Create(201308, Types.Neutral, Cs(Jackal.color, "jackalCanCreateSidekick"), false, jackalSpawnRate);
        jackalCreateSidekickCooldown = Create(201309, Types.Neutral, "jackalCreateSidekickCooldown", 25f, 10f, 60f, 2.5f, jackalCanCreateSidekick);
        jackalkillFakeImpostor = Create(201310, Types.Neutral, Cs(Palette.ImpostorRed, "jackalkillFakeImpostor"), false, jackalCanCreateSidekick);
        sidekickCanKill = Create(201311, Types.Neutral, "sidekickCanKill", true, jackalCanCreateSidekick);
        sidekickCanUseVents = Create(201312, Types.Neutral, "sidekickCanUseVents", true, jackalCanCreateSidekick);
        sidekickPromotesToJackal = Create(201313, Types.Neutral, "sidekickPromotesToJackal", false, jackalCanCreateSidekick);
        jackalPromotedFromSidekickCanCreateSidekick = Create(201314, Types.Neutral, "jackalPromotedFromSidekickCanCreateSidekick", true, sidekickPromotesToJackal);

        pavlovsownerSpawnRate = Create(202500, Types.Neutral, Cs(Pavlovsdogs.color, "Pavlovsowner"), rates, null, true);
        pavlovsownerKillCooldown = Create(202501, Types.Neutral, "killCooldown", 25f, 10f, 60f, 2.5f, pavlovsownerSpawnRate);
        pavlovsownerCreateDogCooldown = Create(202502, Types.Neutral, "pavlovsownerCreateDogCooldown", 25f, 10f, 60f, 2.5f, pavlovsownerSpawnRate);
        pavlovsownerCreateDogNum = Create(202503, Types.Neutral, "pavlovsownerCreateDogNum", 3f, 1f, 15f, 1f, pavlovsownerSpawnRate);
        pavlovsownerCanUseSabo = Create(202504, Types.Neutral, "pavlovsownerCanUseSabo", true, pavlovsownerSpawnRate);
        pavlovsownerHasImpostorVision = Create(202505, Types.Neutral, "hasImpVision", true, pavlovsownerSpawnRate);
        pavlovsownerCanUseVents = Create(202506, Types.Neutral, "pavlovsownerCanUseVents",
            ["Pavlovsdogs", "Pavlovsowner", "pavlovsownerCanUseVents3"], pavlovsownerSpawnRate);
        pavlovsownerRampage = Create(202507, Types.Neutral, "pavlovsownerRampage", true, pavlovsownerSpawnRate);
        pavlovsownerRampageKillCooldown = Create(202508, Types.Neutral, "pavlovsownerRampageKillCooldown", 15f, 5f, 60f, 2.5f, pavlovsownerRampage);
        pavlovsownerRampageDeathTime = Create(202509, Types.Neutral, "pavlovsownerRampageDeathTime", 60f, 30f, 180f, 2.5f, pavlovsownerRampageKillCooldown);

        infectedSpawnRate = Create(201600, Types.Neutral, Cs(Infected.color, "Infected"), rates, null, true);
        infectedKillCooldown = Create(201601, Types.Neutral, "killCooldown", 25f, 10f, 60f, 2.5f, infectedSpawnRate);
        infectedMaxPlayer = Create(201602, Types.Neutral, "infectedMaxPlayer", 3, 2, 15, 1, infectedSpawnRate);
        infectedActiveLimit = Create(201603, Types.Neutral, "infectedActiveLimit", 2, 2, 15, 1, infectedSpawnRate);
        infectedCanUseVents = Create(201604, Types.Neutral, "canUseVents", true, infectedSpawnRate);
        infectedHasImpostorVision = Create(201605, Types.Neutral, "hasImpVision", true, infectedSpawnRate);
        infectedCanUseGuess = Create(201606, Types.Neutral, "infectedCanUseGuess", true, infectedSpawnRate);
        infectedGuessCount = Create(201607, Types.Neutral, "infectedGuessCount", 3, 1, 15, 1, infectedCanUseGuess);

        arsonistSpawnRate = Create(201200, Types.Neutral, Cs(Arsonist.color, "Arsonist"), rates, null, true);
        arsonistCooldown = Create(201201, Types.Neutral, "arsonistCooldown", 12.5f, 5f, 60f, 2.5f, arsonistSpawnRate);
        arsonistDuration = Create(201202, Types.Neutral, "arsonistDuration", 0.25f, 0f, 10f, 0.125f, arsonistSpawnRate);
        arsonistIgniteCdRemoved = Create(201203, Types.Neutral, "arsonistIgniteCdRemoved", false, arsonistSpawnRate);

        pelicanSpawnRate = Create(203100, Types.Neutral, Cs(Pelican.color, "Pelican"), rates, null, true);
        pelicanCooldown = Create(203101, Types.Neutral, "pelicanCooldown", 25f, 2.5f, 60f, 2.5f, pelicanSpawnRate);
        pelicanReduceCooldown = Create(203102, Types.Neutral, "pelicanReduceCooldown", 20f, 2.5f, 60f, 2.5f, pelicanSpawnRate);
        pelicanCanUseVents = Create(203103, Types.Neutral, "canUseVents", true, pelicanSpawnRate);
        pelicanHasImpVision = Create(203104, Types.Neutral, "hasImpVision", true, pelicanSpawnRate);

        swooperSpawnRate = Create(201500, Types.Neutral, Cs(Swooper.color, "Swooper"), rates, null, true);
        swooperKillCooldown = Create(201501, Types.Neutral, "killCooldown", 25f, 10f, 60f, 2.5f, swooperSpawnRate);
        swooperCooldown = Create(201502, Types.Neutral, "swooperCooldown", 20f, 10f, 60f, 2.5f, swooperSpawnRate);
        swooperDuration = Create(201503, Types.Neutral, "swooperDuration", 15f, 1f, 20f, 0.5f, swooperSpawnRate);
        swooperSpeed = Create(201504, Types.Neutral, "swooperSpeed", 1.5f, 1f, 3f, 0.125f, swooperSpawnRate);
        swooperCanUseVents = Create(201505, Types.Neutral, "canUseVents", true, swooperSpawnRate);
        swooperHasImpVision = Create(201506, Types.Neutral, "hasImpVision", true, swooperSpawnRate);

        werewolfSpawnRate = Create(202000, Types.Neutral, Cs(Werewolf.color, "Werewolf"), rates, null, true);
        werewolfRampageCooldown = Create(202001, Types.Neutral, "werewolfRampageCooldown", 25f, 10f, 60f, 2.5f, werewolfSpawnRate);
        werewolfRampageDuration = Create(202002, Types.Neutral, "werewolfRampageDuration", 15f, 0.5f, 20f, 0.5f, werewolfSpawnRate);
        werewolfKillCooldown = Create(202003, Types.Neutral, "werewolfKillCooldown", 3f, 1f, 60f, 0.5f, werewolfSpawnRate);
        werewolfCanUseVents = Create(202004, Types.Neutral, "canUseVents", ["optionOff", "werewolfCanUseVents1", "optionOn"], werewolfSpawnRate);

        juggernautSpawnRate = Create(202100, Types.Neutral, Cs(Juggernaut.color, "Juggernaut"), rates, null, true);
        juggernautCooldown = Create(202101, Types.Neutral, "killCooldown", 25f, 2.5f, 60f, 2.5f, juggernautSpawnRate);
        juggernautHasImpVision = Create(202102, Types.Neutral, "hasImpVision", true, juggernautSpawnRate);
        juggernautCanUseVents = Create(201103, Types.Neutral, "canUseVents", true, juggernautSpawnRate);
        juggernautReducedkillEach = Create(201104, Types.Neutral, "juggernautReducedkillEach", 5f, 1f, 15f, 0.5f, juggernautSpawnRate);

        vultureSpawnRate = Create(201700, Types.Neutral, Cs(Vulture.color, "Vulture"), rates, null, true);
        vultureCooldown = Create(201701, Types.Neutral, "vultureCooldown", 12.5f, 10f, 60f, 2.5f, vultureSpawnRate);
        vultureNumberToWin = Create(201702, Types.Neutral, "vultureNumberToWin", 3f, 1f, 15f, 1f, vultureSpawnRate);
        vultureCanUseVents = Create(201703, Types.Neutral, "canUseVents", true, vultureSpawnRate);
        vultureShowArrows = Create(201704, Types.Neutral, "vultureShowArrows", true, vultureSpawnRate);

        lawyerSpawnRate = Create(201800, Types.Neutral, Cs(Lawyer.color, "Lawyer"), rates, null, true);
        lawyerTargetKnows = Create(201802, Types.Neutral, "lawyerTargetKnows", true, lawyerSpawnRate);
        lawyerVision = Create(201803, Types.Neutral, "lawyerVision", 1.5f, 0.25f, 3f, 0.25f, lawyerSpawnRate);
        lawyerKnowsRole = Create(201804, Types.Neutral, "lawyerKnowsRole", true, lawyerSpawnRate);
        lawyerCanCallEmergency = Create(201805, Types.Neutral, "canCallEmergency", true, lawyerSpawnRate);
        lawyerStolenWin = Create(201806, Types.Neutral, "lawyerStolenWin", false, lawyerSpawnRate);
        lawyerTargetCanBeJester = Create(201807, Types.Neutral, "lawyerTargetCanBeJester", false, lawyerSpawnRate);

        executionerSpawnRate = Create(201900, Types.Neutral, Cs(Executioner.color, "Executioner"), rates, null, true);
        executionerCanCallEmergency = Create(201901, Types.Neutral, "canCallEmergency", true, executionerSpawnRate);
        executionerPromotesToLawyer = Create(201902, Types.Neutral, "executionerPromotesToLawyer", true, executionerSpawnRate);
        //executionerOnTargetDead = Create(201903, Types.Neutral, "目标死亡后变为", [Cs(Pursuer.color, "Pursuer"), Cs(Jester.color, "Jester"), Cs(Amnisiac.color, "Amnisiac"), "Crewmate"], executionerSpawnRate);

        //pursuerSpawnRate = Create(202700, Types.Neutral, cs(Pursuer.color, "Pursuer"), rates, null, true);
        pursuerBlanksCooldown = Create(202701, Types.Neutral, "pursuerBlanksCooldown", 20f, 5f, 60f, 2.5f, lawyerSpawnRate);
        pursuerBlanksNumber = Create(202702, Types.Neutral, "pursuerBlanksNumber", 6f, 1f, 20f, 1f, lawyerSpawnRate);

        doomsayerSpawnRate = Create(202200, Types.Neutral, Cs(Doomsayer.color, "Doomsayer"), rates, null, true);
        doomsayerCooldown = Create(202201, Types.Neutral, "doomsayerCooldown", 20f, 2.5f, 60f, 2.5f, doomsayerSpawnRate);
        doomsayerHasMultipleShotsPerMeeting = Create(202202, Types.Neutral, "doomsayerHasMultipleShotsPerMeeting", true, doomsayerSpawnRate);
        doomsayerOnlineTarger = Create(202203, Types.Neutral, "doomsayerOnlineTarger", false, doomsayerSpawnRate);
        doomsayerDormationNum = Create(202204, Types.Neutral, "doomsayerDormationNum", 5f, 2f, 10f, 1f, doomsayerSpawnRate);
        doomsayerCanGuessImpostor = Create(202205, Types.Neutral, $"{"doomsayerCanGuess".Translate()} {Cs(Palette.ImpostorRed, "ImpostorRolesText".Translate())}", true, doomsayerSpawnRate);
        doomsayerCanGuessNeutral = Create(202206, Types.Neutral, $"{"doomsayerCanGuess".Translate()} {Cs(Color.gray, "NeutralRolesText".Translate())}", true, doomsayerSpawnRate);
        doomsayerKillToWin = Create(202207, Types.Neutral, "doomsayerKillToWin", 3f, 1f, 10f, 1f, doomsayerSpawnRate);

        schrodingersCatSpawnRate = Create(203300, Types.Neutral, Cs(SchrodingersCat.color, "SchrodingersCat"), rates, null, true);
        schrodingersCatIsGuessable = Create(203301, Types.Neutral, "schrodingersCatIsGuessable", ["optionOff", "schrodingersCatIsGuessable1", "optionOn"], schrodingersCatSpawnRate);
        schrodingersCatCanKill = Create(203302, Types.Neutral, "schrodingersCatCanKill", true, schrodingersCatSpawnRate);
        schrodingersCatCooldown = Create(203303, Types.Neutral, "killCooldown", 20f, 2.5f, 60f, 2.5f, schrodingersCatCanKill);
        schrodingersCatHasImpVision = Create(203304, Types.Neutral, "hasImpVision", true, schrodingersCatSpawnRate);
        schrodingersCatTeamChanges = Create(203305, Types.Neutral, "schrodingersCatTeamChanges", true, schrodingersCatSpawnRate);
        schrodingersCatMaxChangeCount = Create(203306, Types.Neutral, "schrodingersCatMaxChangeCount", 3, 1, 15, 1, schrodingersCatTeamChanges);

        akujoSpawnRate = Create(202300, Types.Neutral, Cs(Akujo.color, "Akujo"), rates, null, true);
        akujoTimeLimit = Create(202301, Types.Neutral, "akujoTimeLimit", 450f, 120f, 1200f, 30f, akujoSpawnRate);
        akujoForceKeeps = Create(202302, Types.Neutral, "akujoForceKeeps", false, akujoSpawnRate);
        akujoNumKeeps = Create(202303, Types.Neutral, "akujoNumKeeps", 1f, 0f, 5f, 1f, akujoSpawnRate);
        akujoKnowsRoles = Create(20234, Types.Neutral, "akujoKnowsRoles", true, akujoSpawnRate);
        akujoHonmeiCannotFollowWin = Create(202305, Types.Neutral, "akujoHonmeiCannotFollowWin", true, akujoSpawnRate);
        akujoHonmeiOptimizeWin = Create(202306, Types.Neutral, "akujoHonmeiOptimizeWin", true, akujoSpawnRate);

        thiefSpawnRate = Create(202400, Types.Neutral, Cs(Thief.color, "Thief"), rates, null, true);
        thiefCooldown = Create(202401, Types.Neutral, "killCooldown", 25f, 5f, 120f, 2.5f, thiefSpawnRate);
        thiefCanKillSheriff = Create(202402, Types.Neutral, $"{"thiefCanKill".Translate()}{Cs(Sheriff.color, "Sheriff".Translate())}", true, thiefSpawnRate);
        thiefCanKillDeputy = Create(202403, Types.Neutral, $"{"thiefCanKill".Translate()}{Cs(Sheriff.color, "Deputy".Translate())}", true, thiefSpawnRate);
        thiefCanKillVeteran = Create(202404, Types.Neutral, $"{"thiefCanKill".Translate()}{Cs(Veteran.color, "Veteran".Translate())}", true, thiefSpawnRate);
        thiefHasImpVision = Create(202405, Types.Neutral, "hasImpVision", true, thiefSpawnRate);
        thiefCanUseVents = Create(202406, Types.Neutral, "canUseVents", true, thiefSpawnRate);
        thiefCanStealWithGuess = Create(202407, Types.Neutral, "thiefCanStealWithGuess", true, thiefSpawnRate);

        //-------------------------- Crewmate Options 300000 -------------------------- //

        guesserSpawnRate = Create(301000, Types.Crewmate, Cs(Vigilante.color, "Vigilante"), rates, null, true,
            isHidden: () => GuesserGM.Enabled);
        guesserNumberOfShots = Create(301001, Types.Crewmate, "guesserNumberOfShots", 3f, 1f, 15f, 1f, guesserSpawnRate,
            isHidden: () => GuesserGM.Enabled);
        guesserHasMultipleShotsPerMeeting = Create(301002, Types.Crewmate, "guesserHasMultipleShotsPerMeeting", true, guesserSpawnRate,
            isHidden: () => GuesserGM.Enabled);
        guesserShowInfoInGhostChat = Create(301003, Types.Crewmate, "guesserShowInfoInGhostChat", true, guesserSpawnRate,
            isHidden: () => GuesserGM.Enabled);

        sheriffSpawnRate = Create(301400, Types.Crewmate, Cs(Sheriff.color, "Sheriff"), rates, null, true);
        sheriffCooldown = Create(301401, Types.Crewmate, "sheriffCooldown", 25f, 10f, 60f, 2.5f, sheriffSpawnRate);
        sheriffMisfireKills = Create(301402, Types.Crewmate, "sheriffMisfireKills",
            ["sheriffMisfireKills1", "sheriffMisfireKills2", "sheriffMisfireKills3"], sheriffSpawnRate);
        sheriffCanKillNeutrals = Create(301421, Types.Crewmate, "sheriffCanKillNeutrals", true, sheriffSpawnRate);
        sheriffCanKillAmnesiac = Create(301422, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Amnisiac.color, "Amnisiac".Translate())}", false, sheriffCanKillNeutrals);
        sheriffCanKillSurvivor = Create(301423, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Survivor.color, "Survivor".Translate())}", false, sheriffCanKillNeutrals);
        sheriffCanKillPursuer = Create(301424, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Pursuer.color, "Pursuer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillPartTimer = Create(301425, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(PartTimer.color, "PartTimer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillBandLeader = Create(301426, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(BandLeader.color, "BandLeader".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillJester = Create(301427, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Jester.color, "Jester".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillLawyer = Create(301428, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Lawyer.color, "Lawyer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillExecutioner = Create(301429, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Executioner.color, "Executioner".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillVulture = Create(301430, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Vulture.color, "Vulture".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillDoomsayer = Create(301431, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Doomsayer.color, "Doomsayer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillWitness = Create(301432, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Witness.color, "Witness".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillThief = Create(301433, Types.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Thief.color, "Thief".Translate())}", true, sheriffCanKillNeutrals);

        deputySpawnRate = Create(301700, Types.Crewmate, Cs(Sheriff.color, "Deputy"), rates, null);
        deputyNumberOfHandcuffs = Create(301701, Types.Crewmate, "deputyNumberOfHandcuffs", 5f, 1f, 15f, 1f, deputySpawnRate);
        deputyHandcuffCooldown = Create(301702, Types.Crewmate, "deputyHandcuffCooldown", 20f, 10f, 60f, 2.5f, deputySpawnRate);
        deputyHandcuffDuration = Create(301703, Types.Crewmate, "deputyHandcuffDuration", 10f, 5f, 60f, 2.5f, deputySpawnRate);
        deputyGetsPromoted = Create(301704, Types.Crewmate, "deputyGetsPromoted",
            ["optionOff", "deputyGetsPromoted2", "deputyGetsPromoted3"], deputySpawnRate);
        deputyKnowsSheriff = Create(301705, Types.Crewmate, "deputyKnowsSheriff", true, deputySpawnRate);
        deputyKeepsHandcuffs = Create(301706, Types.Crewmate, "deputyKeepsHandcuffs", true, deputyGetsPromoted);

        jailorSpawnRate = Create(304100, Types.Crewmate, Cs(Jailor.color, "Jailor"), rates, null, true);
        jailorCooldown = Create(304101, Types.Crewmate, "jailorCooldown", 20f, 10f, 60f, 2.5f, jailorSpawnRate);
        jailorUseCount = Create(304102, Types.Crewmate, "jailorUseCount", 3, 1, 15, 1, jailorSpawnRate);

        mayorSpawnRate = Create(301100, Types.Crewmate, Cs(Mayor.color, "Mayor"), rates, null, true);
        mayorMode = Create(301101, Types.Crewmate, "mayorMode", ["mayorMode0", "mayorMode1", "mayorMode2"], mayorSpawnRate);
        mayorVote = Create(301102, Types.Crewmate, "mayorVote", 2, 1, 5, 1, mayorSpawnRate, false,
            isHidden: () => mayorMode.Selection == 2);
        mayorMultiVoting = Create(301103, Types.Crewmate, "mayorMultiVoting", true, mayorSpawnRate, false,
            isHidden: () => mayorMode.Selection != 0);
        mayorRevealVision = Create(301104, Types.Crewmate, "mayorRevealVision", ["-20%", "-30%", "-40%", "-50%"], mayorSpawnRate, false,
            isHidden: () => mayorMode.Selection != 1);
        mayorInitialVotes = Create(301105, Types.Crewmate, "mayorInitialVotes", 3f, 1f, 3f, 0.5f, mayorSpawnRate, false,
            isHidden: () => mayorMode.Selection != 2);
        mayorAddVotes = Create(301106, Types.Crewmate, "mayorAddVotes", 1f, 1f, 2.5f, 0.25f, mayorSpawnRate, false,
            isHidden: () => mayorMode.Selection != 2);
        mayorMaxVotes = Create(301107, Types.Crewmate, "mayorMaxVoteCount", 5, 1, 15, 1, mayorSpawnRate, false,
            isHidden: () => mayorMode.Selection != 2);
        mayorVoteLimit = Create(301108, Types.Crewmate, "mayorOneUseVotes", 4, 2, 9, 1, mayorSpawnRate, false,
            isHidden: () => mayorMode.Selection != 2);
        /*mayorSaveVoteAnime = Create(301130, Types.Crewmate, "mayorSaveVoteAnime", true, mayorSpawnRate, false,
            isHidden: () => mayorMode.selection != 2);*/
        mayorAnonymousVote = Create(301131, Types.Crewmate, "mayorAnonymousVote", true, mayorSpawnRate, false,
            isHidden: () => mayorMode.Selection == 1);
        mayorMeetingButton = Create(301109, Types.Crewmate, "mayorMeetingButton", false, mayorSpawnRate);

        prosecutorSpawnRate = Create(303700, Types.Crewmate, Cs(Prosecutor.color, "Prosecutor"), rates, null, true);
        prosecutorCanSeeVoteColors = Create(303701, Types.Crewmate, "mayorCanSeeVoteColors", true, prosecutorSpawnRate);
        prosecutorTasksNeededToSeeVoteColors = Create(303702, Types.Crewmate, "mayorTasksNeededToSeeVoteColors", 5f, 0f, 20f, 1f, prosecutorCanSeeVoteColors);
        prosecutorDiesOnIncorrectPros = Create(303703, Types.Crewmate, "prosecutorDiesOnIncorrectPros", true, prosecutorSpawnRate);
        prosecutorCanCallEmergency = Create(303704, Types.Crewmate, "canCallEmergency", true, prosecutorSpawnRate);

        veteranSpawnRate = Create(302200, Types.Crewmate, Cs(Veteran.color, "Veteran"), rates, null, true);
        veteranCooldown = Create(302201, Types.Crewmate, "veteranCooldown", 25f, 5f, 60f, 2.5f, veteranSpawnRate);
        veteranAlertDuration = Create(302202, Types.Crewmate, "veteranAlertDuration", 12.5f, 2.5f, 20f, 0.5f, veteranSpawnRate);

        engineerSpawnRate = Create(301200, Types.Crewmate, Cs(Engineer.color, "Engineer"), rates, null, true);
        engineerRemoteFix = Create(301201, Types.Crewmate, "engineerRemoteFix", true, engineerSpawnRate);
        engineerResetFixAfterMeeting = Create(301202, Types.Crewmate, "engineerResetFixAfterMeeting", true, engineerRemoteFix);
        engineerNumberOfFixes = Create(301203, Types.Crewmate, "engineerNumberOfFixes", 1f, 1f, 3f, 1f, engineerRemoteFix);
        //engineerExpertRepairs = Create(301204, Types.Crewmate, "engineerExpertRepairs", false, engineerSpawnRate);
        engineerHighlightForImpostors = Create(301205, Types.Crewmate, "engineerHighlightForImpostors", true, engineerSpawnRate);
        engineerHighlightForTeamJackal = Create(301206, Types.Crewmate, "engineerHighlightForTeamJackal", true, engineerSpawnRate);

        swapperSpawnRate = Create(302300, Types.Crewmate, Cs(Swapper.color, "Swapper"), rates, null, true);
        swapperCanCallEmergency = Create(302301, Types.Crewmate, "canCallEmergency", true, swapperSpawnRate);
        swapperCanFixSabotages = Create(302302, Types.Crewmate, "swapperCanFixSabotages", true, swapperSpawnRate);
        swapperCanOnlySwapOthers = Create(302303, Types.Crewmate, "swapperCanOnlySwapOthers", false, swapperSpawnRate);
        swapperSwapsNumber = Create(302304, Types.Crewmate, "swapperSwapsNumber", 1f, 0f, 5f, 1f, swapperSpawnRate);
        swapperRechargeTasksNumber = Create(302305, Types.Crewmate, "swapperRechargeTasksNumber", 2f, 1f, 10f, 1f, swapperSpawnRate);

        balancerSpawnRate = Create(303300, Types.Crewmate, Cs(Balancer.color, "Balancer"), rates, null, true);
        balancerCount = Create(303301, Types.Crewmate, "balancerCount", 1, 1, 3, 1, balancerSpawnRate);
        balancerVoteTime = Create(303302, Types.Crewmate, "balancerVoteTime", 60, 15, 150, 5, balancerSpawnRate);

        medicSpawnRate = Create(302000, Types.Crewmate, Cs(Medic.color, "Medic"), rates, null, true);
        medicShowShielded = Create(302001, Types.Crewmate, "medicShowShielded",
            ["medicShowShielded1", "medicShowShielded2", "medicShowShielded3"], medicSpawnRate);
        medicBreakShield = Create(302002, Types.Crewmate, "medicBreakShield", true, medicSpawnRate);
        medicShowAttemptToMedic = Create(302003, Types.Crewmate, "medicShowAttemptToMedic", true, medicBreakShield);
        medicShowAttemptToShielded = Create(302004, Types.Crewmate, "medicShowAttemptToShielded", false, medicBreakShield);
        medicGuessShield = Create(302009, Types.Crewmate, "medicGuessShield", false, medicSpawnRate);
        medicResetTargetAfterMeeting = Create(302005, Types.Crewmate, "medicResetTargetAfterMeeting", false, medicSpawnRate);
        medicSetOrShowShieldAfterMeeting = Create(302006, Types.Crewmate, "medicSetOrShowShieldAfterMeeting",
            ["medicSetOrShowShieldAfterMeeting1", "medicSetOrShowShieldAfterMeeting2", "medicSetOrShowShieldAfterMeeting3"], medicSpawnRate);
        medicReportNameDuration = Create(302007, Types.Crewmate, "medicReportNameDuration", 5f, 0f, 60f, 2.5f, medicSpawnRate);
        medicReportColorDuration = Create(302008, Types.Crewmate, "medicReportColorDuration", 30f, 0f, 120f, 2.5f, medicSpawnRate);

        detectiveSpawnRate = Create(301900, Types.Crewmate, Cs(Detective.color, "Detective"), rates, null, true);
        detectiveAnonymousFootprints = Create(301901, Types.Crewmate, "detectiveAnonymousFootprints",
            ["optionOff", "detectiveAnonymousFootprints1", "optionOn"], detectiveSpawnRate);
        detectiveFootprintIntervall = Create(301902, Types.Crewmate, "detectiveFootprintIntervall", 0.25f, 0.25f, 10f, 0.25f, detectiveSpawnRate);
        detectiveFootprintDuration = Create(301903, Types.Crewmate, "detectiveFootprintDuration", 12.5f, 0.5f, 30f, 0.5f, detectiveSpawnRate);
        detectiveReportNameDuration = Create(301904, Types.Crewmate, "detectiveReportNameDuration", 10f, 0f, 60f, 2.5f, detectiveSpawnRate);
        detectiveReportColorDuration = Create(301905, Types.Crewmate, "detectiveReportColorDuration", 30f, 0f, 120f, 2.5f, detectiveSpawnRate);

        redemptorSpawnRate = Create(303900, Types.Crewmate, Cs(Redemptor.color, "Redemptor"), rates, null, true);
        redemptorRevelation = Create(303901, Types.Crewmate, "redemptorRevelation", false, redemptorSpawnRate);
        redemptorRevelationCooldown = Create(303902, Types.Crewmate, "redemptorRevelationCooldown", 25f, 10f, 60f, 2.5f, redemptorRevelation);
        redemptorRevelationDuration = Create(303903, Types.Crewmate, "redemptorRevelationDuration", 5f, 1f, 15f, 0.5f, redemptorRevelation);
        redemptorPrayer = Create(303904, Types.Crewmate, "redemptorPrayer", false, redemptorSpawnRate);
        redemptorPrayerCooldown = Create(303905, Types.Crewmate, "redemptorPrayerCooldown", 25f, 10f, 60f, 2.5f, redemptorPrayer);
        redemptorPrayerDuration = Create(303906, Types.Crewmate, "redemptorPrayerDuration", 5f, 2f, 15f, 0.5f, redemptorPrayer);
        redemptorReviveDuration = Create(303907, Types.Crewmate, "redemptorReviveDuration", 1.5f, 0f, 15f, 0.5f, redemptorSpawnRate);

        bodyGuardSpawnRate = Create(303400, Types.Crewmate, Cs(BodyGuard.color, "BodyGuard"), rates, null, true);
        bodyGuardResetTargetAfterMeeting = Create(303401, Types.Crewmate, "bodyGuardResetTargetAfterMeeting", true, bodyGuardSpawnRate);
        bodyGuardShowShielded = Create(303402, Types.Crewmate, "bodyGuardShowShielded", true, bodyGuardSpawnRate);
        bodyGuardFlash = Create(303403, Types.Crewmate, "bodyGuardFlash", true, bodyGuardSpawnRate);

        seerSpawnRate = Create(302400, Types.Crewmate, Cs(Seer.color, "Seer"), rates, null, true);
        seerMode = Create(302401, Types.Crewmate, "seerMode", ["seerMode1", "seerMode2", "seerMode3"], seerSpawnRate);
        seerLimitSoulDuration = Create(302402, Types.Crewmate, "seerLimitSoulDuration", false, seerSpawnRate);
        seerSoulDuration = Create(302403, Types.Crewmate, "seerSoulDuration", 30f, 0f, 120f, 2.5f, seerLimitSoulDuration);

        hackerSpawnRate = Create(302500, Types.Crewmate, Cs(Hacker.color, "Hacker"), rates, null, true);
        hackerCooldown = Create(302501, Types.Crewmate, "hackerCooldown", 15f, 5f, 60f, 2.5f, hackerSpawnRate);
        hackerHackeringDuration = Create(302502, Types.Crewmate, "hackerHackeringDuration", 10f, 2.5f, 60f, 2.5f, hackerSpawnRate);
        hackerOnlyColorType = Create(302503, Types.Crewmate, "hackerOnlyColorType", false, hackerSpawnRate);
        hackerToolsNumber = Create(302504, Types.Crewmate, "hackerToolsNumber", 5f, 1f, 30f, 1f, hackerSpawnRate);
        hackerRechargeTasksNumber = Create(302505, Types.Crewmate, "hackerRechargeTasksNumber", 2f, 1f, 5f, 1f, hackerSpawnRate);
        hackerNoMove = Create(302506, Types.Crewmate, "hackerNoMove", true, hackerSpawnRate);

        trackerSpawnRate = Create(302600, Types.Crewmate, Cs(Tracker.color, "Tracker"), rates, null, true);
        trackerUpdateIntervall = Create(302601, Types.Crewmate, "trackerUpdateIntervall", 0.5f, 0f, 30f, 0.5f, trackerSpawnRate);
        trackerResetTargetAfterMeeting = Create(302602, Types.Crewmate, "trackerResetTargetAfterMeeting ", false, trackerSpawnRate);
        trackerCanTrackCorpses = Create(302603, Types.Crewmate, "trackerCanTrackCorpses", true, trackerSpawnRate);
        trackerCorpsesTrackingCooldown = Create(302604, Types.Crewmate, "trackerCorpsesTrackingCooldown", 17.5f, 5f, 60f, 2.5f, trackerCanTrackCorpses);
        trackerCorpsesTrackingDuration = Create(302605, Types.Crewmate, "trackerCorpsesTrackingDuration", 7.5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses);

        snitchSpawnRate = Create(302700, Types.Crewmate, Cs(Snitch.color, "Snitch"), rates, null, true);
        snitchLeftTasksForReveal = Create(302701, Types.Crewmate, "snitchLeftTasksForReveal", 1, 0, 10, 1, snitchSpawnRate);
        snitchSeeMeeting = Create(302702, Types.Crewmate, "snitchSeeMeeting", true, snitchSpawnRate);
        //snitchCanSeeRoles = Create(302706, Types.Crewmate, "snitchCanSeeRoles", false, snitchSeeMeeting);
        snitchIncludeNeutralTeam = Create(302703, Types.Crewmate, "snitchIncludeNeutralTeam",
            ["optionOff", "snitchIncludeNeutralTeam2", "snitchIncludeNeutralTeam3", "snitchIncludeNeutralTeam4"], snitchSpawnRate);
        snitchTeamNeutraUseDifferentArrowColor = Create(302704, Types.Crewmate, "snitchTeamNeutraUseDifferentArrowColor", true, snitchIncludeNeutralTeam);
        snitchCanGuessIfTaksDone = Create(302705, Types.Crewmate, "snitchCanGuessIfTaksDone", false, snitchSpawnRate);

        prophetSpawnRate = Create(303600, Types.Crewmate, Cs(Prophet.color, "Prophet"), rates, null, true);
        prophetCooldown = Create(303601, Types.Crewmate, "prophetCooldown", 20f, 5f, 60f, 2.5f, prophetSpawnRate);
        prophetNumExamines = Create(303602, Types.Crewmate, "prophetNumExamines", 4, 1, 10, 1, prophetSpawnRate);
        prophetCanCallEmergency = Create(303603, Types.Crewmate, "canCallEmergency", true, prophetSpawnRate);
        prophetIsRevealed = Create(303604, Types.Crewmate, "prophetIsRevealed", false, prophetSpawnRate);
        prophetExaminesToBeRevealed = Create(303605, Types.Crewmate, "prophetExaminesToBeRevealed", 3, 1, 10, 1, prophetIsRevealed);
        prophetKillCrewAsRed = Create(303606, Types.Crewmate, "prophetKillCrewAsRed", false, prophetSpawnRate);
        prophetBenignNeutralAsRed = Create(303607, Types.Crewmate, "prophetBenignNeutralAsRed", false, prophetSpawnRate);
        prophetEvilNeutralAsRed = Create(303608, Types.Crewmate, "prophetEvilNeutralAsRed", true, prophetSpawnRate);
        prophetKillNeutralAsRed = Create(303609, Types.Crewmate, "prophetKillNeutralAsRed", true, prophetSpawnRate);

        infoSleuthSpawnRate = Create(303800, Types.Crewmate, Cs(InfoSleuth.color, "InfoSleuth"), rates, null, true);
        infoSleuthInfoType = Create(303801, Types.Crewmate, "infoSleuthInfoType",
            ["infoSleuthInfoType1", "infoSleuthInfoType2", "infoSleuthInfoType3"], infoSleuthSpawnRate);

        mediumSpawnRate = Create(303100, Types.Crewmate, Cs(Medium.color, "Medium"), rates, null, true);
        mediumCooldown = Create(303101, Types.Crewmate, "mediumCooldown", 7.5f, 2.5f, 120f, 2.5f, mediumSpawnRate);
        mediumDuration = Create(303102, Types.Crewmate, "mediumDuration", 0.5f, 0f, 15f, 0.5f, mediumSpawnRate);
        mediumOneTimeUse = Create(303103, Types.Crewmate, "mediumOneTimeUse", false, mediumSpawnRate);
        mediumChanceAdditionalInfo = Create(303104, Types.Crewmate, "mediumChanceAdditionalInfo", rates, mediumSpawnRate);

        trapperSpawnRate = Create(303500, Types.Crewmate, Cs(Trapper.color, "Trapper"), rates, null, true);
        trapperCooldown = Create(303501, Types.Crewmate, "trapperCooldown", 20f, 5f, 120f, 2.5f, trapperSpawnRate);
        trapperMaxCharges = Create(303502, Types.Crewmate, "trapperMaxCharges", 5f, 1f, 15f, 1f, trapperSpawnRate);
        trapperRechargeTasksNumber = Create(303503, Types.Crewmate, "trapperRechargeTasksNumber", 2f, 1f, 15f, 1f, trapperSpawnRate);
        trapperTrapNeededTriggerToReveal = Create(303504, Types.Crewmate, "trapperTrapNeededTriggerToReveal", 1, 1, 5, 1, trapperSpawnRate);
        trapperInfoType = Create(303505, Types.Crewmate, "trapperInfoType", ["Role", "trapperInfoType2", "Name"], trapperSpawnRate);
        trapperTrapDuration = Create(303506, Types.Crewmate, "trapperTrapDuration", 5f, 1f, 15f, 0.5f, trapperSpawnRate);

        spySpawnRate = Create(302800, Types.Crewmate, Cs(Spy.color, "Spy"), rates, null, true,
            isHidden: () => isDraftMode.GetBool());
        spyEvilCanKillSpy = Create(302805, Types.Crewmate, "spyEvilCanShootSpy", false, spySpawnRate);
        spyCanDieToSheriff = Create(302801, Types.Crewmate, "spyCanDieToSheriff", false, spySpawnRate);
        spyImpostorsCanKillAnyone = Create(302802, Types.Crewmate, "spyImpostorsCanKillAnyone", true, spySpawnRate);
        spyCanEnterVents = Create(302803, Types.Crewmate, "canUseVents", true, spySpawnRate);
        spyHasImpostorVision = Create(302804, Types.Crewmate, "hasImpVision", true, spySpawnRate);

        portalmakerSpawnRate = Create(302900, Types.Crewmate, Cs(Portalmaker.color, "Portalmaker"), rates, null, true);
        portalmakerCooldown = Create(302901, Types.Crewmate, "portalmakerCooldown", 15f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerUsePortalCooldown = Create(302902, Types.Crewmate, "portalmakerUsePortalCooldown", 15f, 10f, 60f, 2.5f, portalmakerSpawnRate);
        portalmakerLogOnlyColorType = Create(302903, Types.Crewmate, "portalmakerLogOnlyColorType", true, portalmakerSpawnRate);
        portalmakerLogHasTime = Create(302904, Types.Crewmate, "portalmakerLogHasTime", true, portalmakerSpawnRate);
        portalmakerCanPortalFromAnywhere = Create(302905, Types.Crewmate, "portalmakerCanPortalFromAnywhere", true, portalmakerSpawnRate);

        securityGuardSpawnRate = Create(303000, Types.Crewmate, Cs(SecurityGuard.color, "SecurityGuard"), rates, null, true);
        securityGuardCooldown = Create(303001, Types.Crewmate, "securityGuardCooldown", 15f, 10f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardTotalScrews = Create(303002, Types.Crewmate, "securityGuardTotalScrews", 6f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamPrice = Create(303003, Types.Crewmate, "securityGuardCamPrice", 2f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardVentPrice = Create(303004, Types.Crewmate, "securityGuardVentPrice", 1f, 1f, 15f, 1f, securityGuardSpawnRate);
        securityGuardCamDuration = Create(303005, Types.Crewmate, "securityGuardCamDuration", 10f, 2.5f, 60f, 2.5f, securityGuardSpawnRate);
        securityGuardCamMaxCharges = Create(303006, Types.Crewmate, "securityGuardCamMaxCharges", 5f, 1f, 30f, 1f, securityGuardSpawnRate);
        securityGuardCamRechargeTasksNumber = Create(303007, Types.Crewmate, "securityGuardCamRechargeTasksNumber", 3f, 1f, 10f, 1f, securityGuardSpawnRate);
        securityGuardNoMove = Create(303008, Types.Crewmate, "securityGuardNoMove", true, securityGuardSpawnRate);

        jumperSpawnRate = Create(303200, Types.Crewmate, Cs(Jumper.color, "Jumper"), rates, null, true);
        jumperJumpTime = Create(303201, Types.Crewmate, "jumperJumpTime", 10f, 0f, 60f, 2.5f, jumperSpawnRate);
        jumperMaxCharges = Create(303202, Types.Crewmate, "jumperMaxCharges", 3, 0, 10, 1, jumperSpawnRate);
        jumperResetPlaceAfterMeeting = Create(303203, Types.Crewmate, "jumperResetPlaceAfterMeeting", false, jumperSpawnRate);
        jumperChargesGainOnMeeting = Create(303204, Types.Crewmate, "jumperChargesGainOnMeeting", 2, 0, 10, 1, jumperSpawnRate);

        //-------------------------- Modifier (40000 - 49999) -------------------------- //

        modifiersAreHidden = Create(400000, Types.Modifier, Cs(Color.yellow, "modifiersAreHidden"), false, null, true);

        modifierLover = Create(401600, Types.Modifier, Cs(Lovers.color, "Lover"), rates, null, true);
        modifierLoverImpLoverRate = Create(401601, Types.Modifier, "modifierLoverImpLoverRate", rates, modifierLover);
        modifierLoverNeutraValid = Create(401602, Types.Modifier, "modifierLoverNeutraValid", true, modifierLoverImpLoverRate);
        modifierLoverCanGetModifiers = Create(401603, Types.Modifier, "modifierLoverCanGetModifiers", false, modifierLover);
        modifierLoverEnableChat = Create(401604, Types.Modifier, "modifierLoverEnableChat", true, modifierLover);
        modifierLoverAvengerChance = Create(401605, Types.Modifier, "modifierLoverAvengerChance", rates, modifierLover);

        avengerIsGuessable = Create(401651, Types.Modifier, "avengerIsGuessable", false, modifierLoverAvengerChance);
        avengerKillCooldown = Create(401652, Types.Modifier, "killCooldown", 25f, 10f, 60f, 2.5f, modifierLoverAvengerChance);
        avengerTargetKnowPlayer = Create(401662, Types.Modifier, "avengerTargetKnowPlayer", true, modifierLoverAvengerChance);
        avengerKnowTarget = Create(401663, Types.Modifier, "avengerKnowTarget", true, modifierLoverAvengerChance);
        avengerShowArrows = Create(401654, Types.Modifier, "avengerShowArrows", true, avengerKnowTarget);
        avengerUpdateIntervall = Create(401655, Types.Modifier, "avengerUpdateIntervall", 5f, 0.5f, 15f, 0.5f, avengerShowArrows);
        avengerCanFreeKill = Create(401653, Types.Modifier, "avengerCanFreeKill",
            ["avengerCanFreeKill.1", "avengerCanFreeKill.2"], avengerKnowTarget);
        avengerHasImpVision = Create(401656, Types.Modifier, "hasImpVision", true, modifierLoverAvengerChance);
        avengerCanUseVents = Create(401657, Types.Modifier, "canUseVents", false, modifierLoverAvengerChance);
        avengerOnlyAliveWin = Create(401661, Types.Modifier, "avengerOnlyAliveWin", true, modifierLoverAvengerChance);
        avengerWinCondition = Create(401658, Types.Modifier, "avengerWinCondition",
            ["avengerWinCondition.1", "avengerWinCondition.2", "avengerWinCondition.3"], modifierLoverAvengerChance);
        avengerTargetWasKilledByOther = Create(401659, Types.Modifier, "avengerTargetWasKilledByOther",
            ["DeathReason.Suicide", GetString("", Cs(Jester.color, "Jester")), GetString("", Cs(Amnisiac.color, "Amnisiac")), GetString("", Cs(Survivor.color, "Survivor"))], modifierLoverAvengerChance);
        avengerTargetWasExiled = Create(401660, Types.Modifier, "avengerTargetWasExiled",
            ["DeathReason.Suicide", GetString("", Cs(Jester.color, "Jester")), GetString("", Cs(Amnisiac.color, "Amnisiac")), GetString("", Cs(Survivor.color, "Survivor"))], modifierLoverAvengerChance);

        modifierAssassin = Create(100000, Types.Modifier, Cs(Assassin.color, "modifierAssassin"), rates, null, true,
            isHidden: () => GuesserGM.Enabled);
        modifierAssassinQuantity = Create(100001, Types.Modifier, "modifierAssassinQuantity", ratesCount, modifierAssassin,
            isHidden: () => GuesserGM.Enabled);
        modifierAssassinNumberOfShots = Create(100002, Types.Modifier, "modifierAssassinNumberOfShots", 3f, 1f, 15f, 1f, modifierAssassin,
            isHidden: () => GuesserGM.Enabled);
        modifierAssassinMultipleShotsPerMeeting = Create(100003, Types.Modifier, "modifierAssassinMultipleShotsPerMeeting", true, modifierAssassin,
            isHidden: () => GuesserGM.Enabled);
        guesserEvilCanKillCrewmate = Create(100005, Types.Modifier, "guesserEvilCanKillCrewmate", true, modifierAssassin,
            isHidden: () => GuesserGM.Enabled);
        modifierAssassinKillsThroughShield = Create(100007, Types.Modifier, "modifierAssassinKillsThroughShield", false, modifierAssassin,
            isHidden: () => GuesserGM.Enabled);

        modifierDisperser = Create(401000, Types.Modifier, Cs(Palette.ImpostorRed, "Disperser"), rates, null, true);
        modifierDisperserDispersesToVent = Create(401001, Types.Modifier, "modifierDisperserDispersesToVent", true, modifierDisperser);

        modifierPoucher = Create(403700, Types.Modifier, Cs(Palette.ImpostorRed, "Poucher"), rates, null, true, null,
            onChange: (x) => { if (poucherSpawnRate.Selection > 0) poucherSpawnRate.updateSelection(0); });

        modifierProfessional = Create(403900, Types.Modifier, Cs(Palette.ImpostorRed, "Professional"), rates, null, true, null,
            onChange: (x) => { if (professionalSpawnRate.Selection > 0) professionalSpawnRate.updateSelection(0); });
        modifierProfessionalWhoCanSeeBodies = Create(403901, Types.Modifier, "professionalWhoCanSeeBodies",
            ["professionalWhoCanSeeBodies.1", "professionalWhoCanSeeBodies.2", "professionalWhoCanSeeBodies.3"], modifierProfessional,
            onChange: (x) => { if (professionalWhoCanSeeBodies.Selection != x.Selection) professionalWhoCanSeeBodies.updateSelection(x.Selection); });

        modifierSpecoality = Create(403500, Types.Modifier, Cs(Palette.ImpostorRed, "Specoality"), rates, null, true);
        modifierSpecoalityIsGlobal = Create(403501, Types.Modifier, "modifierSpecoalityIsGlobal", false, modifierSpecoality);

        modifierVortox = Create(403800, Types.Modifier, Cs(Vortox.color, "Vortox"), rates, null, true);
        modifierVortoxReversal = Create(403801, Types.Modifier, "modifierVortoxReversal", true, modifierVortox);
        modifierVortoxSkipMeeting = Create(403802, Types.Modifier, "modifierVortoxSkipMeeting", true, modifierVortox);
        modifierVortoxSkipNum = Create(403803, Types.Modifier, "modifierVortoxSkipNum", 4, 1, 10, 1, modifierVortoxSkipMeeting);

        modifierLastImpostor = Create(401100, Types.Modifier, Cs(Palette.ImpostorRed, "LastImpostor"), false, null, true);
        modifierLastImpostorDeduce = Create(401101, Types.Modifier, "modifierLastImpostorDeduce", 5f, 2.5f, 15f, 2.5f, modifierLastImpostor);

        modifierBloody = Create(401200, Types.Modifier, Cs(Color.yellow, "Bloody"), rates, null, true);
        modifierBloodyQuantity = Create(401201, Types.Modifier, Cs(Color.yellow, "modifierBloodyQuantity"), ratesCount, modifierBloody);
        modifierBloodyDuration = Create(401202, Types.Modifier, "modifierBloodyDuration", 10f, 3f, 60f, 0.5f, modifierBloody);

        modifierAntiTeleport = Create(401300, Types.Modifier, Cs(Color.yellow, "AntiTeleport"), rates, null, true);
        modifierAntiTeleportQuantity = Create(401301, Types.Modifier, Cs(Color.yellow, "modifierAntiTeleportQuantity"), ratesCount, modifierAntiTeleport);

        modifierTieBreaker = Create(401400, Types.Modifier, Cs(Color.yellow, "TieBreaker"), rates, null, true);

        modifierBait = Create(401500, Types.Modifier, Cs(Color.yellow, "Bait"), rates, null, true);
        modifierBaitSwapCrewmate = Create(401501, Types.Modifier, "modifierBaitSwapCrewmate", false, modifierBait);
        modifierBaitReportDelayMin = Create(401502, Types.Modifier, "modifierBaitReportDelayMin", 0f, 0f, 10f, 0.125f, modifierBait);
        modifierBaitReportDelayMax = Create(401503, Types.Modifier, "modifierBaitReportDelayMax", 0.5f, 0f, 10f, 0.5f, modifierBait);
        modifierBaitShowKillFlash = Create(401504, Types.Modifier, "modifierBaitShowKillFlash", true, modifierBait);

        modifierAftermath = Create(403600, Types.Modifier, Cs(Color.yellow, "Aftermath"), rates, null, true);

        modifierSunglasses = Create(401700, Types.Modifier, Cs(Color.yellow, "Sunglasses"), rates, null, true);
        modifierSunglassesQuantity = Create(401701, Types.Modifier, Cs(Color.yellow, "modifierSunglassesQuantity"), ratesCount, modifierSunglasses);
        modifierSunglassesVision = Create(401702, Types.Modifier, "modifierSunglassesVision", ["-10%", "-20%", "-30%", "-40%", "-50%"], modifierSunglasses);

        modifierTorch = Create(401800, Types.Modifier, Cs(Color.yellow, "Torch"), rates, null, true);
        modifierTorchQuantity = Create(401801, Types.Modifier, Cs(Color.yellow, "modifierTorchQuantity"), ratesCount, modifierTorch);
        modifierTorchVision = Create(401802, Types.Modifier, "modifierTorchVision", 1.5f, 1f, 3f, 0.125f, modifierTorch);

        modifierFlash = Create(401900, Types.Modifier, Cs(Color.yellow, "Flash"), rates, null, true);
        modifierFlashQuantity = Create(401901, Types.Modifier, Cs(Color.yellow, "modifierFlashQuantity"), ratesCount, modifierFlash);
        modifierFlashSpeed = Create(401902, Types.Modifier, "modifierFlashSpeed", 1.25f, 1f, 3f, 0.125f, modifierFlash);

        modifierMultitasker = Create(402000, Types.Modifier, Cs(Color.yellow, "Multitasker"), rates, null, true);
        modifierMultitaskerQuantity = Create(402001, Types.Modifier, Cs(Color.yellow, "modifierMultitaskerQuantity"), ratesCount, modifierMultitasker);

        modifierMini = Create(402100, Types.Modifier, Cs(Color.yellow, "Mini"), rates, null, true);
        modifierMiniGrowingUpDuration = Create(402101, Types.Modifier, "modifierMiniGrowingUpDuration", 400f, 100f, 1500f, 25f, modifierMini);
        modifierMiniGrowingUpInMeeting = Create(402102, Types.Modifier, "modifierMiniGrowingUpInMeeting", true, modifierMini);

        modifierGiant = Create(402200, Types.Modifier, Cs(Color.yellow, "Giant"), rates, null, true);
        modifierGiantSpped = Create(402201, Types.Modifier, "modifierGiantSpped", 0.75f, 0.5f, 1.5f, 0.05f, modifierGiant);

        modifierIndomitable = Create(402300, Types.Modifier, Cs(Color.yellow, "Indomitable"), rates, null, true);

        modifierBlind = Create(402400, Types.Modifier, Cs(Color.yellow, "Blind"), rates, null, true);

        modifierWatcher = Create(402500, Types.Modifier, Cs(Color.yellow, "Watcher"), rates, null, true);

        modifierRadar = Create(402600, Types.Modifier, Cs(Color.yellow, "Radar"), rates, null, true);

        modifierTunneler = Create(402700, Types.Modifier, Cs(Color.yellow, "Tunneler"), rates, null, true);
        modifierTunnelerNoTask = Create(402701, Types.Modifier, "modifierTunnelerNoTask", false, modifierTunneler);

        modifierButtonBarry = Create(402800, Types.Modifier, Cs(Color.yellow, "ButtonBarry"), rates, null, true);
        modifierButtonSabotageRemoteMeetings = Create(402801, Types.Modifier, "modifierButtonSabotageRemoteMeetings", true, modifierButtonBarry);

        modifierSlueth = Create(402900, Types.Modifier, Cs(Color.yellow, "Slueth"), rates, null, true);

        modifierCursed = Create(403000, Types.Modifier, Cs(Color.yellow, "Cursed"), rates, null, true);
        modifierHideCursed = Create(403001, Types.Modifier, "modifierShowCursed", false, modifierCursed);

        modifierVip = Create(403100, Types.Modifier, Cs(Color.yellow, "Vip"), rates, null, true);
        modifierVipQuantity = Create(403101, Types.Modifier, Cs(Color.yellow, "modifierVipQuantity"), ratesCount, modifierVip);
        modifierVipShowColor = Create(403102, Types.Modifier, "modifierVipShowColor", true, modifierVip);

        modifierChameleon = Create(403300, Types.Modifier, Cs(Color.yellow, "Chameleon"), rates, null, true);
        modifierChameleonQuantity = Create(403301, Types.Modifier, Cs(Color.yellow, "modifierChameleonQuantity"), ratesCount, modifierChameleon);
        modifierChameleonHoldDuration = Create(403302, Types.Modifier, "modifierChameleonHoldDuration", 3f, 1f, 10f, 0.5f, modifierChameleon);
        modifierChameleonFadeDuration = Create(403303, Types.Modifier, "modifierChameleonFadeDuration", 1f, 0.25f, 10f, 0.25f, modifierChameleon);
        modifierChameleonMinVisibility = Create(403304, Types.Modifier, "modifierChameleonMinVisibility", ["0%", "10%", "20%", "30%", "40%", "50%"], modifierChameleon);

        modifierShifter = Create(403400, Types.Modifier, Cs(Color.yellow, "Shifter"), rates, null, true);
        modifierShiftNeutral = Create(403401, Types.Modifier, "modifierShiftNeutral", false, modifierShifter);
        modifierShiftALLNeutral = Create(403402, Types.Modifier, "modifierShiftALLNeutral", false, modifierShiftNeutral);
        modifierShiftReload = Create(403403, Types.Modifier, "modifierShiftReload", true, modifierShifter);

        //-------------------------- Ghost Role 50000 - 59999 -------------------------- //

        clogSpawnRate = Create(500400, Types.GhostRole, Cs(Clog.color, "ClogOptions"), rates, null, true);
        clogGhostCooldown = Create(500401, Types.GhostRole, "clogGhostCooldown", 15f, 5f, 120f, 2.5f, clogSpawnRate);
        clogGhostDuration = Create(500402, Types.GhostRole, "clogGhostDuration", 10f, 2f, 18f, 0.5f, clogSpawnRate);
        clogGhostRange = Create(500403, Types.GhostRole, "clogGhostRange", 0.5f, 0.125f, 2.5f, 0.125f, clogSpawnRate);
        clogUseNum = Create(500404, Types.GhostRole, "clogUseNum", 3, 1, 20, 1, clogSpawnRate);
        clogOnlyUsedOnce = Create(500405, Types.GhostRole, "clogOnlyUsedOnce", false, clogSpawnRate);

        specterSpawnRate = Create(500200, Types.GhostRole, Cs(Specter.color, "SpecterOption"), rates, null, true);
        specterDuration = Create(500201, Types.GhostRole, "specterDuration", 1.5f, 0.25f, 5f, 0.25f, specterSpawnRate);
        specterResetRole = Create(500202, Types.GhostRole, "amnisiacResetRole", true, specterSpawnRate);
        specterAfterMeetingTakeRole = Create(500203, Types.GhostRole, "specterAfterMeetingTakeRole", false, specterSpawnRate);
        specterAfterMeetingRevived = Create(500204, Types.GhostRole, "specterAfterMeetingRevived", false, specterSpawnRate);

        ghostEngineerSpawnRate = Create(500100, Types.GhostRole, Cs(GhostEngineer.color, "GhostEngineerOptions"), rates, null, true);

        poltergeistSpawnRate = Create(500300, Types.GhostRole, Cs(Poltergeist.color, "PoltergeistOptions"), rates, null, true);
        poltergeistCooldown = Create(500301, Types.GhostRole, "poltergeistCooldown", 5f, 2.5f, 60f, 2.5f, poltergeistSpawnRate);
        poltergeistRadius = Create(500302, Types.GhostRole, "poltergeistRadius", 0.75f, 0.5f, 2f, 0.125f, poltergeistSpawnRate);

        //-------------------------- Guesser Gamemode 2000 - 2999 -------------------------- //

        //GuesserGM
        GuesserGM.AddOptions();

        //-------------------------- Guesser Gamemode 2000 - 2999 -------------------------- //
    }
}
