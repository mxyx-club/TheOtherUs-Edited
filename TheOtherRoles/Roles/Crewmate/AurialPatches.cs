namespace TheOtherRoles.Roles.Crewmate;

/// <summary>
/// Applies the Aurial's poor base vision after the normal crew-light and
/// Sunglasses calculations. Torch deliberately keeps its existing override.
/// </summary>
[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
internal static class AurialLightRadiusPatch
{
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo player)
    {
        if (player == null || Aurial.aurial == null || !Aurial.aurial.IsAlive() ||
            player.PlayerId != Aurial.aurial.PlayerId)
            return;

        // ShipStatusPatch intentionally lets Torch replace every lower-level
        // vision modifier, and Aurial follows the same precedence rule.
        if (Torch.torch.Any(torch => torch != null && torch.PlayerId == player.PlayerId))
            return;

        __result *= Aurial.vision;
    }
}
