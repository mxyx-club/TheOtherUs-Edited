using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Yoyo : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Yoyo),
        (p) => new Yoyo(p),
        RoleId.Yoyo,
        RoleType.Impostor,
        "Yoyo",
        color,
        102900,
        AddOptions
    );
    public Yoyo(PlayerControl p) : base(p, roleInfo) { }

    public Vector3? markedLocation;
    public static float SilhouetteVisibility => silhouetteVisibility == 0 && (PlayerControl.LocalPlayer.Is(RoleId.Yoyo) || CanSeeGhostInfo) ? 0.1f : silhouetteVisibility;
    public static float silhouetteVisibility;

    public static float blinkDuration;
    public static float markCooldown;
    public static bool markStaysOverMeeting;
    public static bool hasAdminTable;
    public static float adminCooldown;
    public static CustomOption yoyoMarkCooldown;
    public static CustomOption yoyoBlinkDuration;
    public static CustomOption yoyoMarkStaysOverMeeting;
    public static CustomOption yoyoHasAdminTable;
    public static CustomOption yoyoAdminTableCooldown;
    public static CustomOption yoyoSilhouetteVisibility;

    public CustomButton yoyoButton;
    public CustomButton yoyoAdminTableButton;
    public static Sprite markButtonSprite = new ResourceSprite("YoyoMarkButtonSprite.png");
    public static Sprite blinkButtonSprite = new ResourceSprite("YoyoBlinkButtonSprite.png");
    public static RemoteProcess<(PlayerControl player, Vector3 position)> YoyoMarkLocation = new("YoyoMarkLocation", (data, _) =>
    // public static void yoyoMarkLocation(byte playerId, Vector3 pos)
    {

        if (data.player.TryGetRole<Yoyo>(out var yoyo))
        {
            yoyo.markedLocation = data.position;
        }

        var silhouette = new Silhouette(data.player, data.position, -1, false);
    });
    public static RemoteProcess<(PlayerControl player, bool isFirstJump, Vector3 position)> YoyoBlink = new("YoyoBlink", (data, _) =>
    // public static void yoyoBlink(byte playerId, bool isFirstJump, byte[] buff)
    {
        Message($"blink fistjumpo: {data.isFirstJump}");
        if (data.player != null && data.player.TryGetRole<Yoyo>(out var yoyo))
        {
            if (yoyo.markedLocation == null) return;
            var markedPos = (Vector3)yoyo.markedLocation;
            yoyo.Player.NetTransform.SnapTo(markedPos);

            var markedSilhouette = Silhouette.AllObjects.FirstOrDefault(s => s.GameObject!.transform.position.x == markedPos.x && s.GameObject.transform.position.y == markedPos.y);
            if (markedSilhouette != null)
                markedSilhouette.permanent = false;

            // Create Silhoutte At Start Position:
            if (data.isFirstJump)
            {
                yoyo.markedLocation = data.position;
                new Silhouette(data.player, data.position, Yoyo.blinkDuration, true);
            }
            else
            {
                new Silhouette(data.player, data.position, 5, true);
                yoyo.markedLocation = null;
            }
        }
    });

    private static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        yoyoMarkCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "yoyoMarkCooldown", 15f, 2.5f, 120f, 2.5f, roleInfo.RoleOption);
        yoyoBlinkDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "yoyoBlinkDuration", 15f, 2.5f, 120f, 2.5f, roleInfo.RoleOption);
        yoyoMarkStaysOverMeeting = CustomOption.Create(configId++, CustomOptionType.Impostor, "yoyoMarkStaysOverMeeting", false, roleInfo.RoleOption);
        yoyoHasAdminTable = CustomOption.Create(configId++, CustomOptionType.Impostor, "yoyoHasAdminTable", true, roleInfo.RoleOption);
        yoyoAdminTableCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "yoyoAdminTableCooldown", 15f, 2.5f, 120f, 2.5f, yoyoHasAdminTable);
        yoyoSilhouetteVisibility = CustomOption.Create(configId++, CustomOptionType.Impostor, "yoyoSilhouetteVisibility", ["0%", "10%", "20%", "30%", "40%", "50%"], roleInfo.RoleOption);
    }

    public static void markLocation(Vector3 position)
    {
    }

    public override void Initialize()
    {
        blinkDuration = yoyoBlinkDuration.GetFloat();
        markCooldown = yoyoMarkCooldown.GetFloat();
        markStaysOverMeeting = yoyoMarkStaysOverMeeting.GetBool();
        hasAdminTable = yoyoHasAdminTable.GetBool();
        adminCooldown = yoyoAdminTableCooldown.GetFloat();
        silhouetteVisibility = yoyoSilhouetteVisibility.GetSelection() / 10f;
        markedLocation = null;

    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        CustomObject.UpdateAll();
    }

    public override void CleanUp(HudManager __instance)
    {
        yoyoButton?.Destroy();
        yoyoAdminTableButton?.Destroy();
        yoyoButton = null;
        yoyoAdminTableButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Yoyo button
        yoyoButton?.Destroy();
        yoyoButton = new CustomButton(
            () =>
            {
                var pos = PlayerControl.LocalPlayer.transform.position;

                if (markedLocation == null)
                {
                    Message($"marked location is null in button press");
                    YoyoMarkLocation.Invoke((PlayerControl.LocalPlayer, pos));
                    SoundEffectsManager.play("tricksterPlaceBox");
                    yoyoButton.Sprite = blinkButtonSprite;
                    yoyoButton.Timer = 10f;
                    yoyoButton.HasEffect = false;
                    yoyoButton.buttonText = "BlinkText".Translate();
                }
                else
                {
                    Message("in else for some reason");
                    // Jump to location
                    Message($"trying to blink!");
                    var exit = (Vector3)markedLocation;
                    if (SubmergedCompatibility.IsSubmerged)
                        SubmergedCompatibility.ChangeFloor(exit.y > -7);
                    YoyoBlink.Invoke((PlayerControl.LocalPlayer, true, pos));
                    yoyoButton.EffectDuration = blinkDuration;
                    yoyoButton.Timer = 10f;
                    yoyoButton.HasEffect = true;
                    yoyoButton.buttonText = "ReturningText".Translate();
                    SoundEffectsManager.play("morphlingMorph");
                }
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                if (markStaysOverMeeting)
                    yoyoButton.Timer = 10f;
                else
                {
                    markedLocation = null;
                    yoyoButton.Timer = yoyoButton.MaxTimer;
                    yoyoButton.Sprite = markButtonSprite;
                    yoyoButton.buttonText = "YoyoMarkText".Translate();
                }
            },
            markButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            false,
            blinkDuration,
            () =>
            {
                if (Player.isUsingTransportation())
                {
                    yoyoButton.Timer = 0.5f;
                    yoyoButton.DeputyTimer = 0.5f;
                    yoyoButton.isEffectActive = true;
                    yoyoButton.actionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
                    return;
                }
                else if (Player.inVent)
                {
                    __instance.ImpostorVentButton.DoClick();
                }
                // jump back!
                var pos = PlayerControl.LocalPlayer.transform.position;
                var exit = (Vector3)markedLocation;
                if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(exit.y > -7);
                YoyoBlink.Invoke((PlayerControl.LocalPlayer, false, pos));
                yoyoButton.Timer = yoyoButton.MaxTimer;
                yoyoButton.isEffectActive = false;
                yoyoButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                yoyoButton.HasEffect = false;
                yoyoButton.Sprite = markButtonSprite;
                yoyoButton.buttonText = "YoyoMarkText".Translate();
                SoundEffectsManager.play("morphlingMorph");
                if (Minigame.Instance)
                    Minigame.Instance.Close();
            },
            buttonText: "YoyoMarkText".Translate()
        )
        {
            MaxTimer = markCooldown,
        };

        yoyoButton?.Destroy();
        yoyoAdminTableButton = new CustomButton(
           () =>
           {
               if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
               {
                   var __instance = FastDestroyableSingleton<HudManager>.Instance;
                   __instance.InitMap();
                   MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
               }
           },
           () =>
           {
               return Player == PlayerControl.LocalPlayer && Player.IsAlive() && hasAdminTable && !PlayerControl.LocalPlayer.Data.IsDead;
           },
           () =>
           {
               return true;
           },
           () =>
           {
               yoyoAdminTableButton.Timer = yoyoAdminTableButton.MaxTimer;
               yoyoAdminTableButton.isEffectActive = false;
               yoyoAdminTableButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
           },
           Hacker.getAdminSprite(),
           __instance,
            __instance.AbilityButton,
           ModInputManager.secondaryAbilityInput.keyCode,
           true,
           0f,
           null,
           () =>
           {
               if (!MapBehaviour.Instance || !MapBehaviour.Instance.isActiveAndEnabled)
               {
                   var __instance = FastDestroyableSingleton<HudManager>.Instance;
                   __instance.InitMap();
                   MapBehaviour.Instance.ShowCountOverlay(allowedToMove: true, showLivePlayerPosition: true, includeDeadBodies: true);
               }
               ;
           },
           () =>
           {
               yoyoAdminTableButton.Timer = yoyoAdminTableButton.MaxTimer;
               if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
           },
           GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3,
           "AdminMapText".Translate()
       )
        {
            MaxTimer = adminCooldown,
            EffectDuration = 10f,
        };
    }
}