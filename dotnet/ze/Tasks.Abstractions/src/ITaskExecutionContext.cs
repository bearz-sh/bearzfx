namespace Ze.Tasks;

public interface ITaskExecutionContext : IActionExecutionContext
{
    ITask Task { get; }

    TaskStatus Status { get; set; }
}