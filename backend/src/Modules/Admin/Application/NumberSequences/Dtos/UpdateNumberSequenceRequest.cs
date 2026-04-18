using ErpSuite.Modules.Admin.Domain.Entities;

namespace ErpSuite.Modules.Admin.Application.NumberSequences.Dtos;

public sealed record UpdateNumberSequenceRequest(
    string Module,
    string DocumentType,
    string Prefix,
    int StartingNumber,
    int NextNumber,
    int PaddingLength,
    int IncrementBy,
    string? Suffix = null,
    NumberSequenceResetPolicy ResetPolicy = NumberSequenceResetPolicy.Manual,
    bool IsActive = true);
