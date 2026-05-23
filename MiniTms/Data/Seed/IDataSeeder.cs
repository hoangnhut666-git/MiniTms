namespace MiniTms.Data.Seed;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
