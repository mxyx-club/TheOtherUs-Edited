using System;
using System.Linq;
using Hazel;
using InnerNet;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Modules;

[HarmonyPatch]
public static class ChatCommands
{
    public static bool isLover(this PlayerControl player)
    {
        return player != null && (player == Lovers.lover1 || player == Lovers.lover2);
    }

    public static bool isTeamCultist(this PlayerControl player)
    {
        return !(player == null) && (player == Cultist.cultist || player == Follower.follower) &&
               Cultist.cultist != null && Follower.follower != null;
    }

    public static bool isTeamCultistAndLover(this PlayerControl player)
    {
        return !(player == null) && (player == Follower.follower || player == player.getPartner()) &&
               player.getPartner() != null && Follower.follower != null;
    }

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
                    if (gm.StartsWith("prop") || gm.StartsWith("ph")) gameMode = CustomGamemodes.PropHunt;
                    if (gm.StartsWith("guess") || gm.StartsWith("gm")) gameMode = CustomGamemodes.Guesser;
                    if (gm.StartsWith("hide") || gm.StartsWith("hn")) gameMode = CustomGamemodes.HideNSeek;

                    if (AmongUsClient.Instance.AmHost)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                            (byte)CustomRPC.ShareGamemode, SendOption.Reliable);
                        writer.Write((byte)gameMode);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.shareGameMode((byte)gameMode);
                    }
                    else
                    {
                        __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, "Nice try, but you have to be the host to use this feature 这是房主至高无上的权利");
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
                    MapOption.isCanceled = true;
                    handled = true;
                }
                // 强制紧急会议或结束会议
                else if (chat.StartsWith("/meeting") || chat.StartsWith("/mt"))
                {
                    if (IsMeeting) MeetingHud.Instance.RpcClose();
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
                        target.Exiled();
                        target.RpcMurderPlayer(target, true);
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
                var roleInfo = "";
                for (var i = 0; i < localRole.Count; i++)
                {
                    roleInfo = RoleInfo.GetRoleDescription(localRole[i]);

                    __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, $"{localRole[i].name}:\n {roleInfo}\n");

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
            if (chat.StartsWith("/tp ") && CachedPlayer.LocalPlayer.Data.IsDead)
            {
                var playerName = text[4..].ToLower();
                PlayerControl target =
                    CachedPlayer.AllPlayers.FirstOrDefault(x => x.Data.PlayerName.ToLower().Equals(playerName));
                if (target != null)
                {
                    CachedPlayer.LocalPlayer.transform.position = target.transform.position;
                    handled = true;
                }
            }

            if (chat.StartsWith("/team") && CachedPlayer.LocalPlayer.PlayerControl.isLover() && CachedPlayer.LocalPlayer.PlayerControl.isTeamCultist())
            {
                if (Cultist.cultist == CachedPlayer.LocalPlayer.PlayerControl)
                    Cultist.chatTarget = flipBitwise(Cultist.chatTarget);
                if (Follower.follower == CachedPlayer.LocalPlayer.PlayerControl)
                    Follower.chatTarget = flipBitwise(Follower.chatTarget);
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

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class EnableChat
    {
        public static void Postfix(HudManager __instance)
        {
            if (!__instance.Chat.isActiveAndEnabled && (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay || MapOption.DebugMode ||
                                                        (CachedPlayer.LocalPlayer.PlayerControl.isLover() && Lovers.enableChat) ||
                                                        CachedPlayer.LocalPlayer.PlayerControl.isTeamCultist()))
                __instance.Chat.SetVisible(true);

            if (Multitasker.multitasker.FindAll(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId).Count > 0 || MapOption.transparentTasks)
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
            var sourcePlayer = PlayerControl.AllPlayerControls.ToArray().ToList()
                .FirstOrDefault(x => x.Data != null && x.Data.PlayerName.Equals(playerName, StringComparison.Ordinal));

            if (CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.Data.Role.IsImpostor && __instance != null
                 && ((Spy.spy != null && sourcePlayer.PlayerId == Spy.spy.PlayerId)
                 || (Sidekick.sidekick != null && Sidekick.wasTeamRed && sourcePlayer.PlayerId == Sidekick.sidekick.PlayerId)
                 || (Jackal.jackal != null && Jackal.wasTeamRed && sourcePlayer.PlayerId == Jackal.jackal.PlayerId)))
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
            var playerControl = CachedPlayer.LocalPlayer.PlayerControl;
            var flag = MeetingHud.Instance != null
                       || LobbyBehaviour.Instance != null
                       || playerControl.Data.IsDead
                       || sourcePlayer.PlayerId == CachedPlayer.LocalPlayer.PlayerId;
            if (__instance != FastDestroyableSingleton<HudManager>.Instance.Chat) return true;
            if (playerControl == null) return true;
            if (MapOption.DebugMode) return flag;
            if (!playerControl.isTeamCultist() && !playerControl.isLover()) return flag;
            if ((playerControl.isTeamCultist() && Follower.chatTarget) ||
                (playerControl.isLover() && Lovers.enableChat) ||
                (playerControl.isTeamCultistAndLover() && !Follower.chatTarget))
                return sourcePlayer.getChatPartner() == playerControl || playerControl.getChatPartner() == playerControl == (bool)sourcePlayer || flag;
            return flag;
        }
    }
}