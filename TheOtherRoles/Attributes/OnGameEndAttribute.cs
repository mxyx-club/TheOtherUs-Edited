namespace TheOtherRoles.Attributes;

internal class OnGameEndAttribute : InitializerAttribute<OnGameEndAttribute>
{
    public OnGameEndAttribute() : base() { }
    public OnGameEndAttribute(int priority) : base(priority) { }
}