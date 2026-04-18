using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class NumberSequence : BaseAuditableEntity
{
    public string Module { get; private set; } = string.Empty;
    public string DocumentType { get; private set; } = string.Empty;
    public string Prefix { get; private set; } = string.Empty;
    public string? Suffix { get; private set; }
    public int StartingNumber { get; private set; }
    public int NextNumber { get; private set; }
    public int PaddingLength { get; private set; }
    public int IncrementBy { get; private set; }
    public NumberSequenceResetPolicy ResetPolicy { get; private set; }
    public DateTime? LastResetOn { get; private set; }
    public bool IsActive { get; private set; } = true;

    private NumberSequence() { }

    public static NumberSequence Create(
        string module,
        string documentType,
        string prefix,
        int startingNumber,
        int nextNumber,
        int paddingLength,
        int incrementBy,
        NumberSequenceResetPolicy resetPolicy,
        string? suffix = null,
        bool isActive = true)
    {
        return new NumberSequence
        {
            Module = module,
            DocumentType = documentType,
            Prefix = prefix,
            Suffix = suffix,
            StartingNumber = startingNumber,
            NextNumber = nextNumber,
            PaddingLength = paddingLength,
            IncrementBy = incrementBy,
            ResetPolicy = resetPolicy,
            IsActive = isActive,
            LastResetOn = DateTime.UtcNow.Date
        };
    }

    public void Update(
        string module,
        string documentType,
        string prefix,
        int startingNumber,
        int nextNumber,
        int paddingLength,
        int incrementBy,
        NumberSequenceResetPolicy resetPolicy,
        string? suffix,
        bool isActive)
    {
        Module = module;
        DocumentType = documentType;
        Prefix = prefix;
        Suffix = suffix;
        StartingNumber = startingNumber;
        NextNumber = nextNumber;
        PaddingLength = paddingLength;
        IncrementBy = incrementBy;
        ResetPolicy = resetPolicy;
        IsActive = isActive;
    }

    public string Preview(DateTime asOfUtc)
    {
        return FormatNumber(GetEffectiveNextNumber(asOfUtc));
    }

    public string ConsumeNext(DateTime asOfUtc)
    {
        var effectiveNextNumber = GetEffectiveNextNumber(asOfUtc);
        var current = FormatNumber(effectiveNextNumber);
        NextNumber = effectiveNextNumber + IncrementBy;
        LastResetOn = asOfUtc.Date;
        return current;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private int GetEffectiveNextNumber(DateTime asOfUtc)
    {
        if (ResetPolicy == NumberSequenceResetPolicy.Annual &&
            LastResetOn.HasValue &&
            LastResetOn.Value.Year != asOfUtc.Year)
        {
            return StartingNumber;
        }

        return NextNumber;
    }

    private string FormatNumber(int value)
    {
        var number = value.ToString().PadLeft(PaddingLength, '0');
        return $"{Prefix}{number}{Suffix}";
    }
}
