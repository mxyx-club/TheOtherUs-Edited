#nullable enable
namespace TheOtherRoles.Modules;

public interface IManagerBase
{
    virtual void Load()
    {
    }
};

public static class MangerBases
{
    private static readonly List<IManagerBase> _managers = new();
    public static IReadOnlyList<IManagerBase> AllManager => _managers;
    public static void Add(IManagerBase @base) => _managers.Add(@base);
}
public abstract class ManagerBase<T> : IDisposable, IManagerBase where T : ManagerBase<T>, new()
{
    private static T? _instance;

    public static T Instance => _instance ??= new T();

    public ManagerBase()
    {
        MangerBases.Add(this);
        _instance = (T)this;
    }

    public virtual void Dispose()
    {
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ManagerBaseLoad : RegisterAttribute
{
    [Register]
    internal void Register(List<Type> findTypes, TaskQueue queue)
    {
        foreach (var manager in MangerBases.AllManager)
        {
            if (manager.GetType().IsDefined(typeof(ManagerBaseLoad), true))
                queue.StartTask(manager.Load, $"ManagerBaseLoad name:{manager.GetType().Name}");
        }
    }
}

public class ListManager<T, TList> : ManagerBase<T>, IDisposable where T : ListManager<T, TList>, new()
{
    protected List<TList> List = new();
    public IReadOnlyList<TList> ReadOnlyList => List;
}

public class RegisterAttribute : Attribute;