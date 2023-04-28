namespace Bearz.Std;

#if STD
public
#else
internal
#endif
class EnvSubstitutionException : BearzException
{
    public EnvSubstitutionException()
        : base()
    {
    }

    public EnvSubstitutionException(string message)
        : base(message)
    {
    }

    public EnvSubstitutionException(string message, Exception inner)
        : base(message, inner)
    {
    }
}