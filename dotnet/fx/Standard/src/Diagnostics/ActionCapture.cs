using System.Diagnostics;

namespace Bearz.Diagnostics;

public class ActionCapture : IProcessCapture
{
    private readonly Action<string, Process> action;

    private readonly Action<Process>? onComplete;

    public ActionCapture(Action<string, Process> action, Action<Process>? onComplete = null)
    {
        this.action = action;
        this.onComplete = onComplete;
    }

    public void OnNext(string value, Process process)
        => this.action(value, process);

    public void OnComplete(Process process)
        => this.onComplete?.Invoke(process);
}