using System.Diagnostics;

namespace Bearz.Diagnostics;

public sealed class ObserverCapture : IProcessCapture, IDisposable
{
    private readonly IObserver<string> observer;

    public ObserverCapture(IObserver<string> observer)
    {
        this.observer = observer;
    }

    public void OnNext(string value, Process process)
    {
        this.observer.OnNext(value);
    }

    public void OnComplete(Process process)
    {
        this.observer.OnCompleted();
    }

    public void Dispose()
    {
        this.observer.OnCompleted();
    }
}