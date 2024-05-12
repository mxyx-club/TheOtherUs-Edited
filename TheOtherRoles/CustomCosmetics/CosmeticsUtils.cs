using TheOtherRoles.CustomCosmetics.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TheOtherRoles.CustomCosmetics;

public static class CosmeticsUtils
{
    public static HatData createHatData
        (
            this CustomHatConfig config, 
            Sprite sprite,
            Sprite backSprite,
            Sprite ClimbSprite,
            out string id,
            out HatViewData view
        )
    {
        var viewData = ScriptableObject.CreateInstance<HatViewData>();
        var hat = ScriptableObject.CreateInstance<HatData>();

        viewData.MainImage = sprite;
        viewData.FloorImage = viewData.MainImage;
        viewData.MatchPlayerColor = config.Adaptive;
        if (backSprite)
        {
            viewData.BackImage = backSprite;
            config.Behind = true;
        }

        if (ClimbSprite)
        {
            viewData.ClimbImage = ClimbSprite;
            viewData.LeftClimbImage = viewData.ClimbImage;
        }

        hat.name = config.Name;
        hat.displayOrder = 99;
        id = hat.ProductId = hat.BundleId = "TOUs_Hat_" + config.Name.Replace(' ', '_');
        hat.InFront = !config.Behind;
        hat.NoBounce = !config.Bounce;
        hat.ChipOffset = new Vector2(0f, 0.2f);
        hat.Free = true;
        
        view = viewData;
        hat.ViewDataRef = new AssetReference(viewData.Pointer);
        hat.CreateAddressableAsset();
        return hat;
    }
    
    public static VisorData createVisorData
    (
        this CustomCosmeticConfig config, 
        Sprite sprite,
        out string id,
        out VisorViewData view
    )
    {
        var viewData = ScriptableObject.CreateInstance<VisorViewData>();
        var hat = ScriptableObject.CreateInstance<VisorData>();

        viewData.IdleFrame = sprite;
        viewData.MatchPlayerColor = config.Adaptive;
        
        hat.name = config.Name;
        hat.displayOrder = 99;
        id = hat.ProductId = hat.BundleId = "TOUs_Visor_" + config.Name.Replace(' ', '_');
        hat.ChipOffset = new Vector2(0f, 0.2f);
        hat.Free = true;

        hat.ViewDataRef = new AssetReference(viewData.Pointer);
        hat.CreateAddressableAsset();
        view = viewData;
        return hat;
    }
    
    public static NamePlateData createNamePlateData
    (
        this CustomCosmeticConfig config, 
        Sprite sprite,
        out string Id,
        out NamePlateViewData view
    )
    {
        var viewData = ScriptableObject.CreateInstance<NamePlateViewData>();
        var hat = ScriptableObject.CreateInstance<NamePlateData>();

        viewData.Image = sprite;

        hat.name = config.Name;
        hat.displayOrder = 99;
        Id = hat.ProductId = hat.BundleId = "TOUs_NamePlate_" + config.Name.Replace(' ', '_');
        hat.ChipOffset = new Vector2(0f, 0.2f);
        hat.Free = true;

        view = viewData;
        hat.ViewDataRef = new AssetReference(viewData.Pointer);
        hat.CreateAddressableAsset();
        return hat;
    }
}