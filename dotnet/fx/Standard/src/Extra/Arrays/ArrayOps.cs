using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Bearz.Extra.Arrays;

public static class ArrayOps
{
    /// <summary>
    /// Concatenates the two arrays into a single new array.
    /// </summary>
    /// <param name="array1">The first array to concatenate.</param>
    /// <param name="array2">The second array to concatenate.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new array.</returns>
    public static T[] Concat<T>(T[] array1, T[] array2)
    {
        var result = new T[array1.Length + array2.Length];
        Array.Copy(array1, result, array1.Length);
        Array.Copy(array2, 0, result, array1.Length, array2.Length);
        return result;
    }

    /// <summary>
    /// Concatenates the two arrays into a single new array.
    /// </summary>
    /// <param name="array1">The first array to concatenate.</param>
    /// <param name="array2">The second array to concatenate.</param>
    /// <param name="array3">The third array to concatenate.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new array.</returns>
    public static T[] Concat<T>(T[] array1, T[] array2, T[] array3)
    {
        var result = new T[array1.Length + array2.Length + array3.Length];
        Array.Copy(array1, result, array1.Length);
        Array.Copy(array2, 0, result, array1.Length, array2.Length);
        Array.Copy(array3, 0, result, array1.Length + array2.Length, array3.Length);
        return result;
    }

    /// <summary>
    /// Concatenates multiple arrays into a single new array.
    /// </summary>
    /// <param name="arrays">The array of arrays to concatenate.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new array.</returns>
    public static T[] Concat<T>(params T[][] arrays)
    {
        var result = new T[arrays.Sum(a => a.Length)];
        var offset = 0;
        foreach (var array in arrays)
        {
            Array.Copy(array, 0, result, offset, array.Length);
            offset += array.Length;
        }

        return result;
    }

    /// <summary>
    /// Pops the last item in the array and returns it.
    /// </summary>
    /// <param name="array">The array to resize.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>The last item in the array.</returns>
    /// <exception cref="ArgumentException">Throws when the length of the array is zero.</exception>
    public static T Pop<T>(ref T[] array)
    {
        if (array.Length == 0)
            throw new ArgumentException("Cannot pop the last item from an empty array.");

        var item = array[^1];
        var copy = new T[array.Length - 1];
        Array.Copy(array, copy, array.Length - 1);
        array = copy;
        return item;
    }

    /// <summary>
    /// Compares two arrays for equality.
    /// </summary>
    /// <param name="left">The left side of the compare.</param>
    /// <param name="right">The right side of the compare.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when both objects are equal; otherwise, <c>false</c>.</returns>
    public static bool Equal<T>(T[]? left, T[]? right)
    {
        return Equal(left, right, EqualityComparer<T>.Default);
    }

