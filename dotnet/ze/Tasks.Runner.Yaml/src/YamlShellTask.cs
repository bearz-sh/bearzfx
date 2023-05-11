using Bearz.Cli;
using Bearz.Extra.Strings;
using Bearz.Std;

namespace Ze.Tasks.Runner.Yaml;

public class YamlShellTask : ShellTask
{
    public YamlShellTask(string name)
        : base(name)
    {
    }

    public YamlShellTask(string name, string id)
        : base(name, id)
    {
    }

    public bool AllowOutputs { get; set; }

    public Dictionary<string, InputBlock> Inputs { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    protected override async Task RunTaskAsync(ITaskExecutionContext context, CancellationToken cancellationToken = default)
    {
        var shell = this.Shell ?? (Bearz.Std.Env.IsWindows ? "powershell" : "bash");
        var cliCommand = await ShellCliCommand.RunScriptAsync(
                shell,
                this.Run,
                (si) =>
                {
                    Console.WriteLine(si.Args.ToString());
                    if (!this.WorkingDirectory.IsNullOrWhiteSpace())
                        si.WithCwd(this.WorkingDirectory);
                    if (this.Env.Count > 0)
                        si.WithEnv(this.Env);

                    si.WithStdio(Stdio.Inherit);

                    // TODO: allow taking outputs from the context
                },
                context,
                cancellationToken)
            .ConfigureAwait(false);

        cliCommand.ThrowOnInvalidExitCode();
        if (this.FailOnStdErr)
            cliCommand.ThrowOnStdError();
    }
}