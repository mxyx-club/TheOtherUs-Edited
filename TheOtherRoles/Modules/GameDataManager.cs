#nullable enable
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

public partial class GameDataManager : ManagerBase<GameDataManager>
{
    public Dictionary<byte, string> AllFriendCode { get; private set; } = new();
    public Dictionary<byte, ushort> AnonymousId { get; private set; } = new();
    public List<PlayerControl> AllPlayerControl { get; private set; } = new();

    public string? GameId { get; set; }
    public string? HostPlayer { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; set; }
    public string? SessionToken { get; private set; }
    internal WinCondition WinCondition { get; set; } = WinCondition.Default;
    public string? RoomCode { get => field.IsNullOrWhiteSpace() ? "Local" : field; set; }
    public string? HostCode { get; set; }
    public int PlayerCount { get; set; }
    public byte? MapId { get; set; }

    private bool _isInitialized;

    public const string ApiUrl = "https://toue.mxyx.club";
    private static readonly HttpClient _httpClient = new();
    private static readonly HttpClient _tokenClient = new();

    public void Initialize()
    {
        if (_isInitialized) return;

        AllFriendCode.Clear();
        AnonymousId.Clear();
        AllPlayerControl.Clear();
        ClearEvents();

        EndTime = DateTime.MinValue;
        StartTime = DateTime.UtcNow;

        HostPlayer = Helpers.HostPlayer?.Data?.PlayerName ?? "Unknown";
        RoomCode = GameStartManagerPatch.RoomCode;
        HostCode = Helpers.HostPlayer?.Data?.FriendCode ?? "";
        PlayerCount = PlayerControl.AllPlayerControls?.Count ?? 0;
        MapId = GameOptionsManager.Instance?.currentNormalGameOptions?.MapId;

        if (AmongUsClient.Instance.AmHost)
        {
            GameId = GenerateGameId();
            var writer = StartRPC(CustomRPC.ShareGameId);
            writer.Write(GameId);
            writer.EndRPC();
        }

        byte modUid = 1;
        foreach (var player in PlayerControl.AllPlayerControls!.ToArray().OrderBy(_ => rnd.Next()))
        {
            AnonymousId[player.PlayerId] = modUid++;
            AllPlayerControl.Add(player);
        }

        _ = Task.Run(async () =>
        {
            SessionToken = await GetSessionTokenAsync();
        });

        ModOption.isCanceled = false;
        _isInitialized = true;
        RecordEvent("GameIntro");
    }

    [OnGameStart(-50)]
    public static void OnGameStart()
    {
        Instance.Initialize();
    }

    [OnGameEnd(50)]
    public static void OnGameEnd()
    {
        if (Instance._isInitialized && AmongUsClient.Instance.AmHost) Instance.SaveAllPlayerDataToJson();
        Instance.Reset();
    }

    public static string GetPlayerCode(PlayerControl player)
    {
        return Instance.AllFriendCode.TryGetValue(player.PlayerId, out var code) ? code : "";
    }

    public void ShareFriendCode(byte playerId, string code)
    {
        try
        {
            AllFriendCode[playerId] = code;
            if (PlayerData.AllPlayerData.TryGetValue(playerId, out var playerData))
            {
                playerData.FriendCode = code;
            }
            GameData.Instance?.GetPlayerById(playerId)?.FriendCode = code;
        }
        catch (Exception e) { Message($"Error reading friend code: {e.Message}", "ShareFriendCode"); }
    }

    private string GenerateGameId()
    {
        var timePart = StartTime.Ticks.ToString();
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(timePart));
        var hexHash = BitConverter.ToString(hashBytes, 0, 4).Replace("-", "").ToLowerInvariant();
        return $"{RoomCode}_{hexHash}";
    }

    public void SaveAllPlayerDataToJson()
    {
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
                GameId,
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
                MapId,
            },

            Players = PlayerData.AllPlayerData.Values.Select(p => new
            {
                p.PlayerId,
                p.PlayerName,
                p.ColorName,
                PlayerCode = p.FriendCode,
                RoleInfo = new
                {
                    OriginRole = p.OriginRole.ToString(),
                    MainRole = p.MainRole.ToString(),
                    Modifiers = p.Modifiers.Select(x => x.ToString()),
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
                    KilledBy = p.KilledBy?.Data?.PlayerName ?? "null",
                    DeathTimer = p.DeathTimer.ToString("yyyy-MM-ddTHH:mm:ss"),
                },
            }).ToList(),

            Events = EventLog.Select(e => new object?[]
            {
                e.EventType,
                e.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                e.GameTime,
                e.SourcePlayerId,
                e.TargetPlayerId,
                e.Extra,
            }).ToList()
        };

        var jsonContent = JsonSerializer.Serialize(gameSession, jsonOptions);
        File.WriteAllText(filePath, jsonContent);

        if (ModOption.uploadGameData)
        {
            if (WinCondition == WinCondition.Canceled)
            {
                Info("Game was canceled, skipping data upload.", "GameDataManager");
                return;
            }

            var compactJsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };
            var compactJsonContent = JsonSerializer.Serialize(gameSession, compactJsonOptions);
            UploadPlayerData(compactJsonContent).ContinueWith(task => { Info("Data uploaded successfully!", "GameDataManager"); });
        }
    }

    private async Task<string> GetSessionTokenAsync()
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, ApiUrl + "/api/auth/get-session-token");
            request.Headers.Add("X-Game-Id", GameId);

            var response = await _tokenClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                return result.GetProperty("session_token").GetString() ?? "";
            }

            Error(content, nameof(GameDataManager));
            return "";
        }
        catch (Exception ex)
        {
            Error(ex.Message, nameof(GameDataManager));
            return "";
        }
    }

    private async Task UploadPlayerData(string jsonContent)
    {
        try
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            content.Headers.Add("X-Session-Token", SessionToken);
            content.Headers.Add("X-Game-Id", GameId);

            var response = await _httpClient.PostAsync(ApiUrl + "/api/games/v2", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Error($"Error: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }


    public void Reset()
    {
        _isInitialized = false;
        AllFriendCode.Clear();
        AnonymousId.Clear();
        AllPlayerControl.Clear();
        ClearEvents();
        WinCondition = WinCondition.Default;
        EndTime = DateTime.MinValue;
    }

    public class BindingVerifier
    {
        private static readonly HttpClient httpClient = new();

        public static async Task<(bool success, string? message)> VerifyBinding(string playerCode, string verificationCode)
        {
            try
            {
                if (playerCode.IsNullOrWhiteSpace()) return (false, "还没有登录Among Us，无法验证");
                else if (verificationCode.IsNullOrWhiteSpace()) return (false, "请输入验证码");
                var requestData = new
                {
                    player_code = playerCode,
                    verification_code = verificationCode
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var apiUrl = ApiUrl + "/api/auth/verify-playercode-binding";

                var response = await httpClient.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    bool success = result.GetProperty("success").GetBoolean();
                    var message = result.GetProperty("message").GetString();

                    if (success)
                    {
                        Info($"PlayerCode绑定成功: {message}", "BindingVerifier");
                        return (true, message);
                    }
                    else
                    {
                        Warn($"PlayerCode绑定失败: {message}", "BindingVerifier");
                        return (false, message);
                    }
                }
                else
                {
                    Error($"HTTP错误 {response.StatusCode}: {responseContent}", "BindingVerifier");
                    return (false, $"服务器错误 ({response.StatusCode})");
                }
            }
            catch (Exception ex)
            {
                Error($"验证绑定异常: {ex.Message}", "BindingVerifier");
                return (false, "网络错误，请检查网络连接");
            }
        }
    }

}

