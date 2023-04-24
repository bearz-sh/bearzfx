using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Python;

public class PythonCommand : ShellCliCommand
{
    public PythonCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("python", context, startInfo)
    {
        this.LinuxPaths = new[]
        {
            "/usr/bin/python3",
            "/usr/bin/python",
        };
    }

    public override string Extension => ".py";

    public static PythonCommand Create()
        => new();

    public static PythonCommand Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}