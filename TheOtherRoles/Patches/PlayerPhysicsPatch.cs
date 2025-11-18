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
                if (PlayerControl.LocalPlayer.GetModifier().Contains(RoleId.Flash))
                    __instance.body.velocity *= Flash.speed;
                if (PlayerControl.LocalPlayer.GetModifier().Contains(RoleId.Giant) && !MushroomSabotageActive && !isActiveCamoComms)
                    __instance.body.velocity *= Giant.speed;
                if (PlayerControl.LocalPlayer.TryGetRole<Swooper>(out var swooper) && swooper.IsInvisable)
                    __instance.body.velocity *= Swooper.swoopSpeed;
                if (PlayerControl.LocalPlayer.TryGetRole<Undertaker>(out var undertaker) && undertaker.dragedBody != null)
                    __instance.body.velocity *= Undertaker.velocity;
                if (PlayerControl.LocalPlayer.TryGetRole<Jester>(out var jester) && jester.dragedBody != null)
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