using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniTms.Entities;

namespace MiniTms.Data.Configurations;

public class TripOrderConfiguration : IEntityTypeConfiguration<TripOrder>
{
    public void Configure(EntityTypeBuilder<TripOrder> builder)
    {
        builder.ToTable("TripOrders");

        builder.HasKey(e => e.TripOrderId);

        builder.HasIndex(e => new { e.TripId, e.OrderId }).IsUnique();
        builder.HasIndex(e => e.OrderId).IsUnique();

        builder.HasOne(e => e.Trip)
            .WithMany(e => e.TripOrders)
            .HasForeignKey(e => e.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Order)
            .WithOne(e => e.TripOrder)
            .HasForeignKey<TripOrder>(e => e.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
