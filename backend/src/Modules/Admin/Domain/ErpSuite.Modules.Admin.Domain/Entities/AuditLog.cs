using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class AuditLog : BaseEntity
{
    public long? UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Module { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }

    private AuditLog() { } // EF Core

    public static AuditLog Create(
        long? userId,
        string action,
        string module,
        string? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            Module = module,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress
        };
    }
}
