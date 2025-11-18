// 延期适配
using System.Text;
using TheOtherRoles.Objects;
using static TheOtherRoles.Modules.ModInputManager;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Portalmaker : RoleBase
{
    public static Color color = new Color32(69, 69, 169, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Portalmaker),
        (p) => new Portalmaker(p),
        RoleId.Portalmaker,
        RoleType.Crewmate,
        "Portalmaker",
        color,
        302900,
        AddOptions,
        MaxPlayer: 1
    );

    public Portalmaker(PlayerControl p) : base(p, roleinfo) { }

    public static float cooldown;
    public static float usePortalCooldown;
    public static bool logOnlyHasColors;
    public static bool logShowsTime;
    public static bool canPortalFromAnywhere;

    public static ResourceSprite placePortalButtonSprite = new("PlacePortalButton.png");
    public static ResourceSprite usePortalButtonSprite = new("UsePortalButton.png");

    public static CustomOption portalmakerCooldown;
    public static CustomOption portalmakerUsePortalCooldown;
    public static CustomOption portalmakerLogOnlyColorType;
    public static CustomOption portalmakerLogHasTime;
    public static CustomOption portalmakerCanPortalFromAnywhere;


    public static CustomButton portalmakerPlacePortalButton;
    public static CustomButton portalmakerMoveToPortalButton;
    public static RemoteProcess<Vector3> PlacePortal = new("PlacePortal", (position, _) =>
    {
        var portal = new Portal(position);
    });
    public static RemoteProcess<(PlayerControl player, byte exit)> UsePortal = new("UsePortal", (data, _) =>
    {
        Portal.startTeleport(data.player.PlayerId, data.exit);
    });
    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        portalmakerCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "portalmakerCooldown", 15f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        portalmakerUsePortalCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "portalmakerUsePortalCooldown", 15f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        portalmakerLogOnlyColorType = CustomOption.Create(configId++, CustomOptionType.Crewmate, "portalmakerLogOnlyColorType", true, roleinfo.RoleOption);
        portalmakerLogHasTime = CustomOption.Create(configId++, CustomOptionType.Crewmate, "portalmakerLogHasTime", true, roleinfo.RoleOption);
        portalmakerCanPortalFromAnywhere = CustomOption.Create(configId++, CustomOptionType.Crewmate, "portalmakerCanPortalFromAnywhere", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        cooldown = portalmakerCooldown.GetFloat();
        usePortalCooldown = portalmakerUsePortalCooldown.GetFloat();
        logOnlyHasColors = portalmakerLogOnlyColorType.GetBool();
        logShowsTime = portalmakerLogHasTime.GetBool();
        canPortalFromAnywhere = portalmakerCanPortalFromAnywhere.GetBool();
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        Portal.meetingEndsUpdate();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        // Add Portal info into Portalmaker Chat:
        if ((PlayerControl.LocalPlayer == Player || CanSeeGhostInfo) && Player.IsAlive() && Portal.teleportedPlayers.Count > 0)
        {
            var msg = new StringBuilder(GetString("Portalmaker.LogHeader"));

            foreach (var entry in Portal.teleportedPlayers)
            {
                var timeBeforeMeeting = (float)(DateTime.UtcNow - entry.time).TotalMilliseconds / 1000;

                if (logShowsTime)
                {
                    msg.AppendFormat(GetString("Portalmaker.LogEntry"),
                                  timeBeforeMeeting,
                                  entry.name);
                }
                else
                {
                    msg.AppendFormat(GetString("Portalmaker.LogEntryNoTime"),
                                  entry.name);
                }
            }

            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Player, $"{msg}");
        }

    }

    public override void CleanUp(HudManager __instance)
    {
        portalmakerPlacePortalButton.Destroy();
        portalmakerMoveToPortalButton.Destroy();
        portalmakerPlacePortalButton = null;
        portalmakerMoveToPortalButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        portalmakerPlacePortalButton.Destroy();
        portalmakerPlacePortalButton = new CustomButton(
            () =>
            {
                portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;

                PlacePortal.Invoke(pos);
                SoundEffectsManager.play("tricksterPlaceBox");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() &&
                       !PlayerControl.LocalPlayer.Data.IsDead && Portal.secondPortal == null;
            },
            () => { return PlayerControl.LocalPlayer.CanMove && Portal.secondPortal == null; },
            () => { portalmakerPlacePortalButton.Timer = portalmakerPlacePortalButton.MaxTimer; },
            placePortalButtonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: GetString("PlacePortalText")
        )
        {
            MaxTimer = cooldown
        };

        portalmakerMoveToPortalButton.Destroy();
        portalmakerMoveToPortalButton = new CustomButton(
            () =>
            {
                var didTeleport = false;
                var exit = Portal.secondPortal.portalGameObject.transform.position;

                if (!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    // // Ghosts can portal too, but non-blocking and only with a local animation
                    // var writer = StartRPC(CustomRPC.UsePortal);
                    // writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    // writer.Write((byte)2);
                    // writer.EndRPC();
                    UsePortal.Invoke((PlayerControl.LocalPlayer, 2));
                }

                // RPCProcedure.usePortal(PlayerControl.LocalPlayer.PlayerId, 2);
                UsePortal.Invoke((PlayerControl.LocalPlayer, 2));
                HudManagerStartPatch.usePortalButton.Timer = HudManagerStartPatch.usePortalButton.MaxTimer;
                portalmakerMoveToPortalButton.Timer = HudManagerStartPatch.usePortalButton.MaxTimer;
                SoundEffectsManager.play("portalUse");
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Portal.teleportDuration,
                    new Action<float>(p =>
                    {
                        // Delayed action
                        PlayerControl.LocalPlayer.moveable = false;
                        PlayerControl.LocalPlayer.NetTransform.Halt();
                        if (p >= 0.5f && p <= 0.53f && !didTeleport && !MeetingHud.Instance)
                        {
                            if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(exit.y > -7);
                            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(exit);
                            didTeleport = true;
                        }

                        if (p == 1f) PlayerControl.LocalPlayer.moveable = true;
                    })));
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() &&
                       canPortalFromAnywhere && Portal.bothPlacedAndEnabled;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove &&
                       !Portal.locationNearEntry(PlayerControl.LocalPlayer.transform.position) && !Portal.isTeleporting;
            },
            () => { portalmakerMoveToPortalButton.Timer = portalmakerMoveToPortalButton.MaxTimer; },
            usePortalButtonSprite,
            __instance,
            __instance.AbilityButton,
            null,
            true
        )
        {
            MaxTimer = usePortalCooldown
        };

        // portalmakerButtonText1 -> usePortalButton
        // portalmakerButtonText2 -> portalmakerMoveToPortalButton
    }
}