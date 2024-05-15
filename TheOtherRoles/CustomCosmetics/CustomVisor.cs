using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheOtherRoles.CustomCosmetics;

public class CustomVisor : ICustomCosmetic
{
    public CustomCosmeticConfig config { get; set; }
    public Sprite Resource { get; set; }
    public VisorData Data { get; set; }
    public CosmeticsManagerConfig ManagerConfig { get; set; }
    public string[] Resources => [config.Resource];
    public CustomCosmeticsFlags Flags { get; set; } = CustomCosmeticsFlags.Visor;
    public string Id { get; set; }
    public void Create(List<Sprite> sprites)
    {
        Resource = sprites.FirstOrDefault(n => n.name.Contains(config.Resource));
        Data = config.createVisorData(Resource, out var id, out var view);
        Id = id;
        View = view;
    }

    public VisorViewData View
    {
        get;
        set;
    }
    
    public static implicit operator VisorData(CustomVisor visor) => visor.Data;
    public static implicit operator VisorViewData(CustomVisor visor) => visor.View;
    public static implicit operator CosmeticsManagerConfig(CustomVisor visor) => visor.ManagerConfig;
    public static implicit operator CustomCosmeticConfig(CustomVisor visor) => visor.config;
}