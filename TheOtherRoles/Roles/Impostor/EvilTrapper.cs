using System;
using Hazel;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Impostor;

public static class EvilTrapper
{
    public static PlayerControl evilTrapper;
    public static Color color = Palette.ImpostorRed;

    public static float minDistance;
    public static float maxDistance;
    public static int numTrap;
    public static float extensionTime;
    public static float killTimer;
    public static float cooldown;
    public static float trapRange;
    public static float penaltyTime;
    public static float bonusTime;
    public static bool isTrapKill;
    public static bool meetingFlag;

    public static Sprite trapButtonSprite;
    public static DateTime placedTime;

    public static Sprite getTrapButtonSprite()
    {
        if (trapButtonSprite) return trapButtonSprite;
        trapButtonSprite = loadSpriteFromResources("TheOtherRoles.Resources.TrapperButton.png", 115f);
        return trapButtonSprite;
    }

    public static void setTrap()
    {
        var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
        byte[] buff = new byte[sizeof(float) * 2];
        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
        MessageWriter writer = AmongUsClient.Instance.StartRpc(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.PlaceTrap, SendOption.Reliable);
        writer.WriteBytesAndSize(buff);
        writer.EndMessage();
        RPCProcedure.placeTrap(buff);
        placedTime = DateTime.UtcNow;
    }

    public static void clearAndReload()
    {
        evilTrapper = null;
        numTrap = (int)CustomOptionHolder.evilTrapperNumTrap.getFloat();
        extensionTime = CustomOptionHolder.evilTrapperExtensionTime.getFloat();
        killTimer = CustomOptionHolder.evilTrapperKillTimer.getFloat();
        cooldown = CustomOptionHolder.evilTrapperCooldown.getFloat();
        maxDistance = CustomOptionHolder.evilTrapperMaxDistance.getFloat();
        trapRange = CustomOptionHolder.evilTrapperTrapRange.getFloat();
        penaltyTime = CustomOptionHolder.evilTrapperPenaltyTime.getFloat();
        bonusTime = CustomOptionHolder.evilTrapperBonusTime.getFloat();
        meetingFlag = false;
        KillTrap.clearAllTraps();
    }
}
