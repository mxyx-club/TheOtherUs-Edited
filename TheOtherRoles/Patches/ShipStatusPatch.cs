using AmongUs.GameOptions;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Patches;


[HarmonyPatch(typeof(ShipStatus))]
public class ShipStatusPatch
{
    private static int originalNumCommonTasksOption;
    private static int originalNumShortTasksOption;
    private static int originalNumLongTasksOption;
    public static float originalNumCrewVisionOption;
    public static float originalNumImpVisionOption;
    public static float originalNumKillCooldownOption;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static bool Prefix(ref float __result, ShipStatus __instance,
        [HarmonyArgument(0)] NetworkedPlayerInfo player)
    {
        if (!__instance.Systems.ContainsKey(SystemTypes.Electrical) ||
            GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return true;

        // If player is a role which has Impostor vision
        if (hasImpVision(player))
        {
            //__result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
            __result = GetNeutralLightRadius(__instance, true);
            return false;
        }

        // If Game mode is Hide N Seek and hunter with ability active

        // If there is a Trickster with their ability active
        else if (Trickster.trickster != null && Trickster.lightsOutTimer > 0f)
        {
            var lerpValue = 1f;
            if (Trickster.lightsOutDuration - Trickster.lightsOutTimer < 0.5f)
                lerpValue = Mathf.Clamp01((Trickster.lightsOutDuration - Trickster.lightsOutTimer) * 2);
            else if (Trickster.lightsOutTimer < 0.5) lerpValue = Mathf.Clamp01(Trickster.lightsOutTimer * 2);

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1 - lerpValue) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
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
        /*
        if (Torch.torch.FindAll(x => x.PlayerId == player.PlayerId).Count > 0) t = 1;
        __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod * (1f + (Torch.vision * 0.1f));
        //__result *= 1f - Torch.vision / 0.1f;
        */

        if (Sunglasses.sunglasses.FindAll(x => x.PlayerId == player.PlayerId).Count > 0) // Sunglasses
        {
            __result *= 1f - (Sunglasses.vision * 0.1f);
        }

        if (Torch.torch.FindAll(x => x.PlayerId == player.PlayerId).Count > 0) // Torch
        {
            __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod * Torch.vision;
        }

        if (Mayor.mayor != null && Mayor.mayor.PlayerId == player.PlayerId && Mayor.Revealed) // Mayor Vision
        {
            //__result *= 1f - (Mayor.vision * 0.1f);
            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius * (1f - (Mayor.vision * 0.1f)), t) *
                GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
        }

        return false;
    }

    public static float GetNeutralLightRadius(ShipStatus shipStatus, bool isImpostor)
    {
        if (SubmergedCompatibility.IsSubmerged)
            return SubmergedCompatibility.GetSubmergedNeutralLightRadius(isImpostor);

        if (isImpostor)
            return shipStatus.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
        var lerpValue = 1.0f;
        try
        {
            var switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
            lerpValue = switchSystem.Value / 255f;
        }
        catch
        {
        }

        return Mathf.Lerp(shipStatus.MinLightRadius, shipStatus.MaxLightRadius, lerpValue) *
               GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static void Postfix2(ShipStatus __instance, ref bool __result)
    {
        __result = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static bool Prefix(ShipStatus __instance)
    {
        originalNumCommonTasksOption = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks;
        originalNumShortTasksOption = GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;
        originalNumLongTasksOption = GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static void Postfix3(ShipStatus __instance)
    {
        // Restore original settings after the tasks have been selected
        GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks = originalNumCommonTasksOption;
        GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks = originalNumShortTasksOption;
        GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks = originalNumLongTasksOption;
    }

    public static void resetVanillaSettings()
    {
        GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod = originalNumImpVisionOption;
        GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod = originalNumCrewVisionOption;
        GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown = originalNumKillCooldownOption;
    }
}