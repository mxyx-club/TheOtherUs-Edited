namespace TheOtherRoles.Roles.Impostor;

public class Mimic : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Mimic),
        (p) => new Mimic(p),
        RoleId.Mimic,
        RoleType.Impostor,
        "Mimic",
        color,
        101700,
        AddOptions
    );

    public Mimic(PlayerControl p) : base(p, roleInfo) { }

    public bool HasMimic;
    public RoleBase TargetRole;
    public static RoleId[] ImitatableRoles =
    [
        RoleId.BodyGuard,
        RoleId.Mayor,
        RoleId.Prosecutor,
        RoleId.InfoSleuth,
        RoleId.Trapper,
        RoleId.Portalmaker,
        RoleId.Engineer,
        RoleId.Jumper,
        RoleId.Detective,
        RoleId.Veteran,
        RoleId.Medic,
        RoleId.Swapper,
        RoleId.Seer,
        RoleId.Hacker,
        RoleId.Tracker,
        RoleId.SecurityGuard,
        RoleId.Medium,
        RoleId.Balancer,
        RoleId.Prophet,
        RoleId.Redemptor,
    ];

    public static void AddOptions()
    {
        // No options for Mimic
        var configId = roleInfo.ConfigId + 2;
    }

    public override void Initialize()
    {
        HasMimic = false;
        TargetRole.Destroy();
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        TargetRole?.OnMurderPlayer(Info);
        if (Info.Killer == Player && !HasMimic)
        {
            var newRole = Info.Target.GetRole();
            if (ImitatableRoles.Any(x => x == newRole))
            {
                HasMimic = true;
                TargetRole = CustomRoleManager.CreateRoleById(Player.PlayerId, newRole);
                TargetRole.OnGameStart();
                Message($"模仿职业: {newRole}");
            }
        }
    }
}
