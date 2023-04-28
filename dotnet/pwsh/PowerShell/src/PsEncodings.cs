using System.Text;

using Bearz.Extra.Strings;
using Bearz.Text;

namespace Bearz.PowerShell;

public static class PsEncodings
{
    public static Encoding GetEncoding(string? encodingName)
    {
        if (string.IsNullOrEmpty(encodingName))
        {
            return Encodings.Utf8;
        }

        if (encodingName.EqualsIgnoreCase("utf8nobom"))
        {
            return Encodings.Utf8NoBom;
        }

        if (encodingName.EqualsIgnoreCase("utf8bom"))
        {
            return Encodings.Utf8;
        }

        if (encodingName.EqualsIgnoreCase("bigendianutf32"))
        {
            return Encodings.BigEndianUnicode;
        }

        try
        {
            return Encoding.GetEncoding(encodingName);
        }
        catch (ArgumentException)
        {
            return Encodings.Utf8;
        }
    }
}