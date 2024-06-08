using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using InnerNet;
using TheOtherRoles.Modules;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles;

public class RoleInfo
{
    public string name { get { return getString(nameKey); } }
    public string introDescription { get { return getString(nameKey + "IntroDesc"); } }
    public string shortDescription { get { return getString(nameKey + "ShortDesc"); } }
    public string fullDescription { get { return getString(nameKey + "FullDesc"); } }
    public Color color;
    public RoleId roleId;
    public bool isNeutral;
    public bool isModifier;
    public bool isGuessable;
    public bool isImpostor;
    private string nameKey;
    public RoleInfo(string name, Color color, RoleId roleId, bool isNeutral = false, bool isModifier = false, bool isGuessable = false, bool isImpostor = false)
    {
        nameKey = name;
        this.color = color;
        this.roleId = roleId;
        this.isNeutral = isNeutral;
        this.isModifier = isModifier;
        this.isGuessable = isGuessable;
        this.isImpostor = isImpostor;
    }
    public static RoleInfo impostor = new("Impostor", Palette.ImpostorRed, RoleId.Impostor);
    public static RoleInfo assassin = new("Assassin", Palette.ImpostorRed, RoleId.EvilGuesser, false, true);
    public static RoleInfo morphling = new("Morphling", Morphling.color, RoleId.Morphling);
    public static RoleInfo bomber = new("Bomber", Bomber.color, RoleId.Bomber);
    public static RoleInfo poucher = new("Poucher", Poucher.color, RoleId.Poucher);
    public static RoleInfo mimic = new("Mimic", Mimic.color, RoleId.Mimic);
    public static RoleInfo camouflager = new("Camouflager", Camouflager.color, RoleId.Camouflager);
    public static RoleInfo miner = new("Miner", Miner.color, RoleId.Miner);
    public static RoleInfo eraser = new("Eraser", Eraser.color, RoleId.Eraser);
    public static RoleInfo vampire = new("Vampire", Vampire.color, RoleId.Vampire);
    public static RoleInfo cleaner = new("Cleaner", Cleaner.color, RoleId.Cleaner);
    public static RoleInfo undertaker = new("Undertaker", Undertaker.color, RoleId.Undertaker);
    public static RoleInfo escapist = new("Escapist", Escapist.color, RoleId.Escapist);
    public static RoleInfo warlock = new("Warlock", Warlock.color, RoleId.Warlock);
    public static RoleInfo trickster = new("Trickster", Trickster.color, RoleId.Trickster);
    public static RoleInfo bountyHunter = new("BountyHunter", BountyHunter.color, RoleId.BountyHunter);
    public static RoleInfo cultist = new("Cultist", Cultist.color, RoleId.Cultist);
    public static RoleInfo follower = new("Cleaner", Cleaner.color, RoleId.Follower, true);
    public static RoleInfo terrorist = new("Terrorist", Terrorist.color, RoleId.Terrorist);
    public static RoleInfo blackmailer = new("Blackmailer", Blackmailer.color, RoleId.Blackmailer);
    public static RoleInfo witch = new("Witch", Witch.color, RoleId.Witch);
    public static RoleInfo ninja = new("Ninja", Ninja.color, RoleId.Ninja);
    public static RoleInfo yoyo = new("Yoyo", Yoyo.color, RoleId.Yoyo);
    public static RoleInfo evilTrapper = new("EvilTrapper", EvilTrapper.color, RoleId.EvilTrapper);

    public static RoleInfo amnisiac = new("Amnisiac", Amnisiac.color, RoleId.Amnisiac, true);
    public static RoleInfo jester = new("Jester", Jester.color, RoleId.Jester, true);
    public static RoleInfo vulture = new("Vulture", Vulture.color, RoleId.Vulture, true);
    public static RoleInfo lawyer = new("Lawyer", Lawyer.color, RoleId.Lawyer, true);
    public static RoleInfo executioner = new("Executioner", Executioner.color, RoleId.Executioner, true);
    public static RoleInfo pursuer = new("Pursuer", Pursuer.color, RoleId.Pursuer, true);
    public static RoleInfo jackal = new("Jackal", Jackal.color, RoleId.Jackal, true);
    public static RoleInfo sidekick = new("Sidekick", Sidekick.color, RoleId.Sidekick, true);
    public static RoleInfo swooper = new("Swooper", Swooper.color, RoleId.Swooper, true);
    public static RoleInfo arsonist = new("Arsonist", Arsonist.color, RoleId.Arsonist, true);
    public static RoleInfo werewolf = new("Werewolf", Werewolf.color, RoleId.Werewolf, true);
    public static RoleInfo thief = new("Thief", Thief.color, RoleId.Thief, true);
    public static RoleInfo juggernaut = new("Juggernaut", Juggernaut.color, RoleId.Juggernaut, true);
    public static RoleInfo doomsayer = new("Doomsayer", Doomsayer.color, RoleId.Doomsayer, true);
    public static RoleInfo akujo = new("Akujo", Akujo.color, RoleId.Akujo, true);

