namespace TheOtherRoles.Attributes;

internal class PluginModuleInitializerAttribute : InitializerAttribute<PluginModuleInitializerAttribute>
{
    public PluginModuleInitializerAttribute() : base() { }
    public PluginModuleInitializerAttribute(Priority priority) : base(priority) { }
}