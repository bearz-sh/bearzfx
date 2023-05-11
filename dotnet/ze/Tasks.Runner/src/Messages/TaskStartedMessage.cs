namespace Ze.Tasks.Messages;

public class TaskStartedMessage : GroupStartMessage
{
    public TaskStartedMessage(ITask task)
        : base(task.Name)
    {
        this.Task = task;
    }

    public ITask Task { get; }
}