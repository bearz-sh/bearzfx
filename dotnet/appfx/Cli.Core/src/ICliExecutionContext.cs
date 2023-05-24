using Bearz.Virtual;

namespace Bearz.Cli;

public interface ICliExecutionContext
{
    IServiceProvider Services { get; }

    IEnvironment Env { get; }

    IProcess Process { get; }

    IFileSystem Fs { get; }

    IPath Path { get; }
}