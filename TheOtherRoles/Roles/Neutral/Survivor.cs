namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Survivor : RoleBase, INeutral
{
    public static Color color = new Color32(255, 230, 77, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Survivor),
        (p) => new Survivor(p),
        RoleId.Survivor,
        RoleType.Neutral,
        "Survivor",
        color,
        200200,
        AddOptions
    );

    public Survivor(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Benign;

    public PlayerControl target;
    public List<PlayerControl> blankedList = new();

    public int vestUsed;
    public int blanksUsed;
    public bool vestActive;

    public static bool vestEnable;
    public static int vestNumber;
    public static float vestCooldown;
    public static float vestDuration;
    public static float vestResetCooldown;
    public static bool blanksEnable;
    public static int blanksNumber;
    public static float blanksCooldown;

    public static CustomOption survivorVestEnable;
    public static CustomOption survivorVestNumber;
    public static CustomOption survivorVestCooldown;
    public static CustomOption survivorVestDuration;
    public static CustomOption survivorVestResetCooldown;
    public static CustomOption survivorBlanksEnable;
    public static CustomOption survivorBlanksCooldown;
    public static CustomOption survivorBlanksNumber;

    public CustomButton survivorVestButton;
    public CustomButton survivorBlanksButton;
    public static Sprite VestButtonSprite = new ResourceSprite("TheOtherRoles.Resources.Vest.png");

    public int remainingVests => vestNumber - vestUsed;
    public int remainingBlanks => blanksNumber - blanksUsed;
    public static RemoteProcess<PlayerControl> SurvivorVestActive = new("SurvivorVestActive", (player, _) =>
    {
        if (player.TryGetRole<Survivor>(out var survivor))
        {
            survivor.vestActive = true;
            var task = new LateTask(() => { survivor.vestActive = false; }, Survivor.vestDuration, "Survivor Vest End");
        }
    });
    public static RemoteProcess<(PlayerControl target, byte value)> SurvivorSetBlanked = new("SurvivorSetBlanked", (data, _) =>
    {
        if (data.target == null) return;
        CustomRoleManager.blankedList.Remove(data.target.PlayerId);
        if (data.value > 0) CustomRoleManager.blankedList.Add(data.target.PlayerId);
    });

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        survivorVestEnable = CustomOption.Create(configId++, CustomOptionType.Neutral, "survivorVestEnable", true, roleinfo.RoleOption);
        survivorVestNumber = CustomOption.Create(configId++, CustomOptionType.Neutral, "survivorVestNumber", 5f, 1f, 20f, 1f, survivorVestEnable);
        survivorVestCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "survivorVestCooldown", 20f, 2.5f, 60f, 2.5f, survivorVestEnable);
        survivorVestDuration = CustomOption.Create(configId++, CustomOptionType.Neutral, "survivorVestDuration", 10f, 2.5f, 60f, 0.5f, survivorVestEnable);
        survivorVestResetCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "survivorVestResetCooldown", 5f, 2.5f, 60f, 2.5f, survivorVestEnable);
        survivorBlanksEnable = CustomOption.Create(configId++, CustomOptionType.Neutral, "survivorBlanksEnable", false, roleinfo.RoleOption);
        survivorBlanksCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "survivorBlanksCooldown", 20f, 5f, 60f, 2.5f, survivorBlanksEnable);
        survivorBlanksNumber = CustomOption.Create(configId++, CustomOptionType.Neutral, "survivorBlanksNumber", 6f, 1f, 20f, 1f, survivorBlanksEnable);
    }

    public override void Initialize()
    {
        target = null;
        blankedList.Clear();

        vestActive = false;
        blanksUsed = 0;
        vestUsed = 0;
        vestEnable = survivorVestEnable.GetBool();
        vestNumber = survivorVestNumber.GetInt();
        vestCooldown = survivorVestCooldown.GetFloat();
        vestDuration = survivorVestDuration.GetFloat();
        vestResetCooldown = survivorVestResetCooldown.GetFloat();
        blanksEnable = survivorBlanksEnable.GetBool();
        blanksNumber = survivorBlanksNumber.GetInt();
        blanksCooldown = survivorBlanksCooldown.GetFloat();
    }

    public override void CleanUp(HudManager __instance)
    {
        survivorVestButton?.Destroy();
        survivorVestButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        survivorVestButton?.Destroy();
        survivorVestButton = new CustomButton(
            () =>
            {
                SurvivorVestActive.Invoke(PlayerControl.LocalPlayer);
                vestUsed++;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() &&
                       PlayerControl.LocalPlayer.IsAlive() && vestEnable;
            },
            () =>
            {
                if (survivorVestButton.ButtonTitle != null) survivorVestButton.ButtonTitle.text = $"{remainingVests} / {vestNumber}";
                return PlayerControl.LocalPlayer.CanMove && remainingVests > 0;
            },
            () =>
            {
                survivorVestButton.Timer = survivorVestButton.MaxTimer;
                survivorVestButton.isEffectActive = false;
                survivorVestButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            VestButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            vestDuration,
            () => { survivorVestButton.Timer = survivorVestButton.MaxTimer; },
            buttonText: GetString("VestButton")
        )
        {
            MaxTimer = vestCooldown,
            EffectDuration = vestDuration,
        };

        // Survivor button
        survivorBlanksButton?.Destroy();
        survivorBlanksButton = new CustomButton(
            () =>
            {
                if (target != null)
                {
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;
                    SurvivorSetBlanked.Invoke((target, byte.MaxValue));

                    target = null;

                    blanksUsed++;
                    survivorBlanksButton.Timer = survivorBlanksButton.MaxTimer;
                    SoundEffectsManager.play("pursuerBlank");
                }
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && blanksEnable;
            },
            () =>
            {
                target = SetTarget();
                SetPlayerOutline(target, color);

                survivorBlanksButton.showTargetNameOnButton(target, GetString("PursuerText"));
                if (survivorBlanksButton.ButtonTitle != null) survivorBlanksButton.ButtonTitle.text = $"{remainingBlanks} / {blanksNumber}";

                return blanksNumber > blanksUsed && PlayerControl.LocalPlayer.CanMove && target != null;
            },
            () => { survivorBlanksButton.Timer = survivorBlanksButton.MaxTimer; },
            Pursuer.buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.secondaryAbilityInput.keyCode,
            buttonText: GetString("PursuerText")
        )
        {
            MaxTimer = blanksCooldown,
        };

        // Pursuer button blanks left
        // survivorVestButtonText -> survivorVestButton
        // Pursuer button blanks left
        // survivorBlanksButtonText -> survivorBlanksButton
    }
}
