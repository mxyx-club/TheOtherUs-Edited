using TheOtherRoles.Attributes;

namespace TheOtherRoles.Modules;

public class PlayerData
{
    public static PlayerData LocalData { get => field ??= GetPlayerData(LocalPlayer); private set; }
    public static PlayerControl LocalPlayer { get; private set; }

    public static Dictionary<byte, PlayerData> AllPlayerData { get; private set; } = new();

    public PlayerControl Player { get; private set; }
    public byte PlayerId { get; private set; }
    public bool IsDead => Player.IsDead();
    public int KillCount => GetKillCount(Player);
    public bool IsWinner;
    public bool IsDisconnected;

    public RoleInfo RoleInfo;
    public RoleType RoleType = RoleType.Error;
    public List<RoleId> RoleHistory = new();
    public RoleId MainRole = RoleId.DefaultRole;
    public RoleId OriginRole = RoleId.DefaultRole;
    public List<RoleId> Modifiers = new();
    public RoleId? GhostRole;
    public Tuple<int, int> TaskCount;

    public string PlayerName { get; }
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

    public PlayerData(PlayerControl player)
    {
        Player = player;
        PlayerId = player.PlayerId;
        PlayerName = player.Data.PlayerName;
        FriendCode = player.Data.FriendCode;
        ColorName = player.Data.ColorName;
        AllPlayerData[player.PlayerId] = this;
    }

    public static PlayerData GetPlayerData(PlayerControl player)
    {
        if (player?.Data == null) return null;
        if (AllPlayerData.TryGetValue(player.PlayerId, out var data))
        {
            return data;
        }
        else
        {
            data = new PlayerData(player);
            return data;
        }
    }

    public static PlayerData GetPlayerData(string name)
    {
        if (name.IsNullOrWhiteSpace()) return null;
        return AllPlayerData.Values.FirstOrDefault(data => data.PlayerName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    [OnGameStart(Attributes.Priority.High)]
    public static void Init()
    {
        AllPlayerData.Clear();

        var code = EOSManager.Instance?.FriendCode ?? "";

        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            var data = new PlayerData(player);
        }

        LocalData = AllPlayerData[PlayerControl.LocalPlayer.PlayerId];
        LocalPlayer = PlayerControl.LocalPlayer;

        _ = new LateTask(() =>
        {
            if (ModOption.uploadGameData)
            {
                var writer = StartRPC(CustomRPC.ShareFriendCode);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(code);
                writer.EndRPC();
                GameDataManager.Instance.ShareFriendCode(PlayerControl.LocalPlayer.PlayerId, code);
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

    public static void ClearDeathReason(PlayerControl player)
    {
        if (!AllPlayerData.TryGetValue(player.PlayerId, out var data)) return;
        data.DeathReason = CustomDeathReason.Null;
        data.DeathTimer = DateTime.MinValue;
        data.KilledBy = null;
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