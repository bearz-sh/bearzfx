using Renci.SshNet;
using Renci.SshNet.Common;

namespace Bearz.Extensions.Ssh;

public sealed class RenciScpClient : IDisposable
{
    private readonly ScpClient scpClient;

    public RenciScpClient(SshStartInfo startInfo)
    {
        if (startInfo.CachedConnection is ConnectionInfo info)
        {
            this.scpClient = new ScpClient(info);
            return;
        }

        this.scpClient = new ScpClient(startInfo.CacheConnection());
    }

    public event EventHandler<ScpUploadEventArgs> Uploading
    {
        add => this.scpClient.Uploading += value;
        remove => this.scpClient.Uploading -= value;
    }

    public event EventHandler<ScpDownloadEventArgs> Downloading
    {
        add => this.scpClient.Downloading += value;
        remove => this.scpClient.Downloading -= value;
    }

    public void UploadDirectory(string localPath, string remotePath)
    {
        var di = new DirectoryInfo(localPath);
        if (!di.Exists)
            throw new DirectoryNotFoundException($"Directory '{localPath}' is missing.");

        this.scpClient.Upload(di, remotePath);
    }

    public Task UploadDirectoryAsync(string localPath, string remotePath, CancellationToken cancellationToken = default)
    {
        var task = new Task(() => this.UploadDirectory(localPath, remotePath), cancellationToken);
        task.Start();
        return task;
    }

    public void UploadFile(string localPath, string remotePath)
    {
        var fi = new FileInfo(localPath);
        if (!fi.Exists)
            throw new FileNotFoundException($"File '{localPath}' is missing.");

        this.scpClient.Upload(fi, remotePath);
    }

    public void UploadFile(Stream source, string remotePath)
    {
        this.scpClient.Upload(source, remotePath);
    }

    public Task UploadFileAsync(string localPath, string remotePath, CancellationToken cancellationToken = default)
    {
        var task = new Task(() => this.UploadFile(localPath, remotePath), cancellationToken);
        task.Start();
        return task;
    }

    public Task UploadFileAsync(Stream source, string remotePath, CancellationToken cancellationToken = default)
    {
        var task = new Task(() => this.UploadFile(source, remotePath), cancellationToken);
        task.Start();
        return task;
    }

    public void UploadFiles(IEnumerable<string> localPaths, string remotePath)
    {
        var files = new List<FileInfo>();
        foreach (var localPath in localPaths)
        {
            var fi = new FileInfo(localPath);
            if (!fi.Exists)
                throw new FileNotFoundException($"File '{localPath}' is missing.");

            files.Add(fi);
        }

        foreach (var fi in files)
        {
            this.scpClient.Upload(fi, remotePath);
        }
    }

    public Task UploadFilesAsync(IEnumerable<string> localPaths, string remotePath, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var files = new List<FileInfo>();
        foreach (var localPath in localPaths)
        {
            var fi = new FileInfo(localPath);
            if (!fi.Exists)
                throw new FileNotFoundException($"File '{localPath}' is missing.");

            files.Add(fi);
        }

        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var task = new Task(
            () =>
            {
                foreach (var fi in files)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    this.scpClient.Upload(fi, remotePath);
                }
            },
            cancellationToken);
        task.Start();
        return task;
    }

    public void DownloadDirectory(string remotePath, string localPath)
    {
        var di = new DirectoryInfo(localPath);
        if (!di.Exists)
            di.Create();

        this.scpClient.Download(remotePath, di);
    }

    public Task DownloadDirectoryAsync(string remotePath, string localPath, CancellationToken cancellationToken = default)
    {
        var task = new Task(() => this.DownloadDirectory(remotePath, localPath), cancellationToken);
        task.Start();
        return task;
    }

    public void DownloadFile(string remotePath, string localPath)
    {
        var fi = new FileInfo(localPath);
        if (!fi.Exists)
            fi.Create().Dispose();

        this.scpClient.Download(remotePath, fi);
    }

    public void DownloadFile(string remotePath, Stream destination)
    {
        this.scpClient.Download(remotePath, destination);
    }

    public Task DownloadFileAsync(string remotePath, string localPath, CancellationToken cancellationToken = default)
    {
        var task = new Task(() => this.DownloadFile(remotePath, localPath), cancellationToken);
        task.Start();
        return task;
    }

    public Task DownloadFileAsync(string remotePath, Stream destination, CancellationToken cancellationToken = default)
    {
        var task = new Task(() => this.DownloadFile(remotePath, destination), cancellationToken);
        task.Start();
        return task;
    }

    public void Dispose()
    {
        this.scpClient.Dispose();
    }
}