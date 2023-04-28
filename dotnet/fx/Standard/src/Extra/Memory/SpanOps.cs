using System.Text;

using Bearz.Extra.Arrays;

namespace Bearz.Extra.Memory;

public static class SpanOps
{
    /// <summary>
    /// Appends the value to the end of a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Append<T>(ref Span<T> span, ReadOnlySpan<T> value)
    {
        var copy = new T[span.Length + value.Length];
        span.CopyTo(copy);
        value.CopyTo(copy.AsSpan(span.Length));
        span = copy;
    }

    /// <summary>
    /// Appends the values to the end of a read only span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Append<T>(ref ReadOnlySpan<T> span, ReadOnlySpan<T> value)
    {
        var copy = new T[span.Length + value.Length];
        span.CopyTo(copy);
        value.CopyTo(copy.AsSpan(span.Length));
        span = copy;
    }

    /// <summary>
    /// Appends the value to the end of a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Append<T>(ref Span<T> span, T value)
    {
        var copy = new T[span.Length + 1];
        span.CopyTo(copy);
        copy[^1] = value;
        span = copy;
    }

    /// <summary>
    /// Appends the value to the end of a readonly span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Append<T>(ref ReadOnlySpan<T> span, T value)
    {
        var copy = new T[span.Length + 1];
        span.CopyTo(copy);
        copy[^1] = value;
        span = copy;
    }

    /// <summary>
    /// Appends the value to the end of a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Append<T>(ref ReadOnlySpan<T> span, params T[] value)
        => Append(ref span, value.AsSpan());

    /// <summary>
    /// Appends the value to the end of a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Append<T>(ref Span<T> span, params T[] value)
        => Append(ref span, value.AsSpan());

    /// <summary>
    /// Appends the value to the end of a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Append<T>(ref ReadOnlySpan<T> span, IEnumerable<T> value)
    {
        switch (value)
        {
            case T[] array:
                {
                    var copy = new T[array.Length + span.Length];
                    span.CopyTo(copy);
                    array.CopyTo(copy.AsSpan(span.Length));
                    span = copy;
                }

                break;

            case IList<T> list:
                {
                    var copy = new T[list.Count + span.Length];
                    span.CopyTo(copy);
                    list.CopyTo(copy, span.Length);
                    span = copy;
                }

                break;

            case IReadOnlyCollection<T> roList:
                {
                    var copy = new T[roList.Count + span.Length];
                    span.CopyTo(copy);
                    int j = span.Length;
                    foreach (var item in roList)
                        copy[j++] = item;
                    span = copy;
                }

                break;

            default:
                {
                    Span<T> items = value.ToArray();
                    Append(ref span, items);
                }

                break;
        }
    }

    /// <summary>
    /// Appends the value to the end of a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Append<T>(ref Span<T> span, IEnumerable<T> value)
    {
        switch (value)
        {
            case T[] array:
                {
                    var copy = new T[array.Length + span.Length];
                    span.CopyTo(copy);
                    array.CopyTo(copy.AsSpan(span.Length));
                    span = copy;
                }

                break;

            case IList<T> list:
                {
                    var copy = new T[list.Count + span.Length];
                    span.CopyTo(copy);
                    list.CopyTo(copy, span.Length);
                    span = copy;
                }

                break;

            case IReadOnlyCollection<T> roList:
                {
                    var copy = new T[roList.Count + span.Length];
                    span.CopyTo(copy);
                    int j = span.Length;
                    foreach (var item in roList)
                        copy[j++] = item;
                    span = copy;
                }

                break;

            default:
                {
                    Span<T> items = value.ToArray();
                    Append(ref span, items);
                }

                break;
        }
    }

    /// <summary>
    /// Converts a span of characters to a string, including older versions of .NET.
    /// </summary>
    /// <param name="buffer">The buffer to convert.</param>
    /// <returns>A string of characters.</returns>
    public static string ConvertToString(ReadOnlySpan<char> buffer)
    {
#if NETLEGACY
        return new string(buffer.ToArray());
#else
        return buffer.ToString();
#endif
    }

    /// <summary>
    /// Converts a span of bytes to a string, including older versions of .NET.
    /// </summary>
    /// <param name="buffer">The buffer to convert.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>A string of characters.</returns>
    public static string ConvertToString(ReadOnlySpan<byte> buffer, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
#if NETLEGACY
        return encoding.GetString(buffer.ToArray());
#else
        return encoding.GetString(buffer);
#endif
    }

