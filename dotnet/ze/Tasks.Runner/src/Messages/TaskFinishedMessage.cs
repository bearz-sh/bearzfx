namespace Ze.Tasks.Messages;

public class TaskFinishedMessage : GroupEndMessage
{
    public TaskFinishedMessage(ITask task, TaskStatus status)
        : base(task.Name)
    {
        this.Task = task;
        this.Status = status;
    }

    public TaskStatus Status { get; }

    public ITask Task { get; }
}