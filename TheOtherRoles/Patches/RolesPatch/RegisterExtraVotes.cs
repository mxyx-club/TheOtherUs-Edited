using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Utilities;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches.RolesPatch;

[HarmonyPatch(typeof(MeetingHud))]
public class RegisterExtraVotes
{
    public static Dictionary<byte, int> CalculateAllVotes(MeetingHud __instance)
    {
        Message("开始计票");
        var dictionary = new Dictionary<byte, int>();
        for (var i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            if (Prosecutor.prosecutor != CachedPlayer.LocalPlayer.PlayerControl) continue;
            if (Prosecutor.prosecutor.Data.IsDead || Prosecutor.prosecutor.Data.Disconnected) continue;
            if (!playerVoteArea.DidVote
                || playerVoteArea.AmDead
                || playerVoteArea.VotedFor == PlayerVoteArea.MissedVote
                || playerVoteArea.VotedFor == PlayerVoteArea.DeadVote)
            {
                Message("投票被无效");
                Prosecutor.ProsecuteThisMeeting = false;
                continue;
            }
            else if (Prosecutor.ProsecuteThisMeeting)
            {
                Message("票数增加");
                if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num2))
                    dictionary[playerVoteArea.VotedFor] = num2 + 10;
                else
                    dictionary[playerVoteArea.VotedFor] = 10;
                return dictionary;
            }
        }

        for (var i = 0; i < __instance.playerStates.Length; i++)
        {
            if (Prosecutor.prosecutor != CachedPlayer.LocalPlayer.PlayerControl) continue;
            if (Prosecutor.prosecutor.Data.IsDead || Prosecutor.prosecutor.Data.Disconnected) continue;
            var playerVoteArea = __instance.playerStates[i];
            if (!playerVoteArea.DidVote
                || playerVoteArea.AmDead
                || playerVoteArea.VotedFor == PlayerVoteArea.MissedVote
                || playerVoteArea.VotedFor == PlayerVoteArea.DeadVote) continue;

            if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num))
                dictionary[playerVoteArea.VotedFor] = num + 1;
            else
                dictionary[playerVoteArea.VotedFor] = 1;
        }

        dictionary.MaxPair(out var tie);

        if (tie)
            foreach (var player in __instance.playerStates)
            {
                if (!player.DidVote
                    || player.AmDead
                    || player.VotedFor == PlayerVoteArea.MissedVote
                    || player.VotedFor == PlayerVoteArea.DeadVote) continue;
            }

        return dictionary;
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance,
            [HarmonyArgument(0)] Il2CppStructArray<MeetingHud.VoterState> states,
            [HarmonyArgument(1)] GameData.PlayerInfo exiled,
            [HarmonyArgument(2)] bool tie)
        {
            __instance.exiledPlayer = __instance.wasTie ? null : __instance.exiledPlayer;
            var exiledString = exiled == null ? "null" : exiled.PlayerName;
            Message($"被驱逐玩家 = {exiledString}");
            Message($"是否平票 = {tie}");
        }
    }


    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
    public static class CheckForEndVoting
    {
        private static bool CheckVoted(PlayerVoteArea playerVoteArea)
        {
            if (playerVoteArea.AmDead || playerVoteArea.DidVote)
                return true;

            var playerInfo = GameData.Instance.GetPlayerById(playerVoteArea.TargetPlayerId);
            if (playerInfo == null)
                return true;

            return true;
        }
        public static bool Prefix(MeetingHud __instance)
        {
            if (__instance.playerStates.All(ps => ps.AmDead || ps.DidVote && CheckVoted(ps)))
            {

                var self = CalculateAllVotes(__instance);
                var array = new Il2CppStructArray<MeetingHud.VoterState>(__instance.playerStates.Length);
                var maxIdx = self.MaxPair(out var tie);
                var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == maxIdx.Key);
                for (var i = 0; i < __instance.playerStates.Length; i++)
                {
                    var playerVoteArea = __instance.playerStates[i];
                    array[i] = new MeetingHud.VoterState
                    {
                        VoterId = playerVoteArea.TargetPlayerId,
                        VotedForId = playerVoteArea.VotedFor
                    };
                    __instance.RpcVotingComplete(array, exiled, tie);
                }
            }
            return false;
        }
    }

    // 增加投票动画
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
    public static class PopulateResults
    {
        public static bool Prefix(MeetingHud __instance,
            [HarmonyArgument(0)] Il2CppStructArray<MeetingHud.VoterState> states)
        {
            var allNums = new Dictionary<int, int>();

            __instance.TitleText.text = Object.FindObjectOfType<TranslationController>()
                .GetString(StringNames.MeetingVotingResults, Array.Empty<Il2CppSystem.Object>());
            var amountOfSkippedVoters = 0;

            var isProsecuting = false;
            if (Prosecutor.prosecutor == CachedPlayer.LocalPlayer.PlayerControl)
            {
                if (Prosecutor.prosecutor.Data.IsDead || Prosecutor.prosecutor.Data.Disconnected) return false;
                if (Prosecutor.ProsecuteThisMeeting)
                {
                    isProsecuting = true;
                }
            }

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                playerVoteArea.ClearForResults();
                allNums.Add(i, 0);

                for (var stateIdx = 0; stateIdx < states.Length; stateIdx++)
                {
                    var voteState = states[stateIdx];
                    var playerInfo = GameData.Instance.GetPlayerById(voteState.VoterId);
                    if (Prosecutor.prosecutor != CachedPlayer.LocalPlayer.PlayerControl)
                    {
                        if (Prosecutor.prosecutor.Data.IsDead || Prosecutor.prosecutor.Data.Disconnected) continue;
                        if (Prosecutor.ProsecuteThisMeeting)
                        {
                            if (voteState.VoterId == Prosecutor.prosecutor.PlayerId)
                            {
                                if (playerInfo == null)
                                {
                                    Error(string.Format("找不到投票者的玩家信息: {0}", voteState.VoterId));
                                    Prosecutor.Prosecuted = true;
                                }
                                else if (i == 0 && voteState.SkippedVote)
                                {
                                    __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                    __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                    __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                    __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                    __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                    amountOfSkippedVoters += 5;
                                    Prosecutor.Prosecuted = true;
                                }
                                else if (voteState.VotedForId == playerVoteArea.TargetPlayerId)
                                {
                                    __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                    __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                    __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                    __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                    __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                    allNums[i] += 5;
                                    Prosecutor.Prosecuted = true;
                                }
                            }
                        }
                    }

                    if (isProsecuting) continue;

                    if (playerInfo == null)
                    {
                        Error(string.Format("找不到投票者的玩家信息: {0}",
                            voteState.VoterId));
                    }
                    else if (i == 0 && voteState.SkippedVote)
                    {
                        __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                        amountOfSkippedVoters++;
                    }
                    else if (voteState.VotedForId == playerVoteArea.TargetPlayerId)
                    {
                        __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                        allNums[i]++;
                    }
                }
            }
            return false;
        }
    }
}