#if NETLEGACY

namespace Bearz;

public static class StreamExtensions
{
    public static void Write(this Stream stream, Span<byte> buffer)
    {
        var set = buffer.ToArray();
        stream.Write(set, 0, set.Length);
        Array.Clear(set, 0, set.Length);
    }

    public static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
    {
        var set = buffer.ToArray();
        stream.Write(set, 0, set.Length);
        Array.Clear(set, 0, set.Length);
    }

    public static void Write(this BinaryWriter writer, Span<byte> buffer)
    {
        var set = buffer.ToArray();
        writer.Write(set);
        Array.Clear(set, 0, set.Length);
    }

    public static void Write(this BinaryWriter writer, ReadOnlySpan<byte> buffer)
    {
        var set = buffer.ToArray();
        writer.Write(set);
        Array.Clear(set, 0, set.Length);
    }
}
#endif