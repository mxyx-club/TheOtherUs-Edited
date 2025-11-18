using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Vulture : RoleBase, INeutral
{
    public static Color color = new Color32(139, 69, 19, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Vulture),
        (p) => new Vulture(p),
        RoleId.Vulture,
        RoleType.Neutral,
        "Vulture",
        color,
        203200,
        AddOptions
    );

    public Vulture(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Evil;
    public override bool? CanUseVent => vultureCanUseVents.GetBool();

    public static PlayerControl vulture;
    public static List<Arrow> localArrows = new();
    public static float cooldown = 30f;
    public static int numberToWin = 4;
    public static int eatenBodies;
    public static bool triggerVultureWin;
    public static bool showArrows = true;

    public static CustomOption vultureCooldown;
    public static CustomOption vultureNumberToWin;
    public static CustomOption vultureCanUseVents;
    public static CustomOption vultureShowArrows;

    public CustomButton vultureEatButton;
    public static ResourceSprite buttonSprite = new("VultureButton.png");
    public static RemoteProcess<(PlayerControl player, byte targetId)> CleanBody = new("CleanBody", (data, _) =>
    {
        GetRoles(RoleId.Medium).Do((x) =>
        {
            var medium = x as Medium;
            var deadBody = medium!.futureDeadBodies.Find(x => x.Item1.Player.PlayerId == data.targetId)?.Item1;
            if (deadBody != null) deadBody.wasCleaned = true;

        });

        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == data.targetId)
            {
                UObject.Destroy(array[i].gameObject);
                break;
            }
        }
        if (Vulture.vulture != null && data.player.PlayerId == Vulture.vulture.PlayerId)
        {
            Vulture.eatenBodies++;
            if (Vulture.eatenBodies == Vulture.numberToWin) Vulture.triggerVultureWin = true;
        }
    });

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        vultureCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "vultureCooldown", 12.5f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        vultureNumberToWin = CustomOption.Create(configId++, CustomOptionType.Neutral, "vultureNumberToWin", 3f, 1f, 15f, 1f, roleinfo.RoleOption);
        vultureCanUseVents = CustomOption.Create(configId++, CustomOptionType.Neutral, "canUseVents", true, roleinfo.RoleOption);
        vultureShowArrows = CustomOption.Create(configId++, CustomOptionType.Neutral, "vultureShowArrows", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        vulture = null;
        numberToWin = vultureNumberToWin.GetInt();
        eatenBodies = 0;
        cooldown = vultureCooldown.GetFloat();
        triggerVultureWin = false;
        showArrows = vultureShowArrows.GetBool();
        if (localArrows != null)
            foreach (var arrow in localArrows)
                if (arrow?.arrow != null)
                    UObject.Destroy(arrow.arrow);
        localArrows.Clear();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (vulture == null || player != vulture ||
            localArrows == null || !showArrows) return;
        if (vulture.Data.IsDead)
        {
            foreach (var arrow in localArrows) UObject.Destroy(arrow.arrow);
            localArrows = new();
            return;
        }

        DeadBody[] deadBodies = UObject.FindObjectsOfType<DeadBody>();
        var arrowUpdate = localArrows.Count != deadBodies.Length;
        var index = 0;

        if (arrowUpdate)
        {
            foreach (var arrow in localArrows) UObject.Destroy(arrow.arrow);
            localArrows = new();
        }

        foreach (var db in deadBodies)
        {
            if (arrowUpdate)
            {
                localArrows.Add(new Arrow(Color.blue));
                localArrows[index].arrow.SetActive(true);
            }

            if (localArrows[index] != null) localArrows[index].Update(db.transform.position);
            index++;
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        vultureEatButton?.Destroy();
        vultureEatButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Vulture Eat
        vultureEatButton?.Destroy();
        vultureEatButton = new CustomButton(
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

                                CleanBody.Invoke((vulture, playerInfo.PlayerId));

                                cooldown = vultureEatButton.Timer = vultureEatButton.MaxTimer;
                                SoundEffectsManager.play("vultureEat");
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
            () => { vultureEatButton.Timer = vultureEatButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("VultureText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
