namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Undertaker : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Undertaker),
        (p) => new Undertaker(p),
        RoleId.Undertaker,
        RoleType.Impostor,
        "Undertaker",
        color,
        101300,
        AddOptions
    );

    public Undertaker(PlayerControl p) : base(p, roleInfo) { }

    public DeadBody targetBody;
    public DeadBody dragedBody;

    public static float velocity = 1;
    public static bool canDragAndVent;
    public static float dragingDelaiAfterKill;

    public static CustomOption undertakerDragingDelaiAfterKill;
    public static CustomOption undertakerDragingAfterVelocity;
    public static CustomOption undertakerCanDragAndVent;

    public CustomButton undertakerDragButton;
    public static Sprite buttonSprite = new ResourceSprite("UndertakerDragButton.png");
    public static RemoteProcess<(PlayerControl player, byte targetId)> UndertakerDragAction = new("UndertakerDragAction", (data, _) =>
    // public static void DragBody(byte playerId, byte targetId)
    {
        if (!data.player.TryGetRole<Undertaker>(out var undertaker)) return;
        if (data.targetId == byte.MaxValue)
        {
            undertaker.dragedBody = null;
            return;
        }
        undertaker.dragedBody = GetDeadBody(data.targetId);
    });

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        undertakerDragingDelaiAfterKill = CustomOption.Create(configId++, CustomOptionType.Impostor, "undertakerDragingDelaiAfterKill", 0f, 0f, 15, 0.5f, roleInfo.RoleOption);
        undertakerDragingAfterVelocity = CustomOption.Create(configId++, CustomOptionType.Impostor, "undertakerDragingAfterVelocity", 0.75f, 0.5f, 1.5f, 0.125f, roleInfo.RoleOption);
        undertakerCanDragAndVent = CustomOption.Create(configId++, CustomOptionType.Impostor, "undertakerCanDragAndVent", true, roleInfo.RoleOption);
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

    public override void Initialize()
    {
        targetBody = null;
        dragedBody = null;
        canDragAndVent = undertakerCanDragAndVent.GetBool();
        velocity = undertakerDragingAfterVelocity.GetFloat();
        dragingDelaiAfterKill = undertakerDragingDelaiAfterKill.GetFloat();
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Player != null && PlayerControl.LocalPlayer == Player &&
            Info.Killer == Player && undertakerDragButton != null)
            undertakerDragButton.Timer = dragingDelaiAfterKill;
    }

    public override void CleanUp(HudManager __instance)
    {
        undertakerDragButton?.Destroy();
        undertakerDragButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        undertakerDragButton?.Destroy();
        undertakerDragButton = new CustomButton(
            () =>
            {
                if (dragedBody != null)
                {
                    UndertakerDragAction.Invoke((PlayerControl.LocalPlayer, byte.MaxValue));
                }
                else if (targetBody != null)
                {
                    UndertakerDragAction.Invoke((PlayerControl.LocalPlayer, targetBody.ParentId));
                }
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsAlive() && PlayerControl.LocalPlayer == Player;
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
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("DragBodyText")
        )
        {
            MaxTimer = 0f,
        };
    }
}
