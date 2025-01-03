using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

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

    public static void UpdateToggleText(this ToggleButtonBehaviour button, bool on, string text)
    {
        button.onState = on;
        var color = on ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
        button.Background.color = color;
        button.Text.text = text + ": " + DestroyableSingleton<TranslationController>.Instance.GetString(button.onState
            ? StringNames.SettingsOn : StringNames.SettingsOff, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
        if (button.Rollover)
            button.Rollover.ChangeOutColor(color);
    }

    public static void UpdateButtonText(this ToggleButtonBehaviour button, string text, string state)
    {
        button.onState = false;
        var color = Color.white;
        button.Background.color = color;
        button.Text.text = text + ": " + state;
        if (button.Rollover)
            button.Rollover.ChangeOutColor(color);
    }

    private static ToggleButtonBehaviour AddButton(int index, string name, Action onClicked, GameObject nebulaTab, GameObject toggleButtonTemplate)
    {
        var button = Object.Instantiate(toggleButtonTemplate, null);
        button.transform.SetParent(nebulaTab.transform);
        button.transform.localScale = new Vector3(1f, 1f, 1f);
        button.transform.localPosition = new Vector3(1.3f * (index % 2 * 2 - 1), 1.6f - 0.5f * (index / 2), 0f);
        button.name = name;
        var result = button.GetComponent<ToggleButtonBehaviour>();
        var passiveButton = button.GetComponent<PassiveButton>();
        passiveButton.OnClick = new ButtonClickedEvent();
        passiveButton.OnClick.AddListener((UnityAction)onClicked);
        return result;
    }

    private static ToggleButtonBehaviour toggleCursor;
    private static ToggleButtonBehaviour enableSoundEffects;
    private static ToggleButtonBehaviour showKeyReminder;
    private static ToggleButtonBehaviour showFPS;
    private static ToggleButtonBehaviour localHats;
    private static ToggleButtonBehaviour showChatNotifications;
    private static ToggleButtonBehaviour muteLobbyBGM;
    private static ToggleButtonBehaviour forceUsePlus25Protocol;

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

        //EnableSoundEffects
        enableSoundEffects = AddButton(buttonIndex++, "EnableSoundEffects", () =>
        {
            enableSoundEffects.UpdateToggleText(!enableSoundEffects.onState, GetString("EnableSoundEffectsText"));
            ModOption.enableSoundEffects = Main.EnableSoundEffects.Value = enableSoundEffects.onState;
        }, nebulaTab, toggleButtonTemplate);

        //ToggleCursor
        toggleCursor = AddButton(buttonIndex++, "ToggleCursor", () =>
        {
            enableCursor(false);
            toggleCursor.UpdateToggleText(!toggleCursor.onState, GetString("ToggleCursorText"));
            ModOption.toggleCursor = Main.ToggleCursor.Value = toggleCursor.onState;
            Message($"toggleCursor: {toggleCursor.onState}");
        }, nebulaTab, toggleButtonTemplate);

        //ShowFPS
        showFPS = AddButton(buttonIndex++, "ShowFPS", () =>
        {
            showFPS.UpdateToggleText(!showFPS.onState, GetString("ShowFPS"));
            ModOption.showFPS = Main.ShowFPS.Value = showFPS.onState;
        }, nebulaTab, toggleButtonTemplate);

        //ShowKeyReminder
        showKeyReminder = AddButton(buttonIndex++, "ShowKeyReminder", () =>
        {
            showKeyReminder.UpdateToggleText(!showKeyReminder.onState, GetString("ShowKeyReminder"));
            ModOption.showKeyReminder = Main.ShowKeyReminder.Value = showKeyReminder.onState;
        }, nebulaTab, toggleButtonTemplate);

        //Mute Lobby BGM
        muteLobbyBGM = AddButton(buttonIndex++, "MuteLobbyBGM", () =>
        {
            muteLobbyBGM.UpdateToggleText(!muteLobbyBGM.onState, GetString("MuteLobbyBGM"));
            ModOption.MuteLobbyBGM = Main.MuteLobbyBGM.Value = muteLobbyBGM.onState;
        }, nebulaTab, toggleButtonTemplate);

        //Show Chat Notifications
        showChatNotifications = AddButton(buttonIndex++, "ShowChatNotificationsText", () =>
        {
            showChatNotifications.UpdateToggleText(!showChatNotifications.onState, GetString("ShowChatNotificationsText"));
            ModOption.ShowChatNotifications = Main.ShowChatNotifications.Value = showChatNotifications.onState;
        }, nebulaTab, toggleButtonTemplate);

        //LocalHats
        localHats = AddButton(buttonIndex++, "LocalHats", () =>
        {
            localHats.UpdateToggleText(!localHats.onState, GetString("LocalHatsText"));
            ModOption.localHats = Main.LocalHats.Value = localHats.onState;
        }, nebulaTab, toggleButtonTemplate);

        //Force Use Plus 25 Protocol
        forceUsePlus25Protocol = AddButton(buttonIndex++, "ForceUsePlus25Protocol", () =>
        {
            forceUsePlus25Protocol.UpdateToggleText(!forceUsePlus25Protocol.onState, GetString("ForceUsePlus25Protocol"));
            Main.ForceUsePlus25Protocol.Value = forceUsePlus25Protocol.onState;
        }, nebulaTab, toggleButtonTemplate);

        //キー割り当てボタン
        GameObject TextObject;

        List<ToggleButtonBehaviour> allKeyBindingButtons = new();
        var selectedKeyBinding = -1;

        var defaultButton = Object.Instantiate(applyButtonTemplate, null);
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

            var inputButton = Object.Instantiate(toggleButtonTemplate, null);
            inputButton.transform.SetParent(keyBindingTab.transform);
            inputButton.transform.localScale = new Vector3(1f, 1f, 1f);
            inputButton.transform.localPosition = new Vector3(1.3f * (index % 2 * 2 - 1), 1.5f - 0.5f * (index / 2), 0f);
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

        var keyBindingButton = Object.Instantiate(applyButtonTemplate, null);
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

        tabs[^1] = Object.Instantiate(tabs[1], null);
        var nebulaButton = tabs[^1];
        nebulaButton.gameObject.name = "NebulaButton";
        nebulaButton.transform.SetParent(tabs[0].transform.parent);
        nebulaButton.transform.localScale = new Vector3(1f, 1f, 1f);
        nebulaButton.Content = nebulaTab;
        var textObj = nebulaButton.transform.FindChild("Text_TMP").gameObject;
        textObj.GetComponent<TextTranslatorTMP>().enabled = false;
        textObj.GetComponent<TMP_Text>().text = "modOptionsTitle".Translate();

        tabs.Add(Object.Instantiate(tabs[1], null));
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

            showFPS.UpdateToggleText(Main.ShowFPS.Value, GetString("ShowFPS"));
            enableSoundEffects.UpdateToggleText(Main.EnableSoundEffects.Value, GetString("EnableSoundEffectsText"));
            showKeyReminder.UpdateToggleText(Main.ShowKeyReminder.Value, GetString("ShowKeyReminder"));
            toggleCursor.UpdateToggleText(Main.ToggleCursor.Value, GetString("ToggleCursorText"));
            showChatNotifications.UpdateToggleText(Main.ShowChatNotifications.Value, GetString("ShowChatNotificationsText"));
            muteLobbyBGM.UpdateToggleText(Main.MuteLobbyBGM.Value, GetString("MuteLobbyBGM"));
            localHats.UpdateToggleText(Main.LocalHats.Value, GetString("LocalHatsText"));
            forceUsePlus25Protocol.UpdateToggleText(Main.ForceUsePlus25Protocol.Value, GetString("ForceUsePlus25Protocol"));

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