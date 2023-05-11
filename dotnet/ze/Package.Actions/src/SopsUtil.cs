using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Text;

namespace Ze.Package.Actions;

public static class SopsUtil
{
    public static bool IsSopsAvailable()
        => !Env.Process.Which("sop").IsNullOrWhiteSpace();

    public static string? Decrypt(string path)
    {
        if (path.IsNullOrWhiteSpace() || !Fs.FileExists(path))
            return null;

        var cmd = Env.Process.CreateCommand("sops");
        var result = cmd.WithArgs("--decrypt", path)
            .WithStdio(Stdio.Piped)
            .Output();

        result.ThrowOnInvalidExitCode();

        var sb = StringBuilderCache.Acquire();
        foreach (string line in result.StdOut)
        {
            sb.AppendLine(line);
        }

        return StringBuilderCache.GetStringAndRelease(sb);
    }

    public static string? Encrypt(string path)
    {
        if (path.IsNullOrWhiteSpace() || !Fs.FileExists(path))
            return null;

        var cmd = Env.Process.CreateCommand("sops");
        var result = cmd.WithArgs("--encrypt", path)
            .WithStdio(Stdio.Piped)
            .Output();

        result.ThrowOnInvalidExitCode();

        var sb = StringBuilderCache.Acquire();
        foreach (string line in result.StdOut)
        {
            sb.AppendLine(line);
        }

        return StringBuilderCache.GetStringAndRelease(sb);
    }
}