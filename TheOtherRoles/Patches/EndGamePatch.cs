using AmongUs.GameOptions;
using static TheOtherRoles.Modules.SimpleTable;

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
    BandLeaderTeamWin,
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
        public List<RoleInfo> Roles { get; set; }
        public string RoleNames { get; set; }
        public int TasksCompleted { get; set; }
        public int TasksTotal { get; set; }
        public bool IsGuesser { get; set; }
        public int? Kills { get; set; }
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
        AdditionalTempData.clear();
        List<RoleInfo> killRole =
        [
            RoleInfo.sheriff,
            RoleInfo.jackal,
            RoleInfo.sidekick,
            RoleInfo.swooper,
            RoleInfo.thief,
            RoleInfo.werewolf,
            RoleInfo.juggernaut,
            RoleInfo.pavlovsdogs
        ];

        var table = new SimpleTable()
            .AddColumn(alignment: Alignment.Left)
            .AddColumn(alignment: Alignment.Left)
            .AddColumn(alignment: Alignment.Left)
            .AddColumn(alignment: Alignment.Left)
            .AddRow();

        var AllPlayers = PlayerControl.AllPlayerControls.ToArray();

        foreach (var p in AllPlayers)
        {
            var playerName = cs(p.IsAlive() ? Color.white : new Color(.7f, .7f, .7f), p.Data.PlayerName);

            var roles = RoleInfo.GetRolesString(p, true, true, true);

            var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
            var taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";
            if (p.IsKiller()) taskInfo += $" <color=#FF0000FF>击杀:{GameHistory.GetKillCount(p)}</color>";

            var status = p.IsAlive()
                ? "<color=#00FF00FF>存活</color>"
                : $"<color=#AAAAAAFF>{RoleInfo.GetDeathReasonString(p)}</color>";

            table.AddRow(playerName, roles, taskInfo, status);
        }
        AdditionalTempData.GameEndString = table.ToString();

        foreach (var player in AllPlayers)
        {
            var roles = RoleInfo.getRoleInfoForPlayer(player);
            var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(player.Data);
            var isGuesser = HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(player.PlayerId);
            int? killCount = GameHistory.GetKillCount(player);
            if (killCount == 0 &&
                !(killRole.Contains(RoleInfo.getRoleInfoForPlayer(player, false).FirstOrDefault())
                 || player.Data.Role.IsImpostor)) killCount = null;
            var roleString = RoleInfo.GetRolesString(player, true, true, true, false);
            AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo
            {
                PlayerName = player.Data.PlayerName,
                Roles = roles,
                RoleNames = roleString,
                TasksTotal = tasksTotal,
                TasksCompleted = tasksCompleted,
                IsGuesser = isGuesser,
                Kills = killCount,
                IsAlive = player.IsAlive()
            });
        }

        // Remove Jester, Arsonist, Vulture, Jackal, former Jackals and Sidekick from winners (if they win, they'll be readded)
        var notWinners = new List<PlayerControl>();

        notWinners.AddRange(
        [
            Jester.jester,
            Jackal.Sidekick,
            Arsonist.arsonist,
            Swooper.swooper,
            Vulture.vulture,
            Werewolf.werewolf,
            Lawyer.lawyer,
            Executioner.executioner,
            Witness.Player,
            //Specter.Player,
            Thief.thief,
            Pelican.Player,
            BandLeader.Player,
            Juggernaut.juggernaut,
            Doomsayer.doomsayer,
            PartTimer.partTimer,
            Akujo.akujo,
            Pavlovsdogs.pavlovsowner,
        ]);
        notWinners.RemoveAll(x => x?.Data == null);
        notWinners.AddRange(Amnisiac.Player.Where(p => p != null));
        notWinners.AddRange(Pavlovsdogs.pavlovsdogs.Where(p => p != null));
        notWinners.AddRange(Jackal.jackal.Where(p => p != null));
        notWinners.AddRange(Pursuer.Player.Where(p => p != null));
        notWinners.AddRange(Survivor.Player.Where(p => p != null));

        if (SchrodingersCat.Player != null && SchrodingersCat.State != SchrodingersCat.CatState.Crewmate)
            notWinners.Add(SchrodingersCat.Player);
        if (Akujo.honmeiCannotFollowWin && Akujo.honmei != null)
            notWinners.Add(Akujo.honmei);

        var isCanceled = gameOverReason == (GameOverReason)CustomGameOverReason.Canceled;
        var everyoneDead = AdditionalTempData.playerRoles.All(x => !x.IsAlive);
        var miniLose = gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
        var jesterWin = gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
        var witnessWin = gameOverReason == (GameOverReason)CustomGameOverReason.WitnessWin;
        var impostorWin = (gameOverReason is GameOverReason.ImpostorByKill or GameOverReason.ImpostorBySabotage or GameOverReason.ImpostorByVote)
            || Vortox.triggerImpWin;
        var werewolfWin = gameOverReason == (GameOverReason)CustomGameOverReason.WerewolfWin && Werewolf.werewolf != null;
        var juggernautWin = gameOverReason == (GameOverReason)CustomGameOverReason.JuggernautWin && Juggernaut.juggernaut != null;
        var swooperWin = gameOverReason == (GameOverReason)CustomGameOverReason.SwooperWin && Swooper.swooper != null;
        var pelicanWin = gameOverReason == (GameOverReason)CustomGameOverReason.PelicanWin && Pelican.Player != null;
        var arsonistWin = Arsonist.arsonist != null && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
        var doomsayerWin = Doomsayer.doomsayer != null && gameOverReason == (GameOverReason)CustomGameOverReason.DoomsayerWin;
        var loversWin = Lovers.IsAlive() && (gameOverReason == (GameOverReason)CustomGameOverReason.LoversWin ||
                         (GameManager.Instance.DidHumansWin(gameOverReason) && !Lovers.isKillerLover()));
        var teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin;
        var teamPavlovsWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamPavlovsWin;
        var crewmateWin = GameManager.Instance.DidHumansWin(gameOverReason) ||
                          (gameOverReason is GameOverReason.HumansByVote or GameOverReason.HumansByTask);
        var vultureWin = Vulture.vulture != null && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
        var executionerWin = Executioner.executioner != null && gameOverReason == (GameOverReason)CustomGameOverReason.ExecutionerWin;
        var lawyerSoloWin = Lawyer.lawyer != null && gameOverReason == (GameOverReason)CustomGameOverReason.LawyerSoloWin;
        var akujoWin = Akujo.akujo.IsAlive() && Akujo.honmei.IsAlive() && (gameOverReason == (GameOverReason)CustomGameOverReason.AkujoWin ||
                       (GameManager.Instance.DidHumansWin(gameOverReason) && !Akujo.IsKillerLover()));
        var bandLeaderWin = gameOverReason == (GameOverReason)CustomGameOverReason.BandLeaderTeamWin;

        var bandLeaderAddCrewWin = BandLeader.Player != null && BandLeader.winnerFlags == BandLeader.WinnerFlags.Crewmate && crewmateWin;
        var bandLeaderAddImpWin = BandLeader.Player != null && BandLeader.winnerFlags == BandLeader.WinnerFlags.Impostor && impostorWin;

        bool isPursurerLose = jesterWin || witnessWin || arsonistWin || miniLose || isCanceled || executionerWin;

        TempData.winners = new();
        var winners = new HashSet<PlayerControl>();

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
            winners.Add(Jester.jester);
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
                    if (p == Akujo.akujo || p == Akujo.honmei)
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
                winners.Add(Akujo.akujo);
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
                    if (p == Lovers.lover1 || p == Lovers.lover2)
                        winners.Add(p);
                    else if (Pursuer.Player.Any(x => x.PlayerId == p.PlayerId) && p.IsAlive())
                        winners.Add(p);
                    else if (Survivor.Player.Any(x => x.PlayerId == p.PlayerId) && p.IsAlive())
                        winners.Add(p);
                    else if (!notWinners.Any(x => x.PlayerId == p.PlayerId) && !p.IsImpostor())
                        winners.Add(p);
                    if (bandLeaderAddCrewWin)
                    {
                        winners.Add(BandLeader.Player);
                    }
                    if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Crewmate)
                    {
                        winners.Add(SchrodingersCat.Player);
                    }
                }
                AdditionalTempData.winCondition = WinCondition.LoversTeamWin;
            }
            // Lovers solo win
            else
            {
                winners.Add(Lovers.lover1);
                winners.Add(Lovers.lover2);
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
            if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Jackal)
            {
                winners.Add(SchrodingersCat.Player);
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
            if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Pavlovsowner)
            {
                winners.Add(SchrodingersCat.Player);
            }
        }
        else if (werewolfWin)
        {
            // Werewolf wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.WerewolfWin;
            winners.Add(Werewolf.werewolf);
            if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Werewolf)
            {
                winners.Add(SchrodingersCat.Player);
            }
        }

        // Arsonist win
        else if (arsonistWin)
        {
            winners.Add(Arsonist.arsonist);
            AdditionalTempData.winCondition = WinCondition.ArsonistWin;
            if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Arsonist)
            {
                winners.Add(SchrodingersCat.Player);
            }
        }

        else if (juggernautWin)
        {
            // JuggernautWin wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.JuggernautWin;
            winners.Add(Juggernaut.juggernaut);
            if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Juggernaut)
            {
                winners.Add(SchrodingersCat.Player);
            }
        }

        else if (pelicanWin)
        {
            AdditionalTempData.winCondition = WinCondition.PelicanWin;
            winners.Add(Pelican.Player);
            if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Pelican)
            {
                winners.Add(SchrodingersCat.Player);
            }
        }

        else if (bandLeaderWin)
        {
            foreach (var player in BandLeader.Members)
            {
                winners.Add(player);
            }
            winners.Add(BandLeader.Player);
            AdditionalTempData.winCondition = WinCondition.BandLeaderWin;
        }

        else if (doomsayerWin)
        {
            // DoomsayerWin wins if nobody except jackal is alive
            winners.Add(Doomsayer.doomsayer);
            AdditionalTempData.winCondition = WinCondition.DoomsayerWin;
        }

        //Swooper
        else if (swooperWin)
        {
            // Swooper wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.SwooperWin;
            winners.Add(Swooper.swooper);
            if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Swooper)
            {
                winners.Add(SchrodingersCat.Player);
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
            if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Impostor)
            {
                winners.Add(SchrodingersCat.Player);
            }
        }

        else if (crewmateWin)
        {
            foreach (var player in AllPlayers.Where(x => x.IsCrew()))
            {
                if (notWinners.Any(x => x.PlayerId == player.PlayerId)) continue;
                winners.Add(player);
            }
            AdditionalTempData.winCondition = WinCondition.CrewmateWin;
            if (bandLeaderAddCrewWin)
            {
                winners.Add(BandLeader.Player);
                AdditionalTempData.winCondition = WinCondition.BandLeaderWin;
            }
            if (SchrodingersCat.Player != null && SchrodingersCat.State == SchrodingersCat.CatState.Crewmate)
            {
                winners.Add(SchrodingersCat.Player);
            }
        }

        // Lawyer solo win 
        else if (lawyerSoloWin)
        {
            winners.Add(Lawyer.lawyer);
            AdditionalTempData.winCondition = WinCondition.LawyerSoloWin;
        }

        // Possible Additional winner: Lawyer
        if (!lawyerSoloWin && Lawyer.lawyer != null && Lawyer.target != null &&
            (!Lawyer.target.Data.IsDead || Lawyer.target == Jester.jester) && !Lawyer.notAckedExiled)
        {
            PlayerControl winningClient = null;
            foreach (var winner in winners)
                if (winner.Data.PlayerName == Lawyer.target.Data.PlayerName) winningClient = winner;
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

        if (PartTimer.partTimer != null && PartTimer.target != null &&
            winners.ToArray().Any(x => x.Data.PlayerName == PartTimer.target.Data.PlayerName))
        {
            winners.Add(PartTimer.partTimer);
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

        TempData.winners = winners.Where(x => x?.Data != null && !x.Data.Disconnected).Select(x => new WinningPlayerData(x.Data)).ToIl2CppList();

        Message($"游戏结束 {AdditionalTempData.winCondition}", "OnGameEnd");
        // Reset Settings
        RPCProcedure.resetVariables();
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
                         $"\n{string.Join("\n", data.Roles.Select(x => cs(x.color, x.Name)))}")
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
                winConditionsTexts.Add(cs(mapping.Item1, mapping.Item2.Translate()));
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
        var statistics = new PlayerStatistics(__instance);
        if (ModOption.DisableGameEnd) return false;
        // InstanceExists | Don't check Custom Criteria when in Tutorial
        if (DestroyableSingleton<TutorialManager>.InstanceExists) return true;
        if (CheckAndEndGameForTaskWin(__instance)) return false;
        if (CheckAndEndGameForMiniLose(__instance)) return false;
        if (CheckAndEndGameForJesterWin(__instance)) return false;
        if (CheckAndEndGameForDoomsayerWin(__instance)) return false;
        if (CheckAndEndGameForWitnessWin(__instance)) return false;
        if (CheckAndEndGameForVultureWin(__instance)) return false;
        if (CheckAndEndGameForSabotageWin(__instance)) return false;
        if (CheckAndEndGameForExecutionerWin(__instance)) return false;
        if (CheckAndEndGameForBandLeaderWin(__instance, statistics)) return false;
        if (CheckAndEndGameForAkujoWin(__instance, statistics)) return false;
        if (CheckAndEndGameForArsonistWin(__instance, statistics)) return false;
        if (CheckAndEndGameForWerewolfWin(__instance, statistics)) return false;
        if (CheckAndEndGameForLoverWin(__instance, statistics)) return false;
        if (CheckAndEndGameForJackalWin(__instance, statistics)) return false;
        if (CheckAndEndGameForPavlovsWin(__instance, statistics)) return false;
        if (CheckAndEndGameForSwooperWin(__instance, statistics)) return false;
        if (CheckAndEndGameForPelicanWin(__instance, statistics)) return false;
        if (CheckAndEndGameForJuggernautWin(__instance, statistics)) return false;
        if (CheckAndEndGameForImpostorWin(__instance, statistics)) return false;
        if (CheckAndEndGameForCrewmateWin(__instance, statistics)) return false;
        return false;
    }

    private static bool CheckAndEndGameForMiniLose(ShipStatus __instance)
    {
        if (!Mini.triggerMiniLose) return false;
        //__instance.enabled = false;
        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.MiniLose, false);
        return true;
    }

    private static bool CheckAndEndGameForJesterWin(ShipStatus __instance)
    {
        if (Jester.triggerJesterWin)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JesterWin, false);
            return true;
        }
        return false;
    }

    private static bool CheckAndEndGameForWitnessWin(ShipStatus __instance)
    {
        if (Witness.triggerWitnessWin)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.WitnessWin, false);
            return true;
        }
        return false;
    }

    private static bool CheckAndEndGameForDoomsayerWin(ShipStatus __instance)
    {
        if (Doomsayer.triggerDoomsayerrWin)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.DoomsayerWin, false);
            return true;
        }
        return false;
    }

    private static bool CheckAndEndGameForVultureWin(ShipStatus __instance)
    {
        if (Vulture.triggerVultureWin)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.VultureWin, false);
            return true;
        }
        return false;
    }

    private static bool CheckAndEndGameForSabotageWin(ShipStatus __instance)
    {
        if (MapUtilities.Systems == null) return false;
        var systemType = MapUtilities.Systems.ContainsKey(SystemTypes.LifeSupp)
            ? MapUtilities.Systems[SystemTypes.LifeSupp]
            : null;
        if (systemType != null)
        {
            var lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
            if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f)
            {
                EndGameForSabotage(__instance);
                lifeSuppSystemType.Countdown = 10000f;
                return true;
            }
        }

        var systemType2 = MapUtilities.Systems.ContainsKey(SystemTypes.Reactor)
            ? MapUtilities.Systems[SystemTypes.Reactor]
            : null;
        systemType2 ??= MapUtilities.Systems.ContainsKey(SystemTypes.Laboratory)
                ? MapUtilities.Systems[SystemTypes.Laboratory]
                : null;
        if (systemType2 != null)
        {
            var criticalSystem = systemType2.TryCast<ICriticalSabotage>();
            if (criticalSystem != null && criticalSystem.Countdown < 0f)
            {
                EndGameForSabotage(__instance);
                criticalSystem.ClearSabotage();
                return true;
            }
        }

        return false;
    }

    private static bool CheckAndEndGameForTaskWin(ShipStatus __instance)
    {
        if (ModOption.PreventTaskEnd) return false;
        if (GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame(GameOverReason.HumansByTask, false);
            return true;
        }

        return false;
    }

    private static bool CheckAndEndGameForExecutionerWin(ShipStatus __instance)
    {
        if (Executioner.triggerExecutionerWin)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ExecutionerWin, false);
            return true;
        }

        return false;
    }

    private static bool CheckAndEndGameForLoverWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (statistics.TeamLoversAlive == 2 && statistics.TotalAlive <= 3)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.LoversWin, false);
            return true;
        }

        return false;
    }

    private static bool CheckAndEndGameForBandLeaderWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (statistics.TeamBandLeaderAlive >= statistics.TotalAlive && BandLeader.winnerFlags == BandLeader.WinnerFlags.Neutral && BandLeader.Formed)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BandLeaderTeamWin, false);
            return true;
        }
        return false;
    }

    private static bool CheckAndEndGameForArsonistWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (statistics.TeamArsonistAlive >= statistics.TotalAlive - statistics.TeamArsonistAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJuggernautAlive == 0 &&
            statistics.TeamPavlovsAlive == 0 &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamPelicanAlive == 0 &&
            statistics.TeamAkujoAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            !(statistics.TeamArsonisHasAliveLover && statistics.TeamLoversAlive == 2)
            && !killingCrewAlive())
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ArsonistWin, false);
            return true;
        }
        return false;
    }

    private static bool CheckAndEndGameForAkujoWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if ((statistics.TeamAkujoAlive == 2 && statistics.TotalAlive <= 3)
            || (statistics.TeamAkujoAlive == 2 &&
                statistics.TeamImpostorsAlive == 0 &&
                statistics.TeamArsonistAlive == 0 &&
                statistics.TeamJuggernautAlive == 0 &&
                statistics.TeamPavlovsAlive == 0 &&
                statistics.TeamPelicanAlive == 0 &&
                statistics.TeamWerewolfAlive == 0 &&
                statistics.TeamJackalAlive == 0 &&
                statistics.TeamSwooperAlive == 0 &&
                !(statistics.TeamLoversAlive != 0 && Lovers.isKillerLover())))
        {
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.AkujoWin, false);
            return true;
        }
        return false;
    }

    private static bool CheckAndEndGameForJackalWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (statistics.TeamJackalAlive >= statistics.TotalAlive - statistics.TeamJackalAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJuggernautAlive == 0 &&
            statistics.TeamPavlovsAlive == 0 &&
            statistics.TeamArsonistAlive == 0 &&
            statistics.TeamPelicanAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamAkujoAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            !(statistics.TeamJackalHasAliveLover &&
              statistics.TeamLoversAlive == 2) && !killingCrewAlive())
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TeamJackalWin, false);
            return true;
        }

        return false;
    }
    private static bool CheckAndEndGameForPavlovsWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (statistics.TeamPavlovsAlive >= statistics.TotalAlive - statistics.TeamPavlovsAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamJuggernautAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamArsonistAlive == 0 &&
            statistics.TeamPelicanAlive == 0 &&
            statistics.TeamAkujoAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            !(statistics.TeamPavlovsHasAliveLover && statistics.TeamLoversAlive == 2) && !killingCrewAlive())
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TeamPavlovsWin, false);
            return true;
        }

        return false;
    }
    private static bool CheckAndEndGameForSwooperWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (statistics.TeamSwooperAlive >= statistics.TotalAlive - statistics.TeamSwooperAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJuggernautAlive == 0 &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamPavlovsAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamPelicanAlive == 0 &&
            statistics.TeamArsonistAlive == 0 &&
            !(statistics.TeamSwooperHasAliveLover && statistics.TeamLoversAlive == 2) && !killingCrewAlive())
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.SwooperWin, false);
            return true;
        }
        return false;
    }
    private static bool CheckAndEndGameForPelicanWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (statistics.TeamPelicanAlive >= statistics.TotalAlive - statistics.TeamPelicanAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJuggernautAlive == 0 &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamPavlovsAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            statistics.TeamArsonistAlive == 0 &&
            !(statistics.TeamPelicanHasAliveLover && statistics.TeamLoversAlive == 2) && !killingCrewAlive())
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PelicanWin, false);
            return true;
        }
        return false;
    }
    private static bool CheckAndEndGameForWerewolfWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (
            statistics.TeamWerewolfAlive >= statistics.TotalAlive - statistics.TeamWerewolfAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJuggernautAlive == 0 &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamPavlovsAlive == 0 &&
            statistics.TeamArsonistAlive == 0 &&
            statistics.TeamPelicanAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            !(statistics.TeamWerewolfHasAliveLover && statistics.TeamLoversAlive == 2) && !killingCrewAlive()
        )
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.WerewolfWin, false);
            return true;
        }

        return false;
    }

    private static bool CheckAndEndGameForJuggernautWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (
            statistics.TeamJuggernautAlive >= statistics.TotalAlive - statistics.TeamJuggernautAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamPavlovsAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamPelicanAlive == 0 &&
            statistics.TeamArsonistAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            !(statistics.TeamJuggernautHasAliveLover && statistics.TeamLoversAlive == 2) && !killingCrewAlive()
        )
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JuggernautWin, false);
            return true;
        }

        return false;
    }

    private static bool CheckAndEndGameForImpostorWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (Vortox.triggerImpWin || (statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamPavlovsAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            statistics.TeamArsonistAlive == 0 &&
            statistics.TeamPelicanAlive == 0 &&
            statistics.TeamAkujoAlive == 0 &&
            statistics.TeamJuggernautAlive == 0 &&
            !(statistics.TeamImpostorHasAliveLover && statistics.TeamLoversAlive == 2) && !killingCrewAlive()))
        {
            //__instance.enabled = false;
            GameOverReason endReason;
            switch (TempData.LastDeathReason)
            {
                case DeathReason.Exile:
                    endReason = GameOverReason.ImpostorByVote;
                    break;
                case DeathReason.Kill:
                    endReason = GameOverReason.ImpostorByKill;
                    break;
                default:
                    endReason = GameOverReason.ImpostorByVote;
                    break;
            }

            GameManager.Instance.RpcEndGame(endReason, false);
            return true;
        }

        return false;
    }

    private static bool CheckAndEndGameForCrewmateWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamPavlovsAlive == 0 &&
            statistics.TeamArsonistAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamPelicanAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            statistics.TeamJuggernautAlive == 0)
        {
            if (Akujo.honmeiOptimizeWin || statistics.TeamAkujoAlive <= 1)
            {
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
                return true;
            }
        }
        return false;
    }

    private static void EndGameForSabotage(ShipStatus __instance)
    {
        //__instance.enabled = false;
        GameManager.Instance.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
internal class RPCEndGamePatch
{
    public static bool Prefix(GameOverReason endReason)
    {
        if (endReason == GameOverReason.HumansByTask && ModOption.DisableGameEnd) return false;
        return true;
    }

    public static void Postfix(ref GameOverReason endReason)
    {
        Message($"游戏结束 {(CustomGameOverReason)endReason} {endReason}", "RpcEndGame");
    }
}

internal class PlayerStatistics
{
    public PlayerStatistics(ShipStatus __instance)
    {
        GetPlayerCounts();
    }

    public int TeamImpostorsAlive { get; set; }
    public int TeamJackalAlive { get; set; }
    public int TeamPavlovsAlive { get; set; }
    public int TeamLoversAlive { get; set; }
    public int TotalAlive { get; set; }
    public int TeamSwooperAlive { get; set; }
    public int TeamPelicanAlive { get; set; }
    public int TeamBandLeaderAlive { get; set; }
    public bool TeamImpostorHasAliveLover { get; set; }
    public bool TeamJackalHasAliveLover { get; set; }
    public bool TeamPavlovsHasAliveLover { get; set; }
    public int TeamWerewolfAlive { get; set; }
    public int TeamArsonistAlive { get; set; }
    public int TeamAkujoAlive { get; set; }
    public bool TeamSwooperHasAliveLover { get; set; }
    public bool TeamWerewolfHasAliveLover { get; set; }
    public bool TeamArsonisHasAliveLover { get; set; }
    public bool TeamPelicanHasAliveLover { get; set; }
    public int TeamJuggernautAlive { get; set; }
    public bool TeamJuggernautHasAliveLover { get; set; }

    private static bool isLover(GameData.PlayerInfo p)
    {
        return (Lovers.lover1 != null && Lovers.lover1.PlayerId == p.PlayerId) ||
               (Lovers.lover2 != null && Lovers.lover2.PlayerId == p.PlayerId);
    }

    private void GetPlayerCounts()
    {
        var numJackalAlive = 0;
        var numPavlovsAlive = 0;
        var numBandLeaderAlive = 0;
        var numImpostorsAlive = 0;
        var numLoversAlive = 0;
        var numTotalAlive = 0;
        var numSwooperAlive = 0;
        var numPelicanAlive = 0;
        var numArsonistAlive = 0;
        var numWerewolfAlive = 0;
        var numJuggernautAlive = 0;
        var numAkujoAlive = 0;
        var impLover = false;
        var jackalLover = false;
        var pavlovsLover = false;
        var arsonistLover = false;
        var swooperLover = false;
        var pelicanLover = false;
        var werewolfLover = false;
        var juggernautLover = false;

        foreach (var playerData in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            if (!playerData.Disconnected && !playerData.IsDead)
            {
                numTotalAlive++;

                var lover = isLover(playerData);
                if (lover) numLoversAlive++;

                if (playerData.Role.IsImpostor)
                {
                    numImpostorsAlive++;
                    if (lover) impLover = true;
                }
                if (Jackal.jackal != null && Jackal.jackal.Any(x => x.PlayerId == playerData.PlayerId))
                {
                    numJackalAlive++;
                    if (lover) jackalLover = true;
                }
                if (Jackal.Sidekick != null && Jackal.Sidekick.PlayerId == playerData.PlayerId)
                {
                    numJackalAlive++;
                    if (lover) jackalLover = true;
                }
                if (Arsonist.arsonist != null && Arsonist.arsonist.PlayerId == playerData.PlayerId)
                {
                    numArsonistAlive++;
                    if (lover) arsonistLover = true;
                }
                if (Pavlovsdogs.pavlovsowner != null && Pavlovsdogs.pavlovsowner.PlayerId == playerData.PlayerId && !Pavlovsdogs.loser)
                {
                    numPavlovsAlive++;
                    if (lover) pavlovsLover = true;
                }
                if (Pavlovsdogs.pavlovsdogs != null && Pavlovsdogs.pavlovsdogs.Any(p => p.PlayerId == playerData.PlayerId))
                {
                    numPavlovsAlive++;
                    if (lover) pavlovsLover = true;
                }
                if (BandLeader.Player != null && (BandLeader.Members.Any(p => p.PlayerId == playerData.PlayerId) || BandLeader.Player?.PlayerId == playerData.PlayerId))
                {
                    numBandLeaderAlive++;
                }
                if (Werewolf.werewolf != null && Werewolf.werewolf.PlayerId == playerData.PlayerId)
                {
                    numWerewolfAlive++;
                    if (lover) werewolfLover = true;
                }
                if (Swooper.swooper != null && Swooper.swooper.PlayerId == playerData.PlayerId)
                {
                    numSwooperAlive++;
                    if (lover) swooperLover = true;
                }
                if (Juggernaut.juggernaut != null && Juggernaut.juggernaut.PlayerId == playerData.PlayerId)
                {
                    numJuggernautAlive++;
                    if (lover) juggernautLover = true;
                }
                if (Pelican.Player != null && Pelican.Player.PlayerId == playerData.PlayerId)
                {
                    numPelicanAlive++;
                    if (lover) pelicanLover = true;
                }
                if (Akujo.akujo != null && Akujo.akujo.PlayerId == playerData.PlayerId)
                {
                    numAkujoAlive++;
                }
                if (Akujo.honmei != null && Akujo.honmei.PlayerId == playerData.PlayerId)
                {
                    numAkujoAlive++;
                }
                if (SchrodingersCat.Player.IsAlive() && SchrodingersCat.Player.PlayerId == playerData.PlayerId)
                {
                    if (SchrodingersCat.CanKill)
                    {
                        switch (SchrodingersCat.State)
                        {
                            case SchrodingersCat.CatState.Impostor:
                                numImpostorsAlive++;
                                break;
                            case SchrodingersCat.CatState.Jackal:
                                numJackalAlive++;
                                break;
                            case SchrodingersCat.CatState.Pavlovsowner:
                                numPavlovsAlive++;
                                break;
                            case SchrodingersCat.CatState.Werewolf:
                                numWerewolfAlive++;
                                break;
                            case SchrodingersCat.CatState.Juggernaut:
                                numJuggernautAlive++;
                                break;
                            case SchrodingersCat.CatState.Swooper:
                                numSwooperAlive++;
                                break;
                            case SchrodingersCat.CatState.Arsonist:
                                numArsonistAlive++;
                                break;
                            case SchrodingersCat.CatState.Pelican:
                                numPelicanAlive++;
                                break;
                            case SchrodingersCat.CatState.Crewmate:
                                break;
                            case SchrodingersCat.CatState.None:
                                numTotalAlive--;
                                break;
                            default:
                                numTotalAlive--;
                                break;
                        }
                    }
                    else
                    {
                        numTotalAlive--;
                    }
                }
            }
        }

        TeamJackalAlive = numJackalAlive;
        TeamImpostorsAlive = numImpostorsAlive;
        TeamLoversAlive = numLoversAlive;
        TeamPavlovsAlive = numPavlovsAlive;
        TotalAlive = numTotalAlive;
        TeamAkujoAlive = numAkujoAlive;
        TeamBandLeaderAlive = numBandLeaderAlive;
        TeamImpostorHasAliveLover = impLover;
        TeamJackalHasAliveLover = jackalLover;
        TeamPelicanHasAliveLover = pelicanLover;
        TeamPavlovsHasAliveLover = pavlovsLover;
        TeamWerewolfHasAliveLover = werewolfLover;
        TeamWerewolfAlive = numWerewolfAlive;
        TeamArsonistAlive = numArsonistAlive;
        TeamSwooperHasAliveLover = swooperLover;
        TeamSwooperAlive = numSwooperAlive;
        TeamJuggernautAlive = numJuggernautAlive;
        TeamPelicanAlive = numPelicanAlive;
        TeamArsonisHasAliveLover = arsonistLover;
        TeamJuggernautHasAliveLover = juggernautLover;
    }
}