    /// <summary>
    /// Inserts a value into a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="index">The zero-based position where the item should be inserted.</param>
    /// <param name="value">The value to insert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Insert<T>(ref Span<T> span, int index, T value)
    {
        var copy = new T[span.Length + 1];
        span.Slice(0, index).CopyTo(copy);
        copy[index] = value;
        span.Slice(index).CopyTo(copy.AsSpan(index + 1));
        span = copy;
    }

    /// <summary>
    /// Inserts a value into a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="index">The zero-based position where the item should be inserted.</param>
    /// <param name="value">The value to insert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Insert<T>(ref ReadOnlySpan<T> span, int index, T value)
    {
        var copy = new T[span.Length + 1];
        span.Slice(0, index).CopyTo(copy);
        copy[index] = value;
        span.Slice(index).CopyTo(copy.AsSpan(index + 1));
        span = copy;
    }

    /// <summary>
    /// Inserts a value into a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="index">The zero-based position where the item should be inserted.</param>
    /// <param name="value">The value to insert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Insert<T>(ref Span<T> span, int index, ReadOnlySpan<T> value)
    {
        var copy = new T[span.Length + value.Length];
        span.Slice(0, index).CopyTo(copy);
        value.CopyTo(copy.AsSpan(index));
        span.Slice(index).CopyTo(copy.AsSpan(index + value.Length));
        span = copy;
    }

    /// <summary>
    /// Inserts a value into a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="index">The zero-based position where the item should be inserted.</param>
    /// <param name="value">The value to insert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Insert<T>(ref ReadOnlySpan<T> span, int index, ReadOnlySpan<T> value)
    {
        var copy = new T[span.Length + value.Length];
        span.Slice(0, index).CopyTo(copy);
        value.CopyTo(copy.AsSpan(index));
        span.Slice(index).CopyTo(copy.AsSpan(index + value.Length));
        span = copy;
    }

    /// <summary>
    /// Inserts a value into a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="index">The zero-based position where the item should be inserted.</param>
    /// <param name="value">The value to insert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Insert<T>(ref Span<T> span, int index, params T[] value)
        => Insert(ref span, index, value.AsSpan());

    /// <summary>
    /// Inserts a value into a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="index">The zero-based position where the item should be inserted.</param>
    /// <param name="value">The value to insert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Insert<T>(ref ReadOnlySpan<T> span, int index, params T[] value)
        => Insert(ref span, index, value.AsSpan());

    /// <summary>
    /// Inserts a value into a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="index">The zero-based position where the item should be inserted.</param>
    /// <param name="value">The value to insert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Insert<T>(ref Span<T> span, int index, IEnumerable<T> value)
    {
        switch (value)
        {
            case T[] array:
                {
                    Insert(ref span, index, array.AsSpan());
                }

                break;

            case IList<T> list:
                {
                    var copy = new T[list.Count + span.Length];
                    span.Slice(0, index).CopyTo(copy);
                    list.CopyTo(copy, index);
                    span.Slice(index).CopyTo(copy.AsSpan(index + list.Count));
                    span = copy;
                }

                break;

            case IReadOnlyCollection<T> roList:
                {
                    var copy = new T[roList.Count + span.Length];
                    int j = index;
                    foreach (var item in roList)
                        copy[j++] = item;

                    span.Slice(0, index).CopyTo(copy);
                    span.Slice(index).CopyTo(copy.AsSpan(index + roList.Count));
                    span = copy;
                }

                break;

            default:
                {
                    Insert(ref span, index, value.ToArray().AsSpan());
                }

                break;
        }
    }

    /// <summary>
    /// Inserts a value into a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="index">The zero-based position where the item should be inserted.</param>
    /// <param name="value">The value to insert.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Insert<T>(ref ReadOnlySpan<T> span, int index, IEnumerable<T> value)
    {
        switch (value)
        {
            case T[] array:
                {
                    Insert(ref span, index, array.AsSpan());
                }

                break;

            case IList<T> list:
                {
                    var copy = new T[list.Count + span.Length];
                    span.Slice(0, index).CopyTo(copy);
                    list.CopyTo(copy, index);
                    span.Slice(index).CopyTo(copy.AsSpan(index + list.Count));
                    span = copy;
                }

                break;

            case IReadOnlyCollection<T> roList:
                {
                    var copy = new T[roList.Count + span.Length];
                    int j = index;
                    foreach (var item in roList)
                        copy[j++] = item;

                    span.Slice(0, index).CopyTo(copy);
                    span.Slice(index).CopyTo(copy.AsSpan(index + roList.Count));
                    span = copy;
                }

                break;

            default:
                {
                    Insert(ref span, index, value.ToArray().AsSpan());
                }

                break;
        }
    }

