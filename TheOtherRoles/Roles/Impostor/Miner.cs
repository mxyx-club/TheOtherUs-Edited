namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Miner : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Miner),
        (p) => new Miner(p),
        RoleId.Miner,
        RoleType.Impostor,
        "Miner",
        color,
        102800,
        AddOptions
    );

    public Miner(PlayerControl p) : base(p, roleInfo) { }

    public static readonly List<Vent> Vents = new();
    public static DateTime LastMined;

    public static float cooldown = 30f;
    public KillButton _mineButton;
    public static CustomOption minerCooldown;
    public bool CanPlace;
    public static Vector2 VentSize;

    public CustomButton minerMineButton;
    public static Sprite buttonSprite = new ResourceSprite("Mine.png");
    public static RemoteProcess<(int ventId, Vector3 position, float zAxis)> Mine = new("Mine", (data, _) =>
    {
        var ventPrefab = UObject.FindObjectOfType<Vent>();
        var vent = UObject.Instantiate(ventPrefab, ventPrefab.transform.parent);
        vent.Id = data.ventId;
        vent.transform.position = new Vector3(data.position.x, data.position.y, data.zAxis);

        if (Miner.Vents.Count > 0)
        {
            var leftVent = Miner.Vents[^1];
            vent.Left = leftVent;
            leftVent.Right = vent;
        }
        else
        {
            vent.Left = null;
        }

        vent.Right = null;
        vent.Center = null;
        var allVents = ShipStatus.Instance.AllVents.ToList();
        allVents.Add(vent);
        ShipStatus.Instance.AllVents = allVents.ToArray();
        Miner.Vents.Add(vent);
        Miner.LastMined = DateTime.UtcNow;

        if (SubmergedCompatibility.IsSubmerged)
        {
            vent.gameObject.layer = 12;
            // just in case elevator vent is not blocked
            vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            if (vent.gameObject.transform.position.y > -7)
            {
                vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x,
                    vent.gameObject.transform.position.y, 0.03f);
            }
            else
            {
                vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x,
                    vent.gameObject.transform.position.y, 0.0009f);
                vent.gameObject.transform.localPosition = new Vector3(vent.gameObject.transform.localPosition.x,
                    vent.gameObject.transform.localPosition.y, -0.003f);
            }
        }
    });

    public static void AddOptions()
    {
        minerCooldown = CustomOption.Create(roleInfo.ConfigId + 3, CustomOptionType.Impostor, "minerCooldown", 20f, 10f, 60f, 2.5f, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        cooldown = minerCooldown.GetFloat();
    }

    public override void CleanUp(HudManager __instance)
    {
        minerMineButton?.Destroy();
        minerMineButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        minerMineButton?.Destroy();
        minerMineButton = new CustomButton(
            () =>
            {
                // On Use
                minerMineButton.Timer = minerMineButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;
                var id = getAvailableId();
                Mine.Invoke((id, pos, 0.01f));
            },
            () =>
            {
                // Can See
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                // Can Use
                var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, VentSize, 0);
                hits = hits.ToArray().Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
                return hits.Count == 0 && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                // On Meeting End
                minerMineButton.Timer = minerMineButton.MaxTimer;
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("minerText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
