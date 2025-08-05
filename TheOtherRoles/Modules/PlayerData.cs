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
    public static Dictionary<byte, ushort> ModId = new();

    public PlayerControl Player { get; private set; }
    public byte PlayerId { get; private set; }

    public bool IsWinner;
    public bool IsDisconnected;
    public bool IsDead => Player.IsDead();
    public int KillCount;
    public Tuple<int, int> TaskCount;

    public RoleInfo RoleInfo => RoleInfo.RoleInfoById.GetValueOrDefault(RoleId, RoleInfo.crewmate);
    public RoleType RoleType = RoleType.Crewmate;
    public List<RoleId> RoleHistory = new();
    public RoleId RoleId = RoleId.DefaultRole;
    public RoleId? GhostRole;
    public List<RoleId> Modifiers = new();

    public string PlayerName { get; private set; }
    public string FriendCode { get; private set; }
    public string ColorName { get; private set; }

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

    public static string GetPlayerCode(PlayerControl player)
    {
        return AllFriendCode.TryGetValue(player.PlayerId, out var code) ? code : "";
    }

    [OnGameStart(Attributes.Priority.VeryHigh)]
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
                ColorName = player.Data.GetPlayerColorString(),
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
            GameData.Instance?.GetPlayerById(playerId)?.FriendCode = code;
        }
        catch (Exception e) { Message($"Error reading friend code: {e.Message}", "ShareFriendCode"); }
    }

    public class GlobalInfo
    {
        private const string Web = "https://api.toue.mxyx.club/api/games";
        private const string ApiUrl = Web;
        private static readonly HttpClient httpClient = new();

        public static string GameId { get; private set; }

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
            RoomCode = GameStartManagerPatch.RoomCode;
            HostCode = GetHostPlayer.Data.FriendCode;
            GameId = GetGameId();

            int seed = BitConverter.ToInt32(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(GameId)), 0);
            var rnd = new SRandom(seed);

            byte modUid = 1;
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().OrderBy(_ => rnd.Next()))
            {
                var id = modUid++;
                ModId[player.PlayerId] = id;
                Message($"Set {player.PlayerId} Is {id}", "SetUid");
            }
        }

        public static string GetGameId()
        {
            var timePart = StartTime.ToString("HH:mm");
            var rawData = $"{HostCode}:{timePart}";

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            var hexHash = BitConverter.ToString(hashBytes, 0, 4)
                .Replace("-", "")
                .ToLowerInvariant();
            return $"{RoomCode}_{hexHash}";
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
                        OriginRole = p.RoleId.ToString(),
                        MainRole = p.RoleId.ToString(),
                        //Modifiers = string.Join("|", p.Modifiers),
                        Modifiers = p.Modifiers.Select(x => x.ToString()),
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
                        KilledBy = p.KilledBy?.Data?.PlayerName ?? "null",
                        DeathTimer = p.DeathTimer.ToString("yyyy-MM-ddTHH:mm:ss"),
                    },
                }).ToList()
            };

            var jsonContent = JsonSerializer.Serialize(gameSession, jsonOptions);
            File.WriteAllText(filePath, jsonContent);

            UploadPlayerDataToApi(jsonContent).ContinueWith(task =>
            {
                Info("Data uploaded successfully!", "PlayerData");
            });
        }

        private static async Task UploadPlayerDataToApi(string jsonContent)
        {
            try
            {
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(ApiUrl, content);

                Message($"Status Code: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Message($"Response: {responseContent}");

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
}