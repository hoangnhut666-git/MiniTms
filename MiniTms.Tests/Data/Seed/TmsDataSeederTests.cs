using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiniTms.Data;
using MiniTms.Data.Seed;
using MiniTms.Services;

namespace MiniTms.Tests.Data.Seed;

public class TmsDataSeederTests
{
    [Fact]
    public async Task SeedAsync_populates_expected_master_data_and_is_idempotent()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"MiniTms_{Guid.NewGuid()}")
            .Options;

        await using var context = new ApplicationDbContext(options, new SeedCurrentUserService());
        var seeder = new TmsDataSeederService(context, NullLogger<TmsDataSeederService>.Instance);

        await seeder.SeedAsync();

        var counts = await TmsSeedVerification.GetCountsAsync(context);
        TmsSeedVerification.EnsureExpected(counts);

        Assert.Equal(8, await context.Customers.CountAsync());
        Assert.Equal(4, await context.Vehicles.CountAsync());
        Assert.Equal(3, await context.Vehicles.CountAsync(v => v.IsActive));
        Assert.False(await context.Vehicles.AnyAsync(v => v.Plate == "29C-444.44" && v.IsActive));

        await seeder.SeedAsync();

        var countsAfterSecondRun = await TmsSeedVerification.GetCountsAsync(context);
        Assert.Equal(counts, countsAfterSecondRun);
    }
}
