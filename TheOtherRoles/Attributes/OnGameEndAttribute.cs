namespace TheOtherRoles.Attributes;

internal class OnGameEndAttribute : InitializerAttribute<OnGameEndAttribute>
{
    public OnGameEndAttribute() : base() { }
    public OnGameEndAttribute(Priority priority) : base(priority) { }
}