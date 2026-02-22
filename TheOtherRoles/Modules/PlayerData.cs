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
    public static PlayerData Local { get => field ??= GetPlayerData(PlayerControl.LocalPlayer); private set; }
    public static Dictionary<byte, PlayerData> AllPlayerData = new();
    public static Dictionary<byte, string> AllFriendCode = new();
    public static Dictionary<byte, ushort> AnonymousId = new();

    public PlayerControl Player { get; private set; }
    public byte PlayerId { get; private set; }

    public bool IsWinner;
    public bool IsDisconnected;
    public bool IsDead => Player.IsDead();
    public int KillCount => GetKillCount(Player);
    public Tuple<int, int> TaskCount;

    public RoleInfo RoleInfo => RoleInfo.RoleInfoById.GetValueOrDefault(RoleId, RoleInfo.crewmate);
    public RoleType RoleType { get; set => field = value == RoleType.Error ? field : value; } = RoleType.Crewmate;
    public List<RoleId> RoleHistory = new();
    public RoleId RoleId = RoleId.DefaultRole;
    public RoleId OriginRole = RoleId.DefaultRole;
    public List<RoleId> Modifiers = new();
    public RoleId? GhostRole;

    public string PlayerName { get; set; }
    public string FriendCode { get; set; }
    public string ColorName { get; set; }

    public CustomDeathReason DeathReason { get; set; } = CustomDeathReason.Null;
    public DateTime DeathTimer { get; set; } = DateTime.MinValue;
    public PlayerControl KilledBy { get; set; }

    public int ColorId => Player.CurrentOutfit.ColorId;
    public string HatId => Player.CurrentOutfit.HatId;
    public string SkinId => Player.CurrentOutfit.SkinId;
    public string VisorId => Player.CurrentOutfit.VisorId;
    public string NamePlateId => Player.CurrentOutfit.NamePlateId;
    public string PetId => Player.CurrentOutfit.PetId;

    public static PlayerData GetPlayerData(PlayerControl player)
    {
        if (player?.Data == null) return null;
        if (AllPlayerData.TryGetValue(player.PlayerId, out var data))
        {
            return data;
        }
        else
        {
            data = new PlayerData
            {
                Player = player,
                PlayerId = player.PlayerId,
                PlayerName = player.Data.PlayerName,
                FriendCode = player.Data.FriendCode,
                ColorName = player.Data.ColorName,
            };
            AllPlayerData[player.PlayerId] = data;
            if (player == PlayerControl.LocalPlayer) Local = data;
            return data;
        }
    }

    public static PlayerData GetPlayerData(string name)
    {
        if (name.IsNullOrWhiteSpace()) return null;
        return AllPlayerData.Values.FirstOrDefault(data => data.PlayerName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public static string GetPlayerCode(PlayerControl player)
    {
        return AllFriendCode.TryGetValue(player.PlayerId, out var code) ? code : "";
    }

    [OnGameStart(Attributes.Priority.High)]
    public static void Init()
    {
        AllPlayerData = new();
        AllFriendCode = new();
        var code = EOSManager.Instance?.FriendCode ?? "";

        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            var data = new PlayerData
            {
                Player = player,
                PlayerId = player.PlayerId,
                PlayerName = player.Data.PlayerName,
                ColorName = player.Data.GetPlayerColorString(),
            };
            AllPlayerData[player.PlayerId] = data;
        }
        _ = new LateTask(() =>
        {
            if (ModOption.uploadGameData)
            {
                var writer = StartRPC(CustomRPC.ShareFriendCode);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(code);
                writer.EndRPC();
                ShareFriendCode(PlayerControl.LocalPlayer.PlayerId, code);
            }
        }, 1f, "ShareFriendCode");

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

    public static void SetDeathReason(PlayerControl player, CustomDeathReason deathReason, PlayerControl killer = null)
    {
        if (player.IsAlive()) return;
        if (!AllPlayerData.TryGetValue(player.PlayerId, out var data)) return;

        data.DeathReason = deathReason;
        data.DeathTimer = DateTime.UtcNow;
        if (killer != null)
        {
            data.KilledBy = killer;
        }
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
            AllPlayerData[playerId].FriendCode = code;
            GameData.Instance?.GetPlayerById(playerId)?.FriendCode = code;
        }
        catch (Exception e) { Message($"Error reading friend code: {e.Message}", "ShareFriendCode"); }
    }

    public class GlobalInfo
    {
        public static string ApiUrl => "https://api.toue.mxyx.club";
        private static readonly HttpClient httpClient = new();

        public static string GameId { get; private set; }

        public static string HostPlayer;
        public static DateTime StartTime;
        public static DateTime EndTime;
        public static string SessionToken { get; private set; }
        internal static WinCondition WinCondition { get; set; } = WinCondition.Default;
        public static string RoomCode { get => field.IsNullOrWhiteSpace() ? "Local" : field; set; }
        public static string HostCode { get; set; }
        public static int PlayerCount { get; set; }

        [OnGameStart]
        public static void Init()
        {
            EndTime = DateTime.MinValue;
            StartTime = DateTime.UtcNow;
            HostPlayer = Helpers.HostPlayer.Data.PlayerName;
            RoomCode = GameStartManagerPatch.RoomCode;
            HostCode = Helpers.HostPlayer.Data.FriendCode;
            GameId = GetGameId();
            PlayerCount = PlayerControl.AllPlayerControls.Count;
            SessionToken = GetSessionToken();
            byte modUid = 1;
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().OrderBy(_ => rnd.Next()))
            {
                var id = modUid++;
                AnonymousId[player.PlayerId] = id;
            }

            ModOption.isCanceled = false;
        }

        public static string GetGameId()
        {
            var timePart = StartTime.Ticks.ToString();

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(timePart));

            var hexHash = BitConverter.ToString(hashBytes, 0, 4)
                .Replace("-", "")
                .ToLowerInvariant();
            return $"{RoomCode}_{hexHash}";
        }

        public static void SaveAllPlayerDataToJson()
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
                },

                Players = AllPlayerData.Values.Select(p => new
                {
                    p.PlayerId,
                    p.PlayerName,
                    p.ColorName,
                    PlayerCode = p.FriendCode,
                    RoleInfo = new
                    {
                        OriginRole = p.OriginRole.ToString(),
                        MainRole = p.RoleId.ToString(),
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
                }).ToList()
            };

            var jsonContent = JsonSerializer.Serialize(gameSession, jsonOptions);
            File.WriteAllText(filePath, jsonContent);

            if (ModOption.uploadGameData)
            {
                if (WinCondition == WinCondition.Canceled)
                {
                    Info("Game was canceled, skipping data upload.", "PlayerData");
                    return;
                }

                var compactJsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Converters = { new JsonStringEnumConverter() }
                };
                var compactJsonContent = JsonSerializer.Serialize(gameSession, compactJsonOptions);
                UploadPlayerData(compactJsonContent).ContinueWith(task => { Info("Data uploaded successfully!", "PlayerData"); });
            }
        }

        private static string GetDynamicApiKey()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var salt = "toue-salt-v1";

            using var sha256 = SHA256.Create();
            var input = $"{GameId}.{datePart}.{salt}";
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            return BitConverter.ToString(hash).Replace("-", "").Substring(0, 32);
        }

        private static string GetSessionToken()
        {
            try
            {
                var response = httpClient.GetAsync(ApiUrl + "/api/auth/get-session-token").Result;
                var content = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(content);
                    return result.GetProperty("session_token").GetString();
                }
                else
                {
                    Error($"Failed to get session token: {content}", "PlayerData");
                    return "";
                }
            }
            catch (Exception ex)
            {
                Error($"Error getting session token: {ex.Message}", "PlayerData");
                return "";
            }
        }

        private static async Task UploadPlayerData(string jsonContent)
        {
            try
            {
                var apiKey = GetDynamicApiKey();
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var signature = GenerateSignature(jsonContent, timestamp, apiKey);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                content.Headers.Add("X-Session-Token", SessionToken);
                content.Headers.Add("X-Timestamp", timestamp);
                content.Headers.Add("X-Signature", signature);
                content.Headers.Add("X-Game-Id", GameId);

                var response = await httpClient.PostAsync(ApiUrl + "/api/games/v2", content);
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

        private static string GenerateSignature(string data, string timestamp, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var message = $"{timestamp}.{GameId}.{data}";
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToBase64String(hash);
        }
    }

    public class BindingVerifier
    {
        private static readonly HttpClient httpClient = new();

        public static async Task<(bool success, string message)> VerifyBinding(string playerCode, string verificationCode)
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

                var apiUrl = GlobalInfo.ApiUrl + "/api/auth/verify-playercode-binding";

                var response = await httpClient.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    bool success = result.GetProperty("success").GetBoolean();
                    string message = result.GetProperty("message").GetString();

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

public enum CustomDeathReason
{
    Null,

    HostKill,
    Exile,
    Kill,
    Disconnect,
    GuessFail,
    GuessSuccess,
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
    SheriffSuicide,
    SheriffMisfire,
    Suicide,
    BombVictim,
    Eaten,
    Jailed,
    AvengerFail,
}