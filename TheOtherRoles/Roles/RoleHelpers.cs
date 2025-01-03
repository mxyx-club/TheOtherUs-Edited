using System;
using System.Collections.Generic;
using System.Linq;
using MonoMod.Utils;
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Roles;

public static class RoleHelpers
{
    private static bool _CanSeeRoleInfo;
    public static bool CanSeeRoleInfo
    {
        get => _CanSeeRoleInfo;
        set
        {
            if (PlayerControl.LocalPlayer == Specter.Player) _CanSeeRoleInfo = false;
            else if (Specter.Player.isLover() && Lovers.otherLover(Specter.Player) == PlayerControl.LocalPlayer) _CanSeeRoleInfo = false;
            else _CanSeeRoleInfo = value;
        }
    }

    public static bool CanMultipleShots(PlayerControl dyingTarget)
    {
        if (dyingTarget == CachedPlayer.LocalPlayer.PlayerControl)
            return false;

        if (ModOption.gameMode != CustomGamemodes.Guesser)
        {
            if (PlayerControl.LocalPlayer == Vigilante.vigilante
                && HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId) > 0
                && Vigilante.hasMultipleShotsPerMeeting) return true;
            else if (Assassin.assassin.Any(x => x == PlayerControl.LocalPlayer)
                && HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId) > 0
                && Assassin.assassinMultipleShotsPerMeeting) return true;
        }

        else if (HandleGuesser.isGuesser(CachedPlayer.LocalPlayer.PlayerId)
            && HandleGuesser.remainingShots(CachedPlayer.LocalPlayer.PlayerId) > 0
            && HandleGuesser.hasMultipleShotsPerMeeting) return true;

        return CachedPlayer.LocalPlayer.PlayerControl == Doomsayer.doomsayer && Doomsayer.hasMultipleShotsPerMeeting &&
               Doomsayer.CanShoot;
    }

    public static Dictionary<byte, byte[]> blockedRolePairings = new();
    public static Dictionary<AssignType, List<Assignment>> GhostRoles = new();
    public static List<PlayerControl> GhostPlayer = new();

    public static void blockRole()
    {
        blockedRolePairings.Clear();

        blockedRolePairings.Add((byte)RoleId.Vampire, [(byte)RoleId.Warlock]);
        blockedRolePairings.Add((byte)RoleId.Witch, [(byte)RoleId.Warlock]);
        blockedRolePairings.Add((byte)RoleId.Warlock, [(byte)RoleId.Vampire]);

        if (CustomOptionHolder.pavlovsownerAndJackalAsWell.GetBool())
        {
            blockedRolePairings.Add((byte)RoleId.Jackal, [(byte)RoleId.Pavlovsowner]);
            blockedRolePairings.Add((byte)RoleId.Pavlovsowner, [(byte)RoleId.Jackal]);
        }
        if (Executioner.promotesToLawyer)
        {
            blockedRolePairings.Add((byte)RoleId.Executioner, [(byte)RoleId.Lawyer]);
            blockedRolePairings.Add((byte)RoleId.Lawyer, [(byte)RoleId.Executioner]);
        }

        blockedRolePairings.Add((byte)RoleId.Vulture, [(byte)RoleId.Cleaner]);
        blockedRolePairings.Add((byte)RoleId.Cleaner, [(byte)RoleId.Vulture]);

        blockedRolePairings.Add((byte)RoleId.Ninja, [(byte)RoleId.Swooper]);
        blockedRolePairings.Add((byte)RoleId.Swooper, [(byte)RoleId.Ninja]);
    }

    public static Dictionary<RoleId, int> RoleIsEnable = new();

    public static void ResetRoleSelection()
    {
        RoleIsEnable.Clear();
        RoleIsEnable.AddRange(new()
        {
            { RoleId.Sheriff, CustomOptionHolder.sheriffSpawnRate.GetSelection() },
            { RoleId.Deputy, CustomOptionHolder.deputySpawnRate.GetSelection() },
            { RoleId.BodyGuard, CustomOptionHolder.bodyGuardSpawnRate.GetSelection() },
            { RoleId.Balancer, CustomOptionHolder.balancerSpawnRate.GetSelection() },
            { RoleId.Detective, CustomOptionHolder.detectiveSpawnRate.GetSelection() },
            { RoleId.Engineer, CustomOptionHolder.engineerSpawnRate.GetSelection() },
            { RoleId.Hacker, CustomOptionHolder.hackerSpawnRate.GetSelection() },
            { RoleId.InfoSleuth, CustomOptionHolder.infoSleuthSpawnRate.GetSelection() },
            { RoleId.Jumper, CustomOptionHolder.jumperSpawnRate.GetSelection() },
            { RoleId.Mayor, CustomOptionHolder.mayorSpawnRate.GetSelection() },
            { RoleId.Medic, CustomOptionHolder.medicSpawnRate.GetSelection() },
            { RoleId.Medium, CustomOptionHolder.mediumSpawnRate.GetSelection() },
            { RoleId.Portalmaker, CustomOptionHolder.portalmakerSpawnRate.GetSelection() },
            { RoleId.Prophet, CustomOptionHolder.prophetSpawnRate.GetSelection() },
            { RoleId.Prosecutor, CustomOptionHolder.prosecutorSpawnRate.GetSelection() },
            { RoleId.SecurityGuard, CustomOptionHolder.securityGuardSpawnRate.GetSelection() },
            { RoleId.Seer, CustomOptionHolder.seerSpawnRate.GetSelection() },
            { RoleId.Snitch, CustomOptionHolder.snitchSpawnRate.GetSelection() },
            { RoleId.Spy, CustomOptionHolder.spySpawnRate.GetSelection() },
            { RoleId.Swapper, CustomOptionHolder.swapperSpawnRate.GetSelection() },
            { RoleId.TimeMaster, CustomOptionHolder.timeMasterSpawnRate.GetSelection() },
            { RoleId.Tracker, CustomOptionHolder.trackerSpawnRate.GetSelection() },
            { RoleId.Trapper, CustomOptionHolder.trapperSpawnRate.GetSelection() },
            { RoleId.Veteran, CustomOptionHolder.veteranSpawnRate.GetSelection() },
            { RoleId.Vigilante, CustomOptionHolder.guesserSpawnRate.GetSelection() },

            { RoleId.WolfLord, CustomOptionHolder.wolfLordSpawnRate.GetSelection() },
            { RoleId.Blackmailer, CustomOptionHolder.blackmailerSpawnRate.GetSelection() },
            { RoleId.Bomber, CustomOptionHolder.bomberSpawnRate.GetSelection() },
            { RoleId.BountyHunter, CustomOptionHolder.bountyHunterSpawnRate.GetSelection() },
            { RoleId.Butcher, CustomOptionHolder.butcherSpawnRate.GetSelection() },
            { RoleId.Camouflager, CustomOptionHolder.camouflagerSpawnRate.GetSelection() },
            { RoleId.Cleaner, CustomOptionHolder.cleanerSpawnRate.GetSelection() },
            { RoleId.Eraser, CustomOptionHolder.eraserSpawnRate.GetSelection() },
            { RoleId.Escapist, CustomOptionHolder.escapistSpawnRate.GetSelection() },
            { RoleId.EvilTrapper, CustomOptionHolder.evilTrapperSpawnRate.GetSelection() },
            { RoleId.Gambler, CustomOptionHolder.gamblerSpawnRate.GetSelection() },
            { RoleId.Mimic, CustomOptionHolder.mimicSpawnRate.GetSelection() },
            { RoleId.Miner, CustomOptionHolder.minerSpawnRate.GetSelection() },
            { RoleId.Morphling, CustomOptionHolder.morphlingSpawnRate.GetSelection() },
            { RoleId.Ninja, CustomOptionHolder.ninjaSpawnRate.GetSelection() },
            { RoleId.Poucher, CustomOptionHolder.poucherSpawnRate.GetSelection() },
            { RoleId.Terrorist, CustomOptionHolder.terroristSpawnRate.GetSelection() },
            { RoleId.Trickster, CustomOptionHolder.tricksterSpawnRate.GetSelection() },
            { RoleId.Undertaker, CustomOptionHolder.undertakerSpawnRate.GetSelection() },
            { RoleId.Vampire, CustomOptionHolder.vampireSpawnRate.GetSelection() },
            { RoleId.Warlock, CustomOptionHolder.warlockSpawnRate.GetSelection() },
            { RoleId.Witness, CustomOptionHolder.witnessSpawnRate.GetSelection() },
            { RoleId.Witch, CustomOptionHolder.witchSpawnRate.GetSelection() },
            { RoleId.Yoyo, CustomOptionHolder.yoyoSpawnRate.GetSelection() },
            { RoleId.Grenadier, CustomOptionHolder.grenadierSpawnRate.GetSelection() },

            { RoleId.Akujo, CustomOptionHolder.akujoSpawnRate.GetSelection() },
            { RoleId.Amnisiac, CustomOptionHolder.amnisiacSpawnRate.GetSelection() },
            { RoleId.Arsonist, CustomOptionHolder.arsonistSpawnRate.GetSelection() },
            { RoleId.Doomsayer, CustomOptionHolder.doomsayerSpawnRate.GetSelection() },
            { RoleId.Executioner, CustomOptionHolder.executionerSpawnRate.GetSelection() },
            { RoleId.Jackal, CustomOptionHolder.jackalSpawnRate.GetSelection() },
            { RoleId.Sidekick, CustomOptionHolder.jackalSpawnRate.GetSelection() },
            { RoleId.Jester, CustomOptionHolder.jesterSpawnRate.GetSelection() },
            { RoleId.Juggernaut, CustomOptionHolder.juggernautSpawnRate.GetSelection() },
            { RoleId.Lawyer, CustomOptionHolder.lawyerSpawnRate.GetSelection() },
            { RoleId.PartTimer, CustomOptionHolder.partTimerSpawnRate.GetSelection() },
            { RoleId.Pavlovsowner, CustomOptionHolder.pavlovsownerSpawnRate.GetSelection() },
            { RoleId.Pavlovsdogs, CustomOptionHolder.pavlovsownerSpawnRate.GetSelection() },
            { RoleId.Survivor, CustomOptionHolder.survivorSpawnRate.GetSelection() },
            { RoleId.Swooper, CustomOptionHolder.swooperSpawnRate.GetSelection() },
            { RoleId.Thief, CustomOptionHolder.thiefSpawnRate.GetSelection() },
            { RoleId.Vulture, CustomOptionHolder.vultureSpawnRate.GetSelection() },
            { RoleId.Werewolf, CustomOptionHolder.werewolfSpawnRate.GetSelection() },
            { RoleId.Pursuer, CustomOptionHolder.lawyerSpawnRate.GetSelection() + CustomOptionHolder.executionerSpawnRate.GetSelection() },

            { RoleId.Lover, CustomOptionHolder.modifierLover.GetSelection() },
            { RoleId.Aftermath, CustomOptionHolder.modifierAftermath.GetSelection() },
            { RoleId.AntiTeleport, CustomOptionHolder.modifierAntiTeleport.GetSelection() },
            { RoleId.Assassin, CustomOptionHolder.modifierAssassin.GetSelection() },
            { RoleId.Bait, CustomOptionHolder.modifierBait.GetSelection() },
            { RoleId.Blind, CustomOptionHolder.modifierBlind.GetSelection() },
            { RoleId.Bloody, CustomOptionHolder.modifierBloody.GetSelection() },
            { RoleId.ButtonBarry, CustomOptionHolder.modifierButtonBarry.GetSelection() },
            { RoleId.Chameleon, CustomOptionHolder.modifierChameleon.GetSelection() },
            { RoleId.Cursed, CustomOptionHolder.modifierCursed.GetSelection() },
            { RoleId.Disperser, CustomOptionHolder.modifierDisperser.GetSelection() },
            { RoleId.Flash, CustomOptionHolder.modifierFlash.GetSelection() },
            { RoleId.Giant, CustomOptionHolder.modifierGiant.GetSelection() },
            { RoleId.Indomitable, CustomOptionHolder.modifierIndomitable.GetSelection() },
            { RoleId.Invert, CustomOptionHolder.modifierInvert.GetSelection() },
            { RoleId.LastImpostor, CustomOptionHolder.modifierLastImpostor.GetSelection() },
            { RoleId.Mini, CustomOptionHolder.modifierMini.GetSelection() },
            { RoleId.Multitasker, CustomOptionHolder.modifierMultitasker.GetSelection() },
            { RoleId.Radar, CustomOptionHolder.modifierRadar.GetSelection() },
            { RoleId.Shifter, CustomOptionHolder.modifierShifter.GetSelection() },
            { RoleId.Slueth, CustomOptionHolder.modifierSlueth.GetSelection() },
            { RoleId.Specoality, CustomOptionHolder.modifierSpecoality.GetSelection() },
            { RoleId.Tiebreaker, CustomOptionHolder.modifierTieBreaker.GetSelection() },
            { RoleId.Torch, CustomOptionHolder.modifierTorch.GetSelection() },
            { RoleId.Tunneler, CustomOptionHolder.modifierTunneler.GetSelection() },
            { RoleId.Vip, CustomOptionHolder.modifierVip.GetSelection() },
            { RoleId.Watcher, CustomOptionHolder.modifierWatcher.GetSelection()}
        });
        GhostRoles.Clear();
        GhostPlayer.Clear();

        GhostRoles[AssignType.Crewmate] = new List<Assignment>
        {
            new(RoleId.GhostEngineer, CustomOptionHolder.ghostEngineerSpawnRate.GetSelection()),
        };


        GhostRoles[AssignType.otherNeutral] = new List<Assignment>
        {
            new(RoleId.Specter, CustomOptionHolder.specterSpawnRate.GetSelection())
        };
    }

    public static void clearAndReloadRoles()
    {
        Vigilante.clearAndReload();
        Jester.clearAndReload();
        Mayor.clearAndReload();
        Prosecutor.clearAndReload();
        Portalmaker.clearAndReload();
        Poucher.clearAndReload();
        Mimic.clearAndReload();
        Engineer.clearAndReload();
        Sheriff.clearAndReload();
        InfoSleuth.clearAndReload();
        Gambler.clearAndReload();
        Butcher.clearAndReload();
        Deputy.clearAndReload();
        Amnisiac.clearAndReload();
        Detective.clearAndReload();
        Werewolf.clearAndReload();
        TimeMaster.clearAndReload();
        BodyGuard.clearAndReload();
        Veteran.clearAndReload();
        Medic.clearAndReload();
        Shifter.clearAndReload();
        Swapper.clearAndReload();
        Lovers.clearAndReload();
        Seer.clearAndReload();
        Morphling.clearAndReload();
        Camouflager.clearAndReload();
        Hacker.clearAndReload();
        Tracker.clearAndReload();
        Vampire.clearAndReload();
        Snitch.clearAndReload();
        Jackal.clearAndReload();
        Pavlovsdogs.clearAndReload();
        Eraser.clearAndReload();
        Spy.clearAndReload();
        Trickster.clearAndReload();
        Cleaner.clearAndReload();
        Undertaker.clearAndReload();
        Warlock.clearAndReload();
        SecurityGuard.clearAndReload();
        Arsonist.clearAndReload();
        BountyHunter.clearAndReload();
        Vulture.clearAndReload();
        Medium.clearAndReload();
        Bomber.clearAndReload();
        Lawyer.clearAndReload();
        Executioner.clearAndReload();
        Pursuer.clearAndReload();
        Witch.clearAndReload();
        Jumper.clearAndReload();
        Prophet.clearAndReload();
        Escapist.clearAndReload();
        Ninja.clearAndReload();
        Blackmailer.clearAndReload();
        Thief.clearAndReload();
        Miner.clearAndReload();
        Trapper.clearAndReload();
        Terrorist.clearAndReload();
        Juggernaut.clearAndReload();
        Doomsayer.clearAndReload();
        Swooper.clearAndReload();
        Balancer.clearAndReload();
        Akujo.clearAndReload();
        Yoyo.clearAndReload();
        EvilTrapper.clearAndReload();
        Survivor.clearAndReload();
        PartTimer.clearAndReload();
        Grenadier.clearAndReload();
        Witness.ClearAndReload();
        WolfLord.ClearAndReload();

        // Modifier
        Assassin.clearAndReload();
        Aftermath.clearAndReload();
        Bait.clearAndReload();
        Bloody.clearAndReload();
        AntiTeleport.clearAndReload();
        Tiebreaker.clearAndReload();
        Sunglasses.clearAndReload();
        Torch.clearAndReload();
        Flash.clearAndReload();
        Blind.clearAndReload();
        Watcher.clearAndReload();
        Radar.clearAndReload();
        Tunneler.clearAndReload();
        Multitasker.clearAndReload();
        Disperser.clearAndReload();
        Mini.clearAndReload();
        Giant.clearAndReload();
        Indomitable.clearAndReload();
        Slueth.clearAndReload();
        Cursed.clearAndReload();
        Vip.clearAndReload();
        Invert.clearAndReload();
        Chameleon.clearAndReload();
        ButtonBarry.clearAndReload();
        LastImpostor.clearAndReload();
        Specoality.clearAndReload();
        Vortox.ClearAndReload();

        GhostEngineer.ClearAndReload();
        Specter.ClearAndReload();

        // Gamemodes
        HandleGuesser.clearAndReload();

        blockRole();
        ResetRoleSelection();
        CanSeeRoleInfo = false;
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.AssignRoleOnDeath))]
    public static class AssignRoleOnDeathPatch
    {
        public static bool Prefix([HarmonyArgument(0)] PlayerControl player)
        {
            if (player.IsAlive() || player == null) return false;
            return true;
        }

        public static bool otherNeutral(PlayerControl player)
        {
            if (isNeutral(player) && player != Lawyer.lawyer &&
               !Jackal.jackal.Contains(player) && player != Jackal.sidekick &&
               player != Pavlovsdogs.pavlovsowner && !Pavlovsdogs.pavlovsdogs.Contains(player))
                return true;
            if (PartTimer.partTimer == player && PartTimer.target == null) return true;
            return false;
        }

        public static void Postfix([HarmonyArgument(0)] PlayerControl player)
        {
            if (GhostPlayer.Contains(player)) return;

            if (player.isCrew()) AssignRole(player, AssignType.Crewmate);

            if (otherNeutral(player)) AssignRole(player, AssignType.otherNeutral);
        }

        private static void AssignRole(PlayerControl player, AssignType assignType)
        {
            if (!GhostRoles.TryGetValue(assignType, out var roles) || roles.All(x => x.SpawnRate == 0)) return;
            roles = roles.OrderBy(x => Guid.NewGuid()).ToList();
            foreach (var role in roles.Where(x => x.SpawnRate == 10 && !x.Assigned).ToList())
            {
                AssignRoleToPlayer(player, role);
                return;
            }

            int maxCount = roles.Count;
            int count = 0;
            while (count < maxCount)
            {
                bool assigned = false;
                foreach (var role in roles.Where(x => x.SpawnRate is > 0 and < 10 && !x.Assigned).ToList())
                {
                    if (rnd.Next(1, 101) <= role.SpawnRate * (10 + count))
                    {
                        AssignRoleToPlayer(player, role);
                        assigned = true;
                        break;
                    }
                }
                if (assigned) break;
                count++;
            }
        }

        private static void AssignRoleToPlayer(PlayerControl player, Assignment role)
        {
            var write = StartRPC(PlayerControl.LocalPlayer, CustomRPC.SetGhostRole);
            write.Write(player.PlayerId);
            write.Write((byte)role.RoleId);
            write.EndRPC();

            RPCProcedure.setGhostRole(player.PlayerId, (byte)role.RoleId);
            GhostPlayer.Add(player);
            role.Assigned = true;
        }
    }

    public class Assignment
    {
        public RoleId RoleId { get; set; }
        public int SpawnRate { get; set; }
        public bool Assigned { get; set; }

        public Assignment(RoleId roleId, int Rate)
        {
            RoleId = roleId;
            SpawnRate = Rate;
        }
    }

    public enum AssignType
    {
        None,
        Crewmate,
        Impostor,
        Neutral,
        otherNeutral,
        Custom
    }
}

