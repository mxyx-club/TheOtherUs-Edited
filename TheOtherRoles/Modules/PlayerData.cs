using BepInEx;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TheOtherRoles.Attributes;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Modules;

public class PlayerData
{
    public static PlayerData Local { get; private set; }
    public static Dictionary<byte, PlayerData> AllPlayerData { get; private set; } = new();
    public static Dictionary<byte, string> AllFriendCode = new();
    public PlayerControl Player { get; private set; }
    public byte PlayerId { get; private set; }

    public bool IsWinner;
    public bool IsDisconnected;
    public bool IsDead => Player.IsDead();
    public int KillCount;
    public Tuple<int, int> TaskCount;

    public RoleType RoleType { get; set; }
    public List<RoleId> RoleHistory { get; set; } = new();
    public RoleBase Role { get; set; }
    public List<ModifierBase> Modifiers { get; set; } = new();

    public string PlayerName { get; private set; }
    public string FriendCode { get; set; }
    public string PlayerColor { get; set; }

    public CustomDeathReason DeathReason { get; set; } = CustomDeathReason.Null;
    public DateTime DeathTimer { get; set; } = DateTime.MinValue;
    public PlayerControl KilledBy { get; set; }

    public static PlayerData GetPlayerData(PlayerControl player)
    {
        if (player?.Data == null) return null;
        if (AllPlayerData.TryGetValue(player.PlayerId, out var data))
        {
            return data;
        }
        return null;
    }

    public static string GetPlayerCode(PlayerControl player)
    {
        return AllFriendCode.TryGetValue(player.PlayerId, out var code) ? code : "";
    }

