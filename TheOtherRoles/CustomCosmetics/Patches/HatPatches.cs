using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using PowerTools;
using Rewired.Utils;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.CustomCosmetics.Patches;

[Harmony]
public static class HatPatches
{
    private static TextMeshPro textTemplate;

    /*[HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
    [HarmonyPostfix]
    private static void OnEnablePostfix(HatsTab __instance)
    {
        for (var i = 0; i < __instance.scroller.Inner.childCount; i++)
            Object.Destroy(__instance.scroller.Inner.GetChild(i).gameObject);

        __instance.ColorChips = new Il2CppSystem.Collections.Generic.List<ColorChip>();
        var unlockedHats = DestroyableSingleton<HatManager>.Instance.GetUnlockedHats();
        var packages = new Dictionary<string, List<Tuple<HatData, CustomHat>>>();

        foreach (var hatBehaviour in unlockedHats)
        {
            if (CosmeticsManager.Instance.TryGetHat(hatBehaviour, out var hat))
            {
                if (!packages.ContainsKey(hat.config.Package)) 
                    packages[hat.config.Package] = [];
                packages[hat.config.Package].Add(new Tuple<HatData, CustomHat>(hatBehaviour, hat));
            }
            else
            {
                if (!packages.ContainsKey(CosmeticsManager.InnerslothPackageName))
                    packages[CosmeticsManager.InnerslothPackageName] = [];
                packages[CosmeticsManager.InnerslothPackageName]
                    .Add(new Tuple<HatData, CustomHat>(hatBehaviour, null));
            }
        }

        var yOffset = __instance.YStart;
        textTemplate = GameObject.Find("HatsGroup").transform.FindChild("Text").GetComponent<TextMeshPro>();

        var orderedKeys = packages.Keys.OrderBy(x =>
            x switch
            {
                CosmeticsManager.InnerslothPackageName => 1000,
                _ => 0
            });
        
        foreach (var key in orderedKeys)
        {
            var value = packages[key];
            yOffset = CreateHatPackage(value, key, yOffset, __instance);
        }

        __instance.scroller.ContentYBounds.max = -(yOffset + 4.1f);
    }
    

    private static float CreateHatPackage(List<Tuple<HatData, CustomHat>> hats, string packageName, float yStart,
        HatsTab hatsTab)
    {
        var isDefaultPackage = CosmeticsManager.InnerslothPackageName == packageName;
        /*if (!isDefaultPackage) 
            hats = hats.OrderBy(x => x.Item1.name).ToList();#1#

        var offset = yStart;
        if (textTemplate != null)
        {
            var title = Object.Instantiate(textTemplate, hatsTab.scroller.Inner);
            title.transform.localPosition = new Vector3(2.25f, yStart, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            hatsTab.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(p => { title.SetText(packageName); })));
            offset -= 0.8f * hatsTab.YOffset;
        }

        var i = 0;
        foreach ((HatData hat, CustomHat ext) in hats)
        {
            var xPos = hatsTab.XRange.Lerp(i % hatsTab.NumPerRow / (hatsTab.NumPerRow - 1f));
            var yPos = offset - (i / (float)hatsTab.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * hatsTab.YOffset);
            var colorChip = Object.Instantiate(hatsTab.ColorTabPrefab, hatsTab.scroller.Inner);
            if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OnMouseOver.AddListener((Action)(() => hatsTab.SelectHat(hat)));
                colorChip.Button.OnMouseOut.AddListener((Action)(() =>
                    hatsTab.SelectHat(
                        DestroyableSingleton<HatManager>.Instance.GetHatById(DataManager.Player.Customization.Hat))));
                colorChip.Button.OnClick.AddListener((Action)hatsTab.ClickEquip);
            }
            else
            {
                colorChip.Button.OnClick.AddListener((Action)(() => hatsTab.SelectHat(hat)));
            }

            colorChip.Button.ClickMask = hatsTab.scroller.Hitbox;
            colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.ScrollingUI);
            hatsTab.UpdateMaterials(colorChip.Inner.FrontLayer, hat);
            var background = colorChip.transform.FindChild("Background");
            var foreground = colorChip.transform.FindChild("ForeGround");

            if (ext != null)
            {
                if (background != null)
                {
                    background.localPosition = Vector3.down * 0.243f;
                    var localScale = background.localScale;
                    localScale = new Vector3(localScale.x, 0.8f, localScale.y);
                    background.localScale = localScale;
                }

                if (foreground != null) 
                    foreground.localPosition = Vector3.down * 0.243f;

                if (textTemplate != null)
                {
                    var description = Object.Instantiate(textTemplate, colorChip.transform);
                    description.transform.localPosition = new Vector3(0f, -0.65f, -1f);
                    description.alignment = TextAlignmentOptions.Center;
                    description.transform.localScale = Vector3.one * 0.65f;
                    hatsTab.StartCoroutine(Effects.Lerp(0.1f,
                        new Action<float>(p => { description.SetText($"{ext.config.Name}\nby {ext.config.Author}"); })));
                }
            }

            hat.SetPreview(colorChip.Inner.FrontLayer, hatsTab.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : ((int)DataManager.Player.Customization.Color));

            colorChip.transform.localPosition = new Vector3(xPos, yPos, -1f);
            colorChip.Inner.transform.localPosition = hat.ChipOffset;
            colorChip.Tag = hat;
            colorChip.SelectionHighlight.gameObject.SetActive(false);
            hatsTab.ColorChips.Add(colorChip);
            i++;
        }
        return offset - ((hats.Count - 1) / (float)hatsTab.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * hatsTab.YOffset) -
               1.75f;
    }*/
    
    
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    [HarmonyPostfix]
    private static void HandleAnimationPostfix(PlayerPhysics __instance)
    {
        if (__instance.IsDestroyedOrNull()) return;
        var currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();
        var hatParent = __instance.myPlayer.cosmetics.hat;
        if (currentAnimation == __instance.Animations.group.ClimbUpAnim || currentAnimation == __instance.Animations.group.ClimbDownAnim || !CosmeticsManager.Instance.TryGetHat(hatParent.Hat.ProductId, out var hat)) return;
        
        if (hat.FlipSprite)
            hatParent.FrontLayer.sprite = __instance.FlipX ? hat.FlipSprite : hat.View.MainImage;

        if (hat.BackFlipSprite) 
            hatParent.BackLayer.sprite = __instance.FlipX ? hat.BackFlipSprite : hat.View.BackImage;
    }
    
