using System.Linq;
using System;
using System.Collections.Generic;
using TheOtherRoles.Players;
using static TheOtherRoles.TheOtherRoles;
using UnityEngine;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using System.Threading.Tasks;
using System.Net.Http;

namespace TheOtherRoles
{
    public class RoleInfo {
        public Color color;
        public string name;
        public string introDescription;
        public string shortDescription;
        public RoleId roleId;
        public bool isNeutral;
        public bool isGuessable;
        public bool isModifier;

        public RoleInfo(string name, Color color, string introDescription, string shortDescription, RoleId roleId, bool isNeutral = false, bool isModifier = false, bool isGuessable = false) {
            this.color = color;
            this.name = name;
            this.introDescription = introDescription;
            this.shortDescription = shortDescription;
            this.roleId = roleId;
            this.isNeutral = isNeutral;
            this.isModifier = isModifier;
            this.isGuessable = isGuessable;
        }

        public static RoleInfo jester = new RoleInfo("Jester", Jester.color, "Get voted out", "Get voted out", RoleId.Jester, true);
        public static RoleInfo werewolf = new RoleInfo("Werewolf", Werewolf.color, "Rampage and kill everyone", "Rampage and kill everyone", RoleId.Werewolf, true);
        public static RoleInfo mayor = new RoleInfo("Mayor", Mayor.color, "Your vote counts twice", "Your vote counts twice", RoleId.Mayor);
        public static RoleInfo portalmaker = new RoleInfo("Portalmaker", Portalmaker.color, "You can create portals", "You can create portals", RoleId.Portalmaker);
        public static RoleInfo engineer = new RoleInfo("Engineer",  Engineer.color, "Maintain important systems on the ship", "Repair the ship", RoleId.Engineer);
        public static RoleInfo privateInvestigator = new RoleInfo("Detective",  PrivateInvestigator.color, "See who is interacting with others", "Spy on the ship.", RoleId.PrivateInvestigator);
        public static RoleInfo sheriff = new RoleInfo("Sheriff", Sheriff.color, "Shoot the <color=#FF1919FF>Impostors</color>", "Shoot the Impostors", RoleId.Sheriff);
        public static RoleInfo bodyguard  = new RoleInfo("Body Guard", BodyGuard.color, "Protect someone with your own life", "Protect someone with your own life", RoleId.BodyGuard, false);
        public static RoleInfo deputy = new RoleInfo("Deputy", Sheriff.color, "Handcuff the <color=#FF1919FF>Impostors</color>", "Handcuff the Impostors", RoleId.Deputy);
        public static RoleInfo lighter = new RoleInfo("Lighter", Lighter.color, "Your light never goes out", "Your light never goes out", RoleId.Lighter);
        public static RoleInfo godfather = new RoleInfo("Godfather", Godfather.color, "Kill all Crewmates", "Kill all Crewmates", RoleId.Godfather);
        public static RoleInfo mafioso = new RoleInfo("Mafioso", Mafioso.color, "Work with the <color=#FF1919FF>Mafia</color> to kill the Crewmates", "Kill all Crewmates", RoleId.Mafioso);
        public static RoleInfo janitor = new RoleInfo("Janitor", Janitor.color, "Work with the <color=#FF1919FF>Mafia</color> by hiding dead bodies", "Hide dead bodies", RoleId.Janitor);
        public static RoleInfo morphling = new RoleInfo("Morphling", Morphling.color, "Change your look to not get caught", "Change your look", RoleId.Morphling);
        public static RoleInfo bomber2 = new RoleInfo("Bomber", Bomber2.color, "Give bombs to players", "Bomb Everyone", RoleId.Bomber2);
        public static RoleInfo poucher = new RoleInfo("Poucher", Poucher.color, "Keep info on the players you kill", "Investigate the kills", RoleId.Poucher);
        public static RoleInfo mimic = new RoleInfo("Mimic", Mimic.color, "Pose as a crewmate by killing one", "Pose as a crewmate", RoleId.Mimic);
        public static RoleInfo camouflager = new RoleInfo("Camouflager", Camouflager.color, "Camouflage and kill the Crewmates", "Hide among others", RoleId.Camouflager);
        public static RoleInfo miner = new RoleInfo("Miner", Miner.color, "Make new Vents", "Create Vents", RoleId.Miner);
        public static RoleInfo vampire = new RoleInfo("Vampire", Vampire.color, "Kill the Crewmates with your bites", "Bite your enemies", RoleId.Vampire);
        public static RoleInfo eraser = new RoleInfo("Eraser", Eraser.color, "Kill the Crewmates and erase their roles", "Erase the roles of your enemies", RoleId.Eraser);
        public static RoleInfo trickster = new RoleInfo("Trickster", Trickster.color, "Use your jack-in-the-boxes to surprise others", "Surprise your enemies", RoleId.Trickster);
        public static RoleInfo cleaner = new RoleInfo("Cleaner", Cleaner.color, "Kill everyone and leave no traces", "Clean up dead bodies", RoleId.Cleaner);
        public static RoleInfo undertaker = new RoleInfo("Undertaker", Undertaker.color, "Kill everyone and leave no traces", "Drag up dead bodies to hide them", RoleId.Undertaker);
        public static RoleInfo warlock = new RoleInfo("Warlock", Warlock.color, "Curse other players and kill everyone", "Curse and kill everyone", RoleId.Warlock);
        public static RoleInfo bountyHunter = new RoleInfo("Bounty Hunter", BountyHunter.color, "Hunt your bounty down", "Hunt your bounty down", RoleId.BountyHunter);
        public static RoleInfo jumper = new RoleInfo("Jumper", Jumper.color, "Surprise the <color=#FF1919FF>Impostors</color>", "Surprise the Impostors", RoleId.Jumper);
        public static RoleInfo detective = new RoleInfo("Investigator", Detective.color, "Find the <color=#FF1919FF>Impostors</color> by examining footprints", "Examine footprints", RoleId.Detective);
        public static RoleInfo timeMaster = new RoleInfo("Time Master", TimeMaster.color, "Save yourself with your time shield", "Use your time shield", RoleId.TimeMaster);
        public static RoleInfo veteren = new RoleInfo("Veteran", Veteren.color, "Protect yourself from other", "Protect yourself from others", RoleId.Veteren);
        public static RoleInfo medic = new RoleInfo("Medic", Medic.color, "Protect someone with your shield", "Protect other players", RoleId.Medic);
        public static RoleInfo swapper = new RoleInfo("Swapper", Swapper.color, "Swap votes to exile the <color=#FF1919FF>Impostors</color>", "Swap votes", RoleId.Swapper);
        public static RoleInfo seer = new RoleInfo("Seer", Seer.color, "You will see players die", "You will see players die", RoleId.Seer);
        public static RoleInfo hacker = new RoleInfo("Hacker", Hacker.color, "Hack systems to find the <color=#FF1919FF>Impostors</color>", "Hack to find the Impostors", RoleId.Hacker);
        public static RoleInfo tracker = new RoleInfo("Tracker", Tracker.color, "Track the <color=#FF1919FF>Impostors</color> down", "Track the Impostors down", RoleId.Tracker);
        public static RoleInfo snitch = new RoleInfo("Snitch", Snitch.color, "Finish your tasks to find the <color=#FF1919FF>Impostors</color>", "Finish your tasks", RoleId.Snitch);
        //public static RoleInfo jackal = new RoleInfo("Jackal", Jackal.color, "Kill all Crewmates and <color=#FF1919FF>Impostors</color> to win", "Kill everyone", RoleId.Jackal, true);
        public static RoleInfo jackal = new RoleInfo("Jackal", Jackal.color, "Kill all Crewmates and <color=#FF1919FF>Impostors</color> to win", "Kill everyone", RoleId.Jackal, true);
        public static RoleInfo sidekick = new RoleInfo("Sidekick", Sidekick.color, "Help your Jackal to kill everyone", "Help your Jackal to kill everyone", RoleId.Sidekick, true);
        public static RoleInfo follower = new RoleInfo("Follower", Cleaner.color, "Follow your leader", "Follow your leader", RoleId.Follower, true);
        public static RoleInfo spy = new RoleInfo("Spy", Spy.color, "Confuse the <color=#FF1919FF>Impostors</color>", "Confuse the Impostors", RoleId.Spy);
        public static RoleInfo securityGuard = new RoleInfo("Security Guard", SecurityGuard.color, "Seal vents and place cameras", "Seal vents and place cameras", RoleId.SecurityGuard);
        public static RoleInfo arsonist = new RoleInfo("Arsonist", Arsonist.color, "Let them burn", "Let them burn", RoleId.Arsonist, true);
        public static RoleInfo amnisiac = new RoleInfo("Amnesiac", Amnisiac.color, "Steal roles from the dead", "Steal roles from the dead", RoleId.Amnisiac, true);
        public static RoleInfo goodGuesser = new RoleInfo("Vigilante", Guesser.color, "Guess and shoot", "Guess and shoot", RoleId.NiceGuesser);
        //public static RoleInfo badGuesser = new RoleInfo("Evil Guesser", Palette.ImpostorRed, "Guess and shoot", "Guess and shoot", RoleId.EvilGuesser);
        public static RoleInfo vulture = new RoleInfo("Vulture", Vulture.color, "Eat corpses to win", "Eat dead bodies", RoleId.Vulture, true);
        public static RoleInfo medium = new RoleInfo("Medium", Medium.color, "Question the souls of the dead to gain information", "Question the souls", RoleId.Medium);
        public static RoleInfo trapper = new RoleInfo("Trapper", Trapper.color, "Place traps to find the Impostors", "Place traps", RoleId.Trapper);
        public static RoleInfo lawyer = new RoleInfo("Lawyer", Lawyer.color, "Defend your client", "Defend your client", RoleId.Lawyer, true);
        public static RoleInfo prosecutor = new RoleInfo("Prosecutor", Lawyer.color, "Vote out your target", "Vote out your target", RoleId.Prosecutor, true);
        public static RoleInfo pursuer = new RoleInfo("Pursuer", Pursuer.color, "Blank the Impostors", "Blank the Impostors", RoleId.Pursuer);
        public static RoleInfo impostor = new RoleInfo("Impostor", Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, "Sabotage and kill everyone"), "Sabotage and kill everyone", RoleId.Impostor);
        public static RoleInfo crewmate = new RoleInfo("Crewmate", Color.white, "Find the Impostors", "Find the Impostors", RoleId.Crewmate);
        public static RoleInfo witch = new RoleInfo("Witch", Witch.color, "Cast a spell upon your foes", "Cast a spell upon your foes", RoleId.Witch);
        public static RoleInfo escapist = new RoleInfo("Escapist", Escapist.color, "Get away from kills with ease", "Teleport to get away from bodies", RoleId.Escapist);
        public static RoleInfo cultist = new RoleInfo("Cultist", Cultist.color, "Recruit for your cause", "Recruit for your cause", RoleId.Cultist);
        public static RoleInfo ninja = new RoleInfo("Ninja", Ninja.color, "Surprise and assassinate your foes", "Surprise and assassinate your foes", RoleId.Ninja);
        public static RoleInfo blackmailer = new RoleInfo("Blackmailer", Blackmailer.color, "Blackmail those who seek to hurt you", "Blackmail those who seek to hurt you", RoleId.Blackmailer);
        public static RoleInfo thief = new RoleInfo("Thief", Thief.color, "Steal a killers role by killing them", "Steal a killers role", RoleId.Thief, true);
        public static RoleInfo bomber = new RoleInfo("Terrorist", Bomber.color, "Bomb all Crewmates", "Bomb all Crewmates", RoleId.Bomber);

