namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Infected : RoleBase, INeutral
{
    public static Color color = new Color32(179, 217, 77, byte.MaxValue);
    public static RoleInfo roleinfo = new(
        typeof(Infected),
        (p) => new Infected(p),
        RoleId.Infected,
        RoleType.Neutral,
        "Infected",
        color,
        201600,
        AddOptions,
        IsKiller: true
    );

    public Infected(PlayerControl player) : base(player, roleinfo) { }

    public override bool? CanUseVent => canUseVents.GetBool();
    public override bool? HasImpVision => hasImpostorVision.GetBool();

    public NeutralType NeutralType => NeutralType.Kill;
    public PlayerControl currentTarget;
    public static int MaxPlayer = 3;
    public static int ActiveLimit = 2;
    public static float Cooldown = 30f;
    public static bool IsGuesser;
    public static int GuessCount;

    private static CustomOption killCooldown;
    private static CustomOption maxPlayer;
    private static CustomOption activeLimit;
    private static CustomOption canUseVents;
    private static CustomOption hasImpostorVision;
    private static CustomOption guessCount;
    private static CustomOption canUseGuess;

    public CustomButton InfectedKillButton;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        killCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "killCooldown", 25f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        maxPlayer = CustomOption.Create(configId++, CustomOptionType.Neutral, "infectedMaxPlayer", 3, 1, 15, 1, roleinfo.RoleOption);
        activeLimit = CustomOption.Create(configId++, CustomOptionType.Neutral, "infectedActiveLimit", 2, 1, 15, 1, roleinfo.RoleOption);
        canUseVents = CustomOption.Create(configId++, CustomOptionType.Neutral, "canUseVents", true, roleinfo.RoleOption);
        hasImpostorVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "hasImpVision", true, roleinfo.RoleOption);
        canUseGuess = CustomOption.Create(configId++, CustomOptionType.Neutral, "infectedCanUseGuess", true, roleinfo.RoleOption);
        guessCount = CustomOption.Create(configId++, CustomOptionType.Neutral, "infectedGuessCount", 3, 1, 15, 1, canUseGuess);
    }

    public override void Initialize()
    {
        currentTarget = null;
        Cooldown = killCooldown.GetFloat();
        MaxPlayer = maxPlayer.GetInt();
        ActiveLimit = activeLimit.GetInt();
        ActiveLimit = Mathf.Min(ActiveLimit, MaxPlayer);
        IsGuesser = canUseGuess.GetBool();
        GuessCount = guessCount.GetInt();
    }
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> InfectedKill = new("InfectedKill", (data, _) =>
    // public static void KillPlayer(PlayerControl player, PlayerControl target)
    {
        if (data.player == null || data.target == null) return;

        /*if (SchrodingersCat.Player != null && target == SchrodingersCat.Player && SchrodingersCat.remainingChange > 0)
        {
            RpcCustomMurderPlayer(player, target);
            return;
        }*/

        if (GetRoles(RoleId.Infected).Count() >= MaxPlayer || GetRoles(RoleId.Infected).Count(x => x.IsAlive) >= ActiveLimit || data.target.IsKiller())
        {
            RpcCustomMurderPlayer(data.player, data.target);
        }
        else
        {
            InfectedTarget.Invoke((data.player, data.target));
        }
    });
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> InfectedTarget = new("InfectedTarget", (data, _) =>
    // public static void InfectedTarget(byte playerId, byte targetId)
    {
        CustomRoleManager.RemoveRole(data.target);
        CustomRoleManager.CreateRoleById(data.target.PlayerId, RoleId.Infected);
    });
    public override void CleanUp(HudManager __instance)
    {
        InfectedKillButton?.Destroy();
        InfectedKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        InfectedKillButton?.Destroy();
        InfectedKillButton = new CustomButton(
            () =>
            {
                var target = currentTarget;
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;

                InfectedKill.Invoke((PlayerControl.LocalPlayer, target));

                InfectedKillButton.Timer = InfectedKillButton.MaxTimer;
                currentTarget = null;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsAlive() && PlayerControl.LocalPlayer == Player;
            },
            () =>
            {
                currentTarget = SetTarget(untarget: Player);
                SetPlayerOutline(currentTarget, color);
                InfectedKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));
                return PlayerControl.LocalPlayer.CanMove && currentTarget != null;
            },
            () => { InfectedKillButton.Timer = InfectedKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        )
        {
            MaxTimer = Cooldown,
        };
    }
}