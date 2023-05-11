namespace Ze.Tasks.Messages;

public class WarningMessage : Message
{
    public WarningMessage(string text)
        : base(text)
    {
    }

    public string? File { get; set; }

    public int? Column { get; set; }

    public int? Line { get; set; }
}