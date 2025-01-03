#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using AmongUs.GameOptions;
using Hazel;
using InnerNet;
using PowerTools;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TheOtherRoles.Buttons;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Objects;
using TheOtherRoles.Objects.Map;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.Buttons.HudManagerStartPatch;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.Options.ModOption;
using Object = UnityEngine.Object;

namespace TheOtherRoles;

public enum RoleId
{
    Default,

    Impostor,
    Morphling,
    WolfLord,
    Bomber,
    Poucher,
    Butcher,
    Mimic,
    Camouflager,
    Miner,
    Eraser,
    Vampire,
    Undertaker,
    Escapist,
    Warlock,
    Trickster,
    BountyHunter,
    Cleaner,
    Terrorist,
    Blackmailer,
    Witch,
    Ninja,
    Yoyo,
    EvilTrapper,
    Gambler,
    Grenadier,

    Survivor,
    Amnisiac,
    Jester,
    Vulture,
    Lawyer,
    Executioner,
    Pursuer,
    PartTimer,
    Witness,
    Doomsayer,
    Arsonist,
    Jackal,
    Sidekick,
    Pavlovsowner,
    Pavlovsdogs,
    Werewolf,
    Swooper,
    Juggernaut,
    Akujo,
    Thief,

    Crewmate,
    Vigilante,
    Mayor,
    Prosecutor,
    Portalmaker,
    Engineer,
    Sheriff,
    Deputy,
    BodyGuard,
    Jumper,
    Detective,
    TimeMaster,
    Veteran,
    Medic,
    Swapper,
    Seer,
    Hacker,
    Tracker,
    Snitch,
    Prophet,
    InfoSleuth,
    Spy,
    SecurityGuard,
    Medium,
    Trapper,
    Balancer,

    // Modifier ---
    Lover,
    Assassin,
    Disperser,
    PoucherModifier,
    Vortox,
    Specoality,
    LastImpostor,
    Bloody,
    AntiTeleport,
    Tiebreaker,
    Bait,
    Aftermath,
    Flash,
    Torch,
    Sunglasses,
    Multitasker,
    Mini,
    Giant,
    Vip,
    Indomitable,
    Slueth,
    Cursed,
    Invert,
    Blind,
    Watcher,
    Radar,
    Tunneler,
    ButtonBarry,
    Chameleon,
    Shifter,

    GhostEngineer = 200,
    Specter,
}

public enum CustomRPC
{
    // Main Controls
    ResetVaribles = 100,
    ShareOptions,
    WorkaroundSetRoles,
    SetRole,
    SetModifier,
    SetGhostRole,
    VersionHandshake,
    UseUncheckedVent,
    UncheckedMurderPlayer,
    UncheckedCmdReportDeadBody,
    UncheckedExilePlayer,
    DynamicMapOption,
    SetGameStarting,
    StopStart,
    ShareGameMode = 120,

    // Role functionality
    FixLights = 121,
    FixSubmergedOxygen,
    CleanBody,
    DissectionBody,
    Mine,
    ShowIndomitableFlash,
    DragBody,
    DropBody,
    MedicSetShielded,
    ShowBodyGuardFlash,
    ShieldedMurderAttempt,
    TimeMasterShield,
    TimeMasterRewindTime,
    TurnToImpostor,
    BodyGuardGuardPlayer,
    VeteranAlert,
    VeteranKill,
    ShifterShift,
    SwapperSwap,
    MorphlingMorph,
    CamouflagerCamouflage,
    //DoomsayerMeeting,
    AkujoSetHonmei,
    AkujoSetKeep,
    AkujoSuicide,
    MayorMeeting,
    BarryMeeting,
    ProphetExamine,
    ImpostorPromotesToLastImpostor,
    //CamoComms,
    TrackerUsedTracker,
    VampireSetBitten,
    PlaceGarlic,
    GiveBomb,
    DeputyUsedHandcuffs,
    DeputyPromotes,
    JackalCreatesSidekick,
    PavlovsCreateDog,
    SidekickPromotes,
    ErasePlayerRoles,
    ClearGhostRoles,
    SetFutureErased,
    SetFutureReveal,
    SetFutureShifted,
    Disperse,
    SetFutureShielded,
    SetFutureSpelled,
    PlaceNinjaTrace,
    PlacePortal,
    AmnisiacTakeRole,
    SpecterTakeRole,
    MimicMimicRole,
    UsePortal,
    PlaceJackInTheBox,
    LightsOut,
    PlaceCamera,
    SealVent,
    PartTimerSet,
    GuesserShoot,
    LawyerSetTarget,
    ExecutionerSetTarget,
    ExecutionerPromotesRole,
    LawyerPromotesToPursuer,
    BlackmailPlayer,
    UseCameraTime,
    UseVitalsTime,
    UnblackmailPlayer,
    PursuerSetBlanked,
    Bloody,
    SetFirstKill,
    SetMeetingChatOverlay,
    SetInvisibleGen,
    SetSwoop,
    SetJackalSwoop,
    JackalCanSwooper,
    InfoSleuthTarget,
    InfoSleuthNoTarget,
    GrenadierFlash,
    WitnessReport,
    WitnessSetTarget,
    WolfLordkilled,

    TrapperKill,
    PlaceTrap,
    ClearTrap,
    ActivateTrap,
    DisableTrap,
    TrapperMeetingFlag,
    Prosecute,
    MayorRevealed,
    SurvivorVestActive,

    //SetSwooper,
    SetInvisible,
    ThiefStealsRole,
    SetTrap,
    TriggerTrap,
    PlaceBomb,
    DefuseBomb,
    YoyoMarkLocation,
    YoyoBlink,
    BalancerBalance,

    // Gamemode
    SetGuesserGm,
    SetRevealed,
    HostEndGame,
    HostKill,
    HostRevive,

    // Other functionality
    ShareGhostInfo,
}

public static class RPCProcedure
{
    public enum GhostInfoTypes
    {
        HandcuffNoticed,
        HandcuffOver,
        ArsonistDouse,
        BountyTarget,
        NinjaMarked,
        WarlockTarget,
        GhostChat,
        BlankUsed,
        VampireTimer,
        DeathReasonAndKiller
    }

    // Main Controls

    public static void resetVariables()
    {
        reloadPluginOptions();
        clearAndReloadMapOptions();
        clearAndReloadRoles();
        MapData.Clear();
        Garlic.clearGarlics();
        JackInTheBox.clearJackInTheBoxes();
        NinjaTrace.clearTraces();
        AdditionalVents.clearAndReload();
        Portal.clearPortals();
        Bloodytrail.resetSprites();
        Trap.clearTraps();
        Silhouette.clearSilhouettes();
        ElectricPatch.Reset();
        Clear();
        setCustomButtonCooldowns();
        toggleZoom(true);
        GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 0;
        SurveillanceMinigamePatch.nightVisionOverlays = null;
    }

    public static void HandleShareOptions(byte numberOfOptions, MessageReader reader)
    {
        try
        {
            for (var i = 0; i < numberOfOptions; i++)
            {
                var optionId = reader.ReadPackedUInt32();
                var selection = reader.ReadPackedUInt32();
                var option = CustomOption.options.First(option => option.id == (int)optionId);
                option.updateSelection((int)selection, i == numberOfOptions - 1);
            }
        }
        catch (Exception e)
        {
            Error("Error while deserializing options: " + e.Message);
        }
    }

    public static void shareGameMode(byte gm)
    {
        gameMode = (CustomGamemodes)gm;
        try
        {
            LobbyViewSettingsPatch.currentButtons?.ForEach(x => x.gameObject?.Destroy());
            LobbyViewSettingsPatch.currentButtons?.Clear();
            LobbyViewSettingsPatch.currentButtonTypes?.Clear();
        }
        catch { }
    }

    public static void stopStart(byte playerId)
    {
        if (AmongUsClient.Instance.AmHost && CustomOptionHolder.anyPlayerCanStopStart.GetBool())
        {
            GameStartManager.Instance.ResetStartState();
            PlayerControl.LocalPlayer.RpcSendChat($"{playerById(playerId).Data.PlayerName} 阻止游戏开始");
        }
    }

    public static void workaroundSetRoles(byte numberOfRoles, MessageReader reader)
    {
        for (var i = 0; i < numberOfRoles; i++)
        {
            var playerId = (byte)reader.ReadPackedUInt32();
            var roleId = (byte)reader.ReadPackedUInt32();
            try
            {
                setRole(roleId, playerId);
            }
            catch (Exception e)
            {
                Error("Error while deserializing roles: " + e.Message);
            }
        }
    }