    /// <summary>
    /// Prepends a value at the zero based index in a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Prepend<T>(ref Span<T> span, ReadOnlySpan<T> value)
    {
        var copy = new T[span.Length + value.Length];
        value.CopyTo(copy);
        span.CopyTo(copy.AsSpan(value.Length));
        span = copy;
    }

    /// <summary>
    /// Prepends a value at the zero based index in a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Prepend<T>(ref ReadOnlySpan<T> span, ReadOnlySpan<T> value)
    {
        var copy = new T[span.Length + value.Length];
        value.CopyTo(copy);
        span.CopyTo(copy.AsSpan(value.Length));
        span = copy;
    }

    /// <summary>
    /// Prepends a value at the zero based index in a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Prepend<T>(ref Span<T> span, T value)
    {
        var copy = new Span<T>(new T[span.Length + 1]) { [0] = value };
        span.CopyTo(copy.Slice(1));
        span = copy;
    }

    /// <summary>
    /// Prepends a value at the zero based index in a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Prepend<T>(ref ReadOnlySpan<T> span, T value)
    {
        var copy = new Span<T>(new T[span.Length + 1]) { [0] = value };
        span.CopyTo(copy.Slice(1));
        span = copy;
    }

    /// <summary>
    /// Prepends a value at the zero based index in a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Prepend<T>(ref ReadOnlySpan<T> span, params T[] value)
        => Prepend(ref span, value.AsSpan());

    /// <summary>
    /// Prepends a value at the zero based index in a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Prepend<T>(ref Span<T> span, params T[] value)
        => Prepend(ref span, value.AsSpan());

    /// <summary>
    /// Prepends a value at the zero based index in a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Prepend<T>(ref ReadOnlySpan<T> span, IEnumerable<T> value)
    {
        switch (value)
        {
            case T[] array:
                {
                    Prepend(ref span, array.AsSpan());
                }

                break;

            case IList<T> list:
                {
                    var copy = new T[list.Count + span.Length];
                    list.CopyTo(copy, 0);
                    span.CopyTo(copy.AsSpan(list.Count));
                    span = copy;
                }

                break;

            case IReadOnlyCollection<T> roList:
                {
                    var copy = new T[roList.Count + span.Length];
                    int j = 0;
                    foreach (var item in roList)
                        copy[j++] = item;
                    span.CopyTo(copy.AsSpan(roList.Count));
                    span = copy;
                }

                break;

            default:
                {
                    Prepend(ref span, value.ToArray().AsSpan());
                }

                break;
        }
    }

    /// <summary>
    /// Prepends a value at the zero based index in a span and resizes the span.
    /// </summary>
    /// <param name="span">The reference to resize.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void Prepend<T>(ref Span<T> span, IEnumerable<T> value)
    {
        switch (value)
        {
            case T[] array:
                {
                    Prepend(ref span, array.AsSpan());
                }

                break;

            case IList<T> list:
                {
                    var copy = new T[list.Count + span.Length];
                    list.CopyTo(copy, 0);
                    span.CopyTo(copy.AsSpan(list.Count));
                    span = copy;
                }

                break;

            case IReadOnlyCollection<T> roList:
                {
                    var copy = new T[roList.Count + span.Length];
                    int j = 0;
                    foreach (var item in roList)
                        copy[j++] = item;
                    span.CopyTo(copy.AsSpan(roList.Count));
                    span = copy;
                }

                break;

            default:
                {
                    Prepend(ref span, value.ToArray().AsSpan());
                }

                break;
        }
    }

    /// <summary>
    /// Pops the last item from a span and returns a new span with the item removed using
    /// <c>Slice</c>. This will not affect the original array or span.
    /// </summary>
    /// <param name="span">The span of objects.</param>
    /// <typeparam name="T">The type of object in the span.</typeparam>
    /// <returns>The updated span with one less item.</returns>
    /// <exception cref="ArgumentException">Throws when the span is empty.</exception>
    public static T Pop<T>(ref Span<T> span)
    {
        if (span.IsEmpty)
            throw new ArgumentException("Cannot pop the last item from an empty span.");

        var value = span[^1];
        span = span[..^1];
        return value;
    }

