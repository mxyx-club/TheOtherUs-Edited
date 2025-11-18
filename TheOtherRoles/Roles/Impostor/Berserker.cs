namespace TheOtherRoles.Roles.Impostor;

public class Berserker : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Berserker),
        (p) => new Berserker(p),
        RoleId.Berserker,
        RoleType.Impostor,
        "Berserker",
        color,
        103600,
        AddOptions
    );

    public Berserker(PlayerControl p) : base(p, roleInfo) { }

    public PlayerControl currentTarget;

    public static float KillCooldown = 25f; // 角色的击杀冷却时间
    private static float ChargingTimer = 10f;  // 角色的狂暴蓄力时间（需要在击杀冷却完毕后才会进行蓄力）
    public static float RampageDuration = 3f; // 狂暴持续时间

    public float Timer; // 计时器
    public float Duration; // 效果时长

    public static CustomOption berserkerKillCooldown;
    public static CustomOption berserkerRampageCooldown;
    public static CustomOption berserkerRampageDuration;
    public static CustomButton berserkerKillButton;

    public override void Initialize()
    {
        currentTarget = null;
        Timer = 0f;
        KillCooldown = berserkerKillCooldown.GetFloat();
        ChargingTimer = berserkerRampageCooldown.GetFloat();
        RampageDuration = berserkerRampageDuration.GetFloat();
    }

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        berserkerKillCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "killCooldown", 25f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        berserkerRampageCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "berserkerRampageCooldown", 10f, 0f, 60f, 0.5f, roleInfo.RoleOption);
        berserkerRampageDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "berserkerRampageDuration", 3f, 0.5f, 10f, 0.25f, roleInfo.RoleOption);
    }

    public override void CleanUp(HudManager __instance)
    {
        berserkerKillButton?.Destroy();
        berserkerKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        berserkerKillButton?.Destroy();
        berserkerKillButton = new CustomButton(
            () =>
            {
                var target = currentTarget;
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target)) return;

                berserkerKillButton.EffectDuration = GetDuration();
                target = null;
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                currentTarget = ImpostorSetTarget();
                berserkerKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));

                if (berserkerKillButton.ButtonTitle != null)
                {
                    berserkerKillButton.ButtonTitle.text = !berserkerKillButton.isEffectActive
                         ? $"{(int)(GetDurationPercentage() * 100)} % | {GetDuration():0.00}s"
                        : $"{berserkerKillButton.Timer:0.00}";

                }

                if (berserkerKillButton.Timer <= 0)
                {
                    UpdateTimer();
                }

                return PlayerControl.LocalPlayer.CanMove && currentTarget != null;
            },
            () => { berserkerKillButton.Timer = berserkerKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            true,
            0.5f,
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && currentTarget;
            },
            () =>
            {
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, currentTarget)) return;
            },
            () =>
            {
                berserkerKillButton.Timer = berserkerKillButton.MaxTimer;
                Timer = 0f;
            },
            buttonText: GetString("killButtonText")
        )
        { MaxTimer = KillCooldown, EffectDuration = 0.5f };
    }

    public void UpdateTimer()
    {
        if (Timer >= 0)
        {
            if (Timer <= ChargingTimer) Timer += Time.deltaTime;
        }
        else
        {
            Timer = 0;
        }
    }

    public float GetTimerRatio()
    {
        return Mathf.Clamp01(Timer / ChargingTimer);
    }

    public float GetDuration()
    {
        float remainingRatio = GetTimerRatio();
        return Mathf.Lerp(0.1f, RampageDuration, remainingRatio);
    }

    public float GetDurationPercentage()
    {
        if (RampageDuration <= 0) return 0f;

        float currentDuration = GetDuration();
        return Mathf.Clamp01(currentDuration / RampageDuration);
    }
}
