using System.Text;

using Bearz.Extra.Strings;
using Bearz.Text;

namespace Bearz.Std;

public static class EnvVarEvaluator
{
    private enum TokenKind
    {
        None,
        Windows,
        BashVariable,
        BashInterpolation,
    }

    public static string Evaluate(string template, bool includeWindows = false, bool argsSubsitution = false, Func<string, string?>? getValue = null)
        => Evaluate(template.AsSpan(), includeWindows, argsSubsitution, getValue).AsString();

    public static ReadOnlySpan<char> Evaluate(ReadOnlySpan<char> template, bool includeWindows = false, bool argsSubstituion = false, Func<string, string?>? getValue = null)
    {
        var tokenBuilder = new StringBuilder();
        var output = new StringBuilder();
        var kind = TokenKind.None;
        for (var i = 0; i < template.Length; i++)
        {
            var c = template[i];
            if (kind == TokenKind.None)
            {
                if (includeWindows && c is '%')
                {
                    kind = TokenKind.Windows;
                    continue;
                }

                var z = i + 1;
                var next = char.MinValue;
                if (z < template.Length)
                    next = template[z];

                // escape the $ character.
                if (c is '\\' && next is '$')
                {
                    output.Append(next);
                    i++;
                    continue;
                }

                if (c is '$')
                {
                    // can't be a variable if there is no next character.
                    var remaining = template.Length - 1;
                    if (next is '{' && remaining > 3)
                    {
                        kind = TokenKind.BashInterpolation;
                        i++;
                        continue;
                    }

                    // only a variable if the next character is a letter.
                    if (remaining > 0 && char.IsLetterOrDigit(next))
                    {
                        kind = TokenKind.BashVariable;
                        continue;
                    }
                }

                output.Append(c);
            }
            else
            {
                if (kind == TokenKind.Windows && c is '%')
                {
                    if (tokenBuilder.Length == 0)
                    {
                        // consecutive %, so just append both characters.
                        output.Append('%', 2);
                        continue;
                    }

                    var value = getValue?.Invoke(tokenBuilder.ToString());
                    if (value is not null && value.Length > 0)
                        output.Append(value);
                    tokenBuilder.Clear();
                    kind = TokenKind.None;
                    continue;
                }

                if (kind == TokenKind.BashInterpolation && c is '}')
                {
                    if (tokenBuilder.Length == 0)
                    {
                        // with bash '${}' is a bad substitution.
                        throw new InvalidOperationException("${} is bad substitution.");
                    }

                    var substitution = tokenBuilder.ToString();
                    string key = substitution;
                    string defaultValue = string.Empty;
                    string? message = null;
                    if (substitution.Contains(":-"))
                    {
                        var parts = substitution.Split(":-");
                        key = parts[0];
                        defaultValue = parts[1];
                    }
                    else if (substitution.Contains(":"))
                    {
                        var parts = substitution.Split(":");
                        key = parts[0];
                        defaultValue = parts[1];
                    }
                    else if (substitution.Contains("?"))
                    {
                        var parts = substitution.Split(":?");
                        key = parts[0];
                        message = parts[1];
                    }

                    if (key.Length == 0)
                    {
                        throw new InvalidOperationException("Bad substitution, empty variable name.");
                    }

                    if (!IsValidBashVariable(key.AsSpan()))
                    {
                        throw new InvalidOperationException($"Bad substitution, invalid variable name {key}.");
                    }

                    var value = getValue == null ? Env.Get(key) : getValue(key);
                    if (value is not null)
                        output.Append(value);
                    else if (message is not null)
                        throw new InvalidOperationException(message);
                    else if (defaultValue.Length > 0)
                        output.Append(defaultValue);

                    tokenBuilder.Clear();
                    kind = TokenKind.None;
                    continue;
                }

                if (kind == TokenKind.BashVariable && !(char.IsLetterOrDigit(c) || c is '_'))
                {
                    var key = tokenBuilder.ToString();
                    if (key.Length == 0)
                    {
                        throw new InvalidOperationException("Bad substitution, empty variable name.");
                    }

                    if (argsSubstituion && int.TryParse(key, out var index))
                    {
                        if (index < 0 || index >= Environment.GetCommandLineArgs().Length)
                            throw new InvalidOperationException($"Bad substitution, invalid index {index}.");

                        output.Append(Environment.GetCommandLineArgs()[index]);
                        output.Append(c);
                        tokenBuilder.Clear();
                        kind = TokenKind.None;
                        continue;
                    }

                    if (!IsValidBashVariable(key.AsSpan()))
                    {
                        throw new InvalidOperationException($"Bad substitution, invalid variable name {key}.");
                    }

                    var value = getValue == null ? Env.Get(key) : getValue(key);
                    if (value is not null && value.Length > 0)
                        output.Append(value);

                    output.Append(c);
                    tokenBuilder.Clear();
                    kind = TokenKind.None;
                    continue;
                }

                tokenBuilder.Append(c);
            }
        }

        if (tokenBuilder.Length > 0)
        {
            if (kind == TokenKind.Windows)
            {
                throw new InvalidOperationException("Bad substitution, unclosed %.");
            }

            if (kind == TokenKind.BashVariable)
            {
                throw new InvalidOperationException("Bad substitution, unclosed $.");
            }

            if (kind == TokenKind.BashInterpolation)
            {
                throw new InvalidOperationException("Bad substitution, unclosed ${.");
            }
        }

        var result = output.AsSpan();
        output.Clear();
        return result;
    }

    private static bool IsValidBashVariable(ReadOnlySpan<char> input)
    {
        for (var i = 0; i < input.Length; i++)
        {
            if (i == 0 && !char.IsLetter(input[i]))
                return false;

            if (!char.IsLetterOrDigit(input[i]) && input[i] is not '_')
                return false;
        }

        return true;
    }
}