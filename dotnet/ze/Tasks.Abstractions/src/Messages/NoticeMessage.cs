namespace Ze.Tasks.Messages;

public struct NoticeMessage : IMessage
{
    public NoticeMessage(string text)
    {
        this.Text = text;
        this.CreatedAt = DateTimeOffset.UtcNow;
    }

    public string Text { get; }

    public DateTimeOffset CreatedAt { get; }

    public static implicit operator NoticeMessage(string text)
        => new(text);
}