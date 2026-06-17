namespace TheOtherRoles.Patches;

[HarmonyPatch]
public static class PlayerPhysicsPatches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public class PlayerPhysicsCoSpawnPatch
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (PlayerControl.LocalPlayer == __instance.myPlayer && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
            {
                _ = new LateTask(() =>
                {
                    if (__instance.myPlayer.IsAlive())
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(__instance.myPlayer, GetWelcomeMessage);
                }, 1f, "Welcome Chat");
            }
        }

        private static string GetWelcomeMessage => "WelcomeText".Translate();
    }

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
                if (Phantom.Player != null && Phantom.Player == PlayerControl.LocalPlayer && Phantom.isInvisable)
                    __instance.body.velocity *= Phantom.swoopSpeed;
                if (Undertaker.undertaker.IsAlive() && Undertaker.undertaker == PlayerControl.LocalPlayer && Undertaker.dragedBody != null)
                    __instance.body.velocity *= Undertaker.velocity;
                if (Jester.Player.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer.IsAlive() && Jester.dragedBodys.GetValueOrDefault(PlayerControl.LocalPlayer.PlayerId) != null)
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