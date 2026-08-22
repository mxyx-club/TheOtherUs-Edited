using Reactor.Networking;
using TheOtherRoles.Attributes;
using TheOtherRoles.Objects;
using static TheOtherRoles.Modules.ModInputManager;

namespace TheOtherRoles.Buttons;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
internal static class HudManagerStartPatch
{
    private static bool initialized;

    public static CustomButton zoomOutButton;
    public static CustomButton roleSummaryButton;
    public static CustomButton gameModeButton;
    public static CustomButton ghostEngineerButton;
    public static CustomButton engineerRepairButton;
    public static CustomButton sheriffKillButton;
    public static CustomButton deputyHandcuffButton;
    public static CustomButton amnisiacRememberButton;
    public static CustomButton specterRememberButton;
    public static CustomButton veteranAlertButton;
    public static CustomButton medicShieldButton;
    public static CustomButton alchemystKillButton;
    public static CustomButton shifterShiftButton;
    public static CustomButton bomberBombButton;
    public static CustomButton bomberGiveButton;
    public static CustomButton bountyHunterChangeTarget;
    public static CustomButton disperserDisperseButton;
    public static CustomButton buttonBarryButton;
    public static CustomButton glitchMimicButton;
    public static CustomButton butcherDissectionButton;
    public static CustomButton camouflagerButton;
    public static CustomButton portalmakerPlacePortalButton;
    public static CustomButton usePortalButton;
    public static CustomButton portalmakerMoveToPortalButton;
    public static CustomButton hackerButton;
    public static CustomButton hackerVitalsButton;
    public static CustomButton hackerAdminTableButton;
    public static CustomButton trackerTrackPlayerButton;
    public static CustomButton bodyGuardGuardButton;
    public static CustomButton trackerTrackCorpsesButton;
    public static CustomButton vampireKillButton;
    public static CustomButton vampireRecruitButton;
    public static CustomButton garlicButton;
    public static CustomButton jackalKillButton;
    public static CustomButton jackalSwoopButton;
    public static CustomButton phantomSwoopButton;
    public static CustomButton phantomKillButton;
    public static CustomButton jackalCreateSidekickButton;
    public static CustomButton eraserButton;
    public static CustomButton pavlovsdogsRingButton;
    public static CustomButton pavlovsdogsKillButton;
    public static CustomButton pavlovsownerCreateDogButton;
    public static CustomButton partTimerButton;
    public static CustomButton placeJackInTheBoxButton;
    public static CustomButton lightsOutButton;
    public static CustomButton cleanerCleanButton;
    public static CustomButton undertakerDragButton;
    public static CustomButton JesterDragButton;
    public static CustomButton warlockCurseButton;
    public static CustomButton securityGuardButton;
    public static CustomButton securityGuardCamButton;
    public static CustomButton arsonistButton;
    public static CustomButton arsonistKillButton;
    public static CustomButton vultureEatButton;
    public static CustomButton alchemystButton;
    public static CustomButton pursuerButton;
    public static CustomButton witchSpellButton;
    public static CustomButton jumperMarkButton;
    public static CustomButton jumperJumpButton;
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
    public static CustomButton InfectedKillButton;
    public static CustomButton jailorButton;
    public static CustomButton gaolerAdminTableButton;

    public static CustomButton marionetteButton;
    public static CustomButton marionettePlaceButton;
    public static CustomButton marionetteCameraButton;
    public static CustomButton avengerKillButton;
    public static CustomButton clogPlaceGhost;
    public static CustomButton oracleButton;
    public static CustomButton soulSightButton;
    public static CustomButton dreamcatcherButton;


    public static Dictionary<byte, List<CustomButton>> deputyHandcuffedButtons;
    public static PoolablePlayer targetDisplay;

    [OnGameStart]
    public static void setCustomButtonCooldowns()
    {
        if (!initialized)
        {
            try
            {
                createButtonsPostfix(HudManager.Instance);
            }
            catch
            {
                Warn("Button cooldowns not set, either the gamemode does not require them or there's something wrong.");
                return;
            }
        }

        yoyoButton.MaxTimer = Yoyo.markCooldown;
        yoyoAdminTableButton.MaxTimer = Yoyo.adminCooldown;
        yoyoAdminTableButton.EffectDuration = 10f;
        engineerRepairButton.MaxTimer = 0f;
        ghostEngineerButton.Timer = ghostEngineerButton.MaxTimer = 0f;
        specterRememberButton.MaxTimer = 15f;
        sheriffKillButton.MaxTimer = Sheriff.cooldown;
        deputyHandcuffButton.MaxTimer = Sheriff.handcuffCooldown;
        veteranAlertButton.MaxTimer = Veteran.cooldown;
        survivorVestButton.MaxTimer = Survivor.vestCooldown;
        survivorBlanksButton.MaxTimer = Survivor.blanksCooldown;
        medicShieldButton.MaxTimer = 0f;
        shifterShiftButton.MaxTimer = 0f;
        disperserDisperseButton.MaxTimer = 0f;
        buttonBarryButton.MaxTimer = 0f;
        glitchMimicButton.MaxTimer = Glitch.cooldown;
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
        bountyHunterChangeTarget.MaxTimer = BountyHunter.changeTargetCooldown;
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
        warlockCurseButton.MaxTimer = Warlock.Cooldown;
        securityGuardButton.MaxTimer = SecurityGuard.cooldown;
        securityGuardCamButton.MaxTimer = SecurityGuard.cooldown;
        arsonistButton.MaxTimer = arsonistKillButton.MaxTimer = Arsonist.cooldown;
        vultureEatButton.MaxTimer = Vulture.cooldown;
        amnisiacRememberButton.MaxTimer = 0f;
        grenadierFlashButton.MaxTimer = Grenadier.cooldown;
        bomberGiveButton.MaxTimer = bomberGiveButton.Timer = 0f;
        partTimerButton.MaxTimer = PartTimer.cooldown;
        alchemystButton.MaxTimer = Alchemyst.cooldown;
        alchemystKillButton.MaxTimer = Alchemyst.KillCooldown;
        pursuerButton.MaxTimer = Pursuer.cooldown;
        trackerTrackCorpsesButton.MaxTimer = Tracker.corpsesTrackingCooldown;
        prophetButton.MaxTimer = Prophet.cooldown;
        witchSpellButton.MaxTimer = Witch.cooldown;
        ninjaButton.MaxTimer = Ninja.cooldown;
        phantomSwoopButton.MaxTimer = Phantom.swoopCooldown;
        phantomSwoopButton.MaxTimer = Phantom.swoopCooldown;
        phantomSwoopButton.EffectDuration = Phantom.duration;
        jackalSwoopButton.MaxTimer = Jackal.swoopCooldown;
        jackalSwoopButton.MaxTimer = Jackal.swoopCooldown;
        jackalSwoopButton.EffectDuration = Jackal.duration;
        minerMineButton.MaxTimer = Miner.cooldown;
        blackmailerButton.MaxTimer = Blackmailer.cooldown;
        pelicanKillButton.MaxTimer = Pelican.cooldown;
        thiefKillButton.MaxTimer = Thief.cooldown;
        juggernautKillButton.MaxTimer = Juggernaut.cooldown;
        phantomKillButton.MaxTimer = Phantom.cooldown;
        evilTrapperSetTrapButton.MaxTimer = EvilTrapper.cooldown;
        redemptorReviveButton.MaxTimer = 10f;
        redemptorRevelationButton.MaxTimer = Redemptor.revelationCooldown;
        redemptorPrayerButton.MaxTimer = Redemptor.prayerCooldown;
        doomsayerButton.MaxTimer = Doomsayer.cooldown;
        akujoHonmeiButton.MaxTimer = 0f;
        akujoBackupButton.MaxTimer = 0f;
        pavlovsdogsRingButton.MaxTimer = Pavlovsdogs.ringCooldown;
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
        InfectedKillButton.MaxTimer = Infected.cooldown;
        jailorButton.MaxTimer = Jailor.cooldown;
        marionettePlaceButton.MaxTimer = Marionette.PlaceCooldown;
        marionetteButton.MaxTimer = Marionette.SwapCooldown;
        marionetteCameraButton.MaxTimer = 0f;
        avengerKillButton.MaxTimer = Avenger.killCooldown;
        clogPlaceGhost.MaxTimer = Clog.GhostCooldown;
        oracleButton.MaxTimer = Oracle.ConfessCooldown;
        dreamcatcherButton.MaxTimer = Dreamcatcher.DreamCooldown;
        soulSightButton.MaxTimer = SoulSight.Cooldown;
        vampireRecruitButton.MaxTimer = Vampire.recruitCooldown;

        butcherDissectionButton.EffectDuration = Butcher.dissectionDuration;
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
        glitchMimicButton.EffectDuration = Glitch.duration;
        bomberBombButton.EffectDuration = Bomber.bombDelay + Bomber.bombTimer;
        lightsOutButton.EffectDuration = Trickster.lightsOutDuration;
        arsonistButton.EffectDuration = Arsonist.duration;
        alchemystButton.EffectDuration = Alchemyst.duration;
        trackerTrackCorpsesButton.EffectDuration = Tracker.corpsesTrackingDuration;
        witchSpellButton.EffectDuration = Witch.spellCastingDuration;
        securityGuardCamButton.EffectDuration = SecurityGuard.duration;
        defuseButton.EffectDuration = Terrorist.defuseDuration;
        terroristButton.EffectDuration = Terrorist.destructionTime + Terrorist.bombActiveAfter;
        redemptorRevelationButton.EffectDuration = Redemptor.revelationDuration;
        redemptorPrayerButton.EffectDuration = Redemptor.prayerDuration;
        clogPlaceGhost.EffectDuration = Clog.GhostDuration;
        berserkerKillButton.EffectDuration = 0.5f;
        soulSightButton.EffectDuration = SoulSight.RespawnTimer;
        pavlovsdogsRingButton.EffectDuration = Pavlovsdogs.ringDuration;

        zoomOutButton.MaxTimer = zoomOutButton.Timer = 0f;
    }

    public static void Postfix(HudManager __instance)
    {
        initialized = false;

        try
        {
            if (HudGrid.Instance == null)
            {
                HudGrid.Instance = HudManager.Instance.gameObject.AddComponent<HudGrid>();
            }
            createButtonsPostfix(__instance);
        }
        catch { }
    }

    public static void createRoleSummaryButton(HudManager __instance)
    {
        roleSummaryButton = new CustomButton(
            () =>
            {
                LobbyRoleInfo.RoleSummaryOnClick();
            },
            () => { return PlayerControl.LocalPlayer != null && LobbyBehaviour.Instance; },
            () =>
            {
                if (PlayerCustomizationMenu.Instance || GameSettingMenu.Instance)
                {
                    if (LobbyRoleInfo.RolesSummaryUI != null)
                        UObject.Destroy(LobbyRoleInfo.RolesSummaryUI);
                }
                return true;
            },
            () => { },
            new ResourceSprite("HelpButton.png", 85f),
            __instance,
            __instance.AbilityButton,
            null,
            PositionOffset: new Vector3(0.4f, 3f, 0),
            useGrid: false
        )
        {
            Timer = 0f,
            MaxTimer = 0f
        };

        gameModeButton = new CustomButton(
            () =>
            {
                SetNextGameMode();
            },
            () => { return PlayerControl.LocalPlayer && AmongUsClient.Instance?.AmHost == true && LobbyBehaviour.Instance; },
            () => { return true; },
            () => { },
            new ResourceSprite("Swap.png", 135f),
            __instance,
            __instance.AbilityButton,
            null,
            buttonText: GetString("gameModeButton")
        )
        { Timer = 0f };
    }

