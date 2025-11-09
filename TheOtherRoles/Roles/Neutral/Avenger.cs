using AmongUs.GameOptions;
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
        WinCondition = (WinnerFlags)CustomOptionHolder.avengerWinCondition.GetSelection();
        TargetWasKilledByOther = (AvengerTargetWasDead)CustomOptionHolder.avengerTargetWasKilledByOther.GetSelection();
        TargetWasExiled = (AvengerTargetWasDead)CustomOptionHolder.avengerTargetWasExiled.GetSelection();
        Arrow?.arrow?.Destroy();
        Arrow = null;
    }

    private static void SetAvenger(PlayerControl killer, PlayerControl target, bool exile = false)
    {
        if (target == null) return;

        var otherLover = Lovers.otherLover(target);
        if (Lovers.isLover(target) && Lovers.otherLover(target) != null)
        {
            if (!exile && killer != null && Lovers.IsAvengerLover && killer != target && killer != otherLover && killer.IsAlive())
            {
                ClearAndReload();
                RPCProcedure.erasePlayerRoles(otherLover.PlayerId);

                RPCProcedure.setRole(otherLover.PlayerId, (byte)RoleId.Avenger);
                Player = otherLover;
                Lover = target;
                Target = killer;

                SetRoleType(Player, RoleTypes.Crewmate);
                Lovers.clearAndReload();

                if ((Target != null && Target.AmOwner) || Player.AmOwner) Coroutines.Start(showFlashCoroutine(color, 1.25f, 0.4f));
            }
            else
            {
                if (otherLover.IsAlive())
                {
                    if (exile) otherLover.SetDie(CustomDeathReason.LoverSuicide);
                    else otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    PlayerData.SetDeathReason(otherLover, CustomDeathReason.LoverSuicide);
                }
            }
        }
    }

    private static void AvengerTargetDied(PlayerControl killer, PlayerControl target, bool exile = false)
    {
        if (Player.IsDead() || target == null || target != Target || WinFlag) return;

        var outcome = exile ? TargetWasExiled : TargetWasKilledByOther;

        if (killer == Player)
        {
            WinFlag = true;
            Message("Avenger Win!", "Avenger");
            if (AmongUsClient.Instance.AmHost && WinCondition == WinnerFlags.RevengeWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.AvengerTeamWin, false);
            }
        }
        else if ((int)outcome > 0)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                byte roleId = outcome switch
                {
                    AvengerTargetWasDead.Jester => (byte)RoleId.Jester,
                    AvengerTargetWasDead.Amnisiac => (byte)RoleId.Amnisiac,
                    AvengerTargetWasDead.Survivor => (byte)RoleId.Survivor,
                    _ => (byte)RoleId.Crewmate
                };

                var writer = StartRPC(CustomRPC.SetRole);
                writer.Write(Player.PlayerId);
                writer.Write(roleId);
                writer.EndRPC();
                RPCProcedure.setRole(Player.PlayerId, roleId);
            }
            ClearAndReload();
        }
        else
        {
            Message($"Player Is Die, Exlied:{exile}");
            if (Player.IsAlive())
            {
                if (exile) Player.SetDie(CustomDeathReason.AvengerFail);
                else Player.MurderPlayer(Player, MurderResultFlags.Succeeded);
                PlayerData.SetDeathReason(Player, CustomDeathReason.AvengerFail);
            }
        }
    }

    public static void OnPlayerDeath(PlayerControl killer, PlayerControl target, bool exile = false)
    {
        if (target == null || !ShouldHandleDeath(target)) return;

        SetAvenger(killer, target, exile);

        if (Target == target)
        {
            AvengerTargetDied(killer, target, exile);
            Message($"killer: {killer?.Data?.PlayerName ?? "NULL"} target:{target?.Data?.PlayerName ?? "NULL"}", "AvengerTargetDied");
        }
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
        Suicide,
        Jester,
        Amnisiac,
        Survivor
    }
}
