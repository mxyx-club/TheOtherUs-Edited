using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Amnisiac : RoleBase, INeutral
{
    public static Color color = new(0.5f, 0.7f, 1f, 1f);

    public static RoleInfo roleinfo = new(
        typeof(Amnisiac),
        (p) => new Amnisiac(p),
        RoleId.Amnisiac,
        RoleType.Neutral,
        "Amnisiac",
        color,
        200100,
        AddOptions
    );

    public Amnisiac(PlayerControl player) : base(player, roleinfo, true) { }

    public NeutralType NeutralType => NeutralType.Benign;

    public List<Arrow> localArrows = new();

    public static bool showArrows = true;
    public static bool resetRole;
    public static CustomOption amnisiacShowArrows;
    public static CustomOption amnisiacResetRole;

    public static ResourceSprite buttonSprite = new("Remember.png");
    public CustomButton amnisiacRememberButton;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        amnisiacShowArrows = CustomOption.Create(configId++, CustomOptionType.Neutral, "amnisiacShowArrows", true, roleinfo.RoleOption);
        amnisiacResetRole = CustomOption.Create(configId++, CustomOptionType.Neutral, "amnisiacResetRole", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        showArrows = amnisiacShowArrows.GetBool();
        resetRole = amnisiacResetRole.GetBool();
        foreach (var arrow in localArrows)
            if (arrow?.arrow != null) UObject.Destroy(arrow.arrow);
        localArrows.Clear();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (localArrows == null || !showArrows || InMeeting) return;

        if (Player.Data.IsDead)
        {
            foreach (var arrow in localArrows)
                UObject.Destroy(arrow.arrow);
            localArrows.Clear();
        }

        if (Player.IsAlive())
        {
            DeadBody[] deadBodies = UObject.FindObjectsOfType<DeadBody>();
            bool arrowUpdate = localArrows.Count != deadBodies.Length;
            int index = 0;

            if (arrowUpdate)
            {
                foreach (var arrow in localArrows)
                    UObject.Destroy(arrow.arrow);

                localArrows.Clear();
            }

            foreach (var db in deadBodies)
            {
                if (arrowUpdate)
                {
                    localArrows.Add(new Arrow(color));
                    localArrows[index].arrow.SetActive(true);
                }

                localArrows[index]?.Update(db.transform.position);
                index++;
            }
        }
    }
    public static RemoteProcess<(byte playerId, PlayerControl target)> AmnisiacTakeRole = new("AmnisiacTakeRole", (data, _) =>
    // public static void TakeRole(byte playerId, byte targetId)
    {
        var player = PlayerById(data.playerId);
        if (data.target == null || player == null) return;

        if (data.target.IsImpostor()) turnToImpostor(player);
        var newRole = data.target.GetRole();

        player.GetRoleBase()?.Destroy();
        CustomRoleManager.CreateRole(player, newRole);
    });

    public override void CleanUp(HudManager __instance)
    {
        amnisiacRememberButton?.Destroy();
        amnisiacRememberButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        amnisiacRememberButton?.Destroy();
        amnisiacRememberButton = new CustomButton(
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(),
                             PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    if (collider2D.tag == "DeadBody")
                    {
                        var component = collider2D.GetComponent<DeadBody>();
                        if (component && !component.Reported)
                        {
                            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                            var truePosition2 = component.TruePosition;
                            if (Vector2.Distance(truePosition2, truePosition) <=
                                PlayerControl.LocalPlayer.MaxReportDistance &&
                                PlayerControl.LocalPlayer.CanMove &&
                                !PhysicsHelpers.AnythingBetween(truePosition, truePosition2,
                                    Constants.ShipAndObjectsMask, false))
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                AmnisiacTakeRole.Invoke((playerInfo.PlayerId, PlayerControl.LocalPlayer));
                                break;
                            }
                        }
                    }
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                return __instance.ReportButton.graphic.color == Palette.EnabledColor &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { amnisiacRememberButton.Timer = 0f; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("RememberText")
        )
        {
            MaxTimer = 0f,
        };
    }
}