    public static RoleInfo crewmate = new("Crewmate", Color.white, RoleId.Crewmate);
    public static RoleInfo goodGuesser = new("Vigilante", Guesser.color, RoleId.NiceGuesser);
    public static RoleInfo mayor = new("Mayor", Mayor.color, RoleId.Mayor);
    public static RoleInfo prosecutor = new("Prosecutor", Prosecutor.color, RoleId.Prosecutor);
    public static RoleInfo portalmaker = new("Portalmaker", Portalmaker.color, RoleId.Portalmaker);
    public static RoleInfo engineer = new("Engineer", Engineer.color, RoleId.Engineer);
    public static RoleInfo privateInvestigator = new("PrivateInvestigator", PrivateInvestigator.color, RoleId.PrivateInvestigator);
    public static RoleInfo sheriff = new("Sheriff", Sheriff.color, RoleId.Sheriff);
    public static RoleInfo deputy = new("Deputy", Deputy.color, RoleId.Deputy);
    public static RoleInfo bodyguard = new("BodyGuard", BodyGuard.color, RoleId.BodyGuard);
    public static RoleInfo jumper = new("Jumper", Jumper.color, RoleId.Jumper);
    public static RoleInfo magician = new("Magician", Magician.color, RoleId.Magician);
    public static RoleInfo detective = new("Detective", Detective.color, RoleId.Detective);
    public static RoleInfo timeMaster = new("TimeMaster", TimeMaster.color, RoleId.TimeMaster);
    public static RoleInfo veteren = new("Veteren", Veteren.color, RoleId.Veteren);
    public static RoleInfo medic = new("Medic", Medic.color, RoleId.Medic);
    public static RoleInfo swapper = new("Swapper", Swapper.color, RoleId.Swapper);
    public static RoleInfo seer = new("Seer", Seer.color, RoleId.Seer);
    public static RoleInfo hacker = new("Hacker", Hacker.color, RoleId.Hacker);
    public static RoleInfo tracker = new("Tracker", Tracker.color, RoleId.Tracker);
    public static RoleInfo snitch = new("Snitch", Snitch.color, RoleId.Snitch);
    public static RoleInfo spy = new("Spy", Spy.color, RoleId.Spy);
    public static RoleInfo securityGuard = new("SecurityGuard", SecurityGuard.color, RoleId.SecurityGuard);
    public static RoleInfo medium = new("Medium", Medium.color, RoleId.Medium);
    public static RoleInfo trapper = new("Trapper", Trapper.color, RoleId.Trapper);
    public static RoleInfo prophet = new("Prophet", Prophet.color, RoleId.Prophet);

