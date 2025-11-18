using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Trickster : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Trickster),
        (p) => new Trickster(p),
        RoleId.Trickster,
        RoleType.Impostor,
        "Trickster",
        color,
        102000,
        AddOptions,
        MaxPlayer: 1
    );

    public Trickster(PlayerControl p) : base(p, roleInfo) { }

    public static float placeBoxCooldown = 30f;
    public static float lightsOutCooldown = 30f;
    public static float lightsOutDuration = 10f;
    public static float lightsOutTimer;
    public static CustomOption tricksterPlaceBoxCooldown;
    public static CustomOption tricksterLightsOutCooldown;
    public static CustomOption tricksterLightsOutDuration;

    public CustomButton placeJackInTheBoxButton;
    public CustomButton lightsOutButton;
    public static Sprite placeBoxButtonSprite = new ResourceSprite("PlaceJackInTheBoxButton.png");
    public static Sprite lightOutButtonSprite = new ResourceSprite("LightsOutButton.png");
    public static Sprite tricksterVentButtonSprite = new ResourceSprite("TricksterVentButton.png");
    public static RemoteProcess<(PlayerControl player, Vector3 position)> PlaceJackInTheBox = new("PlaceJackInTheBox", (data, _) =>
    {
        new JackInTheBox(data.player, data.position);
    });
    public static RemoteProcess LightsOut = new("LightsOut", (_) =>
    {
        Trickster.lightsOutTimer = Trickster.lightsOutDuration;
        // If the local player is impostor indicate lights out
        if (HasImpVision(PlayerControl.LocalPlayer.Data))
            new CustomMessage("TricksterLightsOut".Translate(), Trickster.lightsOutDuration);
    });
    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        tricksterPlaceBoxCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "tricksterPlaceBoxCooldown", 20f, 2.5f, 30f, 2.5f, roleInfo.RoleOption);
        tricksterLightsOutCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "tricksterLightsOutCooldown", 25f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        tricksterLightsOutDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "tricksterLightsOutDuration", 12.5f, 5f, 60f, 0.5f, roleInfo.RoleOption);
    }
    public override void Initialize()
    {
        Player = null;
        lightsOutTimer = 0f;
        placeBoxCooldown = tricksterPlaceBoxCooldown.GetFloat();
        lightsOutCooldown = tricksterLightsOutCooldown.GetFloat();
        lightsOutDuration = tricksterLightsOutDuration.GetFloat();
        JackInTheBox.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        if (JackInTheBox.hasJackInTheBoxLimitReached()) JackInTheBox.convertToVents();
    }
    public override void CleanUp(HudManager __instance)
    {
        placeJackInTheBoxButton?.Destroy();
        lightsOutButton?.Destroy();
        placeJackInTheBoxButton = null;
        lightsOutButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        placeJackInTheBoxButton?.Destroy();
        placeJackInTheBoxButton = new CustomButton(
            () =>
            {
                placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;

                PlaceJackInTheBox.Invoke((PlayerControl.LocalPlayer, pos));
                SoundEffectsManager.play("tricksterPlaceBox");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && !JackInTheBox.hasJackInTheBoxLimitReached();
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && !JackInTheBox.hasJackInTheBoxLimitReached();
            },
            () => { placeJackInTheBoxButton.Timer = placeJackInTheBoxButton.MaxTimer; },
            placeBoxButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("TricksterPlaceText")
        )
        {
            MaxTimer = placeBoxCooldown
        };

        lightsOutButton?.Destroy();
        lightsOutButton = new CustomButton(
            () =>
            {
                LightsOut.Invoke();
                SoundEffectsManager.play("lighterLight");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive()
                       && JackInTheBox.hasJackInTheBoxLimitReached() && JackInTheBox.boxesConvertedToVents;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && JackInTheBox.hasJackInTheBoxLimitReached() &&
                       JackInTheBox.boxesConvertedToVents;
            },
            () =>
            {
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
                lightsOutButton.isEffectActive = false;
                lightsOutButton.actionButton.graphic.color = Palette.EnabledColor;
            },
            lightOutButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            lightsOutDuration,
            () =>
            {
                lightsOutButton.Timer = lightsOutButton.MaxTimer;
                SoundEffectsManager.play("lighterLight");
            },
            buttonText: GetString("LightsOutText")
        )
        {
            MaxTimer = lightsOutCooldown,
            EffectDuration = lightsOutDuration,
        };
    }
}
