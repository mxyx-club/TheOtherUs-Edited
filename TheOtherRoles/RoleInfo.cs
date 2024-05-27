using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using InnerNet;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles;

public class RoleInfo
{
    public static RoleInfo impostor = new("Impostor", Palette.ImpostorRed, "ImpostorIntroDesc", "ImpostorShortDesc", RoleId.Impostor);
    public static RoleInfo assassin = new("Assassin", Palette.ImpostorRed, "AssassinIntroDesc", "AssassinShortDesc", RoleId.EvilGuesser, false, true);
    public static RoleInfo godfather = new("Godfather", Godfather.color, "GodfatherIntroDesc", "GodfatherShortDesc", RoleId.Godfather);
    public static RoleInfo mafioso = new("Mafioso", Mafioso.color, "MafiosoIntroDesc", "MafiosoShortDesc", RoleId.Mafioso);
    public static RoleInfo janitor = new("Janitor", Janitor.color, "JanitorIntroDesc", "JanitorShortDesc", RoleId.Janitor);
    public static RoleInfo morphling = new("Morphling", Morphling.color, "MorphlingIntroDesc", "MorphlingShortDesc", RoleId.Morphling);
    public static RoleInfo bomber = new("Bomber", Bomber.color, "BomberIntroDesc", "BomberShortDesc", RoleId.Bomber);
    public static RoleInfo poucher = new("Poucher", Poucher.color, "PoucherIntroDesc", "PoucherShortDesc", RoleId.Poucher, false, true);
    public static RoleInfo mimic = new("Mimic", Mimic.color, "MimicIntroDesc", "MimicShortDesc", RoleId.Mimic);
    public static RoleInfo camouflager = new("Camouflager", Camouflager.color, "CamouflagerIntroDesc", "CamouflagerShortDesc", RoleId.Camouflager);
    public static RoleInfo miner = new("Miner", Miner.color, "MinerIntroDesc", "MinerShortDesc", RoleId.Miner);
    public static RoleInfo eraser = new("Eraser", Eraser.color, "EraserIntroDesc", "EraserShortDesc", RoleId.Eraser);
    public static RoleInfo vampire = new("Vampire", Vampire.color, "VampireIntroDesc", "VampireShortDesc", RoleId.Vampire);
    public static RoleInfo cleaner = new("Cleaner", Cleaner.color, "CleanerIntroDesc", "CleanerShortDesc", RoleId.Cleaner);
    public static RoleInfo undertaker = new("Undertaker", Undertaker.color, "UndertakerIntroDesc", "UndertakerShortDesc", RoleId.Undertaker);
    public static RoleInfo escapist = new("Escapist", Escapist.color, "EscapistIntroDesc", "EscapistShortDesc", RoleId.Escapist);
    public static RoleInfo warlock = new("Warlock", Warlock.color, "WarlockIntroDesc", "WarlockShortDesc", RoleId.Warlock);
    public static RoleInfo trickster = new("Trickster", Trickster.color, "TricksterIntroDesc", "TricksterShortDesc", RoleId.Trickster);
    public static RoleInfo bountyHunter = new("BountyHunter", BountyHunter.color, "BountyHunterIntroDesc", "BountyHunterShortDesc", RoleId.BountyHunter);
    public static RoleInfo cultist = new("Cultist", Cultist.color, "CultistIntroDesc", "CultistShortDesc", RoleId.Cultist);
    public static RoleInfo follower = new("Cleaner", Cleaner.color, "CleanerIntroDesc", "CleanerShortDesc", RoleId.Follower, true);
    public static RoleInfo terrorist = new("Terrorist", Terrorist.color, "TerroristIntroDesc", "TerroristShortDesc", RoleId.Terrorist);
    public static RoleInfo blackmailer = new("Blackmailer", Blackmailer.color, "BlackmailerIntroDesc", "BlackmailerShortDesc", RoleId.Blackmailer);
    public static RoleInfo witch = new("Witch", Witch.color, "WitchIntroDesc", "WitchShortDesc", RoleId.Witch);
    public static RoleInfo ninja = new("Ninja", Ninja.color, "NinjaIntroDesc", "NinjaShortDesc", RoleId.Ninja);
    public static RoleInfo yoyo = new("Yoyo", Yoyo.color, "YoyoIntroDesc", "YoyoShortDesc", RoleId.Yoyo);
    public static RoleInfo evilTrapper = new("EvilTrapper", EvilTrapper.color, "EvilTrapperIntroDesc", "EvilTrapperShortDesc", RoleId.EvilTrapper);

