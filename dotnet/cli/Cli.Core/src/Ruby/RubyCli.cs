using Bearz.Std;

namespace Bearz.Cli.Ruby;

public class RubyCli : ShellCliCommand
{
    public RubyCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("ruby", context, startInfo)
    {
    }

    public override string Extension => ".rb";
}