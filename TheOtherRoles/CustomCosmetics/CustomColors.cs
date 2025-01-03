using System.Collections.Generic;
using System.Linq;
using AmongUs.Data.Legacy;
using TheOtherRoles.Utilities;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace TheOtherRoles.CustomCosmetics;

public class CustomColors
{
    protected static Dictionary<int, string> ColorStrings = new();
    public static List<int> lighterColors = [3, 4, 5, 7, 10, 11, 13, 14, 17];
    public static uint pickableColors = (uint)Palette.ColorNames.Length;

    private enum ColorType
    {
        Tamarind = 18,
        Army,
        Olive,
        Turquoise,
        Mint,
        Lavender,
        Peach,
        Wasabi,
        HotPink,
        Petrol,
        Lemon,
        SignalOrange,
        Teal,
        Blurple,
        Sunrise,
        Ice,
        Darkness,
        RoyalGreen,
        Slime,
        Navy,
        Ocean,
        Sundown,
        PitchBlack,
        Nega,
        Snow,
        SkyBlue,
        Fuchsia,
        Azuki,
        TokiwaGreen,
        Nougat,
        Pitchwhite,
        Posi,
        Emerald,
        Crasyublue,
        Gold,
        Intenseblue,
        Mildpurple,
        XmasRed,
    }

