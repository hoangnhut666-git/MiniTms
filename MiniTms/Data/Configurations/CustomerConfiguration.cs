using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniTms.Entities;

namespace MiniTms.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(e => e.CustomerId);

        builder.Property(e => e.Name).HasMaxLength(250).IsRequired();
        builder.Property(e => e.District).HasMaxLength(100).IsRequired();

        AuditEntityConfiguration.ConfigureAuditProperties(builder);

        builder.HasMany(e => e.Orders)
            .WithOne(e => e.Customer)
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
