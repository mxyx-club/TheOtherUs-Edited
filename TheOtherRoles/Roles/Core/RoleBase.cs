namespace TheOtherRoles.Roles;

public abstract class RoleBase : IRoleInfo
{
    public PlayerControl Player { get; set; }
    public string RoleName { get; }
    public RoleId RoleId { get; }
    public virtual bool? CanUseVent { get; set; }
    public virtual bool? HasImpVision { get; set; }
    public virtual bool? IsKiller { get; set; }
    public RoleInfo RoleInfo { get; }
    public RoleType RoleType { get; }
    public bool? HasTasks { get; }
    public bool IsAlive => Player.IsAlive();
    public byte PlayerId => Player.PlayerId;

    private int InstanceId { get; }
    public static int MaxInstanceId = int.MinValue;

    public RoleBase(PlayerControl player, RoleInfo roleInfo, bool? hasTasks = null)
    {
        Initialize();
        InstanceId = MaxInstanceId;
        MaxInstanceId++;
        Player = player;
        RoleType = roleInfo.RoleType;
        RoleInfo = roleInfo;
        RoleName = roleInfo.Name;
        RoleId = roleInfo.RoleId;
        HasTasks ??= hasTasks ?? (roleInfo.RoleType == RoleType.Crewmate);
        CanUseVent ??= roleInfo.RoleType == RoleType.Impostor;
        HasImpVision ??= roleInfo.RoleType == RoleType.Impostor;
        IsKiller ??= roleInfo.RoleType == RoleType.Impostor;
        CustomRoleManager.AllActiveRoles.TryAdd(player.PlayerId, this);
        CustomRoleManager.AllRoles.Add(this);

        PlayerData.AllPlayerData.TryGetValue(player.PlayerId, out PlayerData playerData);
        if (playerData != null)
        {
            playerData.Role = this;
            playerData.RoleType = roleInfo.RoleType;
            playerData.RoleHistory.Add(roleInfo.RoleId);
        }

        if (Player == PlayerControl.LocalPlayer) CreateButton(HudManager.Instance);
    }

    /// <summary>
    /// 创建玩家实例时会调用的函数
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    /// 销毁玩家实例
    /// </summary>
    public void Destroy()
    {
        CleanUp(HudManager.Instance);
        OnDestroy();
        CustomRoleManager.AllActiveRoles.Remove(Player.PlayerId);
        CustomRoleManager.AllRoles.Remove(this);
        Player = null;
    }

    public override bool Equals(object obj)
    {
        if (obj is not RoleBase) return false;
        return GetHashCode() == obj.GetHashCode();
    }

    public override int GetHashCode() => InstanceId;

    /// <summary>
    /// 销毁职业实例时调用
    /// </summary>
    public virtual void OnDestroy() { }

    public virtual bool SeeRoleColor(PlayerControl seen, PlayerControl seer, out Color color)
    {
        color = Color.white;
        return false;
    }

    public virtual bool SeeRoleTag(PlayerControl seen, PlayerControl seer, out string tag)
    {
        tag = string.Empty;
        return false;
    }

    public virtual bool SeeRoleName(PlayerControl seen, PlayerControl seer) => false;

    public virtual bool SetBasePlayerOutlines(PlayerControl seen, PlayerControl seer, out Color color)
    {
        color = Color.white;
        return false;
    }

    /// <summary>
    /// 游戏开始后会执行的函数<br/>
    /// </summary>
    public virtual void OnGameStart() { }

    /// <summary>
    /// 创建自定义按钮
    /// </summary>
    public virtual void CreateButton(HudManager __instance) { }

    /// <summary>
    /// 销毁按钮
    /// </summary>
    /// <param name="__instance"></param>
    public virtual void CleanUp(HudManager __instance) { }

    /// <summary>
    /// 帧 Task 处理函数，仅当前玩家调用<br/>
    /// 需要所有玩家调用时注册CustomRoleManager.OnFixedUpdateOthers
    /// </summary>
    /// <param name="player">目标玩家</param>
    public virtual void OnFixedUpdate(PlayerControl player) { }

