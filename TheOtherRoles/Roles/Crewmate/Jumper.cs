namespace TheOtherRoles.Roles.Crewmate;

public class Jumper : RoleBase
{
    public static Color color = new Color32(204, 155, 20, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Jumper),
        (p) => new Jumper(p),
        RoleId.Jumper,
        RoleType.Crewmate,
        "Jumper",
        color,
        303200,
        AddOptions
    );

    public Jumper(PlayerControl p) : base(p, roleinfo) { }

    public Vector3 jumpLocation;
    public int Charges = 1;

    public static float JumpTime = 30f;
    public static bool resetPlaceAfterMeeting;
    public static int ChargesGainOnMeeting = 2;
    public static float MaxCharges = 3f;

    public CustomButton jumperMarkButton;
    public CustomButton jumperJumpButton;
    public static Sprite jumpMarkButtonSprite = new ResourceSprite("JumperMarkButton.png");
    public static Sprite jumpJumpButtonSprite = new ResourceSprite("JumperJumpButton.png");

    public static CustomOption jumperJumpTime;
    public static CustomOption jumperResetPlaceAfterMeeting;
    public static CustomOption jumperChargesGainOnMeeting;
    public static CustomOption jumperMaxCharges;

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        jumperJumpTime = CustomOption.Create(configId++, CustomOptionType.Crewmate, "jumperJumpTime", 10f, 0f, 60f, 2.5f, roleinfo.RoleOption);
        jumperMaxCharges = CustomOption.Create(configId++, CustomOptionType.Crewmate, "jumperMaxCharges", 3, 0, 10, 1, roleinfo.RoleOption);
        jumperResetPlaceAfterMeeting = CustomOption.Create(configId++, CustomOptionType.Crewmate, "jumperResetPlaceAfterMeeting", false, roleinfo.RoleOption);
        jumperChargesGainOnMeeting = CustomOption.Create(configId++, CustomOptionType.Crewmate, "jumperChargesGainOnMeeting", 2, 0, 10, 1, roleinfo.RoleOption);
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        Charges += ChargesGainOnMeeting;
        jumpLocation = Vector3.zero;
    }

    public override void Initialize()
    {
        jumpLocation = Vector3.zero;
        resetPlaceAfterMeeting = jumperResetPlaceAfterMeeting.GetBool();
        Charges = jumperMaxCharges.GetInt();
        JumpTime = jumperJumpTime.GetFloat();
        ChargesGainOnMeeting = jumperChargesGainOnMeeting.GetInt();
        MaxCharges = jumperMaxCharges.GetFloat();
    }

    public override void CleanUp(HudManager __instance)
    {
        jumperMarkButton?.Destroy();
        jumperJumpButton?.Destroy();
        jumperMarkButton = null;
        jumperJumpButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Jumper Mark
        jumperMarkButton?.Destroy();
        jumperMarkButton = new CustomButton(
            () =>
            {
                //set location
                jumpLocation = PlayerControl.LocalPlayer.transform.localPosition;
                jumperMarkButton.Timer = jumperMarkButton.MaxTimer;
                //jumperJumpButton.Timer = jumperMarkButton.MaxTimer;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && Charges > 0;
            },
            () =>
            {
                // Jumper.usedPlace = true; // todo：解决此处 usedPlace 属性缺失的冲突
                return Charges > 0f && PlayerControl.LocalPlayer.CanMove;

            },
            () =>
            {
                if (resetPlaceAfterMeeting) jumpLocation = Vector3.zero;
                jumperMarkButton.Timer = jumperMarkButton.MaxTimer;
            },
            jumpMarkButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.secondaryAbilityInput.keyCode,
            buttonText: "jumperMarkText".Translate()
        )
        {
            MaxTimer = JumpTime
        };

        // Jumper Jump
        jumperJumpButton?.Destroy();
        jumperJumpButton = new CustomButton(
            () =>
            {
                //teleport to location if you have one
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(jumpLocation);
                Charges--;
                jumperJumpButton.Timer = jumperJumpButton.MaxTimer;
                //jumperMarkButton.Timer = jumperJumpButton.MaxTimer;

            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && Charges > 0;
            },
            () =>
            {
                // Jumper.usedPlace = true; // todo：解决此处 usedPlace 属性缺失的冲突
                return Charges >= 1f && jumpLocation != Vector3.zero && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                Charges += ChargesGainOnMeeting;
                if (Charges > 0) jumperJumpButton.Timer = jumperJumpButton.MaxTimer;
            },
            jumpJumpButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: "jumperJumpText".Translate()
        )
        {
            MaxTimer = JumpTime
        };
    }
}
