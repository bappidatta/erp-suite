using ErpSuite.Modules.Admin.Domain.Entities;

namespace ErpSuite.Modules.Admin.Application.NumberSequences.Dtos;

public sealed record CreateNumberSequenceRequest(
    string Module,
    string DocumentType,
    string Prefix,
    int StartingNumber = 1,
    int NextNumber = 1,
    int PaddingLength = 5,
    int IncrementBy = 1,
    string? Suffix = null,
    NumberSequenceResetPolicy ResetPolicy = NumberSequenceResetPolicy.Manual,
    bool IsActive = true);
