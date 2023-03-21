namespace Bearz.Diagnostics;

public interface IProcessCapture
{
    void OnNext(string value, System.Diagnostics.Process process);

    void OnComplete(System.Diagnostics.Process process);
}