using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniTms.Entities;

namespace MiniTms.Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(e => e.VehicleId);

        builder.Property(e => e.Plate).HasMaxLength(20).IsRequired();
        builder.Property(e => e.VendorCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.VehicleType).HasMaxLength(20).IsRequired();
        builder.Property(e => e.IsActive).HasDefaultValue(true);

        AuditEntityConfiguration.ConfigureAuditProperties(builder);

        builder.HasMany(e => e.Trips)
            .WithOne(e => e.Vehicle)
            .HasForeignKey(e => e.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
