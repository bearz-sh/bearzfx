using System.Collections.Concurrent;

namespace Ze.Tasks.Messages;

// apache 2.0
// https://github.com/xunit/xunit/blob/77a3667654612e3ef7ba870f4e534c8ac7bebd13/src/xunit.v3.core/Internal/MessageBus.cs
public sealed class MessageBus : IMessageBus
{
    private readonly ConcurrentQueue<IMessage> queue = new();
    private readonly Thread dispatchThread;
    private readonly AutoResetEvent resetEvent = new(initialState: false);
    private readonly bool stopOnFail;
    private readonly List<Action<IMessage>> captures = new();
    private readonly ConcurrentBag<IMessageSink> sinks = new();
    private volatile bool shutdownRequested;
    private volatile bool continueRunning = true;
    private bool disposed;

    public MessageBus(IMessageSink messageSink, bool stopOnFail = false)
    {
        this.sinks.Add(messageSink);
        this.stopOnFail = stopOnFail;
        this.dispatchThread = new Thread(this.Run);
        this.dispatchThread.Start();
    }

    public void Dispose()
    {
        if (this.disposed)
            return;

        this.disposed = true;
        this.captures.Clear();
        foreach (var sink in this.sinks)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (sink is IDisposable disposable)
                disposable.Dispose();
        }

        this.shutdownRequested = true;
        this.resetEvent.Set();
        this.dispatchThread.Join();
        this.resetEvent.Dispose();
    }

    public void Subscribe(IMessageSink sink)
    {
        this.sinks.Add(sink);
    }

    public void Subscribe(Action<IMessage> capture)
    {
        this.captures.Add(capture);
    }

    public void Unsubscribe(Action<IMessage> capture)
    {
        this.captures.Remove(capture);
    }

    public bool Publish(IMessage message)
    {
        if (this.shutdownRequested)
            throw new ObjectDisposedException("MessageBus");

        if (this.stopOnFail && message is IErrorMessage)
            this.continueRunning = false;

        foreach (var capture in this.captures)
            capture(message);

        this.queue.Enqueue(message);
        this.resetEvent.Set();
        return this.continueRunning;
    }

    private void Dispatch()
    {
        while (this.queue.TryDequeue(out var message))
        {
            try
            {
                // ReSharper disable once NonAtomicCompoundOperator
                foreach (var sink in this.sinks)
                {
                    this.continueRunning &= sink.OnNext(message);
                    if (!this.continueRunning)
                        break;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    var errorMessage = new ErrorMessage(ex);
                    foreach (var sink in this.sinks)
                    {
                        if (!sink.OnNext(errorMessage))
                        {
                            this.continueRunning = false;
                            break;
                        }
                    }
                }
                catch
                {
                    // suppress all exceptions from failure to send message;
                }
            }
        }
    }

    private void Run()
    {
        while (!this.shutdownRequested)
        {
            this.resetEvent.WaitOne();
            this.Dispatch();
        }

        // One final dispatch pass
        this.Dispatch();
    }
}