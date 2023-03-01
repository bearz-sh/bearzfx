using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Bearz.Extra.Memory;

namespace Bearz.Extra.Arrays;

public static class ArrayExtensions
{
    /// <summary>
    /// Clears the values of the array.
    /// </summary>
    /// <param name="array">The array to perform the clear operation against.</param>
    /// <param name="index">The start index. Defaults to 0.</param>
    /// <param name="length">The number of items to clear.</param>
    /// <typeparam name="T">The item type in the array.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this T[] array, int index = 0, int length = -1)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (length < 0)
            length = array.Length;

        System.Array.Clear(array, index, length);
    }

    public static bool EqualTo<T>(this T[]? array, T[]? other)
        => ArrayOps.Equal(array, other);

    public static bool EqualTo<T>(this T[]? array, T[]? other, IEqualityComparer<T> comparer)
        => ArrayOps.Equal(array, other, comparer);

    public static bool EqualTo<T>(this T[]? array, T[]? other, Comparison<T> comparer)
        => ArrayOps.Equal(array, other, comparer);

    public static bool EqualTo<T>(this T[]? array, T[]? other, IComparer<T> comparer)
        => ArrayOps.Equal(array, other, comparer);

    public static Span<T> Slice<T>(this T[] array, int start)
        => Slice(array, start, array.Length - start);

    public static Span<T> Slice<T>(this T[] array, int start, int length)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start));

        if ((start + length) > array.Length)
            throw new ArgumentOutOfRangeException(nameof(length));

        return array.AsSpan().Slice(start, length);
    }
}