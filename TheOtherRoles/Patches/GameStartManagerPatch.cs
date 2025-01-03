using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hazel;
using Reactor.Utilities.Extensions;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Patches;

public class GameStartManagerPatch
{
    public static Dictionary<int, PlayerVersion> playerVersions = new();
    public static float timer = 600f;
    private static float kickingTimer;
    private static bool versionSent;

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    public class AmongUsClientOnPlayerJoinedPatch
    {
        public static void Postfix(AmongUsClient __instance)
        {
            if (CachedPlayer.LocalPlayer != null)
            {
                shareGameVersion();
            }
            GameStartManagerUpdatePatch.sendGamemode = true;
        }
    }

    [HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
    public static class ConstantsPatch
    {
        public static void Postfix(ref int __result)
        {
            if (!Main.ForceUsePlus25Protocol.Value) return;
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame) __result += 25;
            Message("强制使用 +25 协议", "NetworkModes");
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public class GameStartManagerStartPatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            // Trigger version refresh
            versionSent = false;
            // Reset lobby countdown timer
            timer = 600f;
            // Reset kicking timer
            kickingTimer = 0f;
            // Copy lobby code
            string code = InnerNet.GameCode.IntToGameName(AmongUsClient.Instance.GameId);
            GUIUtility.systemCopyBuffer = code;
            _ = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.RoomCode,
                new Il2CppReferenceArray<Il2CppSystem.Object>(0)) + "\r\n" + code;
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public class GameStartManagerUpdatePatch
    {
        public static float startingTimer;
        private static bool update;
        private static string currentText = "";
        private static GameObject copiedStartButton;
        public static bool sendGamemode = true;

        public static void Prefix(GameStartManager __instance)
        {
            if (!GameData.Instance) return; // No instance
            __instance.MinPlayers = 1;
            update = GameData.Instance.PlayerCount != __instance.LastPlayerCount;
        }

        public static void Postfix(GameStartManager __instance)
        {
            // Send version as soon as CachedPlayer.LocalPlayer.PlayerControl exists
            if (PlayerControl.LocalPlayer != null && !versionSent)
            {
                versionSent = true;
                shareGameVersion();
            }
#if DEBUG
                return;
#endif
            // Check version handshake infos

            bool versionMismatch = false;
            string message = "";
            foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients.ToArray())
            {
                if (client.Character == null) continue;
                var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
                if (dummyComponent != null && dummyComponent.enabled) continue;
                else if (!playerVersions.ContainsKey(client.Id))
                {
                    versionMismatch = true;
                    message += $"<color=#FF0000FF>{string.Format(GetString("errorNotInstalled"), $"{client.Character.Data.PlayerName}")}\n</color>";
                }
                else
                {
                    PlayerVersion PV = playerVersions[client.Id];
                    int diff = TheOtherRolesPlugin.Version.CompareTo(PV.version);
                    if (diff > 0)
                    {
                        message += $"<color=#FF0000FF>{string.Format(GetString("errorOlderVersion"), $"{client.Character.Data.PlayerName}")} (v{playerVersions[client.Id].version})\n</color>";
                        versionMismatch = true;
                    }
                    else if (diff < 0)
                    {
                        message += $"<color=#FF0000FF>{string.Format(GetString("errorNewerVersion"), $"{client.Character.Data.PlayerName}")} (v{playerVersions[client.Id].version})\n</color>";
                        versionMismatch = true;
                    }
                    else if (!PV.GuidMatches())
                    {
                        // version presumably matches, check if Guid matches
                        message += $"<color=#FF0000FF>{string.Format(GetString("errorWrongVersion"), $"{client.Character.Data.PlayerName}")} v{playerVersions[client.Id].version} <size=30%>({PV.guid})</size>\n</color>";
                        versionMismatch = true;
                    }
                }
            }
            // Display message to the host
            if (AmongUsClient.Instance.AmHost)
            {
                if (versionMismatch)
                {
                    __instance.GameStartText.text = message;
                    __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                    __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                    __instance.GameStartTextParent.SetActive(true);
                }
                else
                {
                    __instance.GameStartText.transform.localPosition = Vector3.zero;
                    __instance.GameStartText.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                    if (!__instance.GameStartText.text.StartsWith(FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameStarting).Replace("{0}", "")) && CustomOptionHolder.anyPlayerCanStopStart.GetBool())
                    {
                        __instance.GameStartText.text = string.Empty;
                        __instance.GameStartTextParent.SetActive(false);
                    }
                }

                if (__instance.startState != GameStartManager.StartingStates.Countdown)
                    copiedStartButton?.Destroy();

                // Make starting info available to clients:
                if (startingTimer <= 0 && __instance.startState == GameStartManager.StartingStates.Countdown)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetGameStarting, SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setGameStarting();

                    // Activate Stop-Button
                    copiedStartButton = UnityEngine.Object.Instantiate(__instance.StartButton.gameObject, __instance.StartButton.gameObject.transform.parent);
                    copiedStartButton.transform.localPosition = __instance.StartButton.transform.localPosition;
                    copiedStartButton.SetActive(true);
                    var startButtonText = copiedStartButton.GetComponentInChildren<TMPro.TextMeshPro>();
                    startButtonText.text = "";
                    startButtonText.fontSize *= 0.68f;
                    startButtonText.fontSizeMax = startButtonText.fontSize;
                    startButtonText.gameObject.transform.localPosition = Vector3.zero;
                    PassiveButton startButtonPassiveButton = copiedStartButton.GetComponent<PassiveButton>();

                    void StopStartFunc()
                    {
                        __instance.ResetStartState();
                        copiedStartButton.Destroy();
                        startingTimer = 0;
                    }
                    startButtonPassiveButton.OnClick.AddListener((Action)(() => StopStartFunc()));
                    __instance.StartCoroutine(Effects.Lerp(.1f, new Action<float>((p) =>
                    {
                        startButtonText.text = "";
                    })));
                }
            }

