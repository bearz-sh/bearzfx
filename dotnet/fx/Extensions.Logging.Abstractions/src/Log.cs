using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bearz.Extensions.Logging;

public static class Log
{
    private static string s_RootCategory = "ROOT";

    private static ILoggerFactory s_loggerFactory = NullLoggerFactory.Instance;

    // order of static fields matter and this one must be initialized last.
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    private static Lazy<ILogger> s_rootLogger = new Lazy<ILogger>(() => Factory.CreateLogger(RootCategory));

    public static ILoggerFactory Factory
    {
        internal get => s_loggerFactory;
        set => s_loggerFactory = value ?? NullLoggerFactory.Instance;
    }

    [SuppressMessage("Minor Code Smell", "S2292:Trivial properties should be auto-implemented")]
    public static string RootCategory
    {
        get => s_RootCategory;
        set => s_RootCategory = value;
    }

    public static ILogger Root => s_rootLogger.Value;

    public static ILogger<T> For<T>()
    {
        return Factory.CreateLogger<T>();
    }

    public static ILogger For(Type type)
    {
        return Factory.CreateLogger(type);
    }

    public static ILogger For(string categoryName)
    {
        return Factory.CreateLogger(categoryName);
    }

    public static IDisposable? BeginScope(string name, object? value)
        => Root.BeginScope(name, value);

    public static IDisposable? BeginScope(ScopeBag bag)
        => Root.BeginScope(bag.ToDictionary());

    public static void Critical(Exception exception)
    {
        Root.Log(LogLevel.Critical, exception, exception.Message);
    }

    public static void Critical(string message, params object?[] args)
    {
        Root.Log(LogLevel.Critical, message, args);
    }

    public static void Critical(EventId eventId, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Critical, eventId, message, args);
    }

    public static void Critical(Exception exception, string message, params object?[] args)
    {
        Root.Log(LogLevel.Critical, exception, message, args);
    }

    public static void Critical(EventId eventId, Exception? exception, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Critical, eventId, exception, message, args);
    }

    public static void Error(Exception exception)
    {
        Root.Log(LogLevel.Error, exception, exception.Message);
    }

    public static void Error(string message)
    {
        Root.Log(LogLevel.Error, message);
    }

    public static void Error(string? message, params object?[] args)
    {
        Root.Log(LogLevel.Error, message, args);
    }

    public static void Error(EventId eventId, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Debug, eventId, message, args);
    }

    public static void Error(Exception exception, string message, params object?[] args)
    {
        Root.Log(LogLevel.Error, exception, message, args);
    }

    public static void Error(EventId eventId, Exception? exception, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Error, eventId, exception, message, args);
    }

    public static void Info(string message, params object?[] args)
    {
        Root.Log(LogLevel.Information, message, args);
    }

    public static void Info(EventId eventId, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Information, eventId, message, args);
    }

    public static void Info(EventId eventId, Exception? exception, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Information, eventId, exception, message, args);
    }

    public static void Debug(string message, params object?[] args)
    {
        Root.Log(LogLevel.Debug, message, args);
    }

    public static void Debug(EventId eventId, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Debug, eventId, message, args);
    }

    public static void Debug(Exception exception, string message, params object?[] args)
    {
        Root.Log(LogLevel.Debug, exception, message, args);
    }

    public static void Debug(EventId eventId, Exception? exception, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Debug, eventId, exception, message, args);
    }

    public static void Warn(string? message, params object?[] args)
    {
        Root.Log(LogLevel.Warning, message, args);
    }

    public static void Warn(EventId eventId, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Warning, eventId, message, args);
    }

    public static void Warn(Exception exception, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Warning, exception, message, args);
    }

    public static void Warn(EventId eventId, Exception? exception, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Warning, eventId, exception, message, args);
    }

    public static void Trace(string? message, params object?[] args)
    {
        Root.Log(LogLevel.Trace, message, args);
    }

    public static void Trace(EventId eventId, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Trace, eventId, message, args);
    }

    public static void Trace(Exception? exception, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Trace, exception, message, args);
    }

    public static void Trace(EventId eventId, Exception? exception, string? message, params object?[] args)
    {
        Root.Log(LogLevel.Trace, eventId, exception, message, args);
    }

    public static void TraceMemberStart(
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        if (!Root.IsEnabled(LogLevel.Trace))
            return;

        Root.LogTrace($"{memberName} Started. Line: {sourceLineNumber}. File: {sourceFilePath}.");
    }

    public static void TraceMemberStart(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        if (!Root.IsEnabled(LogLevel.Trace))
            return;

        Root.LogTrace($"{memberName} Started. Message: {message}. Line: {sourceLineNumber}. File: {sourceFilePath}.");
    }

    public static void TraceMemberEnd(
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        if (!Root.IsEnabled(LogLevel.Trace))
            return;

        Root.LogTrace($"{memberName} End. Line: {sourceLineNumber}. File: {sourceFilePath}.");
    }

    public static void TraceMemberEnd(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        if (!Root.IsEnabled(LogLevel.Trace))
            return;

        Root.LogTrace($"{memberName} End. Message: {message}. Line: {sourceLineNumber}. File: {sourceFilePath}.");
    }
}