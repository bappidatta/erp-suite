using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Sales.Application.Customers.Dtos;

namespace ErpSuite.Modules.Sales.Application.Customers;

public interface ICustomerService
{
    Task<PagedResult<CustomerResponse>> GetCustomersAsync(GetCustomersQuery query, CancellationToken cancellationToken = default);
    Task<CustomerResponse?> GetCustomerByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<CustomerResponse>> UpdateCustomerAsync(long id, UpdateCustomerRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteCustomerAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateCustomerAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateCustomerAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
