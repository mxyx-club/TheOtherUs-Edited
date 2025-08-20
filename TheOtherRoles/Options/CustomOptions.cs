using System.Globalization;
using System.IO;
using System.Text;
using AmongUs.GameOptions;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Il2CppSystem.Linq;
using TheOtherRoles.Mode;
using UnityEngine.UI;
using static TheOtherRoles.Options.CustomOption;

namespace TheOtherRoles.Options;

public enum CustomOptionType
{
    General,
    Guesser,
    Impostor,
    Neutral,
    Crewmate,
    Modifier,
    GhostRole,
    //Advanced,

    // GameMode
}

public class CustomOption
{
    private static List<CustomOption> _options = new(1024);
    public static IReadOnlyList<CustomOption> Options => _options;
    public static int Preset;
    public static ConfigEntry<string> VanillaSettings;

    public int DefaultSelection;
    public ConfigEntry<int> Entry;

    public int Id;
    public bool IsHeader;
    public string Name;
    public Action<CustomOption> OnChange;
    public OptionBehaviour OptionBehaviour;
    public CustomOption Parent;
    public int Selection;
    public object[] Selections;
    public CustomOptionType Type;
    public Func<bool> IsHiddenFunc;
    public bool IsText;
    public CustomGameModes GameMode = CustomGameModes.Classic;

    public RoleId RoleId;
    // Option creation

    public CustomOption(int id, CustomOptionType type, string name, object[] selections, object defaultValue,
        CustomOption parent, bool isHeader, Func<bool> isHidden = null, Action<CustomOption> onChange = null)
    {
        Id = id;
        Name = name;
        Selections = selections;
        var index = Array.IndexOf(selections, defaultValue);
        DefaultSelection = index >= 0 ? index : 0;
        Parent = parent;
        IsHeader = isHeader;
        Type = type;
        OnChange = onChange;
        IsText = false;
        IsHiddenFunc = isHidden;
        Selection = 0;
        if (id != 0)
        {
            Entry = Main.Instance.Config.Bind($"Preset{Preset}", id.ToString(), DefaultSelection);
            Selection = Mathf.Clamp(Entry.Value, 0, selections.Length - 1);
        }

        _options.Add(this);
    }

    public static CustomOption Create(int id, CustomOptionType type, string name, string[] selections,
        CustomOption parent = null, bool isHeader = false, Func<bool> isHidden = null, Action<CustomOption> onChange = null)
    {
        return new CustomOption(id, type, name, selections, "", parent, isHeader, isHidden, onChange);
    }

    public static CustomOption Create(int id, CustomOptionType type, string name, float defaultValue, float min,
        float max, float step, CustomOption parent = null, bool isHeader = false, Func<bool> isHidden = null, Action<CustomOption> onChange = null)
    {
        List<object> selections = new();
        for (var s = min; s <= max; s += step) selections.Add(s);
        return new CustomOption(id, type, name, selections.ToArray(), defaultValue, parent, isHeader, isHidden, onChange);
    }

    public static CustomOption Create(int id, CustomOptionType type, string name, bool defaultValue,
        CustomOption parent = null, bool isHeader = false, Func<bool> isHidden = null, Action<CustomOption> onChange = null)
    {
        string[] selections = name.Contains("Options") ? ["ExpandOptions", "CollapseOptions"] : ["optionOff", "optionOn"];
        var defaultSelection = defaultValue ? selections[1] : selections[0];
        return new CustomOption(id, type, name, selections, defaultSelection, parent, isHeader, isHidden, onChange);
    }

    // Static behaviour

    public void SyncMinMax(bool isMax, params CustomOption[] opts)
    {
        foreach (var opt in opts)
        {

            if (!isMax && Selection > opt.Selection)
            {
                opt.updateSelection(Selection);

            }
            else if (isMax && Selection < opt.Selection)
            {
                opt.updateSelection(Selection);

            }
        }
    }

    public static void switchPreset(int newPreset)
    {
        saveVanillaOptions();
        Preset = newPreset;
        VanillaSettings = Main.Instance.Config.Bind($"Preset{Preset}", "GameOptions", "");
        loadVanillaOptions();
        foreach (var option in Options)
        {
            if (option.Id == 0) continue;

            option.Entry = Main.Instance.Config.Bind($"Preset{Preset}", option.Id.ToString(), option.DefaultSelection);
            option.Selection = Mathf.Clamp(option.Entry.Value, 0, option.Selections.Length - 1);
            if (option.OptionBehaviour is not null and StringOption stringOption)
            {
                stringOption.oldValue = stringOption.Value = option.Selection;
                stringOption.ValueText.text = option.GetString();
            }
        }
    }

    public static void saveVanillaOptions()
    {
        VanillaSettings.Value =
            Convert.ToBase64String(
#if MXYX_CLUB
                GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameManager.Instance.LogicOptions.currentGameOptions));
#else
            GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameManager.Instance.LogicOptions.currentGameOptions, false));
