using TheOtherRoles.Objects;
using static TheOtherRoles.Options.ModOption;

namespace TheOtherRoles.Roles.Neutral;

public class Amnisiac
{
    public static List<PlayerControl> Player = new();
    public static List<Arrow> localArrows = new();
    public static Color color = new(0.5f, 0.7f, 1f, 1f);

    public static bool showArrows = true;
    public static bool resetRole;

    public static Sprite buttonSprite = new ResourceSprite("Remember.png");

    public static void clearAndReload()
    {
        Player.Clear();
        showArrows = CustomOptionHolder.amnisiacShowArrows.GetBool();
        resetRole = CustomOptionHolder.amnisiacResetRole.GetBool();

        foreach (var arrow in localArrows)
            if (arrow?.arrow != null)
                UObject.Destroy(arrow.arrow);
        localArrows.Clear();
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
            case RoleId.SoulSight:
                if (resetRole) SoulSight.ClearAndReload();
                Survivor.Player.Add(target);
                break;
            case RoleId.BandLeader:
                Survivor.Player.Add(target);
                break;

            /*case RoleId.Avenger:
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
            case RoleId.Medium:
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

    public static void TakeRole(byte targetId, byte playerId)
    {
        var target = PlayerById(targetId);
        var local = PlayerById(playerId);
        if (target == null || local == null) return;
        var role = RoleInfo.getRoleInfoForPlayer(target, false, false).FirstOrDefault();
        if (role == null) return;

        ReloadRole(role.roleId, target);
        if (role.roleId == RoleId.Avenger) role = RoleInfo.jester;

        RPCProcedure.setRole(local.PlayerId, (byte)role.roleId);


        if (PlayerControl.LocalPlayer == Arsonist.arsonist)
        {
            var playerCounter = 0;
            var bottomLeft = new Vector3(
                -FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x,
                FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y,
                FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z);
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
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
        else if (PlayerControl.LocalPlayer == BountyHunter.bountyHunter)
        {
            BountyHunter.bountyUpdateTimer = 0f;

            var bottomLeft =
                new Vector3(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x,
                    FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y,
                    FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z) +
                new Vector3(-0.25f, 1f, 0);
            BountyHunter.cooldownText =
                UObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
                    FastDestroyableSingleton<HudManager>.Instance.transform);
            BountyHunter.cooldownText.alignment = TextAlignmentOptions.Center;
            BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -1f, -1f);
            BountyHunter.cooldownText.gameObject.SetActive(true);

            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                if (playerIcons.ContainsKey(p.PlayerId))
                {
                    playerIcons[p.PlayerId].setSemiTransparent(false);
                    playerIcons[p.PlayerId].transform.localPosition = bottomLeft + new Vector3(0f, -1f, 0);
                    playerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.4f;
                    playerIcons[p.PlayerId].gameObject.SetActive(false);
                }
        }
        else if (Sheriff.formerDeputy == target)
        {
            Sheriff.formerDeputy = local;
        }

        Player.RemoveAll(x => x.PlayerId == local.PlayerId);
        foreach (var arrow in localArrows) UObject.Destroy(arrow.arrow);
        localArrows.Clear();
    }

}
