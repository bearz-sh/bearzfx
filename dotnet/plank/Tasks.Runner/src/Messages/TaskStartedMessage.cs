namespace Plank.Tasks.Messages;

public class TaskStartedMessage : Message
{
    public TaskStartedMessage(ITask task)
        : base($"Task {task.Name} started")
    {
        this.Task = task;
    }

    public ITask Task { get; }
}