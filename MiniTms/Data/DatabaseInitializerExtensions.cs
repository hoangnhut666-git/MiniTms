using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniTms.Data.Seed;

namespace MiniTms.Data;

public static class DatabaseInitializerExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        if (!options.ApplyMigrationsOnStartup && !options.SeedOnStartup)
        {
            return;
        }

        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("MiniTms.DatabaseInitializer");

        if (options.ApplyMigrationsOnStartup)
        {
            logger.LogInformation("Applying EF Core migrations...");
            await context.Database.MigrateAsync();
        }

        if (options.SeedOnStartup)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
            await seeder.SeedAsync();
        }
    }
}
