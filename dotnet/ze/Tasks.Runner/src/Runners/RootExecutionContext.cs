using ExecutionContext = Ze.Tasks.Internal.ExecutionContext;

namespace Ze.Tasks.Runner.Runners;

public class RootExecutionContext : ExecutionContext
{
    public RootExecutionContext(IServiceProvider services)
        : base(services)
    {
    }
}