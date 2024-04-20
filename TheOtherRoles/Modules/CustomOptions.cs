using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AmongUs.GameOptions;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Hazel;
using Reactor.Utilities.Extensions;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TheOtherRoles.CustomOption;
using static TheOtherRoles.TheOtherRoles;
using Object = UnityEngine.Object;

namespace TheOtherRoles;

public class CustomOption
{
    public enum CustomOptionType
    {
        General,
        Impostor,
        Neutral,
        Crewmate,
        Modifier,
        Guesser,
        HideNSeekMain,
        HideNSeekRoles,
        PropHunt
    }

    public static List<CustomOption> options = new();
    public static int preset;
    public static ConfigEntry<string> vanillaSettings;

    public int defaultSelection;
    public ConfigEntry<int> entry;

    public int id;
    public bool isHeader;
    public string name;
    public Action onChange;
    public OptionBehaviour optionBehaviour;
    public CustomOption parent;
    public int selection;
    public object[] selections;
    public CustomOptionType type;

    // Option creation

    public CustomOption(int id, CustomOptionType type, string name, object[] selections, object defaultValue,
        CustomOption parent, bool isHeader, Action onChange = null)
    {
        this.id = id;
        this.name = parent == null ? name : "- " + name;
        this.selections = selections;
        var index = Array.IndexOf(selections, defaultValue);
        defaultSelection = index >= 0 ? index : 0;
        this.parent = parent;
        this.isHeader = isHeader;
        this.type = type;
        this.onChange = onChange;
        selection = 0;
        if (id != 0)
        {
            entry = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", id.ToString(), defaultSelection);
            selection = Mathf.Clamp(entry.Value, 0, selections.Length - 1);
        }

        options.Add(this);
    }

    public static CustomOption Create(int id, CustomOptionType type, string name, string[] selections,
        CustomOption parent = null, bool isHeader = false, Action onChange = null)
    {
        return new CustomOption(id, type, name, selections, "", parent, isHeader, onChange);
    }

    public static CustomOption Create(int id, CustomOptionType type, string name, float defaultValue, float min,
        float max, float step, CustomOption parent = null, bool isHeader = false, Action onChange = null)
    {
        List<object> selections = new();
        for (var s = min; s <= max; s += step)
            selections.Add(s);
        return new CustomOption(id, type, name, selections.ToArray(), defaultValue, parent, isHeader, onChange);
    }

    public static CustomOption Create(int id, CustomOptionType type, string name, bool defaultValue,
        CustomOption parent = null, bool isHeader = false, Action onChange = null)
    {
        return new CustomOption(id, type, name, new[] { ModTranslation.getString("OFF"), ModTranslation.getString("ON") }, defaultValue ? ModTranslation.getString("ON") : ModTranslation.getString("OFF"), parent, isHeader,
            onChange);
    }

    // Static behaviour

