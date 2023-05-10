using System.Runtime.CompilerServices;

namespace Bearz.Extra.Arrays;

public static class ArrayExtensions
{
    /// <summary>
    /// Creates a new <see cref="ArraySegment{T}"/> from the given array.
    /// </summary>
    /// <param name="array">The one dimensional array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new <see cref="ArraySegment{T}"/>.</returns>
    public static ArraySegment<T> AsSegment<T>(this T[] array)
        => AsSegment(array, 0, array.Length);

    /// <summary>
    /// Creates a new <see cref="ArraySegment{T}"/> from the given array.
    /// </summary>
    /// <param name="array">The one dimensional array.</param>
    /// <param name="start">The zero-based position that will be the start of the segment.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new <see cref="ArraySegment{T}"/>.</returns>
    public static ArraySegment<T> AsSegment<T>(this T[] array, int start)
        => AsSegment(array, start, array.Length - start);

    /// <summary>
    /// Creates a new <see cref="ArraySegment{T}"/> from the given array.
    /// </summary>
    /// <param name="array">The one dimensional array.</param>
    /// <param name="start">The zero-based position that will be the start of the segment.</param>
    /// <param name="length">The number of elements from the array to include in the segment.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new <see cref="ArraySegment{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="array"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the <paramref name="start"/> or <paramref name="length"/> is less than zero.
    /// Thrown when the <paramref name="start"/> plus <paramref name="length"/> is greater than
    /// <paramref name="array"/>'s length.
    /// </exception>
    public static ArraySegment<T> AsSegment<T>(this T[] array, int start, int length)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start));

        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        if ((start + length) > array.Length)
            throw new ArgumentOutOfRangeException(nameof(length));

        return new ArraySegment<T>(array, start, length);
    }

    /// <summary>
    /// Concatenates the two arrays into a single new array.
    /// </summary>
    /// <param name="source">The source array.</param>
    /// <param name="array1">The first array to concatenate.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new array.</returns>
    public static T[] Concat<T>(this T[] source, T[] array1)
        => ArrayOps.Concat(source, array1);

    /// <summary>
    /// Concatenates the three arrays into a single new array.
    /// </summary>
    /// <param name="source">The source array.</param>
    /// <param name="array1">The first array to concatenate.</param>
    /// <param name="array2">The second array to concatenate.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new array.</returns>
    public static T[] Concat<T>(this T[] source, T[] array1, T[] array2)
#pragma warning disable S2234
        => ArrayOps.Concat(source, array1, array2);
