using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Terrorist : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Terrorist),
        (p) => new Terrorist(p),
        RoleId.Terrorist,
        RoleType.Impostor,
        "Terrorist",
        color,
        102700,
        AddOptions
    );

    public Terrorist(PlayerControl p) : base(p, roleInfo) { }

    public static bool selfExplosion => destructionTime + bombActiveAfter == 0;
    public static float destructionTime = 20f;
    public static float destructionRange = 2f;
    public static float hearRange = 30f;
    public static float defuseDuration = 3f;
    public static float bombCooldown = 15f;
    public static float bombActiveAfter = 3f;

    public static CustomOption terroristBombDestructionTime;
    public static CustomOption terroristBombDestructionRange;
    public static CustomOption terroristBombHearRange;
    public static CustomOption terroristDefuseDuration;
    public static CustomOption terroristBombCooldown;
    public static CustomOption terroristBombActiveAfter;

    public CustomButton terroristButton;
    public static Sprite buttonSprite = new ResourceSprite("Bomb_Button_Plant.png");
    public static RemoteProcess<(PlayerControl player, Vector3 position)> PlaceBomb = new("PlaceBomb", (data, _) =>
    {
        if (data.player?.GetRoleBase() is not Terrorist tr) return;
        var bomb = new Bomb(data.player, data.position);
    });

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        terroristBombDestructionTime = CustomOption.Create(configId++, CustomOptionType.Impostor, "terroristBombDestructionTime", 0f, 0f, 120f, 0.5f, roleInfo.RoleOption);
        terroristBombDestructionRange = CustomOption.Create(configId++, CustomOptionType.Impostor, "terroristBombDestructionRange", 30f, 5f, 250f, 5f, roleInfo.RoleOption);
        terroristBombHearRange = CustomOption.Create(configId++, CustomOptionType.Impostor, "terroristBombHearRange", 60f, 5f, 250f, 5f, roleInfo.RoleOption);
        terroristDefuseDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "terroristDefuseDuration", 2f, 0f, 30f, 0.5f, roleInfo.RoleOption);
        terroristBombCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "terroristBombCooldown", 0f, 5f, 60f, 2.5f, roleInfo.RoleOption);
        terroristBombActiveAfter = CustomOption.Create(configId++, CustomOptionType.Impostor, "terroristBombActiveAfter", 0f, 0f, 15f, 0.5f, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        destructionTime = terroristBombDestructionTime.GetFloat();
        destructionRange = terroristBombDestructionRange.GetFloat() / 10;
        hearRange = terroristBombHearRange.GetFloat() / 10;
        defuseDuration = terroristDefuseDuration.GetFloat();
        bombCooldown = terroristBombCooldown.GetFloat();
        bombActiveAfter = terroristBombActiveAfter.GetFloat();
    }

    public override void CleanUp(HudManager __instance)
    {
        terroristButton?.Destroy();
        terroristButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Terrorist button
        terroristButton?.Destroy();
        terroristButton = new CustomButton(
            () =>
            {
                var pos = PlayerControl.LocalPlayer.transform.position;

                PlaceBomb.Invoke((PlayerControl.LocalPlayer, pos));

                if (selfExplosion)
                {
                    var writer1 = StartRPC(Player, CustomRPC.CustomMurderPlayer);
                    writer1.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer1.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer1.Write(false);
                    writer1.Write(true);
                    writer1.Write((byte)CustomDeathReason.Suicide);
                    writer1.EndRPC();
                    CustomMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false);
                }

                SoundEffectsManager.play(selfExplosion ? "bombExplosion" : "trapperTrap");
            },
            () => { return Player == PlayerControl.LocalPlayer && Player.IsAlive(); },
            () =>
            {
                terroristButton.buttonText = selfExplosion ? GetString("TerroristBombText2") : GetString("TerroristBombText1");
                return PlayerControl.LocalPlayer.CanMove;
            },
            () => { terroristButton.Timer = terroristButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            destructionTime,
            () =>
            {
                terroristButton.Timer = terroristButton.MaxTimer;
                terroristButton.isEffectActive = false;
                terroristButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            }
        )
        {
            MaxTimer = bombCooldown,
        };
    }
}
