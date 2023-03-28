using System.Text;

namespace Bearz.Text;

internal static class StringBuilderExtensions
{
    public static char[] ToArray(this StringBuilder builder)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        var set = new char[builder.Length];
        builder.CopyTo(
            0,
            set,
            0,
            set.Length);
        return set;
    }

    public static ReadOnlySpan<char> AsSpan(this StringBuilder builder)
    {
        var set = ToArray(builder);
        return set;
    }
}