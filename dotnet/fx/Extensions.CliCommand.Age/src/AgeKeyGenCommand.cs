using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Age;

public class AgeKeyGenCommand : CliCommand
{
    public AgeKeyGenCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("age-keygen", context, startInfo)
    {
    }

    public static AgeKeyGenCommand Create()
        => new();

    public static AgeKeyGenCommand Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}