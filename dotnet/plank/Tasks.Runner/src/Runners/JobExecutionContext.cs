using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Plank.Tasks.Internal;

public class JobExecutionContext : ExecutionContext, IJobExecutionContext
{
    public JobExecutionContext(string id, IServiceProvider services)
        : base(services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        this.Log = loggerFactory.CreateLogger(id);
        this.Outputs = new Outputs(id, "jobs", this.Env.SecretMasker);
    }

    public JobExecutionContext(string id, IExecutionContext context)
        : base(context)
    {
        var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
        this.Log = loggerFactory.CreateLogger(id);

        if (context is IActionExecutionContext tec)
        {
            this.Outputs = new Outputs(id, "jobs", this.Env.SecretMasker, tec.Outputs);
        }
        else
        {
            this.Outputs = new Outputs(id, "jobs", this.Env.SecretMasker);
        }
    }

    public JobExecutionContext(string id, IExecutionContext context, IDictionary<string, object?> data)
        : base(context)
    {
        var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
        this.Log = loggerFactory.CreateLogger(id);

        this.Outputs = new Outputs(id, "jobs", this.Env.SecretMasker, data);
    }

    public IOutputs Outputs { get; }
}