namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(ShipStatus))]
public class ShipStatusPatch
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start)), HarmonyPostfix]
    public static void ShipStatusStartPrefix(ShipStatus __instance)
    {
        if (LobbyRoleInfo.RolesSummaryUI != null) LobbyRoleInfo.RolesSummaryUI.SetActive(false);

        if (isFungle)
        {
            var fungleShipStatus = __instance.CastFast<FungleShipStatus>();
            if (CustomOptionHolder.TheFungleMushroomMixupOption.GetBool())
            {
                fungleShipStatus.specialSabotage.secondsForAutoHeal = CustomOptionHolder.TheFungleMushroomMixupTime.GetFloat();
            }
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius)), HarmonyPrefix]
    public static bool CalculateLightPrefix(ref float __result, ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player)
    {
        if (!__instance.Systems.ContainsKey(SystemTypes.Electrical) || IsHideNSeek) return true;

        if (Pelican.eatenPlayers.Count > 0 && Pelican.eatenPlayers.Any(x => x.Key.PlayerId == player.PlayerId))
        {
            __result = __instance.MinLightRadius * 1.25f;
            return false;
        }

        // If player is a role which has Impostor vision
        if (HasImpVision(player))
        {
            __result = GetNeutralLightRadius(__instance, true);
            return false;
        }

        // If there is a Trickster with their ability active
        else if (Trickster.trickster != null && Trickster.lightsOutTimer > 0f)
        {
            var lerpValue = 1f;
            if (Trickster.lightsOutDuration - Trickster.lightsOutTimer < 0.5f)
                lerpValue = Mathf.Clamp01((Trickster.lightsOutDuration - Trickster.lightsOutTimer) * 2);
            else if (Trickster.lightsOutTimer < 0.5) lerpValue = Mathf.Clamp01(Trickster.lightsOutTimer * 2);

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1 - lerpValue) * ModOption.NormalOptions.CrewLightMod;
        }

        // If player is Lawyer, apply Lawyer vision modifier
        else if (Lawyer.lawyer != null && Lawyer.lawyer.PlayerId == player.PlayerId)
        {
            var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius * Lawyer.vision, unlerped);
            return false;
        }

        // Default light radius
        else
        {
            __result = GetNeutralLightRadius(__instance, false);
        }


        // Additional code
        var switchSystem = __instance.Systems[SystemTypes.Electrical]?.TryCast<SwitchSystem>();
        var t = switchSystem != null ? switchSystem.Value / 255f : 1;

        if (Sunglasses.sunglasses.Any(x => x.PlayerId == player.PlayerId)) // Sunglasses
        {
            __result *= 1f - (Sunglasses.vision * 0.1f);
        }

        if (Torch.torch.Any(x => x.PlayerId == player.PlayerId)) // Torch
        {
            __result = __instance.MaxLightRadius * ModOption.NormalOptions.CrewLightMod * Torch.vision;
        }

        if (Mayor.mayor.IsAlive() && Mayor.mayor.PlayerId == player.PlayerId && Mayor.Revealed) // Mayor Vision
        {
            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius * (1f - (Mayor.vision * 0.1f)), t) *
                ModOption.NormalOptions.CrewLightMod;
        }

        if (Specter.Player?.PlayerId == player.PlayerId)
        {
            __result = __instance.MaxLightRadius * ModOption.NormalOptions.CrewLightMod;
        }

        return false;
    }

    public static float GetNeutralLightRadius(ShipStatus shipStatus, bool isImpostor)
    {
        if (SubmergedCompatibility.IsSubmerged)
            return SubmergedCompatibility.GetSubmergedNeutralLightRadius(isImpostor);

        if (isImpostor)
            return shipStatus.MaxLightRadius * ModOption.NormalOptions.ImpostorLightMod;
        var lerpValue = 1.0f;
        try
        {
            var switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
            lerpValue = switchSystem.Value / 255f;
        }
        catch (Exception e)
        {
            Message($"Error getting SwitchSystem value: {e.Message}");
        }
        return Mathf.Lerp(shipStatus.MinLightRadius, shipStatus.MaxLightRadius, lerpValue) * ModOption.NormalOptions.CrewLightMod;
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath)), HarmonyPostfix]
    public static void IsGameOverDueToDeathPostfix(ShipStatus __instance, ref bool __result)
    {
        __result = false;
    }
}