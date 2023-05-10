using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Plank.Tasks.Internal;

public class WorkflowExecutionContext : ExecutionContext
{
    public WorkflowExecutionContext(IServiceProvider services)
        : base(services)
    {
        this.Log = services.GetRequiredService<ILogger<WorkflowExecutionContext>>();
    }
}