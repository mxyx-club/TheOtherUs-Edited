using static TheOtherRoles.Options.ModOption;

namespace TheOtherRoles.Modules;

[HarmonyPatch]
[HarmonyPriority(Priority.First)]
public class Debugger
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class CountdownPatch
    {
        public static void Prefix(GameStartManager __instance)
        {
            if (DebugMode) __instance.countDownTimer = 0;
        }
    }


    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria)), HarmonyPrefix]
    public static bool Prefix()
    {
        return !DisableGameEnd;
    }

    [HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowDefaultNavigation))]
    internal static class AutoPlayAgainPatch
    {
        public static void Postfix(EndGameNavigation __instance)
        {
            if (!DebugMode) return;
            if (AmongUsClient.Instance.AmHost) return;
            __instance.NextGame();
        }
    }
}
