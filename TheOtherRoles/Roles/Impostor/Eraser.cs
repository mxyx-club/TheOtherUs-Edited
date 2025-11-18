namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Eraser : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Eraser),
        (p) => new Eraser(p),
        RoleId.Eraser,
        RoleType.Impostor,
        "Eraser",
        color,
        101600,
        AddOptions
    );

    public Eraser(PlayerControl p) : base(p, roleInfo) { }

    public List<byte> alreadyErased = new();
    public List<PlayerControl> futureErased = new();
    public PlayerControl currentTarget;

    public static float cooldown = 30f;
    public static bool canEraseAnyone;
    public static bool canEraseGuess;

    public static CustomOption eraserCooldown;
    public static CustomOption eraserCanEraseAnyone;
    public static CustomOption erasercanEraseGuess;

    public CustomButton eraserButton;
    public static Sprite buttonSprite = new ResourceSprite("EraserButton.png");

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        eraserCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "eraserCooldown", 25f, 10f, 120f, 2.5f, roleInfo.RoleOption);
        eraserCanEraseAnyone = CustomOption.Create(configId++, CustomOptionType.Impostor, "eraserCanEraseAnyone", false, roleInfo.RoleOption);
        erasercanEraseGuess = CustomOption.Create(configId++, CustomOptionType.Impostor, "erasercanEraseGuess", false, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        Player = null;
        futureErased = new();
        currentTarget = null;
        cooldown = eraserCooldown.GetFloat();
        canEraseAnyone = eraserCanEraseAnyone.GetBool();
        canEraseGuess = erasercanEraseGuess.GetBool();
        alreadyErased.Clear();
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (Player != null && futureErased != null)
        {
            var rasePlayerList = new List<PlayerControl>(futureErased);
            foreach (var target in rasePlayerList)
            {
                target.RemoveRole();
                alreadyErased.Add(target.PlayerId);
            }
            futureErased.Clear();
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        eraserButton?.Destroy();
        eraserButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Eraser erase button
        eraserButton?.Destroy();
        eraserButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                eraserButton.MaxTimer += 10;
                eraserButton.Timer = eraserButton.MaxTimer;

                RpcSetFutureErase.Invoke((PlayerControl.LocalPlayer, currentTarget));
                SoundEffectsManager.play("eraserErase");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                var untargetables = new List<PlayerControl>();
                //if (Spy.Player != null) untargetables.Add(Spy.Player);
                currentTarget = SetTarget(untarget: canEraseAnyone ? [] : untargetables, !canEraseAnyone);
                SetPlayerOutline(currentTarget, color);

                eraserButton.showTargetNameOnButton(currentTarget, GetString("EraserText"));
                return PlayerControl.LocalPlayer.CanMove && currentTarget != null;
            },
            () => { eraserButton.Timer = eraserButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode
        )
        {
            MaxTimer = cooldown,
        };
    }

    public static RemoteProcess<(PlayerControl player, PlayerControl target)> RpcSetFutureErase = new("RpcSetFutureErase", (data, _) =>
    // public static void RpcSetFutureErase(byte playerId, byte targetId)
    {
        if (data.player.TryGetRole<Eraser>(out var eraser))
        {
            eraser.futureErased.Add(data.target);
        }
    });

}
