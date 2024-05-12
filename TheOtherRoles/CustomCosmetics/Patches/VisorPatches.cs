using System;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.CustomCosmetics.Patches;

// Form LasMonjas
[Harmony]
public static class VisorPatches
{
    [HarmonyPatch(typeof(VisorsTab), nameof(VisorsTab.OnEnable)), HarmonyPostfix]
    private static void TabOnEnablePostfix(VisorsTab __instance) 
    {
        foreach (var chip in __instance.scroller.Inner.GetComponentsInChildren<ColorChip>())
        {
            if (!CosmeticsManager.Instance.TryGetVisorView(chip.ProductId ,out var visor)) continue;
            chip.Inner.FrontLayer.sprite = visor.IdleFrame;
        }
    }
    
    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.UpdateMaterial)), HarmonyPrefix]
    private static bool VisorLayerUpdateMaterialPatch(VisorLayer __instance) {
        if (!CosmeticsManager.Instance.TryGetVisorView(__instance.visorData.ProductId ,out var visor)) return true;
        var maskType = __instance.matProperties.MaskType;
        __instance.Image.sharedMaterial = visor.MatchPlayerColor switch
        {
            true when maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI =>
                DestroyableSingleton<HatManager>.Instance.MaskedPlayerMaterial,
            true => DestroyableSingleton<HatManager>.Instance.PlayerMaterial,
            _ => maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI
                ? DestroyableSingleton<HatManager>.Instance.MaskedMaterial
                : FastDestroyableSingleton<HatManager>.Instance.DefaultShader
        };

        __instance.Image.maskInteraction = maskType switch
        {
            PlayerMaterial.MaskType.SimpleUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None
        };
        
        __instance.Image.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
        
        if (visor.MatchPlayerColor)
            PlayerMaterial.SetColors(__instance.matProperties.ColorId, __instance.Image);
        
        switch (__instance.matProperties.MaskLayer)
        {
            case > 0:
                __instance.Image.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
                return false;
            default:
                PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.Image, __instance.matProperties.IsLocalPlayer);
                return false;
        }
    }

    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetFlipX)), HarmonyPrefix]
    private static bool VisorLayerSetFlipXPatchPrefix(VisorLayer __instance, bool flipX) {
        if (!CosmeticsManager.Instance.TryGetVisorView(__instance.visorData.ProductId ,out var asset)) return true;
        __instance.Image.flipX = flipX;
        __instance.Image.sprite = flipX && asset.LeftIdleFrame ? asset.LeftIdleFrame : asset.IdleFrame;
        return false;
    }

    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetFloorAnim)), HarmonyPrefix]
    private static bool VisorLayerSetVisorFloorPositionPatchPrefix(VisorLayer __instance) {
        if (!CosmeticsManager.Instance.TryGetVisorView(__instance.visorData.ProductId ,out var asset)) return true;
        __instance.Image.sprite = asset.FloorFrame ? asset.FloorFrame : asset.IdleFrame;
        return false;
    }
    
    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.PopulateFromViewData)), HarmonyPrefix]
    private static bool VisorLayerPopulateFromViewDataPatchPrefix(VisorLayer __instance) 
    {
        if (!CosmeticsManager.Instance.TryGetVisorView(__instance.visorData.ProductId ,out var asset)) return true;
        __instance.UpdateMaterial();
        if (__instance.IsDestroyedOrNull()) return false;
        __instance.transform.SetLocalZ(__instance.DesiredLocalZPosition);
        __instance.SetFlipX(__instance.Image.flipX);
        return false;
    }

    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetVisor), [typeof(VisorData), typeof(int)]), HarmonyPrefix]
    private static bool VisorLayerSetVisorPatchPrefix(VisorLayer __instance, VisorData data, int color) 
    {
        if (!CosmeticsManager.Instance.TryGetVisorView(__instance.visorData.ProductId ,out var asset)) return true;
        __instance.visorData = data;
        __instance.SetMaterialColor(color);
        __instance.PopulateFromViewData();
        return false;
    }
}