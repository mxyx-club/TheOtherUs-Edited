using Assets.CoreScripts;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Medic : RoleBase
{
    public static Color color = new Color32(126, 251, 194, byte.MaxValue);
    public static Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Medic),
        (p) => new Medic(p),
        RoleId.Medic,
        RoleType.Crewmate,
        "Medic",
        color,
        302000,
        AddOptions
    );

    public Medic(PlayerControl p) : base(p, roleinfo) { _AllMedic.Add(this); }

    private static List<Medic> _AllMedic = new();
    public static IEnumerable<Medic> AllMedic => _AllMedic;

    public PlayerControl currentTarget;
    public PlayerControl shielded; // 正在保护的玩家
    public PlayerControl futureShielded;  // 即将保护的玩家（一般在会议结束中生效的护盾使用）

    public bool usedShield;

    public static int showShielded;
    public static bool showAttemptToShielded;
    public static bool showAttemptToMedic;
    public static bool unbreakableShield = true;
    public static bool setShieldAfterMeeting;
    public static bool showShieldAfterMeeting;
    public static bool meetingAfterShielding;
    public static bool reset;
    public static float ReportNameDuration;
    public static float ReportColorDuration;

    public static CustomOption medicShowShielded;
    public static CustomOption medicBreakShield;
    public static CustomOption medicShowAttemptToMedic;
    public static CustomOption medicShowAttemptToShielded;
    public static CustomOption medicResetTargetAfterMeeting;
    public static CustomOption medicSetOrShowShieldAfterMeeting;
    public static CustomOption medicReportNameDuration;
    public static CustomOption medicReportColorDuration;

    public static CustomButton medicShieldButton;
    public static ResourceSprite buttonSprite = new("ShieldButton.png");
    public static RemoteProcess<(PlayerControl player, PlayerControl target, bool afterMeeting)> SetFutureShielded = new("SetFutureShielded", (data, _) =>
    {
        if (data.player != null && data.player?.TryGetRole<Medic>(out var medic) == true)
        {
            if (data.afterMeeting) medic!.futureShielded = PlayerById(data.target.PlayerId);
            else medic!.shielded = PlayerById(data.target.PlayerId);
            medic.usedShield = true;
        }
    });

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        medicShowShielded = CustomOption.Create(configId++, CustomOptionType.Crewmate, "medicShowShielded",
            ["medicShowShielded1", "medicShowShielded2", "medicShowShielded3"], roleinfo.RoleOption);
        medicBreakShield = CustomOption.Create(configId++, CustomOptionType.Crewmate, "medicBreakShield", true, roleinfo.RoleOption);
        medicShowAttemptToMedic = CustomOption.Create(configId++, CustomOptionType.Crewmate, "medicShowAttemptToMedic", true, medicBreakShield);
        medicShowAttemptToShielded = CustomOption.Create(configId++, CustomOptionType.Crewmate, "medicShowAttemptToShielded", false, medicBreakShield);
        medicResetTargetAfterMeeting = CustomOption.Create(configId++, CustomOptionType.Crewmate, "medicResetTargetAfterMeeting", false, roleinfo.RoleOption);
        medicSetOrShowShieldAfterMeeting = CustomOption.Create(configId++, CustomOptionType.Crewmate, "medicSetOrShowShieldAfterMeeting",
            ["medicSetOrShowShieldAfterMeeting1", "medicSetOrShowShieldAfterMeeting2", "medicSetOrShowShieldAfterMeeting3"], roleinfo.RoleOption);
        medicReportNameDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "medicReportNameDuration", 5f, 0f, 60f, 2.5f, medicBreakShield);
        medicReportColorDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "medicReportColorDuration", 30f, 0f, 120f, 2.5f, medicBreakShield);
    }


    public bool shieldVisible(PlayerControl target)
    {
        if (shielded == null) return false;

        bool isMorphedAsShielded = target.TryGetRole<Morphling>(out var morphling) && morphling.morphTarget == shielded && morphling.morphTimer > 0f;

        bool isDirectShielded = target == shielded && !(target.TryGetRole<Morphling>(out var morph) && morph.morphTimer > 0f);

        if (!isDirectShielded && !isMorphedAsShielded) return false;

        var hasVisibleShield = showShielded == 0 || CanSeeGhostInfo || (showShielded == 1 && (PlayerControl.LocalPlayer == shielded || PlayerControl.LocalPlayer == Player))
            || (showShielded == 2 && PlayerControl.LocalPlayer == Player);

        return hasVisibleShield && (meetingAfterShielding
            || !showShieldAfterMeeting
            || PlayerControl.LocalPlayer == Player
            || CanSeeGhostInfo);
    }

    public override void OnDestroy()
    {
        _AllMedic.Remove(this);
        base.OnDestroy();
    }

    public override void Initialize()
    {
        shielded = null;
        currentTarget = null;
        usedShield = false;
        meetingAfterShielding = false;
        reset = medicResetTargetAfterMeeting.GetBool();
        showShielded = medicShowShielded.GetSelection();
        showAttemptToShielded = medicShowAttemptToShielded.GetBool();
        unbreakableShield = medicBreakShield.GetBool();
        showAttemptToMedic = medicShowAttemptToMedic.GetBool();
        setShieldAfterMeeting = medicSetOrShowShieldAfterMeeting.GetSelection() == 2;
        showShieldAfterMeeting = medicSetOrShowShieldAfterMeeting.GetSelection() == 1;
        ReportNameDuration = medicReportNameDuration.GetFloat();
        ReportColorDuration = medicReportColorDuration.GetFloat();
    }

    /*public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        currentTarget = shielded = null;
        usedShield = false;
    }*/

    public override bool CheckMurderPlayer(MurderInfo Info)
    {
        if (shielded != null && Info.Target == shielded && Player.IsAlive())
        {
            if (CanSeeGhostInfo || (Player.AmOwner && showAttemptToMedic) || (shielded.AmOwner && showAttemptToShielded))
                showFlash(Color.red, 1f, "medicShowAttemptText".Translate());
            return false;
        }
        return true;
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        if (Player.IsDead() || Player != PlayerControl.LocalPlayer) return;

        if (futureShielded != null)
        {
            SetFutureShielded.Invoke((PlayerControl.LocalPlayer, currentTarget, false));
        }
        // Has to be after the setting of the shield
        if (usedShield) meetingAfterShielding = true;
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (shielded.IsDead() || Player.IsDead()) shielded = null;
    }

    public override void OnReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target)
    {
        if (reporter == Player && target != null)
        {
            var deadPlayer = PlayerData.AllPlayerData.Values.Where(x => x.Player?.PlayerId == target?.PlayerId && x.IsDead)?.FirstOrDefault();
            if (deadPlayer != null && deadPlayer.KilledBy != null)
            {
                var timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.DeathTimer).TotalMilliseconds;
                var msg = "";
                var killer = deadPlayer.KilledBy;
                float timer = (float)Math.Round(timeSinceDeath / 1000);

                if (Vortox.Reversal)
                {
                    timer += rnd.Next(-2, 3);
                    if (timer < 0) timer = 1;
                }

                if (timer <= ReportNameDuration)
                {
                    msg = $"尸检报告: 凶手似乎是 {killer.Data.PlayerName}!\n尸体在 {timer} 秒前死亡";
                }
                else if (timer <= ReportColorDuration)
                {
                    var typeOfColor = IsLightColor(killer) ? "浅" : "深";
                    msg = $"尸检报告: 凶手的颜色似乎是 {typeOfColor} 色的!\n尸体在{timer}秒前死亡";
                }
                else
                {
                    msg = $"尸检报告: 死亡时间太久，无法获取信息! \n尸体在{timer}秒前死亡";
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                    {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);

                        // Ghost Info
                        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ShareGhostInfo);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
                        writer.Write(msg);
                        writer.EndRPC();
                    }

                    if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
                        FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
                }
            }
        }
    }


    public override void CleanUp(HudManager __instance)
    {
        medicShieldButton?.Destroy();
        medicShieldButton = null;
    }


    public override void CreateButton(HudManager __instance)
    {
        // Medic Shield
        medicShieldButton?.Destroy();
        medicShieldButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;

                medicShieldButton.Timer = 0f;

                SetFutureShielded.Invoke((PlayerControl.LocalPlayer, currentTarget, setShieldAfterMeeting));
                meetingAfterShielding = false;

                SoundEffectsManager.play("medicShield");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                currentTarget = SetTarget();
                if (!usedShield)
                {
                    SetPlayerOutline(currentTarget, shieldedColor);
                    medicShieldButton.showTargetNameOnButton(currentTarget, GetString("ShieldText"));
                }
                return !usedShield && currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                // if (Medic.reset) Medic.resetShielded();
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("ShieldText")
        )
        {
            MaxTimer = 0f
        };
    }
}

