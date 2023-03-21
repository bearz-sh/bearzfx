namespace Bearz.Std;

public class EnvVarParseTokenException : BearzException
{
    public EnvVarParseTokenException()
        : base()
    {
    }

    public EnvVarParseTokenException(string message)
        : base(message)
    {
    }

    public EnvVarParseTokenException(string message, Exception inner)
        : base(message, inner)
    {
    }
}