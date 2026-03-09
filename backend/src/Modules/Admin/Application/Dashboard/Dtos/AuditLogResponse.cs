namespace ErpSuite.Modules.Admin.Application.Dashboard.Dtos;

public record AuditLogResponse(
    long Id,
    long? UserId,
    string? UserName,
    string Action,
    string Module,
    string? EntityId,
    string? OldValues,
    string? NewValues,
    string? IpAddress,
    DateTime CreatedAt);
