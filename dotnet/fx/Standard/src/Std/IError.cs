using System.Runtime.Serialization;

namespace Bearz.Std;

public interface IError : IInnerError, IEnumerable<KeyValuePair<string, object?>>, ISerializable
{
    string? Message { get; }

    string? Target { get; }

    IList<IError>? Details { get; }

    object? this[string key] { get; }
}