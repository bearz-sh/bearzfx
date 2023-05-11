using Bearz.Std;

namespace Ze.Tasks.Messages;

public class TaskLogMessage : LogMessage
{
    public TaskLogMessage(string message, LogLevel level, ITask task)
        : base(message, level)
    {
        this.Task = task;
    }

    public TaskLogMessage(Error error, LogLevel level, ITask task)
        : base(error, level)
    {
        this.Task = task;
    }

    public TaskLogMessage(Exception exception, LogLevel level, ITask task)
        : base(exception, level)
    {
        this.Task = task;
    }

    public TaskLogMessage(string message, Exception exception, LogLevel level, ITask task)
        : base(message, exception, level)
    {
        this.Task = task;
    }

    public ITask Task { get; }
}