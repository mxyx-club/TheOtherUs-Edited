using System.Text;
using static UnityEngine.GraphicsBuffer;

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
            var handled = ChatCommandRegistry.TryHandle(text, PlayerControl.LocalPlayer, __instance);

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
        public static bool ForceEnableChat;
        public static void Postfix(HudManager __instance)
        {
            if (!__instance.Chat.isActiveAndEnabled && (ModOption.DebugMode
                    || AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay
                    || (PlayerControl.LocalPlayer.isLover() && Lovers.enableChat)))
                __instance.Chat.SetVisible(true);

            if (!InMeeting && !ModOption.DebugMode && Specter.Player != null && PlayerControl.LocalPlayer == Specter.Player)
                __instance.Chat?.SetVisible(false);

            if (ForceEnableChat)
            {
                __instance.Chat.enabled = true;
                __instance.Chat.SetVisible(true);
            }
        }
    }

    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    private static class SetBubbleName
    {
        private static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
        {
            var sourcePlayer = PlayerByName(playerName);

            if (__instance != null && PlayerControl.LocalPlayer.IsImpostor(false, true)
                 && sourcePlayer.IsImpostor(true, true))
            {
                __instance.NameText.color = Palette.ImpostorRed;
            }
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))] //test
    private static class AddChat
    {
        private static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
        {
            var local = PlayerControl.LocalPlayer;
            if (local == null) return true;

            var flag = MeetingHud.Instance || LobbyBehaviour.Instance || CanSeeRoleInfo || ModOption.DebugMode || sourcePlayer.PlayerId == local.PlayerId;

            if (__instance != FastDestroyableSingleton<HudManager>.Instance.Chat) return true;
            if (Blackmailer.blackmailed == sourcePlayer) return false;
            if (!local.isLover()) return flag;
            if (local.isLover() && Lovers.enableChat) return sourcePlayer.getPartner() == local || local.getPartner() == sourcePlayer || flag;
            return flag;
        }

        private static void Postfix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
        {
            if (sourcePlayer.IsDead() || sourcePlayer == PlayerControl.LocalPlayer || !InMeeting) return;

            try
            {
                var local = PlayerControl.LocalPlayer;
                var targetId = sourcePlayer.PlayerId;
                if (MeetingHud.Instance.state is MeetingHud.VoteStates.Proceeding) return;
                var pva = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == targetId);
                if (pva == null) return;
                var rend = new GameObject().AddComponent<SpriteRenderer>();
                rend.transform.SetParent(pva.transform);
                rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                rend.transform.localPosition = new Vector3(-0.5f, 0.2f, -1f);
                rend.sprite = new ResourceSprite("TheOtherRoles.Resources.ChatOverlay.png", 130f);
                rend?.gameObject?.SetActive(true);

                _ = new LateTask(() =>
                {
                    rend?.gameObject?.SetActive(false);
                    rend?.gameObject?.Destroy();
                }, 3f);
            }
            catch
            {
                Message("Chat Notification Overlay is Detected");
            }
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
            __instance.freeChatField.textArea.AllowPaste = true;
            __instance.chatBubblePool.Prefab.Cast<ChatBubble>().TextArea.overrideColorTags = false;

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

    public static void Init()
    {
        ChatCommandRegistry.Register("end", (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame)
            {
                ModOption.isCanceled = true;
                return;
            }
        });

        ChatCommandRegistry.Register("say", (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame && args.Length > 0)
            {
                var message = string.Join(' ', args);
                message = $"{Cs(Palette.Purple, "★【房主消息】★")}\n{message}";
                var writer = StartRPC(CustomRPC.HostSay);
                writer.Write(message);
                writer.EndRPC();
                chat.AddChat(GetHostPlayer, message);
                return;
            }
        });

        ChatCommandRegistry.Register("cmd", (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost)
            {
                chat.AddChat(PlayerControl.LocalPlayer, "CommandsInHost".Translate());
            }
            chat.AddChat(PlayerControl.LocalPlayer, "CommandsInPlayer".Translate());
        });

        ChatCommandRegistry.Register("kill", (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame)
            {
                var target = GetPlayer(args);
                if (target != null)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.HostKill);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();
                    RPCProcedure.hostKill(target.PlayerId);
                    if (InMeeting)
                    {
                        MeetingHud.Instance.CheckForEndVoting();
                    }
                    return;
                }
            }
        });

        ChatCommandRegistry.Register("meeting", (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame)
            {
                if (InMeeting)
                {
                    MeetingHud.Instance.RpcVotingComplete(Array.Empty<MeetingHud.VoterState>(), null, false);
                }
                else
                {
                    var writer = StartRPC(CustomRPC.NoCheckStartMeeting);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(byte.MaxValue);
                    writer.Write(true);
                    writer.EndRPC();
                    PlayerControl.LocalPlayer.NoCheckStartMeeting(null, true);
                }
                return;
            }
        });

        ChatCommandRegistry.Register("Revive", (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame && args.Length > 0)
            {
                var target = GetPlayer(args);
                if (target != null)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.RevivePlayer);
                    writer.Write(target.PlayerId);
                    writer.Write(true);
                    writer.Write(true);
                    writer.EndRPC();
                    RPCProcedure.RevivePlayer(target.PlayerId, true, true);
                }

                return;
            }
        });

        ChatCommandRegistry.Register("ls", (sender, args, chat) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("玩家列表：\n");
            foreach (var player in PlayerControl.AllPlayerControls.ToList().OrderBy(x => x.PlayerId))
            {
                if (player.Data == null || player.Data.Disconnected) continue;
                sb.AppendLine($"{player.PlayerId} - {player.Data.PlayerName}");
            }

            chat.AddChat(PlayerControl.LocalPlayer, sb.ToString());
        });

        ChatCommandRegistry.Register("tp", (sender, args, chat) =>
        {
            if (PlayerControl.LocalPlayer.IsDead() || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
            {
                var target = GetPlayer(args);
                if (target != null)
                {
                    PlayerControl.LocalPlayer.transform.position = target.transform.position;
                    return;
                }
            }
        });

        ChatCommandRegistry.Register("r", (sender, args, chat) =>
        {
            if (args == null || args.Length == 0)
            {
                chat.AddChat(PlayerControl.LocalPlayer, "Usage: /r {role}");
                return;
            }
            foreach (var role in args)
            {
                var roleText = RoleInfo.getRoleDescription(role);
                if (roleText != null) chat.AddChat(PlayerControl.LocalPlayer, roleText);
            }
        });

        ChatCommandRegistry.Register("m", (sender, args, chat) =>
        {
            if (!InGame || PlayerControl.LocalPlayer == null)
            {
                chat.AddChat(PlayerControl.LocalPlayer, "NotInGame".Translate());
                return;
            }

            var localRole = RoleInfo.getRoleInfoForPlayer(PlayerControl.LocalPlayer);
            foreach (var roleInfo in localRole)
            {
                if (roleInfo.roleId == RoleId.Cursed) continue;
                var roleText = RoleInfo.getRoleDescription(roleInfo.Name);
                chat.AddChat(PlayerControl.LocalPlayer, roleText);
            }
        });

        ChatCommandRegistry.Register("room", (sender, args, chat) =>
        {
            if (!InGame && AmongUsClient.Instance.AmHost && args.Length > 0
            && AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame)
            {
                if (int.TryParse(args[0], out var LobbyLimit))
                {
                    LobbyLimit = Math.Clamp(LobbyLimit, 4, CrowdedPlayer.MaxPlayer);
                    if (LobbyLimit != GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers)
                    {
                        GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers = LobbyLimit;
                        FastDestroyableSingleton<GameStartManager>.Instance.LastPlayerCount = LobbyLimit;
                        // TODO Maybe simpler?? 
                        PlayerControl.LocalPlayer.RpcSyncSettings(
#if MXYX_CLUB
                            GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.currentGameOptions));
#else
                            GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.currentGameOptions, false));
#endif
                        chat.AddChat(PlayerControl.LocalPlayer, $"Lobby Size changed to {LobbyLimit} players");
                    }
                    else
                    {
                        chat.AddChat(PlayerControl.LocalPlayer, $"Lobby Size is already {LobbyLimit}");
                    }
                }
                else
                {
                    chat.AddChat(PlayerControl.LocalPlayer, "Invalid Size\nUsage: /room {amount}");
                }
            }
        });

        static PlayerControl GetPlayer(string[] args = null)
        {
            if (args == null || args.Length == 0)
            {
                return PlayerControl.LocalPlayer;
            }
            if (string.IsNullOrEmpty(args[0]))
            {
                return PlayerControl.LocalPlayer;
            }

            var target = PlayerControl.AllPlayerControls.FirstOrDefault(x => x.Data.PlayerName.Equals(args[0]));

            if (target == null && byte.TryParse(args[0], out var result))
            {
                target = PlayerById(result);
            }

            return target;
        }
    }
}

public delegate void ChatCommandHandler(PlayerControl sender, string[] args, ChatController chat);
public static class ChatCommandRegistry
{
    private static readonly Dictionary<string, ChatCommandHandler> _commands = new(StringComparer.OrdinalIgnoreCase);

    public static void Register(string command, ChatCommandHandler handler)
    {
        _commands[command] = handler;
    }

    public static bool TryHandle(string input, PlayerControl sender, ChatController chat)
    {
        if (!input.StartsWith("/")) return false;
        var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return false;
        var cmd = parts[0][1..];
        var args = parts.Skip(1).ToArray();

        var match = _commands.Keys.FirstOrDefault(k => k.Equals(cmd, StringComparison.OrdinalIgnoreCase))
                 ?? _commands.Keys.FirstOrDefault(k => k.StartsWith(cmd, StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            _commands[match](sender, args, chat);
            return true;
        }
        chat.AddChat(sender, $"Unknown command: {cmd}");
        return true;
    }
}