using System.Runtime.CompilerServices;

namespace Bearz.Std;

#if STD
public
#else
internal
#endif
static partial class Env
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Expand(string template, EnvSubstitutionOptions? options = null)
    {
        return EnvSubstitution.Evaluate(template, options);
    }

    public static string? Get(string name, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        return Environment.GetEnvironmentVariable(name, target);
    }

    public static IDictionary<string, string> GetAll(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        var variables = Environment.GetEnvironmentVariables(target);
        var result = new Dictionary<string, string>();
        foreach (var key in variables.Keys)
        {
            var name = key as string;
            if (name is null)
                continue;

            var value = variables[key] as string;
            if (value is null)
                continue;

            result.Add(name, value);
        }

        return result;
    }

    public static string GetRequired(string name, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        var value = Environment.GetEnvironmentVariable(name, target);
        if (value is null)
            throw new InvalidOperationException("Unable to find environment variable: " + name);

        return value;
    }

    public static bool Has(string name, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        return Environment.GetEnvironmentVariable(name, target) != null;
    }

    public static bool TryGet(string name, out string value)
    {
        value = string.Empty;
        var v = Environment.GetEnvironmentVariable(name);
        if (v == null)
        {
            return false;
        }

        value = v;
        return true;
    }

    public static bool TryGet(string name, EnvironmentVariableTarget target, out string value)
    {
        value = string.Empty;
        var v = Environment.GetEnvironmentVariable(name, target);
        if (v == null)
        {
            return false;
        }

        value = v;
        return true;
    }

    public static void Remove(string name, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        Environment.SetEnvironmentVariable(name, null, target);
    }

    public static void Set(
        IReadOnlyDictionary<string, string> values,
        bool overwrite = true,
        EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        if (overwrite)
        {
            foreach (var kvp in values)
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value, target);
            }

            return;
        }

        var existing = GetAll(target);
        foreach (var kvp in values)
        {
            if (existing.ContainsKey(kvp.Key))
                continue;

            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value, target);
        }
    }

    public static void Set(
        IDictionary<string, string> values,
        bool overwrite = true,
        EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        if (overwrite)
        {
            foreach (var kvp in values)
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value, target);
            }

            return;
        }

        var existing = GetAll(target);
        foreach (var kvp in values)
        {
            if (existing.ContainsKey(kvp.Key))
                continue;

            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value, target);
        }
    }

    public static void Set(string name, string value, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        Environment.SetEnvironmentVariable(name, value, target);
    }
}