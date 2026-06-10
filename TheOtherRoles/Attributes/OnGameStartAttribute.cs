namespace TheOtherRoles.Attributes;

internal class OnGameStartAttribute : InitializerAttribute<OnGameStartAttribute>
{
    public OnGameStartAttribute() : base() { }
    public OnGameStartAttribute(int priority) : base(priority) { }
}
