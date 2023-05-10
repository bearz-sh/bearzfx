namespace Plank.Tasks.Messages;

public class TaskFinishedMessage : Message
{
    public TaskFinishedMessage(ITask task, TaskStatus status)
        : base($"Task {task.Name} {status.ToString().ToLower()}")
    {
        this.Task = task;
        this.Status = status;
    }

    public TaskStatus Status { get; }

    public ITask Task { get; }
}