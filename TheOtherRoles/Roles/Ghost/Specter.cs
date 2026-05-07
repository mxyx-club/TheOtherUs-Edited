namespace TheOtherRoles.Roles.Ghost;

public class Specter
{
    public static PlayerControl Player;
    public static Color color = new Color32(154, 147, 80, byte.MaxValue);
    public static PlayerControl Target;

    public static float duration;
    public static bool resetRole;
    public static bool afterMeetingRevive;
    public static bool remember;

    public static bool exiledBeginRevive;
    public static bool revived;

    public static void ClearAndReload()
    {
        Player = null;
        Target = null;
        exiledBeginRevive = false;
        revived = false;
        remember = !CustomOptionHolder.specterAfterMeetingTakeRole.GetBool();
        afterMeetingRevive = CustomOptionHolder.specterAfterMeetingRevived.GetBool();
        resetRole = CustomOptionHolder.specterResetRole.GetBool();
        duration = CustomOptionHolder.specterDuration.GetFloat();
    }


    public static void ReloadRole(RoleId roleId, PlayerControl target)
    {

        switch (roleId)
        {
            case RoleId.Impostor:
                break;
            case RoleId.Morphling:
                if (resetRole) Glitch.clearAndReload();
                break;
            case RoleId.WolfLord:
                if (resetRole) WolfLord.ClearAndReload();
                break;
            case RoleId.Bomber:
                if (resetRole) Bomber.clearAndReload();
                break;
            case RoleId.Mimic:
                if (resetRole) Mimic.clearAndReload(false);
                break;
            case RoleId.Camouflager:
                if (resetRole) Camouflager.clearAndReload();
                break;
            case RoleId.Miner:
                if (resetRole) Miner.clearAndReload();
                break;
            case RoleId.Eraser:
                if (resetRole) Eraser.clearAndReload();
                break;
            case RoleId.Vampire:
                if (resetRole) Vampire.clearAndReload();
                break;
            case RoleId.Undertaker:
                if (resetRole) Undertaker.clearAndReload();
                break;
            case RoleId.Marionette:
                if (resetRole) Marionette.ClearAndReload();
                break;
            case RoleId.Warlock:
                if (resetRole) Warlock.clearAndReload();
                break;
            case RoleId.Trickster:
                if (resetRole) Trickster.clearAndReload();
                break;
            case RoleId.BountyHunter:
                if (resetRole) BountyHunter.clearAndReload();
                break;
            case RoleId.Cleaner:
                if (resetRole) Cleaner.clearAndReload();
                break;
            case RoleId.Terrorist:
                if (resetRole) Terrorist.clearAndReload();
                break;
            case RoleId.Blackmailer:
                if (resetRole) Blackmailer.clearAndReload();
                break;
            case RoleId.Witch:
                if (resetRole) Witch.clearAndReload();
                break;
            case RoleId.Ninja:
                if (resetRole) Ninja.clearAndReload();
                break;
            case RoleId.Yoyo:
                if (resetRole) Yoyo.clearAndReload();
                break;
            case RoleId.EvilTrapper:
                if (resetRole) EvilTrapper.clearAndReload();
                break;
            case RoleId.Grenadier:
                if (resetRole) Grenadier.clearAndReload();
                break;

            case RoleId.Amnisiac:
                break;
            case RoleId.Survivor:
                break;
            case RoleId.Jester:
                if (resetRole) Jester.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.Vulture:
                if (resetRole) Vulture.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.Lawyer:
                Survivor.Player.Add(target);
                break;
            case RoleId.Executioner:
                Survivor.Player.Add(target);
                break;
            case RoleId.Pursuer:
                if (resetRole) Pursuer.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.PartTimer:
                if (resetRole) PartTimer.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.Witness:
                if (resetRole) Witness.ClearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.Doomsayer:
                if (resetRole) Doomsayer.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.Arsonist:
                if (resetRole) Arsonist.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.Jackal:
                break;
            case RoleId.Sidekick:
                Jackal.jackal.Add(target);
                break;
            case RoleId.Pavlovsowner:
                Pavlovsdogs.pavlovsdogs.Add(Pavlovsdogs.pavlovsowner);
                break;
            case RoleId.Pavlovsdogs:
                break;
            case RoleId.Werewolf:
                if (resetRole) Werewolf.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.Swooper:
                if (resetRole) Swooper.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.Juggernaut:
                if (resetRole) Juggernaut.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.Pelican:
                if (resetRole) Pelican.clearAndReload(false);
                Survivor.Player.Add(target);
                break;
            case RoleId.Akujo:
                Survivor.Player.Add(target);
                break;
            case RoleId.Thief:
                if (resetRole) Thief.clearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.BandLeader:
                Survivor.Player.Add(target);
                break;
            case RoleId.SoulSight:
                if (resetRole) SoulSight.ClearAndReload();
                Survivor.Player.Add(target);
                break;
            /*case RoleId.Avenger:
                Jester.Player.Add(Player);
                Survivor.Player.Add(target);
                break;*/

            case RoleId.Crewmate:
                break;
            case RoleId.Vigilante:
                if (resetRole) Vigilante.clearAndReload();
                break;
            case RoleId.Mayor:
                if (resetRole) Mayor.clearAndReload();
                break;
            case RoleId.Prosecutor:
                if (resetRole) Prosecutor.clearAndReload();
                break;
            case RoleId.Portalmaker:
                if (resetRole) Portalmaker.clearAndReload();
                break;
            case RoleId.Engineer:
                if (resetRole) Engineer.clearAndReload();
                break;
            case RoleId.Sheriff:
                break;
            case RoleId.Deputy:
                if (resetRole) Sheriff.Reload();
                break;
            case RoleId.BodyGuard:
                if (resetRole) BodyGuard.clearAndReload();
                break;
            case RoleId.Jumper:
                if (resetRole) Jumper.clearAndReload();
                break;
            case RoleId.Detective:
                if (resetRole) Detective.clearAndReload();
                break;
            case RoleId.Veteran:
                if (resetRole) Veteran.clearAndReload();
                break;
            case RoleId.Medic:
                if (resetRole) Medic.clearAndReload();
                break;
            case RoleId.Swapper:
                if (resetRole) Swapper.clearAndReload();
                break;
            case RoleId.Seer:
                if (resetRole) Seer.clearAndReload();
                break;
            case RoleId.Hacker:
                if (resetRole) Hacker.clearAndReload();
                break;
            case RoleId.Tracker:
                if (resetRole) Tracker.clearAndReload();
                break;
            case RoleId.Snitch:
                if (resetRole) Snitch.clearAndReload();
                break;
            case RoleId.Prophet:
                if (resetRole) Prophet.clearAndReload();
                break;
            case RoleId.Oracle:
                if (resetRole) Oracle.ClearAndReload();
                break;
            case RoleId.InfoSleuth:
                break;
            case RoleId.Spy:
                if (resetRole) Spy.clearAndReload();
                break;
            case RoleId.SecurityGuard:
                if (resetRole) SecurityGuard.clearAndReload();
                break;
            case RoleId.Alchemyst:
                if (resetRole) Alchemyst.clearAndReload();
                break;
            case RoleId.Trapper:
                if (resetRole) Trapper.clearAndReload();
                break;
            case RoleId.Balancer:
                if (resetRole) Balancer.clearAndReload();
                break;
            case RoleId.Redemptor:
                if (resetRole) Redemptor.ClearAndReload();
                break;
            case RoleId.SchrodingersCat:
                if (resetRole) SchrodingersCat.ClearAndReload();
                break;
            case RoleId.Gunsmith:
                if (resetRole) Gunsmith.ClearAndReload();
                break;
            case RoleId.Berserker:
                if (resetRole) Berserker.ClearAndReload();
                break;
            case RoleId.Jailor:
                if (resetRole) Jailor.ClearAndReload();
                break;
            case RoleId.Infected:
                break;
        }

    }


    public static void TakeRole(byte targetId)
    {
        var local = Player;
        var target = PlayerById(targetId);
        if (local == null || target == null) return;

        var role = RoleInfo.getRoleInfoForPlayer(target, false, false).FirstOrDefault();
        if (role == null) return;
        ReloadRole(role.roleId, target);
        if (role.roleId == RoleId.Avenger) role = RoleInfo.jester;

        RPCProcedure.erasePlayerRoles(local.PlayerId);
        revived = true;

        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == targetId)
            {
                UObject.Destroy(array[i].gameObject);
                break;
            }
        }

        RPCProcedure.setRole(local.PlayerId, (byte)role.roleId);

        if (afterMeetingRevive)
        {
            exiledBeginRevive = true;
            return;
        }
        local.Revive();
    }
}