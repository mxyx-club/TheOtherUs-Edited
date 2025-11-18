namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Grenadier : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Grenadier),
        (p) => new Grenadier(p),
        RoleId.Grenadier,
        RoleType.Impostor,
        "Grenadier",
        color,
        103400,
        AddOptions
    );

    public Grenadier(PlayerControl p) : base(p, roleInfo) { }

    public static new PlayerControl Player;
    public static Color flash = new Color32(150, 150, 150, byte.MaxValue);
    public static List<PlayerControl> controls = new();

    public static float cooldown;
    public static float duration;
    public static float radius;
    public static bool indicatorsMode;

    public static CustomOption grenadierCooldown;
    public static CustomOption grenadierDuration;
    public static CustomOption grenadierFlashRadius;
    public static CustomOption grenadierTeamIndicators;

    public CustomButton grenadierFlashButton;
    public static Sprite ButtonSprite = new ResourceSprite("FlashButton.png");
    public static RemoteProcess<bool> GrenadierFlash = new("GrenadierFlash", (clear, _) =>
    {
        if (clear)
        {
            Grenadier.controls.Clear();
            return;
        }

        var closestPlayers = GetClosestPlayers(Grenadier.Player.GetTruePosition(), Grenadier.radius, true);
        Grenadier.controls = closestPlayers;
        foreach (var player in closestPlayers)
        {
            if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                if (player.IsImpostor() && !player.IsDead() && !MeetingHud.Instance)
                {
                    Grenadier.showFlash(Grenadier.flash, Grenadier.duration, 0.24f);
                }
                else if (!player.IsImpostor() && !player.IsDead() && !MeetingHud.Instance)
                {
                    Grenadier.showFlash(Grenadier.flash, Grenadier.duration, 1f);
                }
            }
        }
    });

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        grenadierCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "grenadierCooldown", 20f, 0f, 45f, 2.5f, roleInfo.RoleOption);
        grenadierDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "grenadierDuration", 8f, 4f, 10f, 0.5f, roleInfo.RoleOption);
        grenadierFlashRadius = CustomOption.Create(configId++, CustomOptionType.Impostor, "grenadierFlashRadius", 1f, 0.25f, 5f, 0.125f, roleInfo.RoleOption);
        grenadierTeamIndicators = CustomOption.Create(configId++, CustomOptionType.Impostor, "grenadierTeamIndicators", true, roleInfo.RoleOption);
    }

    public static void showFlash(Color color, float duration = 10f, float alpha = 1f)
    {
        if (FastDestroyableSingleton<HudManager>.Instance == null ||
            FastDestroyableSingleton<HudManager>.Instance.FullScreen == null) return;

        FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        DestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.active = true;

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
        {
            var renderer = FastDestroyableSingleton<HudManager>.Instance.FullScreen;
            var fadeFraction = 0.5f / duration;

            if (InMeeting)
            {
                renderer.enabled = false;
                if (PlayerControl.LocalPlayer.PlayerId == Player?.PlayerId && controls?.Count > 0)
                {
                    GrenadierFlash.Invoke(true);
                }
                return;
            }

            if (p < fadeFraction)
            {
                var fadeInProgress = p / fadeFraction;
                if (renderer != null) renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(fadeInProgress * alpha));
            }
            else if (p > 1 - fadeFraction)
            {
                var fadeOutProgress = (p - (1 - fadeFraction)) / fadeFraction;
                if (renderer != null) renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - fadeOutProgress) * alpha));

                if (PlayerControl.LocalPlayer.PlayerId == Player?.PlayerId && controls?.Count > 0)
                {
                    GrenadierFlash.Invoke(true);
                }
            }
            else
            {
                if (renderer != null) renderer.color = new Color(color.r, color.g, color.b, alpha);
            }

            if (p == 1f && renderer != null) renderer.enabled = false;
        })));
    }

    public override void Initialize()
    {
        controls.Clear();
        cooldown = grenadierCooldown.GetFloat();
        duration = grenadierDuration.GetFloat() + 0.5f;
        radius = grenadierFlashRadius.GetFloat();
        indicatorsMode = grenadierTeamIndicators.GetBool();
    }

    public override void CleanUp(HudManager __instance)
    {
        grenadierFlashButton?.Destroy();
        grenadierFlashButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        grenadierFlashButton?.Destroy();
        grenadierFlashButton = new CustomButton(
            () =>
            {
                // On Use
                GrenadierFlash.Invoke(false);
            },
            () =>
            {
                // Can See
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                // On Click

                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    if (task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor
                                                              || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.StopCharles
                                                              || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                        return false;
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                grenadierFlashButton.Timer = grenadierFlashButton.MaxTimer;
                grenadierFlashButton.isEffectActive = false;
                grenadierFlashButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            ButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            duration,
            () =>
            {
                grenadierFlashButton.Timer = grenadierFlashButton.MaxTimer;
            },
            buttonText: GetString("FlashButton")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = duration,
        };
    }
}
