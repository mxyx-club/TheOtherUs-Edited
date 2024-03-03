using System;
using TheOtherRoles.Helper;
using TheOtherRoles.Utilities;

namespace TheOtherRoles;

[HarmonyPatch]
public static class TasksHandler
{
    public static Tuple<int, int> taskInfo(GameData.PlayerInfo playerInfo)
    {
        var TotalTasks = 0;
        var CompletedTasks = 0;
        if (!playerInfo.Disconnected && playerInfo.Tasks != null &&
            playerInfo.Object &&
            playerInfo.Role && playerInfo.Role.TasksCountTowardProgress &&
            !playerInfo.Object.hasFakeTasks() && !playerInfo.Role.IsImpostor
           )
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
        private static bool Prefix(GameData __instance)
        {
            var totalTasks = 0;
            var completedTasks = 0;
            //任务结算
            foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if ((playerInfo.Object && playerInfo.Object.hasAliveKillingLover()) 
                    // Tasks do not count if a Crewmate has an alive killing Lover
                    || playerInfo.PlayerId == Lawyer.lawyer?.PlayerId 
                    // Tasks of the Lawyer do not count
                    || (playerInfo.PlayerId == Pursuer.pursuer?.PlayerId) 
                    // Tasks of the Pursuer only count, if he's alive
                    || playerInfo.PlayerId == Swooper.swooper?.PlayerId 
                    // Tasks of the Swooper do not count
                    || playerInfo.PlayerId == Thief.thief?.PlayerId 
                    // Thief's tasks only count after joining crew team as sheriff (and then the thief is not the thief anymore)
                    || playerInfo.PlayerId == Amnisiac.amnisiac?.PlayerId 
                    // Thief's tasks only count after joining crew team as sheriff (and then the thief is not the thief anymore)
                   )
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