using AmongUs.GameOptions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(CreateOptionsPicker))]
internal class CreateOptionsPickerPatch
{
    private static List<SpriteRenderer> renderers = new();

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.SetGameMode))]
    public static bool Prefix(CreateOptionsPicker __instance, ref GameModes mode)
    {
        if (mode <= GameModes.HideNSeek)
        {
            TORMapOptions.gameMode = CustomGamemodes.Classic;
            return true;
        }

        __instance.SetGameMode(GameModes.Normal);
        var gm = (CustomGamemodes)((int)mode - 2);
        switch (gm)
        {
            case CustomGamemodes.Guesser:
                __instance.GameModeText.text = ModTranslation.getString("isGuesserGm");
                TORMapOptions.gameMode = CustomGamemodes.Guesser;
                break;
            case CustomGamemodes.HideNSeek:
                __instance.GameModeText.text = ModTranslation.getString("isHideNSeekGM");
                TORMapOptions.gameMode = CustomGamemodes.HideNSeek;
                break;
            case CustomGamemodes.PropHunt:
                __instance.GameModeText.text = ModTranslation.getString("isPropHuntGM");
                TORMapOptions.gameMode = CustomGamemodes.PropHunt;
                break;
        }

        return false;
    }


    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Refresh))]
    public static void Postfix(CreateOptionsPicker __instance)
    {
        __instance.GameModeText.text = TORMapOptions.gameMode switch
        {
            CustomGamemodes.Guesser => ModTranslation.getString("isGuesserGm"),
            CustomGamemodes.HideNSeek => ModTranslation.getString("isHideNSeekGM"),
            CustomGamemodes.PropHunt => ModTranslation.getString("isPropHuntGM"),
            _ => __instance.GameModeText.text
        };
    }
}

[HarmonyPatch(typeof(GameModeMenu))]
internal class GameModeMenuPatch
{
    [HarmonyPatch(typeof(GameModeMenu), nameof(GameModeMenu.OnEnable))]
    public static bool Prefix(GameModeMenu __instance)
    {
        var gameMode = (uint)__instance.Parent.GetTargetOptions().GameMode;
        var num = ((Mathf.CeilToInt(4f / 10f) / 2f) - 0.5f) * -2.5f; // 4 for 4 buttons!
        __instance.controllerSelectable.Clear();
        var num2 = 0;
        __instance.ButtonPool.poolSize = 5;
        for (var i = 0; i <= 5; i++)
        {
            var entry = (GameModes)i;
            if (entry == GameModes.None) continue;
            var chatLanguageButton = __instance.ButtonPool.Get<ChatLanguageButton>();
            chatLanguageButton.transform.localPosition =
                new Vector3(num + (num2 / 10 * 2.5f), 2f - (num2 % 10 * 0.5f), 0f);
            if (i <= 2)
            {
                chatLanguageButton.Text.text =
                    DestroyableSingleton<TranslationController>.Instance.GetString(
                        GameModesHelpers.ModeToName[entry], new Il2CppReferenceArray<Object>(0));
            }
            else
            {
                chatLanguageButton.Text.text = i == 3 ? ModTranslation.getString("isGuesserGm") : ModTranslation.getString("isHideNSeekGM");
                if (i == 5)
                    chatLanguageButton.Text.text = ModTranslation.getString("isPropHuntGM");
            }

            chatLanguageButton.Button.OnClick.RemoveAllListeners();
            chatLanguageButton.Button.OnClick.AddListener((Action)delegate { __instance.ChooseOption(entry); });

            var isCurrentMode = i <= 2 && TORMapOptions.gameMode == CustomGamemodes.Classic
                ? (long)entry == gameMode
                : (i == 3 && TORMapOptions.gameMode == CustomGamemodes.Guesser) ||
                  (i == 4 && TORMapOptions.gameMode == CustomGamemodes.HideNSeek) ||
                  (i == 5 && TORMapOptions.gameMode == CustomGamemodes.PropHunt);
            chatLanguageButton.SetSelected(isCurrentMode);
            __instance.controllerSelectable.Add(chatLanguageButton.Button);
            if (isCurrentMode) __instance.defaultButtonSelected = chatLanguageButton.Button;
            num2++;
        }

        ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton,
            __instance.defaultButtonSelected, __instance.controllerSelectable);
        return false;
    }
}