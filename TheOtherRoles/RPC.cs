#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using MS.Internal.Xml.XPath;
using PowerTools;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using TheOtherRoles.Objects.Map;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.HudManagerStartPatch;
using static TheOtherRoles.Options.MapOption;
using static TheOtherRoles.Roles.RoleClass;
using Object = UnityEngine.Object;

namespace TheOtherRoles;

public enum RoleId
{
    Impostor,
    Morphling,
    Bomber,
    Poucher,
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
    Cultist,
    Cleaner,
    Terrorist,
    Blackmailer,
    Witch,
    Ninja,
    Yoyo,
    EvilTrapper,
    Follower,

    Survivor,
    Amnisiac,
    Jester,
    Vulture,
    Lawyer,
    Executioner,
    Pursuer,
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


    Crew,
    Crewmate,
    NiceGuesser,
    Mayor,
    Prosecutor,
    Portalmaker,
    Engineer,
    PrivateInvestigator,
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
    Spy,
    SecurityGuard,
    Medium,
    Trapper,
    Prophet,
    Magician,

    // Modifier ---
    Lover,
    EvilGuesser,
    Disperser,
    PoucherModifier,
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
}

public enum CustomRPC
{
    // Main Controls

    ResetVaribles = 60,
    ShareOptions,
    ForceEnd,
    WorkaroundSetRoles,
    SetRole,
    SetModifier,
    VersionHandshake,
    UseUncheckedVent,
    UncheckedMurderPlayer,
    UncheckedCmdReportDeadBody,
    UncheckedExilePlayer,
    DynamicMapOption,
    SetGameStarting,
    ShareGamemode,
    VersionHandshakeEx,
    StopStart,

    // Role functionality

    EngineerFixLights = 101,
    EngineerFixSubmergedOxygen,
    EngineerUsedRepair,
    CleanBody,
    Mine,
    ShowIndomitableFlash,
    DragBody,
    DropBody,
    MedicSetShielded,
    ShowBodyGuardFlash,
    ShowCultistFlash,
    ShowFollowerFlash,
    ShieldedMurderAttempt,
    TimeMasterShield,
    TimeMasterRewindTime,
    TurnToImpostor,
    BodyGuardGuardPlayer,
    PrivateInvestigatorWatchPlayer,
    PrivateInvestigatorWatchFlash,
    VeteranAlert,
    VeteranKill,
    ShifterShift,
    SwapperSwap,
    MorphlingMorph,
    CamouflagerCamouflage,
    DoomsayerMeeting,
    AkujoSetHonmei,
    AkujoSetKeep,
    AkujoSuicide,
    MayorMeeting,
    BarryMeeting,
    ProphetExamine,
    ImpostorPromotesToLastImpostor,

    //CamoComms,
    TrackerUsedTracker = 150,
    VampireSetBitten,
    PlaceGarlic,
    GiveBomb,
    DeputyUsedHandcuffs,
    DeputyPromotes,
    JackalCreatesSidekick,
    PavlovsCreateDog,
    SidekickPromotes,
    ErasePlayerRoles,
    SetFutureErased,
    SetFutureReveal,
    SetFutureShifted,
    Disperse,
    SetFutureShielded,
    SetFutureSpelled,
    PlaceNinjaTrace,
    PlacePortal,
    AmnisiacTakeRole,
    MimicMimicRole,
    UsePortal,
    CultistCreateImposter,
    TurnToCrewmate,
    PlaceJackInTheBox,
    LightsOut,
    PlaceCamera,
    SealVent,
    ArsonistWin,
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
    SetPosition,
    SetPositionESC,
    SetTiebreak,
    SetInvisibleGen,
    SetSwoop,
    SetJackalSwoop,
    JackalCanSwooper,

    TrapperKill,
    PlaceTrap,
    ClearTrap,
    ActivateTrap,
    DisableTrap,
    TrapperMeetingFlag,
    Prosecute,
    MayorRevealed,
    SurvivorVestActive,

    // SetSwooper,
    SetInvisible,
    ThiefStealsRole,
    SetTrap,
    TriggerTrap,
    PlaceBomb,
    DefuseBomb,
    //ShareRoom,
    YoyoMarkLocation,
    YoyoBlink,

    // Gamemode
    SetGuesserGm,
    HuntedShield,
    HuntedRewindTime,
    SetProp,
    SetRevealed,
    PropHuntStartTimer,
    PropHuntSetInvis,
    PropHuntSetSpeedboost,
    HostEndGame,

