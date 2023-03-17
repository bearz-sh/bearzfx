using System.Collections.Concurrent;
using System.Text.Json;

using Bearz.Security.Cryptography;
using Bearz.Text;

namespace Bearz.Extensions.Secrets;

public sealed class JsonSecretVault : SecretVault, IDisposable
{
    private readonly IEncryptionProvider cipher;

    private readonly IDisposable? disposableOptions;

    private readonly SemaphoreSlim syncLock = new(1, 1);

    private readonly ConcurrentDictionary<string, JsonSecretRecord> secrets = new();

    public JsonSecretVault(JsonSecretVaultOptions options)
    {
        if (options.EncryptionProvider is not null)
        {
            this.cipher = options.EncryptionProvider;
        }
        else
        {
            var o = new SymmetricEncryptionProviderOptions();
            o.SetKey(options.Key);
            this.disposableOptions = o;
            this.cipher = new SymmetricEncryptionProvider(o);
        }

        this.Options = options;
        base.Options = options;
    }

    public override bool SupportsSynchronous => true;

    public override string Kind => "json-vault";

    private new JsonSecretVaultOptions Options { get; }

    public override ISecretRecord CreateRecord(string name)
    {
        return new JsonSecretRecord(this.FormatName(name));
    }

    public override IEnumerable<string> ListNames()
    {
        this.Load();
        return this.secrets.Keys.ToArray();
    }

    public override string? GetSecretValue(string name)
    {
        this.Load();
        name = this.FormatName(name);
        if (this.secrets.TryGetValue(name, out var secret)
            && secret is not null)
        {
            if (secret.Value is { Length: > 0 })
            {
                var decrypted = this.cipher.Decrypt(Convert.FromBase64String(secret.Value));
                return Encodings.Utf8NoBom.GetString(decrypted);
            }

            return string.Empty;
        }

        return null;
    }

    public override ISecretRecord? GetSecret(string name)
    {
        this.Load();
        name = this.FormatName(name);
        if (this.secrets.TryGetValue(name, out var secret)
            && secret is not null)
        {
            var copy = new JsonSecretRecord(secret);
            if (copy.Value is { Length: > 0 })
            {
                var decrypted = this.cipher.Decrypt(Convert.FromBase64String(copy.Value));
                copy.Value = Encodings.Utf8NoBom.GetString(decrypted);
            }

            return copy;
        }

        return null;
    }

    public override void SetSecretValue(string name, string secret)
    {
        this.Load();
        name = this.FormatName(name);
        var value = secret;

        if (!this.secrets.TryGetValue(name, out var existing) || existing is null)
        {
           existing = new JsonSecretRecord(name);
           existing.WithCreatedAt(DateTime.UtcNow);
           this.secrets.TryAdd(name, existing);
        }
        else
        {
           existing.WithUpdatedAt(DateTime.UtcNow);
        }

        if (value.Length > 0)
        {
           var encrypted = this.cipher.Encrypt(Encodings.Utf8NoBom.GetBytes(value));
           existing.Value = Convert.ToBase64String(encrypted);
        }

        this.Save();
    }

    public override void SetSecret(ISecretRecord secret)
    {
        this.Load();
        var value = secret.Value;

        // ReSharper disable once InconsistentlySynchronizedField
        if (!this.secrets.TryGetValue(secret.Name, out var existing) || existing is null)
        {
            existing = new JsonSecretRecord(secret.Name) { ExpiresAt = secret.ExpiresAt, };
            existing.UpdateTags(secret.Tags);
            existing.WithCreatedAt(DateTime.UtcNow);
        }
        else
        {
            existing.UpdateTags(secret.Tags);
            existing.ExpiresAt = secret.ExpiresAt;
            existing.WithUpdatedAt(DateTime.UtcNow);
        }

        if (value.Length > 0)
        {
            var encrypted = this.cipher.Encrypt(Encodings.Utf8NoBom.GetBytes(value));
            existing.Value = Convert.ToBase64String(encrypted);
        }

        this.Save();
    }

    public override void DeleteSecret(string name)
    {
        name = this.FormatName(name);
        this.secrets.TryRemove(name, out _);
    }

    public void Dispose()
    {
        if (this.cipher is IDisposable disposable)
            disposable.Dispose();

        if (this.disposableOptions is not null)
            this.disposableOptions.Dispose();
    }

    private void Save()
    {
        this.syncLock.Wait();
        try
        {
            var path = this.Options.Path;
            if (string.IsNullOrWhiteSpace(path))
                return;

            var records = this.secrets.Values.Select(x => new JsonSecretRecord(x)).ToArray();
            var json = JsonSerializer.Serialize(records);
            File.WriteAllText(path, json);
        }
        finally
        {
            this.syncLock.Release();
        }
    }

    private void Load()
    {
        if (this.secrets.Count > 0)
            return;

        this.syncLock.Wait();
        try
        {
            var path = this.Options.Path;
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);
            var records = JsonSerializer.Deserialize<JsonSecretRecord[]>(json);
            if (records is not null)
            {
                foreach (var record in records)
                {
                    this.secrets.TryAdd(record.Name, record);
                }
            }
        }
        finally
        {
            this.syncLock.Release();
        }
    }

    internal class JsonSecretRecord : SecretRecord
    {
        public JsonSecretRecord(string name)
            : base(name)
        {
        }

        public JsonSecretRecord(ISecretRecord record)
            : base(record)
        {
        }

        internal JsonSecretRecord WithUpdatedAt(DateTime? updatedAt)
        {
            this.UpdatedAt = updatedAt;
            return this;
        }

        internal JsonSecretRecord WithCreatedAt(DateTime? createdAt)
        {
            this.CreatedAt = createdAt;
            return this;
        }

        internal void UpdateTags(IDictionary<string, string?> tags)
        {
            this.Tags = new Dictionary<string, string?>(tags, StringComparer.OrdinalIgnoreCase);
        }
    }
}