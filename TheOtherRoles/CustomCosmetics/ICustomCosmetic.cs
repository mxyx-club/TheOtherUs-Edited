using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

namespace TheOtherRoles.CustomCosmetics;

public interface ICustomCosmetic
{
    public CustomCosmeticConfig config { get; set; }
    public Sprite Resource { get; set; }
    public CosmeticsManagerConfig ManagerConfig { get; set; }

    public string[] Resources { get; }

    public CustomCosmeticsFlags Flags
    {
        get;
        set;
    }
    
    public string Id { get; set; }

    public void Create(List<Sprite> sprites);
};

public class CustomCosmeticConfig
{
    [JsonPropertyName("author")] public string Author { get; set; } = "Unknown";
    [JsonPropertyName("condition")] public string Condition { get; set; } = "None";
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("package")] public string Package { get; set; } = "None";
    [JsonPropertyName("resource")] public string Resource { get; set; }
    [JsonPropertyName("adaptive")] public bool Adaptive { get; set; } = false;
};