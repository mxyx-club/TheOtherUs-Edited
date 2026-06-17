#nullable enable
using AmongUs.GameOptions;
using PowerTools;
using TheOtherRoles.Attributes;
using TheOtherRoles.Mode;
using TheOtherRoles.Objects;
using TheOtherRoles.Patches;
using static TheOtherRoles.Buttons.HudManagerStartPatch;
using static TheOtherRoles.Options.ModOption;

namespace TheOtherRoles;

public enum CustomRPC : byte
{
    // Main Controls
    ShareOptions = 80,
    VersionHandshake,
    SetRole,
    SetModifier,
    SetGhostRole,
    UseUncheckedVent,
    DynamicMapOption,
    SetGameStarting,
    StopStart,
    DraftModePickOrder,
    DraftModePick,
    ShareGameMode,
    ShareFriendCode,
    ShareGameId,
    Exiled,
    NoCheckEndGame,

    CustomMurderPlayer,
    RevivePlayer,
    HostControl,
    SendChatToChannel,

    // Role functionality
    SetFutureErased = 110,
    SetFutureReveal,
    SetFutureShifted,
    FixLights,
    FixingSabotage,
    FixSubmergedOxygen,
    LawyerSetTarget,
    ExecutionerSetTarget,
    ExecutionerPromotesRole,
    LawyerPromotesToPursuer,
    SetFutureShielded,
    SetFutureSpelled,
    CleanBody,
    CreateDeadBody,
    AkujoSetHonmei,
    AkujoSetKeep,
    AkujoSuicide,
    AkujoSetUnifiedVote,
    Mine,
    ShowIndomitableFlash,
    UndertakerDragAction,
    MedicSetShielded,
    ShowBodyGuardFlash,
    ShieldedMurderAttempt,
    CursedTurn,
    BodyGuardGuardPlayer,
    VeteranAlert,
    ShifterShift,
    SwapperSwap,
    GlitchMimic,
    CamouflagerCamouflage,
    NoCheckStartMeeting,
    ProphetExamine,
    ImpostorPromotesToLastImpostor,
    //CamoComms,
    MayorRevealed,
    MayorMultiVote,
    MayorSetVoteCount,
    TrackerUsedTracker,
    VampireSetBitten,
    PlaceGarlic,
    GiveBomb,
    DeputyUsedHandcuffs,
    DeputyPromotes,
    JackalCreatesSidekick,
    PavlovsCreateDog,
    PavlovsRing,
    SidekickPromotes,
    ClearGhostRoles,
    Disperse,
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
    BlackmailPlayer,
    UseCameraTime,
    UseVitalsTime,
    SetBlanked,
    SetFirstKill,
    SetInvisibleGen,
    SetSwoop,
    SetJackalSwoop,
    JackalCanSwooper,
    InfoSleuthSetTarget,
    GrenadierFlash,
    WitnessReport,
    WitnessSetTarget,
    WolfLordkilled,
    PelicanKill,
    RedemptorRevive,
    RedemptorPrayer,
    BandLeaderFormed,
    CreateBandMember,
    SchrodingersCatSetState,
    SyncGunsmithChange,
    InfectedTarget,
    JailorSendMessage,
    GuesserMessage,
    JailorJail,
    ExiledJailed,
    SetAvengerLover,
    JesterWinner,
    GaolerMarkPrisoner,

    TrapperKill,
    PlaceTrap,
    ActivateTrap,
    DisableTrap,
    Prosecute,
    SurvivorVestActive,
    PoltergeistMove,
    jesterDragBody,
    PlaceClogGhost,
    SetConfesser,
    SendOracleReport,
    SoulSightSuicide,
    SoulSightRevive,
    SoulSightScore,
    DreamcatcherSetDreamer,

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

    PlaceDecoy,
    DecoyDestroy,
    DecoySwap,

    // Gamemode
    SetGuesserGm,

    // Other functionality
    ShareGhostInfo,
    ShareDeathReasonAndKiller,
}

