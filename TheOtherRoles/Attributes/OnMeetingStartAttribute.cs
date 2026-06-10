namespace TheOtherRoles.Attributes;

internal class OnMeetingStartAttribute : InitializerAttribute<OnMeetingStartAttribute>
{
    public OnMeetingStartAttribute() : base() { }
    public OnMeetingStartAttribute(int priority) : base(priority) { }
}
