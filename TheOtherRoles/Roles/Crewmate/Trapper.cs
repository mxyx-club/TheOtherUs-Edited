using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Trapper : RoleBase
{
    public static Color color = new Color32(110, 57, 105, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Trapper),
        (p) => new Trapper(p),
        RoleId.Trapper,
        RoleType.Crewmate,
        "Trapper",
        color,
        303500,
        AddOptions
    );

    public Trapper(PlayerControl p) : base(p, roleinfo) { }

    public static float cooldown = 30f;
    public static int maxCharges = 5;
    public static int rechargeTasksNumber = 3;
    public static int rechargedTasks = 3;
    public int charges = 1;
    public static int trapCountToReveal = 2;
    public static int infoType; // 0 = Role, 1 = Good/Evil, 2 = Name
    public static float trapDuration = 5f;

    public CustomButton trapperButton;
    public static Sprite trapButtonSprite = new ResourceSprite("Trapper_Place_Button.png");

    public static CustomOption trapperCooldown;
    public static CustomOption trapperMaxCharges;
    public static CustomOption trapperRechargeTasksNumber;
    public static CustomOption trapperTrapNeededTriggerToReveal;
    public static CustomOption trapperInfoType;
    public static CustomOption trapperTrapDuration;
    public static RemoteProcess<(PlayerControl player, Vector3 position)> SetTrap = new("SetTrap", (data, _) =>
    {
        if (data.player.TryGetRole<Trapper>(out var trapper))
        {
            trapper.charges -= 1;
            var trap = new Trap(data.player, data.position);
        }
    });

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        trapperCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trapperCooldown", 20f, 5f, 120f, 2.5f, roleinfo.RoleOption);
        trapperMaxCharges = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trapperMaxCharges", 5f, 1f, 15f, 1f, roleinfo.RoleOption);
        trapperRechargeTasksNumber = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trapperRechargeTasksNumber", 2f, 1f, 15f, 1f, roleinfo.RoleOption);
        trapperTrapNeededTriggerToReveal = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trapperTrapNeededTriggerToReveal", 2f, 1f, 10f, 1f, roleinfo.RoleOption);
        trapperInfoType = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trapperInfoType", ["Role", "trapperInfoType2", "Name"], roleinfo.RoleOption);
        trapperTrapDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trapperTrapDuration", 5f, 1f, 15f, 0.5f, roleinfo.RoleOption);
    }
    public override void Initialize()
    {
        cooldown = trapperCooldown.GetFloat();
        maxCharges = trapperMaxCharges.GetInt();
        rechargeTasksNumber = trapperRechargeTasksNumber.GetInt();
        rechargedTasks = trapperRechargeTasksNumber.GetInt();
        charges = trapperMaxCharges.GetInt() / 2;
        trapCountToReveal = trapperTrapNeededTriggerToReveal.GetInt();
        infoType = trapperInfoType.GetSelection();
        trapDuration = trapperTrapDuration.GetFloat();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (player != Player || Player.IsDead()) return;
        var (playerCompleted, _) = TasksHandler.taskInfo(Player.Data);
        if (playerCompleted == rechargedTasks)
        {
            rechargedTasks += rechargeTasksNumber;
            if (maxCharges > charges) charges++;
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        trapButtonSprite?.Destroy();
        trapButtonSprite = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Trapper button
        trapperButton?.Destroy();
        trapperButton = new CustomButton(
            () =>
            {
                var pos = PlayerControl.LocalPlayer.transform.position;

                SetTrap.Invoke((PlayerControl.LocalPlayer, pos));

                SoundEffectsManager.play("trapperTrap");
                trapperButton.Timer = trapperButton.MaxTimer;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                if (trapperButton.ButtonTitle != null) trapperButton.ButtonTitle.text = $"{charges} / {maxCharges}";
                return PlayerControl.LocalPlayer.CanMove && charges > 0;
            },
            () => { trapperButton.Timer = trapperButton.MaxTimer; },
            trapButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("trapperTrapText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}