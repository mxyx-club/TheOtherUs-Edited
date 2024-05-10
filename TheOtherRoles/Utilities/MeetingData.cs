using System.Collections.Generic;
using System.Linq;

namespace TheOtherRoles.Utilities;

[Harmony]
#nullable enable
public static class MeetingData
{
    public static readonly List<MeetingVote> meetingVotes = [];
    public static int CurrentMeetingHudId { get; private set; }

    public static List<MeetingVote> PlayerVotes(this PlayerControl player, bool isSrc)
    {
        return meetingVotes.Where(n =>
            (n.SrcPlayerId == player.PlayerId && isSrc) || (n.SuspectPlayerId == player.PlayerId && !isSrc)).ToList();
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
    [HarmonyPostfix]
    private static void MeetingHud_CastVote(byte srcPlayerId, byte suspectPlayerId)
    {
        meetingVotes.Add(new MeetingVote
        {
            MeetingId = CurrentMeetingHudId,
            SuspectPlayerId = suspectPlayerId,
            SrcPlayerId = srcPlayerId
        });
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ServerStart))]
    [HarmonyPostfix]
    private static void MeetingHud_ServerStart()
    {
        CurrentMeetingHudId++;
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    private static void OnGameEnd()
    {
        meetingVotes.Clear();
    }

    public static MeetingVote Get(this PlayerControl player, int MeetingIndex = 0)
    {
        return meetingVotes.FirstOrDefault(n =>
            n.MeetingId == CurrentMeetingHudId - MeetingIndex && n.SrcPlayerId == player.PlayerId)!;
    }
}

public class MeetingVote
{
    public byte SrcPlayerId { get; set; }

    public byte SuspectPlayerId { get; set; }

    public int MeetingId { get; set; }
}