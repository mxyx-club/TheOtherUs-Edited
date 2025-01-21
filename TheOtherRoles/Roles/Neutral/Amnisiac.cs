using System.Collections.Generic;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.Options.ModOption;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Roles.Neutral;

public class Amnisiac
{
    public static List<PlayerControl> Player = new();
    public static List<Arrow> localArrows = new();
    public static Color color = new(0.5f, 0.7f, 1f, 1f);
    public static List<PoolablePlayer> poolIcons = new();

    public static bool showArrows = true;
    public static bool resetRole;

    public static ResourceSprite buttonSprite = new("Remember.png");

    public static void clearAndReload()
    {
        Player.Clear();
        showArrows = CustomOptionHolder.amnisiacShowArrows.GetBool();
        resetRole = CustomOptionHolder.amnisiacResetRole.GetBool();
        if (localArrows != null)
            foreach (var arrow in localArrows)
                if (arrow?.arrow != null)
                    Object.Destroy(arrow.arrow);
        localArrows.Clear();
    }

    public static void TakeRole(byte targetId, byte playerId)
    {
        var target = playerById(targetId);
        var local = playerById(playerId);
        if (target == null || local == null) return;
        var targetInfo = RoleInfo.getRoleInfoForPlayer(target, false, false);
        var roleInfo = targetInfo.FirstOrDefault();
        if (target.isImpostor()) turnToImpostor(local);
        switch (roleInfo!.roleId)
        {
            case RoleId.Impostor:
                break;
            case RoleId.Morphling:
                if (resetRole) Morphling.clearAndReload();
                Morphling.morphling = local;
                break;
            case RoleId.WolfLord:
                if (resetRole) WolfLord.ClearAndReload();
                WolfLord.Player = local;
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
                Butcher.butcher = local;
                break;
            case RoleId.Mimic:
                if (resetRole) Mimic.clearAndReload(false);
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

                BountyHunter.bountyUpdateTimer = 0f;
                if (CachedPlayer.LocalPlayer.PlayerControl == BountyHunter.bountyHunter)
                {
                    var bottomLeft =
                        new Vector3(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x,
                            FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y,
                            FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z) +
                        new Vector3(-0.25f, 1f, 0);
                    BountyHunter.cooldownText =
                        Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
                            FastDestroyableSingleton<HudManager>.Instance.transform);
                    BountyHunter.cooldownText.alignment = TextAlignmentOptions.Center;
                    BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -1f, -1f);
                    BountyHunter.cooldownText.gameObject.SetActive(true);

                    foreach (PlayerControl p in CachedPlayer.AllPlayers)
                        if (playerIcons.ContainsKey(p.PlayerId))
                        {
                            playerIcons[p.PlayerId].setSemiTransparent(false);
                            playerIcons[p.PlayerId].transform.localPosition = bottomLeft + new Vector3(0f, -1f, 0);
                            playerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.4f;
                            playerIcons[p.PlayerId].gameObject.SetActive(false);
                        }
                }
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
                Gambler.gambler = local;
                break;
            case RoleId.Grenadier:
                if (resetRole) Grenadier.clearAndReload();
                Grenadier.grenadier = local;
                break;

            case RoleId.Survivor:
                Survivor.Player.Add(local);
                break;
            case RoleId.Jester:
                if (resetRole) Jester.clearAndReload();
                Jester.jester = local;
                Player.Add(target);
                break;
            case RoleId.Vulture:
                if (resetRole) Vulture.clearAndReload();
                Vulture.vulture = local;
                Player.Add(target);
                break;
            case RoleId.Lawyer:
                Lawyer.lawyer = local;
                Player.Add(target);
                break;
            case RoleId.Executioner:
                Executioner.executioner = local;
                Player.Add(target);
                break;
            case RoleId.Pursuer:
                if (resetRole) Pursuer.clearAndReload();
                Pursuer.Player.Add(local);
                Player.Add(target);
                break;
            case RoleId.PartTimer:
                if (resetRole) PartTimer.clearAndReload();
                PartTimer.partTimer = local;
                Player.Add(target);
                break;
            case RoleId.Witness:
                if (resetRole) Witness.ClearAndReload();
                Witness.Player = local;
                Player.Add(target);
                break;
            case RoleId.Doomsayer:
                if (resetRole) Doomsayer.clearAndReload();
                Doomsayer.doomsayer = local;
                Player.Add(target);
                break;
            case RoleId.Arsonist:
                if (resetRole) Arsonist.clearAndReload();
                Arsonist.arsonist = local;
                Player.Add(target);

