using ExecutionContext = Plank.Tasks.Internal.ExecutionContext;

namespace Plank.Tasks.Runner.Runners;

public class RootExecutionContext : ExecutionContext
{
    public RootExecutionContext(IServiceProvider services)
        : base(services)
    {
    }
}