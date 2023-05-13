using Bearz.Std;

using Ze.Tasks.Messages;

namespace Ze.Tasks;

public static class TaskExecutionExtensions
{
    public static void Skip(this ITaskExecutionContext ctx)
    {
        ctx.Status = TaskStatus.Skipped;
        ctx.Bus.Publish(new TaskSkippedMessage(ctx.Task));
    }

    public static void Log(this ITaskExecutionContext ctx, TaskLogMessage message)
    {
        ctx.Bus.Publish(message);
    }

    public static void Log(this ITaskExecutionContext ctx, string message, LogLevel level)
    {
        ctx.Bus.Publish(new TaskLogMessage(message, level, ctx.Task));
    }

    public static void Log(this ITaskExecutionContext ctx, string message, LogLevel level, Dictionary<string, object?> data)
    {
        ctx.Bus.Publish(new TaskLogMessage(message, level, ctx.Task)
        {
            Data = data,
        });
    }

    public static void Log(this ITaskExecutionContext ctx, Error error, LogLevel level)
    {
        ctx.Bus.Publish(new TaskLogMessage(error, level, ctx.Task));
    }

    public static void Log(this ITaskExecutionContext ctx, string message, Exception exception, LogLevel level)
    {
        ctx.Bus.Publish(new TaskLogMessage(message, exception, level, ctx.Task));
    }

    public static void Log(this ITaskExecutionContext ctx, Exception exception, LogLevel level)
    {
        ctx.Bus.Publish(new TaskLogMessage(exception, level, ctx.Task));
    }

    public static void WriteLine(this ITaskExecutionContext ctx, WriteMessage message)
    {
        ctx.Bus.Publish(message);
    }

    public static void Notice(this ITaskExecutionContext ctx, NoticeMessage message)
    {
        ctx.Bus.Publish(message);
    }

    public static void Command(this ITaskExecutionContext ctx, string message)
    {
        Log(ctx, message, LogLevel.Command);
    }

    public static void Command(this ITaskExecutionContext ctx, string message, Dictionary<string, object?> data)
    {
        Log(ctx, message, LogLevel.Command, data);
    }

    public static void Command(this ITaskExecutionContext ctx, string message, params object?[] args)
    {
        message = string.Format(message, args);
        Log(ctx, message, LogLevel.Command);
    }

    public static void Command(this ITaskExecutionContext ctx, string message, Exception exception)
    {
        Log(ctx, message, exception, LogLevel.Command);
    }

    public static void Command(this ITaskExecutionContext ctx, Exception exception)
    {
        Log(ctx, exception, LogLevel.Command);
    }

    public static void Command(this ITaskExecutionContext ctx, Error error)
    {
        Log(ctx, error, LogLevel.Command);
    }

    public static void Debug(this ITaskExecutionContext ctx, string message)
    {
        Log(ctx, message, LogLevel.Debug);
    }

    public static void Debug(this ITaskExecutionContext ctx, string message, Dictionary<string, object?> data)
    {
        Log(ctx, message, LogLevel.Debug, data);
    }

    public static void Debug(this ITaskExecutionContext ctx, string message, params object?[] args)
    {
        message = string.Format(message, args);
        Log(ctx, message, LogLevel.Debug);
    }

    public static void Debug(this ITaskExecutionContext ctx, string message, Exception exception)
    {
        Log(ctx, message, exception, LogLevel.Debug);
    }

    public static void Debug(this ITaskExecutionContext ctx, Exception exception)
    {
        Log(ctx, exception, LogLevel.Debug);
    }

    public static void Debug(this ITaskExecutionContext ctx, Error error)
    {
        Log(ctx, error, LogLevel.Command);
    }

    public static void EndGroup(this ITaskExecutionContext ctx, string name)
    {
        ctx.Bus.Publish(new GroupStartMessage(name));
    }

    public static void Error(this ITaskExecutionContext ctx, string message)
    {
        ctx.Status = TaskStatus.Failed;
        Log(ctx, message, LogLevel.Error);
    }

