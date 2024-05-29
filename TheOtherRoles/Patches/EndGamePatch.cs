using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Roles;
using TheOtherRoles.Roles.Impostor;
using TheOtherRoles.Roles.Modifier;
using TheOtherRoles.Roles.Neutral;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

internal enum CustomGameOverReason
{
    LoversWin = 10,
    TeamJackalWin = 11,
    MiniLose = 12,
    JesterWin = 13,
    ArsonistWin = 14,
    VultureWin = 15,
    LawyerSoloWin = 16,
    ExecutionerWin = 17,
    SwooperWin = 18,
    WerewolfWin = 19,
    JuggernautWin = 20,
    DoomsayerWin = 21,
    AkujoWin = 22,
    ProsecutorWin,
}

internal enum WinCondition
{
    Default,
    LoversTeamWin,
    LoversSoloWin,
    JesterWin,
    JackalWin,
    SwooperWin,
    MiniLose,
    ArsonistWin,
    VultureWin,
    LawyerSoloWin,
    AdditionalLawyerBonusWin,
    AdditionalLawyerStolenWin,
    AdditionalAlivePursuerWin,
    ProsecutorWin,
    ExecutionerWin,
    WerewolfWin,
    JuggernautWin,
    DoomsayerWin,
    AkujoWin,
    EveryoneDied
}

internal static class AdditionalTempData
{
    // Should be implemented using a proper GameOverReason in the future
    public static WinCondition winCondition = WinCondition.Default;
    public static List<WinCondition> additionalWinConditions = new();
    public static List<PlayerRoleInfo> playerRoles = new();
    public static float timer;

    public static void clear()
    {
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
        Helpers.toggleZoom(true);
    }

    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        AdditionalTempData.clear();

        foreach (var playerControl in CachedPlayer.AllPlayers)
        {
            var roles = RoleInfo.getRoleInfoForPlayer(playerControl);
            var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(playerControl.Data);
            //末日预言家赌的功能
            var isGuesser = HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(playerControl.PlayerId);
            int? killCount = GameHistory.deadPlayers.FindAll(x =>
                x.killerIfExisting != null && x.killerIfExisting.PlayerId == playerControl.PlayerId).Count;
            if (killCount == 0 &&
                !(new List<RoleInfo>
                          { RoleInfo.sheriff, RoleInfo.jackal, RoleInfo.sidekick, RoleInfo.thief, RoleInfo.juggernaut }
                      .Contains(RoleInfo.getRoleInfoForPlayer(playerControl, false).FirstOrDefault()) ||
                  playerControl.Data.Role.IsImpostor)) killCount = null;
            var roleString = RoleInfo.GetRolesString(playerControl, true);
            AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo
            {
                PlayerName = playerControl.Data.PlayerName,
                Roles = roles,
                RoleNames = roleString,
                TasksTotal = tasksTotal,
                TasksCompleted = tasksCompleted,
                IsGuesser = isGuesser,
                Kills = killCount,
                IsAlive = !playerControl.Data.IsDead
            });

