namespace Bearz.Casa.Data.Models;

public class Environment
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public List<Secret> Secrets { get; set; } = new();

    public List<Setting> Settings { get; set; } = new();
}