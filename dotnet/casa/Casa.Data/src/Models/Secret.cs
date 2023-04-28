using System.ComponentModel.DataAnnotations.Schema;

namespace Bearz.Casa.Data.Models;

public class Secret
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int EnvironmentId { get; set; }

    [ForeignKey(nameof(EnvironmentId))]
    public Environment? Environment { get; set; }
}