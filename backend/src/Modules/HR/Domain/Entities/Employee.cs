using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.HR.Domain.Entities;

public enum EmploymentStatus { Active = 1, OnLeave = 2, Terminated = 3, Resigned = 4 }
public enum EmploymentType { FullTime = 1, PartTime = 2, Contract = 3, Intern = 4 }

public class Employee : BaseAuditableEntity
{
    public string EmployeeNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public long? DepartmentId { get; private set; }
    public string? Designation { get; private set; }
    public EmploymentStatus Status { get; private set; } = EmploymentStatus.Active;
    public EmploymentType EmploymentType { get; private set; } = EmploymentType.FullTime;
    public DateTime DateOfJoining { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public long? ManagerId { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public Department? Department { get; private set; }

    private Employee() { }

    public static Employee Create(string employeeNumber, string firstName, string lastName,
        string? email, string? phone, long? departmentId, string? designation,
        EmploymentStatus status, EmploymentType employmentType,
        DateTime dateOfJoining, DateTime? dateOfBirth, long? managerId, string? notes)
    {
        return new Employee
        {
            EmployeeNumber = employeeNumber,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            DepartmentId = departmentId,
            Designation = designation,
            Status = status,
            EmploymentType = employmentType,
            DateOfJoining = dateOfJoining,
            DateOfBirth = dateOfBirth,
            ManagerId = managerId,
            Notes = notes
        };
    }

    public void Update(string firstName, string lastName, string? email, string? phone,
        long? departmentId, string? designation, EmploymentStatus status,
        EmploymentType employmentType, DateTime dateOfJoining,
        DateTime? dateOfBirth, long? managerId, string? notes)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DepartmentId = departmentId;
        Designation = designation;
        Status = status;
        EmploymentType = employmentType;
        DateOfJoining = dateOfJoining;
        DateOfBirth = dateOfBirth;
        ManagerId = managerId;
        Notes = notes;
    }

    public void Terminate() => Status = EmploymentStatus.Terminated;
    public void Activate() => Status = EmploymentStatus.Active;
}
