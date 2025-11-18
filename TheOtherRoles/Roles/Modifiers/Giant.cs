namespace TheOtherRoles.Roles.Modifier;

public class Giant : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Giant),
        (p) => new Giant(p),
        RoleId.Giant,
        "Giant",
        color,
        402200,
        AddOptions
    );

    public Giant(PlayerControl p) : base(p, roleinfo) { }

    public static float speed = 0.75f;
    public static readonly float size = 1.05f;

    public static CustomOption modifierGiantSpped;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierGiantSpped = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierGiantSpped", 0.75f, 0.5f, 1.5f, 0.05f, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        speed = modifierGiantSpped.GetFloat();
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (!isCamoComms && !isActiveCamoComms && !MushroomSabotageActive &&
            Player.TryGetRole<Morphling>(out var morphling) && morphling.morphTimer == 0f)
        {
            var collider = Player.Collider.CastFast<CircleCollider2D>();
            collider.offset = 0.3636057f * Vector2.down;

            Player.transform.localScale = new Vector3(size, size, 1f);
            collider.radius = 0.2233912f * 0.85f;
        }
        else if (Player.GetModifier().Contains(RoleId.Mini))
        {
            Player.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        }

        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        foreach (var body in array.Where(x => x.ParentId == Player.PlayerId))
        {
            try
            {
                body.transform.localScale = new Vector3(size, size, 1f);
            }
            catch { }
        }
    }
}
