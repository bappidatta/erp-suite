using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public bool MustChangePassword { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public long RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    private User() { } // EF Core

    public static User Create(string email, string passwordHash, string firstName, string lastName, long roleId, bool mustChangePassword = false)
    {
        return new User
        {
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            RoleId = roleId,
            MustChangePassword = mustChangePassword
        };
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        MustChangePassword = false;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}