    private static Dictionary<ColorType, (Color32, Color32, bool)> CustomColorData = new()
    {
        {ColorType.Tamarind, (new Color32(48, 28, 34, byte.MaxValue), new Color32(30, 11, 16, byte.MaxValue), true)},
        {ColorType.Olive, (new Color32(154, 140, 61, byte.MaxValue), new Color32(104, 95, 40, byte.MaxValue), true)},
        {ColorType.Mint, (new Color32(111, 192, 156, byte.MaxValue), new Color32(65, 148, 111, byte.MaxValue), true)},
        {ColorType.Lavender, (new Color32(173, 126, 201, byte.MaxValue), new Color32(131, 58, 203, byte.MaxValue), true)},
        {ColorType.Peach, (new Color32(255, 164, 119, byte.MaxValue), new Color32(238, 128, 100, byte.MaxValue), true)},
        {ColorType.HotPink, (new Color32(255, 51, 102, byte.MaxValue), new Color32(232, 0, 58, byte.MaxValue), true)},
        {ColorType.Lemon, (new Color32(219, 253, 47, byte.MaxValue), new Color32(116, 229, 16, byte.MaxValue), true)},
        {ColorType.SignalOrange, (new Color32(247, 68, 23, byte.MaxValue), new Color32(155, 46, 15, byte.MaxValue), true)},
        {ColorType.Teal, (new Color32(37, 184, 191, byte.MaxValue), new Color32(18, 137, 134, byte.MaxValue), true)},
        {ColorType.Sunrise, (new Color32(255, 202, 25, byte.MaxValue), new Color32(219, 68, 66, byte.MaxValue), true)},
        {ColorType.Ice, (new Color32(0xA8, 0xDF, 0xFF, byte.MaxValue), new Color32(0x59, 0x9F, 0xC8, byte.MaxValue), true)},
        {ColorType.Snow, (new Color32(229, 249, 255, byte.MaxValue), new Color32(135, 226, 255, byte.MaxValue), true)},
        {ColorType.SkyBlue, (new Color32(89, 210, 255, byte.MaxValue), new Color32(37, 169, 232, byte.MaxValue), true)},
        {ColorType.Pitchwhite, (new Color32(255, 255, 255, byte.MaxValue), new Color32(255, 255, 255, byte.MaxValue), true)},
        {ColorType.Posi, (new Color32(255, 255, 255, byte.MaxValue), new Color32(0, 0, 0, byte.MaxValue), true)},
        {ColorType.Emerald, (new Color32(98, 214, 133, byte.MaxValue), new Color32(82, 179, 111, byte.MaxValue), true)},
        {ColorType.Gold, (new Color32(255, 216, 70, byte.MaxValue), new Color32(226, 168, 13, byte.MaxValue), true)},
        {ColorType.Intenseblue, (new Color32(83, 136, 255, byte.MaxValue), new Color32(76, 122, 230, byte.MaxValue), true)},
        {ColorType.XmasRed, (new Color32(219, 41, 41, byte.MaxValue), new Color32(255, 255, 255, byte.MaxValue), true)},

        {ColorType.Army, (new Color32(39, 45, 31, byte.MaxValue), new Color32(11, 30, 24, byte.MaxValue), false)},
        {ColorType.Turquoise, (new Color32(22, 132, 176, byte.MaxValue), new Color32(15, 89, 117, byte.MaxValue), false)},
        {ColorType.PitchBlack, (new Color32(0, 0, 0, byte.MaxValue), new Color32(0, 0, 0, byte.MaxValue), false)},
        {ColorType.Wasabi, (new Color32(112, 143, 46, byte.MaxValue), new Color32(72, 92, 29, byte.MaxValue), false)},
        {ColorType.Petrol, (new Color32(0, 99, 105, byte.MaxValue), new Color32(0, 61, 54, byte.MaxValue), false)},
        {ColorType.Darkness, (new Color32(36, 39, 40, byte.MaxValue), new Color32(10, 10, 10, byte.MaxValue), false)},
        {ColorType.RoyalGreen, (new Color32(9, 82, 33, byte.MaxValue), new Color32(0, 46, 8, byte.MaxValue), false)},
        {ColorType.Slime, (new Color32(244, 255, 188, byte.MaxValue), new Color32(167, 239, 112, byte.MaxValue), false)},
        {ColorType.Navy, (new Color32(9, 43, 119, byte.MaxValue), new Color32(0, 13, 56, byte.MaxValue), false)},
        {ColorType.Nega, (new Color32(0, 0, 0, byte.MaxValue), new Color32(255, 255, 255, byte.MaxValue), false)},
        {ColorType.Ocean, (new Color32(55, 159, 218, byte.MaxValue), new Color32(62, 92, 158, byte.MaxValue), false)},
        {ColorType.Sundown, (new Color32(252, 194, 100, byte.MaxValue), new Color32(197, 98, 54, byte.MaxValue), false)},
        {ColorType.Fuchsia, (new Color32(164, 17, 129, byte.MaxValue), new Color32(104, 3, 79, byte.MaxValue), false)},
        {ColorType.Azuki, (new Color32(150, 81, 77, byte.MaxValue), new Color32(115, 40, 35, byte.MaxValue), false)},
        {ColorType.TokiwaGreen, (new Color32(0, 123, 67, byte.MaxValue), new Color32(0, 84, 83, byte.MaxValue), false)},
        {ColorType.Nougat, (new Color32(160, 101, 56, byte.MaxValue), new Color32(115, 15, 78, byte.MaxValue), false)},
        {ColorType.Crasyublue, (new Color32(2, 38, 106, byte.MaxValue), new Color32(64, 0, 111, byte.MaxValue), false)},
        {ColorType.Mildpurple, (new Color32(109, 83, 131, byte.MaxValue), new Color32(82, 54, 105, byte.MaxValue), false)},
        {ColorType.Blurple, (new Color32(61, 44, 142, byte.MaxValue), new Color32(25, 14, 90, byte.MaxValue), false)},
    };

    public static void Load()
    {
        var longlist = Palette.ColorNames.ToList();
        var colorlist = Palette.PlayerColors.ToList();
        var shadowlist = Palette.ShadowColors.ToList();

        var id = 60000;
        foreach (var entry in CustomColorData.OrderBy(entry => (int)entry.Key))
        {
            var colorType = entry.Key;
            var (color, shadow, isLighterColor) = entry.Value;

            longlist.Add((StringNames)id);
            ColorStrings[id] = colorType.ToString();
            colorlist.Add(color);
            shadowlist.Add(shadow);
            if (isLighterColor)
                lighterColors.Add(colorlist.Count - 1);
            id++;
        }
        Palette.ColorNames = longlist.ToArray();
        Palette.PlayerColors = colorlist.ToArray();
        Palette.ShadowColors = shadowlist.ToArray();
    }

