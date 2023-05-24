namespace Bearz.Extensions.Ssh;

public class SshOutput
{
    public string Text { get; set; } = string.Empty;

    public IReadOnlyList<string> StdOut { get; set; }

    public IReadOnlyList<string> StdErr { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? ExitedAt { get; set; }

    public int ExitCode { get; set; }
}