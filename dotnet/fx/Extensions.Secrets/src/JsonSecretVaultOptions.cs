using Bearz.Security.Cryptography;

namespace Bearz.Extensions.Secrets;

public class JsonSecretVaultOptions : SecretVaultOptions
{
    public string Path { get; set; } = string.Empty;

    public byte[] Key { get; set; } = Array.Empty<byte>();

    public IEncryptionProvider? EncryptionProvider { get; set; }
}