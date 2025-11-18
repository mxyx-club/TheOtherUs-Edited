namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Cleaner : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Cleaner),
        (p) => new Cleaner(p),
        RoleId.Cleaner,
        RoleType.Impostor,
        "Cleaner",
        color,
        102100,
        AddOptions
    );

    public Cleaner(PlayerControl p) : base(p, roleInfo) { }

    public static float cooldown = 30f;

    public CustomButton cleanerCleanButton;
    public static Sprite buttonSprite = new ResourceSprite("CleanButton.png");
    public static CustomOption cleanerCooldown;
    public static RemoteProcess<byte> CleanBody = new("CleanBody", (targetId, _) =>
    {
        GetRoles(RoleId.Medium).Do((x) =>
        {
            var medium = x as Medium;
            var deadBody = medium!.futureDeadBodies.Find(x => x.Item1.Player.PlayerId == targetId)?.Item1;
            if (deadBody != null) deadBody.wasCleaned = true;

        });

        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == targetId)
            {
                UObject.Destroy(array[i].gameObject);
                break;
            }
        }
    });
    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        cleanerCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "cleanerCooldown", 25f, 10f, 60f, 2.5f, roleInfo.RoleOption);
    }
    public override void Initialize()
    {
        cooldown = cleanerCooldown.GetFloat();
    }
    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Player != null && PlayerControl.LocalPlayer == Player &&
            Info.Killer == Player && cleanerCleanButton != null)
            cleanerCleanButton.Timer = Player.killTimer;
    }

    public override void CleanUp(HudManager __instance)
    {
        cleanerCleanButton?.Destroy();
        cleanerCleanButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Cleaner Clean
        cleanerCleanButton?.Destroy();
        cleanerCleanButton = new CustomButton(
            () =>
            {
                foreach (var collider2D in Physics2D.OverlapCircleAll(
                             PlayerControl.LocalPlayer.GetTruePosition(),
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

                                CleanBody.Invoke(playerInfo.PlayerId);

                                Player.killTimer = cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer;
                                SoundEffectsManager.play("cleanerClean");
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
            () => { cleanerCleanButton.Timer = cleanerCleanButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("CleanText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}