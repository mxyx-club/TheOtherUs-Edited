using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BepInEx;
using TheOtherRoles.CustomCosmetics.Configs;
using TheOtherRoles.Modules;
using UnityEngine;
using static System.IO.StreamReader;

namespace TheOtherRoles.CustomCosmetics;

[Harmony]
public class CosmeticsManager : ManagerBase<CosmeticsManager>
{
    public readonly List<Sprite> sprites = [];
    public HashSet<Sprite> HatSprites => sprites.Where(n => n.name.StartsWith("Hats/")).ToHashSet();
    public HashSet<Sprite> VisorSprites => sprites.Where(n => n.name.StartsWith("Visors/")).ToHashSet();
    public HashSet<Sprite> NamePlateSprites => sprites.Where(n => n.name.StartsWith("NamePlates/")).ToHashSet();
    
    public static readonly string CosmeticDir = Path.Combine(Paths.GameRootPath, "Cosmetics");
    public static readonly string HatDir = Path.Combine(CosmeticDir, "Hats");
    public static readonly string VisorDir = Path.Combine(CosmeticDir, "Visors");
    public static readonly string NamePlateDir = Path.Combine(CosmeticDir, "NamePlates");
    public static readonly string ManagerConfigDir = Path.Combine(CosmeticDir, "ManagerConfig");
    public static readonly string LocalDir = Path.Combine(CosmeticDir, "Local");
    public static readonly string LocalHatDir = Path.Combine(LocalDir, "Hats");
    public static readonly string LocalVisorDir = Path.Combine(LocalDir, "Visors");
    public static readonly string LocalNamePlateDir = Path.Combine(LocalDir, "NamePlates");
    
    public const string InnerslothPackageName = "Innersloth";

