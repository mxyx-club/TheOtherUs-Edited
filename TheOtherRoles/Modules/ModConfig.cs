namespace TheOtherRoles.Modules;

public static class ModConfig
{
    public static YamlConfigManager Settings = new("ModConfig.yml");
    public static YamlConfigManager PlayerData = new("PlayerData.yml");

    public static void Initialize()
    {
        IsCPUProcessorAffinity = Settings.GetOrCreate("cpu.affinity.enabled", false);
        ProcessorAffinityMask = Settings.GetOrCreate("cpu.affinity.mask", (ulong)0);
        ToggleCursor = Settings.GetOrCreate("game.toggleCursor", true);
        EnableSoundEffects = Settings.GetOrCreate("game.enableSoundEffects", true);
        ShowFPS = Settings.GetOrCreate("game.showFPS", true);
        ShowKeyReminder = Settings.GetOrCreate("game.showKeyReminder", true);
        UploadGameData = Settings.GetOrCreate("game.uploadGameData", true);
        AutoScreenshot = Settings.GetOrCreate("game.autoScreenshot", true);

        FriendCode = PlayerData.GetOrCreate("player.friendcode", string.Empty);
        Settings.Load();
        PlayerData.Load();
    }

    public static ConfigOption<bool> IsCPUProcessorAffinity { get; private set; }
    public static ConfigOption<ulong> ProcessorAffinityMask { get; private set; }
    public static ConfigOption<bool> ToggleCursor { get; private set; }
    public static ConfigOption<bool> EnableSoundEffects { get; private set; }
    public static ConfigOption<bool> ShowFPS { get; private set; }
    public static ConfigOption<bool> ShowKeyReminder { get; private set; }
    public static ConfigOption<bool> UploadGameData { get; private set; }
    public static ConfigOption<bool> AutoScreenshot { get; private set; }
    public static ConfigOption<string> FriendCode { get; private set; }

}
