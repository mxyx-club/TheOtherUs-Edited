using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Marionette : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public Decoy decoy;
    public int marionetteMode;

    public float PlaceCooldown;
    public float SwapCooldown;
    public int ShowDecoy;
    public bool MonitoringCanMove;

    public static CustomOption marionettePlaceCooldown;
    public static CustomOption marionetteDecoyDelayedDisplay;
    public static CustomOption marionetteDecoyPermanent;
    public static CustomOption marionetteResetPlaceAfterMeeting;
    public static CustomOption marionetteDecoyDuration;
    public static CustomOption marionetteSwapCooldown;
    public static CustomOption marionetteShowDecoy;
    public static CustomOption marionetteMonitoringCanMove;

    public static Sprite decoyButtonSprite = new ResourceSprite("DecoyButton.png", 115f);
    public static Sprite swapButtonSprite = new ResourceSprite("DecoySwapButton.png", 115f);
    public static Sprite destroyButtonSprite = new ResourceSprite("DecoyDestroyButton.png", 115f);
    public static Sprite monitorButtonSprite = new ResourceSprite("DecoyMonitorButton.png", 115f);

    public CustomButton marionetteButton;
    public CustomButton marionetteCameraButton;
    public CustomButton marionettePlaceButton;


    public Marionette(PlayerControl p) : base(p, roleInfo) { }

    public static RoleInfo roleInfo = new(
        typeof(Marionette),
        (p) => new Marionette(p),
        RoleId.Marionette,
        RoleType.Impostor,
        "Marionette",
        color,
        101800,
        AddOptions
    );

    public static RemoteProcess<(PlayerControl player, Vector3 position)> PlaceDecoy = new("PlaceDecoy", (data, _) =>
    {
        var marionette = data.player.GetRole<Marionette>();
        if (marionette == null) return;
        marionette.decoy = new Decoy(data.player, data.position);
    });

    public static RemoteProcess<(PlayerControl player, int? decoyId, Vector3 playerPos, Vector3 decoyPos)> DecoySwap = new("DecoySwap", (data, _) =>
    {
        var decoy = Decoy.AllObjects.FirstOrDefault(x => x.Id == data.decoyId);
        if (decoy?.GameObject == null || decoy?.Renderer == null) return;

        bool playerFlip = data.player.cosmetics.FlipX;
        bool decoyFlip = decoy.Renderer.flipX;

        data.player.NetTransform.SnapTo(data.decoyPos);
        decoy.GameObject.transform.position = data.playerPos;

        data.player.cosmetics.SetFlipX(decoyFlip);
        decoy.Renderer.flipX = playerFlip;
    });

    public static RemoteProcess<(PlayerControl player, int? decoyId)> DecoyDestroy = new("DecoyDestroy", (data, _) =>
    {
        var marionette = data.player.GetRole<Marionette>();
        if (marionette == null) return;
        var decoy = Decoy.AllObjects.FirstOrDefault(x => x.Id == data.decoyId);
        decoy?.Destroy();
        marionette.decoy = null;
    });

    public void SetMarionetteMode(int mode)
    {
        marionetteMode = mode;
        if (marionetteMode == 0)
        {
            marionetteButton.Sprite = swapButtonSprite;
            marionetteButton.buttonText = GetString("swapButtonText");
        }
        else
        {
            marionetteButton.Sprite = destroyButtonSprite;
            marionetteButton.buttonText = GetString("destroyButtonText");
        }
    }

    public static void AddOptions()
    {
        marionettePlaceCooldown = CustomOption.Create(101801, CustomOptionType.Impostor, "marionettePlaceCooldown", 12.5f, 5f, 30f, 2.5f, roleInfo.RoleOption);
        marionetteDecoyDelayedDisplay = CustomOption.Create(101802, CustomOptionType.Impostor, "marionetteDecoyDelayedDisplay", 12.5f, 5f, 30f, 2.5f, roleInfo.RoleOption);
        marionetteDecoyPermanent = CustomOption.Create(101803, CustomOptionType.Impostor, "marionetteDecoyPermanent", false, roleInfo.RoleOption);
        marionetteResetPlaceAfterMeeting = CustomOption.Create(101804, CustomOptionType.Impostor, "marionetteResetPlaceAfterMeeting", false, marionetteDecoyPermanent);
        marionetteDecoyDuration = CustomOption.Create(101805, CustomOptionType.Impostor, "marionetteDecoyDuration", 60f, 25f, 120f, 5f, roleInfo.RoleOption,
            isHidden: () => marionetteDecoyPermanent.GetBool());
        marionetteSwapCooldown = CustomOption.Create(101806, CustomOptionType.Impostor, "marionetteSwapCooldown", 10f, 5f, 45f, 2.5f, roleInfo.RoleOption);
        marionetteShowDecoy = CustomOption.Create(101807, CustomOptionType.Impostor, "marionetteShowDecoy", ["marionetteShowDecoy1", "marionetteShowDecoy2", "marionetteShowDecoy3"], roleInfo.RoleOption);
        marionetteMonitoringCanMove = CustomOption.Create(101808, CustomOptionType.Impostor, "marionetteMonitoringCanMove", true, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        PlaceCooldown = marionettePlaceCooldown.GetFloat();
        SwapCooldown = marionetteSwapCooldown.GetFloat();
        ShowDecoy = marionetteShowDecoy.GetQuantity();
        MonitoringCanMove = marionetteMonitoringCanMove.GetBool();

        marionetteMode = 0;
        decoy = null;
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        if ((Decoy.ResetPlaceAfterMeeting && Decoy.DecoyPermanent) || !Decoy.DecoyPermanent)
        {
            decoy?.Destroy();
            decoy = null;
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        marionetteButton?.Destroy();
        marionetteButton = null;
        marionetteCameraButton?.Destroy();
        marionetteCameraButton = null;
        marionettePlaceButton?.Destroy();
        marionettePlaceButton = null;
        decoy = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        marionettePlaceButton?.Destroy();
        marionettePlaceButton = new(
            () =>
            {
                PlaceDecoy.Invoke((PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position));
                marionettePlaceButton.Timer = marionettePlaceButton.MaxTimer;

                marionetteMode = 0;

                SetMarionetteMode(0);
                marionetteButton.Timer = marionetteButton.MaxTimer;
            },
            () =>
            {
                return Player.IsAlive() && decoy == null;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove || HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer;
            },
            () =>
            {
                marionettePlaceButton.Timer = marionettePlaceButton.MaxTimer = PlaceCooldown;
            },
            decoyButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("decoyButtonText")
        );

        marionetteButton?.Destroy();
        marionetteButton = new(
            () =>
            {
                if (marionetteMode == 0)
                {
                    if (HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer) HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);

                    DecoySwap.Invoke((PlayerControl.LocalPlayer, decoy.Id, PlayerControl.LocalPlayer.transform.position, decoy.GameObject.transform.position));
                }
                else
                {

                    DecoyDestroy.Invoke((PlayerControl.LocalPlayer, decoy.Id));
                    marionettePlaceButton.Timer = marionettePlaceButton.MaxTimer;
                }
                marionetteButton.Timer = marionetteButton.MaxTimer;
            },
            () =>
            {
                return Player.IsAlive() && decoy != null; ;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove || HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer; ;
            },
            () =>
            {
                marionetteButton.Timer = marionetteButton.MaxTimer;
                if (!Decoy.DecoyPermanent) SetMarionetteMode(0);
            },
            decoyButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("swapButtonText")
        );
        marionetteButton.SetAidAction(ModInputManager.changeAbilityInput.keyCode, true, () => { SetMarionetteMode((marionetteMode + 1) % 2); });

        marionetteCameraButton?.Destroy();
        marionetteCameraButton = new(
            () =>
            {
                if (HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer)
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                    if (!MonitoringCanMove) PlayerControl.LocalPlayer.moveable = true;
                }
                else
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(decoy.Behaviour);
                    PlayerControl.LocalPlayer.NetTransform.Halt();
                    if (!MonitoringCanMove) PlayerControl.LocalPlayer.moveable = false;
                }
            },
            () =>
            {
                return Player.IsAlive() && decoy != null;
            },
            () =>
            {
                if (marionetteCameraButton.ButtonTitle != null && decoy != null)
                {
                    marionetteCameraButton.ButtonTitle.text = Decoy.DecoyPermanent
                    ? $"持续存在{(Decoy.ResetPlaceAfterMeeting ? "|会议重置" : "")}"
                    : (Decoy.DecoyDuration - decoy.elapsedTime).ToString("00");
                }

                return PlayerControl.LocalPlayer.CanMove || HudManager.Instance.PlayerCam.Target != PlayerControl.LocalPlayer;
            },
            () =>
            {
                marionetteCameraButton.Timer = 0f;
            },
            monitorButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.secondaryAbilityInput.keyCode,
            buttonText: GetString("monitorButtonText")
        );

    }
}
