using InnerNet;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
public static class PlayerPhysicsUpdatePatch
{
    public static void Postfix(PlayerPhysics __instance)
    {
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started) return;
        updateUndertakerMoveSpeed(__instance);
    }

    private static void updateUndertakerMoveSpeed(PlayerPhysics playerPhysics)
    {
        if (Undertaker.undertaker == null || Undertaker.undertaker != CachedPlayer.LocalPlayer.PlayerControl) return;
        if (Undertaker.deadBodyDraged != null && playerPhysics.AmOwner && GameData.Instance && playerPhysics.myPlayer.CanMove)
            playerPhysics.body.velocity *= Undertaker.velocity;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.Awake))]
public static class PlayerPhysiscs_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix(PlayerPhysics __instance)
    {
        if (!__instance.body) return;
        __instance.body.interpolation = RigidbodyInterpolation2D.Interpolate;
    }
}