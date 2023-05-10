namespace Plank.Tasks;

public interface ITaskMessage : IMessage
{
    ITask Task { get; }
}