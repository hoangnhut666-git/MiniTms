using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniTms.Entities;

namespace MiniTms.Data.Configurations;

public class RateCardConfiguration : IEntityTypeConfiguration<RateCard>
{
    public void Configure(EntityTypeBuilder<RateCard> builder)
    {
        builder.ToTable("RateCard");

        builder.HasKey(e => e.RateCardId);

        builder.Property(e => e.VendorCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.ToDistrict).HasMaxLength(100).IsRequired();
        builder.Property(e => e.VehicleType).HasMaxLength(20).IsRequired();
        builder.Property(e => e.BaseCost).HasColumnType("decimal(12,0)");
        builder.Property(e => e.DropFee).HasColumnType("decimal(12,0)");
        builder.Property(e => e.IsActive).HasDefaultValue(true);

        AuditEntityConfiguration.ConfigureAuditProperties(builder);

        builder.HasIndex(e => new { e.VendorCode, e.ToDistrict, e.VehicleType })
            .IsUnique();
    }
}
