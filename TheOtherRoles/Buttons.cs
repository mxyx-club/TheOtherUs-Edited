using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.Objects.CustomButton;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace TheOtherRoles;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
internal static class HudManagerStartPatch
{
    private static bool initialized;

    private static readonly float defaultMaxTimer = 0.5f;
    public static CustomButton engineerRepairButton;
    public static CustomButton sheriffKillButton;
    private static CustomButton deputyHandcuffButton;
    public static CustomButton timeMasterShieldButton;
    private static CustomButton amnisiacRememberButton;
    public static CustomButton veterenAlertButton;
    public static CustomButton medicShieldButton;
    private static CustomButton cultistTurnButton;
    private static CustomButton shifterShiftButton;
    public static CustomButton bomberBombButton;
    public static CustomButton bomberGiveButton;
    private static CustomButton disperserDisperseButton;
    private static CustomButton buttonBarryButton;
    private static CustomButton morphlingButton;
    private static CustomButton camouflagerButton;
    public static CustomButton portalmakerPlacePortalButton;
    private static CustomButton usePortalButton;
    private static CustomButton portalmakerMoveToPortalButton;
    public static CustomButton hackerButton;
    //private static CustomButton changeChatButton;
    public static CustomButton hackerVitalsButton;
    public static CustomButton hackerAdminTableButton;
    public static CustomButton trackerTrackPlayerButton;
    public static CustomButton bodyGuardGuardButton;
    public static CustomButton privateInvestigatorWatchButton;
    private static CustomButton trackerTrackCorpsesButton;
    public static CustomButton vampireKillButton;
    public static CustomButton garlicButton;
    public static CustomButton jackalKillButton;
    public static CustomButton sidekickKillButton;
    public static CustomButton jackalSwoopButton;
    public static CustomButton swooperSwoopButton;
    public static CustomButton swooperKillButton;
    private static CustomButton jackalSidekickButton;
    private static CustomButton eraserButton;
    private static CustomButton placeJackInTheBoxButton;
    private static CustomButton lightsOutButton;
    public static CustomButton cleanerCleanButton;
    public static CustomButton undertakerDragButton;
    public static CustomButton warlockCurseButton;
    public static CustomButton securityGuardButton;
    public static CustomButton securityGuardCamButton;
    public static CustomButton arsonistButton;
    public static CustomButton vultureEatButton;
    public static CustomButton mediumButton;
    public static CustomButton pursuerButton;
    public static CustomButton witchSpellButton;
    public static CustomButton jumperButton;
    public static CustomButton escapistButton;
    public static CustomButton ninjaButton;
    public static CustomButton werewolfRampageButton;
    public static CustomButton werewolfKillButton;
    public static CustomButton minerMineButton;
    public static CustomButton mayorMeetingButton;
    public static CustomButton blackmailerButton;
    public static CustomButton thiefKillButton;
    public static CustomButton juggernautKillButton;
    public static CustomButton evilTrapperSetTrapButton;
    public static CustomButton doomsayerButton;
    public static CustomButton akujoHonmeiButton;
    public static CustomButton akujoBackupButton;
    public static CustomButton yoyoButton;
    public static CustomButton yoyoAdminTableButton;
    public static CustomButton trapperButton;
    public static CustomButton prophetButton;
    public static CustomButton terroristButton;
    public static CustomButton defuseButton;
    public static CustomButton zoomOutButton;
    public static CustomButton roleSummaryButton;
    private static CustomButton hunterLighterButton;
    private static CustomButton hunterAdminTableButton;
    private static CustomButton hunterArrowButton;
    private static CustomButton huntedShieldButton;
    public static CustomButton propDisguiseButton;
    private static CustomButton propHuntUnstuckButton;
    public static CustomButton propHuntRevealButton;
    private static CustomButton propHuntInvisButton;
    private static CustomButton propHuntSpeedboostButton;
    public static CustomButton propHuntAdminButton;
    public static CustomButton propHuntFindButton;
    public static CustomButton pavlovsdogsKillButton;
    public static CustomButton pavlovsownerCreateDogButton;

    public static Dictionary<byte, List<CustomButton>> deputyHandcuffedButtons;
    public static PoolablePlayer targetDisplay;
    public static GameObject propSpriteHolder;
    public static SpriteRenderer propSpriteRenderer;

    public static TMP_Text securityGuardButtonScrewsText;
    public static TMP_Text securityGuardChargesText;
    public static TMP_Text deputyButtonHandcuffsText;
    public static TMP_Text pursuerButtonBlanksText;
    public static TMP_Text hackerAdminTableChargesText;
    public static TMP_Text hackerVitalsChargesText;
    public static TMP_Text trapperChargesText;
    public static TMP_Text prophetButtonText;
    public static TMP_Text portalmakerButtonText1;
    public static TMP_Text portalmakerButtonText2;
    public static TMP_Text huntedShieldCountText;
    public static TMP_Text PavlovsdogKillSelfText;
    public static TMP_Text akujoTimeRemainingText;
    public static TMP_Text akujoBackupLeftText;

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
        engineerRepairButton.MaxTimer = defaultMaxTimer;
        sheriffKillButton.MaxTimer = Sheriff.cooldown;
        deputyHandcuffButton.MaxTimer = Deputy.handcuffCooldown;
        timeMasterShieldButton.MaxTimer = TimeMaster.cooldown;
        veterenAlertButton.MaxTimer = Veteren.cooldown;
        medicShieldButton.MaxTimer = defaultMaxTimer;
        shifterShiftButton.MaxTimer = defaultMaxTimer;
        disperserDisperseButton.MaxTimer = defaultMaxTimer;
        buttonBarryButton.MaxTimer = defaultMaxTimer;
        morphlingButton.MaxTimer = Morphling.cooldown;
        bomberBombButton.MaxTimer = Bomber.cooldown;
        camouflagerButton.MaxTimer = Camouflager.cooldown;
        portalmakerPlacePortalButton.MaxTimer = Portalmaker.cooldown;
        usePortalButton.MaxTimer = Portalmaker.usePortalCooldown;
        portalmakerMoveToPortalButton.MaxTimer = Portalmaker.usePortalCooldown;
        hackerButton.MaxTimer = Hacker.cooldown;
        hackerVitalsButton.MaxTimer = Hacker.cooldown;
        hackerAdminTableButton.MaxTimer = Hacker.cooldown;
        vampireKillButton.MaxTimer = Vampire.cooldown;
        trackerTrackPlayerButton.MaxTimer = defaultMaxTimer;
        jumperButton.MaxTimer = Jumper.JumpTime;
        escapistButton.MaxTimer = Escapist.EscapeTime;
        bodyGuardGuardButton.MaxTimer = defaultMaxTimer;
        garlicButton.MaxTimer = defaultMaxTimer;
        jackalKillButton.MaxTimer = Jackal.cooldown;
        werewolfKillButton.MaxTimer = Werewolf.killCooldown;
        sidekickKillButton.MaxTimer = Sidekick.cooldown;
        jackalSidekickButton.MaxTimer = Jackal.createSidekickCooldown;
        eraserButton.MaxTimer = Eraser.cooldown;
        placeJackInTheBoxButton.MaxTimer = Trickster.placeBoxCooldown;
        lightsOutButton.MaxTimer = Trickster.lightsOutCooldown;
        cleanerCleanButton.MaxTimer = Cleaner.cooldown;
        undertakerDragButton.MaxTimer = defaultMaxTimer;
        warlockCurseButton.MaxTimer = Warlock.cooldown;
        securityGuardButton.MaxTimer = SecurityGuard.cooldown;
        securityGuardCamButton.MaxTimer = SecurityGuard.cooldown;
        arsonistButton.MaxTimer = Arsonist.cooldown;
        vultureEatButton.MaxTimer = Vulture.cooldown;
        amnisiacRememberButton.MaxTimer = defaultMaxTimer;
        bomberGiveButton.MaxTimer = 0f;
        bomberGiveButton.Timer = 0f;
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
        thiefKillButton.MaxTimer = Thief.cooldown;
        juggernautKillButton.MaxTimer = Juggernaut.cooldown;
        swooperKillButton.MaxTimer = Swooper.cooldown;
        evilTrapperSetTrapButton.MaxTimer = EvilTrapper.cooldown;

        doomsayerButton.MaxTimer = Doomsayer.cooldown;
        akujoHonmeiButton.MaxTimer = defaultMaxTimer;
        akujoBackupButton.MaxTimer = defaultMaxTimer;

        pavlovsdogsKillButton.MaxTimer = Pavlovsdogs.cooldown;
        pavlovsownerCreateDogButton.MaxTimer = Pavlovsdogs.createDogCooldown;

        mayorMeetingButton.MaxTimer = defaultMaxTimer;
        trapperButton.MaxTimer = Trapper.cooldown;
        terroristButton.MaxTimer = Terrorist.bombCooldown;
        hunterLighterButton.MaxTimer = Hunter.lightCooldown;
        hunterAdminTableButton.MaxTimer = Hunter.AdminCooldown;
        hunterArrowButton.MaxTimer = Hunter.ArrowCooldown;
        huntedShieldButton.MaxTimer = Hunted.shieldCooldown;
        defuseButton.MaxTimer = defaultMaxTimer;
        defuseButton.Timer = defaultMaxTimer;
        propDisguiseButton.MaxTimer = defaultMaxTimer;
        propHuntUnstuckButton.MaxTimer = PropHunt.unstuckCooldown;
        propHuntRevealButton.MaxTimer = PropHunt.revealCooldown;
        propHuntInvisButton.MaxTimer = PropHunt.invisCooldown;
        propHuntSpeedboostButton.MaxTimer = PropHunt.speedboostCooldown;
        propHuntAdminButton.MaxTimer = PropHunt.adminCooldown;
        propHuntFindButton.MaxTimer = PropHunt.findCooldown;

