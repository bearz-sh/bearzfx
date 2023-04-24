using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Deno;

public class DenoCommand : ShellCliCommand
{
    public DenoCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("deno", context, startInfo)
    {
    }

    public override string Extension => ".ts";

    public static DenoCommand Create()
        => new();

    public static DenoCommand Create(CommandStartInfo startInfo)
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