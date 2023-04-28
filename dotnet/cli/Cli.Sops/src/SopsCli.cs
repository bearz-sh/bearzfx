using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Sops;

public class SopsCli : CliCommand
{
    public SopsCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("sops", context, startInfo)
    {
    }

    public static SopsCli Create()
        => new();

    public static SopsCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}