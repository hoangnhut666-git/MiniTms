using MiniTms.Data.Seed;

namespace MiniTms.Tests.Data.Seed;

public class TmsSeedDataTests
{
    [Fact]
    public void Catalog_matches_seed_sql_row_counts()
    {
        Assert.Equal(TmsSeedVerification.ExpectedCustomers, TmsSeedData.Customers.Count);
        Assert.Equal(TmsSeedVerification.ExpectedOrders, TmsSeedData.Orders.Count);
        Assert.Equal(TmsSeedVerification.ExpectedVehicles, TmsSeedData.Vehicles.Count);
        Assert.Equal(TmsSeedVerification.ExpectedRateCards, TmsSeedData.RateCards.Count);
    }

    [Fact]
    public void Orders_reference_existing_customers()
    {
        var customerIds = TmsSeedData.Customers.Select(c => c.CustomerId).ToHashSet();
        Assert.All(TmsSeedData.Orders, o => Assert.Contains(o.CustomerId, customerIds));
    }
}
