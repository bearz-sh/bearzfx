namespace Bearz.Diagnostics;

[Serializable]
public class NotFoundOnPathException : FileNotFoundException
{
    public NotFoundOnPathException()
    {
    }

    public NotFoundOnPathException(string fileName)
        : base($"The executable {fileName} was not found on the environment's path.", fileName)
    {
    }

    public NotFoundOnPathException(string fileName, string message)
        : base(message, fileName)
    {
    }

    public NotFoundOnPathException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public NotFoundOnPathException(string fileName, string message, Exception innerException)
        : base(message, fileName, innerException)
    {
    }

    protected NotFoundOnPathException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    {
    }
}