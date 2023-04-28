using Bearz.Security.Cryptography;
using Bearz.Text;

using Microsoft.EntityFrameworkCore;

using DbEnvironment = Bearz.Casa.Data.Models.Environment;
using DbSecret = Bearz.Casa.Data.Models.Secret;
using DbSetting = Bearz.Casa.Data.Models.Setting;

namespace Bearz.Casa.Data.Services;

public class Environment
{
    private readonly Dictionary<string, Secret> secrets = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Setting> settings = new(StringComparer.OrdinalIgnoreCase);
    private readonly DbEnvironment model;
    private readonly DbContext db;
    private readonly IEncryptionProvider cipher;

    internal Environment(DbContext db, DbEnvironment model, IEncryptionProvider cipher)
    {
        this.db = db;
        this.model = model;
        this.cipher = cipher;
        foreach (var secret in model.Secrets)
        {
            this.secrets.Add(secret.Name, new Secret(secret, this.cipher));
        }

        foreach (var variable in model.Settings)
        {
            this.settings.Add(variable.Name, new Setting(variable));
        }
    }

    public string Name => this.model.Name;

    public IEnumerable<Secret> Secrets => this.secrets.Values;

    public IEnumerable<Setting> Settings => this.settings.Values;

    public void DeleteSecret(string name)
    {
        name = NormalizeEnvName(name);
        if (this.secrets.TryGetValue(name, out var secret))
        {
            this.db.Remove(secret.Model);
            this.db.SaveChanges();
            this.secrets.Remove(name);
        }
    }

    public async Task DeleteSecretAsync(string name, CancellationToken cancellationToken = default)
    {
        name = NormalizeEnvName(name);
        if (this.secrets.TryGetValue(name, out var secret))
        {
            this.db.Remove(secret.Model);
            await this.db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            this.secrets.Remove(name);
        }
    }

    public void DeleteVariable(string name)
    {
        name = NormalizeEnvName(name);
        if (this.settings.TryGetValue(name, out var variable))
        {
            this.db.Remove(variable.Model);
            this.db.SaveChanges();
            this.settings.Remove(name);
        }
    }

    public string? GetSecret(string name)
    {
        name = NormalizeEnvName(name);

        if (this.secrets.TryGetValue(name, out var secret))
        {
            return secret.Value;
        }

        return null;
    }

    public Secret? GetSecretRecord(string name)
    {
        name = NormalizeEnvName(name);

        if (this.secrets.TryGetValue(name, out var secret))
        {
            return secret;
        }

        return null;
    }

    public string? GetVariable(string name)
    {
        name = NormalizeEnvName(name);
        if (this.settings.TryGetValue(name, out var variable))
        {
            return variable.Value;
        }

        return null;
    }

    public void SetSecret(string name, string value)
    {
        name = NormalizeEnvName(name);
        if (!this.secrets.TryGetValue(name, out var secret))
        {
            var dbSecret = new DbSecret
            {
                Name = name,
                EnvironmentId = this.model.Id,
            };
            secret = new Secret(dbSecret, this.cipher);
            this.db.Add(dbSecret);
        }

        secret.Value = value;
        this.db.SaveChanges();
        this.secrets[name] = secret;
    }

    public void SetSecret(string name, string value, DateTime? expiresAt)
    {
        name = NormalizeEnvName(name);
        if (!this.secrets.TryGetValue(name, out var secret))
        {
            var dbSecret = new DbSecret
            {
                Name = name,
                EnvironmentId = this.model.Id,
            };
            secret = new Secret(dbSecret, this.cipher);
            this.db.Add(dbSecret);
        }

        secret.Value = value;
        secret.ExpiresAt = expiresAt;

        this.db.SaveChanges();
        this.secrets[name] = secret;
    }

    public async Task SetSecretAsync(
        string name,
        string value,
        CancellationToken cancellationToken = default)
    {
        name = NormalizeEnvName(name);
        if (!this.secrets.TryGetValue(name, out var secret))
        {
            var dbSecret = new DbSecret
            {
                Name = name,
                EnvironmentId = this.model.Id,
            };
            secret = new Secret(dbSecret, this.cipher);
            this.db.Add(dbSecret);
        }

        secret.Value = value;

        await this.db.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        this.secrets[name] = secret;
    }

    public async Task SetSecretAsync(
        string name,
        string value,
        DateTime? expiresAt,
        CancellationToken cancellationToken = default)
    {
        name = NormalizeEnvName(name);
        if (!this.secrets.TryGetValue(name, out var secret))
        {
            var dbSecret = new DbSecret
            {
                Name = name,
                EnvironmentId = this.model.Id,
            };
            secret = new Secret(dbSecret, this.cipher);
            this.db.Add(dbSecret);
        }

        secret.Value = value;
        secret.ExpiresAt = expiresAt;

        await this.db.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        this.secrets[name] = secret;
    }

    public void SetSetting(string name, string value)
    {
        name = NormalizeEnvName(name);
        if (this.settings.TryGetValue(name, out var variable))
        {
            variable.Value = value;
        }
        else
        {
            var dbVariable = new DbSetting
            {
                Name = name,
                Value = value,
                EnvironmentId = this.model.Id,
            };

            this.db.Add(dbVariable);
            variable = new Setting(dbVariable);
        }

        this.db.SaveChanges();
        this.settings[name] = variable;
    }

    public void SetSetting(Setting setting)
    {
        setting.Name = NormalizeEnvName(setting.Name);
        if (!this.settings.TryGetValue(setting.Name, out var model))
        {
            var entity = new DbSetting();
            entity.Environment = this.model;
            model = new Setting(entity);

            this.settings.Add(setting.Name, model);
        }

        model.Value = setting.Value;

        this.db.SaveChanges();
        this.settings[setting.Name] = model;
    }

    protected void Save()
    {
        this.db.SaveChanges();
    }

    private static string NormalizeEnvName(ReadOnlySpan<char> name)
    {
        var sb = StringBuilderCache.Acquire();
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(char.ToUpperInvariant(c));
            }

            if (c is '_' or '-' or '/' or ':' or '.' or ' ')
            {
                sb.Append('_');
            }
        }

        return StringBuilderCache.GetStringAndRelease(sb);
    }
}