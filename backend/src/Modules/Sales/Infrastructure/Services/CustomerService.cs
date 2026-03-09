using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Sales.Application.Customers;
using ErpSuite.Modules.Sales.Application.Customers.Dtos;
using ErpSuite.Modules.Sales.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Sales.Infrastructure.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ErpDbContext _dbContext;

    public CustomerService(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CustomerResponse>> GetCustomersAsync(GetCustomersQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(c =>
                c.Code.ToLower().Contains(term) ||
                c.Name.ToLower().Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term)));
        }

        if (query.IsActive.HasValue)
            queryable = queryable.Where(c => c.IsActive == query.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(query.Country))
            queryable = queryable.Where(c => c.Country == query.Country);

        queryable = query.SortBy?.ToLower() switch
        {
            "code" => query.SortDescending ? queryable.OrderByDescending(c => c.Code) : queryable.OrderBy(c => c.Code),
            "createdat" => query.SortDescending ? queryable.OrderByDescending(c => c.CreatedAt) : queryable.OrderBy(c => c.CreatedAt),
            "creditlimit" => query.SortDescending ? queryable.OrderByDescending(c => c.CreditLimit) : queryable.OrderBy(c => c.CreditLimit),
            _ => query.SortDescending ? queryable.OrderByDescending(c => c.Name) : queryable.OrderBy(c => c.Name)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CustomerResponse>(
            items.Select(MapToResponse).ToList(),
            totalCount, page, pageSize);
    }

    public async Task<CustomerResponse?> GetCustomerByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return customer is null ? null : MapToResponse(customer);
    }

    public async Task<Result<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.Customers.AnyAsync(c => c.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<CustomerResponse>("A customer with this code already exists.");

        var customer = Customer.Create(normalizedCode, request.Name, request.ContactPerson,
            request.Email, request.Phone, request.Website, request.TaxId,
            request.AddressLine1, request.AddressLine2, request.City, request.State,
            request.PostalCode, request.Country, request.CreditLimit, request.Currency,
            request.PaymentTerms, request.DefaultTaxCodeId, request.Notes);

        customer.SetAudit(currentUserId);
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(customer));
    }

    public async Task<Result<CustomerResponse>> UpdateCustomerAsync(long id, UpdateCustomerRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (customer is null)
            return Result.Failure<CustomerResponse>("Customer not found.");

        customer.Update(request.Name, request.ContactPerson, request.Email, request.Phone,
            request.Website, request.TaxId, request.AddressLine1, request.AddressLine2,
            request.City, request.State, request.PostalCode, request.Country,
            request.CreditLimit, request.Currency, request.PaymentTerms,
            request.DefaultTaxCodeId, request.Notes);

        customer.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(customer));
    }

    public async Task<Result> DeleteCustomerAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (customer is null)
            return Result.Failure("Customer not found.");

        customer.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateCustomerAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (customer is null) return Result.Failure("Customer not found.");

        customer.Activate();
        customer.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateCustomerAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (customer is null) return Result.Failure("Customer not found.");

        customer.Deactivate();
        customer.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static CustomerResponse MapToResponse(Customer c) => new(
        c.Id, c.Code, c.Name, c.ContactPerson, c.Email, c.Phone, c.Website, c.TaxId,
        c.AddressLine1, c.AddressLine2, c.City, c.State, c.PostalCode, c.Country,
        c.CreditLimit, c.Currency, c.PaymentTerms, c.DefaultTaxCodeId, c.IsActive,
        c.Notes, c.CreatedAt, c.CreatedBy, c.UpdatedAt);
}
