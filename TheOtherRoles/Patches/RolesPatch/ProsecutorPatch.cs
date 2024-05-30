using Hazel;
using TheOtherRoles.Roles.Crewmate;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches.RolesPatch;

public class ProsecutorPatch
{
    public static void UpdateButton(PlayerControl p, MeetingHud __instance)
    {
        if (p != Prosecutor.prosecutor) return;

        var skip = __instance.SkipVoteButton;
        Prosecutor.Prosecute.gameObject.SetActive(skip.gameObject.active && !Prosecutor.Prosecuted);
        Prosecutor.Prosecute.voteComplete = skip.voteComplete;
        Prosecutor.Prosecute.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
        Prosecutor.Prosecute.GetComponentsInChildren<TextMeshPro>()[0].text = "起诉";
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHudStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (Prosecutor.prosecutor != CachedPlayer.LocalPlayer.PlayerControl) return;

            var skip = __instance.SkipVoteButton;
            Prosecutor.Prosecute = Object.Instantiate(skip, skip.transform.parent);
            Prosecutor.Prosecute.Parent = __instance;
            Prosecutor.Prosecute.SetTargetPlayerId(251);
            Prosecutor.Prosecute.transform.localPosition = skip.transform.localPosition + new Vector3(0f, -0.17f, 0f);
            skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
            UpdateButton(Prosecutor.prosecutor, __instance);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == Prosecutor.prosecutor)
                    UpdateButton(Prosecutor.prosecutor, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public class MeetingHudConfirm
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == Prosecutor.prosecutor)
                {
                    Prosecutor.Prosecute.ClearButtons();
                    UpdateButton(Prosecutor.prosecutor, __instance);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
        public class MeetingHudSelect
        {
            public static void Postfix(MeetingHud __instance, int __0)
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == Prosecutor.prosecutor)
                {
                    Prosecutor.Prosecute.ClearButtons();
                    UpdateButton(Prosecutor.prosecutor, __instance);
                    if (__0 != 251) Prosecutor.Prosecute.ClearButtons();
                    UpdateButton(Prosecutor.prosecutor, __instance);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public class MeetingHudVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (CachedPlayer.LocalPlayer.PlayerControl == Prosecutor.prosecutor)
                    UpdateButton(Prosecutor.prosecutor, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHudUpdate
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (CachedPlayer.LocalPlayer.PlayerControl != Prosecutor.prosecutor) return;
                switch (__instance.state)
                {
                    case MeetingHud.VoteStates.Discussion:
                        if (__instance.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
                        {
                            Prosecutor.Prosecute.SetDisabled();
                            break;
                        }
                        Prosecutor.Prosecute.SetEnabled();
                        break;
                }
                UpdateButton(Prosecutor.prosecutor, __instance);
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerVoteArea))]
public class AllowExtraVotes
{
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
    public static class VoteForMe
    {
        public static bool Prefix(PlayerVoteArea __instance)
        {
            if (Prosecutor.prosecutor != CachedPlayer.LocalPlayer.PlayerControl) return true;
            if (__instance.Parent.state == MeetingHud.VoteStates.Proceeding ||
                __instance.Parent.state == MeetingHud.VoteStates.Results)
                return false;

            if (__instance != Prosecutor.Prosecute)
            {
                if (Prosecutor.StartProsecute)
                {
                    Prosecutor.ProsecuteThisMeeting = true;
                    Prosecutor.StartProsecute = false;

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(Prosecutor.prosecutor.NetId, (byte)CustomRPC.Prosecute, SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Message("检察官会议 = " + Prosecutor.ProsecuteThisMeeting.ToString());
                }
                return true;
            }
            else
            {
                Prosecutor.StartProsecute = true;
                MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(false);
                ProsecutorPatch.UpdateButton(Prosecutor.prosecutor, MeetingHud.Instance);
                if (!AmongUsClient.Instance.AmHost)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(Prosecutor.prosecutor.NetId, (byte)CustomRPC.Prosecute, SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                return false;
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
class StartMeetingPatch
{
    public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
    {
        if (__instance == null)
        {
            return;
        }
        if (Prosecutor.prosecutor != null) Prosecutor.StartProsecute = false;
        return;
    }
}
[HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
public static class AirshipExileController_WrapUpAndSpawn
{
    public static void Postfix(AirshipExileController __instance) => ExilePros.ExileControllerPostfix(__instance);
}

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public class ExilePros
{
    public static void ExileControllerPostfix(ExileController __instance)
    {
        if (Prosecutor.prosecutor != null)
        {
            if (Prosecutor.ProsecuteThisMeeting)
            {
                var exiled = __instance.exiled?.Object;
                if (exiled != null && exiled == (Helpers.isKiller(exiled) || Helpers.isEvil(exiled)) && Prosecutor.diesOnIncorrectPros)
                {                    //ButtonTarget.DontRevive = Prosecutor.prosecutor.PlayerId;
                    Prosecutor.prosecutor.Exiled();
                }
                Prosecutor.ProsecuteThisMeeting = false;
            }
        }
    }

    public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), [typeof(GameObject)])]
    public static void Prefix(GameObject obj)
    {
        if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
        if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
    }
}

[HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
[HarmonyPriority(Priority.First)]
class ExileControllerPatch
{
    public static ExileController lastExiled;
    public static void Prefix(ExileController __instance)
    {
        lastExiled = __instance;
    }
}