    [OnGameStart]
    public static void Init()
    {
        AllPlayerData = new();
        AllFriendCode = new();

        var code = EOSManager.Instance?.FriendCode ?? "";

        var writer = StartRPC(CustomRPC.ShareFriendCode);
        writer.Write(PlayerControl.LocalPlayer.PlayerId);
        writer.Write(code);
        writer.EndRPC();
        ShareFriendCode(PlayerControl.LocalPlayer.PlayerId, code);

        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            var data = new PlayerData
            {
                Player = player,
                PlayerId = player.PlayerId,
                PlayerName = player.Data.PlayerName,
                FriendCode = player.Data.FriendCode,
                PlayerColor = GetPlayerCode(player),
            };
            AllPlayerData[player.PlayerId] = data;
        }
    }

    public static int GetKillCount(PlayerControl killer)
    {
        if (killer == null) return 0;

        return AllPlayerData.Values.Count(data => data.KilledBy == killer && data.Player != killer);
    }

    public static PlayerControl GetLastKiller()
    {
        var data = AllPlayerData.Values
            .Where(x => x.IsDead && x.Player != x.KilledBy && x.KilledBy.IsAlive())?
            .OrderByDescending(dp => dp.DeathTimer)?
            .FirstOrDefault();
        return data?.KilledBy;
    }

    public static string GetDeathReasonString(PlayerControl p)
    {
        if (p.IsAlive() || !CanSeeGhostInfo) return "";

        var deadPlayer = AllPlayerData.Values.FirstOrDefault(x => x.Player.PlayerId == p.PlayerId);
        if (deadPlayer == null) return "";

        var reason = deadPlayer.DeathReason;
        var killer = deadPlayer.KilledBy;
        var killerName = deadPlayer.KilledBy?.Data.PlayerName ?? "NULL";

        Color killerColor = Palette.CrewmateBlue;
        if (deadPlayer != null && deadPlayer.KilledBy != null)
            killerColor = deadPlayer.KilledBy.GetRoleInfo()?.Color ?? Palette.CrewmateBlue;

        killerName = Cs(killerColor, killerName);
        return string.Format(GetString($"DeathReason.{reason}"), killerName);
    }

    public static void SetDeathReason(PlayerControl player, CustomDeathReason deathReason, PlayerControl killer = null)
    {
        if (player.IsAlive()) return;
        if (!AllPlayerData.TryGetValue(player.PlayerId, out var target)) return;

        byte playerId = player.PlayerId;

        target.DeathReason = deathReason;
        target.DeathTimer = DateTime.UtcNow;
        if (killer != null) target.KilledBy = killer;
    }

    public static void RpcSetDeathReason(PlayerControl player, CustomDeathReason deathReason, PlayerControl killer)
    {
        var writer = StartRPC(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareDeathReasonAndKiller);
        writer.Write(player.PlayerId);
        writer.Write((byte)deathReason);
        writer.Write(killer.PlayerId);
        writer.EndRPC();
        SetDeathReason(player, deathReason, killer);
    }

    public static void ShareFriendCode(byte playerId, string code)
    {
        try
        {
            AllFriendCode[playerId] = code;
            GameData.Instance?.GetPlayerById(playerId)?.FriendCode = code;
        }
        catch (Exception e) { Message($"Error reading friend code: {e.Message}", "ShareFriendCode"); }
    }

    public static implicit operator PlayerControl(PlayerData data) => data.Player;

    public class GlobalInfo
    {
        private const string ApiUrl = "http://localhost:6997/game-data";
        private static readonly HttpClient httpClient = new();

        public static string HostPlayer;
        public static DateTime StartTime;
        public static DateTime EndTime;
        internal static WinCondition WinCondition { get; set; } = WinCondition.Default;
        public static string RoomCode { get => field.IsNullOrWhiteSpace() ? "Local" : field; set; }
        public static string HostCode { get; set; }
        public static int PlayerCount { get; set; }

        [OnGameStart]
        public static void Init()
        {
            EndTime = DateTime.MinValue;
            StartTime = DateTime.UtcNow;
            HostPlayer = GetHostPlayer.Data.PlayerName;
        }

        public static string GetGameId()
        {
            var timePart = StartTime.ToString("HH:mm");
            var rawData = $"{HostCode}:{timePart}";

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                var hexHash = BitConverter.ToString(hashBytes, 0, 4)
                    .Replace("-", "")
                    .ToLowerInvariant();
                return $"{RoomCode}_{hexHash}";
            }
        }

        public static void SaveAllPlayerDataToJson()
        {
            foreach (var data in AllFriendCode)
            {
                Message($"{data.Key}: {data.Value}");
            }

            var directoryPath = Path.Combine(Paths.GameRootPath, Main.Name, "GameData");
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filePath = Path.Combine(directoryPath, $"GameSession_{timestamp}.json");
            Directory.CreateDirectory(directoryPath);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };

            var gameSession = new
            {
                Global = new
                {
                    ModVersion = $"{Main.Name} - {Main.Version}{Main.VersionSuffix}",
                    GameVersion = Application.version,
                    GameId = GetGameId(),
                    HostPlayer,
                    StartTime = StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndTime = EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Duration = (EndTime - StartTime).ToString(@"hh\:mm\:ss"),
                    WinCondition = WinCondition.ToString(),
                    RoomCode,
                    PlayerCount,
                    HostCode,
                    GameMode = ModOption.GameMode.ToString(),
                    DeBugMode = ModOption.DebugMode,
                    RoleDraftMode = CustomOptionHolder.isDraftMode.GetBool(),
                },

                Players = AllPlayerData.Values.Select(p => new
                {
                    p.PlayerId,
                    p.PlayerName,
                    p.PlayerColor,
                    p.FriendCode,
                    RoleInfo = new
                    {
                        OriginRole = p.Role.RoleId.ToString(),
                        MainRole = p.Role.ToString(),
                        //Modifiers = string.Join("|", p.Modifiers),
                        Modifiers = p.Modifiers.Select(x => x.RoleId.ToString()),
                        //RoleHistory = string.Join(" => ", p.RoleHistory.Select(r => r.ToString())),
                        RoleHistory = p.RoleHistory.Select(x => x.ToString()),
                        RoleType = p.RoleType.ToString(),
                    },
                    GameplayStats = new
                    {
                        p.IsWinner,
                        p.IsDead,
                        p.IsDisconnected,
                        p.KillCount,
                        Tasks = p.TaskCount != null ? new
                        {
                            Completed = p.TaskCount.Item1,
                            Total = p.TaskCount.Item2,
                            Progress = $"{p.TaskCount.Item1 / (double)p.TaskCount.Item2:P0}"
                        } : null,
                        p.DeathReason,
                        p.KilledBy,
                        DeathTimer = p.DeathTimer.ToString("yyyy-MM-ddTHH:mm:ss"),
                    },
                }).ToList()
            };

            var jsonContent = JsonSerializer.Serialize(gameSession, jsonOptions);
            File.WriteAllText(filePath, jsonContent);

            UploadPlayerDataToApi(jsonContent).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Error($"Upload failed: {task.Exception?.InnerException?.Message}", "PlayerData");
                }
                else
                {
                    Info("Data uploaded successfully!", "PlayerData");
                }
            });
        }

        private static async Task UploadPlayerDataToApi(string jsonContent)
        {
            try
            {
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(ApiUrl, content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Error($"API returned {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Error("API upload error: " + ex.Message);
            }
        }
    }
}

