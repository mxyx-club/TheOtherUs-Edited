namespace TheOtherRoles.Modules;

public static class ModConfig
{
    public static void Initialize()
    {
        IsCPUProcessorAffinity = Manager.GetOrCreate("cpu.affinity.enabled", false, "Enable CPU affinity");
        ProcessorAffinityMask = Manager.GetOrCreate("cpu.affinity.mask", (ulong)0, "CPU affinity mask");
        ToggleCursor = Manager.GetOrCreate("game.toggleCursor", true, "Toggle cursor");
        EnableSoundEffects = Manager.GetOrCreate("game.enableSoundEffects", true, "Enable sound effects");
        ShowFPS = Manager.GetOrCreate("game.showFPS", true, "Show FPS");
        ShowKeyReminder = Manager.GetOrCreate("game.showKeyReminder", true, "Show key reminder");
        ButtonArrangement = Manager.GetOrCreate("game.buttonArrangement", 3, "Button arrangement");
        UploadGameData = Manager.GetOrCreate("game.uploadGameData", true, "Upload game data");
        AutoScreenshot = Manager.GetOrCreate("game.autoScreenshot", true, "Auto screenshot");
    }

    public static ConfigOption<bool> IsCPUProcessorAffinity { get; private set; }
    public static ConfigOption<ulong> ProcessorAffinityMask { get; private set; }
    public static ConfigOption<bool> ToggleCursor { get; private set; }
    public static ConfigOption<bool> EnableSoundEffects { get; private set; }
    public static ConfigOption<bool> ShowFPS { get; private set; }
    public static ConfigOption<bool> ShowKeyReminder { get; private set; }
    public static ConfigOption<int> ButtonArrangement { get; private set; }
    public static ConfigOption<bool> UploadGameData { get; private set; }
    public static ConfigOption<bool> AutoScreenshot { get; private set; }

    public static void Save() => Manager.Save();
    public static void Reload() => Manager.Reload();
    public static YamlConfigManager Manager => YamlConfigManager.Instance;
}
