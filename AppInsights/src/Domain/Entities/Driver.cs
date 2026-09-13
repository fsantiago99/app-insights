using AppInsights.Domain.Common;

namespace AppInsights.Domain.Entities;

public class Driver : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string? CarNumber { get; set; }

    public ICollection<Lap> Laps { get; set; } = [];
}
