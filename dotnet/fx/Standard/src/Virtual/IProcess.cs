using Bearz.Std;

namespace Bearz.Virtual;

public interface IProcess
{
    string CommandLine { get; }

    IReadOnlyList<string> Argv { get; }

    int ExitCode { get; set; }

    int Id { get; }

    TextWriter Out { get; }

    TextWriter Error { get; }

    Command CreateCommand(string fileName, CommandStartInfo? startInfo = null);

    void Kill(int pid);

    void Exit(int code);

    CommandOutput Run(
        string fileName,
        CommandArgs? args,
        string? cwd = null,
        IEnumerable<KeyValuePair<string, string>>? env = null,
        Stdio stdout = Stdio.Inherit,
        Stdio stderr = Stdio.Inherit);

    Task<CommandOutput> RunAsync(
        string fileName,
        CommandArgs? args,
        string? cwd = null,
        IEnumerable<KeyValuePair<string, string>>? env = null,
        Stdio stdout = Stdio.Inherit,
        Stdio stderr = Stdio.Inherit,
        CancellationToken cancellationToken = default);

    string? Which(string command, IEnumerable<string>? prependPaths = null, bool useCache = true);
}