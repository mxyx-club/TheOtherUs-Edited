using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Vampire : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Vampire),
        (p) => new Vampire(p),
        RoleId.Vampire,
        RoleType.Impostor,
        "Vampire",
        color,
        101500,
        AddOptions
    );

    public Vampire(PlayerControl p) : base(p, roleInfo) { }

    public PlayerControl currentTarget;
    public PlayerControl bitten;
    public bool targetNearGarlic;

    public static float delay = 10f;
    public static float cooldown = 30f;
    public static bool canKillNearGarlics = true;
    public static bool localPlacedGarlic;
    public static bool garlicsActive = true;
    public static bool garlicButton;
    public static CustomOption vampireKillDelay;
    public static CustomOption vampireCooldown;
    public static CustomOption vampireGarlicButton;
    public static CustomOption vampireCanKillNearGarlics;

    public CustomButton vampireKillButton;
    public static Sprite buttonSprite = new ResourceSprite("VampireButton.png");
    public static Sprite garlicButtonSprite = new ResourceSprite("GarlicButton.png");
    public static RemoteProcess<(PlayerControl player, byte targetId, bool reset)> VampireSetBitten = new("VampireSetBitten", (data, _) =>
    {
        if (data.player.TryGetRole<Vampire>(out var vampire))
        {
            if (data.reset)
            {
                vampire.bitten = null;
                return;
            }
            vampire.bitten = PlayerById(data.targetId);
        }
    });
    private static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        vampireKillDelay = CustomOption.Create(configId++, CustomOptionType.Impostor, "vampireKillDelay", 5f, 1f, 10f, 0.5f, roleInfo.RoleOption);
        vampireCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "vampireCooldown", 25f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        vampireGarlicButton = CustomOption.Create(configId++, CustomOptionType.Impostor, "vampireGarlicButton", true, roleInfo.RoleOption);
        vampireCanKillNearGarlics = CustomOption.Create(configId++, CustomOptionType.Impostor, "vampireCanKillNearGarlics", true, vampireGarlicButton);
    }

    public override void Initialize()
    {
        bitten = null;
        targetNearGarlic = false;
        localPlacedGarlic = false;
        currentTarget = null;
        garlicsActive = roleInfo.RoleOption.GetSelection() > 0;
        delay = vampireKillDelay.GetFloat();
        cooldown = vampireCooldown.GetFloat();
        canKillNearGarlics = vampireCanKillNearGarlics.GetBool();
        garlicButton = vampireGarlicButton.GetBool();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        CustomObject.UpdateAll();
    }

    public override bool OnCheckReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target)
    {
        if (reporter == bitten && bitten.AmOwner)
        {
            RpcCustomMurderPlayer(Player, bitten, false);
            bitten = null;
            return false;
        }
        return true;
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        bitten = null;
    }

    public override void CleanUp(HudManager __instance)
    {
        vampireKillButton?.Destroy();
        vampireKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        vampireKillButton?.Destroy();
        vampireKillButton = new CustomButton(
           () =>
           {
               var target = currentTarget;
               if (!RoleHelpers.CheckMurderPlayer(PlayerControl.LocalPlayer, currentTarget)) return;
               if (targetNearGarlic)
               {
                   RpcCustomMurderPlayer(PlayerControl.LocalPlayer, currentTarget, true);

                   vampireKillButton.MaxTimer = PlayerControl.LocalPlayer.Is(RoleId.LastImpostor)
                        ? cooldown - LastImpostor.deduce
                        : cooldown;
                   vampireKillButton.HasEffect = false; // Block effect on this click
                   vampireKillButton.Timer = vampireKillButton.MaxTimer;
               }
               else
               {
                   // Notify players about bitten
                   VampireSetBitten.Invoke((PlayerControl.LocalPlayer, bitten.PlayerId, false));

                   _ = new LateTask(() =>
                   {
                       RpcCustomMurderPlayer(Player, bitten, false);

                       VampireSetBitten.Invoke((PlayerControl.LocalPlayer, byte.MaxValue, true));

                   }, delay, "Vampire Kill");
                   SoundEffectsManager.play("vampireBite");

                   vampireKillButton.HasEffect = true; // Trigger effect on this click
               }
           },
           () =>
           {
               return Player == PlayerControl.LocalPlayer && Player.IsAlive();
           },
           () =>
           {
               currentTarget = ImpostorSetTarget();

               bool nearGarlic = false;
               if (currentTarget != null)
               {
                   foreach (var garlic in Garlic.AllObjects)
                       if (Vector2.Distance(garlic.GameObject.transform.position, currentTarget.transform.position) <= 1.95f)
                           targetNearGarlic = true;
               }

               targetNearGarlic = nearGarlic;

               if (targetNearGarlic)
                   vampireKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));
               else
                   vampireKillButton.showTargetNameOnButton(currentTarget, GetString("VampireText"));

               if (targetNearGarlic && canKillNearGarlics)
               {
                   vampireKillButton.actionButton.graphic.sprite = __instance.KillButton.graphic.sprite;
                   vampireKillButton.showButtonText = true;
               }
               else
               {
                   vampireKillButton.actionButton.graphic.sprite = buttonSprite;
                   vampireKillButton.showButtonText = false;
               }

               return currentTarget != null && PlayerControl.LocalPlayer.CanMove && (!targetNearGarlic || canKillNearGarlics);
           },
           () =>
           {
               vampireKillButton.MaxTimer = PlayerControl.LocalPlayer.Is(RoleId.LastImpostor)
                    ? cooldown - LastImpostor.deduce
                    : cooldown;
               vampireKillButton.Timer = vampireKillButton.MaxTimer;
               vampireKillButton.isEffectActive = false;
               vampireKillButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
           },
           buttonSprite,
           __instance,
            __instance.KillButton,
           ModInputManager.modKillInput.keyCode,
           false,
           0f,
           () =>
           {
               vampireKillButton.MaxTimer = PlayerControl.LocalPlayer.Is(RoleId.LastImpostor)
                    ? cooldown - LastImpostor.deduce
                    : cooldown;
               vampireKillButton.isEffectActive = false;
               vampireKillButton.Timer = vampireKillButton.MaxTimer - delay;
           },
           buttonText: "VampireText".Translate()
       )
        {
            MaxTimer = cooldown,
            EffectDuration = delay,
        };
    }
}
