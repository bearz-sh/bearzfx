using System.Collections;

using Bearz.Extra.Strings;
using Bearz.Text;

namespace Bearz.Virtual;

public class VirtualEnvironmentPath : IEnvironmentPath
{
    private readonly IEnvironment env;

    private readonly string key;

    internal VirtualEnvironmentPath(IEnvironment env)
    {
        this.env = env;
        this.key = this.env.IsWindows ? "Path" : "PATH";
    }

    public IEnumerator<string> GetEnumerator()
    {
        foreach (var next in this.SplitPaths())
            yield return next;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();

    public void Add(string path, bool prepend = false)
    {
        if (this.Has(path))
            return;

        if (prepend)
        {
            this.Set($"{path}{Bearz.Std.Path.PathSeparator}{this.env.Get(this.key)}");
        }
        else
        {
            this.Set($"{this.env.Get(this.key)}{Bearz.Std.Path.PathSeparator}{path}");
        }
    }

    public void Remove(string path)
    {
        var paths = this.SplitPaths();
        if (!this.Has(path, paths))
            return;

        var sb = StringBuilderCache.Acquire();
        foreach (var p in paths)
        {
            if (p == path)
                continue;

            if (sb.Length > 0)
                sb.Append(Bearz.Std.Path.PathSeparator);

            sb.Append(p);
        }

        this.Set(StringBuilderCache.GetStringAndRelease(sb));
    }

    public bool Has(string path)
        => this.Has(path, this.SplitPaths());

    public void Set(string paths)
        => this.env[this.key] = paths;

    public string Get()
        => this.env[this.key] ?? string.Empty;

    private bool Has(string path, IReadOnlyList<string> paths)
    {
        if (this.env.IsWindows)
        {
            foreach (var p in paths)
            {
                if (p.EqualsIgnoreCase(path))
                    return true;
            }

            return false;
        }

        foreach (var p in paths)
        {
            if (p.Equals(path))
                return true;
        }

        return false;
    }

    private IReadOnlyList<string> SplitPaths()
        => this.Get().Split(new[] { Bearz.Std.Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
}