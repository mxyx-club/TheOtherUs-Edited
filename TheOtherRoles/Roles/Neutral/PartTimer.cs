namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class PartTimer : RoleBase, INeutral
{
    public static Color color = new Color32(0, 255, 0, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(PartTimer),
        (p) => new PartTimer(p),
        RoleId.PartTimer,
        RoleType.Neutral,
        "PartTimer",
        color,
        200400,
        AddOptions
    );

    public PartTimer(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Benign;

    public PlayerControl currentTarget;
    public PlayerControl target;

    public static float cooldown;
    public static int DeathDefaultTurn;
    public int deathTurn;
    public static bool knowsRole;

    public static CustomOption partTimerCooldown;
    public static CustomOption partTimerDeathTurn;
    public static CustomOption partTimerKnowsRole;

    public CustomButton partTimerButton;
    public static ResourceSprite buttonSprite = new("PartTimerButton.png");
    public static RemoteProcess<(PlayerControl player, byte targetId)> PartTimerSet = new("PartTimerSet", (data, _) =>
    {
        if (data.player == null) return;

        if (data.player.TryGetRole<PartTimer>(out var partTimer))
        {
            if (data.targetId == byte.MaxValue)
            {
                partTimer.target = null;
                return;
            }
            var target = PlayerById(data.targetId);
            partTimer.target = target;
            partTimer.deathTurn = PartTimer.DeathDefaultTurn;
        }
    });

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        partTimerCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "partTimerCooldown", 20f, 2.5f, 60f, 2.5f, roleinfo.RoleOption);
        partTimerDeathTurn = CustomOption.Create(configId++, CustomOptionType.Neutral, "partTimerDeathTurn", 2, 1, 6, 1, roleinfo.RoleOption);
        partTimerKnowsRole = CustomOption.Create(configId++, CustomOptionType.Neutral, "partTimerIsCheckTargetRole", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        currentTarget = null;
        target = null;
        cooldown = partTimerCooldown.GetFloat();
        deathTurn = DeathDefaultTurn = partTimerDeathTurn.GetInt();
        knowsRole = partTimerKnowsRole.GetBool();
    }

    public override bool SeeRoleName(PlayerControl seen, PlayerControl seer)
    {
        if ((seen == Player && target == seer) || (seen == target && Player == seer))
        {
            return true;
        }
        return false;
    }

    public override bool SeeRoleTag(PlayerControl seen, PlayerControl seer, out string tag)
    {
        if ((seen == Player && target == seer) || (seen == target && Player == seer) || CanSeeGhostInfo)
        {
            tag = " ★";
            return true;
        }
        tag = string.Empty;
        return false;
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        if (Player.IsAlive() && target == null) deathTurn--;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (Player.IsDead()) return;

        if (target != null && target.IsDead())
        {
            var playerInfoTransform = target?.cosmetics.nameText.transform.parent.FindChild("Info");
            var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();
            if (playerInfo != null) playerInfo.text = "";

            PartTimerSet.Invoke((Player, byte.MaxValue));
        }
    }

    public override string UpdateMeetingVoteText(MeetingHud __instance)
    {
        var meetingInfoText = string.Format(GetString("PartTimerMeetingInfo"), deathTurn);
        return meetingInfoText;
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        if (Player.IsAlive())
        {
            if (deathTurn <= 0 && target == null) Player.Exiled();
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        partTimerButton?.Destroy();
        partTimerButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        partTimerButton?.Destroy();
        partTimerButton = new CustomButton(
            () =>
            {
                var target = currentTarget;
                if (target == null) return;
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;

                PartTimerSet.Invoke((PlayerControl.LocalPlayer, target.PlayerId));
                SoundEffectsManager.play("jackalSidekick");

                partTimerButton.Timer = partTimerButton.MaxTimer;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && target == null;
            },
            () =>
            {
                currentTarget = SetTarget();
                if (target != null) SetPlayerOutline(currentTarget, color);

                partTimerButton.showTargetNameOnButton(currentTarget, GetString("partTimerButton"));
                return PlayerControl.LocalPlayer.CanMove && currentTarget != null; ;
            },
            () => { partTimerButton.Timer = partTimerButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("partTimerButton")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
