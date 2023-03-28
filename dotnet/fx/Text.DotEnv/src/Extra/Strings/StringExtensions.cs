using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Bearz.Extra.Strings;

internal static class StringExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(this ReadOnlySpan<char> self)
    {
#if NETLEGACY
        return new string(self.ToArray());
#else
        return self.ToString();
#endif
    }

#if NETLEGACY
    /// <summary>
    /// Indicates whether this instance contains the value.
    /// </summary>
    /// <param name="source">The instance.</param>
    /// <param name="value">The value.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns><see langword="true" /> if contains the given value; otherwise, <see langword="false" />.</returns>
    public static bool Contains(this string? source, string value, StringComparison comparison)
    {
        if (source is null)
            return false;

        return value.IndexOf(value, comparison) > -1;
    }

    /// <summary>
    /// Splits a <see cref="string"/> into substrings using the separator.
    /// </summary>
    /// <param name="source">The string instance to split.</param>
    /// <param name="separator">The separator that is used to split the string.</param>
    /// <returns>The <see cref="T:string[]"/>.</returns>
    public static string[] Split(this string source, string separator)
    {
        return source.Split(separator.ToCharArray());
    }

    /// <summary>
    /// Splits a <see cref="string"/> into substrings using the separator.
    /// </summary>
    /// <param name="source">The string instance to split.</param>
    /// <param name="separator">The separator that is used to split the string.</param>
    /// <param name="options">The string split options.</param>
    /// <returns>The <see cref="T:string[]"/>.</returns>
    public static string[] Split(this string source, string separator, StringSplitOptions options)
    {
        return source.Split(separator.ToCharArray(), options);
    }
#endif
}