    [HarmonyPatch]
    public static class CustomColorPatches
    {
        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Object>))]
        private class ColorStringPatch
        {
            [HarmonyPriority(Priority.Last)]
            public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if ((int)name >= 60000)
                {
                    if (ColorStrings.TryGetValue((int)name, out var text))
                    {
                        __result = $"color{text}".Translate();
                        return false;
                    }
                    else
                    {
                        Error($"Key '{(int)name}' not found in ColorStrings.");
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.SetUp))]
        private class ChatNotificationColorsPatch
        {
            public static bool Prefix(ChatNotification __instance, PlayerControl sender, string text)
            {
                if (ShipStatus.Instance && !ModOption.ShowChatNotifications)
                {
                    return false;
                }
                __instance.timeOnScreen = 5f;
                __instance.gameObject.SetActive(true);
                __instance.SetCosmetics(sender.Data);
                string str;
                Color color;
                try
                {
                    str = ColorUtility.ToHtmlStringRGB(Palette.TextColors[__instance.player.ColorId]);
                    color = Palette.TextOutlineColors[__instance.player.ColorId];
                }
                catch
                {
                    Color32 c = Palette.PlayerColors[__instance.player.ColorId];
                    str = ColorUtility.ToHtmlStringRGB(c);
                    color = c.r + c.g + c.b > 180 ? Palette.Black : Palette.White;
                    //Message($"{c.r}, {c.g}, {c.b}");
                }
                __instance.playerColorText.text = __instance.player.ColorBlindName;
                __instance.playerNameText.text = "<color=#" + str + ">" + (string.IsNullOrEmpty(sender.Data.PlayerName) ? "..." : sender.Data.PlayerName);
                __instance.playerNameText.outlineColor = color;
                __instance.chatText.text = text;
                return false;
            }
        }
        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
        private static class PlayerTabEnablePatch
        {
            public static void Postfix(PlayerTab __instance)
            {
                // Replace instead
                var chips = __instance.ColorChips.ToArray();
                // TODO: Design an algorithm to dynamically position chips to optimally fill space
                var cols = 8;

                for (var i = 0; i < chips.Count; i++)
                {
                    var chip = chips[i];
                    int row = i / cols, col = i % cols; // Dynamically do the positioningS
                    chip.transform.localPosition = new Vector3(-0.975f + col * 0.5f, 1.475f - row * 0.5f, chip.transform.localPosition.z);
                    chip.transform.localScale *= 0.76f;
                }
                for (var j = chips.Count; j < chips.Length; j++)
                {
                    // If number isn't in order, hide it
                    var chip = chips[j];
                    chip.transform.localScale *= 0f;
                    chip.enabled = false;
                    chip.Button.enabled = false;
                    chip.Button.OnClick.RemoveAllListeners();
                }
            }
        }

        [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.LoadPlayerPrefs))]
        private static class LoadPlayerPrefsPatch
        {
            // Fix Potential issues with broken colors
            private static bool needsPatch;

            public static void Prefix([HarmonyArgument(0)] bool overrideLoad)
            {
                if (!LegacySaveManager.loaded || overrideLoad)
                    needsPatch = true;
            }

            public static void Postfix()
            {
                if (!needsPatch) return;
                LegacySaveManager.colorConfig %= pickableColors;
                needsPatch = false;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor))]
        private static class PlayerControlCheckColorPatch
        {
            private static bool isTaken(PlayerControl player, uint color)
            {
                foreach (var p in GameData.Instance.AllPlayers.GetFastEnumerator())
                    if (!p.Disconnected && p.PlayerId != player.PlayerId && p.DefaultOutfit.ColorId == color)
                        return true;
                return false;
            }

            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte bodyColor)
            {
                // Fix incorrect color assignment
                uint color = bodyColor;
                if (isTaken(__instance, color) || color >= Palette.PlayerColors.Length)
                {
                    var num = 0;
                    while (num++ < 50 && (color >= pickableColors || isTaken(__instance, color)))
                        color = (color + 1) % pickableColors;
                }

                __instance.RpcSetColor((byte)color);
                return false;
            }
        }
    }
}