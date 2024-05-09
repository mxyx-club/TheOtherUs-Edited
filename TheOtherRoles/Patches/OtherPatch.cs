using TheOtherRoles.Modules;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
    public static class HatManager_Initialize
    {
        public static void Postfix(HatManager __instance)
        {
            CosmeticsUnlocker.unlockCosmetics(__instance);
        }
    }
}
