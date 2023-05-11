using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bearz.Extensions.Logging;

public sealed class ScopeBag
{
    private readonly Dictionary<string, object?> bag = new(StringComparer.OrdinalIgnoreCase);

    public object? this[string key]
    {
        get
        {
            if (this.bag.TryGetValue(key, out var value))
                return value;

            return null;
        }
        set => this.bag[key] = value;
    }

    public void Add(string name, object value)
    {
        this.bag[name] = value;
    }

    public void Remove(string name)
    {
        this.bag.Remove(name);
    }

    public void Remove(params string[] names)
    {
        foreach (var name in names)
        {
            this.bag.Remove(name);
        }
    }

    public ScopeBag Set(string name, object value)
    {
        this.bag[name] = value;
        return this;
    }

    public ScopeBag StartedAt()
    {
        this.bag["startedAt"] = DateTime.UtcNow;
        return this;
    }

    public ScopeBag EndedAt()
    {
        this.bag["endedAt"] = DateTime.UtcNow;
        return this;
    }

    public ScopeBag WithUser(string userName)
    {
        this.bag["user"] = userName;
        return this;
    }

    public ScopeBag WithOs(OSPlatform platform, string? version = null, string? name = null)
    {
        this.bag["os.type"] = platform.ToString();
        if (version is not null)
            this.bag["os.version"] = version;

        if (name is not null)
            this.bag["os.name"] = name;
        return this;
    }

    public ScopeBag WithUserId<T>(T id)
    {
        this.bag["userId"] = id;
        return this;
    }

    public ScopeBag WithHttpResponse(HttpResponseMessage response)
    {
        this.bag["http.status_code"] = (int)response.StatusCode;
        this.bag["http.status"] = response.StatusCode.ToString();
        this.bag["http.url"] = response.RequestMessage?.RequestUri?.ToString();
        this.bag["http.response_content_length"] =
            response.Headers.GetValues("Content-Length").FirstOrDefault();
        this.bag["http.method"] = response.RequestMessage?.Method.ToString();
        return this;
    }

    public ScopeBag WithStopwatch(Stopwatch stopwatch)
    {
        this.bag["duration"] = stopwatch.ElapsedMilliseconds;
        return this;
    }

    public ScopeBag WithStopwatch(OperationStopwatch stopwatch)
    {
        this.bag["timespan"] = stopwatch.StartedAt;
        this.bag["duration"] = stopwatch.ElapsedMilliseconds;
        return this;
    }

    public ScopeBag WithDictionary(IDictionary<string, object?> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            this.bag[kvp.Key] = kvp.Value;
        }

        return this;
    }

    public Dictionary<string, object?> ToDictionary()
    {
        return this.bag.ToDictionary(x => x.Key, x => x.Value);
    }

    public Dictionary<string, string?> ToStringDictionary()
    {
        // useful for application insights.
        return this.bag.ToDictionary(x => x.Key, x => x.Value?.ToString());
    }
}