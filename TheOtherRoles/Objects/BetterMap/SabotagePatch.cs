using System;
using TheOtherRoles.Utilities;

// 参考 => https://github.com/Koke1024/Town-Of-Moss/blob/main/TownOfMoss/Patches/MeltDownBoost.cs
// 来源 => https://github.com/SuperNewRoles/SuperNewRoles/blob/master/SuperNewRoles/MapOption/MapOption.cs

namespace TheOtherRoles.Objects.BetterMap;

public static class ElectricPatch
{
    public static bool IsReactorDurationSetting;
    public static void Reset()
    {
        onTask = false;
        IsReactorDurationSetting = CustomOptionHolder.IsReactorDurationSetting.getBool();
    }
    public static bool onTask = false;
    public static bool done = false;
    public static DateTime lastUpdate;

    [HarmonyPatch(typeof(SwitchMinigame), nameof(SwitchMinigame.Begin))]
    class VitalsMinigameStartPatch
    {
        static void Postfix(VitalsMinigame __instance)
        {
            onTask = true;
            done = false;
        }
    }
    [HarmonyPatch(typeof(SwitchMinigame), nameof(SwitchMinigame.FixedUpdate))]
    class SwitchMinigameClosePatch
    {
        static void Postfix(SwitchMinigame __instance)
        {
            lastUpdate = DateTime.UtcNow;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) =>
            {
                if (p == 1f)
                {
                    float diff = (float)(DateTime.UtcNow - lastUpdate).TotalMilliseconds;
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
                case ShipStatus.MapType.Ship when __instance.Countdown >= CustomOptionHolder.SkeldLifeSuppTimeLimit.getFloat():
                    __instance.Countdown = CustomOptionHolder.SkeldLifeSuppTimeLimit.getFloat();
                    return;
                case ShipStatus.MapType.Hq when __instance.Countdown >= CustomOptionHolder.MiraLifeSuppTimeLimit.getFloat():
                    __instance.Countdown = CustomOptionHolder.MiraLifeSuppTimeLimit.getFloat();
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
            if (!__instance.IsActive)
                return;
            switch (MapUtilities.CachedShipStatus.Type)
            {
                case ShipStatus.MapType.Ship when __instance.Countdown >= CustomOptionHolder.SkeldReactorTimeLimit.getFloat():
                    __instance.Countdown = CustomOptionHolder.SkeldReactorTimeLimit.getFloat();
                    return;
                case ShipStatus.MapType.Hq when __instance.Countdown >= CustomOptionHolder.MiraReactorTimeLimit.getFloat():
                    __instance.Countdown = CustomOptionHolder.MiraReactorTimeLimit.getFloat();
                    return;
                case ShipStatus.MapType.Pb when __instance.Countdown >= CustomOptionHolder.PolusReactorTimeLimit.getFloat():
                    __instance.Countdown = CustomOptionHolder.PolusReactorTimeLimit.getFloat();
                    return;
                case ShipStatus.MapType.Fungle when __instance.Countdown >= CustomOptionHolder.FungleReactorTimeLimit.getFloat():
                    __instance.Countdown = CustomOptionHolder.FungleReactorTimeLimit.getFloat();
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
        if (ElectricPatch.IsReactorDurationSetting)
        {
            if (!__instance.IsActive)
                return;

            if (MapUtilities.CachedShipStatus != null)
            {
                if (__instance.Countdown >= CustomOptionHolder.AirshipReactorTimeLimit.getFloat())
                    __instance.Countdown = CustomOptionHolder.AirshipReactorTimeLimit.getFloat();
            }
        }
    }
}