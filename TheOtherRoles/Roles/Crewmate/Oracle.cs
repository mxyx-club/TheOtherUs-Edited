namespace TheOtherRoles.Roles.Crewmate;

public class Oracle
{
    public static PlayerControl Player;
    public static Color32 color = new(191, 0, 191, byte.MaxValue);
    public static Sprite ConfessSprite = new ResourceSprite("ConfessButton.png");

    public static PlayerControl CurrentTarget;
    public static PlayerControl Confesser;
    public static CRoleType ConfesserType;

    public static float ConfessCooldown;
    public static int RevealAccuracyRate;
    public static bool CanNotGuessConfess;

    public enum CRoleType
    {
        None,
        Crewmate,
        Impostor,
        Neutral,
    }

    public static IEnumerable<PlayerControl> AlivePlayers => PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsAlive());

    public static void ClearAndReload()
    {
        Player = null;
        CurrentTarget = null;
        Confesser = null;
        ConfesserType = CRoleType.None;
        ConfessCooldown = CustomOptionHolder.oracleConfessCooldown.GetFloat();
        RevealAccuracyRate = CustomOptionHolder.oracleRevealAccuracyRate.GetInt();
        CanNotGuessConfess = CustomOptionHolder.oracleCanNotGuessConfess.GetBool();
    }

    public static void CheckConfesserTeam()
    {
        if (Player != null && Confesser != null && Player.Data.IsDead && ConfesserType == CRoleType.None && Player.AmOwner)
        {
            var acc = rnd.Chance(RevealAccuracyRate);
            var roletype = CRoleType.None;
            if (Confesser.IsNeutral()) roletype = CRoleType.Neutral;
            else if (Confesser.IsImpostor(AndCat: true)) roletype = CRoleType.Impostor;
            else if (Confesser.IsCrew(AndCat: true)) roletype = CRoleType.Crewmate;

            var roleTypeList = EnumHelper.GetAllValues<CRoleType>().ToList();
            roleTypeList.Remove(roletype);
            if (!acc) roletype = roleTypeList.Shuffle().First();

            var writer = StartRPC(CustomRPC.SetConfesser);
            writer.Write(Confesser.PlayerId);
            writer.Write((int)roletype);
            writer.EndRPC();
            ConfesserType = roletype;
            Message($"Confesser RoleType: {roletype}, Acc {acc}");
        }
    }


    public static void SendOracleReport()
    {
        if (Confesser == null || Player.IsDead() || !Player.AmOwner) return;

        var text = BuildReport();
        var writer = StartRPC(CustomRPC.SendOracleReport);
        writer.Write(Confesser.PlayerId);
        writer.Write(text);
        writer.EndRPC();
        HudManager.Instance.Chat.AddChat(Player, text);
    }


    public static string BuildReport()
    {
        if (Confesser.IsDead()) return "你的信徒已经死亡，你无法获得信息！";


        var alivePlayers = AlivePlayers.Where(x => x != PlayerControl.LocalPlayer && x != Confesser).ToList().Shuffle();
        var evilPlayers = AlivePlayers.Where(x => x.IsImpostor() || x.IsKillerNeutral() || x.IsEvilNeutral()).ToList().Shuffle();

        if (alivePlayers.Count <= 2) return "剩余人数太少，无法获得信息！";

        if (evilPlayers.Count == 0) return $"你的信徒 {Confesser.Data.PlayerName} 得知场上没有邪恶的玩家了！";

        alivePlayers.Shuffle();
        evilPlayers.Shuffle();

        try
        {

            var flag = rnd.Chance(80);

            if (flag)
            {
                var secondPlayer = alivePlayers[0];
                var firstTwoEvil = evilPlayers.Any(plr => plr == Confesser || plr == secondPlayer);

                if (firstTwoEvil)
                {
                    var thirdPlayer = alivePlayers[1];
                    return $"你的信徒 {Confesser.Data.PlayerName} 坦白得知自己、{secondPlayer.Data.PlayerName} 、{thirdPlayer.Data.PlayerName} 至少有一位是邪恶阵营的！";
                }
                else
                {
                    var thirdPlayer = evilPlayers[0];
                    return $"你的信徒 {Confesser.Data.PlayerName} 坦白得知自己、{secondPlayer.Data.PlayerName} 、{thirdPlayer.Data.PlayerName} 至少有一位是邪恶阵营的！";
                }
            }
            else
            {
                var players = alivePlayers.OrderBy(x => rnd.Next()).Take(2).ToList();
                players.Add(Confesser);

                var uniformTeam = players.All(x => x.IsNeutral()) || players.All(x => x.IsCrew(AndCat: true)) || players.All(x => x.IsImpostor(AndCat: true));
                if (uniformTeam)
                {
                    return $"你的信徒 {Confesser.Data.PlayerName} 坦白得知自己、{players[0].Data.PlayerName} 、{players[1].Data.PlayerName} 的阵营是一致的！";
                }
                else
                {
                    return $"你的信徒 {Confesser.Data.PlayerName} 坦白得知自己、{players[0].Data.PlayerName} 、{players[1].Data.PlayerName} 的阵营是不一致的！";
                }
            }
        }
        catch (Exception ex)
        {
            Error(ex);

            var players = alivePlayers.OrderBy(x => rnd.Next()).Take(1).ToList();
            players.Add(Confesser);

            var uniformTeam = players.All(x => x.IsNeutral()) || players.All(x => x.IsCrew(AndCat: true)) || players.All(x => x.IsImpostor(AndCat: true));
            if (uniformTeam)
            {
                return $"你的信徒 {Confesser.Data.PlayerName} 坦白得知自己、{players[0].Data.PlayerName} 的阵营是一致的！";
            }
            else
            {
                return $"你的信徒 {Confesser.Data.PlayerName} 坦白得知自己、{players[0].Data.PlayerName} 的阵营是不一致的！";
            }
        }
    }
}
