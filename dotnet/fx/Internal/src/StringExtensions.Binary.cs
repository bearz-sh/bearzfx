using System.Text;

namespace Bearz;

internal static partial class StringExtensions
{
    /// <summary>
    /// Converts the <see cref="string"/> to a <see cref="T:byte[]"/> array.
    /// </summary>
    /// <param name="source">The string to convert.</param>
    /// <param name="encoding">The encoding that converts the string into bytes. Defaults to the system default.</param>
    /// <returns>The encoded string as bytes.</returns>
    public static byte[] ToBytes(this string? source, Encoding? encoding = null)
        => source == null ?
            Array.Empty<byte>() :
            (encoding ?? Encoding.Default).GetBytes(source);

    /// <summary>
    /// Converts the <see cref="string"/> value into a <see cref="Stream"/>.
    /// </summary>
    /// <param name="source">The value.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="writable">Creates a writable stream if <see langword="true" />.</param>
    /// <returns>The <see cref="Stream"/>.</returns>
    public static Stream ToStream(this string source, Encoding? encoding = null, bool writable = true)
        => new MemoryStream((encoding ?? Encoding.Default).GetBytes(source), writable);
}