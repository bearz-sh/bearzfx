using Microsoft.Extensions.Logging;

using Ze.Tasks.Messages;

using LogLevel = Ze.Tasks.Messages.LogLevel;

namespace Ze.Tasks.Runner.Runners;

public class LoggerMessageSink : IMessageSink
{
    private readonly ILogger log;

    public LoggerMessageSink(ILogger<LoggerMessageSink> log)
    {
        this.log = log;
    }

    public bool OnNext(IMessage message)
    {
        if (message is LogMessage taskLogMessage)
        {
            var melLevel = Microsoft.Extensions.Logging.LogLevel.Information;
            var level = taskLogMessage.Level;

            // map log levels
            switch (level)
            {
                case LogLevel.Command:
                    break;

                case LogLevel.Debug:
                    melLevel = Microsoft.Extensions.Logging.LogLevel.Debug;
                    break;

                case LogLevel.Error:
                    melLevel = Microsoft.Extensions.Logging.LogLevel.Error;
                    break;

                case LogLevel.Warning:
                    melLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
                    break;
                case LogLevel.Info:
                    melLevel = Microsoft.Extensions.Logging.LogLevel.Information;
                    break;
                case LogLevel.Trace:
                    melLevel = Microsoft.Extensions.Logging.LogLevel.Trace;
                    break;
                default:
                    melLevel = Microsoft.Extensions.Logging.LogLevel.Information;
                    break;
            }

            this.log.Log(melLevel, taskLogMessage.Exception, taskLogMessage.Text);
            return false;
        }

        if (message is GroupStartMessage startMessage)
        {
            this.log.LogInformation("Task {GroupName}", startMessage.Text);
            Console.WriteLine();
            return true;
        }

        if (message is GroupEndMessage)
        {
            return false;
        }

        this.log.LogInformation(message.Text);
        return false;
    }
}