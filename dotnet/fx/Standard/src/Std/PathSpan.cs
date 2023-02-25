using System.Text;

using Bearz.Extra.Memory;
using Bearz.Extra.Strings;
using Bearz.Text;

namespace Bearz.Std;

public ref struct PathSpan
{
    private readonly ReadOnlySpan<char> span;

    private string? expanded;

    public PathSpan(string? path)
    {
        this.span = path == null ? new ReadOnlySpan<char>(Array.Empty<char>()) : path.AsSpan();
    }

    public PathSpan(ReadOnlySpan<char> span)
    {
        this.span = span;
    }

    public int Length => this.span.Length;

    public bool IsEmpty => this.span.IsEmpty;

    public char this[int index] => this.span[index];

    public static implicit operator string(PathSpan span)
    {
        return span.ToString();
    }

    public static implicit operator ReadOnlySpan<char>(PathSpan span)
    {
        return span.span;
    }

    public static implicit operator PathSpan(ReadOnlySpan<char> span)
    {
        return new PathSpan(span);
    }

    public static implicit operator PathSpan(string str)
    {
#if NETLEGACY
        return new PathSpan(str.AsSpan());
#else
        return new PathSpan(str);
#endif
    }

    public static implicit operator PathSpan(char[] chars)
    {
        return new PathSpan(chars);
    }

    public bool Exists()
    {
        return Path.Exists(this);
    }

    public ReadOnlySpan<char> Extension()
    {
#if NETLEGACY
        return Path.Extension(this.ToString()).AsSpan();
#else
        return Path.Extension(this);
#endif
    }

    public PathSpan ChangeExtension(ReadOnlySpan<char> extension)
    {
        var ext = this.Extension();
        if (ext.Length == 0)
        {
            return this.span.Concat(extension);
        }
        else
        {
            var result = new char[this.span.Length - ext.Length + extension.Length];
            this.span.Slice(0, this.span.Length - ext.Length).CopyTo(result);
            extension.CopyTo(result.AsSpan(this.span.Length - ext.Length));
            return result;
        }
    }

    public PathSpan ChangeExtension(string extension)
    {
        return Path.ChangeExtension(this, extension);
    }

    public PathSpan Dirname()
    {
#if NETLEGACY
        return new PathSpan(Path.Dirname(this.ToString()));
#else
        return Path.Dirname(this.span);
#endif
    }

    public PathSpan Basename()
    {
#if NETLEGACY
        return new PathSpan(Path.Basename(this.ToString()));
#else
        return Path.Basename(this.span);
#endif
    }

    public PathSpan BasenameWithoutExtension()
    {
#if NETLEGACY
        return new PathSpan(Path.BasenameWithoutExtension(this.ToString()));
#else
        return Path.BasenameWithoutExtension(this.span);
#endif
    }

    public byte[] ReadFile()
    {
        return Fs.ReadFile(this.ToString());
    }

    public string ReadTextFile(Encoding? encoding = null)
    {
        return Fs.ReadTextFile(this.ToString(), encoding);
    }

    public void WriteFile(byte[] data)
    {
        Fs.WriteFile(this.ToString(), data);
    }

    public void WriteTextFile(string text, Encoding? encoding = null)
    {
        Fs.WriteTextFile(this.ToString(), text, encoding);
    }

    public void MakeDirectory()
    {
        Fs.MakeDirectory(this.ToString());
    }

#if NET7_0_OR_GREATER

    public void MakeDirectory(UnixFileMode mode)
    {
        Fs.MakeDirectory(this.ToString(), mode);
    }

#endif

    public void RemoveDirectory(bool recursive = false)
    {
        Fs.RemoveDirectory(this.ToString(), recursive);
    }

    public PathSpan Join(ReadOnlySpan<char> path1)
    {
        if (path1.IsEmpty)
            return this;

        if (this.span.IsEmpty)
            return new PathSpan(path1);

        var path = this.span;
        var last = path[^1];
        var first1 = path1[0];
        if (last == Path.DirectorySeparator || last == Path.AltDirectorySeparator)
        {
            path = path.Slice(0, path.Length - 1);
        }

        if (first1 == Path.DirectorySeparator || first1 == Path.AltDirectorySeparator)
        {
            path1 = path1.Slice(1);
        }

        path = path.Join(Path.DirectorySeparator, path1);
        return new PathSpan(path);
    }

    public PathSpan Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2)
    {
        if (path1.IsEmpty)
            return this;

        if (this.span.IsEmpty)
            return new PathSpan(path1);

        var path = this.span;
        var last = path[^1];
        var first1 = path1[0];
        var last1 = path1[^1];
        var first2 = path2[0];
        if (last == Path.DirectorySeparator || last == Path.AltDirectorySeparator)
        {
            path = path.Slice(0, path.Length - 1);
        }

        if (first1 == Path.DirectorySeparator || first1 == Path.AltDirectorySeparator)
        {
            if (last1 == Path.DirectorySeparator || last1 == Path.AltDirectorySeparator)
                path1 = path1.Slice(1, path1.Length - 2);
            else
                path1 = path1.Slice(1);
        }
        else if (last1 == Path.DirectorySeparator || last1 == Path.AltDirectorySeparator)
        {
            path1 = path1.Slice(0, path1.Length - 1);
        }

        if (first2 == Path.DirectorySeparator || first2 == Path.AltDirectorySeparator)
        {
            path2 = path2.Slice(1);
        }

        path = path.Join(Path.DirectorySeparator, path1, path2);
        return new PathSpan(path);
    }

    public PathSpan Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3)
    {
        if (path1.IsEmpty)
            return this;

        if (this.span.IsEmpty)
            return new PathSpan(path1);

        var path = this.span;
        var last = path[^1];
        var first1 = path1[0];
        var last1 = path1[^1];
        var first2 = path2[0];
        var last2 = path2[^1];
        var first3 = path3[0];
        if (last == Path.DirectorySeparator || last == Path.AltDirectorySeparator)
        {
            path = path.Slice(0, path.Length - 1);
        }

        if (first1 == Path.DirectorySeparator || first1 == Path.AltDirectorySeparator)
        {
            if (last1 == Path.DirectorySeparator || last1 == Path.AltDirectorySeparator)
                path1 = path1.Slice(1, path1.Length - 2);
            else
                path1 = path1.Slice(1);
        }
        else if (last1 == Path.DirectorySeparator || last1 == Path.AltDirectorySeparator)
        {
            path1 = path1.Slice(0, path1.Length - 1);
        }

        if (first2 == Path.DirectorySeparator || first2 == Path.AltDirectorySeparator)
        {
            if (last2 == Path.DirectorySeparator || last2 == Path.AltDirectorySeparator)
                path2 = path2.Slice(1, path2.Length - 2);
            else
                path2 = path2.Slice(1);
        }
        else if (last2 == Path.DirectorySeparator || last2 == Path.AltDirectorySeparator)
        {
            path1 = path1.Slice(0, path2.Length - 1);
        }

        if (first3 == Path.DirectorySeparator || first3 == Path.AltDirectorySeparator)
        {
            path3 = path3.Slice(1);
        }

        path = path.Join(Path.DirectorySeparator, path1, path2, path3);
        return new PathSpan(path);
    }

    public PathSpan Join(
        ReadOnlySpan<char> path1,
        ReadOnlySpan<char> path2,
        ReadOnlySpan<char> path3,
        ReadOnlySpan<char> path4)
    {
        if (path1.IsEmpty)
            return this;

        if (this.span.IsEmpty)
            return new PathSpan(path1);

        var path = this.span;
        var last = path[^1];
        var first1 = path1[0];
        var last1 = path1[^1];
        var first2 = path2[0];
        var last2 = path2[^1];
        var first3 = path3[0];
        var last3 = path3[^1];
        var first4 = path4[0];
        if (last == Path.DirectorySeparator || last == Path.AltDirectorySeparator)
        {
            path = path.Slice(0, path.Length - 1);
        }

        if (first1 == Path.DirectorySeparator || first1 == Path.AltDirectorySeparator)
        {
            if (last1 == Path.DirectorySeparator || last1 == Path.AltDirectorySeparator)
                path1 = path1.Slice(1, path1.Length - 2);
            else
                path1 = path1.Slice(1);
        }
        else if (last1 == Path.DirectorySeparator || last1 == Path.AltDirectorySeparator)
        {
            path1 = path1.Slice(0, path1.Length - 1);
        }

        if (first2 == Path.DirectorySeparator || first2 == Path.AltDirectorySeparator)
        {
            if (last2 == Path.DirectorySeparator || last2 == Path.AltDirectorySeparator)
                path2 = path2.Slice(1, path2.Length - 2);
            else
                path2 = path2.Slice(1);
        }
        else if (last2 == Path.DirectorySeparator || last2 == Path.AltDirectorySeparator)
        {
            path1 = path1.Slice(0, path2.Length - 1);
        }

        if (first3 == Path.DirectorySeparator || first3 == Path.AltDirectorySeparator)
        {
            if (last3 == Path.DirectorySeparator || last3 == Path.AltDirectorySeparator)
                path3 = path3.Slice(1, path3.Length - 2);
            else
                path3 = path3.Slice(1);
        }
        else if (last3 == Path.DirectorySeparator || last3 == Path.AltDirectorySeparator)
        {
            path3 = path3.Slice(0, path3.Length - 1);
        }

        if (first4 == Path.DirectorySeparator || first4 == Path.AltDirectorySeparator)
        {
            path4 = path4.Slice(1);
        }

        path = path.Join(Path.DirectorySeparator, path1, path2, path3, path4);
        return new PathSpan(path);
    }

    public override string ToString()
    {
        if (this.expanded != null)
            return this.expanded;

        this.expanded = this.span.AsString();
        return this.expanded;
    }
}