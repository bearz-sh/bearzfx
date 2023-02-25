namespace Bearz.Extra.Collections;

public static class ListExtensions
{
    public static T Pop<T>(this List<T> list)
    {
        var item = list[^1];
        list.RemoveAt(list.Count - 1);
        return item;
    }

    public static T Unshift<T>(this List<T> list, T item)
    {
        list.Insert(0, item);
        return item;
    }

    public static T Shift<T>(this List<T> list)
    {
        var item = list[0];
        list.RemoveAt(0);
        return item;
    }
}