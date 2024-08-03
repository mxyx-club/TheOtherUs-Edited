using System;
using TheOtherRoles.Utilities;

namespace TheOtherRoles;

[HarmonyPatch]
public static class TasksHandler
{
    public static Tuple<int, int> taskInfo(NetworkedPlayerInfo playerInfo)
    {
        var TotalTasks = 0;
        var CompletedTasks = 0;
        if (!playerInfo.Disconnected && playerInfo.Tasks != null &&
            playerInfo.Object &&
            playerInfo.Role && playerInfo.Role.TasksCountTowardProgress &&
            !playerInfo.Object.hasFakeTasks() && !playerInfo.Role.IsImpostor)
            foreach (var playerInfoTask in playerInfo.Tasks.GetFastEnumerator())
            {
                if (playerInfoTask.Complete) CompletedTasks++;
                TotalTasks++;
            }

        return Tuple.Create(CompletedTasks, TotalTasks);
    }


    [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
    private static class GameDataRecomputeTaskCountsPatch
    {
        private static bool ShouldCountTasks(NetworkedPlayerInfo playerInfo)
        {
            return !(playerInfo.Object && playerInfo.Object.hasAliveKillingLover())
                && playerInfo.PlayerId != Thief.thief?.PlayerId
                && playerInfo.PlayerId != Amnisiac.amnisiac?.PlayerId
                && playerInfo.PlayerId != Akujo.honmei?.PlayerId;
        }

        private static bool Prefix(GameData __instance)
        {
            var totalTasks = 0;
            var completedTasks = 0;
            //任务结算
            foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (!ShouldCountTasks(playerInfo))
                    continue;

                var (playerCompleted, playerTotal) = taskInfo(playerInfo);
                totalTasks += playerTotal;
                completedTasks += playerCompleted;
            }

            __instance.TotalTasks = totalTasks;
            __instance.CompletedTasks = completedTasks;
            return false;
        }
    }
}