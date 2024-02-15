using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Helper;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch
    {
        public static string fullCredentialsVersion =
$@"<size=130%><color=#ff351f>我们的超多职业</color></size> v{TheOtherRolesPlugin.Version.ToString() + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}";
        public static string fullCredentials =
        $@"<size=70%>模组制作：<color=#FCCE03FF>Spex</color>
模组修改：<color=#FFB793>沫夏悠轩</color>, <color=#FCCE03FF>善良的好人</color>
汉化:<color=#FFB793>沫夏悠轩</color></size>";

        public static string mainMenuCredentials =
    $@"模组作者: <color=#FCCE03FF>Spex</color>
<size=85%>模组修改：<color=#FFB793>沫夏悠轩</color>, <color=#FCCE03FF>善良的好人</color>
汉化：<color=#FFB793>沫夏悠轩</color></size>";

        public static string contributorsCredentials =
$@"<size=70%> <color=#FCCE03FF>特别感谢 Smeggy, Scoom, Xer, and Mr_Fluuff</color></size>";

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        internal static class PingTrackerPatch
        {
            public static GameObject modStamp;

            static void Postfix(PingTracker __instance)
            {
                __instance.text.alignment = TMPro.TextAlignmentOptions.TopRight;
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    string gameModeText = $"";
                    if (HideNSeek.isHideNSeekGM) gameModeText = $"躲猫猫模式";
                    else if (HandleGuesser.isGuesserGm) gameModeText = $"赌怪模式";
                    else if (PropHunt.isPropHuntGM) gameModeText = "道具躲猫猫";
                    if (gameModeText != "") gameModeText = Helpers.cs(Color.yellow, gameModeText) + "\n";
                    __instance.text.text = $"<size=120%><color=#ff351f>我们的超多职业</color></size> v{TheOtherRolesPlugin.Version.ToString() + ("\n<size=75%><color=#FFB793>沫夏悠轩</color> - <color=#FFB793>mxyx.club</color></size>")}\n<size=90%>{gameModeText}</size>" + __instance.text.text;
                    if (CachedPlayer.LocalPlayer.Data.IsDead || (!(CachedPlayer.LocalPlayer.PlayerControl == null) && (CachedPlayer.LocalPlayer.PlayerControl == Lovers.lover1 || CachedPlayer.LocalPlayer.PlayerControl == Lovers.lover2)))
                    {
                        var transform = __instance.transform;
                        var localPosition = transform.localPosition;
                        localPosition = new Vector3(3.45f, localPosition.y, localPosition.z);
                        transform.localPosition = localPosition;
                    }
                    else
                    {
                        var transform = __instance.transform;
                        var localPosition = transform.localPosition;
                        localPosition = new Vector3(4.2f, localPosition.y, localPosition.z);
                        transform.localPosition = localPosition;
                    }
                }
                else
                {
                    string gameModeText = $"";
                    if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek) gameModeText = $"躲猫猫模式";
                    else if (TORMapOptions.gameMode == CustomGamemodes.Guesser) gameModeText = $"赌怪模式";
                    else if (TORMapOptions.gameMode == CustomGamemodes.PropHunt) gameModeText = $"道具躲猫猫";
                    if (gameModeText != "") gameModeText = Helpers.cs(Color.yellow, gameModeText) + "\n";

                    __instance.text.text = $"{fullCredentialsVersion}\n  {gameModeText + fullCredentials}\n {__instance.text.text}";
                    var transform = __instance.transform;
                    var localPosition = transform.localPosition;
                    localPosition = new Vector3(3.5f, localPosition.y, localPosition.z);
                    transform.localPosition = localPosition;
                }
            }
        }

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static class LogoPatch
        {
            public static SpriteRenderer renderer;
            public static Sprite bannerSprite;
            public static Sprite horseBannerSprite;
            public static Sprite banner2Sprite;
            private static PingTracker instance;

            public static GameObject motdObject;
            public static TextMeshPro motdText;

            static void Postfix(PingTracker __instance)
            {
                var torLogo = new GameObject("bannerLogo_TOR");
                torLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
                torLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);

                renderer = torLogo.AddComponent<SpriteRenderer>();
                loadSprites();
                renderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Banner.png", 300f);

                instance = __instance;
                loadSprites();
                // renderer.sprite = TORMapOptions.enableHorseMode ? horseBannerSprite : bannerSprite;
                renderer.sprite = EventUtility.isEnabled ? banner2Sprite : bannerSprite;
                var credentialObject = new GameObject("credentialsTOR");
                var credentials = credentialObject.AddComponent<TextMeshPro>();
                credentials.SetText($"v{TheOtherRolesPlugin.Version.ToString() + (TheOtherRolesPlugin.betaDays > 0 ? "-BETA" : "")}\n<size=30f%>\n</size>{mainMenuCredentials}\n<size=30%>\n</size>{contributorsCredentials}");
                credentials.alignment = TMPro.TextAlignmentOptions.Center;
                credentials.fontSize *= 0.05f;

                credentials.transform.SetParent(torLogo.transform);
                credentials.transform.localPosition = Vector3.down * 1.25f;
                motdObject = new GameObject("torMOTD");
                motdText = motdObject.AddComponent<TextMeshPro>();
                motdText.alignment = TMPro.TextAlignmentOptions.Center;
                motdText.fontSize *= 0.04f;

                motdText.transform.SetParent(torLogo.transform);
                motdText.enableWordWrapping = true;
                var rect = motdText.gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(5.2f, 0.25f);

                motdText.transform.localPosition = Vector3.down * 2.25f;
                motdText.color = new Color(1, 53f / 255, 31f / 255);
                Material mat = motdText.fontSharedMaterial;
                mat.shaderKeywords = new[] { "OUTLINE_ON" };
                motdText.SetOutlineColor(Color.white);
                motdText.SetOutlineThickness(0.025f);
            }

            public static void loadSprites()
            {
                if (bannerSprite == null) bannerSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Banner.png", 300f);
                if (banner2Sprite == null) banner2Sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Banner2.png", 300f);
                if (horseBannerSprite == null) horseBannerSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.bannerTheHorseRoles.png", 300f);
            }

            public static void updateSprite()
            {
                loadSprites();
                if (renderer != null)
                {
                    float fadeDuration = 1f;
                    instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) =>
                    {
                        renderer.color = new Color(1, 1, 1, 1 - p);
                        if (p == 1)
                        {
                            renderer.sprite = TORMapOptions.enableHorseMode ? horseBannerSprite : bannerSprite;
                            instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>((p) =>
                            {
                                renderer.color = new Color(1, 1, 1, p);
                            })));
                        }
                    })));
                }
            }
        }

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
        public static class MOTD
        {
            public static List<string> motds = new();
            private static float timer = 0f;
            private static float maxTimer = 5f;
            private static int currentIndex = 0;

            public static void Postfix()
            {
                if (motds.Count == 0)
                {
                    timer = maxTimer;
                    return;
                }
                if (motds.Count > currentIndex && LogoPatch.motdText != null)
                    LogoPatch.motdText.SetText(motds[currentIndex]);
                else return;

                // fade in and out:
                float alpha = Mathf.Clamp01(Mathf.Min(new float[] { timer, maxTimer - timer }));
                if (motds.Count == 1) alpha = 1;
                LogoPatch.motdText.color = LogoPatch.motdText.color.SetAlpha(alpha);
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = maxTimer;
                    currentIndex = (currentIndex + 1) % motds.Count;
                }
            }

            public static async Task loadMOTDs()
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://api.mxyx.club/download/among-us/TOUs/home.txt");
                response.EnsureSuccessStatusCode();
                string motds = await response.Content.ReadAsStringAsync();
                foreach (string line in motds.Split("\n", StringSplitOptions.RemoveEmptyEntries))
                {
                    MOTD.motds.Add(line);
                }
            }
        }
    }
}
