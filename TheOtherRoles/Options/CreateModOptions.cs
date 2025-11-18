using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine.Events;
using static UnityEngine.UI.Button;

namespace TheOtherRoles.Options;

[HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
public static class StartOptionMenuPatch
{
    public static void UpdateCustomText(this ToggleButtonBehaviour button, Color color, string text = null)
    {
        button.onState = false;
        button.Background.color = color;
        if (text != null)
            button.Text.text = text;

        if (button.Rollover)
            button.Rollover.ChangeOutColor(color);
    }

    public static void UpdateToggleText(this ToggleButtonBehaviour button, bool on, string title)
    {
        button.onState = on;
        var color = on ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
        button.Background.color = color;
        button.Text.text = title + ": " + GetString(button.onState ? "ModOptions.On" : "ModOptions.Off");
        if (button.Rollover)
            button.Rollover.ChangeOutColor(color);
    }

    public static void UpdateButtonText(this ToggleButtonBehaviour button, string text, string title, bool onState = false)
    {
        button.onState = onState;
        var color = onState ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
        button.Background.color = color;
        button.Text.text = title + ": " + text;
        if (button.Rollover)
            button.Rollover.ChangeOutColor(color);
    }

    private static string GetCPUAffinityMaskText()
    {
        ulong mask = Main.ProcessorAffinityMask.Value;
        string showCore;
        switch (mask)
        {
            case 0b1UL:
                showCore = GetString("ProcessorAffinityMask.1");
                break;
            case 0b11UL:
                showCore = GetString("ProcessorAffinityMask.2");
                break;
            case 0b1010UL:
                showCore = GetString("ProcessorAffinityMask.3");
                break;
            case 0b1111UL:
                showCore = GetString("ProcessorAffinityMask.4");
                break;
            case 0UL:
            default:
                showCore = GetString("ProcessorAffinityMask.Off");
                break;
        }
        return $"{showCore}";
    }

    private static ToggleButtonBehaviour AddButton(int index, string name, Action onClicked, GameObject nebulaTab, GameObject toggleButtonTemplate)
    {
        var button = UObject.Instantiate(toggleButtonTemplate, null);
        button.transform.SetParent(nebulaTab.transform);
        button.transform.localScale = new Vector3(1f, 1f, 1f);
        button.transform.localPosition = new Vector3(1.3f * ((index % 2 * 2) - 1), 1.6f - (0.5f * (index / 2)), 0f);
        button.name = name;
        var result = button.GetComponent<ToggleButtonBehaviour>();
        var passiveButton = button.GetComponent<PassiveButton>();
        passiveButton.OnClick = new ButtonClickedEvent();
        passiveButton.OnClick.AddListener((UnityAction)onClicked);
        return result;
    }

    private static ToggleButtonBehaviour processorAffinityMask;
    private static ToggleButtonBehaviour toggleCursor;
    private static ToggleButtonBehaviour enableSoundEffects;
    private static ToggleButtonBehaviour ButtonArrangement;
    private static ToggleButtonBehaviour showKeyReminder;
    private static ToggleButtonBehaviour showFPS;

    public static void Postfix(OptionsMenuBehaviour __instance)
    {
        List<TabGroup> tabs = new(__instance.Tabs.ToArray());

        PassiveButton passiveButton;

        //設定項目を追加する

        GameObject nebulaTab = new("NebulaTab");
        nebulaTab.transform.SetParent(__instance.transform);
        nebulaTab.transform.localScale = new Vector3(1f, 1f, 1f);
        nebulaTab.SetActive(false);

        GameObject keyBindingTab = new("KeyBindingTab");
        keyBindingTab.transform.SetParent(__instance.transform);
        keyBindingTab.transform.localScale = new Vector3(1f, 1f, 1f);
        keyBindingTab.SetActive(false);

        var applyButtonTemplate = tabs[1].Content.transform.GetChild(0).FindChild("ApplyButton").gameObject;
        var toggleButtonTemplate = tabs[0].Content.transform.FindChild("MiscGroup").FindChild("StreamerModeButton").gameObject;

        var buttonIndex = 0;

        //ProcessorAffinityMask
        processorAffinityMask = AddButton(buttonIndex++, "ProcessorAffinityMask", () =>
        {
            ulong current = Main.ProcessorAffinityMask.Value;
            ulong next;
            switch (current)
            {
                case 0UL:
                    Main.IsCPUProcessorAffinity.Value = true;
                    next = 0b1UL;
                    break;
                case 0b1UL:
                    next = 0b11UL;
                    break;
                case 0b11UL:
                    next = 0b1010UL;
                    break;
                case 0b1010UL:
                    next = 0b1111UL;
                    break;
                case 0b1111UL:
                default:
                    Main.IsCPUProcessorAffinity.Value = false;
                    next = 0UL;
                    break;
            }

            Main.ProcessorAffinityMask.Value = next;
            Main.UpdateCPUProcessorAffinity();
            processorAffinityMask.UpdateButtonText(GetCPUAffinityMaskText(), GetString("ProcessorAffinityMask"), next != 0UL);
        }, nebulaTab, toggleButtonTemplate);

        //EnableSoundEffects
        enableSoundEffects = AddButton(buttonIndex++, "EnableSoundEffects", () =>
        {
            enableSoundEffects.UpdateToggleText(!enableSoundEffects.onState, GetString("EnableSoundEffectsText"));
            Main.EnableSoundEffects.Value = enableSoundEffects.onState;
            if (!ModOption.enableSoundEffects) SoundEffectsManager.stopAll();
        }, nebulaTab, toggleButtonTemplate);

        //ToggleCursor
        toggleCursor = AddButton(buttonIndex++, "ToggleCursor", () =>
        {
            toggleCursor.UpdateToggleText(!toggleCursor.onState, GetString("ToggleCursorText"));
            Main.ToggleCursor.Value = toggleCursor.onState;
            Main.enableCursor(Main.ToggleCursor.Value);
            Message($"toggleCursor: {toggleCursor.onState}");
        }, nebulaTab, toggleButtonTemplate);

        //ShowFPS
        showFPS = AddButton(buttonIndex++, "ShowFPS", () =>
        {
            showFPS.UpdateToggleText(!showFPS.onState, GetString("ShowFPS"));
            Main.ShowFPS.Value = showFPS.onState;
        }, nebulaTab, toggleButtonTemplate);

        //ButtonArrangement
        ButtonArrangement = AddButton(buttonIndex++, "ButtonArrangement", () =>
        {
            var next = (Main.ButtonArrangement.Value % 3) + 1;
            Main.ButtonArrangement.Value = next;
            ButtonArrangement.UpdateButtonText(GetString($"ButtonArrangement.{next}"), GetString("ButtonArrangement"), next != 1);
        }, nebulaTab, toggleButtonTemplate);

        //ShowKeyReminder
        showKeyReminder = AddButton(buttonIndex++, "ShowKeyReminder", () =>
        {
            showKeyReminder.UpdateToggleText(!showKeyReminder.onState, GetString("ShowKeyReminder"));
            Main.ShowKeyReminder.Value = showKeyReminder.onState;
        }, nebulaTab, toggleButtonTemplate);

        //キー割り当てボタン
        GameObject TextObject;

        List<ToggleButtonBehaviour> allKeyBindingButtons = new();
        var selectedKeyBinding = -1;

        var defaultButton = UObject.Instantiate(applyButtonTemplate, null);
        defaultButton.transform.SetParent(keyBindingTab.transform);
        defaultButton.transform.localScale = new Vector3(1f, 1f, 1f);
        defaultButton.transform.localPosition = new Vector3(0f, -2.5f, 0f);
        defaultButton.name = "RestoreDefaultsButton";
        defaultButton.transform.GetChild(0).GetComponent<SpriteRenderer>().size = new Vector2(2.25f, 0.4f);
        TextObject = defaultButton.transform.FindChild("Text_TMP").gameObject;
        TextObject.GetComponent<TextMeshPro>().text = GetString("keyBinding.restoreDefaults");
        TextObject.GetComponent<TextMeshPro>().rectTransform.sizeDelta *= 2;
        TextObject.GetComponent<TextTranslatorTMP>().enabled = false;
        passiveButton = defaultButton.GetComponent<PassiveButton>();
        passiveButton.OnClick = new ButtonClickedEvent();
        passiveButton.OnClick.AddListener((UnityAction)(() =>
        {
            selectedKeyBinding = -1;
            //_ = SoundManager.Instance.PlaySound(Module.MetaScreen.getSelectClip(), false, 0.8f);

            for (var i = 0; i < ModInputManager.allInputs.Count; i++)
            {
                var input = ModInputManager.allInputs[i];
                input.resetToDefault();
                allKeyBindingButtons[i].UpdateCustomText(Color.white, GetString("keyBinding." + input.identifier) + ": " + ModInputManager.allKeyCodes[input.keyCode].displayKey);
            }
        }
        ));

        foreach (var input in ModInputManager.allInputs)
        {
            var index = allKeyBindingButtons.Count;

            var inputButton = UObject.Instantiate(toggleButtonTemplate, null);
            inputButton.transform.SetParent(keyBindingTab.transform);
            inputButton.transform.localScale = new Vector3(1f, 1f, 1f);
            inputButton.transform.localPosition = new Vector3(1.3f * ((index % 2 * 2) - 1), 1.5f - (0.5f * (index / 2)), 0f);
            inputButton.name = input.identifier;
            var inputToggleButton = inputButton.GetComponent<ToggleButtonBehaviour>();
            inputToggleButton.BaseText = 0;
            inputToggleButton.Text.text = GetString("keyBinding." + input.identifier) + ": " + ModInputManager.allKeyCodes[input.keyCode].displayKey;
            passiveButton = inputButton.GetComponent<PassiveButton>();
            passiveButton.OnClick = new ButtonClickedEvent();
            passiveButton.OnClick.AddListener((UnityAction)(() =>
            {
                if (selectedKeyBinding == index)
                {
                    selectedKeyBinding = -1;
                    inputToggleButton.UpdateCustomText(Color.white, null);
                }
                else
                {
                    selectedKeyBinding = index;
                    allKeyBindingButtons[selectedKeyBinding].UpdateCustomText(Color.yellow,
                        GetString($"{GetString($"keyBinding.{input.identifier}")}: {GetString("keyBinding.recording")}"));
                    inputToggleButton.UpdateCustomText(Color.yellow, null);
                }
            }));

            allKeyBindingButtons.Add(inputToggleButton);
        }

        var keyBindingButton = UObject.Instantiate(applyButtonTemplate, null);
        keyBindingButton.transform.SetParent(nebulaTab.transform);
        keyBindingButton.transform.localScale = new Vector3(1f, 1f, 1f);
        keyBindingButton.transform.localPosition = new Vector3(0f, -1.5f, 0f);
        keyBindingButton.name = "KeyBindingButton";
        keyBindingButton.transform.GetChild(0).GetComponent<SpriteRenderer>().size = new Vector2(2.25f, 0.4f);
        TextObject = keyBindingButton.transform.FindChild("Text_TMP").gameObject;
        TextObject.GetComponent<TextMeshPro>().text = GetString("keyBinding");
        TextObject.GetComponent<TextMeshPro>().rectTransform.sizeDelta *= 2;
        TextObject.GetComponent<TextTranslatorTMP>().enabled = false;
        passiveButton = keyBindingButton.GetComponent<PassiveButton>();
        passiveButton.OnClick = new ButtonClickedEvent();
        passiveButton.OnClick.AddListener((UnityAction)(() =>
        {
            //_ = SoundManager.Instance.PlaySound(Module.MetaScreen.getSelectClip(), false, 0.8f);
            __instance.OpenTabGroup(tabs.Count - 1);
        }
        ));

        IEnumerator getEnumerator()
        {
            while (true)
            {
                /*
                if (HudManager.InstanceExists && !GameStartManager.InstanceExists)
                {
                    keyBindingButton.gameObject.SetActive(false);
                */

                if (keyBindingTab.gameObject.active && Input.anyKeyDown && selectedKeyBinding != -1)
                {
                    foreach (var entry in ModInputManager.allKeyCodes)
                    {
                        if (!Input.GetKeyDown(entry.Key))
                            continue;

                        var input = ModInputManager.allInputs[selectedKeyBinding];
                        input.changeKeyCode(entry.Key);
                        allKeyBindingButtons[selectedKeyBinding].UpdateCustomText(Color.white, GetString("keyBinding." + input.identifier) + ": " + ModInputManager.allKeyCodes[input.keyCode].displayKey);
                        selectedKeyBinding = -1;
                        break;
                    }
                }
                else if (!keyBindingTab.gameObject.active && selectedKeyBinding != -1)
                {
                    allKeyBindingButtons[selectedKeyBinding].UpdateCustomText(Color.white, null);
                    selectedKeyBinding = -1;
                }
                yield return null;
            }
        }

        _ = HudManager.InstanceExists
            ? HudManager.Instance.StartCoroutine(getEnumerator().WrapToIl2Cpp())
            : __instance.StartCoroutine(getEnumerator().WrapToIl2Cpp());


        //タブを追加する

        tabs[^1] = UObject.Instantiate(tabs[1], null);
        var nebulaButton = tabs[^1];
        nebulaButton.gameObject.name = "NebulaButton";
        nebulaButton.transform.SetParent(tabs[0].transform.parent);
        nebulaButton.transform.localScale = new Vector3(1f, 1f, 1f);
        nebulaButton.Content = nebulaTab;
        var textObj = nebulaButton.transform.FindChild("Text_TMP").gameObject;
        textObj.GetComponent<TextTranslatorTMP>().enabled = false;
        textObj.GetComponent<TMP_Text>().text = "ModOptions.Title".Translate();

        tabs.Add(UObject.Instantiate(tabs[1], null));
        var keyBindingTabButton = tabs[^1];
        keyBindingTabButton.gameObject.name = "KeyBindingButton";
        keyBindingTabButton.transform.SetParent(tabs[0].transform.parent);
        keyBindingTabButton.transform.localScale = new Vector3(1f, 1f, 1f);
        keyBindingTabButton.Content = keyBindingTab;
        keyBindingTabButton.gameObject.SetActive(false);

        passiveButton = nebulaButton.gameObject.GetComponent<PassiveButton>();
        passiveButton.OnClick = new ButtonClickedEvent();
        passiveButton.OnClick.AddListener((UnityAction)(() =>
        {
            __instance.OpenTabGroup(tabs.Count - 2);

            processorAffinityMask.UpdateButtonText(GetCPUAffinityMaskText(), GetString("ProcessorAffinityMask"), Main.ProcessorAffinityMask.Value != 0UL);
            showFPS.UpdateToggleText(Main.ShowFPS.Value, GetString("ShowFPS"));
            enableSoundEffects.UpdateToggleText(Main.EnableSoundEffects.Value, GetString("EnableSoundEffectsText"));
            ButtonArrangement.UpdateButtonText(GetString($"ButtonArrangement.{Main.ButtonArrangement.Value}"), GetString("ButtonArrangement"), Main.ButtonArrangement.Value != 1);
            showKeyReminder.UpdateToggleText(Main.ShowKeyReminder.Value, GetString("ShowKeyReminder"));
            toggleCursor.UpdateToggleText(Main.ToggleCursor.Value, GetString("ToggleCursorText"));

            passiveButton.OnMouseOver.Invoke();
        }
        ));

        float y = tabs[0].transform.localPosition.y, z = tabs[0].transform.localPosition.z;
        if (tabs.Count == 4)
        {
            for (var i = 0; i < 3; i++)
            {
                tabs[i].transform.localPosition = new Vector3(1.7f * (i - 1), y, z);
            }
        }
        else if (tabs.Count == 5)
        {
            for (var i = 0; i < 4; i++)
            {
                tabs[i].transform.localPosition = new Vector3(1.62f * (i - 1.5f), y, z);
            }
        }

        __instance.Tabs = new Il2CppReferenceArray<TabGroup>(tabs.ToArray());
    }
}