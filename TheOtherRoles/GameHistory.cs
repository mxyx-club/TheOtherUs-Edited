using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheOtherRoles;

public enum CustomDeathReason
{
    HostCmdKill,
    Exile,
    Kill,
    Disconnect,
    Guess,
    Shift,
    LawyerSuicide,
    LoverSuicide, // not necessary
    WitchExile,
    Bomb,
    LoveStolen,
    Loneliness,
    Arson,
    FakeSK,
    SheriffKill,
    SheriffMisfire,
    SheriffMisadventure,
    Suicide,
    BombVictim,
    Eaten,
}
public class DeadPlayer
{
    public CustomDeathReason DeathReason { get; set; }
    public PlayerControl KillerIfExisting { get; set; }
    public PlayerControl Player { get; }
    public DateTime TimeOfDeath { get; }
    public bool wasCleaned { get; set; }

    public DeadPlayer(PlayerControl Player, DateTime TimeOfDeath, CustomDeathReason DeathReason, PlayerControl KillerIfExisting)
    {
        this.Player = Player;
        this.TimeOfDeath = TimeOfDeath;
        this.DeathReason = DeathReason;
        this.KillerIfExisting = KillerIfExisting;
        wasCleaned = false;
    }
}

internal static class GameHistory
{
    public static List<Tuple<Vector3, bool>> localPlayerPositions = new();
    public static List<DeadPlayer> DeadPlayers = new();

    public static void Clear()
    {
        localPlayerPositions.Clear();
        DeadPlayers.Clear();
    }

    public static void ClearDeadPlayer(PlayerControl player)
    {
        DeadPlayers.RemoveAll(x => x.Player.PlayerId == player.PlayerId);
    }

    public static void OverrideDeathReasonAndKiller(PlayerControl player, CustomDeathReason deathReason, PlayerControl killer = null)
    {
        if (player.IsAlive()) return;
        var target = DeadPlayers.FirstOrDefault(x => x.Player.PlayerId == player.PlayerId);
        byte playerId = player.PlayerId;
        if (target != null)
        {
            target.DeathReason = deathReason;
            if (killer != null) target.KillerIfExisting = killer;
        }
        else if (player != null)
        {
            // Create dead player if needed:
            var dp = new DeadPlayer(player, DateTime.UtcNow, deathReason, killer);
            DeadPlayers.Add(dp);
        }
    }

    public static void RpcOverrideDeathReasonAndKiller(PlayerControl player, CustomDeathReason deathReason, PlayerControl killer)
    {
        if (player.IsAlive()) return;
        var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareGhostInfo);
        writer.Write(player.PlayerId);
        writer.Write((byte)RPCProcedure.GhostInfoTypes.DeathReasonAndKiller);
        writer.Write(player.PlayerId);
        writer.Write((byte)deathReason);
        writer.Write(killer.PlayerId);
        writer.EndRPC();
        OverrideDeathReasonAndKiller(player, deathReason, killer);
    }

    public static int GetKillCount(PlayerControl killer)
    {
        if (killer == null) return 0;

        return DeadPlayers.Count(dp => dp.KillerIfExisting == killer && dp.Player != killer);
    }

    public static PlayerControl GetLastKiller()
    {
        var db = DeadPlayers?
            .Where(dp => dp.Player != dp.KillerIfExisting && dp.KillerIfExisting.IsAlive())
            .OrderByDescending(dp => dp.TimeOfDeath)
            .FirstOrDefault();
        return db?.KillerIfExisting;
    }
}