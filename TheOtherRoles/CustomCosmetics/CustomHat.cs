using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.CustomCosmetics.Configs;
using UnityEngine;

namespace TheOtherRoles.CustomCosmetics;

public class CustomHat : ICustomCosmetic
{
    public CustomCosmeticConfig config { get; set; }

    public CustomHatConfig HatConfig => (CustomHatConfig)config;
    public Sprite Resource { get; set; }
    #nullable enable
    public Sprite? BackSprite { get; set; }
    public Sprite? ClimbSprite { get; set; }
    public Sprite? FlipSprite { get; set; }
    public Sprite? BackFlipSprite { get; set; }
    #nullable disable
    public HatData Data { get; set; }
    public CosmeticsManagerConfig ManagerConfig { get; set; }

    public string[] Resources => new List<string>{
        HatConfig.Resource, HatConfig.BackResource, HatConfig.ClimbResource,
         HatConfig.BackFlipResource,
        HatConfig.FlipResource
    }.Where(n => !n.IsNullOrWhiteSpace()).ToArray();

    public CustomCosmeticsFlags Flags { get; set; } = CustomCosmeticsFlags.Hat;

    public string Id { get; set; }
    public void Create(List<Sprite> sprites)
    {
        Resource = sprites.FirstOrDefault(n => n.name.Contains(HatConfig.Resource));
        BackSprite = sprites.FirstOrDefault(n => n.name.Contains(HatConfig.BackResource));
        ClimbSprite = sprites.FirstOrDefault(n => n.name.Contains(HatConfig.ClimbResource));
        BackFlipSprite = sprites.FirstOrDefault(n => n.name.Contains(HatConfig.BackFlipResource));
        FlipSprite = sprites.FirstOrDefault(n => n.name.Contains(HatConfig.FlipResource));
        Data = HatConfig.createHatData(Resource, BackSprite, ClimbSprite, out var id, out var view);
        View = view;
        Id = id;
    }

    public HatViewData View
    {
        get;
        set;
    }

    public static implicit operator HatData(CustomHat hat) => hat.Data;
    public static implicit operator HatViewData(CustomHat hat) => hat.View;
    public static implicit operator CosmeticsManagerConfig(CustomHat hat) => hat.ManagerConfig;
    public static implicit operator CustomCosmeticConfig(CustomHat hat) => hat.config;
}