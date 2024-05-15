using AmongUs.Data;
using Epic.OnlineServices.Presence;
using Il2CppSystem;
using Innersloth.Assets;
using MonoMod.Utils;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace TheOtherRoles.CustomCosmetics.Patches;

[Harmony]
public static class NamePlatesPatches
{
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.PreviewNameplate)), HarmonyPrefix]
    private static bool  VisorLayerUpdateMaterialPatchPostfix(PlayerVoteArea __instance, string plateID)
    {
        if (!CosmeticsManager.Instance.TryGetNamePlate(plateID, out var nameplate)) return true;
        __instance.gameObject.SetActive(true);
        __instance.Background.sprite = nameplate.Resource;
        __instance.PlayerIcon.gameObject.SetActive(false);
        __instance.NameText.text = DataManager.Player.Customization.Name;
        __instance.LevelNumberText.text = ProgressionManager.Instance.CurrentVisualLevel;
        return false;
    }

    // form LasMonjas
    [HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable)), HarmonyPrefix]
    private static bool NameplatesTabOnEnablePatchPre(NameplatesTab __instance)
    {
        if (__instance.HasLocalPlayer())
            __instance.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
        else
            __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);
        
        __instance.PlayerPreview.gameObject.SetActive(false);
        __instance.previewArea.PreviewNameplate(DataManager.Player.Customization.NamePlate);
        var unlockedNamePlates = DestroyableSingleton<HatManager>.Instance.GetUnlockedNamePlates();
        for (int i = 0; i < unlockedNamePlates.Length; i++)
        {
            var plate = unlockedNamePlates[i];
            var num = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
            var num2 = __instance.YStart - (float)i / __instance.NumPerRow * __instance.YOffset;
            var chip = Object.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);
            chip.transform.localPosition = new Vector3(num, num2, -1f);
            chip.Button.ClickMask = __instance.scroller.Hitbox;
            if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
            {
                chip.Button.OnMouseOver.AddListener((UnityAction)(() =>
                {
                    __instance.SelectNameplate(plate);
                }));
                chip.Button.OnMouseOut.AddListener((UnityAction)(() =>
                {
                    __instance.SelectNameplate(DestroyableSingleton<HatManager>.Instance.GetNamePlateById(DataManager.Player.Customization.NamePlate));
                }));
                chip.Button.OnClick.AddListener((UnityAction)(() =>
                {
                    __instance.ClickEquip();
                }));
            }
            else
            {
                chip.Button.OnClick.AddListener((UnityAction)(() =>
                {
                    __instance.SelectNameplate(plate);
                }));
            }

            var nChip = chip.CastFast<NameplateChip>();
            if (CosmeticsManager.Instance.TryGetNamePlate(plate.ProductId, out var sp))
            {
                nChip.image.sprite = sp.Resource;
            }
            else
            {
                var asset = plate.CreateAddressableAsset();
                asset.LoadAsync((Action)(() => nChip.image.sprite = asset.GetAsset().Image)); 
            }
            nChip.ProductId = plate.ProdId;
            __instance.ColorChips.Add(nChip);
        }
        if (unlockedNamePlates.Length != 0)
        {
            __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);
        }
        __instance.plateId = DataManager.Player.Customization.NamePlate;
        __instance.currentNameplateIsEquipped = true;
        __instance.SetScrollerBounds();
        return false;
    }
}