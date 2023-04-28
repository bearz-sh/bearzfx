using DbSetting = Bearz.Casa.Data.Models.Setting;

namespace Bearz.Casa.Data.Services;

public class Setting
{
    private readonly DbSetting model;

    public Setting()
    {
        this.model = new DbSetting();
    }

    internal Setting(DbSetting model)
    {
        this.model = model;
    }

    public string Name
    {
        get => this.model.Name;
        set => this.model.Name = value;
    }

    public string Value
    {
        get => this.model.Value;
        set => this.model.Value = value;
    }

    internal DbSetting Model => this.model;
}