namespace TheOtherRoles.Roles.Modifier;

public class Blind : ModifierBase
{
    public static Color color = Color.yellow;
    public static readonly RoleInfo roleinfo = new(
        typeof(Blind),
        (p) => new Blind(p),
        RoleId.Blind,
        "Blind",
        color,
        402400
    );

    public Blind(PlayerControl p) : base(p, roleinfo) { }

    public override void Initialize() { }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (PlayerControl.LocalPlayer == Player)
        {
            DestroyableSingleton<HudManager>.Instance?.ReportButton?.SetActive(false);
        }
    }
}
