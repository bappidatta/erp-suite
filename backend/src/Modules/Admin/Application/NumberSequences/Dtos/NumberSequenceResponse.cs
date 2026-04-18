using ErpSuite.Modules.Admin.Domain.Entities;

namespace ErpSuite.Modules.Admin.Application.NumberSequences.Dtos;

public sealed record NumberSequenceResponse(
    long Id,
    string Module,
    string DocumentType,
    string Prefix,
    string? Suffix,
    int StartingNumber,
    int NextNumber,
    int PaddingLength,
    int IncrementBy,
    NumberSequenceResetPolicy ResetPolicy,
    string ResetPolicyName,
    bool IsActive,
    string Preview,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