    public static RoleInfo amnisiac = new("Amnisiac", Amnisiac.color, "AmnisiacIntroDesc", "AmnisiacShortDesc", RoleId.Amnisiac, true);
    public static RoleInfo jester = new("Jester", Jester.color, "JesterIntroDesc", "JesterShortDesc", RoleId.Jester, true);
    public static RoleInfo vulture = new("Vulture", Vulture.color, "VultureIntroDesc", "VultureShortDesc", RoleId.Vulture, true);
    public static RoleInfo lawyer = new("Lawyer", Lawyer.color, "LawyerIntroDesc", "LawyerShortDesc", RoleId.Lawyer, true);
    public static RoleInfo prosecutor = new("Lawyer", Lawyer.color, "LawyerIntroDesc", "LawyerShortDesc", RoleId.Prosecutor, true);
    public static RoleInfo pursuer = new("Pursuer", Pursuer.color, "PursuerIntroDesc", "PursuerShortDesc", RoleId.Pursuer, true);
    public static RoleInfo jackal = new("Jackal", Jackal.color, "JackalIntroDesc", "JackalShortDesc", RoleId.Jackal, true);
    public static RoleInfo sidekick = new("Sidekick", Sidekick.color, "SidekickIntroDesc", "SidekickShortDesc", RoleId.Sidekick, true);
    public static RoleInfo swooper = new("Swooper", Swooper.color, "SwooperIntroDesc", "SwooperShortDesc", RoleId.Swooper, true);
    public static RoleInfo arsonist = new("Arsonist", Arsonist.color, "ArsonistIntroDesc", "ArsonistShortDesc", RoleId.Arsonist, true);
    public static RoleInfo werewolf = new("Werewolf", Werewolf.color, "WerewolfIntroDesc", "WerewolfShortDesc", RoleId.Werewolf, true);
    public static RoleInfo thief = new("Thief", Thief.color, "ThiefIntroDesc", "ThiefShortDesc", RoleId.Thief, true);
    public static RoleInfo juggernaut = new("Juggernaut", Juggernaut.color, "JuggernautIntroDesc", "JuggernautShortDesc", RoleId.Juggernaut, true);
    public static RoleInfo doomsayer = new("Doomsayer", Doomsayer.color, "DoomsayerIntroDesc", "DoomsayerShortDesc", RoleId.Doomsayer, true);
    public static RoleInfo akujo = new("Akujo", Akujo.color, "AkujoIntroDesc", "AkujoShortDesc", RoleId.Akujo, true);

