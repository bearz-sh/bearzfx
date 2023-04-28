using Bearz.Std;

namespace Bearz.Extensions.CliCommand.NodeJs;

public class NodeJsCli : ShellCliCommand
{
    public NodeJsCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("node", context, startInfo)
    {
    }

    public override string Extension => ".js";
}