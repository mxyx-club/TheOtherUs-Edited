using System.Collections.Generic;
using System.Linq;
using Hazel;
using TheOtherRoles.Buttons;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Sheriff
{
    public static List<PlayerControl> Player = new();
    public static PlayerControl Deputy;
    public static PlayerControl formerDeputy; // Needed for keeping handcuffs + shifting
    public static PlayerControl currentTarget;

    public static Color color = new Color32(248, 205, 70, byte.MaxValue);

    public static float cooldown = 30f;
    public static bool canKillNeutrals;
    public static bool canKillLawyer;
    public static bool canKillSurvivor;
    public static bool canKillJester;
    public static bool canKillPursuer;
    public static bool canKillPartTimer;
    public static bool canKillVulture;
    public static bool canKillThief;
    public static bool canKillAmnesiac;
    public static bool canKillExecutioner;
    public static bool canKillDoomsayer;
    public static bool spyCanDieToSheriff;
    public static int misfireKills; // Self: 0, Target: 1, Both: 2

    //Deputy
    public static List<byte> handcuffedPlayers = new();
    public static int promotesToSheriff; // No: 0, Immediately: 1, After Meeting: 2
    public static bool keepsHandcuffsOnPromotion;
    public static float handcuffDuration;
    public static float remainingHandcuffs;
    public static float handcuffCooldown;
    public static bool knowsSheriff;
    public static Dictionary<byte, float> handcuffedKnows = new();

    public static ResourceSprite handcuffSprite = new("DeputyHandcuffButton.png");
    public static ResourceSprite handcuffedSprite = new("DeputyHandcuffed.png");

    // Can be used to enable / disable the handcuff effect on the target's buttons
    public static void setHandcuffedKnows(bool active = true, byte playerId = byte.MaxValue)
    {
        if (playerId == byte.MaxValue)
            playerId = CachedPlayer.LocalPlayer.PlayerId;

        if (active && playerId == CachedPlayer.LocalPlayer.PlayerId)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
            writer.Write(CachedPlayer.LocalPlayer.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffNoticed);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        if (active)
        {
            handcuffedKnows.Add(playerId, handcuffDuration);
            handcuffedPlayers.RemoveAll(x => x == playerId);
        }

        if (playerId == CachedPlayer.LocalPlayer.PlayerId)
        {
            HudManagerStartPatch.setAllButtonsHandcuffedStatus(active);
            SoundEffectsManager.play("deputyHandcuff");
        }
    }

    public static void replaceCurrentSheriff()
    {
        if (Deputy == null) return;
        Player ??= new();
        formerDeputy = Deputy;
        Player.Add(Deputy);
        Deputy = null;
        currentTarget = null;
        cooldown = CustomOptionHolder.sheriffCooldown.GetFloat();
    }

    public static bool sheriffCanKillNeutral(PlayerControl target)
    {
        return (target != Mini.mini || Mini.isGrownUp()) &&
               (target.Data.Role.IsImpostor ||
                Jackal.jackal.Any(x => x == target) ||
                Jackal.Sidekick == target ||
                Juggernaut.juggernaut == target ||
                Werewolf.werewolf == target ||
                Swooper.swooper == target ||
                Pavlovsdogs.pavlovsowner == target ||
                Pavlovsdogs.pavlovsdogs.Any(p => p == target) ||
                (spyCanDieToSheriff && Spy.spy == target) ||
                (canKillNeutrals &&
                    (Akujo.akujo == target || isKillerNeutral(target) ||
                        (Survivor.Player.Any(p => p == target) && canKillSurvivor) ||
                        (Jester.jester == target && canKillJester) ||
                        (Vulture.vulture == target && canKillVulture) ||
                        (Thief.thief == target && canKillThief) || Witness.Player == target ||
                        (Amnisiac.Player.Any(p => p == target) && canKillAmnesiac) ||
                        (PartTimer.partTimer == target && canKillPartTimer) ||
                        (Lawyer.lawyer == target && canKillLawyer) ||
                        (Executioner.executioner == target && canKillExecutioner) ||
                        (Pursuer.Player.Any(p => p == target) && canKillPursuer) ||
                        (Doomsayer.doomsayer == target && canKillDoomsayer))));
    }

    public static void clearAndReload(bool resetCuffs = true)
    {
        if (resetCuffs)
        {
            handcuffedPlayers = new();
            handcuffedKnows = new();
            HudManagerStartPatch.setAllButtonsHandcuffedStatus(false, true);
        }
        Player = new();
        currentTarget = null;
        formerDeputy = null;

        Deputy = null;
        currentTarget = null;
        Reload();
    }

    public static void Reload()
    {
        misfireKills = CustomOptionHolder.sheriffMisfireKills.GetSelection();
        cooldown = CustomOptionHolder.sheriffCooldown.GetFloat();
        canKillNeutrals = CustomOptionHolder.sheriffCanKillNeutrals.GetBool();
        canKillSurvivor = CustomOptionHolder.sheriffCanKillSurvivor.GetBool();
        canKillLawyer = CustomOptionHolder.sheriffCanKillLawyer.GetBool();
        canKillJester = CustomOptionHolder.sheriffCanKillJester.GetBool();
        canKillPursuer = CustomOptionHolder.sheriffCanKillPursuer.GetBool();
        canKillPartTimer = CustomOptionHolder.sheriffCanKillPartTimer.GetBool();
        canKillVulture = CustomOptionHolder.sheriffCanKillVulture.GetBool();
        canKillThief = CustomOptionHolder.sheriffCanKillThief.GetBool();
        canKillAmnesiac = CustomOptionHolder.sheriffCanKillAmnesiac.GetBool();
        canKillExecutioner = CustomOptionHolder.sheriffCanKillExecutioner.GetBool();
        spyCanDieToSheriff = CustomOptionHolder.spyCanDieToSheriff.GetBool();
        canKillDoomsayer = CustomOptionHolder.sheriffCanKillDoomsayer.GetBool();

        promotesToSheriff = CustomOptionHolder.deputyGetsPromoted.GetSelection();
        remainingHandcuffs = CustomOptionHolder.deputyNumberOfHandcuffs.GetFloat();
        handcuffCooldown = CustomOptionHolder.deputyHandcuffCooldown.GetFloat();
        keepsHandcuffsOnPromotion = CustomOptionHolder.deputyKeepsHandcuffs.GetBool();
        handcuffDuration = CustomOptionHolder.deputyHandcuffDuration.GetFloat();
        knowsSheriff = CustomOptionHolder.deputyKnowsSheriff.GetBool();
    }
}
