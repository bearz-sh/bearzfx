// Copyright © 2017 - 2021 Chocolatey Software, Inc
// Copyright © 2011 - 2017 RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/tokens/TokenReplacer.cs

using System.Reflection;
using System.Text.RegularExpressions;

namespace Cocoa.Tokens;

public static class TokenReplacer
{
    public static string ReplaceTokens<TConfig>(TConfig configuration, string textToReplace, string tokenPrefix = "[[", string tokenSuffix = "]]")
    {
        if (string.IsNullOrEmpty(textToReplace))
            return string.Empty;

        var dictionary = CreateDictionaryFromConfiguration(configuration);
        if (dictionary.Count == 0)
            return textToReplace;

        var regex = new Regex($"{Regex.Escape(tokenPrefix)}(?<key>\\w+){Regex.Escape(tokenSuffix)}");
        string output = regex.Replace(textToReplace, m =>
            {
                var originalKey = m.Groups["key"].Value;
                var key = originalKey.ToSafeString();
                if (!dictionary.ContainsKey(key))
                {
                    return tokenPrefix + originalKey + tokenSuffix;
                }

                string value = dictionary[key];
                return value;
            });

        return output;
    }

    public static IEnumerable<string> GetTokens(string textWithTokens, string tokenPrefix = "[[", string tokenSuffix = "]]")
    {
        var regexMatches = Regex.Matches(textWithTokens, $"{Regex.Escape(tokenPrefix)}(?<key>\\w+){Regex.Escape(tokenSuffix)}");
        foreach (Match regexMatch in regexMatches)
        {
            yield return regexMatch.Groups["key"].ToSafeString();
        }
    }

    private static IDictionary<string, string> CreateDictionaryFromConfiguration<TConfig>(TConfig configuration)
    {
        if (configuration is IDictionary<string, string> dictionary)
            return dictionary;

        var propertyDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (configuration is null)
            return propertyDictionary;

        foreach (PropertyInfo property in configuration.GetType().GetProperties())
        {
            propertyDictionary.Add(property.Name, property.GetValue(configuration, null).ToSafeString());
        }

        return propertyDictionary;
    }
}