namespace TheOtherRoles.Roles;

internal interface INeutral
{
    RoleType RoleType => RoleType.Neutral;
    NeutralType NeutralType { get; }
    bool NotWinner => true;
}

public enum NeutralType
{
    Benign,
    Evil,
    Kill,
}