using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

using Bearz.Text.Yaml;

using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeResolvers;

namespace Bearz.PowerShell.Yaml;

public static class PsYamlWriter
{
    public static void WriteYaml(object? input, SerializationOptions options, TextWriter writer)
    {
        if (input is not null)
            input = new List<object> { input };

        var normalizedObject = Visit(input);

        var builder = new SerializerBuilder()
            .WithQuotingNecessaryStrings();

        var o = options;
        if (o != SerializationOptions.None)
        {
            if (o.HasFlag(SerializationOptions.Roundtrip))
                builder.EnsureRoundtrip();

            if (o.HasFlag(SerializationOptions.DisableAliases))
                builder.DisableAliases();

            if (o.HasFlag(SerializationOptions.JsonCompatible))
                builder.JsonCompatible();

            if (o.HasFlag(SerializationOptions.DefaultToStaticType))
                builder.WithTypeResolver(new StaticTypeResolver());

            if (o.HasFlag(SerializationOptions.IndentedSequences))
                builder.WithIndentedSequences();
        }

        var serializer = builder.Build();
        serializer.Serialize(writer, normalizedObject);
    }

    public static YamlNode Visit(object? value)
    {
        switch (value)
        {
            case null:
                return new YamlScalarNode(null);

            case byte[] b:
                return new YamlScalarNode(Convert.ToBase64String(b));

            case IDictionary d:
                return Visit(d);

            case PSObject pso:
                return Visit(pso);

            case IList l:
                return Visit(l);

            default:
                return new YamlScalarNode(value?.ToString());
        }
    }

    public static YamlSequenceNode Visit(IList value)
    {
        var list = new YamlSequenceNode();
        foreach (var item in value)
        {
            list.Add(Visit(item));
        }

        return list;
    }

    public static YamlMappingNode Visit(IDictionary value)
    {
        var dict = new YamlMappingNode();
        foreach (var key in value.Keys)
        {
            if (key is string name)
            {
                dict.Add(name, Visit(value[key]));
            }
        }

        return dict;
    }

    public static YamlMappingNode Visit(PSObject value)
    {
        var dict = new YamlMappingNode();
        foreach (var prop in value.Properties)
        {
            dict.Add(prop.Name, Visit(prop.Value));
        }

        return dict;
    }
}