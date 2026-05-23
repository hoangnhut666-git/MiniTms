namespace MiniTms.Data;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public bool ApplyMigrationsOnStartup { get; set; }
    public bool SeedOnStartup { get; set; }
}