    // Modifier
    public static RoleInfo lover = new("Lover", Lovers.color, RoleId.Lover, false, true);
    public static RoleInfo disperser = new("Disperser", Color.red, RoleId.Disperser, false, true);
    public static RoleInfo poucherModifier = new("Poucher", Poucher.color, RoleId.PoucherModifier, false, true);
    public static RoleInfo lastImpostor = new("LastImpostor", Palette.ImpostorRed, RoleId.LastImpostor, false, true);
    public static RoleInfo bloody = new("Bloody", Color.yellow, RoleId.Bloody, false, true);
    public static RoleInfo antiTeleport = new("AntiTeleport", Color.yellow, RoleId.AntiTeleport, false, true);
    public static RoleInfo tiebreaker = new("TieBreaker", Color.yellow, RoleId.Tiebreaker, false, true);
    public static RoleInfo bait = new("Bait", Color.yellow, RoleId.Bait, false, true);
    public static RoleInfo sunglasses = new("Sunglasses", Color.yellow, RoleId.Sunglasses, false, true);
    public static RoleInfo torch = new("Torch", Color.yellow, RoleId.Torch, false, true);
    public static RoleInfo flash = new("Flash", Color.yellow, RoleId.Flash, false, true);
    public static RoleInfo multitasker = new("Multitasker", Color.yellow, RoleId.Multitasker, false, true);
    public static RoleInfo giant = new("Giant", Color.yellow, RoleId.Giant, false, true);
    public static RoleInfo mini = new("Mini", Color.yellow, RoleId.Mini, false, true);
    public static RoleInfo vip = new("Vip", Color.yellow, RoleId.Vip, false, true);
    public static RoleInfo indomitable = new("Indomitable", Color.yellow, RoleId.Indomitable, false, true);
    public static RoleInfo slueth = new("Slueth", Color.yellow, RoleId.Slueth, false, true);
    public static RoleInfo cursed = new("Cursed", Color.yellow, RoleId.Cursed, false, true, true);
    public static RoleInfo invert = new("Invert", Color.yellow, RoleId.Invert, false, true);
    public static RoleInfo blind = new("Blind", Color.yellow, RoleId.Blind, false, true);
    public static RoleInfo watcher = new("Watcher", Color.yellow, RoleId.Watcher, false, true);
    public static RoleInfo radar = new("Radar", Color.yellow, RoleId.Radar, false, true);
    public static RoleInfo tunneler = new("Tunneler", Color.yellow, RoleId.Tunneler, false, true);
    public static RoleInfo buttonBarry = new("ButtonBarry", Color.yellow, RoleId.ButtonBarry, false, true);
    public static RoleInfo chameleon = new("Chameleon", Color.yellow, RoleId.Chameleon, false, true);
    public static RoleInfo shifter = new("Shifter", Color.yellow, RoleId.Shifter, false, true);

    //躲猫猫
    public static RoleInfo hunter = new("Hunter", Palette.ImpostorRed, RoleId.Impostor);
    public static RoleInfo hunted = new("Hunted", Color.white, RoleId.Crewmate);
    public static RoleInfo prop = new("Prop", Color.white, RoleId.Crewmate);

