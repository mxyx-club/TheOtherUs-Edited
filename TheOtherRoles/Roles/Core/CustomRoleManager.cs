using AmongUs.GameOptions;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles;
#nullable enable
public static class CustomRoleManager
{
    public static List<IRoleInfo> AllRoles = new();
    public static Dictionary<byte, RoleBase> AllActiveRoles = new();
    public static Dictionary<byte, List<ModifierBase>> AllActiveModifier = new();
    public static readonly Dictionary<RoleId, RoleInfo> AllRolesInfo = new();

    public static RoleInfo? GetRoleInfo(this RoleId role) => AllRolesInfo.TryGetValue(role, out var info) ? info : null;
    public static RoleBase? GetRoleBase(this PlayerControl player) => AllActiveRoles.TryGetValue(player.PlayerId, out var roleBase)
        ? roleBase : null;
    public static IEnumerable<ModifierBase> GetModifierBase(this PlayerControl player) => AllActiveModifier.TryGetValue(player.PlayerId, out var roleBase)
        ? roleBase : new();

    public static RoleBase? CreateRole(PlayerControl? player, RoleId role)
    {
        if (player == null || !AllRolesInfo.TryGetValue(role, out var roleInfo)) return null;

        var roleInstance = roleInfo.CreateInstance(player);
        return roleInstance;
    }

    public static RoleBase? CreateRoleById(byte playerId, RoleId roleId)
    {
        if (!AllRolesInfo.TryGetValue(roleId, out var roleInfo))
            return null;

        var player = PlayerById(playerId);
        if (player == null || player.Data.Disconnected)
            return null;

        var roleInstance = roleInfo.CreateInstance(player);
        return roleInstance;
    }

    public static ModifierBase? CreateModifier(PlayerControl? player, RoleId roleId)
    {
        if (player == null || !AllRolesInfo.TryGetValue(roleId, out var roleInfo)) return null;

        var roleInstance = roleInfo.CreateModifier(player);
        return roleInstance;
    }

    public static void ShiftRole(this PlayerControl player, PlayerControl target, bool reset = false)
    {
        if (player == null || target == null) return;

        if (!AllActiveRoles.TryGetValue(player.PlayerId, out var playerRole) || !AllActiveRoles.TryGetValue(target.PlayerId, out var targetRole)) return;

        (playerRole.Player, targetRole.Player) = (targetRole.Player, playerRole.Player);

        AllActiveRoles.Remove(player.PlayerId);
        AllActiveRoles.Remove(target.PlayerId);

        AllActiveRoles.Add(playerRole.Player.PlayerId, playerRole);
        AllActiveRoles.Add(targetRole.Player.PlayerId, targetRole);

        if (reset)
        {
            playerRole.Initialize();
            targetRole.Initialize();
        }
    }

    public static void RemoveRole(this PlayerControl? player)
    {
        if (player == null || !AllActiveRoles.TryGetValue(player.PlayerId, out var playerRole)) return;
        playerRole?.Destroy();
    }

    public static void RemoveRole(this PlayerControl? player, RoleId roleId)
    {
        if (player == null) return;
        foreach (var role in AllRoles.Where(x => x.PlayerId == player.PlayerId))
        {
            if (role.RoleId == roleId)
            {
                role.Destroy();
                break;
            }
        }
    }

    public static void SetRoleType(PlayerControl player, RoleTypes roleType)
    {
        try
        {
            if (player == null || player.Data == null) return;
            var data = player.Data;
            if (data.Role)
            {
                data.Role.Deinitialize(player);
                UObject.Destroy(data.Role.gameObject);
            }
            if (RoleManager.Instance == null) return;
            var roleBehaviour = UObject.Instantiate(RoleManager.Instance.AllRoles.First(r => r.Role == roleType), GameData.Instance.transform);
            roleBehaviour.Initialize(player);
            player.Data.Role = roleBehaviour;
            player.Data.RoleType = roleType;
            roleBehaviour.AdjustTasks(player);
        }
        catch (Exception e)
        {
            Error(e);
        }
    }

    /// <summary>
    /// 全部对象的销毁事件
    /// </summary>
    public static void Dispose()
    {
        Info($"Dispose ActiveRoles");
        OnFixedUpdateOthers.Clear();

        handcuffedPlayers = new();
        handcuffedKnows = new();
        CustomButton.setAllButtonsHandcuffedStatus(false, true);

        AllActiveRoles.Values.ToArray().Do(roleClass => roleClass.Destroy());
    }

    /// <summary>
    /// 其他玩家视角下的帧 Task 处理事件
    /// 用于干涉其他职业
    /// </summary>
    public static HashSet<Action<PlayerControl>> OnFixedUpdateOthers = new();

    public static void OnFixedUpdate(PlayerControl player)
    {
        if (InGame && IntroCutscene.Instance == null)
        {
            try
            {
                player.GetRoleBase()?.OnFixedUpdate(player);
                player.GetModifierBase()?.Do(x => x.OnFixedUpdate(player));

                OnFixedUpdateOthers.Do(x => x.Invoke(player));
            }
            catch (Exception e)
            {
                Warn(e);
            }
        }
    }

    public static void OnGameStart()
    {
        try
        {
            AllActiveRoles.Values.Do(x => x.OnGameStart());
            AllActiveModifier.Values.SelectMany(x => x).Do(x => x.OnGameStart());
        }
        catch (Exception e)
        {
            Warn(e);
        }
    }

    public static void OnHudUpdate(HudManager hudManager)
    {
        if (IntroCutscene.Instance == null)
        {
            try
            {
                AllActiveRoles.Values.Do(x => x.OnHudUpdate(hudManager));
                AllActiveModifier.Values.SelectMany(x => x).Do(x => x.OnHudUpdate(hudManager));
            }
            catch (Exception e)
            {
                Warn(e);
            }
        }
    }

