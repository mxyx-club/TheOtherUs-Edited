namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Jester : RoleBase, INeutral
{
    public static Color color = new Color32(236, 98, 165, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Jester),
        (p) => new Jester(p),
        RoleId.Jester,
        RoleType.Neutral,
        "Jester",
        color,
        203100,
        AddOptions
    );

    public Jester(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Evil;
    public override bool? CanUseVent => jesterCanVent.GetBool();

    public bool triggerJesterWin;
    public DeadBody targetBody;
    public DeadBody dragedBody;

    public static bool canCallEmergency = true;
    public static bool hasImpostorVision;
    public static bool canDragDeadBody;
    public static float velocity;

    public static CustomOption jesterCanCallEmergency;
    public static CustomOption jesterCanVent;
    public static CustomOption jesterHasImpostorVision;
    public static CustomOption jesterCanDragDeadBody;
    public static CustomOption jesterDragingVelocity;

    public CustomButton JesterDragButton;

    public static RemoteProcess<(PlayerControl player, byte targetId)> DragBody = new("DragBody", (data, _) =>
    // public static void DragBody(byte playerId, byte targetId)
    {
        if (!data.player.TryGetRole<Jester>(out var jester)) return;

        if (data.targetId == byte.MaxValue)
        {
            jester.dragedBody = null;
            return;
        }
        jester.dragedBody = GetDeadBody(data.targetId);
    });

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        jesterCanCallEmergency = CustomOption.Create(configId++, CustomOptionType.Neutral, "canCallEmergency", true, roleinfo.RoleOption);
        jesterCanVent = CustomOption.Create(configId++, CustomOptionType.Neutral, "jesterCanVent", true, roleinfo.RoleOption);
        jesterHasImpostorVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "hasImpVision", true, roleinfo.RoleOption);
        jesterCanDragDeadBody = CustomOption.Create(configId++, CustomOptionType.Neutral, "jesterCanDragDeadBody", true, roleinfo.RoleOption);
        jesterDragingVelocity = CustomOption.Create(configId++, CustomOptionType.Neutral, "undertakerDragingAfterVelocity", 0.75f, 0.5f, 1.5f, 0.125f, jesterCanDragDeadBody);
    }

    public override void CleanUp(HudManager __instance)
    {
        JesterDragButton?.Destroy();
        JesterDragButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Jester Button
        JesterDragButton?.Destroy();
        JesterDragButton = new CustomButton(
            () =>
            {
                if (dragedBody != null)
                {
                    DragBody.Invoke((PlayerControl.LocalPlayer, byte.MaxValue));
                }
                else if (targetBody != null)
                {
                    DragBody.Invoke((PlayerControl.LocalPlayer, targetBody.ParentId));
                }
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsAlive() && PlayerControl.LocalPlayer == Player && canDragDeadBody;
            },
            () =>
            {
                var array = Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                      PlayerControl.LocalPlayer.MaxReportDistance * 0.5f, Constants.PlayersOnlyMask)
                 .Where(collider => collider.tag == "DeadBody")
                 .Select(collider => collider.GetComponent<DeadBody>())
                 .Where(deadBody => deadBody != null);

                targetBody = array.FirstOrDefault(db => db.ParentId != PlayerControl.LocalPlayer.PlayerId);

                return (targetBody || dragedBody) && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            Undertaker.buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("DragBodyText")
        )
        { MaxTimer = 0f };

    }

    public override void Initialize()
    {
        triggerJesterWin = false;
        dragedBody = null;
        targetBody = null;
        canCallEmergency = jesterCanCallEmergency.GetBool();
        hasImpostorVision = jesterHasImpostorVision.GetBool();
        canDragDeadBody = jesterCanDragDeadBody.GetBool();
        velocity = jesterDragingVelocity.GetFloat();
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (exiled != null && Player.PlayerId == exiled.PlayerId)
        {
            triggerJesterWin = true;
            return;
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        dragedBody = null;
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (Player.IsDead() || InMeeting) return;

        if (dragedBody != null)
        {
            dragedBody.transform.position = Player.transform.position;
        }
    }
}
