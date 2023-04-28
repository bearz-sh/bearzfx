using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Bearz.Text;

// TODO: polyfill GetChunks
public static class StringBuilderExtensions
{
    public static StringBuilder Append(
        this StringBuilder builder,
        Span<char> input)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        foreach (var t in input)
            builder.Append(t);

        return builder;
    }

    public static StringBuilder Append(
        this StringBuilder builder,
        ReadOnlySpan<char> input)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        foreach (var t in input)
            builder.Append(t);

        return builder;
    }

    public static StringBuilder AppendCliArgument(this StringBuilder sb, string argument)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // based on the logic from http://stackoverflow.com/questions/5510343/escape-command-line-arguments-in-c-sharp.
            // The method given there doesn't minimize the use of quotation. For that, I drew from
            // https://blogs.msdn.microsoft.com/twistylittlepassagesallalike/2011/04/23/everyone-quotes-command-line-arguments-the-wrong-way/

            // the essential encoding logic is:
            // (1) non-empty strings with no special characters require no encoding
            // (2) find each substring of 0-or-more \ followed by " and replace it by twice-as-many \, followed by \"
            // (3) check if argument ends on \ and if so, double the number of backslashes at the end
            // (4) add leading and trailing "
            if (!ContainsWindowsSpecialCharacter(argument))
            {
                sb.Append(argument);
                return sb;
            }

            sb.Append('"');

            var backSlashCount = 0;
            foreach (var ch in argument)
            {
                switch (ch)
                {
                    case '\\':
                        ++backSlashCount;
                        break;

                    case '"':
                        sb.Append('\\', repeatCount: (2 * backSlashCount) + 1);
                        backSlashCount = 0;
                        sb.Append(ch);
                        break;

                    default:
                        sb.Append('\\', repeatCount: backSlashCount);
                        backSlashCount = 0;
                        sb.Append(ch);
                        break;
                }
            }

            sb.Append('\\', repeatCount: 2 * backSlashCount)
                .Append('"');

            return sb;
        }

        if (!ContainsSpecialCharacter(argument))
        {
            sb.Append(argument);
            return sb;
        }

        sb.Append('"');
        foreach (var @char in argument)
        {
            switch (@char)
            {
                case '$':
                case '`':
                case '"':
                case '\\':
                    sb.Append('\\');
                    break;
            }

            sb.Append(@char);
        }

        sb.Append('"');

        return sb;
    }

    public static StringBuilder AppendSpace(this StringBuilder stringBuilder)
    {
        return stringBuilder.Append(' ');
    }

    public static StringBuilder AppendSpace(this StringBuilder stringBuilder, int count)
    {
        return stringBuilder.Append(' ', count);
    }

    public static StringBuilder AppendSpace(this StringBuilder stringBuilder, int count, int tabSize)
    {
        return stringBuilder.Append(' ', count * tabSize);
    }

    public static StringBuilder AppendWithAngles(this StringBuilder stringBuilder, string value)
    {
        return stringBuilder
            .Append('<')
            .Append(value)
            .Append('>');
    }

    public static StringBuilder AppendWithAngleBrackets(this StringBuilder stringBuilder, string value)
    {
        return stringBuilder
            .Append('<')
            .Append(value)
            .Append('>');
    }

    public static StringBuilder AppendWithBraces(this StringBuilder stringBuilder, string value)
    {
        return stringBuilder
            .Append('{')
            .Append(value)
            .Append('}');
    }

    public static StringBuilder AppendWithBrackets(this StringBuilder stringBuilder, string value)
    {
        return stringBuilder
            .Append('[')
            .Append(value)
            .Append(']');
    }

    public static StringBuilder AppendWithParens(this StringBuilder stringBuilder, string value)
    {
        return stringBuilder
            .Append('(')
            .Append(value)
            .Append(')');
    }

    public static StringBuilder AppendWithQuotes(this StringBuilder stringBuilder, StringBuilder builder, char quote)
    {
        return stringBuilder
            .Append(quote)
            .Append(builder)
            .Append(quote);
    }

    public static StringBuilder AppendWithQuotes(this StringBuilder stringBuilder, bool value, char quote)
    {
        return stringBuilder
            .Append(quote)
            .Append(value)
            .Append(quote);
    }

    public static StringBuilder AppendWithQuotes(this StringBuilder stringBuilder, int value, char quote)
    {
        return stringBuilder
            .Append(quote)
            .Append(value)
            .Append(quote);
    }

    public static StringBuilder AppendWithQuotes(this StringBuilder stringBuilder, object value, char quote)
    {
        return stringBuilder
            .Append(quote)
            .Append(value)
            .Append(quote);
    }

    public static StringBuilder AppendWithQuotes(this StringBuilder stringBuilder, ReadOnlySpan<char> value)
    {
        return AppendWithQuotes(stringBuilder, value, '"');
    }

    public static StringBuilder AppendWithQuotes(this StringBuilder stringBuilder, ReadOnlySpan<char> value, char quote)
    {
        return stringBuilder
            .Append(quote)
            .Append(value)
            .Append(quote);
    }

    public static StringBuilder AppendWithQuotes(this StringBuilder stringBuilder, string value)
    {
        return AppendWithQuotes(stringBuilder, value, '"');
    }

    public static StringBuilder AppendWithQuotes(this StringBuilder stringBuilder, string value, char quote)
    {
        return stringBuilder
            .Append(quote)
            .Append(value)
            .Append(quote);
    }

    public static void CopyTo(this StringBuilder builder, Span<char> span)
    {
        var set = new char[builder.Length];
        builder.CopyTo(
            0,
            set,
            0,
            span.Length);
        set.CopyTo(span);
    }

    public static int IndexOf(
        this StringBuilder builder,
        string searchText,
        StringComparison comparison)
    {
        return builder
            .AsSpan()
            .IndexOf(searchText.AsSpan(), comparison);
    }

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

    public static bool Contains(
        this StringBuilder builder,
        string searchText,
        StringComparison comparison)
    {
        return Contains(builder, searchText.AsSpan(), comparison);
    }

    public static bool Contains(
        this StringBuilder builder,
        ReadOnlySpan<char> searchText,
        StringComparison comparison)
    {
        return builder
            .AsSpan()
            .Contains(searchText, comparison);
    }

    private static bool ContainsWindowsSpecialCharacter(string s)
    {
        if (s.Length == 0)
            return false;

        foreach (var c in s)
        {
            var isSpecial = c is ' ' or '"';
            if (isSpecial)
                return true;
        }

        return false;
    }

    private static bool ContainsSpecialCharacter(string s)
    {
        if (s.Length == 0)
            return false;

        foreach (var c in s)
        {
            var isSpecial = c switch
            {
                '\\' or '\'' or '"' => true,
                _ => char.IsWhiteSpace(c),
            };

            if (isSpecial)
                return true;
        }

        return false;
    }
}