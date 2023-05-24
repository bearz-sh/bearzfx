using Bearz.Std;

namespace Bearz.Cli.Deno;

public class DenoCli : ShellCliCommand
{
    public DenoCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("deno", context, startInfo)
    {
    }

    public override string Extension => ".ts";

    public static DenoCli Create()
        => new();

    public static DenoCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);

    protected override CommandArgs GenerateScriptArgs(string tempFile)
    {
        return new CommandArgs()
        {
            "run",
            "--unstable",
            "--allow-all",
            tempFile,
        };
    }
}