    public static void createButtonsPostfix(HudManager __instance)
    {
        // get map id, or raise error to wait...
        var mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;

        createRoleSummaryButton(__instance);

        zoomOutButton = new CustomButton(
            () => { toggleZoom(); },
            () =>
            {
                if (!CanSeeGhostInfo) return false;
                if (PlayerControl.LocalPlayer.IsAlive()) return false;
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data);
                var numberOfLeftTasks = playerTotal - playerCompleted;
                return numberOfLeftTasks <= 0 || !CustomOptionHolder.finishTasksBeforeHauntingOrZoomingOut.GetBool();
            },
            () => { return true; },
            () => { },
            UnityHelper.loadSpriteFromResources("TheOtherRoles.Resources.ZoomOut.png", 85f), // Invisible button!
            __instance,
            __instance.AbilityButton,
            KeyCode.KeypadPlus,
            PositionOffset: new Vector3(0.4f, 2.35f, 0f),
            useGrid: false
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
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.HeliSabotage, 0 | 16);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.HeliSabotage, 1 | 16);
                    }
                    else if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                    {
                        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.FixSubmergedOxygen);
                        writer.EndRPC();
                        RPCProcedure.FixSubmergedOxygen();
                    }
                SoundEffectsManager.play("engineerRepair");
                Engineer.remainingFixes--;
                Engineer.UsedFix = true;
                engineerRepairButton.Timer = 0f;
            },
            () =>
            {
                return Engineer.engineer.IsAlive() && Engineer.engineer == PlayerControl.LocalPlayer && Engineer.remoteFix && Engineer.remainingFixes > 0;
            },
            () =>
            {
                engineerRepairButton.UsesCount = Engineer.remainingFixes;
                return isSabotageActive() && Engineer.remainingFixes > 0 && (!Engineer.oneFixPerRound || !Engineer.UsedFix) && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                Engineer.UsedFix = false;
                if (Engineer.remainingFixes < Engineer.resetFixAfterMeeting) Engineer.remainingFixes = Engineer.resetFixAfterMeeting;
            },
            Engineer.buttonSprite,
            __instance,
            __instance.AbilityButton,
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
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.HeliSabotage, 0 | 16);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.HeliSabotage, 1 | 16);
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
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            buttonText: GetString("RepairText")
        );

        //Sheriff Kill
        sheriffKillButton = new CustomButton(
            () =>
            {
                var target = Sheriff.currentTarget;
                if (target != null)
                {
                    if (Sheriff.sheriffCanKill(target))
                    {
                        if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target, true, false, CustomDeathReason.SheriffKill)) return;

                        sheriffKillButton.Timer = sheriffKillButton.MaxTimer;
                        Sheriff.currentTarget = null;
                    }
                    else if (CheckMurderPlayer(PlayerControl.LocalPlayer, target))
                    {
                        switch (Sheriff.misfireKills)
                        {
                            case 0:
                                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, true, true, CustomDeathReason.SheriffSuicide);
                                break;
                            case 1:
                                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target, true, true, CustomDeathReason.SheriffMisfire);
                                break;
                            case 2:
                                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target, true, true, CustomDeathReason.SheriffMisfire);
                                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, true, true, CustomDeathReason.SheriffSuicide);
                                break;
                        }
                        sheriffKillButton.Timer = sheriffKillButton.MaxTimer;
                        Sheriff.currentTarget = null;
                    }
                }
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsAlive() && Sheriff.Player.Any(x => x == PlayerControl.LocalPlayer);
            },
            () =>
            {
                Sheriff.currentTarget = SetTarget(inVented: ModOption.CanKillInVent);
                SetPlayerOutline(Sheriff.currentTarget, Sheriff.color);

                sheriffKillButton.showTargetNameOnButton(Sheriff.currentTarget);
                return Sheriff.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { sheriffKillButton.Timer = sheriffKillButton.MaxTimer; },
            Sheriff.killButtonSprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        // Deputy Handcuff
        deputyHandcuffButton = new CustomButton(
            () =>
            {
                if (Sheriff.currentTarget == null) return;
                if (CheckUseAbility(PlayerControl.LocalPlayer, Sheriff.currentTarget)) return;

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
                return PlayerControl.LocalPlayer.IsAlive() && (PlayerControl.LocalPlayer == Sheriff.Deputy
                       || Sheriff.Player.Any(x => x == PlayerControl.LocalPlayer && x == Sheriff.formerDeputy));
            },
            () =>
            {
                Sheriff.currentTarget = SetTarget(distances: Sheriff.handcuffRangeExtension);
                SetPlayerOutline(Sheriff.currentTarget, Sheriff.color);

                deputyHandcuffButton.showTargetNameOnButton(Sheriff.currentTarget);
                if (deputyHandcuffButton.ButtonTitle != null) deputyHandcuffButton.ButtonTitle.text = $"{Sheriff.remainingHandcuffs}";
                return Sheriff.remainingHandcuffs > 0 && Sheriff.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer; },
            Sheriff.handcuffSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("HandcuffText")
        );

        // Veteran Alert
        veteranAlertButton = new CustomButton(
            () =>
            {
                var writer = StartRPC(CustomRPC.VeteranAlert);
                writer.EndRPC();
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
                veteranAlertButton.IsEffectActive = false;
                veteranAlertButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Veteran.buttonSprite,
            __instance,
            __instance.AbilityButton,
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
                if (CheckUseAbility(PlayerControl.LocalPlayer, Medic.currentTarget)) return;

                medicShieldButton.Timer = 0f;

                var writer = StartRPC(Medic.setShieldAfterMeeting ? CustomRPC.SetFutureShielded : CustomRPC.MedicSetShielded);
                writer.Write(Medic.currentTarget.PlayerId);
                writer.EndRPC();
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
                if (!Medic.usedShield)
                {
                    Medic.currentTarget = SetTarget();
                    SetPlayerOutline(Medic.currentTarget, Medic.shieldedColor);
                    medicShieldButton.showTargetNameOnButton(Medic.currentTarget);
                }
                return !Medic.usedShield && Medic.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                if (Medic.reset) Medic.resetShielded();
            },
            Medic.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("ShieldText")
        );

        // doomsayer Shield
        doomsayerButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Doomsayer.currentTarget)) return;

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
                doomsayerButton.showTargetNameOnButton(Doomsayer.currentTarget);
                return PlayerControl.LocalPlayer.CanMove && Doomsayer.currentTarget != null;
            },
            () => { doomsayerButton.Timer = doomsayerButton.MaxTimer; },
            Doomsayer.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            0f,
            () =>
            {
                doomsayerButton.Timer = doomsayerButton.MaxTimer;
                var msg = Doomsayer.GetInfo(Doomsayer.currentTarget);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");

                // Ghost Info
                var writer = StartRPC(CustomRPC.ShareGhostInfo);
                writer.Write(Doomsayer.doomsayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
                writer.Write(msg);
                writer.EndRPC();
            },
            buttonText: GetString("doomsayerText")
        );

        // Akujo Honmei
        akujoHonmeiButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Akujo.currentTarget)) return;

                var writer = StartRPC(CustomRPC.AkujoSetHonmei);
                writer.Write(Akujo.akujo.PlayerId);
                writer.Write(Akujo.currentTarget.PlayerId);
                writer.EndRPC();
                RPCProcedure.akujoSetHonmei(PlayerControl.LocalPlayer.PlayerId, Akujo.currentTarget.PlayerId);
            },
            () =>
            {
                return PlayerControl.LocalPlayer == Akujo.akujo
                       && PlayerControl.LocalPlayer.IsAlive()
                       && Akujo.honmei == null
                       && Akujo.timeLeft > 0;
            },
            () =>
            {
                return Akujo.currentTarget != null;
            },
            () => { akujoHonmeiButton.Timer = akujoHonmeiButton.MaxTimer; },
            Akujo.honmeiSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("AkujoHonmeiText")
        );

        // Akujo Keep
        akujoBackupButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Akujo.currentTarget)) return;

                var writer = StartRPC(CustomRPC.AkujoSetKeep);
                writer.Write(Akujo.akujo.PlayerId);
                writer.Write(Akujo.currentTarget.PlayerId);
                writer.EndRPC();
                RPCProcedure.akujoSetKeep(PlayerControl.LocalPlayer.PlayerId, Akujo.currentTarget.PlayerId);
            },
            () =>
            {
                return PlayerControl.LocalPlayer == Akujo.akujo && PlayerControl.LocalPlayer.IsAlive() && Akujo.keepsLeft > 0;
            },
            () =>
            {
                akujoBackupButton.UsesCount = Akujo.keepsLeft;
                return Akujo.currentTarget != null && Akujo.keepsLeft > 0 && Akujo.timeLeft > 0;
            },
            () => { akujoBackupButton.Timer = akujoBackupButton.MaxTimer; },
            Akujo.keepSprite,
            __instance,
            __instance.AbilityButton,
            KeyCode.C,
            buttonText: GetString("AkujoBackupText")
        );

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
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("PlaceTrapText")
        );

        // Shifter shift
        shifterShiftButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Shifter.currentTarget)) return;

                var writer = StartRPC(CustomRPC.SetFutureShifted);
                writer.Write(Shifter.currentTarget.PlayerId);
                writer.EndRPC();
                RPCProcedure.setFutureShifted(Shifter.currentTarget.PlayerId);
                SoundEffectsManager.play("shifterShift");
            },
            () =>
            {
                return Shifter.shifter.IsAlive() && Shifter.shifter == PlayerControl.LocalPlayer && Shifter.futureShift == null;
            },
            () =>
            {
                if (Shifter.futureShift == null)
                {
                    Shifter.currentTarget = SetTarget();
                    SetPlayerOutline(Shifter.currentTarget, Color.yellow);
                    shifterShiftButton.showTargetNameOnButton(Shifter.currentTarget);
                }
                return Shifter.currentTarget && Shifter.futureShift == null &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            Shifter.buttonSprite,
            __instance,
            __instance.AbilityButton,
            modifierAbilityInput.keyCode,
            true,
            buttonText: GetString("ShiftText")
        );

        // Disperser disperse
        disperserDisperseButton = new CustomButton(
            () =>
            {
                var writer = StartRPC(CustomRPC.Disperse);
                writer.EndRPC();
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
            __instance,
            __instance.AbilityButton,
            modifierAbilityInput.keyCode,
            true,
            buttonText: GetString("DisperseText")
        );

        mayorMeetingButton = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                Mayor.UsedMeetingButton = true;

                var writer = StartRPC(CustomRPC.NoCheckStartMeeting);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(byte.MaxValue);
                writer.Write(true);
                writer.EndRPC();
                PlayerControl.LocalPlayer.NoCheckStartMeeting(null, true);

                mayorMeetingButton.Timer = 1f;
            },
            () =>
            {
                return Mayor.mayor.IsAlive() && Mayor.mayor == PlayerControl.LocalPlayer && Mayor.meetingButton && !Mayor.UsedMeetingButton;
            },
            () =>
            {
                mayorMeetingButton.actionButton.OverrideText(GetString("MayorButtonText") + "(" + Mayor.UsedMeetingButton + ")");
                var sabotageActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                {
                    if ((task.TaskType is TaskTypes.FixLights or TaskTypes.RestoreOxy or TaskTypes.ResetReactor
                            or TaskTypes.ResetSeismic or TaskTypes.FixComms or TaskTypes.StopCharles)
                            || (CustomOptionHolder.TheFungleMushroomMixupOption.GetBool() &&
                                CustomOptionHolder.TheFungleMushroomMixupCantOpenMeeting.GetBool() &&
                                PlayerControl.LocalPlayer.IsMushroomMixupActive())
                            || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                        sabotageActive = true;
                }

                return !sabotageActive && PlayerControl.LocalPlayer.CanMove;
            },
            () => { mayorMeetingButton.Timer = mayorMeetingButton.MaxTimer; },
            Mayor.emergencySprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("MayorButtonText")
        );

        // ButtonBarry Meetings
        buttonBarryButton = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                ButtonBarry.remoteMeetingsLeft--;

                var writer = StartRPC(CustomRPC.NoCheckStartMeeting);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(byte.MaxValue);
                writer.Write(true);
                writer.EndRPC();
                PlayerControl.LocalPlayer.NoCheckStartMeeting(null, true);

                buttonBarryButton.Timer = 1f;

            },
            () =>
            {
                return ButtonBarry.buttonBarry.IsAlive() && ButtonBarry.buttonBarry == PlayerControl.LocalPlayer;
            },
            () =>
            {
                var sabotageActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                {
                    if (((task.TaskType is TaskTypes.FixLights or TaskTypes.RestoreOxy or TaskTypes.ResetReactor
                            or TaskTypes.ResetSeismic or TaskTypes.FixComms or TaskTypes.StopCharles)
                            || (CustomOptionHolder.TheFungleMushroomMixupOption.GetBool() &&
                                CustomOptionHolder.TheFungleMushroomMixupCantOpenMeeting.GetBool() &&
                                PlayerControl.LocalPlayer.IsMushroomMixupActive())
                            || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)) && !ButtonBarry.SabotageRemoteMeetings)
                        sabotageActive = true;
                }
                return !sabotageActive && PlayerControl.LocalPlayer.CanMove && ButtonBarry.remoteMeetingsLeft > 0;
            },
            () => { buttonBarryButton.Timer = buttonBarryButton.MaxTimer; },
            ButtonBarry.buttonSprite,
            __instance,
            __instance.AbilityButton,
            modifierAbilityInput.keyCode,
            true,
            buttonText: "buttonBarryText".Translate()
        );

        glitchMimicButton = new CustomButton(
            () =>
            {
                if (Glitch.sampledTarget != null)
                {
                    var writer = StartRPC(CustomRPC.GlitchMimic);
                    writer.Write(Glitch.sampledTarget.PlayerId);
                    writer.Write(true);
                    writer.EndRPC();
                    RPCProcedure.GlitchMimic(Glitch.sampledTarget.PlayerId, true);
                    Glitch.sampledTarget = null;
                    glitchMimicButton.EffectDuration = Glitch.duration;
                    SoundEffectsManager.play("morphlingSample");
                }
                else
                {
                    var mimicTargets = new List<byte>();
                    foreach (var target in GameDataManager.Instance.AllPlayerControl)
                    {
                        if (target != Glitch.Player && !target.Data.Disconnected)
                        {
                            if (!target.Data.IsDead)
                            {
                                mimicTargets.Add(target.PlayerId);
                            }
                            else
                            {
                                foreach (var body in UObject.FindObjectsOfType<DeadBody>())
                                {
                                    if (body.ParentId == target.PlayerId) mimicTargets.Add(target.PlayerId);
                                }
                            }
                        }
                    }

                    var pk = new PlayerMenu((x) =>
                    {
                        var writer = StartRPC(CustomRPC.GlitchMimic);
                        writer.Write(x.PlayerId);
                        writer.Write(false);
                        writer.EndRPC();
                        RPCProcedure.GlitchMimic(x.PlayerId, false);
                        glitchMimicButton.Sprite = Glitch.morphSprite;

                        CustomButton.setButtonTargetDisplay(Glitch.sampledTarget, glitchMimicButton);
                        SoundEffectsManager.play("morphlingMorph");
                    }, (y) =>
                    {
                        return mimicTargets.ToArray().Contains(y.PlayerId);
                    });
                    PlayerControl.LocalPlayer.NetTransform.Halt();
                    Coroutines.Start(pk.Open(0f, true));

                    glitchMimicButton.IsEffectActive = false;
                    glitchMimicButton.EffectDuration = 0f;
                    glitchMimicButton.Timer = 0f;
                    _ = new LateTask(() => { glitchMimicButton.Timer = 0f; }, 0.5f, "Glitch");
                }
            },
            () =>
            {
                return Glitch.Player.IsAlive() && Glitch.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && !isActiveCamoComms && !MushroomSabotageActive;
            },
            () =>
            {
                glitchMimicButton.Sprite = Glitch.sampleSprite;
                Glitch.sampledTarget = null;
                CustomButton.setButtonTargetDisplay(null);

                glitchMimicButton.Timer = glitchMimicButton.MaxTimer;
                glitchMimicButton.IsEffectActive = false;
                glitchMimicButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Glitch.sampleSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            Glitch.duration,
            () =>
            {
                if (Glitch.sampledTarget == null)
                {
                    glitchMimicButton.Timer = glitchMimicButton.MaxTimer;
                    glitchMimicButton.Sprite = Glitch.sampleSprite;
                    SoundEffectsManager.play("morphlingMorph");
                    CustomButton.setButtonTargetDisplay(null);
                }
            },
            buttonText: GetString("SampleText")
        );

        // Camouflager camouflage
        camouflagerButton = new CustomButton(
            () =>
            {
                var writer = StartRPC(CustomRPC.CamouflagerCamouflage);
                writer.Write(1);
                writer.EndRPC();
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
                camouflagerButton.IsEffectActive = false;
                camouflagerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Camouflager.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            Camouflager.duration,
            () =>
            {
                camouflagerButton.Timer = camouflagerButton.MaxTimer;
                SoundEffectsManager.play("morphlingMorph");
            },
            buttonText: GetString("CamoText")
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
                return Hacker.hacker.IsAlive() && Hacker.hacker == PlayerControl.LocalPlayer;
            },
            () => { return true; },
            () =>
            {
                hackerButton.Timer = hackerButton.MaxTimer;
                hackerButton.IsEffectActive = false;
                hackerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Hacker.buttonSprite,
            __instance,
            __instance.AbilityButton,
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
                if (hackerAdminTableButton.ButtonTitle != null) hackerAdminTableButton.ButtonTitle.text = $"{Hacker.chargesAdminTable} / {Hacker.toolsNumber}";
                return Hacker.chargesAdminTable > 0;
            },
            () =>
            {
                hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
                hackerAdminTableButton.IsEffectActive = false;
                hackerAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Hacker.getAdminSprite(),
            __instance,
            __instance.AbilityButton,
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
                if (!hackerVitalsButton.IsEffectActive) PlayerControl.LocalPlayer.moveable = true;
                if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
            },
            mapId == 3,
            GetString("AdminMapText")
        );

        hackerVitalsButton = new CustomButton(
            () =>
            {
                if (mapId != 1)
                {
                    if (Hacker.vitals == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("panel_vitals") || x.gameObject.name.Contains("Vitals"));
                        if (e == null || Camera.main == null) return;
                        Hacker.vitals = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    Hacker.vitals.transform.SetParent(Camera.main.transform, false);
                    Hacker.vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Hacker.vitals.Begin(null);
                }
                else
                {
                    if (Hacker.doorLog == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>()
                            .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        Hacker.doorLog = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
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
                return Hacker.hacker.IsAlive() && Hacker.hacker == PlayerControl.LocalPlayer && mapId != 0 && mapId != 3;
            },
            () =>
            {
                if (hackerVitalsButton.ButtonTitle != null)
                    hackerVitalsButton.ButtonTitle.text = $"{Hacker.chargesVitals} / {Hacker.toolsNumber}";
                hackerVitalsButton.actionButton.graphic.sprite =
                    isMira ? Hacker.getLogSprite() : Hacker.getVitalsSprite();
                hackerVitalsButton.actionButton.OverrideText(isMira ? GetString("hackerDoorLogText") : GetString("hackerVitalText"));
                return Hacker.chargesVitals > 0;
            },
            () =>
            {
                hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                hackerVitalsButton.IsEffectActive = false;
                hackerVitalsButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Hacker.getVitalsSprite(),
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            0f,
            () => true,
            () =>
            {
                if (mapId != 1)
                {
                    if (Hacker.vitals == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("panel_vitals") || x.gameObject.name.Contains("Vitals"));
                        if (e == null || Camera.main == null) return;
                        Hacker.vitals = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    Hacker.vitals.transform.SetParent(Camera.main.transform, false);
                    Hacker.vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Hacker.vitals.Begin(null);
                }
                else
                {
                    if (Hacker.doorLog == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>()
                            .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        Hacker.doorLog = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    Hacker.doorLog.transform.SetParent(Camera.main.transform, false);
                    Hacker.doorLog.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Hacker.doorLog.Begin(null);
                }

            },
            () =>
            {
                hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
                if (!hackerAdminTableButton.IsEffectActive) PlayerControl.LocalPlayer.moveable = true;
                if (Minigame.Instance)
                {
                    if (isMira) Hacker.doorLog.ForceClose();
                    else Hacker.vitals.ForceClose();
                }
            },
            false,
            isMira ? GetString("hackerDoorLogText") : GetString("hackerVitalText")
        );

        // Tracker button
        trackerTrackPlayerButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Tracker.currentTarget)) return;

                var writer = StartRPC(CustomRPC.TrackerUsedTracker);
                writer.Write(Tracker.currentTarget.PlayerId);
                writer.EndRPC();
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
                if (!Tracker.usedTracker)
                {
                    Tracker.currentTarget = SetTarget();
                    SetPlayerOutline(Tracker.currentTarget, Tracker.color);
                    trackerTrackPlayerButton.showTargetNameOnButton(Tracker.currentTarget);
                }

                return PlayerControl.LocalPlayer.CanMove && Tracker.currentTarget != null && !Tracker.usedTracker;
            },
            () =>
            {
                if (Tracker.resetTargetAfterMeeting) Tracker.resetTracked();
            },
            Tracker.buttonSprite,
            __instance,
            __instance.AbilityButton,
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
                trackerTrackCorpsesButton.IsEffectActive = false;
                trackerTrackCorpsesButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Tracker.trackCorpsesButtonSprite,
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            true,
            Tracker.corpsesTrackingDuration,
            () => { trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer; },
            buttonText: GetString("TrackerCorpsesText")
        );

        bodyGuardGuardButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, BodyGuard.currentTarget)) return;
                var writer = StartRPC(CustomRPC.BodyGuardGuardPlayer);
                writer.Write(BodyGuard.currentTarget.PlayerId);
                writer.EndRPC();
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
                if (!BodyGuard.usedGuard)
                {
                    BodyGuard.currentTarget = SetTarget();
                    SetPlayerOutline(BodyGuard.currentTarget, BodyGuard.color);
                    bodyGuardGuardButton.showTargetNameOnButton(BodyGuard.currentTarget);
                }
                return PlayerControl.LocalPlayer.CanMove && BodyGuard.currentTarget != null &&
                       !BodyGuard.usedGuard;
            },
            () =>
            {
                if (BodyGuard.reset) BodyGuard.resetGuarded();
            },
            BodyGuard.guardButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("bodyGuardText")
        );

        vampireKillButton = new CustomButton(
            () =>
            {
                var target = Vampire.currentTarget;
                if (!CheckMurderPlayer(PlayerControl.LocalPlayer, target)) return;

                if (Vampire.targetNearGarlic)
                {
                    RpcCustomMurderPlayer(Vampire.vampire, target);
                    vampireKillButton.HasEffect = false; // Block effect on this click
                    vampireKillButton.Timer = vampireKillButton.MaxTimer;
                    return;
                }
                else
                {
                    Vampire.bitten = target;
                    // Notify players about bitten
                    var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
                    writer.Write(Vampire.bitten.PlayerId);
                    writer.EndRPC();
                    RPCProcedure.vampireSetBitten(Vampire.bitten.PlayerId);

                    _ = new LateTask(() =>
                    {
                        if (Vampire.vampire.IsAlive() && Vampire.bitten.IsAlive())
                        {
                            RpcCustomMurderPlayer(Vampire.vampire, Vampire.bitten, false);

                            var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten);
                            writer.Write(byte.MaxValue);
                            writer.EndRPC();
                            RPCProcedure.vampireSetBitten(byte.MaxValue);
                        }
                    }, Vampire.delay);

                    SoundEffectsManager.play("vampireBite");

                    vampireKillButton.EffectDuration = Vampire.delay;
                    vampireKillButton.HasEffect = true; // Trigger effect on this click
                }

                vampireKillButton.Timer = vampireKillButton.MaxTimer;
                vampireKillButton.HasEffect = false;
            },
            () =>
            {
                return Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                Vampire.currentTarget = ImpostorSetTarget();

                bool targetNearGarlic = false;
                if (Vampire.currentTarget != null)
                {
                    foreach (var garlic in Garlic.AllObjects)
                        if (Vector2.Distance(garlic.GameObject.transform.position, Vampire.currentTarget.transform.position) <= 1.95f)
                            targetNearGarlic = true;
                }

                Vampire.targetNearGarlic = targetNearGarlic;

                if (Vampire.targetNearGarlic)
                    vampireKillButton.showTargetNameOnButton(Vampire.currentTarget);
                else
                    vampireKillButton.showTargetNameOnButton(Vampire.currentTarget);
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
                vampireKillButton.Timer = vampireKillButton.MaxTimer;
                vampireKillButton.IsEffectActive = false;
                vampireKillButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Vampire.buttonSprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            false,
            0f,
            () => { vampireKillButton.Timer = vampireKillButton.MaxTimer - Vampire.delay; },
            buttonText: "VampireText".Translate()
        );


        vampireRecruitButton = new CustomButton(
            () =>
            {
                var target = SetTarget();
                if (target == null) return;
                if (target.IsImpostor() || target == Vampire.currentThrall) return;
                if (Vampire.currentThrall != null) return;

                Vampire.currentThrall = target;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireRecruit, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                Vampire.hasRecruited = true;
            },
            () => Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer && !Vampire.hasRecruited,
            () =>
            {
                // 可用条件：可移动，目标可招募，冷却结束
                var target = SetTarget();
                return PlayerControl.LocalPlayer.CanMove && target != null && !target.IsImpostor() && target != Vampire.currentThrall;
            },
            () => { vampireRecruitButton.Timer = vampireRecruitButton.MaxTimer; },
            Vampire.recruitButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: "招募"
        );

        garlicButton = new CustomButton(
            () =>
            {
                Vampire.localPlacedGarlic = true;
                var pos = PlayerControl.LocalPlayer.transform.position;

                var writer = StartRPC(CustomRPC.PlaceGarlic);
                writer.Write(pos);
                writer.EndRPC();
                RPCProcedure.placeGarlic(pos);
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
            __instance,
            __instance.AbilityButton,
            null,
            true,
            buttonText: GetString("GarlicText")
        );

        prophetButton = new CustomButton(
                () =>
                {
                    if (CheckUseAbility(PlayerControl.LocalPlayer, Prophet.currentTarget)) return;
                    if (Prophet.currentTarget != null)
                    {
                        var writer = StartRPC(CustomRPC.ProphetExamine);
                        writer.Write(Prophet.currentTarget.PlayerId);
                        writer.EndRPC();
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

                    prophetButton.UsesCount = Prophet.examinesLeft;
                    return Prophet.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
                },
                () => { prophetButton.Timer = prophetButton.MaxTimer; },
                Prophet.buttonSprite,
                __instance,
                __instance.AbilityButton,
                abilityInput.keyCode,
                buttonText: GetString("ProphetText")
            );

        portalmakerPlacePortalButton = new CustomButton(
            () =>
            {
                portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;

                var writer = StartRPC(CustomRPC.PlacePortal);
                writer.Write(pos);
                writer.EndRPC();
                RPCProcedure.placePortal(pos);
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
            __instance,
            __instance.AbilityButton,
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
                    var writer = StartRPC(CustomRPC.UsePortal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(portalMakerSoloTeleport ? (byte)1 : (byte)0);
                    writer.EndRPC();
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
                    usePortalButton.ButtonTitle.text =
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
                        (Portalmaker.canPortalFromAnywhere &&
                         PlayerControl.LocalPlayer == Portalmaker.portalmaker)) && !Portal.isTeleporting;
            },
            () => { usePortalButton.Timer = usePortalButton.MaxTimer; },
            Portalmaker.usePortalButtonSprite,
            __instance,
            __instance.AbilityButton,
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
                    var writer = StartRPC(CustomRPC.UsePortal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)2);
                    writer.EndRPC();
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
            __instance,
            __instance.AbilityButton,
            null,
            true
        );

        // Jackal Kill
        jackalKillButton = new CustomButton(
            () =>
            {
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, Jackal.killTarget)) return;

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
                var untargetablePlayers = new List<PlayerControl>();
                untargetablePlayers.AddRange(Jackal.jackal);
                if (Jackal.Sidekick != null)
                    untargetablePlayers.Add(Jackal.Sidekick);
                if (SchrodingersCat.State == SchrodingersCat.CatState.Jackal && SchrodingersCat.Player.IsAlive())
                    untargetablePlayers.Add(SchrodingersCat.Player);
                Jackal.killTarget = SetTarget(ignoreList: untargetablePlayers, inVented: ModOption.NeutCanKillInVent);
                SetPlayerOutline(Jackal.killTarget, Palette.ImpostorRed);

                jackalKillButton.showTargetNameOnButton(Jackal.killTarget);
                return Jackal.killTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { jackalKillButton.Timer = jackalKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        // Jackal Sidekick Button
        jackalCreateSidekickButton = new CustomButton(
            () =>
            {
                var target = Jackal.currentTarget;
                if (CheckUseAbility(PlayerControl.LocalPlayer, target)) return;

                if (Jackal.killFakeImpostor && target.IsImpostor())
                {
                    //uncheckedMurderPlayer(Jackal.jackal.PlayerId, player.PlayerId, 1);
                    RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target, deathReason: CustomDeathReason.FakeSK);
                    jackalCreateSidekickButton.Timer = jackalCreateSidekickButton.MaxTimer;
                    return;
                }


                var writer = StartRPC(CustomRPC.JackalCreatesSidekick);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                RPCProcedure.jackalCreatesSidekick(target.PlayerId);
                SoundEffectsManager.play("jackalSidekick");
                jackalCreateSidekickButton.Timer = jackalCreateSidekickButton.MaxTimer;
            },
            () =>
            {
                return Jackal.canCreateSidekick && Jackal.Sidekick == null && Jackal.jackal.Any(x => x.IsAlive() && x == PlayerControl.LocalPlayer);
            },
            () =>
            {

                var untargetablePlayers = new List<PlayerControl>();
                untargetablePlayers.AddRange(Jackal.jackal);
                if (Jackal.Sidekick != null) untargetablePlayers.Add(Jackal.Sidekick);
                if (Mini.mini != null && !Mini.isGrownUp) untargetablePlayers.Add(Mini.mini);
                Jackal.currentTarget = SetTarget(ignoreList: untargetablePlayers, inVented: ModOption.NeutCanKillInVent);
                SetPlayerOutline(Jackal.currentTarget, Palette.ImpostorRed);

                // Show now text since the button already says sidekick
                jackalCreateSidekickButton.showTargetNameOnButton(Jackal.currentTarget);
                return Jackal.canCreateSidekick && Jackal.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { jackalCreateSidekickButton.Timer = jackalCreateSidekickButton.MaxTimer; },
            Jackal.SidekickButton,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("jackalSidekickText")
        );

        jackalSwoopButton = new CustomButton(
            () =>
            { /* On Use */
                var invisibleWriter = StartRPC(CustomRPC.SetJackalSwoop);
                invisibleWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                invisibleWriter.Write(true);
                invisibleWriter.EndRPC();
                RPCProcedure.setJackalSwoop(PlayerControl.LocalPlayer.PlayerId, true);
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
                jackalSwoopButton.IsEffectActive = false;
                jackalSwoopButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Jackal.isInvisable = false;
            },
            Phantom.SwoopButtonSprite,
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            true,
            Jackal.duration,
            () => { jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer; },
            buttonText: GetString("SwoopText")
        );

        phantomKillButton = new CustomButton(
            () =>
            {
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, Phantom.currentTarget)) return;
                phantomKillButton.Timer = phantomKillButton.MaxTimer;
                Phantom.currentTarget = null;
            },
            () => { return Phantom.Player != null && Phantom.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {

                var untargetablePlayers = new List<PlayerControl>();
                if (Mini.mini != null && !Mini.isGrownUp) untargetablePlayers.Add(Mini.mini);
                if (SchrodingersCat.State == SchrodingersCat.CatState.Phantom && SchrodingersCat.Player.IsAlive())
                    untargetablePlayers.Add(SchrodingersCat.Player);
                Phantom.currentTarget = SetTarget(ignoreList: untargetablePlayers, inVented: ModOption.NeutCanKillInVent);
                SetPlayerOutline(Phantom.currentTarget, Palette.ImpostorRed);
                phantomKillButton.showTargetNameOnButton(Phantom.currentTarget);

                return Phantom.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { phantomKillButton.Timer = phantomKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode
        );

        phantomSwoopButton = new CustomButton(
            () =>
            { /* On Use */
                var invisibleWriter = StartRPC(CustomRPC.SetSwoop);
                invisibleWriter.Write(Phantom.Player.PlayerId);
                invisibleWriter.Write(true);
                invisibleWriter.EndRPC();
                RPCProcedure.setSwoop(Phantom.Player.PlayerId, true);
            },
            () => { /* Can See */ return Phantom.Player != null && Phantom.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                /* On Click */
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {  /* On Meeting End */
                phantomSwoopButton.Timer = phantomSwoopButton.MaxTimer;
                phantomSwoopButton.IsEffectActive = false;
                phantomSwoopButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Phantom.isInvisable = false;
            },
            Phantom.SwoopButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            Phantom.duration,
            () => { phantomSwoopButton.Timer = phantomSwoopButton.MaxTimer; },
            buttonText: GetString("SwoopText")
        );

        pavlovsdogsKillButton = new CustomButton(
            () =>
            {
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, Pavlovsdogs.killTarget)) return;
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
                return PlayerControl.LocalPlayer.IsAlive()
                    && Pavlovsdogs.pavlovsdogs.Any(x => x == PlayerControl.LocalPlayer);
            },
            () =>
            {
                if (Pavlovsdogs.enableRampage && Pavlovsdogs.pavlovsowner.IsDead() && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Pavlovsdogs.deathTime -= Time.deltaTime;
                    if (pavlovsdogsKillButton.ButtonTitle != null)
                        pavlovsdogsKillButton.ButtonTitle.text = string.Format("SerialKillerSuicideText".Translate(), (int)Pavlovsdogs.deathTime + 1);

                    if (Pavlovsdogs.deathTime <= 0)
                        PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer, true);
                }

                var untargetablePlayers = new List<PlayerControl>();

                untargetablePlayers.AddRange(Pavlovsdogs.pavlovsdogs);
                if (Pavlovsdogs.pavlovsowner != null)
                    untargetablePlayers.Add(Pavlovsdogs.pavlovsowner);
                if (SchrodingersCat.State == SchrodingersCat.CatState.Pavlovsowner && SchrodingersCat.Player.IsAlive())
                    untargetablePlayers.Add(SchrodingersCat.Player);
                if (Mini.mini != null && !Mini.isGrownUp)
                    untargetablePlayers.Add(Mini.mini);

                untargetablePlayers.AddRange(Pavlovsdogs.pavlovsdogs);
                Pavlovsdogs.killTarget = SetTarget(ignoreList: untargetablePlayers, inVented: ModOption.NeutCanKillInVent);
                SetPlayerOutline(Pavlovsdogs.killTarget, Palette.ImpostorRed);

                pavlovsdogsKillButton.showTargetNameOnButton(Pavlovsdogs.killTarget);

                return Pavlovsdogs.killTarget && PlayerControl.LocalPlayer.CanMove;
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
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        pavlovsownerCreateDogButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Pavlovsdogs.currentTarget)) return;


                var writer = StartRPC(CustomRPC.PavlovsCreateDog);
                writer.Write(Pavlovsdogs.currentTarget.PlayerId);
                writer.EndRPC();
                RPCProcedure.pavlovsCreateDog(Pavlovsdogs.currentTarget.PlayerId);
                SoundEffectsManager.play("jackalSidekick");

                pavlovsownerCreateDogButton.Timer = pavlovsownerCreateDogButton.MaxTimer;
            },
            () =>
            {
                return Pavlovsdogs.pavlovsowner.IsAlive()
                    && Pavlovsdogs.pavlovsowner == PlayerControl.LocalPlayer
                    && Pavlovsdogs.canCreateDog;
            },
            () =>
            {
                pavlovsownerCreateDogButton.UsesCount = Pavlovsdogs.createDogNum;
                var untargetablePlayers = new List<PlayerControl>();
                if (Mini.mini != null && !Mini.isGrownUp) untargetablePlayers.Add(Mini.mini);
                untargetablePlayers.AddRange(Pavlovsdogs.pavlovsdogs);
                Pavlovsdogs.currentTarget = SetTarget(ignoreList: untargetablePlayers, inVented: ModOption.NeutCanKillInVent);
                SetPlayerOutline(Pavlovsdogs.currentTarget, Palette.ImpostorRed);

                // Show now text since the button already says sidekick
                pavlovsownerCreateDogButton.showTargetNameOnButton(Pavlovsdogs.currentTarget);
                return Pavlovsdogs.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { pavlovsownerCreateDogButton.Timer = pavlovsownerCreateDogButton.MaxTimer; },
            Pavlovsdogs.CreateDogButton,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("pavlovsCreateDogText")
        );

        pavlovsdogsRingButton = new CustomButton(
            () =>
            {
                var writer = StartRPC(CustomRPC.PavlovsRing);
                writer.Write(Pavlovsdogs.ringDuration);
                writer.EndRPC();
                SoundEffectsManager.play("ring");
            },
            () =>
            {
                return Pavlovsdogs.pavlovsowner.IsAlive()
                    && Pavlovsdogs.pavlovsowner.AmOwner
                    && Pavlovsdogs.ringEnable
                    && Pavlovsdogs.pavlovsdogs.Any(x => x.IsAlive());
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                pavlovsdogsRingButton.IsEffectActive = false;
                pavlovsdogsRingButton.Timer = pavlovsdogsRingButton.MaxTimer;
            },
            Pavlovsdogs.RingButton,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            Pavlovsdogs.ringDuration,
            () =>
            {
                pavlovsdogsRingButton.IsEffectActive = false;
                pavlovsdogsRingButton.Timer = pavlovsdogsRingButton.MaxTimer;
            },
            buttonText: GetString("pavlovsRingText")
        );

        minerMineButton = new CustomButton(
            () =>
            {
                /* On Use */
                minerMineButton.Timer = minerMineButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
                var id = getAvailableId();

                var writer = StartRPC(CustomRPC.Mine);
                writer.Write(id);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.WriteBytesAndSize(buff);
                writer.Write(0.01f);
                writer.EndRPC();
                RPCProcedure.Mine(id, buff, 0.01f);
            },
            () =>
            {
                /* Can See */
                return Miner.miner.IsAlive() && Miner.miner == PlayerControl.LocalPlayer;
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
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("minerText")
        );

        bomberBombButton = new CustomButton(
            () =>
            {
                /* On Use */
                if (CheckUseAbility(PlayerControl.LocalPlayer, Bomber.currentTarget)) return;
                var bombWriter = StartRPC(CustomRPC.GiveBomb);
                bombWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                bombWriter.Write(Bomber.currentTarget.PlayerId);
                bombWriter.Write(false);
                bombWriter.EndRPC();
                Bomber.giveBomb(PlayerControl.LocalPlayer.PlayerId, Bomber.currentTarget.PlayerId);
                if (Bomber.triggerBothCooldowns)
                {
                    Bomber.Player.killTimer = bomberBombButton.MaxTimer * Mini.Multiplier;
                }
                bomberBombButton.Timer = bomberBombButton.MaxTimer;
            },
            () =>
            {
                /* Can See */
                return Bomber.Player != null && Bomber.Player == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* On Click */
                Bomber.currentTarget = SetTarget();
                if (Bomber.ActiveBomb?.HasBombPlayer == null) SetPlayerOutline(Bomber.currentTarget, Bomber.color);
                return Bomber.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                /* On Meeting End */
                bomberBombButton.Timer = bomberBombButton.MaxTimer;
                bomberBombButton.IsEffectActive = false;
                bomberBombButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Bomber.ActiveBomb?.Destroy();
                Bomber.ActiveBomb = null;
            },
            Bomber.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: "giveBombText".Translate()
        );

        bomberGiveButton = new CustomButton(
            () =>
            {
                /* On Use */
                if (CheckUseAbility(PlayerControl.LocalPlayer, Bomber.currentBombTarget)) return;

                if (!Bomber.canGiveToBomber && Bomber.currentBombTarget == Bomber.Player)
                {
                    var bombPlayer = Bomber.ActiveBomb?.HasBombPlayer;
                    if (bombPlayer != null) RpcCustomMurderPlayer(Bomber.Player, bombPlayer, false);

                    var clearWriter = StartRPC(CustomRPC.GiveBomb);
                    clearWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                    clearWriter.Write(byte.MaxValue);
                    clearWriter.Write(false);
                    clearWriter.EndRPC();
                    Bomber.giveBomb(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                    return;
                }

                if (Bomber.hotPotatoMode)
                {
                    var bombWriter = StartRPC(CustomRPC.GiveBomb);
                    bombWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                    bombWriter.Write(Bomber.currentBombTarget.PlayerId);
                    bombWriter.Write(true);
                    bombWriter.EndRPC();
                    Bomber.giveBomb(PlayerControl.LocalPlayer.PlayerId, Bomber.currentBombTarget.PlayerId, true);
                }
                else
                {
                    var bombPlayer = Bomber.ActiveBomb?.HasBombPlayer;
                    if (bombPlayer != null) RpcCustomMurderPlayer(bombPlayer, Bomber.currentBombTarget, false);
                    var bombWriter = StartRPC(CustomRPC.GiveBomb);
                    bombWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                    bombWriter.Write(byte.MaxValue);
                    bombWriter.Write(false);
                    bombWriter.EndRPC();
                    Bomber.giveBomb(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                }
            },
            () =>
            {
                /* Can See */
                return Bomber.Player != null && Bomber.ActiveBomb?.HasBombPlayer == PlayerControl.LocalPlayer &&
                       Bomber.ActiveBomb.IsActive && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                /* Can Click */
                Bomber.currentBombTarget = SetTarget();
                if (Bomber.ActiveBomb?.HasBombPlayer == null) SetPlayerOutline(Bomber.currentTarget, Bomber.color);
                return Bomber.currentBombTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                /* On Meeting End */
            },
            Bomber.buttonSprite,
            __instance,
            __instance.AbilityButton,
            hotkey: null,
            PositionOffset: new Vector3(-4.5f, 1.5f, 0),
            useGrid: false,
            buttonText: "giveBombText".Translate()
        );

        grenadierFlashButton = new CustomButton(
            () =>
            {
                /* On Use */
                var writer = StartRPC(CustomRPC.GrenadierFlash);
                writer.Write(false);
                writer.EndRPC();
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
                grenadierFlashButton.IsEffectActive = false;
                grenadierFlashButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Grenadier.ButtonSprite,
            __instance,
            __instance.AbilityButton,
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
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, Werewolf.currentTarget)) return;

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

                var untargetablePlayers = new List<PlayerControl>();
                if (SchrodingersCat.State == SchrodingersCat.CatState.Werewolf && SchrodingersCat.Player.IsAlive())
                    untargetablePlayers.Add(SchrodingersCat.Player);

                Werewolf.currentTarget = SetTarget(ignoreList: untargetablePlayers, inVented: ModOption.NeutCanKillInVent);
                werewolfKillButton.showTargetNameOnButton(Werewolf.currentTarget);
                return Werewolf.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { werewolfKillButton.Timer = werewolfKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
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
                werewolfRampageButton.IsEffectActive = false;
                werewolfRampageButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                Werewolf.canKill = false;
                //  Werewolf.canUseVents = false;
                Werewolf.hasImpostorVision = false;
            },
            Werewolf.buttonSprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            true,
            Werewolf.rampageDuration,
            () =>
            {
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
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, Juggernaut.currentTarget)) return;

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
                var untargetablePlayers = new List<PlayerControl>();
                if (SchrodingersCat.State == SchrodingersCat.CatState.Juggernaut && SchrodingersCat.Player.IsAlive())
                    untargetablePlayers.Add(SchrodingersCat.Player);

                Juggernaut.currentTarget = SetTarget(ignoreList: untargetablePlayers, inVented: ModOption.NeutCanKillInVent);
                juggernautKillButton.showTargetNameOnButton(Juggernaut.currentTarget);
                return Juggernaut.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { juggernautKillButton.Timer = juggernautKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        // 鹈鹕击杀 Kill
        pelicanKillButton = new CustomButton(
            () =>
            {
                if (Pelican.currentTarget == null) return;

                if (!CheckMurderPlayer(PlayerControl.LocalPlayer, Pelican.currentTarget)) return;

                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.PelicanKill);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(Pelican.currentTarget.PlayerId);
                writer.EndRPC();
                Pelican.PelicanKill(PlayerControl.LocalPlayer.PlayerId, Pelican.currentTarget.PlayerId);

                pelicanKillButton.Timer = Pelican.reduceCooldown;
                Pelican.currentTarget = null;
            },
            () =>
            {
                return Pelican.Player != null && Pelican.Player == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                var untargetablePlayers = new List<PlayerControl>();
                if (Mini.mini != null && !Mini.isGrownUp) untargetablePlayers.Add(Mini.mini);
                Pelican.currentTarget = SetTarget(ignoreList: untargetablePlayers, inVented: ModOption.NeutCanKillInVent);
                SetPlayerOutline(Pelican.currentTarget, Palette.ImpostorRed);

                pelicanKillButton.showTargetNameOnButton(Pelican.currentTarget);
                return Pelican.currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                //pelicanKillButton.MaxTimer = Pelican.cooldown;
                pelicanKillButton.Timer = pelicanKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("VultureText")
        );

        // Eraser erase button
        eraserButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Eraser.currentTarget)) return;
                eraserButton.MaxTimer += 10;
                eraserButton.Timer = eraserButton.MaxTimer;

                var writer = StartRPC(CustomRPC.SetFutureErased);
                writer.Write(Eraser.currentTarget.PlayerId);
                writer.EndRPC();
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
                Eraser.currentTarget = SetTarget(ignoreList: Eraser.canEraseAnyone ? [] : untargetables, !Eraser.canEraseAnyone);
                SetPlayerOutline(Eraser.currentTarget, Eraser.color);

                eraserButton.showTargetNameOnButton(Eraser.currentTarget);
                return PlayerControl.LocalPlayer.CanMove && Eraser.currentTarget != null;
            },
            () => { eraserButton.Timer = eraserButton.MaxTimer; },
            Eraser.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("EraserText")
        );

        partTimerButton = new CustomButton(
            () =>
            {
                if (PartTimer.currentTarget == null) return;
                if (CheckUseAbility(PlayerControl.LocalPlayer, PartTimer.currentTarget)) return;

                var writer = StartRPC(CustomRPC.PartTimerSet);
                writer.Write(PartTimer.currentTarget.PlayerId);
                writer.EndRPC();
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

                partTimerButton.showTargetNameOnButton(PartTimer.currentTarget);
                return PlayerControl.LocalPlayer.CanMove && PartTimer.currentTarget != null; ;
            },
            () => { partTimerButton.Timer = partTimerButton.MaxTimer; },
            PartTimer.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("partTimerButton")
        );


        placeJackInTheBoxButton = new CustomButton(
            () =>
            {
                placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;

                var writer = StartRPC(CustomRPC.PlaceJackInTheBox);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(pos);
                writer.EndRPC();
                RPCProcedure.placeJackInTheBox(PlayerControl.LocalPlayer, pos);
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
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("TricksterPlaceText")
        );

        lightsOutButton = new CustomButton(
            () =>
            {
                var writer = StartRPC(CustomRPC.LightsOut);
                writer.EndRPC();
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
                JackInTheBox.MeetingEnd();
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
                lightsOutButton.IsEffectActive = false;
                lightsOutButton.actionButton.graphic.color = Palette.EnabledColor;
            },
            Trickster.lightOutButtonSprite,
            __instance,
            __instance.AbilityButton,
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
                var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());

                var writer = StartRPC(CustomRPC.CleanBody);
                writer.Write(db.ParentId);
                writer.Write(Cleaner.cleaner.PlayerId);
                writer.EndRPC();
                RPCProcedure.cleanBody(db.ParentId, Cleaner.cleaner.PlayerId);

                Cleaner.cleaner.killTimer = cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
                SoundEffectsManager.play("cleanerClean");
            },
            () =>
            {
                return Cleaner.cleaner != null && Cleaner.cleaner == PlayerControl.LocalPlayer &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());
                return db != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer; },
            Cleaner.buttonSprite,
            __instance,
            __instance.AbilityButton,
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
                var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());
                Butcher.dissectedId = db?.ParentId ?? byte.MaxValue;
                return db != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { butcherDissectionButton.Timer = butcherDissectionButton.MaxTimer; },
            Butcher.ButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            Butcher.dissectionDuration,
            () =>
            {
                if (Butcher.dissectedId == byte.MaxValue) return;
                var list = new List<Vector3>();
                list.AddRange(MapData.MapSpawnPosition(false));
                list.AddRange(MapData.FindVentSpawnPositions(false));
                list.Shuffle();

                for (var i = 0; i < Butcher.dissectedBodyCount; i++)
                {
                    var pos = list.RandomTake();
                    var writer = StartRPC(CustomRPC.CreateDeadBody);
                    writer.Write(Butcher.dissectedId);
                    writer.Write(pos);
                    writer.Write(i);
                    writer.EndRPC();
                    RPCProcedure.CreateDeadBody(Butcher.dissectedId, pos, i);
                    list.Remove(pos);
                }

                Butcher.dissectedId = byte.MaxValue;
                Butcher.canDissection = false;
                SoundEffectsManager.play("cleanerClean");
            },
            buttonText: GetString("DissectionText")
        );

        // Undertaker Button
        undertakerDragButton = new CustomButton(
            () =>
            {
                if (Undertaker.dragedBody != null)
                {
                    var writer = StartRPC(CustomRPC.UndertakerDragAction);
                    writer.Write(byte.MaxValue);
                    writer.EndRPC();
                    Undertaker.DragBody(byte.MaxValue);
                }
                else if (Undertaker.targetBody != null)
                {
                    var writer = StartRPC(CustomRPC.UndertakerDragAction);
                    writer.Write(Undertaker.targetBody.ParentId);
                    writer.EndRPC();
                    Undertaker.DragBody(Undertaker.targetBody.ParentId);
                }
            },
            () =>
            {
                return Undertaker.undertaker.IsAlive() && Undertaker.undertaker == PlayerControl.LocalPlayer;
            },
            () =>
            {
                var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());

                Undertaker.targetBody = db;
                return (Undertaker.targetBody || Undertaker.dragedBody) && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            Undertaker.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("DragBodyText")
        );

        // Jester Button
        JesterDragButton = new CustomButton(
            () =>
            {
                if (Jester.dragedBodys.GetValueOrDefault(PlayerControl.LocalPlayer.PlayerId) != null)
                {
                    var writer = StartRPC(CustomRPC.jesterDragBody);
                    writer.Write(PlayerControl.LocalPlayer);
                    writer.Write(byte.MaxValue);
                    writer.EndRPC();
                    Jester.DragBody(PlayerControl.LocalPlayer, byte.MaxValue);
                }
                else if (Jester.targetBody != null)
                {
                    var writer = StartRPC(CustomRPC.jesterDragBody);
                    writer.Write(PlayerControl.LocalPlayer);
                    writer.Write(Jester.targetBody.ParentId);
                    writer.EndRPC();
                    Jester.DragBody(PlayerControl.LocalPlayer, Jester.targetBody.ParentId);
                }
            },
            () =>
            {
                return Jester.Player != null && Jester.canDragDeadBody && Jester.Player.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                       && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());

                Jester.targetBody = db;

                return (Jester.targetBody || Jester.dragedBodys.GetValueOrDefault(PlayerControl.LocalPlayer.PlayerId)) && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            Undertaker.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("DragBodyText")
        );

        // Warlock curse
        warlockCurseButton = new CustomButton(
            () =>
            {
                if (Warlock.curseVictim == null)
                {
                    if (CheckUseAbility(PlayerControl.LocalPlayer, Warlock.currentTarget)) return;
                    // Apply Curse
                    Warlock.curseVictim = Warlock.currentTarget;
                    warlockCurseButton.Sprite = Warlock.curseKillButtonSprite;
                    warlockCurseButton.Timer = 1f;
                    SoundEffectsManager.play("warlockCurse");

                    // Ghost Info
                    var writer = StartRPC(CustomRPC.ShareGhostInfo);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.WarlockTarget);
                    writer.Write(Warlock.curseVictim.PlayerId);
                    writer.EndRPC();
                }
                else if (Warlock.curseVictim != null && Warlock.curseVictimTarget != null)
                {
                    if (!RpcCustomMurderPlayer(Warlock.warlock, Warlock.curseVictimTarget, false)) return;

                    // If blanked or killed
                    if (Warlock.RootTime > 0)
                    {
                        AntiTeleport.position = PlayerControl.LocalPlayer.transform.position;
                        PlayerControl.LocalPlayer.moveable = false;
                        // Stop current movement so the warlock is not just running straight into the next object
                        PlayerControl.LocalPlayer.NetTransform.Halt();
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Warlock.RootTime,
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

                    var writer = StartRPC(CustomRPC.ShareGhostInfo);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.WarlockTarget);
                    writer.Write(byte.MaxValue); // This will set it to null!
                    writer.EndRPC();
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
                if (Warlock.curseVictim != null && Warlock.curseVictim.IsDead())
                    Warlock.resetCurse();
                if (Warlock.curseVictim == null)
                {
                    Warlock.currentTarget = SetTarget();
                    SetPlayerOutline(Warlock.currentTarget, Warlock.color);
                }
                else
                {
                    Warlock.curseVictimTarget = SetTarget(sourcePlayer: Warlock.curseVictim, distances: 0.75f, onlyCrewmates: !Warlock.FriendlyFire);
                    SetPlayerOutline(Warlock.curseVictimTarget, Warlock.color);
                }

                if (Warlock.curseVictim != null)
                    warlockCurseButton.showTargetNameOnButton(Warlock.currentTarget);
                else
                    warlockCurseButton.showTargetNameOnButton(Warlock.currentTarget);
                return ((Warlock.curseVictim == null && Warlock.currentTarget != null) ||
                        (Warlock.curseVictim != null && Warlock.curseVictimTarget != null)) &&
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
            __instance,
            __instance.KillButton,
            abilityInput.keyCode,
            buttonText: GetString("CurseText")
        )
        { UseGridPriority = 26 };

        // Security Guard button
        securityGuardButton = new CustomButton(
            () =>
            {
                if (SecurityGuard.ventTarget != null)
                {
                    // Seal vent
                    var writer = StartRPC(CustomRPC.SealVent);
                    writer.WritePacked(SecurityGuard.ventTarget.Id);
                    writer.EndRPC();
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

                    var writer = StartRPC(CustomRPC.PlaceCamera);
                    writer.WriteBytesAndSize(buff);
                    writer.EndRPC();
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
                if (securityGuardButton.ButtonTitle != null)
                    securityGuardButton.ButtonTitle.text = $"{SecurityGuard.remainingScrews}/{SecurityGuard.totalScrews}";


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

                securityGuardButton.buttonText = target != null ? GetString("SealVentText") : GetString("PlaceCamText");

                if (SecurityGuard.ventTarget != null)
                    return SecurityGuard.remainingScrews >= SecurityGuard.ventPrice &&
                           PlayerControl.LocalPlayer.CanMove;
                return !isMira && !isFungle && !SubmergedCompatibility.IsSubmerged &&
                       SecurityGuard.remainingScrews >= SecurityGuard.camPrice &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { securityGuardButton.Timer = securityGuardButton.MaxTimer; },
            SecurityGuard.placeCameraButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode
        );

        securityGuardCamButton = new CustomButton(
            () =>
            {
                if (!isMira)
                {
                    if (SecurityGuard.minigame == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("Surv_Panel") || x.name.Contains("Cam") ||
                            x.name.Contains("BinocularsSecurityConsole"));
                        if (isSkeld || mapId == 3)
                            e = UObject.FindObjectsOfType<SystemConsole>()
                                .FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
                        else if (isAirship)
                            e = UObject.FindObjectsOfType<SystemConsole>()
                                .FirstOrDefault(x => x.gameObject.name.Contains("task_cams"));
                        if (e == null || Camera.main == null) return;
                        SecurityGuard.minigame = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    SecurityGuard.minigame.transform.SetParent(Camera.main.transform, false);
                    SecurityGuard.minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    SecurityGuard.minigame.Begin(null);
                }
                else
                {
                    if (SecurityGuard.minigame == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>()
                            .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        SecurityGuard.minigame = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
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
                if (securityGuardCamButton.ButtonTitle != null)
                    securityGuardCamButton.ButtonTitle.text = $"{SecurityGuard.charges} / {SecurityGuard.maxCharges}";
                securityGuardCamButton.actionButton.graphic.sprite =
                    isMira ? SecurityGuard.getLogSprite() : SecurityGuard.getCamSprite();
                securityGuardCamButton.actionButton.OverrideText(isMira ?
                    GetString("hackerDoorLogText") : GetString("CamButtonText"));
                return PlayerControl.LocalPlayer.CanMove && SecurityGuard.charges > 0;
            },
            () =>
            {
                securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                securityGuardCamButton.IsEffectActive = false;
                securityGuardCamButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            SecurityGuard.getCamSprite(),
            __instance,
            __instance.AbilityButton,
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

        // Arsonist button (涂油)
        arsonistButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Arsonist.currentTarget)) return;
                Arsonist.douseTarget = Arsonist.currentTarget;
                arsonistButton.HasEffect = true;
                SoundEffectsManager.play("arsonistDouse");
            },
            () =>
            {
                return Arsonist.arsonist.IsAlive() && Arsonist.arsonist == PlayerControl.LocalPlayer;
            },
            () =>
            {
                List<PlayerControl> untargetables;
                if (Arsonist.douseTarget != null)
                {
                    untargetables = new();
                    foreach (var player in PlayerControl.AllPlayerControls)
                        if (player.PlayerId != Arsonist.douseTarget.PlayerId)
                            untargetables.Add(player);
                }
                else
                {
                    untargetables = Arsonist.dousedPlayers;
                }

                Arsonist.currentTarget = SetTarget(ignoreList: untargetables, distances: 0.75f, inVented: ModOption.NeutCanKillInVent);
                if (Arsonist.currentTarget != null) SetPlayerOutline(Arsonist.currentTarget, Arsonist.color);

                arsonistButton.showTargetNameOnButton(Arsonist.currentTarget);

                if (arsonistButton.IsEffectActive && Arsonist.douseTarget != Arsonist.currentTarget)
                {
                    Arsonist.douseTarget = null;
                    arsonistButton.Timer = 0f;
                    arsonistButton.IsEffectActive = false;
                }

                return PlayerControl.LocalPlayer.CanMove && Arsonist.currentTarget != null;
            },
            () =>
            {
                arsonistButton.Timer = arsonistButton.MaxTimer;
                arsonistButton.IsEffectActive = false;
                Arsonist.douseTarget = null;
            },
            Arsonist.douseSprite,
            __instance,
            __instance.AbilityButton,
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
                var writer = StartRPC(CustomRPC.ShareGhostInfo);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.ArsonistDouse);
                writer.Write(Arsonist.douseTarget.PlayerId);
                writer.EndRPC();

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
                    RpcCustomMurderPlayer(Arsonist.arsonist, p, false, true, CustomDeathReason.Arson);
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
                Arsonist.currentTarget2 = SetTarget(distances: 0.75f, inVented: ModOption.NeutCanKillInVent);
                var cankill = false;
                if (Arsonist.currentTarget2 && Arsonist.dousedPlayers.Any(x => x == Arsonist.currentTarget2))
                {
                    SetPlayerOutline(Arsonist.currentTarget2, Arsonist.color);
                    arsonistKillButton.showTargetNameOnButton(Arsonist.currentTarget2);
                    cankill = true;
                }

                return PlayerControl.LocalPlayer.CanMove && cankill;
            },
            () =>
            {
                var count = PlayerControl.AllPlayerControls.Count(p => p.IsAlive() && p.IsKiller() && p != Arsonist.arsonist);

                if (count == 0 && Arsonist.igniteCooldownRemoved) arsonistKillButton.Timer = arsonistKillButton.MaxTimer = 0f;
                else arsonistKillButton.Timer = arsonistKillButton.MaxTimer = arsonistButton.MaxTimer;
            },
            Arsonist.igniteSprite,
            __instance,
            __instance.AbilityButton,
            modKillInput.keyCode,
            buttonText: GetString("IgniteText")
        );

        // Vulture Eat
        vultureEatButton = new CustomButton(
            () =>
            {
                var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());
                var writer = StartRPC(CustomRPC.CleanBody);
                writer.Write(db.ParentId);
                writer.Write(Vulture.vulture.PlayerId);
                writer.EndRPC();
                RPCProcedure.cleanBody(db.ParentId, Vulture.vulture.PlayerId);

                Vulture.cooldown = vultureEatButton.Timer = vultureEatButton.MaxTimer;
                SoundEffectsManager.play("vultureEat");
            },
            () =>
            {
                return Vulture.vulture.IsAlive() && Vulture.vulture == PlayerControl.LocalPlayer;
            },
            () =>
            {
                var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());
                return db != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { vultureEatButton.Timer = vultureEatButton.MaxTimer; },
            Vulture.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("VultureText")
        );

        amnisiacRememberButton = new CustomButton(
            () =>
            {
                var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());

                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.AmnisiacTakeRole);
                writer.Write(db.ParentId);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.EndRPC();
                Amnisiac.TakeRole(db.ParentId, PlayerControl.LocalPlayer.PlayerId);
            },
            () =>
            {
                return Amnisiac.Player != null && Amnisiac.Player.Any(x => x == PlayerControl.LocalPlayer) &&
                       !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () =>
            {
                var db = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());
                return db != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { amnisiacRememberButton.Timer = 0f; },
            Amnisiac.buttonSprite,
            __instance,
            __instance.AbilityButton,
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
                var pos = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = PlayerControl.LocalPlayer.MaxReportDistance * 0.36f;
                var target = GetDeadPlayer(pos, maxDistance);
                Specter.Target = target;

                specterRememberButton.showTargetNameOnButton(Specter.Target);
                return target != null && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                specterRememberButton.Timer = 10f;
            },
            Amnisiac.buttonSprite,
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            true,
            Specter.duration,
            () =>
            {
                var target = Specter.Target;
                if (!Specter.afterMeetingRevive) PlayerControl.LocalPlayer.transform.position = PlayerControl.LocalPlayer.GetCloseSpawnPosition();
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.SpecterTakeRole);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                Specter.TakeRole(target.PlayerId);
            },
            buttonText: GetString("ReviveButton")
        );

        // Medium button
        alchemystButton = new CustomButton(
            () =>
            {
                if (Alchemyst.target != null)
                {
                    Alchemyst.soulTarget = Alchemyst.target;
                    alchemystButton.HasEffect = true;
                    SoundEffectsManager.play("mediumAsk");
                }
            },
            () =>
            {
                return Alchemyst.Player.IsAlive() && Alchemyst.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {

                Alchemyst.DeadPlayer target = null;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var closestDistance = float.MaxValue;
                var usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
                foreach (var (dp, ps) in Alchemyst.deadBodies)
                {
                    var distance = Vector2.Distance(ps, truePosition);
                    if (distance <= usableDistance && distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = dp;
                    }
                }
                Alchemyst.target = target;

                if (alchemystButton.IsEffectActive && Alchemyst.target != Alchemyst.soulTarget)
                {
                    Alchemyst.soulTarget = null;
                    alchemystButton.Timer = 0f;
                    alchemystButton.IsEffectActive = false;
                }

                return Alchemyst.target != null && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                alchemystButton.Timer = alchemystButton.MaxTimer;
                alchemystButton.IsEffectActive = false;
                Alchemyst.soulTarget = null;
            },
            Alchemyst.question,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            Alchemyst.duration,
            () =>
            {
                alchemystButton.Timer = alchemystButton.MaxTimer;
                if (Alchemyst.target == null || Alchemyst.target.Player == null) return;
                var msg = Alchemyst.getInfo(Alchemyst.target.Player, Alchemyst.target.KilledBy);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);

                // Ghost Info
                var writer = StartRPC(CustomRPC.ShareGhostInfo);
                writer.Write(Alchemyst.target.Player.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
                writer.Write(msg);
                writer.EndRPC();

                if (!Alchemyst.canUseKill)
                {
                    Alchemyst.skillUseCount++;
                    if (Alchemyst.skillUseCount >= Alchemyst.requiredUses)
                    {
                        Alchemyst.canUseKill = true;
                    }
                }

                // Remove soul
                if (Alchemyst.oneTimeUse)
                {
                    var closestDistance = float.MaxValue;
                    SpriteRenderer target = null;

                    foreach (var (db, ps) in Alchemyst.deadBodies)
                        if (db == Alchemyst.target)
                        {
                            var deadBody = Tuple.Create(db, ps);
                            Alchemyst.deadBodies.Remove(deadBody);
                            break;
                        }

                    foreach (var rend in Alchemyst.souls)
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

                        if (p == 1f && target != null && target.gameObject != null) UObject.Destroy(target.gameObject);
                    })));

                    Alchemyst.souls.Remove(target);
                }

                SoundEffectsManager.stop("mediumAsk");
            },
            buttonText: GetString("MediumText")
        );

        alchemystKillButton = new CustomButton(
            () =>
            {
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, Alchemyst.CurrentTarget)) return;

                Alchemyst.canUseKill = false;
                Alchemyst.skillUseCount = 0;
                alchemystKillButton.Timer = alchemystKillButton.MaxTimer;
                Alchemyst.CurrentTarget = null;
            },
             () =>
             {
                 return Alchemyst.Player.IsAlive() && Alchemyst.Player == PlayerControl.LocalPlayer;
             },
             () =>
             {
                 Alchemyst.CurrentTarget = SetTarget(inVented: ModOption.CanKillInVent);
                 SetPlayerOutline(Alchemyst.CurrentTarget, Alchemyst.color);
                 alchemystKillButton.showTargetNameOnButton(Alchemyst.CurrentTarget);

                 if (alchemystKillButton.ButtonTitle != null) alchemystKillButton.ButtonTitle.text = $"{Alchemyst.skillUseCount} / {Alchemyst.requiredUses}";

                 return PlayerControl.LocalPlayer.CanMove && Alchemyst.CurrentTarget != null && Alchemyst.canUseKill;
             },
              () =>
              {
                  alchemystKillButton.Timer = alchemystKillButton.MaxTimer;
              },
             __instance.KillButton.graphic.sprite,
             __instance,
             __instance.KillButton,
              modKillInput.keyCode,
             buttonText: GetString("killButtonText")
        );

        // Pursuer button
        pursuerButton = new CustomButton(
            () =>
            {
                if (Pursuer.target != null)
                {
                    if (CheckUseAbility(PlayerControl.LocalPlayer, Pursuer.target)) return;
                    var writer = StartRPC(CustomRPC.SetBlanked);
                    writer.Write(Pursuer.target.PlayerId);
                    writer.Write(true);
                    writer.EndRPC();
                    RPCProcedure.SetBlanked(Pursuer.target.PlayerId, true);

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
                pursuerButton.UsesCount = Pursuer.blanksNumber - Pursuer.blanks;
                Pursuer.target = SetTarget();
                SetPlayerOutline(Pursuer.target, Pursuer.color);

                pursuerButton.showTargetNameOnButton(Pursuer.target);
                return Pursuer.blanksNumber > Pursuer.blanks && PlayerControl.LocalPlayer.CanMove && Pursuer.target != null;
            },
            () => { pursuerButton.Timer = pursuerButton.MaxTimer; },
            Pursuer.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("PursuerText")
        );

        survivorVestButton = new CustomButton(
            () =>
            {
                var writer = StartRPC(CustomRPC.SurvivorVestActive);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(Survivor.vestDuration);
                writer.EndRPC();
                RPCProcedure.survivorVestActive(PlayerControl.LocalPlayer.PlayerId, Survivor.vestDuration);
                Survivor.vestUsed++;
            },
            () =>
            {
                return Survivor.Player != null && Survivor.Player.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) &&
                       PlayerControl.LocalPlayer.IsAlive() && Survivor.vestEnable/* && Survivor.remainingVests > 0*/;
            },
            () =>
            {
                survivorVestButton.UsesCount = Survivor.remainingVests;
                return PlayerControl.LocalPlayer.CanMove && Survivor.remainingVests > 0;
            },
            () =>
            {
                survivorVestButton.Timer = survivorVestButton.MaxTimer;
                survivorVestButton.IsEffectActive = false;
                survivorVestButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Survivor.VestButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            Survivor.vestDuration,
            () => { survivorVestButton.Timer = survivorVestButton.MaxTimer; },
            buttonText: GetString("VestButton")
        );

        // Survivor button
        survivorBlanksButton = new CustomButton(
            () =>
            {
                if (Survivor.target != null)
                {
                    if (CheckUseAbility(PlayerControl.LocalPlayer, Survivor.target)) return;
                    var writer = StartRPC(CustomRPC.SetBlanked);
                    writer.Write(Survivor.target.PlayerId);
                    writer.Write(true);
                    writer.EndRPC();
                    RPCProcedure.SetBlanked(Survivor.target.PlayerId, true);

                    Survivor.target = null;

                    Survivor.blanksUsed++;
                    survivorBlanksButton.Timer = survivorBlanksButton.MaxTimer;
                    SoundEffectsManager.play("pursuerBlank");
                }
            },
            () =>
            {
                return Survivor.Player != null && Survivor.Player.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                       && PlayerControl.LocalPlayer.IsAlive() && Survivor.blanksEnable/* && Survivor.remainingBlanks > 0*/;
            },
            () =>
            {
                Survivor.target = SetTarget();
                SetPlayerOutline(Survivor.target, Survivor.color);
                survivorBlanksButton.showTargetNameOnButton(Survivor.target);

                survivorBlanksButton.UsesCount = Survivor.remainingBlanks;
                return Survivor.blanksNumber > Survivor.blanksUsed && PlayerControl.LocalPlayer.CanMove && Survivor.target != null;
            },
            () => { survivorBlanksButton.Timer = survivorBlanksButton.MaxTimer; },
            Pursuer.buttonSprite,
            __instance,
            __instance.AbilityButton,
            KeyCode.C,
            buttonText: GetString("PursuerText")
        );

        // Witch Spell button
        witchSpellButton = new CustomButton(
            () =>
            {
                if (Witch.currentTarget != null)
                {
                    if (CheckUseAbility(PlayerControl.LocalPlayer, Witch.currentTarget)) return;
                    Witch.spellCastingTarget = Witch.currentTarget;
                    SoundEffectsManager.play("witchSpell");
                }
            },
            () =>
            {
                return Witch.witch.IsAlive() && Witch.witch == PlayerControl.LocalPlayer;
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

                Witch.currentTarget = SetTarget(untargetables, !Witch.canSpellAnyone, distances: Witch.spellRangeExtension);
                SetPlayerOutline(Witch.currentTarget, Witch.color);

                witchSpellButton.showTargetNameOnButton(Witch.currentTarget);
                if (witchSpellButton.IsEffectActive && Witch.spellCastingTarget != Witch.currentTarget)
                {
                    Witch.spellCastingTarget = null;
                    witchSpellButton.Timer = 0f;
                    witchSpellButton.IsEffectActive = false;
                }
                return PlayerControl.LocalPlayer.CanMove && Witch.currentTarget != null;
            },
            () =>
            {
                witchSpellButton.Timer = witchSpellButton.MaxTimer;
                witchSpellButton.IsEffectActive = false;
                Witch.spellCastingTarget = null;
            },
            Witch.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            Witch.spellCastingDuration,
            () =>
            {
                if (Witch.spellCastingTarget == null) return;

                var writer = StartRPC(CustomRPC.SetFutureSpelled);
                writer.Write(Witch.currentTarget.PlayerId);
                writer.EndRPC();
                RPCProcedure.setFutureSpelled(Witch.currentTarget.PlayerId);

                Witch.currentCooldownAddition += Witch.cooldownAddition;
                witchSpellButton.MaxTimer = Witch.cooldown + Witch.currentCooldownAddition;
                witchSpellButton.Timer = witchSpellButton.MaxTimer;

                if (Witch.triggerBothCooldowns)
                {
                    Witch.witch.killTimer = witchSpellButton.Timer;
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
            __instance,
            __instance.AbilityButton,
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
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: "jumperJumpText".Translate()
        );

        bountyHunterChangeTarget = new CustomButton(
            () =>
            {
                BountyHunter.bounty = null;
                bountyHunterChangeTarget.Timer = bountyHunterChangeTarget.MaxTimer;
            },
            () =>
            {
                return BountyHunter.bountyHunter.IsAlive() && BountyHunter.bountyHunter == PlayerControl.LocalPlayer;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () => { bountyHunterChangeTarget.Timer = bountyHunterChangeTarget.MaxTimer; },
            BountyHunter.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("ChangeTarget")
            );

        // Ninja mark and assassinate button 
        ninjaButton = new CustomButton(
            () =>
            {
                MessageWriter writer;
                if (Ninja.ninjaMarked.IsAlive())
                {
                    // Create first trace before killing
                    var pos = PlayerControl.LocalPlayer.transform.position;

                    writer = StartRPC(CustomRPC.PlaceNinjaTrace);
                    writer.Write(pos);
                    writer.EndRPC();
                    RPCProcedure.placeNinjaTrace(pos);

                    var invisibleWriter = StartRPC(CustomRPC.SetInvisible);
                    invisibleWriter.Write(Ninja.ninja.PlayerId);
                    invisibleWriter.Write(byte.MinValue);
                    invisibleWriter.EndRPC();
                    RPCProcedure.setInvisible(Ninja.ninja.PlayerId, byte.MinValue);

                    if (SubmergedCompatibility.IsSubmerged)
                        SubmergedCompatibility.ChangeFloor(Ninja.ninjaMarked.transform.localPosition.y > -7);

                    // Perform Kill
                    if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, Ninja.ninjaMarked, true))
                        Ninja.ninja.killTimer = ninjaButton.Timer = ninjaButton.MaxTimer;

                    // Create Second trace after killing
                    pos = Ninja.ninjaMarked.transform.position;

                    var writer3 = StartRPC(CustomRPC.PlaceNinjaTrace);
                    writer3.Write(pos);
                    writer3.EndRPC();
                    RPCProcedure.placeNinjaTrace(pos);

                    Ninja.ninjaMarked = null;
                }
                else
                {
                    if (CheckUseAbility(PlayerControl.LocalPlayer, Ninja.currentTarget)) return;
                    Ninja.ninjaMarked = Ninja.currentTarget;
                    ninjaButton.Timer = 5f;
                    SoundEffectsManager.play("warlockCurse");

                    // Ghost Info
                    writer = StartRPC(CustomRPC.ShareGhostInfo);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.GhostInfoTypes.NinjaMarked);
                    writer.Write(Ninja.ninjaMarked.PlayerId);
                    writer.EndRPC();
                }
            },
            () =>
            {
                return Ninja.ninja.IsAlive() && Ninja.ninja == PlayerControl.LocalPlayer;
            },
            () =>
            {
                Ninja.currentTarget = ImpostorSetTarget();
                ninjaButton.showTargetNameOnButton(Ninja.currentTarget);
                ninjaButton.Sprite = Ninja.ninjaMarked.IsAlive()
                    ? Ninja.killButtonSprite
                    : Ninja.markButtonSprite;
                return (Ninja.currentTarget != null || (Ninja.ninjaMarked != null
                        && !Ninja.ninjaMarked.IsUsingTransportation))
                        && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                // on meeting ends
                ninjaButton.Timer = ninjaButton.MaxTimer;
                Ninja.ninjaMarked = null;
            },
            Ninja.markButtonSprite,
            __instance,
            __instance.KillButton,
            abilityInput.keyCode,
            buttonText: GetString("NinjaText")
        );

        blackmailerButton = new CustomButton(
            () =>
            {
                // Action when Pressed
                var target = Blackmailer.currentTarget;
                if (CheckUseAbility(PlayerControl.LocalPlayer, target)) return;

                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.BlackmailPlayer);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                RPCProcedure.blackmailPlayer(target.PlayerId);
                Blackmailer.currentTarget = null;
            },
            () =>
            {
                return Blackmailer.Player.IsAlive() && Blackmailer.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                // Could Use
                Blackmailer.currentTarget = SetTarget();
                SetPlayerOutline(Blackmailer.currentTarget, Blackmailer.blackmailedColor);

                if (Blackmailer.blackmailed == null)
                {
                    blackmailerButton.showTargetNameOnButton(Blackmailer.currentTarget);
                }
                return Blackmailer.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { blackmailerButton.Timer = blackmailerButton.MaxTimer; },
            Blackmailer.blackmailButtonSprite,
            __instance,
            __instance.AbilityButton,
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

                var writer = StartRPC(CustomRPC.SetTrap);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.WriteBytesAndSize(buff);
                writer.EndRPC();
                RPCProcedure.setTrap(PlayerControl.LocalPlayer.PlayerId, buff);

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
                if (trapperButton.ButtonTitle != null) trapperButton.ButtonTitle.text = $"{Trapper.charges} / {Trapper.maxCharges}";
                return PlayerControl.LocalPlayer.CanMove && Trapper.charges > 0;
            },
            () => { trapperButton.Timer = trapperButton.MaxTimer; },
            Trapper.trapButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("trapperTrapText")
        );

        // Terrorist button
        terroristButton = new CustomButton(
            () =>
            {
                var pos = PlayerControl.LocalPlayer.transform.position;

                if (Terrorist.selfExplosion)
                {
                    terroristButton.HasEffect = false;
                }
                else
                {
                    SoundEffectsManager.play("trapperTrap");
                    terroristButton.HasEffect = true;
                }

                var writer = StartRPC(CustomRPC.PlaceBomb);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(pos);
                writer.EndRPC();
                RPCProcedure.placeBomb(PlayerControl.LocalPlayer, pos);

                terroristButton.Timer = terroristButton.MaxTimer;
            },
            () =>
            {
                return Terrorist.terrorist.IsAlive() && Terrorist.terrorist == PlayerControl.LocalPlayer;
            },
            () =>
            {
                terroristButton.buttonText = Terrorist.selfExplosion ? GetString("TerroristBombText2") : GetString("TerroristBombText1");
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                terroristButton.Timer = terroristButton.MaxTimer;
            },
            Terrorist.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            Terrorist.destructionTime,
            () =>
            {
                terroristButton.Timer = terroristButton.MaxTimer;
                terroristButton.IsEffectActive = false;
                terroristButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            buttonText: Terrorist.selfExplosion ? GetString("TerroristBombText2") : GetString("TerroristBombText1")
        );

        defuseButton = new CustomButton(
            () =>
            {
                defuseButton.EffectDuration = Terrorist.defuseDuration;
                defuseButton.HasEffect = true;
            },
            () =>
            {

                if (Bomb.AllObjects.Count == 0 || Terrorist.selfExplosion)
                {
                    Bomb.TargetBomb = null;
                    return false;
                }
                Bomb.TargetBomb = Bomb.AllObjects.FindWithInRange(PlayerControl.LocalPlayer.GetTruePosition(), 1f);
                return Bomb.TargetBomb != null && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                defuseButton.Timer = 0f;
                defuseButton.IsEffectActive = false;
            },
            Bomb.defuseSprite,
            __instance,
            __instance.AbilityButton,
            hotkey: null,
            true,
            Terrorist.defuseDuration,
            () =>
            {
                var writer = StartRPC(CustomRPC.DefuseBomb);
                writer.Write(Bomb.TargetBomb.Id);
                writer.EndRPC();
                RPCProcedure.defuseBomb(Bomb.TargetBomb.Id);

                defuseButton.Timer = 0f;
            },
            true,
            PositionOffset: new Vector3(-4.5f, 1.5f, 0),
            useGrid: false,
            buttonText: GetString("defuseBombText")
        );

        thiefKillButton = new CustomButton(
            () =>
            {
                var thief = Thief.thief;
                var target = Thief.currentTarget;

                if (!CheckMurderPlayer(thief, target)) return;

                if (!Thief.tiefCanKill(target))
                {
                    // Suicide
                    RpcCustomMurderPlayer(thief, thief, false);
                    Thief.thief.clearAllTasks();
                    return;
                }

                thiefKillButton.Timer = thiefKillButton.MaxTimer;

                RpcCustomMurderPlayer(thief, target, true);
                var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.ThiefStealsRole);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                Thief.StealsRole(target.PlayerId);
            },
            () =>
            {
                return Thief.thief != null && PlayerControl.LocalPlayer == Thief.thief && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                var untargetables = new List<PlayerControl>();
                if (Mini.mini != null && !Mini.isGrownUp) untargetables.Add(Mini.mini);
                Thief.currentTarget = SetTarget(ignoreList: untargetables, inVented: ModOption.NeutCanKillInVent);
                SetPlayerOutline(Thief.currentTarget, Thief.color);

                return Thief.currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { thiefKillButton.Timer = thiefKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        // Yoyo button
        yoyoButton = new CustomButton(
            () =>
            {
                var pos = PlayerControl.LocalPlayer.transform.position;

                if (Yoyo.markedLocation == null)
                {
                    Message($"marked location is null in button press");
                    var writer = StartRPC(CustomRPC.YoyoMarkLocation);
                    writer.Write(pos);
                    writer.EndRPC();
                    RPCProcedure.yoyoMarkLocation(pos);
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
                    var writer = StartRPC(CustomRPC.YoyoBlink);
                    writer.Write(true);
                    writer.Write(pos);
                    writer.EndRPC();
                    RPCProcedure.yoyoBlink(true, pos);
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
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            false,
            Yoyo.blinkDuration,
            () =>
            {
                if (Yoyo.yoyo.IsUsingTransportation)
                {
                    yoyoButton.Timer = 0.5f;
                    yoyoButton.EffectTimer = 0.5f;
                    yoyoButton.IsEffectActive = true;
                    yoyoButton.actionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
                    return;
                }
                else if (Yoyo.yoyo.inVent)
                {
                    __instance.ImpostorVentButton.DoClick();
                }
                // jump back!
                var pos = PlayerControl.LocalPlayer.transform.position;
                var exit = (Vector3)Yoyo.markedLocation;
                if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(exit.y > -7);
                var writer = StartRPC(CustomRPC.YoyoBlink);
                writer.Write(false);
                writer.Write(pos);
                writer.EndRPC();
                RPCProcedure.yoyoBlink(false, pos);
                yoyoButton.Timer = yoyoButton.MaxTimer;
                yoyoButton.IsEffectActive = false;
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
               yoyoAdminTableButton.IsEffectActive = false;
               yoyoAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
           },
           Hacker.getAdminSprite(),
           __instance,
            __instance.AbilityButton,
           secondaryAbilityInput.keyCode,
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
               }
           },
           () =>
           {
               yoyoAdminTableButton.Timer = yoyoAdminTableButton.MaxTimer;
               if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
           },
           buttonText: "AdminMapText".Translate()
       );

        gaolerAdminTableButton = new CustomButton(
            () =>
            {
                if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
                {
                    var __instance = FastDestroyableSingleton<HudManager>.Instance;
                    __instance.InitMap();
                    MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
                }
            },
            () =>
            {
                if (Gaoler.Player == null || PlayerControl.LocalPlayer == null) return false;
                return (Gaoler.Player.IsAlive() && Gaoler.Player == PlayerControl.LocalPlayer) || (Gaoler.Player.Data?.IsDead == true && Gaoler.hasMapPlayer.IsLocalPlayer);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            { },
            Hacker.getAdminSprite(),
            __instance,
             __instance.AbilityButton,
            null,
            buttonText: "AdminMapText".Translate()
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
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                Redemptor.Revelating = false;
                redemptorRevelationButton.Timer = redemptorRevelationButton.MaxTimer;
                redemptorRevelationButton.IsEffectActive = false;
                redemptorRevelationButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Tracker.trackCorpsesButtonSprite,
            __instance,
            __instance.AbilityButton,
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
                writer.Write(true);
                writer.EndRPC();
                Redemptor.RedemptorPrayer(true);
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
                    redemptorReviveButton.showTargetNameOnButton(Redemptor.target);
                }
                return Redemptor.target && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                redemptorPrayerButton.Timer = redemptorPrayerButton.MaxTimer;
                redemptorPrayerButton.IsEffectActive = false;
                redemptorPrayerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Redemptor.reviveButton,
            __instance,
            __instance.AbilityButton,
            modKillInput.keyCode,
            true,
            Redemptor.prayerDuration,
            () =>
            {
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.RedemptorPrayer);
                writer.Write(false);
                writer.EndRPC();
                Redemptor.RedemptorPrayer(false);

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

                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false);

                _ = new LateTask(() =>
                {
                    if (InMeeting) { Message("复活失败", "ReviveTask"); return; }
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.RedemptorRevive);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();
                    Redemptor.RevivePlayer(target.PlayerId);

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

                var db = GetDeadBody(pos, maxDistance);

                if (Redemptor.target != null)
                {
                    redemptorReviveButton.showTargetNameOnButton(Redemptor.target);
                }

                Redemptor.target = PlayerById(db?.ParentId);
                return Redemptor.target && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                redemptorReviveButton.Timer = 10f;
            },
            Redemptor.reviveButton,
            __instance,
            __instance.AbilityButton,
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
                    if (CheckUseAbility(PlayerControl.LocalPlayer, target)) return;

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

                bandLeaderKeyboardistButton.buttonText = (BandLeader.Keyboardist == null ? "bandJoinButton" : "bandKickButton").Translate();

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

                if (bandLeaderKeyboardistButton.ButtonTitle != null) bandLeaderKeyboardistButton.ButtonTitle.text = $"{BandLeader.Keyboardist?.Data?.PlayerName ?? ""}";
                return PlayerControl.LocalPlayer.CanMove && BandLeader.Keyboardist == null
                    ? BandLeader.currentTarget : true;
            },
            () =>
            {
                bandLeaderKeyboardistButton.Sprite = BandLeader.Keyboardist == null ? BandLeader.keyboardButton : BandLeader.keyboardDel;

                bandLeaderKeyboardistButton.Timer = bandLeaderKeyboardistButton.MaxTimer = BandLeader.createCoolDown;
            },
            BandLeader.keyboardButton,
            __instance,
            __instance.AbilityButton,
            null,
            buttonText: "bandJoinButton".Translate()
        );

        bandLeaderBassistButton = new CustomButton(
            () =>
            {
                if (BandLeader.Bassist == null)
                {
                    var target = BandLeader.currentTarget;
                    if (BandLeader.Members.Any(x => x.PlayerId == target?.PlayerId) || target == null) return;
                    if (CheckUseAbility(PlayerControl.LocalPlayer, target)) return;
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

                bandLeaderBassistButton.buttonText = (BandLeader.Keyboardist == null ? "bandJoinButton" : "bandKickButton").Translate();

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
                if (bandLeaderBassistButton.ButtonTitle != null) bandLeaderBassistButton.ButtonTitle.text = $"{BandLeader.Bassist?.Data?.PlayerName ?? ""}";

                return PlayerControl.LocalPlayer.CanMove && BandLeader.Bassist == null
                    ? BandLeader.currentTarget : true;
            },
            () =>
            {
                bandLeaderBassistButton.Sprite = BandLeader.Bassist == null ? BandLeader.bassButton : BandLeader.bassDel;

                bandLeaderBassistButton.Timer = bandLeaderBassistButton.MaxTimer = BandLeader.createCoolDown;
            },
            BandLeader.bassButton,
            __instance,
            __instance.AbilityButton,
            null,
            buttonText: "bandJoinButton".Translate()
        );

        bandLeaderDrummerButton = new CustomButton(
            () =>
            {
                if (BandLeader.Drummer == null)
                {
                    var target = BandLeader.currentTarget;
                    if (BandLeader.Members.Any(x => x.PlayerId == target?.PlayerId) || target == null) return;
                    if (CheckUseAbility(PlayerControl.LocalPlayer, target)) return;
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

                bandLeaderDrummerButton.buttonText = (BandLeader.Keyboardist == null ? "bandJoinButton" : "bandKickButton").Translate();

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
                if (bandLeaderDrummerButton.ButtonTitle != null) bandLeaderDrummerButton.ButtonTitle.text = $"{BandLeader.Drummer?.Data?.PlayerName ?? ""}";

                return PlayerControl.LocalPlayer.CanMove && BandLeader.Drummer == null
                    ? BandLeader.currentTarget : true;
            },
            () =>
            {
                bandLeaderDrummerButton.Sprite = BandLeader.Drummer == null ? BandLeader.drumButton : BandLeader.drumDel;

                bandLeaderDrummerButton.Timer = bandLeaderDrummerButton.MaxTimer = BandLeader.createCoolDown;
            },
            BandLeader.drumButton,
            __instance,
            __instance.AbilityButton,
            null,
            buttonText: "bandJoinButton".Translate()
        );

        bandLeaderKillButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, BandLeader.currentTarget)) return;
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, BandLeader.currentTarget)) return;

                bandLeaderKillButton.Timer = bandLeaderKillButton.MaxTimer;
                BandLeader.currentTarget = null;
            },
            () =>
            {
                return BandLeader.Player.IsAlive() && BandLeader.Player == PlayerControl.LocalPlayer && BandLeader.Formed
                       && BandLeader.WinCondition == BandLeader.WinnerFlags.Impostor;
            },
            () =>
            {
                BandLeader.currentTarget = SetTarget(BandLeader.Members, true);
                SetPlayerOutline(BandLeader.currentTarget, BandLeader.color);

                bandLeaderKillButton.showTargetNameOnButton(BandLeader.currentTarget);

                return PlayerControl.LocalPlayer.CanMove && BandLeader.currentTarget != null;
            },
            () =>
            {
                bandLeaderKillButton.Timer = bandLeaderKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        schrodingersCatKillButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, SchrodingersCat.currentTarget)) return;
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, SchrodingersCat.currentTarget)) return;

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
                    case SchrodingersCat.CatState.Phantom:
                        untargetablePlayers.Add(Phantom.Player);
                        break;
                    case SchrodingersCat.CatState.Arsonist:
                        untargetablePlayers.Add(Arsonist.arsonist);
                        break;
                    case SchrodingersCat.CatState.Pelican:
                        untargetablePlayers.Add(Pelican.Player);
                        break;
                    case SchrodingersCat.CatState.Infected:
                        untargetablePlayers.AddRange(Infected.Player);
                        break;
                }
                untargetablePlayers.RemoveAll(x => x == null);
                var OnlyCrew = SchrodingersCat.State == SchrodingersCat.CatState.Impostor && (Spy.spy == null || !Spy.impostorsCanKillAnyone);
                var target = SetTarget(untargetablePlayers, OnlyCrew, true);

                SchrodingersCat.currentTarget = target;
                SetPlayerOutline(SchrodingersCat.currentTarget, SchrodingersCat.stateColor);

                schrodingersCatKillButton.showTargetNameOnButton(SchrodingersCat.currentTarget);
                return PlayerControl.LocalPlayer.CanMove && SchrodingersCat.currentTarget != null;
            },
            () =>
            {
                schrodingersCatKillButton.Timer = schrodingersCatKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        gunsmithAddBullets = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.SetKillTimer(ModOption.KillCooldown * Mini.Multiplier);
                Gunsmith.remainingChange++;
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.SyncGunsmithChange);
                writer.Write(Gunsmith.remainingChange);
                writer.EndRPC();
                SoundEffectsManager.play("SniperEquip");
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
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            buttonText: GetString("gunsmithAddBullets")
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
                if (gunsmithGetBullets.ButtonTitle != null) gunsmithGetBullets.ButtonTitle.text = $"{Gunsmith.remainingChange} / {Gunsmith.maxChangeCount}";
                return PlayerControl.LocalPlayer.CanMove && PlayerControl.LocalPlayer.killTimer > Gunsmith.setKillCooldown && Gunsmith.remainingChange > 0;
            },
            () => { },
            Gunsmith.GetButton,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("gunsmithGetBullets")
        );

        berserkerKillButton = new CustomButton(
            () =>
            {
                var target = Berserker.currentTarget;
                if (CheckUseAbility(PlayerControl.LocalPlayer, target)) return;
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target)) return;

                berserkerKillButton.EffectDuration = Berserker.GetDuration();
                target = null;
            },
            () =>
            {
                return Berserker.Player.IsAlive() && Berserker.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                Berserker.currentTarget = ImpostorSetTarget();
                berserkerKillButton.showTargetNameOnButton(Berserker.currentTarget);

                if (berserkerKillButton.ButtonTitle != null)
                {
                    berserkerKillButton.ButtonTitle.text = !berserkerKillButton.IsEffectActive
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
                berserkerKillButton.Timer = berserkerKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
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
                if (CheckUseAbility(PlayerControl.LocalPlayer, target)) return;
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target)) return;
                target = null;
            },
            () =>
            {
                berserkerKillButton.Timer = berserkerKillButton.MaxTimer;
                berserkerKillButton.IsEffectActive = false;
                Berserker.Timer = 0f;
            },
            buttonText: GetString("killButtonText")
        );

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
                Poltergeist.targetBody = GetDeadBody(PlayerControl.LocalPlayer.GetTruePosition());
                return Poltergeist.targetBody && PlayerControl.LocalPlayer.CanMove;
            },
            () => { poltergeistButton.Timer = poltergeistButton.MaxTimer; },
            Poltergeist.ButtonSprite,
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            buttonText: GetString("poltergeistButton")
        );

        InfectedKillButton = new CustomButton(
            () =>
            {
                var target = Infected.currentTarget;
                if (!Infected.KillPlayer(PlayerControl.LocalPlayer, target)) return;

                InfectedKillButton.Timer = InfectedKillButton.MaxTimer;
                Infected.currentTarget = null;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsAlive() && Infected.Player.Any(x => x == PlayerControl.LocalPlayer);
            },
            () =>
            {
                var ignoreList = new List<PlayerControl>(Infected.Player);
                if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Infected) ignoreList.Add(SchrodingersCat.Player);
                Infected.currentTarget = SetTarget(ignoreList: ignoreList);
                SetPlayerOutline(Infected.currentTarget, Infected.color);
                InfectedKillButton.showTargetNameOnButton(Infected.currentTarget);

                return PlayerControl.LocalPlayer.CanMove && Infected.currentTarget != null;
            },
            () => { InfectedKillButton.Timer = InfectedKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        );

        jailorButton = new CustomButton(
            () =>
            {
                var target = Jailor.currentTarget;
                if (target == null) return;
                if (CheckUseAbility(PlayerControl.LocalPlayer, target)) return;

                var writer = StartRPC(CustomRPC.JailorJail);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                Jailor.JailPlayer(PlayerControl.LocalPlayer, target);

                SoundEffectsManager.play("deputyHandcuff");
                Jailor.currentTarget = null;
            },
            () =>
            {
                return Jailor.Player.IsAlive() && Jailor.Player == PlayerControl.LocalPlayer && Jailor.usesCount > 0;
            },
            () =>
            {
                jailorButton.UsesCount = Jailor.usesCount;
                Jailor.currentTarget = SetTarget();
                SetPlayerOutline(Jailor.currentTarget, Jailor.color);
                jailorButton.showTargetNameOnButton(Jailor.currentTarget, Jailor.Jailed?.Data?.PlayerName);

                return PlayerControl.LocalPlayer.CanMove && Jailor.currentTarget != null;
            },
            () =>
            {
                // Reset the jailed player
                Jailor.Jailed = null;

                jailorButton.Timer = jailorButton.MaxTimer;
            },
            Jailor.buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            1f,
            () => { },
            buttonText: GetString("jailButtonText")
        );

        marionettePlaceButton = new(
            () =>
            {
                var writer = StartRPC(CustomRPC.PlaceDecoy);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(PlayerControl.LocalPlayer.transform.position);
                writer.EndRPC();
                RPCProcedure.PlaceDecoy(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position);
                marionettePlaceButton.Timer = marionettePlaceButton.MaxTimer;

                Marionette.marionetteMode = 0;

                Marionette.SetMarionetteMode(0);
                marionetteButton.Timer = marionetteButton.MaxTimer;
            },
            () =>
            {
                return Marionette.Player.IsAlive() && PlayerControl.LocalPlayer == Marionette.Player && Marionette.decoy == null;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove || HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer;
            },
            () =>
            {
                marionettePlaceButton.Timer = marionettePlaceButton.MaxTimer = Marionette.PlaceCooldown;
            },
            Marionette.decoyButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("decoyButtonText")
        );

        marionetteButton = new(
            () =>
            {
                if (Marionette.marionetteMode == 0)
                {
                    if (HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer) HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);

                    var writer = StartRPC(CustomRPC.DecoySwap);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(Marionette.decoy.Id);
                    writer.Write(PlayerControl.LocalPlayer.transform.position);
                    writer.Write(Marionette.decoy.GameObject.transform.position);
                    writer.EndRPC();
                    RPCProcedure.DecoySwap(PlayerControl.LocalPlayer, Marionette.decoy.Id, PlayerControl.LocalPlayer.transform.position, Marionette.decoy.GameObject.transform.position);
                }
                else
                {

                    var writer = StartRPC(CustomRPC.DecoyDestroy);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(Marionette.decoy.Id);
                    writer.EndRPC();
                    RPCProcedure.DecoyDestroy(Marionette.Player, Marionette.decoy.Id);
                    marionettePlaceButton.Timer = marionettePlaceButton.MaxTimer;
                }
                marionetteButton.Timer = marionetteButton.MaxTimer;
            },
            () =>
            {
                return Marionette.Player.IsAlive() && PlayerControl.LocalPlayer == Marionette.Player && Marionette.decoy != null; ;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove || HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer; ;
            },
            () =>
            {
                marionetteButton.Timer = marionetteButton.MaxTimer;
                if (!Decoy.DecoyPermanent) Marionette.SetMarionetteMode(0);
            },
            Marionette.decoyButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("swapButtonText")
        );
        marionetteButton.SetAidAction(changeAbilityInput.keyCode, true, () => { Marionette.SetMarionetteMode((Marionette.marionetteMode + 1) % 2); });

        marionetteCameraButton = new(
            () =>
            {
                if (HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer)
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                    if (!Marionette.MonitoringCanMove) PlayerControl.LocalPlayer.moveable = true;
                }
                else
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(Marionette.decoy.Behaviour);
                    PlayerControl.LocalPlayer.NetTransform.Halt();
                    if (!Marionette.MonitoringCanMove) PlayerControl.LocalPlayer.moveable = false;
                }
            },
            () =>
            {
                return Marionette.Player.IsAlive() && PlayerControl.LocalPlayer == Marionette.Player && Marionette.decoy != null;
            },
            () =>
            {
                if (marionetteCameraButton.ButtonTitle != null && Marionette.decoy != null)
                {
                    marionetteCameraButton.ButtonTitle.text = Decoy.DecoyPermanent
                    ? $"持续存在{(Decoy.ResetPlaceAfterMeeting ? "|会议重置" : "")}"
                    : (Decoy.DecoyDuration - Marionette.decoy.elapsedTime).ToString("00");
                }

                return PlayerControl.LocalPlayer.CanMove || HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer;
            },
            () =>
            {
                marionetteCameraButton.Timer = 0f;
            },
            Marionette.monitorButtonSprite,
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            buttonText: GetString("monitorButtonText")
        );

        avengerKillButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Avenger.currentTarget)) return;
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, Avenger.currentTarget)) return;

                avengerKillButton.Timer = avengerKillButton.MaxTimer;
                Avenger.currentTarget = null;
            },
            () =>
            {
                return Avenger.Player.IsAlive() && Avenger.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                Avenger.currentTarget = Avenger.CanFreeKill ? SetTarget(inVented: true) : SetTarget(targetPlayers: [Avenger.Target], inVented: true);
                SetPlayerOutline(Avenger.currentTarget, Avenger.color);

                avengerKillButton.showTargetNameOnButton(Avenger.currentTarget);

                return PlayerControl.LocalPlayer.CanMove && Avenger.currentTarget != null;
            },
            () =>
            {
                avengerKillButton.Timer = avengerKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            modKillInput.keyCode,
            buttonText: GetString("AvengeButtonText")
        );

        clogPlaceGhost = new CustomButton(
            () =>
            {
                var writer = StartRPC(CustomRPC.PlaceClogGhost);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(PlayerControl.LocalPlayer.transform.position);
                writer.EndRPC();
                RPCProcedure.PlaceClogGhost(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position);
                Clog.UsedNum++;
                Clog.IsUsed = true;
            },
            () => { return Clog.Player != null && PlayerControl.LocalPlayer == Clog.Player && PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                clogPlaceGhost.UsesCount = Clog.CanUseNum - Clog.UsedNum;
                return (!Clog.IsUsed || !Clog.OnlyUsedOnce) && (Clog.CanUseNum - Clog.UsedNum > 0) && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                Clog.IsUsed = false;
                clogPlaceGhost.Timer = clogPlaceGhost.MaxTimer;
            },
            Clog.ButtonSprite,
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            true,
            Clog.GhostDuration,
            () =>
            {
                clogPlaceGhost.IsEffectActive = false;
                clogPlaceGhost.Timer = clogPlaceGhost.MaxTimer;
            },
            buttonText: GetString("clogPlaceGhost")
        );

        oracleButton = new CustomButton(
            () =>
            {
                if (CheckUseAbility(PlayerControl.LocalPlayer, Oracle.CurrentTarget)) return;

                var writer = StartRPC(CustomRPC.SetConfesser);
                writer.Write(Oracle.CurrentTarget.PlayerId);
                writer.Write((int)Oracle.CRoleType.None);
                writer.EndRPC();
                Oracle.Confesser = Oracle.CurrentTarget;

                oracleButton.Timer = oracleButton.MaxTimer;
            },
            () =>
            {
                return Oracle.Player.IsAlive() && Oracle.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                Oracle.CurrentTarget = SetTarget();
                SetPlayerOutline(Oracle.CurrentTarget, Oracle.color);

                oracleButton.showTargetNameOnButton(Oracle.CurrentTarget, Oracle.Confesser?.Data?.PlayerName ?? "");

                return PlayerControl.LocalPlayer.CanMove && Oracle.CurrentTarget != null;
            },
            () =>
            {
                oracleButton.Timer = oracleButton.MaxTimer;
            },
            Oracle.ConfessSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("oracleButton")
        );

        dreamcatcherButton = new(
            () =>
            {
                if (CheckUseAbility(Dreamcatcher.Player, Dreamcatcher.CurrentTarget)) return;
                var killed = false;
                if (Dreamcatcher.LastDreamed == Dreamcatcher.CurrentTarget)
                {
                    RpcCustomMurderPlayer(PlayerControl.LocalPlayer, Dreamcatcher.CurrentTarget, true, false, CustomDeathReason.Dreamcrush);
                    killed = true;
                }

                var write = StartRPC(CustomRPC.DreamcatcherSetDreamer);
                write.Write(Dreamcatcher.CurrentTarget.PlayerId);
                write.Write(killed);
                write.EndRPC();
                Dreamcatcher.SetDreamer(Dreamcatcher.CurrentTarget, killed);
                dreamcatcherButton.Timer = dreamcatcherButton.MaxTimer;
            },
            () =>
            {
                return Dreamcatcher.Player.IsAlive() && Dreamcatcher.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                if (Dreamcatcher.Dreamed != null) return false;
                Dreamcatcher.CurrentTarget = SetTarget();
                dreamcatcherButton.Sprite = Dreamcatcher.CurrentTarget != null && Dreamcatcher.CurrentTarget == Dreamcatcher.LastDreamed
                    ? Dreamcatcher.killButtonSprite
                    : Dreamcatcher.dreamButtonSprite;
                dreamcatcherButton.showTargetNameOnButton(Dreamcatcher.CurrentTarget, Dreamcatcher.Dreamed?.Data?.PlayerName ?? "");
                return PlayerControl.LocalPlayer.CanMove && Dreamcatcher.CurrentTarget != null;
            },
            () =>
            {
                Dreamcatcher.LastDreamed = Dreamcatcher.Dreamed ?? null;
                Dreamcatcher.Dreamed = null;
                Dreamcatcher.CurrentTarget = null;
                Dreamcatcher.ShieldUsed = false;

                dreamcatcherButton.Sprite = Dreamcatcher.dreamButtonSprite;
                dreamcatcherButton.Timer = dreamcatcherButton.MaxTimer;
            },
            Dreamcatcher.dreamButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: "dreamButtonText".Translate()
        );


        soulSightButton = new(
            () =>
            {
                var writer = StartRPC(CustomRPC.SoulSightSuicide);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.EndRPC();
                SoulSight.Suicide(PlayerControl.LocalPlayer);

                soulSightButton.EffectDuration = 10f;
            },
            () =>
            {
                return SoulSight.Player != null && SoulSight.Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                return SoulSight.CanRevive;
            },
            () =>
            {
                soulSightButton.Timer = soulSightButton.MaxTimer;
            },
            SoulSight.ButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            true,
            SoulSight.RespawnTimer,
            () => { return true; },
            () => { },
            () =>
            {
                if (!InMeeting && SoulSight.Player != null && SoulSight.Player.Data.IsDead && SoulSight.Reviveing)
                {
                    var writer = StartRPC(CustomRPC.SoulSightRevive);
                    writer.Write(true);
                    writer.EndRPC();
                    PlayerControl.LocalPlayer.ModRevive(true, true);
                    SoulSight.Reviveing = false;
                }
                soulSightButton.Timer = soulSightButton.MaxTimer;
            },
            buttonText: "自刎"
        );

        // Set the default (or settings from the previous game) timers / durations when spawning the buttons
        initialized = true;
        setCustomButtonCooldowns();
        deputyHandcuffedButtons = new Dictionary<byte, List<CustomButton>>();
    }
}