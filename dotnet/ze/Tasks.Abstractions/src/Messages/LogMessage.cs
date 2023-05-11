using System.Diagnostics;

using Bearz.Std;

namespace Ze.Tasks.Messages;

public class LogMessage : Message
{
    public LogMessage(string message, LogLevel level)
        : base(message)
    {
        this.Level = level;
    }

    public LogMessage(Error error, LogLevel level)
        : base(error.Message)
    {
        this.Level = level;
        this.Error = error;
    }

    public LogMessage(Exception exception, LogLevel level)
        : this(exception.Message, exception, level)
    {
    }

    public LogMessage(string message, Exception exception, LogLevel level)
        : base(message)
    {
        this.Level = level;
        this.Error = exception;
        this.Exception = exception;

        var stack = new StackTrace(exception);
        var frame = stack.GetFrame(0);
        if (frame is null)
            return;

        this.File = frame.GetFileName();
        this.Line = frame.GetFileLineNumber();
        this.Column = frame.GetFileColumnNumber();
    }

    public Dictionary<string, object?>? Data { get; set; }

    public Exception? Exception { get; set; }

    public Error? Error { get; set; }

    public LogLevel Level { get;  }

    public string? File { get; set; }

    public int? Line { get; set; }

    public int? Column { get; set; }
}