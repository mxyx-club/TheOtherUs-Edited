namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Butcher : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Butcher),
        (p) => new Butcher(p),
        RoleId.Butcher,
        RoleType.Impostor,
        "Butcher",
        color,
        103100,
        AddOptions
    );

    public Butcher(PlayerControl p) : base(p, roleInfo) { }

    public PlayerControl dissected;
    public bool canDissection;

    public static float dissectionCooldown = 30f;
    public static float dissectionDuration = 10f;
    public static int dissectedBodyCount = 5;

    public static CustomOption butcherDissectionCooldown;
    public static CustomOption butcherDissectionDuration;
    public static CustomOption butcherDissectedBodyCount;

    public CustomButton butcherDissectionButton;
    public static Sprite ButtonSprite = new ResourceSprite("DissectedButton.png");
    public static RemoteProcess<(PlayerControl player, byte targetId)> DissectionBody = new("DissectionBody", (data, _) =>
    {
        var target = PlayerById(data.targetId);
        for (var num = 0; num < Butcher.dissectedBodyCount; num++)
        {
            target!.MyPhysics.StartCoroutine(target.KillAnimations.First().CoPerformKill(data.player, target));
        }
        data.player!.GetRole<Butcher>()!.dissected = target;

        var array = UObject.FindObjectsOfType<DeadBody>().Where(x => x.ParentId == data.targetId).ToList();

        var list = new List<Vector3>();
        list.AddRange(MapData.MapSpawnPosition(false));
        list.AddRange(MapData.FindVentSpawnPositions(false));

        for (var i = 1; i < array.Count; i++)
        {
            array[i].transform.position = list.Random();
        }
    });

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        butcherDissectionCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "butcherDissectionCooldown", 25f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        butcherDissectionDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "butcherDissectionDuration", 1f, 0f, 10f, 0.25f, roleInfo.RoleOption);
        butcherDissectedBodyCount = CustomOption.Create(configId++, CustomOptionType.Impostor, "butcherDissectedBodyCount", 5, 3, 15, 1, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        dissected = null;
        canDissection = true;
        dissectionCooldown = butcherDissectionCooldown.GetFloat();
        dissectionDuration = butcherDissectionDuration.GetFloat();
        dissectedBodyCount = butcherDissectedBodyCount.GetInt();
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        dissected = null;
        canDissection = true;
    }

    public override void CleanUp(HudManager __instance)
    {
        butcherDissectionButton?.Destroy();
        butcherDissectionButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Butcher Dissection
        butcherDissectionButton?.Destroy();
        butcherDissectionButton = new CustomButton(
            () => { },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && canDissection;
            },
            () =>
            {
                return __instance.ReportButton.graphic.color == Palette.EnabledColor &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { butcherDissectionButton.Timer = butcherDissectionButton.MaxTimer; },
            ButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            dissectionDuration,
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

                                DissectionBody.Invoke((PlayerControl.LocalPlayer, playerInfo.PlayerId));

                                canDissection = false;
                                SoundEffectsManager.play("cleanerClean");
                                break;
                            }
                        }
                    }
            },
            buttonText: GetString("DissectionText")
        )
        {
            MaxTimer = dissectionCooldown,
            EffectDuration = dissectionDuration,
        };
    }
}