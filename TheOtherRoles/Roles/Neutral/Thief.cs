using TheOtherRoles.Mode;

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
            target == Arsonist.arsonist ||
            target == Pelican.Player ||
            target == Swooper.swooper ||
            target == Pavlovsdogs.pavlovsowner ||
            Infected.Player.Any(p => p == target) ||
            Pavlovsdogs.pavlovsdogs.Any(p => p == target) ||
            (canKillSheriff && Sheriff.Player.Any(x => x == target)) ||
            (canKillDeputy && target == Sheriff.Deputy) ||
            (canKillVeteran && target == Veteran.veteran);
        Message($"Thief can kill {target?.Data?.PlayerName ?? "NULL"}: {flag}", "Thief");
        return flag;
    }

    public static void StealsRole(byte targetId)
    {
        var target = PlayerById(targetId);
        var thief = Thief.thief;
        if (target == null) return;

        var role = RoleInfo.getRoleInfoForPlayer(target, false, false).FirstOrDefault();
        if (role == null) return;

        if (target == Jackal.Sidekick || Jackal.jackal.Any(x => x == target))
        {
            if (HandleGuesser.isGuesserGm && GuesserGM.guesserGamemodeSidekickIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(thief.PlayerId))
                RPCProcedure.setGuesserGm(thief.PlayerId);
        }
        if (target == Pavlovsdogs.pavlovsowner)
        {
            if (HandleGuesser.isGuesserGm && GuesserGM.guesserGamemodePavlovsdogIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(thief.PlayerId))
                RPCProcedure.setGuesserGm(thief.PlayerId);
        }
        if (target == Witch.witch)
        {
            if (MeetingHud.Instance)
            {
                // In a meeting, if the thief guesses the witch, all targets are saved or no target is saved.
                if (Witch.witchVoteSavesTargets) Witch.futureSpelled = new List<PlayerControl>();
                // If thief kills witch during the round, remove the thief from the list of spelled people, keep the rest
                else Witch.futureSpelled.RemoveAll(x => x.PlayerId == thief.PlayerId);
            }
        }

        if (target == Mimic.mimic)
        {
            Mimic.hasMimic = false;
        }
        if (target == Yoyo.yoyo)
        {
            Yoyo.markedLocation = null;
        }

        if (target == Werewolf.werewolf
            || target == Arsonist.arsonist
            || target == Juggernaut.juggernaut
            || target == Swooper.swooper
            || target == Pelican.Player)
        {
            RPCProcedure.setRole(targetId, (byte)RoleId.Survivor);
        }

        if (target == Jackal.Sidekick)
        {
            RPCProcedure.setRole(targetId, (byte)RoleId.Jackal);
        }
        if (target == Pavlovsdogs.pavlovsowner)
        {

            RPCProcedure.setRole(targetId, (byte)RoleId.Pavlovsdogs);
        }

        if (Lawyer.lawyer != null && target == Lawyer.target) Lawyer.target = thief;

        // SetRole
        RPCProcedure.setRole(thief.PlayerId, (byte)role.roleId);

        if (thief == PlayerControl.LocalPlayer) CustomButton.ResetAllCooldowns();
        clearAndReload();
        formerThief = thief; // After clearAndReload, else it would get reset...
    }

}
