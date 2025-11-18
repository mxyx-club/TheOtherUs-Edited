using static MeetingHud;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Mayor : RoleBase
{
    public static Color color = new Color32(32, 77, 66, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Mayor),
        (p) => new Mayor(p),
        RoleId.Mayor,
        RoleType.Crewmate,
        "Mayor",
        color,
        301100,
        AddOptions
    );

    public Mayor(PlayerControl p) : base(p, roleinfo) { }

    public bool Revealed;
    public static int Vote;

    public static bool meetingButton = true;
    public static int remoteMeetingsLeft = 1;
    public static bool SabotageRemoteMeetings = true;
    public static int vision = 5;

    public static CustomOption mayorMeetingButtonOption;
    public static CustomOption mayorMaxRemoteMeetings;
    public static CustomOption mayorSabotageRemoteMeetings;
    public static CustomOption mayorVote;
    public static CustomOption mayorRevealVision;

    public static GameObject MeetingExtraButton;
    private static TextMeshPro meetingExtraButtonLabel;

    public CustomButton mayorMeetingButton;
    public static Sprite emergencySprite = new ResourceSprite("EmergencyButton.png", 550f);
    public static RemoteProcess<PlayerControl> MayorRevealed = new("MayorRevealed", (player, _) =>
    {
        if (player.TryGetRole<Mayor>(out var mayor))
        {
            mayor.Revealed = true;
        }
    });
    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        mayorMeetingButtonOption = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mayorMeetingButton", false, roleinfo.RoleOption);
        mayorMaxRemoteMeetings = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mayorMaxRemoteMeetings", 1f, 1f, 5f, 1f, mayorMeetingButtonOption);
        mayorSabotageRemoteMeetings = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mayorSabotageRemoteMeetings", false, mayorMeetingButtonOption);
        mayorVote = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mayorVote", 2, 1, 4, 1, roleinfo.RoleOption);
        mayorRevealVision = CustomOption.Create(configId++, CustomOptionType.Crewmate, "mayorRevealVision", ["-20%", "-30%", "-40%", "-50%"], roleinfo.RoleOption);
    }

    public static bool GetRevealed(PlayerControl player)
    {
        if (player == null || player.GetRoleBase() is not Mayor mayor) return false;
        return mayor.Revealed;
    }

    public override void Initialize()
    {
        Revealed = false;
        Vote = mayorVote.GetInt();
        meetingButton = mayorMeetingButtonOption.GetBool();
        remoteMeetingsLeft = mayorMaxRemoteMeetings.GetInt();
        SabotageRemoteMeetings = mayorSabotageRemoteMeetings.GetBool();
        vision = mayorRevealVision.GetSelection() + 2;
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        if (Player.IsAlive() && PlayerControl.LocalPlayer == Player && !Revealed)
        {
            var meetingUI = UObject.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var textTemplate = __instance.playerStates[0].NameText;
            var meetingExtraButtonParent = new GameObject().transform;
            meetingExtraButtonParent.SetParent(meetingUI);
            var meetingExtraButton = UObject.Instantiate(buttonTemplate, meetingExtraButtonParent);
            MeetingExtraButton = meetingExtraButton.gameObject;

            var meetingExtraButtonMask = UObject.Instantiate(maskTemplate, meetingExtraButtonParent);
            meetingExtraButtonLabel = UObject.Instantiate(textTemplate, meetingExtraButton);
            meetingExtraButton.GetComponent<SpriteRenderer>().sprite =
                ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

            meetingExtraButtonParent.localPosition = new Vector3(0, -2.225f, -5);
            meetingExtraButtonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
            meetingExtraButtonLabel.alignment = TextAlignmentOptions.Center;
            meetingExtraButtonLabel.transform.localPosition =
                new Vector3(0, 0, meetingExtraButtonLabel.transform.localPosition.z);

            var localScale = meetingExtraButtonLabel.transform.localScale;
            localScale = new Vector3(
                localScale.x * 1.5f,
                localScale.x * 1.7f,
                localScale.x * 1.7f);
            meetingExtraButtonLabel.transform.localScale = localScale;
            meetingExtraButtonLabel.text = Cs(color, "揭示身份");

            var passiveButton = meetingExtraButton.GetComponent<PassiveButton>();
            passiveButton.OnClick.RemoveAllListeners();
            if (PlayerControl.LocalPlayer.IsAlive())
                passiveButton.OnClick.AddListener((Action)(() => mayorToggleVoteTwice(__instance)));

            meetingExtraButton.parent.gameObject.SetActive(false);
            __instance.StartCoroutine(Effects.Lerp(7.27f, new Action<float>(p =>
            {
                // Button appears delayed, so that its visible in the voting screen only!
                if ((int)p == 1) meetingExtraButton.parent.gameObject.SetActive(true);
            })));
        }
    }

    private static void mayorToggleVoteTwice(MeetingHud __instance)
    {
        __instance.playerStates[0].Cancel();
        if (__instance.state is VoteStates.Results or VoteStates.Discussion || PlayerControl.LocalPlayer.IsDead()) return;

        MayorRevealed.Invoke(PlayerControl.LocalPlayer);
        UObject.Destroy(MeetingExtraButton);
    }


    public override void CleanUp(HudManager __instance)
    {
        mayorMeetingButton?.Destroy();
        mayorMeetingButton = null;
    }


    public override void CreateButton(HudManager __instance)
    {
        mayorMeetingButton?.Destroy();
        mayorMeetingButton = new CustomButton(
            () =>
            {
                //PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
                remoteMeetingsLeft--;

                var writer = StartRPC(CustomRPC.NoCheckStartMeeting);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(byte.MaxValue);
                writer.Write(true);
                writer.EndRPC();
                PlayerControl.LocalPlayer.NoCheckStartMeeting(null, true);

                mayorMeetingButton.Timer = 1f;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && meetingButton;
            },
            () =>
            {
                mayorMeetingButton.actionButton.OverrideText(GetString("MayorButtonText") + "(" + remoteMeetingsLeft + ")");
                var sabotageActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    if ((task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor ||
                    task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles ||
                        (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)) && !SabotageRemoteMeetings)
                        sabotageActive = true;
                return !sabotageActive && PlayerControl.LocalPlayer.CanMove &&
                       remoteMeetingsLeft > 0;
            },
            () => { mayorMeetingButton.Timer = mayorMeetingButton.MaxTimer; },
            emergencySprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            0f,
            () => { },
            false,
            buttonText: GetString("MayorButtonText")
        )
        {
            MaxTimer = 0f,
        };
    }
}