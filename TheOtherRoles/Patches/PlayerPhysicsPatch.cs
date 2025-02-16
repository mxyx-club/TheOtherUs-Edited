using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
public static class PlayerPhysicsUpdatePatch
{
    public static void Postfix(PlayerPhysics __instance)
    {
        if (InGame && __instance && __instance.AmOwner && __instance.myPlayer.CanMove)
        {
            if (PlayerControl.LocalPlayer.IsAlive())
            {
                if (Flash.flash != null && Flash.flash.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId))
                    __instance.body.velocity *= Flash.speed;
                if (Giant.giant != null && Giant.giant == PlayerControl.LocalPlayer && !MushroomSabotageActive && !isCamoComms && Camouflager.camouflageTimer <= 0f)
                    __instance.body.velocity *= Giant.speed;
                if (Swooper.swooper != null && Swooper.swooper == PlayerControl.LocalPlayer && Swooper.isInvisable)
                    __instance.body.velocity *= Swooper.swoopSpeed;
                if (Undertaker.deadBodyDraged != null && __instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                    __instance.body.velocity *= Undertaker.velocity;
            }
            else if (PlayerControl.LocalPlayer.IsDead())
            {
                __instance.body.velocity *= CustomOptionHolder.ghostSpeed.GetFloat();
            }
        }
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