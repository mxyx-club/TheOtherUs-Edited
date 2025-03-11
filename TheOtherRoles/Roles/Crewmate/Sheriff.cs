using System.Collections.Generic;
using System.Linq;
using Hazel;
using TheOtherRoles.Buttons;
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
            playerId = PlayerControl.LocalPlayer.PlayerId;

        if (active && playerId == PlayerControl.LocalPlayer.PlayerId)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffNoticed);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        if (active)
        {
            handcuffedKnows.Add(playerId, handcuffDuration);
            handcuffedPlayers.RemoveAll(x => x == playerId);
        }

        if (playerId == PlayerControl.LocalPlayer.PlayerId)
        {
            HudManagerStartPatch.setAllButtonsHandcuffedStatus(active);
            SoundEffectsManager.play("deputyHandcuff");
        }
    }

    public static void deputyCheckPromotion(bool isMeeting = false)
    {
        if (Deputy == null || Deputy != PlayerControl.LocalPlayer) return;
        if (promotesToSheriff == 0 || Deputy.IsDead() || (promotesToSheriff == 2 && !isMeeting)) return;
        if (Player.Count == 0 || Player.All(x => x.IsDead()))
        {
            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.DeputyPromotes);
            writer.EndRPC();
            replaceCurrentSheriff();
        }
    }

    public static void replaceCurrentSheriff()
    {
        formerDeputy = Deputy;
        Player.TryAdd(Deputy);
        Deputy = null;
        currentTarget = null;
        cooldown = CustomOptionHolder.sheriffCooldown.GetFloat();
    }

    public static bool sheriffCanKillNeutral(PlayerControl target)
    {
        return (target != Mini.mini || Mini.isGrownUp()) &&
               (target.IsImpostor(CustomOptionHolder.spyCanDieToSheriff.GetBool()) ||
                (canKillNeutrals &&
                 (isKillerNeutral(target) ||
                  Akujo.akujo == target ||
                  SchrodingersCat.Player == target ||
                  (BandLeader.Player == target && CustomOptionHolder.sheriffCanKillBandLeader.GetBool()) ||
                  (Witness.Player == target && CustomOptionHolder.sheriffCanKillWitness.GetBool()) ||
                  (Amnisiac.Player.Any(p => p == target) && CustomOptionHolder.sheriffCanKillAmnesiac.GetBool()) ||
                  (Survivor.Player.Any(p => p == target) && CustomOptionHolder.sheriffCanKillSurvivor.GetBool()) ||
                  (Pursuer.Player.Any(p => p == target) && CustomOptionHolder.sheriffCanKillPursuer.GetBool()) ||
                  (Jester.jester == target && CustomOptionHolder.sheriffCanKillJester.GetBool()) ||
                  (Vulture.vulture == target && CustomOptionHolder.sheriffCanKillVulture.GetBool()) ||
                  (Thief.thief == target && CustomOptionHolder.sheriffCanKillThief.GetBool()) ||
                  (PartTimer.partTimer == target && CustomOptionHolder.sheriffCanKillPartTimer.GetBool()) ||
                  (Lawyer.lawyer == target && CustomOptionHolder.sheriffCanKillLawyer.GetBool()) ||
                  (Executioner.executioner == target && CustomOptionHolder.sheriffCanKillExecutioner.GetBool()) ||
                  (Doomsayer.doomsayer == target && CustomOptionHolder.sheriffCanKillDoomsayer.GetBool()
                  ))));
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
        Reload();
    }

    public static void Reload()
    {
        misfireKills = CustomOptionHolder.sheriffMisfireKills.GetSelection();
        cooldown = CustomOptionHolder.sheriffCooldown.GetFloat();
        canKillNeutrals = CustomOptionHolder.sheriffCanKillNeutrals.GetBool();

        promotesToSheriff = CustomOptionHolder.deputyGetsPromoted.GetSelection();
        remainingHandcuffs = CustomOptionHolder.deputyNumberOfHandcuffs.GetFloat();
        handcuffCooldown = CustomOptionHolder.deputyHandcuffCooldown.GetFloat();
        keepsHandcuffsOnPromotion = CustomOptionHolder.deputyKeepsHandcuffs.GetBool();
        handcuffDuration = CustomOptionHolder.deputyHandcuffDuration.GetFloat();
        knowsSheriff = CustomOptionHolder.deputyKnowsSheriff.GetBool();
    }
}
