using TheOtherRoles.Objects;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Neutral;

public class Avenger
{
    public static HashSet<byte> DeathEventHandled = new();
    public static PlayerControl Player;
    public static PlayerControl Lover;
    public static RoleId originRole;
    public static Color color = new Color32(141, 111, 131, byte.MaxValue);

    public static bool EndGame => WinFlag || Player.IsDead() || Target.IsDead();

    public static float killCooldown = 30f;
    public static bool IsGuessable;
    public static bool CanFreeKill { get => !KnowTarget || field; set => field = value; }
    public static bool canUseVents;
    public static bool hasImpostorVision;
    public static bool ShowArrows { get => field && KnowTarget; set => field = value; }
    public static bool KnowTarget;
    public static float UpdateIntervall;
    public static bool OnlyAliveWin;
    public static bool TargetKnowPlayer;
    public static WinnerFlags WinCondition;
    public static AvengerTargetWasDead TargetWasKilledByOther;
    public static AvengerTargetWasDead TargetWasExiled;

    public static bool WinFlag { get => (!OnlyAliveWin || Player.IsAlive()) && field; set => field = value; }
    public static Arrow Arrow;
    public static float ArrowTimer;

    public static PlayerControl Target;
    public static PlayerControl currentTarget;

    public static void ClearAndReload()
    {
        Player = Lover = null;
        Target = currentTarget = null;
        WinFlag = false;
        ArrowTimer = 0f;
        DeathEventHandled.Clear();
        IsGuessable = CustomOptionHolder.avengerIsGuessable.GetBool();
        CanFreeKill = CustomOptionHolder.avengerCanFreeKill.GetBool();
        canUseVents = CustomOptionHolder.avengerCanUseVents.GetBool();
        hasImpostorVision = CustomOptionHolder.avengerHasImpVision.GetBool();
        KnowTarget = CustomOptionHolder.avengerKnowTarget.GetBool();
        killCooldown = CustomOptionHolder.avengerKillCooldown.GetFloat();
        ShowArrows = CustomOptionHolder.avengerShowArrows.GetBool();
        UpdateIntervall = CustomOptionHolder.avengerUpdateIntervall.GetFloat();
        TargetKnowPlayer = CustomOptionHolder.avengerTargetKnowPlayer.GetBool();
        OnlyAliveWin = CustomOptionHolder.avengerOnlyAliveWin.GetBool();
        WinCondition = CustomOptionHolder.avengerWinCondition.GetSelection<WinnerFlags>();
        TargetWasKilledByOther = CustomOptionHolder.avengerTargetWasKilledByOther.GetSelection<AvengerTargetWasDead>();
        TargetWasExiled = CustomOptionHolder.avengerTargetWasExiled.GetSelection<AvengerTargetWasDead>();
        Arrow?.arrow?.Destroy();
        Arrow = null;
    }

    private static void SetAvenger(PlayerControl killer, PlayerControl target, bool exile = false)
    {
        if (target == null) return;

        var otherLover = Lovers.otherLover(target);
        if (otherLover == null) return;

        if (killer != null &&
            Lovers.IsAvengerLover &&
            killer != target &&
            killer != otherLover &&
            killer.IsAlive())
        {
            Message("Avenger Lover Kill Other Player");

            originRole = RoleInfo.getRoleInfoForPlayer(otherLover, false, false)
                        .FirstOrDefault()?.roleId ?? RoleId.DefaultRole;

            RPCProcedure.erasePlayerRoles(otherLover.PlayerId);
            RPCProcedure.setRole(otherLover.PlayerId, (byte)RoleId.Avenger);

            Player = otherLover;
            Lover = target;
            Target = killer;
            Lovers.clearAndReload();

            if (otherLover == Lawyer.target && Lawyer.lawyer != null)
                Lawyer.PromotesToPursuer(false);
            if (otherLover == Executioner.target && Executioner.executioner != null)
                Executioner.PromotesRole();

            if ((TargetKnowPlayer && Target?.AmOwner == true) || Player.AmOwner)
                Coroutines.Start(showFlashCoroutine(color, 1.25f, 0.4f));
        }
        else
        {
            Message($"Lover Is Die, Exiled: {exile}");
            if (Player.IsAlive())
            {
                if (exile)
                    Player.CustomExiled(null, true);
                else
                    Player.MurderPlayer(Player, MurderResultFlags.Succeeded);
                PlayerData.SetDeathReason(Player, CustomDeathReason.AvengerFail);
            }
        }
    }

    private static void AvengerTargetDied(PlayerControl killer, PlayerControl target, bool exile = false)
    {
        if (Player.IsDead() || target == null || target != Target || WinFlag) return;

        var outcome = killer == null ? TargetWasExiled : TargetWasKilledByOther;

        if (killer == Player)
        {
            WinFlag = true;
            Message("Avenger Win!", "Avenger");
            if (AmongUsClient.Instance.AmHost && WinCondition == WinnerFlags.RevengeWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.AvengerTeamWin, false);
            }
            return;
        }

        if ((int)outcome > 2)
        {
            byte roleId = outcome switch
            {
                AvengerTargetWasDead.Jester => (byte)RoleId.Jester,
                AvengerTargetWasDead.Amnisiac => (byte)RoleId.Amnisiac,
                AvengerTargetWasDead.Survivor => (byte)RoleId.Survivor,
                _ => (byte)RoleId.Crewmate
            };
            RPCProcedure.setRole(Player.PlayerId, roleId);
            ClearAndReload();
            return;
        }

        Message($"Player Is Die, Exiled: {exile}", "Avenger");

        if (outcome is AvengerTargetWasDead.RestoreRole or AvengerTargetWasDead.SuicideRestore)
        {
            if (Player.AmOwner)
            {
                var writer = StartRPC(CustomRPC.SetRole);
                writer.Write(Player.PlayerId);
                writer.Write((byte)originRole);
                writer.EndRPC();
                RPCProcedure.setRole(Player.PlayerId, (byte)originRole);
            }
            ClearAndReload();
        }

        if (outcome is AvengerTargetWasDead.Suicide or AvengerTargetWasDead.SuicideRestore)
        {
            if (exile)
                Player.CustomExiled(null, true);
            else
                Player.MurderPlayer(Player, MurderResultFlags.Succeeded);
            PlayerData.SetDeathReason(Player, CustomDeathReason.AvengerFail);
        }
    }

    public static void OnPlayerDeath(PlayerControl killer, PlayerControl target, bool exile = false)
    {
        if (target == null || !ShouldHandleDeath(target)) return;
        SetAvenger(killer, target, exile);

        if (Target == target) AvengerTargetDied(killer, target, exile);
    }

    public static bool ShouldHandleDeath(PlayerControl target)
    {
        if (target == null) return false;
        var playerId = target.PlayerId;

        if (DeathEventHandled.Contains(playerId))
            return false;

        DeathEventHandled.Add(playerId);
        return true;
    }

    public enum WinnerFlags
    {
        FollowWin,
        StealWin,
        RevengeWin
    }

    public enum AvengerTargetWasDead
    {
        Suicide,        // 自杀
        RestoreRole,    // 恢复原职业
        SuicideRestore, // 自杀并恢复原职业
        Jester,         // 变为小丑
        Amnisiac,       // 变为失忆者
        Survivor        // 变为幸存者
    }
}
