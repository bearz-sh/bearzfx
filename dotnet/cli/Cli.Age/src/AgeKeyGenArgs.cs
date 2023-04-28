using Bearz.Std;

using FluentBuilder;

namespace Bearz.Cli.Age;

[AutoGenerateBuilder]
public class AgeKeyGenArgs : ReflectionCommandArgsBuilder
{
    public AgeKeyGenArgs()
        : base(null)
    {
    }

    public string? Output { get; set; }

    public bool Identity { get; set; }

    protected override bool Handle(string name, object? value, Type type, CommandArgs args)
    {
        if (name == nameof(this.Identity))
        {
            if (value is true)
            {
                args.Add("-y");
            }

            return true;
        }

        return false;
    }
}