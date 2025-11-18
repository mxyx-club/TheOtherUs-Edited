namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Witness : RoleBase, INeutral
{
    public static Color color = new Color32(123, 170, 255, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Witness),
        (p) => new Witness(p),
        RoleId.Witness,
        RoleType.Neutral,
        "Witness",
        color,
        203800,
        AddOptions,
        MaxPlayer: 1
    );

    public Witness(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Evil;

    public PlayerControl Target;
    public PlayerControl KillerTarget;
    public bool triggerWitnessWin;

    public static int markTimer;
    public static int exileToWin;
    public static bool meetingDie;
    public static bool skipMeeting;

    public static CustomOption witnessMarkTimer;
    public static CustomOption witnessWinCount;
    public static CustomOption witnessMeetingDie;
    public static CustomOption witnessSkipMeeting;

    public static int exiledCount;
    public static DateTime startTime;
    public static float timeLeft;
    public static bool endTime;

    public static Sprite TargetSprite = new ResourceSprite("TargetIcon.png", 150);

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        witnessMarkTimer = CustomOption.Create(configId++, CustomOptionType.Neutral, "witnessMarkTimer", 30, 20, 90, 5, roleinfo.RoleOption);
        witnessWinCount = CustomOption.Create(configId++, CustomOptionType.Neutral, "witnessWinCount", 2, 1, 6, 1, roleinfo.RoleOption);
        witnessMeetingDie = CustomOption.Create(configId++, CustomOptionType.Neutral, "witnessMeetingDie", true, roleinfo.RoleOption);
        witnessSkipMeeting = CustomOption.Create(configId++, CustomOptionType.Neutral, "witnessSkipMeeting", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        Target = null;
        KillerTarget = null;
        endTime = false;
        triggerWitnessWin = false;
        exiledCount = 0;
        markTimer = witnessMarkTimer.GetInt() + 8;
        exileToWin = witnessWinCount.GetInt();
        meetingDie = witnessMeetingDie.GetBool();
        skipMeeting = witnessSkipMeeting.GetBool();
    }

    public static RemoteProcess<(PlayerControl player, byte targetId)> WitnessReport = new("WitnessReport", (data, _) =>
    // internal static void RpcWitnessReport(byte playerId, byte targetId)
    {
        var target = PlayerById(data.targetId);

        if (data.player.TryGetRole<Witness>(out var witness) && data.player.IsAlive())
        {

            witness.KillerTarget = DetermineKillerTarget(target);

            static PlayerControl DetermineKillerTarget(PlayerControl target)
            {
                if (target == null)
                    return PlayerData.GetLastKiller();

                var deadPlayer = PlayerData.AllPlayerData.Values
                    .Where(dp => dp.IsDead && dp.Player?.PlayerId == target.PlayerId && dp.KilledBy != null && dp.KilledBy.IsAlive())
                    ?.OrderByDescending(dp => dp.DeathTimer)
                    ?.FirstOrDefault();

                return deadPlayer?.KilledBy ?? PlayerData.GetLastKiller();
            }
        }
    });

    public static RemoteProcess<(PlayerControl player, PlayerControl target)> WitnessSetTarget = new("WitnessSetTarget", (data, _) =>
    // public static void RpcSetTarget(byte playerId, byte targetId)
    {
        if (data.player.TryGetRole<Witness>(out var witness))
        {
            witness.Target = data.target;
        }
    });

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (Player.IsDead() && !InMeeting) return;

        if (MeetingHud.Instance)
        {
            if (Target != null)
            {
                setInfo(Target.PlayerId, Cs(Color.red, $"{Target?.Data?.PlayerName} 疑似为本案的凶手"));
            }
            else if ((player == Player || ModOption.DebugMode) && KillerTarget != null)
            {
                setInfo(KillerTarget.PlayerId, Cs(Color.red, $"{KillerTarget?.Data?.PlayerName} 为本案的真凶"));
            }
        }

        void setInfo(int targetPlayerId, string infoText)
        {
            var pva = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == targetPlayerId);
            if (pva == null) return;

            var meetingInfoTransform = pva.NameText.transform.parent.FindChild("WitnessInfo");
            var meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TextMeshPro>() : null;

            if (meetingInfo == null)
            {
                meetingInfo = UObject.Instantiate(pva.NameText, pva.NameText.transform.parent);
                meetingInfo.transform.localPosition += Vector3.up * 0.2f;
                meetingInfo.fontSize *= 0.72f;
                meetingInfo.gameObject.name = "WitnessInfo";
            }

            if (meetingInfo != null)
            {
                meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : infoText;
            }
        }
    }

    public override void OnReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target)
    {
        if (Player.IsAlive())
        {
            WitnessReport.Invoke((PlayerControl.LocalPlayer, target?.PlayerId ?? byte.MaxValue));
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        if (Player.IsAlive() && KillerTarget == null) WitnessReport.Invoke((PlayerControl.LocalPlayer, byte.MaxValue));

        foreach (var playerState in __instance?.playerStates ?? Enumerable.Empty<PlayerVoteArea>())
        {
            var meetingInfoTransform = playerState.NameText.transform.parent.Find("WitnessInfo");
            if (meetingInfoTransform != null)
            {
                UObject.Destroy(meetingInfoTransform.gameObject);
            }
        }

        if (PlayerControl.LocalPlayer == Player && Player.IsAlive())
        {
            foreach (var pva in __instance.playerStates)
            {
                var player = PlayerById(pva.TargetPlayerId);
                if (player.IsAlive())
                {
                    GameObject template = pva.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject targetBox = UObject.Instantiate(template, pva.transform);
                    targetBox.name = "WitnessIcon";
                    targetBox.transform.localPosition = new Vector3(1f, 0.03f, -1f);
                    SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                    renderer.sprite = TargetSprite;
                    renderer.color = Color.red;
                    PassiveButton button = targetBox.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    button.OnClick.AddListener(() => MeetingOnClick(pva, __instance));
                }
            }
            endTime = false;
            startTime = DateTime.UtcNow;
        }
    }

    public override string UpdateMeetingVoteText(MeetingHud __instance)
    {
        var meetingInfoText = "";
        if (timeLeft > 0 && KillerTarget == null)
            meetingInfoText += string.Format(GetString("WitnessTimerLeft2"), $"{TimeSpan.FromSeconds(timeLeft):mm\\:ss}");
        else if (timeLeft > 0 && Target == null)
            meetingInfoText += string.Format(GetString("WitnessTimerLeft"), $"{TimeSpan.FromSeconds(timeLeft):mm\\:ss}");
        else
            meetingInfoText += string.Format(GetString("WitnessWinLeft"), $"{exileToWin - exiledCount}");
        return meetingInfoText;
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (Target != null)
        {
            bool skip = exiled == null && skipMeeting;
            bool targetIsKillerAndNotExiled = Target == KillerTarget && (exiled?.Object == null || Target != exiled?.Object);
            bool targetIsExiledAndNotKiller = Target != KillerTarget && (Target == exiled?.Object ||
                                              (meetingDie && Target.IsDead()));

            if ((!skip && targetIsKillerAndNotExiled) || targetIsExiledAndNotKiller)
            {
                exiledCount++;
            }

            if (exiledCount == exileToWin)
            {
                triggerWitnessWin = true;
            }
        }
        Target = KillerTarget = null;
    }

    private void MeetingOnClick(PlayerVoteArea pva, MeetingHud __instance)
    {
        if (Player == null) return;
        var target = PlayerById(pva.TargetPlayerId);

        if (target.IsDead()) return;
        WitnessSetTarget.Invoke((PlayerControl.LocalPlayer, target));
        Target = target;

        foreach (var playerState in __instance.playerStates)
        {
            var icon = playerState.transform.FindChild("WitnessIcon");
            if (icon != null) UObject.Destroy(icon.gameObject);
        }
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (Player.IsDead() || endTime || Target != null || !Player.AmOwner || !InMeeting) return;
        timeLeft = markTimer - (float)(DateTime.UtcNow - startTime).TotalSeconds;
        if (timeLeft <= 0)
        {
            foreach (var ps in MeetingHud.Instance?.playerStates)
            {
                var icon = ps.transform.FindChild("WitnessIcon");
                if (icon != null) UObject.Destroy(icon.gameObject);
            }
            endTime = true;
        }
    }
}
