using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public bool MustChangePassword { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int LoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public long? DepartmentId { get; private set; }
    public long? ManagerId { get; private set; }
    public long RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    public string FullName => $"{FirstName} {LastName}".Trim();

    private User() { } // EF Core

    public static User Create(string email, string passwordHash, string firstName, string lastName, long roleId, string? phone = null, bool mustChangePassword = false)
    {
        return new User
        {
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            Status = UserStatus.Active,
            RoleId = roleId,
            MustChangePassword = mustChangePassword,
            LoginAttempts = 0
        };
    }

    public void UpdateProfile(string firstName, string lastName, string? phone, long? departmentId, long? managerId)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        DepartmentId = departmentId;
        ManagerId = managerId;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        MustChangePassword = false;
        LoginAttempts = 0;
        LockedUntil = null;
    }

    public void ChangeStatus(UserStatus status)
    {
        Status = status;
        if (status == UserStatus.Active)
            LockedUntil = null;
    }

    public void Activate() => Status = UserStatus.Active;
    public void Deactivate() => Status = UserStatus.Inactive;
    public void Lock() => Status = UserStatus.Locked;
    public void Suspend() => Status = UserStatus.Suspended;

    public void AssignRole(long roleId) => RoleId = roleId;

    public void RecordFailedLogin()
    {
        LoginAttempts++;
        if (LoginAttempts >= 5)
        {
            Status = UserStatus.Locked;
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        }
    }

    public void RecordSuccessfulLogin()
    {
        LoginAttempts = 0;
        LastLoginAt = DateTime.UtcNow;
        if (Status == UserStatus.Locked && LockedUntil <= DateTime.UtcNow)
            Status = UserStatus.Active;
    }
}
