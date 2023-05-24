using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Management.Automation;
using System.Text.RegularExpressions;

using Bearz.Extras.YamlDotNet;

using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Bearz.PowerShell.Yaml;

public static class PsYamlReader
{
    private const string YamlDateTimePattern = @"  
# From the YAML spec: https://yaml.org/type/timestamp.html
[0-9][0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9] # (ymd)
|[0-9][0-9][0-9][0-9] # (year)
 -[0-9][0-9]? # (month)
 -[0-9][0-9]? # (day)
 ([Tt]|[ \t]+)[0-9][0-9]? # (hour)
 :[0-9][0-9] # (minute)
 :[0-9][0-9] # (second)
 (\.[0-9]*)? # (fraction)
 (([ \t]*)Z|[-+][0-9][0-9]?(:[0-9][0-9])?)? # (time zone)
@";

    public static object? ReadYaml(string? yaml, bool merge, bool all, bool asHashtable)
    {
        if (yaml.IsNullOrWhiteSpace())
            return null;

        using var sr = new System.IO.StringReader(yaml);
        IParser parser = new Parser(sr);
        if (merge)
            parser = new MergingParser(parser);

        var stream = new YamlStream();
        stream.Load(parser);

        if (stream.Documents.Count == 0)
            return null;

        if (stream.Documents.Count == 1 || !all)
        {
            return Visit(stream.Documents[0].RootNode, asHashtable);
        }

        var list = new List<object?>();
        foreach (var doc in stream.Documents)
        {
            list.Add(Visit(doc.RootNode, asHashtable));
        }

        return list.ToArray();
    }

    private static object? Visit(YamlNode node, bool asHashtable)
    {
        switch (node)
        {
            case YamlScalarNode scalar:
                return scalar.ToObject();
            case YamlSequenceNode sequence:
                return Visit(sequence, asHashtable);
            case YamlMappingNode mapping:
                return Visit(mapping, asHashtable);
            default:
                throw new NotSupportedException();
        }
    }

    private static object Visit(YamlSequenceNode sequence, bool asHashtable)
    {
        var list = new List<object?>();
        foreach (var item in sequence.Children)
        {
            list.Add(Visit(item, asHashtable));
        }

        return list.ToArray();
    }

    private static object Visit(YamlMappingNode node, bool asHashtable)
    {
        if (asHashtable)
        {
            var ordered = new OrderedDictionary();
            foreach (var child in node.Children)
            {
                if (child.Key is YamlScalarNode label)
                {
                    if (label.Value is null)
                        continue;

                    var value = Visit(child.Value, true);
                    ordered.Add(label.Value, value);
                }
            }

            return ordered;
        }

        var obj = new PSObject();
        foreach (var child in node.Children)
        {
            if (child.Key is YamlScalarNode label)
            {
                if (label.Value is null)
                    continue;

                var value = Visit(child.Value, true);
                obj.Properties.Add(new PSNoteProperty(label.Value, value));
            }
        }

        return obj;
    }
}