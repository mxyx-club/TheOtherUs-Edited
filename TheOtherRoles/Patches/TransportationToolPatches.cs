namespace TheOtherRoles.Patches;

[HarmonyPatch]
public static class TransportationToolPatches
{
    /*
     * Moving Plattform / Zipline / Ladders move the player out of bounds, thus we want to disable functions of the mod if the player is currently using one of these.
     * Save the players anti tp position before using it.
     *
     * Zipline can also break camo, fix that one too.
     */

    // Zipline:
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
    public static void prefix3(ZiplineBehaviour __instance, PlayerControl player, bool fromTop)
    {
        if (PlayerControl.LocalPlayer.TryGetModifier<AntiTeleport>(out var antiTeleport))
            antiTeleport.position = PlayerControl.LocalPlayer.transform.position;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
    public static void postfix(ZiplineBehaviour __instance, PlayerControl player, bool fromTop)
    {
        // Fix camo:
        __instance.StartCoroutine(Effects.Lerp(fromTop ? __instance.downTravelTime : __instance.upTravelTime,
            new Action<float>(p =>
            {
                __instance.playerIdHands.TryGetValue(player.PlayerId, out var hand);
                if (hand != null)
                {
                    if (!isActiveCamoComms && !MushroomSabotageActive)
                    {
                        if (player.TryGetRole<Morphling>(out var morphling) && morphling.morphTimer > 0)
                        {
                            hand.SetPlayerColor(morphling.morphTarget.CurrentOutfit, PlayerMaterial.MaskType.None);
                            // Also set hat color, cause the line destroys it...
                            player.RawSetHat(morphling.morphTarget.Data.DefaultOutfit.HatId,
                                morphling.morphTarget.Data.DefaultOutfit.ColorId);
                        }
                        else
                        {
                            hand.SetPlayerColor(player.CurrentOutfit, PlayerMaterial.MaskType.None);
                        }
                    }
                    else
                    {
                        PlayerMaterial.SetColors(6, hand.handRenderer);
                    }
                }
            })));
    }

    // Save the position of the player prior to starting the climb / gap platform
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
    public static void prefix()
    {
        if (PlayerControl.LocalPlayer.TryGetModifier<AntiTeleport>(out var antiTeleport))
            antiTeleport.position = PlayerControl.LocalPlayer.transform.position;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
    public static void postfix2(PlayerPhysics __instance, Ladder source, byte climbLadderSid)
    {
        // Fix camo:
        var player = __instance.myPlayer;
        __instance.StartCoroutine(Effects.Lerp(5.0f, new Action<float>(p =>
        {
            if (isActiveCamoComms && !MushroomSabotageActive &&
                player.TryGetRole<Morphling>(out var morphling) && morphling.morphTimer > 0.1f)
            {
                player.RawSetHat(morphling.morphTarget.Data.DefaultOutfit.HatId,
                    morphling.morphTarget.Data.DefaultOutfit.ColorId);
            }
        })));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.UsePlatform))]
    public static void prefix2()
    {
        if (PlayerControl.LocalPlayer.TryGetModifier<AntiTeleport>(out var antiTeleport))
            antiTeleport.position = PlayerControl.LocalPlayer.transform.position;
    }
}