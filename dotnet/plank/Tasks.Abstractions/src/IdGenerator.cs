using Bearz.Text;

namespace Plank.Tasks;

public class IdGenerator : IIdGenerator
{
    private int counter;

    public static IIdGenerator Instance { get; set; } = new IdGenerator();

    public string FromName(ReadOnlySpan<char> name)
    {
        var sb = StringBuilderCache.Acquire();

        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c))
            {
                if (char.IsUpper(c))
                {
                    sb.Append(char.ToLower(c));
                    continue;
                }

                sb.Append(c);
            }

            switch (c)
            {
                case ' ':
                    sb.Append("_");
                    break;
                case '_' or '-' or ':':
                    sb.Append(c);
                    break;
            }
        }

        return StringBuilderCache.GetStringAndRelease(sb);
    }

    public string Generate()
    {
        return this.counter++.ToString();
    }
}