    static CosmeticsManager()
    {
        string[] list = [CosmeticDir, HatDir, VisorDir, NamePlateDir, ManagerConfigDir, LocalDir, LocalHatDir, LocalVisorDir, LocalNamePlateDir];
        foreach (var path in list)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path!);
        }
    }
    
    public static readonly CosmeticsManagerConfig DefConfig = new()
    {
        RootUrl = "https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherHats/master",
        hasCosmetics = CustomCosmeticsFlags.Hat
    };
    
    public readonly HashSet<CosmeticsManagerConfig> configs = [];

    public readonly List<ICustomCosmetic> customCosmetics = [];
    public HashSet<CustomHat> CustomHats => customCosmetics.Where(n => n is CustomHat).Cast<CustomHat>().ToHashSet();
    public HashSet<CustomVisor> CustomVisors => customCosmetics.Where(n => n is CustomVisor).Cast<CustomVisor>().ToHashSet();
    public HashSet<CustomNamePlate> CustomNamePlates => customCosmetics.Where(n => n is CustomNamePlate).Cast<CustomNamePlate>().ToHashSet();

    public bool TryGetHatView(string Id, [MaybeNullWhen(false)]out HatViewData data)
    {
        var hat = CustomHats.FirstOrDefault(n => n.Id == Id);
        data = hat?.View;
        return hat == null;
    }
    
    public bool TryGetVisorView(string Id, [MaybeNullWhen(false)]out VisorViewData data)
    {
        var visor = CustomVisors.FirstOrDefault(n => n.Id == Id);
        data = visor?.View;
        return visor == null;
    }
    
    public bool TryGetNamePlateView(string Id, [MaybeNullWhen(false)]out NamePlateViewData data)
    {
        var namePlate = CustomNamePlates.FirstOrDefault(n => n.Id == Id);
        data = namePlate?.View;
        return namePlate == null;
    }
    
    public bool TryGetHat(HatData data, [MaybeNullWhen(false)]out CustomHat Hat)
    {
        var hat = CustomHats.FirstOrDefault(n => n.data == data);
        Hat = hat;
        return hat == null;
    }
    
    public bool TryGetHat(string Id, [MaybeNullWhen(false)]out CustomHat data)
    {
        var hat = CustomHats.FirstOrDefault(n => n.Id == Id);
        data = hat;
        return hat == null;
    }
    
    public bool TryGetVisor(string Id, [MaybeNullWhen(false)]out CustomVisor data)
    {
        var visor = CustomVisors.FirstOrDefault(n => n.Id == Id);
        data = visor;
        return visor == null;
    }
    
    public bool TryGetNamePlate(string Id, [MaybeNullWhen(false)]out CustomNamePlate data)
    {
        var namePlate = CustomNamePlates.FirstOrDefault(n => n.Id == Id);
        data = namePlate;
        return namePlate == null;
    }
    
    public void DefConfigCreateAndInit()
    {
        InitManager();
        Instance.Init(DefConfig);
    }

    public void InitManager()
    {
        LoadConfigFormDisk(new DirectoryInfo(ManagerConfigDir));
        LoadSpriteFormDisk();
        Task.Factory.StartNew(LoadFormDisk);
    }

    public void LoadFormDisk()
    {
        var hatPath = Path.Combine(LocalDir, "CustomHat.json");
        if (File.Exists(hatPath))
        {
            var json = JsonDocument.Parse(File.ReadAllText(hatPath));
            var hats = json.RootElement.GetProperty("hats").Deserialize<List<CustomHatConfig>>();
            foreach (var hatConfig in hats)
            {
                var res = new[]
                {
                    hatConfig.Resource, hatConfig.BackResource, hatConfig.BackFlipResource, hatConfig.ClimbResource,
                    hatConfig.FlipResource
                }.Where(n => n != string.Empty);
                foreach (var r in res)
                {
                    var p = Path.Combine(LocalHatDir, r);
                    var s = SpriteLoader.LoadHatSpriteFormDisk(File.OpenRead(p), $"Hats/Local/{r}");
                    sprites.Add(s);
                }
                
                var hat = new CustomHat
                {
                    ManagerConfig = null,
                    config = hatConfig,
                    Resource = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.Resource),
                    BackSprite = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.BackResource),
                    BackFlipSprite = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.BackFlipResource),
                    ClimbSprite = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.ClimbResource),
                    FlipSprite = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.FlipResource)
                };
                hat.data = hatConfig.createHatData(hat.Resource, hat.BackSprite, hat.ClimbSprite, out var id, out var view);
                hat.Id = id;
                hat.View = view;
                customCosmetics.Add(hat);
            }
        }

        var visorPath = Path.Combine(LocalDir, "CustomVisor.json");
        if (File.Exists(visorPath))
        {
            var json = JsonDocument.Parse(File.ReadAllText(visorPath));
            var visors = json.RootElement.GetProperty("Visors").Deserialize<List<CustomCosmeticConfig>>();
            foreach (var visorConfig in visors)
            {
                var p = Path.Combine(LocalHatDir, visorConfig.Resource);
                var s = SpriteLoader.LoadHatSpriteFormDisk(File.OpenRead(p), $"Visors/Local/{visorConfig.Resource}");
                sprites.Add(s);
                
                var visor = new CustomVisor
                {
                    ManagerConfig = null,
                    config = visorConfig,
                    Resource = GetSprite(CustomCosmeticsFlags.Visor, visorConfig.Resource),
                };
                visor.data = visorConfig.createVisorData(visor.Resource, out var id, out var view);
                visor.Id = id;
                visor.View = view;
                
                customCosmetics.Add(visor);
            }
        }
        
        var NamePlatePath = Path.Combine(LocalDir, "CustomNamePlate.json");
        if (File.Exists(NamePlatePath))
        {
            var json = JsonDocument.Parse(File.ReadAllText(NamePlatePath));
            var namePlates = json.RootElement.GetProperty("NamePlates").Deserialize<List<CustomCosmeticConfig>>();
            foreach (var namePlateConfig in namePlates)
            {
                var p = Path.Combine(LocalHatDir, namePlateConfig.Resource);
                var s = SpriteLoader.LoadHatSpriteFormDisk(File.OpenRead(p), $"NamePlates/Local/{namePlateConfig.Resource}");
                sprites.Add(s);
                
                var NamePlate = new CustomNamePlate
                {
                    ManagerConfig = null,
                    config = namePlateConfig,
                    Resource = GetSprite(CustomCosmeticsFlags.NamePlate, namePlateConfig.Resource)
                };
                NamePlate.data = namePlateConfig.createNamePlateData(NamePlate.Resource, out var id, out var view);
                NamePlate.Id = id;
                NamePlate.View = view;
                
                customCosmetics.Add(NamePlate);
            }
        }
        
        CheckAddAll();
    }

    private static bool AddEnd = false;
    
    [HarmonyPatch(typeof(HatManager))]
    [HarmonyPatch(nameof(HatManager.GetHatById))]
    [HarmonyPatch(nameof(HatManager.GetVisorById))]
    [HarmonyPatch(nameof(HatManager.GetNamePlateById))]
    [HarmonyPatch(nameof(HatManager.Instantiate))]
    [HarmonyPostfix]
    private static void OnHatManager_InstantiatePostfix(HatManager __instance)
    {
        if (AddEnd)
            return;
        var hatList = __instance.allHats.ToList();
        hatList.AddRange(Instance.CustomHats.Where(n => hatList.All(y => y.ProductId != n.Id)).Select(n => (HatData)n.data));
        __instance.allHats = hatList.ToArray();
        
        var VisorList = __instance.allVisors.ToList();
        VisorList.AddRange(Instance.CustomVisors.Where(n => VisorList.All(y => y.ProductId != n.Id)).Select(n => (VisorData)n.data));
        __instance.allVisors = VisorList.ToArray();
        
        var NamePlateList = __instance.allNamePlates.ToList();
        NamePlateList.AddRange(Instance.CustomNamePlates.Where(n => NamePlateList.All(y => y.ProductId != n.Id)).Select(n => (NamePlateData)n.data));
        __instance.allNamePlates = NamePlateList.ToArray();
        AddEnd = true;
    }
    
    public void LoadConfigFormDisk(DirectoryInfo dir)
    {
        var files = dir.GetFiles(".json");
        foreach (var file in files)
        {
            var str = File.ReadAllText(file.FullName);
            var config = JsonSerializer.Deserialize<CosmeticsManagerConfig>(str);
            Init(config);
        }
    }
    
    public void LoadSpriteFormDisk()
    {
        string[] spriteDir = [HatDir, VisorDir, NamePlateDir];
        foreach (var spDir in spriteDir)
        {
            sprites.AddRange(new SpriteLoader(spDir).LoadAllHatSprite(".png"));
        }
    }
    
    public void Init(CosmeticsManagerConfig config)
    {
        configs.Add(config);
        Task.Factory.StartNew(() => Start(config));
    }

    private readonly HttpClient client = new();
    
    #nullable enable
    public Sprite? GetSprite(CustomCosmeticsFlags flags, string name)
    {
        var AllSprite = flags switch
        {
            CustomCosmeticsFlags.Hat => HatSprites,
            CustomCosmeticsFlags.Visor => VisorSprites,
            CustomCosmeticsFlags.NamePlate => NamePlateSprites,
            _ => sprites.ToHashSet()
        };

        return AllSprite.FirstOrDefault(n => n.name.EndsWith(name));
    }
    #nullable disable


    public async void DownLoadSprite(CustomCosmeticsFlags flag, string root, string dir, string[] res)
    {
        try
        {
            var LocalDir = flag switch
            {
                CustomCosmeticsFlags.Hat => HatDir,
                CustomCosmeticsFlags.Visor => VisorDir,
                CustomCosmeticsFlags.NamePlate => NamePlateDir,
                _ => string.Empty
            };
            if (LocalDir == string.Empty) return;
            foreach (var r in res)
            {
                var localPath = Path.Combine(LocalDir, r);
                if (File.Exists(localPath) || sprites.Exists(n => n.name.EndsWith(r.Replace(".png", string.Empty))))
                    continue;
            
                var url = Path.Combine(root, dir, r);
                var stream = await client.GetStreamAsync(url);
                var file = File.Create(localPath);
                await stream.CopyToAsync(file);
            }
        }
        catch (Exception e)
        {
            Exception(e);
        }
    }
    
    public async void Start(CosmeticsManagerConfig config)
    {
        if (config.hasCosmetics.HasFlag(CustomCosmeticsFlags.Hat))
        {
            var ConfigStream = await client.GetStreamAsync(Path.Combine(config.RootUrl, config.HatFileName));
            var json = JsonDocument.Parse(ConfigStream.ReadToEnd());
            var hats = json.RootElement.GetProperty(config.HatPropertyName).Deserialize<List<CustomHatConfig>>();
            foreach (var hatConfig in hats)
            {
                var res = new[]
                {
                    hatConfig.Resource, hatConfig.BackResource, hatConfig.BackFlipResource, hatConfig.ClimbResource,
                    hatConfig.FlipResource
                }.Where(n => n != string.Empty).ToArray();
                DownLoadSprite(CustomCosmeticsFlags.Hat, config.RootUrl, config.HatDirName, res);
                
                var hat = new CustomHat
                {
                    ManagerConfig = config,
                    config = hatConfig,
                    Resource = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.Resource),
                    BackSprite = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.BackResource),
                    BackFlipSprite = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.BackFlipResource),
                    ClimbSprite = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.ClimbResource),
                    FlipSprite = GetSprite(CustomCosmeticsFlags.Hat, hatConfig.FlipResource)
                };
                hat.data = hatConfig.createHatData(hat.Resource, hat.BackSprite, hat.ClimbSprite, out var id, out var view);
                hat.Id = id;
                hat.View = view;
                customCosmetics.Add(hat);
            }
        }
        
        if (config.hasCosmetics.HasFlag(CustomCosmeticsFlags.Visor))
        {
            var ConfigStream = await client.GetStreamAsync(Path.Combine(config.RootUrl, config.VisorFileName));
            var json = JsonDocument.Parse(ConfigStream.ReadToEnd());
            var visors = json.RootElement.GetProperty(config.VisorPropertyName).Deserialize<List<CustomCosmeticConfig>>();
            foreach (var visorConfig in visors)
            {
                var res = new[] { visorConfig.Resource }.Where(n => n != string.Empty).ToArray();
                DownLoadSprite(CustomCosmeticsFlags.Visor, config.RootUrl, config.VisorDirName, res);
                
                var visor = new CustomVisor
                {
                    ManagerConfig = config,
                    config = visorConfig,
                    Resource = GetSprite(CustomCosmeticsFlags.Visor, visorConfig.Resource),
                };
                visor.data = visorConfig.createVisorData(visor.Resource, out var id, out var view);
                visor.Id = id;
                visor.View = view;
                
                customCosmetics.Add(visor);
            }
        }
        
        if (config.hasCosmetics.HasFlag(CustomCosmeticsFlags.NamePlate))
        {
            var ConfigStream = await client.GetStreamAsync(Path.Combine(config.RootUrl, config.NamePlateFileName));
            var json = JsonDocument.Parse(ConfigStream.ReadToEnd());
            var namePlates = json.RootElement.GetProperty(config.NamePlatePropertyName).Deserialize<List<CustomCosmeticConfig>>();
            foreach (var namePlateConfig in namePlates)
            {
                var res = new[] { namePlateConfig.Resource }.Where(n => n != string.Empty).ToArray();
                DownLoadSprite(CustomCosmeticsFlags.NamePlate, config.RootUrl, config.NamePlateDirName, res);
                
                var NamePlate = new CustomNamePlate
                {
                    ManagerConfig = config,
                    config = namePlateConfig,
                    Resource = GetSprite(CustomCosmeticsFlags.NamePlate, namePlateConfig.Resource)
                };
                NamePlate.data = namePlateConfig.createNamePlateData(NamePlate.Resource, out var id, out var view);
                NamePlate.Id = id;
                NamePlate.View = view;
                
                customCosmetics.Add(NamePlate);
            }
        }
        CheckAddAll();
    }

    public void CheckAddAll()
    {
        if (
            CustomHats.Any(n => HatManager.Instance.allHats.All(y => y.ProductId != n.Id))
            ||
            CustomVisors.Any(n => HatManager.Instance.allVisors.All(y => y.ProductId != n.Id))
            ||
            CustomNamePlates.Any(n => HatManager.Instance.allNamePlates.All(y => y.ProductId != n.Id))
        )
            AddEnd = false;
    }
}

public class CosmeticsManagerConfig
{
    public string RootUrl { get; set; }
    public CustomCosmeticsFlags hasCosmetics { get; set; }
    public string HatDirName { get; set; } = "hats";
    public string VisorDirName { get; set; } = "Visors";
    public string NamePlateDirName { get; set; } = "NamePlates";
    public string HatFileName { get; set; } = "CustomHats.json";
    public string VisorFileName { get; set; } ="CustomVisors.json";
    public string NamePlateFileName { get; set; } ="CustomNamePlates.json";
    
    public string HatPropertyName { get; set; } = "hats";
    public string VisorPropertyName { get; set; } ="Visors";
    public string NamePlatePropertyName { get; set; } ="nameplates";
}

[Flags]
public enum CustomCosmeticsFlags
{
    Hat,
    Skin,
    Visor,
    NamePlate,
    Pet
}