    // Other functionality
    ShareTimer,
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
        MediumInfo,
        BlankUsed,
        DetectiveOrMedicInfo,
        VampireTimer,
        DeathReasonAndKiller
    }

    // Main Controls

    public static void resetVariables()
    {
        Garlic.clearGarlics();
        JackInTheBox.clearJackInTheBoxes();
        NinjaTrace.clearTraces();
        AdditionalVents.clearAndReload();
        Portal.clearPortals();
        Bloodytrail.resetSprites();
        Trap.clearTraps();
        Silhouette.clearSilhouettes();
        ElectricPatch.Reset();
        clearAndReloadMapOptions();
        clearAndReloadRoles();
        clearGameHistory();
        setCustomButtonCooldowns();
        reloadPluginOptions();
        toggleZoom(true);
        GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 0;
        SurveillanceMinigamePatch.nightVisionOverlays = null;
        MapBehaviourPatch.clearAndReload();
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
                option.updateSelection((int)selection);
            }
        }
        catch (Exception e)
        {
            Error("Error while deserializing options: " + e.Message);
        }
    }

    public static void forceEnd()
    {
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started) return;
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
            if (!player.Data.Role.IsImpostor)
            {
                GameData.Instance
                    .GetPlayerById(player
                        .PlayerId); // player.RemoveInfected(); (was removed in 2022.12.08, no idea if we ever need that part again, replaced by these 2 lines.) 
                player.SetRole(RoleTypes.Crewmate);

                player.MurderPlayer(player);
                player.Data.IsDead = true;
            }
    }

    public static void shareGameMode(byte gm)
    {
        gameMode = (CustomGamemodes)gm;
    }
    public static void stopStart(byte playerId)
    {
        if (AmongUsClient.Instance.AmHost && CustomOptionHolder.anyPlayerCanStopStart.getBool())
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
                    case RoleId.Crew:
                        Crew.crew = player;
                        break;
                    case RoleId.Werewolf:
                        Werewolf.werewolf = player;
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
                    case RoleId.Magician:
                        Magician.magician = player;
                        break;
                    case RoleId.TimeMaster:
                        TimeMaster.timeMaster = player;
                        break;
                    case RoleId.Amnisiac:
                        Amnisiac.amnisiac = player;
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
                        Jackal.jackal = player;
                        break;
                    case RoleId.Sidekick:
                        Sidekick.sidekick = player;
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
                    case RoleId.Follower:
                        Follower.follower = player;
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
                    case RoleId.PrivateInvestigator:
                        PrivateInvestigator.privateInvestigator = player;
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
                    case RoleId.NiceGuesser:
                        Guesser.niceGuesser = player;
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
                        Pursuer.pursuer.Add(player);
                        break;
                    case RoleId.Survivor:
                        Survivor.survivor.Add(player);
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
                    case RoleId.Escapist:
                        Escapist.escapist = player;
                        break;
                    case RoleId.Cultist:
                        Cultist.cultist = player;
                        var impostors = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid())
                            .ToList();
                        impostors.RemoveAll(x => !x.Data.Role.IsImpostor);
                        Helpers.turnToCrewmate(impostors, player);
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
                }
            }
            if (AmongUsClient.Instance.AmHost && Helpers.roleCanUseVents(player) && !player.Data.Role.IsImpostor)
            {
                player.RpcSetRole(RoleTypes.Engineer);
                player.SetRole(RoleTypes.Engineer);
            }
        }
    }

    public static void setModifier(byte modifierId, byte playerId, byte flag)
    {
        var player = playerById(playerId);
        switch ((RoleId)modifierId)
        {
            case RoleId.EvilGuesser:
                Guesser.evilGuesser.Add(player);
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

    public static void turnToCrewmate(byte targetId)
    {
        var player = playerById(targetId);
        if (player == null) return;
        player.Data.Role.TeamType = RoleTeamTypes.Crewmate;
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
        erasePlayerRoles(player.PlayerId);
        if (player.PlayerId == CachedPlayer.LocalPlayer.PlayerId)
            CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
        setRole((byte)RoleId.Crew, targetId);
        //   player.Data.Role.IsImpostor = false;
    }

    public static void setGameStarting()
    {
        GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 5f;
    }

    // Role functionality

    public static void engineerFixLights()
    {
        var switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
        switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
    }

    public static void engineerFixSubmergedOxygen()
    {
        SubmergedCompatibility.RepairOxygen();
    }

    public static void engineerUsedRepair()
    {
        Engineer.usedFix = true;
        Engineer.remainingFixes--;
        /*
        if (Helpers.shouldShowGhostInfo())
        {
            Helpers.showFlash(Engineer.color, 0.5f, "Engineer Fix");
        }*/
    }

    public static void showIndomitableFlash()
    {
        if (Indomitable.indomitable == CachedPlayer.LocalPlayer.PlayerControl) showFlash(Indomitable.color);
    }

    public static void cleanBody(byte playerId, byte cleaningPlayerId)
    {
        if (Medium.futureDeadBodies != null)
        {
            var deadBody = Medium.futureDeadBodies.Find(x => x.Item1.player.PlayerId == playerId)?.Item1;
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
        PlayerControl player = playerById(targetId);
        LastImpostor.lastImpostor = player;
    }

    public static void amnisiacTakeRole(byte targetId)
    {
        var target = playerById(targetId);
        var amnisiac = Amnisiac.amnisiac;
        if (target == null || amnisiac == null) return;
        var targetInfo = RoleInfo.getRoleInfoForPlayer(target);
        var roleInfo = targetInfo.FirstOrDefault(info => !info.isModifier);
        switch (roleInfo!.roleId)
        {
            case RoleId.Crewmate:
                Amnisiac.clearAndReload();
                break;
            case RoleId.Impostor:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                Amnisiac.clearAndReload();
                break;
            case RoleId.Jester:
                if (Amnisiac.resetRole) Jester.clearAndReload();
                Jester.jester = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;

            case RoleId.BodyGuard:
                if (Amnisiac.resetRole) BodyGuard.clearAndReload();
                BodyGuard.bodyguard = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Werewolf:
                if (Amnisiac.resetRole) Werewolf.clearAndReload();
                Werewolf.werewolf = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;

            case RoleId.Prosecutor:
                Prosecutor.prosecutor = target;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Mayor:
                if (Amnisiac.resetRole) Mayor.clearAndReload();
                Mayor.mayor = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Portalmaker:
                if (Amnisiac.resetRole) Portalmaker.clearAndReload();
                Portalmaker.portalmaker = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Engineer:
                if (Amnisiac.resetRole) Engineer.clearAndReload();
                Engineer.engineer = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Magician:
                if (Amnisiac.resetRole) Magician.clearAndReload();
                Magician.magician = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Sheriff:
                // Never reload Sheriff
                if (Sheriff.formerDeputy != null && Sheriff.formerDeputy == Sheriff.sheriff)
                {
                    Sheriff.formerDeputy = null;
                    Deputy.deputy = amnisiac;
                }
                else Sheriff.sheriff = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Deputy:
                if (Amnisiac.resetRole) Deputy.clearAndReload();
                Deputy.deputy = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Poucher:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Poucher.clearAndReload();
                Amnisiac.clearAndReload();
                break;

            case RoleId.Cultist:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Cultist.clearAndReload();
                Cultist.cultist = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Detective:
                if (Amnisiac.resetRole) Detective.clearAndReload();
                Detective.detective = amnisiac;
                Amnisiac.clearAndReload();
                break;
                
            case RoleId.TimeMaster:
                if (Amnisiac.resetRole) TimeMaster.clearAndReload();
                TimeMaster.timeMaster = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Veteran:
                if (Amnisiac.resetRole) Veteran.clearAndReload();
                Veteran.veteran = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Medic:
                if (Amnisiac.resetRole) Medic.clearAndReload();
                Medic.medic = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Shifter:
                if (Amnisiac.resetRole) Shifter.clearAndReload();
                Shifter.shifter = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Swapper:
                if (Amnisiac.resetRole) Swapper.clearAndReload();
                Swapper.swapper = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Seer:
                if (Amnisiac.resetRole) Seer.clearAndReload();
                Seer.seer = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Morphling:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Morphling.clearAndReload();
                Morphling.morphling = amnisiac;
                Amnisiac.clearAndReload();
                break;
            case RoleId.Bomber:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Bomber.clearAndReload();
                Bomber.bomber = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Yoyo:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Yoyo.clearAndReload();
                Yoyo.yoyo = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Terrorist:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Terrorist.clearAndReload();
                Terrorist.terrorist = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Camouflager:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Camouflager.clearAndReload();
                Camouflager.camouflager = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Hacker:
                if (Amnisiac.resetRole) Hacker.clearAndReload();
                Hacker.hacker = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Tracker:
                if (Amnisiac.resetRole) Tracker.clearAndReload();
                Tracker.tracker = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Vampire:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Vampire.clearAndReload();
                Vampire.vampire = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Follower:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Follower.clearAndReload();
                Follower.follower = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Snitch:
                if (Amnisiac.resetRole) Snitch.clearAndReload();
                Snitch.snitch = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Jackal:
                Jackal.jackal = amnisiac;
                Jackal.formerJackals.Add(target);
                Amnisiac.clearAndReload();
                break;

            case RoleId.Sidekick:
                Jackal.formerJackals.Add(target);
                if (Amnisiac.resetRole) Sidekick.clearAndReload();
                Sidekick.sidekick = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Pavlovsowner:
                Pavlovsdogs.pavlovsdogs.Add(Pavlovsdogs.pavlovsowner);
                Pavlovsdogs.pavlovsowner = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Pavlovsdogs:
                Pavlovsdogs.pavlovsdogs.Add(amnisiac);
                Amnisiac.clearAndReload();
                break;

            case RoleId.Eraser:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Eraser.clearAndReload();
                Eraser.eraser = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Spy:
                if (Amnisiac.resetRole) Spy.clearAndReload();
                Spy.spy = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Trickster:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Trickster.clearAndReload();
                Trickster.trickster = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Mimic:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Mimic.clearAndReload(false);
                Mimic.mimic = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Cleaner:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Cleaner.clearAndReload();
                Cleaner.cleaner = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Warlock:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Warlock.clearAndReload();
                Warlock.warlock = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.SecurityGuard:
                if (Amnisiac.resetRole) SecurityGuard.clearAndReload();
                SecurityGuard.securityGuard = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Survivor:
                Survivor.survivor.Add(amnisiac);
                Amnisiac.clearAndReload();
                break;

            case RoleId.Arsonist:
                if (Amnisiac.resetRole) Arsonist.clearAndReload();
                Arsonist.arsonist = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;

                if (CachedPlayer.LocalPlayer.PlayerControl == Arsonist.arsonist)
                {
                    var playerCounter = 0;
                    var bottomLeft = new Vector3(
                        -FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x,
                        FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y,
                        FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z);
                    foreach (PlayerControl p in CachedPlayer.AllPlayers)
                        if (playerIcons.ContainsKey(p.PlayerId) && p != Arsonist.arsonist)
                        {
                            //Arsonist.poolIcons.Add(p);
                            if (Arsonist.dousedPlayers.Contains(p))
                                playerIcons[p.PlayerId].setSemiTransparent(false);
                            else
                                playerIcons[p.PlayerId].setSemiTransparent(true);

                            playerIcons[p.PlayerId].transform.localPosition = bottomLeft +
                                                                              new Vector3(-0.25f, -0.25f, 0) +
                                                                              (Vector3.right * playerCounter++ * 0.35f);
                            playerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.2f;
                            playerIcons[p.PlayerId].gameObject.SetActive(true);
                        }
                }

                break;

            case RoleId.EvilGuesser:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                // Never Reload Guesser
                Guesser.evilGuesser.Add(amnisiac);
                Amnisiac.clearAndReload();
                break;

            case RoleId.NiceGuesser:
                // Never Reload Guesser
                Guesser.niceGuesser = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.BountyHunter:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) BountyHunter.clearAndReload();
                BountyHunter.bountyHunter = amnisiac;
                Amnisiac.clearAndReload();

                BountyHunter.bountyUpdateTimer = 0f;
                if (CachedPlayer.LocalPlayer.PlayerControl == BountyHunter.bountyHunter)
                {
                    var bottomLeft =
                        new Vector3(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x,
                            FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y,
                            FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z) +
                        new Vector3(-0.25f, 1f, 0);
                    BountyHunter.cooldownText =
                        Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
                            FastDestroyableSingleton<HudManager>.Instance.transform);
                    BountyHunter.cooldownText.alignment = TextAlignmentOptions.Center;
                    BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -1f, -1f);
                    BountyHunter.cooldownText.gameObject.SetActive(true);

                    foreach (PlayerControl p in CachedPlayer.AllPlayers)
                        if (playerIcons.ContainsKey(p.PlayerId))
                        {
                            playerIcons[p.PlayerId].setSemiTransparent(false);
                            playerIcons[p.PlayerId].transform.localPosition = bottomLeft + new Vector3(0f, -1f, 0);
                            playerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.4f;
                            playerIcons[p.PlayerId].gameObject.SetActive(false);
                        }
                }

                break;

            case RoleId.Vulture:
                if (Amnisiac.resetRole) Vulture.clearAndReload();
                Vulture.vulture = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;

            case RoleId.Executioner:
                Executioner.executioner = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;

            case RoleId.Medium:
                if (Amnisiac.resetRole) Medium.clearAndReload();
                Medium.medium = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.PrivateInvestigator:
                if (Amnisiac.resetRole) PrivateInvestigator.clearAndReload();
                PrivateInvestigator.privateInvestigator = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Lawyer:
                // Never reset Lawyer
                Lawyer.lawyer = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;

            case RoleId.Pursuer:
                if (Amnisiac.resetRole) Pursuer.clearAndReload();
                Pursuer.pursuer.Add(amnisiac);
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;

            case RoleId.Witch:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Witch.clearAndReload();
                Witch.witch = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Jumper:
                if (Amnisiac.resetRole) Jumper.clearAndReload();
                Jumper.jumper = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Escapist:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Escapist.clearAndReload();
                Escapist.escapist = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Thief:
                if (Amnisiac.resetRole) Thief.clearAndReload();
                Thief.thief = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;

            case RoleId.Trapper:
                if (Amnisiac.resetRole) Trapper.clearAndReload();
                Trapper.trapper = amnisiac;
                Amnisiac.clearAndReload();
                break;
            case RoleId.Juggernaut:
                if (Amnisiac.resetRole) Juggernaut.clearAndReload();
                Juggernaut.juggernaut = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;
            case RoleId.Doomsayer:
                if (Amnisiac.resetRole) Doomsayer.clearAndReload();
                Doomsayer.doomsayer = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;
            case RoleId.Swooper:
                if (Amnisiac.resetRole) Swooper.clearAndReload();
                Swooper.swooper = amnisiac;
                Amnisiac.clearAndReload();
                Amnisiac.amnisiac = target;
                break;
            case RoleId.Ninja:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Ninja.clearAndReload();
                Ninja.ninja = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Akujo:
                Helpers.turnToImpostor(Akujo.akujo);
                if (Amnisiac.resetRole) Mimic.clearAndReload();
                Akujo.akujo = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Blackmailer:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Blackmailer.clearAndReload();
                Blackmailer.blackmailer = amnisiac;
                Amnisiac.clearAndReload();
                break;

            case RoleId.Miner:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Miner.clearAndReload();
                Miner.miner = amnisiac;
                Amnisiac.clearAndReload();
                break;
            case RoleId.Undertaker:
                Helpers.turnToImpostor(Amnisiac.amnisiac);
                if (Amnisiac.resetRole) Undertaker.clearAndReload();
                Undertaker.undertaker = amnisiac;
                Amnisiac.clearAndReload();
                break;
            case RoleId.Prophet:
                if (Amnisiac.resetRole) Prophet.clearAndReload();
                Prophet.prophet = amnisiac;
                Amnisiac.clearAndReload();
                break;
            case RoleId.EvilTrapper:
                if (Amnisiac.resetRole) EvilTrapper.clearAndReload();
                EvilTrapper.evilTrapper = amnisiac;
                Amnisiac.clearAndReload();
                break;
        }
    }

    public static void mimicMimicRole(byte targetId)
    {
        var target = playerById(targetId);
        if (target == null || Mimic.mimic == null) return;
        var targetInfo = RoleInfo.getRoleInfoForPlayer(target);
        var roleInfo = targetInfo.FirstOrDefault(info => !info.isModifier);
        switch (roleInfo!.roleId)
        {
            case RoleId.BodyGuard:
                if (Amnisiac.resetRole) BodyGuard.clearAndReload();
                BodyGuard.bodyguard = Mimic.mimic;
                bodyGuardGuardButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Mayor:
                if (Amnisiac.resetRole) Mayor.clearAndReload();
                Mayor.mayor = Mimic.mimic;
                mayorMeetingButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;

                Mimic.hasMimic = true;
                break;

            case RoleId.Prosecutor:
                if (Amnisiac.resetRole) Prosecutor.clearAndReload();
                Prosecutor.prosecutor = Mimic.mimic;
                Prosecutor.diesOnIncorrectPros = false;
                Mimic.hasMimic = true;
                break;

            case RoleId.Trapper:
                if (Amnisiac.resetRole) Trapper.clearAndReload();
                Trapper.trapper = Mimic.mimic;
                trapperButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Portalmaker:
                if (Amnisiac.resetRole) Portalmaker.clearAndReload();
                Portalmaker.portalmaker = Mimic.mimic;
                portalmakerPlacePortalButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Engineer:
                if (Amnisiac.resetRole) Engineer.clearAndReload();
                Engineer.engineer = Mimic.mimic;
                engineerRepairButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Jumper:
                if (Amnisiac.resetRole) Jumper.clearAndReload();
                Jumper.jumper = Mimic.mimic;
                jumperButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Detective:
                if (Amnisiac.resetRole) Detective.clearAndReload();
                Detective.detective = Mimic.mimic;
                Mimic.hasMimic = true;
                break;
            /*
        case RoleId.NiceGuesser:
            if (Amnisiac.resetRole) //Guesser.clearAndReload();
                Guesser.niceGuesser = Mimic.mimic;
            Mimic.hasMimic = true;
            break;
            */
            case RoleId.TimeMaster:
                if (Amnisiac.resetRole) TimeMaster.clearAndReload();
                TimeMaster.timeMaster = Mimic.mimic;
                timeMasterShieldButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Veteran:
                if (Amnisiac.resetRole) Veteran.clearAndReload();
                Veteran.veteran = Mimic.mimic;
                veteranAlertButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Medic:
                if (Amnisiac.resetRole) Medic.clearAndReload();
                Medic.medic = Mimic.mimic;
                medicShieldButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Swapper:
                if (Amnisiac.resetRole) Swapper.clearAndReload();
                Swapper.swapper = Mimic.mimic;
                Mimic.hasMimic = true;
                break;

            case RoleId.Seer:
                if (Amnisiac.resetRole) Seer.clearAndReload();
                Seer.seer = Mimic.mimic;
                Mimic.hasMimic = true;
                break;

            case RoleId.Hacker:
                if (Amnisiac.resetRole) Hacker.clearAndReload();
                Hacker.hacker = Mimic.mimic;
                hackerAdminTableButton.PositionOffset = CustomButton.ButtonPositions.upperRowFarLeft;
                hackerVitalsButton.PositionOffset = CustomButton.ButtonPositions.lowerRowFarLeft;
                hackerButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Tracker:
                if (Amnisiac.resetRole) Tracker.clearAndReload();
                Tracker.tracker = Mimic.mimic;
                trackerTrackPlayerButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.SecurityGuard:
                if (Amnisiac.resetRole) SecurityGuard.clearAndReload();
                SecurityGuard.securityGuard = Mimic.mimic;
                securityGuardButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                securityGuardCamButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Medium:
                if (Amnisiac.resetRole) Medium.clearAndReload();
                Medium.medium = Mimic.mimic;
                mediumButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;

            case RoleId.Prophet:
                if (Amnisiac.resetRole) Prophet.clearAndReload();
                Prophet.prophet = Mimic.mimic;
                prophetButton.PositionOffset = CustomButton.ButtonPositions.upperRowLeft;
                Mimic.hasMimic = true;
                break;
        }
    }

    public static void cultistCreateImposter(byte targetId)
    {
        var player = playerById(targetId);
        if (player == null) return;

        erasePlayerRoles(player.PlayerId);


        Helpers.turnToImpostor(player);
        Follower.follower = player;
        Cultist.needsFollower = false;

        if (Follower.getsAssassin) Guesser.evilGuesser.Add(player);
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
            showFlash(Palette.ImpostorRed, 1.5f, getString("medicShowAttemptText"));
    }

    public static void aftermathDead(byte playerId, byte killerId)
    {
        var player = playerById(playerId);
        var killer = playerById(killerId);
        if (killer == null || killer == player) return;

        if (Blackmailer.blackmailer == killer)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.BlackmailPlayer, SendOption.Reliable);
            writer.Write(killerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            blackmailPlayer(killerId);
            blackmailerButton.Timer = blackmailerButton.MaxTimer;
        }
        else if (Bomber.bomber == killer)
        {
            var bombWriter = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.GiveBomb, SendOption.Reliable);
            bombWriter.Write(killerId);
            AmongUsClient.Instance.FinishRpcImmediately(bombWriter);
            giveBomb(killerId);
            bomberBombButton.Timer = bomberBombButton.MaxTimer;
        }
        else if (Terrorist.terrorist == killer)
        {
            if (checkMuderAttempt(Terrorist.terrorist, Terrorist.terrorist) != MurderAttemptResult.BlankKill)
            {
                var pos = killer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
                var writer = AmongUsClient.Instance.StartRpc(killer.NetId, (byte)CustomRPC.PlaceBomb);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                placeBomb(buff);
                SoundEffectsManager.play(Terrorist.selfExplosion ? "bombExplosion" : "trapperTrap");

                if (Terrorist.selfExplosion)
                {
                    var loacl = Terrorist.terrorist.PlayerId;
                    var writer1 = AmongUsClient.Instance.StartRpcImmediately(Terrorist.terrorist.NetId,
                        (byte)CustomRPC.UncheckedMurderPlayer, SendOption.Reliable);
                    writer1.Write(loacl);
                    writer1.Write(loacl);
                    writer1.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer1);
                    uncheckedMurderPlayer(loacl, loacl, byte.MaxValue);
                }
            }
            terroristButton.Timer = terroristButton.MaxTimer;
        }
        else if (Morphling.morphling)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.MorphlingMorph, SendOption.Reliable);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            morphlingMorph(player.PlayerId);
            Morphling.sampledTarget = null;
            morphlingButton.Timer = Morphling.duration;
            SoundEffectsManager.play("morphlingMorph");
        }
        else if (Witch.witch == killer)
        {
            var target = killer;
            if (Witch.currentTarget != null) target = Witch.currentTarget;
            Witch.spellCastingTarget = target;
            SoundEffectsManager.play("witchSpell");
            witchSpellButton.Timer = witchSpellButton.MaxTimer;
        }/*
        else if (Warlock.warlock == killer)
        {

        }*/
        else if (Miner.miner == killer)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.Mine, SendOption.Reliable);
            var pos = killer.transform.position;
            var buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
            var id = getAvailableId();
            writer.Write(id);
            writer.Write(killer.PlayerId);
            writer.WriteBytesAndSize(buff);
            writer.Write(0.01f);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Mine(id, Miner.miner, buff, 0.01f);
            minerMineButton.Timer = minerMineButton.MaxTimer;
        }
        else if (Escapist.escapist == killer)
        {
            if (Escapist.escapeLocation != Vector3.zero)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                    (byte)CustomRPC.SetPositionESC, SendOption.Reliable);
                writer.Write(killer.PlayerId);
                writer.Write(Escapist.escapeLocation.x);
                writer.Write(Escapist.escapeLocation.y);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                killer.NetTransform.RpcSnapTo(Escapist.escapeLocation);
            }
            else
            {
                Escapist.escapeLocation = PlayerControl.LocalPlayer.transform.localPosition;
            }
            escapistButton.Timer = escapistButton.MaxTimer;
        }
        else if (Yoyo.yoyo == killer)
        {
            var pos = CachedPlayer.LocalPlayer.transform.position;
            byte[] buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

            if (Yoyo.markedLocation == null)
            {
                Message($"marked location is null in button press");
                var writer = AmongUsClient.Instance.StartRpc(killer.NetId, (byte)CustomRPC.YoyoMarkLocation, SendOption.Reliable);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                yoyoMarkLocation(buff);
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
                var writer = AmongUsClient.Instance.StartRpc(killer.NetId, (byte)CustomRPC.YoyoBlink, SendOption.Reliable);
                writer.Write(byte.MaxValue);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                yoyoBlink(true, buff);
                yoyoButton.EffectDuration = Yoyo.blinkDuration;
                yoyoButton.Timer = 10f;
                yoyoButton.HasEffect = true;
                yoyoButton.buttonText = "ReturningText".Translate();
                SoundEffectsManager.play("morphlingMorph");
            }
        }
        else if (EvilTrapper.evilTrapper == killer)
        {
            EvilTrapper.setTrap();
            evilTrapperSetTrapButton.Timer = evilTrapperSetTrapButton.MaxTimer;
        }
        else if (Trickster.trickster == killer)
        {
            if (!JackInTheBox.hasJackInTheBoxLimitReached())
            {
                var pos = CachedPlayer.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                var writer = AmongUsClient.Instance.StartRpc(killer.NetId,
                    (byte)CustomRPC.PlaceJackInTheBox);
                writer.WriteBytesAndSize(buff);
                writer.EndMessage();
                placeJackInTheBox(buff);
                SoundEffectsManager.play("tricksterPlaceBox");
                placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;
            }
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                    (byte)CustomRPC.LightsOut, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                lightsOut();
                SoundEffectsManager.play("lighterLight");
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
            }
        }
        else if (Undertaker.undertaker == killer)
        {
            if (Undertaker.deadBodyDraged == null)
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(),
                             CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
                {
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
                                    killer.NetId, (byte)CustomRPC.DragBody,
                                    SendOption.Reliable);
                                writer.Write(playerInfo.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                                dragBody(playerInfo.PlayerId);
                                Undertaker.deadBodyDraged = deadBody;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                    (byte)CustomRPC.DropBody, SendOption.Reliable);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Undertaker.deadBodyDraged = null;
            }
            undertakerDragButton.Timer = 2.5f;
        }
        else if (Cleaner.cleaner == killer)
        {
            foreach (var collider2D in Physics2D.OverlapCircleAll(
                CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(),
                CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
            {
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
                                killer.NetId, (byte)CustomRPC.CleanBody,
                                SendOption.Reliable);
                            writer.Write(playerInfo.PlayerId);
                            writer.Write(Cleaner.cleaner.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            cleanBody(playerInfo.PlayerId, Cleaner.cleaner.PlayerId);

                            Cleaner.cleaner.killTimer = cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
                            SoundEffectsManager.play("cleanerClean");
                            break;
                        }
                    }
                }
            }

            cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
        }
        else if (Eraser.eraser == killer)
        {
            var target = killer;
            if (Eraser.currentTarget != null) target = Eraser.currentTarget;
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.SetFutureErased, SendOption.Reliable);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            setFutureErased(target.PlayerId);
            SoundEffectsManager.play("eraserErase");
            eraserButton.Timer = eraserButton.MaxTimer;
        }
        else if (Camouflager.camouflager == killer)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.CamouflagerCamouflage, SendOption.Reliable);
            writer.Write(1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            camouflagerCamouflage(1);
            SoundEffectsManager.play("morphlingMorph");
            camouflagerButton.Timer = camouflagerButton.MaxTimer;
        }
        else if (Swooper.swooper == killer)
        {
            var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.SetSwoop, SendOption.Reliable, -1);
            invisibleWriter.Write(Swooper.swooper.PlayerId);
            invisibleWriter.Write(byte.MinValue);
            AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
            setSwoop(Swooper.swooper.PlayerId, byte.MinValue);
            swooperSwoopButton.Timer = swooperSwoopButton.MaxTimer + Swooper.duration;
        }
        else if (Jackal.jackal == killer && Jackal.canSwoop)
        {
            var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(killer.NetId,
                (byte)CustomRPC.SetJackalSwoop, SendOption.Reliable, -1);
            invisibleWriter.Write(Jackal.jackal.PlayerId);
            invisibleWriter.Write(byte.MinValue);
            AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
            setJackalSwoop(Jackal.jackal.PlayerId, byte.MinValue);
            jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer + Jackal.duration;
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
        if ((player.Data.Role.IsImpostor || isShiftNeutral(player)) && !oldShifter.Data.IsDead)
        {
            oldShifter.Exiled();
            overrideDeathReasonAndKiller(oldShifter, DeadPlayer.CustomDeathReason.Shift, player);
            if (oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                lawyerPromotesToPursuer();
            }
            else if (oldShifter == Executioner.target && AmongUsClient.Instance.AmHost && Executioner.executioner != null)
            {

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ExecutionerPromotesRole, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                executionerPromotesRole();
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
        if (isActiveCamoComms() && setTimer != 2) return;
        if (isCamoComms()) Camouflager.camoComms = true;
        if (Camouflager.camouflager == null && !Camouflager.camoComms) return;
        if (setTimer == 1) Camouflager.camouflageTimer = Camouflager.duration;
        if (MushroomSabotageActive()) return; // Dont overwrite the fungle "camo"
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
            player.setLook("", 6, "", "", "", "");
    }
    /*
            public static void camoComms() {
                if (!Helpers.isCamoComms()) return;


                if (Helpers.MushroomSabotageActive()) return; // Dont overwrite the fungle "camo"
                foreach (PlayerControl player in CachedPlayer.AllPlayers)
                    player.setLook("", 6, "", "", "", "");

            }
            */

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
                Pursuer.pursuer.Add(Executioner.executioner);
                Executioner.clearAndReload();
            }
        }

        var wasSpy = Spy.spy != null && player == Spy.spy;
        var wasImpostor = player.Data.Role.IsImpostor; // This can only be reached if impostors can be sidekicked.
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
        if (player == Lawyer.lawyer && Lawyer.target != null)
        {
            var playerInfoTransform = Lawyer.target.cosmetics.nameText.transform.parent.FindChild("Info");
            var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();
            if (playerInfo != null) playerInfo.text = "";
        }

        erasePlayerRoles(player.PlayerId);
        Sidekick.sidekick = player;
        if (player.PlayerId == CachedPlayer.LocalPlayer.PlayerId)
            CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
        if ((wasSpy || wasImpostor) && !Jackal.CanImpostorFindSidekick) Sidekick.wasTeamRed = true;
        Sidekick.wasSpy = wasSpy;
        Sidekick.wasImpostor = wasImpostor;
        if (player == CachedPlayer.LocalPlayer.PlayerControl) SoundEffectsManager.play("jackalSidekick");
        if (HandleGuesser.isGuesserGm && CustomOptionHolder.guesserGamemodeSidekickIsAlwaysGuesser.getBool() && !HandleGuesser.isGuesser(targetId))
            setGuesserGm(targetId);

        Jackal.canCreateSidekick = false;
    }

    public static void sidekickPromotes()
    {
        Jackal.removeCurrentJackal();
        Jackal.jackal = Sidekick.sidekick;
        Jackal.canCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick;
        Jackal.wasTeamRed = Sidekick.wasTeamRed;
        Jackal.wasSpy = Sidekick.wasSpy;
        Jackal.wasImpostor = Sidekick.wasImpostor;
        Sidekick.clearAndReload();
        return;
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
                Pursuer.pursuer.Add(Executioner.executioner);
                Executioner.clearAndReload();
            }
        }

        var wasSpy = Spy.spy != null && player == Spy.spy;
        var wasImpostor = player.Data.Role.IsImpostor; // This can only be reached if impostors can be sidekicked.
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
    }

    /// <summary>
    /// 抹除目标玩家的职业
    /// </summary>
    public static void erasePlayerRoles(byte playerId, bool ignoreModifier = true)
    {
        var player = playerById(playerId);
        if (player == null) return;

        // Crewmate roles
        if (Guesser.evilGuesser.Any(x => x.PlayerId == player.PlayerId))
            Guesser.evilGuesser.RemoveAll(x => x.PlayerId == player.PlayerId);
        if (player == Swooper.swooper) Swooper.clearAndReload();
        if (player == Mayor.mayor) Mayor.clearAndReload();
        if (player == Prosecutor.prosecutor) Prosecutor.clearAndReload();
        if (player == Portalmaker.portalmaker) Portalmaker.clearAndReload();
        if (player == Engineer.engineer) Engineer.clearAndReload();
        if (player == PrivateInvestigator.privateInvestigator) PrivateInvestigator.clearAndReload();
        if (player == Sheriff.sheriff) Sheriff.clearAndReload();
        if (player == Deputy.deputy) Deputy.clearAndReload(false);
        if (player == Detective.detective) Detective.clearAndReload();
        if (player == TimeMaster.timeMaster) TimeMaster.clearAndReload();
        if (player == Amnisiac.amnisiac) Amnisiac.clearAndReload();
        if (player == Veteran.veteran) Veteran.clearAndReload();
        if (player == Medic.medic) Medic.clearAndReload();
        if (player == Shifter.shifter) Shifter.clearAndReload();
        if (player == Seer.seer) Seer.clearAndReload();
        if (player == Hacker.hacker) Hacker.clearAndReload();
        if (player == BodyGuard.bodyguard) BodyGuard.clearAndReload();
        if (player == Tracker.tracker) Tracker.clearAndReload();
        if (player == Snitch.snitch) Snitch.clearAndReload();
        if (player == Swapper.swapper) Swapper.clearAndReload();
        if (player == Spy.spy) Spy.clearAndReload();
        if (player == SecurityGuard.securityGuard) SecurityGuard.clearAndReload();
        if (player == Medium.medium) Medium.clearAndReload();
        if (player == Jumper.jumper) Jumper.clearAndReload();
        if (player == Trapper.trapper) Trapper.clearAndReload();
        if (player == Prophet.prophet) Prophet.clearAndReload();

        // Impostor roles
        if (player == Morphling.morphling) Morphling.clearAndReload();
        if (player == Bomber.bomber) Bomber.clearAndReload();
        if (player == Camouflager.camouflager) Camouflager.clearAndReload();
        if (player == Poucher.poucher && !Poucher.spawnModifier) Poucher.clearAndReload();
        if (player == Vampire.vampire) Vampire.clearAndReload();
        if (player == Eraser.eraser) Eraser.clearAndReload();
        if (player == Cultist.cultist) Cultist.clearAndReload();
        if (player == Trickster.trickster) Trickster.clearAndReload();
        if (player == Cleaner.cleaner) Cleaner.clearAndReload();
        if (player == Undertaker.undertaker) Undertaker.clearAndReload();
        if (player == Mimic.mimic) Mimic.clearAndReload();
        if (player == Warlock.warlock) Warlock.clearAndReload();
        if (player == Witch.witch) Witch.clearAndReload();
        if (player == Escapist.escapist) Escapist.clearAndReload();
        if (player == Ninja.ninja) Ninja.clearAndReload();
        if (player == Yoyo.yoyo) Yoyo.clearAndReload();
        if (player == EvilTrapper.evilTrapper) EvilTrapper.clearAndReload();
        if (player == Blackmailer.blackmailer) Blackmailer.clearAndReload();
        if (player == Follower.follower) Follower.clearAndReload();
        if (player == Terrorist.terrorist) Terrorist.clearAndReload();
        if (player == Prophet.prophet) Prophet.clearAndReload();

        // Other roles
        if (player == Jester.jester) Jester.clearAndReload();
        if (player == Werewolf.werewolf) Werewolf.clearAndReload();
        if (player == Miner.miner) Miner.clearAndReload();
        if (player == Arsonist.arsonist) Arsonist.clearAndReload();
        if (Guesser.isGuesser(player.PlayerId)) Guesser.clear(player.PlayerId);
        if (player == Jackal.jackal)
        {
            // Promote Sidekick and hence override the the Jackal or erase Jackal
            if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead)
                sidekickPromotes();
            else
                Jackal.clearAndReload();
        }
        if (player == Pavlovsdogs.pavlovsowner) Pavlovsdogs.pavlovsowner = null;
        if (Pavlovsdogs.pavlovsdogs.Contains(player)) Pavlovsdogs.pavlovsdogs.Remove(player);
        if (player == Sidekick.sidekick) Sidekick.clearAndReload();
        if (player == BountyHunter.bountyHunter) BountyHunter.clearAndReload();
        if (player == Vulture.vulture) Vulture.clearAndReload();
        if (player == Executioner.executioner) Executioner.clearAndReload();
        if (player == Lawyer.lawyer) Lawyer.clearAndReload();
        if (Pursuer.pursuer.Contains(player)) Pursuer.pursuer.Remove(player);
        if (player == Thief.thief) Thief.clearAndReload();
        if (player == Juggernaut.juggernaut) Juggernaut.clearAndReload();
        if (player == Doomsayer.doomsayer) Doomsayer.clearAndReload();
        if (player == Akujo.akujo) Akujo.clearAndReload();

        // Modifier
        if (!ignoreModifier)
        {
            if (player == Lovers.lover1 || player == Lovers.lover2)
                Lovers.clearAndReload(); // The whole Lover couple is being erased
            if (Bait.bait.Any(x => x.PlayerId == player.PlayerId))
                Bait.bait.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (Bloody.bloody.Any(x => x.PlayerId == player.PlayerId))
                Bloody.bloody.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == player.PlayerId))
                AntiTeleport.antiTeleport.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (Sunglasses.sunglasses.Any(x => x.PlayerId == player.PlayerId))
                Sunglasses.sunglasses.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (Torch.torch.Any(x => x.PlayerId == player.PlayerId))
                Torch.torch.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (Flash.flash.Any(x => x.PlayerId == player.PlayerId))
                Flash.flash.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (Multitasker.multitasker.Any(x => x.PlayerId == player.PlayerId))
                Multitasker.multitasker.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (player == Tiebreaker.tiebreaker) Tiebreaker.clearAndReload();
            if (player == Mini.mini) Mini.clearAndReload();
            if (player == Aftermath.aftermath) Aftermath.clearAndReload();
            if (player == Giant.giant) Giant.clearAndReload();
            if (player == Watcher.watcher) Watcher.clearAndReload();
            if (player == Radar.radar) Radar.clearAndReload();
            if (player == Poucher.poucher && Poucher.spawnModifier) Poucher.clearAndReload();
            if (player == ButtonBarry.buttonBarry) ButtonBarry.clearAndReload();
            if (player == Disperser.disperser) Disperser.clearAndReload();
            if (player == Specoality.specoality) Specoality.clearAndReload();
            if (player == Indomitable.indomitable) Indomitable.clearAndReload();
            if (player == Tunneler.tunneler) Tunneler.clearAndReload();
            if (player == Slueth.slueth) Slueth.clearAndReload();
            if (player == Blind.blind) Blind.clearAndReload();
            if (player == Cursed.cursed) Cursed.clearAndReload();
            if (Vip.vip.Any(x => x.PlayerId == player.PlayerId)) Vip.vip.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (Invert.invert.Any(x => x.PlayerId == player.PlayerId))
                Invert.invert.RemoveAll(x => x.PlayerId == player.PlayerId);
            if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId))
                Chameleon.chameleon.RemoveAll(x => x.PlayerId == player.PlayerId);
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
        // AntiTeleport set position
        AntiTeleport.setPosition();

        showFlash(Palette.ImpostorRed);

        if (AntiTeleport.antiTeleport.FindAll(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId).Count == 0 && !CachedPlayer.LocalPlayer.Data.IsDead)
        {
            foreach (PlayerControl player in CachedPlayer.AllPlayers)
            {
                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
                if (Minigame.Instance)
                    Minigame.Instance.ForceClose();
                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                };

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

    public static void giveBomb(byte playerId)
    {
        if (playerId == byte.MaxValue)
        {
            Bomber.hasBomb = null;
            Bomber.bombActive = false;
            Bomber.hasAlerted = false;
            Bomber.timeLeft = 0;
            return;
        }

        Bomber.hasBomb = playerById(playerId);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.bombDelay,
            new Action<float>(p =>
            {
                if (p == 1f) Bomber.bombActive = true;
            })));
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.bombDelay + Bomber.bombTimer,
            new Action<float>(p =>
            {
                // Delayed action
                if (!Bomber.hasBomb.isAlive()) return;
                if (p == 1f && Bomber.bombActive)
                {
                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                    checkMurderAttemptAndKill(Bomber.bomber, Bomber.hasBomb, false, false, true, true);
                    Bomber.hasBomb = null;
                    Bomber.bombActive = false;
                    Bomber.hasAlerted = false;
                    Bomber.timeLeft = 0;
                }

                if (CachedPlayer.LocalPlayer.PlayerControl == Bomber.hasBomb)
                {
                    var totalTime = (int)(Bomber.bombDelay + Bomber.bombTimer);
                    var timeLeft = (int)(totalTime - (totalTime * p));
                    if (timeLeft <= Bomber.bombTimer)
                    {
                        if (Bomber.timeLeft != timeLeft)
                        {
                            new CustomMessage("你手中的炸弹将在 " + timeLeft + " 秒后引爆!", 1f);
                            Bomber.timeLeft = timeLeft;
                        }

                        if (timeLeft % 5 == 0)
                        {
                            if (!Bomber.hasAlerted)
                            {
                                showFlash(Bomber.alertColor);
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

            if (Camouflager.camouflageTimer <= 0 && !MushroomSabotageActive() && !isCamoComms())
                target.setDefaultLook();
            Ninja.isInvisble = false;
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
        Ninja.isInvisble = true;
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
            akujo.MurderPlayer(akujo, MurderResultFlags.Succeeded);
            overrideDeathReasonAndKiller(akujo, DeadPlayer.CustomDeathReason.Loneliness);
        }
    }

    public static void Mine(int ventId, PlayerControl role, byte[] buff, float zAxis)
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
            vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes
                .ElevatorMover); // just in case elevator vent is not blocked
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
            if (Camouflager.camouflageTimer <= 0 && !MushroomSabotageActive() & !isCamoComms())
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
            if (Camouflager.camouflageTimer <= 0 && !MushroomSabotageActive() & !isCamoComms())
                target.setDefaultLook();
            Jackal.isInvisable = false;
            return;
        }

        target.setLook("", 6, "", "", "", "");
        var color = Color.clear;
        var canSee = Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl ||
                     Sidekick.sidekick == CachedPlayer.LocalPlayer.PlayerControl ||
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
        KillTrap trap = new(pos);
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
            if (Camouflager.camouflageTimer <= 0 && !MushroomSabotageActive())
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
        new Portal(position);
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
            new CustomMessage("Lights are out", Trickster.lightsOutDuration);
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
                ? SecurityGuard.getStaticVentSealedSprite()
                : SecurityGuard.getAnimatedVentSealedSprite();
            var rend = vent.myRend;
            if (isFungle())
            {
                newSprite = SecurityGuard.getFungleVentSealedSprite();
                rend = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                animator = vent.transform.GetChild(3).GetComponent<SpriteAnim>();
            }

            animator?.Stop();
            rend.sprite = newSprite;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = SecurityGuard.getSubmergedCentralUpperSealedSprite();
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = SecurityGuard.getSubmergedCentralLowerSealedSprite();
            rend.color = new Color(1f, 1f, 1f, 0.5f);
            vent.name = "FutureSealedVent_" + vent.name;
        }

        ventsToSeal.Add(vent);
    }

    public static void arsonistWin()
    {
        Arsonist.triggerArsonistWin = true;
        foreach (PlayerControl p in CachedPlayer.AllPlayers)
            //if (p != Arsonist.arsonist && !p.Data.IsDead)
            if (p != Arsonist.arsonist)
            {
                p.Exiled();
                overrideDeathReasonAndKiller(p, DeadPlayer.CustomDeathReason.Arson, Arsonist.arsonist);
            }
    }

    public static void lawyerSetTarget(byte playerId)
    {
        Lawyer.target = playerById(playerId);
    }

    public static void executionerSetTarget(byte playerId)
    {
        Executioner.target = playerById(playerId);
    }

    public static void lawyerPromotesToPursuer()
    {
        var player = Lawyer.lawyer;
        var client = Lawyer.target;
        Lawyer.clearAndReload(false);

        Pursuer.pursuer.Add(player);

        if (player.PlayerId == CachedPlayer.LocalPlayer.PlayerId && client != null)
        {
            var playerInfoTransform = client.cosmetics.nameText.transform.parent.FindChild("Info");
            var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();
            if (playerInfo != null) playerInfo.text = "";
        }
    }

    public static void executionerPromotesRole()
    {
        var player = Executioner.executioner;
        var client = Executioner.target;

        Pursuer.pursuer.Add(player);
        Executioner.clearAndReload(false);

        if (player.PlayerId == CachedPlayer.LocalPlayer.PlayerId && client != null)
        {
            var playerInfoTransform = client.cosmetics.nameText.transform.parent.FindChild("Info");
            var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();
            if (playerInfo != null) playerInfo.text = "";
        }
    }

    public static void guesserShoot(byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleId)
    {
        var dyingTarget = playerById(dyingTargetId);
        var dyingLoverPartner = Lovers.bothDie ? dyingTarget.getPartner() : null; // Lover check
        var guessedTarget = playerById(guessedTargetId);
        PlayerControl? dyingAkujoPartner;

        // Lawyer shouldn't be exiled with the client for guesses
        if (dyingTarget == null) return;
        if (Lawyer.target != null && (dyingTarget == Lawyer.target || dyingLoverPartner == Lawyer.target))
            Lawyer.targetWasGuessed = true;

        if (Executioner.target != null && (dyingTarget == Executioner.target || dyingLoverPartner == Executioner.target))
            Executioner.targetWasGuessed = true;

        var guesser = playerById(killerId);
        if (Thief.thief != null && Thief.thief.PlayerId == killerId && Thief.canStealWithGuess)
        {
            var roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
            if (!Thief.thief.Data.IsDead && !Thief.isFailedThiefKill(dyingTarget, guesser, roleInfo))
                thiefStealsRole(dyingTarget.PlayerId);
        }

        if ((Akujo.akujo != null && dyingTarget == Akujo.akujo) || (Akujo.honmei != null && dyingTarget == Akujo.honmei))
            dyingAkujoPartner = dyingTarget == Akujo.akujo ? Akujo.honmei : Akujo.akujo;
        else
            dyingAkujoPartner = null;

        //末日猜测
        if (Doomsayer.doomsayer != null && Doomsayer.doomsayer == guesser && Doomsayer.canGuess)
        {
            var roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
            if (!Doomsayer.doomsayer.Data.IsDead && guessedTargetId == dyingTargetId)
            {
                Doomsayer.killedToWin++;
                if (Doomsayer.killedToWin >= Doomsayer.killToWin) Doomsayer.triggerDoomsayerrWin = true;
                if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
            }
            else
            {
                seedGuessChat(guesser, guessedTarget, guessedRoleId);
                return;
            }
        }

        if (Specoality.specoality != null && Specoality.specoality == guesser && Specoality.linearfunction > 0)
        {
            var roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
            if (!Specoality.specoality.Data.IsDead && guessedTargetId == dyingTargetId)
            {
                if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
            }
            else
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == Specoality.specoality) showFlash(Color.red, 0.75f, "");
                Specoality.canNoGuess = dyingTarget;
                Specoality.linearfunction--;
                seedGuessChat(guesser, guessedTarget, guessedRoleId);
                return;
            }
        }

        if (Lawyer.lawyer != null && Lawyer.lawyer.PlayerId == killerId && Lawyer.target != null && Lawyer.target.PlayerId == dyingTargetId)
        {
            // Lawyer guessed client.
            if (CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(Lawyer.lawyer.Data, Lawyer.lawyer.Data);
                if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
            }

            Lawyer.lawyer.Exiled();
        }

        var partnerId = dyingLoverPartner != null ? dyingLoverPartner.PlayerId : dyingTargetId;

        dyingTarget.Exiled();
        overrideDeathReasonAndKiller(dyingTarget, DeadPlayer.CustomDeathReason.Guess, guesser);
        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);
        byte akujoPartnerId = dyingAkujoPartner != null ? dyingAkujoPartner.PlayerId : byte.MaxValue;

        if (MeetingHud.Instance)
        {
            MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, dyingTargetId);
            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                if (pva.TargetPlayerId == dyingTargetId || pva.TargetPlayerId == partnerId || pva.TargetPlayerId == akujoPartnerId)
                {
                    pva.SetDead(pva.DidReport, true);
                    pva.Overlay.gameObject.SetActive(true);
                }

                //Give players back their vote if target is shot dead
                if (pva.VotedFor != dyingTargetId || pva.VotedFor != partnerId) continue;
                pva.UnsetVote();
                var voteAreaPlayer = playerById(pva.TargetPlayerId);
                if (!voteAreaPlayer.AmOwner) continue;
                MeetingHud.Instance.ClearVote();
            }

            if (AmongUsClient.Instance.AmHost)
                MeetingHud.Instance.CheckForEndVoting();
        }

        if (Doomsayer.doomsayer == null || Doomsayer.doomsayer != guesser)
        {
            HandleGuesser.remainingShots(killerId, true);
        }

        if (FastDestroyableSingleton<HudManager>.Instance != null && guesser != null)
        {
            if (CachedPlayer.LocalPlayer.PlayerControl == dyingTarget)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(guesser.Data,
                    dyingTarget.Data);
                if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
            }
            else if (dyingLoverPartner != null && CachedPlayer.LocalPlayer.PlayerControl == dyingLoverPartner)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingLoverPartner.Data,
                    dyingLoverPartner.Data);
                if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
            }
            else if (dyingAkujoPartner != null && CachedPlayer.LocalPlayer.PlayerControl == dyingAkujoPartner)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingAkujoPartner.Data, dyingAkujoPartner.Data);
                if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
            }
        }

        // remove shoot button from targets for all guessers and close their guesserUI
        if (GuesserGM.isGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer != guesser &&
            !PlayerControl.LocalPlayer.Data.IsDead &&
            GuesserGM.remainingShots(PlayerControl.LocalPlayer.PlayerId) > 0 && MeetingHud.Instance)
        {
            MeetingHud.Instance.playerStates.ToList().ForEach(x =>
            {
                if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null)
                    Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
            });
            if (dyingLoverPartner != null)
                MeetingHud.Instance.playerStates.ToList().ForEach(x =>
                {
                    if (x.TargetPlayerId == dyingLoverPartner.PlayerId && x.transform.FindChild("ShootButton") != null)
                        Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                });

            if (MeetingHudPatch.guesserUI != null && MeetingHudPatch.guesserUIExitButton != null)
            {
                if (MeetingHudPatch.guesserCurrentTarget == dyingTarget.PlayerId)
                    MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                else if (dyingLoverPartner != null &&
                         MeetingHudPatch.guesserCurrentTarget == dyingLoverPartner.PlayerId)
                    MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
            }
        }
        if (guesser != null && guessedTarget != null) seedGuessChat(guesser, guessedTarget, guessedRoleId);
    }

    public static void seedGuessChat(PlayerControl guesser, PlayerControl guessedTarget, byte guessedRoleId)
    {
        if (CachedPlayer.LocalPlayer.Data.IsDead)
        {
            var roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
            var msg = $"{guesser.Data.PlayerName} 赌怪猜测 {guessedTarget.Data.PlayerName} 是 {roleInfo?.name ?? ""}!";
            if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                FastDestroyableSingleton<HudManager>.Instance!.Chat.AddChat(guesser, msg);
            if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
                FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
        }
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
        if (CustomOptionHolder.bodyGuardFlash.getBool()) showFlash(BodyGuard.color);
    }

    public static void showCultistFlash()
    {
        if (Follower.follower == CachedPlayer.LocalPlayer.PlayerControl)
            showFlash(new Color(32f / 51f, 0.007843138f, 74f / 85f));
    }

    public static void showFollowerFlash()
    {
        if (Cultist.cultist == CachedPlayer.LocalPlayer.PlayerControl)
            showFlash(new Color(32f / 51f, 0.007843138f, 74f / 85f));
    }

    public static void bodyGuardGuardPlayer(byte targetId)
    {
        var target = playerById(targetId);
        BodyGuard.usedGuard = true;
        BodyGuard.guarded = target;
    }

    public static void privateInvestigatorWatchPlayer(byte targetId)
    {
        var target = playerById(targetId);
        PrivateInvestigator.watching = target;
    }

    public static void privateInvestigatorWatchFlash(byte targetId)
    {
        var target = playerById(targetId);
        // GetDefaultOutfit().ColorId
        if (CachedPlayer.LocalPlayer.PlayerControl == PrivateInvestigator.privateInvestigator)
        {
            if (PrivateInvestigator.seeFlashColor)
                showFlash(Palette.PlayerColors[target.Data.DefaultOutfit.ColorId]);
            else
                showFlash(PrivateInvestigator.color);
        }
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

    public static void setPosition(byte playerId, float x, float y)
    {
        var target = playerById(playerId);
        target.transform.localPosition = new Vector3(x, y, 0);
        target.transform.position = new Vector3(x, y, 0);
    }

    public static void setPositionESC(byte playerId, float x, float y)
    {
        var target = playerById(playerId);
        target.transform.localPosition = new Vector3(x, y, 0);
        target.transform.position = new Vector3(x, y, 0);
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
            rend.sprite = loadSpriteFromResources("TheOtherRoles.Resources.ChatOverlay.png", 130f);
            if (playerControl.PlayerId != localPlayerId) rend.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(2f,
                (Action<float>)delegate (float p)
                {
                    if (p == 1f)
                    {
                        rend.gameObject.SetActive(false);
                        Object.Destroy(rend.gameObject);
                    }
                }));
        }
        catch
        {
            Message("Chat Notification Overlay is Detected");
        }
    }

    public static void setTiebreak()
    {
        Tiebreaker.isTiebreak = true;
    }

    public static void thiefStealsRole(byte playerId)
    {
        var target = playerById(playerId);
        var thief = Thief.thief;
        if (target == null) return;
        if (target == Sheriff.sheriff) Sheriff.sheriff = thief;
        if (target == Deputy.deputy) Deputy.deputy = thief;
        if (target == Veteran.veteran) Veteran.veteran = thief;
        if (target == Jackal.jackal)
        {
            Jackal.jackal = thief;
            Jackal.formerJackals.Add(target);
        }

        if (target == Sidekick.sidekick)
        {
            Sidekick.sidekick = thief;
            Jackal.formerJackals.Add(target);
            if (HandleGuesser.isGuesserGm && CustomOptionHolder.guesserGamemodeSidekickIsAlwaysGuesser.getBool() && !HandleGuesser.isGuesser(thief.PlayerId))
                setGuesserGm(thief.PlayerId);
        }
        if (target == Pavlovsdogs.pavlovsowner)
        {
            Pavlovsdogs.pavlovsdogs.Add(target);
            Pavlovsdogs.pavlovsowner = thief;
        }
        if (target == Pavlovsdogs.pavlovsdogs.Contains(target))
        {
            Pavlovsdogs.pavlovsdogs.Add(thief);
        }
        if (Pavlovsdogs.pavlovsowner) Pavlovsdogs.pavlovsowner = thief;
        if (Pavlovsdogs.pavlovsdogs.Any())
        {
            Pavlovsdogs.pavlovsdogs.Add(thief);
        }
        //if (target == Guesser.evilGuesser) Guesser.evilGuesser = thief;
        if (target == Poucher.poucher && !Poucher.spawnModifier) Poucher.poucher = thief;
        if (target == Morphling.morphling) Morphling.morphling = thief;
        if (target == Camouflager.camouflager) Camouflager.camouflager = thief;
        if (target == Vampire.vampire) Vampire.vampire = thief;
        if (target == Eraser.eraser) Eraser.eraser = thief;
        if (target == Trickster.trickster) Trickster.trickster = thief;
        if (target == Cleaner.cleaner) Cleaner.cleaner = thief;
        if (target == Warlock.warlock) Warlock.warlock = thief;
        if (target == BountyHunter.bountyHunter) BountyHunter.bountyHunter = thief;
        if (target == Cultist.cultist) Cultist.cultist = thief;
        if (target == Follower.follower) Follower.follower = thief;
        if (target == Witch.witch)
        {
            Witch.witch = thief;
            if (MeetingHud.Instance)
                if (Witch.witchVoteSavesTargets) // In a meeting, if the thief guesses the witch, all targets are saved or no target is saved.
                    Witch.futureSpelled = new List<PlayerControl>();
                else // If thief kills witch during the round, remove the thief from the list of spelled people, keep the rest
                    Witch.futureSpelled.RemoveAll(x => x.PlayerId == thief.PlayerId);
        }

        if (target == Ninja.ninja) Ninja.ninja = thief;
        if (target == Escapist.escapist) Escapist.escapist = thief;
        if (target == Terrorist.terrorist) Terrorist.terrorist = thief;
        if (target == Bomber.bomber) Bomber.bomber = thief;
        if (target == Miner.miner) Miner.miner = thief;
        if (target == Undertaker.undertaker) Undertaker.undertaker = thief;
        if (target == Mimic.mimic)
        {
            Mimic.mimic = thief;
            Mimic.hasMimic = false;
        }
        if (target == Yoyo.yoyo)
        {
            Yoyo.yoyo = thief;
            Yoyo.markedLocation = null;
        }
        if (target.Data.Role.IsImpostor)
        {
            RoleManager.Instance.SetRole(Thief.thief, RoleTypes.Impostor);
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(Thief.thief.killTimer,
                GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
        }

        if (target == Werewolf.werewolf) Werewolf.werewolf = thief;
        if (target == Juggernaut.juggernaut) Juggernaut.juggernaut = thief;
        if (target == Swooper.swooper) Swooper.swooper = thief;

        if (target == Deputy.deputy) Deputy.deputy = thief;
        if (target == Veteran.veteran) Veteran.veteran = thief;
        if (target == Blackmailer.blackmailer) Blackmailer.blackmailer = thief;
        if (target == EvilTrapper.evilTrapper) EvilTrapper.evilTrapper = thief;

        if (Lawyer.lawyer != null && target == Lawyer.target)
            Lawyer.target = thief;
        if (Thief.thief == PlayerControl.LocalPlayer) CustomButton.ResetAllCooldowns();
        Thief.clearAndReload();
        Thief.formerThief = thief; // After clearAndReload, else it would get reset...
    }

    public static void setTrap(byte[] buff)
    {
        if (Trapper.trapper == null) return;
        Trapper.charges -= 1;
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        new Trap(position);
    }

    public static void triggerTrap(byte playerId, byte trapId)
    {
        Trap.triggerTrap(playerId, trapId);
    }

    public static void setGuesserGm(byte playerId)
    {
        var target = playerById(playerId);
        if (target == null) return;
        new GuesserGM(target);
    }

    public static void shareTimer(float punish)
    {
        HideNSeek.timer -= punish;
    }

    public static void huntedShield(byte playerId)
    {
        if (!Hunted.timeshieldActive.Contains(playerId)) Hunted.timeshieldActive.Add(playerId);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Hunted.shieldDuration,
            new Action<float>(p =>
            {
                if (p == 1f) Hunted.timeshieldActive.Remove(playerId);
            })));
    }

    public static void huntedRewindTime(byte playerId)
    {
        Hunted.timeshieldActive.Remove(playerId); // Shield is no longer active when rewinding
        SoundEffectsManager.stop("timemasterShield"); // Shield sound stopped when rewinding
        if (playerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId) resetHuntedRewindButton();
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Hunted.shieldRewindTime,
            new Action<float>(p =>
            {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

        if (!CachedPlayer.LocalPlayer.Data.Role.IsImpostor) return; // only rewind hunter

        TimeMaster.isRewinding = true;

        if (MapBehaviour.Instance)
            MapBehaviour.Instance.Close();
        if (Minigame.Instance)
            Minigame.Instance.ForceClose();
        CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
    }

    public static void propHuntStartTimer(bool blackout = false)
    {
        if (blackout)
        {
            PropHunt.blackOutTimer = PropHunt.initialBlackoutTime;
            PropHunt.transformLayers();
        }
        else
        {
            PropHunt.timerRunning = true;
            PropHunt.blackOutTimer = 0f;
        }

        PropHunt.startTime = DateTime.UtcNow;
        foreach (var pc in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.Role.IsImpostor))
            pc.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
    }

    public static void propHuntSetProp(byte playerId, string propName, float posX)
    {
        var player = playerById(playerId);
        var prop = PropHunt.FindPropByNameAndPos(propName, posX);
        if (prop == null) return;
        try
        {
            player.GetComponent<SpriteRenderer>().sprite = prop.GetComponent<SpriteRenderer>().sprite;
        }
        catch
        {
            player.GetComponent<SpriteRenderer>().sprite =
                prop.transform.GetComponentInChildren<SpriteRenderer>().sprite;
        }

        player.transform.localScale = prop.transform.lossyScale;
        player.Visible = false;
        PropHunt.currentObject[player.PlayerId] = new Tuple<string, float>(propName, posX);
    }

    public static void propHuntSetRevealed(byte playerId)
    {
        SoundEffectsManager.play("morphlingMorph");
        PropHunt.isCurrentlyRevealed.Add(playerId, PropHunt.revealDuration);
        PropHunt.timer -= PropHunt.revealPunish;
    }

    public static void propHuntSetInvis(byte playerId)
    {
        PropHunt.invisPlayers.Add(playerId, PropHunt.invisDuration);
    }

    public static void propHuntSetSpeedboost(byte playerId)
    {
        PropHunt.speedboostActive.Add(playerId, PropHunt.speedboostDuration);
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
            case GhostInfoTypes.MediumInfo:
                var mediumInfo = reader.ReadString();
                if (shouldShowGhostInfo())
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, mediumInfo);
                break;
            case GhostInfoTypes.DetectiveOrMedicInfo:
                var detectiveInfo = reader.ReadString();
                if (shouldShowGhostInfo())
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, detectiveInfo);
                break;
            case GhostInfoTypes.BlankUsed:
                Pursuer.blankedList.Remove(sender);
                break;
            case GhostInfoTypes.VampireTimer:
                vampireKillButton.Timer = reader.ReadByte();
                break;
            case GhostInfoTypes.DeathReasonAndKiller:
                overrideDeathReasonAndKiller(playerById(reader.ReadByte()),
                    (DeadPlayer.CustomDeathReason)reader.ReadByte(), playerById(reader.ReadByte()));
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
    /*
    public static void shareRoom(byte playerId, byte roomId)
    {
        if (Snitch.playerRoomMap.ContainsKey(playerId)) Snitch.playerRoomMap[playerId] = roomId;
        else Snitch.playerRoomMap.Add(playerId, roomId);
    }
    */
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
        if (enableDebugLogMode) Info($"接收 PlayerControl 原版Rpc RpcId{callId} Message Size {reader.Length}");
    }

    private static bool Prefix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        if (RpcNames == null)
            GetRpcNames();

        var packetId = (CustomRPC)callId;
        if (!RpcNames!.ContainsKey(packetId))
            return true;

        if (enableDebugLogMode) Info($"接收 PlayerControl CustomRpc RpcId{callId} Rpc Name{RpcNames?[(CustomRPC)callId] ?? nameof(packetId)} Message Size {reader.Length}");
        switch (packetId)
        {
            // Main Controls

            case CustomRPC.ResetVaribles:
                RPCProcedure.resetVariables();
                break;
            case CustomRPC.ShareOptions:
                RPCProcedure.HandleShareOptions(reader.ReadByte(), reader);
                break;
            case CustomRPC.ForceEnd:
                RPCProcedure.forceEnd();
                break;
            case CustomRPC.WorkaroundSetRoles:
                RPCProcedure.workaroundSetRoles(reader.ReadByte(), reader);
                break;
            case CustomRPC.SetRole:
                var roleId = reader.ReadByte();
                var playerId = reader.ReadByte();
                RPCProcedure.setRole(roleId, playerId);
                break;
            case CustomRPC.SetModifier:
                var modifierId = reader.ReadByte();
                var pId = reader.ReadByte();
                var flag = reader.ReadByte();
                RPCProcedure.setModifier(modifierId, pId, flag);
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
                HandshakeHelper.versionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
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
                var mapId = reader.ReadByte();
                RPCProcedure.dynamicMapOption(mapId);
                break;

            case CustomRPC.SetGameStarting:
                RPCProcedure.setGameStarting();
                break;

            case CustomRPC.VersionHandshakeEx:
                //HandshakeHelper.VersionHandshakeEx(reader);
                break;

            // Role functionality

            case CustomRPC.EngineerFixLights:
                RPCProcedure.engineerFixLights();
                break;
            case CustomRPC.EngineerFixSubmergedOxygen:
                RPCProcedure.engineerFixSubmergedOxygen();
                break;
            case CustomRPC.EngineerUsedRepair:
                RPCProcedure.engineerUsedRepair();
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
                RPCProcedure.amnisiacTakeRole(reader.ReadByte());
                break;

            case CustomRPC.ImpostorPromotesToLastImpostor:
                RPCProcedure.impostorPromotesToLastImpostor(reader.ReadByte());
                break;

            case CustomRPC.MimicMimicRole:
                RPCProcedure.mimicMimicRole(reader.ReadByte());
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

            case CustomRPC.DoomsayerMeeting:
                if (!shouldShowGhostInfo()) break;
                var index = reader.ReadPackedInt32();
                for (var i = 1; i < index; i++)
                {
                    var message = reader.ReadString();
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Doomsayer.doomsayer, message);
                }

                break;

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

            case CustomRPC.PrivateInvestigatorWatchPlayer:
                RPCProcedure.privateInvestigatorWatchPlayer(reader.ReadByte());
                break;

            case CustomRPC.PrivateInvestigatorWatchFlash:
                RPCProcedure.privateInvestigatorWatchFlash(reader.ReadByte());
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
                RPCProcedure.sidekickPromotes();
                break;

            case CustomRPC.ErasePlayerRoles:
                var eraseTarget = reader.ReadByte();
                RPCProcedure.erasePlayerRoles(eraseTarget);
                Eraser.alreadyErased.Add(eraseTarget);
                break;

            case CustomRPC.SetFutureErased:
                RPCProcedure.setFutureErased(reader.ReadByte());
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

            case CustomRPC.ArsonistWin:
                RPCProcedure.arsonistWin();
                break;

            case CustomRPC.GuesserShoot:
                var killerId = reader.ReadByte();
                var dyingTarget = reader.ReadByte();
                var guessedTarget = reader.ReadByte();
                var guessedRoleId = reader.ReadByte();
                RPCProcedure.guesserShoot(killerId, dyingTarget, guessedTarget, guessedRoleId);
                break;

            case CustomRPC.LawyerSetTarget:
                RPCProcedure.lawyerSetTarget(reader.ReadByte());
                break;

            case CustomRPC.LawyerPromotesToPursuer:
                RPCProcedure.lawyerPromotesToPursuer();
                break;

            case CustomRPC.ExecutionerSetTarget:
                RPCProcedure.executionerSetTarget(reader.ReadByte());
                break;

            case CustomRPC.ExecutionerPromotesRole:
                RPCProcedure.executionerPromotesRole();
                break;

            case CustomRPC.PursuerSetBlanked:
                var pid = reader.ReadByte();
                var blankedValue = reader.ReadByte();
                RPCProcedure.pursuerSetBlanked(pid, blankedValue);
                break;
                
            case CustomRPC.GiveBomb:
                RPCProcedure.giveBomb(reader.ReadByte());
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

            case CustomRPC.SetTiebreak:
                RPCProcedure.setTiebreak();
                break;

            case CustomRPC.ShowBodyGuardFlash:
                RPCProcedure.showBodyGuardFlash();
                break;

            case CustomRPC.ShowCultistFlash:
                RPCProcedure.showCultistFlash();
                break;

            case CustomRPC.ShowFollowerFlash:
                RPCProcedure.showFollowerFlash();
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
                var role = playerById(reader.ReadByte());
                var pos = reader.ReadBytesAndSize();
                var zAxis = reader.ReadSingle();
                RPCProcedure.Mine(newVentId, role, pos, zAxis);
                break;

            case CustomRPC.CultistCreateImposter:
                RPCProcedure.cultistCreateImposter(reader.ReadByte());
                break;

            case CustomRPC.TurnToImpostor:
                RPCProcedure.turnToImpostor(reader.ReadByte());
                break;

            case CustomRPC.TurnToCrewmate:
                RPCProcedure.turnToCrewmate(reader.ReadByte());
                break;

            case CustomRPC.ThiefStealsRole:
                var thiefTargetId = reader.ReadByte();
                RPCProcedure.thiefStealsRole(thiefTargetId);
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

            case CustomRPC.ShareGamemode:
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
            case CustomRPC.ShareTimer:
                var punish = reader.ReadSingle();
                RPCProcedure.shareTimer(punish);
                break;
            case CustomRPC.HuntedShield:
                var huntedPlayer = reader.ReadByte();
                RPCProcedure.huntedShield(huntedPlayer);
                break;
            case CustomRPC.HuntedRewindTime:
                var rewindPlayer = reader.ReadByte();
                RPCProcedure.huntedRewindTime(rewindPlayer);
                break;
            case CustomRPC.PropHuntStartTimer:
                RPCProcedure.propHuntStartTimer(reader.ReadBoolean());
                break;
            case CustomRPC.SetProp:
                var targetPlayer = reader.ReadByte();
                var propName = reader.ReadString();
                var posX = reader.ReadSingle();
                RPCProcedure.propHuntSetProp(targetPlayer, propName, posX);
                break;

            case CustomRPC.SetRevealed:
                RPCProcedure.propHuntSetRevealed(reader.ReadByte());
                break;

            case CustomRPC.PropHuntSetInvis:
                RPCProcedure.propHuntSetInvis(reader.ReadByte());
                break;

            case CustomRPC.PropHuntSetSpeedboost:
                RPCProcedure.propHuntSetSpeedboost(reader.ReadByte());
                break;

            case CustomRPC.ShareGhostInfo:
                RPCProcedure.receiveGhostInfo(reader.ReadByte(), reader);
                break;
            /*
            case CustomRPC.ShareRoom:
                var roomPlayer = reader.ReadByte();
                var roomId = reader.ReadByte();
                RPCProcedure.shareRoom(roomPlayer, roomId);
                break;
            */
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
            case CustomRPC.YoyoBlink:
                RPCProcedure.yoyoBlink(reader.ReadByte() == byte.MaxValue, reader.ReadBytesAndSize());
                break;
            case CustomRPC.SetFutureReveal:
                break;
            case CustomRPC.SetPosition:
                break;
            case CustomRPC.SetPositionESC:
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
                break;

            case CustomRPC.SurvivorVestActive:
                RPCProcedure.survivorVestActive();
                break;
                
            case CustomRPC.JackalCanSwooper:
                RPCProcedure.jackalCanSwooper(reader.ReadByte() == byte.MaxValue);
                break;

            case CustomRPC.HostEndGame:
                isCanceled = true;
                break;
        }

        return false;
    }
}
