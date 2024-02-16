using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(MedScanMinigame._WalkToOffset_d__15), nameof(MedScanMinigame._WalkToPad_d__16.MoveNext))]
internal class MedscanMiniGamePatchWTP
{
    private static bool Prefix(MedScanMinigame._WalkToPad_d__16 __instance)
    {
        if (TORMapOptions.disableMedscanWalking)
        {
            var num = __instance.__1__state;
            var medScanMinigame = __instance.__4__this;
            switch (num)
            {
                case 0:
                    __instance.__1__state = -1;
                    medScanMinigame.state = MedScanMinigame.PositionState.WalkingToPad;
                    __instance.__1__state = 1;
                    return true;
                case 1:
                    __instance.__1__state = -1;
                    __instance.__2__current = new WaitForSeconds(0.1f);
                    __instance.__1__state = 2;
                    return true;
                case 2:
                    __instance.__1__state = -1;
                    medScanMinigame.walking = null;
                    return false;
                default:
                    return false;
            }
        }

        return false;
    }
}

[HarmonyPatch(typeof(MedScanMinigame._WalkToOffset_d__15), nameof(MedScanMinigame._WalkToOffset_d__15.MoveNext))]
internal class MedscanMiniGamePatchWTO
{
    private static bool Prefix(MedScanMinigame._WalkToOffset_d__15 __instance)
    {
        if (TORMapOptions.disableMedscanWalking)
        {
            var num = __instance.__1__state;
            var medScanMinigame = __instance.__4__this;
            switch (num)
            {
                case 0:
                    __instance.__1__state = -1;
                    medScanMinigame.state = MedScanMinigame.PositionState.WalkingToOffset;
                    __instance.__1__state = 1;
                    return true;
                case 1:
                    __instance.__1__state = -1;
                    __instance.__2__current = new WaitForSeconds(0.1f);
                    __instance.__1__state = 2;
                    return true;
                case 2:
                    __instance.__1__state = -1;
                    medScanMinigame.walking = null;
                    return false;
                default:
                    return false;
            }
        }

        return false;
    }
}