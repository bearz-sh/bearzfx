using Bearz.Std;

namespace Bearz.Cli.Sh;

public class ShCli : ShellCliCommand
{
    public ShCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("sh", context, startInfo)
    {
        this.WindowsPaths = new[]
        {
            "%ProgramFiles%\\Git\\bin\\sh.exe",
            "%ProgramFiles%\\Git\\usr\\bin\\sh.exe",
        };
    }

    public override string Extension => ".sh";

    public static ShCli Create()
        => new();

    public static ShCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);

    protected override CommandArgs GenerateScriptArgs(string tempFile)
    {
        return new CommandArgs()
        {
            "-e",
            tempFile,
        };
    }
}