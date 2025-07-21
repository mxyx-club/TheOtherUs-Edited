using System.Linq.Expressions;

namespace TheOtherRoles.Helper;

public class Il2CppArgument<T>
{
    public T Value { get; private set; }
    public Il2CppArgument(T value)
    {
        Value = value;
    }

    public static implicit operator Il2CppArgument<T>(T value)
    {
        return new Il2CppArgument<T>(value);
    }
}

public static class Il2CppHelpers
{
    private static class CastHelper<T> where T : Il2CppObjectBase
    {
        public static Func<IntPtr, T> Cast;
        static CastHelper()
        {
            var constructor = typeof(T).GetConstructor(new[] { typeof(IntPtr) });
            var ptr = Expression.Parameter(typeof(IntPtr));
            var create = Expression.New(constructor!, ptr);
            var lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
            Cast = lambda.Compile();
        }
    }

    public static T CastFast<T>(this Il2CppObjectBase obj) where T : Il2CppObjectBase
    {
        if (obj is T casted) return casted;
        return obj.Pointer.CastFast<T>();
    }

    public static T CastFast<T>(this IntPtr ptr) where T : Il2CppObjectBase
    {
        return CastHelper<T>.Cast(ptr);
    }

    public static ISystem.List<T> ToIl2CppList<T>(this IReadOnlyList<T> list)
    {
        Il2CppSystem.Collections.Generic.List<T> result = new(list.Count);
        foreach (var elem in list) result.Add(elem);
        return result;
    }
}