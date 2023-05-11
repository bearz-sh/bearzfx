namespace Ze.Tasks;

public interface IIdGenerator
{
    string FromName(ReadOnlySpan<char> name);

    string Generate();
}