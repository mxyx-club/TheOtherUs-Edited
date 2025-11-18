namespace TheOtherRoles.Roles;

public interface IRoleInfo
{
    string RoleName { get; }
    PlayerControl Player { get; set; }
    RoleInfo RoleInfo { get; }
    RoleId RoleId { get; }
    Color Color => RoleInfo.Color;
    bool IsAlive { get; }
    byte PlayerId { get; }

    void Initialize();
    void Destroy();
}
