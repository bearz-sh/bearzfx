using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Sh;

public class ShCommand : ShellCliCommand
{
    public ShCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("sh", context, startInfo)
    {
        this.WindowsPaths = new[]
        {
            "%ProgramFiles%\\Git\\bin\\sh.exe",
            "%ProgramFiles%\\Git\\usr\\bin\\sh.exe",
        };
    }

    public override string Extension => ".sh";

    public static ShCommand Create()
        => new();

    public static ShCommand Create(CommandStartInfo startInfo)
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