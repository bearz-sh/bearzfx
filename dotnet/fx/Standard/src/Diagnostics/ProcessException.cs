using System.Runtime.Serialization;

namespace Bearz.Diagnostics;

[Serializable]
public class ProcessException : Exception
{
    public ProcessException()
    {
    }

    public ProcessException(int exitCode, string executable)
        : base($"Executable {executable} failed with ExitCode {exitCode}")
    {
        this.ExitCode = exitCode;
        this.FileName = executable;
    }

    public ProcessException(string message)
        : base(message)
    {
    }

    public ProcessException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected ProcessException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }

    public string? FileName { get; set; }

    public int ExitCode { get; set; }
}