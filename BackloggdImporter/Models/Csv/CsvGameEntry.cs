namespace BackloggdImporter.Models.Csv;

internal record CsvGameEntry
{
    public required string Name { get; init; }
    public int? Rating { get; init; }
    public string? Platform { get; init; }
    public string? Status { get; init; }
    public string? Review { get; init; }
    public int? ReleaseYear { get; init; }
}