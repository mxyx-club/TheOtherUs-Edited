using System.Collections.Generic;
using Hazel;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class LastImpostor
{
    public static PlayerControl lastImpostor;
    public static Color color = Palette.ImpostorRed;

    public static float deduce = 2.5f;
    public static bool isEnable;

    public static void clearAndReload()
    {
        lastImpostor = null;
        deduce = CustomOptionHolder.modifierLastImpostorDeduce.GetFloat();
        isEnable = CustomOptionHolder.modifierLastImpostor.GetBool();
    }

    [HarmonyPatch]
    private static class LastImpostor_Patch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update)), HarmonyPostfix]
        private static void Postfix()
        {
            if (!isEnable || !InGame || (ModOption.NumImpostors == 1 && !ModOption.DebugMode)) return;

            if (PlayerControl.LocalPlayer.IsImpostor()
                && PlayerControl.LocalPlayer.IsAlive()
                && lastImpostor != PlayerControl.LocalPlayer
                && PlayerControl.AllPlayerControls.Count(x => x.IsImpostor() && x.IsAlive()) == 1)
            {
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ImpostorPromotesToLastImpostor);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.EndRPC();
                RPCProcedure.impostorPromotesToLastImpostor(PlayerControl.LocalPlayer.PlayerId);
            }

            if (lastImpostor != null && !lastImpostor.IsImpostor()) clearAndReload();
        }
    }
}
