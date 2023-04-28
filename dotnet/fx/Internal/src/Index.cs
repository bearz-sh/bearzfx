#if NETLEGACY

using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Represent a type can be used to index a collection either from the start or the end.</summary>
/// <remarks>
/// Index is used by the C# compiler to support the new index syntax.
/// <code>
/// int[] someArray = new int[5] { 1, 2, 3, 4, 5 } ;
/// int lastElement = someArray[^1]; // lastElement = 5
/// </code>
/// </remarks>
internal readonly struct Index : IEquatable<Index>
{
    private readonly int value;

    /// <summary>Initializes a new instance of the <see cref="Index"/> struct.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
#if !NET35
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public Index(int value, bool fromEnd = false)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
        }

        if (fromEnd)
            this.value = ~value;
        else
            this.value = value;
    }

    /// <summary>Gets create an Index pointing at first element.</summary>
    public static Index Start => new Index(0);

    /// <summary>Gets create an Index pointing at beyond last element.</summary>
    public static Index End => new Index(~0);

    /// <summary>Gets the index value.</summary>
    public int Value
    {
        get
        {
            if (this.value < 0)
                return ~this.value;
            else
                return this.value;
        }
    }

    /// <summary>Gets a value indicating whether indicates whether the index is from the start or the end.</summary>
    public bool IsFromEnd => this.value < 0;

    /// <summary>Converts integer number to an Index.</summary>
    /// <param name="value">The start value.</param>
    public static implicit operator Index(int value) => FromStart(value);

    /// <summary>Create an Index from the start at the position indicated by the value.</summary>
    /// <param name="value">The index value from the start.</param>
    /// <returns>The index.</returns>
#if !NET35
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static Index FromStart(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
        }

        return new Index(value);
    }

    /// <summary>Create an Index from the end at the position indicated by the value.</summary>
    /// <param name="value">The index value from the end.</param>
    /// <returns>The index.</returns>
#if !NET35
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static Index FromEnd(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
        }

        return new Index(~value);
    }

    /// <summary>Calculate the offset from the start using the giving collection length.</summary>
    /// <param name="length">The length of the collection that the Index will be used with. length has to be a positive value.</param>
    /// <remarks>
    /// For performance reason, we don't validate the input length parameter and the returned offset value against negative values.
    /// we don't validate either the returned offset is greater than the input length.
    /// It is expected Index will be used with collections which always have non negative length/count. If the returned offset is negative and
    /// then used to index a collection will get out of range exception which will be same affect as the validation.
    /// </remarks>
    /// <returns>The offset.</returns>
#if !NET35
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public int GetOffset(int length)
    {
        int offset = this.value;
        if (this.IsFromEnd)
        {
            // offset = length - (~value)
            // offset = length + (~(~value) + 1)
            // offset = length + value + 1
            offset += length + 1;
        }

        return offset;
    }

    /// <summary>Indicates whether the current Index object is equal to another object of the same type.</summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns><see langword="true" /> when equal; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object? obj) => obj is Index && this.value == ((Index)obj).value;

    /// <summary>Indicates whether the current Index object is equal to another Index object.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><see langword="true" /> when equal; otherwise, <see langword="false" />.</returns>
    public bool Equals(Index other) => this.value == other.value;

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>The hashcode.</returns>
    public override int GetHashCode() => this.value;

    /// <summary>Converts the value of the current Index object to its equivalent string representation.</summary>
    /// <returns>The string representation of the index.</returns>
    public override string ToString()
    {
        if (this.IsFromEnd)
            return "^" + ((uint)this.Value).ToString();

        return ((uint)this.Value).ToString();
    }
}

#endif