public static class RPCProcedure
{
    public const byte CustomRpcId = 80;

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
        DeathReasonAndKiller
    }

    public enum HostCommand
    {
        HostSay,
        HostSetRole,
        HostClearRole,
        HostKill,
        HostExile,
        HostRevive,
        HostClearTasks,
        HostClearVotes,
    }

    // Main Controls
    [OnGameStart, OnGameEnd]
    public static void resetVariables()
    {
        clearAndReloadModOptions();
        clearAndReloadRoles();
        MapData.Clear();
        GhostRole.ClearAndReload();
        toggleZoom(true);
        GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 0;
        SurveillanceMinigamePatch.nightVisionOverlays = null;
        MeetingHudPatch.MeetingCount = 0;
        ChatControllerPatch.EnableChat.ForceEnableChat = false;
        Message($"ClearAndReload", "RPC");
    }

    public static void HandleShareOptions(byte numberOfOptions, MessageReader reader)
    {
        try
        {
            for (var i = 0; i < numberOfOptions; i++)
            {
                var optionId = reader.ReadPackedUInt32();
                var selection = reader.ReadPackedUInt32();
                var option = CustomOption.Options.First(option => option.Id == (int)optionId);
                option.updateSelection((int)selection);
            }
        }
        catch (Exception e)
        {
            Error("Error while deserializing options: " + e.Message);
        }
    }

    public static void shareGameMode(byte gm)
    {
        GameMode = (CustomGameModes)gm;
    }

    public static void stopStart(byte playerId)
    {
        if (AmongUsClient.Instance.AmHost && CustomOptionHolder.anyPlayerCanStopStart.GetBool())
        {
            GameStartManager.Instance.ResetStartState();
            PlayerControl.LocalPlayer.RpcSendChat($"{PlayerById(playerId)?.Data?.PlayerName} 阻止游戏开始");
        }
    }

    public static void setRole(byte playerId, byte roleId)
    {
        var player = PlayerById(playerId);
        switch ((RoleId)roleId)
        {
            case RoleId.DefaultRole:
            case RoleId.Crewmate:
            case RoleId.Impostor:
                break;
            case RoleId.Jester:
                Jester.Player.Add(player);
                break;
            case RoleId.Werewolf:
                Werewolf.werewolf = player;
                break;
            case RoleId.WolfLord:
                WolfLord.Player = player;
                break;
            case RoleId.Blackmailer:
                Blackmailer.Player = player;
                break;
            case RoleId.Miner:
                Miner.miner = player;
                break;
            case RoleId.Poucher:
                Poucher.poucher = player;
                break;
            case RoleId.Professional:
                Professional.Player = player;
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
                Sheriff.Player.Add(player);
                break;
            case RoleId.Deputy:
                Sheriff.Deputy = player;
                break;
            case RoleId.BodyGuard:
                BodyGuard.bodyguard = player;
                break;
            case RoleId.Detective:
                Detective.detective = player;
                break;
            case RoleId.InfoSleuth:
                InfoSleuth.infoSleuth = player;
                break;
            case RoleId.Amnisiac:
                Amnisiac.Player.Add(player);
                break;
            case RoleId.PartTimer:
                PartTimer.partTimer = player;
                break;
            case RoleId.Grenadier:
                Grenadier.Player = player;
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
            case RoleId.Glitch:
                Glitch.Player = player;
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
                Jackal.Sidekick = player;
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
            case RoleId.Alchemyst:
                Alchemyst.Player = player;
                break;
            case RoleId.Trapper:
                Trapper.trapper = player;
                break;
            case RoleId.Pelican:
                Pelican.Player = player;
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
            case RoleId.Marionette:
                Marionette.Player = player;
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
            case RoleId.Redemptor:
                Redemptor.Player = player;
                break;
            case RoleId.BandLeader:
                BandLeader.Player = player;
                break;
            case RoleId.SchrodingersCat:
                SchrodingersCat.Player = player;
                break;
            case RoleId.Gunsmith:
                Gunsmith.Player = player;
                break;
            case RoleId.Berserker:
                Berserker.Player = player;
                break;
            case RoleId.Infected:
                Infected.Player.Add(player);
                break;
            case RoleId.Jailor:
                Jailor.Player = player;
                break;
            case RoleId.Avenger:
                Avenger.Player = player;
                break;
            case RoleId.Oracle:
                Oracle.Player = player;
                break;
            case RoleId.Dreamcatcher:
                Dreamcatcher.Player = player;
                break;
            case RoleId.SoulSight:
                SoulSight.Player = player;
                break;
            case RoleId.Gaoler:
                Gaoler.Player = player;
                break;
            default:
                Warn("Unknown role ID: " + roleId, "SetRole");
                break;
        }

        if (RoleInfo.RoleInfoById[(RoleId)roleId].roleType == RoleType.Impostor)
            player.SetRoleType(RoleTypes.Impostor);
        else
            player.SetRoleType(RoleTypes.Crewmate);

        var data = PlayerData.GetPlayerData(player);

        if (data != null)
        {
            data.MainRole = (RoleId)roleId;
            data.RoleHistory.Add((RoleId)roleId);
        }
    }

    public static void setModifier(byte playerId, byte modifierId, byte flag = 0)
    {
        var player = PlayerById(playerId);
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
            case RoleId.ProfessionalModifier:
                Professional.Player = player;
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
                Mini.accumulatedGrowthTime = 0;
                break;
            case RoleId.Giant:
                Giant.giant = player;
                break;
            case RoleId.LastImpostor:
                LastImpostor.lastImpostor = player;
                break;
            case RoleId.Vip:
                Vip.vip.Add(player);
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
            default:
                Warn("Unknown role ID: " + modifierId, "SetModifier");
                break;
        }

        var data = PlayerData.GetPlayerData(player);
        if (data != null)
        {
            data.Modifiers.Add((RoleId)modifierId);
        }
    }

    public static void setGhostRole(byte playerId, byte roleId)
    {
        var player = PlayerById(playerId);
        switch ((RoleId)roleId)
        {
            case RoleId.Clog:
                Clog.Player = player;
                break;
            case RoleId.GhostEngineer:
                GhostEngineer.Player = player;
                break;
            case RoleId.Specter:
                Specter.Player = player;
                break;
            case RoleId.Poltergeist:
                Poltergeist.Player = player;
                break;
        }

        var data = PlayerData.GetPlayerData(player);
        if (data != null)
        {
            data.GhostRole = (RoleId)roleId;
            data.RoleHistory.Add((RoleId)roleId);
        }
    }

    public static void HostControl(PlayerControl controller, HostCommand command, MessageReader reader)
    {
        switch (command)
        {
            case HostCommand.HostSay:
                ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.HostChat;
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reader.ReadString());
                HudManager.Instance.Chat.StartCoroutine(HudManager.Instance.Chat.BounceDot());
                SoundManager.Instance.PlaySound(HudManager.Instance?.Chat?.messageSound, false, 1f, null);
                break;
            case HostCommand.HostKill:
                {
                    var target = reader.ReadPlayer();
                    if (target.IsDead()) return;

                    target.Exiled();
                    PlayerData.SetDeathReason(target, CustomDeathReason.HostKill, controller);

                    DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
                    foreach (var body in array)
                    {
                        if (body.ParentId != target.PlayerId) continue;
                        UObject.Destroy(body.gameObject);
                        break;
                    }
                }
                break;
            case HostCommand.HostRevive:
                {
                    var target = reader.ReadPlayer();
                    target?.ModRevive(true, true);
                }
                break;
            case HostCommand.HostClearTasks:
                {
                    var target = reader.ReadPlayer();
                    target.clearAllTasks();
                }
                break;
            case HostCommand.HostExile:
                ExileControllerBeginPatch.ForceExile = true;
                break;
            case HostCommand.HostClearVotes:
                if (!InMeeting) return;
                MeetingHud.Instance.playerStates.ForEach((x) =>
                {
                    x.UnsetVote();
                });
                MeetingHud.Instance.ClearVote();
                break;
            case HostCommand.HostSetRole:
                {
                    var target = reader.ReadPlayer();
                    var roleId = (RoleId)reader.ReadByte();
                    Message("SetRole Role:" + target.Data.PlayerName);
                    if (target != null && RoleInfo.RoleInfoById.TryGetValue(roleId, out var info))
                    {
                        if (info.roleType == RoleType.Impostor)
                        {
                            target.Data.Role.TeamType = RoleTeamTypes.Impostor;
                            target.SetRoleType(RoleTypes.Impostor);
                        }
                        else
                        {
                            target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                            target.SetRoleType(RoleTypes.Crewmate);

                        }
                        setRole(target.PlayerId, (byte)roleId);
                    }
                }
                break;
            case HostCommand.HostClearRole:
                {
                    var target = reader.ReadPlayer();
                    Message("Clean Role:" + target.Data.PlayerName);
                    erasePlayerRoles(target.PlayerId, false);
                }
                break;
            default:
                break;
        }
    }

    public static void ResetRole(RoleId roleId, PlayerControl target, bool reset)
    {

        switch (roleId)
        {
            case RoleId.Impostor:
                break;
            case RoleId.Glitch:
                if (reset) Glitch.clearAndReload();
                break;
            case RoleId.WolfLord:
                if (reset) WolfLord.ClearAndReload();
                break;
            case RoleId.Bomber:
                if (reset) Bomber.clearAndReload();
                break;
            case RoleId.Mimic:
                if (reset) Mimic.clearAndReload(false);
                break;
            case RoleId.Camouflager:
                if (reset) Camouflager.clearAndReload();
                break;
            case RoleId.Miner:
                if (reset) Miner.clearAndReload();
                break;
            case RoleId.Eraser:
                if (reset) Eraser.clearAndReload();
                break;
            case RoleId.Vampire:
                if (reset) Vampire.clearAndReload();
                break;
            case RoleId.Undertaker:
                if (reset) Undertaker.clearAndReload();
                break;
            case RoleId.Marionette:
                if (reset) Marionette.ClearAndReload();
                break;
            case RoleId.Warlock:
                if (reset) Warlock.clearAndReload();
                break;
            case RoleId.Trickster:
                if (reset) Trickster.clearAndReload();
                break;
            case RoleId.BountyHunter:
                if (reset) BountyHunter.clearAndReload();
                break;
            case RoleId.Cleaner:
                if (reset) Cleaner.clearAndReload();
                break;
            case RoleId.Terrorist:
                if (reset) Terrorist.clearAndReload();
                break;
            case RoleId.Blackmailer:
                if (reset) Blackmailer.clearAndReload();
                break;
            case RoleId.Witch:
                if (reset) Witch.clearAndReload();
                break;
            case RoleId.Ninja:
                if (reset) Ninja.clearAndReload();
                break;
            case RoleId.Yoyo:
                if (reset) Yoyo.clearAndReload();
                break;
            case RoleId.EvilTrapper:
                if (reset) EvilTrapper.clearAndReload();
                break;
            case RoleId.Grenadier:
                if (reset) Grenadier.clearAndReload();
                break;

            case RoleId.Amnisiac:
                break;
            case RoleId.Survivor:
                break;
            case RoleId.Jester:
                if (reset) Jester.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Vulture:
                if (reset) Vulture.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Lawyer:
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Executioner:
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Pursuer:
                if (reset) Pursuer.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.PartTimer:
                if (reset) PartTimer.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Witness:
                if (reset) Witness.ClearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Doomsayer:
                if (reset) Doomsayer.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Arsonist:
                if (reset) Arsonist.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Jackal:
                break;
            case RoleId.Sidekick:
                setRole(target.PlayerId, (byte)RoleId.Jackal);
                break;
            case RoleId.Pavlovsowner:
                setRole(Pavlovsdogs.pavlovsowner.PlayerId, (byte)RoleId.Pavlovsdogs);
                break;
            case RoleId.Pavlovsdogs:
                break;
            case RoleId.Werewolf:
                if (reset) Werewolf.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Swooper:
                if (reset) Swooper.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Juggernaut:
                if (reset) Juggernaut.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Pelican:
                if (reset) Pelican.clearAndReload(false);
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Akujo:
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.Thief:
                if (reset) Thief.clearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.BandLeader:
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            case RoleId.SoulSight:
                if (reset) SoulSight.ClearAndReload();
                setRole(target.PlayerId, (byte)RoleId.Survivor);
                break;
            /*case RoleId.Avenger:
                Jester.Player.Add(Player);
                Survivor.Player.Add(target);
                break;*/

            case RoleId.Crewmate:
                break;
            case RoleId.Vigilante:
                if (reset) Vigilante.clearAndReload();
                break;
            case RoleId.Mayor:
                if (reset) Mayor.clearAndReload();
                break;
            case RoleId.Prosecutor:
                if (reset) Prosecutor.clearAndReload();
                break;
            case RoleId.Portalmaker:
                if (reset) Portalmaker.clearAndReload();
                break;
            case RoleId.Engineer:
                if (reset) Engineer.clearAndReload();
                break;
            case RoleId.Sheriff:
                break;
            case RoleId.Deputy:
                if (reset) Sheriff.Reload();
                break;
            case RoleId.BodyGuard:
                if (reset) BodyGuard.clearAndReload();
                break;
            case RoleId.Jumper:
                if (reset) Jumper.clearAndReload();
                break;
            case RoleId.Detective:
                if (reset) Detective.clearAndReload();
                break;
            case RoleId.Veteran:
                if (reset) Veteran.clearAndReload();
                break;
            case RoleId.Medic:
                if (reset) Medic.clearAndReload();
                break;
            case RoleId.Swapper:
                if (reset) Swapper.clearAndReload();
                break;
            case RoleId.Seer:
                if (reset) Seer.clearAndReload();
                break;
            case RoleId.Hacker:
                if (reset) Hacker.clearAndReload();
                break;
            case RoleId.Tracker:
                if (reset) Tracker.clearAndReload();
                break;
            case RoleId.Snitch:
                if (reset) Snitch.clearAndReload();
                break;
            case RoleId.Prophet:
                if (reset) Prophet.clearAndReload();
                break;
            case RoleId.Oracle:
                if (reset) Oracle.ClearAndReload();
                break;
            case RoleId.Dreamcatcher:
                if (reset) Dreamcatcher.ClearAndReload();
                break;
            case RoleId.InfoSleuth:
                break;
            case RoleId.Spy:
                if (reset) Spy.clearAndReload();
                break;
            case RoleId.SecurityGuard:
                if (reset) SecurityGuard.clearAndReload();
                break;
            case RoleId.Alchemyst:
                if (reset) Alchemyst.clearAndReload();
                break;
            case RoleId.Trapper:
                if (reset) Trapper.clearAndReload();
                break;
            case RoleId.Balancer:
                if (reset) Balancer.clearAndReload();
                break;
            case RoleId.Redemptor:
                if (reset) Redemptor.ClearAndReload();
                break;
            case RoleId.SchrodingersCat:
                if (reset) SchrodingersCat.ClearAndReload();
                break;
            case RoleId.Gunsmith:
                if (reset) Gunsmith.ClearAndReload();
                break;
            case RoleId.Berserker:
                if (reset) Berserker.ClearAndReload();
                break;
            case RoleId.Jailor:
                if (reset) Jailor.ClearAndReload();
                break;
            case RoleId.Infected:
                break;
        }

    }

    public static void sendChatToChannel(PlayerControl player, ChatControllerPatch.ChannelType channel, string message)
    {
        switch (channel)
        {
            case ChatControllerPatch.ChannelType.Default:
                break;
            case ChatControllerPatch.ChannelType.Impostor:
                if (CanSeeGhostInfo || PlayerControl.LocalPlayer.IsImpostor(AndCat: true))
                {
                    ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.ImpostorChat;
                    HudManager.Instance.Chat.AddChat(player, message);
                }
                break;
            case ChatControllerPatch.ChannelType.Jackal:
                if (CanSeeGhostInfo || PlayerControl.LocalPlayer == Jackal.Sidekick || Jackal.jackal.Any(y => y == PlayerControl.LocalPlayer))
                {
                    ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.JackalChat;
                    HudManager.Instance.Chat.AddChat(player, message);
                }
                break;
            case ChatControllerPatch.ChannelType.Pavlovs:
                if (CanSeeGhostInfo || Pavlovsdogs.pavlovsowner == PlayerControl.LocalPlayer || Pavlovsdogs.pavlovsdogs.Any(y => y == PlayerControl.LocalPlayer))
                {
                    ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.PavlovsChat;
                    HudManager.Instance.Chat.AddChat(player, message);
                }
                break;
            case ChatControllerPatch.ChannelType.Infected:
                if (CanSeeGhostInfo || Infected.Player.Any(y => y == PlayerControl.LocalPlayer))
                {
                    ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.InfectedChat;
                    HudManager.Instance.Chat.AddChat(player, message);
                }
                break;
            case ChatControllerPatch.ChannelType.Lover:
                if (CanSeeGhostInfo || Lovers.isLover(PlayerControl.LocalPlayer))
                {
                    ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.LoverChat;
                    HudManager.Instance.Chat.AddChat(player, message);
                }
                break;
            case ChatControllerPatch.ChannelType.Jailor:
                if (CanSeeGhostInfo || PlayerControl.LocalPlayer == Jailor.Player || PlayerControl.LocalPlayer == Jailor.Jailed)
                {
                    ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.JailorChat;
                    HudManager.Instance.Chat.AddChat(player, message);
                    if (PlayerControl.LocalPlayer == Jailor.Jailed) HudManager.Instance.Chat.StartCoroutine(HudManager.Instance.Chat.BounceDot());
                    SoundManager.Instance.PlaySound(HudManager.Instance?.Chat?.messageSound, false, 1f, null);
                }
                break;
        }
        ChatControllerPatch.CurrentChatType = ChatControllerPatch.ChatTypes.Default;
    }

    internal static void NoCheckEndGame(CustomGameOverReason reason)
    {
        isCanceled = true;
        Message("Game Canceled by Host");
        if (AmongUsClient.Instance.AmHost) GameManager.Instance.RpcEndGame((GameOverReason)reason, false);
    }


    public static void versionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
    {
        var ver = revision < 0 ? new Version(major, minor, build) : new Version(major, minor, build, revision);
        GameStartManagerPatch.playerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
    }

    public static void useUncheckedVent(int ventId, byte playerId, bool isEnter)
    {
        var player = PlayerById(playerId);
        if (player == null) return;
        // Fill dummy MessageReader and call MyPhysics.HandleRpc as the corountines cannot be accessed
        var reader = new MessageReader();
        var bytes = BitConverter.GetBytes(ventId);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        reader.Buffer = bytes;
        reader.Length = bytes.Length;

        JackInTheBox.startAnimation(ventId);
        player.MyPhysics.HandleRpc(isEnter ? (byte)19 : (byte)20, reader);
    }

    public static void dynamicMapOption(byte mapId)
    {
        GameOptionsManager.Instance.currentNormalGameOptions.MapId = mapId;
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

    public static void RpcFixingSabotage(TaskTypes taskType)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        FixingSabotage(taskType);
    }

    public static void FixSubmergedOxygen()
    {
        SubmergedCompatibility.RepairOxygen();
    }

    public static void showIndomitableFlash()
    {
        if (Indomitable.indomitable == PlayerControl.LocalPlayer) showFlash(Indomitable.color);
    }

    public static void cleanBody(byte playerId, byte cleaningPlayerId)
    {
        if (Alchemyst.futureDeadBodies != null)
        {
            var deadBody = Alchemyst.futureDeadBodies.Find(x => x.Item1.Player.PlayerId == playerId)?.Item1;
            if (deadBody != null) deadBody.wasCleaned = true;
        }

        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == playerId)
            {
                UObject.Destroy(array[i].gameObject);
                break;
            }
        }
        if (Vulture.vulture != null && cleaningPlayerId == Vulture.vulture.PlayerId)
        {
            Vulture.eatenBodies++;
            if (Vulture.eatenBodies == Vulture.vultureNumberToWin) Vulture.triggerVultureWin = true;
        }
        if (SoulSight.Player != null && SoulSight.Player.PlayerId == playerId && SoulSight.Reviveing)
        {
            SoulSight.Reviveing = false;
            SoulSight.CanRevive = false;
        }
    }

    public static void CreateDeadBody(byte targetId, Vector3 pos, int id)
    {
        var target = PlayerById(targetId);
        if (target == null) return;
        var deadBody = UObject.Instantiate(GameManager.Instance.DeadBodyPrefab);
        deadBody.transform.position = pos;
        deadBody.ParentId = targetId;
        //deadBody.tag = "Untagged";
        deadBody.enabled = true;
        deadBody.bodyRenderers.ForEach(delegate (SpriteRenderer b)
        {
            target.SetPlayerMaterialColors(b);
        });
        target.SetPlayerMaterialColors(deadBody.bloodSplatter);

        if (Professional.Player.IsAlive() && Butcher.butcher?.PlayerId == Professional.Player.PlayerId)
        {
            if (deadBody.gameObject.GetComponent<DeadBodyReporter.DeadBodyReporterMarker>() == null)
            {
                _ = new DeadBodyReporter(Butcher.butcher, deadBody);
            }
        }
        deadBody.gameObject.name = $"DeadBody ({target.Data.PlayerName}) {id}";
        Message($"Create DeadBody {id}");
    }

    public static void impostorPromotesToLastImpostor(byte targetId)
    {
        var target = PlayerById(targetId);
        if (target == null) return;
        setModifier(target.PlayerId, (byte)RoleId.LastImpostor);
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

    public static void survivorVestActive(byte playerId, float time)
    {
        Survivor.VestPlayer[playerId] = true;
        _ = new LateTask(() =>
        {
            Survivor.VestPlayer[playerId] = false;
        }, time);
    }

    public static void medicSetShielded(byte shieldedId)
    {
        Medic.usedShield = true;
        Medic.shielded = PlayerById(shieldedId);
        Medic.futureShielded = null;
    }

    public static void shieldedMurderAttempt(byte blank)
    {
        if (Medic.shielded == null || Medic.medic == null) return;

        var isShieldedAndShow = Medic.shielded == PlayerControl.LocalPlayer && Medic.showAttemptToShielded;
        isShieldedAndShow =
            isShieldedAndShow &&
            (Medic.meetingAfterShielding ||
             !Medic.showShieldAfterMeeting); // Dont show attempt, if shield is not shown yet
        var isMedicAndShow = Medic.medic == PlayerControl.LocalPlayer && Medic.showAttemptToMedic;

        if (isShieldedAndShow || isMedicAndShow || CanSeeGhostInfo)
            showFlash(Palette.ImpostorRed, 1.5f, GetString("medicShowAttemptText"));

        if (!Medic.unbreakableShield)
        {
            Medic.shielded = null;
            return;
        }
    }

    public static void RevivePlayer(byte targetId, bool clear, bool setPos)
    {
        var target = PlayerById(targetId);
        target?.ModRevive(clear, setPos);
    }

    public static void shifterShift(byte targetId)
    {
        var player = Shifter.shifter;
        var target = PlayerById(targetId);
        if (target == null || player == null) return;

        Shifter.clearAndReload();

        // Suicide (exile) when impostor or impostor variants
        if (Shifter.NotShift(target) && player.IsAlive())
        {
            Message($"Target Is Neutral: {Shifter.NotShift(target)}", "Shifter");
            player.Exiled();
            PlayerData.SetDeathReason(player, CustomDeathReason.Shift, target);
            if (player == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
            {
                var writer = StartRPC(CustomRPC.LawyerPromotesToPursuer);
                writer.Write(false);
                writer.EndRPC();
                Lawyer.PromotesToPursuer(false);
            }
            else if (player == Executioner.target && AmongUsClient.Instance.AmHost && Executioner.executioner != null)
            {
                var writer = StartRPC(CustomRPC.ExecutionerPromotesRole);
                writer.EndRPC();
                Executioner.PromotesRole();
            }
            return;
        }

        Shifter.shiftRole(player, target);

        // Set cooldowns to max for both players
        if (PlayerControl.LocalPlayer == player || PlayerControl.LocalPlayer == target)
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

        var closestPlayers = GetClosestPlayers(Grenadier.Player.GetTruePosition(), Grenadier.radius, true);
        Grenadier.controls = closestPlayers;
        foreach (var player in closestPlayers)
        {
            if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                if (player.IsImpostor() && !player.IsDead() && !MeetingHud.Instance)
                {
                    Grenadier.showFlash(Grenadier.flash, Grenadier.duration, 0.24f);
                }
                else if (!player.IsImpostor() && !player.IsDead() && !MeetingHud.Instance)
                {
                    Grenadier.showFlash(Grenadier.flash, Grenadier.duration, 1f);
                }
            }
        }
    }

    public static void GlitchMimic(byte playerId, bool mimic)
    {
        var target = PlayerById(playerId);
        if (mimic)
        {
            if (Glitch.Player == null || target == null) return;

            Glitch.morphTimer = Glitch.duration;
            Glitch.morphTarget = target;
            if (Camouflager.camouflageTimer <= 0f)
                Glitch.Player.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId,
                    target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId,
                    target.Data.DefaultOutfit.PetId);
        }
        else
        {
            Glitch.sampledTarget = target;
        }
    }

    public static void camouflagerCamouflage(byte setTimer)
    {
        if (isActiveCamoComms && setTimer != 2) return;
        if (isCamoComms) Camouflager.camoComms = true;
        if (Camouflager.camouflager == null && !Camouflager.camoComms) return;
        if (setTimer == 1) Camouflager.camouflageTimer = Camouflager.duration;
        if (MushroomSabotageActive) return; // Dont overwrite the fungle "camo"
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            player.setLook("", 6, "", "", "", "");
    }

    public static void vampireSetBitten(byte targetId = byte.MaxValue)
    {
        if (targetId == byte.MaxValue)
        {
            Vampire.bitten = null;
            return;
        }

        Vampire.bitten = PlayerById(targetId);
    }

    public static void partTimerSet(byte targetId)
    {
        if (targetId == byte.MaxValue) PartTimer.target = null;
        PlayerControl? target = PlayerById(targetId);
        if (target == null) return;
        PartTimer.target = target;
        PartTimer.deathTurn = PartTimer.DeathDefaultTurn;
    }

    public static void prophetExamine(byte targetId)
    {
        var target = PlayerById(targetId);
        if (target == null) return;
        if (Prophet.examined.ContainsKey(target)) Prophet.examined.Remove(target);
        Prophet.examined.Add(target, Prophet.IsRed(target));
        Prophet.examinesLeft--;
        if ((Prophet.examineNum - Prophet.examinesLeft >= Prophet.examinesToBeRevealed) && Prophet.revealProphet) Prophet.isRevealed = true;
    }

    public static void placeGarlic(Vector3 pos)
    {
        _ = new Garlic(pos);
    }

    public static void trackerUsedTracker(byte targetId)
    {
        Tracker.usedTracker = true;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            if (player.PlayerId == targetId)
                Tracker.tracked = player;
    }

    public static void deputyUsedHandcuffs(byte targetId)
    {
        //var target = PlayerById(targetId);
        Sheriff.remainingHandcuffs--;
        Sheriff.handcuffedPlayers.Add(targetId);
    }

    public static void jackalCreatesSidekick(byte targetId)
    {
        var target = PlayerById(targetId);
        if (target == null) return;
        if (Executioner.target == target && Executioner.executioner != null && !Executioner.executioner.Data.IsDead)
        {
            if (Lawyer.lawyer == null && Executioner.promotesToLawyer)
            {
                setRole(Executioner.executioner.PlayerId, (byte)RoleId.Lawyer);
                Lawyer.lawyer = Executioner.executioner;
                Lawyer.target = Executioner.target;
                Executioner.clearAndReload();
            }
            else
            {
                setRole(Executioner.executioner.PlayerId, (byte)RoleId.Pursuer);
                Executioner.clearAndReload();
            }
        }

        erasePlayerRoles(target.PlayerId);
        setRole(targetId, (byte)RoleId.Sidekick);
        Gaoler.OnImpostorDie(target);

        if (target == PlayerControl.LocalPlayer) SoundEffectsManager.play("jackalSidekick");
        if (HandleGuesser.isGuesserGm && GuesserGM.guesserGamemodeSidekickIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(targetId))
            setGuesserGm(targetId);
        if (target.AmOwner)
            jackalKillButton.Timer = jackalKillButton.MaxTimer * 0.66f;

        Jackal.canCreateSidekick = false;
    }

    public static void sidekickPromotes(byte playerId)
    {
        var player = PlayerById(playerId);
        if (player == null) return;
        setRole(playerId, (byte)RoleId.Jackal);
        Jackal.Sidekick = null;
        Jackal.canCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick;
    }

    public static void pavlovsCreateDog(byte targetId)
    {
        var target = PlayerById(targetId);
        if (target == null) return;
        if (Executioner.target == target && Executioner.executioner != null && !Executioner.executioner.Data.IsDead)
        {
            if (Lawyer.lawyer == null && Executioner.promotesToLawyer)
            {
                setRole(Executioner.executioner.PlayerId, (byte)RoleId.Lawyer);
                Lawyer.target = Executioner.target;
                Executioner.clearAndReload();
            }
            else
            {
                setRole(Executioner.executioner.PlayerId, (byte)RoleId.Pursuer);
                Executioner.clearAndReload();
            }
        }

        FastDestroyableSingleton<RoleManager>.Instance.SetRole(target, RoleTypes.Crewmate);

        erasePlayerRoles(targetId);
        setRole(targetId, (byte)RoleId.Pavlovsdogs);
        Gaoler.OnImpostorDie(target);

        if (targetId == PlayerControl.LocalPlayer.PlayerId)
            PlayerControl.LocalPlayer.moveable = true;
        if (target == PlayerControl.LocalPlayer)
            SoundEffectsManager.play("jackalSidekick");
        if (HandleGuesser.isGuesserGm && GuesserGM.guesserGamemodePavlovsdogIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(targetId))
            setGuesserGm(targetId);
        if (target.AmOwner)
            pavlovsdogsKillButton.Timer = pavlovsdogsKillButton.MaxTimer * 0.66f;
        Pavlovsdogs.createDogNum -= 1;
    }

    public static void PavlovsRing(float duration)
    {
        if (!Pavlovsdogs.pavlovsdogs.Any(x => x.AmOwner)) return;
        SoundEffectsManager.play("ring");
        pavlovsdogsKillButton.Multiplier = Pavlovsdogs.ringMultiplier;
        _ = new LateTask(() =>
        {
            pavlovsdogsKillButton.Multiplier = 1f;
        }, duration, "PavlovsRing");
    }

    /// <summary>
    /// 抹除目标玩家的职业
    /// </summary>
    public static void erasePlayerRoles(byte playerId, bool ignoreModifier = true)
    {
        var player = PlayerById(playerId);
        if (player == null) return;

        // Crewmate roles
        if (player == Swooper.swooper) Swooper.clearAndReload();
        if (player == Mayor.mayor) Mayor.clearAndReload();
        if (player == Prosecutor.prosecutor) Prosecutor.clearAndReload();
        if (player == Portalmaker.portalmaker) Portalmaker.clearAndReload();
        if (player == Jailor.Player) Jailor.ClearAndReload();
        if (player == Engineer.engineer) Engineer.clearAndReload();
        if (player == Sheriff.Deputy) Sheriff.Deputy = null;
        if (player == Detective.detective) Detective.clearAndReload();
        if (player == Veteran.veteran) Veteran.clearAndReload();
        if (player == Medic.medic) Medic.clearAndReload();
        if (player == Seer.seer) Seer.clearAndReload();
        if (player == Hacker.hacker) Hacker.clearAndReload();
        if (player == BodyGuard.bodyguard) BodyGuard.clearAndReload();
        if (player == Balancer.balancer) Balancer.clearAndReload();
        if (player == Redemptor.Player) Redemptor.ClearAndReload(false);
        if (player == Tracker.tracker) Tracker.clearAndReload();
        if (player == Snitch.snitch) Snitch.clearAndReload();
        if (player == Swapper.swapper) Swapper.clearAndReload();
        if (player == Spy.spy) Spy.clearAndReload();
        if (player == Marionette.Player) Marionette.ClearAndReload();
        if (player == SecurityGuard.securityGuard) SecurityGuard.clearAndReload();
        if (player == Alchemyst.Player) Alchemyst.clearAndReload();
        if (player == InfoSleuth.infoSleuth) InfoSleuth.clearAndReload();
        if (player == Jumper.jumper) Jumper.clearAndReload();
        if (player == Trapper.trapper) Trapper.clearAndReload();
        if (player == Oracle.Player) Oracle.ClearAndReload();
        if (player == Dreamcatcher.Player) Dreamcatcher.ClearAndReload();
        if (player == Prophet.prophet) Prophet.clearAndReload();
        if (player == Vigilante.vigilante) Vigilante.clearAndReload();

        // Impostor roles
        if (player == Glitch.Player) Glitch.clearAndReload();
        if (player == Bomber.bomber) Bomber.clearAndReload();
        if (player == Camouflager.camouflager) Camouflager.clearAndReload();
        if (player == Poucher.poucher && !Poucher.spawnModifier) Poucher.clearAndReload();
        if (player == Professional.Player && !Professional.spawnModifier) Professional.clearAndReload();
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
        if (player == Ninja.ninja) Ninja.clearAndReload();
        if (player == Yoyo.yoyo) Yoyo.clearAndReload();
        if (player == EvilTrapper.evilTrapper) EvilTrapper.clearAndReload();
        if (player == Blackmailer.Player) Blackmailer.clearAndReload();
        if (player == Terrorist.terrorist) Terrorist.clearAndReload();
        if (player == Gambler.gambler) Gambler.clearAndReload();
        if (player == Grenadier.Player) Grenadier.clearAndReload();
        if (player == Gunsmith.Player) Gunsmith.ClearAndReload();
        if (player == Berserker.Player) Berserker.ClearAndReload();
        if (player == Gaoler.Player) Gaoler.ClearAndReload();

        // Other roles
        Jester.Player.RemoveAll(x => x.PlayerId == player.PlayerId);
        if (player == Werewolf.werewolf) Werewolf.clearAndReload();
        if (player == Miner.miner) Miner.clearAndReload();
        if (player == Pelican.Player) { Pelican.PelicanDie(true, playerId); }
        if (player == Arsonist.arsonist) Arsonist.clearAndReload();
        if (Guesser.isGuesser(player.PlayerId)) Guesser.clear(player.PlayerId);

        Jackal.jackal.RemoveAll(x => x == player);

        if (player == Pavlovsdogs.pavlovsowner)
        {
            Pavlovsdogs.createDogNum = CustomOptionHolder.pavlovsownerCreateDogNum.GetInt();
            Pavlovsdogs.pavlovsowner = null;
        }
        if (player == Jackal.Sidekick) Jackal.Sidekick = null;
        if (player == BountyHunter.bountyHunter) BountyHunter.clearAndReload();
        if (player == Vulture.vulture) Vulture.clearAndReload();
        if (player == Executioner.executioner) Executioner.clearAndReload();
        if (player == Lawyer.lawyer) Lawyer.clearAndReload();
        if (player == Thief.thief) Thief.clearAndReload();
        if (player == Juggernaut.juggernaut) Juggernaut.clearAndReload();
        if (player == SoulSight.Player) SoulSight.ClearAndReload();
        if (player == Doomsayer.doomsayer) Doomsayer.clearAndReload();
        if (player == Akujo.akujo) Akujo.clearAndReload();
        if (player == Witness.Player) Witness.ClearAndReload();
        if (player == PartTimer.partTimer) PartTimer.clearAndReload();
        if (player == Vortox.Player) Vortox.ClearAndReload();
        if (player == BandLeader.Player) BandLeader.ClearAndReload();
        if (player == SchrodingersCat.Player) SchrodingersCat.ClearAndReload();
        if (player == Avenger.Player)
        {
            Avenger.ClearAndReload();
            Lovers.clearAndReload();
        }

        if (player == Shifter.shifter) Shifter.clearAndReload();

        Sheriff.Player.RemoveAll(x => x.PlayerId == player.PlayerId);
        Infected.Player.RemoveAll(x => x.PlayerId == player.PlayerId);
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
            if (player == Professional.Player && Professional.spawnModifier) Professional.clearAndReload();
            if (player == ButtonBarry.buttonBarry) ButtonBarry.clearAndReload();
            if (player == Disperser.disperser) Disperser.clearAndReload();
            if (player == Indomitable.indomitable) Indomitable.clearAndReload();
            if (player == Tunneler.tunneler) Tunneler.clearAndReload();
            if (player == Slueth.slueth) Slueth.clearAndReload();
            if (player == Blind.blind) Blind.clearAndReload();
        }
        Sheriff.deputyCheckPromotion();
    }

    public static void clearGhostRoles(byte playerId)
    {
        var player = PlayerById(playerId);

        if (player == Poltergeist.Player) Poltergeist.ClearAndReload();
        if (player == GhostEngineer.Player) GhostEngineer.ClearAndReload();
        if (player == Clog.Player) Clog.ClearAndReload();
        if (player == Specter.Player) Specter.ClearAndReload();

        var data = PlayerData.GetPlayerData(player);
        if (data != null)
        {
            data.GhostRole = null;
        }
    }

    public static void infoSleuthSetTarget(byte playerId)
    {
        var player = PlayerById(playerId);
        if (player == null)
        {
            InfoSleuth.target = null;
            return;
        }
        InfoSleuth.target = player;
    }

    public static void PlaceDecoy(PlayerControl player, Vector3 pos)
    {
        Marionette.decoy = new Decoy(player, pos);
    }

    public static void DecoyDestroy(PlayerControl player, int? decoyId)
    {
        var decoy = Decoy.AllObjects.FirstOrDefault(x => x.Id == decoyId);
        decoy?.Destroy();
        Marionette.decoy = null;
    }

    public static void DecoySwap(PlayerControl player, int? decoyId, Vector3 playerPos, Vector3 decoyPos)
    {
        var decoy = Decoy.AllObjects.FirstOrDefault(x => x.Id == decoyId);
        if (decoy?.GameObject == null || decoy?.Renderer == null) return;

        bool playerFlip = player.cosmetics.FlipX;
        bool decoyFlip = decoy.Renderer.flipX;

        player.NetTransform.SnapTo(decoyPos);
        decoy.GameObject.transform.position = playerPos;

        player.cosmetics.SetFlipX(decoyFlip);
        decoy.Renderer.flipX = playerFlip;
    }

    public static void balancerBalance(byte sourceId, byte player1Id, byte player2Id)
    {
        Balancer.IsAbilityUsed--;
        PlayerControl? source = PlayerById(sourceId);
        PlayerControl? player1 = PlayerById(player1Id);
        PlayerControl? player2 = PlayerById(player2Id);
        if (source is null || player1 is null || player2 is null) return;
        Balancer.StartAbility(source, player1, player2);

        if (MeetingHud.Instance)
        {
            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                pva.UnsetVote();
                var voteAreaPlayer = PlayerById(pva.TargetPlayerId);
                if (voteAreaPlayer != null && !voteAreaPlayer.AmOwner) continue;
                MeetingHud.Instance.ClearVote();
            }

            if (AmongUsClient.Instance.AmHost) MeetingHud.Instance.CheckForEndVoting();
        }
    }

    public static void setFutureErased(byte playerId)
    {
        var player = PlayerById(playerId);
        Eraser.futureErased ??= new List<PlayerControl>();
        if (player != null) Eraser.futureErased.Add(player);
    }

    public static void setFutureShifted(byte playerId)
    {
        Shifter.futureShift = PlayerById(playerId);
    }

    public static void disperse()
    {
        Coroutines.Start(showFlashCoroutine(Palette.ImpostorRed, 1f, 0.36f));
        var local = PlayerControl.LocalPlayer;
        if (local.inVent)
        {
            local.MyPhysics.RpcExitVent(Vent.currentVent.Id);
            local.MyPhysics.ExitAllVents();
        }

        if (Minigame.Instance) Minigame.Instance.ForceClose();
        if (MapBehaviour.Instance) MapBehaviour.Instance.Close();

        if (local.inVent)
        {
            local.MyPhysics.RpcExitVent(Vent.currentVent.Id);
            local.MyPhysics.ExitAllVents();
        }
        if (local.IsAlive() && !AntiTeleport.antiTeleport.Any(x => x == local) && !local.IsUsingTransportation)
        {
            if (Disperser.DispersesToVent)
            {
                local.NetTransform.RpcSnapTo
                    (MapData.FindVentSpawnPositions()[rnd.Next(MapData.FindVentSpawnPositions().Count)]);
            }
            else
            {
                local.NetTransform.RpcSnapTo
                    (MapData.MapSpawnPosition()[rnd.Next(MapData.MapSpawnPosition().Count)]);
            }
        }
        Disperser.remainingDisperses--;
    }

    public static void setFutureShielded(byte playerId)
    {
        Medic.futureShielded = PlayerById(playerId);
        Medic.usedShield = true;
    }

    public static void giveBomb(byte playerId, bool bomb = false)
    {
        if (playerId == byte.MaxValue)
        {
            SoundEffectsManager.stop("timemasterShield");
            Bomber.hasBombPlayer = null;
            Bomber.bombActive = false;
            Bomber.hasAlerted = false;
            Bomber.timeLeft = 0;
            return;
        }

        if (bomb)
        {
            Bomber.hasBombPlayer = PlayerById(playerId);
            Bomber.timeLeft += 0.5f;

            return;
        }

        SoundEffectsManager.stop("timemasterShield");
        if (Bomber.hasBombPlayer.IsLocalPlayer) SoundEffectsManager.play("timemasterShield");

        Bomber.hasBombPlayer = PlayerById(playerId);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.bombDelay,
            new Action<float>(p =>
            {
                if (p == 1f) Bomber.bombActive = true;
            })));
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bomber.bombDelay + Bomber.bombTimer,
            new Action<float>(p =>
            {
                // Delayed action
                if (Bomber.bomber.IsDead() || Bomber.hasBombPlayer.IsDead())
                {
                    SoundEffectsManager.stop("timemasterShield");
                    Bomber.hasBombPlayer = null;
                    Bomber.bombActive = false;
                    Bomber.hasAlerted = false;
                    Bomber.timeLeft = 0;
                    return;
                }
                if (p == 1f && Bomber.bombActive)
                {
                    SoundEffectsManager.stop("timemasterShield");
                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                    if (PlayerControl.LocalPlayer == Bomber.bomber) RpcCustomMurderPlayer(Bomber.bomber, Bomber.hasBombPlayer, false);
                    Bomber.hasBombPlayer = null;
                    Bomber.bombActive = false;
                    Bomber.hasAlerted = false;
                    Bomber.timeLeft = 0;
                }

                if (PlayerControl.LocalPlayer == Bomber.hasBombPlayer)
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
        var player = PlayerById(playerId);
        Witch.futureSpelled ??= new List<PlayerControl>();
        if (player != null) Witch.futureSpelled.Add(player);
    }

    public static void placeNinjaTrace(Vector3 pos)
    {
        _ = new NinjaTrace(pos, Ninja.traceTime);
        if (PlayerControl.LocalPlayer != Ninja.ninja)
            Ninja.ninjaMarked = null;
    }

    public static void PlaceClogGhost(PlayerControl player, Vector3 pos)
    {
        _ = new GhostObject(player, pos);
    }

    public static void setInvisible(byte playerId, byte flag)
    {
        var target = PlayerById(playerId);
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
        var canSee = PlayerControl.LocalPlayer.Data.Role.IsImpostor || PlayerControl.LocalPlayer.Data.IsDead;
        if (canSee) color.a = 0.1f;
        target.cosmetics.currentBodySprite.BodySprite.color = color;
        target.cosmetics.colorBlindText.gameObject.SetActive(false);
        target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
        Ninja.invisibleTimer = Ninja.invisibleDuration;
        Ninja.isInvisable = true;
    }

    public static void yoyoMarkLocation(Vector3 pos)
    {
        if (Yoyo.yoyo == null) return;
        Yoyo.markLocation(pos);
        new Silhouette(pos, -1, false);
    }

    public static void yoyoBlink(bool isFirstJump, Vector3 pos)
    {
        Message($"blink fistjumpo: {isFirstJump}");
        if (Yoyo.yoyo == null || Yoyo.markedLocation == null) return;
        var markedPos = (Vector3)Yoyo.markedLocation;
        Yoyo.yoyo.NetTransform.SnapTo(markedPos);

        var markedSilhouette = Silhouette.AllObjects.FirstOrDefault(s => s.GameObject!.transform.position.x == markedPos.x && s.GameObject.transform.position.y == markedPos.y);
        if (markedSilhouette != null)
            markedSilhouette.permanent = false;

        // Create Silhoutte At Start Position:
        if (isFirstJump)
        {
            Yoyo.markLocation(pos);
            new Silhouette(pos, Yoyo.blinkDuration, true);
        }
        else
        {
            new Silhouette(pos, 5, true);
            Yoyo.markedLocation = null;
        }
        if (Chameleon.chameleon.Any(x => x.PlayerId == Yoyo.yoyo.PlayerId)) // Make the Yoyo visible if chameleon!
            Chameleon.lastMoved[Yoyo.yoyo.PlayerId] = Time.time;
    }

    public static void akujoSetHonmei(byte akujoId, byte targetId)
    {
        PlayerControl? akujo = PlayerById(akujoId);
        PlayerControl? target = PlayerById(targetId);

        if (akujo != null && Akujo.honmei == null)
        {
            Akujo.honmei = target;
            Akujo.breakLovers(target);
        }
    }

    public static void akujoSetKeep(byte akujoId, byte targetId)
    {
        var akujo = PlayerById(akujoId);
        PlayerControl? target = PlayerById(targetId);

        if (akujo != null && Akujo.keepsLeft > 0)
        {
            Akujo.keeps.Add(target);
            Akujo.breakLovers(target);
            Akujo.keepsLeft--;
        }
    }

    public static void AkujoSetUnifiedVote(bool used)
    {
        Akujo.hasUsedUnifiedVote = used;
        Akujo.isUnifiedVoteActiveThisMeeting = used;
    }
    public static void akujoSuicide(byte akujoId)
    {
        var akujo = PlayerById(akujoId);
        var partnerId = Akujo.honmei?.PlayerId ?? byte.MaxValue;
        if (akujo != null)
        {
            akujo.Exiled();
            PlayerData.SetDeathReason(akujo, CustomDeathReason.Loneliness);

            if (InMeeting && Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(akujo.KillSfx, false, 0.8f);
            if (PlayerControl.LocalPlayer == Akujo.akujo)
                FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(akujo.Data, akujo.Data);
        }

        if (MeetingHud.Instance)
        {
            ExtendMeetingTime(CustomOptionHolder.guessExtendmeetingTime.GetFloat());
            MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, akujoId);

            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                bool shouldClearVote = CustomOptionHolder.guessReVote.GetBool()
                    || pva.VotedFor == akujoId || pva.VotedFor == partnerId;

                if (shouldClearVote)
                {
                    pva.UnsetVote();
                    var voteAreaPlayer = PlayerById(pva.TargetPlayerId);
                    if (voteAreaPlayer?.AmOwner == false) continue;
                    MeetingHud.Instance.ClearVote();
                }
            }
            if (AmongUsClient.Instance.AmHost) MeetingHud.Instance.CheckForEndVoting();
        }
    }

    public static void Mine(int ventId, byte[] buff, float zAxis)
    {
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));

        var ventPrefab = UObject.FindObjectOfType<Vent>();
        var vent = UObject.Instantiate(ventPrefab, ventPrefab.transform.parent);
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

    public static void setSwoop(byte playerId, bool flag)
    {
        var target = PlayerById(playerId);
        if (target == null) return;
        if (!flag)
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
        var canSee = Swooper.swooper == PlayerControl.LocalPlayer || CanSeeGhostInfo;
        if (canSee) color.a = 0.1f;
        target.cosmetics.currentBodySprite.BodySprite.color = color;
        target.cosmetics.colorBlindText.gameObject.SetActive(false);
        target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
        Swooper.swoopTimer = Swooper.duration;
        Swooper.isInvisable = true;
    }

    public static void setJackalSwoop(byte playerId, bool flag)
    {
        var target = PlayerById(playerId);
        if (target == null) return;
        if (!flag)
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
        var canSee = Jackal.jackal.Any(x => x == PlayerControl.LocalPlayer) ||
                     Jackal.Sidekick == PlayerControl.LocalPlayer || CanSeeGhostInfo;
        if (canSee) color.a = 0.1f;
        target.cosmetics.currentBodySprite.BodySprite.color = color;
        target.cosmetics.colorBlindText.gameObject.SetActive(false);
        target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
        Jackal.swoopTimer = Jackal.duration;
        Jackal.isInvisable = true;
    }

    public static void placeTrap(byte playerId, Vector3 pos)
    {
        var player = PlayerById(playerId);
        _ = new KillTrap(player, pos);
    }

    public static void setInvisibleGen(byte playerId, byte flag)
    {
        var target = PlayerById(playerId);
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
        if (PlayerControl.LocalPlayer.Data.IsDead) color.a = 0.1f;
        target.cosmetics.currentBodySprite.BodySprite.color = color;
        target.cosmetics.colorBlindText.gameObject.SetActive(false);
        //target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
    }

    public static void placePortal(Vector3 pos)
    {
        _ = new Portal(pos);
    }

    public static void usePortal(byte playerId, byte exit)
    {
        Portal.startTeleport(playerId, exit);
    }

    public static void placeJackInTheBox(PlayerControl player, Vector3 pos)
    {
        _ = new JackInTheBox(player, pos);
    }

    public static void lightsOut()
    {
        Trickster.lightsOutTimer = Trickster.lightsOutDuration;
        // If the local player is impostor indicate lights out
        if (HasImpVision(PlayerControl.LocalPlayer.Data))
            _ = new CustomMessage("TricksterLightsOut".Translate(), Trickster.lightsOutDuration);
    }

    public static void placeCamera(byte[] buff)
    {
        var referenceCamera = UObject.FindObjectOfType<SurvCamera>();
        if (referenceCamera == null) return; // Mira HQ

        SecurityGuard.remainingScrews -= SecurityGuard.camPrice;
        SecurityGuard.placedCameras++;

        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));

        var camera = UObject.Instantiate(referenceCamera);
        camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
        camera.CamName = $"Security Camera {SecurityGuard.placedCameras}";
        camera.Offset = new Vector3(0f, 0f, camera.Offset.z);
        if (GameOptionsManager.Instance.currentNormalGameOptions.MapId is 2 or 4)
            camera.transform.localRotation = new Quaternion(0, 0, 1, 1); // Polus and Airship 

        if (SubmergedCompatibility.IsSubmerged)
        {
            // remove 2d box collider of console, so that no barrier can be created. (irrelevant for now, but who knows... maybe we need it later)
            var fixConsole = camera.transform.FindChild("FixConsole");
            if (fixConsole != null)
            {
                var boxCollider = fixConsole.GetComponent<BoxCollider2D>();
                if (boxCollider != null) UObject.Destroy(boxCollider);
            }
        }


        if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard)
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
        if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard)
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
        Lawyer.target = PlayerById(playerId);
    }

    public static void executionerSetTarget(byte playerId)
    {
        Executioner.target = PlayerById(playerId);
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
        var target = PlayerById(playerId);
        Blackmailer.blackmailed = target;
    }

    public static void showBodyGuardFlash()
    {
        if (BodyGuard.guarded?.AmOwner == true) BodyGuard.guarded.ShowFailedMurder();
        if (CustomOptionHolder.bodyGuardFlash.GetBool()) showFlash(BodyGuard.color);
    }

    public static void bodyGuardGuardPlayer(byte targetId)
    {
        var target = PlayerById(targetId);
        BodyGuard.usedGuard = true;
        BodyGuard.guarded = target;
    }

    public static void SetBlanked(byte playerId, bool reset)
    {
        if (PlayerById(playerId) == null) return;

        if (reset)
            Pursuer.blankedList.Remove(playerId);
        else
            Pursuer.blankedList.Add(playerId);
    }

    public static void setFirstKill(byte playerId)
    {
        var target = PlayerById(playerId);
        if (target == null) return;
        firstKillPlayer = target;
    }

    public static void setTrap(byte playerId, byte[] buff)
    {
        var player = PlayerById(playerId);
        if (player == null) return;
        Trapper.charges -= 1;
        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        _ = new Trap(player, position);
    }

    public static void setGuesserGm(byte playerId)
    {
        var target = PlayerById(playerId);
        if (target == null) return;
        _ = new GuesserGM(target);
    }

    public static void receiveGhostInfo(byte senderId, MessageReader reader)
    {
        var sender = PlayerById(senderId);

        var infoType = (GhostInfoTypes)reader.ReadByte();
        switch (infoType)
        {
            case GhostInfoTypes.HandcuffNoticed:
                Sheriff.setHandcuffedKnows(true, senderId);
                break;
            case GhostInfoTypes.HandcuffOver:
                _ = Sheriff.handcuffedKnows.Remove(senderId);
                break;
            case GhostInfoTypes.ArsonistDouse:
                Arsonist.dousedPlayers.Add(PlayerById(reader.ReadByte()));
                break;
            case GhostInfoTypes.BountyTarget:
                BountyHunter.bounty = PlayerById(reader.ReadByte());
                break;
            case GhostInfoTypes.NinjaMarked:
                Ninja.ninjaMarked = PlayerById(reader.ReadByte());
                break;
            case GhostInfoTypes.WarlockTarget:
                Warlock.curseVictim = PlayerById(reader.ReadByte());
                break;
            case GhostInfoTypes.GhostChat:
                string chat = reader.ReadString();
                if (CanSeeGhostInfo) FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, chat);
                break;
            case GhostInfoTypes.BlankUsed:
                Pursuer.blankedList.Remove(senderId);
                break;
            case GhostInfoTypes.DeathReasonAndKiller:
                PlayerData.SetDeathReason(PlayerById(reader.ReadByte()), (CustomDeathReason)reader.ReadByte(), reader.ReadPlayer());
                break;
        }
    }

    public static void placeBomb(PlayerControl player, Vector3 pos)
    {
        _ = new Bomb(player, pos);
    }

    public static void defuseBomb(int id)
    {
        var bomb = Bomb.AllObjects.FirstOrDefault(x => x.Id == id);
        if (bomb?.GameObject == null) return;
        try
        {
            SoundEffectsManager.playAtPosition("bombDefused", bomb.GameObject.transform.position, range: Terrorist.soundRange);
        }
        catch
        {
        }

        bomb.Destroy();
        terroristButton.Timer = terroristButton.MaxTimer;
        terroristButton.IsEffectActive = false;
        terroristButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
    }

    public static void GaolerMarkPrisoner(byte prisonerId)
    {
        // 所有客户端收到此 RPC 后，设置当前囚犯
        Gaoler.currentPrisoner = PlayerById(prisonerId);
    }
}


