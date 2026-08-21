using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using TheOtherRoles.Attributes;
using TheOtherRoles.Patches;

namespace TheOtherRoles;

[BepInAutoPlugin("TheOtherUs.Options.v3")]
[BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInProcess("Among Us.exe")]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class TheOtherRolesPlugin : BasePlugin
{
    public static TheOtherRolesPlugin Instance;
    public const string VersionSuffix = "";
    public static Version version = System.Version.Parse(Version);
    public static string BuildNumber = GetBuildNumber();
    public static int optionsPage = 2;

    public static IRegionInfo[] defaultRegions;
    public Harmony Harmony { get; } = new(Id);

    public static ConfigEntry<string> Ip { get; set; }
    public static ConfigEntry<ushort> Port { get; set; }

    // This is part of the Mini.RegionInstaller, Licensed under GPLv3
    // file="RegionInstallPlugin.cs" company="miniduikboot">
    public static void UpdateRegions()
    {
        var serverManager = FastDestroyableSingleton<ServerManager>.Instance;
        var regions = new[]
        {
            new StaticHttpRegionInfo("Custom", StringNames.NoTranslation, Ip.Value,
                    new Il2CppReferenceArray<ServerInfo>([new("Custom", Ip.Value, Port.Value, false)]))
                .CastFast<IRegionInfo>()
        };

        var currentRegion = serverManager.CurrentRegion;
        Info($"Adding {regions.Length} regions");
        foreach (var region in regions)
            if (region == null)
            {
                Error("Could not add region");
            }
            else
            {
                if (currentRegion != null && region.Name.Equals(currentRegion.Name, StringComparison.OrdinalIgnoreCase))
                    currentRegion = region;
                serverManager.AddOrUpdateRegion(region);
            }

        // AU remembers the previous region that was set, so we need to restore it
        if (currentRegion == null) return;
        Debug("Resetting previous region");
        serverManager.SetRegion(currentRegion);
    }

    public override void Load()
    {
        SetLogSource(Log);
        ModTranslation.Load();
        Instance = this;

        // 初始化Mod配置
        ModInputManager.Load();
        ModConfig.Initialize();

        Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1");
        Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023);
        defaultRegions = ServerManager.DefaultRegions;

        Harmony.PatchAll();
        UpdateRegions();
        CrowdedPlayer.Start();
        CustomColors.Load();
        CustomOptionHolder.Load();
        if (ModConfig.ToggleCursor.Value) enableCursor(true);

        SubmergedCompatibility.Initialize();
        AddToKillDistanceSetting.addKillDistance();
        PluginModuleInitializerAttribute.Invoke();
        LightPatch.Initialize();
        UpdateCPUProcessorAffinity();
        Info($"\n---------------\n Loading TheOtherUs completed!\n TheOtherUs-Edited v{Version}{VersionSuffix}\n Build Number: {BuildNumber}\n Build Date: {GetCompileTime():yyyy-MM-dd HH:mm:ss}\n Mods: {IL2CPPChainloader.Instance.Plugins.Count}\n---------------");
    }

    // CPUの割当を変更する
    public static void UpdateCPUProcessorAffinity()
    {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;

        if (!ModConfig.IsCPUProcessorAffinity.Value || ModConfig.ProcessorAffinityMask.Value == 0)
        {
            try
            {
                ulong allCores = (1UL << Environment.ProcessorCount) - 1;
                System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)allCores;
                Info("CPU Processor Affinity disabled, using all cores.", "CPUAffinity");
            }
            catch (Exception ex)
            {
                Error($"Failed to reset CPU affinity: {ex}", "CPUAffinity");
            }
            return;
        }

        ulong affinity = ModConfig.ProcessorAffinityMask.Value;
        try
        {
            System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)affinity;
            Info($"UpdatedCPUProcessorAffinity To: {affinity}", "CPUAffinity");
        }
        catch (Exception ex)
        {
            Error($"Failed to set CPU affinity: {ex}", "CPUAffinity");
        }
    }

    public static string GetBuildNumber()
    {
        try
        {
            var f = System.Reflection.Assembly.GetExecutingAssembly().GetType("Builtin")
                ?.GetField("BuildNumber", BindingFlags.Public | BindingFlags.Static);
            if (f?.GetValue(null) is int n && n > 0) return n.ToString();
        }
        catch { }
        var mvid = System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToString();
        return mvid.Length >= 6 ? mvid[..6] : mvid;
    }
}

// Deactivate bans, since I always leave my local testing game and ban myself
[HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
public static class AmBannedPatch
{
    public static void Postfix(out bool __result)
    {
        __result = false;
    }
}
