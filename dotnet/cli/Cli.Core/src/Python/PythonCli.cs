using Bearz.Std;

namespace Bearz.Cli.Python;

public class PythonCli : ShellCliCommand
{
    public PythonCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("python", context, startInfo)
    {
        this.LinuxPaths = new[]
        {
            "/usr/bin/python3",
            "/usr/bin/python",
        };
    }

    public override string Extension => ".py";

    public static PythonCli Create()
        => new();

    public static PythonCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}