using AmongUs.GameOptions;
using TheOtherRoles.CustomGameModes;

namespace TheOtherRoles.Roles.Neutral;

public static class Thief
{
    public static PlayerControl thief;
    public static Color color = new Color32(71, 99, 45, byte.MaxValue);
    public static PlayerControl currentTarget;
    public static PlayerControl formerThief;

    public static float cooldown = 30f;

    public static bool hasImpostorVision;
    public static bool canUseVents;
    public static bool canKillSheriff;
    public static bool canKillDeputy;
    public static bool canKillVeteran;
    public static bool canStealWithGuess;

    public static void clearAndReload()
    {
        thief = null;
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

    public static bool tiefCanKill(PlayerControl target)
    {

        var flag = target.IsImpostor() ||
            Jackal.jackal.Any(x => x == target) ||
            target == Jackal.Sidekick ||
            target == Werewolf.werewolf ||
            target == Juggernaut.juggernaut ||
            target == Pelican.Player ||
            target == Swooper.swooper ||
            target == Pavlovsdogs.pavlovsowner ||
            Pavlovsdogs.pavlovsdogs.Any(p => p == target) ||
            (canKillSheriff && Sheriff.Player.Any(x => x == target)) ||
            (canKillDeputy && target == Sheriff.Deputy) ||
            (canKillVeteran && target == Veteran.veteran);
        Message($"Thief can kill {target?.Data?.PlayerName ?? "NULL"}: {flag}", "Thief");
        return flag;
    }

    public static void StealsRole(byte playerId)
    {
        var target = PlayerById(playerId);
        var thief = Thief.thief;
        if (target == null) return;

        if (target.Data.Role.IsImpostor) turnToImpostor(thief);

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
            if (HandleGuesser.isGuesserGm && GuesserGM.guesserGamemodeSidekickIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(thief.PlayerId))
                RPCProcedure.setGuesserGm(thief.PlayerId);
        }
        if (target == Pavlovsdogs.pavlovsowner)
        {
            Pavlovsdogs.pavlovsowner = thief;
            if (HandleGuesser.isGuesserGm && GuesserGM.guesserGamemodePavlovsdogIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(thief.PlayerId))
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
        if (target == Grenadier.Player) Grenadier.Player = thief;
        if (target == WolfLord.Player) WolfLord.Player = thief;
        if (target == BountyHunter.bountyHunter) BountyHunter.bountyHunter = thief;
        if (target == Gunsmith.Player) Gunsmith.Player = thief;
        if (target == Berserker.Player) Berserker.Player = thief;
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

        if (Infected.Player.Any(x => x == target))
        {
            Infected.Player.Add(thief);
        }
        if (target == Sheriff.Deputy) Sheriff.Deputy = thief;
        if (target == Veteran.veteran) Veteran.veteran = thief;
        if (target == Blackmailer.Player) Blackmailer.Player = thief;
        if (target == EvilTrapper.evilTrapper) EvilTrapper.evilTrapper = thief;

        if (Lawyer.lawyer != null && target == Lawyer.target)
            Lawyer.target = thief;
        if (Thief.thief == PlayerControl.LocalPlayer) CustomButton.ResetAllCooldowns();
        clearAndReload();
        formerThief = thief; // After clearAndReload, else it would get reset...
    }

}
