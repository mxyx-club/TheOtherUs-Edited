using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Modifier;

public class Bloody : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Bloody),
        (p) => new Bloody(p),
        RoleId.Bloody,
        "Bloody",
        color,
        401200,
        AddOptions
    );

    public Bloody(PlayerControl p) : base(p, roleinfo) { }

    public static Dictionary<byte, float> active = new();
    public static Dictionary<byte, byte> bloodyKillerMap = new();
    public static float duration = 5f;

    public static CustomOption modifierBloodyDuration;

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierBloodyDuration = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierBloodyDuration", 10f, 3f, 60f, 0.5f, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        active = new();
        bloodyKillerMap = new();
        duration = modifierBloodyDuration.GetFloat();
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Info.Target == Player)
        {
            Bloodytrail.StartBloodTrail(Info.Killer, Info.Target);
        }
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (!active.Any()) return;
        foreach (var p in new Dictionary<byte, float>(active))
        {
            var entry = PlayerById(p.Key);
            var bloodyPlayer = PlayerById(bloodyKillerMap[entry.PlayerId]);

            active[p.Key] = p.Value - Time.fixedDeltaTime;
            if (p.Value <= 0 || entry.IsDead())
            {
                // Skip the creation of the next blood drop, if the killer is dead or the time is up
                active.Remove(p.Key);
                continue;
            }

            _ = new Bloodytrail(entry, bloodyPlayer);
        }
    }
}
