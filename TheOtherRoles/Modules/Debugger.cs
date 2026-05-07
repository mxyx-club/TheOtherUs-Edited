using static TheOtherRoles.Options.ModOption;

namespace TheOtherRoles.Modules;

[HarmonyPatch]
[HarmonyPriority(Priority.First)]
internal class Debugger
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    private static class CountdownPatch
    {
        public static void Prefix(GameStartManager __instance)
        {
            if (DebugMode) __instance.countDownTimer = 0;
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    private static class CheckEndCriteriaPatch
    {
        public static bool Prefix()
        {
            return !DisableGameEnd;
        }
    }

    [HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowDefaultNavigation))]
    private static class AutoPlayAgainPatch
    {
        public static void Postfix(EndGameNavigation __instance)
        {
            if (!DebugMode) return;
            if (AmongUsClient.Instance.AmHost) return;
            __instance.NextGame();
        }
    }

    [HarmonyPatch(typeof(FriendsListManager), nameof(FriendsListManager.CheckFriendCodeOnLogin))]
    private static class FriendsListManager_CheckFriendCodeOnLogin
    {
        public static void Postfix()
        {
            updateFriendCode();
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.OpenGameModeMenu))]
    private static class MainMenuManager_OpenGameModeMenuPatch
    {
        public static void Postfix()
        {
            updateFriendCode();
        }
    }
}