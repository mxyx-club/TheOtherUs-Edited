using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using TheOtherRoles.Buttons;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public static class Thief
{
    public static PlayerControl thief;
    public static Color color = new Color32(71, 99, 45, byte.MaxValue);
    public static PlayerControl currentTarget;
    public static PlayerControl formerThief;

    public static float cooldown = 30f;

    public static bool suicideFlag; // Used as a flag for suicide

    public static bool hasImpostorVision;
    public static bool canUseVents;
    public static bool canKillSheriff;
    public static bool canKillDeputy;
    public static bool canKillVeteran;
    public static bool canStealWithGuess;

    public static void clearAndReload()
    {
        thief = null;
        suicideFlag = false;
        currentTarget = null;
        formerThief = null;
        hasImpostorVision = CustomOptionHolder.thiefHasImpVision.GetBool();
        cooldown = CustomOptionHolder.thiefCooldown.GetFloat();
        canUseVents = CustomOptionHolder.thiefCanUseVents.GetBool();
        canKillSheriff = CustomOptionHolder.thiefCanKillSheriff.GetBool();
        canKillDeputy = CustomOptionHolder.thiefCanKillDeputy.GetBool();
        canKillVeteran = CustomOptionHolder.thiefCanKillVeteran.GetBool();
        canStealWithGuess = CustomOptionHolder.thiefCanStealWithGuess.GetBool();
    }

    public static bool tiefCanKill(PlayerControl target, PlayerControl killer)
    {
        return killer == thief && (target.Data.Role.IsImpostor ||
            Jackal.jackal.Any(x => x == target) ||
            target == Jackal.Sidekick ||
            target == Werewolf.werewolf ||
            target == Juggernaut.juggernaut ||
            target == Swooper.swooper ||
            Pavlovsdogs.pavlovsdogs.Any(p => p == target) ||
            target == Pavlovsdogs.pavlovsowner ||
            (canKillSheriff && Sheriff.Player.Any(x => x == target)) ||
            (canKillDeputy && target == Sheriff.Deputy) ||
            (canKillVeteran && target == Veteran.veteran));
    }

    public static void StealsRole(byte playerId)
    {
        var target = playerById(playerId);
        var thief = Thief.thief;
        if (target == null) return;
        if (Sheriff.Player.Any(x => x == target)) Sheriff.Player.Add(thief);
        if (Sheriff.formerDeputy == target) Sheriff.formerDeputy = thief;
        if (target == Sheriff.Deputy) Sheriff.Deputy = thief;
        if (target == Veteran.veteran) Veteran.veteran = thief;
        if (Jackal.jackal.Any(x => x == target))
        {
            Jackal.jackal.Add(thief);
        }

        if (target == Jackal.Sidekick)
        {
            Jackal.Sidekick = thief;
            Jackal.jackal.Add(target);
            if (HandleGuesser.isGuesserGm && CustomOptionHolder.guesserGamemodeSidekickIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(thief.PlayerId))
                RPCProcedure.setGuesserGm(thief.PlayerId);
        }
        if (target == Pavlovsdogs.pavlovsowner)
        {
            Pavlovsdogs.pavlovsdogs.Add(target);
            Pavlovsdogs.pavlovsowner = thief;
            if (HandleGuesser.isGuesserGm && CustomOptionHolder.guesserGamemodePavlovsdogIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(thief.PlayerId))
                RPCProcedure.setGuesserGm(thief.PlayerId);
        }
        if (Pavlovsdogs.pavlovsdogs.Any(x => x == target))
        {
            Pavlovsdogs.pavlovsdogs.Add(thief);
        }
        if (target == Poucher.poucher && !Poucher.spawnModifier) Poucher.poucher = thief;
        if (target == Butcher.butcher) Butcher.butcher = thief;
        if (target == Morphling.morphling) Morphling.morphling = thief;
        if (target == Camouflager.camouflager) Camouflager.camouflager = thief;
        if (target == Vampire.vampire) Vampire.vampire = thief;
        if (target == Eraser.eraser) Eraser.eraser = thief;
        if (target == Trickster.trickster) Trickster.trickster = thief;
        if (target == Gambler.gambler) Gambler.gambler = thief;
        if (target == Cleaner.cleaner) Cleaner.cleaner = thief;
        if (target == Warlock.warlock) Warlock.warlock = thief;
        if (target == Grenadier.grenadier) Grenadier.grenadier = thief;
        if (target == WolfLord.Player) WolfLord.Player = thief;
        if (target == BountyHunter.bountyHunter) BountyHunter.bountyHunter = thief;
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

        if (target == Werewolf.werewolf)
        {
            Survivor.Player.Add(target);
            Werewolf.werewolf = thief;
        }
        if (target == Arsonist.arsonist)
        {
            Survivor.Player.Add(target);
            Arsonist.arsonist = thief;
        }
        if (target == Juggernaut.juggernaut)
        {
            Survivor.Player.Add(target);
            Juggernaut.juggernaut = thief;
        }
        if (target == Swooper.swooper)
        {
            Survivor.Player.Add(target);
            Swooper.swooper = thief;
        }

        if (target == Sheriff.Deputy) Sheriff.Deputy = thief;
        if (target == Veteran.veteran) Veteran.veteran = thief;
        if (target == Blackmailer.blackmailer) Blackmailer.blackmailer = thief;
        if (target == EvilTrapper.evilTrapper) EvilTrapper.evilTrapper = thief;

        if (Lawyer.lawyer != null && target == Lawyer.target)
            Lawyer.target = thief;
        if (Thief.thief == PlayerControl.LocalPlayer) CustomButton.ResetAllCooldowns();
        clearAndReload();
        formerThief = thief; // After clearAndReload, else it would get reset...
    }

}
