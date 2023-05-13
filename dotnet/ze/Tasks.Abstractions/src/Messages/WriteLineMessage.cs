namespace Ze.Tasks.Messages;

public struct WriteMessage : IMessage
{
    public WriteMessage(string text)
        => this.Text = text;

    public string Text { get; }

    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public static implicit operator WriteMessage(string text)
        => new(text);
}