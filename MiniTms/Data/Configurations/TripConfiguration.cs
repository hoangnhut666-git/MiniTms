using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniTms.Entities;

namespace MiniTms.Data.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("Trips");

        builder.HasKey(e => e.TripId);

        builder.Property(e => e.PlanCode).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Strategy).HasMaxLength(20).IsRequired();
        builder.Property(e => e.TotalCost).HasColumnType("decimal(12,0)");
        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(e => e.PlanCode);

        // FK required; navigation optional so soft-deleted vehicles do not break trip queries.
        builder.Navigation(e => e.Vehicle).IsRequired(false);
    }
}
