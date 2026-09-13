using AppInsights.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppInsights.Infrastructure.Data.Configurations;

public class LapConfiguration : IEntityTypeConfiguration<Lap>
{
    public void Configure(EntityTypeBuilder<Lap> builder)
    {
        builder.Property(l => l.MiniSector1).HasMaxLength(20);
        builder.Property(l => l.MiniSector2).HasMaxLength(20);
        builder.Property(l => l.MiniSector3).HasMaxLength(20);
        builder.Property(l => l.QualifyingSegment).HasMaxLength(10);
        builder.Property(l => l.TrackStatus).HasMaxLength(20);
        builder.Property(l => l.DeletedReason).HasMaxLength(200);

        builder.HasOne(l => l.Driver)
            .WithMany(d => d.Laps)
            .HasForeignKey(l => l.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Team)
            .WithMany(t => t.Laps)
            .HasForeignKey(l => l.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => new { l.DriverId, l.LapNumber, l.QualifyingSegment });
    }
}