    public static RoleInfo crewmate = new("Crewmate", Color.white, "CrewmateIntroDesc", "CrewmateShortDesc", RoleId.Crewmate);
    public static RoleInfo goodGuesser = new("Vigilante", Guesser.color, "VigilanteIntroDesc", "VigilanteShortDesc", RoleId.NiceGuesser);
    public static RoleInfo mayor = new("Mayor", Mayor.color, "MayorIntroDesc", "MayorShortDesc", RoleId.Mayor);
    public static RoleInfo portalmaker = new("Portalmaker", Portalmaker.color, "PortalmakerIntroDesc", "PortalmakerShortDesc", RoleId.Portalmaker);
    public static RoleInfo engineer = new("Engineer", Engineer.color, "EngineerIntroDesc", "EngineerShortDesc", RoleId.Engineer);
    public static RoleInfo privateInvestigator = new("PrivateInvestigator",
                           PrivateInvestigator.color, "PrivateInvestigatorIntroDesc", "PrivateInvestigatorShortDesc", RoleId.PrivateInvestigator);
    public static RoleInfo sheriff = new("Sheriff", Sheriff.color, "SheriffIntroDesc", "SheriffShortDesc", RoleId.Sheriff);
    public static RoleInfo deputy = new("Deputy", Deputy.color, "DeputyIntroDesc", "DeputyShortDesc", RoleId.Deputy);
    public static RoleInfo bodyguard = new("BodyGuard", BodyGuard.color, "BodyGuardIntroDesc", "BodyGuardShortDesc", RoleId.BodyGuard);
    public static RoleInfo lighter = new("Lighter", Lighter.color, "LighterIntroDesc", "LighterShortDesc", RoleId.Lighter);
    public static RoleInfo jumper = new("Jumper", Jumper.color, "JumperIntroDesc", "JumperShortDesc", RoleId.Jumper);
    //public static RoleInfo magician = new("Magician", Magician.color, "MagicianIntroDesc", "MagicianShortDesc", RoleId.Magician);
    public static RoleInfo detective = new("Detective", Detective.color, "DetectiveIntroDesc", "DetectiveShortDesc", RoleId.Detective);
    public static RoleInfo timeMaster = new("TimeMaster", TimeMaster.color, "TimeMasterIntroDesc", "TimeMasterShortDesc", RoleId.TimeMaster);
    public static RoleInfo veteren = new("Veteren", Veteren.color, "VeterenIntroDesc", "VeterenShortDesc", RoleId.Veteren);
    public static RoleInfo medic = new("Medic", Medic.color, "MedicIntroDesc", "MedicShortDesc", RoleId.Medic);
    public static RoleInfo swapper = new("Swapper", Swapper.color, "SwapperIntroDesc", "SwapperShortDesc", RoleId.Swapper);
    public static RoleInfo seer = new("Seer", Seer.color, "SeerIntroDesc", "SeerShortDesc", RoleId.Seer);
    public static RoleInfo hacker = new("Hacker", Hacker.color, "HackerIntroDesc", "HackerShortDesc", RoleId.Hacker);
    public static RoleInfo tracker = new("Tracker", Tracker.color, "TrackerIntroDesc", "TrackerShortDesc", RoleId.Tracker);
    public static RoleInfo snitch = new("Snitch", Snitch.color, "SnitchIntroDesc", "SnitchShortDesc", RoleId.Snitch);
    public static RoleInfo spy = new("Spy", Spy.color, "SpyIntroDesc", "SpyShortDesc", RoleId.Spy);
    public static RoleInfo securityGuard = new("SecurityGuard", SecurityGuard.color, "SecurityGuardIntroDesc", "SecurityGuardShortDesc", RoleId.SecurityGuard);
    public static RoleInfo medium = new("Medium", Medium.color, "MediumIntroDesc", "MediumShortDesc", RoleId.Medium);
    public static RoleInfo trapper = new("Trapper", Trapper.color, "TrapperIntroDesc", "TrapperShortDesc", RoleId.Trapper);
    public static RoleInfo prophet = new("Prophet", Prophet.color, "ProphetIntroDesc", "ProphetShortDesc", RoleId.Prophet);

