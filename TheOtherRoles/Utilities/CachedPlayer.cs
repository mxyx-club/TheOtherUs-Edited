using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TheOtherRoles.Utilities;

public class CachedPlayer
{
    public static readonly Dictionary<IntPtr, CachedPlayer> PlayerPtrs = new();
    public static readonly List<CachedPlayer> AllPlayers = new();
    public static CachedPlayer LocalPlayer;
    public NetworkedPlayerInfo Data => PlayerControl.Data;
    public CustomNetworkTransform NetTransform;
    public PlayerControl PlayerControl;
    public byte PlayerId;
    public PlayerPhysics PlayerPhysics;
    public Transform transform;

    public static byte LocalId => LocalPlayer.PlayerId;
    public string PlayerName => Data.PlayerName;
    public float PlayerSpeed => PlayerPhysics.Speed;
    public float TrueSpeed => PlayerPhysics.TrueSpeed;
    public float SpeedMod => PlayerPhysics.SpeedMod;
    public float GhostSpeed => PlayerPhysics.GhostSpeed;

    public bool Disconnected => Data.Disconnected;
    public bool CanMove => PlayerControl.CanMove;
    public bool IsAlive => !PlayerControl.Data.IsDead;
    public bool IsDead => PlayerControl.Data.IsDead;
    public bool IsDummy => PlayerControl.isDummy;
    public bool InVent => PlayerControl.inVent;

    public Vector2 LastPosition => NetTransform.lastPosition;
    public Vector2 TruePosition => PlayerControl.GetTruePosition();
    public Vector2 ControlOffset => PlayerControl.Collider.offset;

    public static implicit operator bool(CachedPlayer player) => player != null && player.PlayerControl;
    public static implicit operator PlayerControl(CachedPlayer player) => player.PlayerControl;
    public static implicit operator PlayerPhysics(CachedPlayer player) => player.PlayerPhysics;
}

[HarmonyPatch]
public static class CachedPlayerPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Awake))]
    [HarmonyPostfix]
    public static void CachePlayerPatch(PlayerControl __instance)
    {
        if (__instance.notRealPlayer) return;
        var player = new CachedPlayer
        {
            transform = __instance.transform,
            PlayerControl = __instance,
            PlayerPhysics = __instance.MyPhysics,
            NetTransform = __instance.NetTransform
        };
        CachedPlayer.AllPlayers.Add(player);
        CachedPlayer.PlayerPtrs[__instance.Pointer] = player;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    [HarmonyPostfix]
    public static void RemoveCachedPlayerPatch(PlayerControl __instance)
    {
        if (__instance.notRealPlayer) return;
        CachedPlayer.AllPlayers.RemoveAll(p => p.PlayerControl.Pointer == __instance.Pointer);
        CachedPlayer.PlayerPtrs.Remove(__instance.Pointer);
    }

    [HarmonyPatch(typeof(NetworkedPlayerInfo), nameof(NetworkedPlayerInfo.Deserialize))]
    [HarmonyPostfix]
    public static void AddCachedDataOnDeserialize()
    {
        foreach (var cachedPlayer in CachedPlayer.AllPlayers)
        {
            cachedPlayer.PlayerId = cachedPlayer.PlayerControl.PlayerId;
        }
    }

    [HarmonyPatch(typeof(GameData), nameof(GameData.AddPlayer))]
    [HarmonyPostfix]
    public static void AddCachedDataOnAddPlayer()
    {
        foreach (var cachedPlayer in CachedPlayer.AllPlayers)
        {
            cachedPlayer.PlayerId = cachedPlayer.PlayerControl.PlayerId;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Deserialize))]
    [HarmonyPostfix]
    public static void SetCachedPlayerId(PlayerControl __instance)
    {
        CachedPlayer.PlayerPtrs[__instance.Pointer].PlayerId = __instance.PlayerId;
    }

    [HarmonyPatch]
    private class CacheLocalPlayerPatch
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            var type = typeof(PlayerControl).GetNestedTypes(AccessTools.all).FirstOrDefault(t => t.Name.Contains("Start"));
            return AccessTools.Method(type, nameof(IEnumerator.MoveNext));
        }

        [HarmonyPostfix]
        public static void SetLocalPlayer()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (!localPlayer)
            {
                CachedPlayer.LocalPlayer = null;
                return;
            }

            var cached = CachedPlayer.AllPlayers.FirstOrDefault(p => p.PlayerControl.Pointer == localPlayer.Pointer);
            if (cached != null) CachedPlayer.LocalPlayer = cached;
        }
    }
}