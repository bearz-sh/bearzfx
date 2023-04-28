using System.Text;

using Bearz.Text;

namespace Bearz.Extra.IO;

public static class TextReaderExtensions
{
    public static char[] ReadLineAsArray(this TextReader reader)
    {
        var sb = new StringBuilder();
        while (true)
        {
            int ch = reader.Read();
            if (ch == -1) break;
            if (ch is '\r' or '\n')
            {
                if (ch == '\r' && reader.Peek() == '\n')
                {
                    reader.Read();
                }

                var copy = sb.ToArray();
                sb.Clear();
                return copy;
            }

            sb.Append((char)ch);
        }

        if (sb.Length > 0)
        {
            var copy = sb.ToArray();
            sb.Clear();
            return copy;
        }

        return Array.Empty<char>();
    }
}