            // Client update with handshake infos
            else
            {
                if (!playerVersions.ContainsKey(AmongUsClient.Instance.HostId) || TheOtherRolesPlugin.Version.CompareTo(playerVersions[AmongUsClient.Instance.HostId].version) != 0)
                {
                    kickingTimer += Time.deltaTime;
                    if (kickingTimer > 10)
                    {
                        kickingTimer = 0;
                        AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                        SceneChanger.ChangeScene("MainMenu");
                    }

                    __instance.GameStartText.text = $"<color=#FF0000FF>{string.Format(GetString("errorHostNoVersion"), Math.Round(10 - kickingTimer))}</color>";
                    __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                    __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                    __instance.GameStartTextParent.SetActive(true);
                }
                else if (versionMismatch)
                {
                    __instance.GameStartText.text = $"<color=#FF0000FF>{GetString("errorDifferentVersion")}\n</color>" + message;
                    __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                    __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                    __instance.GameStartTextParent.SetActive(true);
                }
                else
                {
                    __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition;
                    __instance.GameStartText.transform.localPosition = Vector3.zero;
                    __instance.GameStartText.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                    if (!__instance.GameStartText.text.StartsWith(FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameStarting).Replace("{0}", "")))
                    {
                        __instance.GameStartText.text = string.Empty;
                        __instance.GameStartTextParent.SetActive(false);
                    }
                }

