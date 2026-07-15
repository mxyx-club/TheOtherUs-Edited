#nullable enable
using System.Text.Json.Serialization;

namespace TheOtherRoles.Modules;

/// <summary>
/// 游戏事件记录
/// </summary>
public record GameEvent
{
    public string EventType { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public float GameTime { get; set; }
    public byte? SourcePlayerId { get; set; }
    public byte? TargetPlayerId { get; set; }
    public object?[]? Extra { get; set; }

    [JsonIgnore]
    public PlayerControl? SourcePlayer => SourcePlayerId.HasValue ? PlayerById(SourcePlayerId.Value) : null;

    [JsonIgnore]
    public PlayerControl? TargetPlayer => TargetPlayerId.HasValue ? PlayerById(TargetPlayerId.Value) : null;
}

public partial class GameDataManager
{
    public List<GameEvent> EventLog { get; private set; } = new();

    /// <summary>
    /// 记录游戏事件
    /// </summary>
    public static void RecordEvent(
        string eventType,
        byte? sourcePlayerId = null,
        byte? targetPlayerId = null,
        params object?[]? extra)
    {
        if (!Instance._isInitialized) return;

        var extraArray = extra?.Length > 0 ? extra : null;
        var now = Time.time;

        if (Instance.EventLog.Count > 0)
        {
            var last = Instance.EventLog[^1];
            if (last.EventType == eventType
                && last.SourcePlayerId == sourcePlayerId
                && last.TargetPlayerId == targetPlayerId
                && ExtrasEqual(last.Extra, extraArray)
                && now - last.GameTime < 2f)
            {
                return;
            }
        }

        Instance.EventLog.Add(new GameEvent
        {
            EventType = eventType,
            Timestamp = DateTime.UtcNow,
            GameTime = now,
            SourcePlayerId = sourcePlayerId,
            TargetPlayerId = targetPlayerId,
            Extra = extraArray,
        });
    }

    /// <summary>
    /// 通过 RPC 记录游戏事件
    /// </summary>
    public static void RpcRecordEvent(
        string eventType,
        byte? sourcePlayerId = null,
        byte? targetPlayerId = null,
        params object?[]? extra)
    {
        if (!Instance._isInitialized) return;

        var writer = StartRPC(CustomRPC.RecordEvent);
        writer.Write(eventType);
        writer.Write(sourcePlayerId ?? byte.MaxValue);
        writer.Write(targetPlayerId ?? byte.MaxValue);
        writer.WriteExtra(extra);
        writer.EndRPC();

        RecordEvent(eventType, sourcePlayerId, targetPlayerId, extra);
    }

    public static void HandleRpcRecordEvent(MessageReader reader)
    {
        var eventType = reader.ReadString();
        var sourceId = reader.ReadByte();
        var targetId = reader.ReadByte();
        var extra = reader.ReadExtra();

        RecordEvent(
            eventType,
            sourceId == byte.MaxValue ? null : sourceId,
            targetId == byte.MaxValue ? null : targetId,
            extra);
    }

    private static bool ExtrasEqual(object?[]? a, object?[]? b)
    {
        if (a == b) return true;
        if (a == null || b == null) return false;
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (!Equals(a[i], b[i])) return false;
        }
        return true;
    }

    public List<GameEvent> GetEvents(string eventType) => EventLog.Where(e => e.EventType == eventType).ToList();

    public List<GameEvent> GetPlayerEvents(byte playerId) => EventLog.Where(e => e.SourcePlayerId == playerId || e.TargetPlayerId == playerId).ToList();

    public void ClearEvents()
    {
        EventLog.Clear();
    }
}
