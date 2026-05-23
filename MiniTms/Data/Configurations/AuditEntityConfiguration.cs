using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniTms.Entities;

namespace MiniTms.Data.Configurations;

internal static class AuditEntityConfiguration
{
    public static void ConfigureAuditProperties<T>(EntityTypeBuilder<T> builder)
        where T : AuditEntity
    {
        builder.Property(e => e.CreatedBy).HasMaxLength(256).IsRequired();
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);
        builder.Property(e => e.DeletedBy).HasMaxLength(256);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2");

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
