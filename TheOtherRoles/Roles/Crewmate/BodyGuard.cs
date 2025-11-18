namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class BodyGuard : RoleBase
{
    public static Color color = new Color32(145, 102, 64, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(BodyGuard),
        (p) => new BodyGuard(p),
        RoleId.BodyGuard,
        RoleType.Crewmate,
        "BodyGuard",
        color,
        303400,
        AddOptions
    );

    public BodyGuard(PlayerControl p) : base(p, roleinfo) { }

    public PlayerControl guarded;
    public bool usedGuard;
    public PlayerControl currentTarget;

    public static bool reset = true;
    public static bool guardFlash;
    public static bool showShielded;

    public static CustomOption bodyGuardShowShielded;
    public static CustomOption bodyGuardFlash;
    public static CustomOption bodyGuardResetTargetAfterMeeting;

    public CustomButton bodyGuardGuardButton;
    public static Sprite guardButtonSprite = new ResourceSprite("Shield.png");


    public static RemoteProcess ShowBodyGuardFlash = new("ShowBodyGuardFlash", (_) =>
    {
        if (BodyGuard.bodyGuardFlash.GetBool()) showFlash(BodyGuard.color);
    });
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> BodyGuardGuardPlayer = new("BodyGuardGuardPlayer", (data, _) =>
    {
        if (data.player == null) return;

        if (data.player.TryGetRole<BodyGuard>(out var bodyGuard))
        {
            bodyGuard!.guarded = data.target;
        }
    });

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        bodyGuardShowShielded = CustomOption.Create(configId++, CustomOptionType.Crewmate, "bodyGuardShowShielded", true, roleinfo.RoleOption);
        bodyGuardFlash = CustomOption.Create(configId++, CustomOptionType.Crewmate, "bodyGuardFlash", true, roleinfo.RoleOption);
        bodyGuardResetTargetAfterMeeting = CustomOption.Create(configId++, CustomOptionType.Crewmate, "bodyGuardResetTargetAfterMeeting", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        showShielded = bodyGuardShowShielded.GetBool();
        guardFlash = bodyGuardFlash.GetBool();
        reset = bodyGuardResetTargetAfterMeeting.GetBool();
        guarded = null;
        usedGuard = false;
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        currentTarget = guarded = null;
        usedGuard = false;
    }

    public override bool CheckMurderPlayer(MurderInfo Info)
    {
        if (Info.Target == guarded && Player.AmOwner)
        {
            // Kill the Killer
            RpcCustomMurderPlayer(Player, Info.Killer, false, true);

            // Kill the BodyGuard
            RpcCustomMurderPlayer(Info.Killer, Player, false, true);

            ShowBodyGuardFlash.Invoke();
            return false;
        }
        return true;
    }

    public override void CleanUp(HudManager __instance)
    {
        bodyGuardGuardButton?.Destroy();
        bodyGuardGuardButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        bodyGuardGuardButton?.Destroy();
        bodyGuardGuardButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                BodyGuardGuardPlayer.Invoke((Player, currentTarget));
                // SoundEffectsManager.play("trackerTrackPlayer");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                currentTarget = SetTarget();
                if (!usedGuard)
                {
                    SetPlayerOutline(currentTarget, color);
                    bodyGuardGuardButton.showTargetNameOnButton(currentTarget, GetString("bodyGuardText"));
                }
                return PlayerControl.LocalPlayer.CanMove && currentTarget != null && !usedGuard;
            },
            () => { },
            guardButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("bodyGuardText")
        )
        {
            MaxTimer = 0f
        };

    }
}
