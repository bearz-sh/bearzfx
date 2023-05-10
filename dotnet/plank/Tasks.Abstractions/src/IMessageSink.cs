namespace Plank.Tasks;

public interface IMessageSink
{
    bool OnNext(IMessage message);
}