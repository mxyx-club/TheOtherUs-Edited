using AmongUs.GameOptions;
using System.Text;
using TheOtherRoles.Attributes;
using TheOtherRoles.Patches;
using static TheOtherRoles.Patches.ChatControllerPatch;

namespace TheOtherRoles.Modules;


public delegate void ChatCommandHandler(PlayerControl sender, string[] args, ChatController chat);
public static class ChatCommandRegistry
{
    private static readonly Dictionary<string, ChatCommandHandler> _commands = new(StringComparer.OrdinalIgnoreCase);

    public static void Register(string command, ChatCommandHandler handler)
    {
        _commands[command] = handler;
    }

    public static void Register(IEnumerable<string> commands, ChatCommandHandler handler)
    {
        foreach (var cmd in commands)
        {
            _commands[cmd] = handler;
        }
    }

    public static bool TryHandle(string input, PlayerControl sender, ChatController chat)
    {
        if (!input.StartsWith("/")) return false;
        var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return false;
        var cmd = parts[0][1..];
        var args = parts.Skip(1).ToArray();

        if (_commands.TryGetValue(cmd, out var handler))
        {
            handler(sender, args, chat);
            return true;
        }

        var matches = _commands.Keys.Where(k => k.StartsWith(cmd, StringComparison.OrdinalIgnoreCase)).ToList();
        if (matches.Count == 1)
        {
            _commands[matches[0]](sender, args, chat);
            return true;
        }
        else if (matches.Count > 1)
        {
            chat.AddChat(sender, string.Format(GetString("Command.Ambiguous"), cmd, string.Join(", ", matches)));
            return true;
        }

        chat.AddChat(sender, string.Format(GetString("Command.Unknown"), cmd));
        return true;
    }


    [PluginModuleInitializer]
    public static void Initialize()
    {
        Register("end", (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame)
            {
                ModOption.isCanceled = true;
                return;
            }
        });

