using MiniTms.Services;

namespace MiniTms.Data;

/// <summary>
/// Used by EF Core design-time tools (migrations) when no HTTP context exists.
/// </summary>
internal sealed class DesignTimeCurrentUserService : ICurrentUserService
{
    public string GetCurrentUserName() => AuditUsers.System;
}
