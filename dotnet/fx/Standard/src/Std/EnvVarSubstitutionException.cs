namespace Bearz.Std;

public class EnvVarSubstitutionException : BearzException
{
    public EnvVarSubstitutionException()
        : base()
    {
    }

    public EnvVarSubstitutionException(string message)
        : base(message)
    {
    }

    public EnvVarSubstitutionException(string message, Exception inner)
        : base(message, inner)
    {
    }
}