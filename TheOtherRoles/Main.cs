using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using TheOtherRoles.CustomCosmetics;
using TheOtherRoles.Objects;

namespace TheOtherRoles;

[BepInAutoPlugin("TheOtherUs.Options.v3")]
[BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInProcess("Among Us.exe")]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class TheOtherRolesPlugin : BasePlugin
{
    public static TheOtherRolesPlugin Instance;
    public const string VersionSuffix = "";
    public static Version version => System.Version.Parse(Version);

    public static int optionsPage = 2;

    public static IRegionInfo[] defaultRegions;
    public Harmony Harmony { get; } = new(Id);

    public static ConfigEntry<bool> IsCPUProcessorAffinity { get; set; }
    public static ConfigEntry<ulong> ProcessorAffinityMask { get; set; }
    public static ConfigEntry<bool> EnableSoundEffects { get; set; }
    public static ConfigEntry<bool> ToggleCursor { get; set; }
    public static ConfigEntry<bool> ShowFPS { get; set; }
    public static ConfigEntry<bool> LocalHats { get; set; }
    public static ConfigEntry<bool> ShowKeyReminder { get; set; }
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

        IsCPUProcessorAffinity = Config.Bind("Custom", "CPUAffinity", false);
        ProcessorAffinityMask = Config.Bind("Custom", "CPUAffinityMask", (ulong)0);
        ToggleCursor = Config.Bind("Custom", "Better Cursor", true);
        EnableSoundEffects = Config.Bind("Custom", "Enable Sound Effects", true);
        ShowFPS = Config.Bind("Custom", "Show FPS", true);
        ShowKeyReminder = Config.Bind("Custom", "ShowKeyReminder", true);
        LocalHats = Config.Bind("Custom", "Load Local Hats", false);

        Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1");
        Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023);
        defaultRegions = ServerManager.DefaultRegions;

        UpdateRegions();
        CrowdedPlayer.Start();
        Harmony.PatchAll();
        CosmeticsManager.Load();
        CustomOptionHolder.Load();
        KillTrap.LoadAudioAssets();
        ModInputManager.Load();
        if (ToggleCursor.Value) enableCursor(true);

        SubmergedCompatibility.Initialize();
        AddToKillDistanceSetting.addKillDistance();
        ChatCommands.Init();
        UpdateCPUProcessorAffinity();
        Info($"\n---------------\n Loading TheOtherUs completed!\n TheOtherUs-Edited v{Version}{VersionSuffix}\n---------------");
    }

    // CPUの割当を変更する
    public static void UpdateCPUProcessorAffinity()
    {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux()) return;

        if (!IsCPUProcessorAffinity.Value || ProcessorAffinityMask.Value == 0)
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

        ulong affinity = ProcessorAffinityMask.Value;
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

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
public static class ChatControllerAwakePatch
{
    private static void Prefix()
    {
        if (!EOSManager.Instance.isKWSMinor)
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
    }
}