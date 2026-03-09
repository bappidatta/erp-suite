using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Application.Organization.Dtos;

namespace ErpSuite.Modules.Admin.Application.Organization;

public interface IOrganizationSettingsService
{
    Task<OrganizationSettingsResponse?> GetAsync(CancellationToken cancellationToken = default);
    Task<Result<OrganizationSettingsResponse>> UpdateAsync(UpdateOrganizationSettingsRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<string>> UploadLogoAsync(Stream logoStream, string fileName, string currentUserId, CancellationToken cancellationToken = default);
}
