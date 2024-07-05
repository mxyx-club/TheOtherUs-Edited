using System;

namespace TheOtherRoles.Roles;


public abstract class RoleBase
{
    public virtual bool CanAssign()
    {
        return true;
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
}
