namespace ErpSuite.Modules.Admin.Application.NumberSequences.Dtos;

public sealed class GetNumberSequencesQuery
{
    public string? SearchTerm { get; init; }
    public string? Module { get; init; }
    public string? DocumentType { get; init; }
    public bool? IsActive { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
