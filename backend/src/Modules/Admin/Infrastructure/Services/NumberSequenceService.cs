using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Application.NumberSequences;
using ErpSuite.Modules.Admin.Application.NumberSequences.Dtos;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Admin.Infrastructure.Services;

public sealed class NumberSequenceService : INumberSequenceService
{
    private const int MaxRetryAttempts = 3;
    private readonly ErpDbContext _dbContext;

    public NumberSequenceService(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<NumberSequenceResponse>> GetNumberSequencesAsync(GetNumberSequencesQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.NumberSequences
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLowerInvariant();
            queryable = queryable.Where(x =>
                x.Module.ToLower().Contains(term) ||
                x.DocumentType.ToLower().Contains(term) ||
                x.Prefix.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Module))
        {
            var module = query.Module.Trim().ToLowerInvariant();
            queryable = queryable.Where(x => x.Module.ToLower() == module);
        }

        if (!string.IsNullOrWhiteSpace(query.DocumentType))
        {
            var documentType = query.DocumentType.Trim().ToLowerInvariant();
            queryable = queryable.Where(x => x.DocumentType.ToLower() == documentType);
        }

        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(x => x.IsActive == query.IsActive.Value);
        }

        queryable = query.SortBy?.ToLowerInvariant() switch
        {
            "documenttype" => query.SortDescending ? queryable.OrderByDescending(x => x.DocumentType) : queryable.OrderBy(x => x.DocumentType),
            "nextnumber" => query.SortDescending ? queryable.OrderByDescending(x => x.NextNumber) : queryable.OrderBy(x => x.NextNumber),
            _ => query.SortDescending
                ? queryable.OrderByDescending(x => x.Module).ThenByDescending(x => x.DocumentType)
                : queryable.OrderBy(x => x.Module).ThenBy(x => x.DocumentType)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<NumberSequenceResponse>(
            items.Select(MapToResponse).ToList(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<NumberSequenceResponse?> GetNumberSequenceByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var sequence = await _dbContext.NumberSequences
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return sequence is null ? null : MapToResponse(sequence);
    }

    public async Task<Result<NumberSequenceResponse>> CreateNumberSequenceAsync(CreateNumberSequenceRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var module = request.Module.Trim();
        var documentType = request.DocumentType.Trim();
        var prefix = request.Prefix.Trim().ToUpperInvariant();
        var suffix = request.Suffix?.Trim().ToUpperInvariant();

        if (await ExistsAsync(module, documentType, null, cancellationToken))
        {
            return Result.Failure<NumberSequenceResponse>("A number sequence for this module and document type already exists.");
        }

        var sequence = NumberSequence.Create(
            module,
            documentType,
            prefix,
            request.StartingNumber,
            request.NextNumber,
            request.PaddingLength,
            request.IncrementBy,
            request.ResetPolicy,
            suffix,
            request.IsActive);

        sequence.SetAudit(currentUserId);
        _dbContext.NumberSequences.Add(sequence);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(sequence));
    }

    public async Task<Result<NumberSequenceResponse>> UpdateNumberSequenceAsync(long id, UpdateNumberSequenceRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var sequence = await _dbContext.NumberSequences.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (sequence is null)
        {
            return Result.Failure<NumberSequenceResponse>("Number sequence not found.");
        }

        var module = request.Module.Trim();
        var documentType = request.DocumentType.Trim();

        if (await ExistsAsync(module, documentType, id, cancellationToken))
        {
            return Result.Failure<NumberSequenceResponse>("A number sequence for this module and document type already exists.");
        }

        sequence.Update(
            module,
            documentType,
            request.Prefix.Trim().ToUpperInvariant(),
            request.StartingNumber,
            request.NextNumber,
            request.PaddingLength,
            request.IncrementBy,
            request.ResetPolicy,
            request.Suffix?.Trim().ToUpperInvariant(),
            request.IsActive);

        sequence.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(sequence));
    }

    public async Task<Result> DeleteNumberSequenceAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var sequence = await _dbContext.NumberSequences.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (sequence is null)
        {
            return Result.Failure("Number sequence not found.");
        }

        sequence.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateNumberSequenceAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var sequence = await _dbContext.NumberSequences.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (sequence is null)
        {
            return Result.Failure("Number sequence not found.");
        }

        sequence.Activate();
        sequence.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateNumberSequenceAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var sequence = await _dbContext.NumberSequences.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (sequence is null)
        {
            return Result.Failure("Number sequence not found.");
        }

        sequence.Deactivate();
        sequence.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<string>> GetPreviewAsync(long id, CancellationToken cancellationToken = default)
    {
        var sequence = await _dbContext.NumberSequences
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return sequence is null
            ? Result.Failure<string>("Number sequence not found.")
            : Result.Success(sequence.Preview(DateTime.UtcNow));
    }

    public async Task<Result<string>> GetNextNumberAsync(string module, string documentType, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedModule = module.Trim().ToLowerInvariant();
        var normalizedDocumentType = documentType.Trim().ToLowerInvariant();

        for (var attempt = 0; attempt < MaxRetryAttempts; attempt++)
        {
            var sequence = await _dbContext.NumberSequences.FirstOrDefaultAsync(
                x => x.IsActive &&
                    x.Module.ToLower() == normalizedModule &&
                    x.DocumentType.ToLower() == normalizedDocumentType,
                cancellationToken);

            if (sequence is null)
            {
                return Result.Failure<string>("No active number sequence configured for this module and document type.");
            }

            var nextNumber = sequence.ConsumeNext(DateTime.UtcNow);
            sequence.SetAudit(currentUserId);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                return Result.Success(nextNumber);
            }
            catch (DbUpdateConcurrencyException) when (attempt < MaxRetryAttempts - 1)
            {
                _dbContext.ChangeTracker.Clear();
            }
            catch (DbUpdateConcurrencyException)
            {
                _dbContext.ChangeTracker.Clear();
                return Result.Failure<string>("Unable to allocate the next number because the sequence was updated by another request. Please try again.");
            }
        }

        return Result.Failure<string>("Unable to allocate the next number because the sequence was updated by another request. Please try again.");
    }

    private async Task<bool> ExistsAsync(string module, string documentType, long? excludeId, CancellationToken cancellationToken)
    {
        var normalizedModule = module.ToLowerInvariant();
        var normalizedDocumentType = documentType.ToLowerInvariant();

        return await _dbContext.NumberSequences.AnyAsync(
            x => x.Id != excludeId &&
                x.Module.ToLower() == normalizedModule &&
                x.DocumentType.ToLower() == normalizedDocumentType,
            cancellationToken);
    }

    private static NumberSequenceResponse MapToResponse(NumberSequence sequence) => new(
        sequence.Id,
        sequence.Module,
        sequence.DocumentType,
        sequence.Prefix,
        sequence.Suffix,
        sequence.StartingNumber,
        sequence.NextNumber,
        sequence.PaddingLength,
        sequence.IncrementBy,
        sequence.ResetPolicy,
        sequence.ResetPolicy.ToString(),
        sequence.IsActive,
        sequence.Preview(DateTime.UtcNow),
        sequence.CreatedAt,
        sequence.CreatedBy,
        sequence.UpdatedAt);
}