    public static void switchPreset(int newPreset)
    {
        saveVanillaOptions();
        preset = newPreset;
        vanillaSettings = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", "GameOptions", "");
        loadVanillaOptions();
        foreach (var option in options)
        {
            if (option.id == 0) continue;

            option.entry =
                TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", option.id.ToString(),
                    option.defaultSelection);
            option.selection = Mathf.Clamp(option.entry.Value, 0, option.selections.Length - 1);
            if (option.optionBehaviour != null && option.optionBehaviour is StringOption stringOption)
            {
                stringOption.oldValue = stringOption.Value = option.selection;
                stringOption.ValueText.text = option.selections[option.selection].ToString();
            }
        }
    }

    public static void saveVanillaOptions()
    {
        vanillaSettings.Value =
            Convert.ToBase64String(
                GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameManager.Instance.LogicOptions.currentGameOptions, false));
    }

    public static void loadVanillaOptions()
    {
        var optionsString = vanillaSettings.Value;
        if (optionsString == "") return;
        GameOptionsManager.Instance.GameHostOptions =
            GameOptionsManager.Instance.gameOptionsFactory.FromBytes(Convert.FromBase64String(optionsString));
        GameOptionsManager.Instance.CurrentGameOptions = GameOptionsManager.Instance.GameHostOptions;
        GameManager.Instance.LogicOptions.SetGameOptions(GameOptionsManager.Instance.CurrentGameOptions);
        GameManager.Instance.LogicOptions.SyncOptions();
    }

    public static void ShareOptionChange(uint optionId)
    {
        var option = options.FirstOrDefault(x => x.id == optionId);
        if (option == null) return;
        var writer = AmongUsClient.Instance!.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
            (byte)CustomRPC.ShareOptions, SendOption.Reliable);
        writer.Write((byte)1);
        writer.WritePacked((uint)option.id);
        writer.WritePacked(Convert.ToUInt32(option.selection));
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public static void ShareOptionSelections()
    {
        if (CachedPlayer.AllPlayers.Count <= 1 ||
            (AmongUsClient.Instance!.AmHost == false && CachedPlayer.LocalPlayer.PlayerControl == null)) return;
        var optionsList = new List<CustomOption>(options);
        while (optionsList.Any())
        {
            var amount = (byte)Math.Min(optionsList.Count, 200); // takes less than 3 bytes per option on average
            var writer = AmongUsClient.Instance!.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.ShareOptions, SendOption.Reliable);
            writer.Write(amount);
            for (var i = 0; i < amount; i++)
            {
                var option = optionsList[0];
                optionsList.RemoveAt(0);
                writer.WritePacked((uint)option.id);
                writer.WritePacked(Convert.ToUInt32(option.selection));
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }

    // Getter

    public int getSelection()
    {
        return selection;
    }

    public bool getBool()
    {
        return selection > 0;
    }

    public float getFloat()
    {
        return (float)selections[selection];
    }

    public int GetInt()
    {
        return (int)getFloat();
    }

    public int getQuantity()
    {
        return selection + 1;
    }

    // Option changes

    public void updateSelection(int newSelection)
    {
        selection = Mathf.Clamp((newSelection + selections.Length) % selections.Length, 0, selections.Length - 1);
        try
        {
            if (onChange != null) onChange();
        }
        catch
        {
            // ignored
        }

        if (optionBehaviour != null && optionBehaviour is StringOption stringOption)
        {
            stringOption.oldValue = stringOption.Value = selection;
            stringOption.ValueText.text = selections[selection].ToString();
            if (AmongUsClient.Instance?.AmHost != true || !CachedPlayer.LocalPlayer.PlayerControl) return;
            if (id == 0 && selection != preset)
            {
                switchPreset(selection); // Switch presets
                ShareOptionSelections();
            }
            else if (entry != null)
            {
                entry.Value = selection; // Save selection to config
                ShareOptionChange((uint)id); // Share single selection
            }
        }
        else if (id == 0 && AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer)
        {
            // Share the preset switch for random maps, even if the menu isnt open!
            switchPreset(selection);
            ShareOptionSelections(); // Share all selections
        }
    }

    public static byte[] serializeOptions()
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                var lastId = -1;
                foreach (var option in options.OrderBy(x => x.id))
                {
                    if (option.id == 0) continue;
                    var consecutive = lastId + 1 == option.id;
                    lastId = option.id;

                    binaryWriter.Write((byte)(option.selection + (consecutive ? 128 : 0)));
                    if (!consecutive) binaryWriter.Write((ushort)option.id);
                }

                binaryWriter.Flush();
                memoryStream.Position = 0L;
                return memoryStream.ToArray();
            }
        }
    }

    public static void deserializeOptions(byte[] inputValues)
    {
        var reader = new BinaryReader(new MemoryStream(inputValues));
        var lastId = -1;
        while (reader.BaseStream.Position < inputValues.Length)
            try
            {
                int selection = reader.ReadByte();
                var id = -1;
                var consecutive = selection >= 128;
                if (consecutive)
                {
                    selection -= 128;
                    id = lastId + 1;
                }
                else
                {
                    id = reader.ReadUInt16();
                }

                if (id == 0) continue;
                lastId = id;
                var option = options.First(option => option.id == id);
                option.updateSelection(selection);
            }
            catch (Exception e)
            {
                Warn($"{e}: 反序列化时 - 试图粘贴无效设置！");
            }
    }

    // Copy to or paste from clipboard (as string)
    public static void copyToClipboard()
    {
        GUIUtility.systemCopyBuffer =
            $"{TheOtherRolesPlugin.VersionString}!{Convert.ToBase64String(serializeOptions())}!{vanillaSettings.Value}";
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

            vanillaSettings.Value = vanillaSettingsSub;
            loadVanillaOptions();
            return true;
        }
        catch (Exception e)
        {
            Warn($"{e}: 尝试粘贴无效设置！");
            SoundEffectsManager.Load();
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
        switch (TORMapOptions.gameMode)
        {
            case CustomGamemodes.Classic:
                createClassicTabs(__instance);
                break;
            case CustomGamemodes.Guesser:
                createGuesserTabs(__instance);
                break;
            case CustomGamemodes.HideNSeek:
                createHideNSeekTabs(__instance);
                break;
            case CustomGamemodes.PropHunt:
                createPropHuntTabs(__instance);
                break;
        }

        // create copy to clipboard and paste from clipboard buttons.
        var template = GameObject.Find("CloseButton");
        var copyButton = Object.Instantiate(template, template.transform.parent);
        copyButton.transform.localPosition += Vector3.down * 0.8f;
        var copyButtonPassive = copyButton.GetComponent<PassiveButton>();
        var copyButtonRenderer = copyButton.GetComponent<SpriteRenderer>();
        copyButtonRenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CopyButton.png", 175f);
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
        var pasteButton = Object.Instantiate(template, template.transform.parent);
        pasteButton.transform.localPosition += Vector3.down * 1.6f;
        var pasteButtonPassive = pasteButton.GetComponent<PassiveButton>();
        var pasteButtonRenderer = pasteButton.GetComponent<SpriteRenderer>();
        pasteButtonRenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PasteButton.png", 175f);
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
        var isReturn = setNames(
            new Dictionary<string, string>
            {
                ["TORSettings"] = "模组设置",
                ["ImpostorSettings"] = "伪装者职业设置",
                ["NeutralSettings"] = "中立职业设置",
                ["CrewmateSettings"] = "船员职业设置",
                ["ModifierSettings"] = "附加职业设置"
            });

        if (isReturn) return;

        // Setup TOR tab
        var template = Object.FindObjectsOfType<StringOption>().FirstOrDefault();
        if (template == null) return;
        var gameSettings = GameObject.Find("Game Settings");
        var gameSettingMenu = Object.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();

        var torSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var torMenu = getMenu(torSettings, "TORSettings");

        var impostorSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var impostorMenu = getMenu(impostorSettings, "ImpostorSettings");

        var neutralSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var neutralMenu = getMenu(neutralSettings, "NeutralSettings");

        var crewmateSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var crewmateMenu = getMenu(crewmateSettings, "CrewmateSettings");

        var modifierSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var modifierMenu = getMenu(modifierSettings, "ModifierSettings");

        var roleTab = GameObject.Find("RoleTab");
        var gameTab = GameObject.Find("GameTab");

        var torTab = Object.Instantiate(roleTab, roleTab.transform.parent);
        var torTabHighlight = getTabHighlight(torTab, "TheOtherRolesTab", "TheOtherRoles.Resources.TabIcon.png");

        var impostorTab = Object.Instantiate(roleTab, torTab.transform);
        var impostorTabHighlight =
            getTabHighlight(impostorTab, "ImpostorTab", "TheOtherRoles.Resources.TabIconImpostor.png");

        var neutralTab = Object.Instantiate(roleTab, impostorTab.transform);
        var neutralTabHighlight =
            getTabHighlight(neutralTab, "NeutralTab", "TheOtherRoles.Resources.TabIconNeutral.png");

        var crewmateTab = Object.Instantiate(roleTab, neutralTab.transform);
        var crewmateTabHighlight =
            getTabHighlight(crewmateTab, "CrewmateTab", "TheOtherRoles.Resources.TabIconCrewmate.png");

        var modifierTab = Object.Instantiate(roleTab, crewmateTab.transform);
        var modifierTabHighlight =
            getTabHighlight(modifierTab, "ModifierTab", "TheOtherRoles.Resources.TabIconModifier.png");

        // Position of Tab Icons
        gameTab.transform.position += Vector3.left * 3f;
        roleTab.transform.position += Vector3.left * 3f;
        torTab.transform.position += Vector3.left * 2f;
        impostorTab.transform.localPosition = Vector3.right * 1f;
        neutralTab.transform.localPosition = Vector3.right * 1f;
        crewmateTab.transform.localPosition = Vector3.right * 1f;
        modifierTab.transform.localPosition = Vector3.right * 1f;

        var tabs = new[] { gameTab, roleTab, torTab, impostorTab, neutralTab, crewmateTab, modifierTab };
        var settingsHighlightMap = new Dictionary<GameObject, SpriteRenderer>
        {
            [gameSettingMenu.RegularGameSettings] = gameSettingMenu.GameSettingsHightlight,
            [gameSettingMenu.RolesSettings.gameObject] = gameSettingMenu.RolesSettingsHightlight,
            [torSettings.gameObject] = torTabHighlight,
            [impostorSettings.gameObject] = impostorTabHighlight,
            [neutralSettings.gameObject] = neutralTabHighlight,
            [crewmateSettings.gameObject] = crewmateTabHighlight,
            [modifierSettings.gameObject] = modifierTabHighlight
        };
        for (var i = 0; i < tabs.Length; i++)
        {
            var button = tabs[i].GetComponentInChildren<PassiveButton>();
            if (button == null) continue;
            var copiedIndex = i;
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((Action)(() => { setListener(settingsHighlightMap, copiedIndex); }));
        }

        destroyOptions(new List<List<OptionBehaviour>>
        {
            torMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            impostorMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            neutralMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            crewmateMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            modifierMenu.GetComponentsInChildren<OptionBehaviour>().ToList()
        });

        var torOptions = new List<OptionBehaviour>();
        var impostorOptions = new List<OptionBehaviour>();
        var neutralOptions = new List<OptionBehaviour>();
        var crewmateOptions = new List<OptionBehaviour>();
        var modifierOptions = new List<OptionBehaviour>();


        var menus = new List<Transform>
        {
            torMenu.transform, impostorMenu.transform, neutralMenu.transform, crewmateMenu.transform,
            modifierMenu.transform
        };
        var optionBehaviours = new List<List<OptionBehaviour>>
            { torOptions, impostorOptions, neutralOptions, crewmateOptions, modifierOptions };

        for (var i = 0; i < options.Count; i++)
        {
            var option = options[i];
            if ((int)option.type > 4) continue;
            if (option.optionBehaviour == null)
            {
                var stringOption = Object.Instantiate(template, menus[(int)option.type]);
                optionBehaviours[(int)option.type].Add(stringOption);
                stringOption.OnValueChanged = new Action<OptionBehaviour>(o => { });
                stringOption.TitleText.text = option.name;
                stringOption.Value = stringOption.oldValue = option.selection;
                stringOption.ValueText.text = option.selections[option.selection].ToString();

                option.optionBehaviour = stringOption;
            }

            option.optionBehaviour.gameObject.SetActive(true);
        }

        setOptions(
            new List<GameOptionsMenu> { torMenu, impostorMenu, neutralMenu, crewmateMenu, modifierMenu },
            new List<List<OptionBehaviour>>
                { torOptions, impostorOptions, neutralOptions, crewmateOptions, modifierOptions },
            new List<GameObject> { torSettings, impostorSettings, neutralSettings, crewmateSettings, modifierSettings }
        );

        adaptTaskCount(__instance);
    }

    private static void createGuesserTabs(GameOptionsMenu __instance)
    {
        var isReturn = setNames(
            new Dictionary<string, string>
            {
                ["TORSettings"] = "模组设置",
                ["GuesserSettings"] = "赌怪模式设置",
                ["ImpostorSettings"] = "伪装者职业设置",
                ["NeutralSettings"] = "中立职业设置",
                ["CrewmateSettings"] = "船员职业设置",
                ["ModifierSettings"] = "附加职业设置"
            });

        if (isReturn) return;

        // Setup TOR tab
        var template = Object.FindObjectsOfType<StringOption>().FirstOrDefault();
        if (template == null) return;
        var gameSettings = GameObject.Find("Game Settings");
        var gameSettingMenu = Object.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();

        var torSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var torMenu = getMenu(torSettings, "TORSettings");

        var guesserSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var guesserMenu = getMenu(guesserSettings, "GuesserSettings");

        var impostorSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var impostorMenu = getMenu(impostorSettings, "ImpostorSettings");

        var neutralSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var neutralMenu = getMenu(neutralSettings, "NeutralSettings");

        var crewmateSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var crewmateMenu = getMenu(crewmateSettings, "CrewmateSettings");

        var modifierSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var modifierMenu = getMenu(modifierSettings, "ModifierSettings");

        var roleTab = GameObject.Find("RoleTab");
        var gameTab = GameObject.Find("GameTab");

        var torTab = Object.Instantiate(roleTab, gameTab.transform.parent);
        var torTabHighlight = getTabHighlight(torTab, "TheOtherRolesTab", "TheOtherRoles.Resources.TabIcon.png");

        var guesserTab = Object.Instantiate(roleTab, torTab.transform);
        var guesserTabHighlight =
            getTabHighlight(guesserTab, "GuesserTab", "TheOtherRoles.Resources.TabIconGuesserSettings.png");

        var impostorTab = Object.Instantiate(roleTab, guesserTab.transform);
        var impostorTabHighlight =
            getTabHighlight(impostorTab, "ImpostorTab", "TheOtherRoles.Resources.TabIconImpostor.png");

        var neutralTab = Object.Instantiate(roleTab, impostorTab.transform);
        var neutralTabHighlight =
            getTabHighlight(neutralTab, "NeutralTab", "TheOtherRoles.Resources.TabIconNeutral.png");

        var crewmateTab = Object.Instantiate(roleTab, neutralTab.transform);
        var crewmateTabHighlight =
            getTabHighlight(crewmateTab, "CrewmateTab", "TheOtherRoles.Resources.TabIconCrewmate.png");

        var modifierTab = Object.Instantiate(roleTab, crewmateTab.transform);
        var modifierTabHighlight =
            getTabHighlight(modifierTab, "ModifierTab", "TheOtherRoles.Resources.TabIconModifier.png");

        roleTab.active = false;
        // Position of Tab Icons
        gameTab.transform.position += Vector3.left * 3f;
        torTab.transform.position += Vector3.left * 3f;
        guesserTab.transform.localPosition = Vector3.right * 1f;
        impostorTab.transform.localPosition = Vector3.right * 1f;
        neutralTab.transform.localPosition = Vector3.right * 1f;
        crewmateTab.transform.localPosition = Vector3.right * 1f;
        modifierTab.transform.localPosition = Vector3.right * 1f;

        var tabs = new[] { gameTab, torTab, impostorTab, neutralTab, crewmateTab, modifierTab, guesserTab };
        var settingsHighlightMap = new Dictionary<GameObject, SpriteRenderer>
        {
            [gameSettingMenu.RegularGameSettings] = gameSettingMenu.GameSettingsHightlight,
            [torSettings.gameObject] = torTabHighlight,
            [impostorSettings.gameObject] = impostorTabHighlight,
            [neutralSettings.gameObject] = neutralTabHighlight,
            [crewmateSettings.gameObject] = crewmateTabHighlight,
            [modifierSettings.gameObject] = modifierTabHighlight,
            [guesserSettings.gameObject] = guesserTabHighlight
        };
        for (var i = 0; i < tabs.Length; i++)
        {
            var button = tabs[i].GetComponentInChildren<PassiveButton>();
            if (button == null) continue;
            var copiedIndex = i;
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((Action)(() => { setListener(settingsHighlightMap, copiedIndex); }));
        }

        destroyOptions(new List<List<OptionBehaviour>>
        {
            torMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            guesserMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            impostorMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            neutralMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            crewmateMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            modifierMenu.GetComponentsInChildren<OptionBehaviour>().ToList()
        });

        var torOptions = new List<OptionBehaviour>();
        var guesserOptions = new List<OptionBehaviour>();
        var impostorOptions = new List<OptionBehaviour>();
        var neutralOptions = new List<OptionBehaviour>();
        var crewmateOptions = new List<OptionBehaviour>();
        var modifierOptions = new List<OptionBehaviour>();


        var menus = new List<Transform>
        {
            torMenu.transform, impostorMenu.transform, neutralMenu.transform, crewmateMenu.transform,
            modifierMenu.transform, guesserMenu.transform
        };
        var optionBehaviours = new List<List<OptionBehaviour>>
            { torOptions, impostorOptions, neutralOptions, crewmateOptions, modifierOptions, guesserOptions };
        var exludedIds = new List<int> { 310, 311, 312, 313, 314, 315, 316, 317, 318 };

        for (var i = 0; i < options.Count; i++)
        {
            var option = options[i];
            if (exludedIds.Contains(option.id)) continue;
            if ((int)option.type > 5) continue;
            if (option.optionBehaviour == null)
            {
                var stringOption = Object.Instantiate(template, menus[(int)option.type]);
                optionBehaviours[(int)option.type].Add(stringOption);
                stringOption.OnValueChanged = new Action<OptionBehaviour>(o => { });
                stringOption.TitleText.text = option.name;
                stringOption.Value = stringOption.oldValue = option.selection;
                stringOption.ValueText.text = option.selections[option.selection].ToString();

                option.optionBehaviour = stringOption;
            }

            option.optionBehaviour.gameObject.SetActive(true);
        }

        setOptions(
            new List<GameOptionsMenu> { torMenu, impostorMenu, neutralMenu, crewmateMenu, modifierMenu, guesserMenu },
            new List<List<OptionBehaviour>>
                { torOptions, impostorOptions, neutralOptions, crewmateOptions, modifierOptions, guesserOptions },
            new List<GameObject>
                { torSettings, impostorSettings, neutralSettings, crewmateSettings, modifierSettings, guesserSettings }
        );

        adaptTaskCount(__instance);
    }

    private static void createHideNSeekTabs(GameOptionsMenu __instance)
    {
        var isReturn = setNames(
            new Dictionary<string, string>
            {
                ["TORSettings"] = "模组设置",
                ["HideNSeekSettings"] = "躲猫猫模式设置"
            });

        if (isReturn) return;

        // Setup TOR tab
        var template = Object.FindObjectsOfType<StringOption>().FirstOrDefault();
        if (template == null) return;
        var gameSettings = GameObject.Find("Game Settings");
        var gameSettingMenu = Object.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();

        var torSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var torMenu = getMenu(torSettings, "TORSettings");

        var hideNSeekSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var hideNSeekMenu = getMenu(hideNSeekSettings, "HideNSeekSettings");

        var roleTab = GameObject.Find("RoleTab");
        var gameTab = GameObject.Find("GameTab");

        var torTab = Object.Instantiate(roleTab, gameTab.transform.parent);
        var torTabHighlight = getTabHighlight(torTab, "TheOtherRolesTab",
            "TheOtherRoles.Resources.TabIconHideNSeekSettings.png");

        var hideNSeekTab = Object.Instantiate(roleTab, torTab.transform);
        var hideNSeekTabHighlight = getTabHighlight(hideNSeekTab, "HideNSeekTab",
            "TheOtherRoles.Resources.TabIconHideNSeekRoles.png");

        roleTab.active = false;
        gameTab.active = false;

        // Position of Tab Icons
        torTab.transform.position += Vector3.left * 3f;
        hideNSeekTab.transform.position += Vector3.right * 1f;

        var tabs = new[] { torTab, hideNSeekTab };
        var settingsHighlightMap = new Dictionary<GameObject, SpriteRenderer>
        {
            [torSettings.gameObject] = torTabHighlight,
            [hideNSeekSettings.gameObject] = hideNSeekTabHighlight
        };
        for (var i = 0; i < tabs.Length; i++)
        {
            var button = tabs[i].GetComponentInChildren<PassiveButton>();
            if (button == null) continue;
            var copiedIndex = i;
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((Action)(() => { setListener(settingsHighlightMap, copiedIndex); }));
        }

        destroyOptions(new List<List<OptionBehaviour>>
        {
            torMenu.GetComponentsInChildren<OptionBehaviour>().ToList(),
            hideNSeekMenu.GetComponentsInChildren<OptionBehaviour>().ToList()
        });

        var torOptions = new List<OptionBehaviour>();
        var hideNSeekOptions = new List<OptionBehaviour>();

        var menus = new List<Transform> { torMenu.transform, hideNSeekMenu.transform };
        var optionBehaviours = new List<List<OptionBehaviour>> { torOptions, hideNSeekOptions };

        for (var i = 0; i < options.Count; i++)
        {
            var option = options[i];
            if (option.type != CustomOptionType.HideNSeekMain &&
                option.type != CustomOptionType.HideNSeekRoles) continue;
            if (option.optionBehaviour == null)
            {
                var index = (int)option.type - 6;
                var stringOption = Object.Instantiate(template, menus[index]);
                optionBehaviours[index].Add(stringOption);
                stringOption.OnValueChanged = new Action<OptionBehaviour>(o => { });
                stringOption.TitleText.text = option.name;
                stringOption.Value = stringOption.oldValue = option.selection;
                stringOption.ValueText.text = option.selections[option.selection].ToString();

                option.optionBehaviour = stringOption;
            }

            option.optionBehaviour.gameObject.SetActive(true);
        }

        setOptions(
            new List<GameOptionsMenu> { torMenu, hideNSeekMenu },
            new List<List<OptionBehaviour>> { torOptions, hideNSeekOptions },
            new List<GameObject> { torSettings, hideNSeekSettings }
        );

        torSettings.gameObject.SetActive(true);
        torTabHighlight.enabled = true;
        gameSettingMenu.RegularGameSettings.SetActive(false);
        gameSettingMenu.GameSettingsHightlight.enabled = false;
    }


    private static void createPropHuntTabs(GameOptionsMenu __instance)
    {
        var isReturn = setNames(
            new Dictionary<string, string>
            {
                ["TORSettings"] = "道具躲猫猫模式设置"
            });

        if (isReturn) return;

        // Setup TOR tab
        var template = Object.FindObjectsOfType<StringOption>().FirstOrDefault();
        if (template == null) return;
        var gameSettings = GameObject.Find("Game Settings");
        var gameSettingMenu = Object.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();

        var torSettings = Object.Instantiate(gameSettings, gameSettings.transform.parent);
        var torMenu = getMenu(torSettings, "TORSettings");
        var roleTab = GameObject.Find("RoleTab");
        var gameTab = GameObject.Find("GameTab");

        var torTab = Object.Instantiate(roleTab, gameTab.transform.parent);
        var torTabHighlight = getTabHighlight(torTab, "TheOtherRolesTab",
            "TheOtherRoles.Resources.TabIconPropHuntSettings.png");

        roleTab.active = false;
        gameTab.active = false;

        // Position of Tab Icons
        torTab.transform.position += Vector3.left * 3f;

        var tabs = new[] { torTab };
        var settingsHighlightMap = new Dictionary<GameObject, SpriteRenderer>
        {
            [torSettings.gameObject] = torTabHighlight
        };
        for (var i = 0; i < tabs.Length; i++)
        {
            var button = tabs[i].GetComponentInChildren<PassiveButton>();
            if (button == null) continue;
            var copiedIndex = i;
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((Action)(() => { setListener(settingsHighlightMap, copiedIndex); }));
        }

        destroyOptions(new List<List<OptionBehaviour>>
        {
            torMenu.GetComponentsInChildren<OptionBehaviour>().ToList()
        });

        var torOptions = new List<OptionBehaviour>();

        var menus = new List<Transform> { torMenu.transform };
        var optionBehaviours = new List<List<OptionBehaviour>> { torOptions };

        for (var i = 0; i < options.Count; i++)
        {
            var option = options[i];
            if (option.type != CustomOptionType.PropHunt) continue;
            if (option.optionBehaviour == null)
            {
                var index = 0;
                var stringOption = Object.Instantiate(template, menus[index]);
                optionBehaviours[index].Add(stringOption);
                stringOption.OnValueChanged = new Action<OptionBehaviour>(o => { });
                stringOption.TitleText.text = option.name;
                stringOption.Value = stringOption.oldValue = option.selection;
                stringOption.ValueText.text = option.selections[option.selection].ToString();

                option.optionBehaviour = stringOption;
            }

            option.optionBehaviour.gameObject.SetActive(true);
        }

        setOptions(
            new List<GameOptionsMenu> { torMenu },
            new List<List<OptionBehaviour>> { torOptions },
            new List<GameObject> { torSettings }
        );

        torSettings.gameObject.SetActive(true);
        torTabHighlight.enabled = true;
        gameSettingMenu.RegularGameSettings.SetActive(false);
        gameSettingMenu.GameSettingsHightlight.enabled = false;
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

    private static void destroyOptions(List<List<OptionBehaviour>> optionBehavioursList)
    {
        foreach (var optionBehaviours in optionBehavioursList)
            foreach (var option in optionBehaviours)
                Object.Destroy(option.gameObject);
    }

    private static bool setNames(Dictionary<string, string> gameObjectNameDisplayNameMap)
    {
        foreach (var entry in gameObjectNameDisplayNameMap)
            if (GameObject.Find(entry.Key) != null)
            {
                // Settings setup has already been performed, fixing the title of the tab and returning
                GameObject.Find(entry.Key).transform.FindChild("GameGroup").FindChild("Text")
                    .GetComponent<TextMeshPro>().SetText(entry.Value);
                return true;
            }

        return false;
    }

    private static GameOptionsMenu getMenu(GameObject setting, string settingName)
    {
        var menu = setting.transform.FindChild("GameGroup").FindChild("SliderInner").GetComponent<GameOptionsMenu>();
        setting.name = settingName;

        return menu;
    }

    private static SpriteRenderer getTabHighlight(GameObject tab, string tabName, string tabSpritePath)
    {
        var tabHighlight = tab.transform.FindChild("Hat Button").FindChild("Tab Background")
            .GetComponent<SpriteRenderer>();
        tab.transform.FindChild("Hat Button").FindChild("Icon").GetComponent<SpriteRenderer>().sprite =
            Helpers.loadSpriteFromResources(tabSpritePath, 100f);
        tab.name = "tabName";

        return tabHighlight;
    }

    private static void setOptions(List<GameOptionsMenu> menus, List<List<OptionBehaviour>> options,
        List<GameObject> settings)
    {
        if (!(menus.Count == options.Count && options.Count == settings.Count))
        {
            Error("List counts are not equal");
            return;
        }

        for (var i = 0; i < menus.Count; i++)
        {
            menus[i].Children = options[i].ToArray();
            settings[i].gameObject.SetActive(false);
        }
    }

    private static void adaptTaskCount(GameOptionsMenu __instance)
    {
        // Adapt task count for main options
        var commonTasksOption =
            __instance.Children.FirstOrDefault(x => x.name == "NumCommonTasks").TryCast<NumberOption>();
        if (commonTasksOption != null) commonTasksOption.ValidRange = new FloatRange(0f, 4f);

        var shortTasksOption =
            __instance.Children.FirstOrDefault(x => x.name == "NumShortTasks").TryCast<NumberOption>();
        if (shortTasksOption != null) shortTasksOption.ValidRange = new FloatRange(0f, 23f);

        var longTasksOption = __instance.Children.FirstOrDefault(x => x.name == "NumLongTasks").TryCast<NumberOption>();
        if (longTasksOption != null) longTasksOption.ValidRange = new FloatRange(0f, 15f);
    }
}

[HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
public class StringOptionEnablePatch
{
    public static bool Prefix(StringOption __instance)
    {
        var option = options.FirstOrDefault(option => option.optionBehaviour == __instance);
        if (option == null) return true;

        __instance.OnValueChanged = new Action<OptionBehaviour>(o => { });
        __instance.TitleText.text = option.name;
        __instance.Value = __instance.oldValue = option.selection;
        __instance.ValueText.text = option.selections[option.selection].ToString();

        return false;
    }
}

[HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
public class StringOptionIncreasePatch
{
    public static bool Prefix(StringOption __instance)
    {
        var option = options.FirstOrDefault(option => option.optionBehaviour == __instance);
        if (option == null) return true;
        option.updateSelection(option.selection + 1);
        if (CustomOptionHolder.isMapSelectionOption(option))
        {
            var currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
            currentGameOptions.SetByte(ByteOptionNames.MapId, (byte)option.selection);
            GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.CurrentGameOptions;
            GameManager.Instance.LogicOptions.SyncOptions();
        }

        return false;
    }
}

[HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
public class StringOptionDecreasePatch
{
    public static bool Prefix(StringOption __instance)
    {
        var option = options.FirstOrDefault(option => option.optionBehaviour == __instance);
        if (option == null) return true;
        option.updateSelection(option.selection - 1);
        if (CustomOptionHolder.isMapSelectionOption(option))
        {
            var currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
            currentGameOptions.SetByte(ByteOptionNames.MapId, (byte)option.selection);
            GameOptionsManager.Instance.GameHostOptions = GameOptionsManager.Instance.CurrentGameOptions;
            GameManager.Instance.LogicOptions.SyncOptions();
        }

        return false;
    }
}

[HarmonyPatch(typeof(StringOption), nameof(StringOption.FixedUpdate))]
public class StringOptionFixedUpdate
{
    public static void Postfix(StringOption __instance)
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out var _)) return;
        var option = options.FirstOrDefault(option => option.optionBehaviour == __instance);
        if (option == null || !CustomOptionHolder.isMapSelectionOption(option)) return;
        if (GameOptionsManager.Instance.CurrentGameOptions.MapId == 6)
            if (option.optionBehaviour != null && option.optionBehaviour is StringOption stringOption)
            {
                stringOption.ValueText.text = option.selections[option.selection].ToString();
            }
            else if (option.optionBehaviour != null && option.optionBehaviour is StringOption stringOptionToo)
            {
                stringOptionToo.oldValue = stringOptionToo.Value = option.selection;
                stringOptionToo.ValueText.text = option.selections[option.selection].ToString();
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
    private static float timer = 1f;

    public static void Postfix(GameOptionsMenu __instance)
    {
        // Return Menu Update if in normal among us settings 
        var gameSettingMenu = Object.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();
        if (gameSettingMenu.RegularGameSettings.active || gameSettingMenu.RolesSettings.gameObject.active) return;

        __instance.GetComponentInParent<Scroller>().ContentYBounds.max = -0.5F + (__instance.Children.Length * 0.55F);
        timer += Time.deltaTime;
        if (timer < 0.1f) return;
        timer = 0f;

        var offset = 2.75f;
        foreach (var option in options)
        {
            if (GameObject.Find("TORSettings") && option.type != CustomOptionType.General &&
                option.type != CustomOptionType.HideNSeekMain && option.type != CustomOptionType.PropHunt)
                continue;
            if (GameObject.Find("ImpostorSettings") && option.type != CustomOptionType.Impostor)
                continue;
            if (GameObject.Find("NeutralSettings") && option.type != CustomOptionType.Neutral)
                continue;
            if (GameObject.Find("CrewmateSettings") && option.type != CustomOptionType.Crewmate)
                continue;
            if (GameObject.Find("ModifierSettings") && option.type != CustomOptionType.Modifier)
                continue;
            if (GameObject.Find("GuesserSettings") && option.type != CustomOptionType.Guesser)
                continue;
            if (GameObject.Find("HideNSeekSettings") && option.type != CustomOptionType.HideNSeekRoles)
                continue;
            if (option?.optionBehaviour != null && option.optionBehaviour.gameObject != null)
            {
                var enabled = true;
                var parent = option.parent;
                while (parent != null && enabled)
                {
                    enabled = parent.selection != 0;
                    parent = parent.parent;
                }

                option.optionBehaviour.gameObject.SetActive(enabled);
                if (enabled)
                {
                    offset -= option.isHeader ? 0.75f : 0.5f;
                    option.optionBehaviour.transform.localPosition = new Vector3(
                        option.optionBehaviour.transform.localPosition.x, offset,
                        option.optionBehaviour.transform.localPosition.z);
                }
            }
        }
    }
}

[HarmonyPatch]
internal class GameOptionsDataPatch
{
    public static int maxPage = 7;

    private static string buildRoleOptions()
    {
        var impRoles = "<size=150%><color=#ff1c1c>伪装者阵营</color></size>" +
                       buildOptionsOfType(CustomOptionType.Impostor, true) + "\n";
        var neutralRoles = "<size=150%><color=#50544c>中立阵营</color></size>" +
                           buildOptionsOfType(CustomOptionType.Neutral, true) + "\n";
        var crewRoles = "<size=150%><color=#08fcfc>船员阵营</color></size>" +
                        buildOptionsOfType(CustomOptionType.Crewmate, true) + "\n";
        var modifiers = "<size=150%><color=#ffec04>附加职业</color></size>" +
                        buildOptionsOfType(CustomOptionType.Modifier, true);
        return impRoles + neutralRoles + crewRoles + modifiers;
    }

    private static string buildModifierExtras(CustomOption customOption)
    {
        // find options children with quantity
        var children = options.Where(o => o.parent == customOption);
        var quantity = children.Where(o => o.name.Contains("Quantity")).ToList();
        if (customOption.getSelection() == 0) return "";
        if (quantity.Count == 1) return $" ({quantity[0].getQuantity()})";
        if (customOption == CustomOptionHolder.modifierLover)
            return $" (1 Evil: {CustomOptionHolder.modifierLoverImpLoverRate.getSelection() * 10}%)";
        return "";
    }

    private static string buildOptionsOfType(CustomOptionType type, bool headerOnly)
    {
        var sb = new StringBuilder("\n");
        var options = CustomOption.options.Where(o => o.type == type);
        if (TORMapOptions.gameMode == CustomGamemodes.Guesser)
        {
            if (type == CustomOptionType.General)
                options = CustomOption.options.Where(o => o.type == type || o.type == CustomOptionType.Guesser);
            var remove = new List<int> { 308, 310, 311, 312, 313, 314, 315, 316, 317, 318 };
            options = options.Where(x => !remove.Contains(x.id));
        }
        else if (TORMapOptions.gameMode == CustomGamemodes.Classic)
        {
            options = options.Where(x =>
                !(x.type == CustomOptionType.Guesser || x == CustomOptionHolder.crewmateRolesFill));
        }
        else if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek)
        {
            options = options.Where(x =>
                x.type == CustomOptionType.HideNSeekMain || x.type == CustomOptionType.HideNSeekRoles);
        }
        else if (TORMapOptions.gameMode == CustomGamemodes.PropHunt)
        {
            options = options.Where(x => x.type == CustomOptionType.PropHunt);
        }

        foreach (var option in options)
            if (option.parent == null)
            {
                var line = $"{option.name}: {option.selections[option.selection]}";
                if (type == CustomOptionType.Modifier) line += buildModifierExtras(option);
                sb.AppendLine(line);
            }
            else if (option.parent.getSelection() > 0)
            {
                if (option.id == 30170) //Deputy
                    sb.AppendLine(
                        $"- {Helpers.cs(Deputy.color, "捕快")}: {option.selections[option.selection].ToString()}");
                else if (option.id == 20136) //Sidekick
                    sb.AppendLine(
                        $"- {Helpers.cs(Sidekick.color, "跟班")}: {option.selections[option.selection].ToString()}");
                else if (option.id == 20181) //Prosecutor
                    sb.AppendLine(
                        $"- {Helpers.cs(Lawyer.color, "处刑者")}: {option.selections[option.selection].ToString()}");
            }

        if (headerOnly) return sb.ToString();
        sb = new StringBuilder();

        foreach (var option in options)
        {
            if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek && option.type != CustomOptionType.HideNSeekMain &&
                option.type != CustomOptionType.HideNSeekRoles) continue;
            if (TORMapOptions.gameMode == CustomGamemodes.PropHunt &&
                option.type != CustomOptionType.PropHunt) continue;
            if (option.parent != null)
            {
                var isIrrelevant = option.parent.getSelection() == 0 ||
                                   (option.parent.parent != null && option.parent.parent.getSelection() == 0);

                var c = isIrrelevant ? Color.grey : Color.white; // No use for now
                if (isIrrelevant) continue;
                sb.AppendLine(Helpers.cs(c, $"{option.name}: {option.selections[option.selection]}"));
            }
            else
            {
                if (option == CustomOptionHolder.crewmateRolesCountMin)
                {
                    var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "船员阵营");
                    var min = CustomOptionHolder.crewmateRolesCountMin.getSelection();
                    var max = CustomOptionHolder.crewmateRolesCountMax.getSelection();
                    var optionValue = "";
                    if (CustomOptionHolder.crewmateRolesFill.getBool())
                    {
                        var crewCount = PlayerControl.AllPlayerControls.Count -
                                        GameOptionsManager.Instance.currentGameOptions.NumImpostors;
                        var minNeutral = CustomOptionHolder.neutralRolesCountMin.getSelection();
                        var maxNeutral = CustomOptionHolder.neutralRolesCountMax.getSelection();
                        if (minNeutral > maxNeutral) minNeutral = maxNeutral;
                        min = crewCount - maxNeutral;
                        max = crewCount - minNeutral;
                        if (min < 0) min = 0;
                        if (max < 0) max = 0;
                        optionValue = "Fill: ";
                    }

                    if (min > max) min = max;
                    optionValue += min == max ? $"{max}" : $"{min} - {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if (option == CustomOptionHolder.neutralRolesCountMin)
                {
                    var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "中立阵营");
                    var min = CustomOptionHolder.neutralRolesCountMin.getSelection();
                    var max = CustomOptionHolder.neutralRolesCountMax.getSelection();
                    if (min > max) min = max;
                    var optionValue = min == max ? $"{max}" : $"{min} - {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if (option == CustomOptionHolder.impostorRolesCountMin)
                {
                    var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "伪装者阵营");
                    var min = CustomOptionHolder.impostorRolesCountMin.getSelection();
                    var max = CustomOptionHolder.impostorRolesCountMax.getSelection();
                    if (max > GameOptionsManager.Instance.currentGameOptions.NumImpostors)
                        max = GameOptionsManager.Instance.currentGameOptions.NumImpostors;
                    if (min > max) min = max;
                    var optionValue = min == max ? $"{max}" : $"{min} - {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if (option == CustomOptionHolder.modifiersCountMin)
                {
                    var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "附加能力");
                    var min = CustomOptionHolder.modifiersCountMin.getSelection();
                    var max = CustomOptionHolder.modifiersCountMax.getSelection();
                    if (min > max) min = max;
                    var optionValue = min == max ? $"{max}" : $"{min} - {max}";
                    sb.AppendLine($"{optionName}: {optionValue}");
                }
                else if (option == CustomOptionHolder.crewmateRolesCountMax ||
                         option == CustomOptionHolder.neutralRolesCountMax ||
                         option == CustomOptionHolder.impostorRolesCountMax ||
                         option == CustomOptionHolder.modifiersCountMax)
                {
                }
                else
                {
                    sb.AppendLine($"\n{option.name}: {option.selections[option.selection].ToString()}");
                }
            }
        }

        return sb.ToString();
    }

    public static string buildAllOptions(string vanillaSettings = "", bool hideExtras = false)
    {
        if (vanillaSettings == "")
            vanillaSettings =
                GameOptionsManager.Instance.CurrentGameOptions.ToHudString(PlayerControl.AllPlayerControls.Count);
        var counter = TheOtherRolesPlugin.optionsPage;
        var hudString = counter != 0 && !hideExtras
            ? Helpers.cs(DateTime.Now.Second % 2 == 0 ? Color.white : Color.red, "(如有必要，请使用滚轮)\n\n")
            : "";

        if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek)
        {
            if (TheOtherRolesPlugin.optionsPage > 1) TheOtherRolesPlugin.optionsPage = 0;
            maxPage = 2;
            switch (counter)
            {
                case 0:
                    hudString += "第1页: 躲猫猫模式设置 \n\n" + buildOptionsOfType(CustomOptionType.HideNSeekMain, false);
                    break;
                case 1:
                    hudString += "第2页: 躲猫猫职业设置 \n\n" + buildOptionsOfType(CustomOptionType.HideNSeekRoles, false);
                    break;
            }
        }
        else if (TORMapOptions.gameMode == CustomGamemodes.PropHunt)
        {
            maxPage = 1;
            switch (counter)
            {
                case 0:
                    hudString += "第1页: 道具躲猫猫模式设置 \n\n" + buildOptionsOfType(CustomOptionType.PropHunt, false);
                    break;
            }
        }
        else
        {
            maxPage = 7;
            switch (counter)
            {
                case 0:
                    hudString += (!hideExtras ? "" : "第1页: 游戏设置 \n\n") + vanillaSettings;
                    break;
                case 1:
                    hudString += "第2页: 超多职业模组设置 \n" + buildOptionsOfType(CustomOptionType.General, false);
                    break;
                case 2:
                    hudString += "第3页: 职业和附加职业设置 \n" + buildRoleOptions();
                    break;
                case 3:
                    hudString += "第4页: 伪装者职业设置 \n" + buildOptionsOfType(CustomOptionType.Impostor, false);
                    break;
                case 4:
                    hudString += "第5页: 中立职业设置 \n" + buildOptionsOfType(CustomOptionType.Neutral, false);
                    break;
                case 5:
                    hudString += "第6页: 船员职业设置 \n" + buildOptionsOfType(CustomOptionType.Crewmate, false);
                    break;
                case 6:
                    hudString += "第7页: 附加职业设置 \n" + buildOptionsOfType(CustomOptionType.Modifier, false);
                    break;
            }
        }

        if (!hideExtras || counter != 0) hudString += $"\n 按Tab或者数字键查看更多... ({counter + 1}/{maxPage})";
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
                                                          || __instance.NumImpostors > 3 || __instance.KillDistance < 0
                                                          || __instance.KillDistance >=
                                                          GameOptionsData.KillDistances.Count
                                                          || __instance.PlayerSpeedMod <= 0f ||
                                                          __instance.PlayerSpeedMod > 3f;
    }

    [HarmonyPatch(typeof(NormalGameOptionsV07), nameof(NormalGameOptionsV07.AreInvalid))]
    [HarmonyPrefix]
    public static bool Prefix(NormalGameOptionsV07 __instance, ref int maxExpectedPlayers)
    {
        return __instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1
                                                          || __instance.NumImpostors > 3 || __instance.KillDistance < 0
                                                          || __instance.KillDistance >=
                                                          GameOptionsData.KillDistances.Count
                                                          || __instance.PlayerSpeedMod <= 0f ||
                                                          __instance.PlayerSpeedMod > 3f;
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
            var index = GameOptionsManager.Instance.currentNormalGameOptions.KillDistance;
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
            __result = "非常短";
            return false;
        }

        return true;
    }

    public static void addKillDistance()
    {
        GameOptionsData.KillDistances = new Il2CppStructArray<float>(new[] { 0.5f, 1f, 1.8f, 2.5f });
        GameOptionsData.KillDistanceStrings = new Il2CppStringArray(new[] { "Very Short", "Short", "Medium", "Long" });
    }
}