    public static void Error(this ITaskExecutionContext ctx, string message, Dictionary<string, object?> data)
    {
        ctx.Status = TaskStatus.Failed;
        Log(ctx, message, LogLevel.Error, data);
    }

    public static void Error(this ITaskExecutionContext ctx, string message, params object?[] args)
    {
        ctx.Status = TaskStatus.Failed;
        message = string.Format(message, args);
        Log(ctx, message, LogLevel.Error);
    }

    public static void Error(this ITaskExecutionContext ctx, string message, Exception exception)
    {
        ctx.Status = TaskStatus.Failed;
        Log(ctx, message, exception, LogLevel.Error);
    }

    public static void Error(this ITaskExecutionContext ctx, Exception exception)
    {
        ctx.Status = TaskStatus.Failed;
        Log(ctx, exception, LogLevel.Error);
    }

    public static void Error(this ITaskExecutionContext ctx, Error error)
    {
        ctx.Status = TaskStatus.Failed;
        Log(ctx, error, LogLevel.Error);
    }

    public static void Info(this ITaskExecutionContext ctx, string message)
    {
        Log(ctx, message, LogLevel.Info);
    }

    public static void Info(this ITaskExecutionContext ctx, string message, Dictionary<string, object?> data)
    {
        Log(ctx, message, LogLevel.Info, data);
    }

    public static void Info(this ITaskExecutionContext ctx, string message, params object?[] args)
    {
        message = string.Format(message, args);
        Log(ctx, message, LogLevel.Info);
    }

    public static void Info(this ITaskExecutionContext ctx, string message, Exception exception)
    {
        Log(ctx, message, exception, LogLevel.Info);
    }

    public static void Info(this ITaskExecutionContext ctx, Exception exception)
    {
        Log(ctx, exception, LogLevel.Info);
    }

    public static void Info(this ITaskExecutionContext ctx, Error error)
    {
        Log(ctx, error, LogLevel.Info);
    }

    public static void StartGroup(this ITaskExecutionContext ctx, string name)
    {
        ctx.Bus.Publish(new GroupStartMessage(name));
    }

    public static void Warn(this ITaskExecutionContext ctx, string message)
    {
        Log(ctx, message, LogLevel.Warning);
    }

    public static void Warn(this ITaskExecutionContext ctx, string message, Dictionary<string, object?> data)
    {
        Log(ctx, message, LogLevel.Warning, data);
    }

    public static void Warn(this ITaskExecutionContext ctx, string message, params object?[] args)
    {
        message = string.Format(message, args);
        Log(ctx, message, LogLevel.Warning);
    }

    public static void Warn(this ITaskExecutionContext ctx, string message, Exception exception)
    {
        Log(ctx, message, exception, LogLevel.Warning);
    }

    public static void Warn(this ITaskExecutionContext ctx, Exception exception)
    {
        Log(ctx, exception, LogLevel.Warning);
    }

    public static void Warn(this ITaskExecutionContext ctx, Error error)
    {
        Log(ctx, error, LogLevel.Warning);
    }

    public static void Trace(this ITaskExecutionContext ctx, string message)
    {
        Log(ctx, message, LogLevel.Trace);
    }

    public static void Trace(this ITaskExecutionContext ctx, string message, Dictionary<string, object?> data)
    {
        Log(ctx, message, LogLevel.Trace, data);
    }

    public static void Trace(this ITaskExecutionContext ctx, string message, params object?[] args)
    {
        message = string.Format(message, args);
        Log(ctx, message, LogLevel.Trace);
    }

    public static void Trace(this ITaskExecutionContext ctx, string message, Exception exception)
    {
        Log(ctx, message, exception, LogLevel.Trace);
    }

    public static void Trace(this ITaskExecutionContext ctx, Exception exception)
    {
        Log(ctx, exception, LogLevel.Trace);
    }

    public static void Trace(this ITaskExecutionContext ctx, Error error)
    {
        Log(ctx, error, LogLevel.Trace);
    }
}