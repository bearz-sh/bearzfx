namespace Plank.Tasks;

public interface IActionExecutionContext : IExecutionContext
{
    IOutputs Outputs { get; }
}