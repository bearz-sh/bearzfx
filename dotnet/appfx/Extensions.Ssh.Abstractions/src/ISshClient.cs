namespace Bearz.Extensions.Ssh;

public interface ISshClient
{
    bool IsConnected { get; }

    void Connect();

    void Disconnect();

    SshOutput Run(string command, CancellationToken cancellationToken = default);

    Task<SshOutput> RunAsync(string command, CancellationToken cancellationToken = default);
}