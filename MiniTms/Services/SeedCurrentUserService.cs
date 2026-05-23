namespace MiniTms.Services;

/// <summary>
/// Returns the seed audit user (tests/CI seeding without HTTP context).
/// </summary>
public sealed class SeedCurrentUserService : ICurrentUserService
{
    public string GetCurrentUserName() => AuditUsers.Seed;
}
