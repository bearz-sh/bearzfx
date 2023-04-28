namespace Bearz.Std;

public static class ErrorConverter
{
    private static readonly List<Func<Exception, Error?>> ExceptionConverters = new();

    public static Error ConvertToError(Exception exception)
    {
        if (ExceptionConverters.Count > 0)
        {
            foreach (var converter in ExceptionConverters)
            {
                var err = converter(exception);
                if (err.HasValue)
                    return err.Value;
            }
        }

        var error = new Error()
        {
            Message = exception.Message,
            Code = exception.GetType().FullName,
            Target = exception.TargetSite?.Name,
        };

        if (exception is AggregateException aggregateException)
        {
            var errors = new List<IError>();
            foreach (var innerException in aggregateException.InnerExceptions)
            {
                var inner = ConvertToError(innerException);
                errors.Add(inner);
            }

            error.Details = errors;
        }

        if (exception.InnerException != null)
            error.InnerError = ConvertToError(exception.InnerException);

        if (exception is ArgumentException argumentException)
            error["parameterName"] = argumentException.ParamName;

        return error;
    }

    public static void RegisterExceptionConverter(Func<Exception, Error?> converter)
    {
        if (ExceptionConverters.Contains(converter))
            ExceptionConverters.Add(converter);
    }
}