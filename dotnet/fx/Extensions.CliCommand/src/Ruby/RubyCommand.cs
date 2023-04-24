using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Ruby;

public class RubyCommand : ShellCliCommand
{
    public RubyCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("ruby", context, startInfo)
    {
    }

    public override string Extension => ".rb";
}