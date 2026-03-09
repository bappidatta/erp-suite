using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Application.Organization;
using ErpSuite.Modules.Admin.Application.Organization.Dtos;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Admin.Infrastructure.Services;

public sealed class OrganizationSettingsService : IOrganizationSettingsService
{
    private readonly ErpDbContext _dbContext;

    public OrganizationSettingsService(ErpDbContext dbContext) => _dbContext = dbContext;

    public async Task<OrganizationSettingsResponse?> GetAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.OrganizationSettings
            .FirstOrDefaultAsync(cancellationToken);

        return settings is null ? null : MapToResponse(settings);
    }

    public async Task<Result<OrganizationSettingsResponse>> UpdateAsync(
        UpdateOrganizationSettingsRequest request,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.OrganizationSettings
            .FirstOrDefaultAsync(cancellationToken);

        if (settings is null)
        {
            settings = OrganizationSettings.Create(request.CompanyName);
            _dbContext.OrganizationSettings.Add(settings);
        }

        settings.Update(
            request.CompanyName,
            request.LegalName,
            request.RegistrationNumber,
            request.Address,
            request.Phone,
            request.Email,
            request.Website,
            request.Currency,
            request.FiscalYearStart,
            request.DateFormat,
            request.TimeZone);

        settings.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(settings));
    }

    public async Task<Result<string>> UploadLogoAsync(Stream logoStream, string fileName, string currentUserId, CancellationToken cancellationToken = default)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return Result.Failure<string>("Only JPG, PNG, and SVG files are allowed.");

        if (logoStream.Length > 5 * 1024 * 1024)
            return Result.Failure<string>("Logo file size must not exceed 5MB.");

        var uploadDir = Path.Combine("uploads", "logos");
        Directory.CreateDirectory(uploadDir);

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadDir, uniqueFileName);

        await using var fileStream = File.Create(filePath);
        await logoStream.CopyToAsync(fileStream, cancellationToken);

        var settings = await _dbContext.OrganizationSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings is null)
        {
            settings = OrganizationSettings.Create("My Organization");
            _dbContext.OrganizationSettings.Add(settings);
        }

        settings.SetLogoPath($"/{filePath.Replace('\\', '/')}");
        settings.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(settings.LogoPath!);
    }

    private static OrganizationSettingsResponse MapToResponse(OrganizationSettings s) => new(
        s.Id, s.CompanyName, s.LegalName, s.RegistrationNumber,
        s.Address, s.Phone, s.Email, s.Website, s.LogoPath,
        s.Currency, s.FiscalYearStart, s.DateFormat, s.TimeZone,
        s.UpdatedAt);
}
