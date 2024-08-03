using System;
using System.Collections.Generic;
using AmongUs.GameOptions;
using InnerNet;
using TheOtherRoles.Modules;
using TMPro;
using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
public static class CredentialsPatch
{
    public static string fullCredentialsVersion = $"<size=130%>{getString("TouTitle")}</size> v{Main.Version + (Main.betaDays > 0 ? "-BETA" : "")}";

    public static string fullCredentials = getString("fullCredentials");

    public static string mainMenuCredentials = getString("mainMenuCredentials");

    public static string contributorsCredentials = getString("contributorsCredentials");

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    internal static class PingTrackerPatch
    {
        private static float DeltaTime;

        private static void Postfix(PingTracker __instance)
        {

            DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;
            var fps = Mathf.Ceil(1f / DeltaTime);
            var PingText = $"<size=80%>Ping: {AmongUsClient.Instance.Ping}ms{(MapOption.showFPS ? $"  FPS: {fps}" : "")}</size>";
            var host = $"<size=80%>{"Host".Translate()}: {GameData.Instance?.GetHost()?.PlayerName}</size>";

            var position = __instance.GetComponent<AspectPosition>();
            var gameModeText = MapOption.gameMode switch
            {
                CustomGamemodes.HideNSeek => getString("isHideNSeekGM"),
                CustomGamemodes.Guesser => getString("isGuesserGm"),
                CustomGamemodes.PropHunt => getString("isPropHuntGM"),
                _ => ""
            };
            if (MapOption.DebugMode) gameModeText += "(Debug Mode)";
            if (gameModeText != "") gameModeText = cs(Color.yellow, gameModeText) + "\n";
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
            {
                __instance.text.alignment = TextAlignmentOptions.TopRight;
                position.Alignment = AspectPosition.EdgeAlignments.RightTop;
                __instance.text.text = $"<size=110%>{getString("TouTitle")}</size>  v{Main.Version}\n{getString("inGameTitle")}\n{gameModeText}{PingText}";
                position.DistanceFromEdge = new Vector3(2.7f, 0.1f, 0);
            }
            else
            {
                __instance.text.alignment = TextAlignmentOptions.TopLeft;
                position.Alignment = AspectPosition.EdgeAlignments.LeftTop;
                __instance.text.text = $"{fullCredentialsVersion}\n{PingText}\n{gameModeText}{fullCredentials}\n{host}";
                position.DistanceFromEdge = new(0.4f, 0.06f);
                try
                {
                    var GameModeText = GameObject.Find("GameModeText")?.GetComponent<TextMeshPro>();
                    GameModeText.text = gameModeText == "" ? (GameOptionsManager.Instance.currentGameOptions.GameMode
                        == GameModes.HideNSeek ? "isVan.HideNSeekGM".Translate() : "isClassicGM".Translate()) : gameModeText;
                    var ModeLabel = GameObject.Find("ModeLabel")?.GetComponentInChildren<TextMeshPro>();
                    ModeLabel.text = "GameMode".Translate();
                }
                catch { }
            }
            position.AdjustPosition();
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

        private static void Postfix(PingTracker __instance)
        {
            var torLogo = new GameObject("bannerLogo_TOR");
            torLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
            torLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);

            renderer = torLogo.AddComponent<SpriteRenderer>();
            loadSprites();
            renderer.sprite = loadSpriteFromResources("TheOtherRoles.Resources.Banner.png", 300f);

            instance = __instance;
            loadSprites();
            // renderer.sprite = TORMapOptions.enableHorseMode ? horseBannerSprite : bannerSprite;
            renderer.sprite = bannerSprite;
            var credentialObject = new GameObject("credentialsTOR");
            var credentials = credentialObject.AddComponent<TextMeshPro>();
            credentials.SetText(
                $"<size=90%>TheOtherUs-Edited v{Main.Version + (Main.betaDays > 0 ? "-BETA" : "")}</size>\n<size=30%>\n</size>{mainMenuCredentials}\n<size=30%>\n</size>{contributorsCredentials}");
            credentials.alignment = TextAlignmentOptions.Center;
            credentials.fontSize *= 0.05f;

            credentials.transform.SetParent(torLogo.transform);
            credentials.transform.localPosition = Vector3.down * 1.5f;
            motdObject = new GameObject("torMOTD");
            motdText = motdObject.AddComponent<TextMeshPro>();
            motdText.alignment = TextAlignmentOptions.Center;
            motdText.fontSize *= 0.04f;

            motdText.transform.SetParent(torLogo.transform);
            motdText.enableWordWrapping = true;
            var rect = motdText.gameObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(5.2f, 0.25f);

            motdText.transform.localPosition = Vector3.down * 2.25f;
            motdText.color = new Color(1, 53f / 255, 31f / 255);
            var mat = motdText.fontSharedMaterial;
            mat.shaderKeywords = new[] { "OUTLINE_ON" };
            motdText.SetOutlineColor(Color.white);
            motdText.SetOutlineThickness(0.025f);
        }

        public static void loadSprites()
        {
            if (bannerSprite == null)
                bannerSprite = loadSpriteFromResources("TheOtherRoles.Resources.Banner.png", 300f);
            if (banner2Sprite == null)
                banner2Sprite = loadSpriteFromResources("TheOtherRoles.Resources.Banner2.png", 300f);
            if (horseBannerSprite == null)
                horseBannerSprite =
                    loadSpriteFromResources("TheOtherRoles.Resources.bannerTheHorseRoles.png", 300f);
        }

        public static void updateSprite()
        {
            loadSprites();
            if (renderer != null)
            {
                var fadeDuration = 1f;
                instance.StartCoroutine(Effects.Lerp(fadeDuration, new Action<float>(p =>
                {
                    renderer.color = new Color(1, 1, 1, 1 - p);
                    if (p == 1)
                    {
                        renderer.sprite = bannerSprite;
                        instance.StartCoroutine(Effects.Lerp(fadeDuration,
                            new Action<float>(p => { renderer.color = new Color(1, 1, 1, p); })));
                    }
                })));
            }
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
    public static class MOTD
    {
        public static List<string> motds = new();
        private static float timer;
        private static readonly float maxTimer = 5f;
        private static int currentIndex;

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
            var alpha = Mathf.Clamp01(Mathf.Min(new[] { timer, maxTimer - timer }));
            if (motds.Count == 1) alpha = 1;
            LogoPatch.motdText.color = LogoPatch.motdText.color.SetAlpha(alpha);
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = maxTimer;
                currentIndex = (currentIndex + 1) % motds.Count;
            }
        }
    }
}