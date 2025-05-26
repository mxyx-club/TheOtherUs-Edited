namespace TheOtherRoles.Helper;

public static class ListHelper
{
    public static T Get<T>(this ISystem.List<T> list, int index)
    {
        return list._items[index];
    }

    public static T Get<T>(this ISystem.List<T> list, Index index)
    {
        return list._items[index];
    }
}