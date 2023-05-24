namespace Bearz.Extensions.Ssh;

public class SshDataReceivedEventArgs : EventArgs
{
    public SshDataReceivedEventArgs(string? data)
    {
        this.Data = data;
    }

    public string? Data { get; }
}