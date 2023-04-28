using System.Collections;
using System.Runtime.Serialization;

namespace Bearz.Std;

public struct Error : IError, IEquatable<Error>
{
    private Dictionary<string, object?>? map = null;

    public Error()
    {
    }

    public Error(string message)
    {
        this.Message = message;
    }

    private Error(SerializationInfo info, StreamingContext context)
    {
        this.Message = info.GetString(nameof(this.Message)) ?? string.Empty;
        this.Target = info.GetString(nameof(this.Target));
        this.Code = info.GetString(nameof(this.Code));

        if (info.GetValue(nameof(this.Details), typeof(IError[])) is IError[] internalErrors)
            this.Details = new List<IError>(internalErrors);

        if (info.GetValue(nameof(this.InnerError), typeof(IInnerError)) is IInnerError error)
            this.InnerError = error;

        if (info.GetValue("Properties", typeof(Dictionary<string, object?>)) is Dictionary<string, object?> properties)
            this.map = properties;
    }

    public string Message { get; set; } = string.Empty;

    public string? Target { get; set; } = null;

    public IList<IError>? Details { get; set; } = null;

    public string? Code { get; set; }

    public IInnerError? InnerError { get; set; } = null;

    public object? this[string key]
    {
        get
        {
            this.map ??= new(StringComparer.OrdinalIgnoreCase);
            if (this.map.TryGetValue(key, out var value))
                return value;

            return null;
        }

        set
        {
            this.map ??= new Dictionary<string, object?>();
            this.map[key] = value;
        }
    }

    public static implicit operator Error(Exception exception)
        => ErrorConverter.ConvertToError(exception);

    public static bool operator ==(Error left, Error right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Error left, Error right)
    {
        return !left.Equals(right);
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        if (this.map is not null)
        {
            foreach (var kvp in this.map)
                yield return kvp;
        }
        else
        {
            yield break;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(this.Message), this.Message);
        info.AddValue(nameof(this.Target), this.Target);
        info.AddValue(nameof(this.Code), this.Code);
        info.AddValue(nameof(this.Details), this.Details?.ToArray(), typeof(IError[]));
        info.AddValue(nameof(this.InnerError), this.InnerError, typeof(IInnerError));
        info.AddValue("Properties", this.map, typeof(Dictionary<string, object?>));
    }

    public bool Equals(Error other)
    {
        return Equals(this.map, other.map) &&
               this.Message == other.Message &&
               this.Target == other.Target &&
               Equals(this.Details, other.Details) &&
               this.Code == other.Code &&
               Equals(this.InnerError, other.InnerError);
    }

    public override bool Equals(object? obj)
    {
        return obj is Error other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            this.Message, this.Code, this.Target, this.InnerError);
    }
}