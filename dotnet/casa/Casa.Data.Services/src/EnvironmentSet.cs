using Bearz.Security.Cryptography;

using Microsoft.EntityFrameworkCore;

using CasaDbContext = Bearz.Casa.Data.Models.CasaDbContext;
using DbEnvironment = Bearz.Casa.Data.Models.Environment;
using DbSecret = Bearz.Casa.Data.Models.Secret;
using DbSetting = Bearz.Casa.Data.Models.Setting;

namespace Bearz.Casa.Data.Services;

public class EnvironmentSet
{
    private readonly CasaDbContext db;

    private readonly IEncryptionProvider cipher;

    public EnvironmentSet(CasaDbContext db, IEncryptionProvider cipher)
    {
        this.db = db;
        this.cipher = cipher;
    }

    public DbEnvironment Create(string name)
    {
        var lowered = name.ToLower();
        var entity = new DbEnvironment { Name = lowered, DisplayName = lowered, };
        this.db.Set<DbEnvironment>().Add(entity);
        this.db.SaveChanges();
        return entity;
    }

    public bool Delete(string name)
    {
        var lowered = name.ToLower();
        var entity = this.db.Set<DbEnvironment>()
            .Include(o => o.Secrets)
            .Include(o => o.Settings)
            .AsSplitQuery().FirstOrDefault(x => x.Name == lowered);

        if (entity is null)
            return false;

        this.db.Set<DbEnvironment>().Remove(entity);
        this.db.SaveChanges();
        return true;
    }

    public Environment GetOrCreate(string name)
    {
        var lowered = name.ToLower();
        var entity = this.db.Set<DbEnvironment>()
            .Include(o => o.Settings)
            .Include(o => o.Secrets)
            .AsSplitQuery()
            .FirstOrDefault(o => o.Name == lowered);
        if (entity is not null)
            return new Environment(this.db, entity, this.cipher);

        entity = new DbEnvironment { DisplayName = name, Name = lowered, };
        this.db.Set<DbEnvironment>().Add(entity);
        this.db.SaveChanges();
        return new Environment(this.db, entity, this.cipher);
    }

    public Environment? Get(string name)
    {
        var lowered = name.ToLower();
        var entity = this.db.Set<DbEnvironment>()
            .Include(o => o.Settings)
            .Include(o => o.Secrets)
            .AsSplitQuery()
            .FirstOrDefault(o => o.Name == lowered);
        if (entity is not null)
            return new Environment(this.db, entity, this.cipher);

        return null;
    }

    public IEnumerable<string> ListNames()
    {
        return this.db.Set<DbEnvironment>().Select(o => o.Name);
    }
}