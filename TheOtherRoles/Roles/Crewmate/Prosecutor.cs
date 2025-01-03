using Hazel;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Prosecutor
{
    public static PlayerControl prosecutor;
    public static Color color = new Color32(178, 128, 0, byte.MaxValue);
    public static bool diesOnIncorrectPros;
    public static bool canCallEmergency;
    public static bool canSeeVoteColors;
    public static int tasksNeededToSeeVoteColors;

    public static bool Prosecuted;
    public static bool StartProsecute;
    public static bool ProsecuteThisMeeting;
    public static PlayerVoteArea Prosecute;


    public static void clearAndReload()
    {
        prosecutor = null;
        ProsecuteThisMeeting = false;
        StartProsecute = false;
        Prosecuted = false;
        canSeeVoteColors = CustomOptionHolder.prosecutorCanSeeVoteColors.GetBool();
        tasksNeededToSeeVoteColors = CustomOptionHolder.prosecutorTasksNeededToSeeVoteColors.GetInt();
        diesOnIncorrectPros = CustomOptionHolder.prosecutorDiesOnIncorrectPros.GetBool();
        canCallEmergency = CustomOptionHolder.prosecutorCanCallEmergency.GetBool();
    }

    [HarmonyPatch]
    public class SkipVoteButtonPatch
    {
        public static void UpdateButton(PlayerControl p, MeetingHud __instance)
        {
            if (p != prosecutor) return;

            var skip = __instance.SkipVoteButton;
            Prosecute.gameObject.SetActive(skip.gameObject.active && !Prosecuted);
            Prosecute.voteComplete = skip.voteComplete;
            Prosecute.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            Prosecute.GetComponentsInChildren<TextMeshPro>()[0].text = "起诉";
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (prosecutor != CachedPlayer.LocalPlayer.PlayerControl) return;

                var skip = __instance.SkipVoteButton;
                Prosecute = Object.Instantiate(skip, skip.transform.parent);
                Prosecute.Parent = __instance;
                Prosecute.SetTargetPlayerId(251);
                Prosecute.transform.localPosition = skip.transform.localPosition + new Vector3(0f, -0.15f, 0f);
                skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
                UpdateButton(prosecutor, __instance);
            }

            [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
            public class MeetingHudClearVote
            {
                public static void Postfix(MeetingHud __instance)
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl == prosecutor)
                        UpdateButton(prosecutor, __instance);
                }
            }

            [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
            public class MeetingHudConfirm
            {
                public static void Postfix(MeetingHud __instance)
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl == prosecutor)
                    {
                        Prosecute.ClearButtons();
                        UpdateButton(prosecutor, __instance);
                    }
                }
            }

            [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
            public class MeetingHudSelect
            {
                public static void Postfix(MeetingHud __instance, int __0)
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl == prosecutor)
                    {
                        Prosecute.ClearButtons();
                        UpdateButton(prosecutor, __instance);
                        if (__0 != 251) Prosecute.ClearButtons();
                        UpdateButton(prosecutor, __instance);
                    }
                }
            }

            [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
            public class MeetingHudVotingComplete
            {
                public static void Postfix(MeetingHud __instance)
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl == prosecutor)
                        UpdateButton(prosecutor, __instance);
                }
            }

            [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
            public class MeetingHudUpdate
            {
                public static void Postfix(MeetingHud __instance)
                {
                    if (CachedPlayer.LocalPlayer.PlayerControl != prosecutor) return;
                    switch (__instance.state)
                    {
                        case MeetingHud.VoteStates.Discussion:
                            if (__instance.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
                            {
                                Prosecute.SetDisabled();
                                break;
                            }
                            Prosecute.SetEnabled();
                            break;
                    }
                    UpdateButton(prosecutor, __instance);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
        public static class VoteForMe
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                if (prosecutor != CachedPlayer.LocalPlayer.PlayerControl) return true;
                if (__instance.Parent.state is MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Results)
                    return false;

                if (__instance != Prosecute)
                {
                    if (StartProsecute)
                    {
                        ProsecuteThisMeeting = true;
                        StartProsecute = false;

                        var writer = AmongUsClient.Instance.StartRpcImmediately(prosecutor.NetId,
                            (byte)CustomRPC.Prosecute, SendOption.Reliable, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    return true;
                }
                else
                {
                    StartProsecute = true;
                    MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(false);
                    UpdateButton(prosecutor, MeetingHud.Instance);
                    if (!AmongUsClient.Instance.AmHost)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(prosecutor.NetId, (byte)CustomRPC.Prosecute, SendOption.Reliable, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    return false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public class StartMeetingPatch
    {
        private static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
        {
            if (__instance == null) return;
            if (prosecutor != null) StartProsecute = false;
            return;
        }
    }
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance)
        {
            ExilePros.ExileControllerPostfix(__instance);
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class ExilePros
    {
        public static void Postfix(ExileController __instance)
        {
            ExileControllerPostfix(__instance);
        }

        public static void ExileControllerPostfix(ExileController __instance)
        {
            if (prosecutor != null && ProsecuteThisMeeting)
            {
                var exiled = __instance.initData.networkedPlayer?.Object;
                if (exiled != null && exiled == exiled.isCrew() && diesOnIncorrectPros)
                {
                    prosecutor.Exiled();
                }

                if (exiled == null) Prosecuted = false;
                ProsecuteThisMeeting = false;
            }
        }

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), [typeof(GameObject)])]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    internal class ExileControllerPatch
    {
        public static ExileController lastExiled;
        public static void Prefix(ExileController __instance)
        {
            lastExiled = __instance;
        }
    }
}