    public static List<RoleInfo> allRoleInfos =
    [
        impostor,
        morphling,
        bomber,
        poucher,
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
        evilTrapper,

        amnisiac,
        jester,
        vulture,
        lawyer,
        executioner,
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
        prosecutor,
        portalmaker,
        engineer,
        privateInvestigator,
        sheriff,
        deputy,
        bodyguard,
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
        magician,

        lover,
        assassin,
        poucherModifier,
        disperser,
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
    private static readonly List<RoleInfo> _AllRoleInfo = [];
    public static IReadOnlyList<RoleInfo> AllRoleInfo => _AllRoleInfo;
    public Color Color { get; set; }
    public RoleId RoleId { get; set; }
    public RoleTeam RoleTeams { get; set; }
    public CustomRoleType RoleType { get; set; } = CustomRoleType.Main;
    public Func<RoleBase> GetRole { get; set; }
    public Func<PlayerControl, RoleControllerBase> CreateRoleController { get; set; }
    public Type RoleClassType { get; set; }

    private static string ReadmePage = "";

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
            if (p == Poucher.poucher && Poucher.spawnModifier) infos.Add(poucherModifier);
            if (p == Giant.giant) infos.Add(giant);
            if (Invert.invert.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
            if (Chameleon.chameleon.Any(x => x.PlayerId == p.PlayerId)) infos.Add(chameleon);
            if (p == Shifter.shifter) infos.Add(shifter);
            if (Guesser.evilGuesser.Any(x => x.PlayerId == p.PlayerId)) infos.Add(assassin);
            if (p == LastImpostor.lastImpostor) infos.Add(lastImpostor);
        }

        var count = infos.Count; // Save count after modifiers are added so that the role count can be checked

        // Special roles
        if (p == Mimic.mimic) infos.Add(mimic);
        if (p == Jester.jester) infos.Add(jester);
        if (p == Swooper.swooper) infos.Add(swooper);
        if (p == Werewolf.werewolf) infos.Add(werewolf);
        if (p == Miner.miner) infos.Add(miner);
        if (p == Poucher.poucher && !Poucher.spawnModifier) infos.Add(poucher);
        if (p == Morphling.morphling) infos.Add(morphling);
        if (p == Bomber.bomber) infos.Add(bomber);
        if (p == Camouflager.camouflager) infos.Add(camouflager);
        if (p == Vampire.vampire) infos.Add(vampire);
        if (p == Eraser.eraser) infos.Add(eraser);
        if (p == Trickster.trickster) infos.Add(trickster);
        if (p == Cleaner.cleaner) infos.Add(cleaner);
        if (p == Undertaker.undertaker) infos.Add(undertaker);
        if (p == PrivateInvestigator.privateInvestigator) infos.Add(privateInvestigator);
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
        if (p == Jackal.jackal || (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);
        if (p == Sidekick.sidekick) infos.Add(sidekick);
        if (p == Follower.follower) infos.Add(follower);
        if (p == Spy.spy) infos.Add(spy);
        if (p == SecurityGuard.securityGuard) infos.Add(securityGuard);
        if (p == Arsonist.arsonist) infos.Add(arsonist);
        if (p == Guesser.niceGuesser) infos.Add(goodGuesser);
        if (p == Mayor.mayor) infos.Add(mayor);
        if (p == Portalmaker.portalmaker) infos.Add(portalmaker);
        if (p == Engineer.engineer) infos.Add(engineer);
        if (p == Sheriff.sheriff || p == Sheriff.formerSheriff) infos.Add(sheriff);
        if (p == Deputy.deputy) infos.Add(deputy);
        if (p == Magician.magician) infos.Add(magician);
        if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
        if (p == Vulture.vulture) infos.Add(vulture);
        if (p == Medium.medium) infos.Add(medium);
        if (p == Lawyer.lawyer) infos.Add(lawyer);
        if (p == Prosecutor.prosecutor) infos.Add(prosecutor);
        if (p == Executioner.executioner && Executioner.executioner) infos.Add(executioner);
        if (p == Trapper.trapper) infos.Add(trapper);
        if (p == Prophet.prophet) infos.Add(prophet);
        if (p == Pursuer.pursuer) infos.Add(pursuer);
        if (p == Jumper.jumper) infos.Add(jumper);
        if (p == Thief.thief) infos.Add(thief);
        if (p == Juggernaut.juggernaut) infos.Add(juggernaut);
        if (p == Doomsayer.doomsayer) infos.Add(doomsayer);
        if (p == Akujo.akujo) infos.Add(akujo);

        // Default roles (just impostor, just crewmate, or hunter / hunted for hide n seek, prop hunt prop ...
        if (infos.Count == count)
        {
            if (p.Data.Role.IsImpostor)
                infos.Add(MapOption.gameMode == CustomGamemodes.HideNSeek ||
                          MapOption.gameMode == CustomGamemodes.PropHunt
                    ? hunter
                    : impostor);
            else
                infos.Add(MapOption.gameMode == CustomGamemodes.HideNSeek ? hunted :
                    MapOption.gameMode == CustomGamemodes.PropHunt ? prop : crewmate);
        }

        return infos;
    }

    public static string GetRolesString(PlayerControl p, bool useColors, bool showModifier = true,
        bool suppressGhostInfo = false)
    {
        string roleName;
        roleName = string.Join(" ",
            getRoleInfoForPlayer(p, showModifier).Select(x => useColors ? cs(x.color, x.name) : x.name).ToArray());
        if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId &&
            CachedPlayer.LocalPlayer.PlayerControl != Lawyer.target) roleName += useColors ? cs(Lawyer.color, " §") : " §";

        if (Executioner.target != null && p.PlayerId == Executioner.target.PlayerId &&
            CachedPlayer.LocalPlayer.PlayerControl != Executioner.target) roleName += useColors ? cs(Executioner.color, " §") : " §";

        if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(p.PlayerId)) roleName += " (赌怪)";