    public static bool OnCheckReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target)
    {
        if (reporter.IsDead()) return false;
        var flag = AllActiveRoles.Values.All(x => x.OnCheckReportDeadBody(reporter, target));
        return flag;
    }

    public static void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        try
        {
            AllActiveRoles.Values.Do(x => x.OnExiledBegin(exiled));
            AllActiveModifier.Values.SelectMany(x => x).Do(x => x.OnExiledBegin(exiled));
        }
        catch (Exception e) { Warn(e); }
    }

    /// <summary>
    /// 处理玩家掉线事件
    /// </summary>
    /// <param name="player">掉线的玩家</param>
    public static void OnPlayerDisconnect(PlayerControl player)
    {
        try
        {
            var ActiveRoles = AllActiveRoles.Values;
            var ActiveAbilities = AllActiveModifier.Values.SelectMany(x => x);
            if (InGame)
            {
                ActiveRoles.Do(x => x.OnPlayerDisconnect(player));
                ActiveAbilities.Do(x => x.Destroy());
                player?.GetRoleBase()?.Destroy();
            }
        }
        catch (Exception e)
        {
            Warn(e);
        }
    }




    public static List<byte> handcuffedPlayers = new();
    public static Dictionary<byte, float> handcuffedKnows = new();
    public static Dictionary<byte, byte> Sheriffs = new();

    // Can be used to enable / disable the handcuff effect on the target's buttons
    public static void setHandcuffedKnows(bool active = true, byte playerId = byte.MaxValue)
    {
        if (playerId == byte.MaxValue) playerId = PlayerControl.LocalPlayer.PlayerId;

        if (active && playerId == PlayerControl.LocalPlayer.PlayerId)
        {
            var writer = StartRPC(CustomRPC.ShareGhostInfo);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffNoticed);
            writer.EndRPC();
        }

        if (active)
        {
            handcuffedKnows.Add(playerId, Deputy.handcuffDuration);
            handcuffedPlayers.RemoveAll(x => x == playerId);
        }

        if (playerId == PlayerControl.LocalPlayer.PlayerId)
        {
            CustomButton.setAllButtonsHandcuffedStatus(active);
            SoundEffectsManager.play("deputyHandcuff");
        }
    }

    public static HashSet<byte> blankedList = new();

    public static bool CheckMurderPlayer(MurderInfo info)
    {
        return AllActiveRoles.Values.All(r => r.CheckMurderPlayer(info))
               && (info.Target.GetModifierBase()?.All(x => x.BeforeMurderPlayer(info)) ?? true)
               && (info.Target.GetRoleBase()?.BeforeMurderPlayer(info) ?? true)
               ;
    }

    public static void MurderPlayer(PlayerControl killer,
                                    PlayerControl target,
                                    bool showAnimation = true,
                                    bool force = false,
                                    CustomDeathReason deathReason = CustomDeathReason.Null)
    {
        if (deathReason == CustomDeathReason.Null) deathReason = killer == target ? CustomDeathReason.Suicide : CustomDeathReason.Kill;

        var info = new MurderInfo(killer, target, showAnimation, force, deathReason);

        if (!info.ForceKill)
        {
            if (!CheckMurderPlayer(info)) return;
        }

        info.Killer.MurderPlayer(target, MurderResultFlags.Succeeded);
        KillAnimationCoPerformKillPatch.hideNextAnimation = showAnimation;
        PlayerData.SetDeathReason(target, deathReason, killer);
    }

}

public class MurderInfo
{
    public PlayerControl Killer { get; set; }
    public PlayerControl Target { get; set; }
    public PlayerControl AnimationKiller { get; set; }
    public PlayerControl AnimationTarget { get; set; }
    public bool ShowAnimation { get; set; } = true;
    public bool InMeeting { get; set; }
    public bool ForceKill { get; set; }
    public CustomDeathReason DeathReason { get; set; }
    public bool IsSuicide => Killer.PlayerId == Target.PlayerId;

    public MurderInfo(PlayerControl Killer, PlayerControl Target, bool ShowAnimation = true, bool InMeeting = false, PlayerControl? AnimationKiller = null, PlayerControl? AnimationTarget = null, bool Force = false, CustomDeathReason DeathReason = CustomDeathReason.Kill)
    {
        this.Killer = Killer;
        this.Target = Target;
        this.ForceKill = Force;
        this.AnimationKiller = AnimationKiller ?? Killer;
        this.AnimationTarget = AnimationTarget ?? Target;
        this.ShowAnimation = ShowAnimation;
        this.InMeeting = InMeeting;
        this.DeathReason = DeathReason;
    }

    public MurderInfo(PlayerControl Killer, PlayerControl Target, bool ShowAnimation = true, bool Force = false, CustomDeathReason DeathReason = CustomDeathReason.Kill)
    {
        this.Killer = Killer;
        this.Target = Target;
        this.ForceKill = Force;
        this.AnimationKiller = Killer;
        this.AnimationTarget = Target;
        this.ShowAnimation = ShowAnimation;
        this.DeathReason = DeathReason;
    }
    public MurderInfo(PlayerControl Killer, PlayerControl Target)
    {
        this.Killer = Killer;
        this.Target = Target;
        this.AnimationKiller = Killer;
        this.AnimationTarget = Target;
        this.DeathReason = CustomDeathReason.Kill;
    }

    public enum MurderAttemptResult
    {
        PerformKill,
        SuppressKill,
        BlankKill,
    }

}