    // Modifier
    public static RoleInfo disperser = new("Disperser", Color.red, "DisperserIntroDesc", "DisperserShortDesc", RoleId.Disperser, false, true);
    public static RoleInfo lastImpostor = new("LastImpostor", Palette.ImpostorRed, "LastImpostorIntroDesc", "LastImpostorShortDesc", RoleId.LastImpostor, false, true);
    public static RoleInfo bloody = new("Bloody", Color.yellow, "BloodyIntroDesc", "BloodyShortDesc", RoleId.Bloody, false, true);
    public static RoleInfo antiTeleport = new("AntiTeleport", Color.yellow, "AntiTeleportIntroDesc", "AntiTeleportShortDesc", RoleId.AntiTeleport, false, true);
    public static RoleInfo tiebreaker = new("TieBreaker", Color.yellow, "TieBreakerIntroDesc", "TieBreakerShortDesc", RoleId.Tiebreaker, false, true);
    public static RoleInfo bait = new("Bait", Color.yellow, "BaitIntroDesc", "BaitShortDesc", RoleId.Bait, false, true);
    public static RoleInfo sunglasses = new("Sunglasses", Color.yellow, "SunglassesIntroDesc", "SunglassesShortDesc", RoleId.Sunglasses, false, true);
    public static RoleInfo torch = new("Torch", Color.yellow, "TorchIntroDesc", "TorchShortDesc", RoleId.Torch, false, true);
    public static RoleInfo flash = new("Flash", Color.yellow, "FlashIntroDesc", "FlashShortDesc", RoleId.Flash, false, true);
    public static RoleInfo multitasker = new("Multitasker", Color.yellow, "MultitaskerIntroDesc", "MultitaskerShortDesc", RoleId.Multitasker, false, true);
    public static RoleInfo lover = new("Lover", Lovers.color, "LoverIntroDesc", "LoverShortDesc", RoleId.Lover, false, true);
    public static RoleInfo giant = new("Giant", Color.yellow, "GiantIntroDesc", "GiantShortDesc", RoleId.Giant, false, true);
    public static RoleInfo mini = new("Mini", Color.yellow, "MiniIntroDesc", "MiniShortDesc", RoleId.Mini, false, true);
    public static RoleInfo vip = new("Vip", Color.yellow, "VipIntroDesc", "VipShortDesc", RoleId.Vip, false, true);
    public static RoleInfo indomitable = new("Indomitable", Color.yellow, "IndomitableIntroDesc", "IndomitableShortDesc", RoleId.Indomitable, false, true);
    public static RoleInfo slueth = new("Slueth", Color.yellow, "SluethIntroDesc", "SluethShortDesc", RoleId.Slueth, false, true);
    public static RoleInfo cursed = new("Cursed", Color.yellow, "CursedIntroDesc", "CursedShortDesc", RoleId.Cursed, false, true, true);
    public static RoleInfo invert = new("Invert", Color.yellow, "InvertIntroDesc", "InvertShortDesc", RoleId.Invert, false, true);
    public static RoleInfo blind = new("Blind", Color.yellow, "BlindIntroDesc", "BlindShortDesc", RoleId.Blind, false, true);
    public static RoleInfo watcher = new("Watcher", Color.yellow, "WatcherIntroDesc", "WatcherShortDesc", RoleId.Watcher, false, true);
    public static RoleInfo radar = new("Radar", Color.yellow, "RadarIntroDesc", "RadarShortDesc", RoleId.Radar, false, true);
    public static RoleInfo tunneler = new("Tunneler", Color.yellow, "TunnelerIntroDesc", "TunnelerShortDesc", RoleId.Tunneler, false, true);
    public static RoleInfo buttonBarry = new("ButtonBarry", Color.yellow, "ButtonBarryIntroDesc", "ButtonBarryShortDesc", RoleId.ButtonBarry, false, true);
    public static RoleInfo chameleon = new("Chameleon", Color.yellow, "ChameleonIntroDesc", "ChameleonShortDesc", RoleId.Chameleon, false, true);
    public static RoleInfo shifter = new("Shifter", Color.yellow, "ShifterIntroDesc", "ShifterShortDesc", RoleId.Shifter, false, true);

    //躲猫猫
    public static RoleInfo hunter = new("Hunter", Palette.ImpostorRed, "HunterIntroDesc", "HunterShortDesc", RoleId.Impostor);
    public static RoleInfo hunted = new("Hunted", Color.white, "HuntedIntroDesc", "HuntedShortDesc", RoleId.Crewmate);
    public static RoleInfo prop = new("Prop", Color.white, "PropIntroDesc", "PropShortDesc", RoleId.Crewmate);

