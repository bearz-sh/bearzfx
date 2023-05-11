namespace Ze.Tasks;

public interface IMessageBus : IDisposable
{
    void Subscribe(IMessageSink sink);

    void Subscribe(Action<IMessage> capture);

    void Unsubscribe(Action<IMessage> capture);

    bool Publish(IMessage message);
}