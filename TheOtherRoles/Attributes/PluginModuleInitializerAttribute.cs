namespace TheOtherRoles.Attributes;

internal class PluginModuleInitializerAttribute : InitializerAttribute<PluginModuleInitializerAttribute>
{
    public PluginModuleInitializerAttribute() : base() { }
    public PluginModuleInitializerAttribute(int priority) : base(priority) { }
}