namespace Ze.Tasks.Runners;

public class TaskRunnerResult
{
    public TaskRunnerStatus Status { get; protected internal set; }

    public static TaskRunnerResult Success()
    {
        return new TaskRunnerResult() { Status = TaskRunnerStatus.Success, };
    }

    public static TaskRunnerResult Failed()
    {
        return new TaskRunnerResult() { Status = TaskRunnerStatus.Failed };
    }

    public static TaskRunnerResult Cancelled()
    {
        return new TaskRunnerResult() { Status = TaskRunnerStatus.Cancelled };
    }
}