using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Docker;

public class DockerCli : CliCommand
{
    public DockerCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("docker", context, startInfo)
    {
    }

    public static DockerCli Create()
        => new();

    public static DockerCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}