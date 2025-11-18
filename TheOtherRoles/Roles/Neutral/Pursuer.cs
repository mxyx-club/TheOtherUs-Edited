namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Pursuer : RoleBase, INeutral
{
    public static Color color = new Color32(145, 164, 30, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Pursuer),
        (p) => new Pursuer(p),
        RoleId.Pursuer,
        RoleType.Neutral,
        "Pursuer",
        color,
        200300,
        AddOptions
    );

    public Pursuer(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Benign;

    public PlayerControl target;
    public List<PlayerControl> blankedList = new();
    public int blanks;

    public static float cooldown = 30f;
    public static int blanksNumber = 5;

    public static CustomOption pursuerBlanksCooldown;
    public static CustomOption pursuerBlanksNumber;

    public CustomButton pursuerButton;
    public static ResourceSprite buttonSprite = new("PursuerButton.png");
    public static RemoteProcess<(PlayerControl target, byte value)> PursuerSetBlanked = new("PursuerSetBlanked", (data, _) =>
    {
        if (data.target == null) return;
        CustomRoleManager.blankedList.Remove(data.target.PlayerId);
        if (data.value > 0) CustomRoleManager.blankedList.Add(data.target.PlayerId);
    });

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        pursuerBlanksCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "pursuerBlanksCooldown", 20f, 5f, 60f, 2.5f, roleinfo.RoleOption);
        pursuerBlanksNumber = CustomOption.Create(configId++, CustomOptionType.Neutral, "pursuerBlanksNumber", 6f, 1f, 20f, 1f, roleinfo.RoleOption);
    }


    public override void Initialize()
    {
        target = null;
        blankedList.Clear();
        blanks = 0;

        cooldown = pursuerBlanksCooldown.GetFloat();
        blanksNumber = pursuerBlanksNumber.GetInt();
    }

    public override void CleanUp(HudManager __instance)
    {
        pursuerButton?.Destroy();
        pursuerButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Pursuer button
        pursuerButton?.Destroy();
        pursuerButton = new CustomButton(
            () =>
            {
                if (target != null)
                {
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;
                    PursuerSetBlanked.Invoke((target, byte.MaxValue));

                    target = null;

                    blanks++;
                    pursuerButton.Timer = pursuerButton.MaxTimer;
                    SoundEffectsManager.play("pursuerBlank");
                }
            },
            () =>
            {
                return Player.IsAlive() && PlayerControl.LocalPlayer == Player;
            },
            () =>
            {
                target = SetTarget();
                SetPlayerOutline(target, color);

                pursuerButton.showTargetNameOnButton(target, GetString("PursuerText"));
                if (pursuerButton.ButtonTitle != null)
                    pursuerButton.ButtonTitle.text = $"{blanksNumber - blanks}";

                return blanksNumber > blanks && PlayerControl.LocalPlayer.CanMove && target != null;
            },
            () => { pursuerButton.Timer = pursuerButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("PursuerText")
        )
        {
            MaxTimer = cooldown,
        };

        // Pursuer button blanks left
        // pursuerButtonBlanksText -> pursuerButton
    }
}