    /// <summary>
    /// HudManager 帧处理函数函数<br/>
    /// 调用对象: 全体玩家
    /// </summary>
    public virtual void OnHudUpdate(HudManager hudManager) { }

    /// <summary>
    /// 报告前检查调用的函数<br/>
    /// 调用对象: 全体玩家
    /// </summary>
    /// <param name="reporter">报告者</param>
    /// <param name="target">被报告的玩家</param>
    /// <returns>false：取消报告</returns>
    public virtual bool OnCheckReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target) => true;

    /// <summary>
    /// 报告时调用的函数<br/>
    /// 调用对象: 全体玩家
    /// </summary>
    /// <param name="reporter">报告者</param>
    /// <param name="target">被报告的玩家</param>
    public virtual void OnReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target) { }

    /// <summary>
    /// 有玩家被复活时会调用的函数<br/>
    /// 调用对象: 全体玩家
    /// </summary>
    /// <param name="player"></param>
    public virtual void OnRevivePlayer(PlayerControl player) { }

    /// <summary>
    /// 会议开始时调用的函数<br/>
    /// 全体玩家都会调用该函数
    /// </summary>
    public virtual void OnMeetingStart(MeetingHud __instance) { }

    /// <summary>
    /// 在会议中每帧刷新时调用的函数<br/>
    /// 全体玩家都会调用该函数
    /// </summary>
    public virtual void OnMeetingUpdate(MeetingHud __instance) { }

    /// <summary>
    /// 更新会议中右下角文本
    /// </summary>
    public virtual string UpdateMeetingVoteText(MeetingHud __instance) => string.Empty;

    /// <summary>
    /// 当玩家因赌怪猜测死亡时
    /// </summary>
    public virtual void OnGuesserShoot(PlayerControl guesser, PlayerControl target) { }

    /// <summary>
    /// 清除投票时调用的函数<br/>
    /// </summary>
    public virtual void ClearVotes(MeetingHud __instance) { }

    /// <summary>
    /// 会议结束后驱逐玩家时调用的函数<br/>
    /// 在大神官会议时，这个方法会被执行两次，请注意！
    /// </summary>
    public virtual void OnExiledBegin(GameData.PlayerInfo exiled) { }

    /// <summary>
    /// 驱逐结束后调用的函数
    /// </summary>
    /// <param name="exiled">被驱逐的玩家</param>
    public virtual void OnExiledWrapUp(GameData.PlayerInfo exiled) { }

    /// <summary>
    /// 有玩家死亡时调用的函数
    /// </summary>
    /// <param name="player">死亡玩家</param>
    /// <param name="isOnMeeting">是否在会议中死亡</param>
    public virtual void OnPlayerDeath(PlayerControl player, bool isOnMeeting = false) { }

    /// <summary>
    /// 玩家因驱逐死亡时调用的函数<br/>
    /// </summary>
    public virtual void OnPlayerExlied(PlayerControl player) { }

    /// <summary>
    /// 有玩家掉线时触发的函数<br/>
    /// </summary>
    public virtual void OnPlayerDisconnect(PlayerControl player) { }

    /// <summary>
    /// 检查能否进行击杀行为<br/>
    /// 全体玩家都会检查本次击杀行为
    /// </summary>
    /// <returns>false：不再向下执行击杀事件</returns>
    public virtual bool CheckMurderPlayer(MurderInfo Info) => true;

    /// <summary>
    /// 当前玩家被击杀前调用的函数<br/>
    /// 仅击杀者会调用该函数
    /// </summary>
    /// <returns>false：不再向下执行击杀事件</returns>
    public virtual bool BeforeMurderPlayer(MurderInfo Info) => true;

    /// <summary>
    /// 玩家被击杀后所有玩家调用的函数<br/>
    /// 全体玩家都会调用该函数
    /// </summary>
    public virtual void OnMurderPlayer(MurderInfo Info) { }

    /// <summary>
    /// 检查游戏能否结束<br/>
    /// </summary>
    /// <returns>false: 结束游戏</returns>
    public virtual bool CheckGameEnd(LogicGameFlowNormal __instance) => true;
}