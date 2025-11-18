namespace TheOtherRoles.Attributes;

internal class OnGameStartAttribute : InitializerAttribute<OnGameStartAttribute>
{
    public OnGameStartAttribute() : base() { }
    public OnGameStartAttribute(Priority priority) : base(priority) { }
}
