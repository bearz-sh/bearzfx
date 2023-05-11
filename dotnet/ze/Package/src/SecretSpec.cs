using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ze.Package;

public class SecretSpec
{
    public bool Generate { get; set; } = true;

    public int Length { get; set; } = 16;

    public bool Lower { get; set; } = true;

    public bool Upper { get; set; } = true;

    public bool Digit { get; set; } = true;

    public bool Special { get; set; } = true;

    public string? SpecialCharacters { get; set; }

    public static Dictionary<string, SecretSpec> ParseFile(string fileName)
    {
        using var streamReader = new StreamReader(fileName);
        var serializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var parsed = serializer.Deserialize<Dictionary<string, SecretSpec?>>(streamReader);
        var secrets = new Dictionary<string, SecretSpec>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in parsed)
        {
            secrets[kvp.Key] = kvp.Value ?? new SecretSpec();
        }

        return secrets;
    }

    public static Dictionary<string, SecretSpec> Parse(string content)
    {
        using var sr = new StringReader(content);
        var serializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var parsed = serializer.Deserialize<Dictionary<string, SecretSpec?>>(sr);
        var secrets = new Dictionary<string, SecretSpec>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in parsed)
        {
            secrets[kvp.Key] = kvp.Value ?? new SecretSpec();
        }

        return secrets;
    }
}