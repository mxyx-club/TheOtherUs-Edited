namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Lawyer : RoleBase, INeutral
{
    public static Color color = new Color32(134, 153, 25, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Lawyer),
        (p) => new Lawyer(p),
        RoleId.Lawyer,
        RoleType.Neutral,
        "Lawyer",
        color,
        203300,
        AddOptions
    );

    public Lawyer(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Evil;
    public override bool? HasImpVision => lawyerHasImpVision.GetBool();

    public PlayerControl Target;
    public bool targetWasGuessed;

    public static bool canCallEmergency = true;
    public static bool targetKnows;
    public static bool stolenWin;
    public static bool notAckedExiled;
    public static CustomOption lawyerTargetKnows;
    public static CustomOption lawyerHasImpVision;
    public static CustomOption lawyerKnowsRole;
    public static CustomOption lawyerCanCallEmergency;
    public static CustomOption lawyerStolenWin;
    public static CustomOption lawyerTargetCanBeJester;
    public static bool knowsRole;
    public static bool targetCanBeJester;
    public static HashSet<PlayerControl> RemovePlayer = new();

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        lawyerTargetKnows = CustomOption.Create(configId++, CustomOptionType.Neutral, "lawyerTargetKnows", true, roleinfo.RoleOption);
        lawyerHasImpVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "hasImpVision", true, roleinfo.RoleOption);
        lawyerKnowsRole = CustomOption.Create(configId++, CustomOptionType.Neutral, "lawyerKnowsRole", true, roleinfo.RoleOption);
        lawyerCanCallEmergency = CustomOption.Create(configId++, CustomOptionType.Neutral, "canCallEmergency", true, roleinfo.RoleOption);
        lawyerStolenWin = CustomOption.Create(configId++, CustomOptionType.Neutral, "lawyerStolenWin", false, roleinfo.RoleOption);
        lawyerTargetCanBeJester = CustomOption.Create(configId++, CustomOptionType.Neutral, "lawyerTargetCanBeJester", false, roleinfo.RoleOption);
    }

    public static RemoteProcess<PlayerControl> PromotesToPursuer = new("PromotesToPursuer", (player, _) =>
    // public static void PromotesToPursuer(byte playerId)
    {
        if (player.TryGetRole<Lawyer>(out var lawyer) && player.IsAlive() && lawyer.Target.IsDead())
        {
            CustomRoleManager.CreateRole(player, RoleId.Pursuer);
            lawyer.Destroy();
        }
    });

    public override void Initialize()
    {
        RemovePlayer.Remove(Target);
        Target = null;
        targetKnows = lawyerTargetKnows.GetBool();
        knowsRole = lawyerKnowsRole.GetBool();
        targetCanBeJester = lawyerTargetCanBeJester.GetBool();
        stolenWin = lawyerStolenWin.GetBool();
        canCallEmergency = lawyerCanCallEmergency.GetBool();
    }

    public override bool SeeRoleName(PlayerControl seen, PlayerControl seer)
    {
        if (seen == Player && seer == Target)
        {
            return true;
        }
        return false;
    }

    public override bool SeeRoleTag(PlayerControl seen, PlayerControl seer, out string tag)
    {
        if ((seen == Player || seen == Target || CanSeeGhostInfo) && seer == Target)
        {
            tag = Cs(color, " §");
            return true;
        }
        tag = string.Empty;
        return false;
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        base.OnExiledBegin(exiled);
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (exiled != null && exiled.PlayerId == Target.PlayerId && !Target.Is(RoleId.Jester) && !targetWasGuessed)
        {
            Player.Exiled();
            PlayerData.RpcSetDeathReason(Player, CustomDeathReason.LawyerSuicide, Player);
        }
        else if (Target.IsDead())
        {
            PromotesToPursuer.Invoke(Player);
        }
    }

    public override void OnPlayerDisconnect(PlayerControl player)
    {
        if (player == Target && Player.IsAlive())
        {
            PromotesToPursuer.Invoke(Player);
        }
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Info.Target.PlayerId == Target.PlayerId)
        {
            PromotesToPursuer.Invoke(Player);
        }
    }

}
