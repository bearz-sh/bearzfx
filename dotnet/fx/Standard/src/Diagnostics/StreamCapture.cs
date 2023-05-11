using System.Diagnostics;
using System.Text;

// ReSharper disable IntroduceOptionalParameters.Global
namespace Bearz.Diagnostics;

public class StreamCapture : IProcessCapture
{
    private readonly TextWriter writer;

    private readonly bool shouldDispose = true;

    public StreamCapture(TextWriter writer)
    {
        this.writer = writer;
        this.shouldDispose = false;
    }

    public StreamCapture(FileInfo fi)
        : this(fi.OpenWrite(), null, -1, false)
    {
    }

    public StreamCapture(FileInfo fi, bool leaveOpen)
        : this(fi.OpenWrite(), null, -1, leaveOpen)
    {
    }

    public StreamCapture(FileInfo fi, Encoding encoding)
        : this(fi.OpenWrite(), encoding, -1, false)
    {
    }

    public StreamCapture(FileStream stream, Encoding? encoding, int bufferSize, bool leaveOpen)
    {
        if (bufferSize < 1)
            bufferSize = 4096;

        this.writer = new StreamWriter(stream, encoding ?? Encoding.Default, bufferSize, leaveOpen);
    }

    public StreamCapture(Stream stream)
        : this(stream, null, -1, false)
    {
    }

    public StreamCapture(Stream stream, Encoding encoding)
        : this(stream, encoding, -1, false)
    {
    }

    public StreamCapture(Stream stream, Encoding encoding, int bufferSize)
        : this(stream, encoding, bufferSize, false)
    {
    }

    public StreamCapture(Stream stream, Encoding? encoding, int bufferSize, bool leaveOpen)
    {
        if (bufferSize < 1)
            bufferSize = 4096;

        this.writer = new StreamWriter(stream, encoding ?? Encoding.Default, bufferSize, leaveOpen);
    }

    public void OnNext(string value, Process process)
    {
        this.writer.WriteLine(value);
    }

    public void OnComplete(Process process)
    {
        this.writer.Flush();

        if (!this.shouldDispose)
            return;

        this.writer.Dispose();
    }
}