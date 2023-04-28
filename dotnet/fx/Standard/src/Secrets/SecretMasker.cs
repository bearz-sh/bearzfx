using System.Diagnostics.CodeAnalysis;

using Bearz.Extra.Strings;

namespace Bearz.Secrets;

public class SecretMasker : ISecretMasker
{
    public static ISecretMasker Default { get; } = new SecretMasker();

    protected List<ReadOnlyMemory<char>> Secrets { get; } = new();

    protected List<Func<ReadOnlyMemory<char>, ReadOnlyMemory<char>>> Generators { get; } = new();

    public void Add(string? secret)
    {
        if (string.IsNullOrWhiteSpace(secret))
            return;

        var memory = secret.AsMemory();

        // don't exit method as there may be new generators.
        if (!this.Secrets.Contains(memory))
            this.Secrets.Add(memory);

        foreach (var generator in this.Generators)
        {
            var next = generator(memory);

            if (!this.Secrets.Contains(next))
                this.Secrets.Add(next);
        }
    }

    public void AddDerivativeGenerator(Func<ReadOnlyMemory<char>, ReadOnlyMemory<char>> generator)
    {
        this.Generators.Add(generator);
    }

    public ReadOnlySpan<char> Mask(ReadOnlySpan<char> value)
    {
        if (this.Secrets.Count == 0 || value.IsEmpty || value.IsWhiteSpace())
            return value;

        return value.SearchAndReplace(this.Secrets, "**********".AsSpan(), StringComparison.OrdinalIgnoreCase);
    }

    [return: NotNullIfNotNull("value")]
    public string? Mask(string? value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value))
            return value;

        if (this.Secrets.Count == 0)
            return value;

        return value.AsSpan()
            .SearchAndReplace(
                this.Secrets,
                "**********".AsSpan(),
                StringComparison.OrdinalIgnoreCase)
            .AsString();
    }
}