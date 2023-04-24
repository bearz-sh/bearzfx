using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Docker;

public class DockerCommand : CliCommand
{
    public DockerCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("docker", context, startInfo)
    {
    }

    public static DockerCommand Create()
        => new();

    public static DockerCommand Create(CommandStartInfo startInfo)
        => new(null, startInfo);
}