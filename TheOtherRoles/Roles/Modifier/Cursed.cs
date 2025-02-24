using System.Linq;
using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class Cursed
{
    public static PlayerControl cursed;
    public static Color color = new Color32(0, 247, 255, byte.MaxValue);
    public static bool hideModifier;

    public static void clearAndReload()
    {
        cursed = null;
        hideModifier = CustomOptionHolder.modifierHideCursed.GetBool();
    }

    [HarmonyPatch]
    private static class Cursed_Patch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update)), HarmonyPostfix]
        private static void Postfix(HudManager __instance)
        {
            if (cursed.IsDead() || !InGame || cursed != PlayerControl.LocalPlayer) return;

            var allPlayers = PlayerControl.AllPlayerControls.ToList();
            var impostorCount = allPlayers.Count(x => x.IsImpostor() && x.IsAlive());

            if (impostorCount >= allPlayers.Count(x => !x.IsImpostor() && x.IsAlive()))
            {
                turnToImpostorRPC(cursed);
            }
        }
    }
}
