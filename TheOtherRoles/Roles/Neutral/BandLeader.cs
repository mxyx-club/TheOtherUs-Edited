namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class BandLeader : RoleBase, INeutral
{
    public static Color color = new Color32(255, 192, 203, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(BandLeader),
        (p) => new BandLeader(p),
        RoleId.BandLeader,
        RoleType.Neutral,
        "BandLeader",
        color,
        200500,
        AddOptions
    );

    public BandLeader(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => winnerFlags is WinnerFlags.Crewmate or WinnerFlags.None ? NeutralType.Benign : NeutralType.Evil;
    public override bool? IsKiller => winnerFlags == WinnerFlags.Impostor;
    public override bool? CanUseVent => winnerFlags == WinnerFlags.Impostor;
    public override bool? HasImpVision => winnerFlags == WinnerFlags.Impostor;
    public PlayerControl[] Members => new[] { Keyboardist, Bassist, Drummer }.Where(x => x != null).ToArray();

    public PlayerControl Keyboardist;
    public PlayerControl Bassist;
    public PlayerControl Drummer;
    public PlayerControl currentTarget;

    public WinnerFlags winnerFlags = WinnerFlags.None;
    public bool Formed;

    public static float killCooldown;
    public static float createCoolDown;
    public static CustomOption bandLeaderKillCooldown;
    public static CustomOption bandLeaderCreateCooldown;

    public static Sprite keyboardButton = new ResourceSprite("BandLeader.Keyboard.png");
    public static Sprite keyboardDel = new ResourceSprite("BandLeader.KeyboardDel.png");
    public static Sprite bassButton = new ResourceSprite("BandLeader.Guitar.png");
    public static Sprite bassDel = new ResourceSprite("BandLeader.GuitarDel.png");
    public static Sprite drumButton = new ResourceSprite("BandLeader.Drum.png");
    public static Sprite drumDel = new ResourceSprite("BandLeader.DrumDel.png");

    public CustomButton bandLeaderKeyboardistButton;
    public CustomButton bandLeaderBassistButton;
    public CustomButton bandLeaderDrummerButton;
    public CustomButton bandLeaderKillButton;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        bandLeaderCreateCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "bandLeaderCreateDistance", 5f, 2.5f, 30f, 0.5f, roleinfo.RoleOption);
        bandLeaderKillCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "bandLeaderKillCooldown", 20f, 2.5f, 60f, 2.5f, roleinfo.RoleOption);
    }

    public static RemoteProcess<(PlayerControl player, byte targetId, int role)> CreateBandMember = new("CreateBandMember", (data, _) =>
    // public static void CreateBandMember(byte playerId, byte targetId, int role)
    {
        Message($"playerId: {data.targetId}, role: {data.role}");
        var target = PlayerById(data.targetId);
        if (data.targetId == byte.MaxValue) target = null;
        if (!(data.player?.TryGetRole<BandLeader>(out var bandLeader) == true)) return;
        switch (data.role)
        {
            case 1:
                bandLeader.Keyboardist = target;
                Message($"Keyboardist: {bandLeader.Keyboardist?.Data?.PlayerName ?? "null"}");
                break;
            case 2:
                bandLeader.Bassist = target;
                Message($"Bassist: {bandLeader.Bassist?.Data?.PlayerName ?? "null"}");
                break;
            case 3:
                bandLeader.Drummer = target;
                Message($"Drummer: {bandLeader.Drummer?.Data?.PlayerName ?? "null"}");
                break;
            default:
                break;
        }
        bandLeader.bandLeaderKeyboardistButton.Timer = bandLeader.bandLeaderKeyboardistButton.MaxTimer = createCoolDown;
        bandLeader.bandLeaderBassistButton.Timer = bandLeader.bandLeaderBassistButton.MaxTimer = createCoolDown;
        bandLeader.bandLeaderDrummerButton.Timer = bandLeader.bandLeaderDrummerButton.MaxTimer = createCoolDown;
    });
    public static RemoteProcess<(PlayerControl player, WinnerFlags winnerFlags)> BandLeaderFormed = new("BandLeaderFormed", (data, _) =>
    {
        var role = data.player.GetRole<BandLeader>();
        role.winnerFlags = data.winnerFlags;
    });

    public override void Initialize()
    {
        Keyboardist = null;
        Bassist = null;
        Drummer = null;
        Formed = false;
        winnerFlags = WinnerFlags.None;
        killCooldown = bandLeaderKillCooldown.GetFloat();
        createCoolDown = bandLeaderCreateCooldown.GetFloat();
    }

    public override bool SeeRoleTag(PlayerControl seen, PlayerControl seer, out string tag)
    {
        var suffix1 = Cs(color, "(K)");
        var suffix2 = Cs(color, "(B)");
        var suffix3 = Cs(color, "(D)");

        if ((seen == Player || seen == Keyboardist || (Formed && Members.Contains(seen)) || CanSeeGhostInfo) && seer == Keyboardist)
        {
            tag = suffix1;
            return true;
        }
        else if ((seen == Player || seen == Bassist || (Formed && Members.Contains(seen)) || CanSeeGhostInfo) && seer == Bassist)
        {
            tag = suffix2;
            return true;
        }
        else if ((seen == Player || seen == Drummer || (Formed && Members.Contains(seen)) || CanSeeGhostInfo) && seer == Drummer)
        {
            tag = suffix3;
            return true;
        }

        tag = string.Empty;
        return false;

    }

    public static void RpcFormed(byte playerId, byte state)
    {
        var player = PlayerById(playerId);
        if (player != null && player.TryGetRole<BandLeader>(out var bandLeader))
        {

            bandLeader.Formed = true;
            bandLeader.winnerFlags = (WinnerFlags)state;
            Message($"Band Leader Formed {(WinnerFlags)state}");
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        if (Player.IsAlive() && !Formed && PlayerControl.LocalPlayer == Player)
        {
            var (allNeutral, allCrew, allImpostor) = (Members.All(x => x.IsNeutral()), Members.All(x => x.IsCrew()), Members.All(x => x.IsImpostor()));

            if (Members.Length == 3 && (allNeutral || allCrew || allImpostor))
            {
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Player, "BandLeader.formed".Translate());
                Formed = true;
                if (allCrew) winnerFlags = WinnerFlags.Crewmate;
                if (allImpostor) winnerFlags = WinnerFlags.Impostor;
                if (allNeutral) winnerFlags = WinnerFlags.Neutral;
                BandLeaderFormed.Invoke((PlayerControl.LocalPlayer, winnerFlags));
            }
            else if (Members.Length == 3)
            {
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Player, "BandLeader.bad".Translate());
            }
        }
    }

    public override string UpdateMeetingVoteText(MeetingHud __instance)
    {
        var meetingInfoText = "";
        if (Formed) meetingInfoText += string.Format(GetString("BandLeaderFormed"), $"{$"{winnerFlags}Team".Translate()}");
        else meetingInfoText += GetString("BandLeaderBad");
        return meetingInfoText;
    }

    public override void CleanUp(HudManager __instance)
    {
        bandLeaderKeyboardistButton?.Destroy();
        bandLeaderBassistButton?.Destroy();
        bandLeaderDrummerButton?.Destroy();
        bandLeaderKillButton?.Destroy();
        bandLeaderKeyboardistButton = null;
        bandLeaderBassistButton = null;
        bandLeaderDrummerButton = null;
        bandLeaderKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        bandLeaderKeyboardistButton?.Destroy();
        bandLeaderKeyboardistButton = new CustomButton(
            () =>
            {
                if (Keyboardist == null)
                {
                    var target = currentTarget;
                    if (target == null || Members.Any(x => x.PlayerId == target?.PlayerId)) return;
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;
                    CreateBandMember.Invoke((PlayerControl.LocalPlayer, target.PlayerId, 1));
                }
                else
                {
                    CreateBandMember.Invoke((PlayerControl.LocalPlayer, byte.MaxValue, 1));
                }

                bandLeaderKeyboardistButton.Sprite = Keyboardist == null ? keyboardButton : keyboardDel;

                bandLeaderKeyboardistButton.buttonText = (Keyboardist == null ? "bandJoinButton" : "bandKickButton").Translate();

                bandLeaderKeyboardistButton.Timer = bandLeaderKeyboardistButton.MaxTimer = createCoolDown;
                bandLeaderBassistButton.Timer = bandLeaderBassistButton.MaxTimer = createCoolDown;
                bandLeaderDrummerButton.Timer = bandLeaderDrummerButton.MaxTimer = createCoolDown;
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer && !Formed;
            },
            () =>
            {
                currentTarget = SetTarget(Members);
                SetPlayerOutline(currentTarget, color);

                if (bandLeaderKeyboardistButton.ButtonTitle != null) bandLeaderKeyboardistButton.ButtonTitle.text = $"{Keyboardist?.Data?.PlayerName ?? ""}";
                return PlayerControl.LocalPlayer.CanMove && Keyboardist == null
                    ? currentTarget : true;
            },
            () =>
            {
                bandLeaderKeyboardistButton.Sprite = Keyboardist == null ? keyboardButton : keyboardDel;

                bandLeaderKeyboardistButton.Timer = bandLeaderKeyboardistButton.MaxTimer = createCoolDown;
            },
            keyboardButton,
            __instance,
            __instance.AbilityButton,
            null,
            buttonText: "bandJoinButton".Translate()
        )
        {
            MaxTimer = 0f,
        };

        bandLeaderBassistButton?.Destroy();
        bandLeaderBassistButton = new CustomButton(
            () =>
            {
                if (Bassist == null)
                {
                    var target = currentTarget;
                    if (Members.Any(x => x.PlayerId == target?.PlayerId) || target == null) return;
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;
                    CreateBandMember.Invoke((PlayerControl.LocalPlayer, target.PlayerId, 2));
                }
                else
                {
                    CreateBandMember.Invoke((PlayerControl.LocalPlayer, byte.MaxValue, 2));
                }

                bandLeaderBassistButton.Sprite = Bassist == null ? bassButton : bassDel;

                bandLeaderBassistButton.buttonText = (Keyboardist == null ? "bandJoinButton" : "bandKickButton").Translate();

                bandLeaderKeyboardistButton.Timer = bandLeaderKeyboardistButton.MaxTimer = createCoolDown;
                bandLeaderBassistButton.Timer = bandLeaderBassistButton.MaxTimer = createCoolDown;
                bandLeaderDrummerButton.Timer = bandLeaderDrummerButton.MaxTimer = createCoolDown;
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer && !Formed;
            },
            () =>
            {
                if (bandLeaderBassistButton.ButtonTitle != null) bandLeaderBassistButton.ButtonTitle.text = $"{Bassist?.Data?.PlayerName ?? ""}";

                return PlayerControl.LocalPlayer.CanMove && Bassist == null
                    ? currentTarget : true;
            },
            () =>
            {
                bandLeaderBassistButton.Sprite = Bassist == null ? bassButton : bassDel;

                bandLeaderBassistButton.Timer = bandLeaderBassistButton.MaxTimer = createCoolDown;
            },
            bassButton,
            __instance,
            __instance.AbilityButton,
            null,
            buttonText: "bandJoinButton".Translate()
        )
        {
            MaxTimer = 0f,
        };

        bandLeaderDrummerButton?.Destroy();
        bandLeaderDrummerButton = new CustomButton(
            () =>
            {
                if (Drummer == null)
                {
                    var target = currentTarget;
                    if (Members.Any(x => x.PlayerId == target?.PlayerId) || target == null) return;
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;
                    CreateBandMember.Invoke((PlayerControl.LocalPlayer, target.PlayerId, 3));
                }
                else
                {
                    CreateBandMember.Invoke((PlayerControl.LocalPlayer, byte.MaxValue, 3));
                }

                bandLeaderDrummerButton.Sprite = Drummer == null ? drumButton : drumDel;

                bandLeaderDrummerButton.buttonText = (Keyboardist == null ? "bandJoinButton" : "bandKickButton").Translate();

                bandLeaderKeyboardistButton.Timer = bandLeaderKeyboardistButton.MaxTimer = createCoolDown;
                bandLeaderBassistButton.Timer = bandLeaderBassistButton.MaxTimer = createCoolDown;
                bandLeaderDrummerButton.Timer = bandLeaderDrummerButton.MaxTimer = createCoolDown;
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer && !Formed;
            },
            () =>
            {
                if (bandLeaderDrummerButton.ButtonTitle != null) bandLeaderDrummerButton.ButtonTitle.text = $"{Drummer?.Data?.PlayerName ?? ""}";

                return PlayerControl.LocalPlayer.CanMove && Drummer == null
                    ? currentTarget : true;
            },
            () =>
            {
                bandLeaderDrummerButton.Sprite = Drummer == null ? drumButton : drumDel;

                bandLeaderDrummerButton.Timer = bandLeaderDrummerButton.MaxTimer = createCoolDown;
            },
            drumButton,
            __instance,
            __instance.AbilityButton,
            null,
            buttonText: "bandJoinButton".Translate()
        )
        {
            MaxTimer = 0f,
        };

        bandLeaderKillButton?.Destroy();
        bandLeaderKillButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                RpcCustomMurderPlayer(Player, currentTarget, true, false, CustomDeathReason.Kill);

                bandLeaderKillButton.Timer = bandLeaderKillButton.MaxTimer;
                currentTarget = null;
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer && Formed
                       && winnerFlags == WinnerFlags.Impostor;
            },
            () =>
            {
                currentTarget = SetTarget(Members);
                SetPlayerOutline(currentTarget, color);

                bandLeaderKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));

                return PlayerControl.LocalPlayer.CanMove && currentTarget != null;
            },
            () =>
            {
                bandLeaderKillButton.Timer = bandLeaderKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        )
        {
            MaxTimer = killCooldown,
        };
    }

    public enum WinnerFlags
    {
        None,
        Crewmate,
        Neutral,
        Impostor
    }
}
