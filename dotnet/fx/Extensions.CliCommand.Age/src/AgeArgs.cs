using Bearz.Extra.Strings;
using Bearz.Std;

using FluentBuilder;

namespace Bearz.Extensions.CliCommand.Age;

[AutoGenerateBuilder]
public class AgeArgs : ReflectionCommandArgsBuilder
{
    public AgeArgs()
        : base(null)
    {
    }

    /// <summary>
    /// Gets or sets the input.  Read the input from the file at PATH. If omitted, read from stdin.
    /// </summary>
    public string? Input { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to encrypt the input. Encrypt the input to the output.
    /// Default if omitted.
    /// </summary>
    public bool Encrypt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to decrypt the input. Decrypt the input to the output.
    /// </summary>
    public bool Decrypt { get; set; }

    /// <summary>
    /// Gets or sets the path to the output file.  Write the output to the file at PATH.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether PEM encoding is used. Encrypt to a PEM encoded format.
    /// </summary>
    public bool Armor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether encryption with a passphrase. Encrypt with a passphrase.
    /// </summary>
    public bool Passphrase { get; set; }

    /// <summary>
    /// Gets or sets the recipients.  Encrypt for the given RECIPIENT. Can be repeated.
    /// </summary>
    public IReadOnlyList<string> Recipients { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the recipients files.  Read recipients from the file at PATH. Can be repeated.
    /// </summary>
    public IReadOnlyList<string> RecipientsFiles { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the identity.  Use the identity file at PATH. Can be repeated.
    /// </summary>
    public IReadOnlyList<string> Identity { get; set; } = Array.Empty<string>();

    public override CommandArgs BuildArgs()
    {
        if (!this.Input.IsNullOrWhiteSpace())
            this.TrailingArguments.Add(this.Input);

        return base.BuildArgs();
    }

    protected override bool Handle(string name, object? value, Type type, CommandArgs args)
    {
        if (name == nameof(this.Recipients))
        {
            if (value is IReadOnlyList<string> recipients)
            {
                foreach (var recipient in recipients)
                {
                    args.Add("-r", recipient);
                }
            }

            return true;
        }

        if (name == nameof(this.RecipientsFiles))
        {
            if (value is IReadOnlyList<string> recipientsFiles)
            {
                foreach (var recipientsFile in recipientsFiles)
                {
                    args.Add("-R", recipientsFile);
                }
            }

            return true;
        }

        if (name == nameof(this.Identity))
        {
            if (value is IReadOnlyList<string> identity)
            {
                foreach (var id in identity)
                {
                    args.Add("-i", id);
                }
            }

            return true;
        }

        if (name == nameof(this.Input))
        {
            return true;
        }

        return false;
    }
}