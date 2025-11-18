using AmongUs.GameOptions;

namespace TheOtherRoles.Patches;

internal enum CustomGameOverReason
{
    Canceled = 10,
    CrewmateWin,
    ImpostorWin,
    LoversWin,
    TeamJackalWin,
    TeamPavlovsWin,
    MiniLose,
    JesterWin,
    WitnessWin,
    ArsonistWin,
    PelicanWin,
    VultureWin,
    LawyerSoloWin,
    ExecutionerWin,
    SwooperWin,
    WerewolfWin,
    JuggernautWin,
    DoomsayerWin,
    AkujoWin,
    BandLeaderWin,
}

internal enum WinCondition
{
    Canceled = -1,
    Default,
    CrewmateWin,
    ImpostorWin,
    MiniLose,
    EveryoneDied,
    TaskerWin,
    LoversTeamWin,
    LoversSoloWin,
    JesterWin,
    JackalWin,
    WitnessWin,
    PavlovsWin,
    SwooperWin,
    PelicanWin,
    ArsonistWin,
    VultureWin,
    LawyerSoloWin,
    BandLeaderWin,
    AdditionalLawyerBonusWin,
    AdditionalLawyerStolenWin,
    AdditionalAlivePursuerWin,
    AdditionalAliveSurvivorWin,
    AdditionalPartTimerWin,
    ExecutionerWin,
    WerewolfWin,
    JuggernautWin,
    DoomsayerWin,
    AkujoSoloWin,
    AkujoTeamWin,
}

internal static class AdditionalTempData
{
    // Should be implemented using a proper GameOverReason in the future
    public static WinCondition winCondition = WinCondition.Default;
    public static List<WinCondition> additionalWinConditions = new();
    public static List<PlayerRoleInfo> playerRoles = new();
    public static float timer;
    public static string GameEndString = "";

    public static void clear()
    {
        GameEndString = "";
        playerRoles.Clear();
        additionalWinConditions.Clear();
        winCondition = WinCondition.Default;
        timer = 0;
    }

    internal class PlayerRoleInfo
    {
        public string PlayerName { get; set; }
        public RoleInfo Role { get; set; }
        public string RoleNames { get; set; }
        public bool IsAlive { get; set; }
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
public class OnGameEndPatch
{
    private static GameOverReason gameOverReason;

    public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        gameOverReason = endGameResult.GameOverReason;
        if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorByKill;

        // Reset zoomed out ghosts
        toggleZoom(true);
    }

    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        /*try
        {
            if (!AmongUsClient.Instance.AmHost) return;
            PlayerData.WinCondition = AdditionalTempData.winCondition;
            PlayerData.EndTime = DateTime.UtcNow;
            foreach (var data in PlayerData.AllPlayerData)
            {
                if (data?.Player?.Data == null) continue;
                data.AllRole = [];
                data.Role = data.Player.GetRoleInfo()?.RoleId ?? RoleId.DefaultRole;
                //data.IsWinner = winners.Any(x => x.PlayerId == data.PlayerId);
                data.TaskCount = TasksHandler.taskInfo(data.Player.Data);
            }
            PlayerData.SaveAllPlayerDataToJson();
        }
        catch (Exception e)
        {
            Error($"Failed to set PlayerData: {e.Message}");
        }*/