    public static void setRole(byte roleId, byte playerId)
    {
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
        {
            if (player.PlayerId == playerId)
            {
                switch ((RoleId)roleId)
                {
                    case RoleId.Jester:
                        Jester.jester = player;
                        break;
                    case RoleId.Werewolf:
                        Werewolf.werewolf = player;
                        break;
                    case RoleId.WolfLord:
                        WolfLord.Player = player;
                        break;
                    case RoleId.Blackmailer:
                        Blackmailer.blackmailer = player;
                        break;
                    case RoleId.Miner:
                        Miner.miner = player;
                        break;
                    case RoleId.Poucher:
                        Poucher.poucher = player;
                        break;
                    case RoleId.Mayor:
                        Mayor.mayor = player;
                        break;
                    case RoleId.Prosecutor:
                        Prosecutor.prosecutor = player;
                        break;
                    case RoleId.Portalmaker:
                        Portalmaker.portalmaker = player;
                        break;
                    case RoleId.Engineer:
                        Engineer.engineer = player;
                        break;
                    case RoleId.Sheriff:
                        Sheriff.sheriff = player;
                        break;
                    case RoleId.BodyGuard:
                        BodyGuard.bodyguard = player;
                        break;
                    case RoleId.Deputy:
                        Deputy.deputy = player;
                        break;
                    case RoleId.Detective:
                        Detective.detective = player;
                        break;
                    case RoleId.InfoSleuth:
                        InfoSleuth.infoSleuth = player;
                        break;
                    case RoleId.TimeMaster:
                        TimeMaster.timeMaster = player;
                        break;
                    case RoleId.Amnisiac:
                        Amnisiac.Player.Add(player);
                        break;
                    case RoleId.PartTimer:
                        PartTimer.partTimer = player;
                        break;
                    case RoleId.Grenadier:
                        Grenadier.grenadier = player;
                        break;
                    case RoleId.Veteran:
                        Veteran.veteran = player;
                        break;
                    case RoleId.Medic:
                        Medic.medic = player;
                        break;
                    case RoleId.Shifter:
                        Shifter.shifter = player;
                        break;
                    case RoleId.Swapper:
                        Swapper.swapper = player;
                        break;
                    case RoleId.Seer:
                        Seer.seer = player;
                        break;
                    case RoleId.Morphling:
                        Morphling.morphling = player;
                        break;
                    case RoleId.Bomber:
                        Bomber.bomber = player;
                        break;
                    case RoleId.Camouflager:
                        Camouflager.camouflager = player;
                        break;
                    case RoleId.Butcher:
                        Butcher.butcher = player;
                        break;
                    case RoleId.Witness:
                        Witness.Player = player;
                        break;
                    case RoleId.Hacker:
                        Hacker.hacker = player;
                        break;
                    case RoleId.Tracker:
                        Tracker.tracker = player;
                        break;
                    case RoleId.Vampire:
                        Vampire.vampire = player;
                        break;
                    case RoleId.Snitch:
                        Snitch.snitch = player;
                        break;
                    case RoleId.Jackal:
                        Jackal.jackal.Add(player);
                        break;
                    case RoleId.Sidekick:
                        Jackal.sidekick = player;
                        break;
                    case RoleId.Pavlovsowner:
                        Pavlovsdogs.pavlovsowner = player;
                        break;
                    case RoleId.Pavlovsdogs:
                        Pavlovsdogs.pavlovsdogs.Add(player);
                        break;
                    case RoleId.Swooper:
                        Swooper.swooper = player;
                        break;
                    case RoleId.Eraser:
                        Eraser.eraser = player;
                        break;
                    case RoleId.Spy:
                        Spy.spy = player;
                        break;
                    case RoleId.Trickster:
                        Trickster.trickster = player;
                        break;
                    case RoleId.Cleaner:
                        Cleaner.cleaner = player;
                        break;
                    case RoleId.Undertaker:
                        Undertaker.undertaker = player;
                        break;
                    case RoleId.Mimic:
                        Mimic.mimic = player;
                        break;
                    case RoleId.Warlock:
                        Warlock.warlock = player;
                        break;
                    case RoleId.SecurityGuard:
                        SecurityGuard.securityGuard = player;
                        break;
                    case RoleId.Arsonist:
                        Arsonist.arsonist = player;
                        break;
                    case RoleId.Vigilante:
                        Vigilante.vigilante = player;
                        break;
                    case RoleId.BountyHunter:
                        BountyHunter.bountyHunter = player;
                        break;
                    case RoleId.Vulture:
                        Vulture.vulture = player;
                        break;
                    case RoleId.Medium:
                        Medium.medium = player;
                        break;
                    case RoleId.Trapper:
                        Trapper.trapper = player;
                        break;
                    case RoleId.Lawyer:
                        Lawyer.lawyer = player;
                        break;
                    case RoleId.Pursuer:
                        Pursuer.Player.Add(player);
                        break;
                    case RoleId.Survivor:
                        Survivor.Player.Add(player);
                        break;
                    case RoleId.Executioner:
                        Executioner.executioner = player;
                        break;
                    case RoleId.Witch:
                        Witch.witch = player;
                        break;
                    case RoleId.Ninja:
                        Ninja.ninja = player;
                        break;
                    case RoleId.Jumper:
                        Jumper.jumper = player;
                        break;
                    case RoleId.Balancer:
                        Balancer.balancer = player;
                        break;
                    case RoleId.Escapist:
                        Escapist.escapist = player;
                        break;
                    case RoleId.Thief:
                        Thief.thief = player;
                        break;
                    case RoleId.Terrorist:
                        Terrorist.terrorist = player;
                        break;
                    case RoleId.Juggernaut:
                        Juggernaut.juggernaut = player;
                        break;
                    case RoleId.Doomsayer:
                        Doomsayer.doomsayer = player;
                        break;
                    case RoleId.Akujo:
                        Akujo.akujo = player;
                        break;
                    case RoleId.Prophet:
                        Prophet.prophet = player;
                        break;
                    case RoleId.Yoyo:
                        Yoyo.yoyo = player;
                        break;
                    case RoleId.EvilTrapper:
                        EvilTrapper.evilTrapper = player;
                        break;
                    case RoleId.Gambler:
                        Gambler.gambler = player;
                        break;
                }
            }
            if (AmongUsClient.Instance.AmHost && Helpers.roleCanUseVents(player) && !player.Data.Role.IsImpostor)
            {
                player.RpcSetRole(RoleTypes.Engineer);
                player.CoSetRole(RoleTypes.Engineer, true);
            }
        }
    }

    public static void setModifier(byte modifierId, byte playerId, byte flag)
    {
        var player = playerById(playerId);
        switch ((RoleId)modifierId)
        {
            case RoleId.Assassin:
                Assassin.assassin.Add(player);
                break;
            case RoleId.Bait:
                Bait.bait.Add(player);
                break;
            case RoleId.Aftermath:
                Aftermath.aftermath = player;
                break;
            case RoleId.Lover:
                if (flag == 0) Lovers.lover1 = player;
                else Lovers.lover2 = player;
                break;
            case RoleId.Bloody:
                Bloody.bloody.Add(player);
                break;
            case RoleId.AntiTeleport:
                AntiTeleport.antiTeleport.Add(player);
                break;
            case RoleId.Tiebreaker:
                Tiebreaker.tiebreaker = player;
                break;
            case RoleId.Sunglasses:
                Sunglasses.sunglasses.Add(player);
                break;
            case RoleId.Torch:
                Torch.torch.Add(player);
                break;
            case RoleId.Flash:
                Flash.flash.Add(player);
                break;
            case RoleId.Slueth:
                Slueth.slueth = player;
                break;
            case RoleId.PoucherModifier:
                Poucher.poucher = player;
                break;
            case RoleId.Cursed:
                Cursed.cursed = player;
                break;
            case RoleId.Blind:
                Blind.blind = player;
                break;
            case RoleId.Watcher:
                Watcher.watcher = player;
                break;
            case RoleId.Radar:
                Radar.radar = player;
                break;
            case RoleId.Vortox:
                Vortox.Player = player;
                break;
            case RoleId.Tunneler:
                Tunneler.tunneler = player;
                break;
            case RoleId.ButtonBarry:
                ButtonBarry.buttonBarry = player;
                break;
            case RoleId.Multitasker:
                Multitasker.multitasker.Add(player);
                break;
            case RoleId.Disperser:
                Disperser.disperser = player;
                break;
            case RoleId.Specoality:
                Specoality.specoality = player;
                break;
            case RoleId.Mini:
                Mini.mini = player;
                break;
            case RoleId.Giant:
                Giant.giant = player;
                break;
            case RoleId.Vip:
                Vip.vip.Add(player);
                break;
            case RoleId.Invert:
                Invert.invert.Add(player);
                break;
            case RoleId.Indomitable:
                Indomitable.indomitable = player;
                break;
            case RoleId.Chameleon:
                Chameleon.chameleon.Add(player);
                break;
            case RoleId.Shifter:
                Shifter.shifter = player;
                break;
        }
    }

    public static void setGhostRole(byte playerId, byte roleId)
    {
        var player = playerById(playerId);
        switch ((RoleId)roleId)
        {
            case RoleId.GhostEngineer:
                GhostEngineer.Player = player;
                break;
            case RoleId.Specter:
                Specter.Player = player;
                break;
        }
    }

    public static void versionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
    {
        var ver = revision < 0 ? new Version(major, minor, build) : new Version(major, minor, build, revision);
        GameStartManagerPatch.playerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
    }

    public static void useUncheckedVent(int ventId, byte playerId, byte isEnter)
    {
        var player = playerById(playerId);
        if (player == null) return;
        // Fill dummy MessageReader and call MyPhysics.HandleRpc as the corountines cannot be accessed
        var reader = new MessageReader();
        var bytes = BitConverter.GetBytes(ventId);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        reader.Buffer = bytes;
        reader.Length = bytes.Length;

        JackInTheBox.startAnimation(ventId);
        player.MyPhysics.HandleRpc(isEnter != 0 ? (byte)19 : (byte)20, reader);
    }

