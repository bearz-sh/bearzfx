namespace Cocoa.Adapters;

public class CocoaDateTime : IDateTime
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
}