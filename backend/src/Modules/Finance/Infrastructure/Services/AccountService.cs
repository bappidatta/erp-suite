using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Finance.Application.Accounts;
using ErpSuite.Modules.Finance.Application.Accounts.Dtos;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Finance.Infrastructure.Services;

public sealed class AccountService : IAccountService
{
    private readonly ErpDbContext _dbContext;

    public AccountService(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<AccountResponse>> GetAccountsAsync(GetAccountsQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Accounts.AsNoTracking().Include(a => a.Parent).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(a =>
                a.Code.ToLower().Contains(term) ||
                a.Name.ToLower().Contains(term));
        }

        if (query.Type.HasValue)
            queryable = queryable.Where(a => a.Type == query.Type.Value);

        if (query.IsActive.HasValue)
            queryable = queryable.Where(a => a.IsActive == query.IsActive.Value);

        if (query.IsHeader.HasValue)
            queryable = queryable.Where(a => a.IsHeader == query.IsHeader.Value);

        if (query.ParentId.HasValue)
            queryable = queryable.Where(a => a.ParentId == query.ParentId.Value);

        queryable = query.SortBy?.ToLower() switch
        {
            "name" => query.SortDescending ? queryable.OrderByDescending(a => a.Name) : queryable.OrderBy(a => a.Name),
            "type" => query.SortDescending ? queryable.OrderByDescending(a => a.Type) : queryable.OrderBy(a => a.Type),
            _ => query.SortDescending ? queryable.OrderByDescending(a => a.Code) : queryable.OrderBy(a => a.Code)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AccountResponse>(
            items.Select(MapToResponse).ToList(),
            totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<AccountTreeNode>> GetAccountTreeAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.Code)
            .ToListAsync(cancellationToken);

        return BuildTree(accounts, null);
    }

    public async Task<AccountResponse?> GetAccountByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts
            .AsNoTracking()
            .Include(a => a.Parent)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return account is null ? null : MapToResponse(account);
    }

    public async Task<Result<AccountResponse>> CreateAccountAsync(CreateAccountRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.Accounts.AnyAsync(a => a.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<AccountResponse>("An account with this code already exists.");

        int level = 0;
        if (request.ParentId.HasValue)
        {
            var parent = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.ParentId.Value, cancellationToken);
            if (parent is null)
                return Result.Failure<AccountResponse>("Parent account not found.");
            level = parent.Level + 1;
        }

        var account = Account.Create(normalizedCode, request.Name, request.Type,
            request.ParentId, request.IsHeader, request.Description, level);

        account.SetAudit(currentUserId);
        _dbContext.Accounts.Add(account);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (request.ParentId.HasValue)
            await _dbContext.Entry(account).Reference(a => a.Parent).LoadAsync(cancellationToken);

        return Result.Success(MapToResponse(account));
    }

    public async Task<Result<AccountResponse>> UpdateAccountAsync(long id, UpdateAccountRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts
            .Include(a => a.Parent)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (account is null)
            return Result.Failure<AccountResponse>("Account not found.");

        int level = 0;
        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == id)
                return Result.Failure<AccountResponse>("An account cannot be its own parent.");

            var parent = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.ParentId.Value, cancellationToken);
            if (parent is null)
                return Result.Failure<AccountResponse>("Parent account not found.");
            level = parent.Level + 1;

            // Check for circular reference: walk up the parent chain
            var currentParentId = parent.ParentId;
            while (currentParentId.HasValue)
            {
                if (currentParentId.Value == id)
                    return Result.Failure<AccountResponse>("Circular parent reference detected.");
                var ancestor = await _dbContext.Accounts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == currentParentId.Value, cancellationToken);
                currentParentId = ancestor?.ParentId;
            }
        }

        account.Update(request.Name, request.Type, request.Description,
            request.ParentId, request.IsHeader, level);

        account.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (request.ParentId.HasValue)
            await _dbContext.Entry(account).Reference(a => a.Parent).LoadAsync(cancellationToken);

        return Result.Success(MapToResponse(account));
    }

    public async Task<Result> DeleteAccountAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (account is null)
            return Result.Failure("Account not found.");

        var hasChildren = await _dbContext.Accounts.AnyAsync(a => a.ParentId == id, cancellationToken);
        if (hasChildren)
            return Result.Failure("Cannot delete an account that has child accounts.");

        account.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateAccountAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (account is null) return Result.Failure("Account not found.");

        account.Activate();
        account.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateAccountAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (account is null) return Result.Failure("Account not found.");

        account.Deactivate();
        account.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static IReadOnlyList<AccountTreeNode> BuildTree(List<Account> allAccounts, long? parentId)
    {
        return allAccounts
            .Where(a => a.ParentId == parentId)
            .Select(a => new AccountTreeNode(
                a.Id, a.Code, a.Name, a.Type, a.Type.ToString(),
                a.IsHeader, a.Level, a.IsActive,
                BuildTree(allAccounts, a.Id)))
            .ToList();
    }

    private static AccountResponse MapToResponse(Account a) => new(
        a.Id, a.Code, a.Name, a.Type, a.Type.ToString(), a.Description,
        a.ParentId, a.Parent?.Name, a.IsHeader, a.Level, a.IsActive,
        a.CreatedAt, a.CreatedBy, a.UpdatedAt);
}
