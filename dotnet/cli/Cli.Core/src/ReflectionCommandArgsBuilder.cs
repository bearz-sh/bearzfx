using System.Collections;
using System.Text;

using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Text;

namespace Bearz.Cli;

public abstract class ReflectionCommandArgsBuilder : CommandArgsBuilder
{
    protected ReflectionCommandArgsBuilder(string? command = null)
    {
        this.Command = command;
    }

    protected string? Command { get; }

    protected virtual string OptionPrefix => "--";

    protected List<string> Arguments { get; } = new List<string>();

    protected List<string> TrailingArguments { get; } = new List<string>();

    public override CommandArgs BuildArgs()
    {
        var args = new CommandArgs();
        return this.BuildArgs(args);
    }

    protected virtual CommandArgs BuildArgs(CommandArgs args)
    {
        if (!this.Command.IsNullOrWhiteSpace())
            args.Add(this.Command);

        if (this.Arguments.Count > 0)
            args.AddRange(this.Arguments);

        var properties = this.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(this);
            var type = property.PropertyType;
            var name = property.Name;

            if (this.Handle(name, value, type, args))
                continue;

            name = this.FormatOptionName(name);

            switch (value)
            {
                case null:
                    continue;

                case string s:
                    if (s.IsNullOrWhiteSpace())
                        continue;
                    args.Add(name, s);
                    break;

                case bool _:
                    args.Add(name);
                    break;

                case int i:
                    args.Add(name, i.ToString());
                    break;

                case Enum enumValue:
                    args.Add(name, enumValue.ToString());
                    break;

                case string[] stringArray:
                    foreach (var item in stringArray)
                    {
                        args.Add(name, item);
                    }

                    break;

                case Array array:
                    foreach (var item in array)
                    {
                        var s2 = item?.ToString();
                        if (!s2.IsNullOrWhiteSpace())
                            args.Add(name, s2);
                    }

                    break;

                case IEnumerable<string> enumerableString:
                    foreach (var item in enumerableString)
                    {
                        args.Add(name, item);
                    }

                    break;

                case IEnumerable enumerable:
                    foreach (var item in enumerable)
                    {
                        var s3 = item.ToString();
                        if (!s3.IsNullOrWhiteSpace())
                            args.Add(name, s3);
                    }

                    break;
            }
        }

        if (this.TrailingArguments.Count > 0)
            args.AddRange(this.TrailingArguments);

        return args;
    }

    protected void AddArgument(string argument)
    {
        this.Arguments.Add(argument);
    }

    protected void AddTrailingArgument(string argument)
    {
        this.TrailingArguments.Add(argument);
    }

    protected virtual bool Handle(string name, object? value, Type type, CommandArgs args)
    {
        return false;
    }

    protected string FormatOptionName(string name)
    {
        var sb = StringBuilderCache.Acquire();
        sb.Append(this.OptionPrefix);

        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                {
                    sb.Append('-');
                }

                sb.Append(char.ToLower(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return StringBuilderCache.GetStringAndRelease(sb);
    }
}