using System.Linq.Expressions;
using ErpSuite.BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.BuildingBlocks.Infrastructure.Persistence;

public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply soft delete query filter globally
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseAuditableEntity.IsDeleted));
                var filterExpression = Expression.Lambda(Expression.Not(property), parameter);
                entityType.SetQueryFilter(filterExpression);
            }
        }

        // Configure audit columns with common conventions
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.CreatedAt))
                    .HasColumnName("created_at");

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.CreatedBy))
                    .HasColumnName("created_by")
                    .HasMaxLength(256);

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.UpdatedAt))
                    .HasColumnName("updated_at");

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.UpdatedBy))
                    .HasColumnName("updated_by")
                    .HasMaxLength(256);

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.Version))
                    .HasColumnName("version")
                    .IsConcurrencyToken();
            }

            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseAuditableEntity.IsDeleted))
                    .HasColumnName("is_deleted");

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseAuditableEntity.DeletedAt))
                    .HasColumnName("deleted_at");

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseAuditableEntity.DeletedBy))
                    .HasColumnName("deleted_by")
                    .HasMaxLength(256);
            }
        }
    }
}
