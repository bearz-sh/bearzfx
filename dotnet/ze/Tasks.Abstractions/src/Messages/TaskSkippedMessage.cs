namespace Ze.Tasks.Messages;

public class TaskSkippedMessage : Message, ITaskMessage
{
    public TaskSkippedMessage(ITask task)
        : base("Task skipped {task.Name")
    {
        this.Task = task;
    }

    public ITask Task { get;  }
}