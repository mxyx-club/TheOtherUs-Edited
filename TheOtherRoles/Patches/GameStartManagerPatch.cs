using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using InnerNet;
using TheOtherRoles.Helper;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using Object = Il2CppSystem.Object;

namespace TheOtherRoles.Patches;

public class GameStartManagerPatch
{
    public static readonly Dictionary<int, HandshakeHelper.PlayerVersion> playerVersions = new();
    public static float timer = 600f;
    private static float kickingTimer;
    private static string lobbyCodeText = "";

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    public class AmongUsClientOnPlayerJoinedPatch
    {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData data)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer!.PlayerControl.NetId,
                    (byte)CustomRPC.ShareGamemode, SendOption.Reliable);
                writer.Write((byte)TORMapOptions.gameMode);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            
#if DEBUG
               return;
#endif
            if (CachedPlayer.LocalPlayer != null)
            {
                HandshakeHelper.shareGameVersion();
                HandshakeHelper.shareGameGUID();
            }
        }
    }
    
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class AmongUsClientOnOnGameEndPatch
    {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData data)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer!.PlayerControl.NetId,
                    (byte)CustomRPC.ShareGamemode, SendOption.Reliable);
                writer.Write((byte)TORMapOptions.gameMode);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public class GameStartManagerStartPatch
    {
        public static void Postfix(GameStartManager __instance)
        {

            // Reset lobby countdown timer
            timer = 600f;

            // Reset kicking timer
            kickingTimer = 0f;

            // Copy lobby code
            var code = GameCode.IntToGameName(AmongUsClient.Instance.GameId);
            GUIUtility.systemCopyBuffer = code;

            lobbyCodeText =
                FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.RoomCode,
                    new Il2CppReferenceArray<Object>(0)) + "\r\n" + code;
#if DEBUG
               return;
#endif
            // Send version as soon as CachedPlayer.LocalPlayer.PlayerControl exists
            if (CachedPlayer.LocalPlayer != null)
            {
                HandshakeHelper.shareGameVersion();
                HandshakeHelper.shareGameGUID();
            }

            HandshakeHelper.PlayerAgainInfo.Clear();
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public class GameStartManagerUpdatePatch
    {
        public static float startingTimer;
        private static bool update;
        private static string currentText = "";

        public static void Prefix(GameStartManager __instance)
        {
            if (!GameData.Instance) return; // No instance
            __instance.MinPlayers = 1;
            update = GameData.Instance.PlayerCount != __instance.LastPlayerCount;
        }

        public static void Postfix(GameStartManager __instance)
        {
            if (__instance.GameStartText == null || __instance.StartButton == null ||
                __instance.startLabelText == null) return;
#if DEBUG
                return;
#endif
            // Check version handshake infos

            var versionMismatch = false;
            var message = "";
            foreach (var client in AmongUsClient.Instance.allClients.ToArray())
            {
                if (client.Character == null) continue;
                var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
                if (dummyComponent != null && dummyComponent.enabled) continue;

                if (!playerVersions.ContainsKey(client.Id))
                {
                    HandshakeHelper.againSend(client.Id, HandshakeHelper.ShareMode.Again);
                    versionMismatch = true;
                    message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} 装了个不同版本的TheOtherUs\n</color>";
                }
                else
                {
                    var PV = playerVersions[client.Id];
                    var diff = Main.Version.CompareTo(PV.version);
                    if (PV.guid == null)
                    {
                        HandshakeHelper.againSend(client.Id, HandshakeHelper.ShareMode.Guid);
                        continue;
                    }

                    switch (diff)
                    {
                        case > 0:
                            message +=
                                $"<color=#FF0000FF>{client.Character.Data.PlayerName} 装了一个旧版本的TheOtherUs (v{playerVersions[client.Id].version.ToString()})\n</color>";
                            versionMismatch = true;
                            break;
                        case < 0:
                            message +=
                                $"<color=#FF0000FF>{client.Character.Data.PlayerName} 装了一个较新版本的TheOtherUs (v{playerVersions[client.Id].version.ToString()})\n</color>";
                            versionMismatch = true;
                            break;
                        default:
                        {
                            if (!PV.GuidMatches())
                            {
                                // version presumably matches, check if Guid matches
                                message +=
                                    $"<color=#FF0000FF>{client.Character.Data.PlayerName} 装了一个修改过的TheOtherUs v{playerVersions[client.Id].version.ToString()} <size=30%>({PV.guid.ToString()})</size>\n</color>";
                                versionMismatch = true;
                            }

                            break;
                        }
                    }
                }
            }

            // Display message to the host
            if (AmongUsClient.Instance.AmHost)
            {
                if (versionMismatch)
                {
                    __instance.StartButton.color = __instance.startLabelText.color = Palette.DisabledClear;
                    __instance.GameStartText.text = message;
                    __instance.GameStartText.transform.localPosition =
                        __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                }
                else
                {
                    __instance.StartButton.color = __instance.startLabelText.color =
                        __instance.LastPlayerCount >= __instance.MinPlayers
                            ? Palette.EnabledColor
                            : Palette.DisabledClear;
                    __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition;
                }

                // Make starting info available to clients:
                if (startingTimer <= 0 && __instance.startState == GameStartManager.StartingStates.Countdown)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer!.PlayerControl.NetId, (byte)CustomRPC.SetGameStarting,
                        SendOption.Reliable);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.setGameStarting();
                }
            }

            // Client update with handshake infos
            else
            {
                if (!playerVersions.ContainsKey(AmongUsClient.Instance.HostId) ||
                    Main.Version.CompareTo(playerVersions[AmongUsClient.Instance.HostId].version) != 0)
                {
                    kickingTimer += Time.deltaTime;
                    if (kickingTimer > 10)
                    {
                        kickingTimer = 0;
                        AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                        SceneChanger.ChangeScene("MainMenu");
                    }

                    __instance.GameStartText.text =
                        $"<color=#FF0000FF>房主没有或不同版本的TheOtherUs\n即将被踢出房间 {Math.Round(10 - kickingTimer)}s</color>";
                    __instance.GameStartText.transform.localPosition =
                        __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                }
                else if (versionMismatch)
                {
                    __instance.GameStartText.text = "<color=#FF0000FF>装了不同版本模组的玩家:\n</color>" + message;
                    __instance.GameStartText.transform.localPosition =
                        __instance.StartButton.transform.localPosition + (Vector3.up * 2);
                }
                else
                {
                    __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition;
                    if (__instance.startState != GameStartManager.StartingStates.Countdown && startingTimer <= 0)
                    {
                        __instance.GameStartText.text = string.Empty;
                    }
                    else
                    {
                        __instance.GameStartText.text = $"距离开始还有 {(int)startingTimer + 1}";
                        if (startingTimer <= 0) __instance.GameStartText.text = string.Empty;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && GameStartManager.InstanceExists &&
                GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown &&
                startingTimer <= 4) GameStartManager.Instance.countDownTimer = 0;
            //カウントダウンキャンセル
            if (Input.GetKeyDown(KeyCode.C) && GameStartManager.InstanceExists &&
                GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown)
            {
                GameStartManager.Instance.ResetStartState();
                if (Cultist.isCultistGame) GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors = 2;
                //                 Cultist.isCultistGame = false;
            }

            // Start Timer
            if (startingTimer > 0) startingTimer -= Time.deltaTime;
            // Lobby timer
            if (!GameData.Instance) return; // No instance

            if (update) currentText = __instance.PlayerCounter.text;

            timer = Mathf.Max(0f, timer -= Time.deltaTime);
            var minutes = (int)timer / 60;
            var seconds = (int)timer % 60;
            var suffix = $" ({minutes:00}:{seconds:00})";

            __instance.PlayerCounter.text = currentText + suffix;
            __instance.PlayerCounter.autoSizeTextContainer = true;
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
            var continueStart = true;

            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var client in AmongUsClient.Instance.allClients.GetFastEnumerator())
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

                    var PV = playerVersions[client.Id];
                    var diff = Main.Version.CompareTo(PV.version);
                    if (diff == 0 && PV.GuidMatches()) continue;
                    continueStart = false;
                    break;
                }

                if (continueStart &&
                    (TORMapOptions.gameMode == CustomGamemodes.HideNSeek ||
                     TORMapOptions.gameMode == CustomGamemodes.PropHunt) &&
                    GameOptionsManager.Instance.CurrentGameOptions.MapId != 6)
                {
                    byte mapId = TORMapOptions.gameMode switch
                    {
                        CustomGamemodes.HideNSeek => (byte)CustomOptionHolder.hideNSeekMap.getSelection(),
                        CustomGamemodes.PropHunt => (byte)CustomOptionHolder.propHuntMap.getSelection(),
                        _ => 0
                    };
                    if (mapId >= 3) mapId++;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DynamicMapOption,
                        SendOption.Reliable);
                    writer.Write(mapId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.dynamicMapOption(mapId);
                }

                if (Cultist.isCultistGame)
                {
                    GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors = 2;
                    Cultist.isCultistGame = false;
                }

                var cultistCheck = CustomOptionHolder.cultistSpawnRate.getSelection() != 0 &&
                                   rnd.Next(1, 101) <= CustomOptionHolder.cultistSpawnRate.getSelection() * 10;
                if (cultistCheck)
                {
                    // We should have Custist (Cultist is only supported on 2 Impostors)
                    Cultist.isCultistGame = true;
                    GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors = 1;
                }
                else if (CustomOptionHolder.dynamicMap.getBool() && continueStart)
                {
                    // 0 = Skeld
                    // 1 = Mira HQ
                    // 2 = Polus
                    // 3 = Dleks - deactivated
                    // 4 = Airship
                    // 5 = Submerged
                    byte chosenMapId = 0;
                    var probabilities = new List<float>();
                    probabilities.Add(CustomOptionHolder.dynamicMapEnableSkeld.getSelection() / 10f);
                    probabilities.Add(CustomOptionHolder.dynamicMapEnableMira.getSelection() / 10f);
                    probabilities.Add(CustomOptionHolder.dynamicMapEnablePolus.getSelection() / 10f);
                    probabilities.Add(CustomOptionHolder.dynamicMapEnableAirShip.getSelection() / 10f);
                    probabilities.Add(CustomOptionHolder.dynamicMapEnableFungle.getSelection() / 10f);
                    probabilities.Add(CustomOptionHolder.dynamicMapEnableSubmerged.getSelection() / 10f);

                    // if any map is at 100%, remove all maps that are not!
                    if (probabilities.Contains(1.0f))
                        for (var i = 0; i < probabilities.Count; i++)
                            if ((int)probabilities[i] != 1)
                                probabilities[i] = 0;

                    var sum = probabilities.Sum();
                    if (sum == 0) return continueStart; // All maps set to 0, why are you doing this???
                    for (var i = 0; i < probabilities.Count; i++)
                        // Normalize to [0,1]
                        probabilities[i] /= sum;
                    var selection = (float)rnd.NextDouble();
                    float cumsum = 0;
                    for (byte i = 0; i < probabilities.Count; i++)
                    {
                        cumsum += probabilities[i];
                        if (!(cumsum > selection)) continue;
                        chosenMapId = i;
                        break;
                    }

                    // Translate chosen map to presets page and use that maps random map preset page
                    if (CustomOptionHolder.dynamicMapSeparateSettings.getBool())
                        CustomOptionHolder.presetSelection.updateSelection(chosenMapId + 2);
                    if (chosenMapId >= 3) chosenMapId++; // Skip dlekS

                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DynamicMapOption,
                        SendOption.Reliable);
                    writer.Write(chosenMapId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.dynamicMapOption(chosenMapId);
                }
            }

            return continueStart;
        }
    }
    
}