[HarmonyPatch]
internal class RPCHandlerPatch
{
    private static string RpcName(byte callId) => callId < 80 ? ((RpcCalls)callId).ToString() : ((CustomRPC)callId).ToString();

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.StartRpcImmediately)), HarmonyPostfix]
    private static void LogSentRpc([HarmonyArgument(1)] byte callId)
    {
        if (CustomOptionHolder.logRpcSend.GetBool())
        {
            string type = callId < RPCProcedure.CustomRpcId ? "Vanilla" : "Custom";
            Info($"RpcId: {callId} Type: {type} Name: {RpcName(callId)}", "SEND");
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc)), HarmonyPrefix]
    private static bool HandleRpcPatch([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        var packetId = (CustomRPC)callId;
        try
        {
            if (CustomOptionHolder.logRpcSend.GetBool())
            {
                string type = callId < RPCProcedure.CustomRpcId ? "Vanilla" : "Custom";
                Info($"RpcId: {callId} Type: {type} Name: {RpcName(callId)} Size: {reader.Length}", "RECV");
            }
        }
        catch { }

        if (callId < RPCProcedure.CustomRpcId) return true;

        switch (packetId)
        {
            // Main Controls
            case CustomRPC.ShareOptions:
                RPCProcedure.HandleShareOptions(reader.ReadByte(), reader);
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

            case CustomRPC.DraftModePickOrder:
                RoleDraft.receivePickOrder(reader.ReadByte(), reader);
                break;

            case CustomRPC.DraftModePick:
                RoleDraft.receivePick(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
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
                RPCProcedure.useUncheckedVent(reader.ReadPackedInt32(), reader.ReadByte(), reader.ReadBoolean());
                break;

            case CustomRPC.CustomMurderPlayer:
                CustomMurderPlayer(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBoolean(), reader.ReadBoolean(), (CustomDeathReason)reader.ReadByte());
                break;

            case CustomRPC.DynamicMapOption:
                RPCProcedure.dynamicMapOption(reader.ReadByte());
                break;

            case CustomRPC.SetGameStarting:
                RPCProcedure.setGameStarting();
                break;

            case CustomRPC.ShareGameId:
                GameDataManager.Instance.GameId = reader.ReadString();
                break;

            // Role functionality

            case CustomRPC.FixLights:
                RPCProcedure.FixLights();
                break;

            case CustomRPC.FixingSabotage:
                RPCProcedure.RpcFixingSabotage((TaskTypes)reader.ReadByte());
                break;

            case CustomRPC.NoCheckEndGame:
                RPCProcedure.NoCheckEndGame((CustomGameOverReason)reader.ReadByte());
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

            case CustomRPC.CreateDeadBody:
                RPCProcedure.CreateDeadBody(reader.ReadByte(), reader.ReadVector3(), reader.ReadInt32());
                break;

            case CustomRPC.BlackmailPlayer:
                RPCProcedure.blackmailPlayer(reader.ReadByte());
                break;

            case CustomRPC.UndertakerDragAction:
                Undertaker.DragBody(reader.ReadByte());
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
                RPCProcedure.swapperSwap(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.GlitchMimic:
                RPCProcedure.GlitchMimic(reader.ReadByte(), reader.ReadBoolean());
                break;

            case CustomRPC.CamouflagerCamouflage:
                RPCProcedure.camouflagerCamouflage(reader.ReadByte());
                break;

            case CustomRPC.VampireSetBitten:
                RPCProcedure.vampireSetBitten(reader.ReadByte());
                break;

            case CustomRPC.PlaceGarlic:
                RPCProcedure.placeGarlic(reader.ReadVector3());
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
                Sheriff.replaceCurrentSheriff();
                break;

            case CustomRPC.JackalCreatesSidekick:
                RPCProcedure.jackalCreatesSidekick(reader.ReadByte());
                break;

            case CustomRPC.PavlovsCreateDog:
                RPCProcedure.pavlovsCreateDog(reader.ReadByte());
                break;

            case CustomRPC.PavlovsRing:
                RPCProcedure.PavlovsRing(reader.ReadSingle());
                break;

            case CustomRPC.SidekickPromotes:
                RPCProcedure.sidekickPromotes(reader.ReadByte());
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
                RPCProcedure.placeNinjaTrace(reader.ReadVector3());
                break;

            case CustomRPC.PlacePortal:
                RPCProcedure.placePortal(reader.ReadVector3());
                break;

            case CustomRPC.UsePortal:
                RPCProcedure.usePortal(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.PlaceJackInTheBox:
                RPCProcedure.placeJackInTheBox(reader.ReadPlayer(), reader.ReadVector3());
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

            case CustomRPC.SetBlanked:
                RPCProcedure.SetBlanked(reader.ReadByte(), reader.ReadBoolean());
                break;

            case CustomRPC.GiveBomb:
                RPCProcedure.giveBomb(reader.ReadByte(), reader.ReadBoolean());
                break;

            case CustomRPC.SetFutureSpelled:
                RPCProcedure.setFutureSpelled(reader.ReadByte());
                break;

            case CustomRPC.SetFirstKill:
                RPCProcedure.setFirstKill(reader.ReadByte());
                break;

            case CustomRPC.ShowBodyGuardFlash:
                RPCProcedure.showBodyGuardFlash();
                break;

            case CustomRPC.SetInvisible:
                RPCProcedure.setInvisible(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.SetSwoop:
                RPCProcedure.setSwoop(reader.ReadByte(), reader.ReadBoolean());
                break;

            case CustomRPC.SetJackalSwoop:
                RPCProcedure.setJackalSwoop(reader.ReadByte(), reader.ReadBoolean());
                break;

            case CustomRPC.SetInvisibleGen:
                RPCProcedure.setInvisibleGen(reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.Mine:
                RPCProcedure.Mine(reader.ReadInt32(), reader.ReadBytesAndSize(), (float)reader.ReadSingle());
                break;

            case CustomRPC.CursedTurn:
                Cursed.TurnToImpostor(reader.ReadByte());
                break;

            case CustomRPC.ThiefStealsRole:
                Thief.StealsRole(reader.ReadByte());
                break;

            case CustomRPC.SetTrap:
                RPCProcedure.setTrap(reader.ReadByte(), reader.ReadBytesAndSize());
                break;

            case CustomRPC.TriggerTrap:
                Trap.triggerTrap(reader.ReadByte(), reader.ReadInt32());
                break;

            case CustomRPC.PlaceBomb:
                RPCProcedure.placeBomb(reader.ReadPlayer(), reader.ReadVector3());
                break;

            case CustomRPC.DefuseBomb:
                RPCProcedure.defuseBomb(reader.ReadInt32());
                break;

            case CustomRPC.ShareGameMode:
                RPCProcedure.shareGameMode(reader.ReadByte());
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
            case CustomRPC.HostControl:
                RPCProcedure.HostControl(reader.ReadPlayer(), (RPCProcedure.HostCommand)reader.ReadByte(), reader);
                break;

            // Game mode
            case CustomRPC.SetGuesserGm:
                RPCProcedure.setGuesserGm(reader.ReadByte());
                break;
            case CustomRPC.ShareGhostInfo:
                RPCProcedure.receiveGhostInfo(reader.ReadByte(), reader);
                break;
            case CustomRPC.NoCheckStartMeeting:
                reader.ReadPlayer().NoCheckStartMeeting(reader.ReadPlayer()?.Data, reader.ReadBoolean());
                break;
            case CustomRPC.ProphetExamine:
                RPCProcedure.prophetExamine(reader.ReadByte());
                break;
            case CustomRPC.YoyoMarkLocation:
                RPCProcedure.yoyoMarkLocation(reader.ReadVector3());
                break;
            case CustomRPC.GrenadierFlash:
                RPCProcedure.grenadierFlash(reader.ReadBoolean());
                break;
            case CustomRPC.WitnessReport:
                Witness.WitnessReport(reader.ReadByte());
                break;
            case CustomRPC.WitnessSetTarget:
                Witness.target = PlayerById(reader.ReadByte());
                break;
            case CustomRPC.WolfLordkilled:
                WolfLord.WolfLordkilled(reader.ReadByte());
                break;
            case CustomRPC.PelicanKill:
                Pelican.PelicanKill(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.YoyoBlink:
                RPCProcedure.yoyoBlink(reader.ReadBoolean(), reader.ReadVector3());
                break;
            case CustomRPC.SetFutureReveal:
                break;
            case CustomRPC.TrapperKill:
                KillTrap.trapKill(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadInt32());
                break;
            case CustomRPC.PlaceTrap:
                RPCProcedure.placeTrap(reader.ReadByte(), reader.ReadVector3());
                break;
            case CustomRPC.ActivateTrap:
                KillTrap.activateTrap(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadInt32());
                break;
            case CustomRPC.DisableTrap:
                KillTrap.disableTrap(reader.ReadByte());
                break;
            case CustomRPC.Prosecute:
                Prosecutor.ProsecuteThisMeeting = reader.ReadBoolean();
                break;

            case CustomRPC.MayorRevealed:
                Mayor.Revealed = true;
                if (Guesser.guesserUI != null) Guesser.guesserUIExitButton.OnClick.Invoke();
                break;

            case CustomRPC.SurvivorVestActive:
                RPCProcedure.survivorVestActive(reader.ReadByte(), reader.ReadSingle());
                break;

            case CustomRPC.JackalCanSwooper:
                Jackal.canSwoop = reader.ReadBoolean();
                break;

            case CustomRPC.InfoSleuthSetTarget:
                RPCProcedure.infoSleuthSetTarget(reader.ReadByte());
                break;

            case CustomRPC.BalancerBalance:
                RPCProcedure.balancerBalance(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;

            case CustomRPC.RedemptorRevive:
                Redemptor.RevivePlayer(reader.ReadByte());
                break;

            case CustomRPC.RedemptorPrayer:
                Redemptor.RedemptorPrayer(reader.ReadBoolean());
                break;

            case CustomRPC.BandLeaderFormed:
                BandLeader.BandLeaderFormed(reader.ReadByte(), reader.ReadBoolean());
                break;

            case CustomRPC.CreateBandMember:
                BandLeader.CreateBandMember(reader.ReadByte(), reader.ReadInt32());
                break;

            case CustomRPC.RevivePlayer:
                RPCProcedure.RevivePlayer(reader.ReadByte(), reader.ReadBoolean(), reader.ReadBoolean());
                break;
            case CustomRPC.SchrodingersCatSetState:
                SchrodingersCat.State = (SchrodingersCat.CatState)reader.ReadByte();
                break;
            case CustomRPC.SyncGunsmithChange:
                Gunsmith.remainingChange = reader.ReadInt32();
                break;
            case CustomRPC.PoltergeistMove:
                Poltergeist.MoveDeadBody(reader.ReadByte(), reader.ReadVector2());
                break;
            case CustomRPC.jesterDragBody:
                Jester.DragBody(reader.ReadPlayer(), reader.ReadByte());
                break;
            case CustomRPC.InfectedTarget:
                Infected.InfectedTarget(reader.ReadByte(), reader.ReadByte(), reader.ReadInt32());
                break;
            case CustomRPC.JailorSendMessage:
                Jailor.JailorSendMessage(reader.ReadPlayer(), reader.ReadString());
                break;
            case CustomRPC.JailorJail:
                Jailor.JailPlayer(reader.ReadPlayer(), reader.ReadPlayer());
                break;
            case CustomRPC.ExiledJailed:
                Jailor.ExiledJailed(reader.ReadPlayer(), reader.ReadPlayer());
                break;
            case CustomRPC.MayorMultiVote:
                Mayor.MultiVote = reader.ReadBoolean();
                break;
            case CustomRPC.MayorSetVoteCount:
                Mayor.CurrentVote = reader.ReadInt32();
                Mayor.MyVotes = reader.ReadSingle();
                break;
            case CustomRPC.ShareFriendCode:
                GameDataManager.Instance.ShareFriendCode(reader.ReadByte(), reader.ReadString());
                break;
            case CustomRPC.ShareDeathReasonAndKiller:
                PlayerData.SetDeathReason(reader.ReadPlayer(), (CustomDeathReason)reader.ReadByte(), reader.ReadPlayer());
                break;
            case CustomRPC.GuesserMessage:
                Guesser.seedGuessChat(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadByte(), false);
                break;
            case CustomRPC.PlaceDecoy:
                RPCProcedure.PlaceDecoy(reader.ReadPlayer(), reader.ReadVector3());
                break;
            case CustomRPC.DecoyDestroy:
                RPCProcedure.DecoyDestroy(reader.ReadPlayer(), reader.ReadInt32());
                break;
            case CustomRPC.DecoySwap:
                RPCProcedure.DecoySwap(reader.ReadPlayer(), reader.ReadInt32(), reader.ReadVector3(), reader.ReadVector3());
                break;
            case CustomRPC.SetAvengerLover:
                Lovers.IsAvengerLover = reader.ReadBoolean();
                break;
            case CustomRPC.Exiled:
                reader.ReadPlayer()?.Exiled();
                break;
            case CustomRPC.JesterWinner:
                Jester.WinnerPlayer = reader.ReadPlayer();
                Jester.triggerJesterWin = true;
                break;
            case CustomRPC.PlaceClogGhost:
                RPCProcedure.PlaceClogGhost(reader.ReadPlayer(), reader.ReadVector3());
                break;
            case CustomRPC.SendChatToChannel:
                RPCProcedure.sendChatToChannel(reader.ReadPlayer(), (ChatControllerPatch.ChannelType)reader.ReadByte(), reader.ReadString());
                break;
            case CustomRPC.SetConfesser:
                {
                    var confesser = reader.ReadPlayer();
                    var roleType = (Oracle.CRoleType)reader.ReadInt32();
                    Oracle.Confesser = confesser;
                    Oracle.ConfesserType = roleType;
                }
                break;
            case CustomRPC.SendOracleReport:
                {
                    var confesser = reader.ReadPlayer();
                    var text = reader.ReadString();
                    if (Oracle.Player == null) break;
                    if (Oracle.Player.AmOwner)
                        HudManager.Instance.Chat.AddChat(confesser, text);
                    else if (CanSeeGhostInfo)
                        HudManager.Instance.Chat.AddChat(Oracle.Player, text);
                }
                break;
            case CustomRPC.SoulSightSuicide:
                SoulSight.Suicide(reader.ReadPlayer());
                break;
            case CustomRPC.SoulSightRevive:
                SoulSight.Player?.ModRevive(true, reader.ReadBoolean());
                break;
            case CustomRPC.SoulSightScore:
                SoulSight.Score = reader.ReadInt32();
                SoulSight.TriggerWin = reader.ReadBoolean();
                break;
            case CustomRPC.DreamcatcherSetDreamer:
                Dreamcatcher.SetDreamer(reader.ReadPlayer(), reader.ReadBoolean());
                break;
            case CustomRPC.GaolerMarkPrisoner:
                RPCProcedure.GaolerMarkPrisoner(reader.ReadByte());
                break;
            case CustomRPC.AkujoSetUnifiedVote:
                RPCProcedure.AkujoSetUnifiedVote(reader.ReadBoolean());
                break;
        }

        return false;
    }
}