    public static List<RoleInfo> allRoleInfos =
    [
        impostor,
        godfather,
        mafioso,
        janitor,
        morphling,
        bomber,
        mimic,
        camouflager,
        miner,
        eraser,
        vampire,
        undertaker,
        escapist,
        warlock,
        trickster,
        bountyHunter,
        cultist,
        cleaner,
        terrorist,
        blackmailer,
        witch,
        ninja,
        yoyo,

        amnisiac,
        jester,
        vulture,
        lawyer,
        prosecutor,
        pursuer,
        doomsayer,
        arsonist,
        jackal,
        sidekick,
        werewolf,
        swooper,
        juggernaut,
        akujo,
        thief,

        crewmate,
        goodGuesser,
        mayor,
        portalmaker,
        engineer,
        privateInvestigator,
        sheriff,
        deputy,
        bodyguard,
        lighter,
        jumper,
        detective,
        timeMaster,
        veteren,
        medic,
        swapper,
        seer,
        hacker,
        tracker,
        snitch,
        spy,
        securityGuard,
        medium,
        trapper,
        prophet,
        //Magician，

        lover,
        assassin,
        disperser,
        poucher,
        lastImpostor,
        bloody,
        antiTeleport,
        tiebreaker,
        bait,
        flash,
        torch,
        sunglasses,
        multitasker,
        mini,
        giant,
        vip,
        indomitable,
        slueth,
        cursed,
        invert,
        blind,
        watcher,
        radar,
        tunneler,
        buttonBarry,
        chameleon,
        shifter,
    ];


    private static string ReadmePage = "";
    public Color color;
    public string name;
    public string introDescription;
    public string shortDescription;
    public RoleId roleId;
    public bool isModifier;
    public bool isNeutral;
    public bool isGuessable;
    public bool isImpostor;

    public RoleInfo(string name, Color color, string introDescription, string shortDescription, RoleId roleId,
        bool isNeutral = false, bool isModifier = false, bool isGuessable = false, bool isImpostor = false)
    {
        this.color = color;
        this.name = name.Translate();
        this.introDescription = introDescription.Translate();
        this.shortDescription = shortDescription.Translate();
        this.roleId = roleId;
        this.isNeutral = isNeutral;
        this.isModifier = isModifier;
        this.isGuessable = isGuessable;
        this.isImpostor = isImpostor;
    }

