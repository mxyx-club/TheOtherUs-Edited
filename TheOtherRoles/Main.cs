using System;
using AmongUs.Data;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using InnerNet;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using TheOtherRoles.CustomCosmetics;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;

namespace TheOtherRoles;

[BepInPlugin(Id, ModName, VersionString)]
[BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInProcess("Among Us.exe")]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public class TheOtherRolesPlugin : BasePlugin
{
    public const string Id = "TheOtherUs.Options.v2"; // Config files name
    public const string ModName = MyPluginInfo.PLUGIN_NAME;
    public const string VersionString = MyPluginInfo.PLUGIN_VERSION;

    public static Version Version = Version.Parse(VersionString);

    public static TheOtherRolesPlugin Instance;

    public static int optionsPage = 2;

    public static IRegionInfo[] defaultRegions;
    public Harmony Harmony { get; } = new(Id);

    public static ConfigEntry<bool> EnableSoundEffects { get; set; }
    public static ConfigEntry<bool> ToggleCursor { get; set; }
    public static ConfigEntry<bool> ShowFPS { get; set; }
    public static ConfigEntry<bool> LocalHats { get; set; }
    public static ConfigEntry<bool> MuteLobbyBGM { get; set; }
    public static ConfigEntry<bool> ShowChatNotifications { get; set; }
    public static ConfigEntry<bool> ForceUsePlus25Protocol { get; set; }
    public static ConfigEntry<bool> ShowKeyReminder { get; set; }
    public static ConfigEntry<string> Ip { get; set; }
    public static ConfigEntry<ushort> Port { get; set; }
    public static ConfigEntry<string> ShowPopUpVersion { get; set; }

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
        ModTranslation.Load();
        SetLogSource(Log);
        Instance = this;

        ToggleCursor = Config.Bind("Custom", "Better Cursor", true);
        EnableSoundEffects = Config.Bind("Custom", "Enable Sound Effects", true);
        ShowPopUpVersion = Config.Bind("Custom", "Show PopUp", "0");
        ShowFPS = Config.Bind("Custom", "Show FPS", true);
        ShowKeyReminder = Config.Bind("Custom", "ShowKeyReminder", true);
        LocalHats = Config.Bind("Custom", "Load Local Hats", false);
        MuteLobbyBGM = Config.Bind("Custom", "Mute Lobby BGM", true);
        ShowChatNotifications = Config.Bind("Custom", "Show Chat Notifications", true);
        ForceUsePlus25Protocol = Config.Bind("Custom", "Force Use Plus 25 Protocol", false);

        Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1");
        Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023);
        defaultRegions = ServerManager.DefaultRegions;

        UpdateRegions();
        // Removes vanilla Servers
        ServerManager.DefaultRegions = new Il2CppReferenceArray<IRegionInfo>(new IRegionInfo[0]);
        CrowdedPlayer.Start();
        Harmony.PatchAll();
        ModOption.reloadPluginOptions();
        CosmeticsManager.Load();
        CustomOptionHolder.Load();
        AssetLoader.LoadAudioAssets();
        ModInputManager.Load();
        if (ToggleCursor.Value) enableCursor(true);

        SubmergedCompatibility.Initialize();
        MainMenuPatch.addSceneChangeCallbacks();
        Info($"\n---------------\n Loading TheOtherUs completed!\n TheOtherUs-Edited v{VersionString}-Lite\n---------------");
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