using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace TheOtherRoles.Patches;

[HarmonyPatch]
internal class LightPatch
{
    public static void Initialize()
    {
        PlayerRadius = 0.5f;
        ClairvoyanceFlag = false;
        FlashlightEnabled = null;
    }

    public static void SetFlashLight(bool? useFlashLight)
    {
        FlashlightEnabled = useFlashLight;
        PlayerControl.LocalPlayer.AdjustLighting();
    }

    [HarmonyPatch(typeof(OneWayShadows), nameof(OneWayShadows.IsIgnored))]
    public static class OneWayShadowsPatch
    {
        public static void Postfix(OneWayShadows __instance, ref bool __result)
        {
            if (!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer?.Data == null) return;

            __result |= HasImpVision(PlayerControl.LocalPlayer.Data);
        }
    }

    [HarmonyPatch(typeof(ShadowCollab), nameof(ShadowCollab.OnEnable))]
    public static class ShadowCameraPatch
    {
        public static IEnumerator GetEnumerator(ShadowCollab __instance)
        {
            var cam = Camera.main;
            while (true)
            {
                __instance.ShadowCamera.orthographicSize = cam.orthographicSize;
                __instance.ShadowQuad.transform.localScale = new Vector3(cam.orthographicSize * cam.aspect, cam.orthographicSize) * 2f;
                yield return null;
            }
        }

        public static void Prefix(ShadowCollab __instance)
        {
            __instance.StartCoroutine(GetEnumerator(__instance).WrapToIl2Cpp());
        }
    }

    public static float PlayerRadius = 0.5f;

    [HarmonyPatch(typeof(LightSource), nameof(LightSource.Update))]
    public static class LightSourceUpdatePatch
    {

        public static bool Prefix(LightSource __instance)
        {
            var position = __instance.transform.position;
            position.z -= 7f;
            __instance.UpdateFlashlightAngle();
            __instance.LightCutawayMaterial.SetFloat("_PlayerRadius", PlayerRadius);
            __instance.LightCutawayMaterial.SetFloat("_LightRadius", __instance.ViewDistance);
            __instance.LightCutawayMaterial.SetVector("_LightOffset", __instance.LightOffset);
            __instance.LightCutawayMaterial.SetFloat("_FlashlightSize", __instance.FlashlightSize);
            __instance.LightCutawayMaterial.SetFloat("_FlashlightAngle", PlayerControl.LocalPlayer.FlashlightAngle);
            __instance.lightChild.transform.position = position;
            __instance.renderer.Render(position);

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
    public static class AdjustLightingPatch
    {
        public static bool Prefix(PlayerControl __instance)
        {
            if (PlayerControl.LocalPlayer != __instance) return false;

            var num = 0f;
            var flashFlag = false;
            if (FlashlightEnabled.HasValue) flashFlag = FlashlightEnabled.Value;
            else if (__instance.IsFlashlightEnabled()) flashFlag = true;
            else if (__instance.lightSource.useFlashlight) flashFlag = true;

            if (__instance.IsFlashlightEnabled())
            {
                if (__instance.Data.Role.IsImpostor)
                    GameOptionsManager.Instance.CurrentGameOptions.TryGetFloat(FloatOptionNames.ImpostorFlashlightSize, out num);
                else
                    GameOptionsManager.Instance.CurrentGameOptions.TryGetFloat(FloatOptionNames.CrewmateFlashlightSize, out num);
            }
            else if (__instance.lightSource.useFlashlight)
            {
                num = __instance.lightSource.flashlightSize;
            }

            __instance.SetFlashlightInputMethod();
            __instance.lightSource.SetupLightingForGameplay(flashFlag, num, __instance.TargetFlashlight.transform);

            return false;
        }
    }

    public static bool ClairvoyanceFlag = false;
    public static bool? FlashlightEnabled = false;

    //影貫通

    [HarmonyPatch(typeof(LightSourceGpuRenderer), nameof(LightSourceGpuRenderer.GPUShadows))]
    public static class LightSourceGpuRendererPatch
    {
        private static Il2CppReferenceArray<Collider2D> origArray;
        private static Il2CppReferenceArray<Collider2D> zeroArray = new(0);

        public static void Prefix(LightSourceGpuRenderer __instance)
        {
            origArray = __instance.hits;
            if (ClairvoyanceFlag)
                __instance.hits = zeroArray;
        }

        public static void Postfix(LightSourceGpuRenderer __instance)
        {
            if (__instance.hits != origArray) __instance.hits = origArray;
        }
    }

    [HarmonyPatch(typeof(LightSourceRaycastRenderer), nameof(LightSourceRaycastRenderer.RaycastShadows))]
    public static class LightSourceRaycastRendererPatch
    {
        private static Il2CppReferenceArray<Collider2D> origArray;
        private static Il2CppReferenceArray<Collider2D> zeroArray = new(0);

        public static void Prefix(LightSourceRaycastRenderer __instance)
        {
            origArray = __instance.hits;
            if (ClairvoyanceFlag)
                __instance.hits = zeroArray;
        }

        public static void Postfix(LightSourceGpuRenderer __instance)
        {
            if (__instance.hits != origArray) __instance.hits = origArray;
        }
    }
}