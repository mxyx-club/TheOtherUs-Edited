using System.Collections.Generic;
using AmongUs.GameOptions;
using InnerNet;
using TMPro;
using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
public static class CredentialsPatch
{
    public static string fullCredentialsVersion = $"<size=130%>{GetString("TouTitle")}</size> v{Main.Version + "-Lite"}";

    public static string fullCredentials = GetString("fullCredentials");

    public static string mainMenuCredentials = GetString("mainMenuCredentials");

    public static string contributorsCredentials = GetString("contributorsCredentials");

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    internal static class PingTrackerPatch
    {
        private static float DeltaTime;

        private static void Postfix(PingTracker __instance)
        {
            DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;
            var fps = Mathf.Ceil(1f / DeltaTime);
            var pingText = $"<size=80%>Ping: {AmongUsClient.Instance.Ping}ms{(ModOption.showFPS ? $"  FPS: {fps}" : "")}</size>";
            var host = $"<size=80%>{"Host".Translate()}: {GameData.Instance?.GetHost()?.PlayerName}</size>";
            __instance.text.SetOutlineThickness(0.01f);
            var position = __instance.GetComponent<AspectPosition>();
            var gameModeText = ModOption.gameMode switch
            {
                CustomGamemodes.Guesser => GetString("isGuesserGm"),
                _ => ""
            };
            if (ModOption.DebugMode) gameModeText += "(Debug Mode)";

            if (!string.IsNullOrEmpty(gameModeText)) gameModeText = cs(Color.yellow, gameModeText) + "\n";

            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
            {
                __instance.text.alignment = TextAlignmentOptions.TopRight;
                position.Alignment = AspectPosition.EdgeAlignments.RightTop;
                __instance.text.text = $"<size=110%>{GetString("TouTitle")}</size>  v{Main.Version}\n{GetString("inGameTitle")}\n{gameModeText}{pingText}";
                position.DistanceFromEdge = new Vector3(2.7f, 0.1f, 0);
            }
            else
            {
                __instance.text.alignment = TextAlignmentOptions.TopLeft;
                position.Alignment = AspectPosition.EdgeAlignments.LeftTop;
                __instance.text.text = $"{fullCredentialsVersion}\n{pingText}\n{gameModeText}{fullCredentials}\n{host}";
                position.DistanceFromEdge = new(0.4f, 0.06f);
                try
                {
                    UpdateGameModeText();
                }
                catch { }
            }
            position.AdjustPosition();
        }

        private static void UpdateGameModeText()
        {
            var gameModeText = ModOption.gameMode switch
            {
                CustomGamemodes.Guesser => GetString("isGuesserGm"),
                CustomGamemodes.Classic => GetString("isClassicGM"),
                _ => ""
            };
            gameModeText = cs(Color.yellow, gameModeText);
            var GameModeText = GameObject.Find("GameModeText")?.GetComponent<TextMeshPro>();
            GameModeText.text = string.IsNullOrEmpty(gameModeText) ? (GameOptionsManager.Instance.currentGameOptions.GameMode
                == GameModes.HideNSeek ? "isVanHideNSeekGM".Translate() : "isClassicGM".Translate()) : gameModeText;
            var modeLabel = GameObject.Find("ModeLabel")?.GetComponentInChildren<TextMeshPro>();
            modeLabel.text = "GameMode".Translate();
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class LogoPatch
    {
        public static SpriteRenderer renderer;
        private static PingTracker instance;

        public static GameObject motdObject;
        public static TextMeshPro motdText;

        private static void Postfix(PingTracker __instance)
        {
            var torLogo = new GameObject("bannerLogo_TOR");
            torLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
            torLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);

            renderer = torLogo.AddComponent<SpriteRenderer>();
            if (IsCN()) renderer.sprite = new ResourceSprite("TheOtherRoles.Resources.Banner2.png", 270f);
            else renderer.sprite = new ResourceSprite("TheOtherRoles.Resources.Banner.png", 300f);
            instance = __instance;
            var credentialObject = new GameObject("credentialsTOR");
            var credentials = credentialObject.AddComponent<TextMeshPro>();
            credentials.SetText(
                $"<size=90%>TheOtherUs-Edited v{Main.Version + "-Lite"}</size>\n<size=30%>\n</size>{mainMenuCredentials}\n<size=30%>\n</size>{contributorsCredentials}");
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