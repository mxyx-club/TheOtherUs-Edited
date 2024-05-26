using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TheOtherRoles.Utilities;
public class VoteData
{
    private static Dictionary<byte, PlayerControl> cachedPlayers = new(15);
    public static PlayerControl GetPlayerById(int playerId) => GetPlayerById((byte)playerId);
    public static PlayerControl GetPlayerById(byte playerId)
    {
        if (cachedPlayers.TryGetValue(playerId, out var cachedPlayer) && cachedPlayer != null)
        {
            return cachedPlayer;
        }
        var player = PlayerControl.AllPlayerControls.ToArray().Where(p => p != null).Where(pc => pc.PlayerId == playerId).FirstOrDefault();
        cachedPlayers[playerId] = player;
        return player;
    }
    public byte Voter { get; private set; } = byte.MaxValue;
    public byte VotedFor { get; private set; } = NoVote;
    public int NumVotes { get; private set; } = 1;
    public const byte Skip = 253;
    public const byte NoVote = 254;
    public VoteData(byte voter) => Voter = voter;

    public void DoVote(byte voteTo, int numVotes)
    {
        Info($"投票：{GetPlayerById(Voter).GetNameWithRole()} => {GetVoteName(voteTo)} x {numVotes}");
    }

    public static string GetVoteName(byte num)
    {
        string name = "invalid";
        var player = GetPlayerById(num);
        if (num < 15 && player != null) name = player?.GetNameWithRole();
        else if (num == Skip) name = "Skip";
        else if (num == NoVote) name = "None";
        else if (num == 255) name = "Dead";
        return name;
    }

}
public static class Data
{
    public static string RemoveHtmlTags(this string str) => Regex.Replace(str, "<[^>]*?>", string.Empty);
    public static string GetAllRoleName(this PlayerControl player)
    {
        if (!player) return null;
        var text = RoleInfo.GetRolesString(player, false, false);
        return text;
    }

    public static string GetNameWithRole(this PlayerControl player, bool forUser = false)
    {
        var ret = $"{player?.Data?.PlayerName}" + $"({player?.GetAllRoleName()})";
        return forUser ? ret : ret.RemoveHtmlTags();
    }
}

