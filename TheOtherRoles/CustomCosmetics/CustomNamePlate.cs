using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheOtherRoles.CustomCosmetics;

public class CustomNamePlate : ICustomCosmetic
{
    public CustomCosmeticConfig config { get; set; }
    public Sprite Resource { get; set; }
    public NamePlateData Data { get; set; }
    public CosmeticsManagerConfig ManagerConfig { get; set; }
    public string[] Resources => [config.Resource];
    
    public CustomCosmeticsFlags Flags { get; set; } = CustomCosmeticsFlags.NamePlate;

    public string Id { get; set; }
    public void Create(List<Sprite> sprites)
    {
        Resource = sprites.FirstOrDefault(n => n.name.Contains(config.Resource));
        Data = config.createNamePlateData(Resource, out var id, out var view);
        Id = id;
        View = view;
    }

    public NamePlateViewData View
    {
        get;
        set;
    }
    
    public static implicit operator NamePlateData(CustomNamePlate namePlate) => namePlate.Data;
    public static implicit operator NamePlateViewData(CustomNamePlate namePlate) => namePlate.View;
    public static implicit operator CosmeticsManagerConfig(CustomNamePlate namePlate) => namePlate.ManagerConfig;
    public static implicit operator CustomCosmeticConfig(CustomNamePlate namePlate) => namePlate.config;
}