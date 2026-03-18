using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Admin.Application.Users.Dtos;

namespace ErpSuite.Modules.Admin.Application.Users;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default);
    Task<UserResponse?> GetUserByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> UpdateUserAsync(long id, UpdateUserRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateUserAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateUserAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
