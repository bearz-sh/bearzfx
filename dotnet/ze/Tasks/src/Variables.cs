using System.Net;
using System.Net.Sockets;
using System.Text.Json;

using Bearz.Extra.Object;
using Bearz.Extra.Strings;
using Bearz.Std;

using Microsoft.Extensions.Configuration;

namespace Ze.Tasks;

public class Variables : IMutableVariables
{
    private readonly Dictionary<string, object?> variables;

    public Variables()
    {
        this.variables = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["os"] = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["windows"] = Env.IsWindows,
                ["linux"] = Env.IsLinux,
                ["mac"] = Env.IsMacOS,
                ["darwin"] = Env.IsMacOS,
            },
            ["host"] = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ip"] = GetLocalIp(),
                },
            ["process"] = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["pid"] = Env.Process.Id,
                ["elevated"] = Env.IsUserElevated,
                ["is64bit"] = Env.IsProcess64Bit,
            },
        };

        var envValues = Env.GetAll();
        var env = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in envValues)
        {
            env[kvp.Key] = kvp.Value;
        }

        env["is_elevated"] = Env.IsUserElevated;

        if (Env.IsWindows)
        {
            env["home"] = Env.HomeDirectory;
            env["user"] = Env.User;
            env["hostname"] = Env.HostName;
        }

        this.variables["env"] = env;
    }

    public Variables(Variables variables)
    {
        this.variables = new Dictionary<string, object?>(variables.variables);
    }

    public Variables(IDictionary<string, object?> variables)
    {
        this.variables = new Dictionary<string, object?>(variables);
    }

    public object? this[string key]
    {
        get
        {
            if (this.variables.TryGetValue(key, out var value))
                return value;

            return default;
        }
        set => this.variables[key] = value;
    }

    public object? this[string key, object? defaultValue]
    {
        get
        {
            if (this.variables.TryGetValue(key, out var value))
                return value;

            return defaultValue;
        }
        set => this.variables[key] = value ?? defaultValue;
    }

    public Variables Add(IConfiguration config)
    {
        ProcessSection(config.GetChildren(), this.variables);
        return this;
    }

    public Variables Add(Dictionary<string, object?> dictionary)
    {
        ProcessVariables(string.Empty, dictionary);
        return this;
    }

    public Variables AddVarFiles(IReadOnlyCollection<string> varFiles)
    {
        if (varFiles.Count == 0)
        {
            return this;
        }

        var builder = new ConfigurationBuilder();
        foreach (var file in varFiles)
        {
            if (!Fs.FileExists(file))
            {
                if (!file.EndsWith(".yml"))
                    continue;

                var secondary = FsPath.ChangeExtension(file, ".yaml");
                if (!Fs.FileExists(secondary))
                    continue;

                builder.AddYamlFile(secondary, true, false);
                continue;
            }

            if (file.EndsWith(".yaml") || file.EndsWith("yml"))
            {
                builder.AddYamlFile(file, true, false);
                continue;
            }

            if (file.EndsWith(".json") || file.EndsWith(".jsonc"))
            {
                builder.AddJsonFile(file, true, false);
            }
        }

        var config = builder.Build();
        return this.Add(config);
    }

    public Variables Dump()
    {
        var json = JsonSerializer.Serialize(this.variables, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        Console.WriteLine(json);
        return this;
    }

    public Variables LoadAsEnvironmentVariables()
    {
        ProcessVariables(string.Empty, this.variables);
        return this;
    }

    public IDictionary<string, object?> ToDictionary()
    {
        return this.variables;
    }

    public bool TryGetValue(string name, out object? value)
    {
        if (!name.Contains('.'))
            return this.variables.TryGetValue(name, out value);

        value = null;
        var parts = name.Split('.');
        var current = (IReadOnlyDictionary<string, object?>)this.variables;
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (i == parts.Length - 1)
            {
                if (current.TryGetValue(part, out value))
                    return true;

                value = default;
                return false;
            }

            if (current.TryGetValue(part, out var next))
            {
                if (next is not IReadOnlyDictionary<string, object?> map)
                {
                    value = default;
                    return false;
                }

                current = map;
                continue;
            }

            value = default;
            return false;
        }

        return false;
    }

    private static void ProcessVariables(string baseKey, IDictionary<string, object?> map)
    {
        foreach (var kvp in map)
        {
            if (kvp.Value is null)
            {
                continue;
            }

            var key = kvp.Key;
            if (!baseKey.IsNullOrWhiteSpace())
                key = $"{baseKey.ToUpperInvariant()}_{key.ToUpperInvariant()}";

            if (kvp.Value is IDictionary<string, object?> childMap)
            {
                ProcessVariables(key, childMap);
                continue;
            }

            if (kvp.Value is IList<object?> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var value = list[i];
                    Env.Set($"{key}_{i}", value.ToSafeString());
                }

                continue;
            }

            if (kvp.Value is Array array)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var value = array.GetValue(i);
                    Env.Set($"{key}_{i}", value.ToSafeString());
                }

                continue;
            }

            Env.Set(key, kvp.Value.ToSafeString());
        }
    }

    private static void ProcessSection(IEnumerable<IConfigurationSection> children, Dictionary<string, object?> context)
    {
        foreach (var child in children)
        {
            if (child.Value is not null)
            {
                var value = Env.Expand(child.Value);
                context[child.Key] = value;
            }
            else
            {
                var childContext = new Dictionary<string, object?>();
                context[child.Key] = childContext;
                ProcessSection(child.GetChildren(), childContext);
            }
        }
    }

    private static string? GetLocalIp()
    {
        string? localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("1.1.1.1", 65530);
            IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint?.Address.ToString();
        }

        return localIP;
    }
}