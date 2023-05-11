using Bearz.Std;

namespace Ze.Tasks;

public interface IErrorMessage : IMessage
{
    Error Error { get; }

    Exception? Exception { get; }
}