        public static RoleInfo hunter = new RoleInfo("Hunter", Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, "Seek and kill everyone"), "Seek and kill everyone", RoleId.Impostor);
        public static RoleInfo hunted = new RoleInfo("Hunted", Color.white, "Hide", "Hide", RoleId.Crewmate);

        public static RoleInfo prop = new RoleInfo("Prop", Color.white, "Disguise As An Object and Survive", "Disguise As An Object", RoleId.Crewmate);



        // Modifier
        public static RoleInfo bloody = new RoleInfo("Bloody", Color.yellow, "Your killer leaves a bloody trail", "Your killer leaves a bloody trail", RoleId.Bloody, false, true);
        public static RoleInfo antiTeleport = new RoleInfo("Anti tp", Color.yellow, "You will not get teleported", "You will not get teleported", RoleId.AntiTeleport, false, true);
        public static RoleInfo tiebreaker = new RoleInfo("Tiebreaker", Color.yellow, "Your vote breaks the tie", "Break the tie", RoleId.Tiebreaker, false, true);
        public static RoleInfo bait = new RoleInfo("Bait", Color.yellow, "Bait your enemies", "Bait your enemies", RoleId.Bait, false, true);
        public static RoleInfo sunglasses = new RoleInfo("Sunglasses", Color.yellow, "You got the sunglasses", "Your vision is reduced", RoleId.Sunglasses, false, true);
        public static RoleInfo torch = new RoleInfo("Torch", Color.yellow, "You got the torch", "You can see in the dark", RoleId.Torch, false, true);
        public static RoleInfo multitasker = new RoleInfo("Multitasker", Color.yellow, "Your task windows are transparent", "Your task windows are transparent", RoleId.Multitasker, false, true);
        public static RoleInfo lover = new RoleInfo("Lover", Lovers.color, $"You are in love", $"You are in love", RoleId.Lover, false, true);
        public static RoleInfo mini = new RoleInfo("Mini", Color.yellow, "No one will harm you until you grow up", "No one will harm you", RoleId.Mini, false, true);
        public static RoleInfo vip = new RoleInfo("VIP", Color.yellow, "You are the VIP", "Everyone is notified when you die", RoleId.Vip, false, true);
        public static RoleInfo indomitable  = new RoleInfo("Indomitable", Color.yellow, "Your role cannot be guessed", "You are Indomitable!", RoleId.Indomitable, false, true);
        public static RoleInfo slueth  = new RoleInfo("Sleuth", Color.yellow, "Learn the roles of bodies you report", "You know the roles of bodies you report", RoleId.Slueth, false, true);
        
        public static RoleInfo cursed  = new RoleInfo("Fanatic", Color.yellow, "You are crewmate....for now", "Discover your true potential", RoleId.Cursed, false, true, true);
        public static RoleInfo invert = new RoleInfo("Invert", Color.yellow, "Your movement is inverted", "Your movement is inverted", RoleId.Invert, false, true);
        public static RoleInfo blind  = new RoleInfo("Blind", Color.yellow, "You cannot see your report button!", "Was that a dead body?", RoleId.Blind, false, true);
        public static RoleInfo watcher  = new RoleInfo("Watcher", Color.yellow, "You can see everyone's votes during meetings", "Pay close attention to the crew's votes", RoleId.Watcher, false, true);
        public static RoleInfo radar  = new RoleInfo("Radar", Color.yellow, "Be on high alert", "Be on high alert", RoleId.Radar, false, true);
        public static RoleInfo tunneler  = new RoleInfo("Tunneler", Color.yellow, "Complete your tasks to gain the ability to vent", "Finish work so you can play", RoleId.Tunneler, false, true);
        public static RoleInfo disperser = new RoleInfo("Disperser", Color.red, "Separate the Crew", "Separate the Crew", RoleId.Disperser, false, true);
        public static RoleInfo chameleon = new RoleInfo("Chameleon", Color.yellow, "You're hard to see when not moving", "You're hard to see when not moving", RoleId.Chameleon, false, true);
        public static RoleInfo shifter = new RoleInfo("Shifter", Color.yellow, "Shift your role", "Shift your role", RoleId.Shifter, false, true);
        public static RoleInfo swooper = new RoleInfo("Swooper", Swooper.color, "Turn Invisible and kill everyone", "Turn Invisible", RoleId.Swooper, false, true);
        public static RoleInfo assassin = new RoleInfo("Assassin", Color.red, "Guess and shoot", "Guess and shoot", RoleId.EvilGuesser, false, true);
        


        public static List<RoleInfo> allRoleInfos = new List<RoleInfo>() {
            impostor,
            godfather,
            mafioso,
            janitor,
            morphling,
            bomber2,
            camouflager,
            vampire,
            eraser,
            trickster,
            cleaner,
            undertaker,
            warlock,
            werewolf,
            cursed,
            bountyHunter,
            witch,
            escapist,
            ninja,
            bodyguard,
            blackmailer,
            miner,
            bomber,
            goodGuesser,
            privateInvestigator,
            mimic,
            poucher,
            assassin,
            //badGuesser,
            disperser,
            cultist,
            lover,
            jester,
            arsonist,
            jackal,
            sidekick,
            vulture,
            pursuer,
            lawyer,
            thief,
            prosecutor,
            crewmate,
            mayor,
            portalmaker,
            engineer,
            sheriff,
            jumper,
            deputy,
            lighter,
            detective,
            timeMaster,
            amnisiac,
            veteren,
            medic,
            swapper,
            seer,
            hacker,
            tracker,
            snitch,
            spy,
            securityGuard,
            bait,
            medium,
            trapper,
            bloody,
            antiTeleport,
            tiebreaker,
            sunglasses,
            torch,
            multitasker,
            mini,
            vip,
            indomitable,
            slueth,
            //swooper,
            blind,
            watcher,
            radar,
            tunneler,
            invert,
            chameleon,
            shifter
        };

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, bool showModifier = true) {
            List<RoleInfo> infos = new List<RoleInfo>();
            if (p == null) return infos;

            // Modifier
            if (showModifier) {
                // after dead modifier
                if (!CustomOptionHolder.modifiersAreHidden.getBool() || PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended)
                {
                    if (Bait.bait.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bait);
                    if (Bloody.bloody.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bloody);
                    if (Vip.vip.Any(x => x.PlayerId == p.PlayerId)) infos.Add(vip);
                    if (p == Tiebreaker.tiebreaker) infos.Add(tiebreaker);
                    if (p == Indomitable.indomitable) infos.Add(indomitable);
                    if (p == Cursed.cursed) infos.Add(cursed);
                }
                if (p == Lovers.lover1 || p == Lovers.lover2) infos.Add(lover);
                if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == p.PlayerId)) infos.Add(antiTeleport);
                if (Sunglasses.sunglasses.Any(x => x.PlayerId == p.PlayerId)) infos.Add(sunglasses);
                if (Torch.torch.Any(x => x.PlayerId == p.PlayerId)) infos.Add(torch);
                if (Multitasker.multitasker.Any(x => x.PlayerId == p.PlayerId)) infos.Add(multitasker);
                if (p == Mini.mini) infos.Add(mini);
                if (p == Blind.blind) infos.Add(blind);
                if (p == Watcher.watcher) infos.Add(watcher);
                if (p == Radar.radar) infos.Add(radar);
                if (p == Tunneler.tunneler) infos.Add(tunneler);
                if (p == Slueth.slueth) infos.Add(slueth);
                if (p == Swooper.swooper) infos.Add(swooper);
                if (p == Disperser.disperser) infos.Add(disperser);
                if (Invert.invert.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
                if (Chameleon.chameleon.Any(x => x.PlayerId == p.PlayerId)) infos.Add(chameleon);
                if (p == Shifter.shifter) infos.Add(shifter);
                if (Guesser.evilGuesser.Any(x => x.PlayerId == p.PlayerId)) infos.Add(assassin);
            }

            int count = infos.Count;  // Save count after modifiers are added so that the role count can be checked

            // Special roles
            if (p == Jester.jester) infos.Add(jester);
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
            if (p == Bomber2.bomber2) infos.Add(bomber2);
            if (p == Camouflager.camouflager) infos.Add(camouflager);
            if (p == Vampire.vampire) infos.Add(vampire);
            if (p == Eraser.eraser) infos.Add(eraser);
            if (p == Trickster.trickster) infos.Add(trickster);
            if (p == Cleaner.cleaner) infos.Add(cleaner);
            if (p == Undertaker.undertaker) infos.Add(undertaker);
            if (p == PrivateInvestigator.privateInvestigator) infos.Add(privateInvestigator);
            if (p == Poucher.poucher) infos.Add(poucher);
            if (p == Mimic.mimic) infos.Add(mimic);
            if (p == Warlock.warlock) infos.Add(warlock);
            if (p == Witch.witch) infos.Add(witch);
            if (p == Escapist.escapist) infos.Add(escapist);
            if (p == Ninja.ninja) infos.Add(ninja);
            if (p == Blackmailer.blackmailer) infos.Add(blackmailer);
            if (p == Bomber.bomber) infos.Add(bomber);
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
            //if (p == Guesser.evilGuesser) infos.Add(badGuesser);
            if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
            if (p == Vulture.vulture) infos.Add(vulture);
            if (p == Medium.medium) infos.Add(medium);
            if (p == Lawyer.lawyer && !Lawyer.isProsecutor) infos.Add(lawyer);
            if (p == Lawyer.lawyer && Lawyer.isProsecutor) infos.Add(prosecutor);
            if (p == Trapper.trapper) infos.Add(trapper);
            if (p == Pursuer.pursuer) infos.Add(pursuer);
            if (p == Jumper.jumper) infos.Add(jumper);
            if (p == Thief.thief) infos.Add(thief);

            // Default roles (just impostor, just crewmate, or hunter / hunted for hide n seek, prop hunt prop ...
            if (infos.Count == count) {
                if (p.Data.Role.IsImpostor)
                    infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek || TORMapOptions.gameMode == CustomGamemodes.PropHunt ? RoleInfo.hunter : RoleInfo.impostor);
                else
                    infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek ? RoleInfo.hunted : TORMapOptions.gameMode == CustomGamemodes.PropHunt ? RoleInfo.prop : RoleInfo.crewmate);
            }

            return infos;
        }

        public static String GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, bool suppressGhostInfo = false) {
            string roleName;
            roleName = String.Join(" ", getRoleInfoForPlayer(p, showModifier).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray());
            if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId && CachedPlayer.LocalPlayer.PlayerControl != Lawyer.target) 
                roleName += (useColors ? Helpers.cs(Pursuer.color, " §") : " §");
            if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(p.PlayerId)) roleName += " (Guesser)";

            if (!suppressGhostInfo && p != null) {
                if (p == Shifter.shifter && (CachedPlayer.LocalPlayer.PlayerControl == Shifter.shifter || Helpers.shouldShowGhostInfo()) && Shifter.futureShift != null)
                    roleName += Helpers.cs(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
                if (p == Vulture.vulture && (CachedPlayer.LocalPlayer.PlayerControl == Vulture.vulture || Helpers.shouldShowGhostInfo()))
                    roleName = roleName + Helpers.cs(Vulture.color, $" ({Vulture.vultureNumberToWin - Vulture.eatenBodies} left)");
                if (Helpers.shouldShowGhostInfo()) {
                    if (Eraser.futureErased.Contains(p))
                        roleName = Helpers.cs(Color.gray, "(erased) ") + roleName;
                    if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                        roleName = Helpers.cs(Vampire.color, $"(bitten {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                    if (Deputy.handcuffedPlayers.Contains(p.PlayerId))
                        roleName = Helpers.cs(Color.gray, "(cuffed) ") + roleName;
                    if (Deputy.handcuffedKnows.ContainsKey(p.PlayerId))  // Active cuff
                        roleName = Helpers.cs(Deputy.color, "(cuffed) ") + roleName;
                    if (p == Warlock.curseVictim)
                        roleName = Helpers.cs(Warlock.color, "(cursed) ") + roleName;
                    if (p == Ninja.ninjaMarked)
                        roleName = Helpers.cs(Ninja.color, "(marked) ") + roleName;
                    if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                        roleName = Helpers.cs(Pursuer.color, "(blanked) ") + roleName;
                    if (Witch.futureSpelled.Contains(p) && !MeetingHud.Instance) // This is already displayed in meetings!
                        roleName = Helpers.cs(Witch.color, "☆ ") + roleName;
                    if (BountyHunter.bounty == p)
                        roleName = Helpers.cs(BountyHunter.color, "(bounty) ") + roleName;
                    if (Arsonist.dousedPlayers.Contains(p))
                        roleName = Helpers.cs(Arsonist.color, "♨ ") + roleName;
                    if (p == Arsonist.arsonist)
                        roleName = roleName + Helpers.cs(Arsonist.color, $" ({CachedPlayer.AllPlayers.Count(x => { return x.PlayerControl != Arsonist.arsonist && !x.Data.IsDead && !x.Data.Disconnected && !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); })} left)");
                    if (p == Jackal.fakeSidekick)
                        roleName = Helpers.cs(Sidekick.color, $" (fake SK) ") + roleName;
                        /*
                    if ((p == Swooper.swooper) && Jackal.canSwoop2)
                        roleName = Helpers.cs(Swooper.color, $" (Swooper) ") + roleName;
                        */

                    // Death Reason on Ghosts
                    if (p.Data.IsDead) {
                        string deathReasonString = "";
                        var deadPlayer = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == p.PlayerId);

                        Color killerColor = new();
                        if (deadPlayer != null && deadPlayer.killerIfExisting != null) {
                            killerColor = RoleInfo.getRoleInfoForPlayer(deadPlayer.killerIfExisting, false).FirstOrDefault().color;
                        }

                        if (deadPlayer != null) {
                            switch (deadPlayer.deathReason) {
                                case DeadPlayer.CustomDeathReason.Disconnect:
                                    deathReasonString = " - disconnected";
                                    break;
                                case DeadPlayer.CustomDeathReason.Exile:
                                    deathReasonString = " - voted out";
                                    break;
                                case DeadPlayer.CustomDeathReason.Kill:
                                    deathReasonString = $" - killed by {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Guess:
                                    if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                        deathReasonString = $" - failed guess";
                                    else
                                        deathReasonString = $" - guessed by {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Shift:
                                    deathReasonString = $" - {Helpers.cs(Color.yellow, "shifted")} {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.WitchExile:
                                    deathReasonString = $" - {Helpers.cs(Witch.color, "witched")} by {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LoverSuicide:
                                    deathReasonString = $" - {Helpers.cs(Lovers.color, "lover died")}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                    deathReasonString = $" - {Helpers.cs(Lawyer.color, "bad Lawyer")}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Bomb:
                                    deathReasonString = $" - bombed by {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Arson:
                                    deathReasonString = $" - burnt by {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                            }
                            roleName = roleName + deathReasonString;
                        }
                    }
                }
            }
            return roleName;
        }


        static string ReadmePage = "";
        public static async Task loadReadme() {
            if (ReadmePage == "") {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherRoles/main/README.md");
                response.EnsureSuccessStatusCode();
                string httpres = await response.Content.ReadAsStringAsync();
                ReadmePage = httpres;
            }
        }
        public static string GetRoleDescription(RoleInfo roleInfo) {
            while (ReadmePage == "") {
            }
                
            int index = ReadmePage.IndexOf($"## {roleInfo.name}");
            int endindex = ReadmePage.Substring(index).IndexOf("### Game Options");
            return ReadmePage.Substring(index, endindex);

        }
    }
}
