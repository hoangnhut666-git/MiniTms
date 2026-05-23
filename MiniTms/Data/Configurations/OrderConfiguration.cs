using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniTms.Entities;

namespace MiniTms.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(e => e.OrderId);

        builder.Property(e => e.Status).HasMaxLength(50).IsRequired().HasDefaultValue(OrderStatus.New);

        AuditEntityConfiguration.ConfigureAuditProperties(builder);

        builder.HasIndex(e => new { e.OrderDate, e.Status });
    }
}
