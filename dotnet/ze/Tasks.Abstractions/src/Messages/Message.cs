namespace Ze.Tasks.Messages;

public class Message : IMessage
{
    public Message(string text)
    {
        this.Text = text;
    }

    public string Text { get; }

    public virtual DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}