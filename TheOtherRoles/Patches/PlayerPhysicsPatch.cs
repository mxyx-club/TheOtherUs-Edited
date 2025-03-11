using TheOtherRoles.CustomCosmetics.CustomHats;
using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
public static class PlayerPhysicsPatches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate)), HarmonyPostfix]
    public static void PlayerPhysicsUpdatePatch(PlayerPhysics __instance)
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

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.Awake)), HarmonyPostfix]
    public static void PlayerPhysicsAwakePatch(PlayerPhysics __instance)
    {
        if (!__instance.body) return;
        __instance.body.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    // This patch is for the custom hats
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation)), HarmonyPostfix]
    private static void HandleAnimationPostfix(PlayerPhysics __instance)
    {
        var currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();
        if (currentAnimation == __instance.Animations.group.ClimbUpAnim) return;
        if (currentAnimation == __instance.Animations.group.ClimbDownAnim) return;
        var hatParent = __instance.myPlayer.cosmetics.hat;
        if (hatParent == null) return;
        if (!hatParent.TryGetCached(out var viewData)) return;
        var extend = hatParent.Hat.GetHatExtension();
        if (extend == null) return;
        if (extend.FlipImage != null)
        {
            if (__instance.FlipX)
                hatParent.FrontLayer.sprite = extend.FlipImage;
            else
            {
                hatParent.FrontLayer.sprite = viewData.MainImage;
            }
        }

        if (extend.BackFlipImage != null)
        {
            if (__instance.FlipX)
                hatParent.BackLayer.sprite = extend.BackFlipImage;
            else
            {
                hatParent.BackLayer.sprite = viewData.BackImage;
            }
        }
    }
}