using System;
using System.Linq;
using AmongUs.Data;
using Hazel;
using InnerNet;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Modules;

[HarmonyPatch]
public static class ChatCommands
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    private static class SendChatPatch
    {
        private static bool Prefix(ChatController __instance)
        {
            var text = __instance.freeChatField.Text;
            var chat = text.ToLower();
            var handled = false;
            // 游戏大厅指令
            if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
            {
                if (chat.StartsWith("/gm"))
                {
                    var gm = text[4..].ToLower();
                    var gameMode = CustomGamemodes.Classic;
                    if (gm.StartsWith("guess") || gm.StartsWith("gm")) gameMode = CustomGamemodes.Guesser;

                    if (AmongUsClient.Instance.AmHost)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                            (byte)CustomRPC.ShareGameMode, SendOption.Reliable);
                        writer.Write((byte)gameMode);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.shareGameMode((byte)gameMode);
                    }
                    else
                    {
                        __instance.AddChat(PlayerControl.LocalPlayer,
                            "Nice try, but you have to be the host to use this feature\n这是房主至高无上的权利");
                    }
                    handled = true;
                }
            }

            if (chat.StartsWith("/kick ") && AmongUsClient.Instance.AmHost)
            {
                var playerName = text[6..];
                PlayerControl target = CachedPlayer.AllPlayers.FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
                if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
                {
                    var client = AmongUsClient.Instance.GetClient(target.OwnerId);
                    if (client != null)
                    {
                        AmongUsClient.Instance.KickPlayer(client.Id, false);
                    }
                }
                handled = true;
            }
            else if (chat.StartsWith("/ban ") && AmongUsClient.Instance.AmHost)
            {
                var playerName = text[5..];
                PlayerControl target = CachedPlayer.AllPlayers.FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
                if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
                {
                    var client = AmongUsClient.Instance.GetClient(target.OwnerId);
                    if (client != null)
                    {
                        AmongUsClient.Instance.KickPlayer(client.Id, true);
                    }
                    handled = true;
                }
            }

            // 游戏中房主指令
            if (AmongUsClient.Instance.AmHost && InGame)
            {
                //  强制结束游戏
                if (chat.StartsWith("/end"))
                {
                    ModOption.isCanceled = true;
                    handled = true;
                }
                // 强制紧急会议或结束会议
                else if (chat.StartsWith("/meeting") || chat.StartsWith("/mt"))
                {
                    if (InMeeting) MeetingHud.Instance.RpcClose();
                    else CachedPlayer.LocalPlayer.PlayerControl.NoCheckStartMeeting(null, true);
                    handled = true;
                }
                else if (chat.StartsWith("/kill "))
                {

                    var playerName = text[6..];
                    var target = playerName is not null and "me"
                        ? CachedPlayer.LocalPlayer.PlayerControl
                        : (PlayerControl)CachedPlayer.AllPlayers.FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
                    if (target != null)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                            (byte)CustomRPC.HostKill, SendOption.Reliable);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.hostKill(target.PlayerId);
                    }
                    handled = true;
                }
                else if (chat.StartsWith("/revive "))
                {
                    var playerName = text[8..];
                    var target = playerName is not null and "me"
                        ? CachedPlayer.LocalPlayer.PlayerControl
                        : (PlayerControl)CachedPlayer.AllPlayers.FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
                    if (target != null)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                            (byte)CustomRPC.HostRevive, SendOption.Reliable);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.hostRevive(target.PlayerId);
                    }
                    handled = true;
                }
            }

            // 游戏中玩家指令
            if (chat.StartsWith("/m") && InGame)
            {
                var localRole = RoleInfo.getRoleInfoForPlayer(CachedPlayer.LocalPlayer.PlayerControl);
                foreach (var roleInfo in localRole)
                {
                    if (roleInfo.roleId == RoleId.Cursed) continue;
                    var roleText = RoleInfo.getRoleDescription(roleInfo.Name);
                    __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, roleText);
                }
                handled = true;
            }
            if (chat.StartsWith("/r "))
            {

                var role = text[3..];
                var roleText = RoleInfo.getRoleDescription(role);
                if (roleText != null) __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, roleText);
                handled = true;
            }

            // 自由模式指令
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
            {
                if (text.ToLower().Equals("/murder"))
                {
                    CachedPlayer.LocalPlayer.PlayerControl.Exiled();
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(CachedPlayer.LocalPlayer.Data, CachedPlayer.LocalPlayer.Data);
                    handled = true;
                }
                else if (chat.StartsWith("/color "))
                {
                    handled = true;
                    if (!int.TryParse(text.AsSpan(7), out var col))
                        __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, "Unable to parse color id\nUsage: /color {id}");
                    col = Math.Clamp(col, 0, Palette.PlayerColors.Length - 1);
                    CachedPlayer.LocalPlayer.PlayerControl.SetColor(col);
                    __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, "Changed color succesfully");
                }
            }

            // 死亡玩家指令
            if (chat.StartsWith("/tp ") && (CachedPlayer.LocalPlayer.IsDead || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started))
            {
                var playerName = text[4..].ToLower();
                PlayerControl target = CachedPlayer.AllPlayers.FirstOrDefault(x => x.Data.PlayerName.ToLower().Equals(playerName));
                if (target != null)
                {
                    CachedPlayer.LocalPlayer.transform.position = target.transform.position;
                    handled = true;
                }
            }

            if (chat.StartsWith("/cmd"))
            {
                if (AmongUsClient.Instance.AmHost)
                {
                    __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, "CommandsInHost".Translate());
                }
                __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, "CommandsInPlayer".Translate());
                handled = true;
            }

            if (handled)
            {
                __instance.freeChatField.Clear();
                __instance.quickChatMenu.Clear();
            }

            return !handled;
        }
    }


    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public class AmongUsClientOnPlayerJoinedPatch
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (PlayerControl.LocalPlayer == __instance.myPlayer && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
            {
                _ = new LateTask(() =>
                {
                    if (__instance.myPlayer.IsAlive())
                    {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(__instance.myPlayer, GetWelcomeMessage);
                    }
                }, 1f, "Welcome Chat");
            }
        }

        private static string GetWelcomeMessage => "WelcomeText".Translate();
    }


    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class EnableChat
    {
        public static void Postfix(HudManager __instance)
        {
            if (!__instance.Chat.isActiveAndEnabled && (ModOption.DebugMode
                    || AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay
                    || (CachedPlayer.LocalPlayer.PlayerControl.isLover() && Lovers.enableChat)))
                __instance.Chat.SetVisible(true);

            if (!InMeeting && !ModOption.DebugMode && Specter.Player != null && PlayerControl.LocalPlayer == Specter.Player)
                __instance.Chat?.SetVisible(false);


            if (ModOption.transparentTasks || Multitasker.multitasker.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId))
            {
                if (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.Disconnected) return;
                if (!Minigame.Instance) return;

                var Base = Minigame.Instance as MonoBehaviour;
                SpriteRenderer[] rends = Base.GetComponentsInChildren<SpriteRenderer>();
                for (var i = 0; i < rends.Length; i++)
                {
                    var oldColor1 = rends[i].color[0];
                    var oldColor2 = rends[i].color[1];
                    var oldColor3 = rends[i].color[2];
                    rends[i].color = new Color(oldColor1, oldColor2, oldColor3, 0.5f);
                }
            }
        }
    }

    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    public static class SetBubbleName
    {
        public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
        {
            var sourcePlayer = PlayerControl.AllPlayerControls.ToList()
                .FirstOrDefault(x => x.Data != null && x.Data.PlayerName.Equals(playerName, StringComparison.Ordinal));

            if (sourcePlayer != null && CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.Data?.Role?.IsImpostor == true
                 && Spy.spy != null && sourcePlayer.PlayerId == Spy.spy.PlayerId)
            {
                __instance.NameText.color = Palette.ImpostorRed;
            }
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))] //test
    public static class AddChat
    {
        public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
        {
            var local = CachedPlayer.LocalPlayer.PlayerControl;
            if (local == null) return true;

            var flag = MeetingHud.Instance != null || LobbyBehaviour.Instance != null
                || local.Data.IsDead || sourcePlayer.PlayerId == CachedPlayer.LocalId;

            if (__instance != FastDestroyableSingleton<HudManager>.Instance.Chat) return true;
            if (ModOption.DebugMode || !local.isLover()) return flag;
            if (local.isLover() && Lovers.enableChat)
                return sourcePlayer.getPartner() == local || local.getPartner() == local == (bool)sourcePlayer || flag;
            return flag;
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class ChatControllerAwakePatch
    {
        public static void Prefix()
        {
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
        }
        public static void Postfix(ChatController __instance)
        {
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;

            if (Input.GetKeyDown(ModInputManager.toggleChat.keyCode))
            {
                if (!__instance.isActiveAndEnabled) return;
                __instance.Toggle();
            }
            if (__instance.IsOpenOrOpening)
            {
                __instance.banButton.MenuButton.enabled = !__instance.IsAnimating;
            }
        }
    }
}