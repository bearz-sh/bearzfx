using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Bearz.Diagnostics;
using Bearz.Secrets;
using Bearz.Virtual;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ze.Tasks.Runner.Yaml;

public class CommandCapture : IProcessCapture
{
    private static readonly char[] CommandSeparator = new[] { ':', ':' };

    private readonly ITaskExecutionContext taskExecutionContext;

    public CommandCapture(ITaskExecutionContext taskContext)
    {
        this.taskExecutionContext = taskContext;
    }

    public void OnNext(string value, Process process)
    {
        if (value[0] is ':' && value[1] is ':')
        {
            var segments = value.Split(CommandSeparator, StringSplitOptions.RemoveEmptyEntries);
            var command = segments[0];

            switch (command)
            {
                case "add-mask":
                    this.taskExecutionContext.SecretMasker.Add(segments[1]);
                    return;

                case "debug":
                    this.taskExecutionContext.Debug(segments[1]);

                    return;

                case "notice":
                    this.taskExecutionContext.Notice(segments[1]);
                    return;

                case "command":
                    this.taskExecutionContext.Command(segments[1]);
                    return;

                case "error":
                    this.taskExecutionContext.Error(segments[1]);
                    return;

                case "warning":
                    this.taskExecutionContext.Warn(segments[1]);
                    return;

                case "group":
                    this.taskExecutionContext.StartGroup(segments[1]);
                    return;

                case "endgroup":
                    this.taskExecutionContext.EndGroup(segments[1]);
                    return;
            }
        }

        this.taskExecutionContext.WriteLine(value);
    }

    [SuppressMessage("Roslynator", "RCS1163:Unused parameter.")]
    public void OnComplete(Process process)
    {
        // do nothing
    }
}