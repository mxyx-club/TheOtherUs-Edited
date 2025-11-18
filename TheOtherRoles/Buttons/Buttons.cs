using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using static TheOtherRoles.Modules.ModInputManager;

namespace TheOtherRoles.Buttons;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
internal static class HudManagerStartPatch
{
    private static bool initialized;
    public static Dictionary<byte, List<CustomButton>> deputyHandcuffedButtons;

    public static CustomButton zoomOutButton;
    public static CustomButton roleSummaryButton;
    public static CustomButton gameModeButton;
    public static CustomButton ghostEngineerButton;
    public static CustomButton specterRememberButton;
    public static CustomButton garlicButton;
    public static CustomButton poltergeistButton;
    public static CustomButton usePortalButton; // 延期适配
    //public static CustomButton bomberGiveButton; // 延期适配
    public static CustomButton defuseButton;

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

        defuseButton.MaxTimer = defuseButton.Timer = 0f;
        defuseButton.EffectDuration = Terrorist.defuseDuration;

        ghostEngineerButton.Timer = ghostEngineerButton.MaxTimer = 0f;
        specterRememberButton.MaxTimer = 15f;
        garlicButton.MaxTimer = 0f;
        poltergeistButton.MaxTimer = Poltergeist.cooldown;
        zoomOutButton.MaxTimer = zoomOutButton.Timer = 0f;
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

        // (共用星门传送按钮) 延期适配
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
                    var writer = StartRPC((byte)CustomRPC.UsePortal);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(portalMakerSoloTeleport ? (byte)1 : (byte)0);
                    writer.EndRPC();
                }

                RPCProcedure.usePortal(PlayerControl.LocalPlayer.PlayerId, portalMakerSoloTeleport ? (byte)1 : (byte)0);
                usePortalButton.Timer = usePortalButton.MaxTimer;
                Portalmaker.portalmakerMoveToPortalButton.Timer = usePortalButton.MaxTimer;
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
                if (PlayerControl.LocalPlayer.Is(RoleId.Portalmaker) && Portal.bothPlacedAndEnabled)
                    usePortalButton.ButtonTitle.text = Portal.locationNearEntry(PlayerControl.LocalPlayer.transform.position) ||
                        !Portalmaker.canPortalFromAnywhere
                            ? ""
                            : "1. " + Portal.firstPortal.room;
                return Portal.bothPlacedAndEnabled;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove &&
                       (Portal.locationNearEntry(PlayerControl.LocalPlayer.transform.position) ||
                       (Portalmaker.canPortalFromAnywhere && PlayerControl.LocalPlayer.Is(RoleId.Portalmaker)))
                       && !Portal.isTeleporting;
            },
            () => { usePortalButton.Timer = usePortalButton.MaxTimer; },
            Portalmaker.usePortalButtonSprite,
            __instance,
            __instance.AbilityButton,
            null,
            true,
            buttonText: GetString("usePortalText")
        )
        {
            MaxTimer = Portalmaker.usePortalCooldown
        };

        // (传递炸弹按钮) 延期适配
        /*bomberGiveButton = new CustomButton(
            () =>
            {
                // On Use
                if (!Bomber.canGiveToBomber && Bomber.currentBombTarget == Bomber.Player)
                {
                    var killWriter = StartRPC(CustomRPC.UncheckedMurderPlayer);
                    killWriter.Write(Bomber.Player.Data.PlayerId);
                    killWriter.Write(Bomber.hasBombPlayer.Data.PlayerId);
                    killWriter.Write(false);
                    killWriter.EndRPC();
                    RPCProcedure.uncheckedMurderPlayer(Bomber.Player.Data.PlayerId, Bomber.hasBombPlayer.PlayerId, false);

                    var clearWriter = StartRPC(CustomRPC.GiveBomb);
                    clearWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                    clearWriter.Write(byte.MaxValue);
                    clearWriter.Write(false);
                    clearWriter.EndRPC();
                    Bomber.RpcGiveBomb(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
                    return;
                }

                if (checkAndDoVetKill(Bomber.currentBombTarget)) return;
                if (Bomber.hotPotatoMode)
                {
                    var bombWriter = StartRPC(CustomRPC.GiveBomb);
                    bombWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                    bombWriter.Write(Bomber.currentBombTarget.PlayerId);
                    bombWriter.Write(true);
                    bombWriter.EndRPC();
                    Bomber.RpcGiveBomb(PlayerControl.LocalPlayer.PlayerId, Bomber.currentBombTarget.PlayerId, true);
                }
                else
                {
                    if (checkMurderAttemptAndKill(Bomber.hasBombPlayer, Bomber.currentBombTarget) == MurderAttemptResult.SuppressKill) return;
                    var bombWriter = StartRPC(CustomRPC.GiveBomb);
                    bombWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                    bombWriter.Write(byte.MaxValue);
                    bombWriter.Write(false);
                    bombWriter.EndRPC();
                    Bomber.RpcGiveBomb(PlayerControl.LocalPlayer.PlayerId, Bomber.currentBombTarget.PlayerId, true);
                }
            },
            () =>
            {
                // Can See
                return PlayerControl.LocalPlayer.IsAlive() && Bomber.hasBombPlayer.Values.Contains(PlayerControl.LocalPlayer) && Bomber.bombActive;
            },
            () =>
            {
                // Can Click
                Bomber.currentBombTarget = SetTarget();
                if (Bomber.hasBombPlayer == null) SetPlayerOutline(Bomber.currentBombTarget, Bomber.color);
                return Bomber.currentBombTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                // On Meeting End
            },
            Bomber.buttonSprite,
            //          0, -0.06f, 0
            new Vector3(-4.5f, 1.5f, 0),
            __instance,
            hotkey: null,
            buttonText: "giveBombText".Translate()
        )
        {
            MaxTimer = bomberGiveButton.Timer = 0f,
            EffectDuration = Bomber.bombDelay + Bomber.bombTimer,
        };*/

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
                defuseButton.isEffectActive = false;
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
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            buttonText: GetString("RepairText")
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
            __instance,
            __instance.AbilityButton,
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
            __instance,
            __instance.AbilityButton,
            secondaryAbilityInput.keyCode,
            buttonText: GetString("poltergeistButton")
        );

        // Set the default (or settings from the previous game) timers / durations when spawning the buttons
        initialized = true;
        setCustomButtonCooldowns();
        deputyHandcuffedButtons = new Dictionary<byte, List<CustomButton>>();
    }
}