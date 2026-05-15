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

    public static IEnumerable<PlayerControl> AlivePlayers => PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsAlive());

    public static string BuildReport()
    {
        if (Confesser.IsDead()) return GetString("Oracle.ConfesserDead");

        var alivePlayers = AlivePlayers.Where(x => x != PlayerControl.LocalPlayer && x != Confesser).ToList().Shuffle();
        var evilPlayers = AlivePlayers.Where(x => x.IsImpostor() || x.IsKillerNeutral() || x.IsEvilNeutral()).ToList().Shuffle();

        if (alivePlayers.Count <= 2) return GetString("Oracle.TooFewPlayers");

        if (evilPlayers.Count == 0) return string.Format(GetString("Oracle.NoEvilPlayers"), Confesser.Data.PlayerName);

        alivePlayers.Shuffle();
        evilPlayers.Shuffle();

        try
        {
            var flag = rnd.Chance(75);

            if (flag)
            {
                var availablePlayers = Vortox.Reversal
                    ? alivePlayers.Where(x => !x.IsImpostor() && !x.IsKillerNeutral() && !x.IsEvilNeutral()).ToList()
                    : alivePlayers.ToList();

                if (Vortox.Reversal && availablePlayers.Count < 2)
                {
                    return string.Format(GetString("Oracle.ReversalInsufficientPlayers"), Confesser.Data.PlayerName);
                }

                availablePlayers.Shuffle();

                var secondPlayer = availablePlayers[0];

                var firstTwoEvil = evilPlayers.Any(plr => plr == Confesser || plr == secondPlayer);

                if (firstTwoEvil)
                {
                    var thirdPlayer = Vortox.Reversal
                        ? availablePlayers.FirstOrDefault(x => x != secondPlayer) ?? availablePlayers[0]
                        : alivePlayers[1];

                    return string.Format(GetString("Oracle.AtLeastOneEvil"),
                        Confesser.Data.PlayerName,
                        secondPlayer.Data.PlayerName,
                        thirdPlayer.Data.PlayerName);
                }
                else
                {
                    var thirdPlayer = Vortox.Reversal
                        ? availablePlayers.FirstOrDefault(x => x != secondPlayer) ?? availablePlayers[0]
                        : evilPlayers[0];

                    return string.Format(GetString("Oracle.AtLeastOneEvil"),
                        Confesser.Data.PlayerName,
                        secondPlayer.Data.PlayerName,
                        thirdPlayer.Data.PlayerName);
                }
            }
            else
            {
                var players = alivePlayers.OrderBy(x => rnd.Next()).Take(2).ToList();
                players.Add(Confesser);

                var uniformTeam = players.All(x => x.IsNeutral()) || players.All(x => x.IsCrew(AndCat: true)) || players.All(x => x.IsImpostor(AndCat: true));
                if (uniformTeam ^ Vortox.Reversal)
                {
                    return string.Format(GetString("Oracle.ConsistentAlignment"),
                        Confesser.Data.PlayerName,
                        players[0].Data.PlayerName,
                        players[1].Data.PlayerName);
                }
                else
                {
                    return string.Format(GetString("Oracle.InconsistentAlignment"),
                        Confesser.Data.PlayerName,
                        players[0].Data.PlayerName,
                        players[1].Data.PlayerName);
                }
            }
        }
        catch (Exception ex)
        {
            Error(ex);

            return string.Format(GetString("Oracle.ReversalInsufficientPlayers"), Confesser.Data.PlayerName);
        }
    }
}
