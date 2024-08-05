using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using TheOtherRoles.Utilities;

// 参照: https://github.com/SuperNewRoles/SuperNewRoles/blob/master/SuperNewRoles/Patches/TaskCountPatch.cs

namespace TheOtherRoles.Patches;

class TaskCount
{
    public static bool WireTaskIsRandom => CustomOptionHolder.WireTaskIsRandomOption.getBool();
    public static int WireTaskNum => CustomOptionHolder.WireTaskNumOption.GetInt();

    [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.Initialize))]
    class NormalPlayerTaskInitializePatch
    {
        static void Postfix(NormalPlayerTask __instance)
        {
            if (__instance.TaskType != TaskTypes.FixWiring || !WireTaskIsRandom) return;
            List<Console> orgList = MapUtilities.CachedShipStatus.AllConsoles.Where((Console t) => t.TaskTypes.Contains(__instance.TaskType)).ToList();
            List<Console> list = new(orgList);

            __instance.MaxStep = WireTaskNum;
            __instance.Data = new byte[WireTaskNum];
            for (int i = 0; i < __instance.Data.Length; i++)
            {
                if (list.Count == 0)
                    list = new List<Console>(orgList);
                int index = GetRandomIndex(list);
                __instance.Data[i] = (byte)list[index].ConsoleId;
                list.RemoveAt(index);
            }
            __instance.StartAt = orgList.First(console => console.ConsoleId == __instance.Data[0]).Room;
        }
    }
    [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.FixedUpdate))]
    public static class NormalPlayerTaskPatch
    {
        public static void Postfix(NormalPlayerTask __instance)
        {
            if (__instance.IsComplete && __instance.Arrow?.isActiveAndEnabled == true)
                __instance.Arrow?.gameObject?.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(AirshipUploadTask), nameof(AirshipUploadTask.FixedUpdate))]
    public static class AirshipUploadTaskPatch
    {
        public static void Postfix(AirshipUploadTask __instance)
        {
            if (__instance.IsComplete)
                __instance.Arrows?.DoIf(x => x != null && x.isActiveAndEnabled, x => x.gameObject?.SetActive(false));
        }
    }
    public static Tuple<int, int> TaskDateNoClearCheck(NetworkedPlayerInfo playerInfo)
    {
        int TotalTasks = 0;
        int CompletedTasks = 0;

        for (int j = 0; j < playerInfo.Tasks.Count; j++)
        {
            TotalTasks++;
            if (playerInfo.Tasks[j].Complete)
            {
                CompletedTasks++;
            }
        }
        return Tuple.Create(CompletedTasks, TotalTasks);
    }
    public static Tuple<int, int> TaskDate(NetworkedPlayerInfo playerInfo)
    {
        int TotalTasks = 0;
        int CompletedTasks = 0;
        if (!playerInfo.Disconnected && playerInfo.Tasks != null &&
            playerInfo.Object &&
            (GameManager.Instance.LogicOptions.currentGameOptions.GetBool(BoolOptionNames.GhostsDoTasks) || !playerInfo.IsDead) &&
            playerInfo.Role && playerInfo.Role.TasksCountTowardProgress
            )
        {
            for (int j = 0; j < playerInfo.Tasks.Count; j++)
            {
                TotalTasks++;
                if (playerInfo.Tasks[j].Complete)
                {
                    CompletedTasks++;
                }
            }
        }
        return Tuple.Create(CompletedTasks, TotalTasks);
    }
}