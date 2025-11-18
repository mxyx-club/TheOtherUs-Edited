namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class InfoSleuth : RoleBase
{
    public static Color color = new Color32(200, 105, 228, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(InfoSleuth),
        (p) => new InfoSleuth(p),
        RoleId.InfoSleuth,
        RoleType.Crewmate,
        "InfoSleuth",
        color,
        303800,
        AddOptions
    );

    public InfoSleuth(PlayerControl p) : base(p, roleinfo) { }

    public PlayerControl target;

    public static int infoType = 1;
    public static CustomOption infoSleuthInfoType;
    public static RemoteProcess<(byte playerId, byte targetId)> InfoSleuthSetTarget = new("InfoSleuthSetTarget", (data, _) =>
    {
        var player = PlayerById(data.playerId);
        var target = PlayerById(data.targetId);
        if (player == null || target == null) return;

        if (player.TryGetRole<InfoSleuth>(out var infoSleuth))
        {
            infoSleuth.target = null;
            infoSleuth.target = target;
        }
    });
    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        infoSleuthInfoType = CustomOption.Create(configId++, CustomOptionType.Crewmate, "infoSleuthInfoType",
            ["infoSleuthInfoType1", "infoSleuthInfoType2", "infoSleuthInfoType3"], roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        target = null;
        infoType = infoSleuthInfoType.GetSelection();
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        var isNotCrew = (target.IsNeutral() || target.IsImpostor()) ^ Vortox.reversal;

        var info = infoType switch
        {
            0 => GetString(isNotCrew ? "InfoSleuth.NotCrew" : "InfoSleuth.Crew"),
            1 => string.Format(GetString("InfoSleuth.Team"), GetTeamString(target)),
            _ => rnd.Next(2) == 0 ? GetString(isNotCrew ? "InfoSleuth.NotCrew" : "InfoSleuth.Crew")
                                  : string.Format(GetString("InfoSleuth.Team"), GetTeamString(target))
        };

        string msg = $"{target.Data.PlayerName} {info}";

        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");
        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ShareGhostInfo);
        writer.Write(Player.PlayerId);
        writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
        writer.Write(msg);
        writer.EndRPC();

        InfoSleuthSetTarget.Invoke((Player.PlayerId, byte.MaxValue));

        static string GetTeamString(PlayerControl player)
        {
            if (Vortox.Reversal)
            {
                if (player.IsCrew()) return rnd.Next(2) == 0 ? "NeutralRolesText".Translate() : "ImpostorRolesText".Translate();
                if (player.IsNeutral() || player.IsImpostor()) return "CrewmateRolesText".Translate();
            }

            return player.IsNeutral() ? "NeutralRolesText".Translate()
                : player.IsImpostor() ? "ImpostorRolesText".Translate()
                : "CrewmateRolesText".Translate();
        }
    }
}