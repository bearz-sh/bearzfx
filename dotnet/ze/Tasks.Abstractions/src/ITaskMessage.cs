namespace Ze.Tasks;

public interface ITaskMessage : IMessage
{
    ITask Task { get; }
}