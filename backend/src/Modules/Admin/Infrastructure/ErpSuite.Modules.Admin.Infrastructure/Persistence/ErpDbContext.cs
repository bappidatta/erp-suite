using ErpSuite.BuildingBlocks.Infrastructure.Persistence;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Sales.Domain.Entities;
using ErpSuite.Modules.Procurement.Domain.Entities;
using ErpSuite.Modules.Finance.Domain.Entities;
using ErpSuite.Modules.Inventory.Domain.Entities;
using ErpSuite.Modules.HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Admin.Infrastructure.Persistence;

public class ErpDbContext : BaseDbContext
{
    public ErpDbContext(DbContextOptions<ErpDbContext> options) : base(options)
    {
    }

    // Admin
    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();
    public DbSet<OrganizationSettings> OrganizationSettings => Set<OrganizationSettings>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<NumberSequence> NumberSequences => Set<NumberSequence>();

    // Sales
    public DbSet<Customer> Customers => Set<Customer>();

    // Procurement
    public DbSet<Vendor> Vendors => Set<Vendor>();

    // Finance
    public DbSet<TaxCode> TaxCodes => Set<TaxCode>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<FinancialPeriod> FinancialPeriods => Set<FinancialPeriod>();

    // Inventory
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Item> Items => Set<Item>();

