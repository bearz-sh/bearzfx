using Bearz.Std;

namespace Plank.Tasks;

public interface IErrorMessage : IMessage
{
    Error Error { get; }

    Exception? Exception { get; }
}