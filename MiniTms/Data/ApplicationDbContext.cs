using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniTms.Data.Seed;
using MiniTms.Entities;
using MiniTms.Services;

namespace MiniTms.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ICurrentUserService currentUserService)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<RateCard> RateCards => Set<RateCard>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<TripOrder> TripOrders => Set<TripOrder>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditInformation();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        TmsDataSeeder.Seed(modelBuilder);
    }

    private void ApplyAuditInformation()
    {
        var userName = currentUserService.GetCurrentUserName();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ISoftDeletable softDeletable && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                softDeletable.MarkAsDeleted(userName);
                continue;
            }

            if (entry.Entity is not IAuditableEntity auditable)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    if (string.IsNullOrEmpty(auditable.CreatedBy))
                    {
                        auditable.MarkAsCreated(userName);
                    }
                    else if (auditable.CreatedAt == default)
                    {
                        auditable.MarkAsCreated(auditable.CreatedBy);
                    }

                    break;

                case EntityState.Modified:
                    if (entry.Entity is ISoftDeletable soft
                        && entry.Property(nameof(ISoftDeletable.IsDeleted)).IsModified
                        && soft.IsDeleted)
                    {
                        soft.MarkAsDeleted(userName);
                    }
                    else if (entry.Entity is not ISoftDeletable { IsDeleted: true })
                    {
                        auditable.MarkAsUpdated(userName);
                    }

                    break;
            }
        }
    }
}
