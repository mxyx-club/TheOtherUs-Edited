namespace TheOtherRoles.Roles;

public abstract class GhostBase : IRoleInfo
{
    public PlayerControl Player { get; set; }
    public string RoleName { get; }
    public RoleInfo RoleInfo { get; }
    public RoleId RoleId { get; }
    public virtual RoleType[] RemoveTeam { get; set; }
    public virtual RoleId[] RemoveRole { get; set; }
    public bool IsAlive => Player.IsAlive();
    public byte PlayerId => Player.PlayerId;

    public GhostBase(PlayerControl player, RoleInfo Info)
    {
        Player = player;
        RoleInfo = Info;
        RoleName = Info.NameKey;
        RoleId = Info.RoleId;

        CustomRoleManager.AllRoles.Add(this);

        PlayerData.AllPlayerData.TryGetValue(player.PlayerId, out PlayerData playerData);
        if (playerData != null)
        {
            playerData.RoleHistory.Add(Info.RoleId);
        }
    }

    /// <summary>
    /// 销毁玩家实例
    /// </summary>
    public void Destroy()
    {
        CustomRoleManager.AllRoles.Remove(this);
        Player = null;
    }

    public void Initialize() { }
}
