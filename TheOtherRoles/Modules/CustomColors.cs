using System.Collections.Generic;
using System.Linq;
using AmongUs.Data.Legacy;
using TheOtherRoles.Utilities;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace TheOtherRoles.Modules;

public class CustomColors
{
    protected static Dictionary<int, string> ColorStrings = [];
    public static List<int> lighterColors = [3, 4, 5, 7, 10, 11, 13, 14, 17];
    public static uint pickableColors = (uint)Palette.ColorNames.Length;

    private static readonly List<int> ORDER =
    [
        0, 1, 2, 3, 4, 5, 6, 7,
        8, 9, 10, 11, 12, 13, 14, 15,
        16, 17, 37, 33, 41, 25, 30, 35,
        27, 23, 32, 38, 21, 40, 31, 34,
        22, 28, 36, 26, 29, 20, 19, 18,
        24, 39, 42, 43, 44, 45, 46, 47,
        48, 49, 50, 51, 52, 53, 54, 55,
    ];

    //private static readonly Dictionary<Color, (Color32, Color32, bool)> CustomColorData = [];

    public static void Load()
    {
        var longlist = Palette.ColorNames.ToList();
        var colorlist = Palette.PlayerColors.ToList();
        var shadowlist = Palette.ShadowColors.ToList();

        var colors = new List<CustomColor>
        {
            new() { // 18
                longname = "colorTamarind",
                color = new Color32(48, 28, 34, byte.MaxValue),
                shadow = new Color32(30, 11, 16, byte.MaxValue),
                isLighterColor = true
            },
            new() { // 19
                longname = "colorArmy",
                color = new Color32(39, 45, 31, byte.MaxValue),
                shadow = new Color32(11, 30, 24, byte.MaxValue),
                isLighterColor = false
            },
            // 20
            new() {
                longname = "colorOlive",
                color = new Color32(154, 140, 61, byte.MaxValue),
                shadow = new Color32(104, 95, 40, byte.MaxValue),
                isLighterColor = true
            },
            new() {
                longname = "colorTurquoise",
                color = new Color32(22, 132, 176, byte.MaxValue),
                shadow = new Color32(15, 89, 117, byte.MaxValue),
                isLighterColor = false
            },
            new() {
                longname = "colorMint",
                color = new Color32(111, 192, 156, byte.MaxValue),
                shadow = new Color32(65, 148, 111, byte.MaxValue),
                isLighterColor = true
            },
            new() {
                longname = "colorLavender",
                color = new Color32(173, 126, 201, byte.MaxValue),
                shadow = new Color32(131, 58, 203, byte.MaxValue),
                isLighterColor = true
            },
            new() { // 24
                
                longname = "colorPitchBlack",
                color = new Color32(0, 0, 0, byte.MaxValue),
                shadow = new Color32(0, 0, 0, byte.MaxValue),
                isLighterColor = false
            },
            new() {
                longname = "colorPeach",
                color = new Color32(255, 164, 119, byte.MaxValue),
                shadow = new Color32(238, 128, 100, byte.MaxValue),
                isLighterColor = true
            },
            new() {
                longname = "colorWasabi",
                color = new Color32(112, 143, 46, byte.MaxValue),
                shadow = new Color32(72, 92, 29, byte.MaxValue),
                isLighterColor = false
            },
            new() {
                longname = "colorHotPink",
                color = new Color32(255, 51, 102, byte.MaxValue),
                shadow = new Color32(232, 0, 58, byte.MaxValue),
                isLighterColor = true
            },
            new() {
                longname = "colorPetrol",
                color = new Color32(0, 99, 105, byte.MaxValue),
                shadow = new Color32(0, 61, 54, byte.MaxValue),
                isLighterColor = false
            },
            new() {
                longname = "colorLemon",
                color = new Color32(219, 253, 47, byte.MaxValue),
                shadow = new Color32(116, 229, 16, byte.MaxValue),
                isLighterColor = true
            },

            new() // 30
            {
                longname = "colorSignalOrange",
                color = new Color32(247, 68, 23, byte.MaxValue),
                shadow = new Color32(155, 46, 15, byte.MaxValue),
                isLighterColor = true
            },
            new() {
                longname = "colorTeal",
                color = new Color32(37, 184, 191, byte.MaxValue),
                shadow = new Color32(18, 137, 134, byte.MaxValue),
                isLighterColor = true
            },
            new() {
                longname = "colorBlurple",
                color = new Color32(61, 44, 142, byte.MaxValue),
                shadow = new Color32(25, 14, 90, byte.MaxValue),
                isLighterColor = false
            },
            new() {
                longname = "colorSunrise",
                color = new Color32(255, 202, 25, byte.MaxValue),
                shadow = new Color32(219, 68, 66, byte.MaxValue),
                isLighterColor = true
            },
            new() {
                longname = "colorIce",
                color = new Color32(0xA8, 0xDF, 0xFF, byte.MaxValue),
                shadow = new Color32(0x59, 0x9F, 0xC8, byte.MaxValue),
                isLighterColor = true
            },
            new() // 35
            {
                longname = "colorDarkness",
                color = new Color32(36, 39, 40, byte.MaxValue),
                shadow = new Color32(10, 10, 10, byte.MaxValue),
                isLighterColor = false
            },
            new() //36
            {
                longname = "colorRoyalGreen",
                color = new Color32(9, 82, 33, byte.MaxValue),
                shadow = new Color32(0, 46, 8, byte.MaxValue),
                isLighterColor = false
            },
            new() // 37
            {
                longname = "colorSlime",
                color = new Color32(244, 255, 188, byte.MaxValue),
                shadow = new Color32(167, 239, 112, byte.MaxValue),
                isLighterColor = false
            },
            new() // 38
            {
                longname = "colorNavy",
                color = new Color32(9, 43, 119, byte.MaxValue),
                shadow = new Color32(0, 13, 56, byte.MaxValue),
                isLighterColor = false
            },
            new() // 39
            {
                longname = "colorNega",
                color = new Color32(0, 0, 0, byte.MaxValue),
                shadow = new Color32(255, 255, 255, byte.MaxValue),
                isLighterColor = false
            },
            new() // 40
            {
                longname = "colorOcean",
                color = new Color32(55, 159, 218, byte.MaxValue),
                shadow = new Color32(62, 92, 158, byte.MaxValue),
                isLighterColor = false
            },
            new() // 41
            {
                longname = "colorSundown",
                color = new Color32(252, 194, 100, byte.MaxValue),
                shadow = new Color32(197, 98, 54, byte.MaxValue),
                isLighterColor = false
            },
            new() // 42
            {
                longname = "colorSnow",
                color = new Color32(229, 249, 255, byte.MaxValue),
                shadow = new Color32(135, 226, 255, byte.MaxValue),
                isLighterColor = true
            },
            new() // 43
            {
                longname = "colorSkyBlue",
                color = new Color32(89, 210, 255, byte.MaxValue),
                shadow = new Color32(37, 169, 232, byte.MaxValue),
                isLighterColor = true
            },
            new() // 44
            {
                longname = "colorFuchsia",
                color = new Color32(164, 17, 129, byte.MaxValue),
                shadow = new Color32(104, 3, 79, byte.MaxValue),
                isLighterColor = false
            },
            new() // 45
            {
                longname = "colorAzuki",
                color = new Color32(150, 81, 77, byte.MaxValue),
                shadow = new Color32(115, 40, 35, byte.MaxValue),
                isLighterColor = false
            },
            new() // 46
            {
                longname = "colorTokiwaGreen",
                color = new Color32(0, 123, 67, byte.MaxValue),
                shadow = new Color32(0, 84, 83, byte.MaxValue),
                isLighterColor = false
            },
            new() // 47
            {
                longname = "colorNougat",
                color = new Color32(160, 101, 56, byte.MaxValue),
                shadow = new Color32(115, 15, 78, byte.MaxValue),
                isLighterColor = false
            },
            new() // 48
            {
                longname = "colorPitchwhite",
                color = new Color32(255, 255, 255, byte.MaxValue),
                shadow = new Color32(255, 255, 255, byte.MaxValue),
                isLighterColor = true
            },
            new() // 49
            {
                longname = "colorPosi",
                color = new Color32(255, 255, 255, byte.MaxValue),
                shadow = new Color32(0, 0, 0, byte.MaxValue),
                isLighterColor = true
            },
            new() // 50
            {
                longname = "colorEmerald",
                color = new Color32(98, 214, 133, byte.MaxValue),
                shadow = new Color32(82, 179, 111, byte.MaxValue),
                isLighterColor = true
            },
            new() // 51
            {
                longname = "colorPeach",
                color = new Color32(255, 164, 119, byte.MaxValue),
                shadow = new Color32(238, 128, 100, byte.MaxValue),
                isLighterColor = true
            },
            new() // 52
            {
                longname = "colorGold",
                color = new Color32(255, 216, 70, byte.MaxValue),
                shadow = new Color32(226, 168, 13, byte.MaxValue),
                isLighterColor = true
            },
            new() // 53
            {
                longname = "colorIntenseblue",
                color = new Color32(83, 136, 255, byte.MaxValue),
                shadow = new Color32(76, 122, 230, byte.MaxValue),
                isLighterColor = true
            },
            new() // 54
            {
                longname = "colorBlurple",
                color = new Color32(89, 60, 214, byte.MaxValue),
                shadow = new Color32(41, 23, 150, byte.MaxValue),
                isLighterColor = false
            },
            new() // 55
            {
                longname = "colorXmasRed",
                color = new Color32(219, 41, 41, byte.MaxValue),
                shadow = new Color32(255, 255, 255, byte.MaxValue),
                isLighterColor = true
            },
        };
        pickableColors += (uint)colors.Count; // Colors to show in Tab
        /** Hidden Colors **/

        /** Add Colors **/
        var id = 50000;
        foreach (var cc in colors)
        {
            longlist.Add((StringNames)id);
            ColorStrings[id++] = cc.longname;
            colorlist.Add(cc.color);
            shadowlist.Add(cc.shadow);
            if (cc.isLighterColor)
                lighterColors.Add(colorlist.Count - 1);
        }

        Palette.ColorNames = longlist.ToArray();
        Palette.PlayerColors = colorlist.ToArray();
        Palette.ShadowColors = shadowlist.ToArray();
    }

    protected internal struct CustomColor
    {
        public string longname;
        public Color32 color;
        public Color32 shadow;
        public bool isLighterColor;
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
                if ((int)name >= 50000)
                {
                    var text = ColorStrings[(int)name];
                    if (text != null)
                    {
                        __result = text.Translate();
                        return false;
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
        private static class PlayerTabEnablePatch
        {
            public static void Postfix(PlayerTab __instance)
            {
                // Replace instead
                var chips = __instance.ColorChips.ToArray();

                var cols = 8; // TODO: Design an algorithm to dynamically position chips to optimally fill space
                for (var i = 0; i < ORDER.Count; i++)
                {
                    var pos = ORDER[i];
                    if (pos < 0 || pos > chips.Length)
                        continue;
                    var chip = chips[pos];
                    int row = i / cols, col = i % cols; // Dynamically do the positioning
                    chip.transform.localPosition = new Vector3(-0.975f + (col * 0.5f), 1.475f - (row * 0.5f),
                        chip.transform.localPosition.z);
                    chip.transform.localScale *= 0.76f;
                }

                for (var j = ORDER.Count; j < chips.Length; j++)
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