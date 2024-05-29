using System;
using System.Linq;
using Hazel;
using TheOtherRoles.Roles.Crewmate;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

[Harmony]
public class CameraPatch
{
    private static float cameraTimer;

    public static void ResetData()
    {
        cameraTimer = 0f;
        SurveillanceMinigamePatch.ResetData();
        PlanetSurveillanceMinigamePatch.ResetData();
    }

    private static void UseCameraTime()
    {
        // Don't waste network traffic if we're out of time.
        if (MapOptions.restrictDevices > 0 && MapOptions.restrictCamerasTime > 0f &&
            CachedPlayer.LocalPlayer.PlayerControl.isAlive() &&
            CachedPlayer.LocalPlayer.PlayerControl != Hacker.hacker &&
            CachedPlayer.LocalPlayer.PlayerControl != SecurityGuard.securityGuard)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.UseCameraTime, SendOption.Reliable);
            writer.Write(cameraTimer);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.useCameraTime(cameraTimer);
        }

        cameraTimer = 0f;
    }

    [HarmonyPatch]
    private class SurveillanceMinigamePatch
    {
        private static int page;
        private static float timer;
        private static TextMeshPro TimeRemaining;

        public static void ResetData()
        {
            if (TimeRemaining == null) return;
            Object.Destroy(TimeRemaining);
            TimeRemaining = null;
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
        private class SurveillanceMinigameBeginPatch
        {
            public static void Prefix(SurveillanceMinigame __instance)
            {
                cameraTimer = 0f;
            }

            public static void Postfix(SurveillanceMinigame __instance)
            {
                // Add securityGuard cameras
                page = 0;
                timer = 0;
                if (ShipStatus.Instance.AllCameras.Length <= 4 || __instance.FilteredRooms.Length <= 0) return;
                __instance.textures = __instance.textures.ToList()
                    .Concat(new RenderTexture[ShipStatus.Instance.AllCameras.Length - 4]).ToArray();
                for (var i = 4; i < ShipStatus.Instance.AllCameras.Length; i++)
                {
                    var surv = ShipStatus.Instance.AllCameras[i];
                    var camera = Object.Instantiate(__instance.CameraPrefab);
                    Transform transform;
                    (transform = camera.transform).SetParent(__instance.transform);
                    var transform1 = surv.transform;
                    var position = transform1.position;
                    transform.position =
                        new Vector3(position.x, position.y, 8f);
                    camera.orthographicSize = 2.35f;
                    var temporary = RenderTexture.GetTemporary(256, 256, 16, (RenderTextureFormat)0);
                    __instance.textures[i] = temporary;
                    camera.targetTexture = temporary;
                }
            }
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
        private class SurveillanceMinigameUpdatePatch
        {
            public static bool Prefix(SurveillanceMinigame __instance)
            {
                cameraTimer += Time.deltaTime;
                if (cameraTimer > 0.1f)
                    UseCameraTime();

                if (MapOptions.restrictDevices > 0)
                {
                    if (TimeRemaining == null)
                    {
                        TimeRemaining =
                            Object.Instantiate(HudManager.Instance.TaskPanel.taskText, __instance.transform);
                        TimeRemaining.alignment = TextAlignmentOptions.Center;
                        TimeRemaining.transform.position = Vector3.zero;
                        TimeRemaining.transform.localPosition = new Vector3(0.0f, -1.7f);
                        TimeRemaining.transform.localScale *= 1.8f;
                        TimeRemaining.color = Palette.White;
                    }

                    if (MapOptions.disableCamsRoundOne && MapOptions.isRoundOne)
                    {
                        __instance.Close();
                        return false;
                    }

                    if (MapOptions.restrictCamerasTime <= 0f &&
                        CachedPlayer.LocalPlayer.PlayerControl != Hacker.hacker &&
                        CachedPlayer.LocalPlayer.PlayerControl != SecurityGuard.securityGuard &&
                        !CachedPlayer.LocalPlayer.Data.IsDead)
                    {
                        __instance.Close();
                        return false;
                    }

                    var timeString = TimeSpan.FromSeconds(MapOptions.restrictCamerasTime).ToString(@"mm\:ss\.ff");
                    TimeRemaining.text = $"Remaining: {timeString}";
                    TimeRemaining.gameObject.SetActive(true);
                }

                // Update normal and securityGuard cameras
                timer += Time.deltaTime;
                var numberOfPages = Mathf.CeilToInt(ShipStatus.Instance.AllCameras.Length / 4f);

                if (timer > 3f || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    timer = 0f;
                    page = (page + 1) % numberOfPages;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    page = (page + numberOfPages - 1) % numberOfPages;
                    timer = 0f;
                }

                switch (__instance.isStatic)
                {
                    case true or true when
                        !PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(CachedPlayer.LocalPlayer.PlayerControl):
                    {
                        __instance.isStatic = false;
                        for (var i = 0; i < __instance.ViewPorts.Length; i++)
                        {
                            __instance.ViewPorts[i].sharedMaterial = __instance.DefaultMaterial;
                            __instance.SabText[i].gameObject.SetActive(false);
                            if ((page * 4) + i < __instance.textures.Length)
                                __instance.ViewPorts[i].material
                                    .SetTexture("_MainTex", __instance.textures[(page * 4) + i]);
                            else
                                __instance.ViewPorts[i].sharedMaterial = __instance.StaticMaterial;
                        }

                        break;
                    }
                    case false when
                        PlayerTask.PlayerHasTaskOfType<HudOverrideTask>(CachedPlayer.LocalPlayer.PlayerControl):
                    {
                        __instance.isStatic = true;
                        for (var j = 0; j < __instance.ViewPorts.Length; j++)
                        {
                            __instance.ViewPorts[j].sharedMaterial = __instance.StaticMaterial;
                            __instance.SabText[j].gameObject.SetActive(true);
                        }

                        break;
                    }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Close))]
        private class SurveillanceMinigameClosePatch
        {
            private static void Prefix(SurveillanceMinigame __instance)
            {
                UseCameraTime();
            }
        }
    }

    [HarmonyPatch]
    private class PlanetSurveillanceMinigamePatch
    {
        private static TextMeshPro TimeRemaining;

        public static void ResetData()
        {
            if (TimeRemaining == null) return;
            Object.Destroy(TimeRemaining);
            TimeRemaining = null;
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Begin))]
        private class PlanetSurveillanceMinigameBeginPatch
        {
            public static void Prefix(PlanetSurveillanceMinigame __instance)
            {
                cameraTimer = 0f;
            }
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Update))]
        private class PlanetSurveillanceMinigameUpdatePatch
        {
            public static bool Prefix(PlanetSurveillanceMinigame __instance)
            {
                cameraTimer += Time.deltaTime;
                if (cameraTimer > 0.1f)
                    UseCameraTime();

                if (MapOptions.restrictDevices <= 0) return true;
                if (TimeRemaining == null)
                {
                    TimeRemaining =
                        Object.Instantiate(HudManager.Instance.TaskPanel.taskText, __instance.transform);
                    TimeRemaining.alignment = TextAlignmentOptions.BottomRight;
                    TimeRemaining.transform.position = Vector3.zero;
                    TimeRemaining.transform.localPosition = new Vector3(0.95f, 4.45f);
                    TimeRemaining.transform.localScale *= 1.8f;
                    TimeRemaining.color = Palette.White;
                }

                if (MapOptions.disableCamsRoundOne && MapOptions.isRoundOne)
                {
                    __instance.Close();
                    return false;
                }

                if (MapOptions.restrictCamerasTime <= 0f &&
                    CachedPlayer.LocalPlayer.PlayerControl != Hacker.hacker &&
                    CachedPlayer.LocalPlayer.PlayerControl != SecurityGuard.securityGuard &&
                    !CachedPlayer.LocalPlayer.Data.IsDead)
                {
                    __instance.Close();
                    return false;
                }

                var timeString = TimeSpan.FromSeconds(MapOptions.restrictCamerasTime).ToString(@"mm\:ss\.ff");
                TimeRemaining.text = $"Remaining: {timeString}";
                TimeRemaining.gameObject.SetActive(true);

                return true;
            }
        }


        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Close))]
        private class PlanetSurveillanceMinigameClosePatch
        {
            private static void Prefix(PlanetSurveillanceMinigame __instance)
            {
                UseCameraTime();
            }
        }
    }

    [HarmonyPatch]
    private class DoorLogPatch
    {
        private static TextMeshPro TimeRemaining;

        public static void ResetData()
        {
            if (TimeRemaining != null)
            {
                Object.Destroy(TimeRemaining);
                TimeRemaining = null;
            }
        }

        [HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
        private class SecurityLogGameBeginPatch
        {
            public static void Prefix(Minigame __instance)
            {
                if (__instance is SecurityLogGame)
                    cameraTimer = 0f;
            }
        }

        [HarmonyPatch(typeof(SecurityLogGame), nameof(SecurityLogGame.Update))]
        private class SecurityLogGameUpdatePatch
        {
            public static bool Prefix(SecurityLogGame __instance)
            {
                cameraTimer += Time.deltaTime;
                if (cameraTimer > 0.1f)
                    UseCameraTime();

                if (MapOptions.restrictDevices <= 0) return true;
                if (TimeRemaining == null)
                {
                    TimeRemaining =
                        Object.Instantiate(HudManager.Instance.TaskPanel.taskText, __instance.transform);
                    TimeRemaining.alignment = TextAlignmentOptions.BottomRight;
                    TimeRemaining.transform.position = Vector3.zero;
                    TimeRemaining.transform.localPosition = new Vector3(1.0f, 4.25f);
                    TimeRemaining.transform.localScale *= 1.6f;
                    TimeRemaining.color = Palette.White;
                }

                if (MapOptions.restrictCamerasTime <= 0f &&
                    CachedPlayer.LocalPlayer.PlayerControl != Hacker.hacker &&
                    CachedPlayer.LocalPlayer.PlayerControl != SecurityGuard.securityGuard &&
                    !CachedPlayer.LocalPlayer.Data.IsDead)
                {
                    __instance.Close();
                    return false;
                }

                var timeString = TimeSpan.FromSeconds(MapOptions.restrictCamerasTime).ToString(@"mm\:ss\.ff");
                TimeRemaining.text = string.Format("Remaining: {0}", timeString);
                TimeRemaining.gameObject.SetActive(true);

                return true;
            }
        }
    }
}