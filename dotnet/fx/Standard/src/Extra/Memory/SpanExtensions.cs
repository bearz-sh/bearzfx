using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Bearz.Extra.Memory;

public static class SpanExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(ReadOnlySpan<char> self)
    {
#if NETLEGACY
        return new string(self.ToArray());
#else
        return self.ToString();
#endif
    }

    public static Span<T> Pop<T>(this Span<T> self, out T value)
    {
        if (self.IsEmpty)
            throw new ArgumentException("Cannot pop the last item from an empty span.");

        value = self[^1];
        return self[..^1];
    }

    public static ReadOnlySpan<T> Pop<T>(this ReadOnlySpan<T> self, out T value)
    {
        if (self.IsEmpty)
            throw new ArgumentException("Cannot pop the last item from an empty span.");

        value = self[^1];
        return self[..^1];
    }

    public static Span<T> Prepend<T>(this Span<T> self, T value)
    {
        var copy = new Span<T>(new T[self.Length + 1]);
        Prepend(self, copy, value);
        return copy;
    }

    public static ReadOnlySpan<T> Prepend<T>(this ReadOnlySpan<T> self, T value)
    {
        var copy = new Span<T>(new T[self.Length + 1]);
        Prepend(self, copy, value);
        return copy;
    }

    public static void Prepend<T>(this Span<T> self, Span<T> destination, T value)
    {
        if (self.Length + 1 != destination.Length)
            throw new ArgumentOutOfRangeException(nameof(destination));

        destination[0] = value;
        self.CopyTo(destination[1..]);
    }

    public static void Prepend<T>(this ReadOnlySpan<T> self, Span<T> destination, T value)
    {
        if (self.Length + 1 != destination.Length)
            throw new ArgumentOutOfRangeException(nameof(destination));

        destination[0] = value;
        self.CopyTo(destination[1..]);
    }

    public static Span<T> PrependRange<T>(this Span<T> self, ReadOnlySpan<T> items)
    {
        var copy = new Span<T>(new T[self.Length + items.Length]);
        PrependRange(self, copy, items);
        return copy;
    }

    public static ReadOnlySpan<T> PrependRange<T>(this ReadOnlySpan<T> self, ReadOnlySpan<T> items)
    {
        var copy = new Span<T>(new T[self.Length + items.Length]);
        PrependRange(self, copy, items);
        return copy;
    }

    public static void PrependRange<T>(this Span<T> self, Span<T> destination, ReadOnlySpan<T> items)
    {
        if (self.Length + items.Length != destination.Length)
            throw new ArgumentOutOfRangeException(nameof(destination));

        items.CopyTo(destination);
        self.CopyTo(destination[items.Length..]);
    }

    public static void PrependRange<T>(this ReadOnlySpan<T> self, Span<T> destination, ReadOnlySpan<T> items)
    {
        if (self.Length + items.Length != destination.Length)
            throw new ArgumentOutOfRangeException(nameof(destination));

        items.CopyTo(destination);
        self.CopyTo(destination[items.Length..]);
    }

    public static Span<T> AppendRange<T>(this Span<T> self, ReadOnlySpan<T> items)
    {
        var copy = new Span<T>(new T[self.Length + items.Length]);
        AppendRange(self, copy, items);
        return copy;
    }

    public static ReadOnlySpan<T> AppendRange<T>(this ReadOnlySpan<T> self, ReadOnlySpan<T> items)
    {
        var copy = new Span<T>(new T[self.Length + items.Length]);
        AppendRange(self, copy, items);
        return copy;
    }

    public static void AppendRange<T>(this Span<T> self, Span<T> destination, ReadOnlySpan<T> items)
    {
        if (self.Length + items.Length != destination.Length)
            throw new ArgumentOutOfRangeException(nameof(destination));

        self.CopyTo(destination);
        items.CopyTo(destination[self.Length..]);
    }

    public static void AppendRange<T>(this ReadOnlySpan<T> self, Span<T> destination, ReadOnlySpan<T> items)
    {
        if (self.Length + items.Length != destination.Length)
            throw new ArgumentOutOfRangeException(nameof(destination));

        self.CopyTo(destination);
        items.CopyTo(destination[self.Length..]);
    }

    public static Span<T> Append<T>(this Span<T> self, int index, T value)
    {
        var copy = new Span<T>(new T[self.Length + 1]);
        Append(self, copy, value);
        return copy;
    }

    public static ReadOnlySpan<T> Append<T>(this ReadOnlySpan<T> self, T value)
    {
        var copy = new Span<T>(new T[self.Length + 1]);
        Append(self, copy, value);
        return copy;
    }

    public static void Append<T>(this Span<T> self, Span<T> destination, T value)
    {
        if (self.Length != destination.Length - 1)
            throw new ArgumentOutOfRangeException(nameof(destination));

        self.CopyTo(destination);
        destination[^1] = value;
    }

    public static void Append<T>(this ReadOnlySpan<T> self, Span<T> destination, T value)
    {
        if (self.Length != destination.Length - 1)
            throw new ArgumentOutOfRangeException(nameof(destination));

        self.CopyTo(destination);
        destination[^1] = value;
    }

    public static Span<T> InsertRange<T>(this Span<T> self, int index, ReadOnlySpan<T> items)
    {
        var copy = new Span<T>(new T[self.Length + items.Length]);
        InsertRange(self, copy, index, items);
        return copy;
    }

    public static ReadOnlySpan<T> InsertRange<T>(this ReadOnlySpan<T> self, int index, ReadOnlySpan<T> items)
    {
        var copy = new Span<T>(new T[self.Length + items.Length]);
        InsertRange(self, copy, index, items);
        return copy;
    }

    public static void InsertRange<T>(this Span<T> self, Span<T> destination, int index, ReadOnlySpan<T> items)
    {
        if (index < 0 || index >= self.Length || index >= destination.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index == 0)
        {
            items.CopyTo(destination);
            self.CopyTo(destination[items.Length..]);
            return;
        }

        if (index == self.Length)
        {
            self.CopyTo(destination);
            items.CopyTo(destination[self.Length..]);
            return;
        }

        self[..index].CopyTo(destination);
        items.CopyTo(destination[index..]);
        self[index..].CopyTo(destination[(index + items.Length)..]);
    }

    public static void InsertRange<T>(this ReadOnlySpan<T> self, Span<T> destination, int index, ReadOnlySpan<T> items)
    {
        if (index < 0 || index >= self.Length || index >= destination.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index == 0)
        {
            items.CopyTo(destination);
            self.CopyTo(destination[items.Length..]);
            return;
        }

        if (index == self.Length)
        {
            self.CopyTo(destination);
            items.CopyTo(destination[self.Length..]);
            return;
        }

        self[..index].CopyTo(destination);
        items.CopyTo(destination[index..]);
        self[index..].CopyTo(destination[(index + items.Length)..]);
    }

    public static ReadOnlySpan<T> Insert<T>(this ReadOnlySpan<T> self, int index, T value)
        where T : IEquatable<T>
    {
        var copy = new Span<T>(new T[self.Length]);
        Insert(self, copy, index, value);
        return copy;
    }

    public static void Insert<T>(this Span<T> self, Span<T> destination, int index, T value)
        where T : IEquatable<T>
    {
        if (index < 0 || index >= self.Length || index >= destination.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index == 0)
        {
            destination[0] = value;
            self.CopyTo(destination[1..]);
            return;
        }

        if (index == self.Length)
        {
            self.CopyTo(destination);
            destination[^1] = value;
            return;
        }

        self[..index].CopyTo(destination);
        destination[index] = value;
        self[index..].CopyTo(destination[(index + 1)..]);
    }

    public static void Insert<T>(this ReadOnlySpan<T> self, Span<T> destination, int index, T value)
        where T : IEquatable<T>
    {
        if (index < 0 || index >= self.Length || index >= destination.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index == 0)
        {
            destination[0] = value;
            self.CopyTo(destination[1..]);
            return;
        }

        if (index == self.Length)
        {
            self.CopyTo(destination);
            destination[^1] = value;
            return;
        }

        self[..index].CopyTo(destination);
        destination[index] = value;
        self[index..].CopyTo(destination[(index + 1)..]);
    }

    public static ReadOnlySpan<T> Splice<T>(
        this ReadOnlySpan<T> self,
        int index,
        int deleteCount,
        ReadOnlySpan<T> items)
    {
        if (deleteCount == 0)
        {
            return InsertRange(self, index, items);
        }

        if (index < 0 || index >= self.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (deleteCount < 0 || deleteCount > self.Length - index)
            throw new ArgumentOutOfRangeException(nameof(deleteCount));

        var copy = new Span<T>(new T[self.Length - deleteCount + items.Length]);

        if (index == 0)
        {
            items.CopyTo(copy);
            self[deleteCount..].CopyTo(copy[items.Length..]);
            return copy;
        }

        if (index == self.Length)
        {
            self[..deleteCount].CopyTo(copy);
            items.CopyTo(copy[deleteCount..]);

            return copy;
        }

        self[..index].CopyTo(copy);
        items.CopyTo(copy[index..]);
        self[(index + deleteCount)..].CopyTo(copy[(index + items.Length)..]);

        return copy;
    }

    public static Span<T> Splice<T>(
        this Span<T> self,
        int index,
        int deleteCount,
        ReadOnlySpan<T> items)
    {
        if (deleteCount == 0)
        {
            return InsertRange(self, index, items);
        }

        if (index < 0 || index >= self.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (deleteCount < 0 || deleteCount > self.Length - index)
            throw new ArgumentOutOfRangeException(nameof(deleteCount));

        var copy = new Span<T>(new T[self.Length - deleteCount + items.Length]);

        if (index == 0)
        {
            items.CopyTo(copy);
            self[deleteCount..].CopyTo(copy[items.Length..]);
            return copy;
        }

        if (index == self.Length)
        {
            self[..deleteCount].CopyTo(copy);
            items.CopyTo(copy[deleteCount..]);

            return copy;
        }

        self[..index].CopyTo(copy);
        items.CopyTo(copy[index..]);
        self[(index + deleteCount)..].CopyTo(copy[(index + items.Length)..]);

        return copy;
    }

    public static void Splice<T>(
        this Span<T> self,
        Span<T> destination,
        int index,
        int deleteCount,
        ReadOnlySpan<T> items)
    {
        if (deleteCount == 0)
        {
            InsertRange(self, destination, index, items);
            return;
        }

        if (index < 0 || index >= self.Length || index >= destination.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (deleteCount < 0 || deleteCount > self.Length - index || deleteCount > destination.Length - index)
            throw new ArgumentOutOfRangeException(nameof(deleteCount));

        if (index == 0)
        {
            items.CopyTo(destination);
            self[deleteCount..].CopyTo(destination[items.Length..]);
            return;
        }

        if (index == self.Length)
        {
            self[..index].CopyTo(destination);
            items.CopyTo(destination[index..]);
            return;
        }

        self[..index].CopyTo(destination);
        items.CopyTo(destination[index..]);
        self[(index + deleteCount)..].CopyTo(destination[(index + items.Length)..]);
    }

    public static void Splice<T>(
        this ReadOnlySpan<T> self,
        Span<T> destination,
        int index,
        int deleteCount,
        ReadOnlySpan<T> items)
    {
        if (deleteCount == 0)
        {
            InsertRange(self, destination, index, items);
            return;
        }

        if (index < 0 || index >= self.Length || index >= destination.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (deleteCount < 0 || deleteCount > self.Length - index || deleteCount > destination.Length - index)
            throw new ArgumentOutOfRangeException(nameof(deleteCount));

        if (index == 0)
        {
            items.CopyTo(destination);
            self[deleteCount..].CopyTo(destination[items.Length..]);
            return;
        }

        if (index == self.Length)
        {
            self[..index].CopyTo(destination);
            items.CopyTo(destination[index..]);
            return;
        }

        self[..index].CopyTo(destination);
        items.CopyTo(destination[index..]);
        self[(index + deleteCount)..].CopyTo(destination[(index + items.Length)..]);
    }

    public static ReadOnlySpan<T> RemoveAt<T>(this ReadOnlySpan<T> self, int index)
    {
        if (index < 0 || index >= self.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        return self[..index][(index + 1)..];
    }

    public static ReadOnlySpan<T> RemoveAt<T>(this ReadOnlySpan<T> self, int index, int deleteCount)
        where T : IEquatable<T>
    {
        if (deleteCount == 0)
            return self;

        if (index < 0 || index >= self.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (deleteCount < 0 || deleteCount > self.Length - index)
            throw new ArgumentOutOfRangeException(nameof(deleteCount));

        return self[..index][(index + deleteCount)..];
    }

    public static void RemoveAt<T>(this Span<T> self, Span<T> destination, int index, int deleteCount)
    {
        if (deleteCount == 0)
            return;

        if (index < 0 || index >= self.Length || index >= destination.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (deleteCount < 0 || deleteCount > self.Length - index || deleteCount > destination.Length - index)
            throw new ArgumentOutOfRangeException(nameof(deleteCount));

        if (index == 0)
        {
            self.Slice(deleteCount).CopyTo(destination);
            return;
        }

        if (index == self.Length - deleteCount)
        {
            self[..index].CopyTo(destination);
            return;
        }

        self[..index].CopyTo(destination);
        self[(index + deleteCount)..].CopyTo(destination[index..]);
    }

    public static void RemoveAt<T>(this ReadOnlySpan<T> self, Span<T> destination, int index, int deleteCount)
    {
        if (deleteCount == 0)
            return;

        if (index < 0 || index >= self.Length || index >= destination.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (deleteCount < 0 || deleteCount > self.Length - index || deleteCount > destination.Length - index)
            throw new ArgumentOutOfRangeException(nameof(deleteCount));

        if (index == 0)
        {
            self.Slice(deleteCount).CopyTo(destination);
            return;
        }

        if (index == self.Length - deleteCount)
        {
            self[..index].CopyTo(destination);
            return;
        }

        self[..index].CopyTo(destination);
        self[(index + deleteCount)..].CopyTo(destination[index..]);
    }

    public static Span<T> Concat<T>(this Span<T> self, ReadOnlySpan<T> other)
    {
        var result = new T[self.Length + other.Length];
        self.CopyTo(result);
        other.CopyTo(result.AsSpan(self.Length));
        return result;
    }

    public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> self, ReadOnlySpan<T> other)
    {
        var result = new T[self.Length + other.Length];
        self.CopyTo(result);
        other.CopyTo(result.AsSpan(self.Length));
        return result;
    }

    public static Span<T> Concat<T>(this Span<T> self, T other)
    {
        var result = new T[self.Length + 1];
        self.CopyTo(result);
        result[self.Length] = other;
        return result;
    }

    public static Span<T> Concat<T>(this Span<T> self, T[] other)
    {
        var result = new T[self.Length + other.Length];
        self.CopyTo(result);
        other.CopyTo(result.AsSpan(self.Length));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(this ReadOnlySpan<T> self, T value, ReadOnlySpan<T> other)
    {
        var result = new T[self.Length + other.Length + 1];
        self.CopyTo(result);
        result[self.Length] = value;
        other.CopyTo(result.AsSpan(self.Length + 1));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(this ReadOnlySpan<T> self, T delimiter, ReadOnlySpan<T> other1, ReadOnlySpan<T> other2)
    {
        var result = new T[self.Length + other1.Length + other2.Length + 2];
        self.CopyTo(result);
        result[self.Length] = delimiter;
        other1.CopyTo(result.AsSpan(self.Length + 1));
        result[self.Length + other1.Length + 1] = delimiter;
        other2.CopyTo(result.AsSpan(self.Length + other1.Length + 2));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(
        this ReadOnlySpan<T> self,
        T delimiter,
        ReadOnlySpan<T> other1,
        ReadOnlySpan<T> other2,
        ReadOnlySpan<T> other3)
    {
        var result = new T[self.Length + other1.Length + other2.Length + other3.Length + 3];
        self.CopyTo(result);
        result[self.Length] = delimiter;
        other1.CopyTo(result.AsSpan(self.Length + 1));
        result[self.Length + other1.Length + 1] = delimiter;
        other2.CopyTo(result.AsSpan(self.Length + other1.Length + 2));
        result[self.Length + other1.Length + other2.Length + 2] = delimiter;
        other3.CopyTo(result.AsSpan(self.Length + other1.Length + other2.Length + 3));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(
        this ReadOnlySpan<T> self,
        T delimiter,
        ReadOnlySpan<T> other1,
        ReadOnlySpan<T> other2,
        ReadOnlySpan<T> other3,
        ReadOnlySpan<T> other4)
    {
        var result = new T[self.Length + other1.Length + other2.Length + other3.Length + other4.Length + 4];
        self.CopyTo(result);
        result[self.Length] = delimiter;
        other1.CopyTo(result.AsSpan(self.Length + 1));
        result[self.Length + other1.Length + 1] = delimiter;
        other2.CopyTo(result.AsSpan(self.Length + other1.Length + 2));
        result[self.Length + other1.Length + other2.Length + 2] = delimiter;
        other3.CopyTo(result.AsSpan(self.Length + other1.Length + other2.Length + 3));
        result[self.Length + other1.Length + other2.Length + other3.Length + 3] = delimiter;
        other4.CopyTo(result.AsSpan(self.Length + other1.Length + other2.Length + other3.Length + 4));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(
        this ReadOnlySpan<T> self,
        T delimiter,
        ReadOnlySpan<T> other1,
        ReadOnlySpan<T> other2,
        ReadOnlySpan<T> other3,
        ReadOnlySpan<T> other4,
        ReadOnlySpan<T> other5)
    {
        var result = new T[self.Length + other1.Length + other2.Length + other3.Length + other4.Length + other5.Length + 5];
        self.CopyTo(result);
        result[self.Length] = delimiter;
        other1.CopyTo(result.AsSpan(self.Length + 1));
        result[self.Length + other1.Length + 1] = delimiter;
        other2.CopyTo(result.AsSpan(self.Length + other1.Length + 2));
        result[self.Length + other1.Length + other2.Length + 2] = delimiter;
        other3.CopyTo(result.AsSpan(self.Length + other1.Length + other2.Length + 3));
        result[self.Length + other1.Length + other2.Length + other3.Length + 3] = delimiter;
        other4.CopyTo(result.AsSpan(self.Length + other1.Length + other2.Length + other3.Length + 4));
        result[self.Length + other1.Length + other2.Length + other3.Length + other4.Length + 4] = delimiter;
        other5.CopyTo(result.AsSpan(self.Length + other1.Length + other2.Length + other3.Length + other4.Length + 5));
        return result;
    }
}