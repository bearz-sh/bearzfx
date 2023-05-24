using System.Text;

namespace Bearz.Extensions.Ssh;

public class PrivateKeyAuthentication : IAuthenticationMethod
{
    private string? passphraseString = null;

    public string Kind => "private-key";

    public string Username { get; set; } = string.Empty;

    public string PrivateKeyPath { get; set; } = string.Empty;

    public byte[]? Passphrase { get; set; } = null;

    public string? PassphraseString
    {
        get
        {
            if (this.Passphrase is null)
                return null;

            return this.passphraseString ??= Encoding.UTF8.GetString(this.Passphrase);
        }
    }
}