                if (!__instance.GameStartText.text.StartsWith(FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameStarting).Replace("{0}", "")) || !CustomOptionHolder.anyPlayerCanStopStart.GetBool())
                    copiedStartButton?.Destroy();
                if (CustomOptionHolder.anyPlayerCanStopStart.GetBool() && copiedStartButton == null && __instance.GameStartText.text.StartsWith(FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameStarting).Replace("{0}", "")))
                {

                    // Activate Stop-Button
                    copiedStartButton = UnityEngine.Object.Instantiate(__instance.StartButton.gameObject, __instance.StartButton.gameObject.transform.parent);
                    copiedStartButton.transform.localPosition = __instance.StartButton.transform.localPosition;
                    copiedStartButton.SetActive(true);
                    var startButtonText = copiedStartButton.GetComponentInChildren<TMPro.TextMeshPro>();
                    startButtonText.text = "";
                    startButtonText.fontSize *= 0.62f;
                    startButtonText.fontSizeMax = startButtonText.fontSize;
                    startButtonText.gameObject.transform.localPosition = Vector3.zero;
                    PassiveButton startButtonPassiveButton = copiedStartButton.GetComponent<PassiveButton>();

                    void StopStartFunc()
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.StopStart, SendOption.Reliable, AmongUsClient.Instance.HostId);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        copiedStartButton.Destroy();
                        __instance.GameStartText.text = string.Empty;
                        startingTimer = 0;
                    }
                    startButtonPassiveButton.OnClick.AddListener((Action)(() => StopStartFunc()));
                    __instance.StartCoroutine(Effects.Lerp(.1f, new Action<float>((p) =>
                    {
                        startButtonText.text = "";
                    })));
                }
            }

            // Start Timer
            if (startingTimer > 0)
            {
                startingTimer -= Time.deltaTime;
            }
            // Lobby timer
            if (!GameData.Instance || !__instance.PlayerCounter) return; // No instance

            if (update) currentText = __instance.PlayerCounter.text;

            timer = Mathf.Max(0f, timer -= Time.deltaTime);
            int minutes = (int)timer / 60;
            int seconds = (int)timer % 60;
            string suffix = $" ({minutes:00}:{seconds:00})";

            if (!AmongUsClient.Instance) return;

            if (AmongUsClient.Instance.AmHost && sendGamemode && CachedPlayer.LocalPlayer != null)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareGameMode, SendOption.Reliable, -1);
                writer.Write((byte)ModOption.gameMode);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.shareGameMode((byte)ModOption.gameMode);
                sendGamemode = false;
            }
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
    public class GameStartManagerBeginGame
    {
        public static bool Prefix(GameStartManager __instance)
        {
#if DEBUG
                return true;
#endif
            // Block game start if not everyone has the same mod version
            bool continueStart = true;

            if (AmongUsClient.Instance.AmHost)
            {
                foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients.GetFastEnumerator())
                {
                    if (client.Character == null) continue;
                    var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
                    if (dummyComponent != null && dummyComponent.enabled)
                        continue;

                    if (!playerVersions.ContainsKey(client.Id))
                    {
                        continueStart = false;
                        break;
                    }

                    PlayerVersion PV = playerVersions[client.Id];
                    int diff = Main.Version.CompareTo(PV.version);
                    if (diff != 0 || !PV.GuidMatches())
                    {
                        continueStart = false;
                        break;
                    }
                }

                if (CustomOptionHolder.dynamicMap.GetBool() && continueStart)
                {
                    // 0 = Skeld
                    // 1 = Mira HQ
                    // 2 = Polus
                    // 3 = Dleks - deactivated
                    // 4 = Airship
                    // 5 = Fungle
                    // 6 = Submerged
                    byte chosenMapId = 0;
                    List<float> probabilities =
                    [
                        CustomOptionHolder.dynamicMapEnableSkeld.GetSelection() / 10f,
                        CustomOptionHolder.dynamicMapEnableMira.GetSelection() / 10f,
                        CustomOptionHolder.dynamicMapEnablePolus.GetSelection() / 10f,
                        CustomOptionHolder.dynamicMapEnableAirShip.GetSelection() / 10f,
                        CustomOptionHolder.dynamicMapEnableFungle.GetSelection() / 10f,
                        CustomOptionHolder.dynamicMapEnableSubmerged.GetSelection() / 10f,
                    ];

                    // if any map is at 100%, remove all maps that are not!
                    if (probabilities.Contains(1.0f))
                    {
                        for (int i = 0; i < probabilities.Count; i++)
                        {
                            if (probabilities[i] != 1.0) probabilities[i] = 0;
                        }
                    }

                    float sum = probabilities.Sum();
                    // All maps set to 0, why are you doing this???
                    if (sum == 0) return continueStart;
                    // Normalize to [0,1]
                    for (int i = 0; i < probabilities.Count; i++) probabilities[i] /= sum;

                    float selection = (float)rnd.NextDouble();
                    float cumsum = 0;
                    for (byte i = 0; i < probabilities.Count; i++)
                    {
                        cumsum += probabilities[i];
                        if (cumsum > selection)
                        {
                            chosenMapId = i;
                            break;
                        }
                    }

                    // Translate chosen map to presets page and use that maps random map preset page
                    if (CustomOptionHolder.dynamicMapSeparateSettings.GetBool())
                        CustomOptionHolder.presetSelection.updateSelection(chosenMapId + 3);
                    if (chosenMapId >= 3) chosenMapId++; // Skip dlekS

                    var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                        (byte)CustomRPC.DynamicMapOption, SendOption.Reliable, -1);
                    writer.Write(chosenMapId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.dynamicMapOption(chosenMapId);
                }
            }
            return continueStart;
        }
    }

    public class PlayerVersion(Version version, Guid guid)
    {
        public readonly Version version = version;
        public readonly Guid guid = guid;

        public bool GuidMatches()
        {
            return Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.Equals(guid);
        }
    }
}