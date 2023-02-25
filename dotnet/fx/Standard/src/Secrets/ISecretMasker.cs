namespace Bearz.Secrets;

public interface ISecretMasker : IMask
{
    void Add(string? secret);

    void AddDerivativeGenerator(Func<ReadOnlyMemory<char>, ReadOnlyMemory<char>> generator);
}