#endif
    }

    public static void loadVanillaOptions()
    {
        var optionsString = VanillaSettings.Value;
        if (optionsString == "") return;
        GameOptionsManager.Instance.GameHostOptions =
            GameOptionsManager.Instance.gameOptionsFactory.FromBytes(Convert.FromBase64String(optionsString));
        GameOptionsManager.Instance.CurrentGameOptions = GameOptionsManager.Instance.GameHostOptions;
        GameManager.Instance.LogicOptions.SetGameOptions(GameOptionsManager.Instance.CurrentGameOptions);
        GameManager.Instance.LogicOptions.SyncOptions();
    }

    public static void ShareOptionChange(uint optionId)
    {
        var option = Options.FirstOrDefault(x => x.Id == optionId);
        if (option == null) return;
        var writer = StartRPC(CustomRPC.ShareOptions);
        writer.Write((byte)1);
        writer.WritePacked((uint)option.Id);
        writer.WritePacked(Convert.ToUInt32(option.Selection));
        writer.EndRPC();
    }

    public static void ShareOptionSelections()
    {
        if (PlayerControl.AllPlayerControls.ToList().Count <= 1 ||
            (!AmongUsClient.Instance!.AmHost && PlayerControl.LocalPlayer == null)) return;
        var optionsList = new List<CustomOption>(Options);
        while (optionsList.Any())
        {
            var amount = (byte)Math.Min(optionsList.Count, 200); // takes less than 3 bytes per option on average
            var writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.ShareOptions, SendOption.Reliable);
            writer.Write(amount);
            for (var i = 0; i < amount; i++)
            {
                var option = optionsList[0];
                optionsList.RemoveAt(0);
                writer.WritePacked((uint)option.Id);
                writer.WritePacked(Convert.ToUInt32(option.Selection));
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }

    // Getter

    public int GetSelection()
    {
        return Selection;
    }

    public bool GetBool()
    {
        return Selection > 0;
    }

    public float GetFloat()
    {
        return (float)Selections[Selection];
    }

    public int GetInt()
    {
        return (int)GetFloat();
    }

    public int GetQuantity()
    {
        return Selection + 1;
    }

    public bool IsHidden()
    {
        return IsHiddenFunc != null && IsHiddenFunc.Invoke();
    }

    public bool IsEnbaled()
    {
        var enabled = true;
        var parent = Parent;
        while (parent != null && enabled)
        {
            enabled = parent.Selection != 0;
            parent = parent.Parent;
        }

        return !IsHidden() && enabled;
    }

    public string GetString()
    {
        var selObj = Selections[Selection];
        var sel = selObj is float f ? Math.Round(f, 2).ToString("0.##", CultureInfo.InvariantCulture) : selObj.ToString();
        return sel switch
        {
            "optionOn" => "<color=#FFFF00FF>" + sel.Translate() + "</color>",
            "optionOff" => "<color=#CCCCCCFF>" + sel.Translate() + "</color>",
            "ExpandOptions" => "<color=#CCCCCCFF>" + sel.Translate() + "</color>",
            "CollapseOptions" => "<color=#CCCCCCFF>" + sel.Translate() + "</color>",
            _ => sel.Translate(),
        };
    }

    public string GetName()
    {
        return Name.Translate();
    }

    // Option changes
    public void updateSelection(int newSelection)
    {
        Selection = Mathf.Clamp((newSelection + Selections.Length) % Selections.Length, 0, Selections.Length - 1);
        try
        {
            OnChange?.Invoke(this);
        }
        catch
        {
            // ignored
        }

        if (OptionBehaviour is not null and StringOption stringOption)
        {
            stringOption.oldValue = stringOption.Value = Selection;
            stringOption.ValueText.text = GetString();
            if (AmongUsClient.Instance?.AmHost != true || !PlayerControl.LocalPlayer) return;
            if (Id == 0 && Selection != Preset)
            {
                switchPreset(Selection); // Switch presets
                ShareOptionSelections();
            }
            else if (Entry != null)
            {
                Entry.Value = Selection; // Save selection to config
                ShareOptionChange((uint)Id); // Share single selection
            }
        }
        else if (Id == 0 && AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer)
        {
            // Share the preset switch for random maps, even if the menu isnt open!
            switchPreset(Selection);
            ShareOptionSelections(); // Share all selections
        }
        if (AmongUsClient.Instance?.AmHost == true) GameOptionsMenuUpdatePatch.update = true;
    }

    public static byte[] serializeOptions()
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                var lastId = -1;
                foreach (var option in Options.OrderBy(x => x.Id))
                {
                    if (option.Id == 0) continue;

                    int idDelta = option.Id - lastId - 1;
                    lastId = option.Id;

                    bool consecutive = idDelta == 0;

                    uint value = (uint)option.Selection;
                    if (!consecutive)
                    {
                        value |= 0x80;
                        value |= (uint)idDelta << 8;
                    }

                    while (value > 0x7F)
                    {
                        binaryWriter.Write((byte)((value & 0x7F) | 0x80));
                        value >>= 7;
                    }
                    binaryWriter.Write((byte)value);
                }

                binaryWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }

    public static void deserializeOptions(byte[] inputValues)
    {
        try
        {
            var reader = new BinaryReader(new MemoryStream(inputValues));
            int lastId = -1;

            while (reader.BaseStream.Position < inputValues.Length)
            {
                uint value = 0;
                int shift = 0;
                byte b;
                do
                {
                    b = reader.ReadByte();
                    value |= (uint)(b & 0x7F) << shift;
                    shift += 7;
                } while ((b & 0x80) != 0);

                bool consecutive = (value & 0x80) == 0;
                int selection = (int)(value & 0x7F);

                int currentId;
                if (consecutive)
                {
                    currentId = lastId + 1;
                }
                else
                {
                    int idDelta = (int)((value >> 8) & 0x7FFFFF);
                    currentId = lastId + idDelta + 1;
                }

                lastId = currentId;

                var option = Options.FirstOrDefault(x => x.Id == currentId);
                if (option != null)
                {
                    option.updateSelection(selection);
                }
            }
        }
        catch (Exception e)
        {
            Warn($"反序列化失败: {e}");
            FastDestroyableSingleton<HudManager>.Instance?.Chat?.AddChat(PlayerControl.LocalPlayer, "设置粘贴失败: 无效的格式");
        }
    }

    // Copy to or paste from clipboard (as string)
    public static void copyToClipboard()
    {
        GUIUtility.systemCopyBuffer = $"{Main.Version}!{Convert.ToBase64String(serializeOptions())}!{VanillaSettings.Value}";
    }

    public static bool pasteFromClipboard()
    {
        var allSettings = GUIUtility.systemCopyBuffer;
        try
        {
            var settingsSplit = allSettings.Split("!");
            var versionInfo = settingsSplit[0];
            var torSettings = settingsSplit[1];
            var vanillaSettingsSub = settingsSplit[2];
            deserializeOptions(Convert.FromBase64String(torSettings));

            VanillaSettings.Value = vanillaSettingsSub;
            loadVanillaOptions();
            return true;
        }
        catch (Exception e)
        {
            Warn($"试图粘贴无效的设置 : {e}");
            FastDestroyableSingleton<HudManager>.Instance?.Chat?.AddChat(PlayerControl.LocalPlayer, "试图粘贴无效的设置");
            SoundEffectsManager.play("fail");
            return false;
        }
    }
}

[HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
internal class GameOptionsMenuStartPatch
{
    public static void Postfix(GameOptionsMenu __instance)
    {
        switch (ModOption.GameMode)
        {
            case CustomGameModes.Classic or CustomGameModes.Anonymous:
                createClassicTabs(__instance);
                break;
        }
        GameObject.Find("ResetToDefault")?.Destroy();
        GameObject.Find("ConfirmEjects")?.Destroy();
        if (GameObject.Find("CopyButton") != null) return;
        // create copy to clipboard and paste from clipboard buttons.
        var template = GameObject.Find("CloseButton");
        if (template == null) return;
        var copyButton = UObject.Instantiate(template, template.transform.parent);
        copyButton.transform.localPosition += Vector3.down * 0.8f;
        copyButton.name = "CopyButton";
        var copyButtonPassive = copyButton.GetComponent<PassiveButton>();
        var copyButtonRenderer = copyButton.GetComponent<SpriteRenderer>();
        copyButtonRenderer.sprite = new ResourceSprite("CopyButton.png", 175f);
        copyButtonPassive.OnClick.RemoveAllListeners();
        copyButtonPassive.OnClick = new Button.ButtonClickedEvent();
        copyButtonPassive.OnClick.AddListener((Action)(() =>
        {
            copyToClipboard();
            copyButtonRenderer.color = Color.green;
            __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p =>
            {
                if (p > 0.95)
                    copyButtonRenderer.color = Color.white;
            })));
        }));

        var pasteButton = UObject.Instantiate(template, template.transform.parent);
        pasteButton.transform.localPosition += Vector3.down * 1.6f;
        pasteButton.name = "PasteButton";
        var pasteButtonPassive = pasteButton.GetComponent<PassiveButton>();
        var pasteButtonRenderer = pasteButton.GetComponent<SpriteRenderer>();
        pasteButtonRenderer.sprite = new ResourceSprite("PasteButton.png", 175f);
        pasteButtonPassive.OnClick.RemoveAllListeners();
        pasteButtonPassive.OnClick = new Button.ButtonClickedEvent();
        pasteButtonPassive.OnClick.AddListener((Action)(() =>
        {
            pasteButtonRenderer.color = Color.yellow;
            var success = pasteFromClipboard();
            pasteButtonRenderer.color = success ? Color.green : Color.red;
            __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p =>
            {
                if (p > 0.95)
                    pasteButtonRenderer.color = Color.white;
            })));
        }));
    }

    private static void createClassicTabs(GameOptionsMenu __instance)
    {
        foreach (var option in __instance.Children)
        {
            switch (option.Title)
            {
                case StringNames.GameVotingTime:
                    option.Cast<NumberOption>().ValidRange = new FloatRange(0, 360);
                    break;
                case StringNames.GameShortTasks:
                    option.Cast<NumberOption>().ValidRange = new FloatRange(0, 23);
                    break;
                case StringNames.GameLongTasks:
                    option.Cast<NumberOption>().ValidRange = new FloatRange(0, 15);
                    break;
                case StringNames.GameCommonTasks:
                    option.Cast<NumberOption>().ValidRange = new FloatRange(0, 4);
                    break;
                case StringNames.GameKillCooldown:
                    option.Cast<NumberOption>().ValidRange = new FloatRange(0, 60);
                    break;
                default:
                    break;
            }
        }

        var template = UObject.FindObjectsOfType<StringOption>().FirstOrDefault();
        if (template == null) return;
        var gameSettings = GameObject.Find("Game Settings");
        if (gameSettings == null) return;

        gameSettings.transform.FindChild("GameGroup").GetComponent<Scroller>().ScrollWheelSpeed = 1f;
        var gameSettingMenu = UObject.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();
        if (gameSettingMenu == null) return;

        var gameTab = GameObject.Find("GameTab");
        var roleTab = GameObject.Find("RoleTab");
        gameTab.transform.position += Vector3.left * 3f;
        GameObject prevTab = gameTab;

        List<GameObject> tabs = new() { gameTab };
        List<GameObject> menus = new() { gameSettingMenu.RegularGameSettings };
        List<SpriteRenderer> highlights = new() { gameSettingMenu.GameSettingsHightlight };

        foreach (var type in EnumHelper.GetAllValues<CustomOptionType>())
        {
            var obj = gameSettings.transform.parent.Find(type + "Tab");
            if (obj != null)
            {
                obj.transform.FindChild("../../GameGroup/Text").GetComponent<TextMeshPro>().SetText(GetString("TabGroup." + type));
                continue;
            }

            var menuObj = UObject.Instantiate(gameSettings, gameSettings.transform.parent);

            menuObj.name = type + "Tab";
            menuObj.transform.FindChild("BackPanel").transform.localScale =
            menuObj.transform.FindChild("Bottom Gradient").transform.localScale = new Vector3(1.6f, 1f, 1f);
            menuObj.transform.FindChild("BackPanel").transform.localPosition += new Vector3(0.4f, 0f, 0f);
            menuObj.transform.FindChild("Bottom Gradient").transform.localPosition += new Vector3(0.2f, 0f, 0f);
            menuObj.transform.FindChild("Background").transform.localScale = new Vector3(1.8f, 1f, 1f);
            menuObj.transform.FindChild("Background").transform.localPosition += new Vector3(0.2f, 0f, 0f);
            menuObj.transform.FindChild("UI_Scrollbar").transform.localPosition += new Vector3(1.6f, 0f, 0f);
            menuObj.transform.FindChild("UI_ScrollbarTrack").transform.localPosition += new Vector3(1.6f, 0f, 0f);
            menuObj.transform.FindChild("GameGroup/SliderInner").transform.localPosition += new Vector3(-0.1f, 0f, 0f);
            menuObj.transform.FindChild("GameGroup/Text").transform.localPosition += new Vector3(-0.6f, 0f, 0f);

            var menu = menuObj.transform.FindChild("GameGroup/SliderInner").GetComponent<GameOptionsMenu>();

            menu.GetComponentsInChildren<OptionBehaviour>().Do(x => UObject.Destroy(x.gameObject));

            var stringOptions = new List<OptionBehaviour>();
            foreach (var option in CustomOption.Options)
            {
                if (option.Type != type) continue;
                if (option.OptionBehaviour == null)
                {
                    var stringOption = UObject.Instantiate(template, menu.transform);
                    stringOptions.Add(stringOption);
                    //optionBehaviours[(int)option.type].Add(stringOption);
                    stringOption.OnValueChanged = new Action<OptionBehaviour>(o => { });
                    stringOption.TitleText.text = option.GetName();
                    stringOption.Value = stringOption.oldValue = option.Selection;
                    stringOption.ValueText.text = option.GetString();
                    stringOption.name = "Option_" + option.Id.ToString();

                    option.OptionBehaviour = stringOption;

                    float yoffset = option.IsText ? 100f : 0f;
                    stringOption.transform.FindChild("Background").localScale = new Vector3(1.6f, 1f, 1f);
                    stringOption.transform.FindChild("Plus_TMP").localPosition += new Vector3(1.4f, yoffset, 0f);
                    stringOption.transform.FindChild("Minus_TMP").localPosition += new Vector3(1.0f, yoffset, 0f);
                    stringOption.transform.FindChild("Value_TMP").localPosition += new Vector3(1.2f, yoffset, 0f);
                    stringOption.transform.FindChild("Value_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(1.6f, 0.26f);
                    stringOption.transform.FindChild("Title_TMP").localPosition += new Vector3(option.IsText ? 0.25f : 0.1f, option.IsText ? -0.1f : 0f, 0f);
                    stringOption.transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(5.5f, 0.37f);
                }

                option.OptionBehaviour.gameObject.SetActive(true);
            }
            menu.Children = stringOptions.ToArray();
            menuObj.gameObject.SetActive(false);

            menus.Add(menuObj);

            var newTab = UObject.Instantiate(roleTab, gameTab.transform.parent);
            newTab.transform.position = prevTab.transform.position + (Vector3.right * 0.85f);
            newTab.name = type + "TabIcon";

            var highlight = newTab.transform.FindChild("Hat Button").FindChild("Tab Background").GetComponent<SpriteRenderer>();
            newTab.transform.FindChild("Hat Button").FindChild("Icon").GetComponent<SpriteRenderer>().sprite =
                new ResourceSprite($"TabIcon.{type}.png", 100f);

            tabs.Add(newTab);
            highlights.Add(highlight);
            prevTab = newTab;
        }

        var settingsHighlightMap = new Dictionary<GameObject, SpriteRenderer>
        {
            [gameSettingMenu.RegularGameSettings] = gameSettingMenu.GameSettingsHightlight,
        };

        for (var i = 0; i < menus.Count; i++)
        {
            settingsHighlightMap[menus[i].gameObject] = highlights[i];
        }

        for (var i = 0; i < tabs.Count; i++)
        {
            var button = tabs[i].GetComponentInChildren<PassiveButton>();
            if (button == null) continue;
            var copiedIndex = i;
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((Action)(() =>
            {
                setListener(settingsHighlightMap, copiedIndex);
                GameOptionsMenuUpdatePatch.update = true;
            }));
        }

        roleTab.active = false;
    }

    private static void setListener(Dictionary<GameObject, SpriteRenderer> settingsHighlightMap, int index)
    {
        foreach (var entry in settingsHighlightMap)
        {
            entry.Key.SetActive(false);
            entry.Value.enabled = false;
        }

        settingsHighlightMap.ElementAt(index).Key.SetActive(true);
        settingsHighlightMap.ElementAt(index).Value.enabled = true;
    }
}

[HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
public class StringOptionEnablePatch
{
    public static bool Prefix(StringOption __instance)
    {
        var option = CustomOption.Options.FirstOrDefault(option => option.OptionBehaviour == __instance);
        if (option == null) return true;

        __instance.OnValueChanged = new Action<OptionBehaviour>(o => { });
        __instance.TitleText.text = option.GetName();
        __instance.Value = __instance.oldValue = option.Selection;
        __instance.ValueText.text = option.GetString();

        return false;
    }
}

[HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
public class StringOptionIncreasePatch
{
    public static bool Prefix(StringOption __instance)
    {
        var option = CustomOption.Options.FirstOrDefault(option => option.OptionBehaviour == __instance);
        if (option == null) return true;
        option.updateSelection(option.Selection + 1);
        return false;
    }
}

[HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
public class StringOptionDecreasePatch
{
    public static bool Prefix(StringOption __instance)
    {
        var option = CustomOption.Options.FirstOrDefault(option => option.OptionBehaviour == __instance);
        if (option == null) return true;
        option.updateSelection(option.Selection - 1);
        return false;
    }
}

[HarmonyPatch(typeof(StringOption), nameof(StringOption.FixedUpdate))]
public class StringOptionFixedUpdate
{
    public static void Postfix(StringOption __instance)
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out var _)) return;
        var option = CustomOption.Options.FirstOrDefault(option => option.OptionBehaviour == __instance);
        if (option == null) return;
        if (GameOptionsManager.Instance.CurrentGameOptions.MapId == 6)
            if (option.OptionBehaviour is not null and StringOption stringOption)
            {
                stringOption.ValueText.text = option.Selections[option.Selection].ToString();
            }
            else if (option.OptionBehaviour is not null and StringOption stringOptionToo)
            {
                stringOptionToo.oldValue = stringOptionToo.Value = option.Selection;
                stringOptionToo.ValueText.text = option.Selections[option.Selection].ToString();
            }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
public class RpcSyncSettingsPatch
{
    public static void Postfix()
    {
        ShareOptionSelections();
        saveVanillaOptions();
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
public class AmongUsClientOnPlayerJoinedPatch
{
    public static void Postfix()
    {
        if (PlayerControl.LocalPlayer != null && AmongUsClient.Instance.AmHost)
        {
            GameManager.Instance.LogicOptions.SyncOptions();
            ShareOptionSelections();
        }
    }
}

[HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
internal class GameOptionsMenuUpdatePatch
{
    public static bool update;
    public static void Postfix(GameOptionsMenu __instance)
    {
        if (!update) return;

        // Return Menu Update if in normal among us settings 
        var gameSettingMenu = UObject.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();
        if (gameSettingMenu.RegularGameSettings.active || gameSettingMenu.RolesSettings.gameObject.active) return;

        foreach (var tab in EnumHelper.GetAllValues<CustomOptionType>())
        {
            if (__instance.transform.parent.parent.name != tab + "Tab") continue;
            __instance.transform.FindChild("../../GameGroup/Text").GetComponent<TextMeshPro>().SetText($"{GetString($"TabGroup.{tab}")}");

            float numItems = __instance.Children.Length;
            var offset = 2.7f;

            foreach (var option in CustomOption.Options)
            {
                if (tab != option.Type)
                {
                    continue;
                }
                if (option?.OptionBehaviour != null && option.OptionBehaviour.gameObject != null)
                {
                    var enabled = option.IsEnbaled();
                    var parent = option.Parent;
                    option.OptionBehaviour.gameObject.SetActive(enabled);

                    var opt = option.OptionBehaviour.transform.Find("Background").GetComponent<SpriteRenderer>();
                    opt.size = new(5.0f, 0.45f);

                    while (parent != null && enabled)
                    {
                        enabled = parent.IsEnbaled();
                        parent = parent.Parent;

                        opt.color = new(0f, 1f, 0f);
                        opt.size = new(4.8f, 0.45f);
                        opt.transform.localPosition = new Vector3(0.11f, 0f);
                        option.OptionBehaviour.transform.Find("Title_TMP").transform.localPosition = new Vector3(-1.08f, 0f);
                        option.OptionBehaviour.transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(5.1f, 0.28f);

                        if (option.Parent?.Parent != null)
                        {
                            opt.color = new(0f, 0f, 1f);
                            opt.size = new(4.6f, 0.45f);
                            opt.transform.localPosition = new Vector3(0.24f, 0f);
                            option.OptionBehaviour.transform.Find("Title_TMP").transform.localPosition = new Vector3(-0.88f, 0f);
                            option.OptionBehaviour.transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(4.9f, 0.28f);

                            if (option.Parent?.Parent?.Parent != null)
                            {
                                opt.color = new(1f, 0f, 0f);
                                opt.size = new(4.4f, 0.45f);
                                opt.transform.localPosition = new Vector3(0.37f, 0f);
                                option.OptionBehaviour.transform.Find("Title_TMP").transform.localPosition = new Vector3(-0.68f, 0f);
                                option.OptionBehaviour.transform.FindChild("Title_TMP").GetComponent<RectTransform>().sizeDelta = new Vector2(4.7f, 0.28f);
                            }
                        }
                    }

                    if (option.IsText)
                    {
                        opt.color = new(0, 0, 0);
                        opt.transform.localPosition = new(100f, 100f, 100f);
                    }

                    option.OptionBehaviour.gameObject.SetActive(enabled);
                    if (enabled)
                    {
                        offset -= option.IsHeader ? 0.7f : 0.5f;
                        option.OptionBehaviour.transform.localPosition = new Vector3(
                            option.OptionBehaviour.transform.localPosition.x,
                            offset,
                            option.OptionBehaviour.transform.localPosition.z);

                        if (option.IsHeader)
                        {
                            numItems += 0.3f;
                        }
                    }
                    else
                    {
                        numItems--;
                    }
                }
            }

            __instance.GetComponentInParent<Scroller>().ContentYBounds.max = (-offset) - 1.5f;
        }

        update = false;
    }
}

[HarmonyPatch]
internal class GameOptionsDataPatch
{
    public static int maxPage = 8;

    private static string buildRoleOptions()
    {
        var impRoles = $"<size=150%><color=#ff1c1c>{"ImpostorRolesText".Translate()}</color></size>{buildOptionsOfType(CustomOptionType.Impostor, true)}\n";
        var neutralRoles = $"<size=150%><color=#50544c>{"NeutralRolesText".Translate()}</color></size>{buildOptionsOfType(CustomOptionType.Neutral, true)}\n";
        var crewRoles = $"<size=150%><color=#08fcfc>{"CrewmateRolesText".Translate()}</color></size>{buildOptionsOfType(CustomOptionType.Crewmate, true)}\n";
        var modifiers = $"<size=150%><color=#ffec04>{"ModifierRolesText".Translate()}</color></size>{buildOptionsOfType(CustomOptionType.Modifier, true)}";
        var ghostRole = $"<size=150%><color=#ffec04>{"GhostRolesText".Translate()}</color></size>{buildOptionsOfType(CustomOptionType.GhostRole, true)}";
        //var advancedSettingst = $"<size=150%><color=#ffec04>{"AdvancedSettingsText".Translate()}</color></size>{buildOptionsOfType(CustomOptionType.Advanced, true)}";
        return impRoles + neutralRoles + crewRoles + modifiers + ghostRole;
    }

    private static string buildModifierExtras(CustomOption customOption)
    {
        // find options children with quantity
        var children = CustomOption.Options.Where(o => o.Parent == customOption);
        var quantity = children.Where(o => o.Name.Contains("Quantity")).ToList();
        if (customOption.GetSelection() == 0) return "";
        if (quantity.Count == 1) return $" ({quantity[0].GetQuantity()})";
        if (customOption == CustomOptionHolder.modifierLover)
            return $" (1 {"EvilLove".Translate()}: {CustomOptionHolder.modifierLoverImpLoverRate.GetSelection() * 10}%)";
        return "";
    }

    private static string buildOptionsOfType(CustomOptionType type, bool headerOnly)
    {
        var sb = new StringBuilder("\n");
        var options = CustomOption.Options.Where(o => o.Type == type && o.IsEnbaled());
        if (GuesserGM.Enabled)
        {
            if (type == CustomOptionType.General) options = CustomOption.Options.Where(o => o.Type == type || o.Type == CustomOptionType.Guesser);
        }
        else if (ModOption.GameMode is CustomGameModes.Classic or CustomGameModes.Anonymous)
        {
            options = options.Where(x => !(x.Type == CustomOptionType.Guesser));
        }

        foreach (var option in options)
            if (option.Parent == null)
            {
                var line = $"{option.GetName()}: {option.GetString()}";
                if (type == CustomOptionType.Modifier) line += buildModifierExtras(option);
                sb.AppendLine(line);
            }
            else if (option.Parent.GetSelection() > 0)
            {
                if (option.Id == 30170) //Deputy
                    sb.AppendLine($"- {Cs(Sheriff.color, "Deputy".Translate())}: {option.GetString()}");
                else if (option.Id == 20142)
                    sb.AppendLine($"- {Cs(Jackal.color, "jackalSwoopChance".Translate())}: {option.GetString()}");
                else if (option.Id == 20135) //Sidekick
                    sb.AppendLine($"- {Cs(Jackal.color, "Sidekick".Translate())}: {option.GetString()}");
            }

        if (headerOnly) return sb.ToString();
        sb = new StringBuilder();

        foreach (var option in options)
        {
            if (option.Parent != null)
            {
                var isIrrelevant = option.Parent.GetSelection() == 0 ||
                    (option.Parent.Parent != null && option.Parent.Parent.GetSelection() == 0);

                var c = isIrrelevant ? Color.grey : Color.white; // No use for now
                if (isIrrelevant) continue;
                sb.AppendLine(Cs(c, $"{option.GetName()}: {option.GetString()}"));
            }
            else
            {
                if (option == CustomOptionHolder.modifiersCountMin)
                {
                    var optionName = Cs(new Color32(204, 204, 0, 255), "CrewmateRolesText".Translate());
                    var neutralMin = CustomOptionHolder.neutralRolesCountMin.GetSelection();
                    var neutralMax = CustomOptionHolder.neutralRolesCountMax.GetSelection();
                    if (RoleDraft.isEnabled) neutralMin = neutralMax;

                    var min = Math.Max(0, PlayerControl.AllPlayerControls.Count - neutralMax - ModOption.NumImpostors);
                    var max = Math.Max(0, PlayerControl.AllPlayerControls.Count - neutralMin - ModOption.NumImpostors);
                    var optionValue = min == max ? $"{max}" : $"{min} ~ {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if (option == CustomOptionHolder.neutralRolesCountMax)
                {
                    var optionName = Cs(new Color32(204, 204, 0, 255), "NeutralRolesText".Translate());
                    var min = CustomOptionHolder.neutralRolesCountMin.GetSelection();
                    var max = CustomOptionHolder.neutralRolesCountMax.GetSelection();
                    if (RoleDraft.isEnabled) min = max;
                    if (min > max) min = max;
                    var optionValue = min == max ? $"{min}" : $"{min} ~ {max}";

                    var killerMin = CustomOptionHolder.killerNeutralRolesCountMin.GetSelection();
                    var killerMax = CustomOptionHolder.killerNeutralRolesCountMax.GetSelection();
                    if (RoleDraft.isEnabled) killerMin = killerMax;
                    var min2 = Mathf.Min(killerMin, min);
                    var max2 = Mathf.Min(killerMax, max);
                    if (min2 > max2) min2 = max2;
                    var count = killerMin + killerMax;
                    var optionValue2 = count == 0 ? "Random".Translate() : min2 == max2 ? $"{min2}" : $"{min2} ~ {max2}";

                    sb.AppendLine($"{optionName}: {optionValue}  ({"NeutralKillerRolesCount".Translate()}: {optionValue2})");
                }
                else if (option == CustomOptionHolder.killerNeutralRolesCountMax)
                {
                    var optionName = Cs(new Color32(204, 204, 0, 255), "ImpostorRolesText".Translate());
                    sb.AppendLine($"{optionName}: {ModOption.NumImpostors}");
                }
                else if (option == CustomOptionHolder.modifiersCountMax)
                {
                    var optionName = Cs(new Color32(204, 204, 0, 255), "ModifierRolesText".Translate());
                    var min = CustomOptionHolder.modifiersCountMin.GetSelection();
                    var max = CustomOptionHolder.modifiersCountMax.GetSelection();
                    if (min > max) min = max;
                    var optionValue = min == max ? $"{max}" : $"{min} ~ {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if (option == CustomOptionHolder.neutralRolesCountMin ||
                         option == CustomOptionHolder.killerNeutralRolesCountMin)
                {
                }
                else
                {
                    sb.AppendLine($"\n{option.GetName()}: {option.GetString()}");
                }
            }
        }

        return sb.ToString();
    }

    public static string buildAllOptions(string vanillaSettings = "", bool hideExtras = false)
    {
        if (vanillaSettings == "")
            vanillaSettings = GameOptionsManager.Instance.CurrentGameOptions.ToHudString(PlayerControl.AllPlayerControls.Count);
        var counter = Main.optionsPage;
        var hudString = counter != 0 && !hideExtras
            ? Cs(DateTime.Now.Second % 2 == 0 ? Color.white : Color.red, "useScrollWheel".Translate())
            : "";

        maxPage = 8;
        switch (counter)
        {
            case 0:
                hudString += (!hideExtras ? "" : "page1".Translate()) + vanillaSettings;
                break;
            case 1:
                hudString += "page2".Translate() + buildOptionsOfType(CustomOptionType.General, false);
                break;
            case 2:
                hudString += "page3".Translate() + buildRoleOptions();
                break;
            case 3:
                hudString += "page4".Translate() + buildOptionsOfType(CustomOptionType.Impostor, false);
                break;
            case 4:
                hudString += "page5".Translate() + buildOptionsOfType(CustomOptionType.Neutral, false);
                break;
            case 5:
                hudString += "page6".Translate() + buildOptionsOfType(CustomOptionType.Crewmate, false);
                break;
            case 6:
                hudString += "page7".Translate() + buildOptionsOfType(CustomOptionType.Modifier, false);
                break;
            case 7:
                hudString += "page8".Translate() + buildOptionsOfType(CustomOptionType.GhostRole, false);
                break;
        }

        if (!hideExtras || counter != 0) hudString += string.Format("pressTabForMore".Translate(), counter + 1, maxPage);
        return hudString;
    }


    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
    private static void Postfix(ref string __result)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
            return; // Allow Vanilla Hide N Seek
        __result = buildAllOptions(__result);
    }
}

[HarmonyPatch]
public class AddToKillDistanceSetting
{
    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.AreInvalid))]
    [HarmonyPrefix]
    public static bool Prefix(GameOptionsData __instance, ref int maxExpectedPlayers)
    {
        //making the killdistances bound check higher since extra short is added
        return __instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1
                                                          || __instance.NumImpostors > 3
                                                          || __instance.KillDistance < 0
                                                          || __instance.KillDistance >= GameOptionsData.KillDistances.Count
                                                          || __instance.PlayerSpeedMod <= 0f
                                                          || __instance.PlayerSpeedMod > 3f;
    }

    [HarmonyPatch(typeof(NormalGameOptionsV07), nameof(NormalGameOptionsV07.AreInvalid))]
    [HarmonyPrefix]
    public static bool Prefix(NormalGameOptionsV07 __instance, ref int maxExpectedPlayers)
    {
        return __instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1
                                                          || __instance.NumImpostors > 3
                                                          || __instance.KillDistance < 0
                                                          || __instance.KillDistance >= GameOptionsData.KillDistances.Count
                                                          || __instance.PlayerSpeedMod <= 0f
                                                          || __instance.PlayerSpeedMod > 3f;
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
    [HarmonyPrefix]
    public static void Prefix(StringOption __instance)
    {
        //prevents indexoutofrange exception breaking the setting if long happens to be selected
        //when host opens the laptop
        if (__instance.Title == StringNames.GameKillDistance && __instance.Value == 3)
        {
            __instance.Value = 1;
            GameOptionsManager.Instance.currentNormalGameOptions.KillDistance = 1;
            GameManager.Instance.LogicOptions.SyncOptions();
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
    [HarmonyPostfix]
    public static void Postfix(StringOption __instance)
    {
        if (__instance.Title == StringNames.GameKillDistance && __instance.Values.Count == 3)
            __instance.Values = new Il2CppStructArray<StringNames>(
                new[]
                {
                    (StringNames)49999, StringNames.SettingShort, StringNames.SettingMedium, StringNames.SettingLong
                });
    }

    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.AppendItem),
        typeof(Il2CppSystem.Text.StringBuilder), typeof(StringNames), typeof(string))]
    [HarmonyPrefix]
    public static void Prefix(ref StringNames stringName, ref string value)
    {
        if (stringName == StringNames.GameKillDistance)
        {
            int index;
            if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal)
                index = GameOptionsManager.Instance.currentNormalGameOptions.KillDistance;
            else
            {
                index = GameOptionsManager.Instance.currentHideNSeekGameOptions.KillDistance;
            }
            value = GameOptionsData.KillDistanceStrings[index];
        }
    }

    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
        typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
    [HarmonyPriority(Priority.Last)]
    public static bool Prefix(ref string __result, ref StringNames id)
    {
        if ((int)id == 49999)
        {
            __result = "KillDistancesVeryShort".Translate();
            return false;
        }

        return true;
    }

    public static void addKillDistance()
    {
        GameOptionsData.KillDistances = new Il2CppStructArray<float>([0.6f, 1f, 1.8f, 2.5f]);
        GameOptionsData.KillDistanceStrings = new Il2CppStringArray(["Very Short", "Short", "Medium", "Long"]);
    }
}

[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public static class GameOptionsNextPagePatch
{
    public static void Postfix(KeyboardJoystick __instance)
    {
        var page = Main.optionsPage;
        if (Input.GetKeyDown(KeyCode.Tab)) Main.optionsPage = (Main.optionsPage + 1) % 7;
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) Main.optionsPage = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) Main.optionsPage = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) Main.optionsPage = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) Main.optionsPage = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) Main.optionsPage = 4;
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) Main.optionsPage = 5;
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) Main.optionsPage = 6;
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) Main.optionsPage = 7;
        if (Input.GetKeyDown(ModInputManager.showOptionPageInput.keyCode)) HudManagerUpdate.ToggleSettings(HudManager.Instance);
        if (Main.optionsPage >= GameOptionsDataPatch.maxPage) Main.optionsPage = 0;

        if (page != Main.optionsPage)
        {
            var position = (Vector3)FastDestroyableSingleton<HudManager>.Instance?.GameSettings?.transform.localPosition;
            FastDestroyableSingleton<HudManager>.Instance.GameSettings.transform.localPosition = new Vector3(position.x, 2.9f, position.z);
        }

        if (Input.GetKeyDown(ModInputManager.helpInput.keyCode))
        {
            var info = RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer, false, false).FirstOrDefault();
            if (LobbyRoleInfo.RolesSummaryUI == null)
            {
                if (InGame && info != null) LobbyRoleInfo.AddInfoCard(info);
                else LobbyRoleInfo.RoleSummaryOnClick();
            }
            else
            {
                UObject.Destroy(LobbyRoleInfo.RolesSummaryUI);
                LobbyRoleInfo.RolesSummaryUI = null;
            }
        }
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public class GameSettingsScalePatch
{
    public static void Prefix(HudManager __instance)
    {
        if (__instance.GameSettings != null) __instance.GameSettings.fontSize = 1.2f;
    }
}

