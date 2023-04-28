using Bearz.Security.Cryptography;
using Bearz.Text;

using DbSecret = Bearz.Casa.Data.Models.Secret;

namespace Bearz.Casa.Data.Services;

public class Secret
{
    private readonly DbSecret model;
    private readonly IEncryptionProvider cipher;
    private string? value;

    internal Secret(DbSecret secret, IEncryptionProvider cipher)
    {
        this.cipher = cipher;
        this.model = secret;
    }

    public string Name
    {
        get => this.model.Name;
        set => this.model.Name = value;
    }

    public string Value
    {
        get
        {
            if (this.value != null)
                return this.value;

            if (this.model.Value.Length == 0)
                return string.Empty;

            var bytes = Convert.FromBase64String(this.model.Value);
            var decrypted = this.cipher.Decrypt(bytes);
            this.value = Encodings.Utf8NoBom.GetString(decrypted);
            return this.value;
        }

        set
        {
            if (value.Length == 0)
            {
                this.value = null;
                this.model.Value = string.Empty;
                return;
            }

            this.value = value;
            var bytes = Encodings.Utf8NoBom.GetBytes(this.value);
            var encrypted = this.cipher.Encrypt(bytes);
            this.model.Value = Convert.ToBase64String(encrypted);
        }
    }

    public DateTime? ExpiresAt
    {
        get => this.model.ExpiresAt;
        set => this.model.ExpiresAt = value;
    }

    internal DbSecret Model => this.model;
}