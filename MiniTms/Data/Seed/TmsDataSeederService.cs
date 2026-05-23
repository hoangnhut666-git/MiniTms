using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MiniTms.Data.Seed;

/// <summary>
/// Idempotent runtime seed for empty databases (CI, tests, dev without re-running migrations).
/// </summary>
public class TmsDataSeederService(
    ApplicationDbContext context,
    ILogger<TmsDataSeederService> logger) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var alreadySeeded = await context.Customers
            .IgnoreQueryFilters()
            .AnyAsync(cancellationToken);

        if (alreadySeeded)
        {
            logger.LogInformation("TMS seed skipped: master data already exists.");
            return;
        }

        logger.LogInformation("Seeding TMS master data from {Source}...", nameof(TmsSeedData));

        await context.Customers.AddRangeAsync(TmsSeedData.Customers, cancellationToken);
        await context.Vehicles.AddRangeAsync(TmsSeedData.Vehicles, cancellationToken);
        await context.RateCards.AddRangeAsync(TmsSeedData.RateCards, cancellationToken);
        await context.Orders.AddRangeAsync(TmsSeedData.Orders, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        var counts = await TmsSeedVerification.GetCountsAsync(context, cancellationToken);
        TmsSeedVerification.EnsureExpected(counts);
        logger.LogInformation(
            "TMS seed complete. Customers={Customers}, Orders={Orders}, Vehicles={Vehicles}, RateCards={RateCards}, Trips={Trips}, TripOrders={TripOrders}",
            counts.Customers,
            counts.Orders,
            counts.Vehicles,
            counts.RateCards,
            counts.Trips,
            counts.TripOrders);
    }
}
