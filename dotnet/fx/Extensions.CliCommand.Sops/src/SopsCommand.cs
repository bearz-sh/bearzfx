using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Sops;

public class SopsCommand : CliCommand
{
    public SopsCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("sops", context, startInfo)
    {
    }

    public static SopsCommand Create()
        => new();

    public static SopsCommand Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}