using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Finance.Application.Accounts.Dtos;
using ErpSuite.BuildingBlocks.Application.Common;

namespace ErpSuite.Modules.Finance.Application.Accounts;

public interface IAccountService
{
    Task<PagedResult<AccountResponse>> GetAccountsAsync(GetAccountsQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountTreeNode>> GetAccountTreeAsync(CancellationToken cancellationToken = default);
    Task<AccountResponse?> GetAccountByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<AccountResponse>> CreateAccountAsync(CreateAccountRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<AccountResponse>> UpdateAccountAsync(long id, UpdateAccountRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAccountAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateAccountAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateAccountAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
