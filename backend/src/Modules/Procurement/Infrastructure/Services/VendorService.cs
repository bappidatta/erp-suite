using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Procurement.Application.Vendors;
using ErpSuite.Modules.Procurement.Application.Vendors.Dtos;
using ErpSuite.Modules.Procurement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Procurement.Infrastructure.Services;

public sealed class VendorService : IVendorService
{
    private readonly ErpDbContext _dbContext;

    public VendorService(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<VendorResponse>> GetVendorsAsync(GetVendorsQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Vendors.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(v =>
                v.Code.ToLower().Contains(term) ||
                v.Name.ToLower().Contains(term) ||
                (v.Email != null && v.Email.ToLower().Contains(term)));
        }

        if (query.IsActive.HasValue)
            queryable = queryable.Where(v => v.IsActive == query.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(query.Country))
            queryable = queryable.Where(v => v.Country == query.Country);

        queryable = query.SortBy?.ToLower() switch
        {
            "code" => query.SortDescending ? queryable.OrderByDescending(v => v.Code) : queryable.OrderBy(v => v.Code),
            "createdat" => query.SortDescending ? queryable.OrderByDescending(v => v.CreatedAt) : queryable.OrderBy(v => v.CreatedAt),
            _ => query.SortDescending ? queryable.OrderByDescending(v => v.Name) : queryable.OrderBy(v => v.Name)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<VendorResponse>(
            items.Select(MapToResponse).ToList(),
            totalCount, page, pageSize);
    }

    public async Task<VendorResponse?> GetVendorByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var vendor = await _dbContext.Vendors.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        return vendor is null ? null : MapToResponse(vendor);
    }

    public async Task<Result<VendorResponse>> CreateVendorAsync(CreateVendorRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.Vendors.AnyAsync(v => v.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<VendorResponse>("A vendor with this code already exists.");

        var vendor = Vendor.Create(normalizedCode, request.Name, request.ContactPerson,
            request.Email, request.Phone, request.Website, request.TaxId,
            request.AddressLine1, request.AddressLine2, request.City, request.State,
            request.PostalCode, request.Country, request.PaymentTerms, request.Currency,
            request.BankName, request.BankAccountNumber, request.BankRoutingNumber,
            request.BankSwiftCode, request.DefaultTaxCodeId, request.LeadTimeDays,
            request.Notes);

        vendor.SetAudit(currentUserId);
        _dbContext.Vendors.Add(vendor);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(vendor));
    }

    public async Task<Result<VendorResponse>> UpdateVendorAsync(long id, UpdateVendorRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var vendor = await _dbContext.Vendors.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (vendor is null)
            return Result.Failure<VendorResponse>("Vendor not found.");

        vendor.Update(request.Name, request.ContactPerson, request.Email, request.Phone,
            request.Website, request.TaxId, request.AddressLine1, request.AddressLine2,
            request.City, request.State, request.PostalCode, request.Country,
            request.PaymentTerms, request.Currency, request.BankName,
            request.BankAccountNumber, request.BankRoutingNumber, request.BankSwiftCode,
            request.DefaultTaxCodeId, request.LeadTimeDays, request.Notes);

        vendor.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(vendor));
    }

    public async Task<Result> DeleteVendorAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var vendor = await _dbContext.Vendors.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (vendor is null)
            return Result.Failure("Vendor not found.");

        vendor.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateVendorAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var vendor = await _dbContext.Vendors.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (vendor is null) return Result.Failure("Vendor not found.");

        vendor.Activate();
        vendor.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateVendorAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var vendor = await _dbContext.Vendors.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (vendor is null) return Result.Failure("Vendor not found.");

        vendor.Deactivate();
        vendor.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static VendorResponse MapToResponse(Vendor v) => new(
        v.Id, v.Code, v.Name, v.ContactPerson, v.Email, v.Phone, v.Website, v.TaxId,
        v.AddressLine1, v.AddressLine2, v.City, v.State, v.PostalCode, v.Country,
        v.PaymentTerms, v.Currency, v.BankName, v.BankAccountNumber, v.BankRoutingNumber,
        v.BankSwiftCode, v.DefaultTaxCodeId, v.LeadTimeDays, v.IsActive, v.Notes,
        v.CreatedAt, v.CreatedBy, v.UpdatedAt);
}
