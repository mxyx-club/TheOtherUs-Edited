namespace TheOtherRoles.Roles.Modifier;

[CustomRpcHolder]
public class Disperser : ModifierBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleinfo = new(
        typeof(Disperser),
        (p) => new Disperser(p),
        RoleId.Disperser,
        "Disperser",
        color,
        401000,
        AddOptions
    );

    public int remainingDisperses;
    public static bool DispersesToVent;
    public static CustomOption modifierDisperserDispersesToVent;

    public CustomButton disperserDisperseButton;
    public static ResourceSprite buttonSprite = new("Disperse.png");

    public Disperser(PlayerControl p) : base(p, roleinfo) { }
    public static RemoteProcess Disperse = new("Disperse", (_) =>
    {
        Coroutines.Start(showFlashCoroutine(Palette.ImpostorRed, 1f, 0.36f));

        if (PlayerControl.LocalPlayer.inVent)
        {
            PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
            PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
        }

        if (Minigame.Instance) Minigame.Instance.ForceClose();
        if (MapBehaviour.Instance) MapBehaviour.Instance.Close();

        if (PlayerControl.LocalPlayer.inVent)
        {
            PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
            PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
        }

        if (PlayerControl.LocalPlayer.IsAlive() && !PlayerControl.LocalPlayer.Is(RoleId.AntiTeleport))
        {
            if (Disperser.DispersesToVent)
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo
                (MapData.FindVentSpawnPositions()[rnd.Next(MapData.FindVentSpawnPositions().Count)]);
            }
            else
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo
                (MapData.MapSpawnPosition()[rnd.Next(MapData.MapSpawnPosition().Count)]);
            }
        }
    });


    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierDisperserDispersesToVent = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierDisperserDispersesToVent", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        remainingDisperses = 1;
        DispersesToVent = modifierDisperserDispersesToVent.GetBool();
    }

    public override void CleanUp(HudManager __instance)
    {
        disperserDisperseButton?.Destroy();
        disperserDisperseButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Disperser disperse
        disperserDisperseButton?.Destroy();
        disperserDisperseButton = new CustomButton(
            () =>
            {
                remainingDisperses--;
                Disperse.Invoke();
                SoundEffectsManager.play("shifterShift");
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer && remainingDisperses > 0;
            },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () => { if (remainingDisperses > 0) disperserDisperseButton.Timer = disperserDisperseButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.modifierAbilityInput.keyCode,
            true,
            buttonText: GetString("DisperseText")
        )
        { MaxTimer = 0f };
    }
}
