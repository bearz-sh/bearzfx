namespace Ze.Tasks;

public interface IActionExecutionContext : IExecutionContext
{
    IOutputs Outputs { get; }
}