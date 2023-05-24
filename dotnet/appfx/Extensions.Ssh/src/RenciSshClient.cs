using System.Diagnostics;

using Renci.SshNet;

namespace Bearz.Extensions.Ssh;

public sealed class RenciSshClient : ISshClient, IDisposable
{
    private readonly Renci.SshNet.SshClient client;

    public RenciSshClient(SshStartInfo startInfo)
    {
        if (startInfo.CachedConnection is ConnectionInfo ci)
        {
            this.client = new SshClient(ci);
            return;
        }

        this.client = new SshClient(startInfo.CacheConnection());
    }

    public event SshDataReceivedEventHandler OutputDataReceived;

    public event SshDataReceivedEventHandler ErrorDataReceived;

    public bool IsConnected => this.client.IsConnected;

    public bool EnableRaisingEvents { get; set; }

    public Task ConnectAsync()
    {
        var task = new Task(this.Connect);
        task.Start();
        return task;
    }

    public Task DisconnectAsync()
    {
        var task = new Task(this.Disconnect);
        task.Start();
        return task;
    }

    public void Connect()
        => this.client.Connect();

    public void Disconnect()
        => this.client.Disconnect();

    public SshOutput Run(string command, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var cmd = this.client.CreateCommand(command);
        var start = DateTime.UtcNow;
        var r = cmd.BeginExecute();

        using var stdOutReader = new System.IO.StreamReader(cmd.OutputStream);
        using var stdErrorReader = new System.IO.StreamReader(cmd.ExtendedOutputStream);
        var stdOut = new List<string>();
        var stdError = new List<string>();
        while (!r.IsCompleted)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                if (this.EnableRaisingEvents)
                {
                    this.OutputDataReceived?.Invoke(this, new SshDataReceivedEventArgs(null));
                    this.OutputDataReceived?.Invoke(this, new SshDataReceivedEventArgs(null));
                }

                cmd.CancelAsync();
                cancellationToken.ThrowIfCancellationRequested();
            }

            while (stdOutReader.ReadLine() is { } line)
            {
                stdOut.Add(line);
                if (this.EnableRaisingEvents)
                    this.OutputDataReceived?.Invoke(this, new SshDataReceivedEventArgs(line));
            }

            while (stdErrorReader.ReadLine() is { } line)
            {
                stdError.Add(line);
                if (this.EnableRaisingEvents)
                    this.ErrorDataReceived?.Invoke(this, new SshDataReceivedEventArgs(line));
            }
        }

        if (this.EnableRaisingEvents)
        {
            this.OutputDataReceived?.Invoke(this, new SshDataReceivedEventArgs(null));
            this.ErrorDataReceived?.Invoke(this, new SshDataReceivedEventArgs(null));
        }

        cmd.EndExecute(r);
        var end = DateTime.UtcNow;

        return new SshOutput()
        {
            Text = command,
            ExitCode = cmd.ExitStatus,
            StdOut = stdOut,
            StartedAt = start,
            ExitedAt = end,
            StdErr = stdError,
        };
    }

    public Task<SshOutput> RunAsync(string command, CancellationToken cancellationToken = default)
    {
        var task = new Task<SshOutput>(() => this.Run(command, cancellationToken), cancellationToken);
        task.Start();
        return task;
    }

    public void Dispose()
    {
        this.client.Dispose();
    }
}