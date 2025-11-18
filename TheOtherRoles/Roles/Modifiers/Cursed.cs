namespace TheOtherRoles.Roles.Modifier;

[CustomRpcHolder]
public class Cursed : ModifierBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleinfo = new(
        typeof(Cursed),
        (p) => new Cursed(p),
        RoleId.Cursed,
        "Cursed",
        color,
        403000,
        AddOptions
    );

    public Cursed(PlayerControl p) : base(p, roleinfo) { }

    public static bool hideModifier;

    public static CustomOption modifierAutoJoin;
    public static CustomOption modifierHideCursed;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierHideCursed = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierShowCursed", false, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        hideModifier = modifierHideCursed.GetBool();
    }

    public override bool BeforeMurderPlayer(MurderInfo Info)
    {
        if (Info.Target == Player && Info.Killer.IsImpostor())
        {
            CursedTurn.Invoke(Info.Target);
            return false;
        }
        return true;
    }
    public static RemoteProcess<PlayerControl> CursedTurn = new("CursedTurn", (player, _) =>
    // .public static void TurnToImpostor(byte playerId)
    {

        // RPCProcedure.erasePlayerRoles(playerId);
        // player.GetRole;
        turnToImpostor(player);
    });
}
