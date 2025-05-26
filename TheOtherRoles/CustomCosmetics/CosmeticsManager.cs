using System.IO;
using System.Text.Json.Serialization;
using BepInEx;
using TheOtherRoles.CustomCosmetics.CustomHats;

namespace TheOtherRoles.CustomCosmetics;

public class CosmeticsManager : ManagerBase<CosmeticsManager>
{
    internal static string CosmeticDir = Path.Combine(Paths.GameRootPath, Main.Name);
    internal static string CustomHatsDir => Path.Combine(CosmeticDir, "CustomHats");
    internal static string CustomVisorsDir => Path.Combine(CosmeticDir, "CustomVisors");
    internal static string CustomPlatesDir => Path.Combine(CosmeticDir, "CustomPlates");
    internal static string CosmeticsConfigDir => Path.Combine(CosmeticDir, "CosmeticsConfig");

    public readonly HashSet<CosmeticsManagerConfig> configs = new();
    public readonly CosmeticsManagerConfig DefConfig = new()
    {
        ConfigName = "TheOtherHats",
        HatDirName = "hats",
        HatFileName = "CustomHats.json",
        RootUrl = "https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherHats/master".GithubUrl(),
        hasCosmetics = CustomCosmeticsFlags.Hat
    };

    internal static string RepositoryUrl => "https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherHats/master".GithubUrl();

    public static void Load()
    {
        Instance.AddConfig();
        CustomHatManager.LoadHats();
        CustomColors.Load();
    }

    public void AddConfig()
    {
        configs.Add(DefConfig);
    }
}

public class CosmeticsManagerConfig
{
    public string ConfigName = "None";
    public string RootUrl { get; set; }
    public CustomCosmeticsFlags hasCosmetics { get; set; }
    public string HatDirName { get; set; } = "hats";
    public string VisorDirName { get; set; } = "Visors";
    public string NamePlateDirName { get; set; } = "NamePlates";
    public string HatFileName { get; set; } = "CustomHats.json";
    public string VisorFileName { get; set; } = "CustomVisors.json";
    public string NamePlateFileName { get; set; } = "CustomNamePlates.json";
    public string HatPropertyName { get; set; } = "hats";
    public string VisorPropertyName { get; set; } = "Visors";
    public string NamePlatePropertyName { get; set; } = "nameplates";
}

[Flags, JsonConverter(typeof(JsonStringEnumConverter))]
public enum CustomCosmeticsFlags
{
    Hat = 1,
    Skin,
    Visor,
    NamePlate,
}
