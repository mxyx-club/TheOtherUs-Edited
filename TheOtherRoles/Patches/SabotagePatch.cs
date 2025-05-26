// 参考 => https://github.com/Koke1024/Town-Of-Moss/blob/main/TownOfMoss/Patches/MeltDownBoost.cs
// 来源 => https://github.com/SuperNewRoles/SuperNewRoles/blob/master/SuperNewRoles/MapOption/MapOption.cs

namespace TheOtherRoles.Patches;

public static class ElectricPatch
{
    public static bool IsReactorDurationSetting;
    public static void Reset()
    {
        onTask = false;
        IsReactorDurationSetting = CustomOptionHolder.IsReactorDurationSetting.GetBool();
    }
    public static bool onTask;
    public static bool done;
    public static DateTime lastUpdate;

    [HarmonyPatch(typeof(SwitchMinigame), nameof(SwitchMinigame.Begin))]
    private class VitalsMinigameStartPatch
    {
        private static void Postfix(VitalsMinigame __instance)
        {
            onTask = true;
            done = false;
        }
    }
    [HarmonyPatch(typeof(SwitchMinigame), nameof(SwitchMinigame.FixedUpdate))]
    private class SwitchMinigameClosePatch
    {
        private static void Postfix(SwitchMinigame __instance)
        {
            lastUpdate = DateTime.UtcNow;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) =>
            {
                if (p == 1f)
                {
                    var diff = (float)(DateTime.UtcNow - lastUpdate).TotalMilliseconds;
                    if (diff > 100 && !done)
                    {
                        done = true;
                        onTask = false;
                    }
                }
            })));
        }
    }
}
[HarmonyPatch(typeof(LifeSuppSystemType), nameof(LifeSuppSystemType.Deteriorate))]
public static class LifeSuppBooster
{
    public static void Prefix(LifeSuppSystemType __instance, float deltaTime)
    {
        if (ElectricPatch.IsReactorDurationSetting)
        {
            if (!__instance.IsActive)
                return;
            switch (MapUtilities.CachedShipStatus.Type)
            {
                case ShipStatus.MapType.Ship when __instance.Countdown >= CustomOptionHolder.SkeldLifeSuppTimeLimit.GetFloat():
                    __instance.Countdown = CustomOptionHolder.SkeldLifeSuppTimeLimit.GetFloat();
                    return;
                case ShipStatus.MapType.Hq when __instance.Countdown >= CustomOptionHolder.MiraLifeSuppTimeLimit.GetFloat():
                    __instance.Countdown = CustomOptionHolder.MiraLifeSuppTimeLimit.GetFloat();
                    return;
                default:
                    return;
            }
        }
    }
}


[HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.Deteriorate))]
public static class MeltdownBooster
{
    public static void Prefix(ReactorSystemType __instance, float deltaTime)
    {
        if (ElectricPatch.IsReactorDurationSetting)
        {
            if (!__instance.IsActive) return;
            switch (MapUtilities.CachedShipStatus.Type)
            {
                case ShipStatus.MapType.Ship when __instance.Countdown >= CustomOptionHolder.SkeldReactorTimeLimit.GetFloat():
                    __instance.Countdown = CustomOptionHolder.SkeldReactorTimeLimit.GetFloat();
                    return;
                case ShipStatus.MapType.Hq when __instance.Countdown >= CustomOptionHolder.MiraReactorTimeLimit.GetFloat():
                    __instance.Countdown = CustomOptionHolder.MiraReactorTimeLimit.GetFloat();
                    return;
                case ShipStatus.MapType.Pb when __instance.Countdown >= CustomOptionHolder.PolusReactorTimeLimit.GetFloat():
                    __instance.Countdown = CustomOptionHolder.PolusReactorTimeLimit.GetFloat();
                    return;
                case ShipStatus.MapType.Fungle when __instance.Countdown >= CustomOptionHolder.FungleReactorTimeLimit.GetFloat():
                    __instance.Countdown = CustomOptionHolder.FungleReactorTimeLimit.GetFloat();
                    return;
                default:
                    return;
            }
        }
    }
}


[HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.Deteriorate))]
public static class HeliMeltdownBooster
{
    public static void Prefix(HeliSabotageSystem __instance)
    {
        if (CustomOptionHolder.IsReactorDurationSetting.GetBool())
        {
            if (!__instance.IsActive)
                return;

            if (MapUtilities.CachedShipStatus != null)
            {
                if (__instance.Countdown >= CustomOptionHolder.AirshipReactorTimeLimit.GetFloat())
                    __instance.Countdown = CustomOptionHolder.AirshipReactorTimeLimit.GetFloat();
            }
        }
    }
}