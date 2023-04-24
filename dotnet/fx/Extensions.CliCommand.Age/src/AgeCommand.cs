using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Age;

public class AgeCommand : CliCommand
{
    public AgeCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("age", context, startInfo)
    {
    }

    public static AgeCommand Create()
        => new();

    public static AgeCommand Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}