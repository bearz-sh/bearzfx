using System;
using System.Linq;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Plank.Package;

public class PlankSpec
{
    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<string> Authors { get; set; } = new();

    public List<string> Maintainers { get; set; } = new();

    public List<PlankSpecDependency> Deps { get; set; } = new();

    public Dictionary<string, string> Labels { get; set; } = new();

    public static PlankSpec ParseFile(string fileName)
    {
        using var streamReader = new StreamReader(fileName);
        var serializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return serializer.Deserialize<PlankSpec>(streamReader);
    }

    public static PlankSpec Parse(string content)
    {
        using var sr = new StringReader(content);
        var serializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return serializer.Deserialize<PlankSpec>(sr);
    }
}

public class PlankSpecDependency
{
    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public string Alias { get; set; } = string.Empty;
}