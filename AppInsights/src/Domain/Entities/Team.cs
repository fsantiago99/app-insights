using AppInsights.Domain.Common;

namespace AppInsights.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Lap> Laps { get; set; } = [];
}
