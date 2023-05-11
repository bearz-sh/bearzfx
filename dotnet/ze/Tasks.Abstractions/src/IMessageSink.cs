namespace Ze.Tasks;

public interface IMessageSink
{
    bool OnNext(IMessage message);
}