// This class is taken and adapted from Town of Us Reactivated, https://github.com/eDonnes124/Town-Of-Us-R/blob/master/source/Patches/CustomOption/Patches.cs, Licensed under GPLv3
[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public class HudManagerUpdate
{
    public static float
        MinX, /*-5.3F*/
        OriginalY = 2.9F,
        MinY = 2.9F;


    public static Scroller Scroller;
    private static Vector3 LastPosition;
    private static float lastAspect;
    private static bool setLastPosition;

    private static readonly TextMeshPro[] settingsTMPs = new TextMeshPro[4];
    private static GameObject settingsBackground;

    private static PassiveButton toggleSettingsButton;
    private static GameObject toggleSettingsButtonObject;

    public static void Prefix(HudManager __instance)
    {
        if (__instance.GameSettings?.transform == null) return;

        // Sets the MinX position to the left edge of the screen + 0.1 units
        var safeArea = Screen.safeArea;
        var aspect = Mathf.Min(Camera.main.aspect, safeArea.width / safeArea.height);
        var safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
        MinX = 0.1f - (safeOrthographicSize * aspect);

        if (!setLastPosition || aspect != lastAspect)
        {
            LastPosition = new Vector3(MinX, MinY);
            lastAspect = aspect;
            setLastPosition = true;
            if (Scroller != null) Scroller.ContentXBounds = new FloatRange(MinX, MinX);
        }

        CreateScroller(__instance);

        Scroller.gameObject.SetActive(__instance.GameSettings.gameObject.activeSelf);

        if (!Scroller.gameObject.active) return;

        var rows = __instance.GameSettings.text.Count(c => c == '\n');
        var LobbyTextRowHeight = 0.12F;
        var maxY = Mathf.Max(MinY, (rows * LobbyTextRowHeight) + ((rows - 38) * LobbyTextRowHeight));

        Scroller.ContentYBounds = new FloatRange(MinY, maxY);

        // Prevent scrolling when the player is interacting with a menu
        if (PlayerControl.LocalPlayer?.CanMove != true)
        {
            __instance.GameSettings.transform.localPosition = LastPosition;

            return;
        }

        if (__instance.GameSettings.transform.localPosition.x != MinX ||
            __instance.GameSettings.transform.localPosition.y < MinY) return;

        LastPosition = __instance.GameSettings.transform.localPosition;
    }

    private static void CreateScroller(HudManager __instance)
    {
        if (Scroller != null) return;

        var target = __instance.GameSettings.transform;

        Scroller = new GameObject("SettingsScroller").AddComponent<Scroller>();
        Scroller.transform.SetParent(__instance.GameSettings.transform.parent);
        Scroller.gameObject.layer = 5;

        Scroller.transform.localScale = Vector3.one;
        Scroller.allowX = false;
        Scroller.allowY = true;
        Scroller.active = true;
        Scroller.velocity = new Vector2(0, 0);
        Scroller.ScrollbarYBounds = new FloatRange(0, 0);
        Scroller.ContentXBounds = new FloatRange(MinX, MinX);
        Scroller.enabled = true;

        Scroller.Inner = target;
        target.SetParent(Scroller.transform);
    }

    [HarmonyPrefix]
    public static void Prefix2(HudManager __instance)
    {
        if (!settingsTMPs[0]) return;
        foreach (var tmp in settingsTMPs) tmp.text = "";
        var settingsString = GameOptionsDataPatch.buildAllOptions(hideExtras: true);
        var blocks = settingsString.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        var curString = "";
        string curBlock;
        var j = 0;

        for (var i = 0; i < blocks.Length; i++)
        {
            if (IsCN()) blocks[i] = $"<line-height=120%>{blocks[i]}</line-height>";
            curBlock = blocks[i];
            if (lineCount(curBlock) + lineCount(curString) < (IsCN() ? 40 : 43))
            {
                curString += curBlock + "\n\n";
            }
            else
            {
                if (j < settingsTMPs.Length)
                    settingsTMPs[j].text = curString;
                j++;

                curString = "\n" + curBlock + "\n\n";
                if (curString.Substring(0, 2) != "\n\n") curString = "\n" + curString;
            }
        }

        if (j < settingsTMPs.Length)
            settingsTMPs[j].text = curString;
        var blockCount = 0;
        foreach (var tmp in settingsTMPs)
            if (tmp.text != "")
                blockCount++;
        for (var i = 0; i < blockCount; i++)
            settingsTMPs[i].transform.localPosition = new Vector3((-blockCount * 1.2f) + (2.7f * i), 2.2f, -500f);
    }

    public static void OpenSettings(HudManager __instance)
    {
        if (__instance.FullScreen == null || (MapBehaviour.Instance && MapBehaviour.Instance.IsOpen)
                                          /*|| AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started*/
                                          || GameOptionsManager.Instance.currentGameOptions.GameMode ==
                                          GameModes.HideNSeek) return;
        settingsBackground = UObject.Instantiate(__instance.FullScreen.gameObject, __instance.transform);
        settingsBackground.SetActive(true);
        var renderer = settingsBackground.GetComponent<SpriteRenderer>();
        renderer.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        renderer.enabled = true;

        for (var i = 0; i < settingsTMPs.Length; i++)
        {
            settingsTMPs[i] = UObject.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
            settingsTMPs[i].alignment = TextAlignmentOptions.TopLeft;
            settingsTMPs[i].enableWordWrapping = false;
            settingsTMPs[i].transform.localScale = Vector3.one * 0.25f;
            settingsTMPs[i].gameObject.SetActive(true);
        }
    }

    public static void CloseSettings()
    {
        foreach (var tmp in settingsTMPs)
            if (tmp)
                tmp.gameObject.Destroy();

        if (settingsBackground) settingsBackground.Destroy();
    }

    public static void ToggleSettings(HudManager __instance)
    {
        if (settingsTMPs[0]) CloseSettings();
        else OpenSettings(__instance);
    }

    public static void Postfix(HudManager __instance)
    {
        if (!toggleSettingsButton || !toggleSettingsButtonObject)
        {
            // add a special button for settings viewing:
            toggleSettingsButtonObject = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
            toggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -0.66f, -500f);
            var renderer = toggleSettingsButtonObject.GetComponent<SpriteRenderer>();
            renderer.sprite = new ResourceSprite("TheOtherRoles.Resources.CurrentSettingsButton.png", 180f);
            toggleSettingsButton = toggleSettingsButtonObject.GetComponent<PassiveButton>();
            toggleSettingsButton.OnClick.RemoveAllListeners();
            toggleSettingsButton.OnClick.AddListener((Action)(() => ToggleSettings(__instance)));
            _ = CustomButton.SetKeyGuideOnSmallButton(toggleSettingsButtonObject, ModInputManager.showOptionPageInput.keyCode);
        }

        toggleSettingsButtonObject.SetActive(__instance.MapButton.gameObject.active && !IsHideNSeek && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen));
        toggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -0.66f, -500f);
    }

    private static PassiveButton rolesSummaryButton;
    private static GameObject rolesSummaryButtonObject;

    [HarmonyPostfix]
    public static void Postfix2(HudManager __instance)
    {
        if (!rolesSummaryButton || !rolesSummaryButtonObject)
        {
            // add a special button for settings viewing:
            rolesSummaryButtonObject = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
            rolesSummaryButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.31f, -500f);
            var renderer = rolesSummaryButtonObject.GetComponent<SpriteRenderer>();
            renderer.sprite = new ResourceSprite("TheOtherRoles.Resources.HelpButton.png", 100f);
            rolesSummaryButton = rolesSummaryButtonObject.GetComponent<PassiveButton>();
            rolesSummaryButton.OnClick.RemoveAllListeners();
            rolesSummaryButton.OnClick.AddListener((Action)(() =>
            {
                LobbyRoleInfo.RoleSummaryOnClick();
            }));
            _ = CustomButton.SetKeyGuideOnSmallButton(rolesSummaryButtonObject, ModInputManager.helpInput.keyCode);
        }

        rolesSummaryButtonObject.SetActive(__instance.MapButton.gameObject.active && !IsHideNSeek && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen));
        rolesSummaryButtonObject.transform.localPosition = __instance.MapButton.transform.transform.localPosition + new Vector3(0, -1.31f, -500f);
    }
}