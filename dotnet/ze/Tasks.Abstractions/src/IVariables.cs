namespace Ze.Tasks;

public interface IVariables
{
    object? this[string name] { get; }

    object? this[string name, object? defaultValue] { get; }

    bool TryGetValue(string name, out object? value);

    IDictionary<string, object?> ToDictionary();
}