using AppInsights.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppInsights.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Driver> Drivers { get; }

    DbSet<Team> Teams { get; }

    DbSet<Lap> Laps { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
