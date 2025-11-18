namespace TheOtherRoles.Roles.Modifier;

[CustomRpcHolder]
public class Shifter : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Shifter),
        (p) => new Shifter(p),
        RoleId.Shifter,
        "Shifter",
        color,
        403400,
        AddOptions
    );

    public Shifter(PlayerControl p) : base(p, roleinfo) { }

    public PlayerControl futureShift;
    public PlayerControl currentTarget;

    public static bool shiftNeutral;
    public static bool shiftALLNeutra;
    public static bool reloadRole;

    public static CustomOption modifierShiftNeutral;
    public static CustomOption modifierShiftALLNeutral;
    public static CustomOption modifierShiftReloadRole;

    public CustomButton shifterShiftButton;
    public static ResourceSprite buttonSprite = new("ShiftButton.png");

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierShiftNeutral = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierShiftNeutral", false, roleinfo.RoleOption);
        modifierShiftALLNeutral = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierShiftALLNeutral", false, modifierShiftNeutral);
        modifierShiftReloadRole = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierShiftReloadRole", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        futureShift = null;
        currentTarget = null;
        shiftNeutral = modifierShiftNeutral.GetBool();
        shiftALLNeutra = modifierShiftALLNeutral.GetBool();
        reloadRole = modifierShiftReloadRole.GetBool();
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (futureShift.IsAlive() && PlayerControl.LocalPlayer == Player)
        {
            ShifterShift.Invoke((PlayerControl.LocalPlayer, futureShift));
        }

    }

    public static bool CanShift(PlayerControl player)
    {
        var role = player.GetRole();
        if (player.IsImpostor()) return false;
        if (shiftNeutral && player.IsNeutral())
        {
            if (shiftALLNeutra)
            {
                return role is not RoleId.Jackal and
                       not RoleId.Sidekick and
                       not RoleId.Pavlovsowner and
                       not RoleId.Pavlovsdogs and
                       not RoleId.Akujo and
                       not RoleId.Lawyer;
            }
            else
            {
                return role is not RoleId.Jackal and
                       not RoleId.Sidekick and
                       not RoleId.Pavlovsowner and
                       not RoleId.Pavlovsdogs and
                       not RoleId.Akujo and
                       not RoleId.Lawyer and
                       not RoleId.Werewolf and
                       not RoleId.Juggernaut and
                       not RoleId.Pelican and
                       not RoleId.Swooper;
            }
        }
        return player.IsCrew();
    }

    public override void CleanUp(HudManager __instance)
    {
        shifterShiftButton?.Destroy();
        shifterShiftButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Shifter shift
        shifterShiftButton?.Destroy();
        shifterShiftButton = new(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;

                SetFutureShifted.Invoke((PlayerControl.LocalPlayer, currentTarget));
                SoundEffectsManager.play("shifterShift");
            },
            () =>
            {
                return Player.IsAlive() && PlayerControl.LocalPlayer == Player && futureShift == null;
            },
            () =>
            {
                currentTarget = SetTarget();
                if (futureShift == null)
                {
                    SetPlayerOutline(currentTarget, Color.yellow);
                    shifterShiftButton.showTargetNameOnButton(currentTarget, GetString("ShiftText"));
                }
                return currentTarget && futureShift == null &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.modifierAbilityInput.keyCode,
            true,
            buttonText: GetString("ShiftText")
        )
        { MaxTimer = 0f };
    }

    public static RemoteProcess<(PlayerControl player, PlayerControl target)> SetFutureShifted = new("SetFutureShifted", (data, _) =>
    // public static void RpcSetShifted(byte playerId, byte targetId)
    {
        // var player = PlayerById(playerId);
        if (data.player.TryGetModifier<Shifter>(out var shifter))
        {
            shifter.futureShift = data.target;
        }
    });

    public static RemoteProcess<(PlayerControl player, PlayerControl target)> ShifterShift = new("ShifterShift", (data, _) =>
    // public static void RpcShift(byte playerId, byte targetId)
    {

        if (data.target == null || data.player == null) return;

        data.player.GetModifier<Shifter>()?.Destroy();

        // Suicide (exile) when impostor or impostor variants
        if (CanShift(data.target) && data.player.IsAlive())
        {
            Message($"Target Is Neutral: {CanShift(data.target)}", "Shifter");
            data.player.Exiled();
            PlayerData.SetDeathReason(data.player, CustomDeathReason.Shift, data.target);

            return;
        }

        CustomRoleManager.ShiftRole(data.player, data.target, reloadRole);

        // Set cooldowns to max for both players
        if (PlayerControl.LocalPlayer == data.player || PlayerControl.LocalPlayer == data.target)
            CustomButton.ResetAllCooldowns();
    });

}