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
                if (Undertaker.undertaker.IsAlive() && Undertaker.undertaker == PlayerControl.LocalPlayer && Undertaker.dragedBody != null)
                    __instance.body.velocity *= Undertaker.velocity;
                if (Jester.jester.IsAlive() && Jester.jester == PlayerControl.LocalPlayer && Jester.dragedBody != null)
                    __instance.body.velocity *= Jester.velocity;
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
}