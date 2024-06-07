#nullable enable
namespace TheOtherRoles.Modules;

public class ManagerBase<T> where T : new()
{
    private static T? _instance;

    public static T Instance => _instance ??= new T();

}