#pragma warning restore S2234

    /// <summary>
    /// Concatenates the multiple arrays into a single new array.
    /// </summary>
    /// <param name="source">The source array.</param>
    /// <param name="arrays">One or more arrays to concatenate.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new array.</returns>
    public static T[] Concat<T>(this T[] source, params T[][] arrays)
    {
        var offset = 0;
        var result = new T[arrays.Sum(a => a.Length) + source.Length];
        Array.Copy(source, 0, result, 0, source.Length);
        offset += source.Length;

        foreach (var array in arrays)
        {
            Array.Copy(array, 0, result, offset, array.Length);
            offset += array.Length;
        }

        return result;
    }

    /// <summary>
    /// Clears the values of the array.
    /// </summary>
    /// <param name="array">The array to perform the clear operation against.</param>
    /// <param name="index">The start index. Defaults to 0.</param>
    /// <param name="length">The number of items to clear.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
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

    /// <summary>
    /// Compares two arrays using the default equality comparer for the type.
    /// </summary>
    /// <param name="array">The left side of the compare.</param>
    /// <param name="other">The right side of the compare.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when both objects are equal; otherwise, <c>false</c>.</returns>
    public static bool EqualTo<T>(this T[]? array, T[]? other)
        => ArrayOps.Equal(array, other);

    /// <summary>
    /// Compares two arrays for equality using the <paramref name="comparer"/>.
    /// </summary>
    /// <param name="array">The left side of the compare.</param>
    /// <param name="other">The right side of the compare.</param>
    /// <param name="comparer">The equality comparer implementation to use.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when both objects are equal; otherwise, <c>false</c>.</returns>
    public static bool EqualTo<T>(this T[]? array, T[]? other, IEqualityComparer<T> comparer)
        => ArrayOps.Equal(array, other, comparer);

    /// <summary>
    /// Compares two arrays for equality using the <paramref name="comparer"/>.
    /// </summary>
    /// <param name="array">The left side of the compare.</param>
    /// <param name="other">The right side of the compare.</param>
    /// <param name="comparer">The comparison implementation to use.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when both objects are equal; otherwise, <c>false</c>.</returns>
    public static bool EqualTo<T>(this T[]? array, T[]? other, Comparison<T> comparer)
        => ArrayOps.Equal(array, other, comparer);

    /// <summary>
    /// Compares two arrays for equality using the <paramref name="comparer"/>.
    /// </summary>
    /// <param name="array">The left side of the compare.</param>
    /// <param name="other">The right side of the compare.</param>
    /// <param name="comparer">The comparer implementation to use.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when both objects are equal; otherwise, <c>false</c>.</returns>
    public static bool EqualTo<T>(this T[]? array, T[]? other, IComparer<T> comparer)
        => ArrayOps.Equal(array, other, comparer);

    /// <summary>
    /// Creates a new copy of the array starting at the specified index.
    /// </summary>
    /// <param name="source">The one dimensional array source of elements.</param>
    /// <param name="index">The zero-based position of where to start the copy.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new array.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the index is less than 0 or greater than the length of the array.
    /// </exception>
    public static T[] Copy<T>(this T[] source, int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index >= source.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        var length = source.Length - index;
        var destination = new T[length];
        Array.Copy(source, index, destination, 0, length);
        return destination;
    }

    /// <summary>
    /// Creates a new copy of the array starting at the specified index with the specified length.
    /// </summary>
    /// <param name="source">The one dimensional array source of elements.</param>
    /// <param name="index">The zero-based position of where to start the copy.</param>
    /// <param name="length">The number of elements to copy.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new array.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the index is less than 0 or greater than the length of the array.
    /// Thrown if the length is greater than the length of the array minus the index.
    /// </exception>
    public static T[] Copy<T>(this T[] source, int index, int length)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index >= source.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (length > source.Length - index)
            throw new ArgumentOutOfRangeException(nameof(length));

        var destination = new T[length];
        Array.Copy(source, index, destination, 0, length);
        return destination;
    }

    /// <summary>
    /// Copies the elements of the array to the specified array starting at the specified index.
    /// </summary>
    /// <param name="source">The one dimensional array source of elements.</param>
    /// <param name="destination">The one dimensional array destination for elements.</param>
    /// <param name="index">The zero-based position of where to start the copy.</param>
    /// <param name="length">The number of elements to copy.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the source array is null.
    /// Thrown when the destination array is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown when the index is less than 0.
    ///  Thrown when the length is less than 0.
    ///  Thrown when the length is greater than the length of the array minus the index.
    ///  Thrown when the length is greater than the length of the destination array.
    /// </exception>
    public static void CopyTo<T>(this T[] source, T[] destination, int index, int length)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        if ((index + length) > source.Length || length > destination.Length)
            throw new ArgumentOutOfRangeException(nameof(length));

        Array.Copy(source, index, destination, 0, length);
    }

    /// <summary>
    /// Copies the elements of the array to the specified array starting at the specified index.
    /// </summary>
    /// <param name="source">The one dimensional array source of elements.</param>
    /// <param name="destination">The one dimensional array destination for elements.</param>
    /// <param name="index">The zero-based position of where to start the copy from the source array..</param>
    /// <param name="destinationIndex">The zero-based position of where to inserting elements in the destination array.</param>
    /// <param name="length">The number of elements to copy.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the source array is null.
    /// Thrown when the destination array is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown when the index is less than 0.
    ///  Thrown when the length is less than 0.
    ///  Thrown when the length is greater than the length of the array minus the index.
    ///  Thrown when the length is greater than the length of the destination array.
    /// </exception>
    public static void CopyTo<T>(this T[] source, T[] destination, int index, int destinationIndex, int length)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        if ((index + length) > source.Length)
            throw new ArgumentOutOfRangeException(nameof(length));

        Array.Copy(source, index, destination, destinationIndex, length);
    }

    /// <summary>
    /// Creates a slice of the given array as a <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="array">The one dimensional array to slice from.</param>
    /// <param name="start">The zero-based position to start the slice.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <see cref="Span{T}"/>.</returns>
    public static Span<T> Slice<T>(this T[] array, int start)
        => Slice(array, start, array.Length - start);

    /// <summary>
    /// Creates a slice of the given array as a <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="array">The one dimensional array to slice from.</param>
    /// <param name="start">The zero-based position to start the slice.</param>
    /// <param name="length">The number of elements to include in the slice.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <see cref="Span{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="array"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the <paramref name="start"/> or <paramref name="length"/> is less than zero.
    /// Thrown when the <paramref name="start"/> plus <paramref name="length"/> is greater than
    /// <paramref name="array"/>'s length.
    /// </exception>
    public static Span<T> Slice<T>(this T[] array, int start, int length)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start));

        if ((start + length) > array.Length)
            throw new ArgumentOutOfRangeException(nameof(length));

        return array.AsSpan(start, length);
    }

    public static void ForEach(this Array array, Action<Array, int[]> action)
    {
        if (array.LongLength == 0)
            return;

        ArrayTraverse walker = new ArrayTraverse(array);
        do action(array, walker.Position);
        while (walker.Step());
    }
}

internal sealed class ArrayTraverse
{
    private readonly int[] maxLengths;

    public ArrayTraverse(Array array)
    {
        this.maxLengths = new int[array.Rank];
        for (int i = 0; i < array.Rank; ++i)
        {
            this.maxLengths[i] = array.GetLength(i) - 1;
        }

        this.Position = new int[array.Rank];
    }

    public int[] Position { get; }

    public bool Step()
    {
        for (int i = 0; i < this.Position.Length; ++i)
        {
            if (this.Position[i] < this.maxLengths[i])
            {
                this.Position[i]++;
                for (int j = 0; j < i; j++)
                {
                   this.Position[j] = 0;
                }

                return true;
            }
        }

        return false;
    }
}