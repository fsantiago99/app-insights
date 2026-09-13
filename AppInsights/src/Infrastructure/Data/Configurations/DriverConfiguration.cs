using AppInsights.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppInsights.Infrastructure.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.Property(d => d.Code).HasMaxLength(3).IsRequired();
        builder.Property(d => d.CarNumber).HasMaxLength(3);

        builder.HasIndex(d => d.Code).IsUnique();
    }
}
