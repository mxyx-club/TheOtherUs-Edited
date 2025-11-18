namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Executioner : RoleBase, INeutral
{
    public static Color color = new Color32(140, 64, 5, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Executioner),
        (p) => new Executioner(p),
        RoleId.Executioner,
        RoleType.Neutral,
        "Executioner",
        color,
        203400,
        AddOptions
    );

    public Executioner(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Evil;

    public PlayerControl Target;
    public bool triggerExecutionerWin;
    public bool targetWasGuessed;

    public static bool canCallEmergency;
    public static bool promotesToLawyer;
    public static int OnTargetDead;
    public static RoleId[] NewRole =
    [
        RoleId.Pursuer,
        RoleId.Jester,
        RoleId.Amnisiac,
        RoleId.Survivor,
        RoleId.Crewmate,
    ];

    public static CustomOption executionerCanCallEmergency;
    public static CustomOption executionerPromotesToLawyer;
    public static CustomOption executionerOnTargetDead;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        executionerCanCallEmergency = CustomOption.Create(configId++, CustomOptionType.Neutral, "canCallEmergency", true, roleinfo.RoleOption);
        executionerPromotesToLawyer = CustomOption.Create(configId++, CustomOptionType.Neutral, "executionerPromotesToLawyer", true, roleinfo.RoleOption);
        executionerOnTargetDead = CustomOption.Create(configId++, CustomOptionType.Neutral, "目标死亡后变为", [Cs(Pursuer.color, "Pursuer"), Cs(Jester.color, "Jester"), Cs(Amnisiac.color, "Amnisiac"), Cs(Survivor.color, "Survivor"), "Crewmate"], roleinfo.RoleOption);
    }

    public static RemoteProcess<PlayerControl> PromotesRole = new("PromotesRole", (player, _) =>
    // public static void PromotesRole(byte playerId)
    {
        // var player = PlayerById(playerId);
        var role = player.GetRole<Executioner>();
        if (role != null && player.IsAlive() && role.Target.IsDead())
        {
            role.Destroy();
            var newRole = NewRole[OnTargetDead];
            CustomRoleManager.CreateRole(player, newRole);
        }
    });
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> ExecutionerSetTarget = new("ExecutionerSetTarget", (data, _) =>
    {
        var role = data.player?.GetRole<Executioner>();
        if (role != null)
        {
            role.Target = data.target;
        }
    });

    public override void Initialize()
    {
        Target = null;
        triggerExecutionerWin = false;
        canCallEmergency = executionerCanCallEmergency.GetBool();
        promotesToLawyer = executionerPromotesToLawyer.GetBool();
        OnTargetDead = executionerOnTargetDead.GetSelection();
    }

    public override bool SeeRoleTag(PlayerControl seen, PlayerControl seer, out string tag)
    {
        if (Player.IsAlive() && Target.IsAlive() && (seen == Player || CanSeeGhostInfo))
        {
            tag = Cs(color, " §");
            return true;
        }
        tag = string.Empty;
        return false;
    }

    public void SetTarget()
    {
        if (Player != PlayerControl.LocalPlayer) return;
        var possibleTargets = new List<PlayerControl>();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsAlive() && !p.IsLover() && !p.Is(RoleId.Mini) && p.IsCrew() && !p.Is(RoleId.Swapper))
                possibleTargets.Add(p);
        }

        if (possibleTargets.Count == 0)
        {
            PromotesRole.Invoke(Player);
        }
        else
        {
            var target = possibleTargets.Random();
            ExecutionerSetTarget.Invoke((Player, target));
        }
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (exiled != null && exiled.PlayerId == Target.PlayerId && !targetWasGuessed)
        {
            triggerExecutionerWin = true;
        }
        else if (Target.IsDead() && Player.AmOwner)
        {
            PromotesRole.Invoke(Player);
        }
    }

    public override void OnPlayerDeath(PlayerControl player, bool isOnMeeting = false)
    {
        if (player == Target)
        {
            PromotesRole.Invoke(Player);
        }
    }

    public override void OnPlayerDisconnect(PlayerControl player)
    {
        if (player == Target)
        {
            PromotesRole.Invoke(Player);
        }
    }
}
