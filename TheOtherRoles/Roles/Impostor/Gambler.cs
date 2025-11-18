namespace TheOtherRoles.Roles.Impostor;
public class Gambler : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Gambler),
        (p) => new Gambler(p),
        RoleId.Gambler,
        RoleType.Impostor,
        "Gambler",
        color,
        103300,
        AddOptions
    );

    public Gambler(PlayerControl p) : base(p, roleInfo) { }

    public static float minCooldown;
    public static float maxCooldown;
    public static int successRate;
    public static CustomOption gamblerMinCooldown;
    public static CustomOption gamblerMaxCooldown;
    public static CustomOption gamblerSuccessRate;

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        gamblerMinCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "gamblerMinCooldown", 2.5f, 0f, 45f, 0.5f, roleInfo.RoleOption);
        gamblerMaxCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "gamblerMaxCooldown", 40f, 10f, 90f, 2.5f, roleInfo.RoleOption);
        gamblerSuccessRate = CustomOption.Create(configId++, CustomOptionType.Impostor, "gamblerSuccessRate", CustomOptionHolder.rates, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        minCooldown = gamblerMinCooldown.GetFloat();
        maxCooldown = gamblerMaxCooldown.GetFloat();
        successRate = gamblerSuccessRate.GetSelection() * 10;
    }
    public static bool GetSuc()
    {
        var rate = rnd.Next(0, 100);
        Message($"Gambler rate {rate} : {successRate}");
        return rate < successRate;
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Info.Killer == Player)
        {
            var cooldown = GetSuc() ? minCooldown : maxCooldown;
            Player.SetKillTimer(cooldown);
        }
    }
}
