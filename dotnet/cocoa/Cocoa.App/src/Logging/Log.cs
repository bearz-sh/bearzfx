using Microsoft.Extensions.Logging;

namespace Cocoa.Logging;

public static class Log
{
    private static ILoggerFactory s_factory = LoggerFactory.Create(lb => lb.AddConsole().SetMinimumLevel(LogLevel.Debug));

    public static void SetFactory(ILoggerFactory factory)
        => s_factory = factory;

    public static ILogger<T> For<T>()
    {
        return s_factory.CreateLogger<T>();
    }

    public static ILogger For(Type type)
    {
        return s_factory.CreateLogger(type);
    }

    public static ILogger For(string categoryName)
    {
        return s_factory.CreateLogger(categoryName);
    }
}