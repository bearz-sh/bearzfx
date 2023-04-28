using Bearz.Extensions.Secrets;

namespace Bearz.Casa.Data.Services;

public class CasaSecretVault : ISecretVault
{
    private readonly Environment env;

    public CasaSecretVault(Environment env)
    {
        this.env = env;
    }

    public bool SupportsSynchronous => true;

    public string Name => this.env.Name;

    public string Kind => "casa";

    public Task<IEnumerable<string>> ListNamesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(this.ListNames());

    public IEnumerable<string> ListNames()
    {
        return this.env.Secrets.Select(o => o.Name)
            .ToList();
    }

    public ISecretRecord CreateRecord(string name)
    {
        return new CasaSecret(name);
    }

    public Task<string?> GetSecretValueAsync(string name, CancellationToken cancellationToken = default)
        => Task.FromResult(this.env.GetSecret(name));

    public string? GetSecretValue(string name)
        => this.env.GetSecret(name);

    public Task<ISecretRecord?> GetSecretAsync(string name, CancellationToken cancellationToken = default)
        => Task.FromResult(this.GetSecret(name));

    public ISecretRecord? GetSecret(string name)
    {
        var record = this.env.GetSecretRecord(name);
        if (record is null)
            return null;

        return new CasaSecret(record.Name)
        {
            Value = record.Name,
            ExpiresAt = record.ExpiresAt,
        };
    }

    public Task SetSecretValueAsync(string name, string secret, CancellationToken cancellationToken = default)
        => this.env.SetSecretAsync(name, secret, cancellationToken);

    public void SetSecretValue(string name, string secret)
        => this.env.SetSecret(name, secret);

    public Task SetSecretAsync(ISecretRecord secret, CancellationToken cancellationToken = default)
        => this.env.SetSecretAsync(secret.Name, secret.Value, secret.ExpiresAt, cancellationToken);

    public void SetSecret(ISecretRecord secret)
    {
        this.env.SetSecret(secret.Name, secret.Value, secret.ExpiresAt);
    }

    public Task DeleteSecretAsync(string name, CancellationToken cancellationToken = default)
        => this.env.DeleteSecretAsync(name, cancellationToken);

    public void DeleteSecret(string name)
    {
        this.env.DeleteSecret(name);
    }

    private sealed class CasaSecret : SecretRecord
    {
        public CasaSecret(string name)
            : base(name)
        {
            this.Tags = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }

        public CasaSecret(ISecretRecord record)
            : base(record)
        {
            this.Tags = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }
    }
}