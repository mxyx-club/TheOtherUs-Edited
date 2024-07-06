using System.Collections.Generic;
using Hazel;
using TheOtherRoles.Modules;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Deputy
{
    public static PlayerControl deputy;
    public static Color color = Sheriff.color;

    public static PlayerControl currentTarget;
    public static List<byte> handcuffedPlayers = [];
    public static int promotesToSheriff; // No: 0, Immediately: 1, After Meeting: 2
    public static bool keepsHandcuffsOnPromotion;
    public static float handcuffDuration;
    public static float remainingHandcuffs;
    public static float handcuffCooldown;
    public static bool knowsSheriff;
    public static Dictionary<byte, float> handcuffedKnows = [];

    public static ResourceSprite buttonSprite = new("DeputyHandcuffButton.png");
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

    public static void clearAndReload(bool resetCuffs = true)
    {
        if (resetCuffs)
        {
        }
        deputy = null;
        currentTarget = null;
        handcuffedPlayers = [];
        handcuffedKnows = [];
        HudManagerStartPatch.setAllButtonsHandcuffedStatus(false, true);
        promotesToSheriff = CustomOptionHolder.deputyGetsPromoted.getSelection();
        remainingHandcuffs = CustomOptionHolder.deputyNumberOfHandcuffs.getFloat();
        handcuffCooldown = CustomOptionHolder.deputyHandcuffCooldown.getFloat();
        keepsHandcuffsOnPromotion = CustomOptionHolder.deputyKeepsHandcuffs.getBool();
        handcuffDuration = CustomOptionHolder.deputyHandcuffDuration.getFloat();
        knowsSheriff = CustomOptionHolder.deputyKnowsSheriff.getBool();
    }
}