[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public static class GameOptionsNextPagePatch
{
    public static void Postfix(KeyboardJoystick __instance)
    {
        var page = TheOtherRolesPlugin.optionsPage;
        if (Input.GetKeyDown(KeyCode.Tab)) TheOtherRolesPlugin.optionsPage = (TheOtherRolesPlugin.optionsPage + 1) % 7;
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) TheOtherRolesPlugin.optionsPage = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) TheOtherRolesPlugin.optionsPage = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) TheOtherRolesPlugin.optionsPage = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) TheOtherRolesPlugin.optionsPage = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) TheOtherRolesPlugin.optionsPage = 4;
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) TheOtherRolesPlugin.optionsPage = 5;
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) TheOtherRolesPlugin.optionsPage = 6;
        if (Input.GetKeyDown(KeyCode.F1))
            HudManagerUpdate.ToggleSettings(HudManager.Instance);
        if (TheOtherRolesPlugin.optionsPage >= GameOptionsDataPatch.maxPage) TheOtherRolesPlugin.optionsPage = 0;

        if (page != TheOtherRolesPlugin.optionsPage)
        {
            var position =
                (Vector3)FastDestroyableSingleton<HudManager>.Instance?.GameSettings?.transform.localPosition;
            FastDestroyableSingleton<HudManager>.Instance.GameSettings.transform.localPosition =
                new Vector3(position.x, 2.9f, position.z);
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
        if (CachedPlayer.LocalPlayer?.PlayerControl.CanMove != true)
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
        ;
        var curString = "";
        string curBlock;
        var j = 0;
        for (var i = 0; i < blocks.Length; i++)
        {
            curBlock = blocks[i];
            if (Helpers.lineCount(curBlock) + Helpers.lineCount(curString) < 43)
            {
                curString += curBlock + "\n\n";
            }
            else
            {
                settingsTMPs[j].text = curString;
                j++;

                curString = "\n" + curBlock + "\n\n";
                if (curString.Substring(0, 2) != "\n\n") curString = "\n" + curString;
            }
        }

        if (j < settingsTMPs.Length) settingsTMPs[j].text = curString;
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
        settingsBackground = Object.Instantiate(__instance.FullScreen.gameObject, __instance.transform);
        settingsBackground.SetActive(true);
        var renderer = settingsBackground.GetComponent<SpriteRenderer>();
        renderer.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        renderer.enabled = true;

        for (var i = 0; i < settingsTMPs.Length; i++)
        {
            settingsTMPs[i] = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
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

    [HarmonyPostfix]
    public static void Postfix(HudManager __instance)
    {
        if (!toggleSettingsButton || !toggleSettingsButtonObject)
        {
            // add a special button for settings viewing:
            toggleSettingsButtonObject =
                Object.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
            toggleSettingsButtonObject.transform.localPosition =
                __instance.MapButton.transform.localPosition + new Vector3(0, -0.66f, -500f);
            var renderer = toggleSettingsButtonObject.GetComponent<SpriteRenderer>();
            renderer.sprite =
                Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CurrentSettingsButton.png", 180f);
            toggleSettingsButton = toggleSettingsButtonObject.GetComponent<PassiveButton>();
            toggleSettingsButton.OnClick.RemoveAllListeners();
            toggleSettingsButton.OnClick.AddListener((Action)(() => ToggleSettings(__instance)));
        }

        toggleSettingsButtonObject.SetActive(__instance.MapButton.gameObject.active &&
                                             !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                                             GameOptionsManager.Instance.currentGameOptions.GameMode !=
                                             GameModes.HideNSeek);
        toggleSettingsButtonObject.transform.localPosition =
            __instance.MapButton.transform.localPosition + new Vector3(0, -0.66f, -500f);
    }
}