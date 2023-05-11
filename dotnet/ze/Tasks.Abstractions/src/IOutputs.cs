namespace Ze.Tasks;

public interface IOutputs
{
    object? this[string name] { get; set; }

    object? Get(string name);

    void Set(string name, object? value);

    void Set(string name, object? value, bool secret);

    IDictionary<string, object?> ToDictionary();
}