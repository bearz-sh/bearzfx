﻿using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

using Bearz.Collections.Generic;
using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Text.DotEnv.Document;
using Bearz.Text.DotEnv.Serialization;
using Bearz.Text.DotEnv.Tokens;

namespace Bearz.Text.DotEnv;

public static class DotEnvSerializer
{
    public static T? Deserialize<T>(TextReader reader, DotEnvSerializerOptions? options = null)
        => (T?)Deserialize(reader, typeof(T), options);

    public static T? Deserialize<T>(string value, DotEnvSerializerOptions? options = null)
        => (T?)Deserialize(value, typeof(T), options);

    public static T? Deserialize<T>(Stream stream, DotEnvSerializerOptions? options = null)
        => (T?)Deserialize(stream, typeof(T), options);

    public static object? Deserialize(string value, Type type, DotEnvSerializerOptions? options = null)
    {
        using var sr = new StringReader(value);
        return Deserialize(sr, type, options);
    }

    public static object? Deserialize(Stream value, Type type, DotEnvSerializerOptions? options = null)
    {
        using var sr = new StreamReader(value, Encoding.UTF8);
        return Deserialize(sr, type, options);
    }

    public static object? Deserialize(TextReader reader, Type type, DotEnvSerializerOptions? options = null)
    {
        options ??= new DotEnvSerializerOptions();
        var r = new DotEnvReader(reader, options ?? new DotEnvSerializerOptions());

        if (type == typeof(EnvDocument) || type == typeof(Dictionary<string, string>) ||
            type == typeof(ConcurrentDictionary<string, string>) ||
            type == typeof(OrderedDictionary<string, string>) ||
            type == typeof(OrderedDictionary) ||
            type == typeof(IDictionary<string, string>) || type == typeof(IReadOnlyDictionary<string, string>))
        {
            var doc = new EnvDocument();
            string? key = null;
            while (r.Read())
            {
                switch (r.Current)
                {
                    case EnvCommentToken commentToken:
                        doc.Add(new EnvComment(commentToken.RawValue));
                        break;

                    case EnvNameToken nameToken:
                        key = nameToken.Value;
                        break;

                    case EnvScalarToken scalarToken:
                        if (key is not null && key.Length > 0)
                        {
                            if (doc.TryGetNameValuePair(key, out var entry) && entry is not null)
                            {
                                entry.RawValue = scalarToken.RawValue;
                                key = null;
                                continue;
                            }

                            doc.Add(key, scalarToken.RawValue);
                            key = null;
                            continue;
                        }

                        throw new InvalidOperationException("Scalar token found without a name token before it.");
                }
            }

            bool expand = options?.Expand == true;

            if (expand)
            {
                Func<string, string?> getVariable = (name) => Env.Get(name);
                if (options?.ExpandVariables is not null)
                {
                    var ev = options.ExpandVariables;
                    getVariable = (name) =>
                    {
                        if (doc.TryGetValue(name, out var value))
                            return value;

                        if (ev.TryGetValue(name, out value))
                            return value;

                        value = Env.Get(name);

                        return value;
                    };
                }

                var eso = new EnvSubstitutionOptions()
                {
                    UnixAssignment = false,
                    UnixCustomErrorMessage = false,
                    GetVariable = getVariable,
                    SetVariable = (name, value) => Env.Set(name, value),
                };
                foreach (var entry in doc)
                {
                    if (entry is EnvNameValuePair pair)
                    {
                        var v = EnvSubstitution.Evaluate(pair.RawValue, eso);

                        // Only set the value if it has changed.
                        if (v.Length != pair.RawValue.Length || !v.SequenceEqual(pair.RawValue))
                            pair.SetRawValue(v);
                    }
                }
            }

            if (type == typeof(EnvDocument))
                return doc;

            if (type == typeof(ConcurrentDictionary<string, string>))
                return new ConcurrentDictionary<string, string>(doc);

            if (type == typeof(OrderedDictionary<string, string>))
            {
                var ordered = new OrderedDictionary<string, string>();
                foreach (var (name, value) in doc.AsNameValuePairEnumerator())
                    ordered.Add(name, value);

                return ordered;
            }

            if (type == typeof(OrderedDictionary))
            {
                var ordered = new OrderedDictionary();
                foreach (var (name, value) in doc.AsNameValuePairEnumerator())
                    ordered.Add(name, value);

                return ordered;
            }

            if (type == typeof(Dictionary<string, string>) || type == typeof(IDictionary<string, string>) ||
                type == typeof(IReadOnlyDictionary<string, string>))
                return new Dictionary<string, string>(doc);
        }

        throw new NotSupportedException($"The type is {type.FullName} not supported.");
    }
}