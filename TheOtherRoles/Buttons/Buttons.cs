using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor.Networking;
using Reactor.Networking.Extensions;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.Buttons.CustomButton;
using static TheOtherRoles.Modules.ModInputManager;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Buttons;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
internal static class HudManagerStartPatch
{
    private static bool initialized;

    private static readonly float multiplier = Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini ? Mini.isGrownUp() ? 0.66f : 2f : 1f;
    public static CustomButton zoomOutButton;
    public static CustomButton roleSummaryButton;
    public static CustomButton gameModeButton;
    public static CustomButton ghostEngineerButton;
    public static CustomButton engineerRepairButton;
    public static CustomButton sheriffKillButton;
    private static CustomButton deputyHandcuffButton;
    public static CustomButton timeMasterShieldButton;
    private static CustomButton amnisiacRememberButton;
    private static CustomButton specterRememberButton;
    public static CustomButton veteranAlertButton;
    public static CustomButton medicShieldButton;
    private static CustomButton shifterShiftButton;
    public static CustomButton bomberBombButton;
    public static CustomButton bomberGiveButton;
    private static CustomButton disperserDisperseButton;
    private static CustomButton buttonBarryButton;
    public static CustomButton morphlingButton;
    public static CustomButton butcherDissectionButton;
    public static CustomButton camouflagerButton;
    public static CustomButton portalmakerPlacePortalButton;
    private static CustomButton usePortalButton;
    private static CustomButton portalmakerMoveToPortalButton;
    public static CustomButton hackerButton;
    public static CustomButton hackerVitalsButton;
    public static CustomButton hackerAdminTableButton;
    public static CustomButton trackerTrackPlayerButton;
    public static CustomButton bodyGuardGuardButton;
    private static CustomButton trackerTrackCorpsesButton;
    public static CustomButton vampireKillButton;
    public static CustomButton garlicButton;
    public static CustomButton jackalKillButton;
    public static CustomButton jackalSwoopButton;
    public static CustomButton swooperSwoopButton;
    public static CustomButton swooperKillButton;
    private static CustomButton jackalCreateSidekickButton;
    public static CustomButton eraserButton;
    public static CustomButton pavlovsdogsKillButton;
    public static CustomButton pavlovsownerCreateDogButton;
    public static CustomButton partTimerButton;
    public static CustomButton placeJackInTheBoxButton;
    public static CustomButton lightsOutButton;
    public static CustomButton cleanerCleanButton;
    public static CustomButton undertakerDragButton;
    public static CustomButton warlockCurseButton;
    public static CustomButton securityGuardButton;
    public static CustomButton securityGuardCamButton;
    public static CustomButton arsonistButton;
    public static CustomButton arsonistKillButton;
    public static CustomButton vultureEatButton;
    public static CustomButton mediumButton;
    public static CustomButton pursuerButton;
    public static CustomButton witchSpellButton;
    public static CustomButton jumperMarkButton;
    public static CustomButton jumperJumpButton;
    public static CustomButton escapistMarkButton;
    public static CustomButton escapistEscapeButton;
    public static CustomButton ninjaButton;
    public static CustomButton werewolfRampageButton;
    public static CustomButton werewolfKillButton;
    public static CustomButton minerMineButton;
    public static CustomButton grenadierFlashButton;
    public static CustomButton mayorMeetingButton;
    public static CustomButton blackmailerButton;
    public static CustomButton thiefKillButton;
    public static CustomButton juggernautKillButton;
    public static CustomButton pelicanKillButton;
    public static CustomButton evilTrapperSetTrapButton;
    public static CustomButton doomsayerButton;
    public static CustomButton akujoHonmeiButton;
    public static CustomButton akujoBackupButton;
    public static CustomButton survivorVestButton;
    public static CustomButton survivorBlanksButton;
    public static CustomButton yoyoButton;
    public static CustomButton yoyoAdminTableButton;
    public static CustomButton trapperButton;
    public static CustomButton prophetButton;
    public static CustomButton terroristButton;
    public static CustomButton defuseButton;
    public static CustomButton redemptorReviveButton;
    public static CustomButton redemptorRevelationButton;
    public static CustomButton redemptorPrayerButton;
    public static CustomButton bandLeaderKeyboardistButton;
    public static CustomButton bandLeaderBassistButton;
    public static CustomButton bandLeaderDrummerButton;
    public static CustomButton bandLeaderKillButton;
    public static CustomButton schrodingersCatKillButton;
    public static CustomButton gunsmithGetBullets;
    public static CustomButton gunsmithAddBullets;
    public static CustomButton berserkerKillButton;
    public static CustomButton poltergeistButton;

    public static Dictionary<byte, List<CustomButton>> deputyHandcuffedButtons;
    public static PoolablePlayer targetDisplay;

    public static TMP_Text securityGuardButtonScrewsText;
    public static TMP_Text securityGuardChargesText;
    public static TMP_Text deputyButtonHandcuffsText;
    public static TMP_Text pursuerButtonBlanksText;
    public static TMP_Text survivorVestButtonText;
    public static TMP_Text survivorBlanksButtonText;
    public static TMP_Text hackerAdminTableChargesText;
    public static TMP_Text hackerVitalsChargesText;
    public static TMP_Text trapperChargesText;
    public static TMP_Text prophetButtonText;
    public static TMP_Text portalmakerButtonText1;
    public static TMP_Text portalmakerButtonText2;
    public static TMP_Text PavlovsdogKillSelfText;
    public static TMP_Text PavlovsdogCreateNumText;
    public static TMP_Text akujoTimeRemainingText;
    public static TMP_Text akujoBackupLeftText;
    public static TMP_Text bandLeaderKeyboardistText;
    public static TMP_Text bandLeaderBassistText;
    public static TMP_Text bandLeaderDrummerText;
    public static TMP_Text gunsmithGetBulletsText;
    public static TMP_Text berserkerKillButtonText;

    public static void setCustomButtonCooldowns()
    {
        if (!initialized)
            try
            {
                createButtonsPostfix(HudManager.Instance);
            }
            catch
            {
                Warn("Button cooldowns not set, either the gamemode does not require them or there's something wrong.");
                return;
            }
        yoyoButton.MaxTimer = Yoyo.markCooldown;
        yoyoAdminTableButton.MaxTimer = Yoyo.adminCooldown;
        yoyoAdminTableButton.EffectDuration = 10f;
        engineerRepairButton.MaxTimer = 0f;
        ghostEngineerButton.Timer = ghostEngineerButton.MaxTimer = 0f;
        specterRememberButton.MaxTimer = 15f;
        sheriffKillButton.MaxTimer = Sheriff.cooldown;
        deputyHandcuffButton.MaxTimer = Sheriff.handcuffCooldown;
        timeMasterShieldButton.MaxTimer = TimeMaster.cooldown;
        veteranAlertButton.MaxTimer = Veteran.cooldown;
        survivorVestButton.MaxTimer = Survivor.vestCooldown;
        survivorBlanksButton.MaxTimer = Survivor.blanksCooldown;
        medicShieldButton.MaxTimer = 0f;
        shifterShiftButton.MaxTimer = 0f;
        disperserDisperseButton.MaxTimer = 0f;
        buttonBarryButton.MaxTimer = 0f;
        morphlingButton.MaxTimer = Morphling.cooldown;
        butcherDissectionButton.MaxTimer = Butcher.dissectionCooldown;
        bomberBombButton.MaxTimer = Bomber.cooldown;
        camouflagerButton.MaxTimer = Camouflager.cooldown;
        portalmakerPlacePortalButton.MaxTimer = Portalmaker.cooldown;
        usePortalButton.MaxTimer = Portalmaker.usePortalCooldown;
        portalmakerMoveToPortalButton.MaxTimer = Portalmaker.usePortalCooldown;
        hackerButton.MaxTimer = Hacker.cooldown;
        hackerVitalsButton.MaxTimer = Hacker.cooldown;
        hackerAdminTableButton.MaxTimer = Hacker.cooldown;
        vampireKillButton.MaxTimer = Vampire.cooldown;
        trackerTrackPlayerButton.MaxTimer = 0f;
        jumperMarkButton.MaxTimer = Jumper.JumpTime;
        jumperJumpButton.MaxTimer = Jumper.JumpTime;
        escapistMarkButton.MaxTimer = Escapist.EscapeTime;
        escapistEscapeButton.MaxTimer = Escapist.EscapeTime;
        bodyGuardGuardButton.MaxTimer = 0f;
        garlicButton.MaxTimer = 0f;
        jackalKillButton.MaxTimer = Jackal.cooldown;
        werewolfKillButton.MaxTimer = Werewolf.killCooldown;
        jackalCreateSidekickButton.MaxTimer = Jackal.createSidekickCooldown;
        eraserButton.MaxTimer = Eraser.cooldown;
        placeJackInTheBoxButton.MaxTimer = Trickster.placeBoxCooldown;
        lightsOutButton.MaxTimer = Trickster.lightsOutCooldown;
        cleanerCleanButton.MaxTimer = Cleaner.cooldown;
        undertakerDragButton.MaxTimer = 0f;
        warlockCurseButton.MaxTimer = Warlock.cooldown;
        securityGuardButton.MaxTimer = SecurityGuard.cooldown;
        securityGuardCamButton.MaxTimer = SecurityGuard.cooldown;
        arsonistButton.MaxTimer = arsonistKillButton.MaxTimer = Arsonist.cooldown;
        vultureEatButton.MaxTimer = Vulture.cooldown;
        amnisiacRememberButton.MaxTimer = 0f;
        grenadierFlashButton.MaxTimer = Grenadier.cooldown;
        bomberGiveButton.MaxTimer = bomberGiveButton.Timer = 0f;
        partTimerButton.MaxTimer = PartTimer.cooldown;
        mediumButton.MaxTimer = Medium.cooldown;
        pursuerButton.MaxTimer = Pursuer.cooldown;
        trackerTrackCorpsesButton.MaxTimer = Tracker.corpsesTrackingCooldown;
        prophetButton.MaxTimer = Prophet.cooldown;
        witchSpellButton.MaxTimer = Witch.cooldown;
        ninjaButton.MaxTimer = Ninja.cooldown;
        swooperSwoopButton.MaxTimer = Swooper.swoopCooldown;
        swooperSwoopButton.MaxTimer = Swooper.swoopCooldown;
        swooperSwoopButton.EffectDuration = Swooper.duration;
        jackalSwoopButton.MaxTimer = Jackal.swoopCooldown;
        jackalSwoopButton.MaxTimer = Jackal.swoopCooldown;
        jackalSwoopButton.EffectDuration = Jackal.duration;
        minerMineButton.MaxTimer = Miner.cooldown;
        blackmailerButton.MaxTimer = Blackmailer.cooldown;
        pelicanKillButton.MaxTimer = Pelican.cooldown;
        thiefKillButton.MaxTimer = Thief.cooldown;
        juggernautKillButton.MaxTimer = Juggernaut.cooldown;
        swooperKillButton.MaxTimer = Swooper.cooldown;
        evilTrapperSetTrapButton.MaxTimer = EvilTrapper.cooldown;
        redemptorReviveButton.MaxTimer = 10f;
        redemptorRevelationButton.MaxTimer = Redemptor.revelationCooldown;
        redemptorPrayerButton.MaxTimer = Redemptor.prayerCooldown;
        doomsayerButton.MaxTimer = Doomsayer.cooldown;
        akujoHonmeiButton.MaxTimer = 0f;
        akujoBackupButton.MaxTimer = 0f;
        pavlovsdogsKillButton.MaxTimer = Pavlovsdogs.cooldown;
        pavlovsownerCreateDogButton.MaxTimer = Pavlovsdogs.createDogCooldown;
        mayorMeetingButton.MaxTimer = 0f;
        trapperButton.MaxTimer = Trapper.cooldown;
        terroristButton.MaxTimer = Terrorist.bombCooldown;
        defuseButton.MaxTimer = defuseButton.Timer = 0f;
        bandLeaderKeyboardistButton.MaxTimer = 0f;
        bandLeaderBassistButton.MaxTimer = 0f;
        bandLeaderDrummerButton.MaxTimer = 0f;
        bandLeaderKillButton.MaxTimer = BandLeader.killCooldown;
        schrodingersCatKillButton.MaxTimer = SchrodingersCat.Cooldown;
        gunsmithAddBullets.MaxTimer = 0f;
        gunsmithGetBullets.MaxTimer = 0f;
        berserkerKillButton.MaxTimer = Berserker.KillCooldown;
        poltergeistButton.MaxTimer = Poltergeist.cooldown;

        butcherDissectionButton.EffectDuration = Butcher.dissectionDuration;
        timeMasterShieldButton.EffectDuration = TimeMaster.shieldDuration;
        veteranAlertButton.EffectDuration = Veteran.alertDuration;
        survivorVestButton.EffectDuration = Survivor.vestDuration;
        hackerButton.EffectDuration = Hacker.duration;
        hackerVitalsButton.EffectDuration = Hacker.duration;
        hackerAdminTableButton.EffectDuration = Hacker.duration;
        vampireKillButton.EffectDuration = Vampire.delay;
        werewolfRampageButton.MaxTimer = Werewolf.rampageCooldown;
        werewolfRampageButton.EffectDuration = Werewolf.rampageDuration;
        grenadierFlashButton.EffectDuration = Grenadier.duration;
        camouflagerButton.EffectDuration = Camouflager.duration;
        morphlingButton.EffectDuration = Morphling.duration;
        bomberBombButton.EffectDuration = Bomber.bombDelay + Bomber.bombTimer;
        lightsOutButton.EffectDuration = Trickster.lightsOutDuration;
        arsonistButton.EffectDuration = Arsonist.duration;
        mediumButton.EffectDuration = Medium.duration;
        trackerTrackCorpsesButton.EffectDuration = Tracker.corpsesTrackingDuration;
        witchSpellButton.EffectDuration = Witch.spellCastingDuration;
        securityGuardCamButton.EffectDuration = SecurityGuard.duration;
        defuseButton.EffectDuration = Terrorist.defuseDuration;
        terroristButton.EffectDuration = Terrorist.destructionTime + Terrorist.bombActiveAfter;
        redemptorRevelationButton.EffectDuration = Redemptor.revelationDuration;
        redemptorPrayerButton.EffectDuration = Redemptor.prayerDuration;
        berserkerKillButton.EffectDuration = 0.5f;

        zoomOutButton.MaxTimer = zoomOutButton.Timer = 0f;
    }

    public static PlayerControl SetTarget(IEnumerable<PlayerControl> untarget = null, bool onlyCrewmates = false,
        bool targetInVents = false, float distances = 0f, PlayerControl targetingPlayer = null)
    {
        return PlayerControlFixedUpdatePatch.SetTarget(onlyCrewmates, targetInVents, untarget, KillDistances: distances, targetingPlayer: targetingPlayer);
    }

    public static void SetPlayerOutline(PlayerControl target, Color color)
    {
        PlayerControlFixedUpdatePatch.SetPlayerOutline(target, color);
    }

    /// <summary>
    /// 化形按钮显示目标模型
    /// </summary>
    private static void setButtonTargetDisplay(PlayerControl target, CustomButton button = null, Vector3? offset = null)
    {
        if (target == null || button == null)
        {
            if (targetDisplay != null)
            {
                // Reset the poolable player
                targetDisplay.gameObject.SetActive(false);
                Object.Destroy(targetDisplay.gameObject);
                targetDisplay = null;
            }

            return;
        }

        // Add poolable player to the button so that the target outfit is shown
        button.actionButton.cooldownTimerText.transform.localPosition =
            new Vector3(0, 0, -1f); // Before the poolable player
        targetDisplay = Object.Instantiate(IntroCutsceneOnDestroyPatch.playerPrefab, button.actionButton.transform);
        var data = target.Data;
        target.SetPlayerMaterialColors(targetDisplay.cosmetics.currentBodySprite.BodySprite);
        targetDisplay.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
        targetDisplay.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
        targetDisplay.cosmetics.nameText.text = ""; // Hide the name!
        targetDisplay.transform.localPosition = new Vector3(0f, 0.22f, -0.01f);
        if (offset != null) targetDisplay.transform.localPosition += (Vector3)offset;
        targetDisplay.transform.localScale = Vector3.one * 0.33f;
        targetDisplay.setSemiTransparent(false);
        targetDisplay.gameObject.SetActive(true);
    }

    public static void Postfix(HudManager __instance)
    {
        initialized = false;

        try
        {
            createButtonsPostfix(__instance);
        }
        catch { }
    }

    public static void createButtonsPostfix(HudManager __instance)
    {
        // get map id, or raise error to wait...
        var mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;

        roleSummaryButton = new CustomButton(
            () =>
            {
                if (LobbyRoleInfo.RolesSummaryUI == null) LobbyRoleInfo.RoleSummaryOnClick();
                else
                {
                    Object.Destroy(LobbyRoleInfo.RolesSummaryUI);
                    LobbyRoleInfo.RolesSummaryUI = null;
                }
            },
            () => { return PlayerControl.LocalPlayer != null && LobbyBehaviour.Instance; },
            () =>
            {
                if (PlayerCustomizationMenu.Instance || GameSettingMenu.Instance)
                {
                    if (LobbyRoleInfo.RolesSummaryUI != null)
                        Object.Destroy(LobbyRoleInfo.RolesSummaryUI);
                }
                return true;
            },
            () => { },
            new ResourceSprite("TheOtherRoles.Resources.HelpButton.png", 85f),
            new Vector3(0.4f, 3f, 0),
            __instance,
            null
        );

        gameModeButton = new CustomButton(
            () =>
            {
                ModOption.gameMode = (CustomGamemodes)((int)(ModOption.gameMode + 1) % Enum.GetNames(typeof(CustomGamemodes)).Length);
                MessageWriter writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ShareGameMode);
                writer.Write((byte)ModOption.gameMode);
                writer.EndRPC();
                RPCProcedure.shareGameMode((byte)ModOption.gameMode);
            },
            () => { return PlayerControl.LocalPlayer && AmongUsClient.Instance?.AmHost == true && LobbyBehaviour.Instance; },
            () => { return true; },
            () => { },
            new ResourceSprite("Swap.png", 135f),
            ButtonPositions.upperRowRight,
            __instance,
            null,
            buttonText: "更换模式"
        )
        { Timer = 0f };

