using System;
using HarmonyLib;
using System.Linq;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using UnityEngine;
using Hazel;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Modules {
    [HarmonyPatch]
    public static class ChatCommands {
        public static bool isLover(this PlayerControl player) => !(player == null) && (player == Lovers.lover1 || player == Lovers.lover2);

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
        private static class SendChatPatch {
            static bool Prefix(ChatController __instance) {
                string text = __instance.freeChatField.Text;
                bool handled = false;
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) {
                    if (text.ToLower().StartsWith("/kick ")) {
                        string playerName = text.Substring(6);
                        PlayerControl target = CachedPlayer.AllPlayers.FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
                        if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan()) {
                            var client = AmongUsClient.Instance.GetClient(target.OwnerId);
                            if (client != null) {
                                AmongUsClient.Instance.KickPlayer(client.Id, false);
                                handled = true;
                            }
                        }
                    } else if (text.ToLower().StartsWith("/ban ")) {
                        string playerName = text.Substring(6);
                        PlayerControl target = CachedPlayer.AllPlayers.FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
                        if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan()) {
                            var client = AmongUsClient.Instance.GetClient(target.OwnerId);
                            if (client != null) {
                                AmongUsClient.Instance.KickPlayer(client.Id, true);
                                handled = true;
                            }
                        }
                    } else if (text.ToLower().StartsWith("/gm")) {
                        string gm = text.Substring(4).ToLower();
                        CustomGamemodes gameMode = CustomGamemodes.Classic;
                        if (gm.StartsWith("prop") || gm.StartsWith("ph")) {
                            gameMode = CustomGamemodes.PropHunt;
                        } else if (gm.StartsWith("guess") || gm.StartsWith("gm")) {
                            gameMode = CustomGamemodes.Guesser;
                        } else if (gm.StartsWith("hide") || gm.StartsWith("hn")) {
                            gameMode = CustomGamemodes.HideNSeek;
                        }
                        // else its classic!

                        if (AmongUsClient.Instance.AmHost) {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGamemode, Hazel.SendOption.Reliable, -1);
                            writer.Write((byte)TORMapOptions.gameMode);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.shareGamemode((byte)gameMode);
                            RPCProcedure.shareGamemode((byte)TORMapOptions.gameMode);
                        } else {
                            __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, "Nice try, but you have to be the host to use this feature");
                        }
                        handled = true;
                    }
                }
                
                if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) {
                    if (text.ToLower().Equals("/murder")) {
                        CachedPlayer.LocalPlayer.PlayerControl.Exiled();
                        FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(CachedPlayer.LocalPlayer.Data, CachedPlayer.LocalPlayer.Data);
                        handled = true;
                    } else if (text.ToLower().StartsWith("/color ")) {
                        handled = true;
                        int col;
                        if (!Int32.TryParse(text.Substring(7), out col)) {
                            __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, "Unable to parse color id\nUsage: /color {id}");
                        }
                        col = Math.Clamp(col, 0, Palette.PlayerColors.Length - 1);
                        CachedPlayer.LocalPlayer.PlayerControl.SetColor(col);
                        __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, "Changed color succesfully");;
                    } 
                }

                if (text.ToLower().StartsWith("/tp ") && CachedPlayer.LocalPlayer.Data.IsDead) {
                    string playerName = text.Substring(4).ToLower();
                    PlayerControl target = CachedPlayer.AllPlayers.FirstOrDefault(x => x.Data.PlayerName.ToLower().Equals(playerName));
                    if (target != null) {
                        CachedPlayer.LocalPlayer.transform.position = target.transform.position;
                        handled = true;
                    }
                }

                if (text.ToLower().StartsWith("/team") && CachedPlayer.LocalPlayer.PlayerControl.isLover() && CachedPlayer.LocalPlayer.PlayerControl.isTeamCultist())
			{
				if (Cultist.cultist == CachedPlayer.LocalPlayer.PlayerControl)
				{
					Cultist.chatTarget = Helpers.flipBitwise(Cultist.chatTarget);
				}
				if (Follower.follower == CachedPlayer.LocalPlayer.PlayerControl)
				{
					Follower.chatTarget = Helpers.flipBitwise(Follower.chatTarget);
				}
				handled = true;
            }

                if (text.ToLower().StartsWith("/role")) {
                    RoleInfo localRole = RoleInfo.getRoleInfoForPlayer(CachedPlayer.LocalPlayer.PlayerControl, false).FirstOrDefault();
                    if (localRole != RoleInfo.impostor && localRole != RoleInfo.crewmate) {
                        string info = RoleInfo.GetRoleDescription(localRole);
                        __instance.AddChat(CachedPlayer.LocalPlayer.PlayerControl, info);
                        handled = true;
                    }
                }

                if (handled) {
                    __instance.freeChatField.Clear();
                    __instance.quickChatMenu.Clear();
                }
                return !handled;
            }
        }
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class EnableChat {
            public static void Postfix(HudManager __instance) {
                if (!__instance.Chat.isActiveAndEnabled && (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay || (CachedPlayer.LocalPlayer.PlayerControl.isLover() && Lovers.enableChat) || CachedPlayer.LocalPlayer.PlayerControl.isTeamCultist()))
                {
                    __instance.Chat.SetVisible(true);
                }

                if ((Multitasker.multitasker.FindAll(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId).Count > 0) || TORMapOptions.transparentTasks) {
                if (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.Disconnected) return;
                if (!Minigame.Instance) return;

                var Base = Minigame.Instance as MonoBehaviour;
                SpriteRenderer[] rends = Base.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < rends.Length; i++)
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
        public static class SetBubbleName { 
            public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName) {
                PlayerControl sourcePlayer = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data != null && x.Data.PlayerName.Equals(playerName));
                if (CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.Data.Role.IsImpostor && (Spy.spy != null && sourcePlayer.PlayerId == Spy.spy.PlayerId || Sidekick.sidekick != null && Sidekick.wasTeamRed && sourcePlayer.PlayerId == Sidekick.sidekick.PlayerId || Jackal.jackal != null && Jackal.wasTeamRed && sourcePlayer.PlayerId == Jackal.jackal.PlayerId) && __instance != null) __instance.NameText.color = Palette.ImpostorRed;
            }
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))] //test
        public static class AddChat {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
		{
			PlayerControl playerControl = CachedPlayer.LocalPlayer.PlayerControl;
			bool flag = MeetingHud.Instance != null || LobbyBehaviour.Instance != null || playerControl.Data.IsDead || sourcePlayer.PlayerId == CachedPlayer.LocalPlayer.PlayerId;
			if (__instance != FastDestroyableSingleton<HudManager>.Instance.Chat)
			{
				return true;
			}
			if (playerControl == null)
			{
				return true;
			}
            /* brb
			if (playerControl == Detective.detective)
			{
				return flag;
			}
            */
			if (!playerControl.isTeamCultist() && !playerControl.isLover())
			{
				return flag;
			}
			if (playerControl.isTeamCultist() && Follower.chatTarget || playerControl.isLover() && Lovers.enableChat || playerControl.isTeamCultistAndLover() && !Follower.chatTarget)
			{
				return sourcePlayer.getChatPartner() == playerControl || playerControl.getChatPartner() == playerControl == (bool)sourcePlayer || flag;
			}
			return flag;
		}
	}

	public static bool isTeamCultist(this PlayerControl player)
	{
		return !(player == null) && (player == Cultist.cultist || player == Follower.follower) && Cultist.cultist != null && Follower.follower != null;
	}
    public static bool isTeamCultistAndLover(this PlayerControl player)
	{
		return !(player == null) && (player == Follower.follower || player == player.getPartner()) && player.getPartner() != null && Follower.follower != null;
	}
    
    }
    
}
