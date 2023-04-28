using System.Buffers;
using System.Text;

using Bearz.Extra.Strings;
using Bearz.Text;

namespace Bearz.Std;

public struct PathBuf
{
    private char[] buffer;

    public PathBuf()
    {
        this.buffer = new char[0];
    }

    public PathBuf(ReadOnlySpan<char> path)
    {
        this.buffer = path.ToArray();
    }

    public int Length { get; private set; }

    public static implicit operator ReadOnlySpan<char>(PathBuf path)
        => path.buffer.AsSpan(0, path.Length);

    public static implicit operator PathBuf(ReadOnlySpan<char> path)
        => new PathBuf(path);

    public static implicit operator PathBuf(StringBuilder builder)
        => new PathBuf(builder.ToArray());

    public PathBuf Add(ReadOnlySpan<char> path)
    {
        this.Resize(path.Length);
        path.CopyTo(this.buffer.AsSpan(this.Length));
        return this;
    }

    public PathBuf Join(ReadOnlySpan<char> path1)
    {
        this.Resize(path1.Length + 1);
        this.buffer[this.Length] = FsPath.DirectorySeparator;
        path1.CopyTo(this.buffer.AsSpan(this.Length + 1));
        this.Length += path1.Length + 1;

        return this;
    }

    public PathBuf CopyTo(Span<char> destination)
    {
        this.buffer.AsSpan(0, this.Length).CopyTo(destination);
        return this;
    }

    public PathBuf CopyTo(char[] destination)
    {
        this.buffer.AsSpan(0, this.Length).CopyTo(destination);
        return this;
    }

    public ReadOnlySpan<char> GetExtensionAsReadOnlySpan()
    {
        return this.GetExtensionAsSpan();
    }

    public PathBuf ChangeExtension(ReadOnlySpan<char> extension)
    {
        var ext = this.GetExtensionAsSpan();
        if (ext.IsEmpty)
            return this;

        var span = this.buffer.AsSpan(0, this.Length);
        span.Slice(span.Length - ext.Length).CopyTo(ext);
        return this;
    }

    public string GetExtension()
    {
        var ext = this.GetExtensionAsReadOnlySpan();
        if (ext.IsEmpty)
            return string.Empty;

        return ext.AsString();
    }

    public bool EndsWith(ReadOnlySpan<char> path)
        => this.buffer.AsSpan(0, this.Length).EndsWith(path);

    public bool StartsWith(ReadOnlySpan<char> path)
        => this.buffer.AsSpan(0, this.Length).StartsWith(path);

    public override string ToString()
    {
        return new string(this.buffer, 0, this.Length);
    }

    private Span<char> GetExtensionAsSpan()
    {
        var span = this.buffer.AsSpan(0, this.Length);

        var index = span.IndexOf('/');
        if (index == -1)
            index = span.IndexOf('\\');

        if (index == -1)
            index = 0;

        index = span.Slice(index).LastIndexOf('.');
        if (index == -1)
            return new Span<char>(Array.Empty<char>());

        return span.Slice(index);
    }

    private void Resize(int growth)
    {
        var size = this.Length + growth;
        if (size < this.buffer.Length)
            return;

        var copy = new char[this.buffer.Length * 2];
        Array.Copy(this.buffer, copy, this.buffer.Length);
        this.buffer = copy;
    }
}