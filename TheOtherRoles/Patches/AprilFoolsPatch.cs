namespace TheOtherRoles.Patches;

[HarmonyPatch]
internal class AprilFoolsPatch
{
    [HarmonyPatch(typeof(Constants), nameof(Constants.ShouldHorseAround)), HarmonyPrefix]
    public static bool ShouldHorseAroundPrefix()
    {
        return false;
    }
    /*
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.BodyType), MethodType.Getter), HarmonyPrefix]

    public static bool PlayerControlBodyTypePrefix(ref PlayerBodyTypes __result)
    {
        __result = PlayerBodyTypes.Normal;
        return false;
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetBodyType)), HarmonyPrefix]

    public static void PlayerPhysicsBodyTypePrefix(ref PlayerBodyTypes bodyType)
    {
        bodyType = PlayerBodyTypes.Normal;
    }

    [HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.SetBodyType)), HarmonyPrefix]

    public static void PoolablePlayerBodyTypePrefix(ref PlayerBodyTypes bodyType)
    {
        bodyType = PlayerBodyTypes.Normal;
    }

    [HarmonyPatch(typeof(PlayerAnimations), nameof(PlayerAnimations.SetBodyType)), HarmonyPrefix]

    public static void PlayerAnimationsBodyTypePrefix(ref PlayerBodyTypes bodyType)
    {
        bodyType = PlayerBodyTypes.Normal;
    }*/
}