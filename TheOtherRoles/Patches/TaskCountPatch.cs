// 参照: https://github.com/SuperNewRoles/SuperNewRoles/blob/master/SuperNewRoles/Patches/TaskCountPatch.cs

namespace TheOtherRoles.Patches;

internal class TaskCount
{
    public static bool WireTaskIsRandom => CustomOptionHolder.WireTaskIsRandomOption.GetBool();
    public static int WireTaskNum => CustomOptionHolder.WireTaskNumOption.GetInt();

    [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.Initialize))]
    private class NormalPlayerTaskInitializePatch
    {
        private static void Postfix(NormalPlayerTask __instance)
        {
            if (__instance.TaskType != TaskTypes.FixWiring || !WireTaskIsRandom) return;
            List<Console> orgList = MapUtilities.CachedShipStatus.AllConsoles.Where((Console t) => t.TaskTypes.Contains(__instance.TaskType)).ToList();
            List<Console> list = orgList.ToArray().ToList();

            __instance.MaxStep = WireTaskNum;
            __instance.Data = new byte[WireTaskNum];
            for (int i = 0; i < __instance.Data.Length; i++)
            {
                if (list.Count == 0)
                    list = orgList.ToArray().ToList();
                int index = list.GetRandomIndex();
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
}