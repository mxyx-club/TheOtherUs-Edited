namespace TheOtherRoles.CustomCosmetics.Patches;

[Harmony]
public static class NamePlatesPatches
{
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.PreviewNameplate)), HarmonyPostfix]
    private static void VisorLayerUpdateMaterialPatchPostfix(PlayerVoteArea __instance, string plateID)
    {
        if (!CosmeticsManager.Instance.TryGetNamePlateView(plateID, out var nameplate)) return;
        __instance.Background.sprite = nameplate.Image;
    }

    // form LasMonjas
    [HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable)), HarmonyPostfix]
    private static void NameplatesTabOnEnablePatchPostfix(NameplatesTab __instance)
    {
        foreach (var chip in __instance.scroller.Inner.GetComponentsInChildren<NameplateChip>())
        {
            if (!CosmeticsManager.Instance.TryGetNamePlateView(chip.ProductId, out var nameplate)) continue;
            chip.image.sprite = nameplate.Image;
        }
    }
}