    /// <summary>
    /// Compares two arrays for equality using the <paramref name="comparer"/>.
    /// </summary>
    /// <param name="left">The left side of the compare.</param>
    /// <param name="right">The right side of the compare.</param>
    /// <param name="comparer">The comparer implementation to use.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when both objects are equal; otherwise, <c>false</c>.</returns>
    public static bool Equal<T>(T[]? left, T[]? right, IComparer<T> comparer)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left == null || right == null)
            return false;

        if (left.Length != right.Length)
            return false;

        for (int i = 0; i < left.Length; i++)
        {
            var lValue = left[i];
            var rValue = right[i];

            if (comparer.Compare(lValue, rValue) == 0)
                continue;

            return false;
        }

        return true;
    }

    /// <summary>
    /// Compares two arrays for equality using the <paramref name="comparer"/>.
    /// </summary>
    /// <param name="left">The left side of the compare.</param>
    /// <param name="right">The right side of the compare.</param>
    /// <param name="comparer">The comparison delegate to use.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when both objects are equal; otherwise, <c>false</c>.</returns>
    public static bool Equal<T>(T[]? left, T[]? right, Comparison<T> comparer)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left == null || right == null)
            return false;

        if (left.Length != right.Length)
            return false;

        for (int i = 0; i < left.Length; i++)
        {
            var lValue = left[i];
            var rValue = right[i];

            if (comparer(lValue, rValue) == 0)
                continue;

            return false;
        }

        return true;
    }

    /// <summary>
    /// Compares two arrays for equality using the <paramref name="comparer"/>.
    /// </summary>
    /// <param name="left">The left side of the compare.</param>
    /// <param name="right">The right side of the compare.</param>
    /// <param name="comparer">The equality comparer implementation to use.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when both objects are equal; otherwise, <c>false</c>.</returns>
    public static bool Equal<T>(T[]? left, T[]? right, IEqualityComparer<T> comparer)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left == null || right == null)
            return false;

        if (left.Length != right.Length)
            return false;

        for (int i = 0; i < left.Length; i++)
        {
            var lValue = left[i];
            var rValue = right[i];

            if (comparer.Equals(lValue, rValue))
                continue;

            return false;
        }

        return true;
    }

    /// <summary>
    /// Changes the size of the of the one dimensional array to the new size.
    /// </summary>
    /// <param name="array">The one dimensional array reference to resize.</param>
    /// <param name="newSize">The size of the new array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Resize<T>([NotNull] ref T[]? array, int newSize)
    {
        Array.Resize(ref array, newSize);
    }

    /// <summary>
    /// Increases the size of the of the one dimensional array by the amount given.
    /// </summary>
    /// <param name="array">The one dimensional array reference to resize.</param>
    /// <param name="amount">The additional amount to grow the array against the current length.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the amount added to the length is less than zero
    /// or exceeds the maximum length of an array.
    /// </exception>
    public static void Grow<T>([NotNull] ref T[]? array, int amount = 1)
    {
        var l = array?.Length ?? 0;
        var newSize = l + amount;
        if (newSize < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, $"Amount ({amount}) + Length ({l}) must not be negative");

#if NET6_0_OR_GREATER
        if (newSize > Array.MaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                $"Amount + Length must not exceed the max length of an array");
        }
#endif

        Array.Resize(ref array, l + amount);
    }

    /// <summary>
    /// Grows the array to closest multiple of the block size when evaluated against the length.
    /// </summary>
    /// <param name="array">The one dimensional array reference to resize.</param>
    /// <param name="length">The length of the array to target.</param>
    /// <param name="blockSize">The number of items to grow the array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the block size is less than one.</exception>
    public static void GrowBy<T>([NotNull] ref T[]? array, int length = -1, int blockSize = 16)
    {
        var l = array?.Length ?? 0;
        if (length < 0)
            length = l + blockSize;

        if (blockSize < 1)
            throw new ArgumentOutOfRangeException(nameof(blockSize));

        int blocks = l / blockSize;
        int size = blocks * blockSize;
        if (size <= length)
        {
            while (size < length)
            {
                blocks++;
                size = blocks * blockSize;
            }
        }

        Array.Resize(ref array, blocks * blockSize);
    }

    /// <summary>
    /// Increases the size of the of the one dimensional array by the amount given.
    /// </summary>
    /// <param name="array">The one dimensional array reference to resize.</param>
    /// <param name="amount">The additional amount to grow the array against the current length.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Shrink<T>([NotNull] ref T[]? array, int amount = 1)
    {
        var l = array?.Length ?? 0;
        var newSize = l - amount;
        if (newSize < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, $"Amount ({amount} + Length ({l}) must not be less than zero.");

#if NET6_0_OR_GREATER
        if (newSize > Array.MaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                $"Amount + Length must not exceed the max length of an array");
        }