    public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, bool showModifier = true)
    {
        var infos = new List<RoleInfo>();
        if (p == null) return infos;

        // Modifier
        if (showModifier)
        {
            // after dead modifier
            if (!CustomOptionHolder.modifiersAreHidden.getBool() || PlayerControl.LocalPlayer.Data.IsDead ||
                AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended)
            {
                if (Bait.bait.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bait);
                if (Bloody.bloody.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bloody);
                if (Vip.vip.Any(x => x.PlayerId == p.PlayerId)) infos.Add(vip);
                if (p == Tiebreaker.tiebreaker) infos.Add(tiebreaker);
                if (p == Indomitable.indomitable) infos.Add(indomitable);
                if (p == Cursed.cursed && !Cursed.showModifier) infos.Add(cursed);
            }

            if (p == Lovers.lover1 || p == Lovers.lover2) infos.Add(lover);
            if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == p.PlayerId)) infos.Add(antiTeleport);
            if (Sunglasses.sunglasses.Any(x => x.PlayerId == p.PlayerId)) infos.Add(sunglasses);
            if (Torch.torch.Any(x => x.PlayerId == p.PlayerId)) infos.Add(torch);
            if (Flash.flash.Any(x => x.PlayerId == p.PlayerId)) infos.Add(flash);
            if (Multitasker.multitasker.Any(x => x.PlayerId == p.PlayerId)) infos.Add(multitasker);
            if (p == Mini.mini) infos.Add(mini);
            if (p == Blind.blind) infos.Add(blind);
            if (p == Watcher.watcher) infos.Add(watcher);
            if (p == Radar.radar) infos.Add(radar);
            if (p == Tunneler.tunneler) infos.Add(tunneler);
            if (p == ButtonBarry.buttonBarry) infos.Add(buttonBarry);
            if (p == Slueth.slueth) infos.Add(slueth);
            if (p == Disperser.disperser) infos.Add(disperser);
            if (p == Giant.giant) infos.Add(giant);
            if (p == Poucher.poucher) infos.Add(poucher);
            if (Invert.invert.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
            if (Chameleon.chameleon.Any(x => x.PlayerId == p.PlayerId)) infos.Add(chameleon);
            if (p == Shifter.shifter) infos.Add(shifter);
            if (Guesser.evilGuesser.Any(x => x.PlayerId == p.PlayerId)) infos.Add(assassin);
            if (p == LastImpostor.lastImpostor) infos.Add(lastImpostor);
        }

        var count = infos.Count; // Save count after modifiers are added so that the role count can be checked

        // Special roles
        if (p == Jester.jester) infos.Add(jester);
        if (p == Swooper.swooper) infos.Add(swooper);
        if (p == Werewolf.werewolf) infos.Add(werewolf);
        if (p == Mayor.mayor) infos.Add(mayor);
        if (p == Portalmaker.portalmaker) infos.Add(portalmaker);
        if (p == Engineer.engineer) infos.Add(engineer);
        if (p == Sheriff.sheriff || p == Sheriff.formerSheriff) infos.Add(sheriff);
        if (p == Deputy.deputy) infos.Add(deputy);
        if (p == Lighter.lighter) infos.Add(lighter);
        if (p == Godfather.godfather) infos.Add(godfather);
        if (p == Miner.miner) infos.Add(miner);
        if (p == Mafioso.mafioso) infos.Add(mafioso);
        if (p == Janitor.janitor) infos.Add(janitor);
        if (p == Morphling.morphling) infos.Add(morphling);
        if (p == Bomber.bomber) infos.Add(bomber);
        if (p == Camouflager.camouflager) infos.Add(camouflager);
        if (p == Vampire.vampire) infos.Add(vampire);
        if (p == Eraser.eraser) infos.Add(eraser);
        if (p == Trickster.trickster) infos.Add(trickster);
        if (p == Cleaner.cleaner) infos.Add(cleaner);
        if (p == Undertaker.undertaker) infos.Add(undertaker);
        if (p == PrivateInvestigator.privateInvestigator) infos.Add(privateInvestigator);
        if (p == Mimic.mimic) infos.Add(mimic);
        if (p == Warlock.warlock) infos.Add(warlock);
        if (p == Witch.witch) infos.Add(witch);
        if (p == Escapist.escapist) infos.Add(escapist);
        if (p == Ninja.ninja) infos.Add(ninja);
        if (p == Yoyo.yoyo) infos.Add(yoyo);
        if (p == EvilTrapper.evilTrapper) infos.Add(evilTrapper);
        if (p == Blackmailer.blackmailer) infos.Add(blackmailer);
        if (p == Terrorist.terrorist) infos.Add(terrorist);
        if (p == Detective.detective) infos.Add(detective);
        if (p == TimeMaster.timeMaster) infos.Add(timeMaster);
        if (p == Cultist.cultist) infos.Add(cultist);
        if (p == Amnisiac.amnisiac) infos.Add(amnisiac);
        if (p == Veteren.veteren) infos.Add(veteren);
        if (p == Medic.medic) infos.Add(medic);
        if (p == Swapper.swapper) infos.Add(swapper);
        if (p == BodyGuard.bodyguard) infos.Add(bodyguard);
        if (p == Seer.seer) infos.Add(seer);
        if (p == Hacker.hacker) infos.Add(hacker);
        if (p == Tracker.tracker) infos.Add(tracker);
        if (p == Snitch.snitch) infos.Add(snitch);
        if (p == Jackal.jackal ||
            (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId)))
            infos.Add(jackal);
        if (p == Sidekick.sidekick) infos.Add(sidekick);
        if (p == Follower.follower) infos.Add(follower);
        if (p == Spy.spy) infos.Add(spy);
        if (p == SecurityGuard.securityGuard) infos.Add(securityGuard);
        if (p == Arsonist.arsonist) infos.Add(arsonist);
        if (p == Guesser.niceGuesser) infos.Add(goodGuesser);
        //if (p == Guesser.evilGuesser) infos.Add(badGuesser);
        if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
        if (p == Vulture.vulture) infos.Add(vulture);
        if (p == Medium.medium) infos.Add(medium);
        if (p == Lawyer.lawyer && !Lawyer.isProsecutor) infos.Add(lawyer);
        if (p == Lawyer.lawyer && Lawyer.isProsecutor) infos.Add(prosecutor);
        if (p == Trapper.trapper) infos.Add(trapper);
        if (p == Prophet.prophet) infos.Add(prophet);
        if (p == Pursuer.pursuer) infos.Add(pursuer);
        if (p == Jumper.jumper) infos.Add(jumper);
        if (p == Thief.thief) infos.Add(thief);
        //天启
        if (p == Juggernaut.juggernaut) infos.Add(juggernaut);
        if (p == Doomsayer.doomsayer) infos.Add(doomsayer);
        if (p == Akujo.akujo) infos.Add(akujo);

        // Default roles (just impostor, just crewmate, or hunter / hunted for hide n seek, prop hunt prop ...
        if (infos.Count == count)
        {
            if (p.Data.Role.IsImpostor)
                infos.Add(MapOptions.gameMode == CustomGamemodes.HideNSeek ||
                          MapOptions.gameMode == CustomGamemodes.PropHunt
                    ? hunter
                    : impostor);
            else
                infos.Add(MapOptions.gameMode == CustomGamemodes.HideNSeek ? hunted :
                    MapOptions.gameMode == CustomGamemodes.PropHunt ? prop : crewmate);
        }

        return infos;
    }

    public static string GetRolesString(PlayerControl p, bool useColors, bool showModifier = true,
        bool suppressGhostInfo = false)
    {
        string roleName;
        roleName = string.Join(" ",
            getRoleInfoForPlayer(p, showModifier).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray());
        if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId &&
            CachedPlayer.LocalPlayer.PlayerControl != Lawyer.target)
            roleName += useColors ? Helpers.cs(Pursuer.color, " §") : " §";
        if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(p.PlayerId)) roleName += " (赌怪)";

        if (!suppressGhostInfo && p != null)
        {
            if (p == Shifter.shifter &&
                (CachedPlayer.LocalPlayer.PlayerControl == Shifter.shifter || Helpers.shouldShowGhostInfo()) &&
                Shifter.futureShift != null)
                roleName += Helpers.cs(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
            if (p == Vulture.vulture && (CachedPlayer.LocalPlayer.PlayerControl == Vulture.vulture || Helpers.shouldShowGhostInfo()))
                roleName += Helpers.cs(Vulture.color, $" (剩余 {Vulture.vultureNumberToWin - Vulture.eatenBodies} )");
            if (Helpers.shouldShowGhostInfo())
            {
                if (Eraser.futureErased.Contains(p))
                    roleName = Helpers.cs(Color.gray, "(被抹除) ") + roleName;
                if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                    roleName = Helpers.cs(Vampire.color,
                        $"(被吸血 {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                if (Deputy.handcuffedPlayers.Contains(p.PlayerId))
                    roleName = Helpers.cs(Color.gray, "(被上拷) ") + roleName;
                if (Deputy.handcuffedKnows.ContainsKey(p.PlayerId)) // Active cuff
                    roleName = Helpers.cs(Deputy.color, "(被上拷) ") + roleName;
                if (p == Warlock.curseVictim)
                    roleName = Helpers.cs(Warlock.color, "(被下咒) ") + roleName;
                if (p == Ninja.ninjaMarked)
                    roleName = Helpers.cs(Ninja.color, "(被标记) ") + roleName;
                if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                    roleName = Helpers.cs(Pursuer.color, "(被塞空包弹) ") + roleName;
                if (Witch.futureSpelled.Contains(p) && !MeetingHud.Instance) // This is already displayed in meetings!
                    roleName = Helpers.cs(Witch.color, "☆ ") + roleName;
                if (BountyHunter.bounty == p)
                    roleName = Helpers.cs(BountyHunter.color, "(被悬赏) ") + roleName;
                if (Arsonist.dousedPlayers.Contains(p))
                    roleName = Helpers.cs(Arsonist.color, "♨ ") + roleName;
                if (p == Arsonist.arsonist)
                    roleName += Helpers.cs(Arsonist.color,
                        $" (剩余 {CachedPlayer.AllPlayers.Count(x => { return x.PlayerControl != Arsonist.arsonist && !x.Data.IsDead && !x.Data.Disconnected && !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); })} )");
                if (p == Jackal.fakeSidekick)
                    roleName = Helpers.cs(Sidekick.color, " (假跟班) ") + roleName;
                if (Akujo.keeps.Contains(p))
                    roleName = Helpers.cs(Color.gray, "(备胎) ") + roleName;
                if (p == Akujo.honmei)
                    roleName = Helpers.cs(Akujo.color, "(真爱) ") + roleName;

                // Death Reason on Ghosts
                if (p.Data.IsDead)
                {
                    var deathReasonString = "";
                    var deadPlayer = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == p.PlayerId);

                    Color killerColor = new();
                    if (deadPlayer != null && deadPlayer.killerIfExisting != null)
                        killerColor = getRoleInfoForPlayer(deadPlayer.killerIfExisting, false).FirstOrDefault().color;

                    if (deadPlayer != null)
                    {
                        switch (deadPlayer.deathReason)
                        {
                            case DeadPlayer.CustomDeathReason.Disconnect:
                                deathReasonString = " - 断开连接";
                                break;
                            case DeadPlayer.CustomDeathReason.SheriffKill:
                                deathReasonString = $" - 出警 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.SheriffMisfire:
                                deathReasonString = " - 走火";
                                break;
                            case DeadPlayer.CustomDeathReason.SheriffMisadventure:
                                deathReasonString = $" - 被误杀于 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Suicide:
                                deathReasonString = " - 自杀";
                                break;
                            case DeadPlayer.CustomDeathReason.BombVictim:
                                deathReasonString = " - 恐袭";
                                break;
                            case DeadPlayer.CustomDeathReason.Exile:
                                deathReasonString = " - 被驱逐";
                                break;
                            case DeadPlayer.CustomDeathReason.Kill:
                                deathReasonString =
                                    $" - 被击杀于 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Guess:
                                if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                    deathReasonString = " - 猜测错误";
                                else
                                    deathReasonString =
                                        $" - 被赌杀于 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Shift:
                                deathReasonString =
                                    $" - {Helpers.cs(Color.yellow, "交换")} {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)} 失败";
                                break;
                            case DeadPlayer.CustomDeathReason.WitchExile:
                                deathReasonString =
                                    $" - {Helpers.cs(Witch.color, "被咒杀于")} {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.LoverSuicide:
                                deathReasonString = $" - {Helpers.cs(Lovers.color, "殉情")}";
                                break;
                            case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                deathReasonString = $" - {Helpers.cs(Lawyer.color, "邪恶律师")}";
                                break;
                            case DeadPlayer.CustomDeathReason.Bomb:
                                deathReasonString =
                                    $" - 被恐袭于 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Arson:
                                deathReasonString =
                                    $" - 被烧死于 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.LoveStolen:
                                deathReasonString =
                                    $" - {Helpers.cs(Lovers.color, "爱人被夺")}";
                                break;
                            case DeadPlayer.CustomDeathReason.Loneliness:
                                deathReasonString =
                                    $" - {Helpers.cs(Akujo.color, "精力衰竭")}";
                                break;
                        }
                        roleName = roleName + deathReasonString;
                    }
                }
            }
        }

        return roleName;
    }

    public static async Task loadReadme()
    {
        if (ReadmePage == "")
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherRoles/main/README.md".GithubUrl());
            response.EnsureSuccessStatusCode();
            var httpres = await response.Content.ReadAsStringAsync();
            ReadmePage = httpres;
        }
    }

    public static string GetRoleDescription(RoleInfo roleInfo)
    {
        while (ReadmePage == "")
        {
        }

        var index = ReadmePage.IndexOf($"## {roleInfo.name}");
        var endindex = ReadmePage.Substring(index).IndexOf("### Game Options");
        return ReadmePage.Substring(index, endindex);
    }
}
