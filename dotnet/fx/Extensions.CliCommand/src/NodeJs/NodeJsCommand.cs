using Bearz.Std;

namespace Bearz.Extensions.CliCommand.NodeJs;

public class NodeJsCommand : ShellCliCommand
{
    public NodeJsCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("node", context, startInfo)
    {
    }

    public override string Extension => ".js";
}