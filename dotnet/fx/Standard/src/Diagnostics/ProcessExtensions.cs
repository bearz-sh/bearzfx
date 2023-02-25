using System.Diagnostics;
using System.Text;

namespace Bearz.Diagnostics;

public static class ProcessExtensions
{
    public static Process Capture(
        this Process process,
        ICollection<string> standardOut,
        ICollection<string> standardError)
    {
        process.RedirectTo(standardOut);
        process.RedirectTo(standardError);
        return process;
    }

    public static Process Run(this Process process)
    {
        process.Start();
        process.WaitForExit();
        return process;
    }

    public static Process Run(this Process process, int timeout)
    {
        process.Start();
        process.WaitForExit(timeout);
        if (!process.HasExited)
        {
            throw new TimeoutException(
                $"Process {process.Id} {process.StartInfo.FileName} has exceeded the timeout of {timeout} ms.");
        }

        return process;
    }

    public static async Task<Process> RunAsync(this Process process, CancellationToken cancellationToken = default)
    {
        process.Start();
        await process.WaitForExitAsync(cancellationToken)
            .ConfigureAwait(false);

        return process;
    }

    public static Process ValidateExitCode(this Process process, Func<int, bool>? validate = null)
    {
        if (!process.HasExited)
            return process;

        validate ??= code => code == 0;
        if (!validate(process.ExitCode))
            throw new ProcessException(process.ExitCode, process.StartInfo.FileName);

        return process;
    }

    public static Process Tee(this Process process, string fileName, bool append = false)
    {
        process.RedirectTo(Console.Out);
        process.RedirectErrorTo(Console.Error);
        var fw = new StreamWriter(fileName, append);

        var streamCapture = new StreamCapture(fw);
        process.RedirectTo(streamCapture);
        process.RedirectErrorTo(streamCapture);
        process.Exited += (sender, args) => { fw.Dispose(); };

        return process;
    }

    public static Process RedirectTo(this Process process, Action<string> action)
        => RedirectTo(process, new ActionCapture(action));

    public static Process RedirectTo(this Process process, ICollection<string> lines)
        => RedirectTo(process, new CollectionCapture(lines));

    public static Process RedirectTo(this Process process, TextWriter writer)
        => RedirectTo(process, new StreamCapture(writer));

    public static Process RedirectTo(this Process process, FileInfo fileInfo)
        => RedirectTo(process, new StreamCapture(fileInfo));

    public static Process RedirectTo(this Process process, FileInfo fileInfo, Encoding encoding)
        => RedirectTo(process, new StreamCapture(fileInfo, encoding));

    public static Process RedirectTo(this Process process, FileInfo fileInfo, bool leaveOpen)
        => RedirectTo(process, new StreamCapture(fileInfo, leaveOpen));

    public static Process RedirectTo(this Process process, Stream stream)
        => RedirectTo(process, new StreamCapture(stream));

    public static Process RedirectTo(this Process process, Stream stream, Encoding encoding)
        => RedirectTo(process, new StreamCapture(stream, encoding));

    public static Process RedirectTo(this Process process, Stream stream, Encoding encoding, int bufferSize)
        => RedirectTo(process, new StreamCapture(stream, encoding, bufferSize));

    public static Process RedirectTo(
        this Process process,
        Stream stream,
        Encoding? encoding,
        int bufferSize,
        bool leaveOpen)
        => RedirectTo(process, new StreamCapture(stream, encoding, bufferSize, leaveOpen));

    public static Process RedirectTo(this Process process, IProcessCapture capture)
    {
        var si = process.StartInfo;
        si.RedirectStandardOutput = true;
        si.UseShellExecute = false;
        si.CreateNoWindow = true;

        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data == null)
            {
                if (capture is IDisposable disposable)
                    disposable.Dispose();

                return;
            }

            capture.WriteLine(args.Data);
        };

        return process;
    }

    public static Process RedirectErrorTo(this Process process, Action<string> action)
        => RedirectErrorTo(process, new ActionCapture(action));

    public static Process RedirectErrorTo(this Process process, ICollection<string> lines)
        => RedirectErrorTo(process, new CollectionCapture(lines));

    public static Process RedirectErrorTo(this Process process, TextWriter writer)
        => RedirectErrorTo(process, new StreamCapture(writer));

    public static Process RedirectErrorTo(this Process process, FileInfo fileInfo)
        => RedirectErrorTo(process, new StreamCapture(fileInfo));

    public static Process RedirectErrorTo(this Process process, FileInfo fileInfo, Encoding encoding)
        => RedirectErrorTo(process, new StreamCapture(fileInfo, encoding));

    public static Process RedirectErrorTo(this Process process, FileInfo fileInfo, bool leaveOpen)
        => RedirectErrorTo(process, new StreamCapture(fileInfo, leaveOpen));

    public static Process RedirectErrorTo(this Process process, Stream stream)
        => RedirectErrorTo(process, new StreamCapture(stream));

    public static Process RedirectErrorTo(this Process process, Stream stream, Encoding encoding)
        => RedirectErrorTo(process, new StreamCapture(stream, encoding));

    public static Process RedirectErrorTo(this Process process, Stream stream, Encoding encoding, int bufferSize)
        => RedirectErrorTo(process, new StreamCapture(stream, encoding, bufferSize));

    public static Process RedirectErrorTo(
        this Process process,
        Stream stream,
        Encoding? encoding,
        int bufferSize,
        bool leaveOpen)
        => RedirectErrorTo(process, new StreamCapture(stream, encoding, bufferSize, leaveOpen));

    public static Process RedirectErrorTo(this Process process, IProcessCapture capture)
    {
        var si = process.StartInfo;
        si.RedirectStandardError = true;
        si.UseShellExecute = false;
        si.CreateNoWindow = true;

        process.EnableRaisingEvents = true;
        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data == null)
            {
                if (capture is IDisposable disposable)
                    disposable.Dispose();

                return;
            }

            capture.WriteLine(args.Data);
        };

        return process;
    }

#if NETLEGACY
    /// <summary>
    /// Waits asynchronously for the process to exit.
    /// </summary>
    /// <param name="process">The process to wait for cancellation.</param>
    /// <param name="cancellationToken">
    /// A cancellation token. If invoked, the task will return
    /// immediately as canceled.</param>
    /// <returns>A Task representing waiting for the process to end.</returns>
    public static Task WaitForExitAsync(
        this Process process,
        CancellationToken cancellationToken = default)
    {
        if (process.HasExited)
            return Task.CompletedTask;

        var tcs = new TaskCompletionSource<object>();
        process.EnableRaisingEvents = true;

        process.Exited += (sender, args) => tcs.TrySetResult(true);
        if (cancellationToken != default)
            cancellationToken.Register(() => tcs.SetCanceled());

        return process.HasExited ? Task.CompletedTask : tcs.Task;
    }
#endif
}