                if (CachedPlayer.LocalPlayer.PlayerControl == Arsonist.arsonist)
                {
                    var playerCounter = 0;
                    var bottomLeft = new Vector3(
                        -FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x,
                        FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y,
                        FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z);
                    foreach (PlayerControl p in CachedPlayer.AllPlayers)
                        if (playerIcons.ContainsKey(p.PlayerId) && p != Arsonist.arsonist)
                        {
                            //Arsonist.poolIcons.Add(p);
                            if (Arsonist.dousedPlayers.Contains(p))
                                playerIcons[p.PlayerId].setSemiTransparent(false);
                            else
                                playerIcons[p.PlayerId].setSemiTransparent(true);

                            playerIcons[p.PlayerId].transform.localPosition = bottomLeft +
                                                                              new Vector3(-0.25f, -0.25f, 0) +
                                                                              (Vector3.right * playerCounter++ * 0.35f);
                            playerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.2f;
                            playerIcons[p.PlayerId].gameObject.SetActive(true);
                        }
                }
                break;
            case RoleId.Jackal:
                Jackal.jackal.Add(local);
                break;
            case RoleId.Sidekick:
                Jackal.jackal.Add(target);
                Jackal.Sidekick = local;
                break;
            case RoleId.Pavlovsowner:
                Pavlovsdogs.pavlovsdogs.Add(Pavlovsdogs.pavlovsowner);
                Pavlovsdogs.pavlovsowner = local;
                break;
            case RoleId.Pavlovsdogs:
                Pavlovsdogs.pavlovsdogs.Add(local);
                break;
            case RoleId.Werewolf:
                if (resetRole) Werewolf.clearAndReload();
                Werewolf.werewolf = local;
                Player.Add(target);
                break;
            case RoleId.Swooper:
                if (resetRole) Swooper.clearAndReload();
                Swooper.swooper = local;
                Player.Add(target);
                break;
            case RoleId.Juggernaut:
                if (resetRole) Juggernaut.clearAndReload();
                Juggernaut.juggernaut = local;
                Player.Add(target);
                break;
            case RoleId.Pelican:
                if (resetRole) Pelican.clearAndReload(false);
                Pelican.Player = local;
                Player.Add(target);
                break;
            case RoleId.Akujo:
                Akujo.akujo = local;
                Player.Add(target);
                break;
            case RoleId.Thief:
                if (resetRole) Thief.clearAndReload();
                Thief.thief = local;
                Player.Add(target);
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
                Prosecutor.prosecutor = target;
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
                if (resetRole) Sheriff.Reload();
                Sheriff.Deputy = local;
                break;
            case RoleId.BodyGuard:
                if (resetRole) BodyGuard.clearAndReload();
                BodyGuard.bodyguard = local;
                break;
            case RoleId.Jumper:
                if (resetRole) Jumper.clearAndReload();
                Jumper.jumper = local;
                break;
            case RoleId.Detective:
                if (resetRole) Detective.clearAndReload();
                Detective.detective = local;
                break;
            case RoleId.TimeMaster:
                if (resetRole) TimeMaster.clearAndReload();
                TimeMaster.timeMaster = local;
                break;
            case RoleId.Veteran:
                if (resetRole) Veteran.clearAndReload();
                Veteran.veteran = local;
                break;
            case RoleId.Medic:
                if (resetRole) Medic.clearAndReload();
                Medic.medic = local;
                break;
            case RoleId.Swapper:
                if (resetRole) Swapper.clearAndReload();
                Swapper.swapper = local;
                break;
            case RoleId.Seer:
                if (resetRole) Seer.clearAndReload();
                Seer.seer = local;
                break;
            case RoleId.Hacker:
                if (resetRole) Hacker.clearAndReload();
                Hacker.hacker = local;
                break;
            case RoleId.Tracker:
                if (resetRole) Tracker.clearAndReload();
                Tracker.tracker = local;
                break;
            case RoleId.Snitch:
                if (resetRole) Snitch.clearAndReload();
                Snitch.snitch = local;
                break;
            case RoleId.Prophet:
                if (resetRole) Prophet.clearAndReload();
                Prophet.prophet = local;
                break;
            case RoleId.InfoSleuth:
                InfoSleuth.infoSleuth = local;
                break;
            case RoleId.Spy:
                if (resetRole) Spy.clearAndReload();
                Spy.spy = local;
                break;
            case RoleId.SecurityGuard:
                if (resetRole) SecurityGuard.clearAndReload();
                SecurityGuard.securityGuard = local;
                break;
            case RoleId.Medium:
                if (resetRole) Medium.clearAndReload();
                Medium.medium = local;
                break;
            case RoleId.Trapper:
                if (resetRole) Trapper.clearAndReload();
                Trapper.trapper = local;
                break;
            case RoleId.Balancer:
                if (resetRole) Balancer.clearAndReload();
                Balancer.balancer = local;
                break;
        }

        Player.RemoveAll(x => x.PlayerId == local.PlayerId);
    }

}
