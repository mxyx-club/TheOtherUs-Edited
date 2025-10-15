namespace TheOtherRoles.Attributes;

internal class OnMeetingStartAttribute : InitializerAttribute<OnMeetingStartAttribute>
{
    public OnMeetingStartAttribute() : base() { }
    public OnMeetingStartAttribute(Priority priority) : base(priority) { }
}
