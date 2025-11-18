namespace TheOtherRoles.Roles.Impostor;

public class Warlock : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Warlock),
        (p) => new Warlock(p),
        RoleId.Warlock,
        RoleType.Impostor,
        "Warlock",
        color,
        102200,
        AddOptions
    );

    public Warlock(PlayerControl p) : base(p, roleInfo) { }

    public PlayerControl currentTarget;
    public PlayerControl curseVictim;
    public PlayerControl curseVictimTarget;
    public static float cooldown = 30f;
    public static float rootTime = 5f;

    public static CustomOption warlockCooldown;
    public static CustomOption warlockRootTime;

    public CustomButton warlockCurseButton;
    public static Sprite curseButtonSprite = new ResourceSprite("CurseButton.png");
    public static Sprite curseKillButtonSprite = new ResourceSprite("CurseKillButton.png");

    private static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        warlockCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "warlockCooldown", 20f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        warlockRootTime = CustomOption.Create(configId++, CustomOptionType.Impostor, "warlockRootTime", 3f, 0f, 15f, 0.25f, roleInfo.RoleOption);
    }
    public override void Initialize()
    {
        currentTarget = null;
        curseVictim = null;
        curseVictimTarget = null;
        cooldown = warlockCooldown.GetFloat();
        rootTime = warlockRootTime.GetFloat();
    }

    public void resetCurse()
    {
        warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
        warlockCurseButton.Sprite = curseButtonSprite;
        warlockCurseButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        currentTarget = null;
        curseVictim = null;
        curseVictimTarget = null;
    }
    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Player != null && PlayerControl.LocalPlayer == Player &&
            Info.Killer == Player && warlockCurseButton != null)
            if (Player.killTimer > warlockCurseButton.Timer)
                warlockCurseButton.Timer = Player.killTimer;
    }

    public override void CleanUp(HudManager __instance)
    {
        warlockCurseButton?.Destroy();
        warlockCurseButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Warlock curse
        warlockCurseButton?.Destroy();
        warlockCurseButton = new CustomButton(
            () =>
            {
                if (curseVictim == null)
                {
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                    // Apply Curse
                    curseVictim = currentTarget;
                    warlockCurseButton.Sprite = curseKillButtonSprite;
                    warlockCurseButton.Timer = 1f;
                    SoundEffectsManager.play("warlockCurse");
                }
                else if (curseVictim != null && curseVictimTarget != null)
                {
                    RpcCustomMurderPlayer(Player, curseVictimTarget, false);

                    // If blanked or killed
                    if (rootTime > 0)
                    {
                        PlayerControl.LocalPlayer.moveable = false;
                        // Stop current movement so the warlock is not just running straight into the next object
                        PlayerControl.LocalPlayer.NetTransform.Halt();
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(rootTime,
                            new Action<float>(p =>
                            {
                                // Delayed action
                                if (p == 1f) PlayerControl.LocalPlayer.moveable = true;
                            })));
                    }

                    curseVictim = null;
                    curseVictimTarget = null;
                    warlockCurseButton.Sprite = curseButtonSprite;
                    Player.killTimer = warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
                }
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
                if (curseVictim != null && curseVictim.IsDead()) resetCurse();
                if (curseVictim == null)
                {
                    currentTarget = SetTarget();
                    SetPlayerOutline(currentTarget, color);
                }
                else
                {
                    curseVictimTarget = SetTarget(targetingPlayer: curseVictim);
                    SetPlayerOutline(curseVictimTarget, color);
                }

                if (curseVictim != null)
                    warlockCurseButton.showTargetNameOnButton(currentTarget, GetString("CurseKillText"));
                else
                    warlockCurseButton.showTargetNameOnButton(currentTarget, GetString("CurseText"));
                return ((curseVictim == null && currentTarget != null) ||
                        (curseVictim != null && curseVictimTarget != null)) &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
                warlockCurseButton.Sprite = curseButtonSprite;
                curseVictim = null;
                curseVictimTarget = null;
            },
            curseButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("CurseText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
