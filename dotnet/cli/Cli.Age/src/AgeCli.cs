using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Age;

public class AgeCli : CliCommand
{
    public AgeCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("age", context, startInfo)
    {
    }

    public static AgeCli Create()
        => new();

    public static AgeCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}