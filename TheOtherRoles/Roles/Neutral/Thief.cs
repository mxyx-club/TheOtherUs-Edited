namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Thief : RoleBase, INeutral
{
    public static Color color = new Color32(71, 99, 45, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Thief),
        (p) => new Thief(p),
        RoleId.Thief,
        RoleType.Neutral,
        "Thief",
        color,
        203700,
        AddOptions
    );

    public Thief(PlayerControl player) : base(player, roleinfo, true) { }

    public NeutralType NeutralType => NeutralType.Evil;
    public override bool? IsKiller => true;
    public override bool? CanUseVent => thiefCanUseVents.GetBool();
    public override bool? HasImpVision => thiefHasImpVision.GetBool();

    public PlayerControl currentTarget;
    public PlayerControl formerThief;

    public static float cooldown = 30f;

    public static bool canKillSheriff;
    public static bool canKillDeputy;
    public static bool canKillVeteran;
    public static bool canStealWithGuess;

    public static CustomOption thiefCooldown;
    public static CustomOption thiefCanKillSheriff;
    public static CustomOption thiefCanKillDeputy;
    public static CustomOption thiefCanKillVeteran;
    public static CustomOption thiefHasImpVision;
    public static CustomOption thiefCanUseVents;
    public static CustomOption thiefCanStealWithGuess;

    public CustomButton thiefKillButton;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        thiefCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "killCooldown", 25f, 5f, 120f, 2.5f, roleinfo.RoleOption);
        thiefCanKillSheriff = CustomOption.Create(configId++, CustomOptionType.Neutral, $"{"thiefCanKill".Translate()}{Cs(Sheriff.color, "Sheriff".Translate())}", true, roleinfo.RoleOption);
        thiefCanKillDeputy = CustomOption.Create(configId++, CustomOptionType.Neutral, $"{"thiefCanKill".Translate()}{Cs(Sheriff.color, "Deputy".Translate())}", true, roleinfo.RoleOption);
        thiefCanKillVeteran = CustomOption.Create(configId++, CustomOptionType.Neutral, $"{"thiefCanKill".Translate()}{Cs(Veteran.color, "Veteran".Translate())}", true, roleinfo.RoleOption);
        thiefHasImpVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "hasImpVision", true, roleinfo.RoleOption);
        thiefCanUseVents = CustomOption.Create(configId++, CustomOptionType.Neutral, "canUseVents", true, roleinfo.RoleOption);
        thiefCanStealWithGuess = CustomOption.Create(configId++, CustomOptionType.Neutral, "thiefCanStealWithGuess", true, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        currentTarget = null;
        formerThief = null;
        cooldown = thiefCooldown.GetFloat();
        canKillSheriff = thiefCanKillSheriff.GetBool();
        canKillDeputy = thiefCanKillDeputy.GetBool();
        canKillVeteran = thiefCanKillVeteran.GetBool();
        canStealWithGuess = thiefCanStealWithGuess.GetBool();
    }

    public bool thiefCanKill(PlayerControl target)
    {
        var targetRole = target.GetRole();
        return target.IsImpostor() ||
               targetRole == RoleId.Jackal ||
               targetRole == RoleId.Sidekick ||
               targetRole == RoleId.Werewolf ||
               targetRole == RoleId.Juggernaut ||
               targetRole == RoleId.Swooper ||
               targetRole == RoleId.Pavlovsdogs ||
               targetRole == RoleId.Pavlovsowner ||
               targetRole == RoleId.Infected ||
               (canKillSheriff && targetRole == RoleId.Sheriff) ||
               (canKillDeputy && targetRole == RoleId.Deputy) ||
               (canKillVeteran && targetRole == RoleId.Veteran);
    }

    public static RemoteProcess<(PlayerControl player, PlayerControl target)> ThiefStealsRole = new("ThiefStealsRole", (data, _) =>
    // public static void StealsRole(byte playerId, byte targetId)
    {
        var role = data.target.GetRole();

        CustomRoleManager.RemoveRole(data.player);
        CustomRoleManager.CreateRole(data.player, role);

        //formerThief = thief; // After clearAndReload, else it would get reset...
    });

    public override void CleanUp(HudManager __instance)
    {
        thiefKillButton?.Destroy();
        thiefKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        thiefKillButton?.Destroy();
        thiefKillButton = new CustomButton(
            () =>
            {
                var thief = Player;
                var target = currentTarget;

                if (!thiefCanKill(target))
                {
                    // Suicide
                    RpcCustomMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false, false);
                }
                else
                {
                    // Steal role if survived.
                    if (Player.IsAlive() && RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target, true, false))
                    {
                        ThiefStealsRole.Invoke((PlayerControl.LocalPlayer, target));
                    }
                }
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                var untargetables = new List<PlayerControl>();
                untargetables.AddRange(Mini.UngrownMinis);
                currentTarget = SetTarget(untarget: untargetables);
                SetPlayerOutline(currentTarget, color);

                return currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { thiefKillButton.Timer = thiefKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
