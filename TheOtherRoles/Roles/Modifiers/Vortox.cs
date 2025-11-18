namespace TheOtherRoles.Roles.Modifier;

public class Vortox : ModifierBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleinfo = new(
        typeof(Vortox),
        (p) => new Vortox(p),
        RoleId.Vortox,
        "Vortox",
        color,
        403800,
        AddOptions
    );

    public static List<Vortox> Players = new();
    public Vortox(PlayerControl p) : base(p, roleinfo) { Players.Add(this); }

    public static bool skipMeeting;
    public static int skipMeetingNum;
    public static int skipCount;
    public static bool triggerImpWin;
    public static bool reversal;

    public static CustomOption modifierVortoxReversal;
    public static CustomOption modifierVortoxSkipMeeting;
    public static CustomOption modifierVortoxSkipNum;

    public static bool Reversal => reversal && Players.Any(x => x.Player.IsAlive());

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        modifierVortoxReversal = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierVortoxReversal", true, roleinfo.RoleOption);
        modifierVortoxSkipMeeting = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierVortoxSkipMeeting", true, roleinfo.RoleOption);
        modifierVortoxSkipNum = CustomOption.Create(configId++, CustomOptionType.Modifiers, "modifierVortoxSkipNum", 4, 1, 10, 1, modifierVortoxSkipMeeting);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Players.Remove(this);
    }

    public override void Initialize()
    {
        triggerImpWin = false;
        reversal = modifierVortoxReversal.GetBool();
        skipMeeting = modifierVortoxSkipMeeting.GetBool();
        skipMeetingNum = modifierVortoxSkipNum.GetInt();
        skipCount = 0;
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (Player.IsAlive() && exiled == null)
        {
            skipCount++;
            if (skipCount == skipMeetingNum) triggerImpWin = true;
        }
    }
}
