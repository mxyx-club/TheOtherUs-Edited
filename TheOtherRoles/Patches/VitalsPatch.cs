using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Helper;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.GameHistory;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

[Harmony]
public class VitalsPatch
{
    private static float vitalsTimer;
    private static TextMeshPro TimeRemaining;
    private static List<TextMeshPro> hackerTexts = new();

    public static void ResetData()
    {
        vitalsTimer = 0f;
        if (TimeRemaining != null)
        {
            Object.Destroy(TimeRemaining);
            TimeRemaining = null;
        }
    }

    private static void UseVitalsTime()
    {
        // Don't waste network traffic if we're out of time.
        if (TORMapOptions.restrictDevices > 0 && TORMapOptions.restrictVitalsTime > 0f &&
            CachedPlayer.LocalPlayer.PlayerControl.isAlive() && CachedPlayer.LocalPlayer.PlayerControl != Hacker.hacker)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.UseVitalsTime, SendOption.Reliable);
            writer.Write(vitalsTimer);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.useVitalsTime(vitalsTimer);
        }

        vitalsTimer = 0f;
    }

    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    private class VitalsMinigameStartPatch
    {
        private static void Postfix(VitalsMinigame __instance)
        {
            vitalsTimer = 0f;

            if (Hacker.hacker != null && CachedPlayer.LocalPlayer.PlayerControl == Hacker.hacker)
            {
                hackerTexts = new List<TextMeshPro>();
                foreach (var panel in __instance.vitals)
                {
                    var text = Object.Instantiate(__instance.SabText, panel.transform);
                    hackerTexts.Add(text);
                    Object.DestroyImmediate(text.GetComponent<AlphaBlink>());
                    text.gameObject.SetActive(false);
                    text.transform.localScale = Vector3.one * 0.75f;
                    text.transform.localPosition = new Vector3(-0.75f, -0.23f, 0f);
                }
            }
        }
    }

    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
    private class VitalsMinigameUpdatePatch
    {
        private static bool Prefix(VitalsMinigame __instance)
        {
            vitalsTimer += Time.deltaTime;
            if (vitalsTimer > 0.1f)
                UseVitalsTime();

            if (TORMapOptions.restrictDevices > 0)
            {
                if (TimeRemaining == null)
                {
                    TimeRemaining = Object.Instantiate(HudManager.Instance.TaskPanel.taskText, __instance.transform);
                    TimeRemaining.alignment = TextAlignmentOptions.BottomRight;
                    TimeRemaining.transform.position = Vector3.zero;
                    TimeRemaining.transform.localPosition = new Vector3(1.7f, 4.45f);
                    TimeRemaining.transform.localScale *= 1.8f;
                    TimeRemaining.color = Palette.White;
                }

                if (TORMapOptions.restrictVitalsTime <= 0f && CachedPlayer.LocalPlayer.PlayerControl != Hacker.hacker &&
                    !CachedPlayer.LocalPlayer.Data.IsDead)
                {
                    __instance.Close();
                    return false;
                }

                var timeString = TimeSpan.FromSeconds(TORMapOptions.restrictVitalsTime).ToString(@"mm\:ss\.ff");
                TimeRemaining.text = string.Format("Remaining: {0}", timeString);
                TimeRemaining.gameObject.SetActive(true);
            }

            return true;
        }

        private static void Postfix(VitalsMinigame __instance)
        {
            // Hacker show time since death
            if (Hacker.hacker != null && Hacker.hacker == CachedPlayer.LocalPlayer.PlayerControl &&
                Hacker.hackerTimer > 0)
                for (var k = 0; k < __instance.vitals.Length; k++)
                {
                    var vitalsPanel = __instance.vitals[k];
                    var player = GameData.Instance.AllPlayers.Get(k);

                    // Hacker update
                    if (!vitalsPanel.IsDead) continue;
                    var deadPlayer = deadPlayers?.Where(x => x.player.PlayerId == player?.PlayerId)?.FirstOrDefault();
                    if (deadPlayer == null || k >= hackerTexts.Count || hackerTexts[k] == null) continue;
                    var timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.timeOfDeath).TotalMilliseconds;
                    hackerTexts[k].gameObject.SetActive(true);
                    hackerTexts[k].text = Math.Round(timeSinceDeath / 1000) + "s";
                }
            else
                foreach (var text in hackerTexts.Where(text => text != null && text.gameObject != null))
                    text.gameObject.SetActive(false);
        }
    }
}