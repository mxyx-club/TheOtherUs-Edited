using System;

namespace TheOtherRoles.Roles;


public abstract class RoleBase : IDisposable
{
    public virtual bool CanAssign()
    {
        return true;
    }

    public virtual void Dispose()
    {
    }

    public virtual void ClearAndReload()
    {
    }

    public virtual void ButtonCreate(HudManager _hudManager)
    {
    }

    public virtual void ResetCustomButton()
    {
    }

    public abstract RoleInfo RoleInfo { get; protected set; }
    public abstract Type RoleType { get; protected set; }
    //public List<RoleControllerBase> Controllers { get; protected set; } = [];

#nullable enable
    public Type? PathType { get; protected set; } = null;
}

public interface Invisable
{
    public bool isInvisable { get; set; }
}