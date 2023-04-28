using Bearz.Std;

namespace Bearz.Cli.Age;

public class AgeKeyGenCli : CliCommand
{
    public AgeKeyGenCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("age-keygen", context, startInfo)
    {
    }

    public static AgeKeyGenCli Create()
        => new();

    public static AgeKeyGenCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}