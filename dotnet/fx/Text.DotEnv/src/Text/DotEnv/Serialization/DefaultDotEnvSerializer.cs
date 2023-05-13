using System.Text;
using System.Xml;

using Bearz.Extra.Strings;

namespace Bearz.Text.DotEnv.Serialization;

public class DefaultDotEnvSerializer : IDotEnvSerializer
{
    private readonly DotEnvSerializerOptions options;

    public DefaultDotEnvSerializer()
    {
        this.options = new DotEnvSerializerOptions() { Expand = true, };
    }

    public DefaultDotEnvSerializer(DotEnvSerializerOptions options)
    {
        this.options = options;
    }

    public string Serialize<T>(T value)
        => Serializer.Serialize(value, typeof(T), this.options);
    
    public void Serialize<T>(Stream stream, T value)
        => Serializer.Serialize(stream, value, this.options);
    
    
    
    public string Serialize(object? value, Type type)
        => Serializer.Serialize(value, type, this.options);
    
    public void Serialize(Stream stream, object? value, Type type)
    {
        using var sw = new StreamWriter(stream, Encoding.UTF8, -1, true);
        var yaml = Serializer.Serialize(value, type, this.options);
        sw.Write(yaml);
    }


    public void Serialize(TextWriter stream, object? value, Type type)
        => Serializer.Serialize(stream, value, type, this.options);

    public Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
    {
        var task = new Task(() => this.Serialize(stream, value, typeof(T)), cancellationToken);
        task.Start();
        return task;
    }

    public Task SerializeAsync(Stream stream, object? value, Type type, CancellationToken cancellationToken = default)
    {
        var task = new Task(() => this.Serialize(stream, value, type), cancellationToken);
        task.Start();
        return task;
    }

    public Task<string> SerializeAsync<T>(T value, CancellationToken cancellationToken = default)
    {
        var task = new Task<string>(() => this.Serialize(value), cancellationToken);
        task.Start();
        return task;
    }
    
    public Task<string> SerializeAsync(object? value, Type type, CancellationToken cancellationToken = default)
    {
        var task = new Task<string>(() => this.Serialize(value, type), cancellationToken);
        task.Start();
        return task;
    }

    public T? Deserialize<T>(ReadOnlySpan<char> yaml)
    {
        return (T?)Serializer.Deserialize(yaml.AsString(), typeof(T), this.options);
    }

    public object? Deserialize(ReadOnlySpan<char> yaml, Type type)
    {
        return Serializer.Deserialize(yaml.AsString(), type);
    }

    public T? Deserialize<T>(Stream stream)
    {
        return (T?)Serializer.Deserialize(stream, typeof(T), this.options);
    }

    public object? Deserialize(Stream stream, Type type)
    {
        return Serializer.Deserialize(stream, type, this.options);
    }

    public Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        var task = new Task<T?>(() => this.Deserialize<T>(stream), cancellationToken);
        task.Start();
        return task;
    }

    public Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default)
    {
        var task = new Task<object?>(() => this.Deserialize(stream, type), cancellationToken);
        task.Start();
        return task;
    }

    public Task<T?> DeserializeAsync<T>(string yaml, CancellationToken cancellationToken = default)
    {
        var task = new Task<T?>(() => this.Deserialize<T>(yaml.AsSpan()), cancellationToken);
        task.Start();
        return task;
    }

    public Task<object?> DeserializeAsync(string yaml, Type type, CancellationToken cancellationToken = default)
    {
        var task = new Task<object?>(() => this.Deserialize(yaml.AsSpan(), type), cancellationToken);
        task.Start();
        return task;
    }
}