        zoomOutButton = new CustomButton(
            () => { toggleZoom(); },
            () =>
            {
                if (!CanSeeRoleInfo) return false;
                if (PlayerControl.LocalPlayer.IsAlive()) return false;
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data);
                var numberOfLeftTasks = playerTotal - playerCompleted;
                return numberOfLeftTasks <= 0 || !CustomOptionHolder.finishTasksBeforeHauntingOrZoomingOut.GetBool();
            },
            () => { return true; },
            () => { },
            UnityHelper.loadSpriteFromResources("TheOtherRoles.Resources.ZoomOut.png", 85f), // Invisible button!
            new Vector3(0.4f, 2.35f, 0f),
            __instance,
            KeyCode.KeypadPlus
        )
        { Timer = 0f };

        // Engineer Repair
        engineerRepairButton = new CustomButton(
            () =>
            {
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    if (task.TaskType == TaskTypes.FixLights)
                    {
                        var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.FixLights);
                        writer.EndRPC();
                        RPCProcedure.FixLights();
                    }
                    else if (task.TaskType == TaskTypes.RestoreOxy)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
                    }
                    else if (task.TaskType == TaskTypes.ResetReactor)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 16);
                    }
                    else if (task.TaskType == TaskTypes.ResetSeismic)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Laboratory, 16);
                    }
                    else if (task.TaskType == TaskTypes.FixComms)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                    }
                    else if (task.TaskType == TaskTypes.StopCharles)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 0 | 16);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 1 | 16);
                    }
                    else if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                    {
                        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.FixSubmergedOxygen);
                        writer.EndRPC();
                        RPCProcedure.FixSubmergedOxygen();
                    }
                SoundEffectsManager.play("engineerRepair");
                Engineer.remainingFixes--;
                engineerRepairButton.Timer = 0f;
            },
            () =>
            {
                return Engineer.engineer != null && Engineer.engineer == PlayerControl.LocalPlayer &&
                       Engineer.remainingFixes > 0 && Engineer.remoteFix && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                return isSabotageActive() && Engineer.remainingFixes > 0 && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                if (Engineer.resetFixAfterMeeting) Engineer.resetFixes();
            },
            Engineer.buttonSprite,
            ButtonPositions.upperRowRight,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("RepairText")
        );

        ghostEngineerButton = new CustomButton(
            () =>
            {
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    if (task.TaskType == TaskTypes.FixLights)
                    {
                        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.FixLights);
                        writer.EndRPC();
                        RPCProcedure.FixLights();
                    }
                    else if (task.TaskType == TaskTypes.RestoreOxy)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
                    }
                    else if (task.TaskType == TaskTypes.ResetReactor)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 16);
                    }
                    else if (task.TaskType == TaskTypes.ResetSeismic)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Laboratory, 16);
                    }
                    else if (task.TaskType == TaskTypes.FixComms)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                    }
                    else if (task.TaskType == TaskTypes.StopCharles)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 0 | 16);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 1 | 16);
                    }
                    else if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                    {
                        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.FixSubmergedOxygen);
                        writer.EndRPC();
                        RPCProcedure.FixSubmergedOxygen();
                    }
                GhostEngineer.Fixes = true;
                ghostEngineerButton.Timer = 0f;
                SoundEffectsManager.play("engineerRepair");
            },
            () =>
            {
                return GhostEngineer.Player != null && GhostEngineer.Player == PlayerControl.LocalPlayer &&
                       !GhostEngineer.Fixes && PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                return isSabotageActive() && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            Engineer.buttonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            secondaryAbilityInput.keyCode,
            buttonText: GetString("RepairText")
        );

        //Sheriff Kill
        sheriffKillButton = new CustomButton(
            () =>
            {
                if (Sheriff.currentTarget == null) return;
                if (checkAndDoVetKill(Sheriff.currentTarget)) return;
                var murderAttemptResult = checkMuderAttempt(PlayerControl.LocalPlayer, Sheriff.currentTarget);
                if (murderAttemptResult == MurderAttemptResult.SuppressKill) return;
                var target = Sheriff.currentTarget;
                if (murderAttemptResult == MurderAttemptResult.PerformKill)
                {
                    byte targetId = 0;
                    var DeathReason = CustomDeathReason.SheriffKill;
                    if (Sheriff.sheriffCanKillNeutral(target))
                    {
                        targetId = target.PlayerId;
                    }
                    else
                    {
                        switch (Sheriff.misfireKills)
                        {
                            case 0:
                                targetId = PlayerControl.LocalPlayer.PlayerId;
                                DeathReason = CustomDeathReason.SheriffMisfire;
                                break;
                            case 1:
                                targetId = target.PlayerId;
                                DeathReason = CustomDeathReason.SheriffMisadventure;
                                break;
                            case 2:
                                targetId = target.PlayerId;
                                DeathReason = CustomDeathReason.SheriffMisadventure;

                                var killWriter2 = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
                                killWriter2.Write(PlayerControl.LocalPlayer.PlayerId);
                                killWriter2.Write(PlayerControl.LocalPlayer.PlayerId);
                                killWriter2.Write(byte.MaxValue);
                                killWriter2.EndRPC();
                                RPCProcedure.uncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                                GameHistory.RpcOverrideDeathReasonAndKiller(PlayerControl.LocalPlayer, CustomDeathReason.SheriffMisfire, PlayerControl.LocalPlayer);
                                break;
                        }
                    }

                    var killWriter = StartRPC(PlayerControl.LocalPlayer, CustomRPC.UncheckedMurderPlayer);
                    killWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                    killWriter.Write(targetId);
                    killWriter.Write(byte.MaxValue);
                    killWriter.EndRPC();
                    RPCProcedure.uncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, targetId, byte.MaxValue);
                    GameHistory.RpcOverrideDeathReasonAndKiller(target, DeathReason, PlayerControl.LocalPlayer);
                }

                if (murderAttemptResult == MurderAttemptResult.BodyGuardKill) checkMurderAttemptAndKill(PlayerControl.LocalPlayer, target);

                sheriffKillButton.Timer = sheriffKillButton.MaxTimer;
                Sheriff.currentTarget = null;
            },
            () =>
            {
                return Sheriff.Player != null && Sheriff.Player.Any(x => x == PlayerControl.LocalPlayer && x.IsAlive());
            },
            () =>
            {
                Sheriff.currentTarget = SetTarget();
                SetPlayerOutline(Sheriff.currentTarget, Sheriff.color);

                showTargetNameOnButton(Sheriff.currentTarget, sheriffKillButton, GetString("killButtonText"));
                return Sheriff.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { sheriffKillButton.Timer = sheriffKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        // Deputy Handcuff
        deputyHandcuffButton = new CustomButton(
            () =>
            {
                if (Sheriff.currentTarget == null) return;
                if (checkAndDoVetKill(Sheriff.currentTarget)) return;
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.DeputyUsedHandcuffs);
                writer.Write(Sheriff.currentTarget.PlayerId);
                writer.EndRPC();
                RPCProcedure.deputyUsedHandcuffs(Sheriff.currentTarget.PlayerId);
                Sheriff.currentTarget = null;
                deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer;

                SoundEffectsManager.play("deputyHandcuff");
            },
            () =>
            {
                return (Sheriff.Deputy.IsAlive() && PlayerControl.LocalPlayer == Sheriff.Deputy)
                       || Sheriff.Player.Any(x => x.IsAlive() && x == Sheriff.formerDeputy && x == PlayerControl.LocalPlayer);
            },
            () =>
            {
                Sheriff.currentTarget = SetTarget();
                SetPlayerOutline(Sheriff.currentTarget, Sheriff.color);

                showTargetNameOnButton(Sheriff.currentTarget, deputyHandcuffButton, GetString("HandcuffText"));
                if (deputyButtonHandcuffsText != null) deputyButtonHandcuffsText.text = $"{Sheriff.remainingHandcuffs}";
                return Sheriff.remainingHandcuffs > 0 && Sheriff.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer; },
            Sheriff.handcuffSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("HandcuffText")
        );
        // Deputy Handcuff button handcuff counter
        deputyButtonHandcuffsText = Object.Instantiate(deputyHandcuffButton.actionButton.cooldownTimerText,
            deputyHandcuffButton.actionButton.cooldownTimerText.transform.parent);
        deputyButtonHandcuffsText.text = "";
        deputyButtonHandcuffsText.enableWordWrapping = false;
        deputyButtonHandcuffsText.transform.localScale = Vector3.one * 0.5f;
        deputyButtonHandcuffsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        // Time Master Rewind Time
        timeMasterShieldButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.TimeMasterShield, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.timeMasterShield();
                SoundEffectsManager.play("timemasterShield");
            },
            () =>
            {
                return TimeMaster.timeMaster != null &&
                       TimeMaster.timeMaster == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
                timeMasterShieldButton.isEffectActive = false;
                timeMasterShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            TimeMaster.buttonSprite,
            ButtonPositions.lowerRowRight,
            __instance,
            abilityInput.keyCode,
            true,
            TimeMaster.shieldDuration,
            () =>
            {
                timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
                SoundEffectsManager.stop("timemasterShield");
            },
            buttonText: GetString("TimeShieldText")
        );

        // Veteran Alert
        veteranAlertButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.VeteranAlert, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.veteranAlert();
            },
            () =>
            {
                return Veteran.veteran != null && Veteran.veteran == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                veteranAlertButton.Timer = veteranAlertButton.MaxTimer;
                veteranAlertButton.isEffectActive = false;
                veteranAlertButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Veteran.buttonSprite,
            ButtonPositions.lowerRowRight, //brb
            __instance,
            abilityInput.keyCode,
            true,
            Veteran.alertDuration,
            () => { veteranAlertButton.Timer = veteranAlertButton.MaxTimer; },
            buttonText: GetString("AlertText")
        );

        // Medic Shield
        medicShieldButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Medic.currentTarget)) return;

                medicShieldButton.Timer = 0f;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    Medic.setShieldAfterMeeting ? (byte)CustomRPC.SetFutureShielded : (byte)CustomRPC.MedicSetShielded,
                    SendOption.Reliable);
                writer.Write(Medic.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                if (Medic.setShieldAfterMeeting)
                    RPCProcedure.setFutureShielded(Medic.currentTarget.PlayerId);
                else
                    RPCProcedure.medicSetShielded(Medic.currentTarget.PlayerId);
                Medic.meetingAfterShielding = false;

                SoundEffectsManager.play("medicShield");
            },
            () =>
            {
                return Medic.medic != null && Medic.medic == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                Medic.currentTarget = SetTarget();
                if (!Medic.usedShield)
                {
                    SetPlayerOutline(Medic.currentTarget, Medic.shieldedColor);
                    showTargetNameOnButton(Medic.currentTarget, medicShieldButton, GetString("ShieldText"));
                }
                return !Medic.usedShield && Medic.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                if (Medic.reset) Medic.resetShielded();
            },
            Medic.buttonSprite,
            ButtonPositions.lowerRowRight,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("ShieldText")
        );

        // doomsayer Shield
        doomsayerButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Doomsayer.currentTarget)) return;

                doomsayerButton.Timer = doomsayerButton.MaxTimer;
                SoundEffectsManager.play("knockKnock");
            },
            () =>
            {
                return Doomsayer.doomsayer != null && Doomsayer.doomsayer == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                Doomsayer.currentTarget = SetTarget();
                showTargetNameOnButton(Doomsayer.currentTarget, doomsayerButton, GetString("doomsayerText"));
                return PlayerControl.LocalPlayer.CanMove && Doomsayer.currentTarget != null;
            },
            () => { doomsayerButton.Timer = doomsayerButton.MaxTimer; },
            Doomsayer.buttonSprite,
            ButtonPositions.lowerRowRight,
            __instance,
            abilityInput.keyCode,
            true,
            0f,
            () =>
            {
                doomsayerButton.Timer = doomsayerButton.MaxTimer;
                var msg = Doomsayer.GetInfo(Doomsayer.currentTarget);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");

                // Ghost Info
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                writer.Write(Doomsayer.doomsayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
                writer.Write(msg);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            },
            buttonText: GetString("doomsayerText")
        );

        // Akujo Honmei
        akujoHonmeiButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Akujo.currentTarget)) return;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AkujoSetHonmei, SendOption.Reliable, -1);
                writer.Write(Akujo.akujo.PlayerId);
                writer.Write(Akujo.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.akujoSetHonmei(PlayerControl.LocalPlayer.PlayerId, Akujo.currentTarget.PlayerId);
            },
            () =>
            {
                return PlayerControl.LocalPlayer == Akujo.akujo
                       && !PlayerControl.LocalPlayer.Data.IsDead
                       && Akujo.honmei == null
                       && Akujo.timeLeft > 0;
            },
            () =>
            {
                return PlayerControl.LocalPlayer == Akujo.akujo
                       && !PlayerControl.LocalPlayer.Data.IsDead
                       && Akujo.currentTarget != null
                       && Akujo.honmei == null
                       && Akujo.timeLeft > 0;
            },
            () => { akujoHonmeiButton.Timer = akujoHonmeiButton.MaxTimer; },
            Akujo.honmeiSprite,
            ButtonPositions.upperRowRight,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("AkujoHonmeiText")
        );
        akujoTimeRemainingText = Object.Instantiate(akujoHonmeiButton.actionButton.cooldownTimerText, __instance.transform);
        akujoTimeRemainingText.text = "";
        akujoTimeRemainingText.enableWordWrapping = false;
        akujoTimeRemainingText.transform.localScale = Vector3.one * 0.45f;
        akujoTimeRemainingText.transform.localPosition = akujoHonmeiButton.actionButton.cooldownTimerText.transform.parent.localPosition + new Vector3(-0.1f, 0.35f, 0f);

        // Akujo Keep
        akujoBackupButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Akujo.currentTarget)) return;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.AkujoSetKeep, SendOption.Reliable, -1);
                writer.Write(Akujo.akujo.PlayerId);
                writer.Write(Akujo.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.akujoSetKeep(PlayerControl.LocalPlayer.PlayerId, Akujo.currentTarget.PlayerId);
            },
            () => { return PlayerControl.LocalPlayer == Akujo.akujo && !PlayerControl.LocalPlayer.Data.IsDead && Akujo.keepsLeft > 0; },
            () =>
            {
                if (akujoBackupLeftText != null)
                {
                    if (Akujo.keepsLeft > 0)
                        akujoBackupLeftText.text = Akujo.keepsLeft.ToString();
                    else
                        akujoBackupLeftText.text = "";
                }
                return PlayerControl.LocalPlayer == Akujo.akujo && !PlayerControl.LocalPlayer.Data.IsDead && Akujo.currentTarget != null && Akujo.keepsLeft > 0 && Akujo.timeLeft > 0;
            },
            () => { akujoBackupButton.Timer = akujoBackupButton.MaxTimer; },
            Akujo.keepSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            KeyCode.C,
            buttonText: GetString("AkujoBackupText")
        );
        akujoBackupLeftText = Object.Instantiate(akujoBackupButton.actionButton.cooldownTimerText, akujoBackupButton.actionButton.cooldownTimerText.transform.parent);
        akujoBackupLeftText.text = "";
        akujoBackupLeftText.enableWordWrapping = false;
        akujoBackupLeftText.transform.localScale = Vector3.one * 0.5f;
        akujoBackupLeftText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        evilTrapperSetTrapButton = new CustomButton(
            () =>
            { // ボタンが押された時に実行
                if (!PlayerControl.LocalPlayer.CanMove || KillTrap.hasTrappedPlayer()) return;
                EvilTrapper.setTrap();
                evilTrapperSetTrapButton.Timer = evilTrapperSetTrapButton.MaxTimer;
            },
            () =>
            { /*ボタン有効になる条件*/
                return PlayerControl.LocalPlayer == EvilTrapper.evilTrapper && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            { /*ボタンが使える条件*/
                return PlayerControl.LocalPlayer.CanMove && !KillTrap.hasTrappedPlayer();
            },
            () =>
            { /*ミーティング終了時*/
                evilTrapperSetTrapButton.Timer = evilTrapperSetTrapButton.MaxTimer;
            },
            EvilTrapper.trapButtonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("PlaceTrapText")
        );

        // Shifter shift
        shifterShiftButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Shifter.currentTarget)) return;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetFutureShifted, SendOption.Reliable);
                writer.Write(Shifter.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setFutureShifted(Shifter.currentTarget.PlayerId);
                SoundEffectsManager.play("shifterShift");
            },
            () =>
            {
                return Shifter.shifter.IsAlive() && Shifter.shifter == PlayerControl.LocalPlayer && Shifter.futureShift == null;
            },
            () =>
            {
                Shifter.currentTarget = SetTarget();
                if (Shifter.futureShift == null)
                {
                    SetPlayerOutline(Shifter.currentTarget, Color.yellow);
                    showTargetNameOnButton(Shifter.currentTarget, shifterShiftButton, GetString("ShiftText"));
                }
                return Shifter.currentTarget && Shifter.futureShift == null &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            Shifter.buttonSprite,
            new Vector3(0, 1f, 0),
            __instance,
            modifierAbilityInput.keyCode,
            true,
            buttonText: GetString("ShiftText")
        );

        // Disperser disperse
        disperserDisperseButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.Disperse, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.disperse();
                SoundEffectsManager.play("shifterShift");
            },
            () =>
            {
                return Disperser.disperser.IsAlive() && Disperser.disperser == PlayerControl.LocalPlayer && Disperser.remainingDisperses != 0;
            },
            () => { return Disperser.remainingDisperses > 0 && PlayerControl.LocalPlayer.CanMove; },
            () => { if (Disperser.remainingDisperses > 0) disperserDisperseButton.Timer = disperserDisperseButton.MaxTimer; },
            Disperser.buttonSprite,
            new Vector3(0, 1f, 0),
            __instance,
            modifierAbilityInput.keyCode,
            true,
            buttonText: GetString("DisperseText")
        );

        mayorMeetingButton = new CustomButton(
            () =>
            {
                //PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                Mayor.remoteMeetingsLeft--;

                handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                handleBomberExplodeOnBodyReport();
                handleTrapperTrapOnBodyReport();
                RPCProcedure.uncheckedCmdReportDeadBody(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                if (AmongUsClient.Instance.AmHost)
                    Mayor.mayor.NoCheckStartMeeting(null, true);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.MayorMeeting, SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                mayorMeetingButton.Timer = 1f;
            },
            () =>
            {
                return Mayor.mayor.IsAlive() && Mayor.mayor == PlayerControl.LocalPlayer && Mayor.meetingButton;
            },
            () =>
            {
                mayorMeetingButton.actionButton.OverrideText(GetString("MayorButtonText") + "(" + Mayor.remoteMeetingsLeft + ")");
                var sabotageActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    if ((task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor ||
                    task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles ||
                        SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask) && !Mayor.SabotageRemoteMeetings)
                        sabotageActive = true;
                return !sabotageActive && PlayerControl.LocalPlayer.CanMove &&
                       Mayor.remoteMeetingsLeft > 0;
            },
            () => { mayorMeetingButton.Timer = mayorMeetingButton.MaxTimer; },
            Mayor.emergencySprite,
            ButtonPositions.lowerRowRight,
            __instance,
            abilityInput.keyCode,
            true,
            0f,
            () => { },
            false,
            buttonText: GetString("MayorButtonText")
        );

        // ButtonBarry Meetings
        buttonBarryButton = new CustomButton(
            () =>
            {
                //PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                ButtonBarry.remoteMeetingsLeft--;

                handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                handleBomberExplodeOnBodyReport();
                handleTrapperTrapOnBodyReport();
                RPCProcedure.uncheckedCmdReportDeadBody(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                if (AmongUsClient.Instance.AmHost)
                    ButtonBarry.buttonBarry.NoCheckStartMeeting(null, true);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.BarryMeeting, SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                buttonBarryButton.Timer = 1f;

            },
            () =>
            {
                return ButtonBarry.buttonBarry != null && ButtonBarry.buttonBarry == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                var sabotageActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    if ((task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor ||
                    task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles ||
                        SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask) && !ButtonBarry.SabotageRemoteMeetings)
                        sabotageActive = true;
                return !sabotageActive && PlayerControl.LocalPlayer.CanMove &&
                       ButtonBarry.remoteMeetingsLeft > 0;
            },
            () => { buttonBarryButton.Timer = buttonBarryButton.MaxTimer; },
            ButtonBarry.buttonSprite,
            new Vector3(0, 1f, 0),
            __instance,
            modifierAbilityInput.keyCode,
            true,
            buttonText: "buttonBarryText".Translate()
        );

        // Morphling morphs
        morphlingButton = new CustomButton(
            () =>
            {
                if (Morphling.sampledTarget != null)
                {
                    if (checkAndDoVetKill(Morphling.currentTarget)) return;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.MorphlingMorph,
                        SendOption.Reliable);
                    writer.Write(Morphling.sampledTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.morphlingMorph(Morphling.sampledTarget.PlayerId);
                    Morphling.sampledTarget = null;
                    morphlingButton.EffectDuration = Morphling.duration;
                    SoundEffectsManager.play("morphlingMorph");
                }
                else if (Morphling.currentTarget != null)
                {
                    Morphling.sampledTarget = Morphling.currentTarget;
                    morphlingButton.Sprite = Morphling.morphSprite;
                    morphlingButton.EffectDuration = 1f;
                    SoundEffectsManager.play("morphlingSample");

                    // Add poolable player to the button so that the target outfit is shown
                    setButtonTargetDisplay(Morphling.sampledTarget, morphlingButton);
                }
            },
            () =>
            {
                return Morphling.morphling != null && Morphling.morphling == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                Morphling.currentTarget = SetTarget();
                SetPlayerOutline(Morphling.currentTarget, Morphling.color);

                if (Morphling.sampledTarget == null) showTargetNameOnButton(Morphling.currentTarget, morphlingButton, GetString("SampleText"));
                return (Morphling.currentTarget || Morphling.sampledTarget) && !isActiveCamoComms &&
                       PlayerControl.LocalPlayer.CanMove && !MushroomSabotageActive;
            },
            () =>
            {
                morphlingButton.Timer = morphlingButton.MaxTimer;
                morphlingButton.Sprite = Morphling.sampleSprite;
                morphlingButton.isEffectActive = false;
                morphlingButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Morphling.sampledTarget = null;
                setButtonTargetDisplay(null);
            },
            Morphling.sampleSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            true,
            Morphling.duration,
            () =>
            {
                if (Morphling.sampledTarget == null)
                {
                    morphlingButton.Timer = morphlingButton.MaxTimer;
                    morphlingButton.Sprite = Morphling.sampleSprite;
                    SoundEffectsManager.play("morphlingMorph");

                    // Reset the poolable player
                    setButtonTargetDisplay(null);
                }
            },
            buttonText: GetString("SampleText")
        );

        // Camouflager camouflage
        camouflagerButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.CamouflagerCamouflage, SendOption.Reliable);
                writer.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.camouflagerCamouflage(1);
                SoundEffectsManager.play("morphlingMorph");
            },
            () =>
            {
                return Camouflager.camouflager != null &&
                       Camouflager.camouflager == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { return !isActiveCamoComms && PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                camouflagerButton.Timer = camouflagerButton.MaxTimer;
                camouflagerButton.isEffectActive = false;
                camouflagerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Camouflager.buttonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            true,
            Camouflager.duration,
            () =>
            {
                camouflagerButton.Timer = camouflagerButton.MaxTimer;
                SoundEffectsManager.play("morphlingMorph");
            },
            buttonText: GetString("CamouflageText")
        );

        // Hacker button
        hackerButton = new CustomButton(
            () =>
            {
                Hacker.hackerTimer = Hacker.duration;
                SoundEffectsManager.play("hackerHack");
            },
            () =>
            {
                return Hacker.hacker != null && Hacker.hacker == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { return true; },
            () =>
            {
                hackerButton.Timer = hackerButton.MaxTimer;
                hackerButton.isEffectActive = false;
                hackerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Hacker.buttonSprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            true,
            0f,
            () => { hackerButton.Timer = hackerButton.MaxTimer; },
            buttonText: GetString("hackerButtonText")
        );

        hackerAdminTableButton = new CustomButton(
            () =>
            {
                if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                {
                    var __instance = FastDestroyableSingleton<HudManager>.Instance;
                    __instance.InitMap();
                    MapBehaviour.Instance.ShowCountOverlay(true, true, true);
                }

                if (Hacker.cantMove) PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                Hacker.chargesAdminTable--;
            },
            () =>
            {
                return Hacker.hacker != null && Hacker.hacker == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (hackerAdminTableChargesText != null) hackerAdminTableChargesText.text = $"{Hacker.chargesAdminTable} / {Hacker.toolsNumber}";
                return Hacker.chargesAdminTable > 0;
            },
            () =>
            {
                hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
                hackerAdminTableButton.isEffectActive = false;
                hackerAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Hacker.getAdminSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            secondaryAbilityInput.keyCode,
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
        );

        // Hacker Admin Table Charges
        hackerAdminTableChargesText = Object.Instantiate(hackerAdminTableButton.actionButton.cooldownTimerText,
            hackerAdminTableButton.actionButton.cooldownTimerText.transform.parent);
        hackerAdminTableChargesText.text = "";
        hackerAdminTableChargesText.enableWordWrapping = false;
        hackerAdminTableChargesText.transform.localScale = Vector3.one * 0.5f;
        hackerAdminTableChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        hackerVitalsButton = new CustomButton(
            () =>
            {
                if (GameOptionsManager.Instance.currentNormalGameOptions.MapId != 1)
                {
                    if (Hacker.vitals == null)
                    {
                        var e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("panel_vitals") || x.gameObject.name.Contains("Vitals"));
                        if (e == null || Camera.main == null) return;
                        Hacker.vitals = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    Hacker.vitals.transform.SetParent(Camera.main.transform, false);
                    Hacker.vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Hacker.vitals.Begin(null);
                }
                else
                {
                    if (Hacker.doorLog == null)
                    {
                        var e = Object.FindObjectsOfType<SystemConsole>()
                            .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        Hacker.doorLog = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    Hacker.doorLog.transform.SetParent(Camera.main.transform, false);
                    Hacker.doorLog.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Hacker.doorLog.Begin(null);
                }

                if (Hacker.cantMove) PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 

                Hacker.chargesVitals--;
            },
            () =>
            {
                return Hacker.hacker != null && Hacker.hacker == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead &&
                       GameOptionsManager.Instance.currentGameOptions.MapId != 0 &&
                       GameOptionsManager.Instance.currentNormalGameOptions.MapId != 3;
            },
            () =>
            {
                if (hackerVitalsChargesText != null)
                    hackerVitalsChargesText.text = $"{Hacker.chargesVitals} / {Hacker.toolsNumber}";
                hackerVitalsButton.actionButton.graphic.sprite =
                    isMira ? Hacker.getLogSprite() : Hacker.getVitalsSprite();
                hackerVitalsButton.actionButton.OverrideText(isMira ? GetString("hackerDoorLogText") : GetString("hackerVitalText"));
                return Hacker.chargesVitals > 0;
            },
            () =>
            {
                hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                hackerVitalsButton.isEffectActive = false;
                hackerVitalsButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Hacker.getVitalsSprite(),
            ButtonPositions.lowerRowCenter,
            __instance,
            abilityInput.keyCode,
            true,
            0f,
            () => true,
            () =>
            {
                if (GameOptionsManager.Instance.currentNormalGameOptions.MapId != 1)
                {
                    if (Hacker.vitals == null)
                    {
                        var e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("panel_vitals") || x.gameObject.name.Contains("Vitals"));
                        if (e == null || Camera.main == null) return;
                        Hacker.vitals = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    Hacker.vitals.transform.SetParent(Camera.main.transform, false);
                    Hacker.vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Hacker.vitals.Begin(null);
                }
                else
                {
                    if (Hacker.doorLog == null)
                    {
                        var e = Object.FindObjectsOfType<SystemConsole>()
                            .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        Hacker.doorLog = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    Hacker.doorLog.transform.SetParent(Camera.main.transform, false);
                    Hacker.doorLog.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Hacker.doorLog.Begin(null);
                }

            },
            () =>
            {
                hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                if (!hackerAdminTableButton.isEffectActive) PlayerControl.LocalPlayer.moveable = true;
                if (Minigame.Instance)
                {
                    if (isMira) Hacker.doorLog.ForceClose();
                    else Hacker.vitals.ForceClose();
                }
            },
            false,
            isMira ? GetString("hackerDoorLogText") : GetString("hackerVitalText")
        );

        // Hacker Vitals Charges
        hackerVitalsChargesText = Object.Instantiate(hackerVitalsButton.actionButton.cooldownTimerText,
            hackerVitalsButton.actionButton.cooldownTimerText.transform.parent);
        hackerVitalsChargesText.text = "";
        hackerVitalsChargesText.enableWordWrapping = false;
        hackerVitalsChargesText.transform.localScale = Vector3.one * 0.5f;
        hackerVitalsChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        // Tracker button
        trackerTrackPlayerButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Tracker.currentTarget)) return;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.TrackerUsedTracker, SendOption.Reliable);
                writer.Write(Tracker.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.trackerUsedTracker(Tracker.currentTarget.PlayerId);
                SoundEffectsManager.play("trackerTrackPlayer");
            },
            () =>
            {
                return Tracker.tracker != null && Tracker.tracker == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                Tracker.currentTarget = SetTarget();
                if (!Tracker.usedTracker)
                {
                    SetPlayerOutline(Tracker.currentTarget, Tracker.color);
                    showTargetNameOnButton(Tracker.currentTarget, trackerTrackPlayerButton, GetString("TrackerText"));
                }

                return PlayerControl.LocalPlayer.CanMove && Tracker.currentTarget != null && !Tracker.usedTracker;
            },
            () =>
            {
                if (Tracker.resetTargetAfterMeeting) Tracker.resetTracked();
            },
            Tracker.buttonSprite,
            ButtonPositions.lowerRowRight,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("TrackerText")
        );

        trackerTrackCorpsesButton = new CustomButton(
            () =>
            {
                Tracker.corpsesTrackingTimer = Tracker.corpsesTrackingDuration;
                SoundEffectsManager.play("trackerTrackCorpses");
            },
            () =>
            {
                return Tracker.tracker != null && Tracker.tracker == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead && Tracker.canTrackCorpses;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer;
                trackerTrackCorpsesButton.isEffectActive = false;
                trackerTrackCorpsesButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Tracker.trackCorpsesButtonSprite,
            ButtonPositions.lowerRowCenter,
            __instance,
            secondaryAbilityInput.keyCode,
            true,
            Tracker.corpsesTrackingDuration,
            () => { trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer; },
            buttonText: GetString("TrackerCorpsesText")
        );

        bodyGuardGuardButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(BodyGuard.currentTarget)) return;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.BodyGuardGuardPlayer, SendOption.Reliable);
                writer.Write(BodyGuard.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.bodyGuardGuardPlayer(BodyGuard.currentTarget.PlayerId);
                // SoundEffectsManager.play("trackerTrackPlayer");
            },
            () =>
            {
                return BodyGuard.bodyguard != null && BodyGuard.bodyguard == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                BodyGuard.currentTarget = SetTarget();
                if (!BodyGuard.usedGuard)
                {
                    SetPlayerOutline(Medic.currentTarget, Medic.shieldedColor);
                    showTargetNameOnButton(BodyGuard.currentTarget, bodyGuardGuardButton, GetString("bodyGuardText"));
                }
                return PlayerControl.LocalPlayer.CanMove && BodyGuard.currentTarget != null &&
                       !BodyGuard.usedGuard;
            },
            () =>
            {
                if (BodyGuard.reset) BodyGuard.resetGuarded();
            },
            BodyGuard.guardButtonSprite,
            ButtonPositions.lowerRowRight, //brb
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("bodyGuardText")
        );

        vampireKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Vampire.currentTarget)) return;
                var murder = checkMuderAttempt(Vampire.vampire, Vampire.currentTarget);
                if (murder == MurderAttemptResult.PerformKill)
                {
                    if (Vampire.targetNearGarlic)
                    {
                        var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
                        writer.Write(Vampire.vampire.PlayerId);
                        writer.Write(Vampire.currentTarget.PlayerId);
                        writer.Write(byte.MaxValue);
                        writer.EndRPC();
                        RPCProcedure.uncheckedMurderPlayer(Vampire.vampire.PlayerId, Vampire.currentTarget.PlayerId, byte.MaxValue);

                        vampireKillButton.HasEffect = false; // Block effect on this click
                        vampireKillButton.Timer = vampireKillButton.MaxTimer;
                    }
                    else
                    {
                        Vampire.bitten = Vampire.currentTarget;
                        // Notify players about bitten
                        var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
                        writer.Write(Vampire.bitten.PlayerId);
                        writer.Write((byte)0);
                        writer.EndRPC();
                        RPCProcedure.vampireSetBitten(Vampire.bitten.PlayerId, 0);

                        var lastTimer = (byte)Vampire.delay;
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Vampire.delay,
                            new Action<float>(p =>
                            {
                                // Delayed action
                                if (p <= 1f)
                                {
                                    var timer = (byte)vampireKillButton.Timer;
                                    if (timer != lastTimer)
                                    {
                                        lastTimer = timer;
                                        var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareGhostInfo);
                                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                                        writer.Write((byte)RPCProcedure.GhostInfoTypes.VampireTimer);
                                        writer.Write(timer);
                                        writer.EndRPC();
                                    }
                                }

                                if (p == 1f)
                                {
                                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                    var res = checkMurderAttemptAndKill(Vampire.vampire, Vampire.bitten, showAnimation: false);
                                    if (res == MurderAttemptResult.PerformKill)
                                    {
                                        var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
                                        writer.Write(byte.MaxValue);
                                        writer.Write(byte.MaxValue);
                                        writer.EndRPC();
                                        RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
                                    }
                                }
                            })));
                        SoundEffectsManager.play("vampireBite");

                        vampireKillButton.HasEffect = true; // Trigger effect on this click
                    }
                }
                else if (murder == MurderAttemptResult.BlankKill)
                {
                    vampireKillButton.Timer = vampireKillButton.MaxTimer;
                    vampireKillButton.HasEffect = false;
                }
                else if (murder == MurderAttemptResult.BodyGuardKill)
                {
                    checkMurderAttemptAndKill(Vampire.vampire, Vampire.currentTarget);
                }
                else
                {
                    vampireKillButton.HasEffect = false;
                }
            },
            () =>
            {
                return Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                List<PlayerControl> untargetablePlayers = [];
                if (Spy.spy != null && !Spy.impostorsCanKillAnyone) untargetablePlayers.Add(Spy.spy);
                if (SchrodingersCat.Player.IsAlive() && SchrodingersCat.State == SchrodingersCat.CatState.Impostor) untargetablePlayers.Add(SchrodingersCat.Player);
                var target = SetTarget(untargetablePlayers, !(Spy.spy != null && Spy.impostorsCanKillAnyone), true);

                bool targetNearGarlic = false;
                if (target != null)
                    foreach (var garlic in Garlic.garlics)
                        if (Vector2.Distance(garlic.garlic.transform.position, target.transform.position) <= 1.95f)
                            targetNearGarlic = true;
                Vampire.targetNearGarlic = targetNearGarlic;
                Vampire.currentTarget = target;
                SetPlayerOutline(Vampire.currentTarget, Vampire.color);

                if (Vampire.targetNearGarlic)
                    showTargetNameOnButton(Vampire.currentTarget, vampireKillButton, GetString("killButtonText"));
                else
                    showTargetNameOnButton(Vampire.currentTarget, vampireKillButton, GetString("VampireText"));
                if (Vampire.targetNearGarlic && Vampire.canKillNearGarlics)
                {
                    vampireKillButton.actionButton.graphic.sprite = __instance.KillButton.graphic.sprite;
                    vampireKillButton.showButtonText = true;
                }
                else
                {
                    vampireKillButton.actionButton.graphic.sprite = Vampire.buttonSprite;
                    vampireKillButton.showButtonText = false;
                }

                return Vampire.currentTarget != null && PlayerControl.LocalPlayer.CanMove &&
                       (!Vampire.targetNearGarlic || Vampire.canKillNearGarlics);
            },
            () =>
            {
                vampireKillButton.MaxTimer = LastImpostor.lastImpostor == PlayerControl.LocalPlayer
                    ? Vampire.cooldown - LastImpostor.deduce
                    : Vampire.cooldown;
                vampireKillButton.Timer = vampireKillButton.MaxTimer;
                vampireKillButton.isEffectActive = false;
                vampireKillButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Vampire.buttonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            modKillInput.keyCode,
            false,
            0f,
            () => { vampireKillButton.Timer = vampireKillButton.MaxTimer; },
            buttonText: "VampireText".Translate()
        );

        garlicButton = new CustomButton(
            () =>
            {
                Vampire.localPlacedGarlic = true;
                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.PlaceGarlic);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.placeGarlic(buff);
                SoundEffectsManager.play("garlic");
            },
            () =>
            {
                return Vampire.garlicButton && !Vampire.localPlacedGarlic && !PlayerControl.LocalPlayer.Data.IsDead &&
                       Vampire.garlicsActive;
            },
            () =>
            {
                return Vampire.garlicButton && PlayerControl.LocalPlayer.CanMove &&
                       !Vampire.localPlacedGarlic;
            },
            () => { },
            Vampire.garlicButtonSprite,
            new Vector3(0, 0f, 0),
            __instance,
            null,
            true,
            buttonText: GetString("GarlicText")
        );

        prophetButton = new CustomButton(
                () =>
                {
                    if (checkAndDoVetKill(Prophet.currentTarget)) return;
                    if (Prophet.currentTarget != null)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ProphetExamine, SendOption.Reliable, -1);
                        writer.Write(Prophet.currentTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.prophetExamine(Prophet.currentTarget.PlayerId);
                        prophetButton.Timer = prophetButton.MaxTimer;
                    }
                },
                () =>
                {
                    return Prophet.prophet != null && PlayerControl.LocalPlayer == Prophet.prophet
                           && !PlayerControl.LocalPlayer.Data.IsDead && Prophet.examinesLeft > 0;
                },
                () =>
                {
                    Prophet.currentTarget = SetTarget();
                    SetPlayerOutline(Prophet.currentTarget, Prophet.color);

                    if (prophetButtonText != null)
                    {
                        if (Prophet.examinesLeft > 0)
                            prophetButtonText.text = $"{Prophet.examinesLeft}";
                        else
                            prophetButtonText.text = "";
                    }
                    return Prophet.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
                },
                () => { prophetButton.Timer = prophetButton.MaxTimer; },
                Prophet.buttonSprite,
                ButtonPositions.lowerRowRight,
                __instance,
                abilityInput.keyCode,
                buttonText: GetString("ProphetText")
            );
        prophetButtonText = Object.Instantiate(prophetButton.actionButton.cooldownTimerText, prophetButton.actionButton.cooldownTimerText.transform.parent);
        prophetButtonText.text = "";
        prophetButtonText.enableWordWrapping = false;
        prophetButtonText.transform.localScale = Vector3.one * 0.5f;
        prophetButtonText.transform.localPosition += new Vector3(-0.05f, 0.55f, -1f);

        portalmakerPlacePortalButton = new CustomButton(
            () =>
            {
                portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.PlacePortal);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.placePortal(buff);
                SoundEffectsManager.play("tricksterPlaceBox");
            },
            () =>
            {
                return Portalmaker.portalmaker != null &&
                       Portalmaker.portalmaker == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead && Portal.secondPortal == null;
            },
            () => { return PlayerControl.LocalPlayer.CanMove && Portal.secondPortal == null; },
            () => { portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer; },
            Portalmaker.placePortalButtonSprite,
            ButtonPositions.lowerRowRight,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("PlacePortalText")
        );

        usePortalButton = new CustomButton(
            () =>
            {
                var didTeleport = false;
                Vector3 exit = Portal.findExit(PlayerControl.LocalPlayer.transform.position);
                Vector3 entry = Portal.findEntry(PlayerControl.LocalPlayer.transform.position);

                var portalMakerSoloTeleport = !Portal.locationNearEntry(PlayerControl.LocalPlayer.transform.position);
                if (portalMakerSoloTeleport)
                {
                    exit = Portal.firstPortal.portalGameObject.transform.position;
                    entry = PlayerControl.LocalPlayer.transform.position;
                }

                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(entry);

                if (!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    // Ghosts can portal too, but non-blocking and only with a local animation
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UsePortal, SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(portalMakerSoloTeleport ? (byte)1 : (byte)0);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                RPCProcedure.usePortal(PlayerControl.LocalPlayer.PlayerId, portalMakerSoloTeleport ? (byte)1 : (byte)0);
                usePortalButton.Timer = usePortalButton.MaxTimer;
                portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer;
                SoundEffectsManager.play("portalUse");
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Portal.teleportDuration,
                    new Action<float>(p =>
                    {
                        // Delayed action
                        PlayerControl.LocalPlayer.moveable = false;
                        PlayerControl.LocalPlayer.NetTransform.Halt();
                        if (p >= 0.5f && p <= 0.53f && !didTeleport && !MeetingHud.Instance)
                        {
                            if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(exit.y > -7);
                            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(exit);
                            didTeleport = true;
                        }

                        if (p == 1f) PlayerControl.LocalPlayer.moveable = true;
                    })));
            },
            () =>
            {
                if (PlayerControl.LocalPlayer == Portalmaker.portalmaker && Portal.bothPlacedAndEnabled)
                    portalmakerButtonText1.text =
                        Portal.locationNearEntry(PlayerControl.LocalPlayer.transform.position) ||
                        !Portalmaker.canPortalFromAnywhere
                            ? ""
                            : "1. " + Portal.firstPortal.room;
                return Portal.bothPlacedAndEnabled;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove &&
                       (Portal.locationNearEntry(PlayerControl.LocalPlayer.transform.position) ||
                        Portalmaker.canPortalFromAnywhere &&
                         PlayerControl.LocalPlayer == Portalmaker.portalmaker) && !Portal.isTeleporting;
            },
            () => { usePortalButton.Timer = usePortalButton.MaxTimer; },
            Portalmaker.usePortalButtonSprite,
            new Vector3(1f, 0f, 0),
            __instance,
            null,
            true,
            buttonText: GetString("usePortalText")
        );

        portalmakerMoveToPortalButton = new CustomButton(
            () =>
            {
                var didTeleport = false;
                var exit = Portal.secondPortal.portalGameObject.transform.position;

                if (!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    // Ghosts can portal too, but non-blocking and only with a local animation
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UsePortal, SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)2);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                RPCProcedure.usePortal(PlayerControl.LocalPlayer.PlayerId, 2);
                usePortalButton.Timer = usePortalButton.MaxTimer;
                portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer;
                SoundEffectsManager.play("portalUse");
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Portal.teleportDuration,
                    new Action<float>(p =>
                    {
                        // Delayed action
                        PlayerControl.LocalPlayer.moveable = false;
                        PlayerControl.LocalPlayer.NetTransform.Halt();
                        if (p >= 0.5f && p <= 0.53f && !didTeleport && !MeetingHud.Instance)
                        {
                            if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(exit.y > -7);
                            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(exit);
                            didTeleport = true;
                        }

                        if (p == 1f) PlayerControl.LocalPlayer.moveable = true;
                    })));
            },
            () =>
            {
                return Portalmaker.canPortalFromAnywhere && Portal.bothPlacedAndEnabled &&
                       PlayerControl.LocalPlayer == Portalmaker.portalmaker;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove &&
                       !Portal.locationNearEntry(PlayerControl.LocalPlayer.transform.position) && !Portal.isTeleporting;
            },
            () => { portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer; },
            Portalmaker.usePortalButtonSprite,
            new Vector3(1f, 1f, 0),
            __instance,
            null,
            true
        );


        portalmakerButtonText1 = Object.Instantiate(usePortalButton.actionButton.cooldownTimerText,
            usePortalButton.actionButton.cooldownTimerText.transform.parent);
        portalmakerButtonText1.text = "";
        portalmakerButtonText1.enableWordWrapping = false;
        portalmakerButtonText1.transform.localScale = Vector3.one * 0.5f;
        portalmakerButtonText1.transform.localPosition += new Vector3(-0.05f, 0.55f, -1f);

        portalmakerButtonText2 = Object.Instantiate(portalmakerMoveToPortalButton.actionButton.cooldownTimerText,
            portalmakerMoveToPortalButton.actionButton.cooldownTimerText.transform.parent);
        portalmakerButtonText2.text = "";
        portalmakerButtonText2.enableWordWrapping = false;
        portalmakerButtonText2.transform.localScale = Vector3.one * 0.5f;
        portalmakerButtonText2.transform.localPosition += new Vector3(-0.05f, 0.55f, -1f);

        // Jackal Kill
        jackalKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Jackal.currentTarget)) return;
                if (checkMurderAttemptAndKill(PlayerControl.LocalPlayer, Jackal.currentTarget) ==
                    MurderAttemptResult.SuppressKill) return;

                jackalKillButton.Timer = jackalKillButton.MaxTimer;
            },
            () =>
            {
                return (Jackal.jackal.Any(x => x == PlayerControl.LocalPlayer) ||
                        (Jackal.Sidekick == PlayerControl.LocalPlayer && Jackal.sidekickCanKill)) &&
                        PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                showTargetNameOnButton(Jackal.currentTarget, jackalKillButton, GetString("killButtonText"));
                return Jackal.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { jackalKillButton.Timer = jackalKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode
        );

        // Jackal Sidekick Button
        jackalCreateSidekickButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Jackal.currentTarget)) return;
                var target = Jackal.currentTarget;

                if (Jackal.killFakeImpostor && target.IsImpostor())
                {
                    //uncheckedMurderPlayer(Jackal.jackal.PlayerId, player.PlayerId, 1);
                    checkMurderAttemptAndKill(PlayerControl.LocalPlayer, target);
                    GameHistory.RpcOverrideDeathReasonAndKiller(target, CustomDeathReason.FakeSK, PlayerControl.LocalPlayer);
                    jackalCreateSidekickButton.Timer = jackalCreateSidekickButton.MaxTimer;
                    return;
                }

                var writer = StartRPC(CustomRPC.JackalCreatesSidekick);
                writer.Write(Jackal.currentTarget.PlayerId);
                writer.EndRPC();
                RPCProcedure.jackalCreatesSidekick(Jackal.currentTarget.PlayerId);
                SoundEffectsManager.play("jackalSidekick");
                jackalCreateSidekickButton.Timer = jackalCreateSidekickButton.MaxTimer;
            },
            () =>
            {
                var untargetablePlayers = new List<PlayerControl>();
                untargetablePlayers.AddRange(Jackal.jackal);
                if (Jackal.Sidekick != null) untargetablePlayers.Add(Jackal.Sidekick);
                if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
                Jackal.currentTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(Jackal.currentTarget, Palette.ImpostorRed);

                return Jackal.canCreateSidekick && Jackal.Sidekick == null && Jackal.jackal.Any(x => x.IsAlive() && x == PlayerControl.LocalPlayer);
            },
            () =>
            {

                // Show now text since the button already says sidekick
                showTargetNameOnButton(Jackal.currentTarget, jackalCreateSidekickButton, GetString("jackalSidekickText"));
                return Jackal.canCreateSidekick && Jackal.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { jackalCreateSidekickButton.Timer = jackalCreateSidekickButton.MaxTimer; },
            Jackal.SidekickButton,
            ButtonPositions.lowerRowCenter,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("jackalSidekickText")
        );

        jackalSwoopButton = new CustomButton(
            () =>
            { /* On Use */
                var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetJackalSwoop, SendOption.Reliable, -1);
                invisibleWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                invisibleWriter.Write(byte.MinValue);
                AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                RPCProcedure.setJackalSwoop(PlayerControl.LocalPlayer.PlayerId, byte.MinValue);
            },
            () =>
            {   /* Can See */
                return Jackal.jackal != null && Jackal.canSwoop &&
                       Jackal.jackal.Any(x => x == PlayerControl.LocalPlayer) && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {   /* On Click */
                return Jackal.canSwoop && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {  /* On Meeting End */
                jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer;
                jackalSwoopButton.isEffectActive = false;
                jackalSwoopButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Jackal.isInvisable = false;
            },
            Swooper.SwoopButtonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            secondaryAbilityInput.keyCode,
            true,
            Jackal.duration,
            () => { jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer; },
            buttonText: GetString("SwoopText")
        );

        // Swooper Kill
        swooperKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Swooper.currentTarget)) return;
                if (checkMurderAttemptAndKill(Swooper.swooper, Swooper.currentTarget) == MurderAttemptResult.SuppressKill) return;

                swooperKillButton.Timer = swooperKillButton.MaxTimer;
                Swooper.currentTarget = null;
            },
            () => { return Swooper.swooper != null && Swooper.swooper == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { showTargetNameOnButton(Swooper.currentTarget, swooperKillButton, GetString("killButtonText")); return Swooper.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { swooperKillButton.Timer = swooperKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            //new Vector3(0, 1f, 0),
            __instance,
            modKillInput.keyCode
        );

        swooperSwoopButton = new CustomButton(
            () =>
            { /* On Use */
                var invisibleWriter = StartRPC(CustomRPC.SetSwoop);
                invisibleWriter.Write(Swooper.swooper.PlayerId);
                invisibleWriter.Write(byte.MinValue);
                invisibleWriter.EndRPC();
                RPCProcedure.setSwoop(Swooper.swooper.PlayerId, byte.MinValue);
            },
            () => { /* Can See */ return Swooper.swooper != null && Swooper.swooper == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                /* On Click */
                // Exclude Jackal from targeting the Mini unless it has grown up
                var untargetablePlayers = new List<PlayerControl>();
                if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
                Swooper.currentTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(Swooper.currentTarget, Palette.ImpostorRed);

                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {  /* On Meeting End */
                swooperSwoopButton.Timer = swooperSwoopButton.MaxTimer;
                swooperSwoopButton.isEffectActive = false;
                swooperSwoopButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Swooper.isInvisable = false;
            },
            Swooper.SwoopButtonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            abilityInput.keyCode,
            true,
            Swooper.duration,
            () => { swooperSwoopButton.Timer = swooperSwoopButton.MaxTimer; },
            buttonText: GetString("SwoopText")
        );

        pavlovsdogsKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Pavlovsdogs.killTarget)) return;
                if (checkMurderAttemptAndKill(PlayerControl.LocalPlayer, Pavlovsdogs.killTarget) == MurderAttemptResult.SuppressKill) return;
                if (Pavlovsdogs.enableRampage)
                {
                    Pavlovsdogs.deathTime = Pavlovsdogs.rampageDeathTime;
                    pavlovsdogsKillButton.MaxTimer = Pavlovsdogs.pavlovsowner.IsDead() ? Pavlovsdogs.rampageKillCooldown : Pavlovsdogs.cooldown;
                }
                pavlovsdogsKillButton.Timer = pavlovsdogsKillButton.MaxTimer;
                Pavlovsdogs.killTarget = null;
            },
            () =>
            {
                return Pavlovsdogs.pavlovsdogs != null
                       && Pavlovsdogs.pavlovsdogs.Any(x => x == PlayerControl.LocalPlayer)
                       && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (Pavlovsdogs.enableRampage && Pavlovsdogs.pavlovsowner.IsDead() && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Pavlovsdogs.deathTime -= Time.deltaTime;
                    if (PavlovsdogKillSelfText != null)
                        PavlovsdogKillSelfText.text = string.Format("SerialKillerSuicideText".Translate(), (int)Pavlovsdogs.deathTime + 1);

                    if (Pavlovsdogs.deathTime <= 0)
                        PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer, true);
                }

                var untargetablePlayers = new List<PlayerControl>();
                untargetablePlayers.AddRange(Pavlovsdogs.pavlovsdogs);
                if (Pavlovsdogs.pavlovsowner != null) untargetablePlayers.Add(Pavlovsdogs.pavlovsowner);
                if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
                Pavlovsdogs.killTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(Pavlovsdogs.killTarget, Palette.ImpostorRed);

                showTargetNameOnButton(Pavlovsdogs.killTarget, pavlovsdogsKillButton, GetString("killButtonText")); return Pavlovsdogs.killTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                if (Pavlovsdogs.enableRampage)
                {
                    Pavlovsdogs.deathTime = Pavlovsdogs.rampageDeathTime;
                    pavlovsdogsKillButton.MaxTimer = Pavlovsdogs.pavlovsowner.IsDead() ? Pavlovsdogs.rampageKillCooldown : Pavlovsdogs.cooldown;
                }
                pavlovsdogsKillButton.Timer = pavlovsdogsKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );
        PavlovsdogKillSelfText = Object.Instantiate(pavlovsdogsKillButton.actionButton.cooldownTimerText,
            pavlovsdogsKillButton.actionButton.cooldownTimerText.transform.parent);
        PavlovsdogKillSelfText.text = "";
        PavlovsdogKillSelfText.enableWordWrapping = false;
        PavlovsdogKillSelfText.transform.localScale = Vector3.one * 0.5f;
        PavlovsdogKillSelfText.transform.localPosition += new Vector3(-0.05f, 0.55f, -1f);

        pavlovsownerCreateDogButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Pavlovsdogs.currentTarget)) return;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.PavlovsCreateDog, SendOption.Reliable);
                writer.Write(Pavlovsdogs.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.pavlovsCreateDog(Pavlovsdogs.currentTarget.PlayerId);
                SoundEffectsManager.play("jackalSidekick");

                pavlovsownerCreateDogButton.Timer = pavlovsownerCreateDogButton.MaxTimer;
            },
            () =>
            {
                return Pavlovsdogs.pavlovsowner != null
                    && Pavlovsdogs.pavlovsowner == PlayerControl.LocalPlayer
                    && !PlayerControl.LocalPlayer.Data.IsDead
                    && Pavlovsdogs.canCreateDog;
            },
            () =>
            {
                if (PavlovsdogCreateNumText != null)
                    PavlovsdogCreateNumText.text = $"{Pavlovsdogs.createDogNum}";

                var untargetablePlayers = new List<PlayerControl>();
                if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
                Pavlovsdogs.currentTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(Pavlovsdogs.currentTarget, Palette.ImpostorRed);

                // Show now text since the button already says sidekick
                showTargetNameOnButton(Pavlovsdogs.currentTarget, pavlovsownerCreateDogButton, GetString("pavlovsCreateDogText"));
                return Pavlovsdogs.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { pavlovsownerCreateDogButton.Timer = pavlovsownerCreateDogButton.MaxTimer; },
            Pavlovsdogs.CreateDogButton,
            ButtonPositions.upperRowCenter,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("pavlovsCreateDogText")
        );

        PavlovsdogCreateNumText = Object.Instantiate(pavlovsownerCreateDogButton.actionButton.cooldownTimerText,
            pavlovsownerCreateDogButton.actionButton.cooldownTimerText.transform.parent);
        PavlovsdogCreateNumText.text = "";
        PavlovsdogCreateNumText.enableWordWrapping = false;
        PavlovsdogCreateNumText.transform.localScale = Vector3.one * 0.5f;
        PavlovsdogCreateNumText.transform.localPosition += new Vector3(-0.05f, 0.55f, -1f);


        minerMineButton = new CustomButton(
            () =>
            {
                /* On Use */
                minerMineButton.Timer = minerMineButton.MaxTimer;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.Mine, SendOption.Reliable);
                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var id = getAvailableId();
                writer.Write(id);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);


                writer.WriteBytesAndSize(buff);


                writer.Write(0.01f);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.Mine(id, buff, 0.01f);
            },
            () =>
            {
                /* Can See */
                return Miner.miner != null && Miner.miner == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* Can Use */
                var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position,
                    Miner.VentSize, 0);
                hits = hits.ToArray().Where(c =>
                        (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                    .ToArray();
                return hits.Count == 0 && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                /* On Meeting End */
                minerMineButton.Timer = minerMineButton.MaxTimer;
            },
            Miner.buttonSprite,
            ButtonPositions.upperRowLeft, //brb
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("minerText")
        );

        bomberBombButton = new CustomButton(
            () =>
            {
                /* On Use */
                if (checkAndDoVetKill(Bomber.currentTarget)) return;
                var bombWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.GiveBomb, SendOption.Reliable);
                bombWriter.Write(Bomber.currentTarget.PlayerId);
                bombWriter.Write(false);
                AmongUsClient.Instance.FinishRpcImmediately(bombWriter);
                RPCProcedure.giveBomb(Bomber.currentTarget.PlayerId);
                if (Bomber.triggerBothCooldowns)
                {
                    Bomber.bomber.killTimer = bomberBombButton.MaxTimer * multiplier;
                }
                bomberBombButton.Timer = bomberBombButton.MaxTimer;
            },
            () =>
            {
                /* Can See */
                return Bomber.bomber != null && Bomber.bomber == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* On Click */
                Bomber.currentTarget = SetTarget();
                if (Bomber.hasBombPlayer == null) SetPlayerOutline(Bomber.currentTarget, Bomber.color);
                return Bomber.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                /* On Meeting End */
                bomberBombButton.Timer = bomberBombButton.MaxTimer;
                bomberBombButton.isEffectActive = false;
                bomberBombButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Bomber.hasBombPlayer = null;
            },
            Bomber.buttonSprite,
            ButtonPositions.upperRowLeft, //brb
            __instance,
            abilityInput.keyCode,
            buttonText: "giveBombText".Translate()
        );

        bomberGiveButton = new CustomButton(
            () =>
            {
                /* On Use */
                if (!Bomber.canGiveToBomber && Bomber.currentBombTarget == Bomber.bomber)
                {
                    var killWriter = StartRPC(CustomRPC.UncheckedMurderPlayer);
                    killWriter.Write(Bomber.bomber.Data.PlayerId);
                    killWriter.Write(Bomber.hasBombPlayer.Data.PlayerId);
                    killWriter.Write(0);
                    killWriter.EndRPC();
                    RPCProcedure.uncheckedMurderPlayer(Bomber.bomber.Data.PlayerId, Bomber.hasBombPlayer.Data.PlayerId, 0);

                    var clearWriter = StartRPC(CustomRPC.GiveBomb);
                    clearWriter.Write(byte.MaxValue);
                    clearWriter.Write(false);
                    clearWriter.EndRPC();
                    RPCProcedure.giveBomb(byte.MaxValue);
                    return;
                }

                if (checkAndDoVetKill(Bomber.currentBombTarget)) return;
                if (Bomber.hotPotatoMode)
                {
                    var bombWriter = StartRPC(CustomRPC.GiveBomb);
                    bombWriter.Write(Bomber.currentBombTarget.PlayerId);
                    bombWriter.Write(true);
                    bombWriter.EndRPC();
                    RPCProcedure.giveBomb(Bomber.currentBombTarget.PlayerId, true);
                }
                else
                {
                    if (checkMurderAttemptAndKill(Bomber.hasBombPlayer, Bomber.currentBombTarget) == MurderAttemptResult.SuppressKill) return;
                    var bombWriter = StartRPC(CustomRPC.GiveBomb);
                    bombWriter.Write(byte.MaxValue);
                    bombWriter.Write(false);
                    bombWriter.EndRPC();
                    RPCProcedure.giveBomb(byte.MaxValue);
                }
            },
            () =>
            {
                /* Can See */
                return Bomber.bomber != null && Bomber.hasBombPlayer == PlayerControl.LocalPlayer &&
                       Bomber.bombActive && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* Can Click */
                Bomber.currentBombTarget = SetTarget();
                if (Bomber.hasBombPlayer == null) SetPlayerOutline(Bomber.currentTarget, Bomber.color);
                return Bomber.currentBombTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                /* On Meeting End */
            },
            Bomber.buttonSprite,
            //          0, -0.06f, 0
            new Vector3(-4.5f, 1.5f, 0),
            __instance,
            hotkey: null,
            buttonText: "giveBombText".Translate()
        );

        grenadierFlashButton = new CustomButton(
            () =>
            {
                /* On Use */
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.GrenadierFlash, SendOption.Reliable);
                writer.Write(false);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.grenadierFlash(false);
            },
            () =>
            {
                /* Can See */
                return Grenadier.Player != null && Grenadier.Player == PlayerControl.LocalPlayer
                       && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* On Click */

                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    if (task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor
                        || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.StopCharles
                        || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                        return false;
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                grenadierFlashButton.Timer = grenadierFlashButton.MaxTimer;
                grenadierFlashButton.isEffectActive = false;
                grenadierFlashButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Grenadier.ButtonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            true,
            Grenadier.duration,
            () =>
            {
                grenadierFlashButton.Timer = grenadierFlashButton.MaxTimer;
            },
            buttonText: GetString("FlashButton")
        );

        // Werewolf Kill
        werewolfKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Werewolf.currentTarget)) return;
                if (checkMurderAttemptAndKill(Werewolf.werewolf, Werewolf.currentTarget) ==
                    MurderAttemptResult.SuppressKill) return;

                werewolfKillButton.Timer = werewolfKillButton.MaxTimer;
                Werewolf.currentTarget = null;
            },
            () =>
            {
                return Werewolf.werewolf != null && Werewolf.werewolf == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead && Werewolf.canKill;
            },
            () =>
            {
                Werewolf.currentTarget = SetTarget();
                showTargetNameOnButton(Werewolf.currentTarget, werewolfKillButton, GetString("killButtonText"));
                return Werewolf.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { werewolfKillButton.Timer = werewolfKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        werewolfRampageButton = new CustomButton(
            () =>
            {
                Werewolf.canKill = true;
                Werewolf.hasImpostorVision = true;
                werewolfKillButton.Timer = 0f;
                werewolfRampageButton.PositionOffset = new Vector3(0, 1.92f, 0);
            },
            () =>
            {
                /* Can See */
                return Werewolf.werewolf != null && Werewolf.werewolf == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* On Click */
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                /* On Meeting End */
                werewolfRampageButton.Timer = werewolfRampageButton.MaxTimer;
                werewolfRampageButton.isEffectActive = false;
                werewolfRampageButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Werewolf.canKill = false;
                //  Werewolf.canUseVents = false;
                Werewolf.hasImpostorVision = false;
                werewolfRampageButton.PositionOffset = ButtonPositions.upperRowRight;
            },
            Werewolf.buttonSprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            true,
            Werewolf.rampageDuration,
            () =>
            {
                werewolfRampageButton.PositionOffset = ButtonPositions.upperRowRight;
                werewolfRampageButton.Timer = werewolfRampageButton.MaxTimer;
                Werewolf.canKill = false;
                Werewolf.hasImpostorVision = false;
            },
            buttonText: "WerewolfRampage".Translate()
        );

        // 天启击杀 Kill
        juggernautKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Juggernaut.currentTarget)) return;
                if (checkMurderAttemptAndKill(Juggernaut.juggernaut, Juggernaut.currentTarget) ==
                    MurderAttemptResult.SuppressKill) return;

                Juggernaut.cooldown = Mathf.Max(0, Juggernaut.cooldown - Juggernaut.reducedkill);
                juggernautKillButton.MaxTimer = Juggernaut.cooldown;

                juggernautKillButton.Timer = juggernautKillButton.MaxTimer;
                Juggernaut.currentTarget = null;
            },
            () =>
            {
                return Juggernaut.juggernaut != null &&
                       Juggernaut.juggernaut == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                Juggernaut.currentTarget = SetTarget();
                showTargetNameOnButton(Juggernaut.currentTarget, juggernautKillButton, GetString("killButtonText"));
                return Juggernaut.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { juggernautKillButton.Timer = juggernautKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        // 鹈鹕击杀 Kill
        pelicanKillButton = new CustomButton(
            () =>
            {
                if (Pelican.currentTarget == null) return;
                if (checkAndDoVetKill(Pelican.currentTarget)) return;
                var murderAttemptResult = checkMuderAttempt(Pelican.Player, Pelican.currentTarget);
                if (murderAttemptResult == MurderAttemptResult.SuppressKill) return;

                if (murderAttemptResult == MurderAttemptResult.PerformKill)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.PelicanKill);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(Pelican.currentTarget.PlayerId);
                    writer.EndRPC();
                    Pelican.PelicanKill(PlayerControl.LocalPlayer.PlayerId, Pelican.currentTarget.PlayerId);
                }
                if (murderAttemptResult == MurderAttemptResult.BodyGuardKill)
                    checkMurderAttemptAndKill(Pelican.Player, Pelican.currentTarget);

                pelicanKillButton.Timer = Pelican.reduceCooldown;
                Pelican.currentTarget = null;
            },
            () =>
            {
                return Pelican.Player != null && Pelican.Player == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                var untargetablePlayers = new List<PlayerControl>();
                if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
                Pelican.currentTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(Pelican.currentTarget, Palette.ImpostorRed);

                showTargetNameOnButton(Pelican.currentTarget, pelicanKillButton, GetString("VultureText"));
                return Pelican.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                //pelicanKillButton.MaxTimer = Pelican.cooldown;
                pelicanKillButton.Timer = pelicanKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            buttonText: GetString("VultureText")
        );

        // Eraser erase button
        eraserButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Eraser.currentTarget)) return;
                eraserButton.MaxTimer += 10;
                eraserButton.Timer = eraserButton.MaxTimer;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetFutureErased, SendOption.Reliable);
                writer.Write(Eraser.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setFutureErased(Eraser.currentTarget.PlayerId);
                SoundEffectsManager.play("eraserErase");
            },
            () =>
            {
                return Eraser.eraser != null && Eraser.eraser == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                var untargetables = new List<PlayerControl>();
                if (Spy.spy != null) untargetables.Add(Spy.spy);
                Eraser.currentTarget = SetTarget(untarget: Eraser.canEraseAnyone ? [] : untargetables, !Eraser.canEraseAnyone);
                SetPlayerOutline(Eraser.currentTarget, Eraser.color);

                showTargetNameOnButton(Eraser.currentTarget, eraserButton, GetString("EraserText"));
                return PlayerControl.LocalPlayer.CanMove && Eraser.currentTarget != null;
            },
            () => { eraserButton.Timer = eraserButton.MaxTimer; },
            Eraser.buttonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode
        );

        partTimerButton = new CustomButton(
            () =>
            {
                if (PartTimer.currentTarget == null) return;
                if (checkAndDoVetKill(PartTimer.currentTarget)) return;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.PartTimerSet, SendOption.Reliable);
                writer.Write(PartTimer.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.partTimerSet(PartTimer.currentTarget.PlayerId);
                SoundEffectsManager.play("jackalSidekick");

                partTimerButton.Timer = partTimerButton.MaxTimer;
            },
            () =>
            {
                return PartTimer.partTimer != null && PartTimer.partTimer == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead && PartTimer.target == null;
            },
            () =>
            {
                PartTimer.currentTarget = SetTarget();
                if (PartTimer.target != null) SetPlayerOutline(PartTimer.currentTarget, PartTimer.color);

                showTargetNameOnButton(PartTimer.currentTarget, partTimerButton, GetString("partTimerButton"));
                return PlayerControl.LocalPlayer.CanMove && PartTimer.currentTarget != null; ;
            },
            () => { partTimerButton.Timer = partTimerButton.MaxTimer; },
            PartTimer.buttonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("partTimerButton")
        );


        placeJackInTheBoxButton = new CustomButton(
            () =>
            {
                placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.PlaceJackInTheBox);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.placeJackInTheBox(buff);
                SoundEffectsManager.play("tricksterPlaceBox");
            },
            () =>
            {
                return Trickster.trickster != null && Trickster.trickster == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead && !JackInTheBox.hasJackInTheBoxLimitReached();
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && !JackInTheBox.hasJackInTheBoxLimitReached();
            },
            () => { placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer; },
            Trickster.placeBoxButtonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("TricksterPlaceText")
        );

        lightsOutButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.LightsOut, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.lightsOut();
                SoundEffectsManager.play("lighterLight");
            },
            () =>
            {
                return Trickster.trickster != null && Trickster.trickster == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead
                       && JackInTheBox.hasJackInTheBoxLimitReached() && JackInTheBox.boxesConvertedToVents;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && JackInTheBox.hasJackInTheBoxLimitReached() &&
                       JackInTheBox.boxesConvertedToVents;
            },
            () =>
            {
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
                lightsOutButton.isEffectActive = false;
                lightsOutButton.actionButton.graphic.color = Palette.EnabledColor;
            },
            Trickster.lightOutButtonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            true,
            Trickster.lightsOutDuration,
            () =>
            {
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
                SoundEffectsManager.play("lighterLight");
            },
            buttonText: GetString("LightsOutText")
        );

        // Cleaner Clean
        cleanerCleanButton = new CustomButton(
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             PlayerControl.LocalPlayer.GetTruePosition(),
                             PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                            var truePosition2 = component.TruePosition;
                            if (Vector2.Distance(truePosition2, truePosition) <=
                                PlayerControl.LocalPlayer.MaxReportDistance &&
                                PlayerControl.LocalPlayer.CanMove &&
                                !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                    Constants.ShipAndObjectsMask, false))
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                var writer = AmongUsClient.Instance.StartRpcImmediately(
                                    PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CleanBody,
                                    SendOption.Reliable);
                                writer.Write(playerInfo.PlayerId);
                                writer.Write(Cleaner.cleaner.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                RPCProcedure.cleanBody(playerInfo.PlayerId, Cleaner.cleaner.PlayerId);

                                Cleaner.cleaner.killTimer = cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
                                SoundEffectsManager.play("cleanerClean");
                                break;
                            }
                        }
                    }
            },
            () =>
            {
                return Cleaner.cleaner != null && Cleaner.cleaner == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                return __instance.ReportButton.graphic.color == Palette.EnabledColor &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer; },
            Cleaner.buttonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("CleanText")
        );

        // Butcher Dissection
        butcherDissectionButton = new CustomButton(
            () => { },
            () =>
            {
                return Butcher.butcher != null && Butcher.butcher == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead && Butcher.canDissection;
            },
            () =>
            {
                return __instance.ReportButton.graphic.color == Palette.EnabledColor &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { butcherDissectionButton.Timer = butcherDissectionButton.MaxTimer; },
            Butcher.ButtonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            true,
            Butcher.dissectionDuration,
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             PlayerControl.LocalPlayer.GetTruePosition(),
                             PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                            var truePosition2 = component.TruePosition;
                            if (Vector2.Distance(truePosition2, truePosition) <=
                                PlayerControl.LocalPlayer.MaxReportDistance &&
                                PlayerControl.LocalPlayer.CanMove &&
                                !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                    Constants.ShipAndObjectsMask, false))
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                    (byte)CustomRPC.DissectionBody, SendOption.Reliable);
                                writer.Write(playerInfo.PlayerId);
                                writer.Write(Butcher.butcher.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                RPCProcedure.dissectionBody(playerInfo.PlayerId, Butcher.butcher.PlayerId);

                                Butcher.canDissection = false;
                                SoundEffectsManager.play("cleanerClean");
                                break;
                            }
                        }
                    }
            },
            buttonText: GetString("DissectionText")
        );

        // Undertaker Button
        undertakerDragButton = new CustomButton(
            () =>
            {
                if (Undertaker.deadBodyDraged == null)
                {
                    foreach (var collider2D in Physics2D.OverlapCircleAll(
                                 PlayerControl.LocalPlayer.GetTruePosition(),
                                 PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                        if (collider2D.tag == "DeadBody")
                        {
                            var deadBody = collider2D.GetComponent<DeadBody>();
                            if (deadBody && !deadBody.Reported)
                            {
                                var playerPosition = PlayerControl.LocalPlayer.GetTruePosition();
                                var deadBodyPosition = deadBody.TruePosition;
                                if (Vector2.Distance(deadBodyPosition, playerPosition) <=
                                    PlayerControl.LocalPlayer.MaxReportDistance &&
                                    PlayerControl.LocalPlayer.CanMove &&
                                    !PhysicsHelpers.AnythingBetween(playerPosition, deadBodyPosition,
                                        Constants.ShipAndObjectsMask, false) && !Undertaker.isDraging)
                                {
                                    var playerInfo = GameData.Instance.GetPlayerById(deadBody.ParentId);
                                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.DragBody);
                                    writer.Write(playerInfo.PlayerId);
                                    writer.Write(true);
                                    writer.EndRPC();
                                    RPCProcedure.dragBody(playerInfo.PlayerId, true);
                                    Undertaker.deadBodyDraged = deadBody;
                                    break;
                                }
                            }
                        }
                }
                else
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.DragBody);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(false);
                    writer.EndRPC();
                    RPCProcedure.dragBody(PlayerControl.LocalPlayer.PlayerId, false);
                    Undertaker.deadBodyDraged = null;
                }
            },
            () =>
            {
                return Undertaker.undertaker != null &&
                       Undertaker.undertaker == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (Undertaker.deadBodyDraged != null) return true;

                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             PlayerControl.LocalPlayer.GetTruePosition(),
                             PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var deadBody = collider2D.GetComponent<DeadBody>();
                        var deadBodyPosition = deadBody.TruePosition;
                        deadBodyPosition.x -= 0.2f;
                        deadBodyPosition.y -= 0.2f;
                        return PlayerControl.LocalPlayer.CanMove &&
                               Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(),
                                   deadBodyPosition) < 0.80f;
                    }

                return false;
            },
            //() => { return ((__instance.ReportButton.renderer.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove) || Undertaker.deadBodyDraged != null); },
            () => { },
            Undertaker.buttonSprite,
            ButtonPositions.upperRowLeft, //brb
            __instance,
            abilityInput.keyCode,
            true,
            0f,
            () => { },
            buttonText: GetString("DragBodyText")
        );

        // Warlock curse
        warlockCurseButton = new CustomButton(
            () =>
            {
                if (Warlock.curseVictim == null)
                {
                    if (checkAndDoVetKill(Warlock.currentTarget)) return;
                    // Apply Curse
                    Warlock.curseVictim = Warlock.currentTarget;
                    warlockCurseButton.Sprite = Warlock.curseKillButtonSprite;
                    warlockCurseButton.Timer = 1f;
                    SoundEffectsManager.play("warlockCurse");

                    // Ghost Info
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo,
                        SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.WarlockTarget);
                    writer.Write(Warlock.curseVictim.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (Warlock.curseVictim != null && Warlock.curseVictimTarget != null)
                {
                    var murder = checkMurderAttemptAndKill(Warlock.warlock, Warlock.curseVictimTarget,
                        showAnimation: false);
                    if (murder == MurderAttemptResult.SuppressKill) return;

                    // If blanked or killed
                    if (Warlock.rootTime > 0)
                    {
                        AntiTeleport.position = PlayerControl.LocalPlayer.transform.position;
                        PlayerControl.LocalPlayer.moveable = false;
                        PlayerControl.LocalPlayer.NetTransform
                            .Halt(); // Stop current movement so the warlock is not just running straight into the next object
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Warlock.rootTime,
                            new Action<float>(p =>
                            {
                                // Delayed action
                                if (p == 1f) PlayerControl.LocalPlayer.moveable = true;
                            })));
                    }

                    Warlock.curseVictim = null;
                    Warlock.curseVictimTarget = null;
                    warlockCurseButton.Sprite = Warlock.curseButtonSprite;
                    Warlock.warlock.killTimer = warlockCurseButton.Timer = warlockCurseButton.MaxTimer;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo,
                        SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.WarlockTarget);
                    writer.Write(byte.MaxValue); // This will set it to null!
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            },
            () =>
            {
                return Warlock.warlock != null && Warlock.warlock == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {

                // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
                if (Warlock.curseVictim != null && (Warlock.curseVictim.Data.Disconnected || Warlock.curseVictim.Data.IsDead))
                    Warlock.resetCurse();
                if (Warlock.curseVictim == null)
                {
                    Warlock.currentTarget = SetTarget();
                    SetPlayerOutline(Warlock.currentTarget, Warlock.color);
                }
                else
                {
                    Warlock.curseVictimTarget = SetTarget(targetingPlayer: Warlock.curseVictim);
                    SetPlayerOutline(Warlock.curseVictimTarget, Warlock.color);
                }

                if (Warlock.curseVictim != null)
                    showTargetNameOnButton(Warlock.currentTarget, warlockCurseButton, GetString("CurseKillText"));
                else
                    showTargetNameOnButton(Warlock.currentTarget, warlockCurseButton, GetString("CurseText"));
                return (Warlock.curseVictim == null && Warlock.currentTarget != null ||
                        Warlock.curseVictim != null && Warlock.curseVictimTarget != null) &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
                warlockCurseButton.Sprite = Warlock.curseButtonSprite;
                Warlock.curseVictim = null;
                Warlock.curseVictimTarget = null;
            },
            Warlock.curseButtonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("CurseText")
        );

        // Security Guard button
        securityGuardButton = new CustomButton(
            () =>
            {
                if (SecurityGuard.ventTarget != null)
                {
                    // Seal vent
                    var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SealVent);
                    writer.WritePacked(SecurityGuard.ventTarget.Id);
                    writer.EndMessage();
                    RPCProcedure.sealVent(SecurityGuard.ventTarget.Id);
                    SecurityGuard.ventTarget = null;
                }
                else if (!isMira && !isFungle && !SubmergedCompatibility.IsSubmerged)
                {
                    // Place camera if there's no vent and it's not MiraHQ or Submerged
                    var pos = PlayerControl.LocalPlayer.transform.position;
                    var buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.PlaceCamera);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placeCamera(buff);
                }

                SoundEffectsManager.play("securityGuardPlaceCam"); // Same sound used for both types (cam or vent)!
                securityGuardButton.Timer = securityGuardButton.MaxTimer;
            },
            () =>
            {
                return SecurityGuard.securityGuard.IsAlive() && SecurityGuard.securityGuard == PlayerControl.LocalPlayer &&
                       SecurityGuard.remainingScrews >= Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice);
            },
            () =>
            {
                securityGuardButton.actionButton.graphic.sprite =
                    SecurityGuard.ventTarget == null && !isMira && !isFungle &&
                    !SubmergedCompatibility.IsSubmerged
                        ? SecurityGuard.placeCameraButtonSprite
                        : SecurityGuard.closeVentButtonSprite;
                if (securityGuardButtonScrewsText != null)
                    securityGuardButtonScrewsText.text = $"{SecurityGuard.remainingScrews}/{SecurityGuard.totalScrews}";


                Vent target = null;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var closestDistance = float.MaxValue;
                for (var i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++)
                {
                    var vent = MapUtilities.CachedShipStatus.AllVents[i];
                    if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") ||
                        vent.gameObject.name.StartsWith("SealedVent_") ||
                        vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
                    if (SubmergedCompatibility.IsSubmerged && vent.Id == 9) continue; // cannot seal submergeds exit only vent!
                    var distance = Vector2.Distance(vent.transform.position, truePosition);
                    if (distance <= vent.UsableDistance && distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = vent;
                    }
                }

                SecurityGuard.ventTarget = target;

                if (SecurityGuard.ventTarget != null)
                    return SecurityGuard.remainingScrews >= SecurityGuard.ventPrice &&
                           PlayerControl.LocalPlayer.CanMove;
                return !isMira && !isFungle && !SubmergedCompatibility.IsSubmerged &&
                       SecurityGuard.remainingScrews >= SecurityGuard.camPrice &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { securityGuardButton.Timer = securityGuardButton.MaxTimer; },
            SecurityGuard.placeCameraButtonSprite,
            ButtonPositions.lowerRowRight,
            __instance,
            abilityInput.keyCode
        );

        // Security Guard button screws counter
        securityGuardButtonScrewsText = Object.Instantiate(securityGuardButton.actionButton.cooldownTimerText,
            securityGuardButton.actionButton.cooldownTimerText.transform.parent);
        securityGuardButtonScrewsText.text = "";
        securityGuardButtonScrewsText.enableWordWrapping = false;
        securityGuardButtonScrewsText.transform.localScale = Vector3.one * 0.5f;
        securityGuardButtonScrewsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        securityGuardCamButton = new CustomButton(
            () =>
            {
                if (!isMira)
                {
                    if (SecurityGuard.minigame == null)
                    {
                        var mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                        var e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("Surv_Panel") || x.name.Contains("Cam") ||
                            x.name.Contains("BinocularsSecurityConsole"));
                        if (isSkeld || mapId == 3)
                            e = Object.FindObjectsOfType<SystemConsole>()
                                .FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
                        else if (isAirship)
                            e = Object.FindObjectsOfType<SystemConsole>()
                                .FirstOrDefault(x => x.gameObject.name.Contains("task_cams"));
                        if (e == null || Camera.main == null) return;
                        SecurityGuard.minigame = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    SecurityGuard.minigame.transform.SetParent(Camera.main.transform, false);
                    SecurityGuard.minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    SecurityGuard.minigame.Begin(null);
                }
                else
                {
                    if (SecurityGuard.minigame == null)
                    {
                        var e = Object.FindObjectsOfType<SystemConsole>()
                            .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        SecurityGuard.minigame = Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    SecurityGuard.minigame.transform.SetParent(Camera.main.transform, false);
                    SecurityGuard.minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    SecurityGuard.minigame.Begin(null);
                }

                SecurityGuard.charges--;

                if (SecurityGuard.cantMove) PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
            },
            () =>
            {
                return SecurityGuard.securityGuard != null &&
                       SecurityGuard.securityGuard == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead && SecurityGuard.remainingScrews <
                       Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice)
                       && !SubmergedCompatibility.IsSubmerged;
            },
            () =>
            {
                if (securityGuardChargesText != null)
                    securityGuardChargesText.text = $"{SecurityGuard.charges} / {SecurityGuard.maxCharges}";
                securityGuardCamButton.actionButton.graphic.sprite =
                    isMira ? SecurityGuard.getLogSprite() : SecurityGuard.getCamSprite();
                securityGuardCamButton.actionButton.OverrideText(isMira ?
                    GetString("hackerDoorLogText") : GetString("CamButtonText"));
                return PlayerControl.LocalPlayer.CanMove && SecurityGuard.charges > 0;
            },
            () =>
            {
                securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                securityGuardCamButton.isEffectActive = false;
                securityGuardCamButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            SecurityGuard.getCamSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            abilityInput.keyCode,
            true,
            0f,
            () =>
            {
                securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                if (Minigame.Instance) SecurityGuard.minigame.ForceClose();
                PlayerControl.LocalPlayer.moveable = true;
            },
            false,
            isMira ? GetString("hackerDoorLogText") : GetString("CamButtonText")
        );

        // Security Guard cam button charges
        securityGuardChargesText = Object.Instantiate(securityGuardCamButton.actionButton.cooldownTimerText,
            securityGuardCamButton.actionButton.cooldownTimerText.transform.parent);
        securityGuardChargesText.text = "";
        securityGuardChargesText.enableWordWrapping = false;
        securityGuardChargesText.transform.localScale = Vector3.one * 0.5f;
        securityGuardChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        // Arsonist button (涂油)
        arsonistButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Arsonist.currentTarget)) return;
                Arsonist.douseTarget = Arsonist.currentTarget;
                arsonistButton.HasEffect = true;
                SoundEffectsManager.play("arsonistDouse");
            },
            () =>
            {
                return Arsonist.arsonist != null && Arsonist.arsonist == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                List<PlayerControl> untargetables;
                if (Arsonist.douseTarget != null)
                {
                    untargetables = new();
                    foreach (var cachedPlayer in PlayerControl.AllPlayerControls)
                        if (cachedPlayer.PlayerId != Arsonist.douseTarget.PlayerId)
                            untargetables.Add(cachedPlayer);
                }
                else
                {
                    untargetables = Arsonist.dousedPlayers;
                }

                Arsonist.currentTarget = SetTarget(untarget: untargetables, distances: 0.5f);
                if (Arsonist.currentTarget != null) SetPlayerOutline(Arsonist.currentTarget, Arsonist.color);

                showTargetNameOnButton(Arsonist.currentTarget, arsonistButton, GetString("DouseText"));

                if (arsonistButton.isEffectActive && Arsonist.douseTarget != Arsonist.currentTarget)
                {
                    Arsonist.douseTarget = null;
                    arsonistButton.Timer = 0f;
                    arsonistButton.isEffectActive = false;
                }

                return PlayerControl.LocalPlayer.CanMove && Arsonist.currentTarget != null;
            },
            () =>
            {
                arsonistButton.Timer = arsonistButton.MaxTimer;
                arsonistButton.isEffectActive = false;
                Arsonist.douseTarget = null;
            },
            Arsonist.douseSprite,
            ButtonPositions.upperRowRight,
            __instance,
            abilityInput.keyCode,
            true,
            Arsonist.duration,
            () =>
            {
                if (Arsonist.douseTarget != null) Arsonist.dousedPlayers.Add(Arsonist.douseTarget);

                arsonistButton.Timer = arsonistButton.MaxTimer;
                arsonistKillButton.Timer = arsonistKillButton.MaxTimer == 0 ? 1f : arsonistKillButton.MaxTimer;
                foreach (var p in Arsonist.dousedPlayers)
                    if (ModOption.playerIcons.ContainsKey(p.PlayerId))
                        ModOption.playerIcons[p.PlayerId].setSemiTransparent(false);

                // Ghost Info
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.ArsonistDouse);
                writer.Write(Arsonist.douseTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                Arsonist.douseTarget = null;
            },
            buttonText: GetString("DouseText")
        );

        // Arsonist button (点火)
        arsonistKillButton = new CustomButton(
            () =>
            {
                foreach (PlayerControl p in Arsonist.dousedPlayers.Where(p => p.IsAlive()))
                {
                    MurderPlayer(Arsonist.arsonist, p, false);
                    GameHistory.RpcOverrideDeathReasonAndKiller(p, CustomDeathReason.Arson, Arsonist.arsonist);
                }
                arsonistKillButton.Timer = arsonistKillButton.MaxTimer;
                arsonistButton.Timer = arsonistButton.MaxTimer;
            },
            () =>
            {
                return Arsonist.arsonist != null && Arsonist.arsonist == PlayerControl.LocalPlayer &&
                       PlayerControl.LocalPlayer.IsAlive() && Arsonist.dousedPlayers.Count > 0;
            },
            () =>
            {
                Arsonist.currentTarget2 = SetTarget(distances: 0.5f);
                var cankill = false;
                if (Arsonist.currentTarget2 && Arsonist.dousedPlayers.Any(x => x == Arsonist.currentTarget2))
                {
                    SetPlayerOutline(Arsonist.currentTarget2, Arsonist.color);
                    showTargetNameOnButton(Arsonist.currentTarget2, arsonistKillButton, GetString("IgniteText"));
                    cankill = true;
                }

                return PlayerControl.LocalPlayer.CanMove && cankill;
            },
            () =>
            {
                var count = PlayerControl.AllPlayerControls.ToList().Count(p => p.IsAlive() && p.IsKiller() && p != Arsonist.arsonist);

                if (count == 0 && Arsonist.igniteCooldownRemoved) arsonistKillButton.Timer = arsonistKillButton.MaxTimer = 0f;
                else arsonistKillButton.Timer = arsonistKillButton.MaxTimer = arsonistButton.MaxTimer;
            },
            Arsonist.igniteSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            modKillInput.keyCode,
            buttonText: GetString("IgniteText")
        );

        // Vulture Eat
        vultureEatButton = new CustomButton(
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             PlayerControl.LocalPlayer.GetTruePosition(),
                             PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                            var truePosition2 = component.TruePosition;
                            if (Vector2.Distance(truePosition2, truePosition) <=
                                PlayerControl.LocalPlayer.MaxReportDistance &&
                                PlayerControl.LocalPlayer.CanMove &&
                                !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                    Constants.ShipAndObjectsMask, false))
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                var writer = AmongUsClient.Instance.StartRpcImmediately(
                                    PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CleanBody,
                                    SendOption.Reliable);
                                writer.Write(playerInfo.PlayerId);
                                writer.Write(Vulture.vulture.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                RPCProcedure.cleanBody(playerInfo.PlayerId, Vulture.vulture.PlayerId);

                                Vulture.cooldown = vultureEatButton.Timer = vultureEatButton.MaxTimer;
                                SoundEffectsManager.play("vultureEat");
                                break;
                            }
                        }
                    }
            },
            () =>
            {
                return Vulture.vulture != null && Vulture.vulture == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                return __instance.ReportButton.graphic.color == Palette.EnabledColor &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { vultureEatButton.Timer = vultureEatButton.MaxTimer; },
            Vulture.buttonSprite,
            ButtonPositions.lowerRowCenter,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("VultureText")
        );

        amnisiacRememberButton = new CustomButton(
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             PlayerControl.LocalPlayer.GetTruePosition(),
                             PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                            var truePosition2 = component.TruePosition;
                            if (Vector2.Distance(truePosition2, truePosition) <=
                                PlayerControl.LocalPlayer.MaxReportDistance &&
                                PlayerControl.LocalPlayer.CanMove &&
                                !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                    Constants.ShipAndObjectsMask, false))
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.AmnisiacTakeRole);
                                writer.Write(playerInfo.PlayerId);
                                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                                writer.EndRPC();
                                Amnisiac.TakeRole(playerInfo.PlayerId, PlayerControl.LocalPlayer.PlayerId);
                                break;
                            }
                        }
                    }
            },
            () =>
            {
                return Amnisiac.Player != null && Amnisiac.Player.Any(x => x == PlayerControl.LocalPlayer) &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                return __instance.ReportButton.graphic.color == Palette.EnabledColor &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { amnisiacRememberButton.Timer = 0f; },
            Amnisiac.buttonSprite,
            ButtonPositions.lowerRowRight, //brb
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("RememberText")
        );

        specterRememberButton = new CustomButton(
            () => { },
            () =>
            {
                return Specter.Player != null && Specter.Player == PlayerControl.LocalPlayer &&
                       PlayerControl.LocalPlayer.Data.IsDead && Specter.remember && !Specter.revived;
            },
            () =>
            {
                var array = Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                      PlayerControl.LocalPlayer.MaxReportDistance * 0.36f,
                      Constants.PlayersOnlyMask).Where(collider => collider.tag == "DeadBody")
                 .Select(collider => collider.GetComponent<DeadBody>())
                 .Where(deadBody => deadBody != null);

                return array.Any(db => db.ParentId != PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                specterRememberButton.Timer = 10f;
            },
            Amnisiac.buttonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            secondaryAbilityInput.keyCode,
            true,
            Specter.duration,
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                             PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                            var truePosition2 = component.TruePosition;
                            if (Vector2.Distance(truePosition2, truePosition) <=
                                PlayerControl.LocalPlayer.MaxReportDistance &&
                                PlayerControl.LocalPlayer.CanMove &&
                                !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                    Constants.ShipAndObjectsMask, false) && component.ParentId != PlayerControl.LocalPlayer.PlayerId)
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);
                                if (!Specter.afterMeetingRevive) PlayerControl.LocalPlayer.transform.position = PlayerControl.LocalPlayer.GetCloseSpawnPosition();
                                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.SpecterTakeRole);
                                writer.Write(playerInfo.PlayerId);
                                writer.EndRPC();
                                Specter.TakeRole(playerInfo.PlayerId);
                                break;
                            }
                        }
                    }
            },
            buttonText: GetString("ReviveButton")
        );

        // Medium button
        mediumButton = new CustomButton(
            () =>
            {
                if (Medium.target != null)
                {
                    Medium.soulTarget = Medium.target;
                    mediumButton.HasEffect = true;
                    SoundEffectsManager.play("mediumAsk");
                }
            },
            () =>
            {
                return Medium.medium.IsAlive() && Medium.medium == PlayerControl.LocalPlayer;
            },
            () =>
            {

                DeadPlayer target = null;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var closestDistance = float.MaxValue;
                var usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
                foreach (var (dp, ps) in Medium.deadBodies)
                {
                    var distance = Vector2.Distance(ps, truePosition);
                    if (distance <= usableDistance && distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = dp;
                    }
                }
                Medium.target = target;

                if (mediumButton.isEffectActive && Medium.target != Medium.soulTarget)
                {
                    Medium.soulTarget = null;
                    mediumButton.Timer = 0f;
                    mediumButton.isEffectActive = false;
                }

                return Medium.target != null && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                mediumButton.Timer = mediumButton.MaxTimer;
                mediumButton.isEffectActive = false;
                Medium.soulTarget = null;
            },
            Medium.question,
            ButtonPositions.lowerRowRight,
            __instance,
            abilityInput.keyCode,
            true,
            Medium.duration,
            () =>
            {
                mediumButton.Timer = mediumButton.MaxTimer;
                if (Medium.target == null || Medium.target.Player == null) return;
                var msg = Medium.getInfo(Medium.target.Player, Medium.target.KillerIfExisting);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);

                // Ghost Info
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                writer.Write(Medium.target.Player.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
                writer.Write(msg);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                // Remove soul
                if (Medium.oneTimeUse)
                {
                    var closestDistance = float.MaxValue;
                    SpriteRenderer target = null;

                    foreach (var (db, ps) in Medium.deadBodies)
                        if (db == Medium.target)
                        {
                            var deadBody = Tuple.Create(db, ps);
                            Medium.deadBodies.Remove(deadBody);
                            break;
                        }

                    foreach (var rend in Medium.souls)
                    {
                        var distance = Vector2.Distance(rend.transform.position,
                            PlayerControl.LocalPlayer.GetTruePosition());
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            target = rend;
                        }
                    }

                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>(p =>
                    {
                        if (target != null)
                        {
                            var tmp = target.color;
                            tmp.a = Mathf.Clamp01(1 - p);
                            target.color = tmp;
                        }

                        if (p == 1f && target != null && target.gameObject != null) Object.Destroy(target.gameObject);
                    })));

                    Medium.souls.Remove(target);
                }

                SoundEffectsManager.stop("mediumAsk");
            },
            buttonText: GetString("MediumText")
        );

        // Pursuer button
        pursuerButton = new CustomButton(
            () =>
            {
                if (Pursuer.target != null)
                {
                    if (checkAndDoVetKill(Pursuer.target)) return;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PursuerSetBlanked, SendOption.Reliable);
                    writer.Write(Pursuer.target.PlayerId);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.pursuerSetBlanked(Pursuer.target.PlayerId, byte.MaxValue);

                    Pursuer.target = null;

                    Pursuer.blanks++;
                    pursuerButton.Timer = pursuerButton.MaxTimer;
                    SoundEffectsManager.play("pursuerBlank");
                }
            },
            () =>
            {
                return Pursuer.Player != null && Pursuer.Player.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) &&
                       PlayerControl.LocalPlayer.IsAlive()/* && Pursuer.blanks < Pursuer.blanksNumber*/;
            },
            () =>
            {
                Pursuer.target = SetTarget();
                SetPlayerOutline(Pursuer.target, Pursuer.color);

                showTargetNameOnButton(Pursuer.target, pursuerButton, GetString("PursuerText"));
                if (pursuerButtonBlanksText != null)
                    pursuerButtonBlanksText.text = $"{Pursuer.blanksNumber - Pursuer.blanks}";

                return Pursuer.blanksNumber > Pursuer.blanks && PlayerControl.LocalPlayer.CanMove && Pursuer.target != null;
            },
            () => { pursuerButton.Timer = pursuerButton.MaxTimer; },
            Pursuer.buttonSprite,
            ButtonPositions.upperRowRight,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("PursuerText")
        );

        // Pursuer button blanks left
        pursuerButtonBlanksText = Object.Instantiate(pursuerButton.actionButton.cooldownTimerText,
            pursuerButton.actionButton.cooldownTimerText.transform.parent);
        pursuerButtonBlanksText.text = "";
        pursuerButtonBlanksText.enableWordWrapping = false;
        pursuerButtonBlanksText.transform.localScale = Vector3.one * 0.5f;
        pursuerButtonBlanksText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);



        survivorVestButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SurvivorVestActive, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.survivorVestActive();
                Survivor.vestUsed++;
            },
            () =>
            {
                return Survivor.Player != null && Survivor.Player.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) &&
                       PlayerControl.LocalPlayer.IsAlive() && Survivor.vestEnable/* && Survivor.remainingVests > 0*/;
            },
            () =>
            {
                if (survivorVestButtonText != null) survivorVestButtonText.text = $"{Survivor.remainingVests} / {Survivor.vestNumber}";
                return PlayerControl.LocalPlayer.CanMove && Survivor.remainingVests > 0;
            },
            () =>
            {
                survivorVestButton.Timer = survivorVestButton.MaxTimer;
                survivorVestButton.isEffectActive = false;
                survivorVestButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Survivor.VestButtonSprite,
            ButtonPositions.upperRowRight,
            __instance,
            abilityInput.keyCode,
            true,
            Survivor.vestDuration,
            () => { survivorVestButton.Timer = survivorVestButton.MaxTimer; },
            buttonText: GetString("VestButton")
        );
        // Pursuer button blanks left
        survivorVestButtonText = Object.Instantiate(survivorVestButton.actionButton.cooldownTimerText,
            survivorVestButton.actionButton.cooldownTimerText.transform.parent);
        survivorVestButtonText.text = "";
        survivorVestButtonText.enableWordWrapping = false;
        survivorVestButtonText.transform.localScale = Vector3.one * 0.5f;
        survivorVestButtonText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);


        // Survivor button
        survivorBlanksButton = new CustomButton(
            () =>
            {
                if (Survivor.target != null)
                {
                    if (checkAndDoVetKill(Survivor.target)) return;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PursuerSetBlanked, SendOption.Reliable);
                    writer.Write(Survivor.target.PlayerId);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.pursuerSetBlanked(Survivor.target.PlayerId, byte.MaxValue);

                    Survivor.target = null;

                    Survivor.blanksUsed++;
                    survivorBlanksButton.Timer = survivorBlanksButton.MaxTimer;
                    SoundEffectsManager.play("pursuerBlank");
                }
            },
            () =>
            {
                return Survivor.Player != null && Survivor.Player.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) &&
                       PlayerControl.LocalPlayer.IsAlive() && Survivor.blanksEnable/* && Survivor.remainingBlanks > 0*/;
            },
            () =>
            {
                Survivor.target = SetTarget();
                SetPlayerOutline(Survivor.target, Survivor.color);

                showTargetNameOnButton(Survivor.target, survivorBlanksButton, GetString("PursuerText"));
                if (survivorBlanksButtonText != null) survivorBlanksButtonText.text = $"{Survivor.remainingBlanks} / {Survivor.blanksNumber}";

                return Survivor.blanksNumber > Survivor.blanksUsed && PlayerControl.LocalPlayer.CanMove && Survivor.target != null;
            },
            () => { survivorBlanksButton.Timer = survivorBlanksButton.MaxTimer; },
            Pursuer.buttonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            KeyCode.C,
            buttonText: GetString("PursuerText")
        );
        // Pursuer button blanks left
        survivorBlanksButtonText = Object.Instantiate(survivorBlanksButton.actionButton.cooldownTimerText,
            survivorBlanksButton.actionButton.cooldownTimerText.transform.parent);
        survivorBlanksButtonText.text = "";
        survivorBlanksButtonText.enableWordWrapping = false;
        survivorBlanksButtonText.transform.localScale = Vector3.one * 0.5f;
        survivorBlanksButtonText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);


        // Witch Spell button
        witchSpellButton = new CustomButton(
            () =>
            {
                if (Witch.currentTarget != null)
                {
                    if (checkAndDoVetKill(Witch.currentTarget)) return;
                    Witch.spellCastingTarget = Witch.currentTarget;
                    SoundEffectsManager.play("witchSpell");
                }
            },
            () =>
            {
                return Witch.witch != null && Witch.witch == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                List<PlayerControl> untargetables;
                if (Witch.spellCastingTarget != null)
                {
                    untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != Witch.spellCastingTarget.PlayerId).ToList();
                }
                else
                {
                    untargetables = new();
                    if (Spy.spy != null && !Witch.canSpellAnyone) untargetables.Add(Spy.spy);
                }

                Witch.currentTarget = SetTarget(untargetables, !Witch.canSpellAnyone);
                SetPlayerOutline(Witch.currentTarget, Witch.color);

                showTargetNameOnButton(Witch.currentTarget, witchSpellButton, GetString("WitchText"));
                if (witchSpellButton.isEffectActive && Witch.spellCastingTarget != Witch.currentTarget)
                {
                    Witch.spellCastingTarget = null;
                    witchSpellButton.Timer = 0f;
                    witchSpellButton.isEffectActive = false;
                }
                return PlayerControl.LocalPlayer.CanMove && Witch.currentTarget != null;
            },
            () =>
            {
                witchSpellButton.Timer = witchSpellButton.MaxTimer;
                witchSpellButton.isEffectActive = false;
                Witch.spellCastingTarget = null;
            },
            Witch.buttonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            true,
            Witch.spellCastingDuration,
            () =>
            {
                if (Witch.spellCastingTarget == null) return;
                var attempt = checkMuderAttempt(Witch.witch, Witch.spellCastingTarget);
                if (attempt == MurderAttemptResult.PerformKill)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetFutureSpelled, SendOption.Reliable);
                    writer.Write(Witch.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFutureSpelled(Witch.currentTarget.PlayerId);
                }

                if (attempt is MurderAttemptResult.BlankKill or MurderAttemptResult.PerformKill)
                {
                    Witch.currentCooldownAddition += Witch.cooldownAddition;
                    witchSpellButton.MaxTimer = Witch.cooldown + Witch.currentCooldownAddition;
                    PlayerControlFixedUpdatePatch.miniCooldownUpdate(); // Modifies the MaxTimer if the witch is the mini
                    witchSpellButton.Timer = witchSpellButton.MaxTimer;
                    if (Witch.triggerBothCooldowns)
                    {
                        Witch.witch.killTimer = ModOption.KillCooldown * multiplier;
                    }
                }
                else
                {
                    witchSpellButton.Timer = 0f;
                }

                Witch.spellCastingTarget = null;
            },
            buttonText: "WitchText".Translate()
        );

        // Jumper Mark
        jumperMarkButton = new CustomButton(
            () =>
            {
                //set location
                Jumper.jumpLocation = PlayerControl.LocalPlayer.transform.localPosition;
                jumperMarkButton.Timer = jumperMarkButton.MaxTimer;
                //jumperJumpButton.Timer = jumperMarkButton.MaxTimer;
            },
            () =>
            {
                return Jumper.jumper != null && Jumper.jumper == PlayerControl.LocalPlayer
                && !PlayerControl.LocalPlayer.Data.IsDead && Jumper.Charges > 0;
            },
            () =>
            {
                Jumper.usedPlace = true;
                return Jumper.Charges > 0f && PlayerControl.LocalPlayer.CanMove;

            },
            () =>
            {
                if (Jumper.resetPlaceAfterMeeting) Jumper.jumpLocation = Vector3.zero;
                jumperMarkButton.Timer = jumperMarkButton.MaxTimer;
            },
            Jumper.jumpMarkButtonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            secondaryAbilityInput.keyCode,
            buttonText: "jumperMarkText".Translate()
        );

        // Jumper Jump
        jumperJumpButton = new CustomButton(
            () =>
            {
                //teleport to location if you have one
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Jumper.jumpLocation);
                Jumper.Charges--;
                jumperJumpButton.Timer = jumperJumpButton.MaxTimer;
                //jumperMarkButton.Timer = jumperJumpButton.MaxTimer;

            },
            () =>
            {
                return Jumper.jumper != null && Jumper.jumper == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.IsAlive() && Jumper.Charges > 0;
            },
            () =>
            {
                Jumper.usedPlace = true;
                return Jumper.Charges >= 1f && Jumper.jumpLocation != Vector3.zero && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                Jumper.Charges += Jumper.ChargesGainOnMeeting;
                if (Jumper.Charges > 0) jumperJumpButton.Timer = jumperJumpButton.MaxTimer;
            },
            Jumper.jumpJumpButtonSprite,
            ButtonPositions.upperRowRight,
            __instance,
            abilityInput.keyCode,
            buttonText: "jumperJumpText".Translate()
        );

        // Escapist Escape
        escapistMarkButton = new CustomButton(
            () =>
            {
                //set location
                Escapist.escapeLocation = PlayerControl.LocalPlayer.transform.localPosition;
                escapistMarkButton.Timer = escapistMarkButton.MaxTimer;
                //escapistEscapeButton.Timer = escapistMarkButton.MaxTimer;
            },
            () =>
            {
                return Escapist.escapist != null && Escapist.escapist == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                Escapist.usedPlace = true;
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                if (Escapist.resetPlaceAfterMeeting) Escapist.escapeLocation = Vector3.zero;
                escapistMarkButton.Timer = escapistMarkButton.MaxTimer;
            },
            Escapist.escapeEscapeButtonSprite,
            ButtonPositions.lowerRowCenter, //brb
            __instance,
            secondaryAbilityInput.keyCode,
            buttonText: "jumperMarkText".Translate()
        );

        // Escapist Escape
        escapistEscapeButton = new CustomButton(
            () =>
            {
                //set location
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Escapist.escapeLocation);
                escapistEscapeButton.Timer = escapistEscapeButton.MaxTimer;
                //escapistMarkButton.Timer = escapistEscapeButton.MaxTimer;
            },
            () =>
            {
                return Escapist.escapist != null && Escapist.escapist == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                Escapist.usedPlace = true;
                return Escapist.escapeLocation != Vector3.zero && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                escapistEscapeButton.Timer = escapistEscapeButton.MaxTimer;
            },
            Escapist.escapeEscapeButtonSprite,
            ButtonPositions.upperRowLeft, //brb
            __instance,
            abilityInput.keyCode,
            buttonText: "jumperJumpText".Translate()
        );

        // Ninja mark and assassinate button 
        ninjaButton = new CustomButton(
            () =>
            {
                MessageWriter writer;
                if (Ninja.ninjaMarked != null)
                {
                    // Murder attempt with teleport
                    var attempt = checkMuderAttempt(Ninja.ninja, Ninja.ninjaMarked);
                    if (attempt == MurderAttemptResult.BodyGuardKill)
                    {
                        checkMurderAttemptAndKill(Ninja.ninja, Ninja.ninjaMarked);
                        return;
                    }

                    if (attempt == MurderAttemptResult.PerformKill || attempt == MurderAttemptResult.ReverseKill)
                    {
                        // Create first trace before killing
                        var pos = PlayerControl.LocalPlayer.transform.position;
                        var buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                        writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.PlaceNinjaTrace);
                        writer.WriteBytesAndSize(buff);
                        writer.EndMessage();
                        RPCProcedure.placeNinjaTrace(buff);

                        var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(
                            PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetInvisible,
                            SendOption.Reliable);
                        invisibleWriter.Write(Ninja.ninja.PlayerId);
                        invisibleWriter.Write(byte.MinValue);
                        AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                        RPCProcedure.setInvisible(Ninja.ninja.PlayerId, byte.MinValue);
                        if (!checkAndDoVetKill(Ninja.ninjaMarked))
                        {
                            // Perform Kill
                            var writer2 = AmongUsClient.Instance.StartRpcImmediately(
                                PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer,
                                SendOption.Reliable);
                            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer2.Write(Ninja.ninjaMarked.PlayerId);
                            writer2.Write(byte.MaxValue);
                            AmongUsClient.Instance.FinishRpcImmediately(writer2);
                            if (SubmergedCompatibility.IsSubmerged)
                                SubmergedCompatibility.ChangeFloor(Ninja.ninjaMarked.transform.localPosition.y > -7);
                            RPCProcedure.uncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId,
                                Ninja.ninjaMarked.PlayerId, byte.MaxValue);
                        }

                        // Create Second trace after killing
                        pos = Ninja.ninjaMarked.transform.position;
                        buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                        var writer3 = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.PlaceNinjaTrace);
                        writer3.WriteBytesAndSize(buff);
                        writer3.EndMessage();
                        RPCProcedure.placeNinjaTrace(buff);
                    }

                    if (attempt is MurderAttemptResult.BlankKill or MurderAttemptResult.PerformKill)
                    {
                        Ninja.ninja.killTimer = ninjaButton.Timer = ninjaButton.MaxTimer;
                    }
                    else if (attempt == MurderAttemptResult.SuppressKill)
                    {
                        ninjaButton.Timer = 0f;
                    }

                    Ninja.ninjaMarked = null;
                    return;
                }

                if (Ninja.currentTarget != null)
                {
                    if (checkAndDoVetKill(Ninja.currentTarget)) return;
                    Ninja.ninjaMarked = Ninja.currentTarget;
                    ninjaButton.Timer = 5f;
                    SoundEffectsManager.play("warlockCurse");

                    // Ghost Info
                    writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.NinjaMarked);
                    writer.Write(Ninja.ninjaMarked.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            },
            () =>
            {
                return Ninja.ninja.IsAlive() && Ninja.ninja == PlayerControl.LocalPlayer;
            },
            () =>
            {
                // CouldUse
                var untargetables = new List<PlayerControl>();
                if (Spy.spy != null && !Spy.impostorsCanKillAnyone) untargetables.Add(Spy.spy);
                if (Mini.mini != null && !Mini.isGrownUp()) untargetables.Add(Mini.mini);
                Ninja.currentTarget = SetTarget(untargetables, Spy.spy == null || !Spy.impostorsCanKillAnyone);
                SetPlayerOutline(Ninja.currentTarget, Ninja.color);

                showTargetNameOnButton(Ninja.currentTarget, ninjaButton, GetString("NinjaText"));
                ninjaButton.Sprite = Ninja.ninjaMarked != null
                    ? Ninja.killButtonSprite
                    : Ninja.markButtonSprite;
                return (Ninja.currentTarget != null || Ninja.ninjaMarked != null
                        && !Ninja.ninjaMarked.isUsingTransportation())
                        && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                // on meeting ends
                ninjaButton.Timer = ninjaButton.MaxTimer;
                Ninja.ninjaMarked = null;
            },
            Ninja.markButtonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("NinjaText")
        );

        blackmailerButton = new CustomButton(
            () =>
            {
                // Action when Pressed
                var target = Blackmailer.currentTarget;
                if (checkAndDoVetKill(target)) return;
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.BlackmailPlayer);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                RPCProcedure.blackmailPlayer(target.PlayerId);
                Blackmailer.currentTarget = null;
            },
            () =>
            {
                return Blackmailer.blackmailer.IsAlive() && Blackmailer.blackmailer == PlayerControl.LocalPlayer;
            },
            () =>
            {
                // Could Use
                Blackmailer.currentTarget = SetTarget();
                SetPlayerOutline(Blackmailer.currentTarget, Blackmailer.blackmailedColor);

                if (Blackmailer.blackmailed == null)
                {
                    showTargetNameOnButton(Blackmailer.currentTarget, blackmailerButton, GetString("BlackmailerText"));
                }
                return Blackmailer.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { blackmailerButton.Timer = blackmailerButton.MaxTimer; },
            Blackmailer.blackmailButtonSprite,
            ButtonPositions.upperRowLeft, //brb
            __instance,
            abilityInput.keyCode,
            true,
            1f,
            () => { },
            false,
            buttonText: GetString("BlackmailerText")
        );

        // Trapper button
        trapperButton = new CustomButton(
            () =>
            {
                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetTrap);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.setTrap(buff);

                SoundEffectsManager.play("trapperTrap");
                trapperButton.Timer = trapperButton.MaxTimer;
            },
            () =>
            {
                return Trapper.trapper != null && Trapper.trapper == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (trapperChargesText != null) trapperChargesText.text = $"{Trapper.charges} / {Trapper.maxCharges}";
                return PlayerControl.LocalPlayer.CanMove && Trapper.charges > 0;
            },
            () => { trapperButton.Timer = trapperButton.MaxTimer; },
            Trapper.trapButtonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("trapperTrapText")
        );

        // Terrorist button
        terroristButton = new CustomButton(
            () =>
            {
                if (checkMuderAttempt(Terrorist.terrorist, Terrorist.terrorist) != MurderAttemptResult.BlankKill)
                {
                    var pos = PlayerControl.LocalPlayer.transform.position;
                    var buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.PlaceBomb);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placeBomb(buff);

                    if (Terrorist.selfExplosion)
                    {
                        var loacl = Terrorist.terrorist.PlayerId;
                        var writer1 = StartRPC(Terrorist.terrorist, CustomRPC.UncheckedMurderPlayer);
                        writer1.Write(loacl);
                        writer1.Write(loacl);
                        writer1.Write(byte.MaxValue);
                        writer1.EndRPC();
                        RPCProcedure.uncheckedMurderPlayer(loacl, loacl, byte.MaxValue);
                    }

                    SoundEffectsManager.play(Terrorist.selfExplosion ? "bombExplosion" : "trapperTrap");
                }

                terroristButton.Timer = terroristButton.MaxTimer;
                Terrorist.isPlanted = true;
            },
            () =>
            {
                return Terrorist.terrorist != null && Terrorist.terrorist == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { return PlayerControl.LocalPlayer.CanMove && !Terrorist.isPlanted; },
            () =>
            {
                terroristButton.Timer = terroristButton.MaxTimer;
            },
            Terrorist.buttonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            true,
            Terrorist.destructionTime,
            () =>
            {
                terroristButton.Timer = terroristButton.MaxTimer;
                terroristButton.isEffectActive = false;
                terroristButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            buttonText: Terrorist.selfExplosion ? GetString("TerroristBombText2") : GetString("TerroristBombText1")
        );

        defuseButton = new CustomButton(
            () => { defuseButton.HasEffect = true; },
            () =>
            {
                return Terrorist.bomb != null && Bomb.canDefuse && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (defuseButton.isEffectActive && !Bomb.canDefuse)
                {
                    defuseButton.Timer = 0f;
                    defuseButton.isEffectActive = false;
                }

                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                defuseButton.Timer = 0f;
                defuseButton.isEffectActive = false;
            },
            Bomb.defuseSprite,
            new Vector3(4f, 1f, 0),
            __instance,
            hotkey: null,
            true,
            Terrorist.defuseDuration,
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.DefuseBomb, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.defuseBomb();

                defuseButton.Timer = 0f;
                Bomb.canDefuse = false;
            },
            true,
            buttonText: GetString("defuseBombText")
        );

        thiefKillButton = new CustomButton(
            () =>
            {
                var thief = Thief.thief;
                var target = Thief.currentTarget;
                var result = checkMuderAttempt(thief, target);
                if (result == MurderAttemptResult.BlankKill)
                {
                    thiefKillButton.Timer = thiefKillButton.MaxTimer;
                    return;
                }

                if (Thief.suicideFlag)
                {
                    // Suicide
                    var writer2 = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
                    writer2.Write(thief.PlayerId);
                    writer2.Write(thief.PlayerId);
                    writer2.Write(0);
                    writer2.EndRPC();
                    RPCProcedure.uncheckedMurderPlayer(thief.PlayerId, thief.PlayerId, 0);
                    Thief.thief.clearAllTasks();
                }

                if (result is MurderAttemptResult.ReverseKill or MurderAttemptResult.BodyGuardKill)
                {
                    checkMurderAttemptAndKill(PlayerControl.LocalPlayer, target);
                }

                // Steal role if survived.
                if (!Thief.thief.Data.IsDead && result == MurderAttemptResult.PerformKill)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.ThiefStealsRole);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();
                    Thief.StealsRole(target.PlayerId);
                }

                // Kill the victim (after becoming their role - so that no win is triggered for other teams)
                if (result == MurderAttemptResult.PerformKill)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
                    writer.Write(thief.PlayerId);
                    writer.Write(target.PlayerId);
                    writer.Write(byte.MaxValue);
                    writer.EndRPC();
                    RPCProcedure.uncheckedMurderPlayer(thief.PlayerId, target.PlayerId, byte.MaxValue);
                }
            },
            () =>
            {
                return Thief.thief != null && PlayerControl.LocalPlayer == Thief.thief && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                var untargetables = new List<PlayerControl>();
                if (Mini.mini != null && !Mini.isGrownUp()) untargetables.Add(Mini.mini);
                Thief.currentTarget = SetTarget(untarget: untargetables);
                SetPlayerOutline(Thief.currentTarget, Thief.color);

                return Thief.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { thiefKillButton.Timer = thiefKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        // Trapper Charges
        trapperChargesText = Object.Instantiate(trapperButton.actionButton.cooldownTimerText,
            trapperButton.actionButton.cooldownTimerText.transform.parent);
        trapperChargesText.text = "";
        trapperChargesText.enableWordWrapping = false;
        trapperChargesText.transform.localScale = Vector3.one * 0.5f;
        trapperChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        // Yoyo button
        yoyoButton = new CustomButton(
            () =>
            {
                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                if (Yoyo.markedLocation == null)
                {
                    Message($"marked location is null in button press");
                    var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.YoyoMarkLocation, SendOption.Reliable);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.yoyoMarkLocation(buff);
                    SoundEffectsManager.play("tricksterPlaceBox");
                    yoyoButton.Sprite = Yoyo.blinkButtonSprite;
                    yoyoButton.Timer = 10f;
                    yoyoButton.HasEffect = false;
                    yoyoButton.buttonText = "BlinkText".Translate();
                }
                else
                {
                    Message("in else for some reason");
                    // Jump to location
                    Message($"trying to blink!");
                    var exit = (Vector3)Yoyo.markedLocation;
                    if (SubmergedCompatibility.IsSubmerged)
                        SubmergedCompatibility.ChangeFloor(exit.y > -7);
                    var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.YoyoBlink, SendOption.Reliable);
                    writer.Write(byte.MaxValue);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.yoyoBlink(true, buff);
                    yoyoButton.EffectDuration = Yoyo.blinkDuration;
                    yoyoButton.Timer = 10f;
                    yoyoButton.HasEffect = true;
                    yoyoButton.buttonText = "ReturningText".Translate();
                    SoundEffectsManager.play("morphlingMorph");
                }
            },
            () => { return Yoyo.yoyo != null && Yoyo.yoyo == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                if (Yoyo.markStaysOverMeeting)
                    yoyoButton.Timer = 10f;
                else
                {
                    Yoyo.markedLocation = null;
                    yoyoButton.Timer = yoyoButton.MaxTimer;
                    yoyoButton.Sprite = Yoyo.markButtonSprite;
                    yoyoButton.buttonText = "YoyoMarkText".Translate();
                }
            },
            Yoyo.markButtonSprite,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            false,
            Yoyo.blinkDuration,
            () =>
            {
                if (Yoyo.yoyo.isUsingTransportation())
                {
                    yoyoButton.Timer = 0.5f;
                    yoyoButton.DeputyTimer = 0.5f;
                    yoyoButton.isEffectActive = true;
                    yoyoButton.actionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
                    return;
                }
                else if (Yoyo.yoyo.inVent)
                {
                    __instance.ImpostorVentButton.DoClick();
                }
                // jump back!
                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
                var exit = (Vector3)Yoyo.markedLocation;
                if (SubmergedCompatibility.IsSubmerged)
                    SubmergedCompatibility.ChangeFloor(exit.y > -7);
                var writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.YoyoBlink, SendOption.Reliable);
                writer.Write((byte)0);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.yoyoBlink(false, buff);
                yoyoButton.Timer = yoyoButton.MaxTimer;
                yoyoButton.isEffectActive = false;
                yoyoButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                yoyoButton.HasEffect = false;
                yoyoButton.Sprite = Yoyo.markButtonSprite;
                yoyoButton.buttonText = "YoyoMarkText".Translate();
                SoundEffectsManager.play("morphlingMorph");
                if (Minigame.Instance)
                    Minigame.Instance.Close();
            },
            buttonText: "YoyoMarkText".Translate()
        );

        yoyoAdminTableButton = new CustomButton(
           () =>
           {
               if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
               {
                   var __instance = FastDestroyableSingleton<HudManager>.Instance;
                   __instance.InitMap();
                   MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
               }
           },
           () => { return Yoyo.yoyo.IsAlive() && Yoyo.yoyo == PlayerControl.LocalPlayer && Yoyo.hasAdminTable; },
           () =>
           {
               return PlayerControl.LocalPlayer.CanMove;
           },
           () =>
           {
               yoyoAdminTableButton.Timer = yoyoAdminTableButton.MaxTimer;
               yoyoAdminTableButton.isEffectActive = false;
               yoyoAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
           },
           Hacker.getAdminSprite(),
           ButtonPositions.lowerRowCenter,
           __instance,
           KeyCode.G,
           true,
           0f,
           () => PlayerControl.LocalPlayer.IsAlive() && PlayerControl.LocalPlayer.CanMove,
           () =>
           {
               if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
               {
                   var __instance = FastDestroyableSingleton<HudManager>.Instance;
                   __instance.InitMap();
                   MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
               };
           },
           () =>
           {
               yoyoAdminTableButton.Timer = yoyoAdminTableButton.MaxTimer;
               if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
           },
           GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3,
           "AdminMapText".Translate()
       );

        redemptorRevelationButton = new CustomButton(
            () =>
            {
                Redemptor.Revelating = true;
            },
            () =>
            {
                return Redemptor.revelation && Redemptor.Player.IsAlive() && Redemptor.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                redemptorRevelationButton.PositionOffset = Redemptor.prayer ? ButtonPositions.upperRowLeft : ButtonPositions.upperRowCenter;
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                Redemptor.Revelating = false;
                redemptorRevelationButton.Timer = redemptorRevelationButton.MaxTimer;
                redemptorRevelationButton.isEffectActive = false;
                redemptorRevelationButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Tracker.trackCorpsesButtonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            secondaryAbilityInput.keyCode,
            true,
            Redemptor.revelationDuration,
            () =>
            {
                Redemptor.Revelating = false;
                redemptorRevelationButton.Timer = redemptorRevelationButton.MaxTimer;
            },
            buttonText: GetString("RedemptorRevelation")
        );

        redemptorPrayerButton = new CustomButton(
            () =>
            {
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.RedemptorPrayer);
                writer.Write(byte.MaxValue);
                writer.EndRPC();
                Redemptor.RedemptorPrayer(byte.MaxValue);
            },
            () =>
            {
                return Redemptor.prayer && Redemptor.Player.IsAlive() &&
                Redemptor.Player == PlayerControl.LocalPlayer && Redemptor.RevivedPlayer == null;
            },
            () =>
            {
                if (Redemptor.target != null)
                {
                    showTargetNameOnButton(Redemptor.target, redemptorReviveButton, GetString("ReviveButton"));
                }
                return Redemptor.target && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                redemptorPrayerButton.Timer = redemptorPrayerButton.MaxTimer;
                redemptorPrayerButton.isEffectActive = false;
                redemptorPrayerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Redemptor.reviveButton,
            ButtonPositions.upperRowCenter,
            __instance,
            modKillInput.keyCode,
            true,
            Redemptor.prayerDuration,
            () =>
            {
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.RedemptorPrayer);
                writer.Write((byte)0);
                writer.EndRPC();
                Redemptor.RedemptorPrayer(0);

                if (Redemptor.target != null)
                {
                    var writer2 = StartRPC(PlayerControl.LocalPlayer, CustomRPC.RedemptorRevive);
                    writer2.Write(Redemptor.target.PlayerId);
                    writer2.EndRPC();
                    Redemptor.RevivePlayer(Redemptor.target.PlayerId);

                    redemptorPrayerButton.Timer = redemptorPrayerButton.MaxTimer;
                }
            },
            buttonText: GetString("RedemptorPrayer")
        );

        redemptorReviveButton = new CustomButton(
            () =>
            {
                var target = Redemptor.target;
                if (target == null) return;

                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.UncheckedMurderPlayer);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(byte.MaxValue);
                writer.EndRPC();
                RPCProcedure.uncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);

                _ = new LateTask(() =>
                {
                    if (InMeeting) { Message("复活失败", "ReviveTask"); return; }
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.RedemptorRevive);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();
                    Redemptor.RevivePlayer(target.PlayerId);

                    DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
                    for (var i = 0; i < array.Length; i++)
                    {
                        if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            Object.Destroy(array[i].gameObject);
                            break;
                        }
                    }
                }, Redemptor.reviveDuration, "RedemptorRevive");
            },
            () =>
            {
                return Redemptor.Player.IsAlive() && Redemptor.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                var pos = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = PlayerControl.LocalPlayer.MaxReportDistance * 0.21f;

                var deadBody = Physics2D.OverlapCircleAll(pos, maxDistance, Constants.PlayersOnlyMask)
                    .Where(collider => collider.CompareTag("DeadBody"))
                    .Select(collider => collider.GetComponent<DeadBody>())
                    .FirstOrDefault(db => db != null && playerById(db.ParentId)?.Data?.IsDead == true &&
                                          !(playerById(db.ParentId)?.Data?.Disconnected == true));

                if (Redemptor.target != null)
                {
                    showTargetNameOnButton(Redemptor.target, redemptorReviveButton, GetString("RedemptorRevive"));
                }
                Redemptor.target = playerById(deadBody?.ParentId);
                return Redemptor.target && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                redemptorReviveButton.Timer = 10f;
            },
            Redemptor.reviveButton,
            ButtonPositions.upperRowRight,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("RedemptorRevive")
        );

        bandLeaderKeyboardistButton = new CustomButton(
            () =>
            {
                if (BandLeader.Keyboardist == null)
                {
                    var target = BandLeader.currentTarget;
                    if (target == null || BandLeader.Members.Any(x => x.PlayerId == target?.PlayerId)) return;
                    if (checkAndDoVetKill(target)) return;
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.CreateBandMember);
                    writer.Write(target.PlayerId);
                    writer.Write(1);
                    writer.EndRPC();
                    BandLeader.CreateBandMember(target.PlayerId, 1);
                }
                else
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.CreateBandMember);
                    writer.Write(byte.MaxValue);
                    writer.Write(1);
                    writer.EndRPC();
                    BandLeader.CreateBandMember(byte.MaxValue, 1);
                }

                bandLeaderKeyboardistButton.Sprite = BandLeader.Keyboardist == null ? BandLeader.keyboardButton : BandLeader.keyboardDel;

                bandLeaderKeyboardistButton.buttonText = BandLeader.Keyboardist == null ? "招募成员" : "踢出乐队";

                bandLeaderKeyboardistButton.Timer = bandLeaderKeyboardistButton.MaxTimer = BandLeader.createCoolDown;
                bandLeaderBassistButton.Timer = bandLeaderBassistButton.MaxTimer = BandLeader.createCoolDown;
                bandLeaderDrummerButton.Timer = bandLeaderDrummerButton.MaxTimer = BandLeader.createCoolDown;
            },
            () =>
            {
                return BandLeader.Player.IsAlive() && BandLeader.Player == PlayerControl.LocalPlayer && !BandLeader.Formed;
            },
            () =>
            {
                BandLeader.currentTarget = SetTarget(BandLeader.Members);
                SetPlayerOutline(BandLeader.currentTarget, BandLeader.color);

                if (bandLeaderKeyboardistText != null) bandLeaderKeyboardistText.text = $"{BandLeader.Keyboardist?.Data?.PlayerName ?? ""}";
                return PlayerControl.LocalPlayer.CanMove && BandLeader.Keyboardist == null
                    ? BandLeader.currentTarget : true;
            },
            () =>
            {
                bandLeaderKeyboardistButton.Sprite = BandLeader.Keyboardist == null ? BandLeader.keyboardButton : BandLeader.keyboardDel;

                bandLeaderKeyboardistButton.Timer = bandLeaderKeyboardistButton.MaxTimer = BandLeader.createCoolDown;
            },
            BandLeader.keyboardButton,
            ButtonPositions.upperRowRight,
            __instance,
            null,
            buttonText: "招募乐手"
        );

        bandLeaderKeyboardistText = Object.Instantiate(bandLeaderKeyboardistButton.actionButton.cooldownTimerText,
            bandLeaderKeyboardistButton.actionButton.cooldownTimerText.transform.parent);
        bandLeaderKeyboardistText.text = "";
        bandLeaderKeyboardistText.enableWordWrapping = false;
        bandLeaderKeyboardistText.transform.localScale = Vector3.one * 0.5f;
        bandLeaderKeyboardistText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        bandLeaderBassistButton = new CustomButton(
            () =>
            {
                if (BandLeader.Bassist == null)
                {
                    var target = BandLeader.currentTarget;
                    if (BandLeader.Members.Any(x => x.PlayerId == target?.PlayerId) || target == null) return;
                    if (checkAndDoVetKill(target)) return;
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.CreateBandMember);
                    writer.Write(target.PlayerId);
                    writer.Write(2);
                    writer.EndRPC();
                    BandLeader.CreateBandMember(target.PlayerId, 2);
                }
                else
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.CreateBandMember);
                    writer.Write(byte.MaxValue);
                    writer.Write(2);
                    writer.EndRPC();
                    BandLeader.CreateBandMember(byte.MaxValue, 2);
                }

                bandLeaderBassistButton.Sprite = BandLeader.Bassist == null ? BandLeader.bassButton : BandLeader.bassDel;

                bandLeaderBassistButton.buttonText = BandLeader.Bassist == null ? "招募成员" : "踢出乐队";

                bandLeaderKeyboardistButton.Timer = bandLeaderKeyboardistButton.MaxTimer = BandLeader.createCoolDown;
                bandLeaderBassistButton.Timer = bandLeaderBassistButton.MaxTimer = BandLeader.createCoolDown;
                bandLeaderDrummerButton.Timer = bandLeaderDrummerButton.MaxTimer = BandLeader.createCoolDown;
            },
            () =>
            {
                return BandLeader.Player.IsAlive() && BandLeader.Player == PlayerControl.LocalPlayer && !BandLeader.Formed;
            },
            () =>
            {
                if (bandLeaderBassistText != null) bandLeaderBassistText.text = $"{BandLeader.Bassist?.Data?.PlayerName ?? ""}";

                return PlayerControl.LocalPlayer.CanMove && BandLeader.Bassist == null
                    ? BandLeader.currentTarget : true;
            },
            () =>
            {
                bandLeaderBassistButton.Sprite = BandLeader.Bassist == null ? BandLeader.bassButton : BandLeader.bassDel;

                bandLeaderBassistButton.Timer = bandLeaderBassistButton.MaxTimer = BandLeader.createCoolDown;
            },
            BandLeader.bassButton,
            ButtonPositions.upperRowCenter,
            __instance,
            null,
            buttonText: "招募乐手"
        );

        bandLeaderBassistText = Object.Instantiate(bandLeaderBassistButton.actionButton.cooldownTimerText,
            bandLeaderBassistButton.actionButton.cooldownTimerText.transform.parent);
        bandLeaderBassistText.text = "";
        bandLeaderBassistText.enableWordWrapping = false;
        bandLeaderBassistText.transform.localScale = Vector3.one * 0.5f;
        bandLeaderBassistText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        bandLeaderDrummerButton = new CustomButton(
            () =>
            {
                if (BandLeader.Drummer == null)
                {
                    var target = BandLeader.currentTarget;
                    if (BandLeader.Members.Any(x => x.PlayerId == target?.PlayerId) || target == null) return;
                    if (checkAndDoVetKill(target)) return;
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.CreateBandMember);
                    writer.Write(target.PlayerId);
                    writer.Write(3);
                    writer.EndRPC();
                    BandLeader.CreateBandMember(target.PlayerId, 3);
                }
                else
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.CreateBandMember);
                    writer.Write(byte.MaxValue);
                    writer.Write(3);
                    writer.EndRPC();
                    BandLeader.CreateBandMember(byte.MaxValue, 3);
                }

                bandLeaderDrummerButton.Sprite = BandLeader.Drummer == null ? BandLeader.drumButton : BandLeader.drumDel;

                bandLeaderDrummerButton.buttonText = BandLeader.Drummer == null ? "招募成员" : "踢出乐队";

                bandLeaderKeyboardistButton.Timer = bandLeaderKeyboardistButton.MaxTimer = BandLeader.createCoolDown;
                bandLeaderBassistButton.Timer = bandLeaderBassistButton.MaxTimer = BandLeader.createCoolDown;
                bandLeaderDrummerButton.Timer = bandLeaderDrummerButton.MaxTimer = BandLeader.createCoolDown;
            },
            () =>
            {
                return BandLeader.Player.IsAlive() && BandLeader.Player == PlayerControl.LocalPlayer && !BandLeader.Formed;
            },
            () =>
            {
                if (bandLeaderDrummerText != null) bandLeaderDrummerText.text = $"{BandLeader.Drummer?.Data?.PlayerName ?? ""}";

                return PlayerControl.LocalPlayer.CanMove && BandLeader.Drummer == null
                    ? BandLeader.currentTarget : true;
            },
            () =>
            {
                bandLeaderDrummerButton.Sprite = BandLeader.Drummer == null ? BandLeader.drumButton : BandLeader.drumDel;

                bandLeaderDrummerButton.Timer = bandLeaderDrummerButton.MaxTimer = BandLeader.createCoolDown;
            },
            BandLeader.drumButton,
            ButtonPositions.upperRowLeft,
            __instance,
            null,
            buttonText: "招募乐手"
        );

        bandLeaderDrummerText = Object.Instantiate(bandLeaderDrummerButton.actionButton.cooldownTimerText,
            bandLeaderDrummerButton.actionButton.cooldownTimerText.transform.parent);
        bandLeaderDrummerText.text = "";
        bandLeaderDrummerText.enableWordWrapping = false;
        bandLeaderDrummerText.transform.localScale = Vector3.one * 0.5f;
        bandLeaderDrummerText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        bandLeaderKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(BandLeader.currentTarget)) return;
                if (checkMurderAttemptAndKill(BandLeader.Player, BandLeader.currentTarget) ==
                    MurderAttemptResult.SuppressKill) return;

                bandLeaderKillButton.Timer = bandLeaderKillButton.MaxTimer;
                BandLeader.currentTarget = null;
            },
            () =>
            {
                return BandLeader.Player.IsAlive() && BandLeader.Player == PlayerControl.LocalPlayer && BandLeader.Formed
                       && BandLeader.winnerFlags == BandLeader.WinnerFlags.Impostor;
            },
            () =>
            {
                BandLeader.currentTarget = SetTarget(BandLeader.Members);
                SetPlayerOutline(BandLeader.currentTarget, BandLeader.color);

                showTargetNameOnButton(BandLeader.currentTarget, bandLeaderKillButton, GetString("killButtonText"));

                return PlayerControl.LocalPlayer.CanMove && BandLeader.currentTarget != null;
            },
            () =>
            {
                bandLeaderKillButton.Timer = bandLeaderKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        schrodingersCatKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(SchrodingersCat.currentTarget)) return;
                if (checkMurderAttemptAndKill(SchrodingersCat.Player, SchrodingersCat.currentTarget) ==
                    MurderAttemptResult.SuppressKill) return;

                schrodingersCatKillButton.Timer = schrodingersCatKillButton.MaxTimer;
                SchrodingersCat.currentTarget = null;
            },
            () =>
            {
                return SchrodingersCat.Player.IsAlive() && SchrodingersCat.Player == PlayerControl.LocalPlayer && SchrodingersCat.CanKill &&
                       SchrodingersCat.State is not SchrodingersCat.CatState.None and not SchrodingersCat.CatState.Crewmate;
            },
            () =>
            {
                List<PlayerControl> untargetablePlayers = [];
                switch (SchrodingersCat.State)
                {
                    case SchrodingersCat.CatState.Impostor:
                        if (Spy.spy != null && !Spy.impostorsCanKillAnyone) untargetablePlayers.Add(Spy.spy);
                        break;
                    case SchrodingersCat.CatState.Jackal:
                        untargetablePlayers.Add(Jackal.Sidekick);
                        untargetablePlayers.AddRange(Jackal.jackal);
                        break;
                    case SchrodingersCat.CatState.Pavlovsowner:
                        untargetablePlayers.Add(Pavlovsdogs.pavlovsowner);
                        untargetablePlayers.AddRange(Pavlovsdogs.pavlovsdogs);
                        break;
                    case SchrodingersCat.CatState.Werewolf:
                        untargetablePlayers.Add(Werewolf.werewolf);
                        break;
                    case SchrodingersCat.CatState.Juggernaut:
                        untargetablePlayers.Add(Juggernaut.juggernaut);
                        break;
                    case SchrodingersCat.CatState.Swooper:
                        untargetablePlayers.Add(Swooper.swooper);
                        break;
                    case SchrodingersCat.CatState.Arsonist:
                        untargetablePlayers.Add(Arsonist.arsonist);
                        break;
                    case SchrodingersCat.CatState.Pelican:
                        untargetablePlayers.Add(Pelican.Player);
                        break;
                }
                untargetablePlayers.RemoveAll(x => x == null);
                var OnlyCrew = SchrodingersCat.State == SchrodingersCat.CatState.Impostor && (Spy.spy == null || !Spy.impostorsCanKillAnyone);
                var target = SetTarget(untargetablePlayers, OnlyCrew, true);

                SchrodingersCat.currentTarget = target;
                SetPlayerOutline(SchrodingersCat.currentTarget, SchrodingersCat.color);

                showTargetNameOnButton(SchrodingersCat.currentTarget, schrodingersCatKillButton, GetString("killButtonText"));
                return PlayerControl.LocalPlayer.CanMove && SchrodingersCat.currentTarget != null;
            },
            () =>
            {
                schrodingersCatKillButton.Timer = schrodingersCatKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        gunsmithGetBullets = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.killTimer = Gunsmith.setKillCooldown;
                Gunsmith.remainingChange--;
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.SyncGunsmithChange);
                writer.Write(Gunsmith.remainingChange);
                writer.EndRPC();
            },
            () =>
            {
                return Gunsmith.Player.IsAlive() && Gunsmith.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                if (gunsmithGetBulletsText != null) gunsmithGetBulletsText.text = $"{Gunsmith.remainingChange} / {Gunsmith.maxChangeCount}";
                return PlayerControl.LocalPlayer.CanMove && PlayerControl.LocalPlayer.killTimer > Gunsmith.setKillCooldown && Gunsmith.remainingChange > 0;
            },
            () => { },
            Gunsmith.GetButton,
            ButtonPositions.upperRowLeft,
            __instance,
            abilityInput.keyCode,
            buttonText: GetString("gunsmithGetBullets")
        );

        gunsmithGetBulletsText = Object.Instantiate(gunsmithGetBullets.actionButton.cooldownTimerText,
            gunsmithGetBullets.actionButton.cooldownTimerText.transform.parent);
        gunsmithGetBulletsText.text = "";
        gunsmithGetBulletsText.enableWordWrapping = false;
        gunsmithGetBulletsText.transform.localScale = Vector3.one * 0.5f;
        gunsmithGetBulletsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        gunsmithAddBullets = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.SetKillTimer(ModOption.KillCooldown * multiplier);
                Gunsmith.remainingChange++;
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.SyncGunsmithChange);
                writer.Write(Gunsmith.remainingChange);
                writer.EndRPC();
            },
            () =>
            {
                return Gunsmith.Player.IsAlive() && Gunsmith.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && !FastDestroyableSingleton<HudManager>.Instance.KillButton.isCoolingDown && Gunsmith.remainingChange < Gunsmith.maxChangeCount;
            },
            () => { },
            Gunsmith.AddButton,
            ButtonPositions.lowerRowCenter,
            __instance,
            secondaryAbilityInput.keyCode,
            buttonText: GetString("gunsmithAddBullets")
        );

        berserkerKillButton = new CustomButton(
            () =>
            {
                var target = Berserker.currentTarget;
                if (checkAndDoVetKill(target)) return;
                if (checkMurderAttemptAndKill(Berserker.Player, target) == MurderAttemptResult.SuppressKill) return;

                berserkerKillButton.EffectDuration = Berserker.GetDuration();
                target = null;
            },
            () =>
            {
                return Berserker.Player.IsAlive() && Berserker.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                List<PlayerControl> untargetablePlayers = [];
                if (Spy.spy != null && !Spy.impostorsCanKillAnyone) untargetablePlayers.Add(Spy.spy);
                if (SchrodingersCat.Player.IsAlive() && SchrodingersCat.State == SchrodingersCat.CatState.Impostor) untargetablePlayers.Add(SchrodingersCat.Player);
                var target = SetTarget(untargetablePlayers, !(Spy.spy != null && Spy.impostorsCanKillAnyone), true);

                Berserker.currentTarget = target;
                SetPlayerOutline(Berserker.currentTarget, Berserker.color);
                showTargetNameOnButton(Berserker.currentTarget, berserkerKillButton, GetString("killButtonText"));

                if (berserkerKillButtonText != null)
                {
                    berserkerKillButtonText.text = !berserkerKillButton.isEffectActive
                        ? $"{(int)(Berserker.GetDurationPercentage() * 100)} % | {Berserker.GetDuration():0.00}s"
                        : $"{berserkerKillButton.Timer:0.00}";
                }

                if (berserkerKillButton.Timer <= 0)
                {
                    Berserker.UpdateTimer();
                }
                else
                {
                    Berserker.Timer = 0;
                }

                return PlayerControl.LocalPlayer.CanMove && Berserker.currentTarget != null;
            },
            () =>
            {
                berserkerKillButton.MaxTimer = LastImpostor.lastImpostor == PlayerControl.LocalPlayer
                    ? Berserker.KillCooldown - LastImpostor.deduce
                    : Berserker.KillCooldown;
                berserkerKillButton.Timer = berserkerKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowCenter,
            __instance,
            modKillInput.keyCode,
            true,
            0.5f,
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && Berserker.currentTarget;
            },
            () =>
            {
                var target = Berserker.currentTarget;
                if (checkAndDoVetKill(target)) return;
                if (checkMurderAttemptAndKill(Berserker.Player, target) == MurderAttemptResult.SuppressKill) return;
                target = null;
            },
            () =>
            {
                berserkerKillButton.MaxTimer = LastImpostor.lastImpostor == PlayerControl.LocalPlayer
                    ? Berserker.KillCooldown - LastImpostor.deduce
                    : Berserker.KillCooldown;
                berserkerKillButton.Timer = berserkerKillButton.MaxTimer;
                berserkerKillButton.isEffectActive = false;
                Berserker.Timer = 0f;
            },
            buttonText: GetString("killButtonText")
        );

        berserkerKillButtonText = Object.Instantiate(berserkerKillButton.actionButton.cooldownTimerText,
            berserkerKillButton.actionButton.cooldownTimerText.transform.parent);
        berserkerKillButtonText.text = "";
        berserkerKillButtonText.enableWordWrapping = false;
        berserkerKillButtonText.transform.localScale = Vector3.one * 0.5f;
        berserkerKillButtonText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        poltergeistButton = new(
            () =>
            {
                var deadBody = Poltergeist.targetBody;
                if (deadBody == null) return;
                var writer = StartRPC(CustomRPC.PoltergeistMove);
                writer.Write(deadBody.ParentId);
                writer.Write(PlayerControl.LocalPlayer.GetTruePosition());
                writer.EndRPC();
                Poltergeist.MoveDeadBody(deadBody.ParentId, PlayerControl.LocalPlayer.GetTruePosition());

                poltergeistButton.Timer = poltergeistButton.MaxTimer;
            },
            () =>
            {
                return Poltergeist.Player == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.IsDead();
            },
            () =>
            {
                var array = Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                      PlayerControl.LocalPlayer.MaxReportDistance * Poltergeist.radius, Constants.PlayersOnlyMask)
                 .Where(collider => collider.tag == "DeadBody")
                 .Select(collider => collider.GetComponent<DeadBody>())
                 .Where(deadBody => deadBody != null);

                Poltergeist.targetBody = array.FirstOrDefault();
                return Poltergeist.targetBody && PlayerControl.LocalPlayer.CanMove;
            },
            () => { poltergeistButton.Timer = poltergeistButton.MaxTimer; },
            Poltergeist.ButtonSprite,
            ButtonPositions.upperRowCenter,
            __instance,
            secondaryAbilityInput.keyCode,
            buttonText: GetString("poltergeistButton")
        );

        // Set the default (or settings from the previous game) timers / durations when spawning the buttons
        initialized = true;
        setCustomButtonCooldowns();
        deputyHandcuffedButtons = new Dictionary<byte, List<CustomButton>>();
    }
}