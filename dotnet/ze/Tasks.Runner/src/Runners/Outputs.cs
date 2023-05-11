using System.Collections.Concurrent;

using Bearz.Secrets;
using Bearz.Text;

namespace Ze.Tasks.Internal;

public class Outputs : IOutputs
{
    private readonly ConcurrentDictionary<string, object?> data = new(StringComparer.OrdinalIgnoreCase);

    private readonly string id;

    private readonly string type;

    public Outputs(string id, string type, ISecretMasker masker)
    {
        this.type = type;
        this.id = id;
        this.Masker = masker;
    }

    public Outputs(string id, string type, ISecretMasker masker, IDictionary<string, object?> data)
    {
        this.type = type;
        this.id = id;
        this.Masker = masker;
        this.data = new ConcurrentDictionary<string, object?>(data, StringComparer.OrdinalIgnoreCase);
    }

    public Outputs(string id, string type, ISecretMasker masker, IOutputs outputs)
    {
        this.type = type;
        this.id = id;
        this.Masker = masker;
        this.data = new ConcurrentDictionary<string, object?>(outputs.ToDictionary(), StringComparer.OrdinalIgnoreCase);
    }

    private ISecretMasker Masker { get; }

    public object? this[string name]
    {
        get => this.Get(name);
        set => this.Set(name, value);
    }

    public object? Get(string name)
    {
        if (this.data.TryGetValue(name, out var value))
        {
            return value;
        }

        return null;
    }

    public void Set(string name, object? value)
        => this.Set(name, value, false);

    public void Set(string name, object? value, bool secret)
    {
        var sb = StringBuilderCache.Acquire();
        sb.Append(this.type).Append('.').Append(this.id).Append('.');
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c))
            {
                if (char.IsUpper(c))
                {
                    sb.Append(char.ToLower(c));
                    continue;
                }

                sb.Append(c);
            }

            sb.Append('_');
        }

        if (secret)
        {
            var str = value?.ToString();
            this.Masker.Add(str);
        }

        this.data[StringBuilderCache.GetStringAndRelease(sb)] = value;
    }

    public IDictionary<string, object?> ToDictionary()
        => this.data.ToDictionary(x => x.Key, x => x.Value);
}