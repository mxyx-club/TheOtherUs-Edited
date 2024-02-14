using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Patches
{

    [HarmonyPatch(typeof(MedScanMinigame._WalkToOffset_d__15), nameof(MedScanMinigame._WalkToPad_d__16.MoveNext))]
    class MedscanMiniGamePatchWTP
    {
        static bool Prefix(MedScanMinigame._WalkToPad_d__16 __instance)
        {
            if (TORMapOptions.disableMedscanWalking)
            {
                int num = __instance.__1__state;
                MedScanMinigame medScanMinigame = __instance.__4__this;
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
                        medScanMinigame.walking = (Coroutine)null;
                        return false;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(MedScanMinigame._WalkToOffset_d__15), nameof(MedScanMinigame._WalkToOffset_d__15.MoveNext))]
    class MedscanMiniGamePatchWTO
    {
        static bool Prefix(MedScanMinigame._WalkToOffset_d__15 __instance)
        {
            if (TORMapOptions.disableMedscanWalking)
            {
                int num = __instance.__1__state;
                MedScanMinigame medScanMinigame = __instance.__4__this;
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
                        medScanMinigame.walking = (Coroutine)null;
                        return false;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}