        timeMasterShieldButton.EffectDuration = TimeMaster.shieldDuration;
        veterenAlertButton.EffectDuration = Veteren.alertDuration;
        hackerButton.EffectDuration = Hacker.duration;
        hackerVitalsButton.EffectDuration = Hacker.duration;
        hackerAdminTableButton.EffectDuration = Hacker.duration;
        vampireKillButton.EffectDuration = Vampire.delay;
        werewolfRampageButton.MaxTimer = Werewolf.rampageCooldown;
        werewolfRampageButton.EffectDuration = Werewolf.rampageDuration;
        camouflagerButton.EffectDuration = Camouflager.duration;
        morphlingButton.EffectDuration = Morphling.duration;
        bomberBombButton.EffectDuration = Bomber.bombDelay + Bomber.bombTimer;
        lightsOutButton.EffectDuration = Trickster.lightsOutDuration;
        arsonistButton.EffectDuration = Arsonist.duration;
        mediumButton.EffectDuration = Medium.duration;
        trackerTrackCorpsesButton.EffectDuration = Tracker.corpsesTrackingDuration;
        witchSpellButton.EffectDuration = Witch.spellCastingDuration;
        securityGuardCamButton.EffectDuration = SecurityGuard.duration;
        hunterLighterButton.EffectDuration = Hunter.lightDuration;
        hunterArrowButton.EffectDuration = Hunter.ArrowDuration;
        huntedShieldButton.EffectDuration = Hunted.shieldDuration;
        defuseButton.EffectDuration = Terrorist.defuseDuration;
        terroristButton.EffectDuration = Terrorist.destructionTime + Terrorist.bombActiveAfter;
        propHuntUnstuckButton.EffectDuration = PropHunt.unstuckDuration;
        propHuntRevealButton.EffectDuration = PropHunt.revealDuration;
        propHuntInvisButton.EffectDuration = PropHunt.invisDuration;
        propHuntSpeedboostButton.EffectDuration = PropHunt.speedboostDuration;
        propHuntAdminButton.EffectDuration = PropHunt.adminDuration;
        propHuntFindButton.EffectDuration = PropHunt.findDuration;
        // Already set the timer to the max, as the button is enabled during the game and not available at the start
        lightsOutButton.Timer = lightsOutButton.MaxTimer;
        zoomOutButton.MaxTimer = 0f;
        //changeChatButton.MaxTimer = 0f;

    }

    public static void showTargetNameOnButton(PlayerControl target, CustomButton button, string defaultText)
    {
        Helpers.showTargetNameOnButton(target, button, defaultText);
    }


    public static void showTargetNameOnButtonExplicit(PlayerControl target, CustomButton button, string defaultText)
    {
        Helpers.showTargetNameOnButtonExplicit(target, button, defaultText);
    }

    public static void resetTimeMasterButton()
    {
        timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
        timeMasterShieldButton.isEffectActive = false;
        timeMasterShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        SoundEffectsManager.stop("timemasterShield");
    }

    public static void resetHuntedRewindButton()
    {
        huntedShieldButton.Timer = huntedShieldButton.MaxTimer;
        huntedShieldButton.isEffectActive = false;
        huntedShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        SoundEffectsManager.stop("timemasterShield");
    }

    private static void addReplacementHandcuffedButton(CustomButton button, Vector3? positionOffset = null,
        Func<bool> couldUse = null)
    {
        var positionOffsetValue =
            positionOffset ?? button.PositionOffset; // For non custom buttons, we can set these manually.
        positionOffsetValue.z = -0.1f;
        couldUse ??= button.CouldUse;
        var replacementHandcuffedButton = new CustomButton(() => { }, () => { return true; }, couldUse, () => { },
            Deputy.getHandcuffedButtonSprite(), positionOffsetValue, button.hudManager, button.hotkey,
            true, Deputy.handcuffDuration, () => { }, button.mirror);
        replacementHandcuffedButton.Timer = replacementHandcuffedButton.EffectDuration;
        replacementHandcuffedButton.actionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
        replacementHandcuffedButton.isEffectActive = true;
        replacementHandcuffedButton.hotkey = KeyCode.V;
        if (deputyHandcuffedButtons.ContainsKey(CachedPlayer.LocalPlayer.PlayerId))
            deputyHandcuffedButtons[CachedPlayer.LocalPlayer.PlayerId].Add(replacementHandcuffedButton);
        else
            deputyHandcuffedButtons.Add(CachedPlayer.LocalPlayer.PlayerId, [replacementHandcuffedButton]);
    }

    // Disables / Enables all Buttons (except the ones disabled in the Deputy class), and replaces them with new buttons.
    public static void setAllButtonsHandcuffedStatus(bool handcuffed, bool reset = false)
    {
        if (reset)
        {
            deputyHandcuffedButtons = [];
            return;
        }

        if (handcuffed && !deputyHandcuffedButtons.ContainsKey(CachedPlayer.LocalPlayer.PlayerId))
        {
            var maxI = buttons.Count;
            for (var i = 0; i < maxI; i++)
            {
                try
                {
                    if (buttons[i].HasButton()) // For each custombutton the player has
                        addReplacementHandcuffedButton(
                            buttons[i]); // The new buttons are the only non-handcuffed buttons now!
                    buttons[i].isHandcuffed = true;
                }
                catch (NullReferenceException)
                {
                    // Note: idk what this is good for, but i copied it from above /gendelo
                    Warn("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
                }
            }

            // Non Custom (Vanilla) Buttons. The Originals are disabled / hidden in UpdatePatch.cs already, just need to replace them. Can use any button, as we replace onclick etc anyways.
            // Kill Button if enabled for the Role
            if (FastDestroyableSingleton<HudManager>.Instance.KillButton.isActiveAndEnabled)
                addReplacementHandcuffedButton(arsonistButton, ButtonPositions.upperRowRight,
                    () => { return FastDestroyableSingleton<HudManager>.Instance.KillButton.currentTarget != null; });
            // Vent Button if enabled
            if (CachedPlayer.LocalPlayer.PlayerControl.roleCanUseVents())
                addReplacementHandcuffedButton(arsonistButton, ButtonPositions.upperRowCenter,
                    () =>
                    {
                        return FastDestroyableSingleton<HudManager>.Instance.ImpostorVentButton.currentTarget != null;
                    });
            // Report Button
            addReplacementHandcuffedButton(arsonistButton,
                !CachedPlayer.LocalPlayer.Data.Role.IsImpostor
                    ? new Vector3(-1f, -0.06f, 0)
                    : ButtonPositions.lowerRowRight,
                () =>
                {
                    return FastDestroyableSingleton<HudManager>.Instance.ReportButton.graphic.color ==
                           Palette.EnabledColor;
                });
        }
        else if (!handcuffed &&
                 deputyHandcuffedButtons.ContainsKey(CachedPlayer.LocalPlayer
                     .PlayerId)) // Reset to original. Disables the replacements, enables the original buttons.
        {
            foreach (var replacementButton in deputyHandcuffedButtons[CachedPlayer.LocalPlayer.PlayerId])
            {
                replacementButton.HasButton = () => { return false; };
                replacementButton.Update(); // To make it disappear properly.
                buttons.Remove(replacementButton);
            }

            deputyHandcuffedButtons.Remove(CachedPlayer.LocalPlayer.PlayerId);

            foreach (var button in buttons) button.isHandcuffed = false;
        }
    }

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

    public static void createRoleSummaryButton(HudManager __instance)
    {
        roleSummaryButton = new CustomButton(
        () =>
        {
            if (LobbyRoleInfo.RolesSummaryUI == null)
            {
                LobbyRoleInfo.RoleSummaryOnClick();
            }
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
                {
                    Object.Destroy(LobbyRoleInfo.RolesSummaryUI);
                }
            }
            return true;
        },
        () => { },
        loadSpriteFromResources("TheOtherRoles.Resources.HelpButton.png", 150f),
        new Vector3(0.4f, 3f, 0),
        __instance,
        null
        );
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
        createRoleSummaryButton(__instance);
        // Engineer Repair
        engineerRepairButton = new CustomButton(
            () =>
            {
                engineerRepairButton.Timer = 0f;
                var usedRepairWriter = AmongUsClient.Instance.StartRpcImmediately(
                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.EngineerUsedRepair,
                    SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(usedRepairWriter);
                RPCProcedure.engineerUsedRepair();
                SoundEffectsManager.play("engineerRepair");
                foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
                    if (task.TaskType == TaskTypes.FixLights)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(
                            CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.EngineerFixLights,
                            SendOption.Reliable);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.engineerFixLights();
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
                    else if (SubmergedCompatibility.IsSubmerged &&
                             task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(
                            CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.EngineerFixSubmergedOxygen,
                            SendOption.Reliable);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.engineerFixSubmergedOxygen();
                    }
            },
            () =>
            {
                return Engineer.engineer != null && Engineer.engineer == CachedPlayer.LocalPlayer.PlayerControl &&
                       Engineer.remainingFixes > 0 && Engineer.remoteFix && !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                var sabotageActive = false;
                foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
                    if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy ||
                        task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic ||
                        task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles
                        || (SubmergedCompatibility.IsSubmerged &&
                            task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                        sabotageActive = true;
                return sabotageActive && Engineer.remainingFixes > 0 &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove && !Engineer.usedFix;
            },
            () =>
            {
                if (Engineer.resetFixAfterMeeting) Engineer.resetFixes();
            },
            Engineer.getButtonSprite(),
            ButtonPositions.upperRowRight,
            __instance,
            KeyCode.F,
            buttonText: getString("RepairText")
        );

        //Sheriff Kill
        sheriffKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Sheriff.currentTarget)) return;
                var murderAttemptResult = checkMuderAttempt(Sheriff.sheriff, Sheriff.currentTarget);
                if (murderAttemptResult == MurderAttemptResult.SuppressKill) return;
                var target = Sheriff.currentTarget;
                if (murderAttemptResult == MurderAttemptResult.PerformKill)
                {
                    byte targetId = 0;
                    if ((target != Mini.mini || Mini.isGrownUp()) &&
                        (target.Data.Role.IsImpostor ||
                         Jackal.jackal == target ||
                         Sidekick.sidekick == target ||
                         Juggernaut.juggernaut == target ||
                         Werewolf.werewolf == target ||
                         Swooper.swooper == target ||
                         Pavlovsdogs.pavlovsowner == target ||
                         Pavlovsdogs.pavlovsdogs.Any(p => p == target) ||
                         (Sheriff.spyCanDieToSheriff && Spy.spy == target) ||
                         (Sheriff.canKillNeutrals &&
                          (Akujo.akujo == target || isKiller(target) ||
                          (Jester.jester == target && Sheriff.canKillJester) ||
                           (Vulture.vulture == target && Sheriff.canKillVulture) ||
                           (Thief.thief == target && Sheriff.canKillThief) ||
                           (Amnisiac.amnisiac == target && Sheriff.canKillAmnesiac) ||
                           (Lawyer.lawyer == target && Sheriff.canKillLawyer) ||
                           (Executioner.executioner == target && Sheriff.canKillExecutioner) ||
                           (Pursuer.pursuer == target && Sheriff.canKillPursuer) ||
                           (Doomsayer.doomsayer == target && Sheriff.canKillDoomsayer)))))
                    {
                        targetId = target.PlayerId;
                    }
                    else if (Sheriff.misfireKills == 0)
                    {
                        targetId = CachedPlayer.LocalPlayer.PlayerId;
                    }
                    else if (Sheriff.misfireKills == 1)
                    {
                        targetId = target.PlayerId;
                    }
                    else if (Sheriff.misfireKills == 2)
                    {
                        targetId = target.PlayerId;
                        var killWriter2 = AmongUsClient.Instance.StartRpcImmediately(
                            CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer,
                            SendOption.Reliable);
                        killWriter2.Write(Sheriff.sheriff.Data.PlayerId);
                        killWriter2.Write(CachedPlayer.LocalPlayer.PlayerId);
                        killWriter2.Write(byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(killWriter2);
                        RPCProcedure.uncheckedMurderPlayer(Sheriff.sheriff.Data.PlayerId,
                            CachedPlayer.LocalPlayer.PlayerId, byte.MaxValue);
                    }

                    var killWriter = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer,
                        SendOption.Reliable);
                    killWriter.Write(Sheriff.sheriff.Data.PlayerId);
                    killWriter.Write(targetId);
                    killWriter.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                    RPCProcedure.uncheckedMurderPlayer(Sheriff.sheriff.Data.PlayerId, targetId, byte.MaxValue);
                }

                if (murderAttemptResult == MurderAttemptResult.BodyGuardKill)
                    checkMurderAttemptAndKill(Sheriff.sheriff, target);

                sheriffKillButton.Timer = sheriffKillButton.MaxTimer;
                Sheriff.currentTarget = null;
            },
            () =>
            {
                return Sheriff.sheriff != null && Sheriff.sheriff == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Sheriff.currentTarget, sheriffKillButton, getString("killButtonText"));
                return Sheriff.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { sheriffKillButton.Timer = sheriffKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            KeyCode.Q
        );

        // Deputy Handcuff
        deputyHandcuffButton = new CustomButton(
            () =>
            {
                byte targetId = 0;
                var target = Sheriff.sheriff == CachedPlayer.LocalPlayer.PlayerControl
                    ? Sheriff.currentTarget
                    : Deputy.currentTarget; // If the deputy is now the sheriff, sheriffs target, else deputies target
                checkWatchFlash(target);
                targetId = target.PlayerId;
                if (checkAndDoVetKill(target)) return;
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.DeputyUsedHandcuffs, SendOption.Reliable);
                writer.Write(targetId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.deputyUsedHandcuffs(targetId);
                Deputy.currentTarget = null;
                deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer;

                SoundEffectsManager.play("deputyHandcuff");
            },
            () =>
            {
                return ((Deputy.deputy != null && Deputy.deputy == CachedPlayer.LocalPlayer.PlayerControl) ||
                        (Sheriff.sheriff != null && Sheriff.sheriff == CachedPlayer.LocalPlayer.PlayerControl &&
                         Sheriff.sheriff == Sheriff.formerDeputy && Deputy.keepsHandcuffsOnPromotion)) &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Deputy.currentTarget, deputyHandcuffButton, getString("HandcuffText"));
                if (deputyButtonHandcuffsText != null) deputyButtonHandcuffsText.text = $"{Deputy.remainingHandcuffs}";
                return ((Deputy.deputy != null && Deputy.deputy == CachedPlayer.LocalPlayer.PlayerControl &&
                         Deputy.currentTarget) ||
                        (Sheriff.sheriff != null && Sheriff.sheriff == CachedPlayer.LocalPlayer.PlayerControl &&
                         Sheriff.sheriff == Sheriff.formerDeputy && Sheriff.currentTarget)) &&
                       Deputy.remainingHandcuffs > 0 &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer; },
            Deputy.getButtonSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F
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
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.TimeMasterShield, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.timeMasterShield();
                SoundEffectsManager.play("timemasterShield");
            },
            () =>
            {
                return TimeMaster.timeMaster != null &&
                       TimeMaster.timeMaster == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {
                timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
                timeMasterShieldButton.isEffectActive = false;
                timeMasterShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            TimeMaster.getButtonSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            true,
            TimeMaster.shieldDuration,
            () =>
            {
                timeMasterShieldButton.Timer = timeMasterShieldButton.MaxTimer;
                SoundEffectsManager.stop("timemasterShield");
            },
            buttonText: getString("TimeShieldText")
        );

        // Veteren Alert
        veterenAlertButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.VeterenAlert, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.veterenAlert();
            },
            () =>
            {
                return Veteren.veteren != null && Veteren.veteren == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {
                veterenAlertButton.Timer = veterenAlertButton.MaxTimer;
                veterenAlertButton.isEffectActive = false;
                veterenAlertButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Veteren.getButtonSprite(),
            ButtonPositions.lowerRowRight, //brb
            __instance,
            KeyCode.F,
            true,
            Veteren.alertDuration,
            () => { veterenAlertButton.Timer = veterenAlertButton.MaxTimer; },
            buttonText: getString("AlertText")
        );

        // Medic Shield
        medicShieldButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Medic.currentTarget)) return;
                checkWatchFlash(Medic.currentTarget);

                medicShieldButton.Timer = 0f;

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
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
                return Medic.medic != null && Medic.medic == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Medic.currentTarget, medicShieldButton, getString("ShieldText"));
                return !Medic.usedShield && Medic.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {
                if (Medic.reset) Medic.resetShielded();
            },
            Medic.getButtonSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F
        );

        // doomsayer Shield
        doomsayerButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Doomsayer.currentTarget)) return;
                checkWatchFlash(Doomsayer.currentTarget);

                doomsayerButton.Timer = doomsayerButton.MaxTimer;
                /*
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.SetFutureReveal, SendOption.Reliable);
                writer.Write(Doomsayer.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setFutureReveal(Doomsayer.currentTarget.PlayerId);
                */
                SoundEffectsManager.play("knockKnock");
            },
            () =>
            {
                return Doomsayer.doomsayer != null && Doomsayer.doomsayer == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Doomsayer.currentTarget, doomsayerButton, getString("doomsayerText"));
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Doomsayer.currentTarget != null;
            },
            () => { doomsayerButton.Timer = doomsayerButton.MaxTimer; },
            Doomsayer.buttonSprite,
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            true,
            0f,
            () =>
            {
                doomsayerButton.Timer = doomsayerButton.MaxTimer;
                var msg = Doomsayer.GetInfo(Doomsayer.currentTarget);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer.PlayerControl, $"{msg}");

                // Ghost Info
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                writer.Write(Doomsayer.currentTarget.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.MediumInfo);
                writer.Write(msg);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        );

        // Akujo Honmei
        akujoHonmeiButton = new CustomButton(
            () =>
            {
                if (Veteren.veteren != null && Akujo.currentTarget == Veteren.veteren && Veteren.alertActive)
                {
                    checkMurderAttemptAndKill(Veteren.veteren, Akujo.akujo);
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.AkujoSetHonmei, SendOption.Reliable, -1);
                writer.Write(Akujo.akujo.PlayerId);
                writer.Write(Akujo.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.akujoSetHonmei(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, Akujo.currentTarget.PlayerId);
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Akujo.honmei == null && Akujo.timeLeft > 0; },
            () =>
            {
                return CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Akujo.currentTarget != null && Akujo.honmei == null && Akujo.timeLeft > 0;
            },
            () => { akujoHonmeiButton.Timer = akujoHonmeiButton.MaxTimer; },
            Akujo.getHonmeiSprite(),
            ButtonPositions.upperRowRight,
            __instance,
            KeyCode.F,
            buttonText: "(F)" + getString("AkujoHonmeiText")
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
                if (Veteren.veteren != null && Akujo.currentTarget == Veteren.veteren && Veteren.alertActive)
                {
                    checkMurderAttemptAndKill(Veteren.veteren, Akujo.akujo);
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.AkujoSetKeep, SendOption.Reliable, -1);
                writer.Write(Akujo.akujo.PlayerId);
                writer.Write(Akujo.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.akujoSetKeep(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, Akujo.currentTarget.PlayerId);
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Akujo.keepsLeft > 0; },
            () =>
            {
                if (akujoBackupLeftText != null)
                {
                    if (Akujo.keepsLeft > 0)
                        akujoBackupLeftText.text = Akujo.keepsLeft.ToString();
                    else
                        akujoBackupLeftText.text = "";
                }
                return CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Akujo.currentTarget != null && Akujo.keepsLeft > 0 && Akujo.timeLeft > 0;
            },
            () => { akujoBackupButton.Timer = akujoBackupButton.MaxTimer; },
            Akujo.getKeepSprite(),
            ButtonPositions.upperRowCenter,
            __instance,
            KeyCode.C,
            buttonText: "(C)" + getString("AkujoBackupText")
        );
        akujoBackupLeftText = Object.Instantiate(akujoBackupButton.actionButton.cooldownTimerText, akujoBackupButton.actionButton.cooldownTimerText.transform.parent);
        akujoBackupLeftText.text = "";
        akujoBackupLeftText.enableWordWrapping = false;
        akujoBackupLeftText.transform.localScale = Vector3.one * 0.5f;
        akujoBackupLeftText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        evilTrapperSetTrapButton = new CustomButton(
            () =>
            { // ボタンが押された時に実行
                if (!CachedPlayer.LocalPlayer.PlayerControl.CanMove || KillTrap.hasTrappedPlayer()) return;
                EvilTrapper.setTrap();
                evilTrapperSetTrapButton.Timer = evilTrapperSetTrapButton.MaxTimer;
            },
            () =>
            { /*ボタン有効になる条件*/
                return CachedPlayer.LocalPlayer.PlayerControl == EvilTrapper.evilTrapper && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead;
            },
            () =>
            { /*ボタンが使える条件*/
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !KillTrap.hasTrappedPlayer();
            },
            () =>
            { /*ミーティング終了時*/
                evilTrapperSetTrapButton.Timer = evilTrapperSetTrapButton.MaxTimer;
            },
            EvilTrapper.getTrapButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            buttonText: getString("PlaceTrapText")
        );

        // Shifter shift
        shifterShiftButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Shifter.currentTarget)) return;
                checkWatchFlash(Shifter.currentTarget);

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.SetFutureShifted, SendOption.Reliable);
                writer.Write(Shifter.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setFutureShifted(Shifter.currentTarget.PlayerId);
                SoundEffectsManager.play("shifterShift");
            },
            () =>
            {
                return Shifter.shifter != null && Shifter.shifter == CachedPlayer.LocalPlayer.PlayerControl &&
                       Shifter.futureShift == null && !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Shifter.currentTarget, shifterShiftButton, getString("ShiftText"));
                return Shifter.currentTarget && Shifter.futureShift == null &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { },
            Shifter.getButtonSprite(),
            new Vector3(0, 1f, 0),
            __instance,
            null,
            true
        );

        // Disperser disperse
        disperserDisperseButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.Disperse, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.disperse();
                SoundEffectsManager.play("shifterShift");
            },
            () =>
            {
                return Disperser.disperser != null && Disperser.disperser == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () => { return Disperser.remainingDisperses > 0 && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { if (Disperser.remainingDisperses > 0) disperserDisperseButton.Timer = disperserDisperseButton.MaxTimer; },
            Disperser.getButtonSprite(),
            new Vector3(0, 1f, 0),
            __instance,
            null,
            true,
            buttonText: getString("DisperseText")
        );

        mayorMeetingButton = new CustomButton(
            () =>
            {
                //CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                Mayor.remoteMeetingsLeft--;

                handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                handleBomberExplodeOnBodyReport();
                handleTrapperTrapOnBodyReport();
                RPCProcedure.uncheckedCmdReportDeadBody(CachedPlayer.LocalPlayer.PlayerId, byte.MaxValue);
                if (AmongUsClient.Instance.AmHost)
                {
                    Mayor.mayor.NoCheckStartMeeting(null, true);
                }
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.MayorMeeting, SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                mayorMeetingButton.Timer = 1f;
            },
            () =>
            {
                return Mayor.mayor != null && Mayor.mayor == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead && Mayor.meetingButton;
            },
            () =>
            {
                mayorMeetingButton.actionButton.OverrideText(getString("MayorButtonText") + "(" + Mayor.remoteMeetingsLeft + ")");
                var sabotageActive = false;
                foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
                    if ((task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor ||
                    task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles ||
                        (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)) && !Mayor.SabotageRemoteMeetings)
                        sabotageActive = true;
                return !sabotageActive && CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                       Mayor.remoteMeetingsLeft > 0;
            },
            () => { mayorMeetingButton.Timer = mayorMeetingButton.MaxTimer; },
            Mayor.getMeetingSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            true,
            0f,
            () => { },
            false,
            "Meeting"
        );

        // ButtonBarry Meetings
        buttonBarryButton = new CustomButton(
            () =>
            {
                //CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                ButtonBarry.remoteMeetingsLeft--;

                handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                handleBomberExplodeOnBodyReport();
                handleTrapperTrapOnBodyReport();
                RPCProcedure.uncheckedCmdReportDeadBody(CachedPlayer.LocalPlayer.PlayerId, byte.MaxValue);
                if (AmongUsClient.Instance.AmHost)
                {
                    ButtonBarry.buttonBarry.NoCheckStartMeeting(null, true);
                }
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.BarryMeeting, SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                buttonBarryButton.Timer = 1f;

            },
            () =>
            {
                return ButtonBarry.buttonBarry != null && ButtonBarry.buttonBarry == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                var sabotageActive = false;
                foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks.GetFastEnumerator())
                    if ((task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor ||
                    task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles ||
                        (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)) && !ButtonBarry.SabotageRemoteMeetings)
                        sabotageActive = true;
                return !sabotageActive && CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                       ButtonBarry.remoteMeetingsLeft > 0;
            },
            () => { buttonBarryButton.Timer = buttonBarryButton.MaxTimer; },
            ButtonBarry.getButtonSprite(),
            new Vector3(0, 1f, 0),
            __instance,
            null,
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
                    checkWatchFlash(Morphling.currentTarget);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.MorphlingMorph,
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
                    morphlingButton.Sprite = Morphling.getMorphSprite();
                    morphlingButton.EffectDuration = 1f;
                    SoundEffectsManager.play("morphlingSample");

                    // Add poolable player to the button so that the target outfit is shown
                    setButtonTargetDisplay(Morphling.sampledTarget, morphlingButton);
                }
            },
            () =>
            {
                return Morphling.morphling != null && Morphling.morphling == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (Morphling.sampledTarget == null)
                    showTargetNameOnButton(Morphling.currentTarget, morphlingButton, getString("SampleText"));
                return (Morphling.currentTarget || Morphling.sampledTarget) && !isActiveCamoComms() &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove && !MushroomSabotageActive();
            },
            () =>
            {
                morphlingButton.Timer = morphlingButton.MaxTimer;
                morphlingButton.Sprite = Morphling.getSampleSprite();
                morphlingButton.isEffectActive = false;
                morphlingButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Morphling.sampledTarget = null;
                setButtonTargetDisplay(null);
            },
            Morphling.getSampleSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            true,
            Morphling.duration,
            () =>
            {
                if (Morphling.sampledTarget == null)
                {
                    morphlingButton.Timer = morphlingButton.MaxTimer;
                    morphlingButton.Sprite = Morphling.getSampleSprite();
                    SoundEffectsManager.play("morphlingMorph");

                    // Reset the poolable player
                    setButtonTargetDisplay(null);
                }
            }
        );

        // Camouflager camouflage
        camouflagerButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.CamouflagerCamouflage, SendOption.Reliable);
                writer.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.camouflagerCamouflage(1);
                SoundEffectsManager.play("morphlingMorph");
            },
            () =>
            {
                return Camouflager.camouflager != null &&
                       Camouflager.camouflager == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () => { return !isActiveCamoComms() && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {
                camouflagerButton.Timer = camouflagerButton.MaxTimer;
                camouflagerButton.isEffectActive = false;
                camouflagerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Camouflager.getButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            true,
            Camouflager.duration,
            () =>
            {
                camouflagerButton.Timer = camouflagerButton.MaxTimer;
                SoundEffectsManager.play("morphlingMorph");
            }
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
                return Hacker.hacker != null && Hacker.hacker == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () => { return true; },
            () =>
            {
                hackerButton.Timer = hackerButton.MaxTimer;
                hackerButton.isEffectActive = false;
                hackerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Hacker.getButtonSprite(),
            ButtonPositions.upperRowRight,
            __instance,
            KeyCode.F,
            true,
            0f,
            () => { hackerButton.Timer = hackerButton.MaxTimer; },
            buttonText: getString("hackerButtonText")
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

                if (Hacker.cantMove) CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                Hacker.chargesAdminTable--;
            },
            () =>
            {
                return Hacker.hacker != null && Hacker.hacker == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (hackerAdminTableChargesText != null)
                    hackerAdminTableChargesText.text = $"{Hacker.chargesAdminTable} / {Hacker.toolsNumber}";
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
            KeyCode.Q,
            true,
            0f,
            () =>
            {
                hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
                if (!hackerVitalsButton.isEffectActive) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
            },
            GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3,
            getString("AdminMapText")
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

                if (Hacker.cantMove) CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 

                Hacker.chargesVitals--;
            },
            () =>
            {
                return Hacker.hacker != null && Hacker.hacker == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead &&
                       GameOptionsManager.Instance.currentGameOptions.MapId != 0 &&
                       GameOptionsManager.Instance.currentNormalGameOptions.MapId != 3;
            },
            () =>
            {
                if (hackerVitalsChargesText != null)
                    hackerVitalsChargesText.text = $"{Hacker.chargesVitals} / {Hacker.toolsNumber}";
                hackerVitalsButton.actionButton.graphic.sprite =
                    isMira() ? Hacker.getLogSprite() : Hacker.getVitalsSprite();
                hackerVitalsButton.actionButton.OverrideText(isMira() ? getString("hackerDoorLogText") : getString("hackerVitalText"));
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
            KeyCode.Q,
            true,
            0f,
            () =>
            {
                hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                if (!hackerAdminTableButton.isEffectActive) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                if (Minigame.Instance)
                {
                    if (isMira()) Hacker.doorLog.ForceClose();
                    else Hacker.vitals.ForceClose();
                }
            },
            false,
            isMira() ? getString("hackerDoorLogText") : getString("hackerVitalText")
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
                checkWatchFlash(Tracker.currentTarget);

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.TrackerUsedTracker, SendOption.Reliable);
                writer.Write(Tracker.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.trackerUsedTracker(Tracker.currentTarget.PlayerId);
                SoundEffectsManager.play("trackerTrackPlayer");
            },
            () =>
            {
                return Tracker.tracker != null && Tracker.tracker == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (!Tracker.usedTracker)
                    showTargetNameOnButton(Tracker.currentTarget, trackerTrackPlayerButton, getString("TrackerText"));
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Tracker.currentTarget != null &&
                       !Tracker.usedTracker;
            },
            () =>
            {
                if (Tracker.resetTargetAfterMeeting) Tracker.resetTracked();
            },
            Tracker.getButtonSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F
        );

        trackerTrackCorpsesButton = new CustomButton(
            () =>
            {
                Tracker.corpsesTrackingTimer = Tracker.corpsesTrackingDuration;
                SoundEffectsManager.play("trackerTrackCorpses");
            },
            () =>
            {
                return Tracker.tracker != null && Tracker.tracker == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead && Tracker.canTrackCorpses;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {
                trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer;
                trackerTrackCorpsesButton.isEffectActive = false;
                trackerTrackCorpsesButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Tracker.getTrackCorpsesButtonSprite(),
            ButtonPositions.lowerRowCenter,
            __instance,
            KeyCode.Q,
            true,
            Tracker.corpsesTrackingDuration,
            () => { trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer; },
            buttonText: getString("TrackerCorpsesText")
        );

        bodyGuardGuardButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(BodyGuard.currentTarget)) return;
                checkWatchFlash(BodyGuard.currentTarget);
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.BodyGuardGuardPlayer, SendOption.Reliable);
                writer.Write(BodyGuard.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.bodyGuardGuardPlayer(BodyGuard.currentTarget.PlayerId);
                // SoundEffectsManager.play("trackerTrackPlayer");
            },
            () =>
            {
                return BodyGuard.bodyguard != null && BodyGuard.bodyguard == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (!BodyGuard.usedGuard)
                    showTargetNameOnButton(BodyGuard.currentTarget, bodyGuardGuardButton, getString("bodyGuardText"));
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && BodyGuard.currentTarget != null &&
                       !BodyGuard.usedGuard;
            },
            () =>
            {
                if (BodyGuard.reset) BodyGuard.resetGuarded();
            },
            BodyGuard.getGuardButtonSprite(),
            ButtonPositions.lowerRowRight, //brb
            __instance,
            KeyCode.F
        );

        privateInvestigatorWatchButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(PrivateInvestigator.currentTarget)) return;
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.PrivateInvestigatorWatchPlayer, SendOption.Reliable);
                writer.Write(PrivateInvestigator.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.privateInvestigatorWatchPlayer(PrivateInvestigator.currentTarget.PlayerId);
                // SoundEffectsManager.play("trackerTrackPlayer");
            },
            () =>
            {
                return PrivateInvestigator.privateInvestigator != null &&
                       PrivateInvestigator.privateInvestigator == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (PrivateInvestigator.watching == null)
                    showTargetNameOnButton(PrivateInvestigator.currentTarget, privateInvestigatorWatchButton, getString("PrivateInvestigatorWatchText"));
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && PrivateInvestigator.currentTarget != null &&
                       PrivateInvestigator.watching == null;
            },
            () => { PrivateInvestigator.watching = null; },
            PrivateInvestigator.getButtonSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F
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
                        var writer = AmongUsClient.Instance.StartRpcImmediately(
                            CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer,
                            SendOption.Reliable);
                        writer.Write(Vampire.vampire.PlayerId);
                        writer.Write(Vampire.currentTarget.PlayerId);
                        writer.Write(byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.uncheckedMurderPlayer(Vampire.vampire.PlayerId, Vampire.currentTarget.PlayerId,
                            byte.MaxValue);

                        vampireKillButton.HasEffect = false; // Block effect on this click
                        vampireKillButton.Timer = vampireKillButton.MaxTimer;
                    }
                    else
                    {
                        Vampire.bitten = Vampire.currentTarget;
                        // Notify players about bitten
                        var writer = AmongUsClient.Instance.StartRpcImmediately(
                            CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.VampireSetBitten,
                            SendOption.Reliable);
                        writer.Write(Vampire.bitten.PlayerId);
                        writer.Write((byte)0);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
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
                                        var writer = AmongUsClient.Instance.StartRpcImmediately(
                                            CachedPlayer.LocalPlayer.PlayerControl.NetId,
                                            (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                                        writer.Write((byte)RPCProcedure.GhostInfoTypes.VampireTimer);
                                        writer.Write(timer);
                                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    }
                                }

                                if (p == 1f)
                                {
                                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                    var res = checkMurderAttemptAndKill(Vampire.vampire, Vampire.bitten,
                                        showAnimation: false);
                                    if (res == MurderAttemptResult.PerformKill)
                                    {
                                        var writer = AmongUsClient.Instance.StartRpcImmediately(
                                            CachedPlayer.LocalPlayer.PlayerControl.NetId,
                                            (byte)CustomRPC.VampireSetBitten, SendOption.Reliable);
                                        writer.Write(byte.MaxValue);
                                        writer.Write(byte.MaxValue);
                                        AmongUsClient.Instance.FinishRpcImmediately(writer);
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
                return Vampire.vampire != null && Vampire.vampire == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (Vampire.targetNearGarlic)
                    showTargetNameOnButton(Vampire.currentTarget, vampireKillButton, getString("killButtonText"));
                else
                    showTargetNameOnButton(Vampire.currentTarget, vampireKillButton, getString("VampireText"));
                if (Vampire.targetNearGarlic && Vampire.canKillNearGarlics)
                {
                    vampireKillButton.actionButton.graphic.sprite = __instance.KillButton.graphic.sprite;
                    vampireKillButton.showButtonText = true;
                }
                else
                {
                    vampireKillButton.actionButton.graphic.sprite = Vampire.getButtonSprite();
                    vampireKillButton.showButtonText = false;
                }

                return Vampire.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                       (!Vampire.targetNearGarlic || Vampire.canKillNearGarlics);
            },
            () =>
            {
                vampireKillButton.Timer = vampireKillButton.MaxTimer;
                vampireKillButton.isEffectActive = false;
                vampireKillButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Vampire.getButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.Q,
            false,
            0f,
            () => { vampireKillButton.Timer = vampireKillButton.MaxTimer; }
        );

        garlicButton = new CustomButton(
            () =>
            {
                Vampire.localPlacedGarlic = true;
                var pos = CachedPlayer.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.PlaceGarlic);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.placeGarlic(buff);
                SoundEffectsManager.play("garlic");
            },
            () =>
            {
                return Vampire.garlicButton && !Vampire.localPlacedGarlic && !CachedPlayer.LocalPlayer.Data.IsDead &&
                       Vampire.garlicsActive && !HideNSeek.isHideNSeekGM && !PropHunt.isPropHuntGM;
            },
            () =>
            {
                return Vampire.garlicButton && CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                       !Vampire.localPlacedGarlic;
            },
            () => { },
            Vampire.getGarlicButtonSprite(),
            new Vector3(0, 0f, 0),
            __instance,
            null,
            true,
            buttonText: getString("GarlicText")
        );

        prophetButton = new CustomButton(
                () =>
                {
                    if (checkAndDoVetKill(Prophet.currentTarget)) return;
                    if (Prophet.currentTarget != null)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ProphetExamine, SendOption.Reliable, -1);
                        writer.Write(Prophet.currentTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.prophetExamine(Prophet.currentTarget.PlayerId);
                        prophetButton.Timer = prophetButton.MaxTimer;
                    }
                },
                () => { return Prophet.prophet != null && CachedPlayer.LocalPlayer.PlayerControl == Prophet.prophet && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Prophet.examinesLeft > 0; },
                () =>
                {
                    if (prophetButtonText != null)
                    {
                        if (Prophet.examinesLeft > 0)
                            prophetButtonText.text = $"{Prophet.examinesLeft}";
                        else
                            prophetButtonText.text = "";
                    }
                    return Prophet.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => { prophetButton.Timer = prophetButton.MaxTimer; },
                Prophet.getButtonSprite(),
                ButtonPositions.lowerRowRight,
                __instance,
                KeyCode.F,
                buttonText: getString("ProphetText")
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

                var pos = CachedPlayer.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.PlacePortal);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.placePortal(buff);
                SoundEffectsManager.play("tricksterPlaceBox");
            },
            () =>
            {
                return Portalmaker.portalmaker != null &&
                       Portalmaker.portalmaker == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead && Portal.secondPortal == null;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Portal.secondPortal == null; },
            () => { portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer; },
            Portalmaker.getPlacePortalButtonSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            buttonText: getString("PlacePortalText")
        );

        usePortalButton = new CustomButton(
            () =>
            {
                var didTeleport = false;
                Vector3 exit = Portal.findExit(CachedPlayer.LocalPlayer.transform.position);
                Vector3 entry = Portal.findEntry(CachedPlayer.LocalPlayer.transform.position);

                var portalMakerSoloTeleport = !Portal.locationNearEntry(CachedPlayer.LocalPlayer.transform.position);
                if (portalMakerSoloTeleport)
                {
                    exit = Portal.firstPortal.portalGameObject.transform.position;
                    entry = CachedPlayer.LocalPlayer.transform.position;
                }

                CachedPlayer.LocalPlayer.NetTransform.RpcSnapTo(entry);

                if (!CachedPlayer.LocalPlayer.Data.IsDead)
                {
                    // Ghosts can portal too, but non-blocking and only with a local animation
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UsePortal, SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write(portalMakerSoloTeleport ? (byte)1 : (byte)0);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                RPCProcedure.usePortal(CachedPlayer.LocalPlayer.PlayerId, portalMakerSoloTeleport ? (byte)1 : (byte)0);
                usePortalButton.Timer = usePortalButton.MaxTimer;
                portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer;
                SoundEffectsManager.play("portalUse");
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Portal.teleportDuration,
                    new Action<float>(p =>
                    {
                        // Delayed action
                        CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                        CachedPlayer.LocalPlayer.NetTransform.Halt();
                        if (p >= 0.5f && p <= 0.53f && !didTeleport && !MeetingHud.Instance)
                        {
                            if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(exit.y > -7);
                            CachedPlayer.LocalPlayer.NetTransform.RpcSnapTo(exit);
                            didTeleport = true;
                        }

                        if (p == 1f) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                    })));
            },
            () =>
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == Portalmaker.portalmaker && Portal.bothPlacedAndEnabled)
                    portalmakerButtonText1.text =
                        Portal.locationNearEntry(CachedPlayer.LocalPlayer.transform.position) ||
                        !Portalmaker.canPortalFromAnywhere
                            ? ""
                            : "1. " + Portal.firstPortal.room;
                return Portal.bothPlacedAndEnabled;
            },
            () =>
            {
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                       (Portal.locationNearEntry(CachedPlayer.LocalPlayer.transform.position) ||
                        (Portalmaker.canPortalFromAnywhere &&
                         CachedPlayer.LocalPlayer.PlayerControl == Portalmaker.portalmaker)) && !Portal.isTeleporting;
            },
            () => { usePortalButton.Timer = usePortalButton.MaxTimer; },
            Portalmaker.getUsePortalButtonSprite(),
            new Vector3(1f, 0f, 0),
            __instance,
            KeyCode.H,
            true,
            buttonText: getString("usePortalText")
        );

        portalmakerMoveToPortalButton = new CustomButton(
            () =>
            {
                var didTeleport = false;
                var exit = Portal.secondPortal.portalGameObject.transform.position;

                if (!CachedPlayer.LocalPlayer.Data.IsDead)
                {
                    // Ghosts can portal too, but non-blocking and only with a local animation
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UsePortal, SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write((byte)2);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                RPCProcedure.usePortal(CachedPlayer.LocalPlayer.PlayerId, 2);
                usePortalButton.Timer = usePortalButton.MaxTimer;
                portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer;
                SoundEffectsManager.play("portalUse");
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Portal.teleportDuration,
                    new Action<float>(p =>
                    {
                        // Delayed action
                        CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                        CachedPlayer.LocalPlayer.NetTransform.Halt();
                        if (p >= 0.5f && p <= 0.53f && !didTeleport && !MeetingHud.Instance)
                        {
                            if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(exit.y > -7);
                            CachedPlayer.LocalPlayer.NetTransform.RpcSnapTo(exit);
                            didTeleport = true;
                        }

                        if (p == 1f) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                    })));
            },
            () =>
            {
                return Portalmaker.canPortalFromAnywhere && Portal.bothPlacedAndEnabled &&
                       CachedPlayer.LocalPlayer.PlayerControl == Portalmaker.portalmaker;
            },
            () =>
            {
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                       !Portal.locationNearEntry(CachedPlayer.LocalPlayer.transform.position) && !Portal.isTeleporting;
            },
            () => { portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer; },
            Portalmaker.getUsePortalButtonSprite(),
            new Vector3(1f, 1f, 0),
            __instance,
            KeyCode.J,
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


        // Jackal Sidekick Button
        jackalSidekickButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Jackal.currentTarget)) return;
                checkWatchFlash(Jackal.currentTarget);
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.JackalCreatesSidekick, SendOption.Reliable);
                writer.Write(Jackal.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.jackalCreatesSidekick(Jackal.currentTarget.PlayerId);
                SoundEffectsManager.play("jackalSidekick");
            },
            () =>
            {
                return Jackal.canCreateSidekick && Jackal.jackal != null &&
                       Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Jackal.currentTarget, jackalSidekickButton, getString("jackalSidekickText")); // Show now text since the button already says sidekick
                return Jackal.canCreateSidekick && Jackal.currentTarget != null &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { jackalSidekickButton.Timer = jackalSidekickButton.MaxTimer; },
            Jackal.SidekickButton,
            ButtonPositions.lowerRowCenter,
            __instance,
            KeyCode.F
        );

        // Jackal Kill
        jackalKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Jackal.currentTarget)) return;
                if (checkMurderAttemptAndKill(Jackal.jackal, Jackal.currentTarget) ==
                    MurderAttemptResult.SuppressKill) return;

                jackalKillButton.Timer = jackalKillButton.MaxTimer;
                Jackal.currentTarget = null;
            },
            () =>
            {
                return Jackal.jackal != null && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Jackal.currentTarget, jackalKillButton, getString("killButtonText"));
                return Jackal.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { jackalKillButton.Timer = jackalKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            //CustomButton.ButtonPositions.upperRowRight,
            ButtonPositions.upperRowCenter,
            __instance,
            KeyCode.Q
        );

        jackalSwoopButton = new CustomButton(
            () =>
            { /* On Use */
                MessageWriter invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetJackalSwoop, SendOption.Reliable, -1);
                invisibleWriter.Write(Jackal.jackal.PlayerId);
                invisibleWriter.Write(byte.MinValue);
                AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                RPCProcedure.setJackalSwoop(Jackal.jackal.PlayerId, byte.MinValue);
            },
            () =>
            {   /* Can See */
                return Jackal.jackal != null && Jackal.canSwoop &&
                       Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {   /* On Click */
                return Jackal.canSwoop && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {  /* On Meeting End */
                jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer;
                jackalSwoopButton.isEffectActive = false;
                jackalSwoopButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Jackal.isInvisable = false;
            },
            Swooper.SwoopButton,
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            true,
            Jackal.duration,
            () => { jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer; },
            buttonText: getString("SwoopText")
        );

        // Sidekick Kill
        sidekickKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Sidekick.currentTarget)) return;
                if (checkMurderAttemptAndKill(Sidekick.sidekick, Sidekick.currentTarget) ==
                    MurderAttemptResult.SuppressKill) return;
                sidekickKillButton.Timer = sidekickKillButton.MaxTimer;
                Sidekick.currentTarget = null;
            },
            () =>
            {
                return Sidekick.canKill && Sidekick.sidekick != null &&
                       Sidekick.sidekick == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Sidekick.currentTarget, sidekickKillButton, getString("killButtonText"));
                return Sidekick.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { sidekickKillButton.Timer = sidekickKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            //CustomButton.ButtonPositions.upperRowRight,
            ButtonPositions.upperRowCenter,
            __instance,
            KeyCode.Q
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
            () => { return Swooper.swooper != null && Swooper.swooper == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
            () => { showTargetNameOnButton(Swooper.currentTarget, swooperKillButton, getString("killButtonText")); return Swooper.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { swooperKillButton.Timer = swooperKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowCenter,
            //new Vector3(0, 1f, 0),
            __instance,
            KeyCode.Q
        );

        swooperSwoopButton = new CustomButton(
            () =>
            { /* On Use */
                MessageWriter invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetSwoop, SendOption.Reliable, -1);
                invisibleWriter.Write(Swooper.swooper.PlayerId);
                invisibleWriter.Write(byte.MinValue);
                AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                RPCProcedure.setSwoop(Swooper.swooper.PlayerId, byte.MinValue);
            },
            () => { /* Can See */ return Swooper.swooper != null && Swooper.swooper == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
            () => {  /* On Click */ return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {  /* On Meeting End */
                swooperSwoopButton.Timer = swooperSwoopButton.MaxTimer;
                swooperSwoopButton.isEffectActive = false;
                swooperSwoopButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Swooper.isInvisable = false;
            },
            Swooper.SwoopButton,
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            true,
            Swooper.duration,
            () => { swooperSwoopButton.Timer = swooperSwoopButton.MaxTimer; },
            buttonText: getString("SwoopText")
        );

        pavlovsdogsKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Pavlovsdogs.killTarget)) return;
                if (checkMurderAttemptAndKill(CachedPlayer.LocalPlayer.PlayerControl, Pavlovsdogs.killTarget) == MurderAttemptResult.SuppressKill) return;
                if (Pavlovsdogs.enableRampage)
                {
                    Pavlovsdogs.deathTime = Pavlovsdogs.rampageDeathTime;
                    pavlovsdogsKillButton.MaxTimer = Pavlovsdogs.ownerIsDead ? Pavlovsdogs.rampageKillCooldown : Pavlovsdogs.cooldown;
                }
                pavlovsdogsKillButton.Timer = pavlovsdogsKillButton.MaxTimer;
                Pavlovsdogs.killTarget = null;
            },
            () =>
            {
                return Pavlovsdogs.pavlovsdogs != null
                       && Pavlovsdogs.pavlovsdogs.Any(x => x == CachedPlayer.LocalPlayer.PlayerControl)
                       && !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (Pavlovsdogs.enableRampage && Pavlovsdogs.ownerIsDead && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
                {
                    Pavlovsdogs.deathTime -= Time.deltaTime;
                    if (PavlovsdogKillSelfText != null)
                    {
                        PavlovsdogKillSelfText.text = string.Format("SerialKillerSuicideText".Translate(), ((int)Pavlovsdogs.deathTime) + 1);
                    }

                    if (Pavlovsdogs.deathTime <= 0)
                    {
                        PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer, true);
                    }
                }
                showTargetNameOnButton(Pavlovsdogs.killTarget, pavlovsdogsKillButton, getString("killButtonText")); return Pavlovsdogs.killTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {
                if (Pavlovsdogs.enableRampage && Pavlovsdogs.rampageDeathTimeIsMeetingReset)
                {
                    Pavlovsdogs.deathTime = Pavlovsdogs.rampageDeathTime;
                    pavlovsdogsKillButton.MaxTimer = Pavlovsdogs.ownerIsDead ? Pavlovsdogs.rampageKillCooldown : Pavlovsdogs.cooldown;
                }
                pavlovsdogsKillButton.Timer = pavlovsdogsKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowCenter,
            __instance,
            KeyCode.Q
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
                checkWatchFlash(Pavlovsdogs.currentTarget);
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.PavlovsCreateDog, SendOption.Reliable);
                writer.Write(Pavlovsdogs.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.pavlovsCreateDog(Pavlovsdogs.currentTarget.PlayerId);
                SoundEffectsManager.play("jackalSidekick");
                Pavlovsdogs.createDogNum -= 1;
                pavlovsownerCreateDogButton.Timer = pavlovsownerCreateDogButton.MaxTimer;
            },
            () =>
            {
                return Pavlovsdogs.pavlovsowner != null
                    && Pavlovsdogs.pavlovsowner == CachedPlayer.LocalPlayer.PlayerControl
                    && !CachedPlayer.LocalPlayer.Data.IsDead
                    && Pavlovsdogs.CanCreateDog;
            },
            () =>
            {
                showTargetNameOnButton(Pavlovsdogs.currentTarget, pavlovsownerCreateDogButton, getString("pavlovsCreateDogText"));
                // Show now text since the button already says sidekick
                return Pavlovsdogs.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { pavlovsownerCreateDogButton.Timer = pavlovsownerCreateDogButton.MaxTimer; },
            Pavlovsdogs.CreateDogButton,
            ButtonPositions.lowerRowCenter,
            __instance,
            KeyCode.F
        );

        minerMineButton = new CustomButton(
            () =>
            {
                /* On Use */
                minerMineButton.Timer = minerMineButton.MaxTimer;

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.Mine, SendOption.Reliable);
                var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var id = getAvailableId();
                writer.Write(id);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);


                writer.WriteBytesAndSize(buff);


                writer.Write(0.01f);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.Mine(id, Miner.miner, buff, 0.01f);
            },
            () =>
            {
                /* Can See */
                return Miner.miner != null && Miner.miner == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* Can Use */
                var hits = Physics2D.OverlapBoxAll(CachedPlayer.LocalPlayer.PlayerControl.transform.position,
                    Miner.VentSize, 0);
                hits = hits.ToArray().Where(c =>
                        (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                    .ToArray();
                return hits.Count == 0 && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {
                /* On Meeting End */
                minerMineButton.Timer = minerMineButton.MaxTimer;
            },
            Miner.getMineButtonSprite(),
            ButtonPositions.upperRowLeft, //brb
            __instance,
            KeyCode.F,
            buttonText: getString("minerText")
        );

        bomberBombButton = new CustomButton(
            () =>
            {
                /* On Use */
                if (checkAndDoVetKill(Bomber.currentTarget)) return;
                checkWatchFlash(Bomber.currentTarget);
                var bombWriter = AmongUsClient.Instance.StartRpcImmediately(
                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.GiveBomb, SendOption.Reliable);
                bombWriter.Write(Bomber.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(bombWriter);
                RPCProcedure.giveBomb(Bomber.currentTarget.PlayerId);
                Bomber.bomber.killTimer = Bomber.bombTimer + Bomber.bombDelay;
                bomberBombButton.Timer = bomberBombButton.MaxTimer;
            },
            () =>
            {
                /* Can See */
                return Bomber.bomber != null && Bomber.bomber == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* On Click */
                return Bomber.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {
                /* On Meeting End */
                bomberBombButton.Timer = bomberBombButton.MaxTimer;
                bomberBombButton.isEffectActive = false;
                bomberBombButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Bomber.hasBomb = null;
            },
            Bomber.getButtonSprite(),
            ButtonPositions.upperRowLeft, //brb
            __instance,
            KeyCode.F
        );

        bomberGiveButton = new CustomButton(
            () =>
            {
                /* On Use */
                if (Bomber.currentBombTarget == Bomber.bomber)
                {
                    var killWriter = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer,
                        SendOption.Reliable);
                    killWriter.Write(Bomber.bomber.Data.PlayerId);
                    killWriter.Write(Bomber.hasBomb.Data.PlayerId);
                    killWriter.Write(0);
                    AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                    RPCProcedure.uncheckedMurderPlayer(Bomber.bomber.Data.PlayerId, Bomber.hasBomb.Data.PlayerId, 0);

                    var bombWriter1 = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.GiveBomb, SendOption.Reliable);
                    bombWriter1.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(bombWriter1);
                    RPCProcedure.giveBomb(byte.MaxValue);
                    return;
                }

                if (checkAndDoVetKill(Bomber.currentBombTarget)) return;
                if (checkMurderAttemptAndKill(Bomber.hasBomb, Bomber.currentBombTarget) ==
                    MurderAttemptResult.SuppressKill) return;
                var bombWriter = AmongUsClient.Instance.StartRpcImmediately(
                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.GiveBomb, SendOption.Reliable);
                bombWriter.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(bombWriter);
                RPCProcedure.giveBomb(byte.MaxValue);
            },
            () =>
            {
                /* Can See */
                return Bomber.bomber != null && Bomber.hasBomb == CachedPlayer.LocalPlayer.PlayerControl &&
                       Bomber.bombActive && !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* Can Click */
                return Bomber.currentBombTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {
                /* On Meeting End */
            },
            Bomber.getButtonSprite(),
            //          0, -0.06f, 0
            new Vector3(-4.5f, 1.5f, 0),
            __instance,
            KeyCode.Q
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
                return Werewolf.werewolf != null && Werewolf.werewolf == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead && Werewolf.canKill;
            },
            () =>
            {
                showTargetNameOnButton(Werewolf.currentTarget, werewolfKillButton, getString("killButtonText"));
                return Werewolf.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { werewolfKillButton.Timer = werewolfKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            new Vector3(0, 1f, 0),
            __instance,
            KeyCode.Q
        );

        werewolfRampageButton = new CustomButton(
            () =>
            {
                Werewolf.canKill = true;
                Werewolf.hasImpostorVision = true;
                werewolfKillButton.Timer = 0f;
            },
            () =>
            {
                /* Can See */
                return Werewolf.werewolf != null && Werewolf.werewolf == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* On Click */
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove;
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
            },
            Werewolf.getRampageButtonSprite(),
            ButtonPositions.lowerRowRight, //brb
            __instance,
            KeyCode.F,
            true,
            Werewolf.rampageDuration,
            () =>
            {
                werewolfRampageButton.Timer = werewolfRampageButton.MaxTimer;
                Werewolf.canKill = false;
                Werewolf.hasImpostorVision = false;
            }
        );

        // 天启击杀 Kill
        juggernautKillButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Juggernaut.currentTarget)) return;
                if (checkMurderAttemptAndKill(Juggernaut.juggernaut, Juggernaut.currentTarget) ==
                    MurderAttemptResult.SuppressKill) return;
                if (juggernautKillButton.MaxTimer >= 0f)
                {
                    Juggernaut.setkill();
                    juggernautKillButton.MaxTimer = Juggernaut.cooldown;
                }

                juggernautKillButton.Timer = juggernautKillButton.MaxTimer;
                Juggernaut.currentTarget = null;
            },
            () =>
            {
                return Juggernaut.juggernaut != null &&
                       Juggernaut.juggernaut == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Juggernaut.currentTarget, juggernautKillButton, getString("killButtonText"));
                return Juggernaut.currentTarget && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { juggernautKillButton.Timer = juggernautKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            new Vector3(0, 1f, 0),
            __instance,
            KeyCode.Q
        );

        // Eraser erase button
        eraserButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Eraser.currentTarget)) return;
                checkWatchFlash(Eraser.currentTarget);
                eraserButton.MaxTimer += 10;
                eraserButton.Timer = eraserButton.MaxTimer;

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.SetFutureErased, SendOption.Reliable);
                writer.Write(Eraser.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setFutureErased(Eraser.currentTarget.PlayerId);
                SoundEffectsManager.play("eraserErase");
            },
            () =>
            {
                return Eraser.eraser != null && Eraser.eraser == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Eraser.currentTarget, eraserButton, getString("EraserText"));
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Eraser.currentTarget != null;
            },
            () => { eraserButton.Timer = eraserButton.MaxTimer; },
            Eraser.getButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F
        );

        placeJackInTheBoxButton = new CustomButton(
            () =>
            {
                placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;

                var pos = CachedPlayer.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.PlaceJackInTheBox);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.placeJackInTheBox(buff);
                SoundEffectsManager.play("tricksterPlaceBox");
            },
            () =>
            {
                return Trickster.trickster != null && Trickster.trickster == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead && !JackInTheBox.hasJackInTheBoxLimitReached();
            },
            () =>
            {
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !JackInTheBox.hasJackInTheBoxLimitReached();
            },
            () => { placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer; },
            Trickster.getPlaceBoxButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            buttonText: getString("TricksterPlaceText")
        );

        lightsOutButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.LightsOut, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.lightsOut();
                SoundEffectsManager.play("lighterLight");
            },
            () =>
            {
                return Trickster.trickster != null && Trickster.trickster == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead
                       && JackInTheBox.hasJackInTheBoxLimitReached() && JackInTheBox.boxesConvertedToVents;
            },
            () =>
            {
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && JackInTheBox.hasJackInTheBoxLimitReached() &&
                       JackInTheBox.boxesConvertedToVents;
            },
            () =>
            {
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
                lightsOutButton.isEffectActive = false;
                lightsOutButton.actionButton.graphic.color = Palette.EnabledColor;
            },
            Trickster.getLightsOutButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            true,
            Trickster.lightsOutDuration,
            () =>
            {
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
                SoundEffectsManager.play("lighterLight");
            },
            buttonText: getString("LightsOutText")
        );

        // Cleaner Clean
        cleanerCleanButton = new CustomButton(
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(),
                             CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            var truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
                            var truePosition2 = component.TruePosition;
                            if (Vector2.Distance(truePosition2, truePosition) <=
                                CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance &&
                                CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                                !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                    Constants.ShipAndObjectsMask, false))
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                var writer = AmongUsClient.Instance.StartRpcImmediately(
                                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CleanBody,
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
                return Cleaner.cleaner != null && Cleaner.cleaner == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                return __instance.ReportButton.graphic.color == Palette.EnabledColor &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer; },
            Cleaner.getButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            buttonText: getString("CleanText")
        );

        // Cleaner Clean
        undertakerDragButton = new CustomButton(
            () =>
            {
                if (Undertaker.deadBodyDraged == null)
                {
                    foreach (var collider2D in Physics2D.OverlapCircleAll(
                                 CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(),
                                 CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
                        if (collider2D.tag == "DeadBody")
                        {
                            var deadBody = collider2D.GetComponent<DeadBody>();
                            if (deadBody && !deadBody.Reported)
                            {
                                var playerPosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
                                var deadBodyPosition = deadBody.TruePosition;
                                if (Vector2.Distance(deadBodyPosition, playerPosition) <=
                                    CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance &&
                                    CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                                    !PhysicsHelpers.AnythingBetween(playerPosition, deadBodyPosition,
                                        Constants.ShipAndObjectsMask, false) && !Undertaker.isDraging)
                                {
                                    var playerInfo = GameData.Instance.GetPlayerById(deadBody.ParentId);
                                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DragBody,
                                        SendOption.Reliable);
                                    writer.Write(playerInfo.PlayerId);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    RPCProcedure.dragBody(playerInfo.PlayerId);
                                    Undertaker.deadBodyDraged = deadBody;
                                    break;
                                }
                            }
                        }
                }
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DropBody, SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Undertaker.deadBodyDraged = null;
                }
            },
            () =>
            {
                return Undertaker.undertaker != null &&
                       Undertaker.undertaker == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (Undertaker.deadBodyDraged != null) return true;

                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(),
                             CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var deadBody = collider2D.GetComponent<DeadBody>();
                        var deadBodyPosition = deadBody.TruePosition;
                        deadBodyPosition.x -= 0.2f;
                        deadBodyPosition.y -= 0.2f;
                        return CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                               Vector2.Distance(CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(),
                                   deadBodyPosition) < 0.80f;
                    }

                return false;
            },
            //() => { return ((__instance.ReportButton.renderer.color == Palette.EnabledColor && CachedPlayer.LocalPlayer.PlayerControl.CanMove) || Undertaker.deadBodyDraged != null); },
            () => { },
            Undertaker.getButtonSprite(),
            ButtonPositions.upperRowLeft, //brb
            __instance,
            KeyCode.F,
            true,
            0f,
            () => { },
            buttonText: getString("DragBodyText")
        );

        // Warlock curse
        warlockCurseButton = new CustomButton(
            () =>
            {
                if (Warlock.curseVictim == null)
                {
                    if (checkAndDoVetKill(Warlock.currentTarget)) return;
                    checkWatchFlash(Warlock.currentTarget);
                    // Apply Curse
                    Warlock.curseVictim = Warlock.currentTarget;
                    warlockCurseButton.Sprite = Warlock.getCurseKillButtonSprite();
                    warlockCurseButton.Timer = 1f;
                    SoundEffectsManager.play("warlockCurse");

                    // Ghost Info
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo,
                        SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
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
                        AntiTeleport.position = CachedPlayer.LocalPlayer.transform.position;
                        CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                        CachedPlayer.LocalPlayer.NetTransform
                            .Halt(); // Stop current movement so the warlock is not just running straight into the next object
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Warlock.rootTime,
                            new Action<float>(p =>
                            {
                                // Delayed action
                                if (p == 1f) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                            })));
                    }

                    Warlock.curseVictim = null;
                    Warlock.curseVictimTarget = null;
                    warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
                    Warlock.warlock.killTimer = warlockCurseButton.Timer = warlockCurseButton.MaxTimer;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo,
                        SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.WarlockTarget);
                    writer.Write(byte.MaxValue); // This will set it to null!
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            },
            () =>
            {
                return Warlock.warlock != null && Warlock.warlock == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (Warlock.curseVictim != null)
                    showTargetNameOnButton(Warlock.currentTarget, warlockCurseButton, getString("CurseKillText"));
                else
                    showTargetNameOnButton(Warlock.currentTarget, warlockCurseButton, getString("CurseText"));
                return ((Warlock.curseVictim == null && Warlock.currentTarget != null) ||
                        (Warlock.curseVictim != null && Warlock.curseVictimTarget != null)) &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {
                warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
                warlockCurseButton.Sprite = Warlock.getCurseButtonSprite();
                Warlock.curseVictim = null;
                Warlock.curseVictimTarget = null;
            },
            Warlock.getCurseButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F
        );

        // Security Guard button
        securityGuardButton = new CustomButton(
            () =>
            {
                if (SecurityGuard.ventTarget != null)
                {
                    // Seal vent
                    var writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.SealVent);
                    writer.WritePacked(SecurityGuard.ventTarget.Id);
                    writer.EndMessage();
                    RPCProcedure.sealVent(SecurityGuard.ventTarget.Id);
                    SecurityGuard.ventTarget = null;
                }
                else if (!isMira() && !isFungle() && !SubmergedCompatibility.IsSubmerged)
                {
                    // Place camera if there's no vent and it's not MiraHQ or Submerged
                    var pos = CachedPlayer.LocalPlayer.transform.position;
                    var buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    var writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId,
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
                return SecurityGuard.securityGuard != null &&
                       SecurityGuard.securityGuard == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead && SecurityGuard.remainingScrews >=
                       Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice);
            },
            () =>
            {
                securityGuardButton.actionButton.graphic.sprite =
                    SecurityGuard.ventTarget == null && !isMira() && !isFungle() &&
                    !SubmergedCompatibility.IsSubmerged
                        ? SecurityGuard.getPlaceCameraButtonSprite()
                        : SecurityGuard.getCloseVentButtonSprite();
                if (securityGuardButtonScrewsText != null)
                    securityGuardButtonScrewsText.text = $"{SecurityGuard.remainingScrews}/{SecurityGuard.totalScrews}";

                if (SecurityGuard.ventTarget != null)
                    return SecurityGuard.remainingScrews >= SecurityGuard.ventPrice &&
                           CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                return !isMira() && !isFungle() && !SubmergedCompatibility.IsSubmerged &&
                       SecurityGuard.remainingScrews >= SecurityGuard.camPrice &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { securityGuardButton.Timer = securityGuardButton.MaxTimer; },
            SecurityGuard.getPlaceCameraButtonSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F
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
                if (!isMira())
                {
                    if (SecurityGuard.minigame == null)
                    {
                        var mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                        var e = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("Surv_Panel") || x.name.Contains("Cam") ||
                            x.name.Contains("BinocularsSecurityConsole"));
                        if (isSkeld() || mapId == 3)
                            e = Object.FindObjectsOfType<SystemConsole>()
                                .FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
                        else if (isAirship())
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

                if (SecurityGuard.cantMove) CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 
            },
            () =>
            {
                return SecurityGuard.securityGuard != null &&
                       SecurityGuard.securityGuard == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead && SecurityGuard.remainingScrews <
                       Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice)
                       && !SubmergedCompatibility.IsSubmerged;
            },
            () =>
            {
                if (securityGuardChargesText != null)
                    securityGuardChargesText.text = $"{SecurityGuard.charges} / {SecurityGuard.maxCharges}";
                securityGuardCamButton.actionButton.graphic.sprite =
                    isMira() ? SecurityGuard.getLogSprite() : SecurityGuard.getCamSprite();
                securityGuardCamButton.actionButton.OverrideText(isMira() ?
                    getString("hackerDoorLogText") : getString("CamButtonText"));
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && SecurityGuard.charges > 0;
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
            KeyCode.Q,
            true,
            0f,
            () =>
            {
                securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                if (Minigame.Instance) SecurityGuard.minigame.ForceClose();
                CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
            },
            false,
            isMira() ? getString("hackerDoorLogText") : getString("CamButtonText")
        );

        // Security Guard cam button charges
        securityGuardChargesText = Object.Instantiate(securityGuardCamButton.actionButton.cooldownTimerText,
            securityGuardCamButton.actionButton.cooldownTimerText.transform.parent);
        securityGuardChargesText.text = "";
        securityGuardChargesText.enableWordWrapping = false;
        securityGuardChargesText.transform.localScale = Vector3.one * 0.5f;
        securityGuardChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        // Arsonist button
        arsonistButton = new CustomButton(
            () =>
            {
                var dousedEveryoneAlive = Arsonist.dousedEveryoneAlive();
                if (dousedEveryoneAlive)
                {
                    var winWriter = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ArsonistWin, SendOption.Reliable);
                    AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                    RPCProcedure.arsonistWin();
                    arsonistButton.HasEffect = false;
                }
                else if (Arsonist.currentTarget != null)
                {
                    if (checkAndDoVetKill(Arsonist.currentTarget)) return;
                    checkWatchFlash(Arsonist.currentTarget);
                    Arsonist.douseTarget = Arsonist.currentTarget;
                    arsonistButton.HasEffect = true;
                    SoundEffectsManager.play("arsonistDouse");
                }
            },
            () =>
            {
                return Arsonist.arsonist != null && Arsonist.arsonist == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                var dousedEveryoneAlive = Arsonist.dousedEveryoneAlive();
                if (!dousedEveryoneAlive)
                    showTargetNameOnButton(Arsonist.currentTarget, arsonistButton, getString("DouseText"));
                if (dousedEveryoneAlive) arsonistButton.actionButton.graphic.sprite = Arsonist.getIgniteSprite();

                if (arsonistButton.isEffectActive && Arsonist.douseTarget != Arsonist.currentTarget)
                {
                    Arsonist.douseTarget = null;
                    arsonistButton.Timer = 0f;
                    arsonistButton.isEffectActive = false;
                }

                return CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                       (dousedEveryoneAlive || Arsonist.currentTarget != null);
            },
            () =>
            {
                arsonistButton.Timer = arsonistButton.MaxTimer;
                arsonistButton.isEffectActive = false;
                Arsonist.douseTarget = null;
            },
            Arsonist.getDouseSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            true,
            Arsonist.duration,
            () =>
            {
                if (Arsonist.douseTarget != null) Arsonist.dousedPlayers.Add(Arsonist.douseTarget);

                arsonistButton.Timer = Arsonist.dousedEveryoneAlive() ? 0 : arsonistButton.MaxTimer;

                foreach (var p in Arsonist.dousedPlayers)
                    if (MapOption.playerIcons.ContainsKey(p.PlayerId))
                        MapOption.playerIcons[p.PlayerId].setSemiTransparent(false);

                // Ghost Info
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.ArsonistDouse);
                writer.Write(Arsonist.douseTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                Arsonist.douseTarget = null;
            }
        );

        // Vulture Eat
        vultureEatButton = new CustomButton(
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(),
                             CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            var truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
                            var truePosition2 = component.TruePosition;
                            if (Vector2.Distance(truePosition2, truePosition) <=
                                CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance &&
                                CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                                !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                    Constants.ShipAndObjectsMask, false))
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                var writer = AmongUsClient.Instance.StartRpcImmediately(
                                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CleanBody,
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
                return Vulture.vulture != null && Vulture.vulture == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                return __instance.ReportButton.graphic.color == Palette.EnabledColor &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { vultureEatButton.Timer = vultureEatButton.MaxTimer; },
            Vulture.getButtonSprite(),
            ButtonPositions.lowerRowCenter,
            __instance,
            KeyCode.F,
            buttonText: getString("VultureText")
        );

        amnisiacRememberButton = new CustomButton(
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(),
                             CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            var truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
                            var truePosition2 = component.TruePosition;
                            if (Vector2.Distance(truePosition2, truePosition) <=
                                CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance &&
                                CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                                !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                    Constants.ShipAndObjectsMask, false))
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                var writer = AmongUsClient.Instance.StartRpcImmediately(
                                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.AmnisiacTakeRole,
                                    SendOption.Reliable);
                                writer.Write(playerInfo.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                RPCProcedure.amnisiacTakeRole(playerInfo.PlayerId);
                                break;
                            }
                        }
                    }
            },
            () =>
            {
                return Amnisiac.amnisiac != null && Amnisiac.amnisiac == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                return __instance.ReportButton.graphic.color == Palette.EnabledColor &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { amnisiacRememberButton.Timer = 0f; },
            Amnisiac.getButtonSprite(),
            ButtonPositions.lowerRowRight, //brb
            __instance,
            KeyCode.F,
            buttonText: getString("RememberText")
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
                return Medium.medium != null && Medium.medium == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (mediumButton.isEffectActive && Medium.target != Medium.soulTarget)
                {
                    Medium.soulTarget = null;
                    mediumButton.Timer = 0f;
                    mediumButton.isEffectActive = false;
                }

                return Medium.target != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {
                mediumButton.Timer = mediumButton.MaxTimer;
                mediumButton.isEffectActive = false;
                Medium.soulTarget = null;
            },
            Medium.getQuestionSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            true,
            Medium.duration,
            () =>
            {
                mediumButton.Timer = mediumButton.MaxTimer;
                if (Medium.target == null || Medium.target.player == null) return;
                var msg = Medium.getInfo(Medium.target.player, Medium.target.killerIfExisting);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer.PlayerControl, msg);

                // Ghost Info
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                writer.Write(Medium.target.player.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.MediumInfo);
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
                            CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition());
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
            buttonText: getString("MediumText")
        );

        // Pursuer button
        pursuerButton = new CustomButton(
            () =>
            {
                if (Pursuer.target != null)
                {
                    if (checkAndDoVetKill(Pursuer.target)) return;
                    checkWatchFlash(Pursuer.target);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetBlanked, SendOption.Reliable);
                    writer.Write(Pursuer.target.PlayerId);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setBlanked(Pursuer.target.PlayerId, byte.MaxValue);

                    Pursuer.target = null;

                    Pursuer.blanks++;
                    pursuerButton.Timer = pursuerButton.MaxTimer;
                    SoundEffectsManager.play("pursuerBlank");
                }
            },
            () =>
            {
                return Pursuer.pursuer != null && Pursuer.pursuer == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead && Pursuer.blanks < Pursuer.blanksNumber;
            },
            () =>
            {
                showTargetNameOnButton(Pursuer.target, pursuerButton, getString("PursuerText"));
                if (pursuerButtonBlanksText != null)
                    pursuerButtonBlanksText.text = $"{Pursuer.blanksNumber - Pursuer.blanks}";

                return Pursuer.blanksNumber > Pursuer.blanks && CachedPlayer.LocalPlayer.PlayerControl.CanMove &&
                       Pursuer.target != null;
            },
            () => { pursuerButton.Timer = pursuerButton.MaxTimer; },
            Pursuer.buttonSprite,
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F
        );

        // Pursuer button blanks left
        pursuerButtonBlanksText = Object.Instantiate(pursuerButton.actionButton.cooldownTimerText,
            pursuerButton.actionButton.cooldownTimerText.transform.parent);
        pursuerButtonBlanksText.text = "";
        pursuerButtonBlanksText.enableWordWrapping = false;
        pursuerButtonBlanksText.transform.localScale = Vector3.one * 0.5f;
        pursuerButtonBlanksText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);


        // Witch Spell button
        witchSpellButton = new CustomButton(
            () =>
            {
                if (Witch.currentTarget != null)
                {
                    if (checkAndDoVetKill(Witch.currentTarget)) return;
                    checkWatchFlash(Witch.currentTarget);
                    Witch.spellCastingTarget = Witch.currentTarget;
                    SoundEffectsManager.play("witchSpell");
                }
            },
            () =>
            {
                return Witch.witch != null && Witch.witch == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Witch.currentTarget, witchSpellButton, "");
                if (witchSpellButton.isEffectActive && Witch.spellCastingTarget != Witch.currentTarget)
                {
                    Witch.spellCastingTarget = null;
                    witchSpellButton.Timer = 0f;
                    witchSpellButton.isEffectActive = false;
                }

                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Witch.currentTarget != null;
            },
            () =>
            {
                showTargetNameOnButton(null, arsonistButton, getString("WitchText"));
                witchSpellButton.Timer = witchSpellButton.MaxTimer;
                witchSpellButton.isEffectActive = false;
                Witch.spellCastingTarget = null;
            },
            Witch.getButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            true,
            Witch.spellCastingDuration,
            () =>
            {
                if (Witch.spellCastingTarget == null) return;
                var attempt = checkMuderAttempt(Witch.witch, Witch.spellCastingTarget);
                if (attempt == MurderAttemptResult.PerformKill)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetFutureSpelled,
                        SendOption.Reliable);
                    writer.Write(Witch.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setFutureSpelled(Witch.currentTarget.PlayerId);
                }

                if (attempt == MurderAttemptResult.BlankKill || attempt == MurderAttemptResult.PerformKill)
                {
                    Witch.currentCooldownAddition += Witch.cooldownAddition;
                    witchSpellButton.MaxTimer = Witch.cooldown + Witch.currentCooldownAddition;
                    PlayerControlFixedUpdatePatch.miniCooldownUpdate(); // Modifies the MaxTimer if the witch is the mini
                    witchSpellButton.Timer = witchSpellButton.MaxTimer;
                    if (Witch.triggerBothCooldowns)
                    {
                        var multiplier = Mini.mini != null && CachedPlayer.LocalPlayer.PlayerControl == Mini.mini
                            ? Mini.isGrownUp() ? 0.66f : 2f
                            : 1f;
                        Witch.witch.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown *
                                                multiplier;
                    }
                }
                else
                {
                    witchSpellButton.Timer = 0f;
                }

                Witch.spellCastingTarget = null;
            }
        );

        // Jumper Jump
        jumperButton = new CustomButton(
            () =>
            {
                if (Jumper.jumpLocation == Vector3.zero)
                {
                    //set location
                    Jumper.jumpLocation = PlayerControl.LocalPlayer.transform.localPosition;
                    jumperButton.Sprite = Jumper.jumpButtonSprite;
                    Jumper.Charges = Jumper.ChargesOnPlace;
                    jumperButton.buttonText = "jumperJumpText".Translate();
                }
                else if (Jumper.Charges >= 1f)
                {
                    //teleport to location if you have one
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetPosition, SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(Jumper.jumpLocation.x);
                    writer.Write(Jumper.jumpLocation.y);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Jumper.jumpLocation);


                    Jumper.Charges -= 1f;
                }

                if (Jumper.Charges > 0) jumperButton.Timer = jumperButton.MaxTimer;
            },
            () =>
            {
                return Jumper.jumper != null && Jumper.jumper == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                //if (jumperChargesText != null) jumperChargesText.text = $"{Jumper.jumperCharges}";
                Jumper.usedPlace = true;
                return (Jumper.jumpLocation == Vector3.zero || Jumper.Charges >= 1f) &&
                       PlayerControl.LocalPlayer.CanMove;

            },
            () =>
            {
                if (Jumper.resetPlaceAfterMeeting)
                {
                    Jumper.resetPlaces();
                    jumperButton.Sprite = Jumper.jumpMarkButtonSprite;
                    jumperButton.buttonText = "jumperMarkText".Translate();
                }
                if (Jumper.jumpLocation == Vector3.zero) jumperButton.buttonText = "jumperJumpText".Translate();
                Jumper.Charges += Jumper.ChargesGainOnMeeting;
                if (Jumper.Charges > Jumper.MaxCharges) Jumper.Charges = Jumper.MaxCharges;

                if (Jumper.Charges > 0) jumperButton.Timer = jumperButton.MaxTimer;
            },
            Jumper.jumpMarkButtonSprite,
            ButtonPositions.lowerRowRight, //brb
            __instance,
            KeyCode.F,
            buttonText: "jumperMarkText".Translate()
        );

        // Escapist Escape
        escapistButton = new CustomButton(
            () =>
            {
                if (Escapist.escapeLocation == Vector3.zero)
                {
                    //set location
                    Escapist.escapeLocation = PlayerControl.LocalPlayer.transform.localPosition;
                    escapistButton.Sprite = Escapist.escapeMarkButtonSprite;
                    Escapist.Charges = Escapist.ChargesOnPlace;
                    escapistButton.buttonText = "jumperJumpText".Translate();
                }
                else if (Escapist.Charges >= 1f)
                {
                    //teleport to location if you have one
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetPositionESC, SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(Escapist.escapeLocation.x);
                    writer.Write(Escapist.escapeLocation.y);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Escapist.escapeLocation);


                    Escapist.Charges -= 1f;
                }

                if (Escapist.Charges > 0) escapistButton.Timer = escapistButton.MaxTimer;
            },
            () =>
            {
                return Escapist.escapist != null && Escapist.escapist == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                //if (jumperChargesText != null) jumperChargesText.text = $"{Jumper.jumperCharges}";
                Escapist.usedPlace = true;
                return (Escapist.escapeLocation == Vector3.zero || Escapist.Charges >= 1f) &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                if (Escapist.resetPlaceAfterMeeting)
                {
                    Escapist.resetPlaces();
                    escapistButton.Sprite = Escapist.escapeButtonSprite;
                    escapistButton.buttonText = "jumperMarkText".Translate();
                }
                if (Escapist.escapeLocation == Vector3.zero) escapistButton.buttonText = "jumperJumpText".Translate();
                Escapist.Charges += Escapist.ChargesGainOnMeeting;
                if (Escapist.Charges > Escapist.MaxCharges) Escapist.Charges = Escapist.MaxCharges;

                if (Escapist.Charges > 0) escapistButton.Timer = escapistButton.MaxTimer;
            },
            Escapist.escapeButtonSprite,
            ButtonPositions.upperRowLeft, //brb
            __instance,
            KeyCode.F,
            buttonText: "jumperMarkText".Translate()
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
                        var pos = CachedPlayer.LocalPlayer.transform.position;
                        var buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                        writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                            (byte)CustomRPC.PlaceNinjaTrace);
                        writer.WriteBytesAndSize(buff);
                        writer.EndMessage();
                        RPCProcedure.placeNinjaTrace(buff);

                        var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(
                            CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetInvisible,
                            SendOption.Reliable);
                        invisibleWriter.Write(Ninja.ninja.PlayerId);
                        invisibleWriter.Write(byte.MinValue);
                        AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                        RPCProcedure.setInvisible(Ninja.ninja.PlayerId, byte.MinValue);
                        if (!checkAndDoVetKill(Ninja.ninjaMarked))
                        {
                            // Perform Kill
                            var writer2 = AmongUsClient.Instance.StartRpcImmediately(
                                CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer,
                                SendOption.Reliable);
                            writer2.Write(CachedPlayer.LocalPlayer.PlayerId);
                            writer2.Write(Ninja.ninjaMarked.PlayerId);
                            writer2.Write(byte.MaxValue);
                            AmongUsClient.Instance.FinishRpcImmediately(writer2);
                            if (SubmergedCompatibility.IsSubmerged)
                                SubmergedCompatibility.ChangeFloor(Ninja.ninjaMarked.transform.localPosition.y > -7);
                            RPCProcedure.uncheckedMurderPlayer(CachedPlayer.LocalPlayer.PlayerId,
                                Ninja.ninjaMarked.PlayerId, byte.MaxValue);
                        }

                        // Create Second trace after killing
                        pos = Ninja.ninjaMarked.transform.position;
                        buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                        var writer3 = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                            (byte)CustomRPC.PlaceNinjaTrace);
                        writer3.WriteBytesAndSize(buff);
                        writer3.EndMessage();
                        RPCProcedure.placeNinjaTrace(buff);
                    }

                    if (attempt is MurderAttemptResult.BlankKill or MurderAttemptResult.PerformKill)
                    {
                        ninjaButton.Timer = ninjaButton.MaxTimer;
                        Ninja.ninja.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
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
                    checkWatchFlash(Witch.currentTarget);
                    Ninja.ninjaMarked = Ninja.currentTarget;
                    ninjaButton.Timer = 5f;
                    SoundEffectsManager.play("warlockCurse");

                    // Ghost Info
                    writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.NinjaMarked);
                    writer.Write(Ninja.ninjaMarked.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            },
            () =>
            {
                return Ninja.ninja != null && Ninja.ninja == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                // CouldUse
                showTargetNameOnButton(Ninja.currentTarget, ninjaButton, getString("NinjaText"));
                ninjaButton.Sprite = Ninja.ninjaMarked != null
                    ? Ninja.getKillButtonSprite()
                    : Ninja.getMarkButtonSprite();
                return (Ninja.currentTarget != null || (Ninja.ninjaMarked != null &&
                                                        !TransportationToolPatches.isUsingTransportation(
                                                            Ninja.ninjaMarked))) && CachedPlayer.LocalPlayer
                    .PlayerControl.CanMove;
            },
            () =>
            {
                // on meeting ends
                ninjaButton.Timer = ninjaButton.MaxTimer;
                Ninja.ninjaMarked = null;
            },
            Ninja.getMarkButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F
        );

        blackmailerButton = new CustomButton(
            () =>
            {
                // Action when Pressed
                if (Blackmailer.currentTarget != null)
                {
                    if (checkAndDoVetKill(Blackmailer.currentTarget)) return;
                    checkWatchFlash(Blackmailer.currentTarget);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.BlackmailPlayer, SendOption.Reliable);
                    writer.Write(Blackmailer.currentTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.blackmailPlayer(Blackmailer.currentTarget.PlayerId);
                    blackmailerButton.Timer = blackmailerButton.MaxTimer;
                }
            },
            () =>
            {
                return Blackmailer.blackmailer != null &&
                       Blackmailer.blackmailer == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                // Could Use
                var text = getString("BlackmailerText");
                if (Blackmailer.blackmailed != null) text = Blackmailer.blackmailed.Data.PlayerName;
                showTargetNameOnButtonExplicit(Blackmailer.currentTarget, blackmailerButton,
                    text); //Show target name under button if setting is true
                return Blackmailer.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { blackmailerButton.Timer = blackmailerButton.MaxTimer; },
            Blackmailer.getBlackmailButtonSprite(),
            ButtonPositions.upperRowLeft, //brb
            __instance,
            KeyCode.F,
            true,
            1f,
            () => { },
            false,
            "Blackmail"
        );

        // Jackal Sidekick Button
        cultistTurnButton = new CustomButton(
            () =>
            {
                if (checkAndDoVetKill(Cultist.currentTarget)) return;
                checkWatchFlash(Cultist.currentTarget);
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.CultistCreateImposter, SendOption.Reliable);
                writer.Write(Cultist.currentTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.cultistCreateImposter(Cultist.currentTarget.PlayerId);
                SoundEffectsManager.play("jackalSidekick");
            },
            () =>
            {
                return Cultist.needsFollower && Cultist.cultist != null &&
                       Cultist.cultist == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                showTargetNameOnButton(Cultist.currentTarget, cultistTurnButton,
                    getString("ConvertText")); // Show now text since the button already says sidekick
                return Cultist.needsFollower && Cultist.currentTarget != null &&
                       CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () => { jackalSidekickButton.Timer = jackalSidekickButton.MaxTimer; },
            Cultist.getSidekickButtonSprite(),
            ButtonPositions.upperRowLeft, //brb
            __instance,
            KeyCode.F
        );

        // Trapper button
        trapperButton = new CustomButton(
            () =>
            {
                var pos = CachedPlayer.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.SetTrap);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                RPCProcedure.setTrap(buff);

                SoundEffectsManager.play("trapperTrap");
                trapperButton.Timer = trapperButton.MaxTimer;
            },
            () =>
            {
                return Trapper.trapper != null && Trapper.trapper == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (trapperChargesText != null) trapperChargesText.text = $"{Trapper.charges} / {Trapper.maxCharges}";
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Trapper.charges > 0;
            },
            () => { trapperButton.Timer = trapperButton.MaxTimer; },
            Trapper.getButtonSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            buttonText: getString("trapperTrapText")
        );

        // Terrorist button
        terroristButton = new CustomButton(
            () =>
            {
                if (checkMuderAttempt(Terrorist.terrorist, Terrorist.terrorist) != MurderAttemptResult.BlankKill)
                {
                    var pos = CachedPlayer.LocalPlayer.transform.position;
                    var buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    var writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.PlaceBomb);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.placeBomb(buff);

                    SoundEffectsManager.play(Terrorist.selfExplosion ? "bombExplosion" : "trapperTrap");
                }

                terroristButton.Timer = terroristButton.MaxTimer;
                Terrorist.isPlanted = true;

                if (Terrorist.selfExplosion)
                {
                    var loacl = CachedPlayer.LocalPlayer.PlayerId;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer,
                        SendOption.Reliable);
                    writer.Write(Terrorist.terrorist.Data.PlayerId);
                    writer.Write(loacl);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.uncheckedMurderPlayer(Terrorist.terrorist.Data.PlayerId, loacl, byte.MaxValue);
                }
            },
            () =>
            {
                return Terrorist.terrorist != null && Terrorist.terrorist == CachedPlayer.LocalPlayer.PlayerControl &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && !Terrorist.isPlanted; },
            () =>
            {
                terroristButton.Timer = terroristButton.MaxTimer;
            },
            Terrorist.getButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.F,
            true,
            Terrorist.destructionTime,
            () =>
            {
                terroristButton.Timer = terroristButton.MaxTimer;
                terroristButton.isEffectActive = false;
                terroristButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            buttonText: Terrorist.bombText
        );

        defuseButton = new CustomButton(
            () => { defuseButton.HasEffect = true; },
            () =>
            {
                return Terrorist.bomb != null && Bomb.canDefuse && !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                if (defuseButton.isEffectActive && !Bomb.canDefuse)
                {
                    defuseButton.Timer = 0f;
                    defuseButton.isEffectActive = false;
                }

                return CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {
                defuseButton.Timer = 0f;
                defuseButton.isEffectActive = false;
            },
            Bomb.getDefuseSprite(),
            new Vector3(4f, 1f, 0),
            __instance,
            null,
            true,
            Terrorist.defuseDuration,
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.DefuseBomb, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.defuseBomb();

                defuseButton.Timer = 0f;
                Bomb.canDefuse = false;
            },
            true,
            buttonText: getString("defuseBombText")
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
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer,
                        SendOption.Reliable);
                    writer2.Write(thief.PlayerId);
                    writer2.Write(thief.PlayerId);
                    writer2.Write(0);
                    RPCProcedure.uncheckedMurderPlayer(thief.PlayerId, thief.PlayerId, 0);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    Thief.thief.clearAllTasks();
                }

                // Steal role if survived.
                if (!Thief.thief.Data.IsDead && result == MurderAttemptResult.PerformKill)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ThiefStealsRole,
                        SendOption.Reliable);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.thiefStealsRole(target.PlayerId);
                }

                // Kill the victim (after becoming their role - so that no win is triggered for other teams)
                if (result == MurderAttemptResult.PerformKill)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedMurderPlayer,
                        SendOption.Reliable);
                    writer.Write(thief.PlayerId);
                    writer.Write(target.PlayerId);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.uncheckedMurderPlayer(thief.PlayerId, target.PlayerId, byte.MaxValue);
                }
            },
            () =>
            {
                return Thief.thief != null && CachedPlayer.LocalPlayer.PlayerControl == Thief.thief &&
                       !CachedPlayer.LocalPlayer.Data.IsDead;
            },
            () => { return Thief.currentTarget != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () => { thiefKillButton.Timer = thiefKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPositions.upperRowRight,
            __instance,
            KeyCode.Q,
            buttonText: getString("killButtonText")
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
                var pos = CachedPlayer.LocalPlayer.transform.position;
                byte[] buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                if (Yoyo.markedLocation == null)
                {
                    Message($"marked location is null in button press");
                    MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.YoyoMarkLocation, SendOption.Reliable);
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
                    {
                        SubmergedCompatibility.ChangeFloor(exit.y > -7);
                    }
                    MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.YoyoBlink, SendOption.Reliable);
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
            () => { return Yoyo.yoyo != null && Yoyo.yoyo == CachedPlayer.LocalPlayer.PlayerControl && !CachedPlayer.LocalPlayer.Data.IsDead; },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
            () =>
            {
                if (Yoyo.markStaysOverMeeting)
                {
                    yoyoButton.Timer = 10f;
                }
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
            KeyCode.F,
            false,
            Yoyo.blinkDuration,
            () =>
            {
                if (TransportationToolPatches.isUsingTransportation(Yoyo.yoyo))
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
                var pos = CachedPlayer.LocalPlayer.transform.position;
                byte[] buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
                var exit = (Vector3)Yoyo.markedLocation;
                if (SubmergedCompatibility.IsSubmerged)
                {
                    SubmergedCompatibility.ChangeFloor(exit.y > -7);
                }
                MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.YoyoBlink, SendOption.Reliable);
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
                {
                    Minigame.Instance.Close();
                }
            },
            buttonText: "YoyoMarkText".Translate()
        );

        yoyoAdminTableButton = new CustomButton(
           () =>
           {
               if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
               {
                   HudManager __instance = FastDestroyableSingleton<HudManager>.Instance;
                   __instance.InitMap();
                   MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
               }
           },
           () => { return Yoyo.yoyo != null && Yoyo.yoyo == CachedPlayer.LocalPlayer.PlayerControl && Yoyo.hasAdminTable && !CachedPlayer.LocalPlayer.Data.IsDead; },
           () =>
           {
               return true;
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
           () =>
           {
               yoyoAdminTableButton.Timer = yoyoAdminTableButton.MaxTimer;
               if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
           },
           GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3,
           "AdminMapText".Translate()
       );



        zoomOutButton = new CustomButton(
            () => { toggleZoom(); },
            () =>
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == null || !CachedPlayer.LocalPlayer.Data.IsDead || (CachedPlayer.LocalPlayer.Data.Role.IsImpostor && !CustomOptionHolder.deadImpsBlockSabotage.getBool())) return false;
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(CachedPlayer.LocalPlayer.Data);
                var numberOfLeftTasks = playerTotal - playerCompleted;
                return numberOfLeftTasks <= 0 || !CustomOptionHolder.finishTasksBeforeHauntingOrZoomingOut.getBool();
            },
            () => { return true; },
            () => { },
            loadSpriteFromResources("TheOtherRoles.Resources.MinusButton.png", 150f), // Invisible button!
            new Vector3(0.4f, 2.8f, 0),
            __instance,
            KeyCode.KeypadPlus
        )
        {
            Timer = 0f
        };

        hunterLighterButton = new CustomButton(
            () =>
            {
                Hunter.lightActive.Add(CachedPlayer.LocalPlayer.PlayerId);
                SoundEffectsManager.play("lighterLight");

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareTimer, SendOption.Reliable);
                writer.Write(Hunter.lightPunish);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.shareTimer(Hunter.lightPunish);
            },
            () => { return HideNSeek.isHunter() && !CachedPlayer.LocalPlayer.Data.IsDead; },
            () => { return true; },
            () =>
            {
                hunterLighterButton.Timer = 30f;
                hunterLighterButton.isEffectActive = false;
                hunterLighterButton.actionButton.graphic.color = Palette.EnabledColor;
            },
            Hunter.getLightSprite(),
            ButtonPositions.upperRowFarLeft,
            __instance,
            KeyCode.F,
            true,
            Hunter.lightDuration,
            () =>
            {
                Hunter.lightActive.Remove(CachedPlayer.LocalPlayer.PlayerId);
                hunterLighterButton.Timer = hunterLighterButton.MaxTimer;
                SoundEffectsManager.play("lighterLight");
            },
            buttonText: "(F)" + getString("LighterText")
        );

        hunterAdminTableButton = new CustomButton(
            () =>
            {
                if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                {
                    var __instance = FastDestroyableSingleton<HudManager>.Instance;
                    __instance.InitMap();
                    MapBehaviour.Instance.ShowCountOverlay(true, true, false);
                }

                CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement 

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareTimer, SendOption.Reliable);
                writer.Write(Hunter.AdminPunish);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.shareTimer(Hunter.AdminPunish);
            },
            () => { return HideNSeek.isHunter() && !CachedPlayer.LocalPlayer.Data.IsDead; },
            () => { return true; },
            () =>
            {
                hunterAdminTableButton.Timer = hunterAdminTableButton.MaxTimer;
                hunterAdminTableButton.isEffectActive = false;
                hunterAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Hacker.getAdminSprite(),
            ButtonPositions.lowerRowCenter,
            __instance,
            KeyCode.G,
            true,
            Hunter.AdminDuration,
            () =>
            {
                hunterAdminTableButton.Timer = hunterAdminTableButton.MaxTimer;
                if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
            },
            false,
            "(G)" + getString("AdminMapText")
        );

        hunterArrowButton = new CustomButton(
            () =>
            {
                Hunter.arrowActive = true;
                SoundEffectsManager.play("trackerTrackPlayer");

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareTimer, SendOption.Reliable);
                writer.Write(Hunter.ArrowPunish);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.shareTimer(Hunter.ArrowPunish);
            },
            () => { return HideNSeek.isHunter() && !CachedPlayer.LocalPlayer.Data.IsDead; },
            () => { return true; },
            () =>
            {
                hunterArrowButton.Timer = 30f;
                hunterArrowButton.isEffectActive = false;
                hunterArrowButton.actionButton.graphic.color = Palette.EnabledColor;
            },
            Hunter.getArrowSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.R,
            true,
            Hunter.ArrowDuration,
            () =>
            {
                Hunter.arrowActive = false;
                hunterArrowButton.Timer = hunterArrowButton.MaxTimer;
                SoundEffectsManager.play("trackerTrackPlayer");
            }
        );

        huntedShieldButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.HuntedShield, SendOption.Reliable);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.huntedShield(CachedPlayer.LocalPlayer.PlayerId);
                SoundEffectsManager.play("timemasterShield");

                Hunted.shieldCount--;
            },
            () => { return HideNSeek.isHunted() && !CachedPlayer.LocalPlayer.Data.IsDead; },
            () =>
            {
                if (huntedShieldCountText != null) huntedShieldCountText.text = $"{Hunted.shieldCount}";
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Hunted.shieldCount > 0;
            },
            () =>
            {
                huntedShieldButton.Timer = huntedShieldButton.MaxTimer;
                huntedShieldButton.isEffectActive = false;
                huntedShieldButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            TimeMaster.getButtonSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            true,
            Hunted.shieldDuration,
            () =>
            {
                huntedShieldButton.Timer = huntedShieldButton.MaxTimer;
                SoundEffectsManager.stop("timemasterShield");
            }
        );

        huntedShieldCountText = Object.Instantiate(huntedShieldButton.actionButton.cooldownTimerText,
            huntedShieldButton.actionButton.cooldownTimerText.transform.parent);
        huntedShieldCountText.text = "";
        huntedShieldCountText.enableWordWrapping = false;
        huntedShieldCountText.transform.localScale = Vector3.one * 0.5f;
        huntedShieldCountText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);


        propDisguiseButton = new CustomButton(
            () =>
            {
                // Prop stuff
                var player = PlayerControl.LocalPlayer;
                var disguiseTarget = PropHunt.currentTarget;
                if (disguiseTarget != null)
                {
                    player.transform.localScale = disguiseTarget.transform.lossyScale;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetProp, SendOption.Reliable);
                    writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer.Write(disguiseTarget.gameObject.name);
                    writer.Write(disguiseTarget.gameObject.transform.position.x);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.propHuntSetProp(CachedPlayer.LocalPlayer.PlayerId, disguiseTarget.gameObject.name,
                        disguiseTarget.gameObject.transform.position.x);
                    SoundEffectsManager.play("morphlingMorph");
                    propDisguiseButton.Timer = 1f;
                }
            },
            () =>
            {
                return PropHunt.isPropHuntGM && !PlayerControl.LocalPlayer.Data.Role.IsImpostor &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                propSpriteRenderer.sprite = PropHunt.currentTarget?.GetComponent<SpriteRenderer>()?.sprite;
                if (propSpriteRenderer.sprite == null)
                    propSpriteRenderer.sprite = PropHunt.currentTarget?.transform
                        .GetComponentInChildren<SpriteRenderer>()?.sprite;
                if (propSpriteRenderer.sprite != null)
                    propSpriteHolder.transform.localScale *= 1 / propSpriteRenderer.bounds.size.magnitude;
                return PropHunt.currentTarget != null &&
                       PropHunt.currentTarget?.GetComponent<SpriteRenderer>()?.sprite != null;
            },
            () => { },
            null,
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.F,
            buttonText: "伪装"
        );
        propSpriteHolder = new GameObject("TORPropButtonPropSpritePreview");
        propSpriteRenderer = propSpriteHolder.AddComponent<SpriteRenderer>();
        propSpriteHolder.transform.SetParent(propDisguiseButton.actionButtonGameObject.transform, false);
        propSpriteHolder.transform.localPosition = new Vector3(0, 0, -2f);

        propHuntUnstuckButton = new CustomButton(
            () => { PlayerControl.LocalPlayer.Collider.enabled = false; },
            () =>
            {
                var IsDead = PropHunt.isPropHuntGM && !PlayerControl.LocalPlayer.Data.IsDead;
                return PropHunt.enableUnstuck switch
                {
                    0 => false,
                    1 => IsDead && PlayerControl.LocalPlayer.Data.Role.IsImpostor,
                    2 => IsDead && !PlayerControl.LocalPlayer.Data.Role.IsImpostor,
                    3 => IsDead,
                    _ => false,
                };
            },
            () => { return true; },
            () => { },
            PropHunt.getUnstuckButtonSprite(),
            ButtonPositions.upperRowLeft,
            __instance,
            KeyCode.LeftShift,
            true,
            1f,
            () =>
            {
                PlayerControl.LocalPlayer.Collider.enabled = true;
                propHuntUnstuckButton.Timer = propHuntUnstuckButton.MaxTimer;
            },
            buttonText: "穿墙"
        );

        propHuntRevealButton = new CustomButton(
            () =>
            {
                // select a random crewplayer to reveal.
                var candidates = PlayerControl.AllPlayerControls.ToArray().Where(x =>
                        !x.Data.Role.IsImpostor && !x.Data.IsDead &&
                        !PropHunt.isCurrentlyRevealed.ContainsKey(x.PlayerId))
                    .ToList();
                var rng = new Random();
                var selectedPlayer = candidates[rng.Next(candidates.Count)];
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.SetRevealed, SendOption.Reliable);
                writer.Write(selectedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.propHuntSetRevealed(selectedPlayer.PlayerId);
            },
            () =>
            {
                return PropHunt.isPropHuntGM && !PlayerControl.LocalPlayer.Data.IsDead &&
                       PlayerControl.LocalPlayer.Data.Role.IsImpostor;
            },
            () => { return PropHunt.timer - PropHunt.revealPunish > 0; },
            () => { },
            PropHunt.getRevealButtonSprite(),
            ButtonPositions.upperRowFarLeft,
            __instance,
            KeyCode.R,
            true,
            5f,
            () => { propHuntRevealButton.Timer = propHuntRevealButton.MaxTimer; },
            buttonText: getString("揭示")
        );

        propHuntInvisButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.PropHuntSetInvis, SendOption.Reliable);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.propHuntSetInvis(CachedPlayer.LocalPlayer.PlayerId);
                SoundEffectsManager.play("morphlingMorph");
            },
            () =>
            {
                return PropHunt.isPropHuntGM && !PlayerControl.LocalPlayer.Data.IsDead &&
                       !PlayerControl.LocalPlayer.Data.Role.IsImpostor && PropHunt.enableInvis;
            },
            () => { return PropHunt.currentObject.ContainsKey(PlayerControl.LocalPlayer.PlayerId); },
            () => { },
            PropHunt.getInvisButtonSprite(),
            ButtonPositions.upperRowFarLeft,
            __instance,
            KeyCode.I,
            true,
            5f,
            () =>
            {
                SoundEffectsManager.play("morphlingMorph");
                propHuntInvisButton.Timer = propHuntInvisButton.MaxTimer;
            },
            buttonText: getString("SwoopText")
        );

        propHuntSpeedboostButton = new CustomButton(
            () =>
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.PropHuntSetSpeedboost, SendOption.Reliable);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.propHuntSetSpeedboost(CachedPlayer.LocalPlayer.PlayerId);
                SoundEffectsManager.play("timemasterShield");
            },
            () =>
            {
                return PropHunt.isPropHuntGM && !PlayerControl.LocalPlayer.Data.IsDead &&
                       !PlayerControl.LocalPlayer.Data.Role.IsImpostor && PropHunt.enableSpeedboost;
            },
            () => { return true; },
            () => { },
            PropHunt.getSpeedboostButtonSprite(),
            ButtonPositions.lowerRowCenter,
            __instance,
            KeyCode.G,
            true,
            5f,
            () =>
            {
                SoundEffectsManager.stop("timemasterShield");
                propHuntSpeedboostButton.Timer = propHuntSpeedboostButton.MaxTimer;
            },
            buttonText: "疾跑"
        );

        propHuntAdminButton = new CustomButton(
            () =>
            {
                if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                {
                    var __instance = FastDestroyableSingleton<HudManager>.Instance;
                    __instance.InitMap();
                    MapBehaviour.Instance.ShowCountOverlay(true, true, false);
                }

                CachedPlayer.LocalPlayer.NetTransform.Halt(); // Stop current movement
            },
            () =>
            {
                return PropHunt.isPropHuntGM && !PlayerControl.LocalPlayer.Data.IsDead &&
                       PlayerControl.LocalPlayer.Data.Role.IsImpostor;
            },
            () => { return true; },
            () =>
            {
                propHuntAdminButton.Timer = hunterAdminTableButton.MaxTimer;
                propHuntAdminButton.isEffectActive = false;
                propHuntAdminButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Hacker.getAdminSprite(),
            ButtonPositions.lowerRowRight,
            __instance,
            KeyCode.G,
            true,
            PropHunt.adminDuration,
            () =>
            {
                propHuntAdminButton.Timer = propHuntAdminButton.MaxTimer;
                if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
            },
            false,
            getString("AdminMapText")
        );
        propHuntFindButton = new CustomButton(
            () => { SoundEffectsManager.play("timemasterShield"); },
            () =>
            {
                return PropHunt.isPropHuntGM && !PlayerControl.LocalPlayer.Data.IsDead &&
                       PlayerControl.LocalPlayer.Data.Role.IsImpostor;
            },
            () => { return true; },
            () => { },
            PropHunt.getFindButtonSprite(),
            ButtonPositions.lowerRowCenter,
            __instance,
            KeyCode.F,
            true,
            5f,
            () =>
            {
                SoundEffectsManager.stop("timemasterShield");
                propHuntFindButton.Timer = propHuntFindButton.MaxTimer;
                propHuntFindButton.isEffectActive = false;
            },
            buttonText: "探测"
        );

        // Set the default (or settings from the previous game) timers / durations when spawning the buttons
        initialized = true;
        setCustomButtonCooldowns();
        deputyHandcuffedButtons = new Dictionary<byte, List<CustomButton>>();
    }
}