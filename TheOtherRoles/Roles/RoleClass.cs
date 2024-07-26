using System.Collections.Generic;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles;

[HarmonyPatch]
public static class RoleClass
{
    public static void clearAndReloadRoles()
    {
        Aftermath.clearAndReload();
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
        Cursed.clearAndReload();
        Butcher.clearAndReload();
        Deputy.clearAndReload();
        Amnisiac.clearAndReload();
        Detective.clearAndReload();
        Werewolf.clearAndReload();
        TimeMaster.clearAndReload();
        BodyGuard.clearAndReload();
        Veteran.clearAndReload();
        Medic.clearAndReload();
        PrivateInvestigator.clearAndReload();
        Shifter.clearAndReload();
        Swapper.clearAndReload();
        Lovers.clearAndReload();
        Seer.clearAndReload();
        Morphling.clearAndReload();
        Camouflager.clearAndReload();
        Cultist.clearAndReload();
        Hacker.clearAndReload();
        Tracker.clearAndReload();
        Vampire.clearAndReload();
        Snitch.clearAndReload();
        Jackal.clearAndReload();
        Sidekick.clearAndReload();
        Pavlovsdogs.clearAndReload();
        Follower.clearAndReload();
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
        Akujo.clearAndReload();
        Yoyo.clearAndReload();
        EvilTrapper.clearAndReload();
        Survivor.clearAndReload();

        // Modifier
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
        Vip.clearAndReload();
        Invert.clearAndReload();
        Chameleon.clearAndReload();
        ButtonBarry.clearAndReload();
        LastImpostor.clearAndReload();
        Specoality.clearAndReload();

        // Gamemodes
        HandleGuesser.clearAndReload();
        HideNSeek.clearAndReload();
        PropHunt.clearAndReload();

        blockedRole();
    }

    public static class Crew
    {
        public static PlayerControl crew;
        public static Color color = Palette.White;

        public static void clearAndReload()
        {
            crew = null;
        }
    }

    internal static Dictionary<byte, byte[]> blockedRolePairings = [];

    public static void blockedRole()
    {
        blockedRolePairings = [];

        blockedRolePairings.Add((byte)RoleId.Vampire, [(byte)RoleId.Warlock]);
        blockedRolePairings.Add((byte)RoleId.Witch, [(byte)RoleId.Warlock]);
        blockedRolePairings.Add((byte)RoleId.Warlock, [(byte)RoleId.Vampire]);

        if (Pavlovsdogs.asWell)
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
}
