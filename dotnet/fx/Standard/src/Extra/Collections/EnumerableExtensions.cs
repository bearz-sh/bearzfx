namespace Bearz.Extra.Collections;

public static class EnumerableExtensions
{
#if NETLEGACY

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> self)
    {
        return new HashSet<T>(self);
    }

#endif

    public static Span<T> ToSpan<T>(this IEnumerable<T> self, Span<T> destination, int index, int length)
    {
        int i = 0;
        int s1 = length - 1;
        int s2 = destination.Length - 1;
        foreach (var t in self.Skip(index).Take(length))
        {
            if (i == s1 || i == s2)
                break;

            if (i < index)
                continue;

            destination[i] = t;

            i++;
        }

        return destination;
    }

    public static Span<T> ToSpan<T>(IEnumerable<T> self)
    {
        if (self is T[] array)
            return array.AsSpan();

        if (self is IList<T> list)
        {
            var copy = new T[list.Count];
            list.CopyTo(copy, 0);
            return copy.AsSpan();
        }

        if (self is IReadOnlyCollection<T> readOnlyCollection)
        {
            var copy = new Span<T>(new T[readOnlyCollection.Count]);
            int i = 0;
            foreach (var t in self)
            {
                copy[i] = t;
                i++;
            }

            return copy;
        }

        if (self is ICollection<T> collection)
        {
            var copy = new Span<T>(new T[collection.Count]);
            int i = 0;
            foreach (var t in self)
            {
                copy[i] = t;
                i++;
            }

            return copy;
        }

        return self.ToArray().AsSpan();
    }

    public static bool EqualTo<T>(this IEnumerable<T>? left, IEnumerable<T>? right, IComparer<T> comparer)
    {
        if (left == null && right == null)
            return true;

        if (left == null || right == null)
            return false;

        using var leftEnumerator = left.GetEnumerator();
        using var rightEnumerator = right.GetEnumerator();

        while (true)
        {
            var lNext = leftEnumerator.MoveNext();
            var rNext = rightEnumerator.MoveNext();

            if (!lNext && !rNext)
                return true;

            if (!lNext || !rNext)
                return false;

            var lValue = leftEnumerator.Current;
            var rValue = rightEnumerator.Current;

            if (comparer.Compare(lValue, rValue) != 0)
                return false;
        }
    }

    public static bool EqualTo<T>(this IEnumerable<T>? left, IEnumerable<T>? right, Comparison<T> compare)
    {
        if (left == null && right == null)
            return true;

        if (left == null || right == null)
            return false;

        using var leftEnumerator = left.GetEnumerator();
        using var rightEnumerator = right.GetEnumerator();

        while (true)
        {
            var lNext = leftEnumerator.MoveNext();
            var rNext = rightEnumerator.MoveNext();

            if (!lNext && !rNext)
                return true;

            if (!lNext || !rNext)
                return false;

            var lValue = leftEnumerator.Current;
            var rValue = rightEnumerator.Current;

            if (compare(lValue, rValue) != 0)
                return false;
        }
    }

    public static bool EqualTo<T>(this IEnumerable<T>? left, IEnumerable<T>? right)
    {
        if (left == null && right == null)
            return true;

        if (left == null || right == null)
            return false;

        using var leftEnumerator = left.GetEnumerator();
        using var rightEnumerator = right.GetEnumerator();

        while (true)
        {
            var lNext = leftEnumerator.MoveNext();
            var rNext = rightEnumerator.MoveNext();

            if (!lNext && !rNext)
                return true;

            if (!lNext || !rNext)
                return false;

            var lValue = leftEnumerator.Current;
            var rValue = rightEnumerator.Current;

            if (lValue is IEquatable<T> lEquatable && !lEquatable.Equals(rValue))
                return false;

            if (lValue is IComparable<T> lComparable && lComparable.CompareTo(rValue) != 0)
                return false;

            if (lValue is null && rValue is null)
                continue;

            if (lValue is null || rValue is null)
                return false;

            if (!lValue.Equals(rValue))
                return false;
        }
    }
}