    /// <summary>
    /// Pops the last item from a span and returns the first item and updates the
    /// span reference with the item removed using slice. <c>ref></c> is used to update the span.
    /// </summary>
    /// <param name="span">The span of objects.</param>
    /// <typeparam name="T">The type of object in the span.</typeparam>
    /// <returns>The updated span with one less item.</returns>
    /// <exception cref="ArgumentException">Throws when the span is empty.</exception>
    public static T Pop<T>(ref ReadOnlySpan<T> span)
    {
        if (span.IsEmpty)
            throw new ArgumentException("Cannot pop the last item from an empty span.");

        var value = span[^1];
        span = span[..^1];
        return value;
    }

    /// <summary>
    /// Shifts the first item from a span and returns the first item and updates the
    /// span reference with the item removed using slice. <c>ref></c> is used to update the span.
    /// </summary>
    /// <param name="span">The span of objects.</param>
    /// <typeparam name="T">The type of object in the span.</typeparam>
    /// <returns>The updated span with one less item.</returns>
    /// <exception cref="ArgumentException">Throws when the span is empty.</exception>
    public static T Shift<T>(ref Span<T> span)
    {
        if (span.IsEmpty)
            throw new ArgumentException("Cannot shift the first item from an empty span.");

        var value = span[0];
        span = span[1..];
        return value;
    }

    /// <summary>
    /// Shifts the first item from a readonly span and returns the first item and updates the
    /// span reference with the item removed using slice. <c>ref></c> is used to update the span.
    /// </summary>
    /// <param name="span">The span of objects.</param>
    /// <typeparam name="T">The type of object in the span.</typeparam>
    /// <returns>The updated span with one less item.</returns>
    /// <exception cref="ArgumentException">Throws when the span is empty.</exception>
    public static T Shift<T>(ref ReadOnlySpan<T> span)
    {
        if (span.IsEmpty)
            throw new ArgumentException("Cannot shift the first item from an empty span.");

        var value = span[0];
        span = span[1..];
        return value;
    }

    /// <summary>
    /// Attempts to pop the last item from a span and update the reference to the span with the item
    /// removed using slice. <c>ref></c> is used to update the span reference.
    /// </summary>
    /// <param name="span">The span of objects.</param>
    /// <param name="value">The last item that was removed from the span.</param>
    /// <typeparam name="T">The type of object in the span.</typeparam>
    /// <returns><c>True</c> when the last item was removed; otherwise, <c>false</c>.</returns>
    public static bool TryPop<T>(ref Span<T> span, out T? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = span[^1];
        span = span[..^1];
        return true;
    }

    /// <summary>
    /// Attempts to pop the last item from a read only span and update the reference to the span with the item
    /// removed using slice. <c>ref></c> is used to update the span reference.
    /// </summary>
    /// <param name="span">The span of objects.</param>
    /// <param name="value">The last item that was removed from the span.</param>
    /// <typeparam name="T">The type of object in the span.</typeparam>
    /// <returns><c>True</c> when the last item was removed; otherwise, <c>false</c>.</returns>
    public static bool TryPop<T>(ref ReadOnlySpan<T> span, out T? value)
    {
        if (span.IsEmpty)
        {
            value = default;
            return false;
        }

        value = span[^1];
        span = span[..^1];
        return true;
    }

    /// <summary>
    /// Attempts to shift the first item from a span and returns the first item and updates the
    /// span reference with the item removed using slice. <c>ref></c> is used to update the span.
    /// </summary>
    /// <param name="span">The span of objects.</param>
    /// <param name="value">The first item that was removed from the span.</param>
    /// <typeparam name="T">The type of object in the span.</typeparam>
    /// <returns>The updated span with one less item.</returns>
    /// <exception cref="ArgumentException">Throws when the span is empty.</exception>
    public static bool TryShift<T>(ref Span<T> span, out T? value)
    {
        value = default;
        if (span.IsEmpty)
            return false;

        value = span[0];
        span = span[1..];
        return true;
    }

    /// <summary>
    /// Attempts to shift the first item from a readonly span and returns the first item and updates the
    /// span reference with the item removed using slice. <c>ref></c> is used to update the span.
    /// </summary>
    /// <param name="span">The span of objects.</param>
    /// <param name="value">The first item that was removed from the span.</param>
    /// <typeparam name="T">The type of object in the span.</typeparam>
    /// <returns>The updated span with one less item.</returns>
    /// <exception cref="ArgumentException">Throws when the span is empty.</exception>
    public static bool TryShift<T>(ref ReadOnlySpan<T> span, out T? value)
    {
        value = default;
        if (span.IsEmpty)
            return false;

        value = span[0];
        span = span[1..];
        return true;
    }
}