public enum CustomDeathReason
{
    Null,

    HostKill,
    Exile,
    Kill,
    Disconnect,
    Guess,
    Shift,
    LawyerSuicide,
    LoverSuicide,
    WitchExile,
    Bomb,
    Veteran,
    LoveStolen,
    Loneliness,
    Arson,
    FakeSK,
    SheriffKill,
    SheriffMisfire,
    SheriffMisadventure,
    Suicide,
    BombVictim,
    Eaten,
    Jailed,
}
/*
public class PlayerData
{
    private const string ApiUrl = "https://test.com/game-data";
    private static readonly HttpClient httpClient = new();
    public static List<PlayerDataInfo> AllPlayerData = new();

    public static Dictionary<byte, string> AllFriendCode = new();

    public static string HostPlayer;
    public static DateTime StartTime;
    public static DateTime EndTime;
    internal static WinCondition WinCondition { get; set; } = WinCondition.Default;
    public static string RoomCode { get => field.IsNullOrWhiteSpace() ? "Loacl" : field; set; }
    public static string HostCode { get; set; }
    public static int PlayerCount { get; set; }

    public static PlayerDataInfo GetPlayerData(PlayerControl player)
    {
        return AllPlayerData.FirstOrDefault(p => p.Player == player);
    }

    public static string GetPlayerCode(PlayerControl player)
    {
        return AllFriendCode.TryGetValue(player.PlayerId, out var code) ? code : "";
    }

    [GameStart]
    public static void Start()
    {
        AllPlayerData.Clear();
        AllFriendCode.Clear();

        var code = EOSManager.Instance?.FriendCode ?? "";

        var writer = StartRPC(CustomRPC.ShareFriendCode);
        writer.Write(PlayerControl.LocalPlayer.PlayerId);
        writer.Write(code);
        writer.EndRPC();
        ShareFriendCode(PlayerControl.LocalPlayer.PlayerId, code);
    }

    public static void ShareFriendCode(byte playerId, string code)
    {
        try
        {
            PlayerData.AllFriendCode[playerId] = code;
            GameData.Instance?.GetPlayerById(playerId)?.FriendCode = code;
        }
        catch (Exception e) { Message($"Error reading friend code: {e.Message}", "ShareFriendCode"); }

    }

    public static void Initialize()
    {
        try
        {
            StartTime = DateTime.UtcNow;
            WinCondition = WinCondition.Default;
            EndTime = DateTime.UtcNow;
            HostCode = GetHostPlayer?.Data?.FriendCode ?? "ERROR";
            HostPlayer = GetHostPlayer?.Data?.PlayerName ?? "ERROR";
            PlayerCount = PlayerControl.AllPlayerControls.Count;
            RoomCode = GameStartManagerPatch.RoomCode;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var playerData = new PlayerDataInfo
                {
                    Player = player,
                    PlayerId = player.PlayerId,
                    PlayerCode = AllFriendCode[player.PlayerId],
                    PlayerColor = player.Data.ColorName ?? "Default",
                    OriginRole = player.GetRole(),

                };
                AllPlayerData.Add(playerData);
            }
        }
        catch (Exception e)
        {
            Error($"initializing Error: {e.Message}\n{e.StackTrace}", "PlayerData");
        }
    }

    public static string GetGameId()
    {
        var timePart = StartTime.ToString("HH:mm");
        var rawData = $"{HostCode}:{timePart}";

        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            var hexHash = BitConverter.ToString(hashBytes, 0, 4)
                .Replace("-", "")
                .ToLowerInvariant();
            return $"{RoomCode}_{hexHash}";
        }
    }

    public static void SaveAllPlayerDataToJson()
    {
        foreach (var data in AllFriendCode)
        {
            Message($"{data.Key}: {data.Value}");
        }

        var directoryPath = Path.Combine(Paths.GameRootPath, Main.Name, "GameData");
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filePath = Path.Combine(directoryPath, $"GameSession_{timestamp}.json");
        Directory.CreateDirectory(directoryPath);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        var gameSession = new
        {
            Global = new
            {
                ModVersion = $"{Main.Name} - {Main.Version}{Main.VersionSuffix}",
                GameVersion = Application.version,
                GameId = GetGameId(),
                HostPlayer,
                StartTime = StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                EndTime = EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                Duration = (EndTime - StartTime).ToString(@"hh\:mm\:ss"),
                WinCondition = WinCondition.ToString(),
                RoomCode,
                PlayerCount,
                HostCode,
                GameMode = ModOption.gameMode.ToString(),
                DeBugMode = ModOption.DebugMode,
                RoleDraftMode = CustomOptionHolder.isDraftMode.GetBool(),
            },

            Players = AllPlayerData.Select(p => new
            {
                p.PlayerId,
                p.PlayerName,
                p.PlayerColor,
                p.PlayerCode,
                RoleInfo = new
                {
                    OriginRole = p.OriginRole.ToString(),
                    MainRole = p.Role.ToString(),
                    RoleDetails = p.AllRole.Select(r => r.RoleId.ToString()).ToArray(),
                    RoleType = p.RoleType.ToString(),
                },
                GameplayStats = new
                {
                    p.IsWinner,
                    p.IsDead,
                    p.IsDisconnected,
                    p.KillCount,
                    Tasks = p.TaskCount != null ? new
                    {
                        Completed = p.TaskCount.Item1,
                        Total = p.TaskCount.Item2,
                        Progress = $"{p.TaskCount.Item1 / (double)p.TaskCount.Item2:P0}"
                    } : null,
                    p.DeathReason,
                    p.KilledBy,
                    DeathTimer = p.DeathTimer.ToString("yyyy-MM-ddTHH:mm:ss"),
                },
            }).ToList()
        };

        var jsonContent = JsonSerializer.Serialize(gameSession, jsonOptions);
        File.WriteAllText(filePath, jsonContent);

        UploadPlayerDataToApi(jsonContent).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Error($"Upload failed: {task.Exception?.InnerException?.Message}", "PlayerData");
            }
            else
            {
                Info("Data uploaded successfully!", "PlayerData");
            }
        });
    }

    private static async Task UploadPlayerDataToApi(string jsonContent)
    {
        try
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ApiUrl, content).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Error($"API returned {response.StatusCode}: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Error("API upload error: " + ex.Message);
        }
    }

    public class PlayerDataInfo
    {
        public PlayerControl Player { get; set; }
        public string PlayerName => Player?.Data?.PlayerName ?? "Unknown";
        public string PlayerCode { get; set; }
        public string PlayerColor { get; set; }
        public byte PlayerId { get; set; }
        public Tuple<int, int> TaskCount { get; set; }
        public RoleId Role { get; set; } = RoleId.DefaultRole;
        public RoleId OriginRole { get; set; } = RoleId.DefaultRole;
        public RoleInfo[] AllRole { get; set; } = [];
        public RoleType RoleType { get; set; } = RoleType.Special;
        public bool IsDead => Player.IsDead();
        public bool IsWinner { get => !IsDisconnected && field; set; }
        public int KillCount => GameHistory.GetKillCount(Player);
        public bool IsDisconnected => Player?.Data?.Disconnected ?? true;

        public string DeathReason
        {
            get
            {
                if (!IsDead || GameHistory.DeadPlayers == null || GameHistory.DeadPlayers.Count == 0) return "Alive";
                return GameHistory.GetDeadPlayer(PlayerId)?.DeathReason.ToString() ?? "Unknown";
            }
        }

        public DateTime DeathTimer
        {
            get
            {
                if (!IsDead) return DateTime.MinValue;
                return GameHistory.GetDeadPlayer(PlayerId)?.TimeOfDeath ?? DateTime.MinValue;
            }
        }

        public string KilledBy
        {
            get
            {
                if (!IsDead) return "Unknown";
                var killer = GameHistory.GetDeadPlayer(PlayerId)?.KillerIfExisting;
                return killer != null ? killer?.Data?.PlayerName ?? "Error" : "Unknown";
            }
        }
    }

}*/