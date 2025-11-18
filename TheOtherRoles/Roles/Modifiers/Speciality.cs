namespace TheOtherRoles.Roles.Modifier;

public class Specoality : ModifierBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleinfo = new(
        typeof(Specoality),
        (p) => new Specoality(p),
        RoleId.Specoality,
        "Specoality",
        color,
        403500,
        AddOptions
    );

    public int linearfunction = 1;
    public static bool IsGlobalModifier;

    public static CustomOption modifierSpecoalityIsGlobal;

    public Specoality(PlayerControl p) : base(p, roleinfo) { }

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierSpecoalityIsGlobal = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierSpecoalityIsGlobal", false, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        linearfunction = 1;
        IsGlobalModifier = modifierSpecoalityIsGlobal.GetBool();
    }
}
