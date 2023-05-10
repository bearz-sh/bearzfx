using Bearz.Cli;
using Bearz.Extra.Strings;
using Bearz.Std;

namespace Plank.Tasks;

public class ShellTask : PlankTask
{
    public ShellTask(string name)
        : base(name)
    {
    }

    public ShellTask(string name, string id)
        : base(name, id)
    {
    }

    public string Run { get; set; } = string.Empty;

    public string? WorkingDirectory { get; set; }

    public string? Shell { get; set; }

    public bool FailOnStdErr { get; set; } = false;

    public Dictionary<string, string?> Env { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    protected override async Task RunTaskAsync(ITaskExecutionContext context, CancellationToken cancellationToken = default)
    {
        var shell = this.Shell ?? (Bearz.Std.Env.IsWindows ? "powershell" : "bash");
        var cliCommand = await ShellCliCommand.RunScriptAsync(
                shell,
                this.Run,
                (si) =>
                {
                    if (!this.WorkingDirectory.IsNullOrWhiteSpace())
                        si.WithCwd(this.WorkingDirectory);
                    if (this.Env.Count > 0)
                        si.WithEnv(this.Env);
                },
                cancellationToken)
            .ConfigureAwait(false);

        cliCommand.ThrowOnInvalidExitCode();
        if (this.FailOnStdErr)
            cliCommand.ThrowOnStdError();
    }
}