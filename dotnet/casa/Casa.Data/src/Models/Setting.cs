namespace Bearz.Casa.Data.Models;

public class Setting
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public int EnvironmentId { get; set; }

    public Environment? Environment { get; set; }
}