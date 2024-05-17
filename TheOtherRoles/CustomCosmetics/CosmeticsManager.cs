using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BepInEx;
using TheOtherRoles.CustomCosmetics.Configs;
using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.CustomCosmetics;

[Harmony]
public class CosmeticsManager : ManagerBase<CosmeticsManager>
{
    public readonly BlockingCollection<Sprite> Sprites = [];
    public HashSet<Sprite> HatSprites => Sprites.Where(n => n.name.StartsWith("Hats/")).ToHashSet();
    public HashSet<Sprite> VisorSprites => Sprites.Where(n => n.name.StartsWith("Visors/")).ToHashSet();
    public HashSet<Sprite> NamePlateSprites => Sprites.Where(n => n.name.StartsWith("NamePlates/")).ToHashSet();

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
        string[] list =
        [
            CosmeticDir, HatDir, VisorDir, NamePlateDir, ManagerConfigDir, LocalDir, LocalHatDir, LocalVisorDir,
            LocalNamePlateDir
        ];
        foreach (var path in list)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path!);
        }
    }

    public static readonly CosmeticsManagerConfig DefConfig = new()
    {
        ConfigName = "TheOtherHats",
        RootUrl = "https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherHats/master",
        hasCosmetics = CustomCosmeticsFlags.Hat
    };

    public static readonly CosmeticsManagerConfig SNRConfig = new()
    {
        ConfigName = "SuperNewCosmetic",
        RootUrl = "https://raw.githubusercontent.com/SuperNewRoles/SuperNewCosmetics/main",
        hasCosmetics = CustomCosmeticsFlags.Hat | CustomCosmeticsFlags.Visor | CustomCosmeticsFlags.NamePlate
    };

    public readonly HashSet<CosmeticsManagerConfig> configs = [];

    public readonly BlockingCollection<ICustomCosmetic> CustomCosmetics = [];
    public readonly ConcurrentQueue<ICustomCosmetic> NoLoad = [];
    public HashSet<CustomHat> CustomHats => 
        CustomCosmetics.Where(n => n is CustomHat custom && custom.Data != null).Cast<CustomHat>().ToHashSet();

    public HashSet<CustomVisor> CustomVisors =>
        CustomCosmetics.Where(n => n is CustomVisor custom && custom.Data != null).Cast<CustomVisor>().ToHashSet();

    public HashSet<CustomNamePlate> CustomNamePlates =>
        CustomCosmetics.Where(n => n is CustomNamePlate custom && custom.Data != null).Cast<CustomNamePlate>().ToHashSet();
    
    
    public bool TryGetHatView(string Id, [MaybeNullWhen(false)] out HatViewData data)
    {
        var hat = CustomHats.FirstOrDefault(n => n.Id == Id);
        data = hat?.View;
        return hat != null;
    }
    
    public bool TryGet<T>(string Id, [MaybeNullWhen(false)] out T data) where T : class, ICustomCosmetic
    {
        var custom = CustomCosmetics.FirstOrDefault(n => n.Id == Id && n is T) as T;
        data = custom;
        return custom != null;
    }
    
    public bool TryGet(string Id, [MaybeNullWhen(false)] out ICustomCosmetic data)
    {
        var custom = CustomCosmetics.FirstOrDefault(n => n.Id == Id);
        data = custom;
        return custom != null;
    }

    public bool TryGetVisorView(string Id, [MaybeNullWhen(false)] out VisorViewData data)
    {
        var visor = CustomVisors.FirstOrDefault(n => n.Id == Id);
        data = visor?.View;
        return visor != null;
    }

    public bool TryGetNamePlateView(string Id, [MaybeNullWhen(false)] out NamePlateViewData data)
    {
        var namePlate = CustomNamePlates.FirstOrDefault(n => n.Id == Id);
        data = namePlate?.View;
        return namePlate != null;
    }

    public bool TryGetHat(HatData data, [MaybeNullWhen(false)] out CustomHat Hat)
    {
        var hat = CustomHats.FirstOrDefault(n => n.Data == data);
        Hat = hat;
        return hat != null;
    }

    public bool TryGetHat(string Id, [MaybeNullWhen(false)] out CustomHat data)
    {
        var hat = CustomHats.FirstOrDefault(n => n.Id == Id);
        data = hat;
        return hat != null;
    }

    public bool TryGetVisor(string Id, [MaybeNullWhen(false)] out CustomVisor data)
    {
        var visor = CustomVisors.FirstOrDefault(n => n.Id == Id);
        data = visor;
        return visor != null;
    }

    public bool TryGetNamePlate(string Id, [MaybeNullWhen(false)] out CustomNamePlate data)
    {
        var namePlate = CustomNamePlates.FirstOrDefault(n => n.Id == Id);
        data = namePlate;
        return namePlate != null;
    }

    public void AddDefConfig()
    {
        configs.Add(DefConfig);
        configs.Add(SNRConfig);
    }
    
    
    public void DefConfigCreateAndInit()
    {
        ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
        AddDefConfig();
        Task.Run(() => LoadConfigFormDisk(new DirectoryInfo(ManagerConfigDir))).GetAwaiter().OnCompleted(
            () =>
            {
                foreach (var config in configs)
                {
                    Start(config);
                }
            });
    }

    private static bool AddEnd;
    

    [HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize)), HarmonyPostfix]
    private static void OnInit(HatManager __instance) => OnHatManager_InstantiatePostfix(__instance);
    
    
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetVisorById))]
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetUnlockedHats))]
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetUnlockedVisors))]
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetUnlockedNamePlates))]
    [HarmonyPrefix]
    private static void OnHatManager_InstantiatePostfix(HatManager __instance)
    {
        if (AddEnd) return;
        
        var hatList = __instance.allHats.ToList();
        hatList.AddRange(Instance.CustomHats.Where(hat => hat.Data != null && hatList.All(y => hat.Data != y)).Select(hat => hat.Data));
        __instance.allHats = new Il2CppReferenceArray<HatData>(hatList.ToArray());
        
        var visorList = __instance.allVisors.ToList();
        visorList.AddRange(Instance.CustomVisors.Where(visor => visor.Data != null && visorList.All(y => visor.Data != y)).Select(visor => visor.Data));
        __instance.allVisors = new Il2CppReferenceArray<VisorData>(visorList.ToArray());

        var namePlateList = __instance.allNamePlates.ToList();
        namePlateList.AddRange(Instance.CustomNamePlates.Where(nameplate => nameplate.Data != null && namePlateList.All(y => nameplate.Data != y)).Select(nameplate => nameplate.Data));
        __instance.allNamePlates = new Il2CppReferenceArray<NamePlateData>(namePlateList.ToArray());
        
        Info("AddEnd");
        AddEnd = true;
    }

    private static bool checkEd;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
    [HarmonyPatch(typeof(VisorsTab), nameof(VisorsTab.OnEnable))]
    [HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable))]
    private static void OnTabPrefix()
    {
        if (checkEd) return;
        CheckAddAll();
        checkEd = true;
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd)), HarmonyPostfix]
    private static void OnEnd() => checkEd = false;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.OnDisable))]
    private static void OnTabDisablePostfix()
    {
        if (SpriteReader.Instance.createRunning || Instance.NoLoad.Any())
        {
            checkEd = false;
        }
    }

    public void LoadConfigFormDisk(DirectoryInfo dir)
    {
        var files = dir.GetFiles(".json");
        foreach (var file in files)
        {
            if (configs.Any(n => n.ConfigName == file.Name))
                continue;
            var str = File.ReadAllText(file.FullName);
            var config = JsonSerializer.Deserialize<CosmeticsManagerConfig>(str);
            config.ConfigName = file.Name;
            configs.Add(config);
        };
    }
    

    private readonly HttpClient _client = new();

    public async Task<bool> DownLoadSprite(ICustomCosmetic customCosmetic, string root, string dir)
    {
        try
        {
            foreach (var r in customCosmetic.Resources)
            {
                var localPath = GetLocalPath(customCosmetic.Flags, r);
                if (File.Exists(localPath) || Sprites.Any(n => n.name.Contains(r)))
                {
                    continue;
                }

                var url = $"{root}/{dir}/{r}";
                Info($"DownLoad {customCosmetic.Flags} {url} {r}");
                await using var stream = await _client.GetStreamAsync(url);
                await using var file = File.Create(localPath!);
                await stream.CopyToAsync(file);
            }

            return true;
        }
        catch (Exception e)
        {
            Exception(e);
            return false;
        }
    }

    public static string GetLocalPath(CustomCosmeticsFlags flag, string fileName)
    {
        var localDir = flag switch
        {
            CustomCosmeticsFlags.Hat => HatDir,
            CustomCosmeticsFlags.Visor => VisorDir,
            CustomCosmeticsFlags.NamePlate => NamePlateDir,
            _ => string.Empty
        };
        var localPath = Path.Combine(localDir, fileName);
        return localPath;
    }

    private static readonly JsonDocumentOptions options = new()
    {
        AllowTrailingCommas = true
    };

    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        AllowTrailingCommas = true
    };

    public T GetJson<T>(string url, CustomCosmeticsFlags flag, string propertyName)
    {
        Info($"GetJson {url} {flag} {propertyName}");
        var jsonText = _client.GetStringAsync(url).Result;
        Info($"Read Json\n {flag}:\n {jsonText}");
        var json = JsonDocument.Parse(jsonText, options);
        var deserialize = json.RootElement.GetProperty(propertyName).Deserialize<T>(serializerOptions);
        return deserialize;
    }

    public void Start(CosmeticsManagerConfig config)
    {
        Info($"Start {config.RootUrl}");
        try
        {
            if (config.hasCosmetics.HasFlag(CustomCosmeticsFlags.Hat))
            {
                Info("Start Hat");
                var hats = GetJson<List<CustomHatConfig>>($"{config.RootUrl}/{config.HatFileName}",
                    CustomCosmeticsFlags.Hat, config.HatPropertyName);
                foreach (var hat in hats.Select(hatConfig => new CustomHat
                         {
                             ManagerConfig = config,
                             config = hatConfig
                         }))
                {
                    if (!DownLoadSprite(hat, config.RootUrl, config.HatDirName).Result)
                    {
                        continue;
                    }
                    CustomCosmetics.Add(hat);
                    NoLoad.Enqueue(hat);
                }
            }

            if (config.hasCosmetics.HasFlag(CustomCosmeticsFlags.Visor))
            {
                Info("Start Visor");
                var visors = GetJson<List<CustomCosmeticConfig>>($"{config.RootUrl}/{config.VisorFileName}",
                    CustomCosmeticsFlags.Visor, config.VisorPropertyName);
                foreach (var visor in visors.Select(visorConfig => new CustomVisor
                         {
                             ManagerConfig = config,
                             config = visorConfig,
                         }))
                {
                    if (!DownLoadSprite(visor, config.RootUrl, config.VisorDirName).Result)
                    {
                        continue;
                    }

                    CustomCosmetics.Add(visor);
                    NoLoad.Enqueue(visor);
                }
            }

            if (config.hasCosmetics.HasFlag(CustomCosmeticsFlags.NamePlate))
            {
                Info("Start NamePlate");
                var namePlates = GetJson<List<CustomCosmeticConfig>>($"{config.RootUrl}/{config.NamePlateFileName}",
                    CustomCosmeticsFlags.NamePlate, config.NamePlatePropertyName);
                foreach (var namePlate in namePlates.Select(namePlateConfig => new CustomNamePlate
                         {
                             ManagerConfig = config,
                             config = namePlateConfig
                         }))
                {
                    if (!DownLoadSprite(namePlate, config.RootUrl, config.NamePlateDirName).Result)
                    {
                        continue;
                    }
                    CustomCosmetics.Add(namePlate);
                    NoLoad.Enqueue(namePlate);
                }
            }
        }
        catch (Exception ex)
        {
            Exception(ex);
        }
    }

    public static void CheckAddAll()
    {
        if (!HatManager._instance) return;
        foreach (var cc in Instance.CustomCosmetics.Where(cc => HatManager.Instance.allHats.All(n => n.ProductId != cc.Id) && HatManager.Instance.allVisors.All(n => n.ProductId != cc.Id) && HatManager.Instance.allNamePlates.All(n => n.ProductId != cc.Id)))
        {
            AddEnd = false;
            break;
        }

        Info($"Check {AddEnd}");
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