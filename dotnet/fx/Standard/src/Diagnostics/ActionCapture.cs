namespace Bearz.Diagnostics;

public class ActionCapture : IProcessCapture
{
    private readonly Action<string> action;

    public ActionCapture(Action<string> action)
        => this.action = action;

    public void WriteLine(string value)
        => this.action(value);
}