            if (Cultist.isCultistGame) GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors = 2;
        }

        // Remove Jester, Arsonist, Vulture, Jackal, former Jackals and Sidekick from winners (if they win, they'll be readded)
        var notWinners = new List<PlayerControl>();
        if (Jester.jester != null) notWinners.Add(Jester.jester);
        if (Sidekick.sidekick != null) notWinners.Add(Sidekick.sidekick);
        if (Amnisiac.amnisiac != null) notWinners.Add(Amnisiac.amnisiac);
        if (Jackal.jackal != null) notWinners.Add(Jackal.jackal);
        if (Arsonist.arsonist != null) notWinners.Add(Arsonist.arsonist);
        if (Swooper.swooper != null) notWinners.Add(Swooper.swooper);
        if (Vulture.vulture != null) notWinners.Add(Vulture.vulture);
        if (Werewolf.werewolf != null) notWinners.Add(Werewolf.werewolf);
        if (Lawyer.lawyer != null) notWinners.Add(Lawyer.lawyer);
        if (Pursuer.pursuer != null) notWinners.Add(Pursuer.pursuer);
        if (Thief.thief != null) notWinners.Add(Thief.thief);
        //天启添加
        if (Juggernaut.juggernaut != null) notWinners.Add(Juggernaut.juggernaut);
        if (Doomsayer.doomsayer != null) notWinners.Add(Doomsayer.doomsayer);
        if (Akujo.akujo != null) notWinners.Add(Akujo.akujo);
        if (Akujo.honmei != null && Akujo.honmeiCannotFollowWin) notWinners.Add(Akujo.honmei);
        notWinners.AddRange(Jackal.formerJackals);

        var winnersToRemove = new List<WinningPlayerData>();
        foreach (var winner in TempData.winners.GetFastEnumerator())
            if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName))
                winnersToRemove.Add(winner);
        foreach (var winner in winnersToRemove) TempData.winners.Remove(winner);

        var everyoneDead = AdditionalTempData.playerRoles.All(x => !x.IsAlive);
        var jesterWin = Jester.jester != null && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
        var werewolfWin = gameOverReason == (GameOverReason)CustomGameOverReason.WerewolfWin &&
                          Werewolf.werewolf != null && !Werewolf.werewolf.Data.IsDead;
        var juggernautWin = gameOverReason == (GameOverReason)CustomGameOverReason.JuggernautWin &&
                            Juggernaut.juggernaut != null && !Juggernaut.juggernaut.Data.IsDead;
        var swooperWin = gameOverReason == (GameOverReason)CustomGameOverReason.SwooperWin &&
                            Swooper.swooper != null && !Swooper.swooper.Data.IsDead;
        var arsonistWin = Arsonist.arsonist != null &&
                          gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
        var miniLose = Mini.mini != null && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
        var doomsayerWin = Doomsayer.doomsayer != null &&
                           gameOverReason == (GameOverReason)CustomGameOverReason.DoomsayerWin;
        // Either they win if they are among the last 3 players, or they win if they are both Crewmates and both alive and the Crew wins (Team Imp/Jackal Lovers can only win solo wins)
        var loversWin = Lovers.existingAndAlive() &&
                        (gameOverReason == (GameOverReason)CustomGameOverReason.LoversWin ||
                         (GameManager.Instance.DidHumansWin(gameOverReason) &&
                          !Lovers.existingWithKiller()));
        var teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin &&
                            ((Jackal.jackal != null && !Jackal.jackal.Data.IsDead) ||
                            (Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead));
        var vultureWin = Vulture.vulture != null && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
        var executionerWin = Executioner.executioner != null && gameOverReason == (GameOverReason)CustomGameOverReason.ExecutionerWin;
        var akujoWin = Akujo.akujo != null && gameOverReason == (GameOverReason)CustomGameOverReason.AkujoWin && Akujo.honmei != null && !Akujo.honmei.Data.IsDead && !Akujo.akujo.Data.IsDead;
        bool lawyerSoloWin = Lawyer.lawyer != null && gameOverReason == (GameOverReason)CustomGameOverReason.LawyerSoloWin;

        var isPursurerLose = jesterWin || arsonistWin || miniLose;

        // Mini lose
        if (miniLose)
        {
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Mini.mini.Data);
            wpd.IsYou = false; // If "no one is the Mini", it will display the Mini, but also show defeat to everyone
            TempData.winners.Add(wpd);
            AdditionalTempData.winCondition = WinCondition.MiniLose;
        }

        // Jester win
        else if (jesterWin)
        {
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Jester.jester.Data);
            TempData.winners.Add(wpd);
            AdditionalTempData.winCondition = WinCondition.JesterWin;
        }

        // Arsonist win
        else if (arsonistWin)
        {
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Arsonist.arsonist.Data);
            TempData.winners.Add(wpd);
            AdditionalTempData.winCondition = WinCondition.ArsonistWin;
        }

        // Everyone Died
        else if (everyoneDead)
        {
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            AdditionalTempData.winCondition = WinCondition.EveryoneDied;
        }

        // Vulture win
        else if (vultureWin)
        {
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Vulture.vulture.Data);
            TempData.winners.Add(wpd);
            AdditionalTempData.winCondition = WinCondition.VultureWin;
        }

        // Jester win
        else if (executionerWin)
        {
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Executioner.executioner.Data);
            TempData.winners.Add(wpd);
            AdditionalTempData.winCondition = WinCondition.ExecutionerWin;
        }

        // Lovers win conditions
        else if (loversWin)
        {
            // Double win for lovers, crewmates also win
            if (!Lovers.existingWithKiller())
            {
                AdditionalTempData.winCondition = WinCondition.LoversTeamWin;
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl p in CachedPlayer.AllPlayers)
                {
                    if (p == null) continue;
                    if (p == Lovers.lover1 || p == Lovers.lover2)
                        TempData.winners.Add(new WinningPlayerData(p.Data));
                    else if (p == Pursuer.pursuer && !Pursuer.pursuer.Data.IsDead)
                        TempData.winners.Add(new WinningPlayerData(p.Data));
                    else if (p != Jester.jester && p != Jackal.jackal && p != Werewolf.werewolf &&
                             p != Juggernaut.juggernaut && p != Doomsayer.doomsayer &&
                             p != Sidekick.sidekick && p != Arsonist.arsonist && p != Vulture.vulture &&
                             p != Swooper.swooper &&
                             p != Akujo.akujo &&
                             !Jackal.formerJackals.Contains(p) && !p.Data.Role.IsImpostor)
                        TempData.winners.Add(new WinningPlayerData(p.Data));
                }
            }
            // Lovers solo win
            else
            {
                AdditionalTempData.winCondition = WinCondition.LoversSoloWin;
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                TempData.winners.Add(new WinningPlayerData(Lovers.lover1.Data));
                TempData.winners.Add(new WinningPlayerData(Lovers.lover2.Data));
            }
        }

        // Jackal win condition (should be implemented using a proper GameOverReason in the future)
        else if (teamJackalWin)
        {
            // Jackal wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.JackalWin;
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Jackal.jackal.Data);
            wpd.IsImpostor = false;
            TempData.winners.Add(wpd);
            // If there is a sidekick. The sidekick also wins
            if (Sidekick.sidekick != null)
            {
                var wpdSidekick = new WinningPlayerData(Sidekick.sidekick.Data);
                wpdSidekick.IsImpostor = false;
                TempData.winners.Add(wpdSidekick);
            }

            foreach (var player in Jackal.formerJackals)
            {
                var wpdFormerJackal = new WinningPlayerData(player.Data);
                wpdFormerJackal.IsImpostor = false;
                TempData.winners.Add(wpdFormerJackal);
            }
        }

        else if (werewolfWin)
        {
            // Werewolf wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.WerewolfWin;
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Werewolf.werewolf.Data);
            wpd.IsImpostor = false;
            TempData.winners.Add(wpd);
        }
        //天启添加
        else if (juggernautWin)
        {
            // JuggernautWin wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.JuggernautWin;
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Juggernaut.juggernaut.Data);
            wpd.IsImpostor = false;
            TempData.winners.Add(wpd);
        }
        //末日预言家
        else if (doomsayerWin)
        {
            // DoomsayerWin wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.DoomsayerWin;
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Doomsayer.doomsayer.Data);
            TempData.winners.Add(wpd);
            AdditionalTempData.winCondition = WinCondition.DoomsayerWin;
        }
        //Swooper
        else if (swooperWin)
        {
            // Swooper wins if nobody except jackal is alive
            AdditionalTempData.winCondition = WinCondition.SwooperWin;
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            var wpd = new WinningPlayerData(Swooper.swooper.Data);
            wpd.IsImpostor = false;
            TempData.winners.Add(wpd);
        }

        // Akujo win
        else if (akujoWin)
        {
            AdditionalTempData.winCondition = WinCondition.AkujoWin;
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            TempData.winners.Add(new WinningPlayerData(Akujo.akujo.Data));
            TempData.winners.Add(new WinningPlayerData(Akujo.honmei.Data));
        }


        // Lawyer solo win 
        else if (lawyerSoloWin)
        {
            TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
            WinningPlayerData wpd = new WinningPlayerData(Lawyer.lawyer.Data);
            TempData.winners.Add(wpd);
            AdditionalTempData.winCondition = WinCondition.LawyerSoloWin;
        }

        // Possible Additional winner: Lawyer
        if (!lawyerSoloWin && Lawyer.lawyer != null && Lawyer.target != null &&
            (!Lawyer.target.Data.IsDead || Lawyer.target == Jester.jester) && !Pursuer.notAckedExiled)
        {
            WinningPlayerData winningClient = null;
            foreach (var winner in TempData.winners.GetFastEnumerator())
                if (winner.PlayerName == Lawyer.target.Data.PlayerName)
                    winningClient = winner;
            if (winningClient != null)
            {
                // The Lawyer wins if the client is winning (and alive, but if he wasn't the Lawyer shouldn't exist anymore)
                if (!TempData.winners.ToArray().Any(x => x.PlayerName == Lawyer.lawyer.Data.PlayerName))
                    TempData.winners.Add(new WinningPlayerData(Lawyer.lawyer.Data));
                AdditionalTempData.additionalWinConditions.Add(WinCondition
                    .AdditionalLawyerBonusWin); // The Lawyer wins together with the client 
            }
        }

        // Possible Additional winner: Pursuer
        if (Pursuer.pursuer != null && !Pursuer.pursuer.Data.IsDead && !Pursuer.notAckedExiled && !isPursurerLose)
        {
            if (!TempData.winners.ToArray().Any(x => x.PlayerName == Pursuer.pursuer.Data.PlayerName))
                TempData.winners.Add(new WinningPlayerData(Pursuer.pursuer.Data));
            AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalAlivePursuerWin);
        }

        AdditionalTempData.timer =
            (float)(DateTime.UtcNow - (HideNSeek.isHideNSeekGM ? HideNSeek.startTime : PropHunt.startTime))
            .TotalMilliseconds / 1000;

        // Reset Settings
        if (MapOptions.gameMode == CustomGamemodes.HideNSeek) ShipStatusPatch.resetVanillaSettings();
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
            Object.Destroy(pb.gameObject);
        var num = Mathf.CeilToInt(7.5f);
        var list = TempData.winners.ToArray().ToList().OrderBy(delegate (WinningPlayerData b)
        {
            if (!b.IsYou) return 0;
            return -1;
        }).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            var winningPlayerData2 = list[i];
            var num2 = i % 2 == 0 ? -1 : 1;
            var num3 = (i + 1) / 2;
            var num4 = num3 / (float)num;
            var num5 = Mathf.Lerp(1f, 0.75f, num4);
            float num6 = i == 0 ? -8 : -1;
            var poolablePlayer = Object.Instantiate(__instance.PlayerPrefab, __instance.transform);
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

            poolablePlayer.UpdateFromPlayerOutfit(winningPlayerData2, PlayerMaterial.MaskType.None,
                winningPlayerData2.IsDead, true);

            poolablePlayer.cosmetics.nameText.color = Color.white;
            poolablePlayer.cosmetics.nameText.transform.localScale =
                new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
            var localPosition = poolablePlayer.cosmetics.nameText.transform.localPosition;
            localPosition = new Vector3(
                localPosition.x,
                localPosition.y, -15f);
            poolablePlayer.cosmetics.nameText.transform.localPosition = localPosition;
            poolablePlayer.cosmetics.nameText.text = winningPlayerData2.PlayerName;

            foreach (var roles in from data in AdditionalTempData.playerRoles
                                  where data.PlayerName == winningPlayerData2.PlayerName
                                  select poolablePlayer.cosmetics.nameText.text +=
                         $"\n{string.Join("\n", data.Roles.Select(x => Helpers.cs(x.color, x.name)))}")
            {
            }
        }

        // Additional code
        var bonusText = Object.Instantiate(__instance.WinText.gameObject);
        var position1 = __instance.WinText.transform.position;
        bonusText.transform.position = new Vector3(position1.x,
            position1.y - 0.5f, position1.z);
        bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        var textRenderer = bonusText.GetComponent<TMP_Text>();
        textRenderer.text = "";

        switch (AdditionalTempData.winCondition)
        {
            case WinCondition.EveryoneDied:
                textRenderer.text = "无人生还";
                textRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                break;
            case WinCondition.JesterWin:
                textRenderer.text = "听我说谢谢你";
                textRenderer.color = Jester.color;
                break;
            case WinCondition.DoomsayerWin:
                textRenderer.text = "末日预言家获胜";
                textRenderer.color = Doomsayer.color;
                break;
            case WinCondition.ArsonistWin:
                textRenderer.text = "用火焰净化一切";
                textRenderer.color = Arsonist.color;
                break;
            case WinCondition.VultureWin:
                textRenderer.text = "吃饱饱！";
                textRenderer.color = Vulture.color;
                break;
            case WinCondition.LawyerSoloWin:
                textRenderer.text = "律师获胜";
                textRenderer.color = Lawyer.color;
                break;
            case WinCondition.WerewolfWin:
                textRenderer.text = "月下狼人获胜！";
                textRenderer.color = Werewolf.color;
                break;
            case WinCondition.JuggernautWin:
                textRenderer.text = "天启获胜";
                textRenderer.color = Juggernaut.color;
                break;
            case WinCondition.SwooperWin:
                textRenderer.text = "隐身人获胜!";
                textRenderer.color = Swooper.color;
                break;
            case WinCondition.ExecutionerWin:
                textRenderer.text = "小嘴叭叭!";
                textRenderer.color = Executioner.color;
                break;
            case WinCondition.LoversTeamWin:
                textRenderer.text = "船员和恋人获胜";
                textRenderer.color = Lovers.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
                break;
            case WinCondition.LoversSoloWin:
                textRenderer.text = "与你的爱恋心意合一~";
                textRenderer.color = Lovers.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
                break;
            case WinCondition.JackalWin:
                textRenderer.text = "豺狼的全家福.jpg";
                textRenderer.color = Jackal.color;
                break;
            case WinCondition.AkujoWin:
                textRenderer.text = "请给我扭曲你人生的权利！";
                textRenderer.color = Akujo.color;
                __instance.BackgroundBar.material.SetColor("_Color", Akujo.color);
                break;

            case WinCondition.MiniLose:
                textRenderer.text = "他就只是个孩子啊！";
                textRenderer.color = Mini.color;
                break;
        }

        foreach (var cond in AdditionalTempData.additionalWinConditions)
            switch (cond)
            {
                case WinCondition.AdditionalLawyerStolenWin:
                    textRenderer.text += $"\n{Helpers.cs(Lawyer.color, "律师代替客户胜利")}";
                    break;
                case WinCondition.AdditionalLawyerBonusWin:
                    textRenderer.text += $"\n{Helpers.cs(Lawyer.color, "律师和客户胜利")}";
                    break;
                case WinCondition.AdditionalAlivePursuerWin:
                    textRenderer.text += $"\n{Helpers.cs(Pursuer.color, "起诉人存活")}";
                    break;
            }

        if (MapOptions.showRoleSummary || HideNSeek.isHideNSeekGM || PropHunt.isPropHuntGM)
        {
            if (Camera.main != null)
            {
                var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                var roleSummary = Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f,
                    position.y - 0.1f, -214f);
                roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                var roleSummaryText = new StringBuilder();
                if (HideNSeek.isHideNSeekGM || PropHunt.isPropHuntGM)
                {
                    var minutes = (int)AdditionalTempData.timer / 60;
                    var seconds = (int)AdditionalTempData.timer % 60;
                    roleSummaryText.AppendLine($"<color=#FAD934FF>剩余时间: {minutes:00}:{seconds:00}</color> \n");
                }

                roleSummaryText.AppendLine("游戏总结:");
                foreach (var data in AdditionalTempData.playerRoles)
                {
                    //var roles = string.Join(" ", data.Roles.Select(x => Helpers.cs(x.color, x.name)));
                    var roles = data.RoleNames;
                    //if (data.IsGuesser) roles += " (Guesser)";
                    var taskInfo = data.TasksTotal > 0
                        ? $" - <color=#FAD934FF>({data.TasksCompleted}/{data.TasksTotal})</color>"
                        : "";
                    if (data.Kills != null) taskInfo += $" - <color=#FF0000FF>(击杀: {data.Kills})</color>";
                    roleSummaryText.AppendLine(
                        $"{Helpers.cs(data.IsAlive ? Color.white : new Color(.7f, .7f, .7f), data.PlayerName)} - {roles}{taskInfo}");
                }

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
        if (DestroyableSingleton<TutorialManager>
            .InstanceExists) // InstanceExists | Don't check Custom Criteria when in Tutorial
            return true;
        var statistics = new PlayerStatistics(__instance);
        if (CheckAndEndGameForMiniLose(__instance)) return false;
        if (CheckAndEndGameForJesterWin(__instance)) return false;
        if (CheckAndEndGameForDoomsayerWin(__instance)) return false;
        if (CheckAndEndGameForArsonistWin(__instance)) return false;
        if (CheckAndEndGameForVultureWin(__instance)) return false;
        if (CheckAndEndGameForSabotageWin(__instance)) return false;
        if (CheckAndEndGameForTaskWin(__instance)) return false;
        if (CheckAndEndGameForExecutionerWin(__instance)) return false;
        if (CheckAndEndGameForWerewolfWin(__instance, statistics)) return false;
        if (CheckAndEndGameForLoverWin(__instance, statistics)) return false;
        if (CheckAndEndGameForJackalWin(__instance, statistics)) return false;
        if (CheckAndEndGameForSwooperWin(__instance, statistics)) return false;
        if (CheckAndEndGameForImpostorWin(__instance, statistics)) return false;
        if (CheckAndEndGameForCrewmateWin(__instance, statistics)) return false;
        if (CheckAndEndGameForJuggernautWin(__instance, statistics)) return false;
        if (CheckAndEndGameForAkujoWin(__instance, statistics)) return false;
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

    private static bool CheckAndEndGameForArsonistWin(ShipStatus __instance)
    {
        if (Arsonist.triggerArsonistWin)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ArsonistWin, false);
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
        if (systemType2 == null)
            systemType2 = MapUtilities.Systems.ContainsKey(SystemTypes.Laboratory)
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
        if ((HideNSeek.isHideNSeekGM && !HideNSeek.taskWinPossible) || PropHunt.isPropHuntGM) return false;
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

    private static bool CheckAndEndGameForAkujoWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (statistics.TeamAkujoAlive == 2 && statistics.TotalAlive <= 3)
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
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamAkujoAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            !(statistics.TeamJackalHasAliveLover &&
              statistics.TeamLoversAlive == 2) && !Helpers.killingCrewAlive())
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TeamJackalWin, false);
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
            statistics.TeamWerewolfAlive == 0 &&
            !(statistics.TeamSwooperHasAliveLover &&
            statistics.TeamLoversAlive == 2) &&
            !Helpers.killingCrewAlive())
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.SwooperWin, false);
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
            statistics.TeamSwooperAlive == 0 &&
            !(statistics.TeamWerewolfHasAliveLover &&
              statistics.TeamLoversAlive == 2) &&
            !Helpers.killingCrewAlive()
        )
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.WerewolfWin, false);
            return true;
        }

        return false;
    }

    //天启添加
    private static bool CheckAndEndGameForJuggernautWin(ShipStatus __instance, PlayerStatistics statistics)
    {
        if (
            statistics.TeamJuggernautAlive >= statistics.TotalAlive - statistics.TeamJuggernautAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            !(statistics.TeamJuggernautHasAliveLover &&
              statistics.TeamLoversAlive == 2) &&
            !Helpers.killingCrewAlive()
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
        if (HideNSeek.isHideNSeekGM || PropHunt.isPropHuntGM)
            if (0 != statistics.TotalAlive - statistics.TeamImpostorsAlive)
                return false;

        if (statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            statistics.TeamAkujoAlive == 0 &&
            statistics.TeamJuggernautAlive == 0 &&
            !(statistics.TeamImpostorHasAliveLover && statistics.TeamLoversAlive == 2) && !Helpers.killingCrewAlive())
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
        if (HideNSeek.isHideNSeekGM && HideNSeek.timer <= 0 && !HideNSeek.isWaitingTimer)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
            return true;
        }
        if (PropHunt.isPropHuntGM && PropHunt.timer <= 0 && PropHunt.timerRunning)
        {
            GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
            return true;
        }
        if (statistics.TeamImpostorsAlive == 0 &&
            statistics.TeamJackalAlive == 0 &&
            statistics.TeamWerewolfAlive == 0 &&
            statistics.TeamSwooperAlive == 0 &&
            statistics.TeamJuggernautAlive == 0 &&
            statistics.TeamAkujoAlive <= 1)
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
            return true;
        }
        return false;
    }

    private static void EndGameForSabotage(ShipStatus __instance)
    {
        //__instance.enabled = false;
        GameManager.Instance.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
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
    public int TeamLoversAlive { get; set; }
    public int TotalAlive { get; set; }
    public int TeamSwooperAlive { get; set; }
    public bool TeamImpostorHasAliveLover { get; set; }
    public bool TeamJackalHasAliveLover { get; set; }
    public int TeamWerewolfAlive { get; set; }
    public int TeamAkujoAlive { get; set; }

    public bool TeamSwooperHasAliveLover { get; set; }
    public bool TeamWerewolfHasAliveLover { get; set; }

    //天启添加
    public int TeamJuggernautAlive { get; set; }
    public bool TeamJuggernautHasAliveLover { get; set; }

    private bool isLover(GameData.PlayerInfo p)
    {
        return (Lovers.lover1 != null && Lovers.lover1.PlayerId == p.PlayerId) ||
               (Lovers.lover2 != null && Lovers.lover2.PlayerId == p.PlayerId);
    }

    private void GetPlayerCounts()
    {
        var numJackalAlive = 0;
        var numImpostorsAlive = 0;
        var numLoversAlive = 0;
        var numTotalAlive = 0;
        var numSwooperAlive = 0;
        var swooperLover = false;
        var impLover = false;
        var jackalLover = false;
        var numWerewolfAlive = 0;
        var werewolfLover = false;
        //天启添加
        var numJuggernautAlive = 0;
        var juggernautLover = false;
        var numAkujoAlive = 0;

        foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            if (!playerInfo.Disconnected)
                if (!playerInfo.IsDead)
                {
                    numTotalAlive++;

                    var lover = isLover(playerInfo);
                    if (lover) numLoversAlive++;

                    if (playerInfo.Role.IsImpostor)
                    {
                        numImpostorsAlive++;
                        if (lover) impLover = true;
                    }

                    if (Jackal.jackal != null && Jackal.jackal.PlayerId == playerInfo.PlayerId)
                    {
                        numJackalAlive++;
                        if (lover) jackalLover = true;
                    }

                    if (Sidekick.sidekick != null && Sidekick.sidekick.PlayerId == playerInfo.PlayerId)
                    {
                        numJackalAlive++;
                        if (lover) jackalLover = true;
                    }

                    if (Werewolf.werewolf != null && Werewolf.werewolf.PlayerId == playerInfo.PlayerId)
                    {
                        numWerewolfAlive++;
                        if (lover) werewolfLover = true;
                    }
                    if (Swooper.swooper != null && Swooper.swooper.PlayerId == playerInfo.PlayerId)
                    {
                        numSwooperAlive++;
                        if (lover) swooperLover = true;
                    }
                    //天启添加
                    if (Juggernaut.juggernaut != null && Juggernaut.juggernaut.PlayerId == playerInfo.PlayerId)
                    {
                        numJuggernautAlive++;
                        if (lover) juggernautLover = true;
                    }
                    if (Akujo.akujo != null && Akujo.akujo.PlayerId == playerInfo.PlayerId)
                    {
                        numAkujoAlive++;
                    }
                    if (Akujo.honmei != null && Akujo.honmei.PlayerId == playerInfo.PlayerId)
                    {
                        numAkujoAlive++;
                    }
                }

        TeamJackalAlive = numJackalAlive;
        TeamImpostorsAlive = numImpostorsAlive;
        TeamLoversAlive = numLoversAlive;
        TotalAlive = numTotalAlive;
        TeamAkujoAlive = numAkujoAlive;
        TeamImpostorHasAliveLover = impLover;
        TeamJackalHasAliveLover = jackalLover;
        TeamWerewolfHasAliveLover = werewolfLover;
        TeamWerewolfAlive = numWerewolfAlive;
        TeamSwooperHasAliveLover = swooperLover;
        TeamSwooperAlive = numSwooperAlive;
        //天启添加
        TeamJuggernautAlive = numJuggernautAlive;
        TeamJuggernautHasAliveLover = juggernautLover;
    }
}