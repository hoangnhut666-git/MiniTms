using Microsoft.EntityFrameworkCore;
using MiniTms.Entities;

namespace MiniTms.Data.Seed;

/// <summary>
/// Configures EF Core migration seed data via <see cref="ModelBuilder.HasData"/>.
/// </summary>
public static class TmsDataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasData(TmsSeedData.Customers);
        modelBuilder.Entity<Vehicle>().HasData(TmsSeedData.Vehicles);
        modelBuilder.Entity<RateCard>().HasData(TmsSeedData.RateCards);
        modelBuilder.Entity<Order>().HasData(TmsSeedData.Orders);
    }
}
