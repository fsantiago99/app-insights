using AppInsights.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppInsights.Infrastructure.Data.Configurations;

public class WeatherReadingConfiguration : IEntityTypeConfiguration<WeatherReading>
{
    public void Configure(EntityTypeBuilder<WeatherReading> builder)
    {
        builder.HasKey(w => w.LapId);

        builder.HasOne(w => w.Lap)
            .WithOne(l => l.Weather)
            .HasForeignKey<WeatherReading>(w => w.LapId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
