using System;

namespace TheOtherRoles.Roles;

public abstract class RoleControllerBase : IDisposable
{
    public abstract RoleBase _RoleBase { get; set; }

    public PlayerControl Player { get; protected set; }

    protected RoleControllerBase(PlayerControl player)
    {
        Player = player;
        _RoleManager._AllControllerBases.Add(this);
    }

    public virtual bool SetShowRoleTeam(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeamPlayers)
    {
        return false;
    }

    public virtual bool SetShowRoleInfo(IntroCutscene __instance)
    {
        return false;
    }

    public virtual void Update(HudManager __instance)
    {

    }

    public virtual void Dispose()
    {
        _RoleBase = null;
        Player = null;
        _RoleManager._AllControllerBases.Remove(this);
    }
}