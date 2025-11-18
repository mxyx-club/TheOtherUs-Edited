namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Sidekick : RoleBase, INeutral
{
    public static Color color = new Color32(0, 180, 235, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Sidekick),
        (p) => new Sidekick(p),
        RoleId.Sidekick,
        RoleType.Neutral,
        "Sidekick",
        color,
        206200,
        null,
        false,
        true
    );

    public Sidekick(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Kill;

    public override bool? CanUseVent => Jackal.sidekickCanUseVents.GetBool();
    public override bool? HasImpVision => Jackal.jackalAndSidekickHaveImpostorVision.GetBool();
    public override bool? IsKiller => true;

    public PlayerControl MyJackal;
    public PlayerControl currentTarget;

    public static float cooldown;
    public static bool sidekickCanKill;
    public static bool promotesToJackal;
    public static bool promotedFromSidekickCanCreateSidekick;

    public CustomButton KillButton;
    public static RemoteProcess<PlayerControl> SidekickPromotes = new("SidekickPromotes", (player, _) =>
    {
        if (player == null) return;
        CustomRoleManager.RemoveRole(player);
        var role = CustomRoleManager.CreateRoleById(player.PlayerId, RoleId.Jackal) as Jackal;
        role!.MySidekick = null;
        role.CanCreateSidekick = Sidekick.promotedFromSidekickCanCreateSidekick;
    });

    public override void Initialize()
    {
        sidekickCanKill = Jackal.sidekickCanKill.GetBool();
        promotesToJackal = Jackal.sidekickPromotesToJackal.GetBool();
        cooldown = Jackal.jackalKillCooldown.GetFloat();
        promotedFromSidekickCanCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick.GetBool();

        HasImpVision = Jackal.jackalAndSidekickHaveImpostorVision.GetBool();
        CanUseVent = Jackal.sidekickCanUseVents.GetBool();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
        if ((Player.TryGetRole<Jackal>(out var jackal) && jackal.MySidekick.IsDead()) || !promotesToJackal || jackal.MySidekick != player) return;
        if (player != jackal.MySidekick && player.IsDead())
        {
            SidekickPromotes.Invoke(jackal.MySidekick);
        }

        // jackal Set Target
        if (Player.IsAlive())
        {
            var untargetablePlayers = new HashSet<PlayerControl>();
            var role = player.GetRole();
            if (role is RoleId.Jackal or RoleId.Sidekick) untargetablePlayers.Add(player);
            if (player.TryGetModifier<Mini>(out var mini) && !mini.isGrownUp()) untargetablePlayers.Add(player);
            currentTarget = SetTarget(untargetablePlayers);
            SetPlayerOutline(currentTarget, Palette.ImpostorRed);
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        KillButton?.Destroy();
        KillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        KillButton?.Destroy();
        KillButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, currentTarget, true, false, CustomDeathReason.Kill);

                KillButton.Timer = KillButton.MaxTimer;
            },
            () =>
            {
                return Player.IsAlive() && PlayerControl.LocalPlayer == Player && sidekickCanKill;
            },
            () =>
            {
                currentTarget = SetTarget();
                KillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));
                return currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { KillButton.Timer = KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
           ModInputManager.modKillInput.keyCode
        )
        {
            MaxTimer = cooldown,
        };
    }
}
