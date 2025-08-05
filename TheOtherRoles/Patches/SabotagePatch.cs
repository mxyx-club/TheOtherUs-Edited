using TheOtherRoles.Attributes;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
public static class SabotagePatch
{
    public static bool IsReactorDurationSetting;
    [OnGameStart]
    public static void Reset()
    {
        IsReactorDurationSetting = CustomOptionHolder.IsReactorDurationSetting.GetBool();
    }

    private static void ModifySkeld()
    {
        ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>().LifeSuppDuration = CustomOptionHolder.SkeldLifeSuppTimeLimit.GetFloat();
        ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>().ReactorDuration = CustomOptionHolder.SkeldReactorTimeLimit.GetFloat();
    }

    private static void ModifyMira()
    {
        ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>().LifeSuppDuration = CustomOptionHolder.MiraLifeSuppTimeLimit.GetFloat();
        ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>().ReactorDuration = CustomOptionHolder.MiraReactorTimeLimit.GetFloat();
    }
    private static void ModifyPolus()
    {
        ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>().ReactorDuration = CustomOptionHolder.PolusReactorTimeLimit.GetFloat();
    }

    private static void ModifyFungle()
    {
        ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>().ReactorDuration = CustomOptionHolder.FungleReactorTimeLimit.GetFloat();
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate)), HarmonyPrefix]
    public static void ShipStatusAwake(ShipStatus __instance)
    {
        if (__instance.Type == (ShipStatus.MapType)6) return;

        if (!IsReactorDurationSetting) return;
        switch (GameOptionsManager.Instance.CurrentGameOptions.MapId)
        {
            case 0: ModifySkeld(); break;
            case 1: ModifyMira(); break;
            case 2: ModifyPolus(); break;
            case 5: ModifyFungle(); break;
            default: break;
        }
    }

    [HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.UpdateSystem))]
    private class HeliSabotageSystemPatch
    {
        private static void Postfix(HeliSabotageSystem __instance, [HarmonyArgument(1)] MessageReader msgReader)
        {
            if (!IsReactorDurationSetting) return;
            if ((PeekByte(msgReader, -1) & 240) == (int)HeliSabotageSystem.Tags.DamageBit)
            {
                __instance.Countdown = CustomOptionHolder.AirshipReactorTimeLimit.GetFloat();
            }
        }
        public static byte PeekByte(MessageReader reader, int offset = 0)
        {
            return reader.Buffer[reader.readHead + offset];
        }
    }

    [HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.Deteriorate))]
    public static class HeliMeltdownBooster
    {
        public static void Prefix(HeliSabotageSystem __instance)
        {
            if (IsReactorDurationSetting)
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
}