    // HR
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("public");

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(512).IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(128).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(128).IsRequired();
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<int>();
            entity.Property(e => e.MustChangePassword).HasColumnName("must_change_password");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.LoginAttempts).HasColumnName("login_attempts");
            entity.Property(e => e.LockedUntil).HasColumnName("locked_until");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Role entity
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(256);
            entity.Property(e => e.IsSystem).HasColumnName("is_system").HasDefaultValue(false);

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Permission entity
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(128).IsRequired();
            entity.Property(e => e.Module).HasColumnName("module").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(256);

            entity.HasIndex(e => new { e.Module, e.Action }).IsUnique();
        });

        // Configure RolePermission entity
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("role_permissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");

            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();

            // Match the global soft-delete filter on Role so EF doesn't warn about
            // a required navigation pointing to a potentially-filtered entity.
            entity.HasQueryFilter(rp => !rp.Role.IsDeleted);
        });

        // Configure RevokedToken entity
        modelBuilder.Entity<RevokedToken>(entity =>
        {
            entity.ToTable("revoked_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Jti).HasColumnName("jti").HasMaxLength(128).IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");

            entity.HasIndex(e => e.Jti).IsUnique();
            entity.HasIndex(e => e.ExpiresAt);
        });

        // Configure Company entity
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("companies");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.TaxId).HasColumnName("tax_id").HasMaxLength(100);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(256);
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
            entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(500);
            entity.Property(e => e.BaseCurrency).HasColumnName("base_currency").HasMaxLength(3);
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure OrganizationSettings entity
        modelBuilder.Entity<OrganizationSettings>(entity =>
        {
            entity.ToTable("organization_settings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyName).HasColumnName("company_name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.LegalName).HasColumnName("legal_name").HasMaxLength(256);
            entity.Property(e => e.RegistrationNumber).HasColumnName("registration_number").HasMaxLength(100);
            entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(500);
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(256);
            entity.Property(e => e.Website).HasColumnName("website").HasMaxLength(256);
            entity.Property(e => e.LogoPath).HasColumnName("logo_path").HasMaxLength(512);
            entity.Property(e => e.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("USD");
            entity.Property(e => e.FiscalYearStart).HasColumnName("fiscal_year_start").HasMaxLength(10);
            entity.Property(e => e.DateFormat).HasColumnName("date_format").HasMaxLength(50);
            entity.Property(e => e.TimeZone).HasColumnName("time_zone").HasMaxLength(100);
        });

        // Configure AuditLog entity
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Module).HasColumnName("module").HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityId).HasColumnName("entity_id").HasMaxLength(50);
            entity.Property(e => e.OldValues).HasColumnName("old_values").HasColumnType("jsonb");
            entity.Property(e => e.NewValues).HasColumnName("new_values").HasColumnType("jsonb");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(50);

            entity.HasIndex(e => e.Module);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<NumberSequence>(entity =>
        {
            entity.ToTable("number_sequences");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Module).HasColumnName("module").HasMaxLength(100).IsRequired();
            entity.Property(e => e.DocumentType).HasColumnName("document_type").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Prefix).HasColumnName("prefix").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Suffix).HasColumnName("suffix").HasMaxLength(50);
            entity.Property(e => e.StartingNumber).HasColumnName("starting_number");
            entity.Property(e => e.NextNumber).HasColumnName("next_number");
            entity.Property(e => e.PaddingLength).HasColumnName("padding_length");
            entity.Property(e => e.IncrementBy).HasColumnName("increment_by");
            entity.Property(e => e.ResetPolicy).HasColumnName("reset_policy").HasConversion<int>();
            entity.Property(e => e.LastResetOn).HasColumnName("last_reset_on");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);

            entity.HasIndex(e => new { e.Module, e.DocumentType }).IsUnique();
        });

        // ── Sales Entities ──

        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.ContactPerson).HasColumnName("contact_person").HasMaxLength(256);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(256);
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
            entity.Property(e => e.Website).HasColumnName("website").HasMaxLength(256);
            entity.Property(e => e.TaxId).HasColumnName("tax_id").HasMaxLength(100);
            entity.Property(e => e.AddressLine1).HasColumnName("address_line1").HasMaxLength(256);
            entity.Property(e => e.AddressLine2).HasColumnName("address_line2").HasMaxLength(256);
            entity.Property(e => e.City).HasColumnName("city").HasMaxLength(100);
            entity.Property(e => e.State).HasColumnName("state").HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasColumnName("postal_code").HasMaxLength(20);
            entity.Property(e => e.Country).HasColumnName("country").HasMaxLength(100);
            entity.Property(e => e.CreditLimit).HasColumnName("credit_limit").HasPrecision(18, 2);
            entity.Property(e => e.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("USD");
            entity.Property(e => e.PaymentTerms).HasColumnName("payment_terms").HasMaxLength(100);
            entity.Property(e => e.DefaultTaxCodeId).HasColumnName("default_tax_code_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
        });

        // ── Procurement Entities ──

        // Configure Vendor entity
        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.ToTable("vendors");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.ContactPerson).HasColumnName("contact_person").HasMaxLength(256);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(256);
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
            entity.Property(e => e.Website).HasColumnName("website").HasMaxLength(256);
            entity.Property(e => e.TaxId).HasColumnName("tax_id").HasMaxLength(100);
            entity.Property(e => e.AddressLine1).HasColumnName("address_line1").HasMaxLength(256);
            entity.Property(e => e.AddressLine2).HasColumnName("address_line2").HasMaxLength(256);
            entity.Property(e => e.City).HasColumnName("city").HasMaxLength(100);
            entity.Property(e => e.State).HasColumnName("state").HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasColumnName("postal_code").HasMaxLength(20);
            entity.Property(e => e.Country).HasColumnName("country").HasMaxLength(100);
            entity.Property(e => e.PaymentTerms).HasColumnName("payment_terms").HasMaxLength(100);
            entity.Property(e => e.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("USD");
            entity.Property(e => e.BankName).HasColumnName("bank_name").HasMaxLength(256);
            entity.Property(e => e.BankAccountNumber).HasColumnName("bank_account_number").HasMaxLength(50);
            entity.Property(e => e.BankRoutingNumber).HasColumnName("bank_routing_number").HasMaxLength(50);
            entity.Property(e => e.BankSwiftCode).HasColumnName("bank_swift_code").HasMaxLength(20);
            entity.Property(e => e.DefaultTaxCodeId).HasColumnName("default_tax_code_id");
            entity.Property(e => e.LeadTimeDays).HasColumnName("lead_time_days").HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
        });

        // ── Finance Entities ──

        // Configure TaxCode entity
        modelBuilder.Entity<TaxCode>(entity =>
        {
            entity.ToTable("tax_codes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Rate).HasColumnName("rate").HasPrecision(8, 4);
            entity.Property(e => e.Type).HasColumnName("type").HasConversion<int>();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.AppliesToSales).HasColumnName("applies_to_sales").HasDefaultValue(true);
            entity.Property(e => e.AppliesToPurchases).HasColumnName("applies_to_purchases").HasDefaultValue(true);

            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure Account (Chart of Accounts) entity
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Type).HasColumnName("type").HasConversion<int>();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.IsHeader).HasColumnName("is_header").HasDefaultValue(false);
            entity.Property(e => e.Level).HasColumnName("level").HasDefaultValue(0);

            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.ToTable("journal_entries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Number).HasColumnName("number").HasMaxLength(50).IsRequired();
            entity.Property(e => e.EntryDate).HasColumnName("entry_date");
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
            entity.Property(e => e.Reference).HasColumnName("reference").HasMaxLength(100);
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<int>();
            entity.Property(e => e.PostedAt).HasColumnName("posted_at");
            entity.Property(e => e.PostedBy).HasColumnName("posted_by").HasMaxLength(256);
            entity.Property(e => e.TotalDebit).HasColumnName("total_debit").HasPrecision(18, 2);
            entity.Property(e => e.TotalCredit).HasColumnName("total_credit").HasPrecision(18, 2);

            entity.HasIndex(e => e.Number).IsUnique();
            entity.HasIndex(e => e.EntryDate);
        });

        modelBuilder.Entity<JournalEntryLine>(entity =>
        {
            entity.ToTable("journal_entry_lines");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JournalEntryId).HasColumnName("journal_entry_id");
            entity.Property(e => e.LineNumber).HasColumnName("line_number");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.DebitAmount).HasColumnName("debit_amount").HasPrecision(18, 2);
            entity.Property(e => e.CreditAmount).HasColumnName("credit_amount").HasPrecision(18, 2);

            entity.HasOne(e => e.JournalEntry)
                .WithMany(e => e.Lines)
                .HasForeignKey(e => e.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FinancialPeriod>(entity =>
        {
            entity.ToTable("financial_periods");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<int>();
            entity.Property(e => e.ClosedAt).HasColumnName("closed_at");
            entity.Property(e => e.ClosedBy).HasColumnName("closed_by").HasMaxLength(256);

            entity.HasIndex(e => new { e.StartDate, e.EndDate });
        });

        // ── Inventory Entities ──

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.ParentCategoryId).HasColumnName("parent_category_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
        });

        // Configure UnitOfMeasure entity
        modelBuilder.Entity<UnitOfMeasure>(entity =>
        {
            entity.ToTable("units_of_measure");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(256);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);

            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure Warehouse entity
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("warehouses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Location).HasColumnName("location").HasMaxLength(256);
            entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(500);
            entity.Property(e => e.ContactPerson).HasColumnName("contact_person").HasMaxLength(256);
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);

            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure Item entity
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.UomId).HasColumnName("uom_id");
            entity.Property(e => e.Type).HasColumnName("type").HasConversion<int>().HasDefaultValue(ItemType.Product);
            entity.Property(e => e.ValuationMethod).HasColumnName("valuation_method").HasConversion<int>().HasDefaultValue(ValuationMethod.WeightedAverage);
            entity.Property(e => e.StandardCost).HasColumnName("standard_cost").HasPrecision(18, 4);
            entity.Property(e => e.SalePrice).HasColumnName("sale_price").HasPrecision(18, 4);
            entity.Property(e => e.ReorderLevel).HasColumnName("reorder_level").HasPrecision(18, 4);
            entity.Property(e => e.Barcode).HasColumnName("barcode").HasMaxLength(100);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(2000);
            entity.Property(e => e.ImagePath).HasColumnName("image_path").HasMaxLength(512);

            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Uom)
                .WithMany()
                .HasForeignKey(e => e.UomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── HR Entities ──

        // Configure Department entity
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.ParentDepartmentId).HasColumnName("parent_department_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);

            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure Employee entity
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmployeeNumber).HasColumnName("employee_number").HasMaxLength(50).IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(128).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(128).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(256);
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Designation).HasColumnName("designation").HasMaxLength(256);
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<int>().HasDefaultValue(EmploymentStatus.Active);
            entity.Property(e => e.EmploymentType).HasColumnName("employment_type").HasConversion<int>().HasDefaultValue(EmploymentType.FullTime);
            entity.Property(e => e.DateOfJoining).HasColumnName("date_of_joining");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(2000);

            entity.HasIndex(e => e.EmployeeNumber).IsUnique();

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
