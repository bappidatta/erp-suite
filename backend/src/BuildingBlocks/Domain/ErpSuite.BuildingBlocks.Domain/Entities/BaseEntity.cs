namespace ErpSuite.BuildingBlocks.Domain.Entities;

public abstract class BaseEntity
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public DateTime? UpdatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public int Version { get; protected set; }

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void SetAudit(string userId)
    {
        if (string.IsNullOrEmpty(CreatedBy))
        {
            CreatedBy = userId;
            CreatedAt = DateTime.UtcNow;
        }
        else
        {
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }
        Version++;
    }
}