        if (!suppressGhostInfo && p != null)
        {
            if (p == Shifter.shifter &&
                (CachedPlayer.LocalPlayer.PlayerControl == Shifter.shifter || shouldShowGhostInfo()) &&
                Shifter.futureShift != null)
                roleName += cs(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
            if (p == Vulture.vulture && (CachedPlayer.LocalPlayer.PlayerControl == Vulture.vulture || shouldShowGhostInfo()))
                roleName += cs(Vulture.color, $" (剩余 {Vulture.vultureNumberToWin - Vulture.eatenBodies} )");
            if (shouldShowGhostInfo())
            {
                if (Eraser.futureErased.Contains(p))
                    roleName = cs(Color.gray, "(被抹除) ") + roleName;
                if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                    roleName = cs(Vampire.color,
                        $"(被吸血 {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                if (Deputy.handcuffedPlayers.Contains(p.PlayerId))
                    roleName = cs(Color.gray, "(被上拷) ") + roleName;
                if (Deputy.handcuffedKnows.ContainsKey(p.PlayerId)) // Active cuff
                    roleName = cs(Deputy.color, "(被上拷) ") + roleName;
                if (p == Warlock.curseVictim)
                    roleName = cs(Warlock.color, "(被下咒) ") + roleName;
                if (p == Ninja.ninjaMarked)
                    roleName = cs(Ninja.color, "(被标记) ") + roleName;
                if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                    roleName = cs(Pursuer.color, "(被塞空包弹) ") + roleName;
                if (Witch.futureSpelled.Contains(p) && !MeetingHud.Instance) // This is already displayed in meetings!
                    roleName = cs(Witch.color, "☆ ") + roleName;
                if (BountyHunter.bounty == p)
                    roleName = cs(BountyHunter.color, "(被悬赏) ") + roleName;
                if (Arsonist.dousedPlayers.Contains(p))
                    roleName = cs(Arsonist.color, "♨ ") + roleName;
                if (p == Arsonist.arsonist)
                    roleName += cs(Arsonist.color,
                        $" (剩余 {CachedPlayer.AllPlayers.Count(x => { return x.PlayerControl != Arsonist.arsonist && !x.Data.IsDead && !x.Data.Disconnected && !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); })} )");
                if (p == Jackal.fakeSidekick)
                    roleName = cs(Sidekick.color, " (假跟班) ") + roleName;
                if (Akujo.keeps.Contains(p))
                    roleName = cs(Color.gray, "(备胎) ") + roleName;
                if (p == Akujo.honmei)
                    roleName = cs(Akujo.color, "(真爱) ") + roleName;

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
                                deathReasonString = $" - 出警 {cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.SheriffMisfire:
                                deathReasonString = " - 走火";
                                break;
                            case DeadPlayer.CustomDeathReason.SheriffMisadventure:
                                deathReasonString = $" - 被误杀于 {cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
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
                                    $" - 被击杀于 {cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Guess:
                                if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                    deathReasonString = " - 猜测错误";
                                else
                                    deathReasonString =
                                        $" - 被赌杀于 {cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Shift:
                                deathReasonString =
                                    $" - {cs(Color.yellow, "交换")} {cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)} 失败";
                                break;
                            case DeadPlayer.CustomDeathReason.WitchExile:
                                deathReasonString =
                                    $" - {cs(Witch.color, "被咒杀于")} {cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.LoverSuicide:
                                deathReasonString = $" - {cs(Lovers.color, "殉情")}";
                                break;
                            case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                deathReasonString = $" - {cs(Lawyer.color, "邪恶律师")}";
                                break;
                            case DeadPlayer.CustomDeathReason.Bomb:
                                deathReasonString =
                                    $" - 被恐袭于 {cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Arson:
                                deathReasonString =
                                    $" - 被烧死于 {cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.LoveStolen:
                                deathReasonString =
                                    $" - {cs(Lovers.color, "爱人被夺")}";
                                break;
                            case DeadPlayer.CustomDeathReason.Loneliness:
                                deathReasonString =
                                    $" - {cs(Akujo.color, "精力衰竭")}";
                                break;
                        }
                        roleName += deathReasonString;
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
            var response = await client.GetAsync("https://raw.githubusercontent.com/mxyx-club/TheOtherUs-Edited/Beta/README.md".GithubUrl());
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

    public enum RoleTeam
    {
        Crewmate,
        Impostor,
        Neutral,
        Special,
    }

    [Flags]
    public enum CustomRoleType
    {
        Main,
        Modifier,
        MainAndModifier = Main | Modifier
    }
}
