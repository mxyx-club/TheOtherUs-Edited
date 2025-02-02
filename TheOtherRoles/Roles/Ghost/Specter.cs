using System.Linq;
using UnityEngine;

namespace TheOtherRoles.Roles.Ghost;

public class Specter
{
    public static PlayerControl Player;
    public static Color color = new Color32(154, 147, 80, byte.MaxValue);

    public static float duration;
    public static bool resetRole;
    public static bool afterMeetingRevived;

    public static bool revive;
    public static bool remember;

    public static void ClearAndReload()
    {
        Player = null;
        revive = false;
        remember = !CustomOptionHolder.specterAfterMeetingTakeRole.GetBool();
        afterMeetingRevived = CustomOptionHolder.specterAfterMeetingRevived.GetBool();
        resetRole = CustomOptionHolder.specterResetRole.GetBool();
        duration = CustomOptionHolder.specterDuration.GetFloat();
    }

    public static void TakeRole(byte targetId)
    {
        var local = Player;
        var target = playerById(targetId);
        if (local == null || target == null) return;

        RPCProcedure.erasePlayerRoles(local.PlayerId);
        var roleInfo = RoleInfo.getRoleInfoForPlayer(target).FirstOrDefault(x => x.roleType is not RoleType.Modifier and not RoleType.Ghost);
        if (target.isImpostor()) turnToImpostor(local);

        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == targetId)
            {
                Object.Destroy(array[i].gameObject);
                break;
            }
        }

        if (roleInfo != null)
        {
            switch (roleInfo.roleId)
            {
                case RoleId.Amnisiac:
                    Amnisiac.Player.Add(local);
                    break;
                case RoleId.Impostor:
                    break;
                case RoleId.Morphling:
                    if (resetRole) Morphling.clearAndReload();
                    Morphling.morphling = local;
                    break;
                case RoleId.Bomber:
                    if (resetRole) Bomber.clearAndReload();
                    Bomber.bomber = local;
                    break;
                case RoleId.Poucher:
                    if (resetRole) Poucher.clearAndReload();
                    Poucher.poucher = local;
                    break;
                case RoleId.Butcher:
                    if (resetRole) Butcher.clearAndReload();
                    Butcher.butcher = local;
                    break;
                case RoleId.Mimic:
                    if (Mimic.mimic != null) RPCProcedure.erasePlayerRoles(Mimic.mimic.PlayerId);
                    if (resetRole) Mimic.clearAndReload();
                    Mimic.mimic = local;
                    break;
                case RoleId.Camouflager:
                    if (resetRole) Camouflager.clearAndReload();
                    Camouflager.camouflager = local;
                    break;
                case RoleId.Miner:
                    if (resetRole) Miner.clearAndReload();
                    Miner.miner = local;
                    break;
                case RoleId.Eraser:
                    if (resetRole) Eraser.clearAndReload();
                    Eraser.eraser = local;
                    break;
                case RoleId.Vampire:
                    if (resetRole) Vampire.clearAndReload();
                    Vampire.vampire = local;
                    break;
                case RoleId.Undertaker:
                    if (resetRole) Undertaker.clearAndReload();
                    Undertaker.undertaker = local;
                    break;
                case RoleId.WolfLord:
                    if (resetRole) WolfLord.ClearAndReload();
                    WolfLord.Player = local;
                    break;
                case RoleId.Escapist:
                    if (resetRole) Escapist.clearAndReload();
                    Escapist.escapist = local;
                    break;
                case RoleId.Warlock:
                    if (resetRole) Warlock.clearAndReload();
                    Warlock.warlock = local;
                    break;
                case RoleId.Trickster:
                    if (resetRole) Trickster.clearAndReload();
                    Trickster.trickster = local;
                    break;
                case RoleId.BountyHunter:
                    if (resetRole) BountyHunter.clearAndReload();
                    BountyHunter.bountyHunter = local;
                    break;
                case RoleId.Cleaner:
                    if (resetRole) Cleaner.clearAndReload();
                    Cleaner.cleaner = local;
                    break;
                case RoleId.Terrorist:
                    if (resetRole) Terrorist.clearAndReload();
                    Terrorist.terrorist = local;
                    break;
                case RoleId.Blackmailer:
                    if (resetRole) Blackmailer.clearAndReload();
                    Blackmailer.blackmailer = local;
                    break;
                case RoleId.Witch:
                    if (resetRole) Witch.clearAndReload();
                    Witch.witch = local;
                    break;
                case RoleId.Ninja:
                    if (resetRole) Ninja.clearAndReload();
                    Ninja.ninja = local;
                    break;
                case RoleId.Yoyo:
                    if (resetRole) Yoyo.clearAndReload();
                    Yoyo.yoyo = local;
                    break;
                case RoleId.EvilTrapper:
                    if (resetRole) EvilTrapper.clearAndReload();
                    EvilTrapper.evilTrapper = local;
                    break;
                case RoleId.Gambler:
                    if (resetRole) Gambler.clearAndReload();
                    Gambler.gambler = local;
                    break;
                case RoleId.Grenadier:
                    if (resetRole) Grenadier.clearAndReload();
                    Grenadier.Player = local;
                    break;
                case RoleId.Survivor:
                    Survivor.Player.RemoveAll(x => x == target);
                    Survivor.Player.Add(local);
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Jester:
                    Jester.jester = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Vulture:
                    Vulture.vulture = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Lawyer:
                    Lawyer.lawyer = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Executioner:
                    Executioner.executioner = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Pursuer:
                    Pursuer.Player.RemoveAll(x => x == target);
                    Pursuer.Player.Add(local);
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.PartTimer:
                    PartTimer.partTimer = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Witness:
                    Witness.Player = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Doomsayer:
                    Doomsayer.doomsayer = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Arsonist:
                    Arsonist.arsonist = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Jackal:
                    Jackal.jackal.Add(local);
                    break;
                case RoleId.Sidekick:
                    Jackal.Sidekick = local;
                    Jackal.jackal.Add(target);
                    break;
                case RoleId.Pavlovsowner:
                    Pavlovsdogs.pavlovsowner = local;
                    Pavlovsdogs.pavlovsdogs.Add(target);
                    break;
                case RoleId.Pavlovsdogs:
                    Pavlovsdogs.pavlovsdogs.Add(local);
                    break;
                case RoleId.Werewolf:
                    Werewolf.werewolf = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Swooper:
                    Swooper.swooper = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Juggernaut:
                    Juggernaut.juggernaut = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Pelican:
                    Pelican.clearAndReload(false);
                    Pelican.Player = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Akujo:
                    Akujo.akujo = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Thief:
                    Thief.thief = local;
                    Amnisiac.Player.Add(target);
                    break;
                case RoleId.Crewmate:
                    break;
                case RoleId.Vigilante:
                    if (resetRole) Vigilante.clearAndReload();
                    Vigilante.vigilante = local;
                    break;
                case RoleId.Mayor:
                    if (resetRole) Mayor.clearAndReload();
                    Mayor.mayor = local;
                    break;
                case RoleId.Prosecutor:
                    if (resetRole) Prosecutor.clearAndReload();
                    Prosecutor.prosecutor = local;
                    break;
                case RoleId.Portalmaker:
                    if (resetRole) Portalmaker.clearAndReload();
                    Portalmaker.portalmaker = local;
                    break;
                case RoleId.Engineer:
                    if (resetRole) Engineer.clearAndReload();
                    Engineer.engineer = local;
                    break;
                case RoleId.Sheriff:
                    Sheriff.Player.Add(local);
                    if (Sheriff.formerDeputy == target) Sheriff.formerDeputy = local;
                    break;
                case RoleId.Deputy:
                    if (Amnisiac.resetRole) Sheriff.Reload();
                    Sheriff.Deputy = local;
                    break;
                case RoleId.BodyGuard:
                    if (Amnisiac.resetRole) BodyGuard.clearAndReload();
                    BodyGuard.bodyguard = local;
                    break;
                case RoleId.Jumper:
                    if (Amnisiac.resetRole) Jumper.clearAndReload();
                    Jumper.jumper = local;
                    break;
                case RoleId.Detective:
                    if (Amnisiac.resetRole) Detective.clearAndReload();
                    Detective.detective = local;
                    break;
                case RoleId.TimeMaster:
                    if (Amnisiac.resetRole) TimeMaster.clearAndReload();
                    TimeMaster.timeMaster = local;
                    break;
                case RoleId.Veteran:
                    if (Amnisiac.resetRole) Veteran.clearAndReload();
                    Veteran.veteran = local;
                    break;
                case RoleId.Medic:
                    if (Amnisiac.resetRole) Medic.clearAndReload();
                    Medic.medic = local;
                    break;
                case RoleId.Swapper:
                    if (Amnisiac.resetRole) Swapper.clearAndReload();
                    Swapper.swapper = local;
                    break;
                case RoleId.Seer:
                    if (Amnisiac.resetRole) Seer.clearAndReload();
                    Seer.seer = local;
                    break;
                case RoleId.Hacker:
                    if (Amnisiac.resetRole) Hacker.clearAndReload();
                    Hacker.hacker = local;
                    break;
                case RoleId.Tracker:
                    if (Amnisiac.resetRole) Tracker.clearAndReload();
                    Tracker.tracker = local;
                    break;
                case RoleId.Snitch:
                    if (Amnisiac.resetRole) Snitch.clearAndReload();
                    Snitch.snitch = local;
                    break;
                case RoleId.Prophet:
                    if (Amnisiac.resetRole) Prophet.clearAndReload();
                    Prophet.prophet = local;
                    break;
                case RoleId.InfoSleuth:
                    if (resetRole) InfoSleuth.clearAndReload();
                    InfoSleuth.infoSleuth = local;
                    break;
                case RoleId.Spy:
                    if (Amnisiac.resetRole) Spy.clearAndReload();
                    Spy.spy = local;
                    break;
                case RoleId.SecurityGuard:
                    if (Amnisiac.resetRole) SecurityGuard.clearAndReload();
                    SecurityGuard.securityGuard = local;
                    break;
                case RoleId.Medium:
                    if (Amnisiac.resetRole) Medium.clearAndReload();
                    Medium.medium = local;
                    break;
                case RoleId.Trapper:
                    if (Amnisiac.resetRole) Trapper.clearAndReload();
                    Trapper.trapper = local;
                    break;
                case RoleId.Balancer:
                    if (Amnisiac.resetRole) Balancer.clearAndReload();
                    Balancer.balancer = local;
                    break;
            }
        }

        if (afterMeetingRevived)
        {
            revive = true;
            return;
        }
        local.Revive();
    }
}