    [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetHat), typeof(int))]
    [HarmonyPrefix]
    private static bool SetHatPrefix(HatParent __instance, int color)
    {
        if (__instance.IsDestroyedOrNull() || __instance.Hat.IsNullOrDestroyed()) return true;
        if (!CosmeticsManager.Instance.TryGetHatView(__instance.Hat.ProductId, out var viewData)) return true;
        __instance.viewAsset = null;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(color);
        return false;
    }

    [HarmonyPatch(typeof(HatParent), nameof(HatParent.UpdateMaterial))]
    [HarmonyPrefix]
    private static bool UpdateMaterialPrefix(HatParent __instance)
    {
        if (__instance.IsDestroyedOrNull() || __instance.Hat.IsNullOrDestroyed()) return false;
        if (!CosmeticsManager.Instance.TryGetHat(__instance.Hat.ProductId, out var hat)) return true;
        var asset = hat.View;
        if (asset.MatchPlayerColor)
        {
            __instance.FrontLayer.sharedMaterial = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;
            if (__instance.BackLayer)
                __instance.BackLayer.sharedMaterial = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;
        }
        else
        {
            __instance.FrontLayer.sharedMaterial = DestroyableSingleton<HatManager>.Instance.DefaultShader;
            if (__instance.BackLayer)
                __instance.BackLayer.sharedMaterial = DestroyableSingleton<HatManager>.Instance.DefaultShader;
        }

        var colorId = __instance.matProperties.ColorId;
        PlayerMaterial.SetColors(colorId, __instance.FrontLayer);
        if (__instance.BackLayer) 
            PlayerMaterial.SetColors(colorId, __instance.BackLayer);

        __instance.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
        if (__instance.BackLayer)
            __instance.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);

        var maskType = __instance.matProperties.MaskType;
        switch (maskType)
        {
            case PlayerMaterial.MaskType.ScrollingUI:
                if (__instance.FrontLayer)
                    __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                if (__instance.BackLayer)
                    __instance.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                break;
            case PlayerMaterial.MaskType.Exile:
                if (__instance.FrontLayer)
                    __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

                if (__instance.BackLayer)
                    __instance.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

                break;
            
            default:
                if (__instance.FrontLayer) __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.None;

                if (__instance.BackLayer) __instance.BackLayer.maskInteraction = SpriteMaskInteraction.None;

                break;
        }

        if (__instance.matProperties.MaskLayer > 0) return false;
        PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.FrontLayer, __instance.matProperties.IsLocalPlayer);
        if (!__instance.BackLayer) return false;
        PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.BackLayer, __instance.matProperties.IsLocalPlayer);

        return false;
    }

    [HarmonyPatch(typeof(HatParent), nameof(HatParent.LateUpdate))]
    [HarmonyPrefix]
    private static bool LateUpdatePrefix(HatParent __instance)
    {
        if (!__instance.Hat) return false;
        if (!CosmeticsManager.Instance.TryGetHat(__instance.Hat.ProductId, out var hat)) return true;
        var hatViewData = hat.View;
        if (__instance.FrontLayer.sprite == hatViewData.ClimbImage ||
            __instance.FrontLayer.sprite == hatViewData.FloorImage)
        {
            if (__instance.FrontLayer.sprite != hatViewData.ClimbImage &&
                __instance.FrontLayer.sprite != hatViewData.LeftClimbImage) return false;
            var spriteAnimNodeSync = __instance.SpriteSyncNode != null
                ? __instance.SpriteSyncNode
                : __instance.GetComponent<SpriteAnimNodeSync>();
            if (spriteAnimNodeSync) spriteAnimNodeSync.NodeId = 0;
        }
        else
        {
            if ((__instance.Hat.InFront || hatViewData.BackImage) && hatViewData.LeftMainImage)
                __instance.FrontLayer.sprite =
                    __instance.Parent.flipX ? hatViewData.LeftMainImage : hatViewData.MainImage;

            if (hatViewData.BackImage && hatViewData.LeftBackImage)
            {
                __instance.BackLayer.sprite =
                    __instance.Parent.flipX ? hatViewData.LeftBackImage : hatViewData.BackImage;
                return false;
            }

            if (hatViewData.BackImage || __instance.Hat.InFront || !hatViewData.LeftMainImage) return false;
            __instance.BackLayer.sprite =
                __instance.Parent.flipX ? hatViewData.LeftMainImage : hatViewData.MainImage;
            return false;
        }

        return false;
    }

    [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetFloorAnim))]
    [HarmonyPrefix]
    private static bool SetFloorAnimPrefix(HatParent __instance)
    {
        if (!__instance.Hat) return false;
        if (!CosmeticsManager.Instance.TryGetHat(__instance.Hat.ProductId, out var hat)) return true;
        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = hat.View.FloorImage;
        return false;
    }

    [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetIdleAnim))]
    [HarmonyPrefix]
    private static bool SetIdleAnimPrefix(HatParent __instance, int colorId)
    {
        if (!__instance.Hat) return false;
        if (!CosmeticsManager.Instance.TryGetHat(__instance.Hat.ProductId, out var hat)) return true;
        __instance.viewAsset = null;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(colorId);
        return false;
    }

    [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetClimbAnim))]
    [HarmonyPrefix]
    private static bool SetClimbAnimPrefix(HatParent __instance)
    {
        if (!__instance.Hat) return false;
        if (!CosmeticsManager.Instance.TryGetHat(__instance.Hat.ProductId, out var hat)) return true;
        if (!__instance.options.ShowForClimb) return false;
        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = hat.View.ClimbImage;
        return false;
    }

    [HarmonyPatch(typeof(HatParent), nameof(HatParent.PopulateFromViewData))]
    [HarmonyPrefix]
    private static bool PopulateFromHatViewDataPrefix(HatParent __instance)
    {
        if (!CosmeticsManager.Instance.TryGetHat(__instance.Hat.ProductId, out var hat)) return true;
        __instance.UpdateMaterial();
        var asset = hat.View;

        var spriteAnimNodeSync = __instance.SpriteSyncNode
            ? __instance.SpriteSyncNode
            : __instance.GetComponent<SpriteAnimNodeSync>();
        if (spriteAnimNodeSync) spriteAnimNodeSync.NodeId = __instance.Hat.NoBounce ? 1 : 0;

        if (__instance.Hat.InFront)
        {
            __instance.BackLayer.enabled = false;
            __instance.FrontLayer.enabled = true;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else if (asset.BackImage)
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = true;
            __instance.BackLayer.sprite = asset.BackImage;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = false;
            __instance.FrontLayer.sprite = null;
            __instance.BackLayer.sprite = asset.MainImage;
        }

        if (!__instance.options.Initialized || !__instance.HideHat()) return false;
        __instance.FrontLayer.enabled = false;
        __instance.BackLayer.enabled = false;
        return false;
    }
}