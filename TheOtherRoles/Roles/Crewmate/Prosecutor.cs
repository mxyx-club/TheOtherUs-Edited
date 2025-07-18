namespace TheOtherRoles.Roles.Crewmate;

public static class Prosecutor
{
    public static PlayerControl prosecutor;
    public static Color color = new Color32(178, 128, 0, byte.MaxValue);
    public static bool diesOnIncorrectPros;
    public static bool canCallEmergency;
    public static int tasksNeededToSeeVoteColors;

    public static bool Prosecuted;
    public static bool StartProsecute;
    public static bool ProsecuteThisMeeting;
    public static PlayerVoteArea Prosecute;

    public static bool CanSeeVoteColors
    {
        get
        {
            if (!field) return false;
            return PlayerControl.LocalPlayer == prosecutor && TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data).Item1 >= tasksNeededToSeeVoteColors;
        }
        private set;
    }

    public static void clearAndReload()
    {
        prosecutor = null;
        ProsecuteThisMeeting = false;
        StartProsecute = false;
        Prosecuted = false;
        CanSeeVoteColors = CustomOptionHolder.prosecutorCanSeeVoteColors.GetBool();
        tasksNeededToSeeVoteColors = CustomOptionHolder.prosecutorTasksNeededToSeeVoteColors.GetInt();
        diesOnIncorrectPros = CustomOptionHolder.prosecutorDiesOnIncorrectPros.GetBool();
        canCallEmergency = CustomOptionHolder.prosecutorCanCallEmergency.GetBool();
    }

    [HarmonyPatch]
    public class Prosecutor_Patch
    {
        public static void UpdateButton(PlayerControl p, MeetingHud __instance)
        {
            if (p != prosecutor || !prosecutor.CanUseMeetingAbility() || Prosecute == null) return;

            var skip = __instance.SkipVoteButton;
            Prosecute?.gameObject.SetActive(skip.gameObject.active && !Prosecuted);
            Prosecute?.voteComplete = skip.voteComplete;
            Prosecute?.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            Prosecute?.GetComponentsInChildren<TextMeshPro>()[0].text = "Prosecutor.Button".Translate();
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting)), HarmonyPrefix]
        private static void StartMeetingPrefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null) return;

            if (prosecutor != null) StartProsecute = false;
            return;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        public static void MeetingStartPostfix(MeetingHud __instance)
        {
            if (prosecutor != PlayerControl.LocalPlayer || !prosecutor.CanUseMeetingAbility()) return;

            var skip = __instance.SkipVoteButton;
            Prosecute = UObject.Instantiate(skip, skip.transform.parent);
            Prosecute.Parent = __instance;
            Prosecute.SetTargetPlayerId(251);
            Prosecute.transform.localPosition = skip.transform.localPosition + new Vector3(0f, -0.15f, 0f);
            skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
            UpdateButton(prosecutor, __instance);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote)), HarmonyPostfix]
        public static void MeetingClearVotePostfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer != prosecutor) return;
            UpdateButton(prosecutor, __instance);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm)), HarmonyPostfix]
        public static void MeetingConfirmPostfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer != prosecutor || !prosecutor.CanUseMeetingAbility()) return;
            Prosecute.ClearButtons();
            UpdateButton(prosecutor, __instance);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select)), HarmonyPostfix]
        public static void MeetingSelectPostfix(MeetingHud __instance, int __0)
        {
            if (PlayerControl.LocalPlayer != prosecutor || !prosecutor.CanUseMeetingAbility()) return;
            Prosecute.ClearButtons();
            UpdateButton(prosecutor, __instance);
            if (__0 != 251) Prosecute.ClearButtons();
            UpdateButton(prosecutor, __instance);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete)), HarmonyPostfix]
        public static void MeetingVotingCompletePostfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer != prosecutor || !prosecutor.CanUseMeetingAbility()) return;
            UpdateButton(prosecutor, __instance);
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update)), HarmonyPostfix]
        public static void MeetingUpdatePostfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer != prosecutor || !prosecutor.CanUseMeetingAbility()) return;
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

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe)), HarmonyPrefix]
        public static bool PlayerVoteAreaVoteForMePrefix(PlayerVoteArea __instance)
        {
            if (prosecutor != PlayerControl.LocalPlayer || !prosecutor.CanUseMeetingAbility()) return true;

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