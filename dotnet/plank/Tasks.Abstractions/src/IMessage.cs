namespace Plank.Tasks;

public interface IMessage
{
    string Text { get; }

    DateTimeOffset CreatedAt { get; }
}