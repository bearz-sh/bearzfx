using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Bearz.Extra.Memory;

public static class SpanExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(ReadOnlySpan<char> span)
    {
#if NETLEGACY
        return new string(span.ToArray());
#else
        return span.ToString();
#endif
    }

    public static Span<T> Concat<T>(this Span<T> span, ReadOnlySpan<T> other)
    {
        var result = new T[span.Length + other.Length];
        span.CopyTo(result);
        other.CopyTo(result.AsSpan(span.Length));
        return result;
    }

    public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
    {
        var result = new T[span.Length + other.Length];
        span.CopyTo(result);
        other.CopyTo(result.AsSpan(span.Length));
        return result;
    }

    public static Span<T> Concat<T>(this Span<T> span, T other)
    {
        var result = new T[span.Length + 1];
        span.CopyTo(result);
        result[span.Length] = other;
        return result;
    }

    public static Span<T> Concat<T>(this Span<T> span, T[] other)
    {
        var result = new T[span.Length + other.Length];
        span.CopyTo(result);
        other.CopyTo(result.AsSpan(span.Length));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(this ReadOnlySpan<T> span, T value, ReadOnlySpan<T> other)
    {
        var result = new T[span.Length + other.Length + 1];
        span.CopyTo(result);
        result[span.Length] = value;
        other.CopyTo(result.AsSpan(span.Length + 1));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(this ReadOnlySpan<T> span, T delimiter, ReadOnlySpan<T> other1, ReadOnlySpan<T> other2)
    {
        var result = new T[span.Length + other1.Length + other2.Length + 2];
        span.CopyTo(result);
        result[span.Length] = delimiter;
        other1.CopyTo(result.AsSpan(span.Length + 1));
        result[span.Length + other1.Length + 1] = delimiter;
        other2.CopyTo(result.AsSpan(span.Length + other1.Length + 2));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(
        this ReadOnlySpan<T> span,
        T delimiter,
        ReadOnlySpan<T> other1,
        ReadOnlySpan<T> other2,
        ReadOnlySpan<T> other3)
    {
        var result = new T[span.Length + other1.Length + other2.Length + other3.Length + 3];
        span.CopyTo(result);
        result[span.Length] = delimiter;
        other1.CopyTo(result.AsSpan(span.Length + 1));
        result[span.Length + other1.Length + 1] = delimiter;
        other2.CopyTo(result.AsSpan(span.Length + other1.Length + 2));
        result[span.Length + other1.Length + other2.Length + 2] = delimiter;
        other3.CopyTo(result.AsSpan(span.Length + other1.Length + other2.Length + 3));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(
        this ReadOnlySpan<T> span,
        T delimiter,
        ReadOnlySpan<T> other1,
        ReadOnlySpan<T> other2,
        ReadOnlySpan<T> other3,
        ReadOnlySpan<T> other4)
    {
        var result = new T[span.Length + other1.Length + other2.Length + other3.Length + other4.Length + 4];
        span.CopyTo(result);
        result[span.Length] = delimiter;
        other1.CopyTo(result.AsSpan(span.Length + 1));
        result[span.Length + other1.Length + 1] = delimiter;
        other2.CopyTo(result.AsSpan(span.Length + other1.Length + 2));
        result[span.Length + other1.Length + other2.Length + 2] = delimiter;
        other3.CopyTo(result.AsSpan(span.Length + other1.Length + other2.Length + 3));
        result[span.Length + other1.Length + other2.Length + other3.Length + 3] = delimiter;
        other4.CopyTo(result.AsSpan(span.Length + other1.Length + other2.Length + other3.Length + 4));
        return result;
    }

    public static ReadOnlySpan<T> Join<T>(
        this ReadOnlySpan<T> span,
        T delimiter,
        ReadOnlySpan<T> other1,
        ReadOnlySpan<T> other2,
        ReadOnlySpan<T> other3,
        ReadOnlySpan<T> other4,
        ReadOnlySpan<T> other5)
    {
        var result = new T[span.Length + other1.Length + other2.Length + other3.Length + other4.Length + other5.Length + 5];
        span.CopyTo(result);
        result[span.Length] = delimiter;
        other1.CopyTo(result.AsSpan(span.Length + 1));
        result[span.Length + other1.Length + 1] = delimiter;
        other2.CopyTo(result.AsSpan(span.Length + other1.Length + 2));
        result[span.Length + other1.Length + other2.Length + 2] = delimiter;
        other3.CopyTo(result.AsSpan(span.Length + other1.Length + other2.Length + 3));
        result[span.Length + other1.Length + other2.Length + other3.Length + 3] = delimiter;
        other4.CopyTo(result.AsSpan(span.Length + other1.Length + other2.Length + other3.Length + 4));
        result[span.Length + other1.Length + other2.Length + other3.Length + other4.Length + 4] = delimiter;
        other5.CopyTo(result.AsSpan(span.Length + other1.Length + other2.Length + other3.Length + other4.Length + 5));
        return result;
    }
}