#endif

        Resize(ref array, newSize);
    }

    /// <summary>
    /// Shrinks the array to closest multiple of the block size when evaluated against the length.
    /// </summary>
    /// <param name="array">The one dimensional array reference to resize.</param>
    /// <param name="minimumLength">The length of the array to target.</param>
    /// <param name="blockSize">The number of items to shrink the array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the block size is less than one.</exception>
    public static void ShrinkBy<T>([NotNull] ref T[]? array, int minimumLength = -1, int blockSize = 16)
    {
        var l = array?.Length ?? 0;
        if (minimumLength < 0)
            minimumLength = l - blockSize;

        if (minimumLength == l)
        {
            array ??= new T[minimumLength];
            return;
        }

        if (blockSize < 1)
            throw new ArgumentOutOfRangeException(nameof(blockSize));

        int blocks = l / blockSize;
        int size = blocks * blockSize;
        if (size >= minimumLength)
        {
            while (size > minimumLength)
            {
                blocks--;
                size = blocks * blockSize;
            }
        }

        Resize(ref array, blocks * blockSize);
    }

    /// <summary>
    /// Pops the last items from an array.
    /// </summary>
    /// <param name="array">The one dimensional array reference to resize.</param>
    /// <param name="count">The number of elements to collect.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>An array of elements that were removed from the end of the array.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the array is empty or the count is greater than the length of the array.
    /// </exception>
    public static T[] PopRange<T>(ref T[] array, int count)
    {
        if (array.Length == 0)
            throw new ArgumentException("Cannot pop the last item from an empty array.");

        if (count > array.Length)
            throw new ArgumentException("Cannot pop more items than the array contains.");

        var items = new T[count];
        Array.Copy(array, array.Length - count, items, 0, count);
        var copy = new T[array.Length - count];
        Array.Copy(array, copy, array.Length - count);
        array = copy;
        return items;
    }

    /// <summary>
    /// Clears the array of values.
    /// </summary>
    /// <param name="array">The one dimensional array to clear.</param>
    /// <param name="index">The zero-based position to start clearing values.</param>
    /// <param name="length">The number of elements to clear. If the length is less than 0, it is
    /// set to to the length of the array minus the index.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the length exceeds the length of the array.
    /// </exception>
    public static void Clear<T>(T[] array, int index = 0, int length = -1)
    {
        if (length == -1)
            length = array.Length - index;

        if (length > array.Length - index)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length),
                length,
                $"Length ({length}) + Index ({index}) must not exceed the length of the array ({array.Length}).");
        }

        Array.Clear(array, index, length);
    }

    /// <summary>
    /// Swaps the values of the two items in the array.
    /// </summary>
    /// <param name="array">The one dimensional array where values will be swapped.</param>
    /// <param name="index1">The first zero-based index where the first item exists.</param>
    /// <param name="index2">The second zero-based index where the second item exists.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Swap<T>(T[] array, int index1, int index2)
    {
        (array[index1], array[index2]) = (array[index2], array[index1]);
    }

    /// <summary>
    /// Shifts the first item from the array and returns it and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>Returns the last item in the array.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the array is empty.
    /// </exception>
    public static T Shift<T>(ref T[] array)
    {
        if (array.Length == 0)
            throw new ArgumentException("Cannot shift the first item from an empty array.");

        var item = array[0];
        var copy = new T[array.Length - 1];
        Array.Copy(array, 1, copy, 0, array.Length - 1);
        array = copy;
        return item;
    }

    /// <summary>
    /// Appends the item to the end of the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="element1">The first item to append.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Append<T>(ref T[] array, T element1)
    {
        var copy = new T[array.Length + 1];
        Array.Copy(array, copy, array.Length);
        copy[^1] = element1;
        array = copy;
    }

    /// <summary>
    /// Appends two items to the end of the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="element1">The first item to append.</param>
    /// <param name="element2">The second item to append.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Append<T>(ref T[] array, T element1, T element2)
    {
        var copy = new T[array.Length + 2];
        Array.Copy(array, copy, array.Length);
        copy[^2] = element1;
        copy[^1] = element2;
        array = copy;
    }

    /// <summary>
    /// Appends three items to the end of the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="element1">The first item to append.</param>
    /// <param name="element2">The second item to append.</param>
    /// <param name="element3">The third item to append.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Append<T>(ref T[] array, T element1, T element2, T element3)
    {
        var copy = new T[array.Length + 3];
        Array.Copy(array, copy, array.Length);
        copy[^3] = element1;
        copy[^2] = element2;
        copy[^1] = element3;
        array = copy;
    }

    /// <summary>
    /// Appends elements to the end of the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="elements">The elements to append at the end of the array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Append<T>(ref T[] array, params T[] elements)
    {
        var copy = new T[array.Length + elements.Length];
        Array.Copy(array, copy, array.Length);
        Array.Copy(elements, 0, copy, array.Length, elements.Length);
        array = copy;
    }

    /// <summary>
    /// Appends elements in a span to the end of the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="elements">The elements to append at the end of the array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Append<T>(ref T[] array, Span<T> elements)
    {
        var copy = new T[array.Length + elements.Length];
        Array.Copy(array, copy, array.Length);
        elements.CopyTo(copy.AsSpan(array.Length));
        array = copy;
    }

    /// <summary>
    /// Appends elements in a readonly span to the end of the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="elements">The elements to append at the end of the array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Append<T>(ref T[] array, ReadOnlySpan<T> elements)
    {
        var copy = new T[array.Length + elements.Length];
        Array.Copy(array, copy, array.Length);
        elements.CopyTo(copy.AsSpan(array.Length));
        array = copy;
    }

    /// <summary>
    /// Appends elements in an enumerable object to the end of the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="elements">The elements to append at the end of the array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Append<T>(ref T[] array, IEnumerable<T> elements)
    {
        switch (elements)
        {
            case T[] array1:
                Append(ref array, array1.AsSpan());
                break;

            case List<T> list:
                {
                    var copy = new T[array.Length + list.Count];
                    Array.Copy(array, copy, array.Length);
                    list.CopyTo(copy, array.Length);
                    array = copy;
                }

                break;

            case IReadOnlyCollection<T> readOnlyCollection:
                {
                    var copy = new T[array.Length + readOnlyCollection.Count];
                    Array.Copy(array, copy, array.Length);
                    var j = array.Length;
                    foreach (var item in readOnlyCollection)
                        copy[j++] = item;

                    array = copy;
                }

                break;

            default:
#if !NETLEGACY
                if (elements.TryGetNonEnumeratedCount(out var count))
                {
                    var copy = new T[array.Length + count];
                    Array.Copy(array, copy, array.Length);
                    var j = array.Length;
                    foreach (var item in elements)
                        copy[j++] = item;

                    array = copy;
                    break;
                }
#endif
                Append(ref array, elements.ToArray().AsSpan());
                break;
        }
    }

    /// <summary>
    /// Inserts an element into the array at the specified index and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="index">The zero-based position to start the insertion.</param>
    /// <param name="element1">The first element to append.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Insert<T>(ref T[] array, int index, T element1)
    {
        var copy = new T[array.Length + 1];
        Array.Copy(array, 0, copy, 0, index);
        copy[index] = element1;
        Array.Copy(array, index, copy, index + 1, array.Length - index);
        array = copy;
    }

    /// <summary>
    /// Inserts two elements into the array at the specified index and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="index">The zero-based position to start the insertion.</param>
    /// <param name="element1">The first element to append.</param>
    /// <param name="element2">The second element to append.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Insert<T>(ref T[] array, int index, T element1, T element2)
    {
        var copy = new T[array.Length + 2];
        Array.Copy(array, 0, copy, 0, index);
        copy[index] = element1;
        copy[index + 1] = element2;
        Array.Copy(array, index, copy, index + 2, array.Length - index);
        array = copy;
    }

    /// <summary>
    /// Inserts three elements into the array at the specified index and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="index">The zero-based position to start the insertion.</param>
    /// <param name="element1">The first element to append.</param>
    /// <param name="element2">The second element to append.</param>
    /// <param name="element3">The third element to append.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Insert<T>(ref T[] array, int index, T element1, T element2, T element3)
    {
        var copy = new T[array.Length + 3];
        Array.Copy(array, 0, copy, 0, index);
        copy[index] = element1;
        copy[index + 1] = element2;
        copy[index + 2] = element3;
        Array.Copy(array, index, copy, index + 3, array.Length - index);
        array = copy;
    }

    /// <summary>
    /// Inserts elements into the array at the specified index and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="index">The zero-based position to start the insertion.</param>
    /// <param name="elements">The elements to be inserted.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Insert<T>(ref T[] array, int index, params T[] elements)
    {
        var copy = new T[array.Length + elements.Length];
        Array.Copy(array, 0, copy, 0, index);
        Array.Copy(elements, 0, copy, index, elements.Length);
        Array.Copy(array, index, copy, index + elements.Length, array.Length - index);
        array = copy;
    }

    /// <summary>
    /// Inserts a span of elements into the array at the specified index and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="index">The zero-based position to start the insertion.</param>
    /// <param name="elements">The elements to be inserted.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Insert<T>(ref T[] array, int index, Span<T> elements)
    {
        var copy = new T[array.Length + elements.Length];
        Array.Copy(array, 0, copy, 0, index);
        elements.CopyTo(copy.AsSpan(index));
        Array.Copy(array, index, copy, index + elements.Length, array.Length - index);
        array = copy;
    }

    /// <summary>
    /// Inserts a readonly span of elements into the array at the specified index and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="index">The zero-based position to start the insertion.</param>
    /// <param name="elements">The elements to be inserted.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Insert<T>(ref T[] array, int index, ReadOnlySpan<T> elements)
    {
        var copy = new T[array.Length + elements.Length];
        Array.Copy(array, 0, copy, 0, index);
        elements.CopyTo(copy.AsSpan(index));
        Array.Copy(array, index, copy, index + elements.Length, array.Length - index);
        array = copy;
    }

    /// <summary>
    /// Inserts an enumerable of elements into the array at the specified index and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="index">The zero-based position to start the insertion.</param>
    /// <param name="elements">The elements to be inserted.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Insert<T>(ref T[] array, int index, IEnumerable<T> elements)
    {
        switch (elements)
        {
            case T[] array1:
                Insert(ref array, index, array1);
                break;

            case IList<T> list:
                {
                    var copy = new T[array.Length + list.Count];
                    Array.Copy(array, 0, copy, 0, index);
                    list.CopyTo(copy, index);
                    Array.Copy(array, index, copy, index + list.Count, array.Length - index);
                    array = copy;
                }

                break;

            case IReadOnlyCollection<T> readOnlyCollection:
                {
                    var copy = new T[array.Length + readOnlyCollection.Count];
                    Array.Copy(array, 0, copy, 0, index);
                    var j = index;
                    foreach (var item in readOnlyCollection)
                        copy[j++] = item;

                    Array.Copy(array, index, copy, index + readOnlyCollection.Count, array.Length - index);
                    array = copy;
                }

                break;

            default:
#if !NETLEGACY
                if (elements.TryGetNonEnumeratedCount(out var count))
                {
                    var copy = new T[array.Length + count];
                    Array.Copy(array, 0, copy, 0, index);
                    var j = index;
                    foreach (var item in elements)
                        copy[j++] = item;

                    Array.Copy(array, index, copy, index + count, array.Length - index);
                    array = copy;
                    break;
                }
#endif

                Insert(ref array, index, elements.ToArray());
                break;
        }
    }

    /// <summary>
    /// Prepends an element to the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="element1">The first element to prepend.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Prepend<T>(ref T[] array, T element1)
    {
        var copy = new T[array.Length + 1];
        Array.Copy(array, 0, copy, 1, array.Length);
        copy[0] = element1;
        array = copy;
    }

    /// <summary>
    /// Prepends two elements to the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="element1">The first element to prepend.</param>
    /// <param name="element2">The second element to prepend.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Prepend<T>(ref T[] array, T element1, T element2)
    {
        var copy = new T[array.Length + 2];
        Array.Copy(array, 0, copy, 2, array.Length);
        copy[0] = element1;
        copy[1] = element2;
        array = copy;
    }

    /// <summary>
    /// Prepends three elements to the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="element1">The first element to prepend.</param>
    /// <param name="element2">The second element to prepend.</param>
    /// <param name="element3">The third element to prepend.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Prepend<T>(ref T[] array, T element1, T element2, T element3)
    {
        var copy = new T[array.Length + 3];
        Array.Copy(array, 0, copy, 3, array.Length);
        copy[0] = element1;
        copy[1] = element2;
        copy[2] = element3;
        array = copy;
    }

    /// <summary>
    /// Prepends elements to the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="elements">The elements to prepend.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Prepend<T>(ref T[] array, params T[] elements)
    {
        var copy = new T[array.Length + elements.Length];
        Array.Copy(array, 0, copy, elements.Length, array.Length);
        Array.Copy(elements, copy, elements.Length);
        array = copy;
    }

    /// <summary>
    /// Prepends a span of elements to the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="elements">The elements to prepend.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Prepend<T>(ref T[] array, Span<T> elements)
    {
        var copy = new T[array.Length + elements.Length];
        Array.Copy(array, 0, copy, elements.Length, array.Length);
        elements.CopyTo(copy);
        array = copy;
    }

    /// <summary>
    /// Prepends a readonly span of elements to the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="elements">The elements to prepend.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Prepend<T>(ref T[] array, ReadOnlySpan<T> elements)
    {
        var copy = new T[array.Length + elements.Length];
        Array.Copy(array, 0, copy, elements.Length, array.Length);
        elements.CopyTo(copy);
        array = copy;
    }

    /// <summary>
    /// Prepends an enumerable of elements to the array and then resizes the array.
    /// </summary>
    /// <param name="array">The one dimensional array reference that will be resized.</param>
    /// <param name="elements">The elements to prepend.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    public static void Prepend<T>(ref T[] array, IEnumerable<T> elements)
    {
        switch (elements)
        {
            case T[] array1:
                Prepend(ref array, array1);
                break;

            case List<T> list:
                {
                    var copy = new T[array.Length + list.Count];
                    Array.Copy(array, 0, copy, list.Count, array.Length);
                    list.CopyTo(copy);
                    array = copy;
                }

                break;

            case IReadOnlyCollection<T> readOnlyCollection:
                {
                    var copy = new T[array.Length + readOnlyCollection.Count];
                    Array.Copy(array, 0, copy, readOnlyCollection.Count, array.Length);
                    var j = 0;
                    foreach (var item in readOnlyCollection)
                        copy[j++] = item;

                    array = copy;
                }

                break;

            default:
                Prepend(ref array, elements.ToArray());
                break;
        }
    }

    /// <summary>
    /// Retrieves a one dimensional array from the shared pool.
    /// </summary>
    /// <param name="minimumLength">The minimum length of the array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>The rented one dimensional array from the shared poo.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the <paramref name="minimumLength"/> is less than zero.
    /// </exception>
    public static T[] Rent<T>(int minimumLength)
    {
        if (minimumLength < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumLength));

        return ArrayPool<T>.Shared.Rent(minimumLength);
    }

    /// <summary>
    /// Returns a one dimensional rented array returned from <see cref="Rent{T}"/> method to the
    /// shared pool.
    /// </summary>
    /// <param name="array">A rented array to return to the shared pool.</param>
    /// <param name="clearArray">
    /// Instructs the pool to clear the contents of the array.
    /// When the contents of an array are not cleared, the contents are available
    /// when the array is rented again.
    /// </param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the array is null.
    /// </exception>
    public static void Return<T>(T[] array, bool clearArray = false)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        ArrayPool<T>.Shared.Return(array, clearArray);
    }

    /// <summary>
    /// Creates a new <see cref="ArraySegment{T}"/> from the given array.
    /// </summary>
    /// <param name="array">The one dimensional array.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new <see cref="ArraySegment{T}"/>.</returns>
    public static ArraySegment<T> AsSegment<T>(T[] array)
        => AsSegment(array, 0, array.Length);

    /// <summary>
    /// Creates a new <see cref="ArraySegment{T}"/> from the given array.
    /// </summary>
    /// <param name="array">The one dimensional array.</param>
    /// <param name="start">The zero-based position that will be the start of the segment.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A new <see cref="ArraySegment{T}"/>.</returns>
    public static ArraySegment<T> AsSegment<T>(T[] array, int start)
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
    public static ArraySegment<T> AsSegment<T>(T[] array, int start, int length)
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
    /// Creates a slice of the given array as a <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="array">The one dimensional array to slice from.</param>
    /// <param name="start">The zero-based position to start the slice.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <see cref="Span{T}"/>.</returns>
    public static Span<T> Slice<T>(T[] array, int start)
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
    public static Span<T> Slice<T>(T[] array, int start, int length)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start));

        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        if ((start + length) > array.Length)
            throw new ArgumentOutOfRangeException(nameof(length));

        return array.AsSpan(start, length);
    }

    /// <summary>
    /// Attempts to pop the last item from the end of the array and resize the array.
    /// </summary>
    /// <param name="array">The one dimensional array.</param>
    /// <param name="element">The last item of the array, if available.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when an element is available; otherwise, <c>false</c>.</returns>
    public static bool TryPop<T>(ref T[] array, out T? element)
    {
        if (array.Length == 0)
        {
            element = default;
            return false;
        }

        element = array[^1];
        var copy = new T[array.Length - 1];
        Array.Copy(array, copy, array.Length - 1);
        array = copy;
        return true;
    }

    /// <summary>
    /// Attempts to shift the first item from the beginning of the array and resize the array.
    /// </summary>
    /// <param name="array">The one dimensional array.</param>
    /// <param name="element">The last item of the array, if available.</param>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns><c>True</c> when an element is available; otherwise, <c>false</c>.</returns>
    public static bool TryShift<T>(ref T[] array, out T? element)
    {
        if (array.Length == 0)
        {
            element = default;
            return false;
        }

        element = array[0];
        var copy = new T[array.Length - 1];
        Array.Copy(array, 1, copy, 0, array.Length - 1);
        array = copy;
        return true;
    }
}