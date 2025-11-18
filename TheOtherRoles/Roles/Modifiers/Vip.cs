namespace TheOtherRoles.Roles.Modifier;

public class Vip : ModifierBase
{
    public static Color color = Color.yellow;
    public static bool showColor = true;
    public static CustomOption modifierVipShowColor;
    public static readonly RoleInfo roleinfo = new(
        typeof(Vip),
        (p) => new Vip(p),
        RoleId.Vip,
        "Vip",
        color,
        403100,
        AddOptions
    );
    public Vip(PlayerControl p) : base(p, roleinfo) { }
    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierVipShowColor = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierVipShowColor", true, roleinfo.RoleOption);
    }
    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Player.PlayerId == Info.Target.PlayerId)
        {
            var color = Color.yellow;
            if (showColor)
            {
                color = Color.white;
                if (Info.Target.Data.Role.IsImpostor) color = Color.red;
                else if (Info.Target.GetRoleInfo().RoleType == RoleType.Neutral) color = Color.blue;
            }

            showFlash(color, 1.25f);
        }
    }
    public override void Initialize()
    {
        showColor = modifierVipShowColor.GetBool();
    }
}
