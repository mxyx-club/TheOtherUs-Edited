using System.Linq;

namespace TheOtherRoles.CustomCosmetics.Patches;

[HarmonyPatch(typeof(CosmeticsCache))]
internal static class CosmeticsCachePatches
{
    [HarmonyPatch(nameof(CosmeticsCache.GetHat)), HarmonyPrefix]
    private static bool GetHatPrefix(string id, ref HatViewData __result)
    {
        Info($"trying to load hat {id} from cosmetics cache");
        return !CosmeticsManager.Instance.TryGetHatView(id, out __result);
    }
    
    [HarmonyPatch(nameof(CosmeticsCache.GetVisor)), HarmonyPrefix]
    private static bool GetVisorPrefix(string id, ref VisorViewData __result)
    {
        Info($"trying to load hat {id} from cosmetics cache");
        return !CosmeticsManager.Instance.TryGetVisorView(id, out __result);
    }
    
    [HarmonyPatch(nameof(CosmeticsCache.GetNameplate)), HarmonyPrefix]
    private static bool GetVisorPrefix(string id, ref NamePlateViewData __result)
    {
        Info($"trying to load hat {id} from cosmetics cache");
        return !CosmeticsManager.Instance.TryGetNamePlateView(id, out __result);
    }
    
    [HarmonyPatch(typeof(CosmeticsCache._CoAddHat_d__12), nameof(CosmeticsCache._CoAddHat_d__12.MoveNext)), HarmonyPrefix]
    private static bool _CoAddHat_d__12Prefix(CosmeticsCache._CoAddHat_d__12 __instance, ref bool __result)
    {
        var id = __instance.id;
        if (CosmeticsManager.Instance.CustomHats.All(n => n.Id != id))
            return true;
        __result = true;
        return false;
    }
    
    
    [HarmonyPatch(typeof(CosmeticsCache._CoAddVisor_d__10), nameof(CosmeticsCache._CoAddVisor_d__10.MoveNext)), HarmonyPrefix]
    private static bool __CoAddVisor_d__10Prefix(CosmeticsCache._CoAddVisor_d__10 __instance, ref bool __result)
    {
        var id = __instance.visorId;
        if (CosmeticsManager.Instance.CustomVisors.All(n => n.Id != id))
            return true;
        __result = true;
        return false;
    }
    
    [HarmonyPatch(typeof(CosmeticsCache._CoAddNameplate_d__8), nameof(CosmeticsCache._CoAddNameplate_d__8.MoveNext)), HarmonyPrefix]
    private static bool _CoAddNameplate_d__8Prefix(CosmeticsCache._CoAddNameplate_d__8 __instance, ref bool __result)
    {
        var id = __instance.namePlateId;
        if (CosmeticsManager.Instance.CustomNamePlates.All(n => n.Id != id))
            return true;
        __result = true;
        return false;
    }
}