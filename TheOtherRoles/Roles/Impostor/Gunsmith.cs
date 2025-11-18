namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Gunsmith : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Gunsmith),
        (p) => new Gunsmith(p),
        RoleId.Gunsmith,
        RoleType.Impostor,
        "Gunsmith",
        color,
        103500,
        AddOptions
    );

    public Gunsmith(PlayerControl p) : base(p, roleInfo) { }

    public static float KillCooldown = 25f;
    public static float setKillCooldown;
    public static int maxChangeCount;

    public int remainingChange;

    public static Sprite AddButton = new ResourceSprite("GunsmithAddButton.png");
    public static Sprite GetButton = new ResourceSprite("GunsmithGetButton.png");

    public static CustomOption gunsmithKillCooldown;
    public static CustomOption gunsmithSetKillCooldown;
    public static CustomOption gunsmithMaxChangeCount;

    public CustomButton gunsmithGetBullets;
    public CustomButton gunsmithAddBullets;
    public static RemoteProcess<(PlayerControl player, int remaining)> SyncGunsmithChange = new("SyncGunsmithChange", (data, _) =>
    {
        var role = data.player.GetRole<Gunsmith>();
        role.remainingChange = data.remaining;
    });

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        gunsmithKillCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "killCooldown", 25f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        gunsmithSetKillCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "gunsmithSetKillCooldown", 0f, 0f, 7.5f, 0.5f, roleInfo.RoleOption);
        gunsmithMaxChangeCount = CustomOption.Create(configId++, CustomOptionType.Impostor, "gunsmithMaxChangeCount", 5, 1, 15, 1, roleInfo.RoleOption);
    }

    public override void CleanUp(HudManager __instance)
    {
        gunsmithGetBullets?.Destroy();
        gunsmithAddBullets?.Destroy();
        gunsmithGetBullets = null;
        gunsmithAddBullets = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        gunsmithGetBullets?.Destroy();
        gunsmithGetBullets = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.killTimer = setKillCooldown;
                remainingChange--;
                SyncGunsmithChange.Invoke((PlayerControl.LocalPlayer, remainingChange));
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                if (gunsmithGetBullets.ButtonTitle != null) gunsmithGetBullets.ButtonTitle.text = $"{remainingChange} / {maxChangeCount}";
                return PlayerControl.LocalPlayer.CanMove && PlayerControl.LocalPlayer.killTimer > setKillCooldown && remainingChange > 0;
            },
            () => { },
            GetButton,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("gunsmithGetBullets")
        )
        { MaxTimer = 0f };

        gunsmithAddBullets?.Destroy();
        gunsmithAddBullets = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.SetKillTimer(ModOption.KillCooldown);
                remainingChange++;
                SyncGunsmithChange.Invoke((PlayerControl.LocalPlayer, remainingChange));
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && !FastDestroyableSingleton<HudManager>.Instance.KillButton.isCoolingDown && remainingChange < maxChangeCount;
            },
            () => { },
            AddButton,
            __instance,
            __instance.AbilityButton,
            ModInputManager.secondaryAbilityInput.keyCode,
            buttonText: GetString("gunsmithAddBullets")
        )
        { MaxTimer = 0f };
    }

    public override void Initialize()
    {
        remainingChange = 0;
        KillCooldown = gunsmithKillCooldown.GetFloat();
        setKillCooldown = gunsmithSetKillCooldown.GetFloat();
        maxChangeCount = gunsmithMaxChangeCount.GetInt();
    }
}
