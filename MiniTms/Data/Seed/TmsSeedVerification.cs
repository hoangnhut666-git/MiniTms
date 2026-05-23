using Microsoft.EntityFrameworkCore;

namespace MiniTms.Data.Seed;

public sealed record TmsSeedCounts(
    int Customers,
    int Orders,
    int Vehicles,
    int RateCards,
    int Trips,
    int TripOrders);

public static class TmsSeedVerification
{
    public const int ExpectedCustomers = 8;
    public const int ExpectedOrders = 8;
    public const int ExpectedVehicles = 4;
    public const int ExpectedRateCards = 16;
    public const int ExpectedTrips = 0;
    public const int ExpectedTripOrders = 0;

    public static async Task<TmsSeedCounts> GetCountsAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        return new TmsSeedCounts(
            await context.Customers.IgnoreQueryFilters().CountAsync(cancellationToken),
            await context.Orders.IgnoreQueryFilters().CountAsync(cancellationToken),
            await context.Vehicles.IgnoreQueryFilters().CountAsync(cancellationToken),
            await context.RateCards.IgnoreQueryFilters().CountAsync(cancellationToken),
            await context.Trips.CountAsync(cancellationToken),
            await context.TripOrders.CountAsync(cancellationToken));
    }

    public static void EnsureExpected(TmsSeedCounts counts)
    {
        if (counts.Customers != ExpectedCustomers
            || counts.Orders != ExpectedOrders
            || counts.Vehicles != ExpectedVehicles
            || counts.RateCards != ExpectedRateCards
            || counts.Trips != ExpectedTrips
            || counts.TripOrders != ExpectedTripOrders)
        {
            throw new InvalidOperationException(
                $"Seed verification failed. Expected Customers={ExpectedCustomers}, Orders={ExpectedOrders}, " +
                $"Vehicles={ExpectedVehicles}, RateCards={ExpectedRateCards}, Trips={ExpectedTrips}, TripOrders={ExpectedTripOrders}. " +
                $"Actual Customers={counts.Customers}, Orders={counts.Orders}, Vehicles={counts.Vehicles}, " +
                $"RateCards={counts.RateCards}, Trips={counts.Trips}, TripOrders={counts.TripOrders}.");
        }
    }
}
