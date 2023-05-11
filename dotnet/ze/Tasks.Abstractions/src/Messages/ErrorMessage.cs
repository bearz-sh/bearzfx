using System.Diagnostics;

using Bearz.Std;

namespace Ze.Tasks.Messages;

public class ErrorMessage : Message, IErrorMessage
{
    public ErrorMessage(string message)
        : base(message)
    {
        this.Error = new Error(message) { Code = "ErrorMessage", };
    }

    public ErrorMessage(Exception exception)
        : base(exception.Message)
    {
        this.Exception = exception;
        var trace = new StackTrace(exception);
        var frame = trace.GetFrame(0);
        if (frame is not null)
        {
            this.File = frame.GetFileName();
            this.Column = frame.GetFileColumnNumber();
            this.Line = frame.GetFileLineNumber();
        }

        this.Error = (Error)exception;
    }

    public ErrorMessage(Error error)
        : base(error.Message)
    {
        this.Error = error;
    }

    public Error Error { get; }

    public Exception? Exception { get; }

    public string? File { get; set; }

    public int? Line { get; set; }

    public int? Column { get; set; }
}