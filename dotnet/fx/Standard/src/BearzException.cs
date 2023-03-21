namespace Bearz;

public class BearzException : SystemException
{
    public BearzException()
    {
    }

    public BearzException(string message)
        : base(message)
    {
    }

    public BearzException(string message, Exception inner)
        : base(message, inner)
    {
    }
}