        Register("say", (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame && args.Length > 0)
            {
                var message = string.Join(' ', args);
                var writer = StartRPC(CustomRPC.HostControl);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.HostCommand.HostSay);
                writer.Write(message);
                writer.EndRPC();
                CurrentChatType = ChatTypes.HostChat;
                chat.AddChat(GetHostPlayer, message);
                return;
            }
        });

        Register("cmd", (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost)
                chat.AddChat(PlayerControl.LocalPlayer, "CommandsInHost".Translate());
            chat.AddChat(PlayerControl.LocalPlayer, "CommandsInPlayer".Translate());
        });

        Register(["kill", "kl"], (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame)
            {
                var target = GetPlayer(args);
                if (target != null)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.HostControl);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.HostCommand.HostKill);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();

                    target.Exiled();
                    PlayerData.SetDeathReason(target, CustomDeathReason.HostKill, PlayerControl.LocalPlayer);

                    DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
                    foreach (var body in array)
                    {
                        if (body.ParentId != target.PlayerId) continue;
                        UObject.Destroy(body.gameObject);
                        break;
                    }

                    if (InMeeting)
                        MeetingHud.Instance.CheckForEndVoting();
                    return;
                }
            }
        });

        Register(["exile", "ex"], (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame)
            {
                var target = GetPlayer(args);
                if (target != null)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.HostControl);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.HostCommand.HostExile);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();

                    ExileControllerBeginPatch.ForceExile = true;
                    if (InMeeting)
                        MeetingHud.Instance.RpcVotingComplete(Array.Empty<MeetingHud.VoterState>(), target.Data, false);
                }
                return;
            }
        });

        Register(["meeting", "mt"], (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame)
            {
                if (InMeeting)
                    MeetingHud.Instance.RpcVotingComplete(Array.Empty<MeetingHud.VoterState>(), null, false);
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

        Register(["revive", "rv"], (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame && args.Length > 0)
            {
                var target = GetPlayer(args);
                if (target != null)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.HostControl);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.HostCommand.HostRevive);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();
                    RPCProcedure.RevivePlayer(target.PlayerId, true, true);
                }

                return;
            }
        });

        Register(["list", "ls"], (sender, args, chat) =>
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

        Register("tp", (sender, args, chat) =>
        {
            if (PlayerControl.LocalPlayer.IsDead() || ModOption.DebugMode || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
            {
                if (args == null || args.Length == 0)
                {
                    chat.AddChat(PlayerControl.LocalPlayer, "Usage: /tp {player}");
                    return;
                }
                var target = GetPlayer(args);
                if (target != null)
                {
                    PlayerControl.LocalPlayer.transform.position = target.transform.position;
                    return;
                }
            }
        });

        Register("r", (sender, args, chat) =>
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

        Register(["setrole", "sr"], (sender, args, chat) =>
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                chat.AddChat(PlayerControl.LocalPlayer, "You Not Host Player!");
                return;
            }
            if (!InGame)
            {
                chat.AddChat(PlayerControl.LocalPlayer, "NotInGame".Translate());
                return;
            }

            if (args == null || args.Length == 0)
            {
                chat.AddChat(PlayerControl.LocalPlayer,
                    "用来变更目标玩家职业的指令\n\n" +
                    "格式: /sr <职业> <玩家>\n" +
                    "• /sr <职业> - 设置自己的职业\n" +
                    "• /sr <角色> <玩家ID> - 设置他人角色\n" +
                    "• /sr ls - 显示可用职业列表与ID\n" +
                    "• /sr 0 <玩家> 清除目标的职业");
            }
            else if (args.Length == 1 && args[0].Equals("ls", StringComparison.OrdinalIgnoreCase))
            {
                var impostorSb = new StringBuilder();
                impostorSb.AppendLine("伪装者阵营:");
                foreach (var info in RoleInfo.allRoleInfos.Where(x => x.roleType == RoleType.Impostor))
                {
                    impostorSb.AppendLine($"• {(int)info.roleId} - {info.Name}");
                }
                chat.AddChat(PlayerControl.LocalPlayer, impostorSb.ToString());

                var neutralSb = new StringBuilder();
                neutralSb.AppendLine("独立阵营:");
                foreach (var info in RoleInfo.allRoleInfos.Where(x => x.roleType == RoleType.Neutral))
                {
                    neutralSb.AppendLine($"• {(int)info.roleId} - {info.Name}");
                }
                chat.AddChat(PlayerControl.LocalPlayer, neutralSb.ToString());

                var crewmateSb = new StringBuilder();
                crewmateSb.AppendLine("船员阵营:");
                foreach (var info in RoleInfo.allRoleInfos.Where(x => x.roleType == RoleType.Crewmate))
                {
                    crewmateSb.AppendLine($"• {(int)info.roleId} - {info.Name}");
                }
                chat.AddChat(PlayerControl.LocalPlayer, crewmateSb.ToString());
            }
            else if (args.Length > 0 && args[0].Equals("0", StringComparison.OrdinalIgnoreCase))
            {
                var target = GetPlayer(args.Length > 1 ? args.Skip(1).ToArray() : null);
                if (target != null)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.HostControl);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.HostCommand.HostClearRole);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();

                    Message("Clean Role:" + target.Data.PlayerName);
                    RPCProcedure.erasePlayerRoles(target.PlayerId, false);

                    chat.AddChat(PlayerControl.LocalPlayer, $"Clear {target.Data.PlayerName} the Role!");
                }
            }
            else if (AmongUsClient.Instance.AmHost && args.Length > 0)
            {
                var roleId = GetRoleId(args);
                var target = GetPlayer(args.Length > 1 ? args.Skip(1).ToArray() : null);
                if (target != null)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.HostControl);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.HostCommand.HostSetRole);
                    writer.Write(target.PlayerId);
                    writer.Write((byte)roleId);
                    writer.EndRPC();

                    Message("Set Role:" + target.Data.PlayerName);
                    if (target != null && RoleInfo.RoleInfoById.TryGetValue(roleId, out var info))
                    {
                        if (info.roleType == RoleType.Impostor)
                        {
                            target.Data.Role.TeamType = RoleTeamTypes.Impostor;
                            SetRoleType(target, RoleTypes.Impostor);
                        }
                        else
                        {
                            target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                            SetRoleType(target, RoleTypes.Crewmate);

                        }
                        RPCProcedure.setRole(target.PlayerId, (byte)roleId);
                    }

                    chat.AddChat(PlayerControl.LocalPlayer, $"Set {target.Data.PlayerName} the role {roleId}");
                }
            }
        });

        Register(["clearrole", "cr"], (sender, args, chat) =>
        {
            if (!AmongUsClient.Instance.AmHost || PlayerControl.LocalPlayer == null)
            {
                chat.AddChat(PlayerControl.LocalPlayer, "You Not Host Player!");
                return;
            }
            else if (!InGame)
            {
                chat.AddChat(PlayerControl.LocalPlayer, "NotInGame".Translate());
                return;
            }
            else if (args == null || args.Length == 0)
            {
                chat.AddChat(PlayerControl.LocalPlayer, "Usage: /cr <player>\nClear the target player role");
                return;
            }

            var target = GetPlayer(args);
            if (target != null)
            {
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.HostControl);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.HostCommand.HostClearRole);
                writer.Write(target.PlayerId);
                writer.EndRPC();

                Message("Clean Role:" + target.Data.PlayerName);
                RPCProcedure.erasePlayerRoles(target.PlayerId, false);

                chat.AddChat(PlayerControl.LocalPlayer, $"Clear {target.Data.PlayerName} the Role!");
            }
        });

        Register("m", (sender, args, chat) =>
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

        Register(["jail", "jl"], (sender, args, chat) =>
        {
            if (sender == Jailor.Player && sender.IsAlive() && Jailor.Jailed != null && InMeeting)
            {
                var message = string.Join(' ', args).Trim();
                var writer = StartRPC(CustomRPC.JailorSendMessage);
                writer.Write(Jailor.Player.PlayerId);
                writer.Write(message);
                writer.EndRPC();
                Jailor.JailorSendMessage(Jailor.Player, message);

                return;
            }
        });

        Register(["room", "size"], (sender, args, chat) =>
        {
            if (!InGame && AmongUsClient.Instance.AmHost && AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame)
            {
                if (args == null || args.Length == 0)
                {
                    chat.AddChat(PlayerControl.LocalPlayer, "Usage: /room {amount}\nSet the lobby size");
                    return;
                }
                if (int.TryParse(args[0], out var LobbyLimit))
                {
                    LobbyLimit = Math.Clamp(LobbyLimit, 4, CrowdedPlayer.MaxPlayer);
                    if (LobbyLimit != GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers)
                    {
                        GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers = LobbyLimit;
                        FastDestroyableSingleton<GameStartManager>.Instance.LastPlayerCount = LobbyLimit;
                        // TODO Maybe simpler?? 
                        PlayerControl.LocalPlayer.RpcSyncSettings(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.currentGameOptions));
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

        Register(["cleartask", "ct"], (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InGame)
            {
                if (args == null || args.Length == 0)
                {
                    chat.AddChat(PlayerControl.LocalPlayer, "Usage: /ct <player>\nClear the target player tasks");
                    return;
                }
                var target = GetPlayer(args);
                if (target != null)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.HostControl);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)RPCProcedure.HostCommand.HostClearTasks);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();

                    target.clearAllTasks();
                    chat.AddChat(PlayerControl.LocalPlayer, $"Cleared {target?.Data?.PlayerName} Tasks");
                }
            }
        });

        Register(["setname", "sn"], (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost || !InGame || ModOption.DebugMode)
            {
                if (AmongUsClient.Instance.AmHost && args.Length == 2)
                {
                    var player = GetPlayer(args);
                    var text = string.Join(' ', args[1..]).Trim();
                    player.RpcSetName(text);
                }
                else if (args.Length == 1)
                {
                    var text = string.Join(' ', args).Trim();
                    sender.RpcSetName(text);
                }
                else
                {
                    chat.AddChat(sender, "/setname <new_name>");
                }
            }
        });

        Register(["clearvote", "cv"], (sender, args, chat) =>
        {
            if (AmongUsClient.Instance.AmHost && InMeeting)
            {
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.HostControl);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.HostCommand.HostClearVotes);
                writer.Write(byte.MaxValue);
                writer.EndRPC();

                MeetingHud.Instance.playerStates.ForEach((x) =>
                {
                    x.UnsetVote();
                });
                MeetingHud.Instance.ClearVote();
                chat.AddChat(PlayerControl.LocalPlayer, $"Cleared  All Votes");
            }
            else
            {
                chat.AddChat(PlayerControl.LocalPlayer, "You are not the host or not in a meeting.");
            }
        });

        static RoleId GetRoleId(string[] args = null)
        {
            if (args == null || args.Length == 0)
                return RoleId.DefaultRole;
            if (int.TryParse(args[0], out var roleId))
                return (RoleId)roleId;
            var infos = RoleInfo.allRoleInfos.Where(x => x.Name.StartsWith(args[0], StringComparison.OrdinalIgnoreCase));
            if (infos.Count() == 1)
                return infos.First().roleId;
            else if (infos.Count() > 1)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Ambiguous Role Name:");
                foreach (var info in infos)
                {
                    sb.AppendLine($"• {info.roleId} - {info.Name}");
                }
                Message(sb.ToString());
                return RoleId.DefaultRole;
            }
            return RoleId.DefaultRole;
        }

        static PlayerControl GetPlayer(string[] args = null)
        {
            if (args == null || args.Length == 0)
                return PlayerControl.LocalPlayer;
            if (string.IsNullOrEmpty(args[0]))
                return PlayerControl.LocalPlayer;

            var target = PlayerControl.AllPlayerControls.FirstOrDefault(x => x.Data.PlayerName.Equals(args[0], StringComparison.OrdinalIgnoreCase));

            if (target == null && byte.TryParse(args[0], out var result))
                target = PlayerById(result);

            return target;
        }
    }
}