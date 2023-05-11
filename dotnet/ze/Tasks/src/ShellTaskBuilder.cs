using System.Diagnostics.CodeAnalysis;

namespace Ze.Tasks;

public class ShellTaskBuilder : ZeTaskBuilder
{
    private readonly ShellTask shellTask;

    public ShellTaskBuilder(ShellTask task)
        : base(task)
    {
        this.shellTask = task;
    }

    public ShellTaskBuilder WithRun(string run)
    {
        this.shellTask.Run = run;
        return this;
    }

    public ShellTaskBuilder WithShell(string shell)
    {
        this.shellTask.Shell = shell;
        return this;
    }

    public ShellTaskBuilder WithWorkingDirectory(string workingDirectory)
    {
        this.shellTask.WorkingDirectory = workingDirectory;
        return this;
    }

    public ShellTaskBuilder WithEnv(Dictionary<string, string?> env)
    {
        this.shellTask.Env = env;
        return this;
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1316:Tuple element names should use correct casing")]
    public ShellTaskBuilder WithEnv(params (string key, string? value)[] env)
    {
        this.shellTask.Env = env.ToDictionary(x => x.key, x => x.value);
        return this;
    }

    public ShellTaskBuilder WithFailOnStdErr(bool failOnStdErr = true)
    {
        this.shellTask.FailOnStdErr = failOnStdErr;
        return this;
    }
}