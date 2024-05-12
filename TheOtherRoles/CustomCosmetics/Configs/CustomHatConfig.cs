using System.Text.Json.Serialization;

namespace TheOtherRoles.CustomCosmetics.Configs;

public class CustomHatConfig : CustomCosmeticConfig
{

    [JsonPropertyName("bounce")] public bool Bounce { get; set; } = false;

    [JsonPropertyName("climbresource")] public string ClimbResource { get; set; } = string.Empty;

    [JsonPropertyName("behind")] public bool Behind { get; set; }

    [JsonPropertyName("backresource")] public string BackResource { get; set; } = string.Empty;

    [JsonPropertyName("backflipresource")] public string BackFlipResource { get; set; } = string.Empty;

    [JsonPropertyName("flipresource")] public string FlipResource { get; set; } = string.Empty;

    [JsonPropertyName("reshasha")] public string ResHashA { get; set; } = string.Empty;

    [JsonPropertyName("reshashb")] public string ResHashB { get; set; } = string.Empty;

    [JsonPropertyName("reshashbf")] public string ResHashBf { get; set; } = string.Empty;

    [JsonPropertyName("reshashc")] public string ResHashC { get; set; } = string.Empty;

    [JsonPropertyName("reshashf")] public string ResHashF { get; set; } = string.Empty;
}