    public static void uncheckedMurderPlayer(byte sourceId, byte targetId, byte showAnimation)
    {
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started) return;
        var source = playerById(sourceId);
        var target = playerById(targetId);
        if (source != null && target != null)
        {
            if (showAnimation == 0) KillAnimationCoPerformKillPatch.hideNextAnimation = true;
            source.MurderPlayer(target);
        }
    }

    public static void uncheckedCmdReportDeadBody(byte sourceId, byte targetId)
    {
        var source = playerById(sourceId);
        var t = targetId == byte.MaxValue ? null : playerById(targetId).Data;
        source?.ReportDeadBody(t);
    }

    public static void uncheckedExilePlayer(byte targetId)
    {
        var target = playerById(targetId);
        target?.Exiled();
    }

    public static void dynamicMapOption(byte mapId)
    {
        GameOptionsManager.Instance.currentNormalGameOptions.MapId = mapId;
    }

    public static void setCrewmate(PlayerControl player)
    {
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
        if (player.PlayerId == CachedPlayer.LocalPlayer.PlayerId)
            CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
    }


    public static void setGameStarting()
    {
        GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 5f;
    }

    // Role functionality

    public static void FixLights()
    {
        var switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
        switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
    }

    public static void FixSubmergedOxygen()
    {
        SubmergedCompatibility.RepairOxygen();
    }

    public static void showIndomitableFlash()
    {
        if (Indomitable.indomitable == CachedPlayer.LocalPlayer.PlayerControl) showFlash(Indomitable.color);
    }

    public static void cleanBody(byte playerId, byte cleaningPlayerId)
    {
        if (Medium.futureDeadBodies != null)
        {
            var deadBody = Medium.futureDeadBodies.Find(x => x.Item1.Player.PlayerId == playerId)?.Item1;
            if (deadBody != null) deadBody.wasCleaned = true;
        }

        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == playerId)
            {
                Object.Destroy(array[i].gameObject);
                break;
            }
        }
        if (Vulture.vulture != null && cleaningPlayerId == Vulture.vulture.PlayerId)
        {
            Vulture.eatenBodies++;
            if (Vulture.eatenBodies == Vulture.vultureNumberToWin) Vulture.triggerVultureWin = true;
        }
    }

    public static void dissectionBody(byte playerId, byte killerId)
    {
        var player = playerById(playerId);
        var killer = playerById(killerId);
        for (var num = 0; num < Butcher.dissectedBodyCount; num++)
        {
            player.MyPhysics.StartCoroutine(player.KillAnimations.First().CoPerformKill(killer, player));
        }
        Butcher.dissected = player;

        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();

        for (var i = 1; i < array.Length && array[i].ParentId == playerId; i++)
        {
            var randomPosition = MapData.MapSpawnPosition().Random();
            array[i].transform.position = randomPosition;
        }
    }

    public static void dragBody(byte playerId)
    {
        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == playerId)
                Undertaker.deadBodyDraged = array[i];
    }

    public static void dropBody(byte playerId)
    {
        if (Undertaker.undertaker == null || Undertaker.deadBodyDraged == null) return;
        var deadBody = Undertaker.deadBodyDraged;
        Undertaker.deadBodyDraged = null;
        deadBody.transform.position = new Vector3(Undertaker.undertaker.GetTruePosition().x,
            Undertaker.undertaker.GetTruePosition().y, Undertaker.undertaker.transform.position.z);
    }

    public static void timeMasterRewindTime()
    {
        TimeMaster.shieldActive = false; // Shield is no longer active when rewinding
        SoundEffectsManager.stop("timemasterShield"); // Shield sound stopped when rewinding
        if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == CachedPlayer.LocalPlayer.PlayerControl)
            resetTimeMasterButton();
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.rewindTime / 2,
            new Action<float>(p =>
            {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

        if (TimeMaster.timeMaster == null || CachedPlayer.LocalPlayer.PlayerControl == TimeMaster.timeMaster)
            return; // Time Master himself does not rewind

        TimeMaster.isRewinding = true;

        if (MapBehaviour.Instance)
            MapBehaviour.Instance.Close();
        if (Minigame.Instance)
            Minigame.Instance.ForceClose();
        CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
    }

    public static void timeMasterShield()
    {
        TimeMaster.shieldActive = true;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.shieldDuration,
            new Action<float>(p =>
            {
                if (p == 1f) TimeMaster.shieldActive = false;
            })));
    }

    public static void impostorPromotesToLastImpostor(byte targetId)
    {
        var target = playerById(targetId);
        if (target == null) return;
        LastImpostor.lastImpostor = target;
    }

    public static void turnToImpostor(byte targetId)
    {
        var player = playerById(targetId);
        erasePlayerRoles(player.PlayerId);
        Helpers.turnToImpostor(player);
    }

    public static void veteranAlert()
    {
        Veteran.alertActive = true;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Veteran.alertDuration,
            new Action<float>(p =>
            {
                if (p == 1f) Veteran.alertActive = false;
            })));
    }

    public static void survivorVestActive()
    {
        Survivor.vestActive = true;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Survivor.vestDuration,
            new Action<float>(p =>
            {
                if (p == 1f) Survivor.vestActive = false;
            })));
    }

    public static void veteranKill(byte targetId)
    {
        if (CachedPlayer.LocalPlayer.PlayerControl == Veteran.veteran)
        {
            var player = playerById(targetId);
            checkMurderAttemptAndKill(Veteran.veteran, player);
        }
    }

    public static void medicSetShielded(byte shieldedId)
    {
        Medic.usedShield = true;
        Medic.shielded = playerById(shieldedId);
        Medic.futureShielded = null;
    }

    public static void shieldedMurderAttempt(byte blank)
    {
        if (!Medic.unbreakableShield)
        {
            Medic.shielded = null;
            return;
        }

        if (Medic.shielded == null || Medic.medic == null) return;

        var isShieldedAndShow = Medic.shielded == CachedPlayer.LocalPlayer.PlayerControl && Medic.showAttemptToShielded;
        isShieldedAndShow =
            isShieldedAndShow &&
            (Medic.meetingAfterShielding ||
             !Medic.showShieldAfterMeeting); // Dont show attempt, if shield is not shown yet
        var isMedicAndShow = Medic.medic == CachedPlayer.LocalPlayer.PlayerControl && Medic.showAttemptToMedic;

        if (isShieldedAndShow || isMedicAndShow || shouldShowGhostInfo())
            showFlash(Palette.ImpostorRed, 1.5f, GetString("medicShowAttemptText"));
    }

    public static void hostKill(byte targetId)
    {
        var target = playerById(targetId);
        target.Exiled();
        OverrideDeathReasonAndKiller(target, CustomDeathReason.HostCmdKill, HostPlayer);

        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
        foreach (var body in array)
        {
            if (body.ParentId != targetId) continue;
            Object.Destroy(body.gameObject);
            break;
        }
    }

    public static void hostRevive(byte targetId)
    {
        var target = playerById(targetId);
        target.Revive();
        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
        foreach (var body in array)
        {
            if (body.ParentId != targetId) continue;

            Object.Destroy(body.gameObject);
            target.Data.IsDead = false;
            break;
        }
    }

    public static void shifterShift(byte targetId)
    {
        var oldShifter = Shifter.shifter;
        var player = playerById(targetId);
        if (player == null || oldShifter == null) return;

        Shifter.futureShift = null;
        Shifter.clearAndReload();

        // Suicide (exile) when impostor or impostor variants
        if ((player.Data.Role.IsImpostor || Shifter.isShiftNeutral(player)) && oldShifter.IsAlive())
        {
            oldShifter.Exiled();
            OverrideDeathReasonAndKiller(oldShifter, CustomDeathReason.Shift, player);
            if (oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Lawyer.PromotesToPursuer();
            }
            else if (oldShifter == Executioner.target && AmongUsClient.Instance.AmHost && Executioner.executioner != null)
            {

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ExecutionerPromotesRole, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Executioner.PromotesRole();
            }
            return;
        }

        Shifter.shiftRole(oldShifter, player);

        // Set cooldowns to max for both players
        if (CachedPlayer.LocalPlayer.PlayerControl == oldShifter || CachedPlayer.LocalPlayer.PlayerControl == player)
            CustomButton.ResetAllCooldowns();
    }

    public static void swapperSwap(byte playerId1, byte playerId2)
    {
        if (MeetingHud.Instance)
        {
            Swapper.playerId1 = playerId1;
            Swapper.playerId2 = playerId2;
        }
    }

    public static void grenadierFlash(bool clear = false)
    {
        if (clear)
        {
            Grenadier.controls.Clear();
            return;
        }
        var closestPlayers = GetClosestPlayers(Grenadier.grenadier.GetTruePosition(), Grenadier.radius, true);
        Grenadier.controls = closestPlayers;
        foreach (var player in closestPlayers)
        {
            if (CachedPlayer.LocalId == player.PlayerId)
            {
                if (player.isImpostor() && !player.IsDead() && Grenadier.indicatorsMode > 0 && !MeetingHud.Instance)
                {
                    Grenadier.showFlash(Grenadier.flash, Grenadier.duration, 0.2f);
                }
                else if (!player.isImpostor() && !player.IsDead() && !MeetingHud.Instance)
                {
                    Grenadier.showFlash(Grenadier.flash, Grenadier.duration, 1f);
                }
            }
        }
    }

    public static void morphlingMorph(byte playerId)
    {
        var target = playerById(playerId);
        if (Morphling.morphling == null || target == null) return;

        Morphling.morphTimer = Morphling.duration;
        Morphling.morphTarget = target;
        if (Camouflager.camouflageTimer <= 0f)
            Morphling.morphling.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId,
                target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId,
                target.Data.DefaultOutfit.PetId);
    }

    public static void camouflagerCamouflage(byte setTimer)
    {
        if (isActiveCamoComms && setTimer != 2) return;
        if (isCamoComms) Camouflager.camoComms = true;
        if (Camouflager.camouflager == null && !Camouflager.camoComms) return;
        if (setTimer == 1) Camouflager.camouflageTimer = Camouflager.duration;
        if (MushroomSabotageActive) return; // Dont overwrite the fungle "camo"
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
            player.setLook("", 6, "", "", "", "");
    }

    public static void vampireSetBitten(byte targetId, byte performReset)
    {
        if (performReset != 0)
        {
            Vampire.bitten = null;
            return;
        }

        if (Vampire.vampire == null) return;
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
            if (player.PlayerId == targetId && !player.Data.IsDead)
                Vampire.bitten = player;
    }

    public static void partTimerSet(byte targetId)
    {
        if (targetId == byte.MaxValue) PartTimer.target = null;
        PlayerControl target = playerById(targetId);
        if (target == null) return;
        PartTimer.target = target;
        PartTimer.deathTurn = PartTimer.DeathDefaultTurn;
    }

    public static void prophetExamine(byte targetId)
    {
        var target = playerById(targetId);
        if (target == null) return;
        if (Prophet.examined.ContainsKey(target)) Prophet.examined.Remove(target);
        Prophet.examined.Add(target, Prophet.IsRed(target));
        Prophet.examinesLeft--;
        if ((Prophet.examineNum - Prophet.examinesLeft >= Prophet.examinesToBeRevealed) && Prophet.revealProphet) Prophet.isRevealed = true;
    }

    public static void placeGarlic(byte[] buff)
    {
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        new Garlic(position);
    }

    public static void trackerUsedTracker(byte targetId)
    {
        Tracker.usedTracker = true;
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
            if (player.PlayerId == targetId)
                Tracker.tracked = player;
    }

    public static void deputyUsedHandcuffs(byte targetId)
    {
        Deputy.remainingHandcuffs--;
        Deputy.handcuffedPlayers.Add(targetId);
    }

    public static void deputyPromotes()
    {
        if (Deputy.deputy != null)
        {
            // Deputy should never be null here, but there appeared to be a race condition during testing, which was removed.
            Sheriff.replaceCurrentSheriff(Deputy.deputy);
            Sheriff.formerDeputy = Deputy.deputy;
            Deputy.deputy = null;
            // No clear and reload, as we need to keep the number of handcuffs left etc
        }
    }

    public static void jackalCreatesSidekick(byte targetId)
    {
        var target = playerById(targetId);
        if (target == null) return;
        if (Executioner.target == target && Executioner.executioner != null && !Executioner.executioner.Data.IsDead)
        {
            if (Lawyer.lawyer == null && Executioner.promotesToLawyer)
            {
                Lawyer.lawyer = Executioner.executioner;
                Lawyer.target = Executioner.target;
                Executioner.clearAndReload();
            }
            else if (!Executioner.promotesToLawyer)
            {
                Pursuer.Player.Add(Executioner.executioner);
                Executioner.clearAndReload();
            }
        }

        FastDestroyableSingleton<RoleManager>.Instance.SetRole(target, RoleTypes.Crewmate);

        erasePlayerRoles(target.PlayerId);
        Jackal.sidekick = target;

        if (target == CachedPlayer.LocalPlayer.PlayerControl) SoundEffectsManager.play("jackalSidekick");
        if (HandleGuesser.isGuesserGm && CustomOptionHolder.guesserGamemodeSidekickIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(targetId))
            setGuesserGm(targetId);

        Jackal.canCreateSidekick = false;
    }

    public static void sidekickPromotes(byte playerId)
    {
        var player = playerById(playerId);
        if (player == null) return;
        if (Jackal.jackal.All(x => x.PlayerId != playerId && x.IsDead()) && Jackal.promotesToJackal && Jackal.sidekick.IsAlive())
        {
            Jackal.jackal.Add(player);
            Jackal.sidekick = null;
            Jackal.canCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick;
        }
    }

    public static void jackalCanSwooper(bool chance)
    {
        Jackal.canSwoop = chance;
    }

    public static void pavlovsCreateDog(byte targetId)
    {
        var player = playerById(targetId);
        if (player == null) return;
        if (Executioner.target == player && Executioner.executioner != null && !Executioner.executioner.Data.IsDead)
        {
            if (Lawyer.lawyer == null && Executioner.promotesToLawyer)
            {
                Lawyer.lawyer = Executioner.executioner;
                Lawyer.target = Executioner.target;
                Executioner.clearAndReload();
            }
            else if (!Executioner.promotesToLawyer)
            {
                Pursuer.Player.Add(Executioner.executioner);
                Executioner.clearAndReload();
            }
        }

        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
        if (player == Lawyer.lawyer && Lawyer.target != null)
        {
            var playerInfoTransform = Lawyer.target.cosmetics.nameText.transform.parent.FindChild("Info");
            var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();
            if (playerInfo != null) playerInfo.text = "";
        }

        erasePlayerRoles(player.PlayerId);
        Pavlovsdogs.pavlovsdogs.Add(player);
        if (player.PlayerId == CachedPlayer.LocalPlayer.PlayerId)
            CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
        if (player == CachedPlayer.LocalPlayer.PlayerControl) SoundEffectsManager.play("jackalSidekick");
        if (HandleGuesser.isGuesserGm && CustomOptionHolder.guesserGamemodePavlovsdogIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(targetId))
            setGuesserGm(targetId);
        Pavlovsdogs.createDogNum -= 1;
    }

    /// <summary>
    /// 抹除目标玩家的职业
    /// </summary>
    public static void erasePlayerRoles(byte playerId, bool ignoreModifier = true)
    {
        var player = playerById(playerId);
        if (player == null) return;

        // Crewmate roles
        if (player == Swooper.swooper) Swooper.clearAndReload();
        if (player == Mayor.mayor) Mayor.clearAndReload();
        if (player == Prosecutor.prosecutor) Prosecutor.clearAndReload();
        if (player == Portalmaker.portalmaker) Portalmaker.clearAndReload();
        if (player == Engineer.engineer) Engineer.clearAndReload();
        if (player == Sheriff.sheriff) Sheriff.sheriff = null;
        if (player == Deputy.deputy) Deputy.deputy = null;
        if (player == Detective.detective) Detective.clearAndReload();
        if (player == TimeMaster.timeMaster) TimeMaster.clearAndReload();
        if (player == Veteran.veteran) Veteran.clearAndReload();
        if (player == Medic.medic) Medic.clearAndReload();
        if (player == Seer.seer) Seer.clearAndReload();
        if (player == Hacker.hacker) Hacker.clearAndReload();
        if (player == BodyGuard.bodyguard) BodyGuard.clearAndReload();
        if (player == Balancer.balancer) Balancer.clearAndReload();
        if (player == Tracker.tracker) Tracker.clearAndReload();
        if (player == Snitch.snitch) Snitch.clearAndReload();
        if (player == Swapper.swapper) Swapper.clearAndReload();
        if (player == Spy.spy) Spy.clearAndReload();
        if (player == SecurityGuard.securityGuard) SecurityGuard.clearAndReload();
        if (player == Medium.medium) Medium.clearAndReload();
        if (player == InfoSleuth.infoSleuth) InfoSleuth.clearAndReload();
        if (player == Jumper.jumper) Jumper.clearAndReload();
        if (player == Trapper.trapper) Trapper.clearAndReload();
        if (player == Prophet.prophet) Prophet.clearAndReload();
        if (player == Vigilante.vigilante) Vigilante.clearAndReload();

        // Impostor roles
        if (player == Morphling.morphling) Morphling.clearAndReload();
        if (player == Bomber.bomber) Bomber.clearAndReload();
        if (player == Camouflager.camouflager) Camouflager.clearAndReload();
        if (player == Poucher.poucher && !Poucher.spawnModifier) Poucher.clearAndReload();
        if (player == Vampire.vampire) Vampire.clearAndReload();
        if (player == Eraser.eraser) Eraser.clearAndReload();
        if (player == Trickster.trickster) Trickster.clearAndReload();
        if (player == Cleaner.cleaner) Cleaner.clearAndReload();
        if (player == Undertaker.undertaker) Undertaker.clearAndReload();
        if (player == Mimic.mimic) Mimic.clearAndReload();
        if (player == WolfLord.Player) WolfLord.ClearAndReload();
        if (player == Warlock.warlock) Warlock.clearAndReload();
        if (player == Butcher.butcher) Butcher.clearAndReload();
        if (player == Witch.witch) Witch.clearAndReload();
        if (player == Escapist.escapist) Escapist.clearAndReload();
        if (player == Ninja.ninja) Ninja.clearAndReload();
        if (player == Yoyo.yoyo) Yoyo.clearAndReload();
        if (player == EvilTrapper.evilTrapper) EvilTrapper.clearAndReload();
        if (player == Blackmailer.blackmailer) Blackmailer.clearAndReload();
        if (player == Terrorist.terrorist) Terrorist.clearAndReload();
        if (player == Gambler.gambler) Gambler.clearAndReload();
        if (player == Grenadier.grenadier) Grenadier.clearAndReload();

        // Other roles
        if (player == Jester.jester) Jester.clearAndReload();
        if (player == Werewolf.werewolf) Werewolf.clearAndReload();
        if (player == Miner.miner) Miner.clearAndReload();
        if (player == Arsonist.arsonist) Arsonist.clearAndReload();
        if (Guesser.isGuesser(player.PlayerId)) Guesser.clear(player.PlayerId);

        if (Jackal.jackal.Any(x => x == player))
        {
            Jackal.jackal.RemoveAll(x => x == player);
            sidekickPromotes(Jackal.sidekick?.PlayerId ?? byte.MaxValue);
        }

        if (player == Pavlovsdogs.pavlovsowner)
        {
            Pavlovsdogs.createDogNum = CustomOptionHolder.pavlovsownerCreateDogNum.GetInt();
            Pavlovsdogs.pavlovsowner = null;
        }
        if (player == Jackal.sidekick) Jackal.sidekick = null;
        if (player == BountyHunter.bountyHunter) BountyHunter.clearAndReload();
        if (player == Vulture.vulture) Vulture.clearAndReload();
        if (player == Executioner.executioner) Executioner.clearAndReload();
        if (player == Lawyer.lawyer) Lawyer.clearAndReload();
        if (player == Thief.thief) Thief.clearAndReload();
        if (player == Juggernaut.juggernaut) Juggernaut.clearAndReload();
        if (player == Doomsayer.doomsayer) Doomsayer.clearAndReload();
        if (player == Akujo.akujo) Akujo.clearAndReload();
        if (player == Witness.Player) Witness.ClearAndReload();
        if (player == PartTimer.partTimer) PartTimer.clearAndReload();
        if (player == Vortox.Player) Vortox.ClearAndReload();

        if (player == Cursed.cursed) Cursed.clearAndReload();
        if (player == Shifter.shifter) Shifter.clearAndReload();

        Assassin.assassin.RemoveAll(x => x.PlayerId == player.PlayerId);
        Amnisiac.Player.RemoveAll(x => x.PlayerId == playerId);
        Pavlovsdogs.pavlovsdogs.RemoveAll(x => x.PlayerId == player.PlayerId);
        Pursuer.Player.RemoveAll(x => x.PlayerId == player.PlayerId);
        Survivor.Player.RemoveAll(x => x.PlayerId == player.PlayerId);

        // Modifier
        if (!ignoreModifier)
        {
            Bait.bait.RemoveAll(x => x.PlayerId == player.PlayerId);
            Bloody.bloody.RemoveAll(x => x.PlayerId == player.PlayerId);
            AntiTeleport.antiTeleport.RemoveAll(x => x.PlayerId == player.PlayerId);
            Sunglasses.sunglasses.RemoveAll(x => x.PlayerId == player.PlayerId);
            Torch.torch.RemoveAll(x => x.PlayerId == player.PlayerId);
            Flash.flash.RemoveAll(x => x.PlayerId == player.PlayerId);
            Multitasker.multitasker.RemoveAll(x => x.PlayerId == player.PlayerId);
            Vip.vip.RemoveAll(x => x.PlayerId == player.PlayerId);
            Invert.invert.RemoveAll(x => x.PlayerId == player.PlayerId);
            Chameleon.chameleon.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (player == Lovers.lover1 || player == Lovers.lover2) Lovers.clearAndReload(); // The whole Lover couple is being erased
            if (player == Specoality.specoality) Specoality.clearAndReload();
            if (player == Tiebreaker.tiebreaker) Tiebreaker.clearAndReload();
            if (player == Mini.mini) Mini.clearAndReload();
            if (player == Aftermath.aftermath) Aftermath.clearAndReload();
            if (player == Giant.giant) Giant.clearAndReload();
            if (player == Watcher.watcher) Watcher.clearAndReload();
            if (player == Radar.radar) Radar.clearAndReload();
            if (player == Poucher.poucher && Poucher.spawnModifier) Poucher.clearAndReload();
            if (player == ButtonBarry.buttonBarry) ButtonBarry.clearAndReload();
            if (player == Disperser.disperser) Disperser.clearAndReload();
            if (player == Indomitable.indomitable) Indomitable.clearAndReload();
            if (player == Tunneler.tunneler) Tunneler.clearAndReload();
            if (player == Slueth.slueth) Slueth.clearAndReload();
            if (player == Blind.blind) Blind.clearAndReload();
        }
    }

    public static void clearGhostRoles(byte playerId)
    {
        var player = playerById(playerId);

        if (player == GhostEngineer.Player) GhostEngineer.ClearAndReload();
        if (player == Specter.Player) Specter.ClearAndReload();
    }

    public static void infoSleuthTarget(byte playerId)
    {
        var player = playerById(playerId);
        if (player != null) InfoSleuth.target = player;
    }

    public static void infoSleuthNoTarget()
    {
        InfoSleuth.target = null;
    }


    public static void balancerBalance(byte sourceId, byte player1Id, byte player2Id)
    {
        Balancer.IsAbilityUsed--;
        PlayerControl source = playerById(sourceId);
        PlayerControl player1 = playerById(player1Id);
        PlayerControl player2 = playerById(player2Id);
        if (source is null || player1 is null || player2 is null) return;
        Balancer.StartAbility(source, player1, player2);

        if (MeetingHud.Instance)
        {
            MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, byte.MaxValue - 1);
            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                pva.UnsetVote();
                var voteAreaPlayer = playerById(pva.TargetPlayerId);
                if (voteAreaPlayer != null && !voteAreaPlayer.AmOwner) continue;
                MeetingHud.Instance.ClearVote();
            }

            if (AmongUsClient.Instance.AmHost) MeetingHud.Instance.CheckForEndVoting();
        }
    }

    public static void setFutureErased(byte playerId)
    {
        var player = playerById(playerId);
        Eraser.futureErased ??= new List<PlayerControl>();
        if (player != null) Eraser.futureErased.Add(player);
    }

    public static void setFutureShifted(byte playerId)
    {
        Shifter.futureShift = playerById(playerId);
    }

    public static void disperse()
    {
        Coroutines.Start(showFlashCoroutine(Palette.ImpostorRed, 1f, 0.36f));

        if (!AntiTeleport.antiTeleport.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId) && CachedPlayer.LocalPlayer.IsAlive)
        {
            foreach (var player in CachedPlayer.AllPlayers)
            {
                if (Minigame.Instance) Minigame.Instance.ForceClose();
                if (MapBehaviour.Instance) MapBehaviour.Instance.Close();

                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }

                if (Disperser.DispersesToVent)
                {
                    CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo
                    (MapData.FindVentSpawnPositions()[rnd.Next(MapData.FindVentSpawnPositions().Count)]);
                }
                else
                {
                    CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo
                    (MapData.MapSpawnPosition()[rnd.Next(MapData.MapSpawnPosition().Count)]);
                }
            }
            Disperser.remainingDisperses--;
        }
    }

    public static void setFutureShielded(byte playerId)
    {
        Medic.futureShielded = playerById(playerId);
        Medic.usedShield = true;
    }

    public static void giveBomb(byte playerId, bool bomb = false)
    {
        if (playerId == byte.MaxValue)
        {
            Bomber.hasBombPlayer = null;
            Bomber.bombActive = false;
            Bomber.hasAlerted = false;
            Bomber.timeLeft = 0;
            return;
        }

        if (bomb)
        {
            Bomber.hasBombPlayer = playerById(playerId);
            Bomber.timeLeft += (int)0.5;
            return;
        }

        Bomber.hasBombPlayer = playerById(playerId);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.bombDelay,
            new Action<float>(p =>
            {
                if (p == 1f) Bomber.bombActive = true;
            })));
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.bombDelay + Bomber.bombTimer,
            new Action<float>(p =>
            {
                // Delayed action
                if (Bomber.hasBombPlayer.IsDead()) return;
                if (p == 1f && Bomber.bombActive)
                {
                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                    if (Bomber.bomber.IsAlive() && CachedPlayer.LocalPlayer.PlayerControl == Bomber.bomber)
                        checkMurderAttemptAndKill(Bomber.bomber, Bomber.hasBombPlayer, false, false, true, true);
                    Bomber.hasBombPlayer = null;
                    Bomber.bombActive = false;
                    Bomber.hasAlerted = false;
                    Bomber.timeLeft = 0;
                }

                if (CachedPlayer.LocalPlayer.PlayerControl == Bomber.hasBombPlayer)
                {
                    var totalTime = (int)(Bomber.bombDelay + Bomber.bombTimer);
                    var timeLeft = (int)(totalTime - (totalTime * p));
                    if (timeLeft <= Bomber.bombTimer)
                    {
                        if (Bomber.timeLeft != timeLeft)
                        {
                            _ = new CustomMessage("你手中的炸弹将在 " + timeLeft + " 秒后引爆!", 1f);
                            Bomber.timeLeft = timeLeft;
                        }

                        if (timeLeft % 5 == 0)
                        {
                            if (!Bomber.hasAlerted)
                            {
                                Coroutines.Start(showFlashCoroutine(Palette.ImpostorRed, 0.75f));
                                Bomber.hasAlerted = true;
                            }
                        }
                        else
                        {
                            Bomber.hasAlerted = false;
                        }
                    }
                }
            })));
    }

    public static void setFutureSpelled(byte playerId)
    {
        var player = playerById(playerId);
        Witch.futureSpelled ??= new List<PlayerControl>();
        if (player != null) Witch.futureSpelled.Add(player);
    }

    public static void placeNinjaTrace(byte[] buff)
    {
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        new NinjaTrace(position, Ninja.traceTime);
        if (CachedPlayer.LocalPlayer.PlayerControl != Ninja.ninja)
            Ninja.ninjaMarked = null;
    }

    public static void setInvisible(byte playerId, byte flag)
    {
        var target = playerById(playerId);
        if (target == null) return;
        if (flag == byte.MaxValue)
        {
            target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
            target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
            target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(1f);

            if (Camouflager.camouflageTimer <= 0 && !MushroomSabotageActive && !isCamoComms)
                target.setDefaultLook();
            Ninja.isInvisable = false;
            return;
        }

        target.setLook("", 6, "", "", "", "");
        var color = Color.clear;
        var canSee = CachedPlayer.LocalPlayer.Data.Role.IsImpostor || CachedPlayer.LocalPlayer.Data.IsDead;
        if (canSee) color.a = 0.1f;
        target.cosmetics.currentBodySprite.BodySprite.color = color;
        target.cosmetics.colorBlindText.gameObject.SetActive(false);
        target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
        Ninja.invisibleTimer = Ninja.invisibleDuration;
        Ninja.isInvisable = true;
    }

    public static void yoyoMarkLocation(byte[] buff)
    {
        if (Yoyo.yoyo == null) return;
        Vector3 position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        Yoyo.markLocation(position);
        new Silhouette(position, -1, false);
    }

    public static void yoyoBlink(bool isFirstJump, byte[] buff)
    {
        Message($"blink fistjumpo: {isFirstJump}");
        if (Yoyo.yoyo == null || Yoyo.markedLocation == null) return;
        var markedPos = (Vector3)Yoyo.markedLocation;
        Yoyo.yoyo.NetTransform.SnapTo(markedPos);

        var markedSilhouette = Silhouette.silhouettes.FirstOrDefault(s => s.gameObject.transform.position.x == markedPos.x && s.gameObject.transform.position.y == markedPos.y);
        if (markedSilhouette != null)
            markedSilhouette.permanent = false;

        Vector3 position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        // Create Silhoutte At Start Position:
        if (isFirstJump)
        {
            Yoyo.markLocation(position);
            new Silhouette(position, Yoyo.blinkDuration, true);
        }
        else
        {
            new Silhouette(position, 5, true);
            Yoyo.markedLocation = null;
        }
        if (Chameleon.chameleon.Any(x => x.PlayerId == Yoyo.yoyo.PlayerId)) // Make the Yoyo visible if chameleon!
            Chameleon.lastMoved[Yoyo.yoyo.PlayerId] = Time.time;
    }

    public static void akujoSetHonmei(byte akujoId, byte targetId)
    {
        PlayerControl akujo = playerById(akujoId);
        PlayerControl target = playerById(targetId);

        if (akujo != null && Akujo.honmei == null)
        {
            Akujo.honmei = target;
            Akujo.breakLovers(target);
        }
    }

    public static void akujoSetKeep(byte akujoId, byte targetId)
    {
        var akujo = playerById(akujoId);
        PlayerControl target = playerById(targetId);

        if (akujo != null && Akujo.keepsLeft > 0)
        {
            Akujo.keeps.Add(target);
            Akujo.breakLovers(target);
            Akujo.keepsLeft--;
        }
    }

    public static void akujoSuicide(byte akujoId)
    {
        var akujo = playerById(akujoId);
        if (akujo != null)
        {
            akujo.Exiled();
            OverrideDeathReasonAndKiller(akujo, CustomDeathReason.Loneliness);

            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(akujo.KillSfx, false, 0.8f);
            if (PlayerControl.LocalPlayer == Akujo.akujo)
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(akujo.Data, akujo.Data);
        }
    }

    public static void Mine(int ventId, byte[] buff, float zAxis)
    {
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));

        var ventPrefab = Object.FindObjectOfType<Vent>();
        var vent = Object.Instantiate(ventPrefab, ventPrefab.transform.parent);
        vent.Id = ventId;
        vent.transform.position = new Vector3(position.x, position.y, zAxis);

        if (Miner.Vents.Count > 0)
        {
            var leftVent = Miner.Vents[^1];
            vent.Left = leftVent;
            leftVent.Right = vent;
        }
        else
        {
            vent.Left = null;
        }

        vent.Right = null;
        vent.Center = null;
        var allVents = ShipStatus.Instance.AllVents.ToList();
        allVents.Add(vent);
        ShipStatus.Instance.AllVents = allVents.ToArray();
        Miner.Vents.Add(vent);
        Miner.LastMined = DateTime.UtcNow;

        if (SubmergedCompatibility.IsSubmerged)
        {
            vent.gameObject.layer = 12;
            // just in case elevator vent is not blocked
            vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            if (vent.gameObject.transform.position.y > -7)
            {
                vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x,
                    vent.gameObject.transform.position.y, 0.03f);
            }
            else
            {
                vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x,
                    vent.gameObject.transform.position.y, 0.0009f);
                vent.gameObject.transform.localPosition = new Vector3(vent.gameObject.transform.localPosition.x,
                    vent.gameObject.transform.localPosition.y, -0.003f);
            }
        }
    }

    public static void setSwoop(byte playerId, byte flag)
    {
        var target = playerById(playerId);
        if (target == null) return;
        if (flag == byte.MaxValue)
        {
            target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
            target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
            target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(1f);
            if (Camouflager.camouflageTimer <= 0 && !MushroomSabotageActive & !isCamoComms)
                target.setDefaultLook();
            Swooper.isInvisable = false;
            return;
        }

        target.setLook("", 6, "", "", "", "");
        var color = Color.clear;
        var canSee = Swooper.swooper == CachedPlayer.LocalPlayer.PlayerControl || CachedPlayer.LocalPlayer.Data.IsDead;
        if (canSee) color.a = 0.1f;
        target.cosmetics.currentBodySprite.BodySprite.color = color;
        target.cosmetics.colorBlindText.gameObject.SetActive(false);
        target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
        Swooper.swoopTimer = Swooper.duration;
        Swooper.isInvisable = true;
    }

    public static void setJackalSwoop(byte playerId, byte flag)
    {
        var target = playerById(playerId);
        if (target == null) return;
        if (flag == byte.MaxValue)
        {
            target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
            target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
            target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(1f);
            if (Camouflager.camouflageTimer <= 0 && !MushroomSabotageActive & !isCamoComms)
                target.setDefaultLook();
            Jackal.isInvisable = false;
            return;
        }

        target.setLook("", 6, "", "", "", "");
        var color = Color.clear;
        var canSee = Jackal.jackal.Any(x => x == CachedPlayer.LocalPlayer.PlayerControl) ||
                     Jackal.sidekick == CachedPlayer.LocalPlayer.PlayerControl ||
                     CachedPlayer.LocalPlayer.Data.IsDead;
        if (canSee) color.a = 0.1f;
        target.cosmetics.currentBodySprite.BodySprite.color = color;
        target.cosmetics.colorBlindText.gameObject.SetActive(false);
        target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
        Jackal.swoopTimer = Jackal.duration;
        Jackal.isInvisable = true;
    }

    public static void trapperKill(byte trapId, byte trapperId, byte playerId)
    {
        var trapper = playerById(trapperId);
        var target = playerById(playerId);
        KillTrap.trapKill(trapId, trapper, target);
    }

    public static void placeTrap(byte[] buff)
    {
        var pos = Vector3.zero;
        pos.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        pos.y = BitConverter.ToSingle(buff, 1 * sizeof(float)) - 0.2f;
        _ = new KillTrap(pos);
    }

    public static void clearTrap()
    {
        KillTrap.clearAllTraps();
    }

    public static void activateTrap(byte trapId, byte trapperId, byte playerId)
    {
        var trapper = playerById(trapperId);
        var player = playerById(playerId);
        KillTrap.activateTrap(trapId, trapper, player);
    }

    public static void disableTrap(byte trapId)
    {
        KillTrap.disableTrap(trapId);
    }

    public static void trapperMeetingFlag()
    {
        KillTrap.onMeeting();
    }

    public static void setInvisibleGen(byte playerId, byte flag)
    {
        var target = playerById(playerId);
        if (target == null) return;
        if (flag == byte.MaxValue)
        {
            target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
            target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
            target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(1f);
            if (Camouflager.camouflageTimer <= 0 && !MushroomSabotageActive)
                target.setDefaultLook(); // testing
            return;
        }

        target.setLook("", 6, "", "", "", "");
        var color = Color.clear;
        if (CachedPlayer.LocalPlayer.Data.IsDead) color.a = 0.1f;
        target.cosmetics.currentBodySprite.BodySprite.color = color;
        target.cosmetics.colorBlindText.gameObject.SetActive(false);
        //target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
    }

    public static void placePortal(byte[] buff)
    {
        Vector3 position = Vector2.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        _ = new Portal(position);
    }

    public static void usePortal(byte playerId, byte exit)
    {
        Portal.startTeleport(playerId, exit);
    }

    public static void placeJackInTheBox(byte[] buff)
    {
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        new JackInTheBox(position);
    }

    public static void lightsOut()
    {
        Trickster.lightsOutTimer = Trickster.lightsOutDuration;
        // If the local player is impostor indicate lights out
        if (hasImpVision(GameData.Instance.GetPlayerById(CachedPlayer.LocalPlayer.PlayerId)))
            _ = new CustomMessage("TricksterLightsOut".Translate(), Trickster.lightsOutDuration);
    }

    public static void placeCamera(byte[] buff)
    {
        var referenceCamera = Object.FindObjectOfType<SurvCamera>();
        if (referenceCamera == null) return; // Mira HQ

        SecurityGuard.remainingScrews -= SecurityGuard.camPrice;
        SecurityGuard.placedCameras++;

        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));

        var camera = Object.Instantiate(referenceCamera);
        camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
        camera.CamName = $"Security Camera {SecurityGuard.placedCameras}";
        camera.Offset = new Vector3(0f, 0f, camera.Offset.z);
        if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 2 ||
            GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4)
            camera.transform.localRotation = new Quaternion(0, 0, 1, 1); // Polus and Airship 

        if (SubmergedCompatibility.IsSubmerged)
        {
            // remove 2d box collider of console, so that no barrier can be created. (irrelevant for now, but who knows... maybe we need it later)
            var fixConsole = camera.transform.FindChild("FixConsole");
            if (fixConsole != null)
            {
                var boxCollider = fixConsole.GetComponent<BoxCollider2D>();
                if (boxCollider != null) Object.Destroy(boxCollider);
            }
        }


        if (CachedPlayer.LocalPlayer.PlayerControl == SecurityGuard.securityGuard)
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            camera.gameObject.SetActive(false);
        }

        camerasToAdd.Add(camera);
    }

    public static void sealVent(int ventId)
    {
        var vent = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault(x => x != null && x.Id == ventId);
        if (vent == null) return;

        SecurityGuard.remainingScrews -= SecurityGuard.ventPrice;
        if (CachedPlayer.LocalPlayer.PlayerControl == SecurityGuard.securityGuard)
        {
            var animator = vent.GetComponent<SpriteAnim>();

            vent.EnterVentAnim = vent.ExitVentAnim = null;
            var newSprite = animator == null
                ? SecurityGuard.staticVentSealedSprite
                : SecurityGuard.getAnimatedVentSealedSprite();
            var rend = vent.myRend;
            if (isFungle)
            {
                newSprite = SecurityGuard.fungleVentSealedSprite;
                rend = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                animator = vent.transform.GetChild(3).GetComponent<SpriteAnim>();
            }

            animator?.Stop();
            rend.sprite = newSprite;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = SecurityGuard.submergedCentralUpperVentSealedSprite;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = SecurityGuard.submergedCentralLowerVentSealedSprite;
            rend.color = new Color(1f, 1f, 1f, 0.5f);
            vent.name = "FutureSealedVent_" + vent.name;
        }

        ventsToSeal.Add(vent);
    }

    public static void lawyerSetTarget(byte playerId)
    {
        Lawyer.target = playerById(playerId);
    }

    public static void executionerSetTarget(byte playerId)
    {
        Executioner.target = playerById(playerId);
    }

    public static void useCameraTime(float time)
    {
        restrictCamerasTime -= time;
    }

    public static void useVitalsTime(float time)
    {
        restrictVitalsTime -= time;
    }

    public static void blackmailPlayer(byte playerId)
    {
        var target = playerById(playerId);
        Blackmailer.blackmailed = target;
    }

    public static void showBodyGuardFlash()
    {
        if (CustomOptionHolder.bodyGuardFlash.GetBool()) showFlash(BodyGuard.color);
    }

    public static void bodyGuardGuardPlayer(byte targetId)
    {
        var target = playerById(targetId);
        BodyGuard.usedGuard = true;
        BodyGuard.guarded = target;
    }

    public static void unblackmailPlayer()
    {
        Blackmailer.blackmailed = null;
        Blackmailer.alreadyShook = false;
    }

    public static void pursuerSetBlanked(byte playerId, byte value)
    {
        var target = playerById(playerId);
        if (target == null) return;
        Pursuer.blankedList.RemoveAll(x => x.PlayerId == playerId);
        if (value > 0) Pursuer.blankedList.Add(target);
    }

    public static void bloody(byte killerPlayerId, byte bloodyPlayerId)
    {
        if (Bloody.active.ContainsKey(killerPlayerId)) return;
        Bloody.active.Add(killerPlayerId, Bloody.duration);
        Bloody.bloodyKillerMap.Add(killerPlayerId, bloodyPlayerId);
    }

    public static void setFirstKill(byte playerId)
    {
        var target = playerById(playerId);
        if (target == null) return;
        firstKillPlayer = target;
    }

    public static void setChatNotificationOverlay(byte localPlayerId, byte targetPlayerId)
    {
        try
        {
            var playerControl = CachedPlayer.LocalPlayer.PlayerControl;
            if (MeetingHud.Instance.playerStates == null) return;
            var playerVoteArea =
                MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == targetPlayerId);
            if (playerVoteArea == null) return;
            var rend = new GameObject().AddComponent<SpriteRenderer>();
            rend.transform.SetParent(playerVoteArea.transform);
            rend.gameObject.layer = playerVoteArea.Megaphone.gameObject.layer;
            rend.transform.localPosition = new Vector3(-0.5f, 0.2f, -1f);
            rend.sprite = new ResourceSprite("TheOtherRoles.Resources.ChatOverlay.png", 130f);
            if (playerControl.PlayerId != localPlayerId) rend.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(2f, (Action<float>)delegate (float p)
                {
                    if (p == 1f)
                    {
                        rend?.gameObject?.SetActive(false);
                        Object.Destroy(rend?.gameObject);
                    }
                }));
        }
        catch
        {
            Message("Chat Notification Overlay is Detected");
        }
    }

    public static void setTrap(byte[] buff)
    {
        if (Trapper.trapper == null) return;
        Trapper.charges -= 1;
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        _ = new Trap(position);
    }

    public static void triggerTrap(byte playerId, byte trapId)
    {
        Trap.triggerTrap(playerId, trapId);
    }

    public static void setGuesserGm(byte playerId)
    {
        var target = playerById(playerId);
        if (target == null) return;
        _ = new GuesserGM(target);
    }

    public static void receiveGhostInfo(byte senderId, MessageReader reader)
    {
        var sender = playerById(senderId);

        var infoType = (GhostInfoTypes)reader.ReadByte();
        switch (infoType)
        {
            case GhostInfoTypes.HandcuffNoticed:
                Deputy.setHandcuffedKnows(true, senderId);
                break;
            case GhostInfoTypes.HandcuffOver:
                _ = Deputy.handcuffedKnows.Remove(senderId);
                break;
            case GhostInfoTypes.ArsonistDouse:
                Arsonist.dousedPlayers.Add(playerById(reader.ReadByte()));
                break;
            case GhostInfoTypes.BountyTarget:
                BountyHunter.bounty = playerById(reader.ReadByte());
                break;
            case GhostInfoTypes.NinjaMarked:
                Ninja.ninjaMarked = playerById(reader.ReadByte());
                break;
            case GhostInfoTypes.WarlockTarget:
                Warlock.curseVictim = playerById(reader.ReadByte());
                break;
            case GhostInfoTypes.GhostChat:
                string chat = reader.ReadString();
                if (shouldShowGhostInfo()) FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, chat);
                break;
            case GhostInfoTypes.BlankUsed:
                Pursuer.blankedList.Remove(sender);
                break;
            case GhostInfoTypes.VampireTimer:
                vampireKillButton.Timer = reader.ReadByte();
                break;
            case GhostInfoTypes.DeathReasonAndKiller:
                OverrideDeathReasonAndKiller(playerById(reader.ReadByte()), (CustomDeathReason)reader.ReadByte(), playerById(reader.ReadByte()));
                break;
        }
    }

    public static void placeBomb(byte[] buff)
    {
        if (Terrorist.terrorist == null) return;
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        new Bomb(position);
    }

    public static void defuseBomb()
    {
        try
        {
            SoundEffectsManager.playAtPosition("bombDefused", Terrorist.bomb.bomb.transform.position,
                range: Terrorist.hearRange);
        }
        catch
        {
        }

        Terrorist.clearBomb();
        terroristButton.Timer = terroristButton.MaxTimer;
        terroristButton.isEffectActive = false;
        terroristButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
internal class RPCHandlerPatch
{
    private static Dictionary<CustomRPC, string>? RpcNames;

    private static void GetRpcNames()
    {
        RpcNames ??= new Dictionary<CustomRPC, string>();
        var values = EnumHelper.GetAllValues<CustomRPC>();
        foreach (var value in values) RpcNames.Add(value, Enum.GetName(value) ?? string.Empty);
    }

    private static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        var packetId = (CustomRPC)callId;
        if (RpcNames!.ContainsKey(packetId)) return;
        if (DebugMode) Info($"接收 PlayerControl 原版Rpc RpcId{callId} Message Size {reader.Length}");
    }

    private static bool Prefix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        if (RpcNames == null)
            GetRpcNames();

        var packetId = (CustomRPC)callId;
        if (!RpcNames!.ContainsKey(packetId))
            return true;

        if (DebugMode && callId != 120) Info($"接收 PlayerControl CustomRpc RpcId{callId} Rpc {RpcNames?[(CustomRPC)callId] ?? nameof(packetId)} Message Size {reader.Length}");
        switch (packetId)
        {
            // Main Controls

            case CustomRPC.ResetVaribles:
                RPCProcedure.resetVariables();
                break;
            case CustomRPC.ShareOptions:
                RPCProcedure.HandleShareOptions(reader.ReadByte(), reader);
                break;
            case CustomRPC.WorkaroundSetRoles:
                RPCProcedure.workaroundSetRoles(reader.ReadByte(), reader);
                break;
            case CustomRPC.SetRole:
                RPCProcedure.setRole(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.SetModifier:
                RPCProcedure.setModifier(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.SetGhostRole:
                RPCProcedure.setGhostRole(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.VersionHandshake:
                byte major = reader.ReadByte();
                byte minor = reader.ReadByte();
                byte patch = reader.ReadByte();
                float timer = reader.ReadSingle();
                if (!AmongUsClient.Instance.AmHost && timer >= 0f) GameStartManagerPatch.timer = timer;
                int versionOwnerId = reader.ReadPackedInt32();
                byte revision = 0xFF;
                Guid guid;
                if (reader.Length - reader.Position >= 17)
                { // enough bytes left to read
                    revision = reader.ReadByte();
                    // GUID
                    byte[] gbytes = reader.ReadBytes(16);
                    guid = new Guid(gbytes);
                }
                else
                {
                    guid = new Guid(new byte[16]);
                }
                RPCProcedure.versionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                break;

            case CustomRPC.UseUncheckedVent:
                var ventId = reader.ReadPackedInt32();
                var ventingPlayer = reader.ReadByte();
                var isEnter = reader.ReadByte();
                RPCProcedure.useUncheckedVent(ventId, ventingPlayer, isEnter);
                break;

            case CustomRPC.UncheckedMurderPlayer:
                var source = reader.ReadByte();
                var target = reader.ReadByte();
                var showAnimation = reader.ReadByte();
                RPCProcedure.uncheckedMurderPlayer(source, target, showAnimation);
                break;

            case CustomRPC.UncheckedExilePlayer:
                var exileTarget = reader.ReadByte();
                RPCProcedure.uncheckedExilePlayer(exileTarget);
                break;

            case CustomRPC.UncheckedCmdReportDeadBody:
                var reportSource = reader.ReadByte();
                var reportTarget = reader.ReadByte();
                RPCProcedure.uncheckedCmdReportDeadBody(reportSource, reportTarget);
                break;

            case CustomRPC.DynamicMapOption:
                RPCProcedure.dynamicMapOption(reader.ReadByte());
                break;

            case CustomRPC.SetGameStarting:
                RPCProcedure.setGameStarting();
                break;

            // Role functionality

            case CustomRPC.FixLights:
                RPCProcedure.FixLights();
                break;
            case CustomRPC.FixSubmergedOxygen:
                RPCProcedure.FixSubmergedOxygen();
                break;

            case CustomRPC.UseCameraTime:
                RPCProcedure.useCameraTime(reader.ReadSingle());
                break;

            case CustomRPC.UseVitalsTime:
                RPCProcedure.useVitalsTime(reader.ReadSingle());
                break;

            case CustomRPC.CleanBody:
                RPCProcedure.cleanBody(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.DissectionBody:
                RPCProcedure.dissectionBody(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.BlackmailPlayer:
                RPCProcedure.blackmailPlayer(reader.ReadByte());
                break;

            case CustomRPC.UnblackmailPlayer:
                RPCProcedure.unblackmailPlayer();
                break;

            case CustomRPC.DragBody:
                RPCProcedure.dragBody(reader.ReadByte());
                break;

            case CustomRPC.DropBody:
                RPCProcedure.dropBody(reader.ReadByte());
                break;

            case CustomRPC.TimeMasterRewindTime:
                RPCProcedure.timeMasterRewindTime();
                break;

            case CustomRPC.TimeMasterShield:
                RPCProcedure.timeMasterShield();
                break;

            case CustomRPC.AmnisiacTakeRole:
                Amnisiac.TakeRole(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.SpecterTakeRole:
                Specter.TakeRole(reader.ReadByte());
                break;

            case CustomRPC.ImpostorPromotesToLastImpostor:
                RPCProcedure.impostorPromotesToLastImpostor(reader.ReadByte());
                break;

            case CustomRPC.MimicMimicRole:
                Mimic.MimicRole(reader.ReadByte());
                break;

            case CustomRPC.ShowIndomitableFlash:
                RPCProcedure.showIndomitableFlash();
                break;

            case CustomRPC.VeteranAlert:
                RPCProcedure.veteranAlert();
                break;

            case CustomRPC.VeteranKill:
                RPCProcedure.veteranKill(reader.ReadByte());
                break;

            case CustomRPC.MedicSetShielded:
                RPCProcedure.medicSetShielded(reader.ReadByte());
                break;

            case CustomRPC.ShieldedMurderAttempt:
                RPCProcedure.shieldedMurderAttempt(reader.ReadByte());
                break;

            case CustomRPC.ShifterShift:
                RPCProcedure.shifterShift(reader.ReadByte());
                break;

            case CustomRPC.SwapperSwap:
                var playerId1 = reader.ReadByte();
                var playerId2 = reader.ReadByte();
                RPCProcedure.swapperSwap(playerId1, playerId2);
                break;

            case CustomRPC.MorphlingMorph:
                RPCProcedure.morphlingMorph(reader.ReadByte());
                break;

            case CustomRPC.CamouflagerCamouflage:
                var setTimer = reader.ReadByte();
                RPCProcedure.camouflagerCamouflage(setTimer);
                break;

            /*case CustomRPC.DoomsayerMeeting:
                if (!shouldShowGhostInfo()) break;
                var index = reader.ReadPackedInt32();
                for (var i = 1; i < index; i++)
                {
                    var message = reader.ReadString();
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Doomsayer.doomsayer, message);
                }

                break;*/

            case CustomRPC.VampireSetBitten:
                var bittenId = reader.ReadByte();
                var reset = reader.ReadByte();
                RPCProcedure.vampireSetBitten(bittenId, reset);
                break;

            case CustomRPC.PlaceGarlic:
                RPCProcedure.placeGarlic(reader.ReadBytesAndSize());
                break;

            case CustomRPC.TrackerUsedTracker:
                RPCProcedure.trackerUsedTracker(reader.ReadByte());
                break;

            case CustomRPC.BodyGuardGuardPlayer:
                RPCProcedure.bodyGuardGuardPlayer(reader.ReadByte());
                break;

            case CustomRPC.DeputyUsedHandcuffs:
                RPCProcedure.deputyUsedHandcuffs(reader.ReadByte());
                break;

            case CustomRPC.DeputyPromotes:
                RPCProcedure.deputyPromotes();
                break;

            case CustomRPC.JackalCreatesSidekick:
                RPCProcedure.jackalCreatesSidekick(reader.ReadByte());
                break;

            case CustomRPC.PavlovsCreateDog:
                RPCProcedure.pavlovsCreateDog(reader.ReadByte());
                break;

            case CustomRPC.SidekickPromotes:
                RPCProcedure.sidekickPromotes(reader.ReadByte());
                break;

            case CustomRPC.ErasePlayerRoles:
                var eraseTarget = reader.ReadByte();
                RPCProcedure.erasePlayerRoles(eraseTarget);
                Eraser.alreadyErased.Add(eraseTarget);
                break;

            case CustomRPC.ClearGhostRoles:
                RPCProcedure.clearGhostRoles(reader.ReadByte());
                break;

            case CustomRPC.SetFutureErased:
                RPCProcedure.setFutureErased(reader.ReadByte());
                break;

            case CustomRPC.PartTimerSet:
                RPCProcedure.partTimerSet(reader.ReadByte());
                break;

            case CustomRPC.SetFutureShifted:
                RPCProcedure.setFutureShifted(reader.ReadByte());
                break;

            case CustomRPC.Disperse:
                RPCProcedure.disperse();
                break;

            case CustomRPC.SetFutureShielded:
                RPCProcedure.setFutureShielded(reader.ReadByte());
                break;

            case CustomRPC.PlaceNinjaTrace:
                RPCProcedure.placeNinjaTrace(reader.ReadBytesAndSize());
                break;

            case CustomRPC.PlacePortal:
                RPCProcedure.placePortal(reader.ReadBytesAndSize());
                break;

            case CustomRPC.UsePortal:
                RPCProcedure.usePortal(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.PlaceJackInTheBox:
                RPCProcedure.placeJackInTheBox(reader.ReadBytesAndSize());
                break;

            case CustomRPC.LightsOut:
                RPCProcedure.lightsOut();
                break;

            case CustomRPC.PlaceCamera:
                RPCProcedure.placeCamera(reader.ReadBytesAndSize());
                break;

            case CustomRPC.SealVent:
                RPCProcedure.sealVent(reader.ReadPackedInt32());
                break;

            case CustomRPC.GuesserShoot:
                Guesser.guesserShoot(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.LawyerSetTarget:
                RPCProcedure.lawyerSetTarget(reader.ReadByte());
                break;

            case CustomRPC.LawyerPromotesToPursuer:
                Lawyer.PromotesToPursuer(reader.ReadBoolean());
                break;

            case CustomRPC.ExecutionerSetTarget:
                RPCProcedure.executionerSetTarget(reader.ReadByte());
                break;

            case CustomRPC.ExecutionerPromotesRole:
                Executioner.PromotesRole();
                break;

            case CustomRPC.PursuerSetBlanked:
                var pid = reader.ReadByte();
                var blankedValue = reader.ReadByte();
                RPCProcedure.pursuerSetBlanked(pid, blankedValue);
                break;

            case CustomRPC.GiveBomb:
                RPCProcedure.giveBomb(reader.ReadByte(), reader.ReadBoolean());
                break;

            case CustomRPC.SetFutureSpelled:
                RPCProcedure.setFutureSpelled(reader.ReadByte());
                break;

            case CustomRPC.Bloody:
                var bloodyKiller = reader.ReadByte();
                var bloodyDead = reader.ReadByte();
                RPCProcedure.bloody(bloodyKiller, bloodyDead);
                break;

            case CustomRPC.SetFirstKill:
                var firstKill = reader.ReadByte();
                RPCProcedure.setFirstKill(firstKill);
                break;

            case CustomRPC.SetMeetingChatOverlay:
                var targetPlayerId = reader.ReadByte();
                var localPlayerId = reader.ReadByte();
                RPCProcedure.setChatNotificationOverlay(localPlayerId, targetPlayerId);
                break;

            case CustomRPC.ShowBodyGuardFlash:
                RPCProcedure.showBodyGuardFlash();
                break;

            case CustomRPC.SetInvisible:
                var invisiblePlayer = reader.ReadByte();
                var invisibleFlag = reader.ReadByte();
                RPCProcedure.setInvisible(invisiblePlayer, invisibleFlag);
                break;

            case CustomRPC.SetSwoop:
                var invisiblePlayer2 = reader.ReadByte();
                var invisibleFlag2 = reader.ReadByte();
                RPCProcedure.setSwoop(invisiblePlayer2, invisibleFlag2);
                break;

            case CustomRPC.SetJackalSwoop:
                var invisiblePlayer3 = reader.ReadByte();
                var invisibleFlag3 = reader.ReadByte();
                RPCProcedure.setJackalSwoop(invisiblePlayer3, invisibleFlag3);
                break;

            case CustomRPC.SetInvisibleGen:
                var invisiblePlayer4 = reader.ReadByte();
                var invisibleFlag4 = reader.ReadByte();
                RPCProcedure.setInvisibleGen(invisiblePlayer4, invisibleFlag4);
                break;

            case CustomRPC.Mine:
                var newVentId = reader.ReadInt32();
                var pos = reader.ReadBytesAndSize();
                var zAxis = reader.ReadSingle();
                RPCProcedure.Mine(newVentId, pos, zAxis);
                break;

            case CustomRPC.TurnToImpostor:
                RPCProcedure.turnToImpostor(reader.ReadByte());
                break;

            case CustomRPC.ThiefStealsRole:
                var thiefTargetId = reader.ReadByte();
                Thief.StealsRole(thiefTargetId);
                break;

            case CustomRPC.SetTrap:
                RPCProcedure.setTrap(reader.ReadBytesAndSize());
                break;

            case CustomRPC.TriggerTrap:
                var trappedPlayer = reader.ReadByte();
                var trapId = reader.ReadByte();
                RPCProcedure.triggerTrap(trappedPlayer, trapId);
                break;

            case CustomRPC.PlaceBomb:
                RPCProcedure.placeBomb(reader.ReadBytesAndSize());
                break;

            case CustomRPC.DefuseBomb:
                RPCProcedure.defuseBomb();
                break;

            case CustomRPC.ShareGameMode:
                var gm = reader.ReadByte();
                RPCProcedure.shareGameMode(gm);
                break;
            case CustomRPC.AkujoSetHonmei:
                RPCProcedure.akujoSetHonmei(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.AkujoSetKeep:
                RPCProcedure.akujoSetKeep(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.AkujoSuicide:
                RPCProcedure.akujoSuicide(reader.ReadByte());
                break;
            case CustomRPC.StopStart:
                RPCProcedure.stopStart(reader.ReadByte());
                break;

            // Game mode
            case CustomRPC.SetGuesserGm:
                var guesserGm = reader.ReadByte();
                RPCProcedure.setGuesserGm(guesserGm);
                break;
            case CustomRPC.ShareGhostInfo:
                RPCProcedure.receiveGhostInfo(reader.ReadByte(), reader);
                break;
            case CustomRPC.MayorMeeting:
                if (AmongUsClient.Instance.AmHost)
                {
                    MeetingRoomManager.Instance.reporter = Mayor.mayor;
                    MeetingRoomManager.Instance.target = null;
                    AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                    DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(Mayor.mayor);
                    Mayor.mayor.RpcStartMeeting(null);
                }
                break;
            case CustomRPC.BarryMeeting:
                if (AmongUsClient.Instance.AmHost)
                {
                    MeetingRoomManager.Instance.reporter = ButtonBarry.buttonBarry;
                    MeetingRoomManager.Instance.target = null;
                    AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                    DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(ButtonBarry.buttonBarry);
                    ButtonBarry.buttonBarry.RpcStartMeeting(null);
                }
                break;
            case CustomRPC.ProphetExamine:
                RPCProcedure.prophetExamine(reader.ReadByte());
                break;
            case CustomRPC.YoyoMarkLocation:
                RPCProcedure.yoyoMarkLocation(reader.ReadBytesAndSize());
                break;
            case CustomRPC.GrenadierFlash:
                RPCProcedure.grenadierFlash(reader.ReadBoolean());
                break;
            case CustomRPC.WitnessReport:
                Witness.WitnessReport(reader.ReadByte());
                break;
            case CustomRPC.WitnessSetTarget:
                Witness.target = playerById(reader.ReadByte());
                break;
            case CustomRPC.WolfLordkilled:
                WolfLord.WolfLordkilled(reader.ReadByte());
                break;
            case CustomRPC.YoyoBlink:
                RPCProcedure.yoyoBlink(reader.ReadByte() == byte.MaxValue, reader.ReadBytesAndSize());
                break;
            case CustomRPC.SetFutureReveal:
                break;
            case CustomRPC.TrapperKill:
                RPCProcedure.trapperKill(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.PlaceTrap:
                RPCProcedure.placeTrap(reader.ReadBytesAndSize());
                break;
            case CustomRPC.ClearTrap:
                RPCProcedure.clearTrap();
                break;
            case CustomRPC.ActivateTrap:
                RPCProcedure.activateTrap(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.DisableTrap:
                RPCProcedure.disableTrap(reader.ReadByte());
                break;
            case CustomRPC.TrapperMeetingFlag:
                RPCProcedure.trapperMeetingFlag();
                break;
            case CustomRPC.Prosecute:
                Prosecutor.ProsecuteThisMeeting = true;
                break;

            case CustomRPC.MayorRevealed:
                Mayor.Revealed = true;
                if (Guesser.guesserUI != null) Guesser.guesserUIExitButton.OnClick.Invoke();
                break;

            case CustomRPC.SurvivorVestActive:
                RPCProcedure.survivorVestActive();
                break;

            case CustomRPC.JackalCanSwooper:
                RPCProcedure.jackalCanSwooper(reader.ReadByte() == byte.MaxValue);
                break;

            case CustomRPC.InfoSleuthTarget:
                RPCProcedure.infoSleuthTarget(reader.ReadByte());
                break;

            case CustomRPC.InfoSleuthNoTarget:
                RPCProcedure.infoSleuthNoTarget();
                break;

            case CustomRPC.BalancerBalance:
                RPCProcedure.balancerBalance(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.HostEndGame:
                isCanceled = true;
                break;

            case CustomRPC.HostRevive:
                RPCProcedure.hostRevive(reader.ReadByte());
                break;
            case CustomRPC.HostKill:
                RPCProcedure.hostKill(reader.ReadByte());
                break;
        }

        return false;
    }
}
