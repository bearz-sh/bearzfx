using System;
using System.Collections.Generic;

namespace Bearz.Security.Cryptography;

public static class EncryptionExtensions
{
    public static bool SlowEquals(this IList<byte> left, IList<byte> right)
    {
        var l = Math.Min(left.Count, right.Count);
        uint diff = (uint)left.Count ^ (uint)right.Count;
        for (int i = 0; i < l; i++)
        {
            diff |= (uint)(left[i] ^ right[i]);
        }

        return diff == 0;
    }

    public static bool SlowEquals(this Span<byte> left, Span<byte> right)
    {
        var l = Math.Min(left.Length, right.Length);
        uint diff = (uint)left.Length ^ (uint)right.Length;
        for (int i = 0; i < l; i++)
        {
            diff |= (uint)(left[i] ^ right[i]);
        }

        return diff == 0;
    }

    public static bool SlowEquals(this ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        var l = Math.Min(left.Length, right.Length);
        uint diff = (uint)left.Length ^ (uint)right.Length;
        for (int i = 0; i < l; i++)
        {
            diff |= (uint)(left[i] ^ right[i]);
        }

        return diff == 0;
    }
}