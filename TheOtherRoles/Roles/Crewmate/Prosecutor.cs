namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Prosecutor : RoleBase, IPowerCrew
{
    public static Color color = new Color32(178, 128, 0, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Prosecutor),
        (p) => new Prosecutor(p),
        RoleId.Prosecutor,
        RoleType.Crewmate,
        "Prosecutor",
        color,
        303600,
        AddOptions
    );

    public Prosecutor(PlayerControl p) : base(p, roleinfo) { }

    public static bool IsProsecuteMeeting => GetRoles(RoleId.Prosecutor).Cast<Prosecutor>().Any(x => x.Player.IsAlive() && x.ProsecuteThisMeeting);

    public static bool diesOnIncorrectPros;
    public static bool canCallEmergency;
    public static bool canSeeVoteColors;
    public static int tasksNeededToSeeVoteColors;

    public bool IsAbilityUsed;
    public bool StartProsecute;
    public bool ProsecuteThisMeeting;
    public static PlayerVoteArea Prosecute;

    public static CustomOption prosecutorCanSeeVoteColors;
    public static CustomOption prosecutorTasksNeededToSeeVoteColors;
    public static CustomOption prosecutorDiesOnIncorrectPros;
    public static CustomOption prosecutorCanCallEmergency;
    public static RemoteProcess<(PlayerControl player, bool pros)> ProsecuterStatus = new("Prosecute", (data, _) =>
    {
        var role = data.player.GetRole<Prosecutor>();
        role.ProsecuteThisMeeting = data.pros;
    });

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        prosecutorCanSeeVoteColors = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mayorCanSeeVoteColors", true, roleinfo.RoleOption);
        prosecutorTasksNeededToSeeVoteColors = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mayorTasksNeededToSeeVoteColors", 5f, 0f, 20f, 1f, prosecutorCanSeeVoteColors);
        prosecutorDiesOnIncorrectPros = CustomOption.Create(configId++, CustomOptionType.Crewmate, "prosecutorDiesOnIncorrectPros", true, roleinfo.RoleOption);
        prosecutorCanCallEmergency = CustomOption.Create(configId++, CustomOptionType.Crewmate, "canCallEmergency", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        ProsecuteThisMeeting = false;
        StartProsecute = false;
        IsAbilityUsed = false;
        canSeeVoteColors = prosecutorCanSeeVoteColors.GetBool();
        tasksNeededToSeeVoteColors = prosecutorTasksNeededToSeeVoteColors.GetInt();
        diesOnIncorrectPros = prosecutorDiesOnIncorrectPros.GetBool();
        canCallEmergency = prosecutorCanCallEmergency.GetBool();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        if (Player != PlayerControl.LocalPlayer) return;

        var skip = __instance.SkipVoteButton;
        Prosecute = UObject.Instantiate(skip, skip.transform.parent);
        Prosecute.Parent = __instance;
        Prosecute.SetTargetPlayerId(251);
        Prosecute.transform.localPosition = skip.transform.localPosition + new Vector3(0f, -0.15f, 0f);
        skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
        UpdateButton(Player, __instance);

        if (Player != null) StartProsecute = false;
    }

    public override void OnPlayerDisconnect(PlayerControl player)
    {
        if (StartProsecute && player == Player)
        {
            StartProsecute = false;
            ProsecuteThisMeeting = false;
        }
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (ProsecuteThisMeeting)
        {
            if (exiled?.Object?.IsCrew() == true && diesOnIncorrectPros)
            {
                Player.Exiled();
            }

            if (exiled == null) IsAbilityUsed = false;
            ProsecuteThisMeeting = false;
        }
    }

    public override void OnPlayerDeath(PlayerControl player, bool isOnMeeting = false)
    {
        if (Player == player)
        {
            IsAbilityUsed = false;
            StartProsecute = false;
            ProsecuteThisMeeting = false;
        }
    }

    public override void ClearVotes(MeetingHud __instance)
    {
        UpdateButton(Player, __instance);
    }

    public override void OnMeetingUpdate(MeetingHud __instance)
    {
        if (PlayerControl.LocalPlayer != Player) return;
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
        UpdateButton(Player, __instance);
    }

    public static void UpdateButton(PlayerControl p, MeetingHud __instance)
    {
        if (!p.TryGetRole<Prosecutor>(out var role)) return;

        var skip = __instance.SkipVoteButton;
        Prosecute.gameObject.SetActive(skip.gameObject.active && !role.IsAbilityUsed);
        Prosecute.voteComplete = skip.voteComplete;
        Prosecute.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
        Prosecute.GetComponentsInChildren<TextMeshPro>()[0].text = "起诉";
    }

    [HarmonyPatch]
    public static class Prosecutor_Patch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public class MeetingHudConfirm
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (PlayerControl.LocalPlayer.TryGetRole<Prosecutor>(out var role))
                {
                    Prosecute.ClearButtons();
                    UpdateButton(role.Player, __instance);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
        public class MeetingHudSelect
        {
            public static void Postfix(MeetingHud __instance, int __0)
            {
                if (PlayerControl.LocalPlayer.TryGetRole<Prosecutor>(out var role))
                {
                    Prosecute.ClearButtons();
                    UpdateButton(role.Player, __instance);
                    if (__0 != 251) Prosecute.ClearButtons();
                    UpdateButton(role.Player, __instance);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public class MeetingHudVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (PlayerControl.LocalPlayer.TryGetRole<Prosecutor>(out var role))
                    UpdateButton(role.Player, __instance);
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
        public static class VoteForMe
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                if (!PlayerControl.LocalPlayer.TryGetRole<Prosecutor>(out var role)) return true;
                if (__instance.Parent.state is MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Results)
                    return false;

                if (__instance != Prosecute)
                {
                    if (role.StartProsecute)
                    {
                        role.ProsecuteThisMeeting = true;
                        role.StartProsecute = false;

                        ProsecuterStatus.Invoke((role.Player, true));
                    }
                    return true;
                }
                else
                {
                    role.StartProsecute = true;
                    MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(false);
                    UpdateButton(role.Player, MeetingHud.Instance);
                    if (!AmongUsClient.Instance.AmHost)
                    {
                        ProsecuterStatus.Invoke((role.Player, true));
                    }
                    return false;
                }
            }
        }
    }
}