using Bearz.Text.DotEnv;
using Bearz.Text.DotEnv.Serialization;

namespace Bearz.Std;

public static class DotEnv
{
    public static IDotEnvSerializer DotEnvSerializerProvider { get; set; } = new DefaultDotEnvSerializer();

    public static string Stringify<T>(T value)
        => DotEnvSerializerProvider.Serialize(value);

    public static string Stringify(object? value, Type type)
        => DotEnvSerializerProvider.Serialize(value, type);

    public static void Stringify<T>(Stream stream, T value)
        => DotEnvSerializerProvider.Serialize(stream, value);

    public static void Stringify(Stream stream, object? value, Type type)
        => DotEnvSerializerProvider.Serialize(stream, value, type);

    public static Task StringifyAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
        => DotEnvSerializerProvider.SerializeAsync(stream, value, cancellationToken);

    public static Task StringifyAsync(Stream stream, object? value, Type type, CancellationToken cancellationToken = default)
        => DotEnvSerializerProvider.SerializeAsync(stream, value, type, cancellationToken);

    public static Task<string> StringifyAsync<T>(T value, CancellationToken cancellationToken = default)
        => DotEnvSerializerProvider.SerializeAsync(value, cancellationToken);

    public static Task<string> StringifyAsync(object? value, Type type, CancellationToken cancellationToken = default)
        => DotEnvSerializerProvider.SerializeAsync(value, type, cancellationToken);

    public static T? Parse<T>(ReadOnlySpan<char> json)
        => DotEnvSerializerProvider.Deserialize<T>(json);

    public static object? Parse(ReadOnlySpan<char> json, Type type)
        => DotEnvSerializerProvider.Deserialize(json, type);

    public static T? Parse<T>(Stream stream)
        => DotEnvSerializerProvider.Deserialize<T>(stream);

    public static object? Parse(Stream stream, Type type)
        => DotEnvSerializerProvider.Deserialize(stream, type);

    public static Task<T?> ParseAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        => DotEnvSerializerProvider.DeserializeAsync<T>(stream, cancellationToken);

    public static Task<object?> ParseAsync(Stream stream, Type type, CancellationToken cancellationToken = default)
        => DotEnvSerializerProvider.DeserializeAsync(stream, type, cancellationToken);

    public static Task<T?> ParseAsync<T>(string json, CancellationToken cancellationToken = default)
        => DotEnvSerializerProvider.DeserializeAsync<T>(json, cancellationToken);

    public static Task<object?> ParseAsync(string json, Type type, CancellationToken cancellationToken = default)
        => DotEnvSerializerProvider.DeserializeAsync(json, type, cancellationToken);
}