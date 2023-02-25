namespace Bearz.Std;

public interface IInnerError
{
    string? Code { get; }

    IInnerError? InnerError { get; }
}