        /*AdditionalTempData.clear();

        var table = new SimpleTable()
            .AddColumn(alignment: Alignment.Left)
            .AddColumn(alignment: Alignment.Left)
            .AddColumn(alignment: Alignment.Left)
            .AddColumn(alignment: Alignment.Left)
            .AddRow();

        var AllPlayers = PlayerControl.AllPlayerControls.ToList();

        foreach (var p in AllPlayers)
        {
            var playerName = Cs(p.IsAlive() ? Color.white : new Color(.7f, .7f, .7f), p.Data.PlayerName);

            var roles = RoleInfo.GetRolesString(p, true, true, true);

            var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
            var taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";
            if (GameHistory.GetKillCount(p) != 0) taskInfo += $" <color=#FF0000FF>击杀:{GameHistory.GetKillCount(p)}</color>";

            var status = p.IsAlive()
                ? "<color=#00FF00FF>存活</color>"
                : $"<color=#AAAAAAFF>{GameHistory.GetDeathReasonString(p)}</color>";

            table.AddRow(playerName, roles, taskInfo, status);
        }
        AdditionalTempData.GameEndString = table.ToString();

        foreach (var player in AllPlayers)
        {
            var roles = player.GetRoleInfo();
            var roleString = RoleInfo.GetRolesString(player, true, true, true, false);
            AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo
            {
                PlayerName = player.Data.PlayerName,
                Role = roles,
                RoleNames = roleString,
                IsAlive = player.IsAlive()
            });
        }

        // Remove Jester, Arsonist, Vulture, Jackal, former Jackals and Sidekick from winners (if they win, they'll be readded)
        var notWinners = new List<PlayerControl>();

        foreach (var p in AllPlayers)
        {
            if (p.Data == null) continue;
            if (p.GetRoleBase() is INeutral neutral && neutral.NotWinner)
            {
                notWinners.Add(p);
                continue;
            }
            if (p.GetRoleBase() is SchrodingersCat schrodingersCat && schrodingersCat.State != SchrodingersCat.CatState.Crewmate)
            {
                notWinners.Add(p);
                continue;
            }
        }

        foreach (var akujo in GetRoles(RoleId.Akujo).Cast<Akujo>())
        {
            if (Akujo.honmeiCannotFollowWin && akujo.honmei != null)
                notWinners.Add(akujo.honmei);
        }

        foreach (var p in notWinners) Message($"NotWinner: {p?.Data?.PlayerName ?? "null"}");
        var isCanceled = gameOverReason == (GameOverReason)CustomGameOverReason.Canceled;
        var everyoneDead = AdditionalTempData.playerRoles.All(x => !x.IsAlive);
        var miniLose = Mini.mini != null && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
        var jesterWin = Jester.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
        var witnessWin = Witness.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.WitnessWin;
        var impostorWin = (gameOverReason is GameOverReason.ImpostorByKill or GameOverReason.ImpostorBySabotage or GameOverReason.ImpostorByVote)
            || Vortox.triggerImpWin;
        var werewolfWin = gameOverReason == (GameOverReason)CustomGameOverReason.WerewolfWin && Werewolf.Player.IsAlive();
        var juggernautWin = gameOverReason == (GameOverReason)CustomGameOverReason.JuggernautWin && Juggernaut.Player.IsAlive();
        var swooperWin = gameOverReason == (GameOverReason)CustomGameOverReason.SwooperWin && Swooper.Player.IsAlive();
        var pelicanWin = gameOverReason == (GameOverReason)CustomGameOverReason.PelicanWin && Pelican.Player.IsAlive();
        var arsonistWin = Arsonist.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
        var doomsayerWin = Doomsayer.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.DoomsayerWin;
        var loversWin = Lovers.IsAlive() && (gameOverReason == (GameOverReason)CustomGameOverReason.LoversWin ||
                         (GameManager.Instance.DidHumansWin(gameOverReason) && !Lovers.isKillerLover()));
        var teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin &&
                            (Jackal.jackal.Any(x => x.IsAlive()) || Jackal.Sidekick.IsAlive());
        var teamPavlovsWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamPavlovsWin &&
                            (Pavlovsdogs.pavlovsowner.IsAlive() || Pavlovsdogs.pavlovsdogs.Any(p => p.IsAlive()));
        var crewmateWin = GameManager.Instance.DidHumansWin(gameOverReason) ||
                          (gameOverReason is GameOverReason.HumansByVote or GameOverReason.HumansByTask);
        var vultureWin = Vulture.vulture != null && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
        var executionerWin = Executioner.executioner != null && gameOverReason == (GameOverReason)CustomGameOverReason.ExecutionerWin;
        var lawyerSoloWin = Lawyer.lawyer != null && gameOverReason == (GameOverReason)CustomGameOverReason.LawyerSoloWin;
        var akujoWin = Akujo.Player.IsAlive() && Akujo.honmei.IsAlive() && (gameOverReason == (GameOverReason)CustomGameOverReason.AkujoWin ||
                       (GameManager.Instance.DidHumansWin(gameOverReason) && !Akujo.IsKillerLover()));

        var bandLeaderAddCrewWin = BandLeader.Player != null && BandLeader.winnerFlags == BandLeader.WinnerFlags.Crewmate && crewmateWin;
        var bandLeaderAddImpWin = BandLeader.Player != null && BandLeader.winnerFlags == BandLeader.WinnerFlags.Impostor && impostorWin;

        bool isPursurerLose = jesterWin || witnessWin || arsonistWin || miniLose || isCanceled || executionerWin;

        TempData.winners = new();
        var winners = new List<PlayerControl>();

        // Mini lose
        if (miniLose)
        {
            // If "no one is the Mini", it will display the Mini, but also show defeat to everyone
            //var wpd = new WinningPlayerData(Mini.mini.Data) { IsYou = false };
            //TempData.winners.Add(wpd);
            AdditionalTempData.winCondition = WinCondition.MiniLose;
        }
        else if (isCanceled)
        {
            AdditionalTempData.winCondition = WinCondition.Canceled;
        }

        // Everyone Died
        else if (everyoneDead)
        {
            AdditionalTempData.winCondition = WinCondition.EveryoneDied;
        }

        // Jester win
        if (jesterWin)
        {
            winners.Add(Jester.Player);
            AdditionalTempData.winCondition = WinCondition.JesterWin;
        }

        // Witness win
        else if (witnessWin)
        {
            winners.Add(Witness.Player);
            AdditionalTempData.winCondition = WinCondition.WitnessWin;
        }

        // Vulture win
        else if (vultureWin)
        {
            winners.Add(Vulture.vulture);
            AdditionalTempData.winCondition = WinCondition.VultureWin;
        }

        // Jester win
        else if (executionerWin)
        {
            winners.Add(Executioner.executioner);
            AdditionalTempData.winCondition = WinCondition.ExecutionerWin;
        }

        // Akujo win
        else if (akujoWin)
        {
            if (Akujo.honmeiOptimizeWin && !Akujo.IsKillerLover())
            {
                foreach (var p in AllPlayers)
                {
                    if (p == null) continue;
                    if (p == Akujo.Player || p == Akujo.honmei)
                        winners.Add(p);
                    else if (Pursuer.Player.Any(x => x.PlayerId == p.PlayerId) && p.IsAlive())
                        winners.Add(p);
                    else if (Survivor.Player.Any(x => x.PlayerId == p.PlayerId) && p.IsAlive())
                        winners.Add(p);
                    else if (!notWinners.Any(x => x.PlayerId == p.PlayerId) && !p.IsImpostor())
                        winners.Add(p);
                }
                AdditionalTempData.winCondition = WinCondition.AkujoTeamWin;
            }
            else
            {
                winners.Add(Akujo.Player);
                winners.Add(Akujo.honmei);
                AdditionalTempData.winCondition = WinCondition.AkujoSoloWin;
            }
        }

        // Lovers win conditions
        else if (loversWin)
        {
            // Double win for lovers, crewmates also win
            if (!Lovers.isKillerLover())
            {
                foreach (var p in AllPlayers)
                {
                    if (p == null) continue;
                    if (p == Lovers.Player || p == Lovers.Lover)
                        winners.Add(p);
                    else if (Pursuer.Player.Any(x => x.PlayerId == p.PlayerId) && p.IsAlive())
                        winners.Add(p);
                    else if (Survivor.Player.Any(x => x.PlayerId == p.PlayerId) && p.IsAlive())
                        winners.Add(p);
                    else if (!notWinners.Any(x => x.PlayerId == p.PlayerId) && !p.IsImpostor())
                        winners.Add(p);
                }
                AdditionalTempData.winCondition = WinCondition.LoversTeamWin;
            }
            // Lovers solo win
            else
            {
                winners.Add(Lovers.Player);
                winners.Add(Lovers.Lover);
                AdditionalTempData.winCondition = WinCondition.LoversSoloWin;
            }
        }

        else if (teamJackalWin)
        {
            AdditionalTempData.winCondition = WinCondition.JackalWin;
            foreach (var player in Jackal.jackal.GroupBy(x => x.PlayerId).Select(g => g.First()))
            {
                winners.Add(player);
            }
            if (Jackal.Sidekick != null)
            {
                winners.Add(Jackal.Sidekick);
            }
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.State == SchrodingersCat.CatState.Jackal)
            {
                winners.Add(SchrodingersCat.schrodingersCat);
            }
        }
        else if (teamPavlovsWin)
        {
            AdditionalTempData.winCondition = WinCondition.PavlovsWin;
            winners.Add(Pavlovsdogs.pavlovsowner);

            foreach (var player in Pavlovsdogs.pavlovsdogs)
            {
                winners.Add(player);
            }
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.State == SchrodingersCat.CatState.Pavlovsowner)
            {
                winners.Add(SchrodingersCat.schrodingersCat);
            }
        }
        else if (werewolfWin)
        {
            // Werewolf wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.WerewolfWin;
            winners.Add(Werewolf.Player);
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.State == SchrodingersCat.CatState.Werewolf)
            {
                winners.Add(SchrodingersCat.schrodingersCat);
            }
        }

        // Arsonist win
        else if (arsonistWin)
        {
            winners.Add(Arsonist.Player);
            AdditionalTempData.winCondition = WinCondition.ArsonistWin;
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.State == SchrodingersCat.CatState.Arsonist)
            {
                winners.Add(SchrodingersCat.schrodingersCat);
            }
        }

        else if (juggernautWin)
        {
            // JuggernautWin wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.JuggernautWin;
            winners.Add(Juggernaut.Player);
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.State == SchrodingersCat.CatState.Juggernaut)
            {
                winners.Add(SchrodingersCat.schrodingersCat);
            }
        }

        else if (pelicanWin)
        {
            AdditionalTempData.winCondition = WinCondition.PelicanWin;
            winners.Add(Pelican.Player);
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.State == SchrodingersCat.CatState.Pelican)
            {
                winners.Add(SchrodingersCat.schrodingersCat);
            }
        }

        else if (doomsayerWin)
        {
            // DoomsayerWin wins if nobody except jackal is alive
            winners.Add(Doomsayer.Player);
            AdditionalTempData.winCondition = WinCondition.DoomsayerWin;
        }

        //Swooper
        else if (swooperWin)
        {
            // Swooper wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.SwooperWin;
            winners.Add(Swooper.Player);
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.State == SchrodingersCat.CatState.Swooper)
            {
                winners.Add(SchrodingersCat.schrodingersCat);
            }
        }

        else if (impostorWin)
        {
            foreach (var player in AllPlayers.Where(x => !notWinners.Contains(x) && x.Data.Role.IsImpostor))
            {
                winners.Add(player);
            }
            AdditionalTempData.winCondition = WinCondition.ImpostorWin;
            if (bandLeaderAddImpWin)
            {
                winners.Add(BandLeader.Player);
                AdditionalTempData.winCondition = WinCondition.BandLeaderWin;
            }
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.State == SchrodingersCat.CatState.Impostor)
            {
                winners.Add(SchrodingersCat.schrodingersCat);
            }
        }

        else if (crewmateWin)
        {
            foreach (var player in AllPlayers.Where(x => x.IsCrew() && !notWinners.Contains(x)))
            {
                winners.Add(player);
            }
            AdditionalTempData.winCondition = WinCondition.CrewmateWin;
            if (bandLeaderAddCrewWin)
            {
                winners.Add(BandLeader.Player);
                AdditionalTempData.winCondition = WinCondition.BandLeaderWin;
            }
            if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.State == SchrodingersCat.CatState.Crewmate)
            {
                winners.Add(SchrodingersCat.schrodingersCat);
            }
        }

        // Lawyer solo win 
        else if (lawyerSoloWin)
        {
            winners.Add(Lawyer.lawyer);
            AdditionalTempData.winCondition = WinCondition.LawyerSoloWin;
        }

        // Possible Additional winner: Lawyer
        if (!lawyerSoloWin && Lawyer.lawyer != null && Lawyer.Target != null &&
            (!Lawyer.Target.Data.IsDead || Lawyer.Target == Jester.Player) && !Lawyer.notAckedExiled)
        {
            PlayerControl winningClient = null;
            foreach (var winner in winners)
                if (winner.Data.PlayerName == Lawyer.Target.Data.PlayerName) winningClient = winner;
            if (winningClient != null)
            {
                if (!winners.ToArray().Any(x => x.Data.PlayerName == Lawyer.lawyer.Data.PlayerName))
                {
                    if (!Lawyer.lawyer.Data.IsDead && Lawyer.stolenWin)
                    {
                        // The Lawyer replaces the client's victory
                        winners.Remove(winningClient);
                        winners.Add(Lawyer.lawyer);
                        AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerStolenWin);
                    }
                    else
                    {
                        // The Lawyer wins with the client
                        winners.Add(Lawyer.lawyer);
                        AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerBonusWin);
                    }
                }
            }
        }

        // Possible Additional winner: Pursuer
        if (Pursuer.Player != null && Pursuer.Player.Any(p => !p.Data.IsDead) && !Lawyer.notAckedExiled && !isPursurerLose)
        {
            foreach (var player in Pursuer.Player.Where(p => !p.Data.IsDead))
            {
                winners.Add(player);
            }
            AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalAlivePursuerWin);
        }

        // Possible Additional winner: Survivor
        if (Survivor.Player != null && Survivor.Player.Any(p => !p.Data.IsDead) && !isPursurerLose)
        {
            foreach (var player in Survivor.Player.Where(p => !p.Data.IsDead))
            {
                if (!winners.ToArray().Any(x => x.Data.PlayerName == player.Data.PlayerName))
                    winners.Add(player);
            }
            AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalAliveSurvivorWin);
        }

        if (PartTimer.Player != null && PartTimer.target != null &&
            winners.ToArray().Any(x => x.Data.PlayerName == PartTimer.target.Data.PlayerName))
        {
            winners.Add(PartTimer.Player);
            AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalPartTimerWin);
        }

        if (BandLeader.Player != null && BandLeader.winnerFlags == BandLeader.WinnerFlags.Neutral)
        {
            if (winners.Any(x => BandLeader.Members.Select(c => c.Data.PlayerName).Contains(x.Data.PlayerName)))
            {
                foreach (var player in BandLeader.Members)
                {
                    winners.Add(player);
                }
                winners.Add(BandLeader.Player);
                AdditionalTempData.winCondition = WinCondition.BandLeaderWin;
            }
        }

        TempData.winners = winners.Where(x => x?.Data != null && !x.Data.Disconnected).Select(x => new WinningPlayerData(x.Data)).Distinct().ToIl2CppList();

        Message($"游戏结束 {AdditionalTempData.winCondition}", "OnGameEnd");*/
    }
}

[HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
public class EndGameManagerSetUpPatch
{
    public static void Postfix(EndGameManager __instance)
    {
        // Delete and readd PoolablePlayers always showing the name and role of the player
        foreach (var pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>())
            UObject.Destroy(pb.gameObject);
        var num = Mathf.CeilToInt(7.5f);
        var list = TempData.winners.ToList().OrderBy(delegate (WinningPlayerData b) { return !b.IsYou ? 0 : -1; }).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            var winningPlayerData2 = list[i];
            var num2 = i % 2 == 0 ? -1 : 1;
            var num3 = (i + 1) / 2;
            var num4 = num3 / (float)num;
            var num5 = Mathf.Lerp(1f, 0.75f, num4);
            float num6 = i == 0 ? -8 : -1;
            var poolablePlayer = UObject.Instantiate(__instance.PlayerPrefab, __instance.transform);
            poolablePlayer.transform.localPosition = new Vector3(1f * num2 * num3 * num5,
                FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (num3 * 0.01f)) * 0.9f;
            var num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
            var vector = new Vector3(num7, num7, 1f);
            poolablePlayer.transform.localScale = vector;
            if (winningPlayerData2.IsDead)
            {
                poolablePlayer.SetBodyAsGhost();
                poolablePlayer.SetDeadFlipX(i % 2 == 0);
            }
            else
            {
                poolablePlayer.SetFlipX(i % 2 == 0);
            }

            poolablePlayer.UpdateFromPlayerOutfit(winningPlayerData2, PlayerMaterial.MaskType.None, winningPlayerData2.IsDead, true);

            poolablePlayer.cosmetics.nameText.color = Color.white;
            poolablePlayer.cosmetics.nameText.transform.localScale = new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
            var localPosition = poolablePlayer.cosmetics.nameText.transform.localPosition;
            localPosition = new Vector3(localPosition.x, localPosition.y, -15f);
            poolablePlayer.cosmetics.nameText.transform.localPosition = localPosition;
            poolablePlayer.cosmetics.nameText.text = winningPlayerData2.PlayerName;

            foreach (var roles in from data in AdditionalTempData.playerRoles
                                  where data.PlayerName == winningPlayerData2.PlayerName
                                  select poolablePlayer.cosmetics.nameText.text +=
                         $"\n{Cs(data.Role.Color, data.Role.Name)}")
            {
            }
        }

        // Create a dictionary for win conditions
        var winConditionTexts = new Dictionary<WinCondition, (Color, string)>
        {
            { WinCondition.Canceled, (Color.gray, "CanceledEnd") },
            { WinCondition.EveryoneDied, (Palette.DisabledGrey, "EveryoneDied") },
            { WinCondition.JesterWin, (Jester.color, "JesterWin") },
            { WinCondition.DoomsayerWin, (Doomsayer.color, "DoomsayerWin") },
            { WinCondition.ArsonistWin, (Arsonist.color, "ArsonistWin") },
            { WinCondition.PelicanWin, (Pelican.color, "PelicanWin") },
            { WinCondition.VultureWin, (Vulture.color, "VultureWin") },
            { WinCondition.LawyerSoloWin, (Lawyer.color, "LawyerSoloWin") },
            { WinCondition.WerewolfWin, (Werewolf.color, "WerewolfWin") },
            { WinCondition.WitnessWin, (Witness.color, "WitnessWin") },
            { WinCondition.JuggernautWin, (Juggernaut.color, "JuggernautWin") },
            { WinCondition.SwooperWin, (Swooper.color, "SwooperWin") },
            { WinCondition.ExecutionerWin, (Executioner.color, "ExecutionerWin") },
            { WinCondition.LoversTeamWin, (Lovers.color, "LoversTeamWin") },
            { WinCondition.LoversSoloWin, (Lovers.color, "LoversSoloWin") },
            { WinCondition.JackalWin, (Jackal.color, "JackalWin") },
            { WinCondition.PavlovsWin, (Pavlovsdogs.color, "PavlovsWin") },
            { WinCondition.AkujoSoloWin, (Akujo.color, "AkujoSoloWin") },
            { WinCondition.AkujoTeamWin, (Akujo.color, "AkujoTeamWin") },
            { WinCondition.MiniLose, (Mini.color, "MiniLose") },
            { WinCondition.CrewmateWin, (Palette.CrewmateBlue, "CrewmateWin") },
            { WinCondition.ImpostorWin, (Palette.ImpostorRed, "ImpostorWin") },
            { WinCondition.BandLeaderWin, (BandLeader.color, "BandLeaderWin") }
        };

        var winConditionMappings = new Dictionary<WinCondition, (Color, string)>
        {
            { WinCondition.AdditionalLawyerStolenWin, (Lawyer.color, "LawyerStolenWin") },
            { WinCondition.AdditionalLawyerBonusWin, (Lawyer.color, "LawyerBonusWin") },
            { WinCondition.AdditionalPartTimerWin, (PartTimer.color, "PartTimerWin") },
            { WinCondition.AdditionalAlivePursuerWin, (Pursuer.color, "起诉人存活") },
            { WinCondition.AdditionalAliveSurvivorWin, (Survivor.color, "幸存者存活") },
            { WinCondition.BandLeaderWin, (BandLeader.color, "BandLeaderWin") }
        };

        var bonusText = UObject.Instantiate(__instance.WinText.gameObject);
        var position1 = __instance.WinText.transform.position;
        bonusText.transform.position = new Vector3(position1.x, position1.y - 0.5f, position1.z);
        bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        var textRenderer = bonusText.GetComponent<TMP_Text>();
        textRenderer.text = "";

        if (winConditionTexts.TryGetValue(AdditionalTempData.winCondition, out var winText))
        {
            textRenderer.text = winText.Item2.Translate();
            textRenderer.color = winText.Item1;
            __instance.BackgroundBar.material.SetColor("_Color", winText.Item1);
        }

        var winConditionsTexts = new List<string>();
        foreach (var cond in AdditionalTempData.additionalWinConditions)
        {
            if (winConditionMappings.TryGetValue(cond, out var mapping))
            {
                winConditionsTexts.Add(Cs(mapping.Item1, mapping.Item2.Translate()));
            }
        }

        if (winConditionsTexts.Count == 1)
        {
            textRenderer.text += $"<size=50%>\n{winConditionsTexts[0]}</size>";
        }
        else if (winConditionsTexts.Count > 1)
        {
            var combinedText = string.Join(" & ", winConditionsTexts);
            textRenderer.text += $"<size=50%>\n{combinedText}</size>";
        }
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.Normal)
        {
            if (Camera.main != null)
            {
                var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                var roleSummary = UObject.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f,
                    position.y - 0.1f, -214f);
                roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                var roleSummaryText = "游戏总结:\n";

                roleSummaryText += AdditionalTempData.GameEndString;

                var roleSummaryTextMesh = roleSummary.GetComponent<TMP_Text>();
                roleSummaryTextMesh.alignment = TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = Color.white;
                roleSummaryTextMesh.fontSizeMin = 1.5f;
                roleSummaryTextMesh.fontSizeMax = 1.5f;
                roleSummaryTextMesh.fontSize = 1.5f;

                var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
                roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
                roleSummaryTextMesh.text = roleSummaryText.ToString();
            }
        }
        AdditionalTempData.clear();
    }
}

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
internal class CheckEndCriteriaPatch
{
    public static bool Prefix(ShipStatus __instance)
    {
        if (!GameData.Instance) return false;
        if (ModOption.DisableGameEnd) return false;
        // InstanceExists | Don't check Custom Criteria when in Tutorial
        if (DestroyableSingleton<TutorialManager>.InstanceExists) return true;




        return false;
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
internal class RPCEndGamePatch
{
    public static void Postfix(ref GameOverReason endReason)
    {
        Message($"游戏结束 {(CustomGameOverReason)endReason} {endReason}", "RpcEndGame");
    }
}